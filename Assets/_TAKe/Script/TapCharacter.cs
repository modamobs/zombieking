using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TapCharacter : MonoBehaviour
{
    public WearingEquipmentInfo wearingEquipmentInfo;

    [SerializeField] Text text_AllCombatPower;
    [SerializeField] Text text_MainStatValue;
    [SerializeField] Text text_MainStatValue1;
    [SerializeField] Text text_MainStatValue2;
    [SerializeField] Text text_MainStatValue3;
    [SerializeField] Text text_MainStatValue4;
    [SerializeField] Text text_MainStatValue5;
    [SerializeField] Text text_MainStatValue6;
    [SerializeField] Text text_MainStatValue7;
    [SerializeField] Text text_MainStatValue8;
    [SerializeField] Text text_MainStatValue9;

    [SerializeField] Text text_SopName1;
    [SerializeField] Text text_SopName2;
    [SerializeField] Text text_SopName3;
    [SerializeField] Text text_SopName4;
    [SerializeField] Text text_SopName5;
    [SerializeField] Text text_SopName6;
    [SerializeField] Text text_SopName7;
    [SerializeField] Text text_SopName8;
    [SerializeField] Text text_SopName9;
    [SerializeField] Text text_SopName10;

    [SerializeField] Text text_SpopValue1;
    [SerializeField] Text text_SpopValue2;
    [SerializeField] Text text_SpopValue3;
    [SerializeField] Text text_SpopValue4;
    [SerializeField] Text text_SpopValue5;
    [SerializeField] Text text_SpopValue6;
    [SerializeField] Text text_SpopValue7;
    [SerializeField] Text text_SpopValue8;
    [SerializeField] Text text_SpopValue9;
    [SerializeField] Text text_SpopValue10;

    // 스탯 
    // 무기   :   공격력 (stat id : 1) 
    // 방패   :   방어력 (stat id : 2) 
    // 헬멧   :   명중력 (stat id : 3) 
    // 어깨   :   치명타 공격력 (stat id : 4) 
    // 갑옷   :   체력 (stat id : 5) 
    // 팔     :  치명타 성공률 (stat id : 6) 
    // 바지   :   회피율 (stat id : 7) 
    // 부츠   :   치명타 방어력 (stat id : 8) 

    // 목걸이 :    명중력 (stat id : 3) 
    // 귀고리 :    공격 속도 (stat id : 9) 
    // 반지   :   공격력 (stat id : 1) 


    // 장신구 전용 옵션
    /// 1.PvE피해 증가 
    /// 2.PvP피해 증가 
    /// 3.PvE피해 감소 
    /// 4.PvP피해 감소 
    /// #5.골드 획득 증가 
    /// #6.장비 드랍률 증가 
    /// 7.보스 피해 증가 
    /// #8.최대 체력의 5% 회복 (확률) 
    /// 9.버블 추가 획득(확률) 
    /// 10.상대 버블 차감(확률) 

    void Start()
    {
        text_SopName1.text = GameDatabase.GetInstance().chartDB.GetAcceSpecialOptionName(1);
        text_SopName2.text = GameDatabase.GetInstance().chartDB.GetAcceSpecialOptionName(2);
        text_SopName3.text = GameDatabase.GetInstance().chartDB.GetAcceSpecialOptionName(3);
        text_SopName4.text = GameDatabase.GetInstance().chartDB.GetAcceSpecialOptionName(4);
        text_SopName5.text = GameDatabase.GetInstance().chartDB.GetAcceSpecialOptionName(5);
        text_SopName6.text = GameDatabase.GetInstance().chartDB.GetAcceSpecialOptionName(6);
        text_SopName7.text = GameDatabase.GetInstance().chartDB.GetAcceSpecialOptionName(7);
        text_SopName8.text = GameDatabase.GetInstance().chartDB.GetAcceSpecialOptionName(8);
        text_SopName9.text = GameDatabase.GetInstance().chartDB.GetAcceSpecialOptionName(9);
        text_SopName10.text = GameDatabase.GetInstance().chartDB.GetAcceSpecialOptionName(10);
    }

    public void SetStatView (GameDatabase.CharacterDB.StatValue allStat)
    {
        // 장비 매인 스탯 + 옵션 스탯 값 
        text_AllCombatPower.text = string.Format("{0:#,0}", allStat.combat_power);
        text_MainStatValue.text  = string.Format("{0:0.000}", allStat.atk_spd);
        text_MainStatValue1.text = string.Format("{0:#,0}", allStat.stat1_valPower);
        text_MainStatValue2.text = string.Format("{0:#,0}", allStat.stat2_valDefense);
        text_MainStatValue3.text = string.Format("{0:0.000}", allStat.stat3_valAccuracy);
        text_MainStatValue4.text = string.Format("{0:#,0}", allStat.stat4_valCriPower);
        text_MainStatValue5.text = string.Format("{0:#,0}", allStat.stat5_valHealth);
        text_MainStatValue6.text = string.Format("{0:0.000}", allStat.stat6_valCriChance);
        text_MainStatValue7.text = string.Format("{0:0.000}", allStat.stat7_valEvasion);
        text_MainStatValue8.text = string.Format("{0:#,0}", allStat.stat8_valCriDefense);
        text_MainStatValue9.text = string.Format("{0:0.000}", allStat.stat9_valCriEvasion);

        // 장신구 전용 옵션 값 
        text_SpopValue1.text = string.Format("{0:0.000}%", allStat.sop1_val);
        text_SpopValue2.text = string.Format("{0:0.000}%", allStat.sop2_val);
        text_SpopValue3.text = string.Format("{0:0.000}%", allStat.sop3_val);
        text_SpopValue4.text = string.Format("{0:0.000}%", allStat.sop4_val);
        text_SpopValue5.text = string.Format("{0:0.000}%", allStat.sop5_val);
        text_SpopValue6.text = string.Format("{0:0.000}%", allStat.sop6_val);
        text_SpopValue7.text = string.Format("{0:0.000}%", allStat.sop7_val);
        text_SpopValue8.text = string.Format("{0:0.000}%", allStat.sop8_val);
        text_SpopValue9.text = string.Format("{0:0.000}%", allStat.sop9_val);
        text_SpopValue10.text= string.Format("{0:0.000}%", allStat.sop10_val);
    }
}
