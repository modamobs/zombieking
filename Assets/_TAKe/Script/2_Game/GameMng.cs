using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using System;
using UnityEngine.UI;

public class GameMng : MonoSingleton<GameMng>
{
    public ConvenienceFunctionMng cfMng;
    public IG.ModeType mode_type = IG.ModeType.CHAPTER_CONTINUE;
    public IG.StageType stage_type = IG.StageType.NORMAL_MONSTER;

    [SerializeField] List<GameObject> lChapterStageMap = new List<GameObject>();
    [SerializeField] List<GameObject> lDungeonMap = new List<GameObject>();
    public PlayerZombie myPZ;
    public PlayerZombie orPZ;
    public PlayerPet myPet;

    public ZbCam zbCam;
    public cdb_chpt_mnst_stat chapter_db = new cdb_chpt_mnst_stat();
    public LoopChapter loopChapter;
    [System.Serializable]
    public class LoopChapter
    {
        public bool isLoop;
        public int loop_chpt_id;
        public int chpt_stg_nbr;
        public int chpt_stg_nbr_min;
        public int chpt_stg_nbr_max;

        public int GetChapterStageNbr()
        {
            return chpt_stg_nbr;
        }

        public int GetLoopChapterNbr()
        {
            //return Mathf.FloorToInt(chpt_stg_nbr / 10.0f); // 증가하도록
            return Mathf.FloorToInt(chpt_stg_nbr_min / 10.0f); // 첫번째 몬스터 기준 반복 
        }

        public int GetLoopStageNbr()
        {
            return (chpt_stg_nbr % 10) + 1;
        }
    }

    public IG.ImGameUIObject gameUIObject;
    [SerializeField] Transform tr_ZbRoot;

    //##################################
    //##################################
    //##################################
    #region ##### 선언 : 챕터 #####
    public float GameDeltaTime
    {
        get { return (int)(cfMng.convenFun.cfGameSpeed.onOffSpeed + 1) * Time.deltaTime; }
    }

    public float GameSpeed
    {
        get { return (int)cfMng.convenFun.cfGameSpeed.onOffSpeed + 2.0f; }
    }

    public int ChapterStageNbr ()
    {
        if (mode_type == IG.ModeType.CHAPTER_CONTINUE)
            return GameDatabase.GetInstance().tableDB.GetUserInfo().GetChapterStageNbr();
        else 
            return loopChapter.GetChapterStageNbr();
    }

    public int ChapterNbr()
    {
        if(mode_type == IG.ModeType.CHAPTER_CONTINUE)
            return GameDatabase.GetInstance().tableDB.GetUserInfo().GetChapterDvsNbr();
        else  // IG.ModeType.CHAPTER_LOOP
            return loopChapter.GetLoopChapterNbr();
    }

    /// <summary>
    /// 몬스터 1 ~ 10 
    /// </summary>
    /// <returns></returns>
    public int StageNbr()
    {
        if (mode_type == IG.ModeType.CHAPTER_CONTINUE)
            return GameDatabase.GetInstance().tableDB.GetUserInfo().GetStageNbr();
        else // IG.ModeType.CHAPTER_LOOP
            return loopChapter.GetLoopStageNbr();
    }

    void Awake()
    {
        
    }

    #endregion

    //##################################
    //##################################
    //##################################
    #region ##### 게 임 진 행 #####
    /// <summary> SqrMagnitude 거리 체크 </summary>
    public static float GetSqr(Vector3 _a, Vector3 _b)
    {
        float xDiff = _a.x - _b.x;
        float yDiff = _a.y - _b.y;
        float zDiff = _a.z - _b.z;
        return xDiff * xDiff + yDiff * yDiff + zDiff * zDiff;
    }

    public void HideGameUIObject ()
    {
        gameUIObject.chapterStage.go_Root.SetActive(false);
        gameUIObject.dungeonTop.go_Root.SetActive(false);
        gameUIObject.dungeonMine.go_Root.SetActive(false);
        gameUIObject.dungeonRaid.go_Root.SetActive(false);
        gameUIObject.pvpBattleArena.go_Root.SetActive(false);

        gameUIObject.dungeonTop.pu_DungeonReward.gameObject.SetActive(false);
        gameUIObject.dungeonMine.pu_DungeonReward.gameObject.SetActive(false);
        gameUIObject.dungeonRaid.pu_DungeonReward.gameObject.SetActive(false);
    }

    ///// <summary> 좀비 위치 및 데이터 세팅 </summary>
    public async void OpponentToFight(bool myInit, bool orInit)
    {
        while (myPZ == null || orPZ == null) await Task.Delay(100);

        if (myInit && orInit) // 플레이어, 상대 함께 나올때
        {
            tr_ZbRoot.localPosition = new Vector3(-20, 0.1f, 0);
        }

        if (mode_type == IG.ModeType.CHAPTER_CONTINUE || mode_type == IG.ModeType.CHAPTER_LOOP)
        {
            MODE_CHAPTER_Progress();
        }

        if (myInit)
            myPZ.HealthLerp(true);

        if(orInit)
            orPZ.HealthLerp(true);
    }
    #endregion

