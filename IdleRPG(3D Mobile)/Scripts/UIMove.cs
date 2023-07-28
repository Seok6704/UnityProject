using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
UI 위치 관리 스크립트
*/

public class UIMove : MonoBehaviour
{
    public bool startParentPos; // 시작 시, 카메라에 노출되는 MainUI Canvas 위치에서 실행 될 것인지 확인.
    Vector3 pos; // 기존 배치 위치 기억용 변수(ReturnPos)

    void Start()
    {
        pos = transform.position; // 시작 배치 위치 저장.

        if( startParentPos )
        {
            SetParentPos();
        }
    }

    public void SetParentPos() // 현재 UI(Canvas / Panel)를 Parent위치(MainUI)로 이동
    {
        transform.position = transform.parent.position;
    }

    public void ReturnPos() // 현재 UI(Canvas / Panel)를 원래 위치로 이동
    {
        transform.position = pos;
    }
}
