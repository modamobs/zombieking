using BackEnd;
using CodeStage.AntiCheat.ObscuredTypes;
using Firebase.Messaging;
using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static GameDatabase;

public class TapShopLuck : MonoBehaviour
{
    [SerializeField] UI ui;
    [System.Serializable]
    class UI
    {
        public Text txEtherCnt;
        public Text txRubyCnt;
    }

    [SerializeField] UIFreeDiaSoHwan uiFreeSoHwan;
    [System.Serializable]
    struct UIFreeDiaSoHwan
    {
        // 장신구 광고 버튼 
        public Text txAcceVideoReDate;
        public DateTime dtAcceVideoReDate;

        // 무료 장신구 소환 버튼 
        public Image imAcceFreeBtnBg;
        public Text txAcceFreeReDate;
        public DateTime dtAcceFreeReDate;

        // 장비 광고 버튼 
        public Text txEquipVideoReDate;
        public DateTime dtEquipVideoReDate;

        // 무료 장비 소환 버튼 
        public Image imEquipFreeBtnBg;
        public Text txEquipFreeReDate;
        public DateTime dtEquipFreeReDate;
    }

    [SerializeField]
    List<AcceResult> soHwanResults = new List<AcceResult>();


    [SerializeField] Text txBDia11CntBonusEquipPiece; // 장비 11회 뽑기 보너스 장비 조각  
    [SerializeField] Text txBDia11CntBonusRuby; // 장비 11회 뽑기 보너스 루비 

    [SerializeField] Text txRDia11CntBonusEther; // 장신구 11회 뽑기 보너스 에테르 
    [SerializeField] Text txRDia11CntBonusAccePiece; // 장신구 11회 뽑기 보너스 장신구 조각

    Sprite spBtnAcceFreeSohwanOk, spBtnEquipFreeSohwanOk, spBtnGray;
    string free_sohwan;
    string ad_video_minute_reduce;

    void Awake()
    {
        //PlayerPrefs.DeleteKey (PrefsKeys.key_VideoDateEquipFreeSoHwan);
        //PlayerPrefs.DeleteKey(PrefsKeys.key_VideoDateAcceFreeSoHwan);

        spBtnAcceFreeSohwanOk = SpriteAtlasMng.GetInstance().GetSpriteRatingBg(6);
        spBtnEquipFreeSohwanOk = SpriteAtlasMng.GetInstance().GetSpriteRatingBg(7);
        spBtnGray = SpriteAtlasMng.GetInstance().GetSpriteButtonRedOrGray(false);
        free_sohwan = LanguageGameData.GetInstance().GetString("shop.luck.free.sohwan"); // 무료 소환 
        ad_video_minute_reduce = LanguageGameData.GetInstance().GetString("ad.video.minute.reduce"); // 광고 후 {0}분 단축 

        txBDia11CntBonusEquipPiece.text = string.Format("보너스 2. <color=#FFE800>전설 장비 조각</color> x{0}개 지급", GameDatabase.GetInstance().chartDB.GetDicBalance("shop.luck.gacha.equip.dia.price.rt3to7cnt11.bonus.equip.piece.rt7").val_int);
        txBDia11CntBonusRuby.text = string.Format("보너스 3. <color=#FF0000>루비</color> x{0}개 지급", GameDatabase.GetInstance().chartDB.GetDicBalance("shop.luck.gacha.acce.tbc.key.rt3to6cnt11.bonus.ether").val_int);

        txRDia11CntBonusAccePiece.text = string.Format("보너스 2. <color=#FF53AD>영웅 장신구 조각</color> x{0}개 지급", GameDatabase.GetInstance().chartDB.GetDicBalance("shop.luck.gacha.acce.tbc.key.rt3to6cnt11.bonus.acce.piece.rt5").val_int);
        txRDia11CntBonusEther.text = string.Format("보너스 3. <color=#00FF00>에테르</color> x{0}개 지급", GameDatabase.GetInstance().chartDB.GetDicBalance("shop.luck.gacha.equip.dia.price.rt3to7cnt11.bonus.ruby").val_int);
    }

    void OnEnable()
    {
        soHwanResults.Clear();
        SetData();
    }

    public void SetData()
    {
        InitFreeSohwan();
        InitPetSohwan();
        GoodsView();
    }

    public void GoodsView()
    {
        var goods_db = GameDatabase.GetInstance().tableDB.GetTableDB_Goods();
        ui.txEtherCnt.text = string.Format("x{0:#,0}", goods_db.m_ether.ToString());
        ui.txRubyCnt.text = string.Format("x{0:#,0}", goods_db.m_ruby.ToString());
    }

    //-------------------------------------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------------------------------
    #region 확률 보기 
    public void Click_GachaPercentage(string gach_name)
    {
        PopUpMng.GetInstance().ClickOpen_GachaPercentage(gach_name);
    }
    #endregion

    //-------------------------------------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------------------------------
    #region 무료 소환
    void InitFreeSohwan()
    {
        var userInfo_db = GameDatabase.GetInstance().tableDB.GetUserInfo();
        DateTime nDate = BackendGpgsMng.GetInstance().GetNowTime();

        // 무료 장신구 소환하기 
        DateTime tryAcceFreeSohwanDate;
        if (DateTime.TryParse(userInfo_db.m_free_acce_sohwan, out tryAcceFreeSohwanDate) == false)
            tryAcceFreeSohwanDate = nDate;

        // 무료 장비 소환하기 
        DateTime tryEquipFreeSohwanDate;
        if (DateTime.TryParse(userInfo_db.m_free_equip_sohwan, out tryEquipFreeSohwanDate) == false)
            tryEquipFreeSohwanDate = nDate;

        uiFreeSoHwan.dtAcceFreeReDate = tryAcceFreeSohwanDate;
        uiFreeSoHwan.dtEquipFreeReDate = tryEquipFreeSohwanDate;

        StopCoroutine("VideoReDateLoopAcce");
        StartCoroutine("VideoReDateLoopAcce");
        StopCoroutine("VideoReDateLoopEquip");
        StartCoroutine("VideoReDateLoopEquip");

        // 무료 장신구 소환 시간 단축 광고 보기 
        DateTime tryAcceVdSohwanDate;
        if (DateTime.TryParse(PlayerPrefs.GetString(PrefsKeys.key_VideoDateAcceFreeSoHwan), out tryAcceVdSohwanDate) == false)
            tryAcceVdSohwanDate = nDate;
        uiFreeSoHwan.dtAcceVideoReDate = tryAcceVdSohwanDate;

        // 무료 장비 소환 시간 단축 광고 보기 
        DateTime tryEquipVdSohwanDate;
        if (DateTime.TryParse(PlayerPrefs.GetString(PrefsKeys.key_VideoDateEquipFreeSoHwan), out tryEquipVdSohwanDate) == false)
            tryEquipVdSohwanDate = nDate;
        uiFreeSoHwan.dtEquipVideoReDate = tryEquipVdSohwanDate;

        StopCoroutine("FreeSohwanTimeLoop");
        StartCoroutine("FreeSohwanTimeLoop");
    }

