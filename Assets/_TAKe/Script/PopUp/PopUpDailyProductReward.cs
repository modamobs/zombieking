using BackEnd;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

public class PopUpDailyProductReward : MonoBehaviour
{
    public GameObject goRoot;
    [SerializeField] RewardUI rewardUI7DayDiamond;
    [SerializeField] RewardUI rewardUI30DayDiamond;
    [System.Serializable]
    struct RewardUI
    {
        public int dayRewardCount;
        public Text txDay;
        public GameObject goBtnBuy;
        public GameObject goBtnReward;
        public GameObject goBtnRewardCmp;

        public GameObject[] goRewardIcon;
        public Text[] txRewardCnt;
    }


    [SerializeField] AutoSaleUI autoSaleUI;
    [System.Serializable]
    struct AutoSaleUI
    {
        public string isAutoSaleType; // cash or ads 
        public Image imAutoSaleType; // 레드 다야 아이콘 or 광고 아이콘 
        public Text txAutoSaleAdsLevel;

        public DateTime endDate;
        public Text txRemmDate;
        public Text tx7DayTbcPrice;
        public Text tx30DayTbcPrice;
        public Text tx1DayBuleDiaPrice;

        public DateTime videoReDate;
        public Text txVideoReDate;
        public Image imVideoBtnBg;
        public Text txVideoBtnText;

        // purchase.auto.sale.video.free.1hour	        1시간 구매\n광고 시청 후
        // purchase.auto.sale.video.free.1hour.delay	광고 대기중 {0:00}분{1:00}초
    }

    // text 
    string str_remaining_day; // 남은 기간 
    int day7diaRwdCnt, day30diaRwdCnt;

    void Awake()
    {
        str_remaining_day = LanguageGameData.GetInstance().GetString("text.remaining.day");
        day7diaRwdCnt = GameDatabase.GetInstance().chartDB.GetDicBalance("day7.daily.dia.reward.cnt").val_int;
        day30diaRwdCnt = GameDatabase.GetInstance().chartDB.GetDicBalance("day30.daily.dia.reward.cnt").val_int;

        AwakeAUtoSale();
    }

   public void Init()
    {
        InitInfoDailyDia();
        InitInfoAutoSale();
    }

    #region 7일 / 30일 매일 다이아 
    public  void InitInfoDailyDia()
    {
        bool is7DayReward = CheckProduct7DayReward();
        bool is30DayReward = CheckProduct30DayReward();

        if (is7DayReward || is30DayReward)
        {
            NotificationIcon.GetInstance().OnDaily30Day7Day(is7DayReward, is30DayReward);
        }
        else
        {
            NotificationIcon.GetInstance().OffDaily30Day7Day(false, false);
        }
    }

