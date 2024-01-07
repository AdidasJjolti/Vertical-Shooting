using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class GameManager : Singleton<GameManager>
{
    //public GameObject[] enemyObjects;       // Instantiate 함수를 사용하여 적을 생성할 때 게임 오브젝트를 받을 변수
    public string[] enemyObjs;                // 풀링으로 적을 생성할 때 받아올 적의 타입을 저장할 배열 변수
    public Transform[] spawnPoints;
    public float nextSpawnDelay;
    public float curSpawnDelay;

    public GameObject player;                 // 적이 총알을 플레이어에게 쏘게 하기 위해 플레이어를 지정
    public Text scoreText;
    public Image[] lifeImage;
    public Image[] boomImage;
    public GameObject gameOverSet;

    public ObjectManager objectManager;       // ObjectManager 클래스를 가져오기 위한 변수

    public List<Spawn> spawnList;
    public int spawnIndex;
    public bool spawnEnd;


    public override void Awake()
    {
        Debug.Log("GameManager Awake 실행");
        base.Awake();                                                                   // Singleton 클래스의 Awake 실행
        enemyObjs = new string[] { "EnemyS", "EnemyM", "EnemyL", "EnemyB" };            // enemyObjs의 배열을 ObjectManager에서 지정한 적의 타입 4개로 초기화, MakeObj함수의 매개 변수로 타입을 받아와야 함
        spawnList = new List<Spawn>();
        ReadSpawnFile();
    }

    void ReadSpawnFile()
    {
        spawnList.Clear();
        spawnIndex = 0;
        spawnEnd = false;

        TextAsset textFile = Resources.Load("Stage 0") as TextAsset;                  // Resources 폴더에 있는 Stage0 텍스트 파일을 검증하여 가져오기
        StringReader stringReader = new StringReader(textFile.text);                  // textFile로 지정한 파일의 텍스트를 읽기


        while(stringReader != null)                                                   // 읽어올 텍스트 파일이 있으면 실행
        {
            string line = stringReader.ReadLine();                                    // 텍스트 데이터를 한 줄씩 반환
            //Debug.Log(line);

            if (line == null)                                                         // 더 이상 읽어올 줄(마지막 줄 도달)이 없으면 실행 중단
            {
                break;
            }

            Spawn spawnData = new Spawn();
            spawnData.delay = float.Parse(line.Split(',')[0]);                        // 콤마로 구분된 문자열 중에서 첫번째 열을 가져오기, float형으로 파싱
            spawnData.type = line.Split(',')[1];
            spawnData.point = int.Parse(line.Split(',')[2]);
            spawnList.Add(spawnData);
        }

        stringReader.Close();                                                         // 텍스트 파일 닫기
        nextSpawnDelay = spawnList[0].delay
;   }

    void Update()
    {
        curSpawnDelay += Time.deltaTime;

        if(curSpawnDelay > nextSpawnDelay && !spawnEnd)             // spawnEnd로 소환 종료가 되었는지도 판단하기
        {
            SpawnEnemy();
            //nextSpawnDelay = Random.Range(0.5f, 3.0f);            // 소환 주기를 0.5초에서 3초 사이 랜덤으로 지정
            curSpawnDelay = 0;
        }

        Player playerLogic = player.GetComponent<Player>();
        scoreText.text = string.Format("{0:n0}", playerLogic.score);   // 스코어를 문자열로 반환하여 세자리 수마다 콤마 추가
    }
    
    void SpawnEnemy()
    {
        //int ranEnemy = Random.Range(0, 3);       //임의의 적 (Instantiate 함수 사용할 때는 enemyObjects로 지정한 적의 프리팹을 랜덤으로 정할때 사용하고, 오브젝트 풀을 사용할 때는 enemyObjs 배열의 적 타입 스트링을 랜덤으로 정할 때 사용)
        //int ranPoint = Random.Range(0, 9);       //임의의 스폰 위치

        int enemyIndex = 0;

        switch(spawnList[enemyIndex].type)
        {
            case "S":
                enemyIndex = 0;
                break;
            case "M":
                enemyIndex = 1;
                break;
            case "L":
                enemyIndex = 2;
                break;
            case "B":
                enemyIndex = 3;
                break;
        }

        int enemyPoint = spawnList[spawnIndex].point;

        //Instantiate 대신 미리 만들어둔 오브젝트 풀에서 가져오기 위해 코드 수정 (아래 2줄)
        //GameObject enemy = objectManager.MakeObj(enemyObjs[ranEnemy]);                                    // 아래 코드의 원래 코드
        GameObject enemy = objectManager.MakeObj(enemyObjs[enemyIndex]);

        enemy.transform.position = spawnPoints[enemyPoint].position;
                       
        // Instantiate 함수로 적 생성시 사용하는 코드 (아래 1줄)
        //GameObject enemy = Instantiate(enemyObjects[ranEnemy], spawnPoints[ranPoint].position, spawnPoints[ranPoint].rotation);

        Rigidbody2D rigid = enemy.GetComponent<Rigidbody2D>();
        Enemy enemyLogic = enemy.GetComponent<Enemy>();
        enemyLogic.player = player;                        // enemy가 생성되면 플레이어 정보를 전달
        enemyLogic.objectManager = objectManager;          // enemy가 생성되면 오브젝트 매니저 스크립트를 전달

        if(enemyPoint == 5 || enemyPoint == 6)
        {
            rigid.velocity = new Vector2(-1 * enemyLogic.speed, -1);   // 적의 비행 속도만큼 x축 기준 좌측으로도 이동
            enemy.transform.Rotate(Vector3.back * 90);                 // z축 기준 90도로 회전한 상태로 이동
        }
        else if(enemyPoint == 7 || enemyPoint == 8)
        {
            rigid.velocity = new Vector2(enemyLogic.speed, -1);
            enemy.transform.Rotate(Vector3.forward * 90);     // z축 기준 90도로 회전   
        }
        else
        {
            rigid.velocity = new Vector2(0, enemyLogic.speed * -1);
        }

        spawnIndex++;                                           // 몬스터 소환 후 인덱스 올려서 다음 줄 읽어 오기
        if(spawnIndex == spawnList.Count)                       // 리스트의 마지막에 도달한 경우 spawnEnd를 true로 변환
        {
            spawnEnd = true;
            return;
        }

        nextSpawnDelay = spawnList[spawnIndex].delay;           // 다음 적이 등장할 생성 주기 가져오기
    }

    public void UpdateLifeIcon(int life)
    {
        for(int index = 0; index < 3; index++)
        {
            lifeImage[index].color = new Color(1, 1, 1, 0);     // 모든 체력 이미지를 투명하게 만들고
        }

        for (int index = 0; index < life; index++)
        {
            lifeImage[index].color = new Color(1, 1, 1, 1);     // 남은 체력 이미지 갯수만큼만 표시
        }
    }

    public void UpdateBoomIcon(int boom)
    {
        for (int index = 0; index < 3; index++)
        {
            boomImage[index].color = new Color(1, 1, 1, 0);     // 모든 필살기 아이템 이미지를 투명하게 만들고
        }

        for (int index = 0; index < boom; index++)
        {
            boomImage[index].color = new Color(1, 1, 1, 1);     // 남은 필살기 아이템 이미지 갯수만큼만 표시
        }
    }

    public void RespawnPlayer()
    {
        Invoke("RespawnPlayerExe", 2f);
    }

    void RespawnPlayerExe()
    {
        player.transform.position = Vector3.down * 4;
        player.SetActive(true);

        Player playerLogic = player.GetComponent<Player>();
        playerLogic.isHit = false;
    }

    public void GameOver()
    {
        gameOverSet.SetActive(true);
    }

    public void GameRetry()
    {
        SceneManager.LoadScene(0);
    }
}