    /// <summary>
    /// 무료 소환까지 남은 시간 
    /// </summary>
    /// <returns></returns>
    IEnumerator FreeSohwanTimeLoop()
    {
        yield return null;

        DateTime nDate = BackendGpgsMng.GetInstance().GetNowTime();
        bool isAcceDelay = uiFreeSoHwan.dtAcceFreeReDate > nDate, isEquipDelay = uiFreeSoHwan.dtEquipFreeReDate > nDate;
        uiFreeSoHwan.imAcceFreeBtnBg.sprite = spBtnGray;
        uiFreeSoHwan.imEquipFreeBtnBg.sprite = spBtnGray;

        while (isAcceDelay || isEquipDelay)
        {
            nDate = BackendGpgsMng.GetInstance().GetNowTime();
            // 장신구 무료 소환 
            isAcceDelay = uiFreeSoHwan.dtAcceFreeReDate > nDate;
            if (isAcceDelay)
            {
                int totalSec = (int)(uiFreeSoHwan.dtAcceFreeReDate - nDate).TotalSeconds;
                int hours, minute, second;

                totalSec = totalSec % (24 * 3600);
                hours = totalSec / 3600;

                totalSec %= 3600;
                minute = totalSec / 60;

                totalSec %= 60;
                second = totalSec;

                uiFreeSoHwan.txAcceFreeReDate.text = string.Format("{0:00} : {1:00} : {2:00}", hours, minute, second);
            }
            else
            {
                uiFreeSoHwan.txAcceFreeReDate.text = free_sohwan; // txt 무료 소환 
                uiFreeSoHwan.imAcceFreeBtnBg.sprite = spBtnAcceFreeSohwanOk;
            }


            // 장비 뮤료 쇼환 
            isEquipDelay = uiFreeSoHwan.dtEquipFreeReDate > nDate;
            if (isEquipDelay)
            {
                int totalSec = (int)(uiFreeSoHwan.dtEquipFreeReDate - nDate).TotalSeconds;
                int hours, minute, second;

                totalSec = totalSec % (24 * 3600);
                hours = totalSec / 3600;

                totalSec %= 3600;
                minute = totalSec / 60;

                totalSec %= 60;
                second = totalSec;

                uiFreeSoHwan.txEquipFreeReDate.text = string.Format("{0:00} : {1:00} : {2:00}", hours, minute, second);
            }
            else
            {
                uiFreeSoHwan.txEquipFreeReDate.text = free_sohwan; // txt 무료 소환 
                uiFreeSoHwan.imEquipFreeBtnBg.sprite = spBtnEquipFreeSohwanOk;
            }
           
            yield return new WaitForSeconds(0.25f);
        }

        // 장신구 
        if (!isAcceDelay)
        {
            uiFreeSoHwan.txAcceFreeReDate.text = free_sohwan; // txt 무료 소환 
            uiFreeSoHwan.imAcceFreeBtnBg.sprite = spBtnAcceFreeSohwanOk;
        }

        // 장비 
        if (!isEquipDelay)
        {
            uiFreeSoHwan.txEquipFreeReDate.text = free_sohwan; // txt 무료 소환 
            uiFreeSoHwan.imEquipFreeBtnBg.sprite = spBtnEquipFreeSohwanOk;
        }
    }

    /// <summary>
    /// 장신구 무료 소환 : 광고 후 시간 단축 재사용 남은 시간 
    /// </summary>
    IEnumerator VideoReDateLoopAcce()
    {
        yield return null;
        DateTime nDate = BackendGpgsMng.GetInstance().GetNowTime();
        while (uiFreeSoHwan.dtAcceVideoReDate > nDate)
        {
            nDate = BackendGpgsMng.GetInstance().GetNowTime();
            int totalSec = (int)(uiFreeSoHwan.dtAcceVideoReDate - nDate).TotalSeconds;
            int hours, minute, second;

            totalSec = totalSec % (24 * 3600);
            hours = totalSec / 3600;

            totalSec %= 3600;
            minute = totalSec / 60;

            totalSec %= 60;
            second = totalSec;

            uiFreeSoHwan.txAcceVideoReDate.text = string.Format("{0:00} : {1:00} : {2:00}", hours, minute, second);
            yield return new WaitForSeconds(0.25f);
        }

        uiFreeSoHwan.txAcceVideoReDate.text = string.Format(ad_video_minute_reduce, GameDatabase.GetInstance().chartDB.GetDicBalance("free.ads.sohwan.shrink.minute").val_int);
    }

    /// <summary>
    /// 장비 무료 소환 : 광고 후 시간 단축 재사용 남은 시간 
    /// </summary>
    IEnumerator VideoReDateLoopEquip()
    {
        yield return null;
        DateTime nDate = BackendGpgsMng.GetInstance().GetNowTime();
        while (uiFreeSoHwan.dtEquipVideoReDate > nDate)
        {
            nDate = BackendGpgsMng.GetInstance().GetNowTime();
            int totalSec = (int)(uiFreeSoHwan.dtEquipVideoReDate - nDate).TotalSeconds;
            int hours, minute, second;

            totalSec = totalSec % (24 * 3600);
            hours = totalSec / 3600;

            totalSec %= 3600;
            minute = totalSec / 60;

            totalSec %= 60;
            second = totalSec;

            uiFreeSoHwan.txEquipVideoReDate.text = string.Format("{0:00} : {1:00} : {2:00}", hours, minute, second);
            yield return new WaitForSeconds(0.25f);
        }

        uiFreeSoHwan.txEquipVideoReDate.text = string.Format(ad_video_minute_reduce, GameDatabase.GetInstance().chartDB.GetDicBalance("free.ads.sohwan.shrink.minute").val_int);
    }

    /// <summary>
    /// 버튼 : 무료 소환 시간 줄이기 광고 플레이 묻기 (장비 소환or장신구 소환)
    /// </summary>
    public void Click_PlayVideoPopUpOn(bool isAcce)
    {
        int shrink_minute = GameDatabase.GetInstance().chartDB.GetDicBalance("free.ads.sohwan.shrink.minute").val_int;
        if (isAcce)
        {
            if (uiFreeSoHwan.dtAcceVideoReDate < BackendGpgsMng.GetInstance().GetNowTime()) // 광고 시청가능 시간이 가능 
            {
                if (uiFreeSoHwan.dtAcceFreeReDate > BackendGpgsMng.GetInstance().GetNowTime()) // 무료 소환 대기 시간이 남은 
                {
                    if (GameDatabase.GetInstance().tableDB.GetUserInfo().GetAdRemoval() == true)
                    {
                        isSoHwanVideoPlayType = "acce";
                        PlayVideoCompleteFreeSohawanTimeReduce("success");
                    }
                    else
                    {
                        PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener(string.Format(LanguageGameData.GetInstance().GetString("ask.ad.video.acce.sohawan.re.time"), shrink_minute), PlayVideoFreeAcceSohawanTimeReduce);// 짧은 광고 시청 후\n장신구 무료 소환 대기 시간을 {0}분 단축합니다.
                    }
                }
                else PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox(LanguageGameData.GetInstance().GetString("shop.luck.free.sohwan.possible")); // "무료 소환 가능한 상태입니다."
            }
            else PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox(LanguageGameData.GetInstance().GetString("notif.ad.video.delay")); // "광고 대기 시간이 있습니다. 잠시후에 다시 시도해주세요."
        }
        else
        {
            if (uiFreeSoHwan.dtEquipVideoReDate < BackendGpgsMng.GetInstance().GetNowTime()) // 광고 시청가능 시간이 가능 
            {
                if (uiFreeSoHwan.dtEquipFreeReDate > BackendGpgsMng.GetInstance().GetNowTime()) // 무료 소환 대기 시간이 남은 
                {
                    if (GameDatabase.GetInstance().tableDB.GetUserInfo().GetAdRemoval() == true)
                    {
                        isSoHwanVideoPlayType = "equip";
                        PlayVideoCompleteFreeSohawanTimeReduce("success");
                    }
                    else
                    {
                        PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener(string.Format(LanguageGameData.GetInstance().GetString("ask.ad.video.equip.sohawan.re.time"), shrink_minute), PlayVideoFreeEquipSohawanTimeReduce);  // 짧은 광고 시청 후\n장신구 무료 소환 대기 시간을 {0}분 단축합니다.
                    }
                }
                else PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox(LanguageGameData.GetInstance().GetString("shop.luck.free.sohwan.possible")); // "무료 소환 가능한 상태입니다."
            }
            else PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox(LanguageGameData.GetInstance().GetString("notif.ad.video.delay")); // "광고 대기 시간이 있습니다. 잠시후에 다시 시도해주세요."
        }
    }