    /// <summary>
    /// 7일 매일 다이아 
    /// </summary>
    bool CheckProduct7DayReward()
    {
        var goods_db = GameDatabase.GetInstance().tableDB.GetTableDB_Goods();
        // 7일 매일 다이아 구매 보상 받을수 있는 날인지 체크 
        DateTime try7DayStartDate; // 시작 날짜 
        DateTime try7DayEndDate; // 종료 날짜 
        DateTime try7DayDailyRewardDate; // 마지막으로 보상 받은 날짜 
        DateTime daily_ymd = DateTime.Parse(BackendGpgsMng.GetInstance().GetNowTime().ToString("yyyy/MM/dd"));

        if (DateTime.TryParse(goods_db.m_7DayTbcStartDate, out try7DayStartDate) == false)
            try7DayStartDate = daily_ymd;

        if (DateTime.TryParse(goods_db.m_7DayTbcEndDate, out try7DayEndDate) == false)
            try7DayEndDate = new DateTime();

        DateTime.TryParse(goods_db.m_7DayTbcDailyRewardDate, out try7DayDailyRewardDate);
        if (try7DayDailyRewardDate < try7DayStartDate.AddDays(-1))
            try7DayDailyRewardDate = try7DayStartDate.AddDays(-1);

        DateTime m7day_start_ymd = DateTime.Parse(try7DayStartDate.ToString("yyyy/MM/dd"));
        DateTime m7day_end_ymd = DateTime.Parse(try7DayEndDate.ToString("yyyy/MM/dd"));
        DateTime m7DayDailyRewardDate_ymd = DateTime.Parse(try7DayDailyRewardDate.ToString("yyyy/MM/dd"));

        bool is7Deadline = (m7day_start_ymd <= daily_ymd && m7day_end_ymd >= daily_ymd); // 기간 내 
        int reward7DayCnt = is7Deadline == true ? (int)(daily_ymd - m7DayDailyRewardDate_ymd).Days : (int)(m7day_end_ymd - m7DayDailyRewardDate_ymd).Days;
        bool is7DailyReward = reward7DayCnt > 0;
        if (is7DailyReward == true || is7Deadline == true) // 보상 카운트 || 기간내 
        {
            int remm7DayCnt = is7Deadline == true ? (int)(m7day_end_ymd - try7DayDailyRewardDate).Days : 0; // 남은 기간 
            if (remm7DayCnt < 0)
                remm7DayCnt = 0;

            if (goRoot.activeSelf)
            {
                rewardUI7DayDiamond.goRewardIcon[0].SetActive(false);

                int toaRwdDia = (int)(day7diaRwdCnt * reward7DayCnt);
                rewardUI7DayDiamond.dayRewardCount = reward7DayCnt;
                rewardUI7DayDiamond.txDay.text = string.Format("{0} : {1}Day", str_remaining_day, (remm7DayCnt + 1).ToString());
                rewardUI7DayDiamond.txDay.color = is7DailyReward ? Color.yellow : Color.gray;
                rewardUI7DayDiamond.txRewardCnt[1].text = 
                    string.Format("x{0}{1}", is7DailyReward == true ? toaRwdDia : day7diaRwdCnt,
                    is7DailyReward == true ? string.Format("<color=yellow>({0})</color>", reward7DayCnt.ToString()) : "");
                rewardUI7DayDiamond.goBtnBuy.SetActive(false); // 구매 버튼 ON or OFF 
                rewardUI7DayDiamond.goBtnReward.SetActive(is7DailyReward); // 보상 받기 버튼 
                rewardUI7DayDiamond.goBtnRewardCmp.SetActive(!is7DailyReward); // 보상 완료 image 
            }

            // 보상 가능 
            if (is7DailyReward)
            {
                LogPrint.Print(" ======7day tbc========== 오늘 " + reward7DayCnt + "일치 보상 가능 ================ ");
                return true;
            }
            // 오늘 보상 받음 
            else
            {
                LogPrint.Print(" ======7day tbc========== 오늘 보상 이미 받음 ================ ");
                return false;
            }
        }
        else
        {
            if (goRoot.activeSelf)
            {
                foreach (var item in rewardUI7DayDiamond.goRewardIcon)
                    item.SetActive(true);

                rewardUI7DayDiamond.dayRewardCount = 0;
                rewardUI7DayDiamond.txDay.text = "";
                rewardUI7DayDiamond.txRewardCnt[0].text = string.Format("+{0}", GameDatabase.GetInstance().chartDB.GetDicBalance("day7.daily.dia.one.reward.cnt").val_int);
                rewardUI7DayDiamond.txRewardCnt[1].text = string.Format("x{0}", day7diaRwdCnt);
                rewardUI7DayDiamond.goBtnBuy.SetActive(is7Deadline == false); // 구매 버튼 ON or OFF 
                rewardUI7DayDiamond.goBtnReward.SetActive(false); // 보상 받기 버튼 
                rewardUI7DayDiamond.goBtnRewardCmp.SetActive(is7Deadline == true); // 보상 완료 image 
            }

            LogPrint.Print(" ======7day tbc========== 기간 만료 혹은 구매 하지않았음 ================ ");
            return false;
        }
    }

