using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int dmg;
    public bool isRotate;

    void Update()
    {
        if(isRotate)
        {
            transform.Rotate(Vector3.forward * 10);       // 2D���� z������ ȸ���ϱ� ������ Vector3 ���
        }
    }


    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "BorderBullet")    // �Ѿ� ���� �浹ü�� ������ �ı�
        {
            // Destroy(gameObject);                       // Instantiate�� ����� ���� Destroy �Լ��� ���
            gameObject.SetActive(false);
        }
    }
}
