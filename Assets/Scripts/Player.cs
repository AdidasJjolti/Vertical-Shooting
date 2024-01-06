using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int life;
    public int score;
    public float speed;
    public int power;                //�÷��̾� �Ŀ��� ����
    public int maxPower;
    public int boom;                
    public int maxBoom;
    public float maxShotDelay;         //�߻翡 �ʿ��� ��Ÿ��
    public float curShotDelay;         //������ �߻�κ��� ��������� �ð�
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

        if ((h == 1 && isTouchRight) || (h == -1 && isTouchLeft))       // ���������� �̵��ϴµ� ������ ���� ��ų� �������� �̵��ϴµ� ���� ���� ���� ��� �� �̻� �¿� �̵� ���� ���� ����
        {
            h = 0;
        }

        float v = Input.GetAxisRaw("Vertical");

        if ((v == 1 && isTouchTop) || (v == -1 && isTouchBottom))    // �������� �̵��ϴµ� ���� ���� ��ų� �Ʒ������� �̵��ϴµ� �Ʒ� ���� ���� ��� �� �̻� ���� �̵� ���� ���� ����
        {
            v = 0;
        }

        Vector3 curPos = transform.position;
        Vector3 nextPos = new Vector3(h, v, 0) * speed * Time.deltaTime;  // �ӵ���ŭ �ٸ� ��ġ�� �̵�

        transform.position = curPos + nextPos;

        if (Input.GetButtonDown("Horizontal") || Input.GetButtonUp("Horizontal"))     // Ⱦ�̵� �̵� ��ư�� ������ �ְų� �� ��� ����,    -1, 0, 1 �� �ۿ� ����
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

        if(curShotDelay < maxShotDelay)   // ������ �ʿ��� ��Ÿ���� ������ �ʾ����� �ٽ� �� �� ����
        {
            return;
        }

        switch(power)     // �÷��̾� �Ŀ��� ���� �ٸ� ������ �Ѿ��� �߻�
            {
            case 1:
                // �⺻ �Ѿ� �߻�
                //GameObject bullet = Instantiate(bulletObjA, transform.position, transform.rotation);        // ������ bullet ���� ������Ʈ ����, bulletObjA�� �÷��̾� ��ġ���� ȸ�� �״�� ����          (Intatntiate�� ������ ���� ���)
                GameObject bullet = objectManager.MakeObj("bulletPlayerA");
                bullet.transform.position = transform.position;
                Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();                                     // ������ bullet�� rigidbody2D ����
                rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse);                                       // ���� �������� �߻�

                break;

            case 2:
                // �⺻ �Ѿ� �߻�
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

            default:            // �Ŀ��� 1, 2�� �ƴ� ������ ��Ȳ������ ó��

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

        curShotDelay = 0;                                                                           // ������ �߻�κ��� ���� �ð� 0���� �ʱ�ȭ   (�ſ� �߿�)
    }

    void Reload()
    {
        curShotDelay += Time.deltaTime;
    }

    void Boom()
    {
        if (!Input.GetButton("Fire2"))                        // �ʻ�� �Է�Ű(������ ���콺 ��ư)�� ������ ������ �������� ����
            return;

        if (isBoomTime)                                       // �ʻ�� ����Ʈ�� ������ ���̸� �������� ����
            return;


        if(boom == 0)                                         // ȹ���� �ʻ�� �������� ������ �������� ����
        {
            return;
        }

        boom--;                                               // �ʻ�⸦ ����ϸ� 1����ŭ ����
        isBoomTime = true;                                    // �ʻ�� ����Ʈ ������ �߿��� �ߺ��ؼ� �� �� ������ isBoomTime�� true�� ����
        gameManager.UpdateBoomIcon(boom);                         // �ʻ�� ��� ������ŭ ȭ�鿡 ǥ���ϵ��� ����

        // �Ʒ����ʹ� �ʻ�� ���� ����

        boomEffect.SetActive(true);
        Invoke("OffBoomEffect", 4f);

        // GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");             // Enemy �±׸� ���� ��� ������Ʈ�� �迭�� ����              ������Ʈ Ǯ�� ��� �� �̸� ������ �������� ��� �Ѳ����� ��� ���� ������ ������� ����
        GameObject[] enemiesL = objectManager.GetPool("EnemyL");
        GameObject[] enemiesM = objectManager.GetPool("EnemyM");
        GameObject[] enemiesS = objectManager.GetPool("EnemyS");

        for (int index = 0; index < enemiesL.Length; index++)
        {
            if(enemiesL[index].activeSelf)
            {
                Enemy enemyLogic = enemiesL[index].GetComponent<Enemy>();
                enemyLogic.OnHit(1000);                                                    // 1000�� ������� �ָ� Enemy ��ũ��Ʈ �� OnHit �Լ��� ���� ���� ��� ���
            }
        }

        for (int index = 0; index < enemiesM.Length; index++)
        {
            if (enemiesM[index].activeSelf)
            {
                Enemy enemyLogic = enemiesM[index].GetComponent<Enemy>();
                enemyLogic.OnHit(1000);                                                      // 1000�� ������� �ָ� Enemy ��ũ��Ʈ �� OnHit �Լ��� ���� ���� ��� ���
            }
        }

        for (int index = 0; index < enemiesS.Length; index++)
        {
            if (enemiesS[index].activeSelf)
            {
                Enemy enemyLogic = enemiesS[index].GetComponent<Enemy>();
                enemyLogic.OnHit(1000);                                                      // 1000�� ������� �ָ� Enemy ��ũ��Ʈ �� OnHit �Լ��� ���� ���� ��� ���
            }
        }

        // GameObject[] bullets = GameObject.FindGameObjectsWithTag("EnemyBullet");          // EnemyBullet �±׸� ���� ��� ������Ʈ�� �迭�� ����         ������Ʈ Ǯ�� ��� �� �̸� ������ �������� ��� �Ѳ����� ��� ���� ������ ������� ����
        GameObject[] bulletsA = objectManager.GetPool("bulletEnemyA");
        GameObject[] bulletsB = objectManager.GetPool("bulletEnemyB");

        for (int index = 0; index < bulletsA.Length; index++)
        {
            if(bulletsA[index].activeSelf)
            {
                //Destroy(bullets[index]);                                                   // ���� �߻��� ��� �Ѿ� ����             ������Ʈ Ǯ���� ����ϴ� ��� destroy �Լ� ������� ����
                bulletsA[index].SetActive(false);
            }
        }

        for (int index = 0; index < bulletsB.Length; index++)
        {
            if (bulletsB[index].activeSelf)
            {
                //Destroy(bullets[index]);                                                   // ���� �߻��� ��� �Ѿ� ����             ������Ʈ Ǯ���� ����ϴ� ��� destroy �Լ� ������� ����
                bulletsB[index].SetActive(false);
            }
        }
    }


    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Border")   // Border �±׸� ���� ���� ������Ʈ�� �浹�� �� ����
        {
            switch(collision.gameObject.name)      // �浹�� ���� ������Ʈ �̸��� ���� bool�� ����
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
            if(isHit)  // �ѹ� �ǰݵǸ� �Ʒ� �Լ� �������� ����, �������� �ǰݵǴ� ���� ����
            {
                return;
            }

            isHit = true;

            life -= 1;
            gameManager.UpdateLifeIcon(life);     // ���� �Ŵ����� �ִ� ü�� ������ �̹��� �Լ��� �����ϰ� �÷��̾��� life�� �Ű� ������ ����

            if(life == 0)
            {
                gameManager.GameOver();
            }
            else
            {
                gameManager.RespawnPlayer();          // ���� �ǰݵǾ� ��Ȱ��ȭ �Ǳ� ���� ���� ���ӸŴ����� �ִ� ������ �Լ��� ȣ���ϸ� 2�� �� ������
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
            //Destroy(collision.gameObject);        // ������ ȹ�� �Ŀ��� ������ ����
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
        isBoomTime = false;                 // �ʻ�Ⱑ ����Ǹ� isBoomTime�� �ʱ�ȭ �Ͽ� �ٽ� �� �� �ֵ��� ����
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