    /// <summary>
    /// 30일 매일 다이아 
    /// </summary>
    bool CheckProduct30DayReward()
    {
        var goods_db = GameDatabase.GetInstance().tableDB.GetTableDB_Goods();
        // 30일 매일 다이아 구매 보상 받을수 있는 날인지 체크 
        DateTime try30DayStartDate; // 시작 날짜 
        DateTime try30DayEndDate; // 종료 날짜 
        DateTime try30DayDailyRewardDate; // 마지막으로 보상 받은 날짜 
        DateTime daily_ymd = DateTime.Parse(BackendGpgsMng.GetInstance().GetNowTime().ToString("yyyy/MM/dd"));

        if (DateTime.TryParse(goods_db.m_30DayTbcStartDate, out try30DayStartDate) == false)
            try30DayStartDate = daily_ymd;

        if (DateTime.TryParse(goods_db.m_30DayTbcEndDate, out try30DayEndDate) == false)
            try30DayEndDate = new DateTime();

        DateTime.TryParse(goods_db.m_30DayTbcDailyRewardDate, out try30DayDailyRewardDate);
        if (try30DayDailyRewardDate < try30DayStartDate.AddDays(-1))
            try30DayDailyRewardDate = try30DayStartDate.AddDays(-1);

        DateTime m30day_start_ymd = DateTime.Parse(try30DayStartDate.ToString("yyyy/MM/dd"));
        DateTime m30day_end_ymd = DateTime.Parse(try30DayEndDate.ToString("yyyy/MM/dd"));
        DateTime m30DayDailyRewardDate_ymd = DateTime.Parse(try30DayDailyRewardDate.ToString("yyyy/MM/dd"));

        bool is30DayDeadline = (m30day_start_ymd <= daily_ymd && m30day_end_ymd >= daily_ymd); // 기간 내 
        int reward30DayCnt = is30DayDeadline == true ? (int)(daily_ymd - m30DayDailyRewardDate_ymd).Days : (int)(m30day_end_ymd - m30DayDailyRewardDate_ymd).Days;
        bool is30DailyReward = reward30DayCnt > 0;
        if (is30DailyReward == true || is30DayDeadline == true) // 보상 카운트 || 기간내 
        {
            int remm30DayCnt = is30DayDeadline == true ? (int)(m30day_end_ymd - try30DayDailyRewardDate).Days : 0; // 남은 기간 
            if (remm30DayCnt < 0)
                remm30DayCnt = 0;
            
            if (goRoot.activeSelf)
            {
                rewardUI30DayDiamond.goRewardIcon[0].SetActive(false);

                int toaRwdDia = (int)(day30diaRwdCnt * reward30DayCnt);
                rewardUI30DayDiamond.dayRewardCount = reward30DayCnt;
                rewardUI30DayDiamond.txDay.text = string.Format("{0} : {1}Day", str_remaining_day, (remm30DayCnt + 1).ToString());
                rewardUI30DayDiamond.txDay.color = is30DailyReward ? Color.yellow : Color.gray;
                rewardUI30DayDiamond.txRewardCnt[1].text = string.Format("x{0}{1}", is30DailyReward == true ? toaRwdDia : day30diaRwdCnt, 
                    is30DailyReward == true ? string.Format("<color=yellow>({0})</color>", reward30DayCnt.ToString()) : "");
                rewardUI30DayDiamond.goBtnBuy.SetActive(false); // 구매 버튼 ON or OFF 
                rewardUI30DayDiamond.goBtnReward.SetActive(is30DailyReward); // 보상 받기 버튼 
                rewardUI30DayDiamond.goBtnRewardCmp.SetActive(!is30DailyReward); // 보상 완료 image 
            }

            // 보상 가능 
            if (is30DailyReward)
            {
                LogPrint.Print(" ======30day tbc========== 오늘 " + reward30DayCnt + "일치 보상 가능 ================ ");
                return true;
            }
            // 오늘 보상 받음 
            else
            {
                LogPrint.Print(" ======30day tbc========== 오늘 보상 이미 받음 ================ ");
                return false;
            }
        }
        else
        {
            if (goRoot.activeSelf)
            {
                foreach (var item in rewardUI30DayDiamond.goRewardIcon)
                    item.SetActive(true);

                rewardUI30DayDiamond.dayRewardCount = 0;
                rewardUI30DayDiamond.txDay.text = "";
                rewardUI30DayDiamond.txRewardCnt[0].text = string.Format("+{0}", GameDatabase.GetInstance().chartDB.GetDicBalance("day30.daily.dia.one.reward.cnt").val_int);
                rewardUI30DayDiamond.txRewardCnt[1].text = string.Format("x{0}", day30diaRwdCnt);
                rewardUI30DayDiamond.goBtnBuy.SetActive(is30DayDeadline == false); // 구매 버튼 ON or OFF 
                rewardUI30DayDiamond.goBtnReward.SetActive(false); // 보상 받기 버튼 
                rewardUI30DayDiamond.goBtnRewardCmp.SetActive(is30DayDeadline == true); // 보상 완료 image 
            }
            
            LogPrint.Print(" ======30day tbc========== 기간 만료 혹은 구매 하지않았음 ================ ");
            return false;
        }
    }