    string isSoHwanVideoPlayType = "acce"; // accr or equip 
    /// <summary>
    /// 장신구 무료 소환 시간 줄이기 광고 플레이 
    /// </summary>
    public void PlayVideoFreeAcceSohawanTimeReduce()
    {
        isSoHwanVideoPlayType = "acce";
        if (BackendGpgsMng.isEditor)
        {
            PlayVideoCompleteFreeSohawanTimeReduce("success");
        }
        else
        {
            VideoAdsMng.GetInstance().AdShow(PlayVideoCompleteFreeSohawanTimeReduce);
        }
    }

    /// <summary>
    /// 장비 무료 소환 시간 줄이기 광고 플레이 
    /// </summary>
    public void PlayVideoFreeEquipSohawanTimeReduce()
    {
        isSoHwanVideoPlayType = "equip";
        if (BackendGpgsMng.isEditor)
        {
            PlayVideoCompleteFreeSohawanTimeReduce("success");
        }
        else
        {
            VideoAdsMng.GetInstance().AdShow(PlayVideoCompleteFreeSohawanTimeReduce);
        }
    }

    /// <summary>
    /// 펫 무료 소환 시간 줄이기 광고 플레이 
    /// </summary>
    public void PlayVideoFreePetSohawanTimeReduce()
    {
        isSoHwanVideoPlayType = "pet";
        if (BackendGpgsMng.isEditor)
        {
            PlayVideoCompleteFreeSohawanTimeReduce("success");
        }
        else
        {
            VideoAdsMng.GetInstance().AdShow(PlayVideoCompleteFreeSohawanTimeReduce);
        }
    }

    /// <summary>
    /// 무료 소환 시간 줄이기 광고 시청 완료 
    /// </summary>
    async void PlayVideoCompleteFreeSohawanTimeReduce(string result)
    {
        if (string.Equals(result, "success"))
        {
            DateTime nDate = BackendGpgsMng.GetInstance().GetNowTime();
            var userInfo_db = GameDatabase.GetInstance().tableDB.GetUserInfo();

            BackendReturnObject bro1 = null;
            SendQueue.Enqueue(Backend.GameInfo.GetPrivateContents, BackendGpgsMng.tableName_UserInfo, 100, callback => { bro1 = callback; });
            while (bro1 == null) { await Task.Delay(100); }

            if (bro1.IsSuccess())
            {
                if (bro1.GetReturnValuetoJSON()["rows"].Count > 0)
                {
                    JsonData row = bro1.GetReturnValuetoJSON()["rows"][0];
                    if (string.Equals(isSoHwanVideoPlayType, "acce"))
                    {
                        userInfo_db.m_free_acce_sohwan = RowPaser.StrPaser(row, "m_free_acce_sohwan");

                        // 다음 광고 보기 시간 세팅 
                        DateTime nextReDate = nDate.AddMinutes(GameDatabase.GetInstance().chartDB.GetDicBalance("free.ads.shhwan.re.minute").val_int);
                        uiFreeSoHwan.dtAcceVideoReDate = nextReDate;
                        PlayerPrefs.SetString(PrefsKeys.key_VideoDateAcceFreeSoHwan, uiFreeSoHwan.dtAcceVideoReDate.ToString());

                        // 광고 단축 DateTime 변경 
                        DateTime tryAcceFreeSohwanDate;
                        if (DateTime.TryParse(userInfo_db.m_free_acce_sohwan, out tryAcceFreeSohwanDate) == false)
                            tryAcceFreeSohwanDate = nDate;

                        int shrink_minute = GameDatabase.GetInstance().chartDB.GetDicBalance("free.ads.sohwan.shrink.minute").val_int;
                        userInfo_db.m_free_acce_sohwan = tryAcceFreeSohwanDate.AddMinutes(-shrink_minute).ToString();
                        uiFreeSoHwan.dtAcceFreeReDate = tryAcceFreeSohwanDate.AddMinutes(-shrink_minute);
                    }
                    else if (string.Equals(isSoHwanVideoPlayType, "equip"))
                    {
                        userInfo_db.m_free_equip_sohwan = RowPaser.StrPaser(row, "m_free_equip_sohwan");

                        // 다음 광고 보기 시간 세팅 
                        DateTime nextReDate = nDate.AddMinutes(GameDatabase.GetInstance().chartDB.GetDicBalance("free.ads.shhwan.re.minute").val_int);
                        uiFreeSoHwan.dtEquipVideoReDate = nextReDate;
                        PlayerPrefs.SetString(PrefsKeys.key_VideoDateEquipFreeSoHwan, uiFreeSoHwan.dtEquipVideoReDate.ToString());

                        // 광고 단축 DateTime 변경 
                        DateTime tryEquipFreeSohwanDate;
                        if (DateTime.TryParse(userInfo_db.m_free_equip_sohwan, out tryEquipFreeSohwanDate) == false)
                            tryEquipFreeSohwanDate = nDate;

                        int shrink_minute = GameDatabase.GetInstance().chartDB.GetDicBalance("free.ads.sohwan.shrink.minute").val_int;
                        userInfo_db.m_free_equip_sohwan = tryEquipFreeSohwanDate.AddMinutes(-shrink_minute).ToString();
                        uiFreeSoHwan.dtEquipFreeReDate = tryEquipFreeSohwanDate.AddMinutes(-shrink_minute);
                    }
                    else if (string.Equals(isSoHwanVideoPlayType, "pet"))
                    {
                        userInfo_db.m_free_pet_sohwan = RowPaser.StrPaser(row, "m_free_pet_sohwan");

                        // 다음 광고 보기 시간 세팅 
                        DateTime nextReDate = nDate.AddMinutes(GameDatabase.GetInstance().chartDB.GetDicBalance("free.ads.shhwan.re.minute").val_int);
                        petSohwan.dtVideoReDate = nextReDate;
                        userInfo_db.m_free_pet_sohwan_redate = nextReDate.ToString();
                        //PlayerPrefs.SetString(PrefsKeys.key_VideoDateEquipFreeSoHwan, uiFreeSoHwan.dtEquipVideoReDate.ToString());

                        // 광고 단축 DateTime 변경 
                        DateTime tryEquipFreeSohwanDate;
                        if (DateTime.TryParse(userInfo_db.m_free_pet_sohwan, out tryEquipFreeSohwanDate) == false)
                            tryEquipFreeSohwanDate = nDate;

                        int shrink_minute = GameDatabase.GetInstance().chartDB.GetDicBalance("free.ads.sohwan.shrink.minute").val_int;
                        userInfo_db.m_free_pet_sohwan = tryEquipFreeSohwanDate.AddMinutes(-shrink_minute).ToString();
                        petSohwan.dtVideoReDate = tryEquipFreeSohwanDate.AddMinutes(-shrink_minute);
                    }

                    Task tsk1 = GameDatabase.GetInstance().tableDB.SetUpdate_UserInfo(userInfo_db);
                    while (Loading.Full(tsk1.IsCompleted, true) == false) await Task.Delay(100);

                    if (string.Equals(isSoHwanVideoPlayType, "acce"))
                    {
                        InitFreeSohwan();
                        NotificationIcon.GetInstance().CheckNoticeShopLuckSohwan(false, true, false);
                    }
                    else if (string.Equals(isSoHwanVideoPlayType, "equip"))
                    {
                        InitFreeSohwan();
                        NotificationIcon.GetInstance().CheckNoticeShopLuckSohwan(true, false, false);
                    }
                    else if (string.Equals(isSoHwanVideoPlayType, "pet"))
                    {
                        InitPetSohwan();
                        NotificationIcon.GetInstance().CheckNoticeShopLuckSohwan(false, false, true);
                    }
                }
            }
        }
        else if (string.Equals(result, "OnAdFailedToLoad"))
            PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("광고 로드에 실패하였습니다. 잠시 후 다시 시도해주세요.");
        else
            PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("광고 플레이에 실패하였습니다. 잠시 후 다시 시도해주세요.");
    }

