using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follower : MonoBehaviour
{
    public float maxShotDelay;         //�߻翡 �ʿ��� ��Ÿ��
    public float curShotDelay;         //������ �߻�κ��� ��������� �ð�
    public Vector3 followPos;          //���������� ��ǥ
    public int followDelay;            //�÷��̾ ������µ� �ɸ��� ������ ��
    public Transform parent;           // �÷��̾��� ��ǥ
    public Queue<Vector3> parentPos;   // ť�� ���� �÷��̾� ��ǥ�� ����

    public ObjectManager objectManager;


    void Awake()
    {
        parentPos = new Queue<Vector3>();
    }
    void Update()
    {
        Watch();
        Follow();

        Fire();
        Reload();
    }

    void Watch()
    {
        if(!parentPos.Contains(parent.position))       //�÷��̾ �����־ ���� ��ǥ���� �̹� ������ �� �̻� �ֽ� ��ǥ���� �޾ƿ��� ����
        {
            parentPos.Enqueue(parent.position);
        }
        
        if(parentPos.Count > followDelay)              // �Էµ� �÷��̾� ��ǥ���� �������Ⱑ �ڵ�����µ� �ɸ��� ������ ������ ��������, �� ������ ���Ⱑ �ش� �����Ӹ�ŭ �ڿ��� �÷��̾ �����
        {
            followPos = parentPos.Dequeue();
        }
        else if(parentPos.Count < followDelay)         // �������Ⱑ �ڵ�����µ� �ɸ��� ������ ������ ����� �÷��̾��� ��ǥ���� ������ ��������� �÷��̾� ��ġ�� �̵�
        {
            followPos = parent.position;
        }
    }

    void Follow()
    {
        transform.position = followPos;

    }

    void Fire()
    {
        if (!Input.GetButton("Fire1"))
        {
            return;
        }

        if (curShotDelay < maxShotDelay)   // ������ �ʿ��� ��Ÿ���� ������ �ʾ����� �ٽ� �� �� ����
        {
            return;
        }

        GameObject bullet = objectManager.MakeObj("bulletFollower");
        bullet.transform.position = transform.position;
        Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();                                     // ������ bullet�� rigidbody2D ����
        rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse);                                       // ���� �������� �߻�

        curShotDelay = 0;                                                                           // ������ �߻�κ��� ���� �ð� 0���� �ʱ�ȭ   (�ſ� �߿�)
    }

    void Reload()
    {
        curShotDelay += Time.deltaTime;
    }
}