    /// <summary>
    /// 몬스터 데이터 스탯 세팅 
    /// </summary>
    public async void ChapterMonsterStat()
    {
        if (mode_type == IG.ModeType.CHAPTER_CONTINUE || mode_type == IG.ModeType.CHAPTER_LOOP)
        {
            stc_ChapterMonstStats.Clear();
            int chpt = GameMng.GetInstance().ChapterNbr();
            int stg = GameMng.GetInstance().StageNbr();

            var mnst_db = GameDatabase.GetInstance().chartDB.list_cdb_chpt_mnst_stat;
            int indx = mnst_db.FindIndex((cdb_chpt_mnst_stat obj) => obj.chpt_dvs_nbr == chpt);
            chapter_db = indx >= 0 ? mnst_db[indx] : mnst_db[mnst_db.Count - 1];
            GameDatabase.TableDB.Equipment mnst_eqDB = new GameDatabase.TableDB.Equipment()
            {
                eq_rt = chapter_db.eq_rat,
                eq_id = chapter_db.eq_id,
                m_norm_lv = chapter_db.chpt_norm_lv,
                m_ehnt_lv = chapter_db.chpt_ehnt_lv,
                ma_st_rlv = chapter_db.chpt_m_st_rlv,
            };

            int startMonNbr = stage_type == IG.StageType.NORMAL_MONSTER ? 9 : 0;
            for (int nbr = startMonNbr; nbr >= 0; nbr--)
            {
                stc_ChapterMonstStats.Enqueue(GameDatabase.GetInstance().monsterDB.SetMonsterStatValue(mnst_eqDB, nbr, true, chapter_db.chpt_id <= 23 ? true : false)); // # 매인 스탯 #
                await Task.Delay(100);
            }
        }
        else if (mode_type == IG.ModeType.DUNGEON_TOP)
        {
            stc_DungeonTop.qStat = new Queue<GameDatabase.CharacterDB.StatValue>();
            for (int nbr = 4; nbr >= 0; nbr--)
                stc_DungeonTop.qStat.Enqueue(GameDatabase.GetInstance().monsterDB.GetDungeonMonsterStatValue(IG.ModeType.DUNGEON_TOP, nbr, stc_DungeonTop.inDgNbr));
        }
        else if (mode_type == IG.ModeType.DUNGEON_MINE)
        {
            stc_DungeonMine.qStat = new Queue<GameDatabase.CharacterDB.StatValue>();
            for (int nbr = 4; nbr >= 0; nbr--)
                stc_DungeonMine.qStat.Enqueue(GameDatabase.GetInstance().monsterDB.GetDungeonMonsterStatValue(IG.ModeType.DUNGEON_MINE, nbr, stc_DungeonMine.inDgNbr));
        }
        else if (mode_type == IG.ModeType.DUNGEON_RAID)
        {
            stc_DungeonRaid.qStat = new Queue<GameDatabase.CharacterDB.StatValue>();
            stc_DungeonRaid.qStat.Enqueue(GameDatabase.GetInstance().monsterDB.GetDungeonMonsterStatValue(IG.ModeType.DUNGEON_RAID, 0, stc_DungeonRaid.inDgNbr));
        }
        else if (mode_type == IG.ModeType.PVP_BATTLE_ARENA)
        {
            pvpOpponentDb = GameDatabase.GetInstance().pvpBattle.GetDataBattleDbOr();
        }
    }

    //##################################
    //##################################
    //##################################
    #region ##### 일반 필드 챕터 #####
    public Queue<GameDatabase.CharacterDB.StatValue> stc_ChapterMonstStats = new Queue<GameDatabase.CharacterDB.StatValue>(); // 스테이지 몬스터 스텟 
    public void ChapterInit()
    {

    }

    public void ChapterLoopInit(LoopChapter lp)
    {
        loopChapter = lp;
        mode_type = IG.ModeType.CHAPTER_LOOP;
    }

    /// <summary> 챕터 스테이지 일반 모드 or 보스 모드 </summary>
    public void ChapterStageType(bool isClickChange)
    {
        int intStageType = PlayerPrefs.GetInt(PrefsKeys.prky_StageType);
        GetInstance().stage_type = intStageType == 0 ? IG.StageType.NORMAL_MONSTER : (IG.StageType)intStageType;
        MainUI.GetInstance ().UIChapterStageTypeButton();

        if(isClickChange == true)
        {
            if (GetInstance().stage_type == IG.StageType.NORMAL_MONSTER)
                ChapterBossMode();
            else
                ChapterNormalMode();
        }
    }

    public void ChapterLoopStart(int loopID)
    {
        loopChapter = new LoopChapter()
        {
            isLoop = true,
            loop_chpt_id = loopID,
            chpt_stg_nbr = GameDatabase.GetInstance().chartDB.GetDicChapterLoopArrayFirst(loopID).chpt_dvs_nbr * 10,
            chpt_stg_nbr_min = GameDatabase.GetInstance().chartDB.GetDicChapterLoopArrayFirst(loopID).chpt_dvs_nbr * 10,
            chpt_stg_nbr_max = GameDatabase.GetInstance().chartDB.GetDicChapterLoopArrayLast(loopID).chpt_dvs_nbr * 10,
        };

        PlayerPrefs.SetInt(PrefsKeys.prky_StageType, 0);
        GetInstance().ChapterStageType(false);

        PlayerPrefs.SetString(PrefsKeys.prky_LoopChapterDb, JsonUtility.ToJson(loopChapter));
        Routin_ChangeMode(IG.ModeType.CHAPTER_LOOP, true);
    }

