using CodeStage.AntiCheat.ObscuredTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class TapShopPackageEtc : MonoBehaviour
{
    void OnEnable()
    {
        AdsRemoval();
        RubyEtherAds();
        EquipEnhantPack();
        AcceEnhantPack();
        RubyGold();
    }

    #region ----- 광고 제거 상품 -----
    [SerializeField] UIAdsRemoval uIAdsRemoval;
    [System.Serializable]
    struct UIAdsRemoval
    {
        public GameObject goBtnPurchase, goBtnCompletePurchase;
    }

    public void AdsRemoval()
    {
        uIAdsRemoval.goBtnPurchase.SetActive(!GameDatabase.GetInstance().tableDB.GetUserInfo().GetAdRemoval());
        uIAdsRemoval.goBtnCompletePurchase.SetActive(GameDatabase.GetInstance().tableDB.GetUserInfo().GetAdRemoval());
    }
    #endregion

    #region ----- 루비 / 에테르 광고 시청 -----
    [SerializeField] UIRubyEtherAds uIRubyEtherAds;
    [System.Serializable]
    struct UIRubyEtherAds
    {
        public string adRewardName;
        public DateTime rubyDT, etherDT;
        public Text txRubyTime, txEtherTime;
    }

    void RubyEtherAds()
    {
        var userinfo_DB = GameDatabase.GetInstance().tableDB.GetUserInfo();
        DateTime nDT = BackendGpgsMng.GetInstance().GetNowTime();
        if (DateTime.TryParse(userinfo_DB.m_ad_ruby_date, out uIRubyEtherAds.rubyDT) == false)
            uIRubyEtherAds.rubyDT = nDT;
        if (DateTime.TryParse(userinfo_DB.m_ad_ether_date, out uIRubyEtherAds.etherDT) == false)
            uIRubyEtherAds.etherDT = nDT;

        StopCoroutine("Routin_AdsRuby");
        StartCoroutine("Routin_AdsRuby");

        StopCoroutine("Routin_AdsEther");
        StartCoroutine("Routin_AdsEther");
    }

    IEnumerator Routin_AdsRuby()
    {
        int min = GameDatabase.GetInstance().chartDB.GetDicBalance("ads.reward.ruby.min").val_int;
        int max = GameDatabase.GetInstance().chartDB.GetDicBalance("ads.reward.ruby.max").val_int;
        DateTime nBT = BackendGpgsMng.GetInstance().GetNowTime();
        yield return null;
        while ((uIRubyEtherAds.rubyDT - nBT).TotalSeconds > 0)
        {
            nBT = BackendGpgsMng.GetInstance().GetNowTime();
            int totalSec = (int)(uIRubyEtherAds.rubyDT - nBT).TotalSeconds;
            int hours, minute, second;

            totalSec = totalSec % (24 * 3600);
            hours = totalSec / 3600;

            totalSec %= 3600;
            minute = totalSec / 60;

            totalSec %= 60;
            second = totalSec;

            uIRubyEtherAds.txRubyTime.text = string.Format("x{0} ~ x{1}\n<color=#C1C1C1>광고 대기 {2:00}:{3:00}:{4:00}</color>", min, max, hours, minute, second);
            yield return new WaitForSeconds(0.25f);
        }

        uIRubyEtherAds.txRubyTime.text = string.Format("x{0}~x{1}\n<color=#00DAFF>광고 시청 가능</color>", min, max);
    }
    IEnumerator Routin_AdsEther()
    {
        int min = GameDatabase.GetInstance().chartDB.GetDicBalance("ads.reward.ether.min").val_int;
        int max = GameDatabase.GetInstance().chartDB.GetDicBalance("ads.reward.ether.max").val_int;
        DateTime nDate = BackendGpgsMng.GetInstance().GetNowTime();
        yield return null;
        while ((uIRubyEtherAds.etherDT - nDate).TotalSeconds > 0)
        {
            nDate = BackendGpgsMng.GetInstance().GetNowTime();
            int totalSec = (int)(uIRubyEtherAds.etherDT - nDate).TotalSeconds;
            int hours, minute, second;

            totalSec = totalSec % (24 * 3600);
            hours = totalSec / 3600;

            totalSec %= 3600;
            minute = totalSec / 60;

            totalSec %= 60;
            second = totalSec;

            uIRubyEtherAds.txEtherTime.text = string.Format("x{0} ~ x{1}\n<color=#C1C1C1>광고 대기 {2:00}:{3:00}:{4:00}</color>", min, max, hours, minute, second);
            yield return new WaitForSeconds(0.25f);
        }

        uIRubyEtherAds.txEtherTime.text = string.Format("x{0}~x{1}\n<color=#00DAFF>광고 시청 가능</color>", min, max);
    }

    public async void Click_AdsRubyEther(string adName)
    {
        DateTime nDT = await BackendGpgsMng.GetInstance().GetBackendDateTime();
        DateTime dt = string.Equals(adName, "ruby") ? uIRubyEtherAds.rubyDT : string.Equals(adName, "ether") ? uIRubyEtherAds.etherDT : nDT;
        if (dt < nDT)
        {
            uIRubyEtherAds.adRewardName = adName;
            PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener("짧은 광고 시청 후 보상이 지급됩니다.", PlayAdsShow);
        }
        else PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("광고 대기 시간이 있습니다. 잠시후에 다시 시도해주세요.");
    }

    void PlayAdsShow()
    {
        if (GameDatabase.GetInstance().tableDB.GetUserInfo().GetAdRemoval() == true || BackendGpgsMng.isEditor)
        {
            if (string.Equals(uIRubyEtherAds.adRewardName, "ruby"))
                AdRewardRuby("success");
            else if (string.Equals(uIRubyEtherAds.adRewardName, "ether"))
                AdRewardEther("success");
        }
        else
        {
            if (string.Equals(uIRubyEtherAds.adRewardName, "ruby"))
            {
                VideoAdsMng.GetInstance().AdShow(AdRewardRuby);
            }
            else if (string.Equals(uIRubyEtherAds.adRewardName, "ether"))
            {
                VideoAdsMng.GetInstance().AdShow(AdRewardEther);
            }
        }
    }

    // 광고 루비 보상 
    private async void AdRewardRuby(string result)
    {
        if (string.Equals(result, "success"))
        {
            int rwd_Cnt = UnityEngine.Random.Range(GameDatabase.GetInstance().chartDB.GetDicBalance("ads.reward.ruby.min").val_int, GameDatabase.GetInstance().chartDB.GetDicBalance("ads.reward.ruby.max").val_int + 1);
            GameDatabase.GetInstance().tableDB.SetUpdateGoods("ruby", rwd_Cnt, "+");
            var userinfo_DB = GameDatabase.GetInstance().tableDB.GetUserInfo();
            Task<DateTime> resultDT = BackendGpgsMng.GetInstance().GetBackendDateTime();
            await resultDT;
            userinfo_DB.m_ad_ruby_date = resultDT.Result.AddMinutes(GameDatabase.GetInstance().chartDB.GetDicBalance("ads.reward.ruby.re.minutes").val_int).ToString();
            Task tsk1 = GameDatabase.GetInstance().tableDB.SetUpdate_UserInfo(userinfo_DB);
            while (Loading.Full(tsk1.IsCompleted) == false) await Task.Delay(100);
            PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox(string.Format("광고 시청 보상으로 루비 x{0}개를 획득하였습니다.", rwd_Cnt));
            RubyEtherAds();
        }
        else if (string.Equals(result, "OnAdFailedToLoad"))
            PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("광고 로드에 실패하였습니다. 잠시 후 다시 시도해주세요.");
        else
            PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("광고 플레이에 실패하였습니다. 잠시 후 다시 시도해주세요.");
    }

    // 광고 에테르 보상 
    private async void AdRewardEther(string result)
    {
        if (string.Equals(result, "success"))
        {
            int rwd_Cnt = UnityEngine.Random.Range(GameDatabase.GetInstance().chartDB.GetDicBalance("ads.reward.ether.min").val_int, GameDatabase.GetInstance().chartDB.GetDicBalance("ads.reward.ether.max").val_int + 1);
            GameDatabase.GetInstance().tableDB.SetUpdateGoods("ether", rwd_Cnt, "+");
            var userinfo_DB = GameDatabase.GetInstance().tableDB.GetUserInfo();
            Task<DateTime> resultDT = BackendGpgsMng.GetInstance().GetBackendDateTime();
            await resultDT;
            userinfo_DB.m_ad_ether_date = resultDT.Result.AddMinutes(GameDatabase.GetInstance().chartDB.GetDicBalance("ads.reward.ether.re.minutes").val_int).ToString();
            Task tsk1 = GameDatabase.GetInstance().tableDB.SetUpdate_UserInfo(userinfo_DB);
            while (Loading.Full(tsk1.IsCompleted) == false) await Task.Delay(100);
            PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox(string.Format("광고 시청 보상으로 에테르 x{0}개를 획득하였습니다.", rwd_Cnt));
            RubyEtherAds();
        }
        else if (string.Equals(result, "OnAdFailedToLoad"))
            PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("광고 로드에 실패하였습니다. 잠시 후 다시 시도해주세요.");
        else
            PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("광고 플레이에 실패하였습니다. 잠시 후 다시 시도해주세요.");
    }
    #endregion

    #region ----- 장비/장신구 강화 패키지 상품 -----
    [SerializeField] UIEnhantPack uIEquipEnhantPack;
    [SerializeField] UIEnhantPack uIAcceEnhantPack;
    [System.Serializable]
    struct UIEnhantPack
    {
        public Text txRubyOrEtherCnt, txRt1StonCnt, txRt2StonCnt, txRt3StonCnt;
    }

    void EquipEnhantPack()
    {
        int rwd_ruby = GameDatabase.GetInstance().chartDB.GetDicBalance("shop.eq.enhant.pack.ruby").val_int;
        int rwd_rt1_eq_ston = GameDatabase.GetInstance().chartDB.GetDicBalance("shop.eq.enhant.pack.rt1.ston").val_int;
        int rwd_rt2_eq_ston = GameDatabase.GetInstance().chartDB.GetDicBalance("shop.eq.enhant.pack.rt2.ston").val_int;
        int rwd_rt3_eq_ston = GameDatabase.GetInstance().chartDB.GetDicBalance("shop.eq.enhant.pack.rt3.ston").val_int;
        uIEquipEnhantPack.txRubyOrEtherCnt.text = string.Format("x{0}", rwd_ruby);
        uIEquipEnhantPack.txRt1StonCnt.text = string.Format("x{0}", rwd_rt1_eq_ston);
        uIEquipEnhantPack.txRt2StonCnt.text = string.Format("x{0}", rwd_rt2_eq_ston);
        uIEquipEnhantPack.txRt3StonCnt.text = string.Format("x{0}", rwd_rt3_eq_ston);
    }

    void AcceEnhantPack()
    {
        int rwd_ruby = GameDatabase.GetInstance().chartDB.GetDicBalance("shop.ac.enhant.pack.ether").val_int;
        int rwd_rt1_ac_ston = GameDatabase.GetInstance().chartDB.GetDicBalance("shop.ac.enhant.pack.rt1.ston").val_int;
        int rwd_rt2_ac_ston = GameDatabase.GetInstance().chartDB.GetDicBalance("shop.ac.enhant.pack.rt2.ston").val_int;
        int rwd_rt3_ac_ston = GameDatabase.GetInstance().chartDB.GetDicBalance("shop.ac.enhant.pack.rt3.ston").val_int;
        uIAcceEnhantPack.txRubyOrEtherCnt.text = string.Format("x{0}", rwd_ruby);
        uIAcceEnhantPack.txRt1StonCnt.text = string.Format("x{0}", rwd_rt1_ac_ston);
        uIAcceEnhantPack.txRt2StonCnt.text = string.Format("x{0}", rwd_rt2_ac_ston);
        uIAcceEnhantPack.txRt3StonCnt.text = string.Format("x{0}", rwd_rt3_ac_ston);
    }
    #endregion

    #region ----- 루비+골드 / 에테르+골드 상품 -----
    [SerializeField] UIRuybEtherGold uIRubyEtherbGold;
    [System.Serializable]
    struct UIRuybEtherGold
    {
        public Text txRubyCnt, txRubyGoldCnt, txEtherCnt, txEtherGoldCnt;
    }

    void RubyGold()
    {
        uIRubyEtherbGold.txRubyCnt.text = string.Format("{0:0,#}", GameDatabase.GetInstance().chartDB.GetDicBalance("shop.ruby.gold.pack1").val_int);
        uIRubyEtherbGold.txRubyGoldCnt.text = string.Format("{0:0,#}억", GameDatabase.GetInstance().chartDB.GetDicBalance("shop.ruby.gold.pack2").val_long / 100000000);
        uIRubyEtherbGold.txEtherCnt.text = string.Format("{0:0,#}", GameDatabase.GetInstance().chartDB.GetDicBalance("shop.ether.gold.pack1").val_int);
        uIRubyEtherbGold.txEtherGoldCnt.text = string.Format("{0:0,#}억", GameDatabase.GetInstance().chartDB.GetDicBalance("shop.ether.gold.pack2").val_long / 100000000);
    }
    #endregion
}