    /// <summary>
    /// 7일 매일 다이아 보상 받기 
    /// </summary>
    public async void Click_Reward7Day()
    {
        DateTime nDate = BackendGpgsMng.GetInstance().GetNowTime();
        var goods_db = GameDatabase.GetInstance().tableDB.GetTableDB_Goods();
        int toaRwdDia = (int)(day7diaRwdCnt * rewardUI7DayDiamond.dayRewardCount);
        goods_db.m_7DayTbc -= toaRwdDia;
        if (goods_db.m_7DayTbc < 0)
            goods_db.m_7DayTbc = 0;

        goods_db.m_7DayTbcDailyRewardDate = nDate.ToString();
        Task tsk1 = GameDatabase.GetInstance().tableDB.SetUpdateGoods(goods_db);
        while (Loading.Bottom(tsk1.IsCompleted) == false) await Task.Delay(100);

        InitInfoDailyDia();
        if (MainUI.GetInstance().tapIAP.tapShopDiamond.gameObject.activeSelf)
            MainUI.GetInstance().tapIAP.tapShopDiamond.DayProduct();

        MainUI.GetInstance().topUI.GetInfoViewTBC();
    }

    /// <summary>
    /// 30일 매일 다이아 보상 받기 
    /// </summary>
    public async void Click_Reward30Day()
    {
        DateTime nDate = BackendGpgsMng.GetInstance().GetNowTime();
        var goods_db = GameDatabase.GetInstance().tableDB.GetTableDB_Goods();
        int toaRwdDia = (int)(day30diaRwdCnt * rewardUI30DayDiamond.dayRewardCount);
        goods_db.m_30DayTbc -= toaRwdDia;
        if (goods_db.m_30DayTbc < 0)
            goods_db.m_30DayTbc = 0;

        goods_db.m_30DayTbcDailyRewardDate = nDate.ToString();
        Task tsk1 = GameDatabase.GetInstance().tableDB.SetUpdateGoods(goods_db);
        while (Loading.Bottom(tsk1.IsCompleted) == false) await Task.Delay(100);

        InitInfoDailyDia();
        if(MainUI.GetInstance().tapIAP.tapShopDiamond.gameObject.activeSelf)
            MainUI.GetInstance ().tapIAP.tapShopDiamond.DayProduct();

        MainUI.GetInstance().topUI.GetInfoViewTBC();
    }

    public void BuyTBC(IAPButton ib)
    {
        LogPrint.Print("<color=yellow>----BuyTestTBC-----</color>Buy productId : " + ib.productId);
        IAPMng.GetInstance().ABuyTBC(ib.productId);
    }
    #endregion


    #region 장비 자동 판매 1시간 / 7일 / 30일 
    void AwakeAUtoSale()
    {
        autoSaleUI.tx7DayTbcPrice.text = string.Format("x{0}", GameDatabase.GetInstance().chartDB.GetDicBalance("auto.sale.7day.tbc.price").val_int);
        autoSaleUI.tx30DayTbcPrice.text = string.Format("x{0}", GameDatabase.GetInstance().chartDB.GetDicBalance("auto.sale.30day.tbc.price").val_int);
        autoSaleUI.tx1DayBuleDiaPrice.text = string.Format("x{0}", GameDatabase.GetInstance().chartDB.GetDicBalance("auto.sale.1day.blud.dia.price").val_int);
    }

