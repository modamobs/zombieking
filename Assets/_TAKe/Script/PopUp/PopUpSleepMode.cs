using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopUpSleepMode : MonoBehaviour
{
    [SerializeField] Text txSleep;
    [SerializeField] GameObject goSleepScreen;
    private DateTime m_onetch_date;
    [SerializeField] private int m_tchCnt = 0;

    [SerializeField] List<GameObject> disableObjectList = new List<GameObject>();
    [SerializeField] List<Canvas> disableCanvasList = new List<Canvas>();

    private int quality = 0;
    private int dmg_txt = 0;

    public void SetStart()
    {
        OnScreenSleep();
    }

    // 절전 모드로 돌입한다.최대한 이펙트 및 사향을 낮춘다.
    void OnScreenSleep()
    {
        StopCoroutine("SleepTextCount");
        StartCoroutine("SleepTextCount");
    }

    IEnumerator SleepTextCount()
    {
        m_tchCnt = 0;
        goSleepScreen.SetActive(true);
        yield return null;

        dmg_txt = GameDatabase.GetInstance().option.setting_damage_txt;
        quality = GameDatabase.GetInstance().option.setting_quality;
        GameDatabase.GetInstance().option.setting_damage_txt = 0;
        QualitySettings.SetQualityLevel(0);
        SoundManager.GetInstance().SetVolumeBGM(0);
        SoundManager.GetInstance().SetVolumeSFX(0);
        foreach (var item in disableObjectList)
            item.SetActive(false);

        foreach (var item in disableCanvasList)
            item.enabled = false;

        yield return null;
        txSleep.text = "3초 후에 화면이 완전히 꺼집니다.";
        yield return new WaitForSeconds(1f);

        txSleep.text = "2초 후에 화면이 완전히 꺼집니다.";
        yield return new WaitForSeconds(1f);

        txSleep.text = "1초 후에 화면이 완전히 꺼집니다.";
        yield return new WaitForSeconds(1f);

        m_tchCnt = 0;
        goSleepScreen.SetActive(false);
    }

    // 절전 모드를 해제한다. 
    void OffScreenSleep()
    {
        GameDatabase.GetInstance().option.setting_damage_txt = dmg_txt;
        QualitySettings.SetQualityLevel(quality);
        SoundManager.GetInstance().SetVolumeBGM(GameDatabase.GetInstance().option.setting_bgm_snd == 0 ? 1 : 0);
        SoundManager.GetInstance().SetVolumeSFX(GameDatabase.GetInstance().option.setting_sfx_snd == 0 ? 1 : 0);
        //SoundManager.GetInstance ().
        foreach (var item in disableObjectList)
            item.SetActive(true);

        foreach (var item in disableCanvasList)
            item.enabled = true;

        gameObject.SetActive(false);
    }
    
    /// <summary>
    /// 전전모드 해제 
    /// </summary>
    public void Click_SleepRelease()
    {
        m_tchCnt++;
        if (m_tchCnt == 1)
            m_onetch_date = System.DateTime.Now.AddSeconds(1.0f);

        double sGap = (m_onetch_date - System.DateTime.Now).TotalSeconds;
        if (m_tchCnt >= 1 && sGap > 1 || sGap < 0)
        {
            m_tchCnt = 0;
            m_onetch_date = System.DateTime.Now.AddSeconds(1.0f);
        }

        sGap = (m_onetch_date - System.DateTime.Now).TotalSeconds;

        // 전전모드 해제 
        if (m_tchCnt >= 3)
        {
            if (sGap >= 0 && sGap <= 1)
                OffScreenSleep();
            else m_tchCnt = 0;
        }
    }
}