    public void ChapterLoopStop()
    {
        loopChapter = new LoopChapter() { isLoop = false };
        PlayerPrefs.SetString(PrefsKeys.prky_LoopChapterDb, JsonUtility.ToJson(loopChapter));

        Routin_ChangeMode(IG.ModeType.CHAPTER_CONTINUE, true);
    }

    void ChapterBossMode()
    {
        Routin_ChangeMode(mode_type, true);
    }

    void ChapterNormalMode()
    {
        Routin_ChangeMode(mode_type, true);
    }

    /// <summary> 챕터 진행도 (반복 모드 진행도) </summary>
    public void MODE_CHAPTER_Progress()
    {
        // StageNbr() 
        bool isBossMnst = StageNbr() == 10;
        bool isBossMode = stage_type == IG.StageType.BOSS_MONSTER;
        int mnstCnt = 10 - stc_ChapterMonstStats.Count;
        gameUIObject.chapterStage.tx_ChapterStage.text =
            string.Format("챕터 {0},  스테이지 {1}<color=yellow>{2}</color>", chapter_db.chpt_id, chapter_db.stg_id, mode_type == IG.ModeType.CHAPTER_LOOP ? "<size=26>[∞]</size>" : "<size=22>[▶▶]</size>");

        if(isBossMnst == true || isBossMode == true)
        {
            gameUIObject.chapterStage.tx_MonCnt.text = "10/10 <color=red>[BOSS]</color>";
        }
        else
        {
            gameUIObject.chapterStage.tx_MonCnt.text = string.Format("{0}/10", StageNbr());
        }
        
        //gameUIObject.chapterStage.tx_MonCnt.text = string.Format("{0}/10 {1}", StageNbr(), bossTxt);
        
    }

    /// <summary> 챕터 진행 파이트 결과 </summary>
    public async void ChapterWinOrLose(bool isUserBoss)
    {
        bool isBoss = StageNbr() >= 10 || stage_type == IG.StageType.BOSS_MONSTER;
        bool isPlayerWinner = myPZ.GetHp() > 0;
        if (isPlayerWinner) // 플레이어 승리 
        {
            GameDatabase.GetInstance().achievementsDB.ASetInCount(GameDatabase.AchievementsDB.Nbr.nbr2, 1); // 업적, nbr2 스테이지 몬스터 처치!
            GameDatabase.GetInstance().dailyMissionDB.ASetInCount(GameDatabase.DailyMissionDB.Nbr.nbr7, 1); // 일일미션, nbr7 몬스터 처치하기! 
            if (isBoss)
                GameDatabase.GetInstance().dailyMissionDB.ASetInCount(GameDatabase.DailyMissionDB.Nbr.nbr8, 1); // 일일미션, nbr8 보스 몬스터 처치하기!

            ChapterStage_Drop(orPZ.eq_rt, orPZ.eq_id, isBoss);// 몬스터 처치시 랜덤 보상 드롭 
            GameDatabase.GetInstance().tableDB.SetStageIncrease();

            // 보스 처치시 && 챕터 진행중일때에만 (반복중X)  
            if (isBoss == true && mode_type == IG.ModeType.CHAPTER_CONTINUE)
            {
                int nextChapterID = GameDatabase.GetInstance().monsterDB.GetChapterDvsNbrFindChapterID(ChapterNbr());
                GameDatabase.GetInstance().achievementsDB.ASetInCount(GameDatabase.AchievementsDB.Nbr.nbr1, nextChapterID, false, false); // 업적 nbr1 챕터 ?, 모든 스테이지 클리어 하기!

                if (chapter_db.chpt_id >= 5)
                {
                    GameDatabase.GetInstance().rankDB.ChapterBossKillAddPvPScoreSave();
                }
            }
        }
        else
        {
            GameDatabase.GetInstance().tableDB.ChapterLoseNextProgress(isBoss);
            if(isBoss == true && mode_type == IG.ModeType.CHAPTER_CONTINUE) // 보스 && 챕터 진행 모드 중 
            {
                //int nowSType = PlayerPrefs.GetInt(PrefsKeys.prky_StageType);
                //PlayerPrefs.SetInt(PrefsKeys.prky_StageType, nowSType == 0 ? 1 : 0);
                GetInstance().ChapterStageType(false);
            }

            await Task.Delay(1000);
        }

        if (mode_type == IG.ModeType.CHAPTER_LOOP)
        {
            PlayerPrefs.SetString(PrefsKeys.prky_LoopChapterDb, JsonUtility.ToJson(loopChapter));
        }

        ChapterNextMonster(isPlayerWinner, isBoss, isUserBoss);
    }

