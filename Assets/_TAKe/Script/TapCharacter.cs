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
    // 0무기    : 공격력 
    // 1방패    : 방어력 
    // 2헬멧%   : 피해량 감소
    // 3어깨%   : 체력
    // 4갑옷    : 체력 
    // 5팔      : 치명타 공격력
    // 6바지    : 공격력
    // 7부츠%   : 공격 속도

    // 8목걸이% : 치명타 발동률 
    // 9귀고리% : 방어력
    // 10반지%   : 공격력

    // 장신구 전용 옵션
    // 1:pve피해 증가(공격력의 ?%)
    // 2:pvp피해 증가(공격력의 ?%) 
    // 3:pve피해 감소(%) 
    // 4:pvp피해 감소(%) 
    // 5:골드 획득 증가 
    // 6:장비 획득 증가
    // 7:보스 몬스터 피해 증가
    // 8:공격시 현재 체력의 ?%를 회복 
    // 9:공격시 랜덤으로 버블 추가획득
    // 10:공격시 랜덤으로 상대 버블 차감

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
        text_MainStatValue.text  = string.Format("{0:0.000}", allStat.p7_boo_attackSpeed);
        text_MainStatValue1.text = string.Format("{0:#,0}", allStat.p0_wea_attackPower);
        text_MainStatValue2.text = string.Format("{0:#,0}", allStat.p1_shi_defance);
        text_MainStatValue3.text = string.Format("{0:0.000}", allStat.p2_hel_damageReduction);
        text_MainStatValue4.text = string.Format("{0:#,0}", allStat.p5_gau_criticalPower);
        text_MainStatValue5.text = string.Format("{0:#,0}", (allStat.p4_arm_health + (allStat.p4_arm_health * allStat.p3_sho_health / 100)));
        text_MainStatValue6.text = string.Format("{0:0.000}", allStat.p8_nec_criticalRate);

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
