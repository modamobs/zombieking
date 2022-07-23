using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//  Include GPGS namespace
using GooglePlayGames;
using GooglePlayGames.BasicApi;
// Include Backend
using BackEnd;
using static BackEnd.BackendAsyncClass;
using LitJson;
using System.IO;
using UnityEngine.SceneManagement;

using System.Threading;
using System.Threading.Tasks;

using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.Experimental.Rendering;

using System.Text;
using static GameDatabase;
using CodeStage.AntiCheat.ObscuredTypes;

public class BackendGpgsMng : MonoSingleton<BackendGpgsMng>
{
    [SerializeField] bool testNoBackend = true;
    public static bool isEditor = false;
    public string currentVersion = "";
    public bool initSuccess = false;
    public static System.DateTime GetLoginSDate;
    public static System.DateTime GetLoginSystemDate;

    #region ##### Backend 테이블 네임 #####
    // private table 
    public const string tableName_Goods = "Goods";
    public const string tableName_UserInfo = "UserInfo";
    public const string tableName_Equipment = "Equipment";
    public const string tableName_Item = "Item";
    public const string tableName_Skill = "Skill";
    public const string tableName_Pet = "Pet";
    public const string tableName_PetEncy = "PetEncy";

    public const string tableName_EquipmentEncyclopedia = "EquipmentEncyclopedia";
    public const string tableName_InventoryBackUp = "InventoryBackUp";
    public const string tableName_DungeonTop = "DungeonTop";
    public const string tableName_DungeonMine = "DungeonMine";
    public const string tableName_DungeonRaid = "DungeonRaid";
    public const string tableName_Achievements = "Achievements";
    public const string tableName_DailyMission = "DailyMission";
    public const string tableName_Quest = "Quest";
    public const string tableName_UserBlock = "UserBlock";
    
    // publce table 
    public const string tableName_Pub_ChapterClearCharData = "Pub_ChapterClearCharData";
    public const string tableName_Pub_NowCharData = "Pub_NowCharData";
    #endregion

    public static List<Task> awaitTasks = new List<Task>();
    public static async Task AWaitTask()
    {
        PopUpMng.GetInstance().backendWaitingLoading.gameObject.SetActive(true);

        if(awaitTasks.Count > 0)
        {
            // 서버와 통신이 끝날때까지 대기 
            while (awaitTasks.Count > 0)
            {
                await awaitTasks[0];
                awaitTasks.RemoveAt(0);
            }

            awaitTasks.Clear();
            PopUpMng.GetInstance().backendWaitingLoading.gameObject.SetActive(false);
        }
    }

    public bool SendQueueUnprocessedCount() => SendQueue.UnprocessedFuncCount > 0;

    private float one_second = 0f;
    async void QueueLoop()
    {
        while (Application.isPlaying)
        {
            SendQueue.Poll();
            await Task.Delay(100);

            one_second += 0.1f;
            if(one_second >= 1)
            {
                one_second = 0;
            }
        }
    }

