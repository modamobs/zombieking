using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class VideoUnityAds : MonoBehaviour
{
    /// <summary>
    /// 광고 물량 체크 후 실행 
    /// </summary>
    public async Task<bool> ShowRewardAd()
    {
        //bool isNoAd = false;
        //DateTime loadMaxDate = BackendGpgsMng.GetInstance().GetNowTime().AddSeconds(2.0f);
        //while (!videoAd.IsLoaded() && !isNoAd)
        //{
        //    await Task.Delay(500);
        //    isNoAd = (loadMaxDate < BackendGpgsMng.GetInstance().GetNowTime());
        //}

        //if (!isNoAd)
        //{
        //    videoAd.Show();
        //    return true;
        //}

        return false;
    }
}