    async void ChapterNextMonster(bool isWin, bool isboss, bool isUserBoss)
    {
        // ### 플레이어 승리 ### 
        if (isWin)
        {
            if (isboss)
            {
                PopUpMng.GetInstance().Open_ChapterWin(chapter_db.chpt_id, chapter_db.stg_id, StageNbr(), orPZ.zbName);
                await ScreenSwitchingToBlack(false, 0.01f);
                
                ChapterMonsterStat();
                InitGame.GetInstance().ZombiePopFromPool(true);
                InitGame.GetInstance().ZombiePopFromPool(false);
                OpponentToFight(true, true);

                await ScreenSwitchingToWhite(false);
                TopUI(mode_type);
            }
            else
            {
                PlayerZombie epz = orPZ;
                orPZ = null;
                InitGame.GetInstance().ZombiePopFromPool(false); // 다음 몬스터 생성

                while (orPZ.igp.state == IG.ZombieState.Ready || GameMng.GetSqr(myPZ.tr.transf.position, orPZ.tr.transf.position) > 32)
                    await Task.Delay(100);
                
                if (myPZ.changeSkill)
                    myPZ.PlayerSettingSkills();

                OpponentToFight(false, true);
                if(epz.zbType == IG.MonsterType.NORM_MONSTER || epz.zbType == IG.MonsterType.NORM_DGN_MONSTER)
                    ObjectPool.GetInstance().PushToPool(epz.tr.objLife.opName, epz.tr.objLife);
                else ObjectPool.GetInstance().PushToPool(epz.tr.objLife.opName, epz.tr.objLife);
            }
        }
        // ### 몬스터의 승리 (플레이어 패배) ### 
        else
        {
            PopUpMng.GetInstance().Open_ChapterLose(chapter_db.chpt_id, chapter_db.stg_id, StageNbr(), orPZ.zbName, isboss);
            await ScreenSwitchingToBlack(false, 0.01f);
            ChapterMonsterStat();
            InitGame.GetInstance().ZombiePopFromPool(true);
            InitGame.GetInstance().ZombiePopFromPool(false);
            OpponentToFight(true, true);

            await ScreenSwitchingToWhite(false);
            TopUI(mode_type);
        }
    }

    /// <summary>
    /// 챕터 스테이지 진행 필드 드롭 
    /// </summary>
    public void ChapterStage_Drop(int eq_rt, int eq_id, bool isBoss)
    {
        int m_chpt_id = GameDatabase.GetInstance().monsterDB.GetChapterDvsNbrFindChapterID(ChapterNbr()); // 진행중인 챕터 ID
        if (m_chpt_id > 0)
        {
            LogPrint.EditorPrint("myPZ.igp.statValue.sop2_val : " + myPZ.igp.statValue.sop6_val + ", " + myPZ.igp.statValue.petSpOpTotalFigures.sop2_value);
            LogPrint.EditorPrint("myPZ.igp.statValue.sop3_val : " + myPZ.igp.statValue.sop6_val + ", " + myPZ.igp.statValue.petSpOpTotalFigures.sop3_value);

            float bns_eq_drop_pct = myPZ.igp.statValue.sop6_val + myPZ.igp.statValue.petSpOpTotalFigures.sop3_value; // 장신구 드랍률 증가 + 펫 전용 옵션  (2.장비 드랍률 증가)
            float bns_gold_rate = myPZ.igp.statValue.sop5_val + myPZ.igp.statValue.petSpOpTotalFigures.sop1_value; // 장신구 골드 획득 증가 + 펫 전용 옵션 (1.퀘스트/장비 판매 골드 획득 증가)
            int drp_gold = GameDatabase.GetInstance().questDB.GetQuestMonsterDropGold(eq_rt, eq_id); // 골드 드랍 획득 
            GameDatabase.GetInstance().tableDB.DropGold(isBoss == true ? drp_gold * 2 : drp_gold, bns_gold_rate);

            var cdb_drop = GameDatabase.GetInstance().chartDB.GetFieldDropRating(m_chpt_id);
            LogPrint.Print("111 cdb_drop ----------- : " + JsonUtility.ToJson(cdb_drop));
            for (int r = 0; r < (isBoss == true ? 3 : 1); r++)
            {
                float accum_pct = 0f;
                float r_pct = GameDatabase.GetInstance().GetRandomPercent();

                for (int fRt = 7; fRt >= 1; fRt--)
                {
                    float drop_pct = 0f;
                    switch (fRt)
                    {
                        case 7: accum_pct += drop_pct = cdb_drop.drop_rt7; break;
                        case 6: accum_pct += drop_pct = cdb_drop.drop_rt6; break;
                        case 5: accum_pct += drop_pct = cdb_drop.drop_rt5; break;
                        case 4: accum_pct += drop_pct = cdb_drop.drop_rt4; break;
                        case 3: accum_pct += drop_pct = cdb_drop.drop_rt3; break;
                        case 2: accum_pct += drop_pct = cdb_drop.drop_rt2; break;
                        case 1: accum_pct += drop_pct = cdb_drop.drop_rt1; break;
                    }

                    if (accum_pct > 0.0f)
                    {
                        if (bns_eq_drop_pct > 0.0f)
                        {
                            if (r_pct < accum_pct + (accum_pct * (bns_eq_drop_pct * 0.01f)))
                            {
                                GameDatabase.GetInstance().tableDB.DropRewardAcquire(m_chpt_id, fRt, drop_pct);
                                break;
                            }
                        }
                        else if (r_pct < accum_pct)
                        {
                            GameDatabase.GetInstance().tableDB.DropRewardAcquire(m_chpt_id, fRt, drop_pct);
                            break;
                        }
                    }
                }
            }
        }
    }