    public void InitInfoAutoSale()
    {
        var userinfo_db = GameDatabase.GetInstance().tableDB.GetUserInfo();
        autoSaleUI.isAutoSaleType = "";
        DateTime nDate = BackendGpgsMng.GetInstance().GetNowTime();
        DateTime endDate = GameDatabase.GetInstance().convenienceFunctionDB.GetDate_ConvenFunAutoSale();
        if ((endDate - nDate).TotalSeconds <= 0) // 캐시 구매 시간이 없을 떄 
        {
            endDate = GameDatabase.GetInstance().convenienceFunctionDB.GetDate_ConvenFunAutoSaleVideo(); // 광고 구매 시간 체크 
            if ((endDate - nDate).TotalSeconds > 0)
                autoSaleUI.isAutoSaleType = "ads";
            else userinfo_db.m_auto_eq_sale_video_lv = 0;
        }
        else
        {
            autoSaleUI.isAutoSaleType = "cash";
            userinfo_db.m_auto_eq_sale_video_lv = 0;
        }

        GameDatabase.GetInstance().tableDB.SetUpdate_UserInfo(userinfo_db, true);
        autoSaleUI.endDate = endDate;

        if (string.Equals(autoSaleUI.isAutoSaleType, "ads"))
            autoSaleUI.imAutoSaleType.sprite = SpriteAtlasMng.GetInstance().GetSpriteVideo();
        else if (string.Equals(autoSaleUI.isAutoSaleType, "cash"))
            autoSaleUI.imAutoSaleType.sprite = SpriteAtlasMng.GetInstance().GetTransparency(); //autoSaleUI.imAutoSaleType.sprite = SpriteAtlasMng.GetInstance().GetSpriteTBC();
        else autoSaleUI.imAutoSaleType.sprite = SpriteAtlasMng.GetInstance().GetTransparency();

        autoSaleUI.txAutoSaleAdsLevel.text = string.Format("Lv.{0}", userinfo_db.m_auto_eq_sale_video_lv);

        StopCoroutine("Routin_AutoSale");
        StartCoroutine("Routin_AutoSale");

        // 광고 대기 시간 체크 
        //DateTime tryVideoDate;
        //if (DateTime.TryParse(PlayerPrefs.GetString(PrefsKeys.key_VideoDateAutoSale), out tryVideoDate) == false)
        //    tryVideoDate = nDate;

        //autoSaleUI.videoReDate = tryVideoDate;
        //autoSaleUI.imVideoBtnBg.color = autoSaleUI.videoReDate > nDate ? Color.gray : Color.red;
        //StopCoroutine("Routin_Video");
        //StartCoroutine("Routin_Video");
    }

    /// <summary>
    /// 자동 판매 구매 묻는 창 
    /// </summary>
    public void Click_TBCRedDiaBuyAutoSaleDay(int day)
    {
        string strNotice = LanguageGameData.GetInstance().GetString(string.Format("purchase.auto.sale.{0}day.ask", day));
        if(day == 7)
            PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener(strNotice, Listener_AutoSale7);
        else if(day == 30)
            PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener(strNotice, Listener_AutoSale30);
    }

    public void Click_BlueDiaBuyAutoSaleDay()
    {
        string strNotice = LanguageGameData.GetInstance().GetString(string.Format("purchase.auto.sale.{0}day.ask", 1));
        PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener(strNotice, Listener_AutoSale1);
    }

    void Listener_AutoSale1() => BuyAutoSale(false, 1);
    void Listener_AutoSale7() => BuyAutoSale(true, 7);
    void Listener_AutoSale30() => BuyAutoSale(true, 30);
  
