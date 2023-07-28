using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/*
캐릭터 레벨 관련 스크립트.
*/

public class LevelManager : MonoBehaviour
{
    Character cBase;
    StatUpManager stat;
    HpBarManager hpBar;
    public TextMeshProUGUI nowLevel; // 레벨 텍스트
    int requireExp; // 레벨업 요구 경험치
    int maxLevel; // 최대 레벨

    void Start()
    {
        cBase = GameObject.Find("Character").GetComponent<Character>();
        stat = GameObject.Find("Stat").GetComponent<StatUpManager>();
        hpBar = GameObject.Find("Hp").GetComponent<HpBarManager>();
        LevelUpdate();     
    }

    public void LevelUpdate() // 레벨 텍스트 업데이트용 함수
    {
        List<Dictionary<string, object>> levelData = CSVReader.Read("LevelSheet"); 
        // CSV 파일 "LevelSheet" 에서 levelData 리스트로 읽어오는 기능, CSVREADER.cs는 외부에서 참조(상업적 이용 가능 파일). https://github.com/tikonen/blog/blob/master/csvreader/CSVReader.cs
        nowLevel.text = "Lv. " + cBase.level.ToString(); // 레벨 텍스트 작성
        requireExp = (int)levelData[cBase.level]["Exp"]; // 요구 경험치 설정
        maxLevel = (int)levelData[levelData.Count - 1]["Level"]; // 최대 레벨 설정
    }

    public void ExpUp(int exp) // 경험치 습득 함수, EnemyBase에서 몬스터 사망 시, 사용
    {
        cBase.nowExp += exp; // 현재 경험치를 습득 경험치 만큼 증가
        CheckLevelUp();
    }

    void CheckLevelUp() // 레벨업 체크 함수
    {
        if( cBase.nowExp >= requireExp ) // 현재 경험치가 요구 경험치보다 많을 경우
        {
            if( cBase.level < maxLevel) // 레벨이 현재 제작한 최대 레벨보다 낮을 경우만 동작
            {
                cBase.level += 1; // 레벨 1 증가
                stat.statPoint += 1; // 스탯 포인트 1 증가
                cBase.hp = cBase.maxHp; // 레벨업 시, 체력을 다시 최대로 채워줌
                hpBar.HpUpdate(); // 체력바 업데이트
                stat.StatUpdate(); // 스탯 포인트 변화 반영을 위해 호출
                cBase.nowExp -= requireExp; // 현재 경험치 감소(요구 경험치 만큼)
                LevelUpdate();
            }
            else requireExp = int.MaxValue; // 레벨이 현재 제작한 최대 레벨과 같을 경우 레벨업 요구 경험치를 불가능한 수준으로 변경.
            CheckLevelUp(); // 재귀함수로 만들어서 연속 레벨업 구현.
        }
    }
}