    /// <summary>
    /// 버튼 : 장신구 무료 소환 시작 
    /// 중급 ~ 고대 뽑기 
    /// </summary>
    public async void Click_StartFreeAcceSoHwan()
    {
        if (uiFreeSoHwan.dtAcceFreeReDate <= BackendGpgsMng.GetInstance().GetNowTime()) // 무료 소환 가능 
        {
            string sCompSoHwan = await StartAcceSoHwan(true, 2, 10); // 무료 소환 
            if (string.Equals(sCompSoHwan, "success"))
            {
                int add_hour = GameDatabase.GetInstance().chartDB.GetDicBalance("free.acce.sohwan.re.hour").val_int;
                var userInfo_db = GameDatabase.GetInstance().tableDB.GetUserInfo();
                userInfo_db.m_free_acce_sohwan = BackendGpgsMng.GetInstance().GetNowTime().AddHours(add_hour).ToString();
                await GameDatabase.GetInstance().tableDB.SetUpdate_UserInfo(userInfo_db);
                PopUpMng.GetInstance().Open_ShopLuckEquipResult(soHwanResults, "장신구 무료 소환", true, "acce"); // 결과 
                InitFreeSohwan();
                NotificationIcon.GetInstance().CheckNoticeShopLuckSohwan(false, true);

                GameDatabase.GetInstance().dailyMissionDB.ASetInCount(GameDatabase.DailyMissionDB.Nbr.nbr6, 1); // 일일미션, nbr6 상점-행운상점에서 무료 소환하기!
            }
            else PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox(sCompSoHwan);
        }
        else PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("무료 소환 대기 시간이 남아있습니다.");
    }
    /// <summary>
    /// 버튼 : 장비 무료 소환 시작 
    /// 중급 ~ 전설 뽑기 
    /// </summary>
    public async void Click_StartFreeEquipSoHwan()
    {
        if (uiFreeSoHwan.dtEquipFreeReDate < BackendGpgsMng.GetInstance().GetNowTime()) // 무료 소환 가능 
        {
            string sCompSoHwan = await StartAcceSoHwan(false, 2, 10); // 무료 소환 
            if (string.Equals(sCompSoHwan, "success"))
            {
                int add_hour = GameDatabase.GetInstance().chartDB.GetDicBalance("free.equip.sohwan.re.hour").val_int;
                var userInfo_db = GameDatabase.GetInstance().tableDB.GetUserInfo();
                userInfo_db.m_free_equip_sohwan = BackendGpgsMng.GetInstance().GetNowTime().AddHours(add_hour).ToString();
                await GameDatabase.GetInstance().tableDB.SetUpdate_UserInfo(userInfo_db);
                PopUpMng.GetInstance().Open_ShopLuckEquipResult(soHwanResults, "장비 무료 소환", true, "equip"); // 결과 
                InitFreeSohwan();
                NotificationIcon.GetInstance().CheckNoticeShopLuckSohwan(true, false);

                GameDatabase.GetInstance().dailyMissionDB.ASetInCount(GameDatabase.DailyMissionDB.Nbr.nbr6, 1); // 일일미션, nbr6 상점-행운상점에서 무료 소환하기!
            }
            else PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox(sCompSoHwan);
        }
        else PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("무료 소환 대기 시간이 남아있습니다.");
    }

    #endregion
    
    //-------------------------------------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------------------------------
    #region 에테르 (장신구) 뽑기 
    /// <summary>
    /// 버튼 : 일반 ~ 고대 뽑기, 에테르 사용 
    /// </summary>
    public void Click_StartEtherAcceRt1to6(int cnt)
    {
        int price = GameDatabase.GetInstance().chartDB.GetDicBalance(string.Format("shop.luck.gacha.ether.price.rt1to6cnt{0}", cnt)).val_int;
        if (GameDatabase.GetInstance().tableDB.GetTableDB_Goods().m_ether >= price)
        {
            if (cnt == 1)
                PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener(string.Format("에테르 {0}개를 사용하여 <color=#A9855D>일반</color> ~ <color=#FFA436>고대</color> 장신구를 소환합니다.", price), ListenerStartEtherAcceRt1to6cnt1);
            else if (cnt == 10)
                PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener(string.Format("에테르 {0}개를 사용하여 <color=#A9855D>일반</color> ~ <color=#FFA436>고대</color> 장신구를 소환합니다.", price), ListenerStartEtherAcceRt1to6cnt10);
            else PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("소환 횟수 오류입니다.");
        }
        else
        {
            if (GameDatabase.GetInstance().convenienceFunctionDB.GetUseingConvenFunAutoSale())
                PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("에테르가 족합니다.\n에테르는 장비를 분해하여 획득할 수 있습니다");
            else PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener("에테르가 부족합니다.\n에테르는 장비를 분해하여 획득할 수 있습니다.\n장비 자동 판매/분해 구매 탭으로 이동됩니다.", PopUpMng.GetInstance().Open_DailyProductReward);
        }
    }
    
    void ListenerStartEtherAcceRt1to6cnt1() => StartEtherAcceRt1to6(1); // 에테르 
    void ListenerStartEtherAcceRt1to6cnt10() => StartEtherAcceRt1to6(10); // 에테르 

    async void StartEtherAcceRt1to6(int cnt)
    {
        string sComp = await StartAcceSoHwan(true, 1, cnt); // 에테르 소환 
        if (string.Equals(sComp, "success"))
        {
            int price = GameDatabase.GetInstance().chartDB.GetDicBalance(string.Format("shop.luck.gacha.ether.price.rt1to6cnt{0}", cnt)).val_int;
            var goods_db = GameDatabase.GetInstance().tableDB.GetTableDB_Goods();
            goods_db.m_ether -= price;
            await GameDatabase.GetInstance().tableDB.SetUpdateGoods(goods_db);
            PopUpMng.GetInstance().Open_ShopLuckEquipResult(soHwanResults, "장신구 소환", true, "acce"); // 결과 
            GoodsView();
        }
        else PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox(sComp);
    }
    #endregion

    //-------------------------------------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------------------------------
    #region 루비 (장비) 뽑기 
    /// <summary>
    /// 버튼 : 일반 ~ 고대 뽑기, 루비 사용 
    /// </summary>
    public void Click_StartRubyEquipRt1to7(int cnt)
    {
        int price = GameDatabase.GetInstance().chartDB.GetDicBalance(string.Format("shop.luck.gacha.ruby.price.rt1to7cnt{0}", cnt)).val_int;
        if (GameDatabase.GetInstance().tableDB.GetTableDB_Goods().m_ruby >= price)
        {
            if (cnt == 1)
                PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener(string.Format("루비 {0}개를 사용하여 <color=#A9855D>일반</color> ~ <color=#FFE800>전설</color> 장비를 소환합니다.", price), ListenerStartRubyEquipRt1to7cnt1);
            else if (cnt == 10)
                PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener(string.Format("루비 {0}개를 사용하여 <color=#A9855D>일반</color> ~ <color=#FFE800>전설</color> 장비를 소환합니다.", price), ListenerStartRubyEquipRt1to7cnt10);
            else PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("소환 횟수 오류입니다.");
        }
        else
        {
            if (GameDatabase.GetInstance().convenienceFunctionDB.GetUseingConvenFunAutoSale())
                PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("루비가 부족합니다.\n루비는 장비를 분해하여 획득할 수 있습니다");
            else PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener("루비가 부족합니다.\n루비는 장비를 분해하여 획득할 수 있습니다.\n장비 자동 판매/분해 구매 탭으로 이동됩니다.", PopUpMng.GetInstance().Open_DailyProductReward);
        }
    }
    void ListenerStartRubyEquipRt1to7cnt1() => StartRubyEquipRt1to7(1); // 루비 
    void ListenerStartRubyEquipRt1to7cnt10() => StartRubyEquipRt1to7(10); // 루비 
    async void StartRubyEquipRt1to7(int cnt)
    {
        string sComp = await StartAcceSoHwan(false, 1, cnt); // 루비 소환 
        if (string.Equals(sComp, "success"))
        {
            int price = GameDatabase.GetInstance().chartDB.GetDicBalance(string.Format("shop.luck.gacha.ruby.price.rt1to7cnt{0}", cnt)).val_int;
            var goods_db = GameDatabase.GetInstance().tableDB.GetTableDB_Goods();
            goods_db.m_ruby -= price;
            await GameDatabase.GetInstance().tableDB.SetUpdateGoods(goods_db);
            PopUpMng.GetInstance().Open_ShopLuckEquipResult(soHwanResults, "장비 소환", true, "equip"); // 결과 
            GoodsView();
        }
        else PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox(sComp);
    }