    public bool isBtnClickQuit = false;
    private async void OnApplicationFocus(bool focus)
    {
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            if (isEditor == false)
            {
                if (focus)
                {
                    await SettingNowTime();
                    LoopCheckingResetToday();
                    ScoreSendLoop();
                    SendAllDB();
                    NotificationIcon.GetInstance().CheckNoticeMail(false);
                }
                else
                {
                    if (isBtnClickQuit == false)
                    {
                        SendFocusDB();
                    }
                }
            }
        }
    }

    private void OnApplicationQuit()
    {
        if (isEditor)
        {
            SendFocusDB();
        }

#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        fileIDguid00000000000000000000000000000000();
        SendQueue.StopSendQueue();
#endif
        
        Backend.Notification.DisConnect();
    }

    void fileIDguid00000000000000000000000000000000()
    {
#if UNITY_EDITOR
        List<string> scenePaths = new List<string>();
        foreach (UnityEditor.EditorBuildSettingsScene S in UnityEditor.EditorBuildSettings.scenes)
        {
            if (S.enabled)
                scenePaths.Add(S.path);
        }
        string changeString = "texture: {fileID: 0}";
        foreach (string str in scenePaths)
        {
            string tempPath = Application.dataPath.Replace("Assets", "") + str;
            string[] allLines = File.ReadAllLines(tempPath);
            for (int i = 0; i < allLines.Length; ++i)
            {
                string lineStr = allLines[i];
                if (lineStr.Contains("fileID:") && lineStr.Contains("guid: 00000000000000000000000000000000"))
                {
                    lineStr.Remove(0, lineStr.Length);
                    lineStr += changeString;
                }
            }
            File.WriteAllLines(tempPath, allLines);
        }
#endif
    }

    public int GetSceneActiveIndex () => SceneManager.GetActiveScene().buildIndex;

    #region ##### 뒤끝 유저 정보 
    public static BackendUserInfo backendUserInfo = new BackendUserInfo();
    [System.Serializable]
    public  struct BackendUserInfo
    {
        public string m_nickname;
        public string m_indate;
    }
    #endregion
   
    #region ##### Unity 초기화 
    void Awake ()
    {
#if UNITY_EDITOR
        isEditor = true;
#endif
        //PlayerPrefs.DeleteAll();
        currentVersion = Application.version;
        LanguageGameData.GetInstance().IAwake();
        LanguageSystemData.GetInstance().IAwake();
    }

    IEnumerator Start ()
    {
        // LOGO 
        AGetLoadText("쌉 좀비 나이트 키우기에 오신걸 환영합니다~!!", true);
        yield return GameObject.FindObjectOfType<BackendUI>().CanvasLogo();
        yield return new WaitForSeconds(1.0f);

        //#if !UNITY_EDITOR && UNITY_ANDROID
        // ----- GPGS -----
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
        //.AddOauthScope("https://www.googleapis.com/auth/drive.file")
        //.EnableSavedGames()
        .RequestServerAuthCode(false)
        .RequestIdToken()
        .Build();

        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.DebugLogEnabled = false;
        //GPGS 시작.
        PlayGamesPlatform.Activate();
        //#endif

        SendQueue.StartSendQueue(true);
        QueueLoop();

        Backend.Initialize(HandleBackendCallback);
    }
    #endregion

    #region ##### Backend Call back 
    void HandleBackendCallback()
    {
        LogPrint.Print("<color=yellow>HandleBackendCallback</color>");
        Backend.Initialize(() =>
        {
            // 초기화 성공한 경우 실행 
            if (Backend.IsInitialized)
            {
                initSuccess = true;
                LogPrint.Print("<color=yellow> 뒤끝 초기화 성공!! - 서버시간 획득 : </color>" + Backend.Utils.GetServerTime()); // 서버시간 획득

                // 구글 해시키 획득 
                //if (!string.IsNullOrEmpty(Backend.Utils.GetGoogleHash()))
                //    GameObject.Find("TextHashKey").GetComponent<Text>().text = Backend.Utils.GetGoogleHash().ToString();

                //SendQueue.StartSendQueue(true);
                GetServerStatus(); // 서버 상태 확인 
            }
            // 초기화 실패한 경우 실행 
            else
            {
                //ShowMessage("thebackend.initializationFailed", 2f); // 서버 초기화에 실패하였습니다.
            }
        });
    }
    #endregion

    #region ##### 뒤끝 테이블 데이터 => Insert, Update 
    /// <summary> (Insert) 테이블의 데이터 정보를 새로 추가한다. </summary>
    public async Task<string> TaskInsertTableData(string _table_name, Param param)
    {
        LogPrint.Print("<color=yellow>----------------</color>A TaskInsertTableData _table_name : " + _table_name + ", param.GetJson () : " + param.GetJson());
        BackendReturnObject bro1 = null;
        SendQueue.Enqueue(Backend.GameInfo.Insert, _table_name, param, callback => { bro1 = callback; });
        while (bro1 == null) { await Task.Delay(100); }
        LogPrint.Print("<color=yellow>----------------</color>A TaskInsertTableData _table_name : " + _table_name + ", bro1 : " + bro1);

        if (bro1.IsSuccess())
        {
            return bro1.GetReturnValuetoJSON()["inDate"].ToString();
        }
        else LogPrint.PrintError(string.Format("<color=red>TaskInsertTableData Error </color> Code {0}, param.Count : {1}", bro1.GetErrorCode(), param.GetJson()));

        return string.Empty;
    }

    /// <summary> (Update) 테이블의 데이터 정보를 수정한다. </summary>
    public async Task<bool> TaskUpdateTableData(string _table_name, string indate, Param param)
    {
        LogPrint.Print("<color=yellow>----------------</color>A TaskUpdateTableData _table_name : " + _table_name + ", param.GetJson () : " + param.GetJson());
        BackendReturnObject bro1 = null;
        SendQueue.Enqueue(Backend.GameInfo.Update, _table_name, indate, param, callback => { bro1 = callback; });
        while (bro1 == null) { await Task.Delay(100); }
        LogPrint.Print("<color=yellow>----------------</color>A TaskUpdateTableData _table_name : " + _table_name + ", bro1 : " + bro1);

        if (bro1.IsSuccess())
        {
            return true;
        }
        else LogPrint.PrintError(string.Format("<color=red>TaskUpdateTableData Error </color> Code {0}, indate : {1}, param.Count : {2}", bro1.GetErrorCode(), indate, param.GetJson()));

        return false;
    }

    /// <summary> 서버에서 해당 테이블의 아이템 InDate를 찾는다. </summary>
    private async Task<string> TaskGetFindInDate(string _table_name, string _uid)
    {
        string _indate = string.Empty;
        BackendReturnObject bro1 = null;
        SendQueue.Enqueue(Backend.GameInfo.GetPrivateContents, _table_name, callback => { bro1 = callback; });
        while (Loading.Bottom(bro1) == false) { await Task.Delay(100); }

        if (bro1.IsSuccess())
        {
            JsonData jd = bro1.GetReturnValuetoJSON();
            foreach (JsonData row in jd["rows"])
            {
                if (string.Equals(row["aInUid"]["S"].ToString(), _uid))
                {
                    _indate = row["inDate"]["S"].ToString();
                    break;
                }
            }
        }
        else LogPrint.PrintError(string.Format("<color=red>TaskGetFindInDate Error </color> Code {0}, _uid : {1}", bro1.GetErrorCode(), _uid));

        if (!string.IsNullOrEmpty(_indate))
            LogPrint.Print("<color=yellow> [backend], " + _table_name + " 해당 아이템의 InDate값을 찾았습니다. : " + _indate + "</color>");
        else LogPrint.Print("<color=red> [backend], " + _table_name + "해당 아이템의 InDate값을 찾지 못했습니다. : NULL </color>");

        return _indate;
    }
    #endregion


    /// <summary>
    /// 뒤끝 공지사항 받아오기 
    /// </summary>
    public async Task<JsonData> BackendNoticeList()
    {
        BackendReturnObject bro1 = null;
        SendQueue.Enqueue(Backend.Notice.NoticeList, callback => { bro1 = callback; });
        while (bro1 == null) { await Task.Delay(100); }

        LogPrint.Print("공지 : " + bro1);
        return bro1.GetReturnValuetoJSON()["rows"];
    }

    /// <summary>
    /// 서버 상태 체크 
    /// serverStatus : 0->온라인, 1->오프라인, 2->점검
    /// </summary>
    private void GetServerStatus()
    {
    #if UNITY_EDITOR || UNITY_STANDALONE_WIN
           GetLatestVersion(); // Application 버전 확인
#else
        var bro = Backend.Utils.GetServerStatus();
        LogPrint.Print("GetServerStatus bro : " + bro);
        if (bro.IsSuccess())
        {
            int status = (int)bro.GetReturnValuetoJSON()["serverStatus"];
            LogPrint.Print("status:" + status);
            if (status == 0) // 온라인 
                GetLatestVersion(); // Application 버전 확인
            else
            {
                GameObject.FindObjectOfType<BackendUI>().PopServerStatus(status);
            }
        }
        else
        {
            GameObject.FindObjectOfType<BackendUI>().PopServerStatus(1);
        }
#endif
    }

    #region ##### 뒤끝 : 버전 체크 -> 이후 개인정보 취급 방침 팝업 
    // 버전 체크 
    private async void GetLatestVersion()
    {
        LogPrint.Print("<color=yellow> 버전 체크 </color>");
        Task<bool> settingDT = SettingNowTime();
        await settingDT;
        if (settingDT.Result == false)
            return;

#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        CheckPrivacyPolicy();
#else 
        BackendReturnObject bro = Backend.Utils.GetLatestVersion();
        Debug.Log(bro.IsSuccess());
        if (bro.IsSuccess())
        {
            string _version = bro.GetReturnValuetoJSON()["version"].ToString();
            Debug.Log("version info - current: " + currentVersion + " _version: " + _version);
            if (!string.Equals(currentVersion, _version))
            {
                int type = (int)bro.GetReturnValuetoJSON()["type"];
                // type = 1 : 선택, type = 2 : 강제
                GameObject.FindObjectOfType<BackendUI>().PopUpUpdateVersion(type);
            }
            else
            {
                CheckPrivacyPolicy();
            }
        }
        else
        {
            LogPrint.PrintError("backend version check failed");
        }
#endif
    }

    // 개인정보 취급 방침
    public void CheckPrivacyPolicy()
    {
        LogPrint.Print("<color=red>prky_privacy_policy : </color>" + PlayerPrefs.GetString(PrefsKeys.prky_privacy_policy));
        if (string.Equals(PlayerPrefs.GetString(PrefsKeys.prky_privacy_policy), "yes") == false)
        {
            GameObject.FindObjectOfType<BackendUI>().PopUpPrivacyPolicy();
        }
        else
        {
            LoginType();
        }
    }
    #endregion

    #region ##### 시간 
    // 서버 시간 DateTime string
    public async Task<string> GetBackendTime()
    {
        BackendReturnObject bro1 = null;
        SendQueue.Enqueue(Backend.Utils.GetServerTime, callback => { bro1 = callback; });
        while (bro1 == null) { await Task.Delay(100); }

        LogPrint.PrintError("utcTime : " + System.DateTime.Parse(bro1.GetReturnValuetoJSON()["utcTime"].ToString()));
        //return System.DateTime.Parse(_bro.GetReturnValuetoJSON()["utcTime"].ToString()).AddHours(9).ToString();
        return System.DateTime.Parse(bro1.GetReturnValuetoJSON()["utcTime"].ToString()).ToString();
    }

    // 서버 시간 DateTime
    public async Task<System.DateTime> GetBackendDateTime()
    {
        Task<string> sdt = GetBackendTime();
        await sdt;
        return System.DateTime.Parse(sdt.Result.ToString());
    }

    public async Task<bool> SettingNowTime()
    {
        Task<string> _task_time = this.GetBackendTime();
        await _task_time;
        string STime = _task_time.Result.ToString();
        GetLoginSDate = System.DateTime.Parse(STime);
        //GetLoginSDate.AddHours(9);
        GetLoginSystemDate = System.DateTime.UtcNow;

        LogPrint.EditorPrint("GetNowTime : " + GetNowTime() +", Gap : " + (GetLoginSDate - GetLoginSystemDate.AddHours(9)).TotalSeconds);
        if((GetLoginSDate - GetLoginSystemDate.AddHours(9)).TotalSeconds >= -10 && (GetLoginSDate - GetLoginSystemDate.AddHours(9)).TotalSeconds <= 10)
        {
            LogPrint.EditorPrint("11111 " + (GetLoginSDate - GetLoginSystemDate.AddHours(9)).TotalSeconds);
            return true;
        }
        else
        {
            if (SceneManager.GetActiveScene().buildIndex != 0)
            {
                PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("시스템 시간이 변경되었습니다.\n게임을 다시 시작해 주세요.", Application.Quit);
            }
            else
            {
                GameObject.FindObjectOfType<BackendUI>().IntroNotice("서버 시간과 시스템 시간이 다릅니다.\n기기의 설정 - 날짜 및 시간을 자동 설정으로 체크해주세요.", Application.Quit);
            }
            
            LogPrint.EditorPrint("22222 " + (GetLoginSDate - GetLoginSystemDate.AddHours(9)).TotalSeconds);
            return false;
        }

        //LogPrint.PrintError(string.Format("{0:u}", GetLoginSDate));
        //System.TimeSpan sp = System.DateTime.UtcNow - GetLoginSystemDate;
        //LogPrint.Print("sp : " + sp + ", 현재 서버 시간 : " + STime + ", 계산 : " + GetLoginSDate.AddSeconds(sp.TotalSeconds));

        
    }

    /// <summary>
    /// 현재 DateTime
    /// </summary>
    public DateTime GetNowTime()
    {
        TimeSpan sp = DateTime.UtcNow - GetLoginSystemDate;
        return GetLoginSDate.AddSeconds(sp.TotalSeconds);
    }

    CancellationTokenSource tokenSource_SendAllDB;
    public async void SendAllDB()
    {
        tokenSource_SendAllDB?.Cancel();
        tokenSource_SendAllDB = new CancellationTokenSource();

        try
        {
            int TotalMilliseconds = (int)(DateTime.Now.AddMinutes(60).Subtract(DateTime.Now).TotalMilliseconds);
            await Task.Delay(TotalMilliseconds, tokenSource_SendAllDB.Token);
            await SendFocusDB();

            SendAllDB();
        }
        catch (TaskCanceledException ex)
        {
            LogPrint.EditorPrint(ex.Message);
        }
        finally
        {
            tokenSource_SendAllDB.Dispose();
            tokenSource_SendAllDB = null;
        }
    }

    CancellationTokenSource tokenSource;
    /// <summary>
    /// 일일 리셋 체크 루프 
    /// </summary>
    public async void LoopCheckingResetToday()
    {
        tokenSource?.Cancel();
        tokenSource = new CancellationTokenSource();

        try
        {
            int TotalMilliseconds = (int)(DateTime.Today.AddDays(1).Subtract(DateTime.Now).TotalMilliseconds);
            await Task.Delay(TotalMilliseconds, tokenSource.Token);
            await Task.Delay(15000);
            await ResetToday();
            await GameDatabase.GetInstance().attendanceDB.GetIsCheckAttendance();
            await Task.Delay(2500);
            GameDatabase.GetInstance().publicContentDB.ASetPub_CharData(BackendGpgsMng.tableName_Pub_NowCharData); // 캐릭터 데이터 전송 
            await Task.Delay(2500);

            LoopCheckingResetToday();
            
        }
        catch (TaskCanceledException ex)
        {
            LogPrint.EditorPrint(ex.Message);
        }
        finally
        {
            tokenSource.Dispose();
            tokenSource = null;
        }
    }

    CancellationTokenSource tokenSourceRankScoreSend;

    public async void ScoreSendLoop()
    {
        tokenSourceRankScoreSend?.Cancel();
        tokenSourceRankScoreSend = new CancellationTokenSource();

        try
        {
            int TotalMilliseconds = (int)(DateTime.Now.AddMinutes(10).Subtract(DateTime.Now).TotalMilliseconds);
            await Task.Delay(TotalMilliseconds, tokenSourceRankScoreSend.Token);
            BackendGpgsMng.GetInstance().SendScore();
            ScoreSendLoop();
        }
        catch (Exception ex)
        {
            LogPrint.EditorPrint(ex.Message);
        }
        finally
        {
            tokenSourceRankScoreSend.Dispose();
            tokenSourceRankScoreSend = null;
        }
    }

    private string loop_access_ymd = "";
    /// <summary>
    /// 일일 리셋 
    /// </summary>
    public async Task ResetToday()
    {
        if (string.IsNullOrEmpty(loop_access_ymd))
        {
            var user_info = GameDatabase.GetInstance().tableDB.GetUserInfo();
            loop_access_ymd = user_info.m_access_ymd;
        }

        string nowYmd = GameDatabase.GetInstance().attendanceDB.DailyYmd();
        if (string.Equals(loop_access_ymd, nowYmd) == false)
        {
            LogPrint.Print("<color=yellow> ----- OK 11111 접속 날짜가 다음날로 넘어갔습니다 loop_access_ymd : " + loop_access_ymd + ", nowYmd : " + nowYmd + " ----- </color>");

            // 리셋 데이터들 처리 
            Task tsk1 = GameDatabase.GetInstance().dailyMissionDB.ResetDailyMission(); // 리셋 데이터 
            Task tsk2 = GameDatabase.GetInstance().tableDB.ResetDailyUserInfo(nowYmd); // 리셋 데이터 

            while (Loading.Bottom(tsk1.IsCompleted, tsk2.IsCompleted) == false) await Task.Delay(100);

            PopUpMng.GetInstance().OnInit_DailyProductReward(); // 기간제 아이템 리셋 

            if (MainUI.GetInstance().tapIAP.tapShopItem.gameObject.activeSelf)
                MainUI.GetInstance().tapIAP.tapShopItem.SetData();

            if (MainUI.GetInstance().tapDungeon.gameObject.activeSelf)
                MainUI.GetInstance().tapDungeon.Ticket();

            var user_info = GameDatabase.GetInstance().tableDB.GetUserInfo();
            loop_access_ymd = user_info.m_access_ymd;
            LogPrint.Print("<color=yellow> ----- OK 22222 접속 날짜가 다음날로 넘어갔습니다 loop_access_ymd : " + loop_access_ymd + ", nowYmd : " + nowYmd + " ----- </color>");
        }
        else
        {
            LogPrint.Print("<color=red> ----- NO 접속 날짜가 같습니다. loop_access_ymd : " + loop_access_ymd + ", nowYmd : " + nowYmd + " ----- </color>");
        }
    }

    /// <summary>
    /// 자정까지의 DateTime  
    /// </summary>
    public DateTime Get24Hour()
    {
        var nDate = GetNowTime();
        return new DateTime(nDate.Year, nDate.Month, nDate.Day).AddDays(1);
    }
#endregion

#region ##### 각 플랫폼별 로그인 및 가입 
    public void LoginType()
    {
        string getLoginType = PlayerPrefs.GetString("LoginType");
        Debug.Log("getLoginType:" + getLoginType);

#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        ABackendLoginGuest();
#elif UNITY_ANDROID
        ABackendLoginGoogle ();
#elif UNITY_IOS
        ABackendLoginIos ();
#endif
    }

    // ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
#region ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣ GUEST or EDITOR 
    // 게스트 로그인 및 가입 
    public async void ABackendLoginGuest()
    {
        AGetLoadText("로그인중...");
        //Backend.BMember.DeleteGuestInfo();
        LogPrint.Print("GetGuestID : " + Backend.BMember.GetGuestID());
        LogPrint.Print("----------------A LoginGuest");
        BackendReturnObject bro1 = null;
#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
        SendQueue.Enqueue(Backend.BMember.GuestLogin, callback => { bro1 = callback; });
#else
        string customId = string.Format("{0}_a{1}", SystemInfo.deviceUniqueIdentifier.ToString(), 25);
        Debug.Log("c392f70c109020559d7713800dfe529c2f254503_a25, customId : " + customId);
        SendQueue.Enqueue(Backend.BMember.CustomSignUp, customId, customId, callback => { bro1 = callback; });
#endif
        while (bro1 == null) { await Task.Delay(100); }
        LogPrint.Print("----------------A LoginGuest " + bro1);

        // 성공시 - Update() 문에서 토큰 저장 
        //Backend.BMember.SaveToken(bro1);
        if (bro1.IsSuccess())
        {
            AGetBackendGetUserInfo("guest");
            ShowMessage("login.guestSuccess", 2f); // 게스트 상태로 로그인을 하였습니다. 기기 변경시 데이터 초기화 됩니다.
            PlayerPrefs.SetString("LoginType", "guest");
        }
        else
        {
            switch (bro1.GetStatusCode())
            {
                case "401": // 존재하지 않는 아이디의 경우
                    GameObject.FindObjectOfType<BackendUI>().PopUpNickName("guest", false);
                    LogPrint.PrintError("존재하지 않는 아이디의 경우");
                    break;
                case "409": // 중복된 customId 가 존재하는 경우
                    LogPrint.PrintError("중복된 customId 가 존재하는 경우");
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
                    bro1 = null;
                    SendQueue.Enqueue(Backend.BMember.CustomLogin, customId, customId, callback => { bro1 = callback; });
                    while (bro1 == null) { await Task.Delay(100); }

                    LogPrint.Print("----------------A CustomLogin customId : " + customId +", bro1 : " + bro1);
                    AGetBackendGetUserInfo("guest");
#endif
                    break;
                case "403": // 차단당한 유저
                    LogPrint.PrintError("차단된 유저인 경우");
                    break;
            }
        }
    }
