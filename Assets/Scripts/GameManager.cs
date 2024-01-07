using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class GameManager : Singleton<GameManager>
{
    //public GameObject[] enemyObjects;       // Instantiate �Լ��� ����Ͽ� ���� ������ �� ���� ������Ʈ�� ���� ����
    public string[] enemyObjs;                // Ǯ������ ���� ������ �� �޾ƿ� ���� Ÿ���� ������ �迭 ����
    public Transform[] spawnPoints;
    public float nextSpawnDelay;
    public float curSpawnDelay;

    public GameObject player;                 // ���� �Ѿ��� �÷��̾�� ��� �ϱ� ���� �÷��̾ ����
    public Text scoreText;
    public Image[] lifeImage;
    public Image[] boomImage;
    public GameObject gameOverSet;

    public ObjectManager objectManager;       // ObjectManager Ŭ������ �������� ���� ����

    public List<Spawn> spawnList;
    public int spawnIndex;
    public bool spawnEnd;


    public override void Awake()
    {
        Debug.Log("GameManager Awake ����");
        base.Awake();                                                                   // Singleton Ŭ������ Awake ����
        enemyObjs = new string[] { "EnemyS", "EnemyM", "EnemyL", "EnemyB" };            // enemyObjs�� �迭�� ObjectManager���� ������ ���� Ÿ�� 4���� �ʱ�ȭ, MakeObj�Լ��� �Ű� ������ Ÿ���� �޾ƿ;� ��
        spawnList = new List<Spawn>();
        ReadSpawnFile();
    }

    void ReadSpawnFile()
    {
        spawnList.Clear();
        spawnIndex = 0;
        spawnEnd = false;

        TextAsset textFile = Resources.Load("Stage 0") as TextAsset;                  // Resources ������ �ִ� Stage0 �ؽ�Ʈ ������ �����Ͽ� ��������
        StringReader stringReader = new StringReader(textFile.text);                  // textFile�� ������ ������ �ؽ�Ʈ�� �б�


        while(stringReader != null)                                                   // �о�� �ؽ�Ʈ ������ ������ ����
        {
            string line = stringReader.ReadLine();                                    // �ؽ�Ʈ �����͸� �� �پ� ��ȯ
            //Debug.Log(line);

            if (line == null)                                                         // �� �̻� �о�� ��(������ �� ����)�� ������ ���� �ߴ�
            {
                break;
            }

            Spawn spawnData = new Spawn();
            spawnData.delay = float.Parse(line.Split(',')[0]);                        // �޸��� ���е� ���ڿ� �߿��� ù��° ���� ��������, float������ �Ľ�
            spawnData.type = line.Split(',')[1];
            spawnData.point = int.Parse(line.Split(',')[2]);
            spawnList.Add(spawnData);
        }

        stringReader.Close();                                                         // �ؽ�Ʈ ���� �ݱ�
        nextSpawnDelay = spawnList[0].delay
;   }

    void Update()
    {
        curSpawnDelay += Time.deltaTime;

        if(curSpawnDelay > nextSpawnDelay && !spawnEnd)             // spawnEnd�� ��ȯ ���ᰡ �Ǿ������� �Ǵ��ϱ�
        {
            SpawnEnemy();
            //nextSpawnDelay = Random.Range(0.5f, 3.0f);            // ��ȯ �ֱ⸦ 0.5�ʿ��� 3�� ���� �������� ����
            curSpawnDelay = 0;
        }

        Player playerLogic = player.GetComponent<Player>();
        scoreText.text = string.Format("{0:n0}", playerLogic.score);   // ���ھ ���ڿ��� ��ȯ�Ͽ� ���ڸ� ������ �޸� �߰�
    }
    
    void SpawnEnemy()
    {
        //int ranEnemy = Random.Range(0, 3);       //������ �� (Instantiate �Լ� ����� ���� enemyObjects�� ������ ���� �������� �������� ���Ҷ� ����ϰ�, ������Ʈ Ǯ�� ����� ���� enemyObjs �迭�� �� Ÿ�� ��Ʈ���� �������� ���� �� ���)
        //int ranPoint = Random.Range(0, 9);       //������ ���� ��ġ

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

        //Instantiate ��� �̸� ������ ������Ʈ Ǯ���� �������� ���� �ڵ� ���� (�Ʒ� 2��)
        //GameObject enemy = objectManager.MakeObj(enemyObjs[ranEnemy]);                                    // �Ʒ� �ڵ��� ���� �ڵ�
        GameObject enemy = objectManager.MakeObj(enemyObjs[enemyIndex]);

        enemy.transform.position = spawnPoints[enemyPoint].position;
                       
        // Instantiate �Լ��� �� ������ ����ϴ� �ڵ� (�Ʒ� 1��)
        //GameObject enemy = Instantiate(enemyObjects[ranEnemy], spawnPoints[ranPoint].position, spawnPoints[ranPoint].rotation);

        Rigidbody2D rigid = enemy.GetComponent<Rigidbody2D>();
        Enemy enemyLogic = enemy.GetComponent<Enemy>();
        enemyLogic.player = player;                        // enemy�� �����Ǹ� �÷��̾� ������ ����
        enemyLogic.objectManager = objectManager;          // enemy�� �����Ǹ� ������Ʈ �Ŵ��� ��ũ��Ʈ�� ����

        if(enemyPoint == 5 || enemyPoint == 6)
        {
            rigid.velocity = new Vector2(-1 * enemyLogic.speed, -1);   // ���� ���� �ӵ���ŭ x�� ���� �������ε� �̵�
            enemy.transform.Rotate(Vector3.back * 90);                 // z�� ���� 90���� ȸ���� ���·� �̵�
        }
        else if(enemyPoint == 7 || enemyPoint == 8)
        {
            rigid.velocity = new Vector2(enemyLogic.speed, -1);
            enemy.transform.Rotate(Vector3.forward * 90);     // z�� ���� 90���� ȸ��   
        }
        else
        {
            rigid.velocity = new Vector2(0, enemyLogic.speed * -1);
        }

        spawnIndex++;                                           // ���� ��ȯ �� �ε��� �÷��� ���� �� �о� ����
        if(spawnIndex == spawnList.Count)                       // ����Ʈ�� �������� ������ ��� spawnEnd�� true�� ��ȯ
        {
            spawnEnd = true;
            return;
        }

        nextSpawnDelay = spawnList[spawnIndex].delay;           // ���� ���� ������ ���� �ֱ� ��������
    }

    public void UpdateLifeIcon(int life)
    {
        for(int index = 0; index < 3; index++)
        {
            lifeImage[index].color = new Color(1, 1, 1, 0);     // ��� ü�� �̹����� �����ϰ� �����
        }

        for (int index = 0; index < life; index++)
        {
            lifeImage[index].color = new Color(1, 1, 1, 1);     // ���� ü�� �̹��� ������ŭ�� ǥ��
        }
    }

    public void UpdateBoomIcon(int boom)
    {
        for (int index = 0; index < 3; index++)
        {
            boomImage[index].color = new Color(1, 1, 1, 0);     // ��� �ʻ�� ������ �̹����� �����ϰ� �����
        }

        for (int index = 0; index < boom; index++)
        {
            boomImage[index].color = new Color(1, 1, 1, 1);     // ���� �ʻ�� ������ �̹��� ������ŭ�� ǥ��
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