    /// <summary> 화면 전환 </summary>
    private async Task ScreenSwitchingToBlack(bool isFullBlack, float mtp = 0.01f) => await MainUI.GetInstance().PlayScreenPixelSwitchingToBlack(isFullBlack, mtp); // 화면 체인지 효과 (어둡게)
    
    public async Task ScreenSwitchingToWhite (bool isFullBlack) => await MainUI.GetInstance().PlayScreenPixelSwitchingToWhite(isFullBlack); // 화면 체인지 효과 (환하게)
    #endregion

    //##################################
    //##################################
    //##################################
    #region ##### 모드 공통 #####
    public void TopUI(IG.ModeType mdty)
    {
        gameUIObject.chapterStage.go_Root.SetActive(mdty == IG.ModeType.CHAPTER_CONTINUE || mdty == IG.ModeType.CHAPTER_LOOP);
        if (mdty == IG.ModeType.CHAPTER_CONTINUE || mdty == IG.ModeType.CHAPTER_LOOP)
            MainUI.GetInstance().tapStageDropInfo.SetDropInfoView(); // 화면 상단에 드롭정보 표시 

        gameUIObject.dungeonTop.go_Root.SetActive(mdty == IG.ModeType.DUNGEON_TOP);
        gameUIObject.dungeonMine.go_Root.SetActive(mdty == IG.ModeType.DUNGEON_MINE);
        gameUIObject.dungeonRaid.go_Root.SetActive(mdty == IG.ModeType.DUNGEON_RAID);
        gameUIObject.pvpBattleArena.go_Root.SetActive(mdty == IG.ModeType.PVP_BATTLE_ARENA);
    }

    /// <summary>
    /// 모드 변경 
    /// </summary>
    public async Task Routin_ChangeMode(IG.ModeType mdty, bool isFullBlack = false, int onTapID = -1)
    {
        LogPrint.PrintError("---------------- 1 Routin_ChangeMode");
        while (mode_type == IG.ModeType.CHANGE_WAIT) await Task.Delay(100);
        await Task.Delay(100);

        mode_type = IG.ModeType.CHANGE_WAIT;
        await ScreenSwitchingToBlack(isFullBlack, 0.01f);
        await Task.Delay(250);
        LogPrint.PrintError("---------------- 2 Routin_ChangeMode");
        if (myPZ != null) myPZ.Reset();
        if (orPZ != null) orPZ.Reset();

        ObjectPool.GetInstance().ResetOffPool();
        MainUI.GetInstance().tapGameBattleInfo.Init_BuffDebuff();
        
        TopUI(IG.ModeType.CHANGE_WAIT);
        if(onTapID >= 0)
        {
            MainUI.GetInstance().OnTap(onTapID);
            
            if(onTapID == 5)
                MainUI.GetInstance().tapDungeon.TapInfo();
        }

        LogPrint.PrintError("---------------- 3 Routin_ChangeMode");
        mode_type = mdty;
        GetInstance().ChapterMonsterStat();

        // 맵 생성 
        if (mdty == IG.ModeType.CHAPTER_CONTINUE || mdty == IG.ModeType.CHAPTER_LOOP)
        {
            foreach (var item in lDungeonMap)
            {
                if (item.activeSelf)
                    item.SetActive(false);
            }

            for (int i = 0; i < lChapterStageMap.Count; i++)
            {
                lChapterStageMap[i].SetActive(i == chapter_db.map_nbr);
            }
        }
        else
        {
            foreach (var item in lChapterStageMap)
            {
                if (item.activeSelf)
                    item.SetActive(false);
            }

            lDungeonMap[0].SetActive(mdty == IG.ModeType.DUNGEON_TOP);
            lDungeonMap[1].SetActive(mdty == IG.ModeType.DUNGEON_MINE);
            lDungeonMap[2].SetActive(mdty == IG.ModeType.DUNGEON_RAID);
            lDungeonMap[3].SetActive(mdty == IG.ModeType.PVP_BATTLE_ARENA);
            zbCam.DgCam();
        }

        LogPrint.PrintError("---------------- 4 Routin_ChangeMode");
        InitGame.GetInstance().ZombiePopFromPool(true);
        LogPrint.PrintError("---------------- 5 Routin_ChangeMode");
        InitGame.GetInstance().ZombiePopFromPool(false);
        LogPrint.PrintError("---------------- 6 Routin_ChangeMode");
        while (myPZ == null || orPZ == null) await Task.Delay(100);
        LogPrint.PrintError("---------------- 7 Routin_ChangeMode");
        while (!myPZ.gameObject.activeSelf || !orPZ.gameObject.activeSelf) await Task.Delay(100);
        LogPrint.PrintError("---------------- 8 Routin_ChangeMode");
        OpponentToFight(true, true);
        LogPrint.PrintError("---------------- 9 Routin_ChangeMode");
        await ScreenSwitchingToWhite(isFullBlack);
        ConvenienceFunctionMng.GetInstance().UIConvenienceAutoPosion();
        LogPrint.PrintError("---------------- 10 Routin_ChangeMode");
        TopUI(mode_type);
        LogPrint.PrintError("---------------- 11 Routin_ChangeMode");
    }    
    #endregion