#endregion

    // ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
#region ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣ GOOGLE 
    private int MAX_LOGIN = 0;
    // 구글 토큰 받아옴
    public string GetTokens()
    {
        if (PlayGamesPlatform.Instance.localUser.authenticated)
        {
            // 유저 토큰 받기 첫번째 방법
            string _IDtoken = PlayGamesPlatform.Instance.GetIdToken();
            // 두번째 방법
            // string _IDtoken = ((PlayGamesLocalUser)Social.localUser).GetIdToken();
            return _IDtoken;
        }
        else
        {
            Debug.Log("접속되어있지 않습니다. PlayGamesPlatform.Instance.localUser.authenticated :  fail");
            return string.Empty;
        }
    }

    // 구글 로그인 및 가입 
    public async void ABackendLoginGoogle()
    {
        LogPrint.Print("Social.localUser.authenticated : " + Social.localUser.authenticated);
       
        // 이미 로그인 된 경우
        if (Social.localUser.authenticated == true)
        {
            // 구글 토큰일 이용해 뒤끝에 로그인 한다. (미가입자의 경우 자동 가입됨) 
            LogPrint.Print("----------------A LoginGoogle");
            BackendReturnObject bro1 = null;
            SendQueue.Enqueue(Backend.BMember.AuthorizeFederation, GetTokens(), FederationType.Google, "gpgs", callback => { bro1 = callback; });
            while (bro1 == null) { await Task.Delay(100); }
            LogPrint.Print("----------------A LoginGoogle " + bro1);

            //Backend.BMember.SaveToken(bro1);
            if (bro1.IsSuccess()) // 가입 된 계정 및 로그인 성공 
            {
                AGetBackendGetUserInfo("google");
                ShowMessage("login.googleSuccess", 2f); // 구글 계정을 사용하여 로그인을 하였습니다. 
                PlayerPrefs.SetString("LoginType", "android");
            }
            else // 미가입자 
            {
                LogPrint.PrintError("Backend Google 미 가입자 -> 닉네임 설정창 팝업");
            }
        }
        else
        {
            Social.localUser.Authenticate((bool success) =>
           {
               if (success)
               {
                   LogPrint.Print("Google 로그인 성공");
                   ABackendLoginGoogle();
               }
               else
               {
                   if (MAX_LOGIN < 3)
                   {
                       LogPrint.Print("Google 로그인 실패");
                       this.ShowMessage("login.googleFail", 2f); // 구글 로그인 실패하였습니다. 다시 시도합니다.
                        ABackendLoginGoogle();
                   }
                   else
                   {
                       LogPrint.PrintError("Google 최대 로그인 시도 초과");
                        // 게스트로 시작하도록 한다. 
                        ABackendLoginGuest();
                   }
                   MAX_LOGIN++;
               }
           });
        }
    }
#endregion

    // ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
#region ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣ IOS 
    // IOS 로그인 및 가입  
    private async void ABackendLoginIos()
    {

    }
#endregion

#endregion

#region ##### 닉네임 생성 및 변경 
    //  닉네임 생성 
    public async void ACreateNickName(string os_ty, string nickName)
    {
        LogPrint.Print("닉네임 생성  createType : " + os_ty + ", nickName : " + nickName);
        LogPrint.Print("----------------A SignNickName");
        BackendReturnObject bro1 = null;
        SendQueue.Enqueue(Backend.BMember.CreateNickname, nickName, callback => { bro1 = callback; });
        while (bro1 == null) { await Task.Delay(100); }
        LogPrint.Print("----------------A SignNickName " + bro1);

        if (bro1.IsSuccess())
        {
            // 푸시 ON 
            if(isEditor == false)
            {
                Backend.Android.PutDeviceToken();
            }

            AGetBackendGetUserInfo(os_ty);
        }
        else
        {
            if(bro1.GetStatusCode() == "409")
            {
                GameObject.FindObjectOfType<BackendUI>().PopUpNickName(os_ty, true);
            }
        }
    }

    // 닉네임 변경 
    public async void AUpdateNickName(string chng_nickName)
    {
        LogPrint.Print("닉네임 변경 chng_nickName : " + chng_nickName);
        LogPrint.Print("----------------A SignNickName");
        BackendReturnObject bro1 = null;
        SendQueue.Enqueue(Backend.BMember.UpdateNickname, chng_nickName, callback => { bro1 = callback; });
        while (bro1 == null) { await Task.Delay(100); }
        LogPrint.Print("----------------A SignNickName " + bro1);

        if (bro1.IsSuccess())
        {

        }
        else
        {

        }
    }
#endregion

    // ##########################################################################
    // ##########################################################################
    // ##########################################################################
#region #####   뒤끝 데이터  #####
    /// <summary> 뒤끝 유저 정보 (InDate, nickname) </summary>
    public async void AGetBackendGetUserInfo(string os_ty)
    {
        // 뒤끝 유저 정보 (InDate, nickname)
        LogPrint.Print("----------------A GetBackendGetUserInfo");
        BackendReturnObject bro1 = null;
        SendQueue.Enqueue(Backend.BMember.GetUserInfo, callback => { bro1 = callback; });
        while (bro1 == null) { await Task.Delay(100); }
        LogPrint.Print("----------------A GetBackendGetUserInfo " + bro1);

        if (bro1.IsSuccess())
        {
            JsonData row = bro1.GetReturnValuetoJSON()["row"];
            if (row["nickname"] == null)
            {
                GameObject.FindObjectOfType<BackendUI>().PopUpNickName(os_ty, false);
            }
            else
            {
                backendUserInfo = new BackendGpgsMng.BackendUserInfo()
                {
                    m_nickname = row["nickname"].ToString(),
                    m_indate = row["inDate"].ToString(),
                };
                
                byte[] bytesForEncoding = Encoding.UTF8.GetBytes(backendUserInfo.m_indate);
                PrefsKeys.id = Convert.ToBase64String(bytesForEncoding);
                GameObject.FindObjectOfType<BackendUI>().BackendWaitingToGameScene();
                AGetTableList(); // 유저 게임 데이터 정보
                //GetRemoveAds(); // 유저 광고제거 구매정보 가져오기 후 저장
                //UpdateScore2(0);// 최고점수 갱신
                //BackEndChatManager.instance.EnterChatServer(); //채팅서버 접속
            }
        }
        else
        {
            LogPrint.PrintError("유저 정보 오류");
            return;
        }
    }

    // 뒤끝 테이블 데이터 로드 완료 상태 
    [SerializeField]
    private bool isLoadTable, isLoadChart, isLoadGoods, isLoadUserInfo, isLoadEquip, isLoadItem, isLoadSkill, isLoadRank, isLoadInventory, isEncy, isLoadPet, isLoadPetEncy;
    public bool IsAllDataLoaded => 
        isLoadTable && isLoadChart && isLoadGoods && isLoadUserInfo && isLoadEquip && isLoadItem && isLoadSkill && isLoadRank && isLoadInventory && isEncy && isLoadPet && isLoadPetEncy;

    //###################################################################################
#region ---------------------- AGet Load Table DB, Chart Load -----------------------
    int failCnt = 0;
    /// <summary>
    /// 테이블 리스트 
    /// </summary>
    private async void AGetTableList()
    {
        AGetLoadText("데이터 테이블 확인중...");
        Backend.Notification.Connect();
        Backend.Notification.OnAuthorize = (bool Result, string Reason) =>
        {
            LogPrint.Print("<color=yellow> Backend.Notification.OnAuthorize Result : " + Result + ", Reason : " + Reason + "</color>");
        };

        LogPrint.EditorPrint("----------------A GetTableList");
        
        BackendReturnObject bro1 = null;
        SendQueue.Enqueue(Backend.GameInfo.GetTableList, callback => { bro1 = callback; }); // 데이터 테이블 리스트 
        while (bro1 == null) { await Task.Delay(100); }
        LogPrint.EditorPrint("----------------A GetTableList " + bro1);

        if (bro1.GetStatusCode() == "400" && failCnt < 3)
        {
            failCnt++;
            Backend.Initialize(HandleBackendCallback);
        }
        else
        {
            if (bro1.IsSuccess())
            {
                //SetTable(bro1.GetReturnValuetoJSON());
                AGetTableData();
                isLoadTable = true;
            }
            else
            {
                LogPrint.PrintError("테이블 리스트 불러오기 실패");
            }
        }
    }

    /// <summary> 각종 데이터 로드 </summary>
    private async void AGetTableData()
    {
        LogPrint.Print("<color=yellow>---------------- A GetTableData -------------------</color>");
        var chk_chart_version = await AGetChartVersion();
        if (string.Equals(PlayerPrefs.GetString("chart_version"), chk_chart_version) == false)
        {
            await AGetChart();
            PlayerPrefs.SetString("chart_version", chk_chart_version);
        }

        isLoadChart = GameDatabase.GetInstance().chartDB.PaserChart();
        if (isLoadChart)
        {
            AGetLoadText("차트 로드중...");
            await AGetProbabilityCardList();

            AGetLoadText("재화 로드중...");
            await AGetTBCProduct();
            await AGetGoods();

            AGetLoadText("데이터 정보 로드중...");
            await AGetUserInfo();
            if (GameDatabase.GetInstance().tableDB.GetUserInfo().cheat_gotch_bitch > 0)
            {
                GameObject.FindObjectOfType<BackendUI>().IntroNotice("부정 프로그램을 사용한 기록이있습니다. 게임이 종료됩니다.", Application.Quit);
                return;
            }

            await AGetEquipment();
            await AGetItem();
            await AGetSkill();
            await AGetPet();
            await AGetPetEncy();

            AGetLoadText("오늘 할일 찾는중...");
            await AGetAchievements();
            await AGetDailyMission();
            await AGetQuest();
            await AGetEquipmentEncyclopedia();

            AGetLoadText("컨텐츠 정보 로드중...");
            await AGetDungeonTop();
            await AGetDungeonMine();
            await AGetDungeonRaid();
            await AGetRTRankList();
            await GameDatabase.GetInstance().rankDB.AGetRTRank(GameDatabase.RankDB.Enum_RTRankType.Rank_RT_ChptStgTop50MyAround10);

            AGetLoadText("백업 데이터 로드중...");
            await AGetInventoryBackUp();

            AGetLoadText("게임씬 로드중...");
        }
        else
        {
            LogPrint.PrintError("------차트 데이터 오류------");
        }
    }

    private Text loadingSubTxt = null;
    private int lCnt = 0, lMaxCnt = 9;
    private void AGetLoadText(string txt, bool noCnt = false)
    {
        if(!noCnt)
            lCnt++;

        if(loadingSubTxt == null)
            loadingSubTxt = LoadSceneMng.GetInstance().txProgressSubTxt;

        if (loadingSubTxt)
        {
            if(!noCnt)
                loadingSubTxt.text = string.Format("{0} ({1}/{2})", txt, lCnt, lMaxCnt);
            else loadingSubTxt.text = string.Format("{0}", txt);
        }
    }

    private async Task<string> AGetChartVersion()
    {
        BackendReturnObject bro1 = null;
        SendQueue.Enqueue(Backend.Chart.GetChartContents, "10852", callback => { bro1 = callback; });
        while (bro1 == null) { await Task.Delay(100); }

        LogPrint.EditorPrint("version bro1 : " + bro1);
        if (bro1.IsSuccess())
        {
            return bro1.GetReturnValuetoJSON()["rows"][0]["version"]["S"].ToString();
        }
        else return "-1";
    }

    private async Task AGetChart()
    {
        LogPrint.Print("----------------A GetChart");
        BackendReturnObject bro1 = null;
        SendQueue.Enqueue(Backend.Chart.GetAllChartAndSave, true, callback => { bro1 = callback; });
        while (bro1 == null) { await Task.Delay(100); }

        LogPrint.Print("----------------A GetChart " + bro1);
    }

    /// <summary>
    /// 확률 차트 리스트 id 
    /// </summary>
    private async Task AGetProbabilityCardList()
    {
        LogPrint.Print("----------------A GetProbabilityCardList");
        BackendReturnObject bro1 = null;
        SendQueue.Enqueue(Backend.Probability.GetProbabilityCardList, callback => { bro1 = callback; });
        while (bro1 == null) { await Task.Delay(100); }
        LogPrint.Print("----------------A GetProbabilityCardList " + bro1);

        GameDatabase.GetInstance().chartProbabilityDB.SetGachyaChartParsingID(bro1);
    }