    /// <summary>
    /// 자동 판매 구매 시작 
    /// </summary>
    async void BuyAutoSale(bool isTbc, int day)
    {
        if(int.Equals(day, 1) || int.Equals(day, 7) || int.Equals(day, 30))
        {
            bool isBuySuccess = false;
            if (isTbc)
            {
                if(int.Equals(day, 7) || int.Equals(day, 30))
                {
                    int price = GameDatabase.GetInstance().chartDB.GetDicBalance(string.Format("auto.sale.{0}day.tbc.price", day)).val_int;
                    int tbc = await GameDatabase.GetInstance().tableDB.GetMyTBC();
                    if (tbc >= price)
                    {
                        string tbcProductName = GameDatabase.GetInstance().chartDB.GetDicBalance(string.Format("auto.sale.{0}day.tbc.product.key", day)).val_string;
                        string uuid = GameDatabase.GetInstance().tableDB.GetProductUUID(tbcProductName);
                        if (!string.IsNullOrEmpty(uuid)) // TBC 상품 UUID 
                        {
                            BackendReturnObject bro1 = null;
                            SendQueue.Enqueue(Backend.TBC.UseTBC, uuid, tbcProductName, callback => { bro1 = callback; });
                            while (Loading.Bottom(bro1) == false) await Task.Delay(100);

                            isBuySuccess = bro1.IsSuccess();
                            if (!isBuySuccess)
                            {
                                PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox(bro1.GetMessage());
                            }
                        }

                        Task<bool> tsk_buy = GameDatabase.GetInstance().tableDB.AutoSaleAddDay(day);
                        while (Loading.Full(tsk_buy.IsCompleted) == false) await Task.Delay(100);
                        isBuySuccess = true;
                    }
                    else
                    {
                        PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener("다이아가 부족합니다.\n다이아 구매 탭으로 이동됩니다.", MainUI.GetInstance().Listener_MoveTbcShop);
                    }
                }
            }
            else 
            {
                if (int.Equals(day, 1))
                {
                    int dia_price = GameDatabase.GetInstance().chartDB.GetDicBalance(string.Format("auto.sale.{0}day.blud.dia.price", day)).val_int;
                    var goods_db = GameDatabase.GetInstance().tableDB.GetTableDB_Goods();
                    if (goods_db.m_dia >= dia_price)
                    {
                        // 비용 차감 
                        goods_db.m_dia -= dia_price;
                        Task<bool> tsk_goods = GameDatabase.GetInstance().tableDB.SetUpdateGoods(goods_db); // 강화 비용 값 전송 
                        Task<bool> tsk_buy = GameDatabase.GetInstance().tableDB.AutoSaleAddDay(day); // 구매 
                        while (Loading.Full(tsk_goods.IsCompleted, tsk_buy.IsCompleted) == false) await Task.Delay(100);
                        isBuySuccess = true;
                    }
                    else
                    {
                        PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("블루 다이아가 부족합니다.");
                    }
                }
            }

            if (isBuySuccess)
            {
                InitInfoAutoSale();
                ConvenienceFunctionMng.GetInstance().InitConvenienceAutoSale();

                if (GameDatabase.GetInstance().convenienceFunctionDB.GetUseingConvenFunAutoSale())
                    PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox(string.Format("편의 기능 : 장비 판매/분해 <color=yellow>{0}일</color> 이용권을 추가 완료하였습니다.", day));
                else
                    PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox(string.Format("편의 기능 : 장비 판매/분해 <color=yellow>{0}일</color> 이용권을 구매 완료하였습니다.", day));

                await Task.Delay(500);
                MainUI.GetInstance().topUI.GetInfoViewTBC();
                if (ConvenienceFunctionMng.GetInstance().convenFun.cfAutoSale.onOff == IG.ConvenienceFunction.OnOff.OFF)
                {
                    PopUpMng.GetInstance().Open_AutoSale();
                }
            }
        }
    }

    /// <summary>
    /// 자동 판매 남은 시간 
    /// </summary>
    IEnumerator Routin_AutoSale()
    {
        DateTime nDate = BackendGpgsMng.GetInstance().GetNowTime();
        DateTime endDate = autoSaleUI.endDate;
        int totalSec = (int)(endDate - nDate).TotalSeconds;
        yield return null;
        while (endDate > nDate)
        {
            nDate = BackendGpgsMng.GetInstance().GetNowTime();
            totalSec = (int)(endDate - nDate).TotalSeconds;
            if (totalSec >= 0)
            {
                int day, hours, minute, second;
                day = totalSec / (24 * 3600);

                totalSec = totalSec % (24 * 3600);
                hours = totalSec / 3600;

                totalSec %= 3600;
                minute = totalSec / 60;

                totalSec %= 60;
                second = totalSec;

                autoSaleUI.txRemmDate.text = string.Format("(남은 기간 {0:00}일 {1:00}:{2:00}:{3:00})", day, hours, minute, second);
            }

            yield return new WaitForSeconds(0.5f);
        }

        autoSaleUI.txRemmDate.text = "";
        autoSaleUI.imAutoSaleType.sprite = SpriteAtlasMng.GetInstance().GetTransparency();

        if (string.Equals(autoSaleUI.isAutoSaleType, "ads"))
            autoSaleUI.txAutoSaleAdsLevel.text = "Lv.0";
    }
    #endregion


