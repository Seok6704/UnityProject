using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
캐릭터 체력바 관리 스크립트
*/

public class HpBarManager : MonoBehaviour
{
    public Camera cam; // UI 표시할 카메라
    Character cBase; // 캐릭터 스탯 정보
    public Slider hpBar; // 체력바
    int maxHp; // 체력바 최대 체력


    void Start()
    {
        cBase = GameObject.Find("Character").GetComponent<Character>();
        hpBar = GameObject.Find("Hp").GetComponent<Slider>();
        hpBar.minValue = 0; // 체력바 최소치 설정
        hpBar.maxValue = cBase.maxHp; // 체력바 최대치는 캐릭터의 최대체력으로 설정
        HpUpdate(); // 체력바 상태 업데이트
    }

    public void HpUpdate() // 체력바 상태 업데이트 용 함수
    {
        hpBar.value = cBase.hp; // 체력바의 현재 값을 캐릭터의 체력으로 설정.
    }
}
