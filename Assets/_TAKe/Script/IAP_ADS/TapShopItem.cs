using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class TapShopItem : MonoBehaviour
{
    System.DateTime ads_re_time_rating3_potion = new DateTime();
    public Text txAds_re_time_rating3_potion;
    public Image imAds_re_time_rating3_potion;
    [SerializeField] Text txAds_rating3_potion_rwd_cnt;

    System.DateTime ads_re_time_gold = new DateTime();
    public Text txAds_re_time_gold;
    public Image imAds_re_time_gold;
    [SerializeField] Text txAds_gold_rwd_cnt;

    [SerializeField] PotionRt potionRt1;
    [SerializeField] PotionRt potionRt2;
    [SerializeField] PotionRt potionRt3;
    [System.Serializable]
    struct PotionRt
    {
        public Text txPrice_x100;
        public Text txPrice_x500;
        public Text txPrice_x1000;
    }

    [SerializeField] DungeonTicket dungeonTicketTop;
    [SerializeField] DungeonTicket dungeonTicketMine;
    [SerializeField] DungeonTicket dungeonTicketRaid;
    [SerializeField] DungeonTicket dungeonTicketPvp;
    [System.Serializable]
    struct DungeonTicket
    {
        public Text txBuyCntAndMaxCnt;
        public Text txPrice;
    }

    [SerializeField] EquipSton equipStonRt1;
    [SerializeField] EquipSton equipStonRt2;
    [SerializeField] EquipSton equipStonRt3;
    [SerializeField] EquipSton acceStonRt1;
    [SerializeField] EquipSton acceStonRt2;
    [SerializeField] EquipSton acceStonRt3;
    [System.Serializable]
    struct EquipSton
    {
        public int maxBuyCnt;
        public Text txBuyCntAndMaxCnt;
    }

    [SerializeField] Blessing blessingRt1;
    [SerializeField] Blessing blessingRt2;
    [SerializeField] Blessing blessingRt3;
    [System.Serializable]
    struct Blessing
    {
        public int maxBuyCnt;
        public Text txBuyCntAndMaxCnt;
    }

    [SerializeField] Gold gold1;
    [SerializeField] Gold gold2;
    [SerializeField] Gold gold3;
    [System.Serializable]
    struct Gold
    {
        public Text txPrice;
        public Text txGold;
    }

    [SerializeField] Text[] txDailyReset;
    string strReset;

    void Awake()
    {
        strReset = LanguageGameData.GetInstance().GetString("text.reset.attend");

        // 물약 구매 가격 (100개, 500개, 1000개)
        potionRt1.txPrice_x100.text = string.Format("x{0}", string.Format("{0:#,0}", GameDatabase.GetInstance().chartDB.GetDicBalance("shop.item.rt1.potion.100.dia.price").val_int));
        potionRt1.txPrice_x500.text = string.Format("x{0}", string.Format("{0:#,0}", GameDatabase.GetInstance().chartDB.GetDicBalance("shop.item.rt1.potion.500.dia.price").val_int));
        potionRt1.txPrice_x1000.text = string.Format("x{0}", string.Format("{0:#,0}", GameDatabase.GetInstance().chartDB.GetDicBalance("shop.item.rt1.potion.1000.dia.price").val_int));

        potionRt2.txPrice_x100.text = string.Format("x{0}", string.Format("{0:#,0}", GameDatabase.GetInstance().chartDB.GetDicBalance("shop.item.rt2.potion.100.dia.price").val_int));
        potionRt2.txPrice_x500.text = string.Format("x{0}", string.Format("{0:#,0}", GameDatabase.GetInstance().chartDB.GetDicBalance("shop.item.rt2.potion.500.dia.price").val_int));
        potionRt2.txPrice_x1000.text = string.Format("x{0}", string.Format("{0:#,0}", GameDatabase.GetInstance().chartDB.GetDicBalance("shop.item.rt2.potion.1000.dia.price").val_int));

        potionRt3.txPrice_x100.text = string.Format("x{0}", string.Format("{0:#,0}", GameDatabase.GetInstance().chartDB.GetDicBalance("shop.item.rt3.potion.100.dia.price").val_int));
        potionRt3.txPrice_x500.text = string.Format("x{0}", string.Format("{0:#,0}", GameDatabase.GetInstance().chartDB.GetDicBalance("shop.item.rt3.potion.500.dia.price").val_int));
        potionRt3.txPrice_x1000.text = string.Format("x{0}", string.Format("{0:#,0}", GameDatabase.GetInstance().chartDB.GetDicBalance("shop.item.rt3.potion.1000.dia.price").val_int));

        // 던전 티켓 구매 가격 
        dungeonTicketTop.txPrice.text = string.Format("x{0}", GameDatabase.GetInstance().chartDB.GetDicBalance("shop.dungeon.top.ticket.price").val_int);
        dungeonTicketMine.txPrice.text = string.Format("x{0}", GameDatabase.GetInstance().chartDB.GetDicBalance("shop.dungeon.mine.ticket.price").val_int);
        dungeonTicketRaid.txPrice.text = string.Format("x{0}", GameDatabase.GetInstance().chartDB.GetDicBalance("shop.dungeon.raid.ticket.price").val_int);
        dungeonTicketPvp.txPrice.text = string.Format("x{0}", GameDatabase.GetInstance().chartDB.GetDicBalance("shop.dungeon.pvp.ticket.price").val_int);

        // 강화석/축복 
        equipStonRt1.maxBuyCnt = GameDatabase.GetInstance().chartDB.GetDicBalance("shop.item.eq.enhant.ston.max.rt1").val_int;
        equipStonRt2.maxBuyCnt = GameDatabase.GetInstance().chartDB.GetDicBalance("shop.item.eq.enhant.ston.max.rt2").val_int;
        equipStonRt3.maxBuyCnt = GameDatabase.GetInstance().chartDB.GetDicBalance("shop.item.eq.enhant.ston.max.rt3").val_int;

        acceStonRt1.maxBuyCnt = GameDatabase.GetInstance().chartDB.GetDicBalance("shop.item.ac.enhant.ston.max.rt1").val_int;
        acceStonRt2.maxBuyCnt = GameDatabase.GetInstance().chartDB.GetDicBalance("shop.item.ac.enhant.ston.max.rt2").val_int;
        acceStonRt3.maxBuyCnt = GameDatabase.GetInstance().chartDB.GetDicBalance("shop.item.ac.enhant.ston.max.rt3").val_int;

        blessingRt1.maxBuyCnt = GameDatabase.GetInstance().chartDB.GetDicBalance("shop.item.enhant.bless.max.rt1").val_int;
        blessingRt2.maxBuyCnt = GameDatabase.GetInstance().chartDB.GetDicBalance("shop.item.enhant.bless.max.rt2").val_int;
        blessingRt3.maxBuyCnt = GameDatabase.GetInstance().chartDB.GetDicBalance("shop.item.enhant.bless.max.rt3").val_int;
    }

    void OnEnable()
    {
        SetData();
    }

    public void SetData()
    {
        var userInfo_db = GameDatabase.GetInstance().tableDB.GetUserInfo();
        InitInfoGold();
        InitInfoDungeon();
        InitInfoEquipEnhantSton(userInfo_db);
        InitInfoAcceEnhantSton(userInfo_db);
        InitInfoEnhantBlessing(userInfo_db);

        if(userInfo_db.m_attend_ymd == GameDatabase.GetInstance().attendanceDB.DailyYmd())
        {
            StopCoroutine("DailyReset_Loop");
            StartCoroutine("DailyReset_Loop");
        }
        else
        {
            foreach (var txt in txDailyReset)
                txt.text = strReset;
        }

        int ads_rt3Potion_cnt = GameDatabase.GetInstance().chartDB.GetDicBalance("ads.rating3.potion.reward").val_int;
        txAds_rating3_potion_rwd_cnt.text = string.Format("x{0:#,0}개 구매", ads_rt3Potion_cnt);
        SetAdsRating3Potion();

        float ads_gold_rwd_cnt = GameDatabase.GetInstance().chartDB.GetDicBalance("ads.gold.reward").val_float;
        txAds_gold_rwd_cnt.text = string.Format("x{0:#,0}", GameDatabase.GetInstance().questDB.GetQuestMaxSecondRewardGold(ads_gold_rwd_cnt));
        SetAdsGold();
    }

    void SetAdsRating3Potion()
    {
        string ads_rating3_potion_re_time = PlayerPrefs.GetString(PrefsKeys.prky_buy_ads_re_time_rating3_potion);
        DateTime nDate = BackendGpgsMng.GetInstance().GetNowTime();
        if (DateTime.TryParse(ads_rating3_potion_re_time, out ads_re_time_rating3_potion) == false)
            ads_re_time_rating3_potion = nDate;

        StopCoroutine("AdsRating3Potion_Loop");
        StartCoroutine("AdsRating3Potion_Loop");
    }

    void SetAdsGold()
    {
        string ads_gold = PlayerPrefs.GetString(PrefsKeys.prky_buy_ads_re_time_gold);
        DateTime nDate = BackendGpgsMng.GetInstance().GetNowTime();
        if (DateTime.TryParse(ads_gold, out ads_re_time_gold) == false)
            ads_re_time_gold = nDate;

        StopCoroutine("AdsGold_Loop");
        StartCoroutine("AdsGold_Loop");
    }

    IEnumerator DailyReset_Loop()
    {
        DateTime date24 = BackendGpgsMng.GetInstance().Get24Hour();
        DateTime nDate = BackendGpgsMng.GetInstance().GetNowTime();

        yield return null;
        while (date24 > nDate)
        {
            nDate = BackendGpgsMng.GetInstance().GetNowTime();
            int totalSec = (int)(date24 - nDate).TotalSeconds;
            int hours, minute, second;

            totalSec = totalSec % (24 * 3600);
            hours = totalSec / 3600;

            totalSec %= 3600;
            minute = totalSec / 60;

            totalSec %= 60;
            second = totalSec;

            foreach (var txt in txDailyReset)
                txt.text = string.Format("Reset {0:00} : {1:00} : {2:00}", hours, minute, second);
            
            yield return new WaitForSeconds(1f);
        }

        foreach (var txt in txDailyReset)
            txt.text = strReset;
    }

    // ------------------------------------------------------------------------------------------------
    // 물약구매
    #region 
    //일반 물약 구매 
    public async void Click_BuyRt1Potion(int buyCnt)
    {
        int diaPrice = GameDatabase.GetInstance().chartDB.GetDicBalance(string.Format("shop.item.rt1.potion.{0}.dia.price", buyCnt)).val_int;
        var goods_db = GameDatabase.GetInstance().tableDB.GetTableDB_Goods();
        bool isBlueDiaLack = goods_db.m_dia < diaPrice;
        int tbc = isBlueDiaLack == true ? await GameDatabase.GetInstance().tableDB.GetMyTBC() : 0;
        int blue_dia = goods_db.m_dia;

        string txt = string.Format("<color=#FFA500>[ 일반 물약 x{0:#,0} ]</color> 구매합니다.\n<color=#00BEFF>다이아 x{1:#,0} </color>소모", buyCnt, diaPrice);
        if (buyCnt == 100 && blue_dia + tbc >= diaPrice)
            PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener(txt, Listener_BuyRating1Potion100);
        else if (buyCnt == 500 && blue_dia + tbc >= diaPrice)
            PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener(txt, Listener_BuyRating1Potion500);
        else if (buyCnt == 1000 && blue_dia + tbc >= diaPrice)
            PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener(txt, Listener_BuyRating1Potion1000);
        else PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener("다이아가 부족합니다.\n다이아 구매 탭으로 이동됩니다.", MainUI.GetInstance().Listener_MoveTbcShop);
    }
    // 중급 물약 구매 
    public async void Click_BuyRt2Potion(int buyCnt)
    {
        int diaPrice = GameDatabase.GetInstance().chartDB.GetDicBalance(string.Format("shop.item.rt2.potion.{0}.dia.price", buyCnt)).val_int;
        var goods_db = GameDatabase.GetInstance().tableDB.GetTableDB_Goods();
        bool isBlueDiaLack = goods_db.m_dia < diaPrice;
        int tbc = isBlueDiaLack == true ? await GameDatabase.GetInstance().tableDB.GetMyTBC() : 0;
        int blue_dia = goods_db.m_dia;

        string txt = string.Format("<color=#FFA500>[ 중급 물약 x{0:#,0} ]</color> 구매합니다.\n<color=#00BEFF>다이아 x{1:#,0} </color>소모", buyCnt, diaPrice);
        if (buyCnt == 100 && blue_dia + tbc >= diaPrice)
            PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener(txt, Listener_BuyRating2Potion100);
        else if (buyCnt == 500 && blue_dia + tbc >= diaPrice)
            PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener(txt, Listener_BuyRating2Potion500);
        else if (buyCnt == 1000 && blue_dia + tbc >= diaPrice)
            PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener(txt, Listener_BuyRating2Potion1000);
        else PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener("다이아가 부족합니다.\n다이아 구매 탭으로 이동됩니다.", MainUI.GetInstance().Listener_MoveTbcShop);
    }
    // 고급 물약 구매 
    public async void Click_BuyRt3Potion(int buyCnt)
    {
        int diaPrice = GameDatabase.GetInstance().chartDB.GetDicBalance(string.Format("shop.item.rt3.potion.{0}.dia.price", buyCnt)).val_int;
        var goods_db = GameDatabase.GetInstance().tableDB.GetTableDB_Goods();
        bool isBlueDiaLack = goods_db.m_dia < diaPrice;
        int tbc = isBlueDiaLack == true ? await GameDatabase.GetInstance().tableDB.GetMyTBC() : 0;
        int blue_dia = goods_db.m_dia;

        string txt = string.Format("<color=#FFA500>[ 고급 물약 x{0:#,0} ]</color> 구매합니다.\n<color=#00BEFF>다이아 x{1:#,0} </color>소모", buyCnt, diaPrice);
        if (buyCnt == 100 && blue_dia + tbc >= diaPrice)
            PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener(txt, Listener_BuyRating3Potion100);
        else if (buyCnt == 500 && blue_dia + tbc >= diaPrice)
            PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener(txt, Listener_BuyRating3Potion500);
        else if (buyCnt == 1000 && blue_dia + tbc >= diaPrice)
            PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener(txt, Listener_BuyRating3Potion1000);
        else PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener("다이아가 부족합니다.\n다이아 구매 탭으로 이동됩니다.", MainUI.GetInstance().Listener_MoveTbcShop);
    }

    // 광고 고급 물약 구매 
    public void Click_BuyAdsRt3Potion ()
    {
        DateTime nDate = BackendGpgsMng.GetInstance().GetNowTime();
        
        if(ads_re_time_rating3_potion <= nDate)
        {
            if (GameDatabase.GetInstance().tableDB.GetUserInfo().GetAdRemoval() == true)
            {
                Listener_BuyAdsRating3Potion("success");
            }
            else
            {
                int rwd_cnt = GameDatabase.GetInstance().chartDB.GetDicBalance("ads.rating3.potion.reward").val_int;
                string txt = string.Format("<color=#FFA500>[ 고급 물약 x{0:#,0} ]</color> 구매합니다.\n<color=#00BEFF>짧은 광고 시청 후 지급됩니다.</color>", rwd_cnt);
                PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener(txt, AdsStartRating3Potion);
            }
        }
        else PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("광고 시청 대기 시간이 존재합니다.");
    }
    IEnumerator AdsRating3Potion_Loop()
    {
        DateTime nDate = BackendGpgsMng.GetInstance().GetNowTime();
        yield return null;

        if(ads_re_time_rating3_potion > nDate)
            imAds_re_time_rating3_potion.sprite = SpriteAtlasMng.GetInstance().GetSpriteButtonBox("gray");

        while (ads_re_time_rating3_potion > nDate)
        {
            nDate = BackendGpgsMng.GetInstance().GetNowTime();
            int totalSec = (int)(ads_re_time_rating3_potion - nDate).TotalSeconds;
            int hours, minute, second;

            totalSec = totalSec % (24 * 3600);
            hours = totalSec / 3600;

            totalSec %= 3600;
            minute = totalSec / 60;

            totalSec %= 60;
            second = totalSec;

            txAds_re_time_rating3_potion.text = string.Format("{0:00} : {1:00} : {2:00}", hours, minute, second);
            yield return new WaitForSeconds(0.25f);
        }

        txAds_re_time_rating3_potion.text = "광고 시청";
        imAds_re_time_rating3_potion.sprite = SpriteAtlasMng.GetInstance().GetSpriteButtonBox("purple");
    }

    void AdsStartRating3Potion ()
    {
        if(BackendGpgsMng.isEditor)
            Listener_BuyAdsRating3Potion("success");
        else
            VideoAdsMng.GetInstance().AdShow(Listener_BuyAdsRating3Potion);
    }
    public void Listener_BuyAdsRating3Potion(string result)
    {
        int rwd_cnt = GameDatabase.GetInstance().chartDB.GetDicBalance("ads.rating3.potion.reward").val_int;
        if(string.Equals(result, "success"))
        {
            ABuyPotion(3, rwd_cnt, true); // 고급 물약 구매 Ads  
        }
        else if (string.Equals(result, "OnAdFailedToLoad"))
            PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("광고 로드에 실패하였습니다. 잠시 후 다시 시도해주세요.");
        else
            PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("광고 플레이에 실패하였습니다. 잠시 후 다시 시도해주세요.");
    }

    void Listener_BuyRating1Potion100() => ABuyPotion(1, 100); // 일반 물약 100 구매 
    void Listener_BuyRating1Potion500() => ABuyPotion(1, 500); // 일반 물약 500구매 
    void Listener_BuyRating1Potion1000() => ABuyPotion(1, 1000); // 일반 물약 1000구매 

    void Listener_BuyRating2Potion100() => ABuyPotion(2, 100); // 중급 물약 100 구매 
    void Listener_BuyRating2Potion500() => ABuyPotion(2, 500); // 중급 물약 500구매 
    void Listener_BuyRating2Potion1000() => ABuyPotion(2, 1000); // 중급 물약 1000구매 

    void Listener_BuyRating3Potion100() => ABuyPotion(3, 100); // 고급 물약 100 구매 
    void Listener_BuyRating3Potion500() => ABuyPotion(3, 500); // 고급 물약 500구매 
    void Listener_BuyRating3Potion1000() => ABuyPotion(3, 1000); // 고급 물약 1000구매 

    /// <summary>
    /// 물약 구매 및 재화 데이터 변경 
    /// </summary>
    async void ABuyPotion(int rt, int buyCnt, bool isAds = false)
    {
        string rtCorStr = GameDatabase.StringFormat.GetRatingColorText(rt, false);
        string ntfMsg = string.Format(LanguageGameData.GetInstance().GetString("str.frm.shop.item.potion.buy.success"), rtCorStr, buyCnt); // [아이템] {0} 물약 x{1}개를 구매하였습니다.

        if (isAds)
        {
            var item = GameDatabase.GetInstance().tableDB.GetItem(20, rt);
            item.count += buyCnt;
            Task tsk3 = GameDatabase.GetInstance().tableDB.SendDataItem(item);
            while (Loading.Full(tsk3.IsCompleted) == false) await Task.Delay(100);
           
            int get_re_time = GameDatabase.GetInstance().chartDB.GetDicBalance("ads.rating3.potion.re.time").val_int;
            PlayerPrefs.SetString(PrefsKeys.prky_buy_ads_re_time_rating3_potion, BackendGpgsMng.GetInstance().GetNowTime().AddMinutes(get_re_time).ToString());
            SetAdsRating3Potion();

            PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox(ntfMsg); // [아이템] x{0}개를 구매하였습니다.
        }
        else
        {
            int diaPrice = GameDatabase.GetInstance().chartDB.GetDicBalance(string.Format("shop.item.rt{0}.potion.{1}.dia.price", rt, buyCnt)).val_int;
            var goods_db = GameDatabase.GetInstance().tableDB.GetTableDB_Goods();
            bool isBlueDiaLack = goods_db.m_dia < diaPrice;
            int tbc = isBlueDiaLack == true ? await GameDatabase.GetInstance().tableDB.GetMyTBC() : 0;
            int blue_dia = goods_db.m_dia;
            if (blue_dia + tbc >= diaPrice)
            {
                int dedDia = goods_db.m_dia -= diaPrice; // 내 현재 블루 다이아 차감
                int dedTbc = dedDia < 0 ? Math.Abs(dedDia) : 0;
                var item = GameDatabase.GetInstance().tableDB.GetItem(20, rt);
                item.count += buyCnt;

                Task tsk1 = GameDatabase.GetInstance().tableDB.SetUpdateGoods(goods_db);
                Task tsk2 = GameDatabase.GetInstance().tableDB.DeductionTBC(dedTbc);
                Task tsk3 = GameDatabase.GetInstance().tableDB.SendDataItem(item);
                while (Loading.Full(tsk1.IsCompleted, tsk2.IsCompleted, tsk3.IsCompleted) == false) await Task.Delay(100);
                
                PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox(ntfMsg); // [아이템] x{0}개를 구매하였습니다.

                ConvenienceFunctionMng.GetInstance().UIConvenienceAutoPosion();
            }
            else PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener("다이아가 부족합니다.\n다이아 구매 탭으로 이동됩니다.", MainUI.GetInstance().Listener_MoveTbcShop);
        }
    }
    #endregion


    // ------------------------------------------------------------------------------------------------
    // 던전 입장 티켓 구매 
    //  23	        1	        입장권 : 도전의 탑 
    //  24	        1	        입장권 : 광산 
    //  25	        1	        입장권 : 레이드 
    //  30          1           입장권 : PvP 배틀 아레나 
    #region 
    public void InitInfoDungeon()
    {
        int max_topBuyCnt = GameDatabase.GetInstance().chartDB.GetDicBalance("shop.dungeon.top.ticket.buy.max").val_int;
        int max_mineBuyCnt = GameDatabase.GetInstance().chartDB.GetDicBalance("shop.dungeon.mine.ticket.buy.max").val_int;
        int max_raidBuyCnt = GameDatabase.GetInstance().chartDB.GetDicBalance("shop.dungeon.raid.ticket.buy.max").val_int;
        int max_pvpBuyCnt = GameDatabase.GetInstance().chartDB.GetDicBalance("shop.dungeon.pvp.ticket.buy.max").val_int;
        int now_m_daily_buy_ticket_dg_top = GameDatabase.GetInstance().tableDB.GetUserInfo().m_daily_buy_ticket_dg_top;
        int now_m_daily_buy_ticket_dg_mine = GameDatabase.GetInstance().tableDB.GetUserInfo().m_daily_buy_ticket_dg_mine;
        int now_m_daily_buy_ticket_dg_raid = GameDatabase.GetInstance().tableDB.GetUserInfo().m_daily_buy_ticket_dg_raid;
        int now_m_daily_buy_ticket_dg_pvp = GameDatabase.GetInstance().tableDB.GetUserInfo().m_daily_buy_ticket_dg_pvp;

        dungeonTicketTop.txBuyCntAndMaxCnt.text = string.Format("({0} / {1})", now_m_daily_buy_ticket_dg_top, max_topBuyCnt);
        dungeonTicketMine.txBuyCntAndMaxCnt.text = string.Format("({0} / {1})", now_m_daily_buy_ticket_dg_mine, max_mineBuyCnt);
        dungeonTicketRaid.txBuyCntAndMaxCnt.text = string.Format("({0} / {1})", now_m_daily_buy_ticket_dg_raid, max_raidBuyCnt);
        dungeonTicketPvp.txBuyCntAndMaxCnt.text = string.Format("({0} / {1})", now_m_daily_buy_ticket_dg_pvp, max_pvpBuyCnt);

        dungeonTicketTop.txBuyCntAndMaxCnt.color = now_m_daily_buy_ticket_dg_top >= max_topBuyCnt ? Color.red : Color.white;
        dungeonTicketMine.txBuyCntAndMaxCnt.color = now_m_daily_buy_ticket_dg_mine >= max_mineBuyCnt ? Color.red : Color.white;
        dungeonTicketRaid.txBuyCntAndMaxCnt.color = now_m_daily_buy_ticket_dg_raid >= max_raidBuyCnt ? Color.red : Color.white;
        dungeonTicketPvp.txBuyCntAndMaxCnt.color = now_m_daily_buy_ticket_dg_pvp >= max_pvpBuyCnt ? Color.red : Color.white;
    }

    public async void Click_BuyTicketDungeon(string dgName)
    {
        int price = GameDatabase.GetInstance().chartDB.GetDicBalance(string.Format("shop.dungeon.{0}.ticket.price", dgName)).val_int;
        int dailyPurCnt =
            string.Equals(dgName, "top") ?
            GameDatabase.GetInstance().tableDB.GetUserInfo().m_daily_buy_ticket_dg_top :
            string.Equals(dgName, "mine") ?
            GameDatabase.GetInstance().tableDB.GetUserInfo().m_daily_buy_ticket_dg_mine :
            string.Equals(dgName, "raid") ?
            GameDatabase.GetInstance().tableDB.GetUserInfo().m_daily_buy_ticket_dg_raid :
            GameDatabase.GetInstance().tableDB.GetUserInfo().m_daily_buy_ticket_dg_pvp;

        int dailyPurMaxCnt = GameDatabase.GetInstance().chartDB.GetDicBalance(string.Format("shop.dungeon.{0}.ticket.buy.max", dgName)).val_int;

        if (dailyPurCnt >= dailyPurMaxCnt)
        {
            PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("일일 구매 수량이 초과하였습니다. 다음날 구매 가능합니다.");
            return;
        }

        var goods_db = GameDatabase.GetInstance().tableDB.GetTableDB_Goods();
        bool isBlueDiaLack = goods_db.m_dia < price;
        int tbc = isBlueDiaLack == true ? await GameDatabase.GetInstance().tableDB.GetMyTBC() : 0;
        if (goods_db.m_dia + tbc < price)
        {
            PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener("다이아가 부족합니다.\n다이아 구매 탭으로 이동됩니다.", MainUI.GetInstance().Listener_MoveTbcShop);
        }
        else
        {
            string strDgName = LanguageGameData.GetInstance().GetString(string.Format("text.dungeon.{0}", dgName));
            string txt = string.Format("<color=#FFA500>[ {0} 입장권 x1 ]</color> 구매합니다.\n<color=#00BEFF>다이아 x{1} </color>소모 / 일일 최대 구매량 ({2}/{3})", strDgName, price, dailyPurCnt, dailyPurMaxCnt);

            if (string.Equals(dgName, "top"))
            {
                PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener(txt, Listener_APurchaseTicketTop);
            }
            else
            if (string.Equals(dgName, "mine"))
            {
                PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener(txt, Listener_APurchaseTicketMine);
            }
            else
            if (string.Equals(dgName, "raid"))
            {
                PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener(txt, Listener_APurchaseTicketRaid);
            }
            else
            if (string.Equals(dgName, "pvp"))
            {
                PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener(txt, Listener_APurchaseTicketPvpBattleArena);
            }
        }
    }

    public void Listener_APurchaseTicketTop() => APurchaseTicket("top", 23);
    public void Listener_APurchaseTicketMine() => APurchaseTicket("mine", 24);
    public void Listener_APurchaseTicketRaid() => APurchaseTicket("raid", 25);
    public void Listener_APurchaseTicketPvpBattleArena() => APurchaseTicket("pvp", 30);

    private async void APurchaseTicket(string tkName, int item_type_id)
    {
        int price = GameDatabase.GetInstance().chartDB.GetDicBalance(string.Format("shop.dungeon.{0}.ticket.price", tkName)).val_int;
        var goods_db = GameDatabase.GetInstance().tableDB.GetTableDB_Goods();
        bool isBlueDiaLack = goods_db.m_dia < price;
        int tbc = isBlueDiaLack == true ? await GameDatabase.GetInstance().tableDB.GetMyTBC() : 0;
        if (goods_db.m_dia + tbc >= price)
        {
            var userInfo_db = GameDatabase.GetInstance().tableDB.GetUserInfo();
            var item_db = GameDatabase.GetInstance().tableDB.GetItem(item_type_id, 1);

            int dedDia = goods_db.m_dia -= price; // 내 현재 블루 다이아 차감
            int dedTbc = dedDia < 0 ? Math.Abs(dedDia) : 0;

            switch (tkName)
            {
                case "top":
                    userInfo_db.m_daily_buy_ticket_dg_top++;
                    break;
                case "mine":
                    userInfo_db.m_daily_buy_ticket_dg_mine++;
                    break;
                case "raid":
                    userInfo_db.m_daily_buy_ticket_dg_raid++;
                    break;
                case "pvp":
                    userInfo_db.m_daily_buy_ticket_dg_pvp++;
                    break;
            }

            item_db.count++;
            Task tsk1 = GameDatabase.GetInstance().tableDB.SetUpdateGoods(goods_db);
            Task tsk2 = GameDatabase.GetInstance().tableDB.DeductionTBC(dedTbc);
            Task tsk3 = GameDatabase.GetInstance().tableDB.SetUpdate_UserInfo(userInfo_db);
            Task tsk4 = GameDatabase.GetInstance().tableDB.SendDataItem(item_db);

            while (Loading.Full(tsk1.IsCompleted, tsk2.IsCompleted, tsk3.IsCompleted, tsk4.IsCompleted) == false) await Task.Delay(100);

            InitInfoDungeon();

            string ntfMsg = string.Format(LanguageGameData.GetInstance().GetString(string.Format("str.frm.shop.item.dg.{0}.ticket.buy.success", tkName)), 1);
            PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox(ntfMsg); // [아이템] 도전의 탑 입장권 x{0}개를 구매하였습니다.
            NotificationIcon.GetInstance().CheckNoticeContentsTicket();
        }
    }
    #endregion

    // ------------------------------------------------------------------------------------------------
    // 장비 강화석 구매 
    //  21	        1	        일반 장비 강화석
    //  21	        2	        중급 장비 강화석
    //  21	        3	        고급 장비 강화석
    //  21	        4	        희귀 장비 강화석
    #region 
    public void InitInfoEquipEnhantSton(GameDatabase.TableDB.UserInfo userInfo_db)
    {
        equipStonRt1.txBuyCntAndMaxCnt.text = string.Format("({0} / {1})", userInfo_db.m_daily_buy_eq_ehnt_ston_rt1.ToString(), equipStonRt1.maxBuyCnt.ToString());
        equipStonRt2.txBuyCntAndMaxCnt.text = string.Format("({0} / {1})", userInfo_db.m_daily_buy_eq_ehnt_ston_rt2.ToString(), equipStonRt2.maxBuyCnt.ToString());
        equipStonRt3.txBuyCntAndMaxCnt.text = string.Format("({0} / {1})", userInfo_db.m_daily_buy_eq_ehnt_ston_rt3.ToString(), equipStonRt3.maxBuyCnt.ToString());

        equipStonRt1.txBuyCntAndMaxCnt.color = userInfo_db.m_daily_buy_eq_ehnt_ston_rt1 >= equipStonRt1.maxBuyCnt ? Color.red : Color.white;
        equipStonRt2.txBuyCntAndMaxCnt.color = userInfo_db.m_daily_buy_eq_ehnt_ston_rt2 >= equipStonRt2.maxBuyCnt ? Color.red : Color.white;
        equipStonRt3.txBuyCntAndMaxCnt.color = userInfo_db.m_daily_buy_eq_ehnt_ston_rt3 >= equipStonRt3.maxBuyCnt ? Color.red : Color.white;
    }

    /// <summary>
    /// 팝업에서 구매 진행 
    /// </summary>
    public void Click_OpenBuyEquipEnhantSton() => PopUpMng.GetInstance().Open_ShopItemPurchase(PopUpShopItemPurchase.PurchaseType.EquipEnhantSton);
    #endregion

    // ------------------------------------------------------------------------------------------------
    // 장신구 강화석 구매 
    //  27	        1	        일반 장신구 강화석
    //  27	        2	        중급 장신구 강화석
    //  27	        3	        고급 장신구 강화석
    //  27	        4	        희귀 장신구 강화석
    #region
    public void InitInfoAcceEnhantSton(GameDatabase.TableDB.UserInfo userInfo_db)
    {
        acceStonRt1.txBuyCntAndMaxCnt.text = string.Format("({0} / {1})", userInfo_db.m_daily_buy_ac_ehnt_ston_rt1.ToString(), acceStonRt1.maxBuyCnt.ToString());
        acceStonRt2.txBuyCntAndMaxCnt.text = string.Format("({0} / {1})", userInfo_db.m_daily_buy_ac_ehnt_ston_rt2.ToString(), acceStonRt2.maxBuyCnt.ToString());
        acceStonRt3.txBuyCntAndMaxCnt.text = string.Format("({0} / {1})", userInfo_db.m_daily_buy_ac_ehnt_ston_rt3.ToString(), acceStonRt3.maxBuyCnt.ToString());

        acceStonRt1.txBuyCntAndMaxCnt.color = userInfo_db.m_daily_buy_ac_ehnt_ston_rt1 >= acceStonRt1.maxBuyCnt ? Color.red : Color.white;
        acceStonRt2.txBuyCntAndMaxCnt.color = userInfo_db.m_daily_buy_ac_ehnt_ston_rt2 >= acceStonRt2.maxBuyCnt ? Color.red : Color.white;
        acceStonRt3.txBuyCntAndMaxCnt.color = userInfo_db.m_daily_buy_ac_ehnt_ston_rt3 >= acceStonRt3.maxBuyCnt ? Color.red : Color.white;
    }

    /// <summary>
    /// 팝업에서 구매 진행 
    /// </summary>
    public void Click_OpenBuyAcceEnhantSton() => PopUpMng.GetInstance().Open_ShopItemPurchase(PopUpShopItemPurchase.PurchaseType.AcceEnhantSton);
    #endregion

    // ------------------------------------------------------------------------------------------------
    // 강화 축복 주문서 구매 
    //  22	        1	        일반 강화 축복 주문서
    //  22	        2	        중급 강화 축복 주문서
    //  22	        3	        고급 강화 축복 주문서
    #region 
    public void InitInfoEnhantBlessing(GameDatabase.TableDB.UserInfo userInfo_db)
    {
        blessingRt1.txBuyCntAndMaxCnt.text = string.Format("({0} / {1})", userInfo_db.m_daily_buy_ehnt_bless_rt1.ToString(), blessingRt1.maxBuyCnt.ToString());
        blessingRt2.txBuyCntAndMaxCnt.text = string.Format("({0} / {1})", userInfo_db.m_daily_buy_ehnt_bless_rt2.ToString(), blessingRt2.maxBuyCnt.ToString());
        blessingRt3.txBuyCntAndMaxCnt.text = string.Format("({0} / {1})", userInfo_db.m_daily_buy_ehnt_bless_rt3.ToString(), blessingRt3.maxBuyCnt.ToString());

        blessingRt1.txBuyCntAndMaxCnt.color = userInfo_db.m_daily_buy_ehnt_bless_rt1 >= blessingRt1.maxBuyCnt ? Color.red : Color.white;
        blessingRt2.txBuyCntAndMaxCnt.color = userInfo_db.m_daily_buy_ehnt_bless_rt2 >= blessingRt2.maxBuyCnt ? Color.red : Color.white;
        blessingRt3.txBuyCntAndMaxCnt.color = userInfo_db.m_daily_buy_ehnt_bless_rt3 >= blessingRt3.maxBuyCnt ? Color.red : Color.white;
    }

    /// <summary>
    /// 팝업에서 구매 진행 
    /// </summary>
    public void Click_OpenBuyEnhantBlessing() => PopUpMng.GetInstance().Open_ShopItemPurchase(PopUpShopItemPurchase.PurchaseType.EnhantBlessing);
    #endregion

    // ------------------------------------------------------------------------------------------------
    // 골드 구매 
    #region 
    void InitInfoGold()
    {
        // gold10000.txPrice.text = string.Format("x{0:#,###}", GameDatabase.GetInstance().questDB.GetQuestMaxSecondRewardGold(0.1f));
        // gold100000.txPrice.text = string.Format("x{0:#,###}", GameDatabase.GetInstance().questDB.GetQuestMaxSecondRewardGold(1));
        // gold1000000.txPrice.text = string.Format("x{0:#,###}", GameDatabase.GetInstance().questDB.GetQuestMaxSecondRewardGold(10));

        gold1.txGold.text = string.Format("x{0:#,0}", GameDatabase.GetInstance().questDB.GetQuestMaxSecondRewardGold(0.1f));
        gold2.txGold.text = string.Format("x{0:#,0}", GameDatabase.GetInstance().questDB.GetQuestMaxSecondRewardGold(1.0f));
        gold3.txGold.text = string.Format("x{0:#,0}", GameDatabase.GetInstance().questDB.GetQuestMaxSecondRewardGold(10.0f));
        gold1.txPrice.text = string.Format("x{0:#,0}", GameDatabase.GetInstance().chartDB.GetDicBalance("shop.item.gold.id1.dia.price").val_int);
        gold2.txPrice.text = string.Format("x{0:#,0}", GameDatabase.GetInstance().chartDB.GetDicBalance("shop.item.gold.id2.dia.price").val_int);
        gold3.txPrice.text = string.Format("x{0:#,0}", GameDatabase.GetInstance().chartDB.GetDicBalance("shop.item.gold.id3.dia.price").val_int);
    }

    public async void Click_BuyGold(int buyCnt)
    {
        int diaPrice = GameDatabase.GetInstance().chartDB.GetDicBalance(string.Format("shop.item.gold.id{0}.dia.price", buyCnt)).val_int;
        var goods_db = GameDatabase.GetInstance().tableDB.GetTableDB_Goods();
        bool isBlueDiaLack = goods_db.m_dia < diaPrice;
        int tbc = isBlueDiaLack == true ? await GameDatabase.GetInstance().tableDB.GetMyTBC() : 0;

        long gold = 0;
        if(buyCnt == 1)
            gold = GameDatabase.GetInstance().questDB.GetQuestMaxSecondRewardGold(0.1f);
        else if(buyCnt == 2)
            gold = GameDatabase.GetInstance().questDB.GetQuestMaxSecondRewardGold(1.0f);
        else if(buyCnt == 3)
            gold = GameDatabase.GetInstance().questDB.GetQuestMaxSecondRewardGold(10.0f);

        string txt = string.Format("<color=#FFA500>[ 골드 x{0:#,0}원 ]</color> 구매합니다.\n<color=#00BEFF>다이아 x{1:#,0} </color>소모", gold, diaPrice);
        if (buyCnt == 1 && goods_db.m_dia + tbc >= diaPrice)
        {
            PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener(txt, Listener_BuyGold1);
        }
        else if (buyCnt == 2 && goods_db.m_dia + tbc >= diaPrice)
        {
            PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener(txt, Listener_BuyGold2);
        }
        else if (buyCnt == 3 && goods_db.m_dia + tbc >= diaPrice)
        {
            PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener(txt, Listener_BuyGold3);
        }
        else PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener("다이아가 부족합니다.\n다이아 구매 탭으로 이동됩니다.", MainUI.GetInstance().Listener_MoveTbcShop);
    }

    public void Listener_BuyGold1() => ABuyGold(1, GameDatabase.GetInstance().questDB.GetQuestMaxSecondRewardGold(0.1f));
    public void Listener_BuyGold2() => ABuyGold(2, GameDatabase.GetInstance().questDB.GetQuestMaxSecondRewardGold(1.0f));
    public void Listener_BuyGold3() => ABuyGold(3, GameDatabase.GetInstance().questDB.GetQuestMaxSecondRewardGold(10.0f));

    // 광고 골드 구매 
    public void Click_BuyAdsGold()
    {
        DateTime nDate = BackendGpgsMng.GetInstance().GetNowTime();
        if (ads_re_time_gold <= nDate)
        {
            if (GameDatabase.GetInstance().tableDB.GetUserInfo().GetAdRemoval() == true)
            {
                Listener_BuyAdsGold("success");
            }
            else
            {
                long rwd_gold = GameDatabase.GetInstance().questDB.GetQuestMaxSecondRewardGold(GameDatabase.GetInstance().chartDB.GetDicBalance("ads.gold.reward").val_float);
                PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener(string.Format("<color=#FFA500>[ 골드 {0:#,0}원 ]</color> 구매합니다.\n<color=#00BEFF>짧은 광고 시청 후 지급됩니다.</color>", rwd_gold), AdsStartGold);
            }
        }
        else PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("광고 시청 대기 시간이 존재합니다.");
    }
    IEnumerator AdsGold_Loop()
    {
        DateTime nDate = BackendGpgsMng.GetInstance().GetNowTime();
        yield return null;

        if (ads_re_time_gold > nDate)
            imAds_re_time_gold.sprite = SpriteAtlasMng.GetInstance().GetSpriteButtonBox("gray");

        while (ads_re_time_gold > nDate)
        {
            nDate = BackendGpgsMng.GetInstance().GetNowTime();
            int totalSec = (int)(ads_re_time_gold - nDate).TotalSeconds;
            int hours, minute, second;

            totalSec = totalSec % (24 * 3600);
            hours = totalSec / 3600;

            totalSec %= 3600;
            minute = totalSec / 60;

            totalSec %= 60;
            second = totalSec;

            txAds_re_time_gold.text = string.Format("{0:00} : {1:00} : {2:00}", hours, minute, second);
            yield return new WaitForSeconds(0.25f);
        }

        txAds_re_time_gold.text = "광고 시청";
        imAds_re_time_gold.sprite = SpriteAtlasMng.GetInstance().GetSpriteButtonBox("purple");
    }

    void AdsStartGold()
    {
        if (BackendGpgsMng.isEditor)
            Listener_BuyAdsGold("success");
        else
            VideoAdsMng.GetInstance().AdShow(Listener_BuyAdsGold);
    }
    public void Listener_BuyAdsGold(string result)
    {
        if (string.Equals(result, "success"))
        {
            long rwd_gold = GameDatabase.GetInstance().questDB.GetQuestMaxSecondRewardGold(GameDatabase.GetInstance().chartDB.GetDicBalance("ads.gold.reward").val_float);
            ABuyGold(3, rwd_gold, true); // 골드 구매 
        }
        else if (string.Equals(result, "OnAdFailedToLoad"))
            PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("광고 로드에 실패하였습니다. 잠시 후 다시 시도해주세요.");
        else
            PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("광고 플레이에 실패하였습니다. 잠시 후 다시 시도해주세요.");
    }

    public async void ABuyGold(int buyId, long addGold, bool isAds = false)
    {
        string ntfMsg = string.Format(LanguageGameData.GetInstance().GetString("str.frm.shop.item.gold.buy.success"), addGold); // 골드 {0:#,###}원을 구매하였습니다. 
        if (isAds)
        {
            var goods_db = GameDatabase.GetInstance().tableDB.GetTableDB_Goods();
            goods_db.m_gold += addGold;
            Task tsk1 = GameDatabase.GetInstance().tableDB.SetUpdateGoods(goods_db);
            while (Loading.Full(tsk1.IsCompleted) == false) await Task.Delay(100);

            int get_re_time = GameDatabase.GetInstance().chartDB.GetDicBalance("ads.gold.re.time").val_int;
            PlayerPrefs.SetString(PrefsKeys.prky_buy_ads_re_time_gold, BackendGpgsMng.GetInstance().GetNowTime().AddMinutes(get_re_time).ToString());
            SetAdsGold();

            PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox(ntfMsg); // [아이템] x{0}개를 구매하였습니다.
        }
        else
        {
            int diaPrice = GameDatabase.GetInstance().chartDB.GetDicBalance(string.Format("shop.item.gold.id{0}.dia.price", buyId)).val_int;
            var goods_db = GameDatabase.GetInstance().tableDB.GetTableDB_Goods();
            bool isBlueDiaLack = goods_db.m_dia < diaPrice;
            int tbc = isBlueDiaLack == true ? await GameDatabase.GetInstance().tableDB.GetMyTBC() : 0;
            int blue_dia = goods_db.m_dia;
            if (blue_dia + tbc >= diaPrice)
            {
                int dedDia = goods_db.m_dia -= diaPrice; // 내 현재 블루 다이아 차감
                int dedTbc = dedDia < 0 ? Math.Abs(dedDia) : 0;
                goods_db.m_gold += addGold;
                Task tsk1 = GameDatabase.GetInstance().tableDB.SetUpdateGoods(goods_db);
                Task tsk2 = GameDatabase.GetInstance().tableDB.DeductionTBC(dedTbc);
                while (Loading.Full(tsk1.IsCompleted, tsk2.IsCompleted) == false) await Task.Delay(100);

                PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox(ntfMsg); // [아이템] x{0}개를 구매하였습니다.
            }
            else PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("골드 구매에 실패했습니다.\n다시 시도하여 주세요.");
        }
    }
    #endregion
}