#endregion

    private async Task AGetTBCProduct()
    {
        BackendReturnObject bro1 = null;
        SendQueue.Enqueue(Backend.TBC.GetProductList, callback => { bro1 = callback; });
        while (bro1 == null) { await Task.Delay(100); }
        if(bro1.IsSuccess())
        {
            GameDatabase.GetInstance().tableDB.SetTBCProduct(bro1.GetReturnValuetoJSON());
        }
        else Debug.Log("ERROR A GetTBCProduct");
    }

    //###################################################################################
#region --------------------------- Table : Goods ---------------------------
    /// <summary>
    /// 재화 데이터 
    /// </summary>
    private async Task AGetGoods ()
    {
        LogPrint.Print("----------------A GetGoods");
        BackendReturnObject bro1 = null;
        SendQueue.Enqueue(Backend.GameInfo.GetPrivateContents, tableName_Goods, 100, callback => { bro1 = callback; });
        while (bro1 == null) { await Task.Delay(100); }

        LogPrint.Print("----------------A GetGoods " + bro1);
        if (bro1.IsSuccess())
        {
            if (bro1.GetReturnValuetoJSON()["rows"].Count > 0)
                CSetLoadGoods(bro1.GetReturnValuetoJSON());
            else
                SSetInsertGoods();
        }
        else Debug.Log("ERROR A GetGoods");
    }

    /// <summary>
    /// goods Client SET 데이터 세팅 
    /// </summary>
    void CSetLoadGoods(JsonData returnData)
    {
        LogPrint.Print("CSetLoadGoods");
        if (returnData != null)
        {
            if (returnData.Keys.Contains("rows"))
            {
                JsonData row = returnData["rows"][0];
                GameDatabase.TableDB.Goods goods = new GameDatabase.TableDB.Goods()
                {
                    indate = RowPaser.StrPaser(row, "inDate"),
                    m_gold = RowPaser.LongPaser(row, "m_gold"),
                    m_dia = RowPaser.IntPaser(row, "m_dia"),
                    m_ether = RowPaser.IntPaser(row, "m_ether"),
                    m_ruby = RowPaser.IntPaser(row, "m_ruby"),
                    m_battle_coin = RowPaser.IntPaser(row, "m_battle_coin"),

                    m_30DayTbc = RowPaser.IntPaser(row, "m_30DayTbc"),
                    m_30DayTbcDailyRewardDate = RowPaser.StrPaser(row, "m_30DayTbcDailyRewardDate"),
                    m_30DayTbcStartDate = RowPaser.StrPaser(row, "m_30DayTbcStartDate"),
                    m_30DayTbcEndDate = RowPaser.StrPaser(row, "m_30DayTbcEndDate"),
                    m_7DayTbc = RowPaser.IntPaser(row, "m_7DayTbc"),
                    m_7DayTbcDailyRewardDate = RowPaser.StrPaser(row, "m_7DayTbcDailyRewardDate"),
                    m_7DayTbcStartDate = RowPaser.StrPaser(row, "m_7DayTbcStartDate"),
                    m_7DayTbcEndDate = RowPaser.StrPaser(row, "m_7DayTbcEndDate"),
                    m_fr_ticket_dg_top = RowPaser.IntPaser(row, "m_fr_ticket_dg_top"),
                    m_fr_ticket_dg_mine = RowPaser.IntPaser(row, "m_fr_ticket_dg_mine"),
                    m_fr_ticket_dg_raid = RowPaser.IntPaser(row, "m_fr_ticket_dg_raid"),
                    m_fr_ticket_pvp_arena = RowPaser.IntPaser(row, "m_fr_ticket_pvp_arena"),
                };

                LogPrint.Print(JsonUtility.ToJson(goods));
                GameDatabase.GetInstance().tableDB.SetInitDB_Goods(goods);
                isLoadGoods = true;
            }
        } else Debug.Log("contents has no data");
    }

    /// <summary>
    /// goods Server SET 첫 세팅 
    /// </summary>
    private async void SSetInsertGoods()
    {
        JSONObject jd = JSONObject.Create(JsonUtility.ToJson(new GameDatabase.TableDB.Goods()));
        Param param = new Param();
        try
        {
            foreach (string key in jd.keys)
            {
                if (!System.Object.Equals(key, "indate"))
                    param.Add(key, (int)jd[key].n);
            }
        }
        catch (Exception e) { Debug.Log(e); }

        BackendReturnObject bro1 = null;
        SendQueue.Enqueue(Backend.GameInfo.Insert, tableName_Goods, param, callback => { bro1 = callback; });
        while (bro1 == null) { await Task.Delay(100); }

        if (bro1.IsSuccess())
        {
            GameDatabase.GetInstance().tableDB.SetInitDB_Goods(new GameDatabase.TableDB.Goods() { indate = bro1.GetReturnValuetoJSON()["inDate"].ToString() });
            isLoadGoods = true;
        }
        else LogPrint.PrintError("SSetInsertGoods insert error");
    }
#endregion

    //###################################################################################
#region --------------------------- Table : UserInfo ---------------------------
    /// <summary>
    /// 유저 정보 
    /// </summary>
    private async Task AGetUserInfo()
    {
        LogPrint.Print("----------------A GetUserInfo ");
        BackendReturnObject bro1 = null;
        SendQueue.Enqueue(Backend.GameInfo.GetPrivateContents, tableName_UserInfo, 100, callback => { bro1 = callback; });
        while (bro1 == null) { await Task.Delay(100); }

        LogPrint.Print("----------------A GetUserInfo " + bro1);
        if (bro1.IsSuccess())
        {
            if (bro1.GetReturnValuetoJSON()["rows"].Count > 0)
                CSetLoadUserInfo(bro1.GetReturnValuetoJSON());
            else
                SSetInsertUserInfo();
        }
        else Debug.Log("ERROR A GetUserInfo");
    }

    /// <summary>
    /// goods Client SET 데이터 세팅 
    /// </summary>
    void CSetLoadUserInfo(JsonData returnData)
    {
        // ReturnValue가 존재하고, 데이터가 있는지 확인
        if (returnData != null)
        {
            if (returnData.Keys.Contains("rows"))
            {
                JsonData row = returnData["rows"][0];
                GameDatabase.TableDB.UserInfo user_info = new GameDatabase.TableDB.UserInfo()
                {
                    indate = RowPaser.StrPaser(row, "inDate"),

                    cheat_gotch_bitch = RowPaser.LongPaser(row, "cheat_gotch_bitch"),
                    m_access_ymd = RowPaser.StrPaser(row, "m_access_ymd"),
                    m_game_end_date = RowPaser.StrPaser(row, "m_game_end_date"),
                    m_attend_ymd = RowPaser.StrPaser(row, "m_attend_ymd"),
                    m_attend_nbr = RowPaser.IntPaser(row, "m_attend_nbr"),

                    m_char_lv = RowPaser.IntPaser(row, "m_char_lv"),
                    m_eq_lgnd_upgrd_rt7_p = RowPaser.IntPaser(row, "m_eq_lgnd_upgrd_rt7_p"),
                    m_ac_synt_rt6_p = RowPaser.IntPaser(row, "m_ac_synt_rt6_p"),
                    m_ac_advc_rt7_p = RowPaser.IntPaser(row, "m_ac_advc_rt7_p"),
                    m_ac_synt_fail_cnt = RowPaser.IntPaser(row, "m_ac_synt_fail_cnt"),
                    m_ac_sync_succ_cnt = RowPaser.IntPaser(row, "m_ac_sync_succ_cnt"),
                    m_ac_advc_fail_cnt = RowPaser.IntPaser(row, "m_ac_advc_fail_cnt"),
                    m_ac_advc_succ_cnt = RowPaser.IntPaser(row, "m_ac_advc_succ_cnt"),
                    m_invn_lv = RowPaser.IntPaser(row, "m_invn_lv"),

                    m_pet_synt_bns_pct = RowPaser.IntPaser(row, "m_pet_synt_bns_pct"),
                    m_pet_opch_red_cnt = RowPaser.IntPaser(row, "m_pet_opch_red_cnt"),
                    m_pet_opch_blue_cnt = RowPaser.IntPaser(row, "m_pet_opch_blue_cnt"),
                    m_pet_synt_red_cnt = RowPaser.IntPaser(row, "m_pet_synt_red_cnt"),
                    m_pet_synt_blue_cnt = RowPaser.IntPaser(row, "m_pet_synt_blue_cnt"),
                    acheive_pet_rt3 = RowPaser.IntPaser(row, "acheive_pet_rt3"),
                    acheive_pet_rt4 = RowPaser.IntPaser(row, "acheive_pet_rt4"),
                    acheive_pet_rt5 = RowPaser.IntPaser(row, "acheive_pet_rt5"),
                    acheive_pet_rt6 = RowPaser.IntPaser(row, "acheive_pet_rt6"),
                    acheive_pet_rt7 = RowPaser.IntPaser(row, "acheive_pet_rt7"),

                    // 던전 클리어 정보 
                    m_dg_top_nbr_ret = RowPaser.IntPaser(row, "m_dg_top_nbr_ret"),
                    m_dg_top_nbr = RowPaser.IntPaser(row, "m_dg_top_nbr"),
                    m_dg_mine_nbr = RowPaser.IntPaser(row, "m_dg_mine_nbr"),
                    m_dg_raid_nbr = RowPaser.IntPaser(row, "m_dg_raid_nbr"),

                    // 장비 자동 판매/분해 (결제) 
                    m_auto_eq_sale_permanent = RowPaser.IntPaser(row, "m_auto_eq_sale_permanent"),
                    m_auto_eq_sale_date = RowPaser.StrPaser(row, "m_auto_eq_sale_date"),

                    // 장비 자동 판매/분해 (광고)
                    m_auto_eq_sale_video_lv = RowPaser.IntPaser(row, "m_auto_eq_sale_video_lv"),
                    m_auto_eq_sale_video_date = RowPaser.StrPaser(row, "m_auto_eq_sale_video_date"),

                    // 랭킹에 사용됨 
                    m_clar_chpt = RowPaser.IntPaser(row, "m_clar_chpt"), // 챕터 유저 보스에만 사용됨 (리스트 NO) 
                    m_chpt_stg_nbr = RowPaser.IntPaser(row, "m_chpt_stg_nbr"),
                    m_pvp_win_streak = RowPaser.IntPaser(row, "m_pvp_win_streak"),
                    m_pvp_score = RowPaser.IntPaser(row, "m_pvp_score"),
                    m_pvp_today_score = RowPaser.IntPaser(row, "m_pvp_today_score"),
                    m_combat_score = RowPaser.IntPaser(row, "m_combat_score"),

                    // 던전 일일 티켓 구매한 수량 
                    m_daily_buy_ticket_dg_top = RowPaser.IntPaser(row, "m_daily_buy_ticket_dg_top"),
                    m_daily_buy_ticket_dg_mine = RowPaser.IntPaser(row, "m_daily_buy_ticket_dg_mine"),
                    m_daily_buy_ticket_dg_raid = RowPaser.IntPaser(row, "m_daily_buy_ticket_dg_raid"),
                    m_daily_buy_ticket_dg_pvp = RowPaser.IntPaser(row, "m_daily_buy_ticket_dg_pvp"),

                    // 아이템 일일 구매한 수량 
                    m_daily_buy_eq_ehnt_ston_rt1 = RowPaser.IntPaser(row, "m_daily_buy_eq_ehnt_ston_rt1"),  // 장비 강화석  
                    m_daily_buy_eq_ehnt_ston_rt2 = RowPaser.IntPaser(row, "m_daily_buy_eq_ehnt_ston_rt2"),
                    m_daily_buy_eq_ehnt_ston_rt3 = RowPaser.IntPaser(row, "m_daily_buy_eq_ehnt_ston_rt3"),
                    m_daily_buy_ac_ehnt_ston_rt1 = RowPaser.IntPaser(row, "m_daily_buy_ac_ehnt_ston_rt1"),  // 장신구 강화석 
                    m_daily_buy_ac_ehnt_ston_rt2 = RowPaser.IntPaser(row, "m_daily_buy_ac_ehnt_ston_rt2"),
                    m_daily_buy_ac_ehnt_ston_rt3 = RowPaser.IntPaser(row, "m_daily_buy_ac_ehnt_ston_rt3"),
                    m_daily_buy_ehnt_bless_rt1 = RowPaser.IntPaser(row, "m_daily_buy_ehnt_bless_rt1"),      // 강화 축복 주문서 
                    m_daily_buy_ehnt_bless_rt2 = RowPaser.IntPaser(row, "m_daily_buy_ehnt_bless_rt2"),
                    m_daily_buy_ehnt_bless_rt3 = RowPaser.IntPaser(row, "m_daily_buy_ehnt_bless_rt3"),

                    m_ad_removal = RowPaser.IntPaser(row, "m_ad_removal"),// 광고 제거 
                    m_ad_ruby_date = RowPaser.StrPaser(row, "m_ad_ruby_date"),// 광고 루비  
                    m_ad_ether_date = RowPaser.StrPaser(row, "m_ad_ether_date"),// 광고 에테르 

                    // 장신구, 장비 무료 소환 
                    m_free_acce_sohwan = RowPaser.StrPaser(row, "m_free_acce_sohwan"),
                    m_free_equip_sohwan = RowPaser.StrPaser(row, "m_free_equip_sohwan"),
                    m_free_pet_sohwan = RowPaser.StrPaser(row, "m_free_pet_sohwan"),
                    m_free_pet_sohwan_redate = RowPaser.StrPaser(row, "m_free_pet_sohwan_redate"),
                };

                GameDatabase.GetInstance().tableDB.SetInitTableDB_UserInfo(user_info);
                isLoadUserInfo = true;
            }
        } else Debug.Log("contents has no data");
    }

    /// <summary>
    /// goods Server SET 첫 세팅 
    /// </summary>
    private async void SSetInsertUserInfo()
    {
        LogPrint.Print("SSetInsertUserInfo");
        JSONObject jd = JSONObject.Create(JsonUtility.ToJson(new GameDatabase.TableDB.UserInfo()));
        Param param = new Param();
        try
        {
            foreach (string key in jd.keys)
            {
                if (!string.Equals(key, "indate"))
                {
                    if(string.Equals(key, "m_today_ymd"))
                        param.Add(key, GameDatabase.GetInstance().attendanceDB.DailyYmd());
                    else
                    {
                        param.Add(key, jd[key].n);
                    }
                }
            }
        }
        catch (Exception e) { Debug.Log(e); }

        BackendReturnObject bro1 = null;
        SendQueue.Enqueue(Backend.GameInfo.Insert, tableName_UserInfo, param, callback => { bro1 = callback; });
        while (bro1 == null) { await Task.Delay(100); }

        if (bro1.IsSuccess())
        {
            GameDatabase.GetInstance().tableDB.SetInitTableDB_UserInfo(new GameDatabase.TableDB.UserInfo() { indate = bro1.GetReturnValuetoJSON()["inDate"].ToString() });
            isLoadUserInfo = true; 
        }
        else
        { LogPrint.PrintError("SSetInsertUserInfo insert error"); }
    }
