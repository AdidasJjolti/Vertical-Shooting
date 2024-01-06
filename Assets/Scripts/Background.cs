using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    public float speed;

    public int startIndex;
    public int endIndex;
    public Transform[] sprites;
    float viewHeight;                                                                              // 카메라 y축 길이*2의 값인 화면 길이를 받아오는 변수

    private void Awake()
    {
        viewHeight = Camera.main.orthographicSize * 2;                                             // 메인 카메라 사이즈를 구하고 사이즈의 2배인 카메라 세로 길이를 viewHeight로 설정
    }

    void Update()
    {
        Vector3 curPos = transform.position;
        Vector3 nextPos = Vector3.down * speed * Time.deltaTime;
        transform.position = curPos + nextPos;                                                     // 배경의 위치는 현재 위치에서 nextPos의 속도만큼 아래로 내려온 거리로 계속 변경


        if (sprites[endIndex].position.y < viewHeight * -1)                                         // endIndex(가장 아래에 위치한 번호)를 가진 배경 스프라이트가 카메라 세로 길이만큼 아래로 벗어나면 실행
        {
            Vector3 backSpritePos = sprites[startIndex].localPosition;                             // startIndex(가장 위에 위치한 번호)를 가진 배경 스프라이트의 localPosition을 가져오기
            Vector3 frontSpritePos = sprites[endIndex].localPosition;                              // endIndex(가장 아래에 위치한 번호)를 가진 배경 스크라이트의 localPosition을 가져오기

            //Vector3 backSpritePos = sprites[startIndex].position;                                // startIndex(가장 위에 위치한 번호)를 가진 배경 스프라이트의 localPosition을 가져오기
            //Vector3 frontSpritePos = sprites[endIndex].position;                                 // endIndex(가장 아래에 위치한 번호)를 가진 배경 스크라이트의 localPosition을 가져오기

            sprites[endIndex].transform.localPosition = backSpritePos + Vector3.up * viewHeight;   // 가장 아래에 위치한 배경 스프라이트가 화면 밖으로 아래로 벗어나면 가장 위에 위치한 배경 스프라이트의 위치에서 카메라 세로 길이만큼 위로 위치 이동


            //endIndex 배경 스프라이트가 가장 위로 올라갔을 때 Index번호를 다시 설정하는 과정
            int startIndexSave = startIndex;                                                       // startIndexSave를 startIndex의 최초 값인 2로 초기화
            startIndex = endIndex;                                                                 // startIndex를 0(endIndex의 원래 값)으로 변경
            endIndex = (startIndexSave - 1 == -1) ? sprites.Length - 1 : startIndexSave - 1;       // endIndex는 1로 변경 (가운데에 위치), 만약 startIndexSave -1이 -1이 되면 배열 범위를 벗어 나므로 2로 변경
        }
    }
}
