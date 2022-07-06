using BackEnd;
using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class PopUpMng : MonoSingleton<PopUpMng>
{
    public BackendWaitingLoading backendWaitingLoading;
    [SerializeField] List<OnPop> onPopUpList = new List<OnPop>();
    public int GetOnPopUpCount() => onPopUpList.Count;

    public void AddOnPop(OnPop op) => onPopUpList.Add(op);
    public void RmvOnPop(OnPop op) => onPopUpList.Remove(op);
    void RmvOnPop()
    {
        var mdty = GameMng.GetInstance().mode_type;
        if (mdty == IG.ModeType.CHAPTER_LOOP || mdty == IG.ModeType.CHAPTER_CONTINUE)
        {
            if (onPopUpList.Count > 0)
            {
                int arrLastIndx = onPopUpList.Count - 1;
                if (onPopUpList[arrLastIndx].isLockEscape == true)
                {
                    PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener("게임을 종료하시겠습니까?", QuitGame);
                }
                else
                {
                    OnPop opLast = onPopUpList[arrLastIndx];
                    onPopUpList[arrLastIndx].gameObject.SetActive(false);
                    onPopUpList.Remove(opLast);
                }
            }
            else PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener("게임을 종료하시겠습니까?", QuitGame);
        }
    }

    public void RmvAllClosePop(GameObject exception = null)
    {
        for (int i = 0; i < onPopUpList.Count; i++)
        {
            if(onPopUpList.Count > 0)
            {
                foreach (var item in onPopUpList)
                {
                    if (GameObject.Equals(exception, item.gameObject) == false)
                    {
                        item.gameObject.SetActive(false);
                        onPopUpList.Remove(item);
                        break;
                    }
                }
            }
        }
    }

    //public void WaitLoadingTask(Task[] task)
    //{
    //    if (task == null)
    //        return;

    //    if(task.Length > 0)
    //    {
    //        backendWaitingLoading.gameObject.SetActive(true);
    //        backendWaitingLoading.SetBkendTaskWaiting(task);
    //    }
    //}

    //public async Task AWaitTaskList(List<Task> list)
    //{
    //    if(list.Count > 0)
    //    {
    //        backendWaitingLoading.gameObject.SetActive(true);
    //        await backendWaitingLoading.Await(list);
    //    }
    //}

    //[SerializeField] GameObject goBottomLoading;
    //public void LoadingBottom() => goBottomLoading.SetActive(!goBottomLoading.activeSelf);

    void Start()
    {
        popUpDailyProductReward.gameObject.SetActive(true);
        popUpDailyProductReward.goRoot.SetActive(false);
        popUpDailyProductReward.InitInfoDailyDia();
        
        StartCoroutine("CheckLoopKeyCodeEscape");
    }

    IEnumerator CheckLoopKeyCodeEscape()
    {
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                RmvOnPop();
            }

            yield return null;
        }
    }

    async void QuitGame()
    {
        Loading.Full(false);
        BackendGpgsMng.GetInstance().isBtnClickQuit = true;
        await BackendGpgsMng.GetInstance ().SendFocusDB();

#if !UNITY_EDITOR
        Application.Quit();
#endif
    }
    
    // ##################################################
    #region ##### PvP 승리 메시지 ######
    [SerializeField] PopUpPvpWinnerComment popUpPvpWinnerComment;
    public void Open_PopUpPvpWinnerComment()
    {
        popUpPvpWinnerComment.gameObject.SetActive(true);
        popUpPvpWinnerComment.SetData();
    }
    #endregion

    // ##################################################
    #region ##### 오프라인 보상 ######
    [SerializeField] PopUpOfflineReward popUpOfflineReward;
    public void Open_OfflineReward(System.DateTime nDate)
    {
        string endDate = GameDatabase.GetInstance().tableDB.GetUserInfo().m_game_end_date;
        DateTime tryEndDate;
        if (DateTime.TryParse(endDate, out tryEndDate) == false)
            tryEndDate = nDate;

        int totalHour = (int)(nDate - tryEndDate).Hours;
        int totalMinute = (int)(nDate - tryEndDate).TotalMinutes;
        LogPrint.Print("-----PopUpOfflineReward----- totalHour : " + totalHour);
        LogPrint.Print("-----PopUpOfflineReward----- totalMinute : " + totalMinute);

        LogPrint.Print("<color=yellow> -----Open_OfflineReward----- endDate ; " + endDate + ", tryEndDate : " + tryEndDate + "</color>");
        if(totalHour > 0 || totalMinute >= 30)
        {
            popUpOfflineReward.gameObject.SetActive(true);
            popUpOfflineReward.SetData(nDate, tryEndDate);
        }

        //// 이전 종료 시간을 현재 접속시간으로 변경 
        //BackendReturnObject userinfo_bro = null;
        //var userinfo_db = GameDatabase.GetInstance().tableDB.GetUserInfo();
        //userinfo_db.m_game_end_date = await BackendGpgsMng.GetInstance().GetBackendTime();
        //Param userinfo_prm = new Param();
        //userinfo_prm.Add("m_game_end_date", userinfo_db.m_game_end_date);
        //SendQueue.Enqueue(Backend.GameInfo.Update, BackendGpgsMng.tableName_UserInfo, userinfo_db.indate, userinfo_prm, callback => { userinfo_bro = callback; });
        //while (userinfo_bro == null) { await Task.Delay(100); }
        //GameDatabase.GetInstance().tableDB.SetUpdate_UserInfo(userinfo_db, true);
    }
    #endregion

    // ##################################################
    #region ##### 알림창 [ 1줄라인 형식, Box형식 확인 팝업 ] ######
    public PopUpNotice popupNotice;
    public void Click_Open_NoticeBox(int _e) { Open_NoticeBox((popupnotice_enum)_e); }
    public void Open_NoticeBox(popupnotice_enum _e, string _text = "")
    {
        popupNotice.gameObject.SetActive(true);
        popupNotice.OpenPopUpBox(_e, _text);
    }

    string last_msgNotif = "";
    public void LastMsgReset() => last_msgNotif = "";
    /// <summary> 1줄짜리 간단한 알림 및 경고 </summary>
    public void Open_MessageError(string str_id) 
    {
        if (last_msgNotif == str_id)
        {
            popupNotice.SetOneMsg(str_id, Color.red);
            return;
        }
        else last_msgNotif = str_id;

        popupNotice.SetEnqueueOneMsg(str_id, Color.red);
    }
    
    public void Open_MessageNotif(string str_id)
    {
        if(last_msgNotif == str_id)
            return;

        last_msgNotif = str_id;
        popupNotice.SetEnqueueOneMsg(str_id, Color.white);
    }

    /// <summary>
    /// 골드 드롭 알림 
    /// </summary>
    public void Open_DropGold(int gold, int bonus_gold) => popupNotice.SetDropGoldQueue(gold, bonus_gold);

    /// <summary>
    /// 장비 드롭알림 
    /// </summary>
    public void Open_DropEquip(GameDatabase.TableDB.Equipment dropEquip) => popupNotice.DropViewEquipQueue(dropEquip);

    /// <summary>
    /// 아이템 드롭 알림 
    /// </summary>
    public void Open_DropItem(GameDatabase.TableDB.Item dropItem) => popupNotice.DropViewItemQueue(dropItem);

    /// <summary>
    /// 스킬 드롭 알림 
    /// </summary>
    public void OpenDropSkill (GameDatabase.TableDB.Skill dropSkill) => popupNotice.DropViewSkillQueue(dropSkill);

    /// <summary>
    /// 장비 판매(분해) 알림 
    /// </summary>
    public void OpenDropInfoViewAutoSaleDecomposition(bool isSaleOrDecomp, bool isAcOrEq, int eqTy, int eqRt, int eqId, int rwdCnt)
                    => popupNotice.DropSaleDecompositionQueue(isSaleOrDecomp, isAcOrEq, eqTy, eqRt, eqId, rwdCnt);

    #endregion

    // ##################################################
    #region ##### 장비 판매/분해 #####
    public PopUpSale popUpSale;
    public void Open_Sale()
    {
        popUpSale.gameObject.SetActive(true);
        popUpSale.SetData();
    }
    #endregion

    // ##################################################
    #region ##### 장비/아이템 정보 보기 #####
    [SerializeField] PopUpViewEquipmentInfo popUpViewEquipmentInfo;
    public void Open_ViewItemInfo(GameDatabase.TableDB.Equipment eqDb, bool isBotmBtnHide = false, UnityAction dbChangeAction = null, bool isOpShop = false, bool isMyDb = true)
    {
        popUpViewEquipmentInfo.gameObject.SetActive(true);
        popUpViewEquipmentInfo.SetData(eqDb, isBotmBtnHide, dbChangeAction, isOpShop, isMyDb);
    }

    [SerializeField] PopUpViewItemInfo popUpViewItemInfo;
    public void Open_ViewItemInfo(GameDatabase.TableDB.Item itDb)
    {
        popUpViewItemInfo.gameObject.SetActive(true);
        popUpViewItemInfo.SetData(itDb);
    }

    public void Open_ViewGoodsInfo(int gdsType, int gdsCnt)
    {
        popUpViewItemInfo.gameObject.SetActive(true);
        popUpViewItemInfo.SetData(gdsType, gdsCnt);
    }

    /// <summary>
    /// 컨텐츠 탭에서 아이템 미리보기 팝업 
    /// </summary>
    public void Open_ContentsRewardItemInfo(Sprite icon, string rwd_name)
    {
        popUpViewItemInfo.gameObject.SetActive(true);
        popUpViewItemInfo.ContentsRewardPreview(icon, rwd_name);
    }

    public void Open_ContentsDungeonRewardItemInfo (Sprite spr_icon, string rwd_name, int rt = -1)
    {
        popUpViewItemInfo.gameObject.SetActive(true);
        popUpViewItemInfo.ContentsDungeonRewardPreview(spr_icon, rwd_name, rt);
    }
    /// <summary>
    /// 컨텐츠 탭의 각 던전탭에서 스킬 보상 미리보기 팝업 
    /// </summary>
    public void Open_ContentsDungeonSkillRewardItemInfo(int sk_idx, int sk_cnt)
    {
        popUpViewItemInfo.gameObject.SetActive(true);
        popUpViewItemInfo.ContentsDungeonSkillRewardPreview(sk_idx, sk_cnt);
    }
    #endregion

    // ##################################################
    #region ##### 인벤토리 정렬 #####
    public PopUpInventorySort popUpInventorySort;
    public void Open_InventorySort()
    {
        popUpInventorySort.gameObject.SetActive(true);
        popUpInventorySort.Init(true);
    }
    #endregion

    // ##################################################
    #region ##### 스킬 정보 ##### 
    public PopUpSkillInfoMenu popUpSkillInfoMenu;
    public void PopUp_SkinnInfoMenu(GameDatabase.TableDB.Skill sk_data, bool isPopUse, bool isMainPreview = false, bool isOrhers = false)
    {
        popUpSkillInfoMenu.gameObject.SetActive(true);
        popUpSkillInfoMenu.SetData(sk_data, isPopUse, isMainPreview, isOrhers);
    }
    #endregion

    // ##################################################
    #region ##### 랭킹 팝업 #####
    public PopUpRank popUpRank;
    public void Open_Ranking ()
    {
        if (popUpRank.gameObject.activeSelf)
            return;

        RmvAllClosePop();
        popUpRank.gameObject.SetActive(true);
        popUpRank.OnPopRank();
    }
    #endregion

    // ##################################################
    #region ##### 던전 포기 #####
    public void Click_DungeonQuit()
    {
        if (GameMng.GetInstance().mode_type == IG.ModeType.DUNGEON_TOP)
        {
            string stxt = LanguageGameData.GetInstance().GetString("text.dungeon.top.exit");
            popupNotice.OpenAskNoticeBoxListener(stxt, GameMng.GetInstance().DungeonTopExit);
        }
        else if (GameMng.GetInstance().mode_type == IG.ModeType.DUNGEON_MINE)
        {
            string stxt = LanguageGameData.GetInstance().GetString("text.dungeon.mine.exit");
            popupNotice.OpenAskNoticeBoxListener(stxt, GameMng.GetInstance().DungeonMineExit);
        }
        else if (GameMng.GetInstance().mode_type == IG.ModeType.DUNGEON_RAID)
        {
            string stxt = LanguageGameData.GetInstance().GetString("text.dungeon.raid.exit");
            popupNotice.OpenAskNoticeBoxListener(stxt, GameMng.GetInstance().DungeonRaidExit);
        }
    }
    #endregion

    // ##################################################
    #region ##### 출석부 #####
    public PopUpAttendanceBook attendanceBook;
    public void Open_AttendanceBook(bool isClick)
    {
        if(isClick)
        {
            if (attendanceBook.gameObject.activeSelf)
                return;

            RmvAllClosePop();
        }

        attendanceBook.gameObject.SetActive(true);
        attendanceBook.SetData();
    }

    public bool GetIsRewardToDay() => attendanceBook.GetRewrdToDay();
    #endregion

    // ##################################################
    #region ##### 업적 #####
    [SerializeField] PopUpAchievement achievement;
    public void Click_OpenAchievemet()
    {
        if (achievement.gameObject.activeSelf)
            return;

        RmvAllClosePop();
        if (!achievement.gameObject.activeSelf)
            achievement.gameObject.SetActive(true);

        achievement.SetData();
    }

    /// <summary>
    /// 업적 새로 고침 
    /// </summary>
    public void Refresh_Achievemet()
    {
        if (achievement.gameObject.activeSelf)
            achievement.SetData(true);
    }
    public PopUpAchievement.Type AchievementType() => achievement.type;
    #endregion

    // ##################################################
    #region ##### 메일 #####
    public PopUpMail mail;
    public async void Open_Mail()
    {
        if (mail.gameObject.activeSelf)
            return;

        Task tsk1 = GameDatabase.GetInstance().mailDB.AGetAll();
        while (Loading.Full(tsk1.IsCompleted) == false) await Task.Delay(100);

        RmvAllClosePop();
        mail.gameObject.SetActive(true);
        mail.SetData();
    }
    #endregion

    // ##################################################
    #region ##### 설정 #####
    [SerializeField] PopUpSetting setting;
    public void Open_Setting()
    {
        if (setting.gameObject.activeSelf)
            return;

        RmvAllClosePop();
        setting.gameObject.SetActive(true);
        setting.SetData();
    }
    #endregion

    // ##################################################
    #region ##### 유저 정보 (채팅, 랭킹)#####
    [SerializeField] PopUpUserInfo popUpUserInfo;
    public void OpenUserInfo(string nickName, bool isRankOrChat, bool isBlock, string gamerInDate = "")
    {
        popUpUserInfo.gameObject.SetActive(true);
        popUpUserInfo.SetData(nickName, isRankOrChat, isBlock, gamerInDate);
    }
    #endregion

    // ##################################################
    #region ##### 유저 신고 ######
    [SerializeField] PopUpChatReport chatReport;
    public void Open_ChatReport(string nickName)
    {
        chatReport.gameObject.SetActive(true);
        chatReport.SetData(nickName);
    }
    #endregion

    // ##################################################
    #region ##### 유저 차단 리스트 ######
    [SerializeField] PopUpChatBlockList chatBlockList;
    public void Open_ChatBlockList()
    {
        chatBlockList.gameObject.SetActive(true);
        chatBlockList.SetData();
    }

    public void ChatBlockList_UnBlock (string nickName)
    {
        chatBlockList.ASetUnBlock(nickName);
    }
    #endregion

    // ##################################################
    #region ##### 자동 물약 사용 ######
    [SerializeField] PopUpAutoPotion popUpAutoPotion;
    public void Open_AutoPotion()
    {
        popUpAutoPotion.gameObject.SetActive(true);
        popUpAutoPotion.Init();
    }
    #endregion

    // ##################################################
    #region ##### 자동 장비 판매 ######
    [SerializeField] PopUpAutoSale popUpAutoSale;
    public void Open_AutoSale()
    {
        popUpAutoSale.gameObject.SetActive(true);
        popUpAutoSale.Init();
    }
    #endregion

    // ##################################################
    #region ##### 기간제 상품 보상 팝업 ######
    [SerializeField] PopUpDailyProductReward popUpDailyProductReward;
    public void OnInit_DailyProductReward()
    {
        popUpDailyProductReward.InitInfoDailyDia();
    }
    public void Open_DailyProductReward()
    {
        RmvAllClosePop();
        if (popUpDailyProductReward.gameObject.activeSelf == false)
            popUpDailyProductReward.gameObject.SetActive(true);

        popUpDailyProductReward.goRoot.SetActive(true);
        popUpDailyProductReward.Init();
    }
    #endregion

    // ##################################################
    #region ##### 아이템 상점 : 장비, 장신구 강화석 구매 / 강화 축복 주문서 구매 창
    [SerializeField] PopUpShopItemPurchase popUpShopItemPurchase;
    public void Open_ShopItemPurchase(PopUpShopItemPurchase.PurchaseType iType)
    {
        popUpShopItemPurchase.gameObject.SetActive(true);
        popUpShopItemPurchase.Init(iType);
    }
    #endregion

    // ##################################################
    #region ##### 교환 상점 : 장비 조각 -> 장비 교환 
    [SerializeField] PopUpShopPieceExchange popUpShopPieceExchange;
    public void Open_ShopPieceExchange(GameDatabase.TableDB.Item iItem, UnityAction uAct)
    {
        popUpShopPieceExchange.gameObject.SetActive(true);
        popUpShopPieceExchange.Init(iItem, uAct);
    }
    #endregion

    // ##################################################
    #region ##### 행운 상점 : 장비/장신구 뽑기 결과 
    public PopUpShopLuckEquipResult popUpShopLuckEquipResult;
    public void Open_ShopLuckEquipResultEmpty()
    {
        popUpShopLuckEquipResult.gameObject.SetActive(true);
        popUpShopLuckEquipResult.Init();
    }

    public void Open_ShopLuckEquipResult(List<AcceResult> list, string title, bool isTapShopLuck, string type) // type -> 장비 equip, 장신구 acce, 펫 pet 
    {
        if(list.Count > 0)
        {
            popUpShopLuckEquipResult.gameObject.SetActive(true);
            popUpShopLuckEquipResult.SetData(list, title, isTapShopLuck, type);
        }
    }

    public void Close_ShopLuckEquipResult() => popUpShopLuckEquipResult.gameObject.SetActive(false);
    #endregion

    // ##################################################
    #region ##### 장비 도감 
    public PopUpEquipmentEncyclopedia popUpEquipmentEncyclopedia;
    public void Open_EquipmentEncyclopedia()
    {
        if (popUpEquipmentEncyclopedia.gameObject.activeSelf)
            return;

        RmvAllClosePop();
        popUpEquipmentEncyclopedia.gameObject.SetActive(true);
        popUpEquipmentEncyclopedia.SetData();
        EquipEncyclopediaTapNotice();
    }

    /// <summary>
    /// 도감 팝업에서 강화 가능 탭 새로 고침 
    /// </summary>
    public void EquipEncyclopediaTapNotice()
    {
        if(popUpEquipmentEncyclopedia.gameObject.activeSelf)
            popUpEquipmentEncyclopedia.CheckNotifNeedCount();
    }

    /// <summary>
    /// 셀 새로 고침 (위치 변화 없이)
    /// </summary>
    public void RefreshCells_EquipEncyclope(int eqRt)
    {
        if (popUpEquipmentEncyclopedia.gameObject.activeSelf)
            popUpEquipmentEncyclopedia.initOnStartEquipmentEncyclopedia.RefreshCells(eqRt, true);
    }

    void Open_EquipmentEncyclopediaRating(int rt)
    {
        Open_EquipmentEncyclopedia();
        popUpEquipmentEncyclopedia.Click_RatingChange(rt);
    }
    #endregion

    // ##################################################
    #region ##### 장비 숙련도 
    public PopUpProficiency popUpProficiency;
    public void Open_EquipmentProficiency()
    {
        if (popUpProficiency.gameObject.activeSelf)
            return;

        popUpProficiency.gameObject.SetActive(true);
        popUpProficiency.SetData();
    }

    #endregion

    // ##################################################
    #region ##### 스테이지 바복 
    [SerializeField] PopUpStageLoop popUpStageLoop;
    public void Open_StageLoop()
    {
        if (popUpStageLoop.gameObject.activeSelf)
            return;

        RmvAllClosePop();
        popUpStageLoop.gameObject.SetActive(true);
        popUpStageLoop.SetData();
    }

    public void Close_StageLoop() => popUpStageLoop.gameObject.SetActive(false);
    #endregion

    // ##################################################
    #region ##### 전투 결과 (챕터, pvp, 던전)
    [SerializeField] LoserScreen chapterLoserScreen;
    [SerializeField] LoserScreen pvpLoserScreen;
    public void Open_ChapterWin(int chapter, int stage, int stage_nbr, string zbName)
    {
        chapterLoserScreen.gameObject.SetActive(true);
        chapterLoserScreen.ChapterMonsterBossWin(chapter, stage, stage_nbr, zbName);
    }

    public void Open_ChapterLose(int chapter, int stage, int stage_nbr, string zbName, bool isBoss)
    {
        chapterLoserScreen.gameObject.SetActive(true);
        if(isBoss)
            chapterLoserScreen.ChapterMonsterBossLose(chapter, stage, stage_nbr, zbName);
        else
            chapterLoserScreen.ChapterLose(chapter, stage, stage_nbr);
    }

    public void Open_TopScreenPvpWinOrLose(bool isWin, string nickName)
    {
        pvpLoserScreen.gameObject.SetActive(true);
        if (isWin)
            pvpLoserScreen.PvpWin(nickName);
        else
            pvpLoserScreen.PvpLose(nickName);
    }
    public void Close_TopScreenPvpWinOrLose() => pvpLoserScreen.gameObject.SetActive(false);
    #endregion

    // ##################################################
    #region ##### Pvp 배틀 상대 정보 
    [SerializeField] PopUpPvpBattleOpponentInfo popUpPvpBattleOpponentInfo;
    public void Open_PvpBattleOpponent()
    {
        popUpPvpBattleOpponentInfo.gameObject.SetActive(true);
        popUpPvpBattleOpponentInfo.AWait();
    }
    public void Open_PvpBattleOpponentInfo()
    {
        if(popUpPvpBattleOpponentInfo.gameObject.activeSelf == false)
            popUpPvpBattleOpponentInfo.gameObject.SetActive(true);

        popUpPvpBattleOpponentInfo.SetData();
    }

    public void Close_PvpBattleOpponent() => popUpPvpBattleOpponentInfo.gameObject.SetActive(false);
    #endregion

    // ##################################################
    #region ##### Pvp 배틀 결과 팝업 
    public PopUpPvpBattleResult popUpPvpBattleResult;
    public void Open_PvpBattleResult(bool isMyWin, IG.PartsIdx prtIdx, long myCombat)
    {
        popUpPvpBattleResult.gameObject.SetActive(true);
        popUpPvpBattleResult.SetData(isMyWin, myCombat);
    }
    #endregion

    // ##################################################
    #region ##### Pvp 배틀 진행중 홈 하단 탭 
    [SerializeField] PopUpPvpBattleHome popUpPvpBattleHome;
    public void Open_PvpBattleHome()
    {
        popUpPvpBattleHome.gameObject.SetActive(true);
        popUpPvpBattleHome.SetData();
    }

    public async void Close_PvpBattleHome()
    {
        while (GameMng.GetInstance().mode_type != IG.ModeType.CHANGE_WAIT) await Task.Delay(100);
        while (GameMng.GetInstance().mode_type == IG.ModeType.CHANGE_WAIT) await Task.Delay(100);

        if(MainUI.GetInstance().tapDungeon.uiPvPBattleArena.goSubTapMatching.activeSelf)
            MainUI.GetInstance().tapDungeon.uiPvPBattleArena.initOnStartPvPBattleArenaMatching.SetInit();

        popUpPvpBattleHome.gameObject.SetActive(false);
    }
    #endregion

    // ##################################################
    #region ##### 미션/업적 바로가기 
    public async void Open_AchievementsMoveTap(bool _isDaily, int _nbr)
    {
        RmvAllClosePop();

        if (_isDaily)
        {
            switch (_nbr)
            {
                case 0: // 오늘 출석 체크 하기
                    Open_AttendanceBook(true);
                    return;
                case 1: // 친구에게 게임 공유하기
                    MainUI.GetInstance().androidShareUsing.Share();
                    await Task.Delay(500);
                    await GameDatabase.GetInstance().dailyMissionDB.ASetInCount(GameDatabase.DailyMissionDB.Nbr.nbr1, 1); // 일일미션, nbr1 친구에게 게임 공유하기 
                    return;
                case 2: // 장비 강화 성공 
                    MoveSmithy(SmithyTapType.Enhancement, SmithyListType.EQUIP_WEAPON_SHIELD);
                    break;
                //case 3: // 장비 획득 
                //    break;
                case 4: // 컨텐츠 탭 : PvP 배틀 아레나 입장 
                case 5: // 배틀 아레나 승리하기!
                    MainUI.GetInstance().ClickMenuButton(4);
                    MainUI.GetInstance().tapDungeon.Click_TapOpen(4);
                    break;
                case 6: // 장비/장신구 무룍소환 하기 
                    MainUI.GetInstance().Listener_MoveLuckShop();
                    break;
                //case 7: // 몬스터 처치
                //    break;
                //case 8: // 보스 몬스터 처치
                //    break;
                case 9: // 컨텐츠 탭 : 던전 : 도전의탑 입장 
                    MainUI.GetInstance().ClickMenuButton(4);
                    MainUI.GetInstance().tapDungeon.Click_TapOpen(1);
                    break;
                case 10: // 컨텐츠 탭 : 던전 : 광산 입장 
                    MainUI.GetInstance().ClickMenuButton(4);
                    MainUI.GetInstance().tapDungeon.Click_TapOpen(2);
                    break;
                case 11: // 컨텐츠 탭 : 던전 : 레이드 입장 
                    MainUI.GetInstance().ClickMenuButton(4);
                    MainUI.GetInstance().tapDungeon.Click_TapOpen(3);
                    break;
                case 12: // 장비 판매/분해 
                    Open_Sale();
                    return;
                default: return;
            }
        }
        else
        {
            switch (_nbr)
            {
                case 0: // 카페 가입 하기! 
                    Application.OpenURL("https://cafe.naver.com/ssapgamez");
                    GameDatabase.GetInstance().achievementsDB.ASetInCount(GameDatabase.AchievementsDB.Nbr.nbr0, 1, false, true); // 업적, nbr0 카페 가입 하기!
                    return;
                //case 1: // 챕터 ?, 모든 스테이지 클리어 하기!
                //    break;
                //case 2: // 몬스터 처치!
                //    break;
                case 3: // 장비 판매/분해!
                    Open_Sale();
                    return;
                case 4: // 무기 강화 레벨 ? 달성!
                case 5: // 방패 강화 레벨 ? 달성!
                    MoveSmithy(SmithyTapType.Enhancement, SmithyListType.EQUIP_WEAPON_SHIELD);
                    break;
                case 6: // 방어구 강화 레벨 ? 달성!
                    MoveSmithy(SmithyTapType.Enhancement, SmithyListType.EQUIP_COSTUME);
                    break;
                case 7: // 장신구 강화 레벨 ? 달성!
                    MoveSmithy(SmithyTapType.Enhancement, SmithyListType.EQUIP_ACCE);
                    break;
                case 8: // 장신구 합성 성공! 
                case 9: // 장신구 합성 실패 
                    MoveSmithy(SmithyTapType.OrnamentSynthesis, SmithyListType.EQUIP_ACCE);
                    break;
                case 10: // PvP 배틀 아레나 승리 1회!
                    MainUI.GetInstance().ClickMenuButton(4);
                    MainUI.GetInstance().tapDungeon.Click_TapOpen(4);
                    break;
                case 11: // 일반 장비 도감 강화 100% 완성하기!
                case 12: // 중급 장비 도감 강화 100% 완성하기!
                case 13: // 고급 장비 도감 강화 100% 완성하기!
                case 14: // 희귀 장비 도감 강화 100% 완성하기!
                case 15: // 영웅 장비 도감 강화 100% 완성하기!
                case 16: // 고대 장비 도감 강화 100% 완성하기!
                case 17: // 전설 장비 도감 강화 100% 완성하기!
                    int rt = _nbr == 11 ? 1 : _nbr == 12 ? 2 : _nbr == 13 ? 3 : _nbr == 14 ? 4 : _nbr == 15 ? 5 : _nbr == 16 ? 6 : 7;
                    Open_EquipmentEncyclopediaRating(rt);
                    break;
                case 18: // 퀘스트 100레벨 달성하기!
                    MainUI.GetInstance().ClickMenuButton(0);
                    break;
                case 19: // 장신구 옵션 변경하기!
                    MoveSmithy(SmithyTapType.OrnamentChangeOptions, SmithyListType.EQUIP_ACCE);
                    break;
                case 20: // 도전의 탑 층 클리어하기!
                    MainUI.GetInstance().ClickMenuButton(4);
                    MainUI.GetInstance().tapDungeon.Click_TapOpen(1);
                    break;
                case 21: // 배틀 아레나 연승 하기!
                    MainUI.GetInstance().ClickMenuButton(4);
                    MainUI.GetInstance().tapDungeon.Click_TapOpen(4);
                    break;
                default: return;
            }
        }
    }

    /// <summary>
    /// 대장간 이동 
    /// </summary>
    void MoveSmithy(SmithyTapType smthType, SmithyListType listType)
    {
        MainUI.GetInstance().OnTap(2);
        MainUI.GetInstance().tapSmithy.smithyType = SmithyTapType.None;
        MainUI.GetInstance().tapSmithy.smithyListType = listType;
        PopUpMng.GetInstance().popUpInventorySort.enum_SortInvnHighLow = SortInventorytHighLow.HIGH_TO_LOW;
        PopUpMng.GetInstance().popUpInventorySort.enum_SortInventory = SortInventory.RATING;

        int bottomBtnID = smthType == SmithyTapType.Enhancement ? 1 : smthType == SmithyTapType.OrnamentChangeOptions ? 4 : smthType == SmithyTapType.OrnamentSynthesis ? 5 : 1;
        int rightBtnID = listType == SmithyListType.EQUIP_WEAPON_SHIELD ? 1 : listType == SmithyListType.EQUIP_COSTUME ? 2 : listType == SmithyListType.EQUIP_ACCE ? 3 : 4;
        MainUI.GetInstance().tapSmithy.ClickBottomSmithyTypeTap(bottomBtnID);
        MainUI.GetInstance().tapSmithy.ClickRightTapChange(rightBtnID);
    }
    #endregion

    // ##################################################
    #region ##### 쿠폰 
    [SerializeField] PopUpCoupon popUpCoupon;
    public void Open_Coupon()
    {
        popUpCoupon.gameObject.SetActive(true);
        popUpCoupon.SetData();
    }
    #endregion

    // ##################################################
    #region ##### 첫 접속 보상 
    [SerializeField] PopUpFirstReward popUpFirstReward;
    public void Open_FirstReward()
    {
        string indate = BackendGpgsMng.backendUserInfo.m_indate;
        if (DateTime.Parse(indate) >= BackendGpgsMng.GetLoginSDate)
        {
            popUpFirstReward.gameObject.SetActive(true);
            popUpFirstReward.SetData();
        }
    }
    #endregion

    // ##################################################
    #region ##### Credit  
    [SerializeField] GameObject popUpCredit;
    public void Open_Credit()
    {
        popUpCredit.gameObject.SetActive(true);
    }
    #endregion

    // ##################################################
    #region ##### 던전(탑,광산,레이드) 반복 체크   
    [SerializeField] PopUpDungeonLoop popUpDungeonLoop;
    public void ClickOpen_DungeonLoop(string dg_name)
    {
        // 입장권 체크 
        int inTicketCnt = 0;
        if (string.Equals(dg_name, "top"))
            inTicketCnt = GameDatabase.GetInstance().tableDB.GetItemDungeonTicket(IG.ModeType.DUNGEON_TOP);
        else if (string.Equals(dg_name, "mine"))
            inTicketCnt = GameDatabase.GetInstance().tableDB.GetItemDungeonTicket(IG.ModeType.DUNGEON_MINE);
        else if (string.Equals(dg_name, "raid"))
            inTicketCnt = GameDatabase.GetInstance().tableDB.GetItemDungeonTicket(IG.ModeType.DUNGEON_RAID);

        if (inTicketCnt >= 2) // 반복이므로 티켓 2장 이상 
        {
            int inNbr = -1, selectNbr = -1;
            if (string.Equals(dg_name, "top"))
                inNbr = GameDatabase.GetInstance().tableDB.GetUserInfo().GetDgNbr(IG.ModeType.DUNGEON_TOP);
            else if (string.Equals(dg_name, "mine"))
                inNbr = GameDatabase.GetInstance().tableDB.GetUserInfo().GetDgNbr(IG.ModeType.DUNGEON_MINE);
            else if (string.Equals(dg_name, "raid"))
                inNbr = GameDatabase.GetInstance().tableDB.GetUserInfo().GetDgNbr(IG.ModeType.DUNGEON_RAID);

            if (string.Equals(dg_name, "top"))
                selectNbr = (int)MainUI.GetInstance().tapDungeon.dgTopNbr;
            else if (string.Equals(dg_name, "mine"))
                selectNbr = (int)MainUI.GetInstance().tapDungeon.dgMineNbr;
            else if (string.Equals(dg_name, "raid"))
                selectNbr = (int)MainUI.GetInstance().tapDungeon.dgRaidNbr;

            if (selectNbr != -1 && inNbr != -1 && selectNbr <= inNbr) // ((string.Equals(dg_name, "top") && selectNbr < inNbr) || (!string.Equals(dg_name, "top")  && selectNbr <= inNbr)))
            {
                popUpDungeonLoop.gameObject.SetActive(true);
                popUpDungeonLoop.SetData(dg_name);
            }
            else PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("이전 난이도를 클리어 하면 입장할 수 있습니다.");
        }
        else PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("던전 입장권이 부족합니다.\n던전 반복 진행은 입장권 2장 이상 소지하여아 가능합니다.");
    }
    #endregion

    // ##################################################
    #region ##### 뒤끝 공지 
    [SerializeField] PopUpBackendNotice popUpBackendNotice;
    public void Open_BackendNotice(JsonData rows, bool isClick)
    {
        popUpBackendNotice.gameObject.SetActive(true);
        popUpBackendNotice.SetData(rows, isClick);
    }
    #endregion

    // ##################################################
    #region ##### 방치 모드 
    [SerializeField] PopUpSleepMode popUpSleepMode;
    public void ClickOpen_SleepMode()
    {
        popUpSleepMode.gameObject.SetActive(true);
        popUpSleepMode.SetStart();
    }
    #endregion

    // ##################################################
    #region ##### TIP 전체 보기 
    [SerializeField] GameObject popUpTipAllList;
    public void ClickOpen_TipViewAll() => popUpTipAllList.SetActive(true);
    #endregion

    // ##################################################
    #region ##### 장비 판매/분해 (광고)
    [SerializeField] PopUpAutoSaleVideo popUpAutoSaleVideo;
    public void ClickOpen_AutoSaleVideo()
    {
        DateTime nDate = BackendGpgsMng.GetInstance().GetNowTime();
        DateTime cash_endDate = GameDatabase.GetInstance().convenienceFunctionDB.GetDate_ConvenFunAutoSale();
        if ((cash_endDate - nDate).TotalSeconds > 0)
        {
            PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("자동 판매/분해(다이아) 남은 기간이 있습니다.\n다이아로 구매한 시간이 모두 소진되어야 광고 구매 가능합니다.");
        }
        else
        {
            RmvAllClosePop();
            popUpAutoSaleVideo.gameObject.SetActive(true);
            popUpAutoSaleVideo.SetData();
        }
    }
    #endregion

    // ##################################################
    #region ##### 뽑기 확률 보기 
    [SerializeField] PopUpGachaPercentage popUpGachaPercentage;
    public void ClickOpen_GachaPercentage(string gch_name)
    {
        if (!popUpGachaPercentage.gameObject.activeSelf)
        {
            var cdb = GameDatabase.GetInstance().chartDB.Get_cdb_gacha_percentage(gch_name);
            if (!string.IsNullOrEmpty(cdb.gch_name))
            {
                popUpGachaPercentage.gameObject.SetActive(true);
                popUpGachaPercentage.SetData(cdb);
            }
        }
    }
    #endregion

    // ##################################################
    #region ##### 전설 장비(무기/방패/방어구) 전설 배경으로 진화 
    public PopUpEquipLegendUpdrage popUpEquipLegendUpdrage;
    public void Open_PopUpEquipLegendUpdrage(GameDatabase.TableDB.Equipment eqDB)
    {
        if(!popUpEquipLegendUpdrage.gameObject.activeSelf)
        {
            RmvAllClosePop();
            popUpEquipLegendUpdrage.gameObject.SetActive(true);
            popUpEquipLegendUpdrage.SetData(eqDB);
        }
    }

    public void ClickOpen_EquipLegendUpgradeSimpleReminder()
    {
        float v1 = GameDatabase.GetInstance ().chartDB.GetDicBalance("equip.legend.upgrade.rate.count").val_float;
        float v1Max = 5 * 5 * v1;
        float v2 = GameDatabase.GetInstance ().chartDB.GetDicBalance("equip.legend.upgrade.rate.same").val_float;
        float v2Max = 5 * 0.2f;
        float v3 = GameDatabase.GetInstance ().chartDB.GetDicBalance("equip.legend.upgrade.rate.enhant").val_float;
        float v3Max = 35 * 35 * 0.003f * 5;
        float v4 = ((5 * 5) * v1) + (5 * 0.2f) + ((35 * 35 * 0.003f) * 5);
        PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox
            (string.Format
            ("진화 성공률\n" +
            "선택 장비 개수 x 선택 장비 개수 x {0} (최대 {1}%)" +
            "\n+\n진화 장비와 같은 부위 개수 x {2} (최대 {3}%)" +
            "\n+\n재료 강화 레벨 x 재료 강화 레벨 x {4} x 선택 장비 개수\n(최대 {5}%)" +
            "\n=\n<color=yellow>최대 성공률 {6}%</color>",
            v1, v1Max,
            v2, v2Max,
            v3, v3Max,
            v4));
    }
    #endregion

    // ##################################################
    #region ##### 채팅 팝업 / 채팅 간단 메시지 
    public PopUpChat popUpChat;
    public void ClickOpen_PopUpChatOn() => popUpChat.On();
    #endregion

    // ##################################################
    #region ##### 장비/장신구 조각 상위 변환 
    public PopUpPieceConversion popUpPieceConversion;
    public void ClickOpen_PopUpPieceConversion_EquipPiece(int pice_rt)
    {
        if (!popUpPieceConversion.gameObject.activeSelf)
        {
            popUpPieceConversion.gameObject.SetActive(true);
            popUpPieceConversion.SetData(false, pice_rt);
        }
    }

    public void ClickOpen_PopUpPieceConversion_AccePiece(int pice_rt)
    {
        if (!popUpPieceConversion.gameObject.activeSelf)
        {
            popUpPieceConversion.gameObject.SetActive(true);
            popUpPieceConversion.SetData(true, pice_rt);
        }
    }
    #endregion

    // ##################################################
    #region ##### 장비/장신구 조각 상위 변환 
    public PopUpPotionConversion popUpPotionConversion;
    public void ClickOpen_PopUpPotionConversion(int ptn_rt)
    {
        if (!popUpPotionConversion.gameObject.activeSelf)
        {
            popUpPotionConversion.gameObject.SetActive(true);
            popUpPotionConversion.SetSart(ptn_rt);
        }
    }
    #endregion

    // ##################################################
    #region ##### 장비/장신구 조각 상위 변환 
    public PopUpSweep popUpSweep;
    public void Open_PopUpSweep(IG.ModeType _mdTy, int _inNbr, float _clrSec, int tikCnt)
    {
        if (!popUpSweep.gameObject.activeSelf)
        {
            popUpSweep.gameObject.SetActive(true);
            popUpSweep.SetData(_mdTy, _inNbr, _clrSec, tikCnt);
        }
    }
    #endregion

    // ##################################################
    #region ##### 펫 소환
    public PopUpPet popUpPet;
    public void Open_PopUpPet()
    {
        if (!popUpPet.gameObject.activeSelf)
        {
            popUpPet.gameObject.SetActive(true);
            popUpPet.SetData();
        }
    }
    #endregion

    // ##################################################
    #region ##### 펫 소환
    public PopUpPetGacha popUpPetGacha;
    public void Open_PopUpPetGacha(bool isTbc)
    {
        if (!popUpPetGacha.gameObject.activeSelf)
        {
            popUpPetGacha.gameObject.SetActive(true);
            popUpPetGacha.SetData(isTbc);
        }
    }
    #endregion
}