    //##################################
    //##################################
    //##################################
    #region ##### 던전 공용 #####
    public Stc_Dungeon stc_DungeonTop = new Stc_Dungeon();
    public Stc_Dungeon stc_DungeonMine = new Stc_Dungeon();
    public Stc_Dungeon stc_DungeonRaid = new Stc_Dungeon();
    public struct Stc_Dungeon
    {
        private int _inDgNbr;
        public int inDgNbr { get { return _inDgNbr; } set { _inDgNbr = value; } }

        public Queue<GameDatabase.CharacterDB.StatValue> qStat; // 던전내 몬스터 스텟 
        public float clearTime;
    }

    /// <summary>
    /// 던전 포기 
    /// </summary>
    public async void DungeonTopExit()
    {
        // ##### 입장권 차감 
        await GameDatabase.GetInstance().tableDB.ConsumDungeonTicket(IG.ModeType.DUNGEON_TOP);
        Routin_ChangeMode(MainUI.GetInstance().tapDungeon.saveMdTy, true, 0);
    }
    /// <summary>
    /// 던전 포기 
    /// </summary>
    public async void DungeonMineExit()
    {
        // ##### 입장권 차감 
        await GameDatabase.GetInstance().tableDB.ConsumDungeonTicket(IG.ModeType.DUNGEON_MINE);
        Routin_ChangeMode(MainUI.GetInstance().tapDungeon.saveMdTy, true, 0);
    }
    /// <summary>
    /// 던전 포기 
    /// </summary>
    public async void DungeonRaidExit()
    {
        // ##### 입장권 차감 
        await GameDatabase.GetInstance().tableDB.ConsumDungeonTicket(IG.ModeType.DUNGEON_RAID);
        Routin_ChangeMode(MainUI.GetInstance().tapDungeon.saveMdTy, true, 0);
    }

    // 던전 시간 흐름 Txt 
    void DungeonTimeText(float sec)
    {
        int minutes = (int)(Math.Floor((double)(sec / 60)));
        int second = (int)(Math.Floor((double)(sec % 3600 % 60)));
        string[] msec = sec.ToString("N2").Split('.');
        tx_dgFlowTime.text = string.Format("{0:00}:{1:00}.{2}", minutes, second, msec[1]);
    }

    [SerializeField] bool isDgFlowTime = false;
    Text tx_dgFlowTime;
    /// <summary>
    /// 던전 시간 흐름 및 종료 
    /// </summary>
    IEnumerator Routin_DungeonTimeFlow()
    {
        stc_DungeonTop.clearTime = 0;
        stc_DungeonMine.clearTime = 0;
        stc_DungeonRaid.clearTime = 0;

        isDgFlowTime = false;
        tx_dgFlowTime.text = "00:00.00";
        yield return new WaitForSeconds(0.25f);
        float sqr = 0;
        while(sqr > 8.25f || sqr == 0)
        {
            yield return null;
            if(myPZ != null && myPZ.targetPz != null)
            {
                sqr = GameMng.GetSqr(myPZ.tr.transf.position, myPZ.targetPz.tr.transf.position);
            }
        }

        while (mode_type == IG.ModeType.CHANGE_WAIT || (myPZ.igp.state != IG.ZombieState.FIGHT && orPZ.igp.state != IG.ZombieState.FIGHT))
            yield return null;

        float t = Time.time;
        float num = 0.0f;
        while (isDgFlowTime == false)
        {
            yield return null;
            num += (((Time.time - t) * 100) * 0.01f) * GameSpeed; // t - start_t;
            t = Time.time;
            DungeonTimeText(num);
        }

        float clar_t = System.Convert.ToSingle(num.ToString("N5"));
        DungeonTimeText(clar_t);
        switch (mode_type)
        {
            case IG.ModeType.DUNGEON_TOP: stc_DungeonTop.clearTime = clar_t;    break;
            case IG.ModeType.DUNGEON_MINE: stc_DungeonMine.clearTime = clar_t;  break;
            case IG.ModeType.DUNGEON_RAID: stc_DungeonRaid.clearTime = clar_t;  break;
        }
    }

    /// <summary>
    /// 던전 이동중 Slider 
    /// </summary>
    IEnumerator DungeonSlider(Slider sd_pgrs)
    {
        float e_val = sd_pgrs.value + 0.25f;
        while (myPZ.igp.state != IG.ZombieState.FIGHT && orPZ.igp.state != IG.ZombieState.FIGHT)
        {
            sd_pgrs.value = Mathf.Lerp(sd_pgrs.value, e_val, Time.deltaTime);
            yield return null;
        }

        sd_pgrs.value = e_val;
    }
    #endregion

    //##################################
    //##################################
    //##################################
    #region ##### 던전 : 도전의 탑 #####
    /// <summary> 도전의 탑 모드 변경 </summary>
    public void ChangeMode_Top (int inNbr)
    {
        stc_DungeonTop.inDgNbr = inNbr;

        gameUIObject.dungeonTop.sd_pgrs.value = 0.0f;
        foreach (var item in gameUIObject.dungeonTop.go_ClearLabel)
            item.SetActive(false);

        gameUIObject.dungeonTop.tx_Nbr.text = string.Format(LanguageGameData.GetInstance().GetString("text.dungeon.top.stage"), stc_DungeonTop.inDgNbr + 1);
       
        Routin_ChangeMode(IG.ModeType.DUNGEON_TOP, true, 0);

        tx_dgFlowTime = gameUIObject.dungeonTop.tx_Timer;
        StartCoroutine(Routin_DungeonTimeFlow());
    }

