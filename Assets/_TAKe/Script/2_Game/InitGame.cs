using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Coffee.UIExtensions;
using System.Threading.Tasks;
using LitJson;
using System.Threading;
using CodeStage.AntiCheat.ObscuredTypes;

public class InitGame : MonoSingleton<InitGame>
{
    private Transform my_parent, my_respwanPos;
    private Image myHpBarFill;
    private Transform or_parent, or_respwanPos;
    private Image orHpBarFill;

    [SerializeField] bool TEST = false;

    void Awake()
    {
        //Screen.SetResolution(720, 1280, true);
        //Screen.SetResolution(Screen.width, Screen.width * 16 / 9, true);
        Application.targetFrameRate = 60;
        Application.runInBackground = true;
        QualitySettings.vSyncCount = 1;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        int qLv = GameDatabase.GetInstance().option.setting_quality == 0 ? 2 : 0;
        QualitySettings.SetQualityLevel(qLv);

        GameObject go_zombieRoot = GameObject.FindGameObjectWithTag("ZombieRoot");
        my_parent = go_zombieRoot.transform.Find("MyRoot").transform;
        my_respwanPos = my_parent.Find("MyRespwanPosition").transform;
        or_parent = go_zombieRoot.transform.Find("OrRoot").transform;
        or_respwanPos = or_parent.Find("OrRespwanPosition").transform;

        if (TEST)
        {
            if (GameObject.Find("BackendMng") == null)
            {
                SceneManager.LoadScene(0);
                return;
            }
        }
    }

    public async Task Attend(bool isFirstPlay)
    {
        if (!isFirstPlay)
        {
            Task<bool> tsk1 = GameDatabase.GetInstance().attendanceDB.GetIsCheckAttendance();  // 출석 체크 
            while (tsk1.IsCompleted == false) await Task.Delay(100);
            if (tsk1.Result == true)
                PopUpMng.GetInstance().Open_AttendanceBook(false);

            PopUpMng.GetInstance().Open_FirstReward(); // 첫 접속 보상
        }
        else
        {
            TutorialMng.GetInstance().FirstPlayStart(); // 첫 접속시 간단한 매인 UI 설명 시작 
        }
    }