#endregion

    //###################################################################################
#region --------------------------- Table : Equipment ---------------------------
    /// <summary> 서버로 부터 장비 데이터를 불러온다. </summary>
    private async Task AGetEquipment()
    {
        LogPrint.EditorPrint("----------------A GetEquipment ");
        BackendReturnObject bro1 = null;
        SendQueue.Enqueue(Backend.GameInfo.GetPrivateContents, tableName_Equipment, 100, callback => { bro1 = callback; });
        while (bro1 == null) { await Task.Delay(100); }
        LogPrint.EditorPrint("----------------A GetEquipment " + bro1);

        string firstKey = string.Empty;
        LogPrint.EditorPrint("----------------A GetEquipment firstKey check");
        LogPrint.EditorPrint("----------------A GetEquipment firstKey check bro1.FirstKey() == null ? " + (bro1.FirstKey() == null));
        bool is_null_first_key = bro1.FirstKey() == null;
        LogPrint.EditorPrint("----------------A GetEquipment is_null_first_key : " + is_null_first_key);
        if (!is_null_first_key)
            firstKey = bro1.FirstKeystring();
        
        if (bro1.IsSuccess())
        {
            if (bro1.GetReturnValuetoJSON()["rows"].Count > 0)
                CFirstInitSetLoad_Equip(bro1.GetReturnValuetoJSON(), is_null_first_key);
            else
                await CSetInsertInitEquipment(true);
        }
        else Debug.Log("ERROR A GetEquipment");

        // 100개이상(101번째) 데이터가 더있을 경우 
        if (!is_null_first_key && !string.Equals(firstKey, string.Empty))
        {
            LogPrint.EditorPrint("----------------A GetEquipment 100개이상(101번째) 데이터가 더있을 경우 111");
            bro1 = null;
            SendQueue.Enqueue(Backend.GameInfo.GetPrivateContents, tableName_Equipment, firstKey, callback => { bro1 = callback; });
            while (bro1 == null) { await Task.Delay(100); }
            LogPrint.EditorPrint("----------------A GetEquipment 100개이상(101번째) 데이터가 더있을 경우 111 " + bro1);

            is_null_first_key = bro1.FirstKey() == null;
            if (!is_null_first_key)
                firstKey = bro1.FirstKeystring();

            if (bro1.IsSuccess())
            {
                if (bro1.GetReturnValuetoJSON()["rows"].Count > 0)
                    CFirstInitSetLoad_Equip(bro1.GetReturnValuetoJSON(), is_null_first_key);
            }

            if (!is_null_first_key && !string.Equals(firstKey, string.Empty))
            {
                LogPrint.EditorPrint("----------------A GetEquipment 100개이상(101번째) 데이터가 더있을 경우 222");
                bro1 = null;
                SendQueue.Enqueue(Backend.GameInfo.GetPrivateContents, tableName_Equipment, firstKey, callback => { bro1 = callback; });
                while (bro1 == null) { await Task.Delay(100); }
                LogPrint.EditorPrint("----------------A GetEquipment 100개이상(101번째) 데이터가 더있을 경우 222 " + bro1);

                if (bro1.IsSuccess())
                {
                    if (bro1.GetReturnValuetoJSON()["rows"].Count > 0)
                        CFirstInitSetLoad_Equip(bro1.GetReturnValuetoJSON(), is_null_first_key);
                }
            }
        }
    }

    ///  <summary> 처음 시작할때 기본장비 무기/방패를 지급한다. </summary>
    private async Task CSetInsertInitEquipment (bool isCompCheck)
    {
        Task<bool> bTask = GameDatabase.GetInstance().tableDB.SetInitFirstEquip();
        await bTask;
        if (bTask.Result)
        {
            if(isCompCheck)
                isLoadEquip = true;
        }
    }

    /// <summary> 불러온 장비 데이터를 클라이언트에 세팅한다. </summary>
    void CFirstInitSetLoad_Equip(JsonData returnData, bool isCompCheck)
    {
        if (returnData != null)
        {
            if (returnData.Keys.Contains("rows"))
            {
                int eq_use_slot = GameDatabase.GetInstance().tableDB.GetUseEquipSlot();
                foreach (JsonData row in returnData["rows"])
                {
                    GameDatabase.TableDB.Equipment equipment = new GameDatabase.TableDB.Equipment()
                    {
                        indate = RowPaser.StrPaser(row, "inDate"),
                        aInUid = RowPaser.LongPaser(row, "aInUid"),
                        m_state = RowPaser.IntPaser(row, "m_state"),
                        m_lck = RowPaser.IntPaser(row, "m_lck"),
                        eq_ty = RowPaser.IntPaser(row, "eq_ty"),
                        eq_rt = RowPaser.IntPaser(row, "eq_rt"),
                        eq_legend = RowPaser.IntPaser(row, "eq_legend"),
                        eq_legend_sop_id = RowPaser.IntPaser(row, "eq_legend_sop_id"),
                        eq_legend_sop_rlv = RowPaser.IntPaser(row, "eq_legend_sop_rlv"),
                        eq_id = RowPaser.IntPaser(row, "eq_id"),
                        m_ehnt_lv = RowPaser.IntPaser(row, "m_ehnt_lv"),
                        m_norm_lv = RowPaser.IntPaser(row, "m_norm_lv"),
                        ma_st_id = RowPaser.IntPaser(row, "ma_st_id"),
                        ma_st_rlv = RowPaser.IntPaser(row, "ma_st_rlv"),

                        st_sop_ac = JsonUtility.FromJson<GameDatabase.TableDB.StatOp>(RowPaser.StrPaser(row, "st_sop_ac")),
                        st_op = JsonUtility.FromJson<GameDatabase.TableDB.EquipStatOp>(RowPaser.StrPaser(row, "st_op")),
                        st_ms = JsonUtility.FromJson<GameDatabase.TableDB.EquipMagicSocket>(RowPaser.StrPaser(row, "st_ms")),
                    };

                    GameDatabase.GetInstance().tableDB.UpdateClientDB_Equip(equipment);
                }

                GameDatabase.GetInstance().tableDB.SortEquipment();
                if (isCompCheck)
                    isLoadEquip = true;
            }
        }
        else Debug.Log("contents has no data");
    }
    #endregion

    //###################################################################################
    #region --------------------------- Table : Pet, PetEncy ---------------------------
    private async Task AGetPet()
    {
        LogPrint.EditorPrint("----------------A GetPet ");
        BackendReturnObject bro1 = null;
        SendQueue.Enqueue(Backend.GameInfo.GetPrivateContents, tableName_Pet, 100, callback => { bro1 = callback; });
        while (bro1 == null) { await Task.Delay(100); }
        LogPrint.EditorPrint("----------------A GetPet " + bro1);

        if (bro1.IsSuccess())
        {
            if (bro1.GetReturnValuetoJSON()["rows"].Count > 0)
                CFirstInitSetLoad_Pet(bro1.GetReturnValuetoJSON());
            else
            {
                // 기본 관상용 펫 세팅 
                TransactionParam tParam = new TransactionParam();
                var dfPetDB1 = new GameDatabase.TableDB.Pet() { aInUid = GameDatabase.GetInstance().GetUniqueIDX(), m_state = 1, p_rt = 1, p_id = 1 };
                var dfPetDB2 = new GameDatabase.TableDB.Pet() { aInUid = GameDatabase.GetInstance().GetUniqueIDX(), m_state = 0, p_rt = 2, p_id = 1 };
                tParam.AddInsert(BackendGpgsMng.tableName_Pet, GameDatabase.GetInstance().tableDB.ParamCollectionPet(dfPetDB1));
                tParam.AddInsert(BackendGpgsMng.tableName_Pet, GameDatabase.GetInstance().tableDB.ParamCollectionPet(dfPetDB2));

                bro1 = null;
                SendQueue.Enqueue(Backend.GameInfo.TransactionWrite, tParam, callback => { bro1 = callback; });
                while (bro1 == null) await Task.Delay(100);

                LogPrint.EditorPrint("A GetPet AddInsert " + bro1.GetReturnValuetoJSON().ToJson());

                dfPetDB1.indate = bro1.GetReturnValuetoJSON()["putItem"][0]["inDate"].ToString();
                GameDatabase.GetInstance().tableDB.UpdateClientDB_Pet(dfPetDB1);
                dfPetDB2.indate = bro1.GetReturnValuetoJSON()["putItem"][1]["inDate"].ToString();
                GameDatabase.GetInstance().tableDB.UpdateClientDB_Pet(dfPetDB2);

                //bro1 = null;
                //SendQueue.Enqueue(Backend.GameInfo.Insert, tableName_Pet, GameDatabase.GetInstance ().tableDB.ParamCollectionPet(default_PetDB), callback => { bro1 = callback; });
                //while (bro1 == null) { await Task.Delay(100); }

                //default_PetDB.indate = bro1.GetReturnValuetoJSON()["inDate"].ToString();
                //GameDatabase.GetInstance().tableDB.UpdateClientDB_Pet (default_PetDB);
                isLoadPet = true;
            }
        }
        else Debug.Log("ERROR A GetPet");
    }

    /// <summary> 불러온 장비 데이터를 클라이언트에 세팅한다. </summary>
    void CFirstInitSetLoad_Pet(JsonData returnData)
    {
        if (returnData != null)
        {
            if (returnData.Keys.Contains("rows"))
            {
                foreach (JsonData row in returnData["rows"])
                {
                    GameDatabase.TableDB.Pet petDB = new GameDatabase.TableDB.Pet()
                    {
                        indate = RowPaser.StrPaser(row, "inDate"),
                        aInUid = RowPaser.LongPaser(row, "aInUid"),
                        p_id = RowPaser.IntPaser(row, "p_id"),
                        p_rt = RowPaser.IntPaser(row, "p_rt"),
                        p_lv = RowPaser.IntPaser(row, "p_lv"),
                        p_lv_residual = RowPaser.FloatPaser(row, "p_lv_residual"),
                        m_state = RowPaser.IntPaser(row, "m_state"),
                        m_lck = RowPaser.IntPaser(row, "m_lck"),
                    };

                    string str_sOp1 = RowPaser.StrPaser(row, "sOp1");
                    string str_sOp2 = RowPaser.StrPaser(row, "sOp2");
                    string str_sOp3 = RowPaser.StrPaser(row, "sOp3");
                    string str_statOp = RowPaser.StrPaser(row, "statOp");
                    LogPrint.EditorPrint("str_sOp1 : " + str_sOp1);
                    LogPrint.EditorPrint("str_sOp2 : " + str_sOp2);
                    LogPrint.EditorPrint("str_sOp3 : " + str_sOp3);
                    LogPrint.EditorPrint("str_statOp : " + str_statOp);

                    petDB.sOp1 = string.IsNullOrEmpty(str_sOp1) ? new TableDB.StatOp() : JsonUtility.FromJson<GameDatabase.TableDB.StatOp>(str_sOp1);
                    petDB.sOp2 = string.IsNullOrEmpty(str_sOp2) ? new TableDB.StatOp() : JsonUtility.FromJson<GameDatabase.TableDB.StatOp>(str_sOp2);
                    petDB.sOp3 = string.IsNullOrEmpty(str_sOp3) ? new TableDB.StatOp() : JsonUtility.FromJson<GameDatabase.TableDB.StatOp>(str_sOp3);
                    petDB.statOp = string.IsNullOrEmpty(str_statOp) ? new TableDB.PetStatOp() : JsonUtility.FromJson<GameDatabase.TableDB.PetStatOp>(str_statOp);

                    GameDatabase.GetInstance().tableDB.UpdateClientDB_Pet(petDB);
                }

                isLoadPet = true;
            }
        }
        else Debug.Log("contents has no data");
    }

    private async Task AGetPetEncy()
    {
        LogPrint.EditorPrint("----------------A GetPetEncy ");
        BackendReturnObject bro1 = null;
        SendQueue.Enqueue(Backend.GameInfo.GetPrivateContents, tableName_PetEncy, 100, callback => { bro1 = callback; });
        while (bro1 == null) { await Task.Delay(100); }
        LogPrint.EditorPrint("----------------A GetPetEncy " + bro1);

        if (bro1.IsSuccess())
        {
            if (bro1.GetReturnValuetoJSON()["rows"].Count > 0)
                CFirstInitSetLoad_PetEncy(bro1.GetReturnValuetoJSON());
            else
            {
                // 기본 관상용 펫 세팅 
                TransactionParam tParam = new TransactionParam();
                var dfPetDB1 = new GameDatabase.TableDB.PetEncy() { aInUid = GameDatabase.GetInstance().GetUniqueIDX(), p_rt = 1, p_id = 1, incr_cnt = 1 };
                var dfPetDB2 = new GameDatabase.TableDB.PetEncy() { aInUid = GameDatabase.GetInstance().GetUniqueIDX(), p_rt = 2, p_id = 1, incr_cnt = 1 };
                tParam.AddInsert(BackendGpgsMng.tableName_PetEncy, GameDatabase.GetInstance().tableDB.ParamCollectionPet(dfPetDB1));
                tParam.AddInsert(BackendGpgsMng.tableName_PetEncy, GameDatabase.GetInstance().tableDB.ParamCollectionPet(dfPetDB2));

                bro1 = null;
                SendQueue.Enqueue(Backend.GameInfo.TransactionWrite, tParam, callback => { bro1 = callback; });
                while (bro1 == null) await Task.Delay(100);

                LogPrint.EditorPrint("A GetPetEncy AddInsert " + bro1.GetReturnValuetoJSON().ToJson());

                dfPetDB1.indate = bro1.GetReturnValuetoJSON()["putItem"][0]["inDate"].ToString();
                GameDatabase.GetInstance().tableDB.UpdateClientDB_PetEncy(dfPetDB1);
                dfPetDB2.indate = bro1.GetReturnValuetoJSON()["putItem"][1]["inDate"].ToString();
                GameDatabase.GetInstance().tableDB.UpdateClientDB_PetEncy(dfPetDB2);

                isLoadPetEncy = true;
            }
        }
        else Debug.Log("ERROR A GetPet");
    }

    /// <summary> 불러온 장비 데이터를 클라이언트에 세팅한다. </summary>
    void CFirstInitSetLoad_PetEncy(JsonData returnData)
    {
        if (returnData != null)
        {
            if (returnData.Keys.Contains("rows"))
            {
                foreach (JsonData row in returnData["rows"])
                {
                    GameDatabase.TableDB.PetEncy petEncyDB = new GameDatabase.TableDB.PetEncy()
                    {
                        indate = RowPaser.StrPaser(row, "inDate"),
                        aInUid = RowPaser.LongPaser(row, "aInUid"),
                        p_id = RowPaser.IntPaser(row, "p_id"),
                        p_rt = RowPaser.IntPaser(row, "p_rt"),
                        incr_cnt = RowPaser.IntPaser(row, "incr_cnt"),
                    };

                    GameDatabase.GetInstance().tableDB.UpdateClientDB_PetEncy(petEncyDB);
                }

                isLoadPetEncy = true;
            }
        }
        else Debug.Log("contents has no data");
    }
    #endregion

    // ########################################################################################
    #region --------------------------- Table : Item ---------------------------
    /// <summary>
    /// 장비 데이터 
    /// </summary>
    private async Task AGetItem()
    {
        LogPrint.Print("----------------A GetItem ");
        BackendReturnObject bro1 = null;
        SendQueue.Enqueue(Backend.GameInfo.GetPrivateContents, tableName_Item, 100, callback => { bro1 = callback; });
        while (bro1 == null) { await Task.Delay(100); }
        LogPrint.Print("----------------A GetItem " + bro1);

        if (bro1.IsSuccess())
        {
            if (bro1.GetReturnValuetoJSON()["rows"].Count > 0)
                ClientInitSetLoad_Item(bro1.GetReturnValuetoJSON());
            else isLoadItem = true; // SSetInsertItem();
        }
        else Debug.Log("ERROR A GetItem");
    }

    /// <summary>
    /// Client SET 데이터 세팅 
    /// </summary>
    private void ClientInitSetLoad_Item(JsonData returnData)
    {
        LogPrint.Print("CFirstInitSetLoad Item");
        if (returnData != null)
        {
            if (returnData.Keys.Contains("rows"))
            {
                foreach (JsonData row in returnData["rows"])
                {
                    GameDatabase.TableDB.Item item = new GameDatabase.TableDB.Item()
                    {
                        indate = RowPaser.StrPaser(row, "inDate"),
                        aInUid = RowPaser.LongPaser(row, "aInUid"),
                        type = RowPaser.IntPaser(row, "type"),
                        rating = RowPaser.IntPaser(row, "rating"),
                        count = RowPaser.IntPaser(row, "count"),
                    };
                    GameDatabase.GetInstance().tableDB.UpdateClientDB_Item(item);
                    LogPrint.Print("CFirstInitSetLoad item : " + JsonUtility.ToJson(item));
                }

                GameDatabase.GetInstance().tableDB.SortItem();
                isLoadItem = true;
            }
        }
        else Debug.Log("contents has no data");
    }
