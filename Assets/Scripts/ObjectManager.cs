using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    public GameObject enemyLPrefab;
    public GameObject enemyMPrefab;
    public GameObject enemySPrefab;
    public GameObject enemyBPrefab;
    public GameObject itemCoinPrefab;
    public GameObject itemPowerPrefab;
    public GameObject itemBoomPrefab;
    public GameObject bulletPlayerAPrefab;
    public GameObject bulletPlayerBPrefab;
    public GameObject bulletEnemyAPrefab;
    public GameObject bulletEnemyBPrefab;
    public GameObject bulletFollowerPrefab;
    public GameObject bulletBossAPrefab;
    public GameObject bulletBossBPrefab;

    GameObject[] enemyL;
    GameObject[] enemyM;
    GameObject[] enemyS;
    GameObject[] enemyB;

    GameObject[] itemCoin;
    GameObject[] itemPower;
    GameObject[] itemBoom;

    GameObject[] bulletPlayerA;
    GameObject[] bulletPlayerB;
    GameObject[] bulletEnemyA;
    GameObject[] bulletEnemyB;
    GameObject[] bulletFollower;
    GameObject[] bulletBossA;
    //GameObject[] bulletBossB;
    List <GameObject> bulletBossB = new List<GameObject>();

    GameObject[] targetPool;                           // 게임씬에서 활성화하고자 하는 특정 타입의 프리팹만 가져오기 위한 배열 변수

    void Awake()
    {
        enemyL = new GameObject[10];
        enemyM = new GameObject[10];
        enemyS = new GameObject[20];
        enemyB = new GameObject[1];

        itemCoin = new GameObject[20];
        itemPower = new GameObject[10];
        itemBoom = new GameObject[10];

        bulletPlayerA = new GameObject[100];
        bulletPlayerB = new GameObject[100];
        bulletEnemyA = new GameObject[100];
        bulletEnemyB = new GameObject[100];
        bulletFollower = new GameObject[100];
        bulletBossA = new GameObject[100];
        //bulletBossB = new GameObject[100];

        Generate();
    }

    void Generate()
    {
        for (int index = 0; index < enemyL.Length; index++)
        {
            enemyL[index] = Instantiate(enemyLPrefab);                        //Awake 함수에서 지정한 배열의 길이만큼 프리팹을 미리 생성
            enemyL[index].SetActive(false);                                   //생성 후 즉시 비활성화
        }

        for (int index = 0; index < enemyM.Length; index++)
        {
            enemyM[index] = Instantiate(enemyMPrefab);
            enemyM[index].SetActive(false);
        }

        for (int index = 0; index < enemyS.Length; index++)
        {
            enemyS[index] = Instantiate(enemySPrefab);
            enemyS[index].SetActive(false);
        }

        for (int index = 0; index < enemyB.Length; index++)
        {
            enemyB[index] = Instantiate(enemyBPrefab);
            enemyB[index].SetActive(false);
        }

        for (int index = 0; index < itemCoin.Length; index++)
        {
            itemCoin[index] = Instantiate(itemCoinPrefab);
            itemCoin[index].SetActive(false);
        }

        for (int index = 0; index < itemPower.Length; index++)
        {
            itemPower[index] = Instantiate(itemPowerPrefab);
            itemPower[index].SetActive(false);
        }

        for (int index = 0; index < itemBoom.Length; index++)
        {
            itemBoom[index] = Instantiate(itemBoomPrefab);
            itemBoom[index].SetActive(false);
        }

        for (int index = 0; index < bulletPlayerA.Length; index++)
        {
            bulletPlayerA[index] = Instantiate(bulletPlayerAPrefab);
            bulletPlayerA[index].SetActive(false);
        }

        for (int index = 0; index < bulletPlayerB.Length; index++)
        {
            bulletPlayerB[index] = Instantiate(bulletPlayerBPrefab);
            bulletPlayerB[index].SetActive(false);
        }

        for (int index = 0; index < bulletEnemyA.Length; index++)
        {
            bulletEnemyA[index] = Instantiate(bulletEnemyAPrefab);
            bulletEnemyA[index].SetActive(false);
        }

        for (int index = 0; index < bulletEnemyB.Length; index++)
        {
            bulletEnemyB[index] = Instantiate(bulletEnemyBPrefab);
            bulletEnemyB[index].SetActive(false);
        }

        for (int index = 0; index < bulletFollower.Length; index++)
        {
            bulletFollower[index] = Instantiate(bulletFollowerPrefab);
            bulletFollower[index].SetActive(false);
        }

        for (int index = 0; index < bulletBossA.Length; index++)
        {
            bulletBossA[index] = Instantiate(bulletBossAPrefab);
            bulletBossA[index].SetActive(false);
        }

        for (int index = 0; index < 100; index++)
        {
            bulletBossB.Add(Instantiate(bulletBossBPrefab));
            bulletBossB[index].SetActive(false);
        }
    }

    public GameObject MakeObj(string type)              // 오브젝트 풀에 접근하는 함수 생성
    {

        switch (type)                                   // 임의로 지정한 프리팹 타입에 따라 실행할 switch문
        {
            case "EnemyL":
                targetPool = enemyL;
                break;

            case "EnemyM":
                targetPool = enemyM;
                break;

            case "EnemyS":
                targetPool = enemyS;
                break;

            case "EnemyB":
                targetPool = enemyB;
                break;

            case "itemCoin":
                targetPool = itemCoin;
                break;

            case "itemPower":
                targetPool = itemPower;
                break;

            case "itemBoom":
                targetPool = itemBoom;
                break;

            case "bulletPlayerA":
                targetPool = bulletPlayerA;
                break;

            case "bulletPlayerB":
                targetPool = bulletPlayerB;
                break;

            case "bulletEnemyA":
                targetPool = bulletEnemyA;
                break;

            case "bulletEnemyB":
                targetPool = bulletEnemyB;
                break;

            case "bulletFollower":
                targetPool = bulletFollower;
                break;

            case "bulletBossA":
                targetPool = bulletBossA;
                break;

            case "bulletBossB":
                targetPool = bulletBossB.ToArray();
                break;
            }

        for (int index = 0; index < targetPool.Length; index++)
        {
            if (!targetPool[index].activeSelf)              // targetPool 배열 내 오브젝트가 비활성화 상태인 경우 실행
            {
                targetPool[index].SetActive(true);          // targetPool 배열 내 오브젝트를 활성화하고
                return targetPool[index];                   // 반환
            }
        }

        return null;                                        // 해당하는 타입이 없는 경우 null 반환
    }

    public GameObject MakeObjOneMore(string bulletName)           // 게임오브젝트가 더 필요할 때 추가 생산
    {
        if(bulletName == "bulletBossB")
        {
            GameObject bullet = Instantiate(bulletBossBPrefab);
            bulletBossB.Add(bullet);
            bullet.SetActive(true);

            return bulletBossB[bulletBossB.Count - 1];
        }

        return null;
    }

    public GameObject[] GetPool(string type)                // 지정한 오브젝트 풀을 가져오는 함수
    {
        switch (type)                                       // 임의로 지정한 프리팹 타입에 따라 실행할 switch문
        {
            case "EnemyL":
                targetPool = enemyL;
                break;

            case "EnemyM":
                targetPool = enemyM;
                break;

            case "EnemyS":
                targetPool = enemyS;
                break;

            case "EnemyB":
                targetPool = enemyB;
                break;

            case "itemCoin":
                targetPool = itemCoin;
                break;

            case "itemPower":
                targetPool = itemPower;
                break;

            case "itemBoom":
                targetPool = itemBoom;
                break;

            case "bulletPlayerA":
                targetPool = bulletPlayerA;
                break;

            case "bulletPlayerB":
                targetPool = bulletPlayerB;
                break;

            case "bulletEnemyA":
                targetPool = bulletEnemyA;
                break;

            case "bulletEnemyB":
                targetPool = bulletEnemyB;
                break;

            case "bulletFollower":
                targetPool = bulletFollower;
                break;

            case "bulletBossA":
                targetPool = bulletBossA;
                break;

            case "bulletBossB":
                targetPool = bulletBossB.ToArray();
                break;
        }

        return targetPool;
    }
}
