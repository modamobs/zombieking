using CodeStage.AntiCheat.ObscuredTypes;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class PopUpAutoSaleVideo : MonoBehaviour
{
    [SerializeField] AutoSaleUI autoSaleUI;
    [System.Serializable]
    struct AutoSaleUI
    {
        public DateTime endDate;
        public Text txLevel;
        public Text txLevelAndMaxLevel;
        public Text txNextRewardHour;
        public Text txRemmDate;

        public DateTime videoReDate;
        public Text txVideoReDate;
        public Image imVideoBtnBg;
        public Slider sdLvBar;

        // purchase.auto.sale.video.free.1hour	        1시간 구매\n광고 시청 후
        // purchase.auto.sale.video.free.1hour.delay	광고 대기중 {0:00}분{1:00}초
    }

    void Awake()
    {
        //PlayerPrefs.SetString(PrefsKeys.key_VideoDateAutoSale, "");
    }

    public void SetData()
    {
        var userinfo_db = GameDatabase.GetInstance().tableDB.GetUserInfo();
        DateTime nDate = BackendGpgsMng.GetInstance().GetNowTime();
        DateTime endDate = GameDatabase.GetInstance().convenienceFunctionDB.GetDate_ConvenFunAutoSaleVideo();
        autoSaleUI.endDate = endDate;
        if ((endDate - nDate).TotalSeconds <= 0)
        {
            userinfo_db.m_auto_eq_sale_video_lv = 0;
            GameDatabase.GetInstance().tableDB.SetUpdate_UserInfo(userinfo_db, true);
            autoSaleUI.txRemmDate.text = "자동 판매/분해 남은 시간 00:00:00";
        }
        else
        {
            StopCoroutine("Routin_AutoSale");
            StartCoroutine("Routin_AutoSale");
        }

        DateTime tryVideoDate;
        if (DateTime.TryParse(PlayerPrefs.GetString(PrefsKeys.key_VideoDateAutoSale), out tryVideoDate) == false)
            tryVideoDate = nDate;

        autoSaleUI.txLevel.text = string.Format("- 현재 광고 레벨 Lv.{0} -", userinfo_db.m_auto_eq_sale_video_lv);
        autoSaleUI.txLevelAndMaxLevel.text = string.Format("({0}/{1})", userinfo_db.m_auto_eq_sale_video_lv, 5);
        int rwd_hour = userinfo_db.m_auto_eq_sale_video_lv <= 0 ? (ObscuredInt)1 : userinfo_db.m_auto_eq_sale_video_lv;
        autoSaleUI.txNextRewardHour.text = string.Format("광고 시청 시 +{0}시간이 추가됩니다.", rwd_hour);
        autoSaleUI.videoReDate = tryVideoDate;
        autoSaleUI.imVideoBtnBg.color = autoSaleUI.videoReDate > nDate ? Color.gray : Color.white;
        autoSaleUI.sdLvBar.value = (float)(userinfo_db.m_auto_eq_sale_video_lv / 5.0f);



        StopCoroutine("Routin_Video");
        StartCoroutine("Routin_Video");
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
                //day = totalSec / (24 * 3600);

                totalSec = totalSec % (24 * 3600);
                hours = totalSec / 3600;

                totalSec %= 3600;
                minute = totalSec / 60;

                totalSec %= 60;
                second = totalSec;

                autoSaleUI.txRemmDate.text = string.Format("자동 판매/분해 남은 시간 {0:00}:{1:00}:{2:00}", hours, minute, second);
            }

            yield return new WaitForSeconds(0.5f);
        }

        autoSaleUI.txRemmDate.text = "자동 판매/분해 남은 시간 00:00:00";
        AutoSaleTimeEnd();
    }

    async void AutoSaleTimeEnd()
    {
        var userinfo_db = GameDatabase.GetInstance().tableDB.GetUserInfo();
        userinfo_db.m_auto_eq_sale_video_lv = 0;
        userinfo_db.m_auto_eq_sale_video_date = BackendGpgsMng.GetInstance().GetNowTime().ToString();
        Task<bool> a = GameDatabase.GetInstance().tableDB.SetUpdate_UserInfo(userinfo_db);
        while (Loading.Bottom(a.IsCompleted, true) == false) await Task.Delay(100);
        SetData();
    }

    /// <summary>
    /// 자동 판매 광고 보기 딜레이 
    /// </summary>
    IEnumerator Routin_Video()
    {
        DateTime nDate = BackendGpgsMng.GetInstance().GetNowTime();
        yield return null;
        while (autoSaleUI.videoReDate > nDate)
        {
            nDate = BackendGpgsMng.GetInstance().GetNowTime();
            int totalSec = (int)(autoSaleUI.videoReDate - nDate).TotalSeconds;
            int hours, minute, second;

            totalSec = totalSec % (24 * 3600);
            hours = totalSec / 3600;

            totalSec %= 3600;
            minute = totalSec / 60;

            totalSec %= 60;
            second = totalSec;

            autoSaleUI.txVideoReDate.text = string.Format("광고 대기중 {0:00} : {1:00}", minute, second);
            yield return new WaitForSeconds(0.25f);
        }

        autoSaleUI.txVideoReDate.text = "광고 시청 후 시간 추가하기";
    }

    public void Click_VideoPlay()
    {
        var userinfo_db = GameDatabase.GetInstance().tableDB.GetUserInfo();
        DateTime nDate = BackendGpgsMng.GetInstance().GetNowTime();
        if (autoSaleUI.videoReDate < nDate)
        {
            int rwd_hour = userinfo_db.m_auto_eq_sale_video_lv <= 0 ? (ObscuredInt)1: userinfo_db.m_auto_eq_sale_video_lv;
            if(userinfo_db.m_auto_eq_sale_video_lv >= 5)
            {
                PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener(
                string.Format("짧은 광고 시청 후\n장비 자동 판매/분해 (광고) <color=yellow>+{0}시간</color> 추가합니다.\n<color=red>* 최대 사용 시간은 최대 10시간을 넘길 수 없습니다. *</color>",
                rwd_hour, userinfo_db.m_auto_eq_sale_video_lv + 1), PlayVideoComplete);
            }
            else
            {
                PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener(
                string.Format("짧은 광고 시청 후\n장비 자동 판매/분해 (광고) <color=yellow>+{0}시간</color> 추가합니다.\n광고 레벨이 Lv.{1}로 상승됩니다.\n<color=red>* 최대 사용 시간은 최대 10시간을 넘길 수 없습니다. *</color>",
                rwd_hour, userinfo_db.m_auto_eq_sale_video_lv + 1), PlayVideoComplete);
            }
        }
        else PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("광고 시청 대기 시간이 존재합니다.");
    }

    void PlayVideoComplete()
    {
#if UNITY_EDITOR
        RewardVideoAutoSale1Hour("success");
#else
        VideoAdsMng.GetInstance().AdShow(RewardVideoAutoSale1Hour);
#endif
    }

    /// <summary>
    /// 광고 시청 완료시 자동 판매/분해 (광고) 시간 추가 
    /// </summary>
    async void RewardVideoAutoSale1Hour(string result)
    {
        if (string.Equals(result, "success"))
        {
            var userinfo_db = GameDatabase.GetInstance().tableDB.GetUserInfo();
            DateTime nDate = BackendGpgsMng.GetInstance().GetNowTime();
            DateTime tryDate;
            if (DateTime.TryParse(userinfo_db.m_auto_eq_sale_video_date, out tryDate) == false)
                tryDate = nDate;

            if (tryDate <= nDate)
                tryDate = nDate;

            int rwd_hour = userinfo_db.m_auto_eq_sale_video_lv <= 0 ? (ObscuredInt)1: userinfo_db.m_auto_eq_sale_video_lv;
            if ((tryDate.AddHours(rwd_hour) - nDate).TotalSeconds > 36000)
            {
                userinfo_db.m_auto_eq_sale_video_date = tryDate.AddSeconds(36000 - (tryDate - nDate).TotalSeconds).ToString();
            }
            else
            {
                userinfo_db.m_auto_eq_sale_video_date = tryDate.AddHours(rwd_hour).ToString();
            }

            bool isBefMaxLV = false;
            if (userinfo_db.m_auto_eq_sale_video_lv < 5)
                userinfo_db.m_auto_eq_sale_video_lv++;
            else isBefMaxLV = true;

            Task tsk1 = GameDatabase.GetInstance().tableDB.SetUpdate_UserInfo(userinfo_db);
            while (Loading.Bottom(tsk1.IsCompleted) == false) await Task.Delay(100);

            float nextReVideoMinute = GameDatabase.GetInstance().chartDB.GetDicBalance("auto.sale.ads.video.retime").val_int;
            PlayerPrefs.SetString(PrefsKeys.key_VideoDateAutoSale, BackendGpgsMng.GetInstance().GetNowTime().AddMinutes(nextReVideoMinute).ToString());
            SetData();
            ConvenienceFunctionMng.GetInstance().InitConvenienceAutoSale();

            if (userinfo_db.m_auto_eq_sale_video_lv >= 5 && isBefMaxLV)
            {
                PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox(string.Format("장비 자동 판매/분해 (광고) <color=yellow>+{0}시간</color> 추가 완료하였습니다.",
                    rwd_hour, userinfo_db.m_auto_eq_sale_video_lv));
            }
            else
            {
                PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox(string.Format("장비 자동 판매/분해 (광고) <color=yellow>+{0}시간</color> 추가 완료하였습니다.\n광고 레벨이 Lv.{1}로 상승하였습니다.",
                    rwd_hour, userinfo_db.m_auto_eq_sale_video_lv));
            }
            
            await Task.Delay(300);
            if (ConvenienceFunctionMng.GetInstance().convenFun.cfAutoSale.onOff == IG.ConvenienceFunction.OnOff.OFF)
            {
                PopUpMng.GetInstance().Open_AutoSale();
            }
        }
        else if (string.Equals(result, "OnAdFailedToLoad"))
            PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("광고 로드에 실패하였습니다. 잠시 후 다시 시도해주세요.");
        else
            PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("광고 플레이에 실패하였습니다. 잠시 후 다시 시도해주세요.");
    }
}
