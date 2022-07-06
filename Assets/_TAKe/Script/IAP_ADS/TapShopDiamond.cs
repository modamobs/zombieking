using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

public class TapShopDiamond : MonoBehaviour
{
    [SerializeField] VideoFreeDia videoFreeDia;
    [System.Serializable]
    struct VideoFreeDia
    {
        public Text txRewardCnt;
        public DateTime reDate;
        public Text txReDate;
    }

    [SerializeField] DailyProduct product7DayDiamond;
    [SerializeField] DailyProduct product30DayDiamond;
    [System.Serializable]
    struct DailyProduct
    {
        public GameObject goBtnBuy;
        public GameObject goBtnReward;
        public GameObject goBtnRewardCmp;
        public Text txRemmDay;

        public GameObject[] goRewardIcon;
        public Text[] txRewardCnt;
    }


    // TBC (화이트 다이아) 충전 
    public void BuyTBC(IAPButton ib)
    {
        LogPrint.Print("<color=yellow>----BuyTestTBC-----</color>Buy productId : " + ib.productId);
        IAPMng.GetInstance().ABuyTBC(ib.productId);
    }

    string str_remaining_day = "";
    int day7diaRwdCnt, day30diaRwdCnt;
    void Awake()
    {
        str_remaining_day = LanguageGameData.GetInstance().GetString("text.remaining.day");
        videoFreeDia.txRewardCnt.text = string.Format("{0}~{1}", GameDatabase.GetInstance().chartDB.GetDicBalance("video.free.reward.min").val_int, GameDatabase.GetInstance().chartDB.GetDicBalance("video.free.reward.max").val_int - 1);
        day7diaRwdCnt = GameDatabase.GetInstance().chartDB.GetDicBalance("day7.daily.dia.reward.cnt").val_int;
        day30diaRwdCnt = GameDatabase.GetInstance().chartDB.GetDicBalance("day30.daily.dia.reward.cnt").val_int;
    }

    void OnEnable()
    {
        SetData();
    }

    public void SetData()
    {
        DayProduct();
        VideoFreeDiaReward();
    }

    /// <summary>
    /// 기간제 상품 체크 
    /// </summary>
    public void DayProduct()
    {
        var goods_db = GameDatabase.GetInstance().tableDB.GetTableDB_Goods();

        // 7day diamond (tbc)
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
            int toaRwdDia = (int)(day7diaRwdCnt * reward7DayCnt);
            int remm7DayCnt = is7Deadline == true ? (int)(m7day_end_ymd - try7DayDailyRewardDate).Days : 0; // 남은 기간 
            if (remm7DayCnt < 0)
                remm7DayCnt = 0;
            
            product7DayDiamond.txRemmDay.text = string.Format("{0} : {1}Day", str_remaining_day, (remm7DayCnt + 1).ToString());
            product7DayDiamond.goBtnBuy.SetActive(false); // 구매 버튼 ON or OFF 
            product7DayDiamond.goBtnReward.SetActive(is7DailyReward); // 보상 받기 버튼 
            product7DayDiamond.goBtnRewardCmp.SetActive(!is7DailyReward); // 보상 완료 image 

            product7DayDiamond.goRewardIcon[0].SetActive(false);
            product7DayDiamond.txRewardCnt[1].text =
                  string.Format("x{0}{1}", is7DailyReward == true ? toaRwdDia : day7diaRwdCnt,
                  is7DailyReward == true ? string.Format("<color=yellow>({0})</color>", reward7DayCnt.ToString()) : "");
        }
        else
        {
            product7DayDiamond.txRemmDay.text = "";
            product7DayDiamond.goBtnBuy.SetActive(is7Deadline == false); // 구매 버튼 ON or OFF 
            product7DayDiamond.goBtnReward.SetActive(false); // 보상 받기 버튼 
            product7DayDiamond.goBtnRewardCmp.SetActive(is7Deadline == true); // 보상 완료 image 

            foreach (var item in product7DayDiamond.goRewardIcon)
                item.SetActive(true);
            product7DayDiamond.txRewardCnt[0].text = string.Format("+{0}", GameDatabase.GetInstance().chartDB.GetDicBalance("day7.daily.dia.one.reward.cnt").val_int);
            product7DayDiamond.txRewardCnt[1].text = string.Format("x{0}", day7diaRwdCnt);
        }