    async void Start()
    {
        Loading.Full(false);

        string ymd = GameDatabase.GetInstance().attendanceDB.DailyYmd();
        string indate = BackendGpgsMng.backendUserInfo.m_indate;
        if (System.DateTime.Parse(indate) >= BackendGpgsMng.GetLoginSDate) // 첫 접속 유저인지 체크 
        {
            PlayerPrefs.SetString(PrefsKeys.key_Convenience_OnfGameSpeed, "ON");
            PlayerPrefs.SetString(PrefsKeys.key_Convenience_OnfAutoSkill, "ON");
            PlayerPrefs.SetInt(PrefsKeys.prky_StageType, 0);
        }

        await BackendGpgsMng.GetInstance().ResetToday();
        
        MainUI.GetInstance ().topUI.SetGoodsView();
        MainUI.GetInstance ().topUI.GetInfoViewTBC();

        GameDatabase.GetInstance().characterDB.SetPlayerStatValue(); // 내 좀비 데이터 스탯 세팅 
        GameDatabase.GetInstance().tableDB.InitSkillSlot();

        GameDatabase.GetInstance().tableDB.InitChapterNbr();
        // 챕터 반복 모드
        bool isChapterLoop = false;
        string strLoopDb = PlayerPrefs.GetString(PrefsKeys.prky_LoopChapterDb);
        LogPrint.Print("<color=yellow>strLoopDb : </color> " + strLoopDb);
        try
        {
            var loopDb = JsonUtility.FromJson<GameMng.LoopChapter>(strLoopDb);
            isChapterLoop = loopDb.isLoop;
            if (isChapterLoop == true)
            {
                loopDb.chpt_stg_nbr_min = GameDatabase.GetInstance().chartDB.GetDicChapterLoopArrayFirst(loopDb.loop_chpt_id).chpt_dvs_nbr * 10;
                loopDb.chpt_stg_nbr_max = GameDatabase.GetInstance().chartDB.GetDicChapterLoopArrayLast(loopDb.loop_chpt_id).chpt_dvs_nbr * 10;
                GameMng.GetInstance().ChapterLoopInit(loopDb);
            }
            else 
            {
                GameMng.GetInstance().mode_type = IG.ModeType.CHAPTER_CONTINUE;
            }
        }
        catch (System.Exception e) { GameMng.GetInstance().mode_type = IG.ModeType.CHAPTER_CONTINUE; }

        // 옵션 
        string strOptionDb = PlayerPrefs.GetString(PrefsKeys.prky_OptionDb);
        try
        {
            if (string.IsNullOrEmpty(strOptionDb))
                GameDatabase.GetInstance().option = new GameDatabase.Option();
            else
                GameDatabase.GetInstance().option = JsonUtility.FromJson<GameDatabase.Option>(strOptionDb);

            PlayerPrefs.SetString(PrefsKeys.prky_OptionDb, JsonUtility.ToJson(GameDatabase.GetInstance().option));
        }
        catch (System.Exception) { GameDatabase.GetInstance().option = new GameDatabase.Option(); }

        SoundManager.GetInstance().AwakeAfter();
        GameMng.GetInstance().ChapterStageType(false);
        //GameDatabase.GetInstance().publicContentDB.InitSendCharacterDbCombat();

        MainUI.GetInstance().Init();
        ConvenienceFunctionMng.GetInstance().InitConvenience();

        await GameMng.GetInstance ().Routin_ChangeMode(isChapterLoop == true ? IG.ModeType.CHAPTER_LOOP : IG.ModeType.CHAPTER_CONTINUE, false, 0);

        await Task.Delay(500);
        GameDatabase.GetInstance().publicContentDB.ASetPub_CharData(BackendGpgsMng.tableName_Pub_NowCharData); // 캐릭터 데이터 전송 

        await Attend(System.DateTime.Parse(indate) >= BackendGpgsMng.GetLoginSDate);
        await Task.Delay(250);

        PopUpMng.GetInstance().Open_OfflineReward(BackendGpgsMng.GetInstance().GetNowTime()); // 오프라인 보상 
        await Task.Delay(250);

        if (!BackendGpgsMng.isEditor)
        {
            if (!int.Equals(PlayerPrefs.GetString(PrefsKeys.prky_backend_notice_ymd), ymd))
            {
                JsonData noticeRow = await BackendGpgsMng.GetInstance().BackendNoticeList();
                PopUpMng.GetInstance().Open_BackendNotice(noticeRow, false);
                await Task.Delay(250);
            }
        }

        BackendGpgsMng.GetInstance().LoopCheckingResetToday();
        BackendGpgsMng.GetInstance().ScoreSendLoop();
        BackendGpgsMng.GetInstance().SendAllDB();

        //// 특정 버전에서만 
        //if (string.Equals(BackendGpgsMng.GetInstance().currentVersion, "1.0.16"))
        //{
        //    for (int f_eq_ry = 8; f_eq_ry <= 10; f_eq_ry++)
        //    {
        //        var eqDb = GameDatabase.GetInstance().tableDB.GetNowWearingEquipPartsData(f_eq_ry);
        //        //if (eqDb.eq_ty >= 8 && eqDb.eq_ty <= 10) // 업적, nbr7 장신구 강화 레벨 10 ~ 35 달성!
        //        GameDatabase.GetInstance().achievementsDB.ASetInCount(GameDatabase.AchievementsDB.Nbr.nbr7, eqDb.m_ehnt_lv, false); // 업적, nbr7 장신구 강화 레벨 10 ~ 35 달성!
        //    }

        //    int dvsNbr = GameDatabase.GetInstance().tableDB.GetUserInfo().GetChapterDvsNbr();
        //    int chpt_id = GameDatabase.GetInstance().monsterDB.GetChapterDvsNbrFindChapterID(dvsNbr);
        //    GameDatabase.GetInstance().achievementsDB.ASetInCount(GameDatabase.AchievementsDB.Nbr.nbr1, chpt_id, false); // 업적 nbr1 챕터 ?, 모든 스테이지 클리어 하기!
        //}

        await Task.Delay(500);
        Loading.Full(true);
    }