    /// <summary> 던전 : 도전의 탑 결과 </summary>
    public async void DungeonTopWinOrLose() 
    {
        bool dg_playerWin = orPZ.GetHp() <= 0 && myPZ.GetHp() > 0;
        bool dg_mnstLast = stc_DungeonTop.qStat.Count == 0;
        int mnCnt = stc_DungeonTop.qStat.Count;
        gameUIObject.dungeonTop.go_ClearLabel[mnCnt].SetActive(true);

        // 다음 몬스터 진행 
        if (dg_playerWin == true && dg_mnstLast == false)
        {
            PlayerZombie epz = orPZ;
            orPZ = null;
            InitGame.GetInstance().ZombiePopFromPool(false);
            StartCoroutine(DungeonSlider(gameUIObject.dungeonTop.sd_pgrs));

            while (orPZ.igp.state == IG.ZombieState.Ready || GameMng.GetSqr(myPZ.tr.transf.position, orPZ.tr.transf.position) > 32)
                await Task.Delay(100);

            OpponentToFight(false, true); // 배틀 시작 
            if (epz.zbType == IG.MonsterType.NORM_MONSTER || epz.zbType == IG.MonsterType.NORM_DGN_MONSTER)
                ObjectPool.GetInstance().PushToPool(epz.tr.objLife.opName, epz.tr.objLife);
            else ObjectPool.GetInstance().PushToPool(epz.tr.objLife.opName, epz.tr.objLife);
        }
        else // 결과창 
        {
            PopUpMng.GetInstance().popupNotice.ClosePopUpBox();

            isDgFlowTime = true;
            while (stc_DungeonTop.clearTime == 0)
                await Task.Delay(250);

            gameUIObject.dungeonTop.pu_DungeonReward.gameObject.SetActive(true);
            gameUIObject.dungeonTop.pu_DungeonReward.SetData(IG.ModeType.DUNGEON_TOP, GetInstance().stc_DungeonTop.inDgNbr, dg_playerWin, stc_DungeonTop.clearTime);
        }
    }
    #endregion

    //##################################
    //##################################
    //##################################
    #region ##### 던전 : 광산 #####
    /// <summary> 광산 모드 변경 </summary>
    public void ChangeMode_Mine(int inNbr)
    {
        var clrNbr = GameDatabase.GetInstance().tableDB.GetUserInfo().GetDgNbr(IG.ModeType.DUNGEON_MINE);
        if (inNbr <= clrNbr)
        {
            stc_DungeonMine.inDgNbr = inNbr;
            gameUIObject.dungeonMine.sd_pgrs.value = 0.0f;
            foreach (var item in gameUIObject.dungeonMine.go_ClearLabel)
                item.SetActive(false);

            gameUIObject.dungeonMine.tx_Nbr.text = LanguageGameData.GetInstance().GetString(string.Format("text.dungeon.mine.stage_{0}", stc_DungeonMine.inDgNbr));

            Routin_ChangeMode(IG.ModeType.DUNGEON_MINE, true, 0);

            tx_dgFlowTime = gameUIObject.dungeonMine.tx_Timer;
            StartCoroutine(Routin_DungeonTimeFlow());
        }
        else
        {
            PopUpMng.GetInstance().Open_MessageError("입장 불가");
        }
    }

    /// <summary> 던전 : 광산 결과 </summary>
    public async void DungeonMineWinOrLose() 
    {
        bool dg_playerWin = orPZ.GetHp() <= 0 && myPZ.GetHp() > 0;
        bool dg_mnstLast = stc_DungeonMine.qStat.Count == 0;

        LogPrint.Print("dg_playerWin:" + dg_playerWin + ",dg_mnstLast:" + dg_mnstLast);
        // 다음 몬스터 진행 
        if (dg_playerWin == true && dg_mnstLast == false)
        {
            PlayerZombie epz = orPZ;
            orPZ = null;
            InitGame.GetInstance().ZombiePopFromPool(false);
            StartCoroutine(DungeonSlider(gameUIObject.dungeonMine.sd_pgrs));

            while (orPZ.igp.state == IG.ZombieState.Ready || GameMng.GetSqr(myPZ.tr.transf.position, orPZ.tr.transf.position) > 32)
                await Task.Delay(100);

            OpponentToFight(false, true); // 배틀 시작 
            if (epz.zbType == IG.MonsterType.NORM_MONSTER || epz.zbType == IG.MonsterType.NORM_DGN_MONSTER)
                ObjectPool.GetInstance().PushToPool(epz.tr.objLife.opName, epz.tr.objLife);
            else ObjectPool.GetInstance().PushToPool(epz.tr.objLife.opName, epz.tr.objLife);
        }
        else // 결과창 
        {
            PopUpMng.GetInstance().popupNotice.ClosePopUpBox();

            isDgFlowTime = true;
            while (stc_DungeonMine.clearTime == 0)
                await Task.Delay(250);

            gameUIObject.dungeonMine.pu_DungeonReward.gameObject.SetActive(true);
            gameUIObject.dungeonMine.pu_DungeonReward.SetData(IG.ModeType.DUNGEON_MINE, GetInstance().stc_DungeonMine.inDgNbr, dg_playerWin, stc_DungeonMine.clearTime);
        }
    }
    #endregion

