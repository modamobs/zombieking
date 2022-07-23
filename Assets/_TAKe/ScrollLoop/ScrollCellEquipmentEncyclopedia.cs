using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class ScrollCellEquipmentEncyclopedia : MonoBehaviour
{
    [SerializeField]
    GameDatabase.EquipmentEncyclopediaDB.JsonDB DB = new GameDatabase.EquipmentEncyclopediaDB.JsonDB();

    [SerializeField] SubDB subDB = new SubDB();
    [System.Serializable]
    private struct SubDB
    {
        public int index;
        public float ency_enhantMaxLv;// 최대 레벨 
        public int enhantPrice; // 강화 가격 
        public int encyNowCnt, encyNeedCnt;
    }

    [SerializeField] UI ui;
    [System.Serializable]
    class UI
    {
        public Image imIcon;
        public Image imRatingBg;
        public Text txRating;
        public Text txName;
       
        public Text txProgressEnhantLv; // 현재 도감 강화 레벨 

        public CanvasGroup cgEnhantRoot;
        public GameObject goOn;
        public Image imBtnBg;
        public Text txEnhantStart;
        public Text txEnhantSuccessRate;
        public Text txStatName;
        public Text txNowEnhantLv, txNextEnhantLv;
        public Text txNowStat, txNextStat;
        public Text txEnhantPrice;
        public CanvasGroup cgEnhantMaxComplete;
        public Slider sdrBar;

        // 결과 
        public CanvasGroup cagrEnhant;
        public Animation aniEnhant;
        public Text txEnhantResult;
        public Text txEnhantResultNowLv, txEnhantResultNextLv;
        public GameObject goNextLvArrow;
    }

    [SerializeField] Color coSuccess, coFailled;
    [SerializeField] Sprite spBtnOn, spBtnOff;
    [SerializeField] Color coWhiteAlpha150, coWhiteAlpha255;

    [SerializeField] string srSuccess, srFailled; // 강화성공, 강화 실패 
    [SerializeField] string ency_now_level; // 현재 도감 강화 레벨 0/0 
    [SerializeField] string ency_enhant_start;// 현재수량 / 필요수량 [강화] 
    [SerializeField] string ency_enhant_success_rate;  // 성공률 

    private int eqMaxEnhantLv = 35;
    float IncrVal = 1.0f;
    void Awake()
    {
        LogPrint.Print("--");
        srSuccess = LanguageGameData.GetInstance().GetString("ency.enhant.success"); 
        srFailled = LanguageGameData.GetInstance().GetString("ency.enhant.failled"); 
        ency_now_level = LanguageGameData.GetInstance().GetString("ency.now.level"); 
        ency_enhant_start = LanguageGameData.GetInstance().GetString("ency.enhant.start");
        ency_enhant_success_rate = LanguageGameData.GetInstance().GetString("ency.enhant.success.rate");
        eqMaxEnhantLv = GameDatabase.GetInstance().tableDB.GetDicBalanceEquipMaxEnhantLevel();
    }

    void ScrollCellIndex(int idx)
    {
        DB = GameDatabase.GetInstance().equipmentEncyclopediaDB.Get(idx);

        subDB.index = idx;
        subDB.ency_enhantMaxLv = GameDatabase.GetInstance().equipmentEncyclopediaDB.GetMaxEnhantLevel();
        subDB.encyNeedCnt = GameDatabase.GetInstance().equipmentEncyclopediaDB.GetNeedCount(DB.rt);
        subDB.encyNowCnt = DB.cnt;

        float power = GameDatabase.GetInstance().chartDB.GetDicBalance("equip.ency.enhant.price.power").val_float;
        int power_x = GameDatabase.GetInstance().chartDB.GetDicBalance("equip.ency.enhant.price.power").val_int;
        subDB.enhantPrice = (int)Math.Pow(DB.eh_lv + 1, power) * (power_x * (DB.rt + (DB.eh_lv <= 15 ? 0 : (DB.eh_lv - 15))));

        IncrVal = GameDatabase.GetInstance().chartDB.GetDicBalance(string.Format("equip.ency.main.stat.Incr.value.eqty{0}", DB.ty)).val_float; // 장비ty ->도감 장비 매인 스탯 증가 값
        ViewInfo();
    }

    public void CellRefresh() => ScrollCellIndex(subDB.index);

    void ViewInfo()
    {
        ui.cagrEnhant.alpha = 0;
        int enhant_now = DB.eh_lv;
        bool isMaxComp = enhant_now >= subDB.ency_enhantMaxLv;

        ui.imIcon.sprite = SpriteAtlasMng.GetInstance().GetSpriteEquip(DB.ty, DB.rt, DB.id);
        ui.imRatingBg.sprite = SpriteAtlasMng.GetInstance().GetSpriteRatingBg(DB.rt);
        ui.txRating.text = GameDatabase.StringFormat.GetRatingColorText(DB.rt, false);
        ui.txName.text = GameDatabase.StringFormat.GetEquipName(DB.ty, DB.rt, DB.id);
        ui.txProgressEnhantLv.text = string.Format(ency_now_level, enhant_now, subDB.ency_enhantMaxLv); // 현재 도감 강화 레벨 0/0 
        ui.sdrBar.value = (float)(enhant_now / (float)subDB.ency_enhantMaxLv);

        int opst_id = GameDatabase.GetInstance().chartDB.GetDicBalance(string.Format("ency.option.stat.id.equip.ty.{0}", DB.ty)).val_int;
        ui.txStatName.text = GameDatabase.StringFormat.GetEquipStatName(opst_id);
        ui.txNowEnhantLv.text = string.Format("Lv.{0}", enhant_now);
        switch (opst_id)
        {
            case 1:  ui.txNowStat.text = string.Format("{0:#,0}", GameDatabase.GetInstance().characterDB.GetEquipWeaponStat(DB.rt, DB.id, 0, 0, 0, DB.eh_lv) * IncrVal); break;
            case 2:  ui.txNowStat.text = string.Format("{0:#,0}", GameDatabase.GetInstance().characterDB.GetEquipShieldStat(DB.rt, DB.id, 0, 0, 0, DB.eh_lv) * IncrVal); break;
            case 4:  ui.txNowStat.text = string.Format("{0:#,0}", GameDatabase.GetInstance().characterDB.GetEquipShoulderStat(DB.rt, DB.id, 0, 0, 0, DB.eh_lv) * IncrVal); break;
            case 5:  ui.txNowStat.text = string.Format("{0:#,0}", GameDatabase.GetInstance().characterDB.GetEquipArmorStat(DB.rt, DB.id, 0, 0, 0, DB.eh_lv) * IncrVal); break;
            case 8:  ui.txNowStat.text = string.Format("{0:#,0}", GameDatabase.GetInstance().characterDB.GetEquipBootsStat(DB.rt, DB.id, 0, 0, 0, DB.eh_lv) * IncrVal); break;
            default:
                ui.txNowStat.text = "0";
                break;
        }

        ui.cgEnhantMaxComplete.alpha = isMaxComp ? 1 : 0;
        ui.cgEnhantRoot.alpha = isMaxComp ? 0 : 1;
        
        EnhantViewUI();
    }

    void EnhantViewUI()
    {
        // 가능 
        int opst_id = GameDatabase.GetInstance().chartDB.GetDicBalance(string.Format("ency.option.stat.id.equip.ty.{0}", DB.ty)).val_int;
        if (DB.eh_lv < subDB.ency_enhantMaxLv)
        {
            switch (opst_id)
            {
                case 1: ui.txNextStat.text = string.Format("{0:#,0}", GameDatabase.GetInstance().characterDB.GetEquipWeaponStat(DB.rt, DB.id, 0, 0, 0, DB.eh_lv + 1) * IncrVal); break;
                case 2: ui.txNextStat.text = string.Format("{0:#,0}", GameDatabase.GetInstance().characterDB.GetEquipShieldStat(DB.rt, DB.id, 0, 0, 0, DB.eh_lv + 1) * IncrVal); break;
                case 4: ui.txNextStat.text = string.Format("{0:#,0}", GameDatabase.GetInstance().characterDB.GetEquipShoulderStat(DB.rt, DB.id, 0, 0, 0, DB.eh_lv + 1) * IncrVal); break;
                case 5: ui.txNextStat.text = string.Format("{0:#,0}", GameDatabase.GetInstance().characterDB.GetEquipArmorStat(DB.rt, DB.id, 0, 0, 0, DB.eh_lv + 1) * IncrVal); break;
                case 8: ui.txNextStat.text = string.Format("{0:#,0}", GameDatabase.GetInstance().characterDB.GetEquipBootsStat(DB.rt, DB.id, 0, 0, 0, DB.eh_lv + 1) * IncrVal); break;
                default:
                    ui.txNextStat.text = "0";
                    break;
            }

            int eqMaxEnhantLv = GameDatabase.GetInstance().tableDB.GetDicBalanceEquipMaxEnhantLevel();
            float enhantRate = (float)Math.Pow(eqMaxEnhantLv - DB.eh_lv, GameDatabase.GetInstance().chartDB.GetDicBalance("equip.ency.enhant.rate.power").val_float);
            if (DB.eh_lv > 15 && DB.eh_lv <= 20)
                enhantRate *= 0.8f;
            else if (DB.eh_lv > 20 && DB.eh_lv <= 25)
                enhantRate *= 0.6f;
            else if (DB.eh_lv > 26 && DB.eh_lv <= 30)
                enhantRate *= 0.4f;
            else if (DB.eh_lv > 30)
                enhantRate *= 0.2f;

            ui.txNextEnhantLv.text = string.Format("Lv.{0}", DB.eh_lv + 1);
            ui.txEnhantStart.text = string.Format(ency_enhant_start, subDB.encyNowCnt, subDB.encyNeedCnt); // 현재수량 / 필요수량 [강화] 
            ui.txEnhantSuccessRate.text = string.Format(ency_enhant_success_rate, enhantRate > 100.0f ? "100" : enhantRate.ToString("N2")); // 성공률 
            ui.txEnhantPrice.text = string.Format("{0:#,0}", subDB.enhantPrice);
            
            long gold = GameDatabase.GetInstance().tableDB.GetTableDB_Goods().m_gold;
            ui.txEnhantStart.color = subDB.encyNowCnt >= subDB.encyNeedCnt ? coWhiteAlpha255 : coWhiteAlpha150;
            ui.imBtnBg.sprite = gold >= subDB.enhantPrice ? spBtnOn : spBtnOff;
            ui.goOn.SetActive(subDB.encyNowCnt >= subDB.encyNeedCnt && gold >= subDB.enhantPrice);
        }
        // 불가 -> 최대 레벨 도달 
        else
        {
            ui.txNextEnhantLv.text = "-";
            ui.txNextStat.text = "-";
        }
    }

    // 강화 시작 
    public void Click_EnhantStart()
    {
        if (ui.cagrEnhant.alpha > 0)// 강화중이었음 
        {
            StopCoroutine("EnhantStart");
            ui.aniEnhant.Stop();
            CellRefresh();
        }

        if (DB.eh_lv >= GameDatabase.GetInstance().chartDB.GetDicBalance("eq.ency.enhant.max.level").val_int)
            return;

        var goods_db = GameDatabase.GetInstance().tableDB.GetTableDB_Goods();
        int price = subDB.enhantPrice;
        if (goods_db.m_gold >= price)
        {
            if (subDB.encyNowCnt >= subDB.encyNeedCnt)
            {
                int eqMaxEnhantLv = GameDatabase.GetInstance().tableDB.GetDicBalanceEquipMaxEnhantLevel();
                float enhantRate = (float)Math.Pow(eqMaxEnhantLv - DB.eh_lv, GameDatabase.GetInstance().chartDB.GetDicBalance("equip.ency.enhant.rate.power").val_float);
                if (DB.eh_lv > 15 && DB.eh_lv <= 20)
                    enhantRate *= 0.8f;
                else if (DB.eh_lv > 20 && DB.eh_lv <= 25)
                    enhantRate *= 0.6f;
                else if (DB.eh_lv > 26 && DB.eh_lv <= 30)
                    enhantRate *= 0.4f;
                else if (DB.eh_lv > 30)
                    enhantRate *= 0.2f;

                bool isSuccess = GameDatabase.GetInstance().GetRandomPercent() < enhantRate;

                // 성공 
                if (isSuccess)
                {
                    DB.eh_lv++;
                    ui.txEnhantResult.text = srSuccess;
                    ui.txEnhantResult.color = coSuccess;
                    //ui.txEnhantResultNowLv.text = string.Format("Lv.{0}", DB.eh_lv);
                    ui.txEnhantResultNextLv.text = string.Format("Lv.{0}", DB.eh_lv);
                    ui.goNextLvArrow.SetActive(true);
                }
                // 실패
                else
                {
                    ui.txEnhantResult.text = srFailled;
                    ui.txEnhantResult.color = coFailled;
                    //ui.txEnhantResultNowLv.text = string.Format("Lv.{0}", DB.eh_lv);
                    ui.txEnhantResultNextLv.text = "";
                    ui.goNextLvArrow.SetActive(false);
                }

                GameDatabase.GetInstance().tableDB.SetUpdateGoods("gold", price, "-");

                DB.cnt -= subDB.encyNeedCnt;
                subDB.encyNowCnt = DB.cnt;
                GameDatabase.GetInstance().equipmentEncyclopediaDB.UpdateClientDB(DB);

                if (isSuccess)
                {
                    int encyProgEhntLv = GameDatabase.GetInstance().equipmentEncyclopediaDB.GetEncyProgressEnhantLevel(DB.rt);
                    GameDatabase.GetInstance().achievementsDB.ASetInCount((GameDatabase.AchievementsDB.Nbr)DB.rt + 10, encyProgEhntLv, false); // 업적, nbr11~17 일반~전설 장비 강화 도감 100% 완성하기!
                    PopUpMng.GetInstance().popUpEquipmentEncyclopedia.CompleteRate();
                }

                PopUpMng.GetInstance().EquipEncyclopediaTapNotice();
                PopUpMng.GetInstance().popUpEquipmentEncyclopedia.AllEnhantBtn(true);
                NotificationIcon.GetInstance().CheckNoticeEncylo(true);
                StartCoroutine("EnhantStart");
            }
            else PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("강화에 필요한 도감수량이 부족합니다.");
        }
        else PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener("골드가 부족합니다.\n골드 구매 탭으로 이동됩니다.", MainUI.GetInstance().Listener_MoveItemShop);
    }

    IEnumerator EnhantStart()
    {
        ui.aniEnhant.Play("EncyEnhantStart 1");
        while (ui.cagrEnhant.alpha < 1)
            yield return null;
       
        CellRefresh();
    }
}
