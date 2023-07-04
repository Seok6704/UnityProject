using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/*
인 게임 스탯 관련해서 관리해주는 스크립트입니다.
*/

public class Stat_Manager : MonoBehaviour
{
    Character_Base stat;
    public TextMeshProUGUI Lv_text, ATK_text, Gun_Attack_text, Gun_Level_text, Health_text, Critical_text, Critical_DMG_text;

    void Awake()
    {
        stat = GameObject.Find("Main_Char").GetComponent<Character_Base>(); // 변수에 Main_Char 오브젝트의 Character_Base 값 가져오기.

        Lv_text.text = stat.Lv.ToString();
        ATK_text.text = stat.ATK.ToString();
        Gun_Attack_text.text = stat.Gun_ATK.ToString();
        Gun_Level_text.text = stat.Gun_proficiency.ToString();
        Health_text.text = stat.Health.ToString();
        Critical_text.text = stat.Critical.ToString();
        Critical_DMG_text.text = stat.Critical_Dmg.ToString();
    }

}
