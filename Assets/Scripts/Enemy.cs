using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public string enemyName;
    public int enemyScore;
    public float speed;
    public int health;
    public Sprite[] sprites;

    SpriteRenderer spriteRenderer;
    Animator anim;

    public GameObject bulletObjA;
    public GameObject bulletObjB;
    public GameObject itemCoin;
    public GameObject itemPower;
    public GameObject itemBoom;
    public GameObject player;
    public ObjectManager objectManager;

    public float maxShotDelay;         //발사에 필요한 쿨타임
    public float curShotDelay;         //마지막 발사로부터 현재까지의 시간

    public int patternIndex;           //보스 패턴 인덱스
    public int curPatternCount;        //각 패턴별 실행 횟수, 패턴 인덱스가 바뀔 때마다 0으로 초기화
    public int[] maxPatternCount;      //각 패턴별 최대 실행 횟수

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (enemyName == "B")
        {
            anim = GetComponent<Animator>();
        }

    }

    void OnEnable()
    {
        switch(enemyName)         // 적이 다시 활성화 될 때마다 최대 체력으로 초기화하여 활성화
        {
            case "L":
                health = 50;
                break;

            case "M":
                health = 15;
                break;

            case "S":
                health = 3;
                break;

            case "B":
                health = 3000;
                Invoke("Stop", 2);
                break;
        }
    }

     void Stop()
     {
        if(!gameObject.activeSelf)
        {
            return;
        }
        else
        {
            Rigidbody2D rigid = GetComponent<Rigidbody2D>();
            rigid.velocity = Vector2.zero;
        }

        Invoke("Think", 2);
     }

    void Think()
    {
        patternIndex = patternIndex == 3 ? 0 : patternIndex + 1;
        curPatternCount = 0;

        switch(patternIndex)
        {
            case 0:
                FireForward();
                break;


            case 1:
                FireShot();
                break;


            case 2:
                FireArc();
                break;


            case 3:
                FireAround();
                break;
        }
    }

    void FireForward()          // 4발 씩 아래 방향으로 총알 발사
    {
        GameObject bulletR = objectManager.MakeObj("bulletBossA");
        bulletR.transform.position = transform.position + Vector3.right * 0.3f;
        GameObject bulletRR = objectManager.MakeObj("bulletBossA");
        bulletRR.transform.position = transform.position + Vector3.right * 0.45f;
        GameObject bulletL = objectManager.MakeObj("bulletBossA");
        bulletL.transform.position = transform.position + Vector3.left * 0.3f;
        GameObject bulletLL = objectManager.MakeObj("bulletBossA");
        bulletLL.transform.position = transform.position + Vector3.left * 0.45f;

        Rigidbody2D rigidR = bulletR.GetComponent<Rigidbody2D>();                                                           // 생성한 bullet의 rigidbody2D 정의
        Rigidbody2D rigidRR = bulletRR.GetComponent<Rigidbody2D>();
        Rigidbody2D rigidL = bulletL.GetComponent<Rigidbody2D>();                                                           // 생성한 bullet의 rigidbody2D 정의
        Rigidbody2D rigidLL = bulletLL.GetComponent<Rigidbody2D>();
        
        rigidR.AddForce(Vector2.down * 8, ForceMode2D.Impulse);                                                      
        rigidRR.AddForce(Vector2.down * 8, ForceMode2D.Impulse);
        rigidL.AddForce(Vector2.down * 8, ForceMode2D.Impulse);                                                      
        rigidLL.AddForce(Vector2.down * 8, ForceMode2D.Impulse);

        curPatternCount++;

        if(curPatternCount < maxPatternCount[patternIndex])   // 패턴 실행 최대 횟수에 아직 도달하지 않은 경우 한번 더 실행
        {
            Invoke("FireForward", 2);
        }
        else
        {
            Invoke("Think", 2);
        }
    }

    void FireShot()
    {
        for(int index=0; index <5; index++)
        {
            GameObject bullet = objectManager.MakeObj("bulletEnemyB");
            bullet.transform.position = transform.position;

            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();                                     // 생성한 bullet의 rigidbody2D 정의
            Vector2 dirVec = player.transform.position - transform.position;                            // 목표물의 방향 = 목표물 위치 - 자신의 위치, 벡터의 차이로 벡터 방향을 판단 (line 152에서 normalized로 표준화)
            Vector2 ranVec = new Vector2(Random.Range(-0.5f, 0.5f), Random.Range(0f,2f));               // 목표물의 방향에서 모든 총알이 한 방향으로만 나가는 것을 보정하기 위한 랜덤값 추가
            dirVec += ranVec;                                                                           // ranVec에서 정해진 랜덤값을 추가하여 발사
            rigid.AddForce(dirVec.normalized * 3, ForceMode2D.Impulse);                                 // 플레이어 방향으로 발사
        }
   
        curPatternCount++;

        if (curPatternCount < maxPatternCount[patternIndex])   // 패턴 실행 최대 횟수에 아직 도달하지 않은 경우 한번 더 실행
        {
            Invoke("FireShot", 2);
        }
        else
        {
            Invoke("Think", 2);
        }
    }

    void FireArc()       //MaxPatternCount만큼 총알을 원형으로 차례대로 발사
    {
        GameObject bullet = objectManager.MakeObj("bulletEnemyA");
        bullet.transform.position = transform.position;
        bullet.transform.rotation = Quaternion.identity;

        Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();                                                                             // 생성한 bullet의 rigidbody2D 정의
        Vector2 dirVec = new Vector2(Mathf.Cos(Mathf.PI * 10 * curPatternCount/maxPatternCount[patternIndex]), -1);                         // 원형으로 차례대로 발사
        rigid.AddForce(dirVec.normalized * 5, ForceMode2D.Impulse);                                                                         // 플레이어 방향으로 발사
        

        curPatternCount++;

        if (curPatternCount < maxPatternCount[patternIndex])   // 패턴 실행 최대 횟수에 아직 도달하지 않은 경우 한번 더 실행
        {
            Invoke("FireArc", 0.15f);
        }
        else
        {
            Invoke("Think", 2);
        }
    }

    void FireAround()      //MaxPatternCount만큼 총알을 원형으로 일제 발사
    {
        int roundNumA = 50;
        int roundNumB = 40;
        int roundNum = curPatternCount % 2 == 0 ? roundNumA : roundNumB;                                                                 // curPatternCount가 짝수면 50발, 홀수면40발 일제 사격

        for (int index = 0; index < roundNum; index++)
        {
            GameObject bullet;
            bullet = objectManager.MakeObj("bulletBossB");

            if(bullet == null)
            {
                bullet = objectManager.MakeObjOneMore("bulletBossB");
            }

            bullet.transform.position = transform.position;
            bullet.transform.rotation = Quaternion.identity;

            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();                                                                      // 생성한 bullet의 rigidbody2D 정의
            Vector2 dirVec = new Vector2(Mathf.Cos(Mathf.PI * 2 * index / roundNum), Mathf.Sin(Mathf.PI * 2 * index / roundNum));        // 원형으로 일제히 발사, index로 정해지는 총알 발사 순서에 따라 각도가 정해짐
            rigid.AddForce(dirVec.normalized * 2, ForceMode2D.Impulse);                                                                  // 플레이어 방향으로 발사


            
            Vector3 rotVec = Vector3.forward * 360 * index / roundNum + Vector3.forward * 90;                                            // 총알이 퍼지는 방향으로 총알을 회전
            bullet.transform.Rotate(rotVec); 
        }
        
        curPatternCount++;

        if (curPatternCount < maxPatternCount[patternIndex])   // 패턴 실행 최대 횟수에 아직 도달하지 않은 경우 한번 더 실행
        {
            Invoke("FireAround", 0.7f);
        }
        else
        {
            Invoke("Think", 2);
        }
    }

    void Update()
    {
        if(enemyName == "B")                // 보스는 일반 공격은 하지 않음
        {
            return;
        }

        Fire();
        Reload();
    }

    void Fire()
    {
        if (curShotDelay < maxShotDelay)   // 장전에 필요한 쿨타임이 지나지 않았으면 다시 쏠 수 없음
        {
            return;
        }

        if(enemyName == "S")
        {
            //GameObject bullet = Instantiate(bulletObjA, transform.position, transform.rotation);        // 생성할 bullet 게임 오브젝트 정의, bulletObjA를 플레이어 위치에서 회전 그대로 생성
            GameObject bullet = objectManager.MakeObj("bulletEnemyA");
            bullet.transform.position = transform.position;

            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();                                     // 생성한 bullet의 rigidbody2D 정의

            Vector3 dirVec = player.transform.position - transform.position;                            // 목표물의 방향 = 목표물 위치 - 자신의 위치
            rigid.AddForce(dirVec.normalized * 3, ForceMode2D.Impulse);                                 // 플레이어 방향으로 발사
        }
        else if(enemyName == "L")
        {
            //GameObject bulletR = Instantiate(bulletObjB, transform.position + Vector3.right * 0.3f, transform.rotation);        // 생성할 bullet 게임 오브젝트 정의, bulletObjB를 플레이어 위치에서 회전 그대로 생성
            //GameObject bulletL = Instantiate(bulletObjB, transform.position + Vector3.left * 0.3f, transform.rotation);         // 생성할 bullet 게임 오브젝트 정의, bulletObjB를 플레이어 위치에서 회전 그대로 생성
            GameObject bulletR = objectManager.MakeObj("bulletEnemyB");
            bulletR.transform.position = transform.position + Vector3.right * 0.3f;
            GameObject bulletL = objectManager.MakeObj("bulletEnemyB");
            bulletL.transform.position = transform.position + Vector3.left * 0.3f;

            Rigidbody2D rigidR = bulletR.GetComponent<Rigidbody2D>();                                                           // 생성한 bullet의 rigidbody2D 정의
            Rigidbody2D rigidL = bulletL.GetComponent<Rigidbody2D>();                                                           // 생성한 bullet의 rigidbody2D 정의

            Vector3 dirVecR = player.transform.position - (transform.position + Vector3.right * 0.3f);                          // 목표물의 방향 = 목표물 위치 - 자신의 위치
            Vector3 dirVecL = player.transform.position - (transform.position + Vector3.left * 0.3f);                           // 목표물의 방향 = 목표물 위치 - 자신의 위치

            rigidR.AddForce(dirVecR.normalized * 4, ForceMode2D.Impulse);                                                      // 플레이어 방향을 단위화 한 후 플레이어 방향으로 발사
            rigidL.AddForce(dirVecL.normalized * 4, ForceMode2D.Impulse);                                                      // 플레이어 방향을 단위화 한 후 플레이어 방향으로 발사
        }

        curShotDelay = 0;                  // 마지막 발사로부터 지난 시간 0으로 초기화   (매우 중요)
    }

    void Reload()
    {
        curShotDelay += Time.deltaTime;
    }

    public void OnHit(int dmg)  // 총알의 대미지를 매개 변수로 사용
    {
        if (health <= 0)         // 이미 적의 체력이 0 이하일 때 두 번 이상 피격되지 않음
        {
            return;
        }
        else
        {
            health -= dmg;

            if(enemyName == "B")
            {
                anim.SetTrigger("OnHit");
            }
            else
            {
                spriteRenderer.sprite = sprites[1];     // 피격 시 1번 스프라이트로 변경
                Invoke("ReturnSprite", 0.1f);
            }
        }


        if(health <= 0)
        {
            Player playerLogic = player.GetComponent<Player>();      // Player 스크립트 가져오기
            playerLogic.score += enemyScore;                         // Player 스크립트 안에 있는 score 값을 더함


            //int ran = enemyName == "B" ? 0 : Random.Range(0, 10);    // 보스면 아이템 드랍하지 않음

            int ran;

            if (enemyName == "B")
            {
                ran = 0;
            }
            else
            {
                ran = Random.Range(0, 10);
            }
            

            if(ran <1)
            {
                Debug.Log("No Dropped Items");
            }
            else if (ran < 3)
            {
                //Instantiate(itemCoin, transform.position, itemCoin.transform.rotation);             // 적이 죽은 자리에서 아이템 드랍
                GameObject itemCoin = objectManager.MakeObj("itemCoin");
                itemCoin.transform.position = transform.position;

            }
            else if (ran < 5)
            {
                //Instantiate(itemPower, transform.position, itemPower.transform.rotation);
                GameObject itemPower = objectManager.MakeObj("itemPower");
                itemPower.transform.position = transform.position;

            }
            else if (ran < 7)
            {
                //Instantiate(itemBoom, transform.position, itemBoom.transform.rotation);
                GameObject itemBoom = objectManager.MakeObj("itemBoom");
                itemBoom.transform.position = transform.position;

            }

            //Destroy(gameObject);                          // Instantiate 함수로 적을 생성할때만 사용
            gameObject.SetActive(false);                    // 오브젝트 풀에서 사용할 때는 비활성화를 사용
            transform.rotation = Quaternion.identity;       // 적 오브젝트 기본 회전값을 0으로 설정
        }
    }

    void ReturnSprite()
    {
        spriteRenderer.sprite = sprites[0];     // 원래 스프라이트로 변경
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "BorderBullet"  && enemyName != "B")
        {
            //Destroy(gameObject);                                            // Instantiate 함수로 적을 생성할때만 사용
            gameObject.SetActive(false);                                      // 오브젝트 풀에서 사용할 때는 비활성화를 사용
            transform.rotation = Quaternion.identity;
        }
        else if (collision.gameObject.tag == "PlayerBullet")
        {
            Bullet bullet = collision.gameObject.GetComponent<Bullet>();      // Bullet 스크립트를 가져오기
            OnHit(bullet.dmg);

            //Destroy(collision.gameObject);                                  // 적에 닿으면 총알 없애기, Instantiate 함수로 적을 생성할때만 사용
            collision.gameObject.SetActive(false);                            // 오브젝트 풀에서 사용할 때는 비활성화를 사용
        }
    }

}