        // 30day diamond (tbc)
        // 30일 매일 다이아 구매 보상 받을수 있는 날인지 체크 
        DateTime try30DayStartDate; // 시작 날짜 
        DateTime try30DayEndDate; // 종료 날짜 
        DateTime try30DayDailyRewardDate; // 마지막으로 보상 받은 날짜 

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
        if (is30DailyReward == true || is30DayDeadline == true)
        {
            int toaRwdDia = (int)(day30diaRwdCnt * reward30DayCnt);
            int remm30DayCnt = is30DayDeadline == true ? (int)(m30day_end_ymd - try30DayDailyRewardDate).Days : 0; // 남은 기간 
            if (remm30DayCnt < 0)
                remm30DayCnt = 0;
            
            product30DayDiamond.txRemmDay.text = string.Format("{0} : {1}Day", str_remaining_day, (remm30DayCnt + 1).ToString());
            product30DayDiamond.goBtnBuy.SetActive(false); // 구매 버튼 ON or OFF 
            product30DayDiamond.goBtnReward.SetActive(is30DailyReward); // 보상 받기 버튼 
            product30DayDiamond.goBtnRewardCmp.SetActive(!is30DailyReward); // 보상 완료 image 

            product30DayDiamond.goRewardIcon[0].SetActive(false);
            product30DayDiamond.txRewardCnt[1].text =
                   string.Format("x{0}{1}", is30DailyReward == true ? toaRwdDia : day30diaRwdCnt,
                   is30DailyReward == true ? string.Format("<color=yellow>({0})</color>", reward30DayCnt.ToString()) : "");
        }
        else
        {
            product30DayDiamond.txRemmDay.text = "";
            product30DayDiamond.goBtnBuy.SetActive(is30DayDeadline == false); // 구매 버튼 ON or OFF 
            product30DayDiamond.goBtnReward.SetActive(false); // 보상 받기 버튼 
            product30DayDiamond.goBtnRewardCmp.SetActive(is30DayDeadline == true); // 보상 완료 image

            foreach (var item in product30DayDiamond.goRewardIcon)
                item.SetActive(true);
            product30DayDiamond.txRewardCnt[0].text = string.Format("+{0}", GameDatabase.GetInstance().chartDB.GetDicBalance("day30.daily.dia.one.reward.cnt").val_int);
            product30DayDiamond.txRewardCnt[1].text = string.Format("x{0}", day30diaRwdCnt);
        }
    }

    void VideoFreeDiaReward()
    {
        string sFreeDate = PlayerPrefs.GetString(PrefsKeys.key_VideoFreeDate);
        DateTime nDate = BackendGpgsMng.GetInstance().GetNowTime();
        DateTime tryDate;
        bool isDelay = true;
        if (DateTime.TryParse(sFreeDate, out tryDate) == false)
        {
            tryDate = nDate;
            isDelay = false;
        }

        videoFreeDia.reDate = tryDate;

        if (isDelay)
        {
            StopCoroutine("Routin_DateLoop");
            StartCoroutine("Routin_DateLoop");
        }
        else
        {
            videoFreeDia.txReDate.text = "";
        }
    }

    IEnumerator Routin_DateLoop()
    {
        DateTime nDate = BackendGpgsMng.GetInstance().GetNowTime();
        yield return null;
        while (videoFreeDia.reDate > nDate)
        {
            nDate = BackendGpgsMng.GetInstance().GetNowTime();
            int totalSec = (int)(videoFreeDia.reDate - nDate).TotalSeconds;
            int hours, minute, second;

            totalSec = totalSec % (24 * 3600);
            hours = totalSec / 3600;

            totalSec %= 3600;
            minute = totalSec / 60;

            totalSec %= 60;
            second = totalSec;

            videoFreeDia.txReDate.text = string.Format("{0:00} : {1:00} : {2:00}", hours, minute, second);
            yield return new WaitForSeconds(0.25f);
        }

        videoFreeDia.txReDate.text = "";
    }


    // 광고 보기 
    public async void Click_PlayVideoPopUpOn()
    {
        if (videoFreeDia.reDate < await BackendGpgsMng.GetInstance().GetBackendDateTime())
        {
            if(GameDatabase.GetInstance ().tableDB.GetUserInfo().GetAdRemoval() == true)
            {
                APlayVideoResultRewardDia10("success");
            }
            else
            {
                string strNorice = LanguageGameData.GetInstance().GetString("ask.ad.video.blue.dia10").ToString(); // 짧은 광고 시청 후 다이아 10~25개를 랜덤으로 획득합니다.
                PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener(strNorice, PlayVideoRewardBlueDiamond10);
            }
        }
        else
        {
            PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("광고 대기 시간이 있습니다. 잠시후에 다시 시도해주세요.");
        }
    }

    /// <summary>
    /// 광고 플레이 하고 정상 완료후 리턴 이벤트로 보상 지급받을 함수를 태워 보냄 
    /// </summary>
    public void PlayVideoRewardBlueDiamond10()
    {
        if (BackendGpgsMng.isEditor)
        {
            APlayVideoResultRewardDia10("success");
        }
        else
        {
            VideoAdsMng.GetInstance().AdShow(APlayVideoResultRewardDia10);
        }
    }

    /// <summary>
    /// 광고 완료 보상
    /// 다이아 10 
    /// </summary>
    private async void APlayVideoResultRewardDia10(string result)
    {
        LogPrint.Print("APlayVideoResultRewardDia10 result : " + result);
        if (string.Equals(result, "success"))
        {
            var db = GameDatabase.GetInstance().tableDB.GetTableDB_Goods();
            int rnd_rwd_dia = UnityEngine.Random.Range(GameDatabase.GetInstance().chartDB.GetDicBalance("video.free.reward.min").val_int, GameDatabase.GetInstance().chartDB.GetDicBalance("video.free.reward.max").val_int);
            db.m_dia += rnd_rwd_dia;

            Task tsk1 = GameDatabase.GetInstance().tableDB.SetUpdateGoods(db);
            while (Loading.Bottom(tsk1.IsCompleted) == false) await Task.Delay(100);

            PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox(string.Format("광고 시청 보상으로 다이아 x{0}개를 획득하였습니다", rnd_rwd_dia));
            videoFreeDia.reDate = BackendGpgsMng.GetInstance().GetNowTime().AddMinutes(30);
            PlayerPrefs.SetString(PrefsKeys.key_VideoFreeDate, videoFreeDia.reDate.ToString());
            StopCoroutine("Routin_DateLoop");
            StartCoroutine("Routin_DateLoop");
        }
        else if (string.Equals(result, "OnAdFailedToLoad"))
            PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("광고 로드에 실패하였습니다. 잠시 후 다시 시도해주세요.");
        else
            PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("광고 플레이에 실패하였습니다. 잠시 후 다시 시도해주세요.");
    }
}
