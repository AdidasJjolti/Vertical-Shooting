using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int life;
    public int score;
    public float speed;
    public int power;                //플레이어 파워를 설정
    public int maxPower;
    public int boom;                
    public int maxBoom;
    public float maxShotDelay;         //발사에 필요한 쿨타임
    public float curShotDelay;         //마지막 발사로부터 현재까지의 시간
    public bool isTouchTop;
    public bool isTouchBottom;
    public bool isTouchRight;
    public bool isTouchLeft;

    public GameObject bulletObjA;
    public GameObject bulletObjB;
    public GameObject boomEffect;

    public GameManager gameManager;
    public ObjectManager objectManager;

    public bool isHit;
    public bool isBoomTime;

    public GameObject[] followers;

    Animator anim;

    #region constructor
    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        Move();
        Fire();
        Boom();
        Reload();
    }
    #endregion

    void Move()
    {
        float h = Input.GetAxisRaw("Horizontal");

        if ((h == 1 && isTouchRight) || (h == -1 && isTouchLeft))       // 오른쪽으로 이동하는데 오른쪽 벽에 닿거나 왼쪽으로 이동하는데 왼쪽 벽에 닿은 경우 더 이상 좌우 이동 값을 받지 않음
        {
            h = 0;
        }

        float v = Input.GetAxisRaw("Vertical");

        if ((v == 1 && isTouchTop) || (v == -1 && isTouchBottom))    // 위쪽으로 이동하는데 위쪽 벽에 닿거나 아래쪽으로 이동하는데 아래 벽에 닿은 경우 더 이상 상하 이동 값을 받지 않음
        {
            v = 0;
        }

        Vector3 curPos = transform.position;
        Vector3 nextPos = new Vector3(h, v, 0) * speed * Time.deltaTime;  // 속도만큼 다른 위치로 이동

        transform.position = curPos + nextPos;

        if (Input.GetButtonDown("Horizontal") || Input.GetButtonUp("Horizontal"))     // 횡이동 이동 버튼을 누르고 있거나 뗀 경우 실행,    -1, 0, 1 값 밖에 없음
        {
            anim.SetInteger("Input", (int)h);
        }
    }

    void Fire()
    {
        if(!Input.GetButton("Fire1"))
        {
            return;
        }

        if(curShotDelay < maxShotDelay)   // 장전에 필요한 쿨타임이 지나지 않았으면 다시 쏠 수 없음
        {
            return;
        }

        switch(power)     // 플레이어 파워에 따라 다른 종류의 총알을 발사
            {
            case 1:
                // 기본 총알 발사
                //GameObject bullet = Instantiate(bulletObjA, transform.position, transform.rotation);        // 생성할 bullet 게임 오브젝트 정의, bulletObjA를 플레이어 위치에서 회전 그대로 생성          (Intatntiate로 생성할 때만 사용)
                GameObject bullet = objectManager.MakeObj("bulletPlayerA");
                bullet.transform.position = transform.position;
                Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();                                     // 생성한 bullet의 rigidbody2D 정의
                rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse);                                       // 위쪽 방향으로 발사

                break;

            case 2:
                // 기본 총알 발사
                //GameObject bulletR = Instantiate(bulletObjA, transform.position + Vector3.right * 0.1f, transform.rotation);
                //GameObject bulletL = Instantiate(bulletObjA, transform.position + Vector3.left * 0.1f, transform.rotation);

                GameObject bulletR = objectManager.MakeObj("bulletPlayerA");
                bulletR.transform.position = transform.position + Vector3.right * 0.1f;
                GameObject bulletL = objectManager.MakeObj("bulletPlayerA");
                bulletL.transform.position = transform.position + Vector3.left * 0.1f;

                Rigidbody2D rigidR = bulletR.GetComponent<Rigidbody2D>();
                Rigidbody2D rigidL = bulletL.GetComponent<Rigidbody2D>();

                rigidR.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                rigidL.AddForce(Vector2.up * 10, ForceMode2D.Impulse);

                break;

            default:            // 파워가 1, 2가 아닌 나머지 상황에서의 처리

                //GameObject bulletRR = Instantiate(bulletObjA, transform.position + Vector3.right * 0.3f, transform.rotation);
                //GameObject bulletCC = Instantiate(bulletObjB, transform.position, transform.rotation);
                //GameObject bulletLL = Instantiate(bulletObjA, transform.position + Vector3.left * 0.3f, transform.rotation);

                GameObject bulletRR = objectManager.MakeObj("bulletPlayerA");
                bulletRR.transform.position = transform.position + Vector3.right * 0.3f;
                GameObject bulletCC = objectManager.MakeObj("bulletPlayerB");
                bulletCC.transform.position = transform.position;
                GameObject bulletLL = objectManager.MakeObj("bulletPlayerA");
                bulletLL.transform.position = transform.position + Vector3.left * 0.3f;

                Rigidbody2D rigidRR = bulletRR.GetComponent<Rigidbody2D>();
                Rigidbody2D rigidCC = bulletCC.GetComponent<Rigidbody2D>();
                Rigidbody2D rigidLL = bulletLL.GetComponent<Rigidbody2D>();
                rigidRR.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                rigidCC.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                rigidLL.AddForce(Vector2.up * 10, ForceMode2D.Impulse);

                break;
        }

        curShotDelay = 0;                                                                           // 마지막 발사로부터 지난 시간 0으로 초기화   (매우 중요)
    }

    void Reload()
    {
        curShotDelay += Time.deltaTime;
    }

    void Boom()
    {
        if (!Input.GetButton("Fire2"))                        // 필살기 입력키(오른쪽 마우스 버튼)를 누르지 않으면 실행하지 않음
            return;

        if (isBoomTime)                                       // 필살기 이펙트가 나오는 중이면 실행하지 않음
            return;


        if(boom == 0)                                         // 획득한 필살기 아이템이 없으면 실행하지 않음
        {
            return;
        }

        boom--;                                               // 필살기를 사용하면 1개만큼 차감
        isBoomTime = true;                                    // 필살기 이펙트 나오는 중에는 중복해서 쓸 수 없도록 isBoomTime을 true로 변경
        gameManager.UpdateBoomIcon(boom);                         // 필살기 사용 갯수만큼 화면에 표시하도록 실행

        // 아래부터는 필살기 실행 로직

        boomEffect.SetActive(true);
        Invoke("OffBoomEffect", 4f);

        // GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");             // Enemy 태그를 가진 모든 오브젝트를 배열로 저장              오브젝트 풀을 사용 시 미리 만들어둔 프리팹을 모두 한꺼번에 들고 오기 때문에 사용하지 않음
        GameObject[] enemiesL = objectManager.GetPool("EnemyL");
        GameObject[] enemiesM = objectManager.GetPool("EnemyM");
        GameObject[] enemiesS = objectManager.GetPool("EnemyS");

        for (int index = 0; index < enemiesL.Length; index++)
        {
            if(enemiesL[index].activeSelf)
            {
                Enemy enemyLogic = enemiesL[index].GetComponent<Enemy>();
                enemyLogic.OnHit(1000);                                                    // 1000의 대미지를 주면 Enemy 스크립트 내 OnHit 함수에 의해 적이 모두 사망
            }
        }

        for (int index = 0; index < enemiesM.Length; index++)
        {
            if (enemiesM[index].activeSelf)
            {
                Enemy enemyLogic = enemiesM[index].GetComponent<Enemy>();
                enemyLogic.OnHit(1000);                                                      // 1000의 대미지를 주면 Enemy 스크립트 내 OnHit 함수에 의해 적이 모두 사망
            }
        }

        for (int index = 0; index < enemiesS.Length; index++)
        {
            if (enemiesS[index].activeSelf)
            {
                Enemy enemyLogic = enemiesS[index].GetComponent<Enemy>();
                enemyLogic.OnHit(1000);                                                      // 1000의 대미지를 주면 Enemy 스크립트 내 OnHit 함수에 의해 적이 모두 사망
            }
        }

        // GameObject[] bullets = GameObject.FindGameObjectsWithTag("EnemyBullet");          // EnemyBullet 태그를 가진 모든 오브젝트를 배열로 저장         오브젝트 풀을 사용 시 미리 만들어둔 프리팹을 모두 한꺼번에 들고 오기 때문에 사용하지 않음
        GameObject[] bulletsA = objectManager.GetPool("bulletEnemyA");
        GameObject[] bulletsB = objectManager.GetPool("bulletEnemyB");

        for (int index = 0; index < bulletsA.Length; index++)
        {
            if(bulletsA[index].activeSelf)
            {
                //Destroy(bullets[index]);                                                   // 적이 발사한 모든 총알 삭제             오브젝트 풀에서 사용하는 경우 destroy 함수 사용하지 않음
                bulletsA[index].SetActive(false);
            }
        }

        for (int index = 0; index < bulletsB.Length; index++)
        {
            if (bulletsB[index].activeSelf)
            {
                //Destroy(bullets[index]);                                                   // 적이 발사한 모든 총알 삭제             오브젝트 풀에서 사용하는 경우 destroy 함수 사용하지 않음
                bulletsB[index].SetActive(false);
            }
        }
    }


    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Border")   // Border 태그를 가진 게임 오브젝트와 충돌할 때 실행
        {
            switch(collision.gameObject.name)      // 충돌한 게임 오브젝트 이름에 따라 bool값 변경
            {
                case "Top":
                    isTouchTop = true;
                    break;
                case "Bottom":
                    isTouchBottom = true;
                    break;
                case "Right":
                    isTouchRight = true;
                    break;
                case "Left":
                    isTouchLeft = true;
                    break;
            }
        }
        else if(collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "EnemyBullet")
        {
            if(isHit)  // 한번 피격되면 아래 함수 실행하지 않음, 연속으로 피격되는 것을 방지
            {
                return;
            }

            isHit = true;

            life -= 1;
            gameManager.UpdateLifeIcon(life);     // 게임 매니저에 있는 체력 아이콘 이미지 함수를 실행하고 플레이어의 life는 매개 변수로 설정

            if(life == 0)
            {
                gameManager.GameOver();
            }
            else
            {
                gameManager.RespawnPlayer();          // 적에 피격되어 비활성화 되기 전에 먼저 게임매니저에 있는 리스폰 함수를 호출하면 2초 후 리스폰
            }
            gameObject.SetActive(false);
            //Destroy(collision.gameObject);
            
        }
        else if (collision.gameObject.tag == "Item")
        {
            Item item = collision.gameObject.GetComponent<Item>();

            switch (item.type)
            {
                case "Coin":
                    score += 1000;
                    break;

                case "Power":
                    if(power == maxPower)
                    {
                        score += 500;
                    }
                    else
                    {
                        power++;
                        AddFollower();
                    }
                    break;

                case "Boom":
                    if (boom == maxBoom)
                    {
                        score += 500;
                    }
                    else
                    {
                        boom++;
                        gameManager.UpdateBoomIcon(boom);
                    }
                    break;
            }
            //Destroy(collision.gameObject);        // 아이템 획득 후에는 아이템 삭제
            collision.gameObject.SetActive(false);
        }
    }

    void AddFollower()
    {
        if(power == 4)
        {
            followers[0].SetActive(true);
        }
        else if(power ==5)
        {
            followers[1].SetActive(true);
        }
        else if(power == 6)
        {
            followers[2].SetActive(true);
        }
    }

    void OffBoomEffect()
    {
        boomEffect.SetActive(false);
        isBoomTime = false;                 // 필살기가 종료되면 isBoomTime을 초기화 하여 다시 쓸 수 있도록 설정
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Border")
        {
            switch (collision.gameObject.name)
            {
                case "Top":
                    isTouchTop = false;
                    break;
                case "Bottom":
                    isTouchBottom = false;
                    break;
                case "Right":
                    isTouchRight = false;
                    break;
                case "Left":
                    isTouchLeft = false;
                    break;
            }
        }
    }
}