    #endregion

    //-------------------------------------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------------------------------
    #region TBC 뽑기 (장신구) 
    /// <summary> TBC 1회 소환 </summary>
    public async void Click_StartTbcAcceRt2to6cnt1()
    {
        int price = GameDatabase.GetInstance().chartDB.GetDicBalance("shop.luck.gacha.acce.tbc.price.rt2to6cnt1").val_int;
        int tbc = await GameDatabase.GetInstance().tableDB.GetMyTBC();
        if (tbc >= price)
            PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener("다이아 " + price + "개를 사용하여 <color=#58B473>중급</color> ~ <color=#FFA436>고대</color> 장신구를 소환합니다.", ListenerStartTbcAcceRt2to6cnt1);
        else PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener("다이아가 부족합니다.\n다이아 구매 탭으로 이동됩니다.", MainUI.GetInstance().Listener_MoveTbcShop);
    }

    /// <summary> TBC 10회 소환 </summary>
    public async void Click_StartTbcAcceRt2to6cnt10()
    {
        int price = GameDatabase.GetInstance().chartDB.GetDicBalance("shop.luck.gacha.acce.tbc.price.rt2to6cnt10").val_int;
        int tbc = await GameDatabase.GetInstance().tableDB.GetMyTBC();
        if (tbc >= price)
            PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener("다이아 " + price + "개를 사용하여 <color=#58B473>중급</color> ~ <color=#FFA436>고대</color> 장신구를 소환합니다.", ListenerStartTbcAcceRt2to6cnt10);
        else PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener("다이아가 부족합니다.\n다이아 구매 탭으로 이동됩니다.", MainUI.GetInstance().Listener_MoveTbcShop);
    }

    /// <summary> TBC 11회 소환 </summary>
    public async void Click_StartTbcAcceRt3to6cnt11()
    {
        int price = GameDatabase.GetInstance().chartDB.GetDicBalance("shop.luck.gacha.acce.tbc.price.rt3to6cnt11").val_int;
        int tbc = await GameDatabase.GetInstance().tableDB.GetMyTBC();
        if (tbc >= price)
            PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener("다이아 " + price + "개를 사용하여 <color=#58B473>중급</color> ~ <color=#FFA436>고대</color> 장신구를 소환합니다.", ListenerStartTbcAcceRt3to6cnt11);
        else PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener("다이아가 부족합니다.\n다이아 구매 탭으로 이동됩니다.", MainUI.GetInstance().Listener_MoveTbcShop);
    }

    void ListenerStartTbcAcceRt2to6cnt1() => StartTbcUseGacha(2, 1, 0, GameDatabase.GetInstance().chartDB.GetDicBalance("shop.luck.gacha.acce.tbc.key.rt2to6cnt1").val_string);
    void ListenerStartTbcAcceRt2to6cnt10() => StartTbcUseGacha(2, 10, 1, GameDatabase.GetInstance().chartDB.GetDicBalance("shop.luck.gacha.acce.tbc.key.rt2to6cnt10").val_string);
    void ListenerStartTbcAcceRt3to6cnt11() => StartTbcUseGacha(3, 11, 1, GameDatabase.GetInstance().chartDB.GetDicBalance("shop.luck.gacha.acce.tbc.key.rt3to6cnt11").val_string, true);

    async void StartTbcUseGacha(int rt, int cnt, int fixedRt4Cnt, string tbcKey, bool isBonusGoods = false)
    {
        string sComp = await StartAcceSoHwan(true, rt, cnt, fixedRt4Cnt); // 다이아(tbc) 소환 
        if (string.Equals(sComp, "success"))
        {
            string sCompTbc = await GameDatabase.GetInstance().tableDB.DeductionTBC(tbcKey);
            if (string.Equals(sCompTbc, "success"))
            {
                var item_db = GameDatabase.GetInstance().tableDB.GetItem(29, 5);
                var goods_db = GameDatabase.GetInstance().tableDB.GetTableDB_Goods();
                if (isBonusGoods)  // 고급 소환 보너스 지급 
                {
                    goods_db.m_ether += GameDatabase.GetInstance().chartDB.GetDicBalance("shop.luck.gacha.acce.tbc.key.rt3to6cnt11.bonus.ether").val_int;
                    item_db.count += GameDatabase.GetInstance().chartDB.GetDicBalance("shop.luck.gacha.acce.tbc.key.rt3to6cnt11.bonus.acce.piece.rt5").val_int;
                }

                Task tsk1 = GameDatabase.GetInstance().tableDB.SetUpdateGoods(goods_db);
                Task tsk2= GameDatabase.GetInstance().tableDB.SendDataItem(item_db);
                while (Loading.Full(tsk1.IsCompleted, tsk2.IsCompleted, true) == false) await Task.Delay(100);

                PopUpMng.GetInstance().Open_ShopLuckEquipResult(soHwanResults, "장신구 소환", true, "acce"); // 결과 
                await Task.Delay(1000);
                GoodsView();
                MainUI.GetInstance().topUI.SetGoodsView();
                MainUI.GetInstance().topUI.GetInfoViewTBC();
            }
            else PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox(sCompTbc);
        }
        else PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox(sComp);
    }
    #endregion

    //-------------------------------------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------------------------------
    #region 다이아 (장비) 뽑기
    /// <summary> 1회 소환 </summary>
    public async void Click_StartDiaEquipRt2to7cnt1()
    {
        int price = GameDatabase.GetInstance().chartDB.GetDicBalance("shop.luck.gacha.equip.dia.price.rt2to7cnt1").val_int;
        var goods_db = GameDatabase.GetInstance().tableDB.GetTableDB_Goods();
        bool isBlueDiaLack = goods_db.m_dia < price;
        int tbc = isBlueDiaLack == true ? await GameDatabase.GetInstance().tableDB.GetMyTBC() : 0;
        int blue_dia = goods_db.m_dia;
        if (blue_dia + tbc >= price)
            PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener("다이아 " + price + "개를 사용하여 <color=#58B473>중급</color> ~ <color=#FFE800>전설</color> 장비를 소환합니다.", ListenerStartDiaEquipRt2to7cnt1);
        else PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener("다이아가 부족합니다.\n다이아 구매 탭으로 이동됩니다.", MainUI.GetInstance().Listener_MoveTbcShop);
    }
    /// <summary> 10회 소환 </summary>
    public async void Click_StartDiaEquipRt2to7cnt10()
    {
        int price = GameDatabase.GetInstance().chartDB.GetDicBalance("shop.luck.gacha.equip.dia.price.rt2to7cnt10").val_int;
        var goods_db = GameDatabase.GetInstance().tableDB.GetTableDB_Goods();
        bool isBlueDiaLack = goods_db.m_dia < price;
        int tbc = isBlueDiaLack == true ? await GameDatabase.GetInstance().tableDB.GetMyTBC() : 0;
        int blue_dia = goods_db.m_dia;
        if (blue_dia + tbc >= price)
            PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener("다이아 " + price + "개를 사용하여 <color=#58B473>중급</color> ~ <color=#FFE800>전설</color> 장비를 소환합니다.", ListenerStartDiaEquipRt2to7cnt10);
        else PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener("다이아가 부족합니다.\n다이아 구매 탭으로 이동됩니다.", MainUI.GetInstance().Listener_MoveTbcShop);
    }
    /// <summary> 고급 11회 소환 </summary>
    public async void Click_StartDiaEquipRt3to7cnt11()
    {
        int price = GameDatabase.GetInstance().chartDB.GetDicBalance("shop.luck.gacha.equip.dia.price.rt3to7cnt11").val_int;
        var goods_db = GameDatabase.GetInstance().tableDB.GetTableDB_Goods();
        bool isBlueDiaLack = goods_db.m_dia < price;
        int tbc = isBlueDiaLack == true ? await GameDatabase.GetInstance().tableDB.GetMyTBC() : 0;
        int blue_dia = goods_db.m_dia;
        if (blue_dia + tbc >= price)
            PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener("다이아 " + price + "개를 사용하여 <color=#698FFF>고급</color> ~ <color=#FFE800>전설</color> 장비를 소환합니다.", ListenerStartDiaEquipRt3to7cnt11);
        else PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener("다이아가 부족합니다.\n다이아 구매 탭으로 이동됩니다.", MainUI.GetInstance().Listener_MoveTbcShop);
    }

