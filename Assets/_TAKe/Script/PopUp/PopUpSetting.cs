using BackEnd;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class PopUpSetting : MonoBehaviour
{
    [SerializeField] UI ui = new UI();
    [System.Serializable]
    class UI
    {
        public Notice notice;
        public Setting setting;
        public Etc etc;

        public Text txUserNick;
        public Text txGameVersion;

        [System.Serializable]
        public class Notice
        {
            public Text txServerPush;
            public Image imBgServerPush;

            public Text txLocalPush;
            public Image imBgLocalPush;

            public Text txInvenMax;
            public Image imBgInvenMax;

            public Text txEquipSendRt5, txEquipSendRt6, txEquipSendRt7;
            public Image imBgEquipSendRt5, imBgEquipSendRt6, imBgEquipSendRt7;

            public Text txEquipRecvRt5, txEquipRecvRt6, txEquipRecvRt7;
            public Image imBgEquipRecvRt5, imBgEquipRecvRt6, imBgEquipRecvRt7;
        }

        [System.Serializable]
        public class Setting
        {
            public Text txBackgSnd;
            public Image imBgBackSnd;
            public Text txEfctSnd;
            public Image imBgEfctSnd;
            public Text txEquipNoticeVib;
            public Image imBgEquipNoticeVibrt;
            public Text txDmg;
            public Image imBgDmg;
            public Text txQuality;
            public Image imBgQuality;
        }

        [System.Serializable]
        public class Etc
        {

        }
    }
    public void SetData()
    {
        ViewGameInfo();
        ViewNoticeInfo();
        ViewGameSetting();
        ViewEtc();
    }

    void Save()
    {
        PlayerPrefs.SetString(PrefsKeys.prky_OptionDb, JsonUtility.ToJson(GameDatabase.GetInstance().option));
    }

    #region # 게임 정보 #
    void ViewGameInfo()
    {
        ui.txUserNick.text = BackendGpgsMng.backendUserInfo.m_nickname;
        ui.txGameVersion.text = Application.version;
    }
    public async void Click_Notice()
    {
        BackendReturnObject bro1 = null;
        SendQueue.Enqueue(Backend.Notice.NoticeList, callback => { bro1 = callback; });
        while (Loading.Bottom(bro1) == false) { await Task.Delay(100); }

        PopUpMng.GetInstance().Open_BackendNotice(bro1.GetReturnValuetoJSON()["rows"], true);
    }
    public async void Click_Event()
    {
        BackendReturnObject bro1 = null;
        SendQueue.Enqueue(Backend.Event.EventList, callback => { bro1 = callback; });
        while (Loading.Bottom(bro1) == false) { await Task.Delay(100); }

        LogPrint.Print("이벤트 : " + bro1);
    }
    public void Click_Policy1()
    {
        //BackendReturnObject bro1 = null;
        //SendQueue.Enqueue(Backend.Notice.GetPolicy, callback => { bro1 = callback; });
        //while (bro1 == null) { await Task.Delay(100); }
        //LogPrint.Print("정책 서비스 이용약관 : " + bro1);
        
        //string url = bro1.GetReturnValuetoJSON()["termsURL"].ToString();
        
        Application.OpenURL("http://www.thebackend.io/cefb36c41ca31a636204b5a20146fa967d78aa9554a2d76d01e52ddb66b9a120/terms.html");
    }
    public void Click_Policy2()
    {
        //BackendReturnObject bro1 = null;
        //SendQueue.Enqueue(Backend.Notice.GetPolicy, callback => { bro1 = callback; });
        //while (bro1 == null) { await Task.Delay(100); }
        //LogPrint.Print("정책 개인정보 처리방침 : " + bro1);
        
        //string url = bro1.GetReturnValuetoJSON()["privacyURL"].ToString();
        
        Application.OpenURL("http://www.thebackend.io/cefb36c41ca31a636204b5a20146fa967d78aa9554a2d76d01e52ddb66b9a120/privacy.html");
    }

    public async void Click_QuestionMain()
    {
        BackendReturnObject bro1 = null;
        SendQueue.Enqueue(Backend.Question.GetQuestionAuthorize, callback => { bro1 = callback; });
        while (bro1 == null) { await Task.Delay(100); }

        LogPrint.Print("문의 : " + bro1);

        string questionAuthorize = bro1.GetReturnValuetoJSON()["authorize"].ToString();
        string myIndate = BackendGpgsMng.backendUserInfo.m_indate;
        bool isQuestionViewOpen = false;

        // margin(빈 여백)이 10인 1대1 문의창을 생성합니다.
#if UNITY_ANDROID
        isQuestionViewOpen = BackEnd.Support.Android.Question.OpenQuestionView(questionAuthorize, myIndate, 5, 60, 5, 5);
#elif UNITY_IOS
        isQuestionViewOpen = BackEnd.Support.iOS.Question.OpenQuestionView(questionAuthorize, myIndate, 5, 60, 5, 5);
#endif

        if (isQuestionViewOpen)
        {
            Debug.Log("1대1문의창이 생성되었습니다");
        }
    }
    #endregion

    #region # 알림 정보 #
    void ViewNoticeInfo()
    {
        bool isServOn = GameDatabase.GetInstance().option.notice_server_push == 0;
        ui.notice.txServerPush.text = isServOn == true ? "<color=#FFFFFF>ON</color>" : "<color=#9A9A9A>OFF</color>";

        bool isLocalOn = GameDatabase.GetInstance().option.notice_local_push == 0;
        ui.notice.txLocalPush.text = isLocalOn == true ? "<color=#FFFFFF>ON</color>" : "<color=#9A9A9A>OFF</color>";

        bool isInvenMaxOn = GameDatabase.GetInstance().option.notice_inventory_max == 0;
        ui.notice.txInvenMax.text = isInvenMaxOn == true ? "<color=#FFFFFF>ON</color>" : "<color=#9A9A9A>OFF</color>";

        bool isEquipNoticeSendRt5 = GameDatabase.GetInstance().option.notice_equip_send_rt5 == 0;
        bool isEquipNoticeSendRt6 = GameDatabase.GetInstance().option.notice_equip_send_rt6 == 0;
        bool isEquipNoticeSendRt7 = GameDatabase.GetInstance().option.notice_equip_send_rt7 == 0;
        ui.notice.txEquipSendRt5.text = isEquipNoticeSendRt5 == true ? "<color=#FF53AD>영웅 ON</color>" : "<color=#9A9A9A>영웅 OFF</color>";
        ui.notice.txEquipSendRt6.text = isEquipNoticeSendRt6 == true ? "<color=#FFA500>고대 ON</color>" : "<color=#9A9A9A>고대 OFF</color>";
        ui.notice.txEquipSendRt7.text = isEquipNoticeSendRt7 == true ? "<color=#FFE800>전설 ON</color>" : "<color=#9A9A9A>전설 OFF</color>";

        bool isEquipNoticeRecvRt5 = GameDatabase.GetInstance().option.notice_equip_recv_rt5 == 0;
        bool isEquipNoticeRecvRt6 = GameDatabase.GetInstance().option.notice_equip_recv_rt6 == 0;
        bool isEquipNoticeRecvRt7 = GameDatabase.GetInstance().option.notice_equip_recv_rt7 == 0;
        ui.notice.txEquipRecvRt5.text = isEquipNoticeRecvRt5 == true ? "<color=#FF53AD>영웅 ON</color>" : "<color=#9A9A9A>영웅 OFF</color>";
        ui.notice.txEquipRecvRt6.text = isEquipNoticeRecvRt6 == true ? "<color=#FFA500>고대 ON</color>" : "<color=#9A9A9A>고대 OFF</color>";
        ui.notice.txEquipRecvRt7.text = isEquipNoticeRecvRt7 == true ? "<color=#FFE800>전설 ON</color>" : "<color=#9A9A9A>전설 OFF</color>";

    }
    public void Click_ServerPush()
    {
        GameDatabase.GetInstance().option.notice_server_push = GameDatabase.GetInstance().option.notice_server_push == 0 ? 1 : 0;

#if UNITY_ANDROID
        bool isOn = GameDatabase.GetInstance().option.notice_server_push == 0;
        if (isOn)
        {
            Backend.Android.PutDeviceToken();
        }
        else
        {
            Backend.Android.DeleteDeviceToken();
        }
#elif UNITY_IOS

#endif

        ViewNoticeInfo();
        Save();
    }
    public void Click_LocalPush()
    {
        GameDatabase.GetInstance().option.notice_local_push = GameDatabase.GetInstance().option.notice_local_push == 0 ? 1 : 0;
#if UNITY_ANDROID
        bool isOn = GameDatabase.GetInstance().option.notice_server_push == 0;
        if (isOn)
        {
            Backend.Android.PutDeviceToken();
        }
        else
        {
            Backend.Android.DeleteDeviceToken();
        }
#elif UNITY_IOS

#endif

        ViewNoticeInfo();
        Save();
    }

    public void Click_InventoryMax()
    {
        GameDatabase.GetInstance().option.notice_inventory_max = GameDatabase.GetInstance().option.notice_inventory_max == 0 ? 1 : 0;
        ViewNoticeInfo();
        Save();
    }

    public void Click_EquipNoticeSend(int rt)
    {
        if (rt == 5)
            GameDatabase.GetInstance().option.notice_equip_send_rt5 = GameDatabase.GetInstance().option.notice_equip_send_rt5 == 0 ? 1 : 0;
        else if (rt == 6)
            GameDatabase.GetInstance().option.notice_equip_send_rt6 = GameDatabase.GetInstance().option.notice_equip_send_rt6 == 0 ? 1 : 0;
        else if (rt == 7)
            GameDatabase.GetInstance().option.notice_equip_send_rt7 = GameDatabase.GetInstance().option.notice_equip_send_rt7 == 0 ? 1 : 0;

        ViewNoticeInfo();
        Save();
    }
    public void Click_EquipNoticeRecv(int rt)
    {
        if(rt == 5)
            GameDatabase.GetInstance().option.notice_equip_recv_rt5 = GameDatabase.GetInstance().option.notice_equip_recv_rt5 == 0 ? 1 : 0;
        else if(rt == 6)
            GameDatabase.GetInstance().option.notice_equip_recv_rt6 = GameDatabase.GetInstance().option.notice_equip_recv_rt6 == 0 ? 1 : 0;
        else if(rt == 7)
            GameDatabase.GetInstance().option.notice_equip_recv_rt7 = GameDatabase.GetInstance().option.notice_equip_recv_rt7 == 0 ? 1 : 0;

        ViewNoticeInfo();
        Save();
    }
#endregion

#region # 게임 설정 #
    void ViewGameSetting()
    {
        bool isBackSnd = GameDatabase.GetInstance().option.setting_bgm_snd == 0;
        ui.setting.txBackgSnd.text = isBackSnd == true ? "<color=#FFFFFF>ON</color>" : "<color=#9A9A9A>OFF</color>";

        bool isEftSnd = GameDatabase.GetInstance().option.setting_sfx_snd == 0;
        ui.setting.txEfctSnd.text = isEftSnd == true ? "<color=#FFFFFF>ON</color>" : "<color=#9A9A9A>OFF</color>";

        bool isEquipVib = GameDatabase.GetInstance().option.setting_equip_vib == 0;
        ui.setting.txEquipNoticeVib.text = isEquipVib == true ? "<color=#FFFFFF>ON</color>" : "<color=#9A9A9A>OFF</color>";

        bool isDamage = GameDatabase.GetInstance().option.setting_damage_txt == 0;
        ui.setting.txDmg.text = isDamage == true ? "<color=#FFFFFF>ON</color>" : "<color=#9A9A9A>OFF</color>";

        bool isQuality = GameDatabase.GetInstance().option.setting_quality == 0;
        ui.setting.txQuality.text = isQuality == true ? "<color=#FFFFFF>OFF</color>" : "<color=#9A9A9A>ON</color>";
    }
    public void Click_Bgm()
    {
        GameDatabase.GetInstance().option.setting_bgm_snd = GameDatabase.GetInstance().option.setting_bgm_snd == 0 ? 1 : 0;
        ViewGameSetting();
        SoundManager.GetInstance().SetVolumeBGM(GameDatabase.GetInstance().option.setting_bgm_snd == 0 ? 1 : 0);
        Save();
    }
    public void Click_Sfx()
    {
        GameDatabase.GetInstance().option.setting_sfx_snd = GameDatabase.GetInstance().option.setting_sfx_snd == 0 ? 1 : 0;
        ViewGameSetting();
        SoundManager.GetInstance().SetVolumeSFX(GameDatabase.GetInstance().option.setting_sfx_snd == 0 ? 1 : 0);
        Save();
    }
    public void Click_Vib()
    {
        GameDatabase.GetInstance().option.setting_equip_vib = GameDatabase.GetInstance().option.setting_equip_vib == 0 ? 1 : 0;
        ViewGameSetting();
        Save();
    }
    public void Click_Damage()
    {
        GameDatabase.GetInstance().option.setting_damage_txt = GameDatabase.GetInstance().option.setting_damage_txt == 0 ? 1 : 0;
        ViewGameSetting();
        Save();
    }
    public void Click_Quality()
    {
        GameDatabase.GetInstance().option.setting_quality = GameDatabase.GetInstance().option.setting_quality == 0 ? 1 : 0;
        ViewGameSetting();

        int qLv = GameDatabase.GetInstance().option.setting_quality == 0 ? 2 : 0;
        QualitySettings.SetQualityLevel(qLv);
        Save();
    }
#endregion

#region # 기타 #
    void ViewEtc()
    {

    }

    public void Click_Cafe()
    {
        Application.OpenURL("https://cafe.naver.com/ssapgamez");
    }

    public void Click_KakaoTalk()
    {
        Application.OpenURL("https://open.kakao.com/o/gNYMOtPc");
    }

    public void Click_GoogleUrl()
    {
        Application.OpenURL("https://play.google.com/store/apps/details?id=com.two.people.zombie");
    }

    public void Click_Coupon() => PopUpMng.GetInstance().Open_Coupon();

    public void Click_Credit () => PopUpMng.GetInstance().Open_Credit();
    #endregion
}