#endregion

    // ########################################################################################
#region --------------------------- Table : Skill ---------------------------
    /// <summary>
    /// 스킬 데이터 
    /// </summary>
    private async Task AGetSkill()
    {
        LogPrint.Print("----------------A GetSkill ");
        BackendReturnObject bro1 = null;
        SendQueue.Enqueue(Backend.GameInfo.GetPrivateContents, tableName_Skill, 100, callback => { bro1 = callback; });
        while (bro1 == null) { await Task.Delay(100); }
        LogPrint.Print("----------------A GetSkill " + bro1);

        LogPrint.Print("bro1.IsSuccess() : " + bro1.IsSuccess());
        if (bro1.IsSuccess())
        {
            if (bro1.GetReturnValuetoJSON()["rows"].Count >= 6)
            {
                CFirstInitSetLoad_Skill(bro1.GetReturnValuetoJSON());
            }
            else
            {
                var broDb = bro1.GetReturnValuetoJSON()["rows"];
                for (int i = 1; i <= 6; i++)
                {
                    bool isExist = false;
                    if (broDb.Count > 0)
                    {
                        foreach (JsonData row in broDb)
                        {
                            if (RowPaser.IntPaser(row, "idx") == i)
                            {
                                isExist = true;
                                break;
                            }
                        }
                    }

                    if (!isExist)
                    {
                        var newSkDb = GameDatabase.GetInstance().tableDB.GetSkill(i);
                        Param parm = new Param();
                        parm.Add("aInUid", newSkDb.aInUid);
                        parm.Add("idx", newSkDb.idx);
                        parm.Add("level", newSkDb.level);
                        parm.Add("count", newSkDb.count);

                        BackendReturnObject broA = null;
                        SendQueue.Enqueue(Backend.GameInfo.Insert, tableName_Skill, parm, callback => { broA = callback; });
                        while (broA == null) await Task.Delay(100);

                        newSkDb.indate = broA.GetReturnValuetoJSON()["inDate"].ToString();
                        GameDatabase.GetInstance().tableDB.UpdateClientDB_Skill(newSkDb);
                    }

                    await Task.Delay(100);
                }

                isLoadSkill = true;
            }
        }
        else Debug.Log("ERROR A GetSkill");
    }
    /// <summary>
    /// Client SET 데이터 세팅 
    /// </summary>
    private void CFirstInitSetLoad_Skill(JsonData returnData)
    {
        LogPrint.PrintError("CFirstInitSetLoad Skill");
        if (returnData != null)
        {
            if (returnData.Keys.Contains("rows"))
            {
                foreach (JsonData row in returnData["rows"])
                {
                    GameDatabase.TableDB.Skill skill = new GameDatabase.TableDB.Skill()
                    {
                        indate = RowPaser.StrPaser(row, "inDate"),
                        aInUid = RowPaser.LongPaser(row, "aInUid"),
                        idx = RowPaser.IntPaser(row, "idx"),
                        count = RowPaser.IntPaser(row, "count"),
                        level = RowPaser.IntPaser(row, "level"),
                    };

                    // only client data 
                    var chart = GameDatabase.GetInstance().chartDB.GetChartSkill_Data(skill.idx);
                    skill.cliend_rating = chart.s_rating;
                    skill.cliend_type = chart.s_type;

                    GameDatabase.GetInstance().tableDB.UpdateClientDB_Skill(skill);
                }
                isLoadSkill = true;
            }
        }
        else Debug.Log("contents has no data");
    }
#endregion

    // ########################################################################################