    void ListenerStartDiaEquipRt2to7cnt1() => ListenerStartDiaEquip(2, 1, 0);
    void ListenerStartDiaEquipRt2to7cnt10() => ListenerStartDiaEquip(2, 10, 1);
    void ListenerStartDiaEquipRt3to7cnt11() => ListenerStartDiaEquip(3, 11, 1, true);

    async void ListenerStartDiaEquip(int rt, int cnt, int fixedRt4Cnt, bool isBonusGoods = false)
    {
        string sComp = await StartAcceSoHwan(false, rt, cnt, fixedRt4Cnt);
        if (string.Equals(sComp, "success"))
        {
            int price = GameDatabase.GetInstance().chartDB.GetDicBalance(string.Format("shop.luck.gacha.equip.dia.price.rt{0}to7cnt{1}", rt, cnt)).val_int;
            var item_db = GameDatabase.GetInstance().tableDB.GetItem(28, 7);
            var goods_db = GameDatabase.GetInstance().tableDB.GetTableDB_Goods();
            bool isBlueDiaLack = goods_db.m_dia < price;
            int tbc = isBlueDiaLack == true ? await GameDatabase.GetInstance().tableDB.GetMyTBC() : 0;
            int blue_dia = goods_db.m_dia;
            if (blue_dia + tbc >= price)
            {
                if (isBonusGoods) // 고급 소환 보너스 지급 
                {
                    goods_db.m_ruby += GameDatabase.GetInstance().chartDB.GetDicBalance("shop.luck.gacha.equip.dia.price.rt3to7cnt11.bonus.ruby").val_int;
                    item_db.count += GameDatabase.GetInstance().chartDB.GetDicBalance("shop.luck.gacha.equip.dia.price.rt3to7cnt11.bonus.equip.piece.rt7").val_int;
                }

                int dedDia = goods_db.m_dia -= price; // 내 현재 블루 다이아 차감
                int dedTbc = dedDia < 0 ? Math.Abs(dedDia) : 0;
                Task tsk1 = GameDatabase.GetInstance().tableDB.SetUpdateGoods(goods_db);
                Task tsk2 = GameDatabase.GetInstance().tableDB.DeductionTBC(dedTbc);
                Task tsk3 = GameDatabase.GetInstance().tableDB.SendDataItem(item_db);
                while (Loading.Full(tsk1.IsCompleted, tsk2.IsCompleted, tsk3.IsCompleted, true) == false) await Task.Delay(100);

                PopUpMng.GetInstance().Open_ShopLuckEquipResult(soHwanResults, "장비 소환", true, "equip"); // 결과 
                await Task.Delay(1000);
                GoodsView();
                MainUI.GetInstance().topUI.SetGoodsView();
                MainUI.GetInstance().topUI.GetInfoViewTBC();
            }
            else PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener("다이아가 부족합니다.\n다이아 구매 탭으로 이동됩니다.", MainUI.GetInstance().Listener_MoveTbcShop);
        }
        else
        {
            PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox(sComp);
        }
    }
    #endregion

    //-------------------------------------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------------------------------
    #region 펫 뽑기
    [SerializeField] PetSohwan petSohwan;
    [System.Serializable]
    struct PetSohwan
    {
        public DateTime dtFreeDate;
        public DateTime dtVideoReDate;

        public Text txFreeTime, txFreeReTime, txBlueDiaPriceRating3, txRating3Cnt, txRating4Cnt, txRating5Cnt, txBlueDiaPriceRating6, txRedDiaPriceRating6;
        public Image imFreeBtnBg;
    }

    async void InitPetSohwan()
    {
        BackendReturnObject bro1 = null;
        SendQueue.Enqueue(Backend.GameInfo.GetPrivateContents, BackendGpgsMng.tableName_UserInfo, 100, callback => { bro1 = callback; });
        while (bro1 == null) { await Task.Delay(100); }

        if (bro1.IsSuccess())
        {
            if (bro1.GetReturnValuetoJSON()["rows"].Count > 0)
            {
                DateTime nDate = BackendGpgsMng.GetInstance().GetNowTime();
                JsonData row = bro1.GetReturnValuetoJSON()["rows"][0];
                string m_free_pet_sohwan = RowPaser.StrPaser(row, "m_free_pet_sohwan");
                if (DateTime.TryParse(m_free_pet_sohwan, out petSohwan.dtFreeDate) == false)
                    petSohwan.dtFreeDate = nDate;

                string m_free_pet_sohwan_redate = RowPaser.StrPaser(row, "m_free_pet_sohwan_redate");
                if (DateTime.TryParse(m_free_pet_sohwan_redate, out petSohwan.dtVideoReDate) == false)
                    petSohwan.dtVideoReDate = nDate;

                var userinfo_db = GameDatabase.GetInstance().tableDB.GetUserInfo();
                userinfo_db.m_free_pet_sohwan = petSohwan.dtFreeDate.ToString();
                userinfo_db.m_free_pet_sohwan_redate = petSohwan.dtVideoReDate.ToString();
                GameDatabase.GetInstance().tableDB.SetUpdate_UserInfo(userinfo_db, true);

                StopCoroutine("FreePetSohwanTimeLoop");
                StartCoroutine("FreePetSohwanTimeLoop");

                StopCoroutine("FreePetSohwanReVideoTimeLoop");
                StartCoroutine("FreePetSohwanReVideoTimeLoop");

                var petAL_rt3 = GameDatabase.GetInstance().tableDB.GetItem(31, 3);
                var petAL_rt4 = GameDatabase.GetInstance().tableDB.GetItem(31, 4);
                var petAL_rt5 = GameDatabase.GetInstance().tableDB.GetItem(31, 5);

                petSohwan.txRating3Cnt.text = string.Format("보유 수량 : {0}\n10회 개봉하기", petAL_rt3.count);
                petSohwan.txRating4Cnt.text = string.Format("보유 수량 : {0}\n10회 개봉하기", petAL_rt4.count);
                petSohwan.txRating5Cnt.text = string.Format("보유 수량 : {0}\n10회 개봉하기", petAL_rt5.count);

                //petSohwan.txBlueDiaPriceRating3.text = string.Format("x{0}", GameDatabase.GetInstance ().chartDB.GetDicBalance("shop.luck.gacha.pet.blue.dia.price.rt5").val_int);
                petSohwan.txBlueDiaPriceRating6.text = string.Format("x{0}", GameDatabase.GetInstance().chartDB.GetDicBalance("shop.luck.gacha.pet.blue.dia.price.rt6").val_int);
                petSohwan.txRedDiaPriceRating6.text = string.Format("x{0}", GameDatabase.GetInstance().chartDB.GetDicBalance("shop.luck.gacha.pet.red.dia.price.rt6").val_int);
            }
        }
    }

