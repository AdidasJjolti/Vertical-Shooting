using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follower : MonoBehaviour
{
    public float maxShotDelay;         //발사에 필요한 쿨타임
    public float curShotDelay;         //마지막 발사로부터 현재까지의 시간
    public Vector3 followPos;          //보조무기의 좌표
    public int followDelay;            //플레이어를 따라오는데 걸리는 프레임 수
    public Transform parent;           // 플레이어의 좌표
    public Queue<Vector3> parentPos;   // 큐에 넣은 플레이어 좌표값 변수

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
        if(!parentPos.Contains(parent.position))       //플레이어가 멈춰있어서 같은 좌표값이 이미 있으면 더 이상 최신 좌표값을 받아오지 않음
        {
            parentPos.Enqueue(parent.position);
        }
        
        if(parentPos.Count > followDelay)              // 입력된 플레이어 좌표값이 보조무기가 뒤따라오는데 걸리는 프레임 수보다 많아지면, 그 때보조 무기가 해당 프레임만큼 뒤에서 플레이어를 따라옴
        {
            followPos = parentPos.Dequeue();
        }
        else if(parentPos.Count < followDelay)         // 보조무기가 뒤따라오는데 걸리는 프레임 수보다 저장된 플레이어의 좌표수가 적으면 보조무기는 플레이어 위치로 이동
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

        if (curShotDelay < maxShotDelay)   // 장전에 필요한 쿨타임이 지나지 않았으면 다시 쏠 수 없음
        {
            return;
        }

        GameObject bullet = objectManager.MakeObj("bulletFollower");
        bullet.transform.position = transform.position;
        Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();                                     // 생성한 bullet의 rigidbody2D 정의
        rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse);                                       // 위쪽 방향으로 발사

        curShotDelay = 0;                                                                           // 마지막 발사로부터 지난 시간 0으로 초기화   (매우 중요)
    }

    void Reload()
    {
        curShotDelay += Time.deltaTime;
    }
}