#region --------------------------- Table : EquipmentEncyclopedia ---------------------------
    /// <summary>
    /// 장비 도감 데이터 
    /// </summary>
    private async Task AGetEquipmentEncyclopedia()
    {
        GameDatabase.GetInstance ().equipmentEncyclopediaDB.DefaultSetting();

        LogPrint.Print("----------------A GetEquipmentEncyclopedia");
        BackendReturnObject bro1 = null;
        SendQueue.Enqueue(Backend.GameInfo.GetPrivateContents, tableName_EquipmentEncyclopedia, 100, callback => { bro1 = callback; });
        while (bro1 == null) { await Task.Delay(100); }
        LogPrint.Print("----------------A GetEquipmentEncyclopedia " + bro1);

        if (bro1.IsSuccess())
        {
            if (bro1.GetReturnValuetoJSON()["rows"].Count > 0)
            {
                GameDatabase.GetInstance().equipmentEncyclopediaDB.Set(bro1.GetReturnValuetoJSON());
                isEncy = true;
            }
            else
            {
                await ASetInsertEquipmentEncyclopedia();
            }
        }
        else
        {
            LogPrint.PrintError("load failled  GetEquipmentEncyclopedia");
        }
    }

    public async Task ASetInsertEquipmentEncyclopedia()
    {
        LogPrint.Print("----------------A SetInsert EquipmentEncyclopedia");
        var dbEncy = GameDatabase.GetInstance().equipmentEncyclopediaDB.GetAll();
        Param param = new Param();
        for (int fEqRt = 1; fEqRt <= 7; fEqRt++)
        {
            string dbKey = string.Format("Rating{0}", fEqRt);
            if (dbEncy.ContainsKey(dbKey))
            {
                List<string> strDBs = new List<string>();
                foreach (var fdb in dbEncy[dbKey])
                {
                    strDBs.Add(string.Format("{0},{1},{2},{3},{4}", fdb.ty, fdb.rt, fdb.id, fdb.cnt, fdb.eh_lv));
                }
                param.Add(dbKey, JsonUtility.ToJson(new Serialization<string>(strDBs)));
            }
        }

        BackendReturnObject bro1 = null;
        SendQueue.Enqueue(Backend.GameInfo.Insert, BackendGpgsMng.tableName_EquipmentEncyclopedia, param, callback => { bro1 = callback; });
        while (bro1 == null) { await Task.Delay(100); }

        LogPrint.Print("----------------A SetInsert EquipmentEncyclopedia " + bro1);
        if (bro1.IsSuccess())
        {
            GameDatabase.GetInstance().equipmentEncyclopediaDB.Set(bro1.GetReturnValuetoJSON());
            isEncy = true;
        }
        else LogPrint.PrintError("A SetInsert EquipmentEncyclopedia insert error");
    }
    #endregion

    // ########################################################################################
    #region --------------------------- Table : Achievements ---------------------------
    public async Task AGetAchievements()
    {
        LogPrint.Print("----------------A GetAchievements");
        BackendReturnObject bro1 = null;
        SendQueue.Enqueue(Backend.GameInfo.GetPrivateContents, tableName_Achievements, 100, callback => { bro1 = callback; });
        while (bro1 == null) { await Task.Delay(100); }
        LogPrint.Print("----------------A GetAchievements " + bro1);

        if (bro1.IsSuccess())
        {
            if (bro1.GetReturnValuetoJSON()["rows"].Count > 0)
            {
                GameDatabase.GetInstance().achievementsDB.Set(bro1.GetReturnValuetoJSON());
            }
            else
            {
                await ASetInsertAchievements();
            }
        }
        else
        {
            LogPrint.PrintError("load failled AGetAchievements");
        }
    }

    async Task ASetInsertAchievements()
    {
        int mCnt = GameDatabase.GetInstance().achievementsDB.GetChartCount();
        Param param = new Param();
        for (int i = 0; i < mCnt; i++)
        {
            GameDatabase.AchievementsDB.Nbr nbr = (GameDatabase.AchievementsDB.Nbr)i;
            param.Add(nbr.ToString(), JsonUtility.ToJson(new GameDatabase.AchievementsDB.JsonDB()));
        }

        if (param.GetValue().Count > 0)
        {
            LogPrint.Print("----------------A SetInsert Achievements");
            BackendReturnObject bro1 = null;
            SendQueue.Enqueue(Backend.GameInfo.Insert, tableName_Achievements, param, callback => { bro1 = callback; });
            while (bro1 == null) { await Task.Delay(100); }
            LogPrint.Print("----------------A SetInsert Achievements " + bro1);

            if (bro1.IsSuccess())
            {
                GameDatabase.GetInstance().achievementsDB.SetInitialDataSetting(new GameDatabase.AchievementsDB.Achievements() { indate = bro1.GetReturnValuetoJSON()["inDate"].ToString() }); ;
            }
            else { LogPrint.PrintError("A SetInsert Achievements insert error"); }
        }
        else { LogPrint.PrintError("파라미터 정보가 잘못되었습니다."); }
    }
#endregion

    // ########################################################################################
#region --------------------------- Table : DailyMission ---------------------------
    /// <summary>
    /// 미션 테이블 데이터 로드 
    /// </summary>
    public async Task AGetDailyMission()
    {
        LogPrint.Print("----------------A GetDailyMission");
        BackendReturnObject bro1 = null;
        SendQueue.Enqueue(Backend.GameInfo.GetPrivateContents, tableName_DailyMission, 100, callback => { bro1 = callback; });
        while (bro1 == null) { await Task.Delay(100); }
        LogPrint.Print("----------------A GetDailyMission " + bro1);

        if (bro1.IsSuccess())
        {
            if (bro1.GetReturnValuetoJSON()["rows"].Count > 0)
            {
                GameDatabase.GetInstance().dailyMissionDB.Set(bro1.GetReturnValuetoJSON());
            }
            else
            {
                await ASetInsertDailyMission();
            }
        }
        else
        {
            LogPrint.PrintError("load failled AGetDailyMission");
        }
    }

    /// <summary>
    /// 일일 미션 인서트 (최초 1회 세팅)  
    /// </summary>
    async Task ASetInsertDailyMission()
    {
        string _dailyYme = GameDatabase.GetInstance().attendanceDB.DailyYmd();
        int mCnt = GameDatabase.GetInstance().dailyMissionDB.GetChartCount();
        Param param = new Param();
        param.Add("dailyYmd", _dailyYme);
        for (int i = 0; i < mCnt; i++)
        {
            GameDatabase.DailyMissionDB.Nbr nbr = (GameDatabase.DailyMissionDB.Nbr)i;
            param.Add(nbr.ToString(), JsonUtility.ToJson(new GameDatabase.AchievementsDB.JsonDB()));
        }

        if (param.GetValue().Count > 0)
        {
            LogPrint.Print("----------------A SetInsert DailyMission");
            BackendReturnObject bro1 = null;
            SendQueue.Enqueue(Backend.GameInfo.Insert, tableName_DailyMission, param, callback => { bro1 = callback; });
            while (bro1 == null) { await Task.Delay(100); }
            LogPrint.Print("----------------A SetInsert DailyMission " + bro1);

            if (bro1.IsSuccess())
            {
                GameDatabase.GetInstance().dailyMissionDB.SetInitialDataSetting(new GameDatabase.DailyMissionDB.DailyMission()
                { 
                    indate = bro1.GetReturnValuetoJSON()["inDate"].ToString(),
                    dailyYmd = _dailyYme
                });
            }
            else { LogPrint.PrintError("A SetInsert DailyMission insert error"); }
        }
        else LogPrint.PrintError("파라미터 정보가 잘못되었습니다.");
    }

    /// <summary>
    /// 일일 미션 리셋 
    /// </summary>
    public async Task<BackendReturnObject> ASetResetDailyMission(string newYmd)
    {
        int mCnt = GameDatabase.GetInstance().dailyMissionDB.GetChartCount();
        Param param = new Param();
        param.Add("dailyYmd", newYmd);
        for (int i = 0; i < mCnt; i++)
        {
            GameDatabase.DailyMissionDB.Nbr nbr = (GameDatabase.DailyMissionDB.Nbr)i;
            param.Add(nbr.ToString(), JsonUtility.ToJson(new GameDatabase.AchievementsDB.JsonDB()));
        }
            

        if (param.GetValue().Count > 0)
        {
            LogPrint.Print("----------------A SetResetDailyMission");
            BackendReturnObject bro1 = null;
            SendQueue.Enqueue(Backend.GameInfo.Update, tableName_DailyMission, GameDatabase.GetInstance ().dailyMissionDB.GetInDate(), param, callback => { bro1 = callback; });
            while (bro1 == null) { await Task.Delay(100); }
            LogPrint.Print("----------------A SetResetDailyMission " + bro1);
            return bro1;
        }

        return default;
    }
#endregion


    // ########################################################################################
#region --------------------------- Table : Quest ---------------------------
    /// <summary>
    /// 퀘스트 테이블 데이터 로드 
    /// </summary>
    public async Task AGetQuest()
    {
        LogPrint.Print("----------------A GetQuest");
        BackendReturnObject bro1 = null;
        SendQueue.Enqueue(Backend.GameInfo.GetPrivateContents, tableName_Quest, 100, callback => { bro1 = callback; });
        while (bro1 == null) { await Task.Delay(100); }
        LogPrint.Print("----------------A GetQuest " + bro1);

        if (bro1.IsSuccess())
        {
            if (bro1.GetReturnValuetoJSON()["rows"].Count > 0)
            {
                GameDatabase.GetInstance().questDB.Set(bro1.GetReturnValuetoJSON());
            }
            else
            {
                await ASetInsertQuest();
            }
        }
        else LogPrint.PrintError("load failled AGetQuest");
    }

    private async Task ASetInsertQuest()
    {
        LogPrint.Print("----------------A SetInsert Quest");
        BackendReturnObject bro1 = null;
         Param parm = ParamT.Collection(new ParamT.P[] { new ParamT.P() { k = "nbr0", v = 1 } });
        SendQueue.Enqueue(Backend.GameInfo.Insert, tableName_Quest, parm, callback => { bro1 = callback; });
        while (bro1 == null) { await Task.Delay(100); }
        LogPrint.Print("----------------A SetInsert Quest " + bro1);
        if (bro1.IsSuccess())
        {
            GameDatabase.GetInstance().questDB.SetInitial(bro1.GetReturnValuetoJSON());
        }
        else { LogPrint.PrintError("A SetInsert Quest insert error"); }
    }
#endregion

    // ########################################################################################