    /// <summary>
    /// 무료 소환까지 남은 시간 
    /// </summary>
    IEnumerator FreePetSohwanTimeLoop()
    {
        yield return null;

        petSohwan.imFreeBtnBg.sprite = SpriteAtlasMng.GetInstance().GetSpriteButtonRedOrGray(false);
        DateTime nDate = BackendGpgsMng.GetInstance().GetNowTime();
        while ((petSohwan.dtFreeDate - nDate).TotalSeconds > 0)
        {
            nDate = BackendGpgsMng.GetInstance().GetNowTime();
            int totalSec = (int)(petSohwan.dtFreeDate - nDate).TotalSeconds;
            int hours, minute, second;

            totalSec = totalSec % (24 * 3600);
            hours = totalSec / 3600;

            totalSec %= 3600;
            minute = totalSec / 60;

            totalSec %= 60;
            second = totalSec;

            petSohwan.txFreeTime.text = string.Format("{0:00} : {1:00} : {2:00}", hours, minute, second);

            yield return new WaitForSeconds(0.25f);
        }

        petSohwan.txFreeTime.text = "무료 소환 가능"; // txt 무료 소환 
        petSohwan.imFreeBtnBg.sprite = SpriteAtlasMng.GetInstance().GetSpriteButtonRedOrGray(true);
    }

    /// <summary>
    /// 무료 소환 광고 남은 시간 
    /// </summary>
    IEnumerator FreePetSohwanReVideoTimeLoop()
    {
        yield return null;

        DateTime nDate = BackendGpgsMng.GetInstance().GetNowTime();
        while ((petSohwan.dtVideoReDate - nDate).TotalSeconds > 0)
        {
            nDate = BackendGpgsMng.GetInstance().GetNowTime();
            int totalSec = (int)(petSohwan.dtVideoReDate - nDate).TotalSeconds;
            int hours, minute, second;

            totalSec = totalSec % (24 * 3600);
            hours = totalSec / 3600;

            totalSec %= 3600;
            minute = totalSec / 60;

            totalSec %= 60;
            second = totalSec;

            petSohwan.txFreeReTime.text = string.Format("{0:00} : {1:00} : {2:00}", hours, minute, second);

            yield return new WaitForSeconds(0.25f);
        }

        petSohwan.txFreeReTime.text = "30분 단축(<color=yellow>광고</color>)";
    }


    public async void Click_PetSohwanFree(int masRating)
    {
        BackendReturnObject bro1 = null;
        SendQueue.Enqueue(Backend.GameInfo.GetPrivateContents, BackendGpgsMng.tableName_UserInfo, 100, callback => { bro1 = callback; });
        while (bro1 == null) { await Task.Delay(100); }

        if (bro1.IsSuccess())
        {
            if (bro1.GetReturnValuetoJSON()["rows"].Count > 0)
            {
                DateTime nDate = BackendGpgsMng.GetInstance().GetNowTime();
                JsonData row = bro1.GetReturnValuetoJSON()["rows"][0];
                string m_free_pet_sohwan = RowPaser.StrPaser(row, "m_free_pet_sohwan");

                if (DateTime.TryParse(m_free_pet_sohwan, out petSohwan.dtFreeDate) == false)
                    petSohwan.dtFreeDate = nDate;

                if (petSohwan.dtFreeDate <= nDate) // 무료 소환 가능 
                {
                    var userinfo_db = GameDatabase.GetInstance().tableDB.GetUserInfo();
                    userinfo_db.m_free_pet_sohwan = petSohwan.dtFreeDate.ToString();
                    GameDatabase.GetInstance().tableDB.SetUpdate_UserInfo(userinfo_db);
                    PetSohwanStart(masRating, "free", (ObscuredInt)10);
                }
                else PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("무료 소환 대기 시간이 남아있습니다.");
            }
        }
    }

    /// <summary>
    /// 펫 무료 소환 시간 단축 (광고) 
    /// </summary>
    public async void Click_PetSohwanFreeTimeReduction()
    {
        BackendReturnObject bro1 = null;
        SendQueue.Enqueue(Backend.GameInfo.GetPrivateContents, BackendGpgsMng.tableName_UserInfo, 100, callback => { bro1 = callback; });
        while (bro1 == null) { await Task.Delay(100); }

        if (bro1.IsSuccess())
        {
            if (bro1.GetReturnValuetoJSON()["rows"].Count > 0)
            {
                DateTime nDate = BackendGpgsMng.GetInstance().GetNowTime();
                JsonData row = bro1.GetReturnValuetoJSON()["rows"][0];
                string m_free_pet_sohwan = RowPaser.StrPaser(row, "m_free_pet_sohwan");
                string m_free_pet_sohwan_redate = RowPaser.StrPaser(row, "m_free_pet_sohwan_redate");

                if (DateTime.TryParse(m_free_pet_sohwan, out petSohwan.dtFreeDate) == false)
                    petSohwan.dtFreeDate = nDate;


                if (DateTime.TryParse(m_free_pet_sohwan_redate, out petSohwan.dtVideoReDate) == false)
                    petSohwan.dtVideoReDate = nDate;

                if (petSohwan.dtFreeDate > nDate) // 무료 소환 대기시간 있음 
                {
                    if(petSohwan.dtVideoReDate <= nDate)
                    {
                        int shrink_minute = GameDatabase.GetInstance().chartDB.GetDicBalance("free.ads.sohwan.shrink.minute").val_int;
                        if (GameDatabase.GetInstance().tableDB.GetUserInfo().GetAdRemoval() == true)
                        {
                            isSoHwanVideoPlayType = "pet";
                            PlayVideoCompleteFreeSohawanTimeReduce("success");
                        }
                        else
                        {
                            PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener(
                                string.Format("짧은 광고 시청 후\n펫 무료 소환 대기 시간을 {0}분 단축합니다.", shrink_minute), PlayVideoFreePetSohawanTimeReduce);
                        }
                    }
                    else PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("현재 광고 대기 시간이 있습니다.");
                }
                else PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("현재 무료 소환 가능한 상태입니다.");
            }
        }
    }

    public void Click_PetSohwanBlueDiaStart(int maxRating) => PetSohwanStart(maxRating, "blue.dia", (ObscuredInt)10);

    public void Click_PetSohwanRedDiaStart(int maxRating) => PetSohwanStart(maxRating, "red.dia", (ObscuredInt)10);

    public void Click_PetSohwanItemStart(int maxRating)
    {
        var itemPetDB = GameDatabase.GetInstance().tableDB.GetItem(31, maxRating);
        if(itemPetDB.count > 0)
            PetSohwanStart(maxRating, "item", itemPetDB.count >= (ObscuredInt)10 ? (ObscuredInt)10 : (ObscuredInt)itemPetDB.count);
        else PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("소환 가능한 수량이 부족합니다.");
    }

