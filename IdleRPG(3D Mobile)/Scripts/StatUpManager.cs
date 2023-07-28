using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/*
캐릭터 스탯 업그레이드 및 관리 스크립트
*/

public class StatUpManager : MonoBehaviour
{
    public TextMeshProUGUI lvText, hpText, AtkText, rapidText, spText; // 각 스탯 텍스트
    public int statPoint; // 스탯 업에 사용되는 스탯 포인트 레벨업 시, 1씩 지급
    Character cBase;
    HpBarManager hpBar;

    void Awake()
    {
        statPoint = 0;
    }

    void Start()
    {
        cBase = GameObject.Find("Character").GetComponent<Character>();
        hpBar = GameObject.Find("Hp").GetComponent<HpBarManager>();
        StatUpdate();
    }

    public void StatUpdate() // 스탯 텍스트 업데이트 함수 모든 스탯은 string이 아닌 다른 형식이므로, string으로 변환해 준 후 입력
    {
        lvText.text = "Lv. " + cBase.level.ToString();
        hpText.text = "Hp. " + cBase.maxHp.ToString();
        AtkText.text = "Atk. " + cBase.attack.ToString();
        rapidText.text = "Rapid. " + cBase.rapid.ToString();
        spText.text = "Stat UP Point " + statPoint.ToString();
    }

    public void HpUpgrade() // HpUpgrade 버튼 클릭시 호출될 함수
    {
        if(statPoint != 0) // 잔여 스탯 포인트가 있는지 검사
        {
            statPoint -= 1;
            cBase.maxHp += 10; // 체력 업글당 최대 체력 10 증가
            hpBar.hpBar.maxValue = cBase.maxHp; // 체력 바 최대치 수정
            hpBar.HpUpdate();
            StatUpdate();
        }
    }

    public void AtkUpgrade() // AtkUpgrade 버튼 클릭시 호출될 함수
    {
        if(statPoint != 0)
        {
            statPoint -= 1;
            cBase.attack += 5; // 공격력 업글당 공격력 5 증가
            StatUpdate();
        }
    }

}