    //##################################
    //##################################
    //##################################
    #region ##### 던전 : 레이드 #####
    public void ChangeMode_Raid(int inNbr)
    {
        var clrNbr = GameDatabase.GetInstance().tableDB.GetUserInfo().GetDgNbr(IG.ModeType.DUNGEON_RAID);
        LogPrint.Print("inNbr : " + inNbr + ", clrNbr : " + clrNbr);

        if (inNbr <= clrNbr)
        {
            stc_DungeonRaid.inDgNbr = inNbr;
            gameUIObject.dungeonRaid.tx_Nbr.text = LanguageGameData.GetInstance().GetString(string.Format("text.dungeon.raid.stage_{0}", stc_DungeonRaid.inDgNbr));

            Routin_ChangeMode(IG.ModeType.DUNGEON_RAID, true, 0);

            tx_dgFlowTime = gameUIObject.dungeonRaid.tx_Timer;
            StartCoroutine(Routin_DungeonTimeFlow());
        }
        else
        {
            PopUpMng.GetInstance().Open_MessageError("입장불가");
        }
    }

    /// <summary> 던전 : 레이드 결과 </summary>
    public async void DungeonRaidWinOrLose()
    {
        bool dg_playerWin = orPZ.GetHp() <= 0 && myPZ.GetHp() > 0;
        bool dg_mnstLast = stc_DungeonRaid.qStat.Count == 0;
        // 다음 몬스터 진행 
        if (dg_playerWin == true && dg_mnstLast == false)
        {
            PlayerZombie epz = orPZ;
            orPZ = null;
            InitGame.GetInstance().ZombiePopFromPool(false);

            while (orPZ.igp.state == IG.ZombieState.Ready || GameMng.GetSqr(myPZ.tr.transf.position, orPZ.tr.transf.position) > 32)
                await Task.Delay(100);

            OpponentToFight(false, true); // 배틀 시작 
            if (epz.zbType == IG.MonsterType.NORM_MONSTER || epz.zbType == IG.MonsterType.NORM_DGN_MONSTER)
                ObjectPool.GetInstance().PushToPool(epz.tr.objLife.opName, epz.tr.objLife);
            else ObjectPool.GetInstance().PushToPool(epz.tr.objLife.opName, epz.tr.objLife);
        }
        else // 결과창 
        {
            PopUpMng.GetInstance().popupNotice.ClosePopUpBox();

            isDgFlowTime = true;
            while (stc_DungeonRaid.clearTime == 0)
                await Task.Delay(250);

            gameUIObject.dungeonRaid.pu_DungeonReward.gameObject.SetActive(true);
            gameUIObject.dungeonRaid.pu_DungeonReward.SetData(IG.ModeType.DUNGEON_RAID, GetInstance().stc_DungeonRaid.inDgNbr, dg_playerWin, stc_DungeonRaid.clearTime);
        }
    }
    #endregion

    //##################################
    //##################################
    //##################################
    #region PVP 배틀 아레나
    
    public GameDatabase.PvPBattle.Data pvpOpponentDb;
    /// <summary> PvP 시작 </summary>
    public async void ChangeMode_PvP()
    {
        if (GetInstance().mode_type == IG.ModeType.CHAPTER_CONTINUE || GetInstance().mode_type == IG.ModeType.CHAPTER_LOOP)
        {
            string myNick = BackendGpgsMng.backendUserInfo.m_nickname;
            string orNick = GameDatabase.GetInstance().pvpBattle.GetDataBattleDbOr().gamerInfo.gamer_nickName;
            Loading.Open_PvpStartFull(myNick, orNick);

            MainUI.GetInstance().tapDungeon.saveMdTy = GetInstance().mode_type; // 시작 전 챕터 진행중타입 (loop or play) 
            await Routin_ChangeMode(IG.ModeType.PVP_BATTLE_ARENA);

            while (GetInstance().mode_type != IG.ModeType.PVP_BATTLE_ARENA) await Task.Delay(100);
            await Task.Delay(1000);

            PopUpMng.GetInstance().Close_PvpBattleOpponent();
            PopUpMng.GetInstance().Open_PvpBattleHome();
            Loading.Close_PvpStartFull();
        }
    }

    /// <summary> PvP 배틀 결과 </summary>
    public async void PvpWinOrLose()
    {
        PopUpMng.GetInstance().popupNotice.ClosePopUpBox();

        await Task.Delay(500);
        bool dg_playerWin = orPZ.GetHp() <= 0 && myPZ.GetHp() > 0;
        PopUpMng.GetInstance().Open_TopScreenPvpWinOrLose(dg_playerWin, GameDatabase.GetInstance().pvpBattle.GetDataBattleDbOr().gamerInfo.gamer_nickName);

        await Task.Delay(1500);

        PopUpMng.GetInstance ().Close_TopScreenPvpWinOrLose();
        PopUpMng.GetInstance().Open_PvpBattleResult(dg_playerWin, myPZ.igp.parts, myPZ.igp.statValue.combat_power);
    }
    #endregion

}