    //CancellationTokenSource tokenSource;

    //async void TESTAsync()
    //{
    //    tokenSource?.Cancel();
    //    tokenSource = new CancellationTokenSource();
    //    try
    //    {
    //        await Task.Delay(5000, tokenSource.Token);
    //        LogPrint.EditorPrint("--------------TESTAsync 11111 ---------------");
    //    }
    //    catch (TaskCanceledException ex)
    //    {
    //        LogPrint.EditorPrint(ex.Message);
    //    }
    //    finally
    //    {
    //        tokenSource.Dispose();
    //        tokenSource = null;
    //        LogPrint.EditorPrint("--------------TESTAsync 22222 ---------------");
    //    }
    //}

    /// <summary>
    /// 좀비 관련 오브젝트 세팅 
    /// </summary>
    public void ZombiePopFromPool(bool isMy)
    {
        Debug.LogError("ZombiePopFromPool isMy : " + isMy);
        GameObject goPZ = null;
        if (isMy == true)
        {
            goPZ = ObjectPool.GetInstance().PopFromPool(ObjectPool.Game_PlayerZombie).gameObject;
        }
        else
        {
            var mdty = GameMng.GetInstance().mode_type;
            bool isDgn = mdty == IG.ModeType.DUNGEON_MINE || mdty == IG.ModeType.DUNGEON_RAID;
            bool isBoss = false;
            if (mdty == IG.ModeType.DUNGEON_TOP)
                isBoss = GameMng.GetInstance().stc_DungeonTop.qStat.Count == 1 ? true : false;
            else if (mdty == IG.ModeType.DUNGEON_MINE)
                isBoss = GameMng.GetInstance().stc_DungeonMine.qStat.Count == 1 ? true : false;
            else if (mdty == IG.ModeType.DUNGEON_RAID)
                isBoss = GameMng.GetInstance().stc_DungeonRaid.qStat.Count == 1 ? true : false;
            else
                isBoss = GameMng.GetInstance().StageNbr() >= 10 || mdty == IG.ModeType.PVP_BATTLE_ARENA || GameMng.GetInstance().stage_type == IG.StageType.BOSS_MONSTER;

            if (isDgn)
                goPZ = ObjectPool.GetInstance().PopFromPool(ObjectPool.Game_DungeonMonster).gameObject;
            else
            {
                if (isBoss == true)
                    goPZ = ObjectPool.GetInstance().PopFromPool(ObjectPool.Game_PlayerZombie).gameObject;
                else
                    goPZ = ObjectPool.GetInstance().PopFromPool(ObjectPool.Game_NormalMonster).gameObject;
            }
        }

        if(goPZ != null)
        {
            goPZ.SetActive(true);
            IG.Player igp = new IG.Player
            {
                parent = isMy == true ? my_parent : or_parent,
                respwanPos = isMy == true ? my_respwanPos : or_respwanPos,
                playerSkillAction = goPZ.GetComponent<PlayerSkillAction>(),
                activeSkillCancel = new List<IG.SkillNumber>(),
                activateSkills = new List<IG.SkillNumber>(),
                activateExtended = new Dictionary<int, int>(),
                etcZbData = new IG.EtcZbData(),
            };

            if (isMy == true)
            {
                if (GameMng.GetInstance().myPZ != null)
                    ObjectPool.GetInstance().PushToPool(GameMng.GetInstance().myPZ.tr.objLife.opName, GameMng.GetInstance().myPZ.tr.objLife);

                GameMng.GetInstance().myPZ = goPZ.GetComponent<PlayerZombie>();
                GameMng.GetInstance().myPZ.igp = igp;
                GameMng.GetInstance().myPZ.InitPlayer(true);
            }
            else
            {
                if (GameMng.GetInstance().orPZ != null)
                    ObjectPool.GetInstance().PushToPool(GameMng.GetInstance().orPZ.tr.objLife.opName, GameMng.GetInstance().orPZ.tr.objLife);

                GameMng.GetInstance().orPZ = goPZ.GetComponent<PlayerZombie>();
                GameMng.GetInstance().orPZ.igp = igp;
                GameMng.GetInstance().orPZ.InitPlayer(false);
            }
        }
    }
}