    /// <summary>
    /// 펫 소환 시작 
    /// </summary>
    async void PetSohwanStart(int maxRating, string shw_Type, ObscuredInt cnt)
    {
        string sComp = await StartPetSoHwan(maxRating, cnt);
        if (string.Equals(sComp, "success"))
        {
            Loading.Full(false, true);
            bool isStartGo = false;
            if (string.Equals(shw_Type, "blue.dia"))
            {
                var goods_db = GameDatabase.GetInstance().tableDB.GetTableDB_Goods();
                int blue_price = GameDatabase.GetInstance().chartDB.GetDicBalance(string.Format("shop.luck.gacha.pet.blue.dia.price.rt{0}", maxRating)).val_int;
                int dedDia = goods_db.m_dia - blue_price; // 내 현재 블루 다이아 차감
                bool isBlueDiaLack = goods_db.m_dia < blue_price;
                int tbc = isBlueDiaLack == true ? await GameDatabase.GetInstance().tableDB.GetMyTBC() : 0;
                int blue_dia = goods_db.m_dia;
                if (blue_dia + (tbc * 15) >= blue_price)
                {
                    goods_db.m_dia -= blue_price;
                    int ratio_dedTbc = dedDia < 0 ? (int)(Math.Abs(dedDia) / 15) : 0;
                    
                    Task tsk1 = GameDatabase.GetInstance().tableDB.SetUpdateGoods(goods_db);
                    Task tsk2 = GameDatabase.GetInstance().tableDB.DeductionTBC(ratio_dedTbc);
                    await tsk1;
                    await tsk2;
                    isStartGo = true;
                }
                else PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener("다이아가 부족합니다.\n펫 소환은 블루 다이아 부족시 15:1 비율로 레드 다이아가 소모됩니다.\n ex)블루 다이아 1500개 -> 레드 다이아 100개 소모\n다이아 구매 탭으로 이동됩니다.", MainUI.GetInstance().Listener_MoveTbcShop);
            }
            else if(string.Equals(shw_Type, "red.dia"))
            {
                int red_price = GameDatabase.GetInstance().chartDB.GetDicBalance(string.Format("shop.luck.gacha.pet.red.dia.price.rt{0}", maxRating)).val_int;
                int tbc = await GameDatabase.GetInstance().tableDB.GetMyTBC();
                if(tbc >= red_price)
                {
                    await GameDatabase.GetInstance().tableDB.DeductionTBC(red_price);
                    isStartGo = true;
                }
                else PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener("레드 다이아가 부족합니다.\n레드 다이아 구매 탭으로 이동됩니다.", MainUI.GetInstance().Listener_MoveTbcShop);
            }
            else if(string.Equals(shw_Type, "free"))
            {
                int add_hour = GameDatabase.GetInstance().chartDB.GetDicBalance(string.Format("shop.luck.gacha.pet.free.re.hour.rt{0}", maxRating)).val_int;
                var userInfo_db = GameDatabase.GetInstance().tableDB.GetUserInfo();
                userInfo_db.m_free_pet_sohwan = BackendGpgsMng.GetInstance().GetNowTime().AddHours(add_hour).ToString();
                    
                await GameDatabase.GetInstance().tableDB.SetUpdate_UserInfo(userInfo_db);
                isStartGo = true;
            }
            else if (string.Equals(shw_Type, "item"))
            {
                var item_db = GameDatabase.GetInstance().tableDB.GetItem(31, maxRating);
                if(item_db.count > 0)
                {
                    item_db.count -= cnt;
                    await GameDatabase.GetInstance().tableDB.SendDataItem(item_db);
                    isStartGo = true;
                   
                }
                else PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("소환 가능한 수량이 부족합니다.");
            }

            if (isStartGo)
            {
                PopUpMng.GetInstance().Open_ShopLuckEquipResult(soHwanResults, "펫 소환", true, "pet"); // 결과 
                await Task.Delay(1000);

                InitPetSohwan();
                if (string.Equals(shw_Type, "free"))
                    NotificationIcon.GetInstance().CheckNoticeShopLuckSohwan(false, false, true);

                GoodsView();
                MainUI.GetInstance().topUI.SetGoodsView();
                MainUI.GetInstance().topUI.GetInfoViewTBC();
            }
        }
        else
        {
            PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox(sComp);
        }

        Loading.Full(true);
    }

    #endregion

    //-------------------------------------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------------------------------
    #region 뽑기 결과
    //-------------------------------------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------------------------------
    // 장비/장신구 뽑기 진행 및 결과 
    async Task<string> StartAcceSoHwan(bool isAcce, int minRt, int cnt, int rt4Fixed = 0) // isRt4Fixed : 희귀 등급 1개 확정 등장 
    {
        PopUpMng.GetInstance().Open_ShopLuckEquipResultEmpty();
        soHwanResults.Clear();

        // 랜덤 등급 뽑기 + Count -> cnt 
        string rtProbKey1 = isAcce == true ? string.Format("r_ac_sohwan_rt{0}_rt6", minRt) : string.Format("r_eq_sohwan_rt{0}_rt7", minRt);
        string rtProbKey1FileId = GameDatabase.GetInstance().chartProbabilityDB.GetSelectedProbabilityFileId(rtProbKey1);
        BackendReturnObject bro1 = null, bro2 = null;
        SendQueue.Enqueue(Backend.Probability.GetProbabilitys, rtProbKey1FileId, cnt, callback => { bro1 = callback; }); // 랜덤 등급 

        // 고정 등급 뽑기 + Count -> 1
        if (rt4Fixed > 0)
        {
            string rtFixProKey2 = isAcce == true ? "r_ac_sohwan_rt4" : "r_eq_sohwan_rt4";
            string rtFixProbKey2FileId = GameDatabase.GetInstance().chartProbabilityDB.GetSelectedProbabilityFileId(rtFixProKey2);
            SendQueue.Enqueue(Backend.Probability.GetProbability, rtFixProbKey2FileId, callback => { bro2 = callback; });    // 고정 등급 
            while (Loading.Full(bro1, bro2, true) == false) { await Task.Delay(100); }
        }
        else
        {
            while (Loading.Full(bro1, true) == false) { await Task.Delay(100); }
        }

        bool isBro1Success = bro1.IsSuccess();
        bool siBro2Success = rt4Fixed > 0 ? bro2.IsSuccess() : true;

        if (isBro1Success && siBro2Success)
        {
            // 랜덤 결과,  
            JsonData broRow1 = bro1.GetReturnValuetoJSON()["elements"];
            foreach (JsonData brw1 in broRow1)
            {
                soHwanResults.Add(new AcceResult()
                {
                    isRtFixed = false,
                    ac_type = RowPaser.IntPaser(brw1, "ac_type"),
                    ac_rt = RowPaser.IntPaser(brw1, "ac_rt"),
                    ac_id = RowPaser.IntPaser(brw1, "ac_id"),
                });
            }

            // 고급 고정 등급 결과 
            if (rt4Fixed > 0)
            {
                JsonData broRow2 = bro2.GetReturnValuetoJSON()["element"];
                soHwanResults.Add(new AcceResult()
                {
                    isRtFixed = true,
                    ac_type = RowPaser.IntPaser(broRow2, "ac_type"),
                    ac_rt = RowPaser.IntPaser(broRow2, "ac_rt"),
                    ac_id = RowPaser.IntPaser(broRow2, "ac_id"),
                });
            }

            return "success";
        }
        else
        {
            PopUpMng.GetInstance().Close_ShopLuckEquipResult();
            if (bro1.IsSuccess() == false)
                return bro1.GetMessage();
            else
                return bro2.GetMessage();
        }
    }

    // 펫 뽑기 진행 및 결과 
    async Task<string> StartPetSoHwan(int maxRt, int cnt)
    {
        PopUpMng.GetInstance().Open_ShopLuckEquipResultEmpty();
        soHwanResults.Clear();

        // 랜덤 등급 뽑기 + Count -> cnt 
        string rtProbKey1 = string.Format("r_pet_sohwan_rt{0}", maxRt);
        string rtProbKey1FileId = GameDatabase.GetInstance().chartProbabilityDB.GetSelectedProbabilityFileId(rtProbKey1);
        BackendReturnObject bro1 = null, bro2 = null;
        SendQueue.Enqueue(Backend.Probability.GetProbabilitys, rtProbKey1FileId, cnt, callback => { bro1 = callback; }); // 랜덤 등급 
        while (Loading.Full(bro1, true) == false) { await Task.Delay(100); }

        bool isBro1Success = bro1.IsSuccess();

        if (isBro1Success)
        {
            // 랜덤 결과,  
            JsonData broRow1 = bro1.GetReturnValuetoJSON()["elements"];
            foreach (JsonData brw1 in broRow1)
            {
                soHwanResults.Add(new AcceResult()
                {
                    ac_rt = RowPaser.IntPaser(brw1, "p_rt"),
                    ac_id = RowPaser.IntPaser(brw1, "p_id"),
                });
            }

            if (soHwanResults.Count > 0)
                return "success";
            else return "소환 횟수 오류";
        }
        else
        {
            PopUpMng.GetInstance().Close_ShopLuckEquipResult();
            if (bro1.IsSuccess() == false)
                return bro1.GetMessage();
            else
                return bro2.GetMessage();
        }
    }
    #endregion
}
