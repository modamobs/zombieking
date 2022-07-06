using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopUpEquipmentEncyclopedia : MonoBehaviour
{
    public InitOnStartEquipmentEncyclopedia initOnStartEquipmentEncyclopedia;
    
    [SerializeField] UI ui = new UI();
    [System.Serializable]
    class UI
    {
        public Text txTitle;
        public Image[] imBtnRating;
        public GameObject[] goNeedOnCnt;
        public GameObject goNeedOnAll;

        public Text txCompleteRate; // 도감 완성률 
        public Slider pBar;

        public GameObject goSortBtnsRoot;
        public GameObject[] goBtnsSort; // 정렬 버튼 
        public Text[] goBtnsTxt; // 정렬 Txt 
        public EncySort encySort;

        public Image imAllEncyUpBtnBg;
        public Text txAllEncyUpPrice; // 등급 전체 강화하기 골드 
        public Text txAllEncyUp; // 등급 전체 강화 하기 
    }

    [System.Serializable]
    enum EncySort
    {
        ENHANT_OK = 0, // 강화 가능 순서 
        HIGH_LEVEL, // 강화 레벨 높은 순서 
        LOW_LEVEL, // 강화 레벨 낮은 순서 
        DEFAULT, // 장비 순서 
    }

    [SerializeField] Sprite spTitleBtnSelect, spTitleBtnNone;
    [SerializeField] Color coRtBtnSelect, coRtBtnNone;
    [SerializeField] Color coWhiteAph200, coWhiteAph255;

    public void SetData()
    {
        Sort();
        GameDatabase.GetInstance().equipmentEncyclopediaDB.Sort((int)ui.encySort);
        initOnStartEquipmentEncyclopedia.Init();
        UIView();
    }

    void Sort()
    {
        ui.txTitle.text = "장비 강화 도감";
        if (ui.goSortBtnsRoot.activeSelf)
        {
            int sortId = (int)ui.encySort;
            int nextSortId = sortId + 1 > ui.goBtnsSort.Length - 1 ? 0 : sortId + 1;
            int befId = nextSortId - 1 < 0 ? ui.goBtnsSort.Length - 1 : nextSortId - 1;
            for (int i = 0; i < ui.goBtnsSort.Length; i++)
            {
                ui.goBtnsSort[i].SetActive(i == nextSortId);
            }

            ui.txTitle.text = string.Format("강화도감 <size=24>({0})</size>", ui.goBtnsTxt[befId].text.ToString());
        }
    }

    void OnDisable()
    {
        if(MainUI.GetInstance() != null)
            MainUI.GetInstance().RefreshGameStat();

        GameDatabase.GetInstance().equipmentEncyclopediaDB.SendUpdateTableDB();
    }

    public void Click_Sort(int id)
    {
        ui.encySort = (EncySort)id;
        Sort();
        GameDatabase.GetInstance().equipmentEncyclopediaDB.Sort((int)ui.encySort);
        initOnStartEquipmentEncyclopedia.RefreshCells(true);
    }

    public void Click_OnClose()
    {
        gameObject.SetActive(false);
    }

    void UIView()
    {
        ViewRatingButton();
        CompleteRate();
        AllEnhantBtn(true);
    }

    public void CompleteRate()
    {
        int onRt = initOnStartEquipmentEncyclopedia.onRt;
        float v = GameDatabase.GetInstance().equipmentEncyclopediaDB.GetEncycCompletionRate(onRt);
        ui.txCompleteRate.text = string.Format("도감 완성도 {0:0.0}%", (float)(v * 100.0f));
        ui.pBar.value = v;
    }

    void ViewRatingButton()
    {
        int onRt = initOnStartEquipmentEncyclopedia.onRt;
        for (int i = 0; i < ui.imBtnRating.Length; i++)
        {
            ui.imBtnRating[i].color = onRt == i ? coRtBtnSelect : coRtBtnNone;
        }
    }

    public void Click_RatingChange(int rt)
    {
        GameDatabase.GetInstance().equipmentEncyclopediaDB.SendUpdateTableDB();
        GameDatabase.GetInstance().equipmentEncyclopediaDB.Sort((int)ui.encySort);
        initOnStartEquipmentEncyclopedia.Init(rt);
        UIView();
    }

    public void CheckNotifNeedCount()
    {
        int onRt = initOnStartEquipmentEncyclopedia.onRt;
        bool[] b = GameDatabase.GetInstance().equipmentEncyclopediaDB.GetCheckNotifNeedCount();
        for (int i = 0; i < ui.goNeedOnCnt.Length; i++)
        {
            if(ui.goNeedOnCnt[i] != null)
                ui.goNeedOnCnt[i].SetActive(b[i]);
        }
    }

    #region --- 전체 강화 ---
    public long AllEnhantBtn(bool isUI)
    {
        LogPrint.EditorPrint("--------AllEnhantBtn---");
        long all_eh_price = 0;
        long my_gold = GameDatabase.GetInstance().tableDB.GetTableDB_Goods().m_gold;
        int onRt = initOnStartEquipmentEncyclopedia.onRt;
        int ency_cnt = GameDatabase.GetInstance().equipmentEncyclopediaDB.GetCount(onRt);
        int ency_ehOkCnt = 0;
        for (int idx = 0; idx < ency_cnt; idx++)
        {
            var DB = GameDatabase.GetInstance().equipmentEncyclopediaDB.Get(idx);
            if(DB.cnt >= GameDatabase.GetInstance().equipmentEncyclopediaDB.GetNeedCount(DB.rt) && DB.eh_lv < GameDatabase.GetInstance().equipmentEncyclopediaDB.GetMaxEnhantLevel())
            {
                float power = GameDatabase.GetInstance().chartDB.GetDicBalance("equip.ency.enhant.price.power").val_float;
                int power_x = GameDatabase.GetInstance().chartDB.GetDicBalance("equip.ency.enhant.price.power").val_int;
                all_eh_price += (int)Math.Pow(DB.eh_lv + 1, power) * (power_x * DB.rt + (DB.rt + (DB.eh_lv <= 15 ? 0 : (DB.eh_lv - 15))));
                ency_ehOkCnt++;
            }
        }

        if (isUI)
        {
            ui.goNeedOnAll.SetActive(all_eh_price > 0);
            ui.imAllEncyUpBtnBg.sprite = all_eh_price > 0 && my_gold >= all_eh_price ? SpriteAtlasMng.GetInstance().GetSpriteRatingBg(onRt) : SpriteAtlasMng.GetInstance().GetSpriteButtonRedOrGray(false);
            ui.txAllEncyUpPrice.text = string.Format("{0:#,0}", all_eh_price);
            ui.txAllEncyUp.text = string.Format("{0} 등급 전체 1회씩 강화시도<color=#FF00FF>({1})</color>", GameDatabase.StringFormat.GetRatingColorText(onRt), ency_ehOkCnt);
        }

        return all_eh_price;
    }

    bool isPressOn = false;
    public void UpPress() => isPressOn = false;
    public void DwPress()
    {
        isPressOn = true;
        StopCoroutine("Loop_AllEnhantStart");
        StartCoroutine("Loop_AllEnhantStart");
    }

    IEnumerator Loop_AllEnhantStart()
    {
        int ency_ehMaxLv = GameDatabase.GetInstance().equipmentEncyclopediaDB.GetMaxEnhantLevel();
        int equip_ehMaxLv = GameDatabase.GetInstance().tableDB.GetDicBalanceEquipMaxEnhantLevel();
        float press_time = 0.1f;
        yield return null;

        while (isPressOn)
        {
            long all_price = AllEnhantBtn(false);
            if (all_price > 0)
            {
                long my_gold = GameDatabase.GetInstance().tableDB.GetTableDB_Goods().m_gold;
                if (my_gold >= all_price)
                {
                    int onRt = initOnStartEquipmentEncyclopedia.onRt;
                    int cnt = GameDatabase.GetInstance().equipmentEncyclopediaDB.GetCount(onRt);
                    for (int idx = 0; idx < cnt; idx++)
                    {
                        var DB = GameDatabase.GetInstance().equipmentEncyclopediaDB.Get(onRt, idx);
                        float power = GameDatabase.GetInstance().chartDB.GetDicBalance("equip.ency.enhant.price.power").val_float;
                        int power_x = GameDatabase.GetInstance().chartDB.GetDicBalance("equip.ency.enhant.price.power").val_int;
                        long price = (int)Math.Pow(DB.eh_lv + 1, power) * (power_x * DB.rt);
                        long now_gold = GameDatabase.GetInstance().tableDB.GetTableDB_Goods().m_gold;
                        int nedCnt = GameDatabase.GetInstance().equipmentEncyclopediaDB.GetNeedCount(DB.rt);
                        if (now_gold >= price && DB.cnt >= nedCnt && DB.eh_lv < ency_ehMaxLv)
                        {
                            if (my_gold >= price)
                            {
                                float enhantRate = (float)Math.Pow(equip_ehMaxLv - DB.eh_lv, GameDatabase.GetInstance().chartDB.GetDicBalance("equip.ency.enhant.rate.power").val_float);
                                if (DB.eh_lv > 15 && DB.eh_lv <= 20)
                                    enhantRate *= 0.8f;
                                else if (DB.eh_lv > 20 && DB.eh_lv <= 25)
                                    enhantRate *= 0.6f;
                                else if (DB.eh_lv > 26 && DB.eh_lv <= 30)
                                    enhantRate *= 0.4f;
                                else if (DB.eh_lv > 30)
                                    enhantRate *= 0.2f;

                                bool isSuccess = GameDatabase.GetInstance().GetRandomPercent() < enhantRate;
                                if (isSuccess)
                                    DB.eh_lv++;

                                DB.cnt -= nedCnt;
                                GameDatabase.GetInstance().equipmentEncyclopediaDB.UpdateClientDB(DB);
                                int encyProgEhntLv = GameDatabase.GetInstance().equipmentEncyclopediaDB.GetEncyProgressEnhantLevel(DB.rt);
                                GameDatabase.GetInstance().achievementsDB.ASetInCount((GameDatabase.AchievementsDB.Nbr)DB.rt + 10, encyProgEhntLv, false); // 업적, nbr11~17 일반~전설 장비 강화 도감 100% 완성하기!
                                GameDatabase.GetInstance().tableDB.SetUpdateGoods("gold", price, "-");
                            }
                        }
                    }

                    initOnStartEquipmentEncyclopedia.RefreshCells(true);
                    UIView();
                    CheckNotifNeedCount();
                    NotificationIcon.GetInstance().CheckNoticeEncylo(true);
                }
                else
                {
                    PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener("골드가 부족합니다.\n골드 구매 탭으로 이동됩니다.", MainUI.GetInstance().Listener_MoveItemShop);
                    break;
                }
            }
            else break;

            yield return new WaitForSeconds(press_time);
            if (press_time > 0.05f)
                press_time -= 0.2f;
            else if (press_time > 0.025f)
                press_time -= 0.05f;
            else press_time = 0.025f;
        }
    }
    #endregion

}
