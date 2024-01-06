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

    public float maxShotDelay;         //�߻翡 �ʿ��� ��Ÿ��
    public float curShotDelay;         //������ �߻�κ��� ��������� �ð�

    public int patternIndex;           //���� ���� �ε���
    public int curPatternCount;        //�� ���Ϻ� ���� Ƚ��, ���� �ε����� �ٲ� ������ 0���� �ʱ�ȭ
    public int[] maxPatternCount;      //�� ���Ϻ� �ִ� ���� Ƚ��

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
        switch(enemyName)         // ���� �ٽ� Ȱ��ȭ �� ������ �ִ� ü������ �ʱ�ȭ�Ͽ� Ȱ��ȭ
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

    void FireForward()          // 4�� �� �Ʒ� �������� �Ѿ� �߻�
    {
        GameObject bulletR = objectManager.MakeObj("bulletBossA");
        bulletR.transform.position = transform.position + Vector3.right * 0.3f;
        GameObject bulletRR = objectManager.MakeObj("bulletBossA");
        bulletRR.transform.position = transform.position + Vector3.right * 0.45f;
        GameObject bulletL = objectManager.MakeObj("bulletBossA");
        bulletL.transform.position = transform.position + Vector3.left * 0.3f;
        GameObject bulletLL = objectManager.MakeObj("bulletBossA");
        bulletLL.transform.position = transform.position + Vector3.left * 0.45f;

        Rigidbody2D rigidR = bulletR.GetComponent<Rigidbody2D>();                                                           // ������ bullet�� rigidbody2D ����
        Rigidbody2D rigidRR = bulletRR.GetComponent<Rigidbody2D>();
        Rigidbody2D rigidL = bulletL.GetComponent<Rigidbody2D>();                                                           // ������ bullet�� rigidbody2D ����
        Rigidbody2D rigidLL = bulletLL.GetComponent<Rigidbody2D>();
        
        rigidR.AddForce(Vector2.down * 8, ForceMode2D.Impulse);                                                      
        rigidRR.AddForce(Vector2.down * 8, ForceMode2D.Impulse);
        rigidL.AddForce(Vector2.down * 8, ForceMode2D.Impulse);                                                      
        rigidLL.AddForce(Vector2.down * 8, ForceMode2D.Impulse);

        curPatternCount++;

        if(curPatternCount < maxPatternCount[patternIndex])   // ���� ���� �ִ� Ƚ���� ���� �������� ���� ��� �ѹ� �� ����
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

            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();                                     // ������ bullet�� rigidbody2D ����
            Vector2 dirVec = player.transform.position - transform.position;                            // ��ǥ���� ���� = ��ǥ�� ��ġ - �ڽ��� ��ġ, ������ ���̷� ���� ������ �Ǵ� (line 152���� normalized�� ǥ��ȭ)
            Vector2 ranVec = new Vector2(Random.Range(-0.5f, 0.5f), Random.Range(0f,2f));               // ��ǥ���� ���⿡�� ��� �Ѿ��� �� �������θ� ������ ���� �����ϱ� ���� ������ �߰�
            dirVec += ranVec;                                                                           // ranVec���� ������ �������� �߰��Ͽ� �߻�
            rigid.AddForce(dirVec.normalized * 3, ForceMode2D.Impulse);                                 // �÷��̾� �������� �߻�
        }
   
        curPatternCount++;

        if (curPatternCount < maxPatternCount[patternIndex])   // ���� ���� �ִ� Ƚ���� ���� �������� ���� ��� �ѹ� �� ����
        {
            Invoke("FireShot", 2);
        }
        else
        {
            Invoke("Think", 2);
        }
    }

    void FireArc()       //MaxPatternCount��ŭ �Ѿ��� �������� ���ʴ�� �߻�
    {
        GameObject bullet = objectManager.MakeObj("bulletEnemyA");
        bullet.transform.position = transform.position;
        bullet.transform.rotation = Quaternion.identity;

        Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();                                                                             // ������ bullet�� rigidbody2D ����
        Vector2 dirVec = new Vector2(Mathf.Cos(Mathf.PI * 10 * curPatternCount/maxPatternCount[patternIndex]), -1);                         // �������� ���ʴ�� �߻�
        rigid.AddForce(dirVec.normalized * 5, ForceMode2D.Impulse);                                                                         // �÷��̾� �������� �߻�
        

        curPatternCount++;

        if (curPatternCount < maxPatternCount[patternIndex])   // ���� ���� �ִ� Ƚ���� ���� �������� ���� ��� �ѹ� �� ����
        {
            Invoke("FireArc", 0.15f);
        }
        else
        {
            Invoke("Think", 2);
        }
    }

    void FireAround()      //MaxPatternCount��ŭ �Ѿ��� �������� ���� �߻�
    {
        int roundNumA = 50;
        int roundNumB = 40;
        int roundNum = curPatternCount % 2 == 0 ? roundNumA : roundNumB;                                                                 // curPatternCount�� ¦���� 50��, Ȧ����40�� ���� ���

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

            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();                                                                      // ������ bullet�� rigidbody2D ����
            Vector2 dirVec = new Vector2(Mathf.Cos(Mathf.PI * 2 * index / roundNum), Mathf.Sin(Mathf.PI * 2 * index / roundNum));        // �������� ������ �߻�, index�� �������� �Ѿ� �߻� ������ ���� ������ ������
            rigid.AddForce(dirVec.normalized * 2, ForceMode2D.Impulse);                                                                  // �÷��̾� �������� �߻�


            
            Vector3 rotVec = Vector3.forward * 360 * index / roundNum + Vector3.forward * 90;                                            // �Ѿ��� ������ �������� �Ѿ��� ȸ��
            bullet.transform.Rotate(rotVec); 
        }
        
        curPatternCount++;

        if (curPatternCount < maxPatternCount[patternIndex])   // ���� ���� �ִ� Ƚ���� ���� �������� ���� ��� �ѹ� �� ����
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
        if(enemyName == "B")                // ������ �Ϲ� ������ ���� ����
        {
            return;
        }

        Fire();
        Reload();
    }

    void Fire()
    {
        if (curShotDelay < maxShotDelay)   // ������ �ʿ��� ��Ÿ���� ������ �ʾ����� �ٽ� �� �� ����
        {
            return;
        }

        if(enemyName == "S")
        {
            //GameObject bullet = Instantiate(bulletObjA, transform.position, transform.rotation);        // ������ bullet ���� ������Ʈ ����, bulletObjA�� �÷��̾� ��ġ���� ȸ�� �״�� ����
            GameObject bullet = objectManager.MakeObj("bulletEnemyA");
            bullet.transform.position = transform.position;

            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();                                     // ������ bullet�� rigidbody2D ����

            Vector3 dirVec = player.transform.position - transform.position;                            // ��ǥ���� ���� = ��ǥ�� ��ġ - �ڽ��� ��ġ
            rigid.AddForce(dirVec.normalized * 3, ForceMode2D.Impulse);                                 // �÷��̾� �������� �߻�
        }
        else if(enemyName == "L")
        {
            //GameObject bulletR = Instantiate(bulletObjB, transform.position + Vector3.right * 0.3f, transform.rotation);        // ������ bullet ���� ������Ʈ ����, bulletObjB�� �÷��̾� ��ġ���� ȸ�� �״�� ����
            //GameObject bulletL = Instantiate(bulletObjB, transform.position + Vector3.left * 0.3f, transform.rotation);         // ������ bullet ���� ������Ʈ ����, bulletObjB�� �÷��̾� ��ġ���� ȸ�� �״�� ����
            GameObject bulletR = objectManager.MakeObj("bulletEnemyB");
            bulletR.transform.position = transform.position + Vector3.right * 0.3f;
            GameObject bulletL = objectManager.MakeObj("bulletEnemyB");
            bulletL.transform.position = transform.position + Vector3.left * 0.3f;

            Rigidbody2D rigidR = bulletR.GetComponent<Rigidbody2D>();                                                           // ������ bullet�� rigidbody2D ����
            Rigidbody2D rigidL = bulletL.GetComponent<Rigidbody2D>();                                                           // ������ bullet�� rigidbody2D ����

            Vector3 dirVecR = player.transform.position - (transform.position + Vector3.right * 0.3f);                          // ��ǥ���� ���� = ��ǥ�� ��ġ - �ڽ��� ��ġ
            Vector3 dirVecL = player.transform.position - (transform.position + Vector3.left * 0.3f);                           // ��ǥ���� ���� = ��ǥ�� ��ġ - �ڽ��� ��ġ

            rigidR.AddForce(dirVecR.normalized * 4, ForceMode2D.Impulse);                                                      // �÷��̾� ������ ����ȭ �� �� �÷��̾� �������� �߻�
            rigidL.AddForce(dirVecL.normalized * 4, ForceMode2D.Impulse);                                                      // �÷��̾� ������ ����ȭ �� �� �÷��̾� �������� �߻�
        }

        curShotDelay = 0;                  // ������ �߻�κ��� ���� �ð� 0���� �ʱ�ȭ   (�ſ� �߿�)
    }

    void Reload()
    {
        curShotDelay += Time.deltaTime;
    }

    public void OnHit(int dmg)  // �Ѿ��� ������� �Ű� ������ ���
    {
        if (health <= 0)         // �̹� ���� ü���� 0 ������ �� �� �� �̻� �ǰݵ��� ����
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
                spriteRenderer.sprite = sprites[1];     // �ǰ� �� 1�� ��������Ʈ�� ����
                Invoke("ReturnSprite", 0.1f);
            }
        }


        if(health <= 0)
        {
            Player playerLogic = player.GetComponent<Player>();      // Player ��ũ��Ʈ ��������
            playerLogic.score += enemyScore;                         // Player ��ũ��Ʈ �ȿ� �ִ� score ���� ����


            //int ran = enemyName == "B" ? 0 : Random.Range(0, 10);    // ������ ������ ������� ����

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
                //Instantiate(itemCoin, transform.position, itemCoin.transform.rotation);             // ���� ���� �ڸ����� ������ ���
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

            //Destroy(gameObject);                          // Instantiate �Լ��� ���� �����Ҷ��� ���
            gameObject.SetActive(false);                    // ������Ʈ Ǯ���� ����� ���� ��Ȱ��ȭ�� ���
            transform.rotation = Quaternion.identity;       // �� ������Ʈ �⺻ ȸ������ 0���� ����
        }
    }

    void ReturnSprite()
    {
        spriteRenderer.sprite = sprites[0];     // ���� ��������Ʈ�� ����
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "BorderBullet"  && enemyName != "B")
        {
            //Destroy(gameObject);                                            // Instantiate �Լ��� ���� �����Ҷ��� ���
            gameObject.SetActive(false);                                      // ������Ʈ Ǯ���� ����� ���� ��Ȱ��ȭ�� ���
            transform.rotation = Quaternion.identity;
        }
        else if (collision.gameObject.tag == "PlayerBullet")
        {
            Bullet bullet = collision.gameObject.GetComponent<Bullet>();      // Bullet ��ũ��Ʈ�� ��������
            OnHit(bullet.dmg);

            //Destroy(collision.gameObject);                                  // ���� ������ �Ѿ� ���ֱ�, Instantiate �Լ��� ���� �����Ҷ��� ���
            collision.gameObject.SetActive(false);                            // ������Ʈ Ǯ���� ����� ���� ��Ȱ��ȭ�� ���
        }
    }

}