#region --------------------------- Table : InventoryBackUp ---------------------------
    private Param GetInvenData ()
    {
        Param p = new Param();
        p.Add("inven", JsonUtility.ToJson
        (
            new InvenData()
            {
                equips = GameDatabase.GetInstance().tableDB.GetInvenSave_Equip(),
                pets = GameDatabase.GetInstance ().tableDB.GetAllPets().FindAll(obj => string.IsNullOrEmpty(obj.indate)),
                petsEncy = GameDatabase.GetInstance().tableDB.GetAllPetEncy().FindAll(obj => string.IsNullOrEmpty(obj.indate)),
            }
        ));
        return p;
    }
    
    /// <summary> 인벤 백업 데이터 로드 </summary>
    private async Task AGetInventoryBackUp()
    {
        LogPrint.Print("----------------A GetInventoryBackUp ");
        BackendReturnObject bro1 = null;
        SendQueue.Enqueue(Backend.GameInfo.GetPrivateContents, tableName_InventoryBackUp, 100, callback => { bro1 = callback; });
        while (bro1 == null) { await Task.Delay(100); }
        LogPrint.Print("----------------A GetInventoryBackUp " + bro1);

        if (bro1.IsSuccess())
        {
            var invbp = GameDatabase.GetInstance().inventoryDB.GetInvenBackUp();
            JsonData row = bro1.GetReturnValuetoJSON()["rows"];
            LogPrint.PrintError(row.ToJson());
            if (row.Count > 0) // 데이터 있다 
            {
                invbp.in_date = row[0]["inDate"]["S"].ToString();
                try
                {
                    invbp.inven_data = JsonUtility.FromJson<InvenData>(row[0]["inven"]["S"].ToString());
                    LogPrint.PrintError("<color=yellow>백업 인벤토리 LOAD 정상---------------------</color>");
                }
                catch (Exception)
                {
                    LogPrint.PrintError("<color=red>백업 인벤토리 LOAD 에러---------------------</color>");
                    invbp.inven_data = new InvenData()
                    { 
                        equips = new List<GameDatabase.TableDB.Equipment>(),
                        pets = new List<TableDB.Pet>(), 
                        petsEncy = new List<TableDB.PetEncy>()
                    };
                }

                GameDatabase.GetInstance().inventoryDB.SetInvenBackUp(invbp);
                await GameDatabase.GetInstance().inventoryDB.GetLoadToSetting(invbp.inven_data);
                isLoadInventory = true;
            }
            else // 데이터 없다 -> 추가함 
            {
                BackendReturnObject bro2 = Backend.GameInfo.Insert(tableName_InventoryBackUp, GetInvenData());
                invbp.in_date = bro2.GetReturnValuetoJSON()["inDate"].ToString();
                GameDatabase.GetInstance().inventoryDB.SetInvenBackUp(invbp);
                isLoadInventory = true;
            }
        }
        else
        {
            LogPrint.PrintError("load failled AGetInventoryBackUp");
        }
    }

    public async void ASetInventoryBackUp(bool Asy = false)
    {
        var invbp = GameDatabase.GetInstance().inventoryDB.GetInvenBackUp();
        if (string.IsNullOrEmpty(invbp.in_date))
        {
            BackendReturnObject bro = Backend.GameInfo.Insert(tableName_InventoryBackUp, GetInvenData());
            if (isBtnClickQuit == false)
            {
                if (bro.IsSuccess())
                {
                    invbp.in_date = bro.GetReturnValuetoJSON()["inDate"].ToString();
                    GameDatabase.GetInstance().inventoryDB.SetInvenBackUp(invbp);
                }
            }
        }
        else
        {
            if (Asy)
            {
                BackendReturnObject bro1 = null;
                SendQueue.Enqueue(Backend.GameInfo.Update, tableName_InventoryBackUp, invbp.in_date, GetInvenData(), callback => { bro1 = callback; });
                while (bro1 == null) { await Task.Delay(100); }
            }
            else Backend.GameInfo.Update(tableName_InventoryBackUp, invbp.in_date, GetInvenData());
        }
    }

    /// <summary>
    /// 게임 종료 or 특정 시점에 인벤 및 기타 데이터 서버로 보냄 
    /// </summary>
    public async Task SendFocusDB()
    {
        LogPrint.EditorPrint(" ------------------- <color=red> Send FocusDB 1 </color> ------------------- ");

        // 유저 Item 전송 
        var item_transaction = GameDatabase.GetInstance().tableDB.ParamTransactionItem();
        foreach (var db in item_transaction)
        {
            BackendReturnObject bro = Backend.GameInfo.TransactionWrite(db);
            while (bro == null) { await Task.Delay(100); }
        }

        // 도감 전송 
        GameDatabase.GetInstance().equipmentEncyclopediaDB.SendUpdateTableDB(false);

        // 퀘스트 전송 
        Backend.GameInfo.Update(tableName_Quest, GameDatabase.GetInstance().questDB.GetInDate, GameDatabase.GetInstance().questDB.GetAllParamQuest());

        // 유저 goods 전송 
        Backend.GameInfo.Update(tableName_Goods, GameDatabase.GetInstance().tableDB.GetTableDB_Goods().indate, GameDatabase.GetInstance().tableDB.ParamAddGoods());

        // 유저 정보 전송 
        var userInfo_db = GameDatabase.GetInstance().tableDB.GetUserInfo();
        var param_userInfo_db = GameDatabase.GetInstance().tableDB.ParamAddUserInfo(userInfo_db);
        param_userInfo_db.Add("m_chpt_stg_nbr", userInfo_db.m_chpt_stg_nbr);
        param_userInfo_db.Add("m_clar_chpt", userInfo_db.m_clar_chpt);
        Backend.GameInfo.Update(tableName_UserInfo, GameDatabase.GetInstance().tableDB.GetUserInfo().indate, param_userInfo_db);

        // 미션, 업적 
        Backend.GameInfo.Update(tableName_Achievements, GameDatabase.GetInstance().achievementsDB.GetInDate(), GameDatabase.GetInstance().achievementsDB.GetParamAddAchievements());
        Backend.GameInfo.Update(tableName_DailyMission, GameDatabase.GetInstance().dailyMissionDB.GetInDate(), GameDatabase.GetInstance().dailyMissionDB.GetParamAddDailyMission());

        GameDatabase.GetInstance().tableDB.ProficiencyUpdate(true); // 장비 숙련도 

        SendScore();// 랭킹 스코어 등록 
        
        ASetInventoryBackUp();// 인벤 DB, 펫 DB, 펫 도감 DB 

        GameDatabase.GetInstance().publicContentDB.ASetPub_CharData(BackendGpgsMng.tableName_Pub_NowCharData, false); // 캐릭터 데이터 전송 

        LogPrint.EditorPrint(" ------------------- <color=yellow> Send FocusDB 2 </color> ------------------- ");
    }

    public void SendScore()
    {
        if (BackendGpgsMng.isEditor == false)
        {
            var userInfo_db = GameDatabase.GetInstance().tableDB.GetUserInfo();
            Backend.GameInfo.UpdateRTRankTable(tableName_UserInfo, "m_chpt_stg_nbr", userInfo_db.m_chpt_stg_nbr, userInfo_db.indate);
            Backend.GameInfo.UpdateRTRankTable(tableName_UserInfo, "m_pvp_score", userInfo_db.m_pvp_score, userInfo_db.indate);// PvP 점수 등록 
            LogPrint.EditorPrint(" --------------- SendScore --------------- ");
        }
    }
#endregion

    // ########################################################################################
#region --------------------------- Table : Rank, RTRank ---------------------------

    /// <summary>
    /// 실시간 랭킹 리스트 
    /// </summary>
    public async Task AGetRTRankList()
    {
        LogPrint.Print("----------------A GetRTRankList");
        BackendReturnObject bro1 = null;
        SendQueue.Enqueue(Backend.RTRank.RTRankList, callback => { bro1 = callback; });
        while (bro1 == null) { await Task.Delay(100); }
        LogPrint.Print("----------------A GetRTRankList " + bro1);

        if (bro1.IsSuccess())
        {
            GameDatabase.GetInstance().rankDB.CSetRTRankList(bro1.GetReturnValuetoJSON());

            var userInfo_db = GameDatabase.GetInstance().tableDB.GetUserInfo();
            if (userInfo_db.m_pvp_in) // 이전에 강종하였다면 패널티 점수 차감 
            {
                int pvp_penalty_score = userInfo_db.m_pvp_score - GameDatabase.GetInstance().chartDB.GetDicBalance("pvp.force.quit.penalty.score").val_int;
                int pvp_penalty_today_score = userInfo_db.m_pvp_today_score - GameDatabase.GetInstance().chartDB.GetDicBalance("pvp.force.quit.penalty.score").val_int;
                await GameDatabase.GetInstance().rankDB.ASendRTRank_PvpTop100(-1, pvp_penalty_score, pvp_penalty_today_score);
            }
        }
        
        isLoadRank = true;
    }
#endregion

    // ########################################################################################
#region --------------------------- Table : Dungeo - Top,Mine,Raid ---------------------------
    /// <summary> 던전 : 도전의 탑 LOAD </summary>
    public async Task AGetDungeonTop()
    {
        if (string.IsNullOrEmpty(GameDatabase.GetInstance().dungeonDB.GetTop().indate))
        {
            LogPrint.Print("----------------A GetDungeonTop");
            BackendReturnObject bro1 = null;
            SendQueue.Enqueue(Backend.GameInfo.GetPrivateContents, tableName_DungeonTop, 100, callback => { bro1 = callback; });
            while (bro1 == null) { await Task.Delay(100); }
            LogPrint.Print("----------------A GetDungeonTop " + bro1);

            if (bro1.IsSuccess())
            {
                if (bro1.GetReturnValuetoJSON()["rows"].Count > 0)
                {
                    GameDatabase.GetInstance().dungeonDB.SetTop(bro1.GetReturnValuetoJSON());
                }
                else
                {
                    await ASetInsertDungeon(tableName_DungeonTop, JsonUtility.ToJson(new GameDatabase.DungeonDB.Top()));
                }
            }
            else
            {
                LogPrint.PrintError("도전의 탑 데이터 로드 실패");
            }
        }
    }

    /// <summary> 던전 : 광산 LOAD </summary>
    public async Task AGetDungeonMine()
    {
        if (string.IsNullOrEmpty(GameDatabase.GetInstance().dungeonDB.GetMine().indate))
        {
            LogPrint.Print("----------------A GetDungeonMine");
            BackendReturnObject bro1 = null;
            SendQueue.Enqueue(Backend.GameInfo.GetPrivateContents, tableName_DungeonMine, 100, callback => { bro1 = callback; });
            while (bro1 == null) { await Task.Delay(100); }
            LogPrint.Print("----------------A GetDungeonMine " + bro1);

            if (bro1.IsSuccess())
            {
                if (bro1.GetReturnValuetoJSON()["rows"].Count > 0)
                {
                    GameDatabase.GetInstance().dungeonDB.SetMine(bro1.GetReturnValuetoJSON());
                }
                else
                {
                    await ASetInsertDungeon(tableName_DungeonMine, JsonUtility.ToJson(new GameDatabase.DungeonDB.Mine()));
                }
            }
            else
            {
                LogPrint.PrintError("도전의 탑 데이터 로드 실패");
            }
        }
    }

    /// <summary> 던전 : 레이드 LOAD </summary>
    public async Task AGetDungeonRaid()
    {
        if (string.IsNullOrEmpty(GameDatabase.GetInstance().dungeonDB.GetRaid().indate))
        {
            LogPrint.Print("----------------A GetDungeonRaid");
            BackendReturnObject bro1 = null;
            SendQueue.Enqueue(Backend.GameInfo.GetPrivateContents, tableName_DungeonRaid, 100, callback => { bro1 = callback; });
            while (bro1 == null) { await Task.Delay(100); }
            LogPrint.Print("----------------A GetDungeonRaid " + bro1);

            if (bro1.IsSuccess())
            {
                if (bro1.GetReturnValuetoJSON()["rows"].Count > 0)
                {
                    LogPrint.PrintError("AGetDungeonRaid1");
                    GameDatabase.GetInstance().dungeonDB.SetRaid(bro1.GetReturnValuetoJSON());
                }
                else
                {
                    LogPrint.Print("AGetDungeonRaid2");
                    await ASetInsertDungeon(tableName_DungeonRaid, JsonUtility.ToJson(new GameDatabase.DungeonDB.Raid()));
                }
            }
            else
            {
                LogPrint.PrintError("도전의 탑 데이터 로드 실패");
            }
        }
    }

    /// <summary> dungeon Server SET 첫 세팅 </summary>
    private async Task ASetInsertDungeon(string table_name, string str_db)
    {
        JSONObject jd = JSONObject.Create(str_db);
        Param param = new Param();
        try
        {
            foreach (string key in jd.keys)
            {
                if (!System.Object.Equals(key, "indate"))
                    param.Add(key, (float)jd[key].f);
            }
        }
        catch (Exception e) { Debug.Log(e); }

        if(param.GetValue().Count > 0)
        {
            LogPrint.Print("----------------A SetInsert Dungeon");
            BackendReturnObject bro1 = null;
            SendQueue.Enqueue(Backend.GameInfo.Insert, table_name, param, callback => { bro1 = callback; });
            while (bro1 == null) { await Task.Delay(100); }
            LogPrint.Print("----------------A SetInsert Dungeon " + bro1);

            if (bro1.IsSuccess())
            {
                if (table_name == tableName_DungeonTop)
                {
                    GameDatabase.GetInstance().dungeonDB.SetTop(bro1.GetReturnValuetoJSON());
                }
                else if (table_name == tableName_DungeonMine)
                {
                    GameDatabase.GetInstance().dungeonDB.SetMine(bro1.GetReturnValuetoJSON());
                }
                else if (table_name == tableName_DungeonRaid)
                {
                    GameDatabase.GetInstance().dungeonDB.SetRaid(bro1.GetReturnValuetoJSON());
                }
            }
            else { LogPrint.PrintError("A SetInsert Dungeon insert error"); }
        }
        else { LogPrint.PrintError("파라미터 정보가 잘못되었습니다."); }
        
    }
#endregion

    // ########################################################################################
#region --------------------------- Table : PublicDB ---------------------------

#endregion

    // ########################################################################################
#region --------------------------- 우편기능 ---------------------------
    public async Task<JsonData> AGetMail()
    {
        LogPrint.Print("----------------A GetMail");
        BackendReturnObject bro1 = null;
        SendQueue.Enqueue(Backend.Social.Post.GetPostListV2, callback => { bro1 = callback; });
        while (bro1 == null) { await Task.Delay(100); }
        LogPrint.Print("----------------A GetMail " + bro1);

        if (bro1.IsSuccess())
        {
            return bro1.GetReturnValuetoJSON();
        }
        else return null;
    }


#endregion


    //          ##### 뒤끝 데이터 -END- #####
#endregion  ##### 뒤끝 데이터 -END- #####

    // 게임 상단 1줄 메시지 띄우기
    public void ShowMessage(string id, float time)
    {
        //string msg = LanguageSystemData.GetInstance().GetStringDate(id);
        //DispatcherAction(() => InitBackEndUIManager.instance.topMessageLog.ShowPop(msg, time));
    }

    // Dispatcer에서 action 실행 (메인스레드)
    private void DispatcherAction(Action action)
    {
        //Dispatcher.Instance.Invoke(action);
    }
}
