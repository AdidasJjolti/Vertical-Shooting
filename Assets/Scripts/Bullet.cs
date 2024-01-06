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
            transform.Rotate(Vector3.forward * 10);       // 2D에선 z축으로 회전하기 때문에 Vector3 사용
        }
    }


    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "BorderBullet")    // 총알 전용 충돌체에 닿으면 파괴
        {
            // Destroy(gameObject);                       // Instantiate를 사용할 때는 Destroy 함수를 사용
            gameObject.SetActive(false);
        }
    }
}
