using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class VideoAdsMng : MonoSingleton<VideoAdsMng>
{
    public delegate void ResultAdsReward (string _message);
    public event ResultAdsReward resultAdsReward; // 광고 시청 후 리턴 이벤트 

    [SerializeField] VideoAdmobAds videoAdmobAds;
    [SerializeField] VideoUnityAds videoUnityAds;

   public async void AdShow(ResultAdsReward rar)
    {
        resultAdsReward = null;
        resultAdsReward = rar; // 보상 리턴 이벤트 함수 

        bool admobAd = await videoAdmobAds.ShowRewardAd(); // 구글애드몹 광고 요청 후 시작 
        if (admobAd == false) // 구글 광고 플레이 실패 
        {
            // 유니티 ads 실행
            bool unityAd = await videoUnityAds.ShowRewardAd();
            if (unityAd == false) // 유니티 광고 플레이 실패 
            {
                PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("광고 물량이 부족합니다. 잠시후에 다시 시도해주세요.");
            }
            else
            {
                LogPrint.Print("<color=yellow>AdShow 유니티 광고 플레이 성공</color>");
            }
        } 
        else
        {
            LogPrint.Print("<color=yellow>AdShow 구글 애드몹 광고 플레이 성공</color>");
        }
    }

    public void AdResultReward(string result)
    {
        LogPrint.Print("AdResultReward result : " + result);
        resultAdsReward(result);
    }
}
