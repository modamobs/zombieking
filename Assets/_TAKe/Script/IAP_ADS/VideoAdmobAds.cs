using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using GoogleMobileAds.Api;
using System.Threading.Tasks;

public class VideoAdmobAds : MonoBehaviour
{
    static bool isAdVideoLoaded = false;

    private RewardedAd videoAd;
    public static bool ShowAd = false;
    string videoID;
    public void Start()
    {
        // 광고 ID
        // 테스트 : ca-app-pub-3940256099942544/5224354917
        // 실제   : ca-app-pub-8505309295573745/2477800045
        videoID = "ca-app-pub-8505309295573745/2477800045"; // 광고 단위 ID ca-app-pub-숫자 / 숫자 형식

        videoAd = new RewardedAd(videoID);
        Handle(videoAd);
        Load();
    }

    private void Handle(RewardedAd videoAd)
    {
        videoAd.OnAdLoaded += HandleOnAdLoaded;
        videoAd.OnAdFailedToLoad += HandleOnAdFailedToLoad;
        videoAd.OnAdFailedToShow += HandleOnAdFailedToShow;
        videoAd.OnAdOpening += HandleOnAdOpening;
        videoAd.OnAdClosed += HandleOnAdClosed;
        videoAd.OnUserEarnedReward += HandleOnUserEarnedReward;
    }

    private void Load()
    {
        AdRequest request = new AdRequest.Builder().Build();
        videoAd.LoadAd(request);
    }

    public RewardedAd ReloadAd()
    {
        RewardedAd videoAd = new RewardedAd(videoID);
        Handle(videoAd);
        AdRequest request = new AdRequest.Builder().Build();
        videoAd.LoadAd(request);
        return videoAd;
    }

    /// <summary>
    /// 광고 물량 체크 후 실행 
    /// </summary>
    public async Task<bool> ShowRewardAd()
    {
        bool isNoAd = false;
        DateTime loadMaxDate = BackendGpgsMng.GetInstance().GetNowTime().AddSeconds(2.0f);
        while (!videoAd.IsLoaded() && !isNoAd)
        {
            await Task.Delay(100);
            isNoAd = (loadMaxDate < BackendGpgsMng.GetInstance().GetNowTime());
        }

        if(!isNoAd)
        {
            videoAd.Show();
            return true;
        }

        return false;
    }

    //광고가 로드되었을 때
    public void HandleOnAdLoaded(object sender, EventArgs args)
    {

    }
    //광고 로드에 실패했을 때
    public void HandleOnAdFailedToLoad(object sender, AdErrorEventArgs args)
    {
        VideoAdsMng.GetInstance().AdResultReward("OnAdFailedToLoad");
    }
    //광고 보여주기를 실패했을 때
    public void HandleOnAdFailedToShow(object sender, AdErrorEventArgs args)
    {
        VideoAdsMng.GetInstance().AdResultReward("OnAdFailedToShow");
    }
    //광고가 제대로 실행되었을 때
    public void HandleOnAdOpening(object sender, EventArgs args)
    {

    }
    //광고가 종료되었을 때
    public void HandleOnAdClosed(object sender, EventArgs args)
    {
        //새로운 광고 Load
        this.videoAd = ReloadAd();
    }
    //광고를 끝까지 시청하였을 때
    public void HandleOnUserEarnedReward(object sender, EventArgs args)
    {
        LogPrint.Print("sender : " + sender.ToString() + ", args : " + args.ToString()  + ", args.GetType : " + args.GetType());
        //보상 지급 
        VideoAdsMng.GetInstance().AdResultReward("success");
    }
}