    #region --- 광고 ---
    /// <summary>
    /// 자동 판매 광고 보기 묻는다 
    /// </summary>
    public void Click_1HourVideoPlay()
    {
        DateTime nDate = BackendGpgsMng.GetInstance().GetNowTime();
        if (autoSaleUI.videoReDate < nDate)
        {
            PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener(LanguageGameData.GetInstance().GetString("ask.ad.video.auto.sale.1hour"), PlayVideoAutoSale1Hour);
        }
        else PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("광고 시청 대기 시간이 존재합니다.");
    }

    void PlayVideoAutoSale1Hour()
    {
#if UNITY_EDITOR
        RewardVideoAutoSale1Hour("success");
#else
          VideoAdsMng.GetInstance().AdShow(RewardVideoAutoSale1Hour);
#endif
    }

    /// <summary>
    /// 광고 시청 완료시 1시간 자동판매 
    /// </summary>
    async void RewardVideoAutoSale1Hour(string result)
    {
        if (string.Equals(result, "success"))
        {
            DateTime nDate = BackendGpgsMng.GetInstance().GetNowTime();
            var db = GameDatabase.GetInstance().tableDB.GetUserInfo();
            DateTime tryDate;
            if (DateTime.TryParse(db.m_auto_eq_sale_date, out tryDate) == false)
                tryDate = nDate;

            if (tryDate <= nDate)
                tryDate = nDate;

            int rwd_hour = GameDatabase.GetInstance().chartDB.GetDicBalance("auto.sale.add.1hour").val_int;
            db.m_auto_eq_sale_date = tryDate.AddHours(rwd_hour).ToString();
            Task tsk1 = GameDatabase.GetInstance().tableDB.SetUpdate_UserInfo(db);
            while (Loading.Bottom(tsk1.IsCompleted) == false) await Task.Delay(100);

            PlayerPrefs.SetString(PrefsKeys.key_VideoDateAutoSale, BackendGpgsMng.GetInstance().GetNowTime().AddMinutes(30).ToString());
            InitInfoAutoSale();
            ConvenienceFunctionMng.GetInstance().InitConvenienceAutoSale();

            if (GameDatabase.GetInstance().convenienceFunctionDB.GetUseingConvenFunAutoSale())
                PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox(string.Format("편의 기능 : 장비 판매/분해 <color=yellow>{0}시간</color> 이용권을 추가 완료하였습니다.", rwd_hour));
            else
                PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox(string.Format("편의 기능 : 장비 판매/분해 <color=yellow>{0}시간</color> 이용권을 구매 완료하였습니다.", rwd_hour));

            await Task.Delay(300);
            if (ConvenienceFunctionMng.GetInstance().convenFun.cfAutoSale.onOff == IG.ConvenienceFunction.OnOff.OFF)
            {
                PopUpMng.GetInstance().Open_AutoSale();
                goRoot.SetActive(false);
            }
        }
        else if (string.Equals(result, "OnAdFailedToLoad"))
            PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("광고 로드에 실패하였습니다. 잠시 후 다시 시도해주세요.");
        else
            PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("광고 플레이에 실패하였습니다. 잠시 후 다시 시도해주세요.");
    }

    ///// <summary>
    ///// 자동 판매 광고 보기 딜레이 
    ///// </summary>
    //IEnumerator Routin_Video()
    //{
    //    DateTime nDate = BackendGpgsMng.GetInstance().GetNowTime();
    //    yield return null;
    //    while (autoSaleUI.videoReDate > nDate)
    //    {
    //        nDate = BackendGpgsMng.GetInstance().GetNowTime();
    //        int totalSec = (int)(autoSaleUI.videoReDate - nDate).TotalSeconds;
    //        int hours, minute, second;

    //        totalSec = totalSec % (24 * 3600);
    //        hours = totalSec / 3600;

    //        totalSec %= 3600;
    //        minute = totalSec / 60;

    //        totalSec %= 60;
    //        second = totalSec;

    //        autoSaleUI.txVideoReDate.text = string.Format("광고 대기중 {0:00} : {1:00}", minute, second);
    //        yield return new WaitForSeconds(0.25f);
    //    }

    //    autoSaleUI.txVideoReDate.text = "광고 준비 완료";
    //}
    #endregion
}
