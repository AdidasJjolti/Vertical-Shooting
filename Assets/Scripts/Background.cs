using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    public float speed;

    public int startIndex;
    public int endIndex;
    public Transform[] sprites;
    float viewHeight;                                                                              // ī�޶� y�� ����*2�� ���� ȭ�� ���̸� �޾ƿ��� ����

    private void Awake()
    {
        viewHeight = Camera.main.orthographicSize * 2;                                             // ���� ī�޶� ����� ���ϰ� �������� 2���� ī�޶� ���� ���̸� viewHeight�� ����
    }

    void Update()
    {
        Vector3 curPos = transform.position;
        Vector3 nextPos = Vector3.down * speed * Time.deltaTime;
        transform.position = curPos + nextPos;                                                     // ����� ��ġ�� ���� ��ġ���� nextPos�� �ӵ���ŭ �Ʒ��� ������ �Ÿ��� ��� ����


        if (sprites[endIndex].position.y < viewHeight * -1)                                         // endIndex(���� �Ʒ��� ��ġ�� ��ȣ)�� ���� ��� ��������Ʈ�� ī�޶� ���� ���̸�ŭ �Ʒ��� ����� ����
        {
            Vector3 backSpritePos = sprites[startIndex].localPosition;                             // startIndex(���� ���� ��ġ�� ��ȣ)�� ���� ��� ��������Ʈ�� localPosition�� ��������
            Vector3 frontSpritePos = sprites[endIndex].localPosition;                              // endIndex(���� �Ʒ��� ��ġ�� ��ȣ)�� ���� ��� ��ũ����Ʈ�� localPosition�� ��������

            //Vector3 backSpritePos = sprites[startIndex].position;                                // startIndex(���� ���� ��ġ�� ��ȣ)�� ���� ��� ��������Ʈ�� localPosition�� ��������
            //Vector3 frontSpritePos = sprites[endIndex].position;                                 // endIndex(���� �Ʒ��� ��ġ�� ��ȣ)�� ���� ��� ��ũ����Ʈ�� localPosition�� ��������

            sprites[endIndex].transform.localPosition = backSpritePos + Vector3.up * viewHeight;   // ���� �Ʒ��� ��ġ�� ��� ��������Ʈ�� ȭ�� ������ �Ʒ��� ����� ���� ���� ��ġ�� ��� ��������Ʈ�� ��ġ���� ī�޶� ���� ���̸�ŭ ���� ��ġ �̵�


            //endIndex ��� ��������Ʈ�� ���� ���� �ö��� �� Index��ȣ�� �ٽ� �����ϴ� ����
            int startIndexSave = startIndex;                                                       // startIndexSave�� startIndex�� ���� ���� 2�� �ʱ�ȭ
            startIndex = endIndex;                                                                 // startIndex�� 0(endIndex�� ���� ��)���� ����
            endIndex = (startIndexSave - 1 == -1) ? sprites.Length - 1 : startIndexSave - 1;       // endIndex�� 1�� ���� (����� ��ġ), ���� startIndexSave -1�� -1�� �Ǹ� �迭 ������ ���� ���Ƿ� 2�� ����
        }
    }
}
