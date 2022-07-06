using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using SG;
using UnityEngine.UI;
using Coffee.UIExtensions;

[RequireComponent(typeof(UnityEngine.UI.LoopScrollRect))]
[DisallowMultipleComponent]
public class PopUpDungeonReward : MonoBehaviour
{
    public InitOnStartDungeonReward initOnStart;
    public bool isSweep = false;
    public IG.ModeType rwdMty;
    [SerializeField] GameObject go_close, goLoopBtn;
    [SerializeField] Button btnDgTopNextOrRe;
    [SerializeField] Text tx_dg_top_next, txLoopBtn;

    public List<ResultReward> resultRewards = new List<ResultReward>();
    #region ##### struct, enum - Reward #####
    public enum RwdType
    {
        NONE,
        GOLD,           // 골드
        EQUIP,          // 장비
        EQUIP_AC,       // 장신구 
        SKILL,          // 스킬 
        EQUIP_PIECE,    // 장비 조각 
        EQUIP_AC_PIECE, // 장신구 조각 
        EQUIP_ENHANT_STON, // 장비 강화석 
        EQUIP_AC_ENHANT_STON, // 장신구 강화석 
        ENHANT_BLESS,   // 강화 축복 주문서 
        GOODS_ETHER // 에테르 
    }

    [System.Serializable]
    public struct ResultReward
    {
        public RwdType rwd_type;

        // 골드 
        public RwdGold rwdGold;
        [System.Serializable] public struct RwdGold { public float cnt; }

        // 장신구 조각 
        public RwdEquipAcPiece rwdEquipAcPiece;
        [System.Serializable] public struct RwdEquipAcPiece { public int pce_rt, pce_cnt; }

        // 장비 조각 
        public RwdEquipPiece rwdEquipPiece;
        [System.Serializable] public struct RwdEquipPiece { public int pce_rt, pce_cnt; }

        // 장비 (장신구도 포함)
        public RwdEquip rwdEquip;
        [System.Serializable] public struct RwdEquip { public int eq_ty, eq_rt, eq_id; public float pct; }

        // 스킬 
        public RwdSkill rwdSkill;
        [System.Serializable] public struct RwdSkill { public int sk_id, sk_cnt; }

        // 장비 강화석 
        public RwdEquipEnhantSton rwdEquipEnhantSton;
        [System.Serializable] public struct RwdEquipEnhantSton { public int eq_stn_rt, eq_stn_cnt; }

        // 장신구  강화석 
        public RwdEquipAcEnhantSton rwdEquipAcEnhantSton;
        [System.Serializable] public struct RwdEquipAcEnhantSton { public int eqac_stn_rt, eqac_stn_cnt; }

        // 장비,장신구 강화 축복 주문서 
        public RwdEnhantBless rwdEnhantBless;
        [System.Serializable] public struct RwdEnhantBless { public int bls_rt, bls_cnt; }

        // 에테르 
        public RwdGoodsEther rwdGoodsEther;
        [System.Serializable] public struct RwdGoodsEther { public int ether_cnt; }
    }
    #endregion

    #region ##### struct - UI #####
    [SerializeField] UI ui;
    [System.Serializable]
    struct UI
    {
        public Text tx_DungeonName;
        public Text tx_ClearRanKTitle;
        public Text tx_ClearTime;
        public Text tx_ClearHighTime;
        public Text tx_ClearHighRank;
        public Text tx_CloseTime;
    }
    #endregion

    #region ##### struct - RankTime #####
    List<RankTime> rankTime = new List<RankTime>();
    struct RankTime
    {
        public float time;
        public string rank;
    }
    #endregion 

    TapDungeon.DgLoop dgLoop = new TapDungeon.DgLoop();
    
    /// <summary>
    /// 던전 종료 
    /// </summary>
    public void Click_EndPop()
    {
        if (!isSweep)
        {
            GameMng.GetInstance().Routin_ChangeMode(MainUI.GetInstance().tapDungeon.saveMdTy, true, 5);
            GameMng.GetInstance().zbCam.PlayZombieCamera();
        }
        else
        {
            MainUI.GetInstance().tapDungeon.Ticket();
        }
        
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 도전의 탑 다음 단계 도전 
    /// </summary>
    void DgTopNext()
    {
        int new_inNbr = GameDatabase.GetInstance().tableDB.GetUserInfo().GetDgNbr(IG.ModeType.DUNGEON_TOP);
        MainUI.GetInstance().tapDungeon.dgTopNbr = (TapDungeon.DgNbrTop)new_inNbr;
        int dg_indx = GameDatabase.GetInstance().chartDB.list_cdb_dungeon_top.FindIndex(x => int.Equals(x.nbr, new_inNbr));
        if (dg_indx >= 0)
        {
            GameMng.GetInstance().ChangeMode_Top(new_inNbr);
            gameObject.SetActive(false);
        }
        else
        {
            PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("입장 가능한 던전 정보가 없습니다.");
            Click_EndPop();
        }
    }
    /// <summary>
    /// 도전의 탑 재도전 
    /// </summary>
    void DgTopRe()
    {
        int inNbr = (int)MainUI.GetInstance().tapDungeon.dgTopNbr;
        int dg_indx = GameDatabase.GetInstance().chartDB.list_cdb_dungeon_top.FindIndex(x => int.Equals(x.nbr, inNbr));
        if (dg_indx >= 0)
        {
            GameMng.GetInstance().ChangeMode_Top(inNbr);
            gameObject.SetActive(false);
        }
        else
        {
            PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("입장 가능한 던전 정보가 없습니다.");
            Click_EndPop();
        }
    }

    public void Click_LoopInDungeon() => LoopInDungeon();
    void LoopInDungeon()
    {
        StopCoroutine("IELoopContinue");
        if (dgLoop.loopDgType == IG.ModeType.DUNGEON_TOP)
        {
            int dg_indx = GameDatabase.GetInstance().chartDB.list_cdb_dungeon_top.FindIndex(x => int.Equals(x.nbr, dgLoop.loopDgNbr));
            if (dg_indx >= 0)
            {
                GameMng.GetInstance().ChangeMode_Top(dgLoop.loopDgNbr);
                GameDatabase.GetInstance().dailyMissionDB.ASetInCount(GameDatabase.DailyMissionDB.Nbr.nbr9, 1); // 일일미션, nbr9 던전 : 도전의 탑 진행하기! 
            }
            else
            {
                PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("입장 가능한 던전 정보가 잘못되었습니다.");
            }
        }
        else if (dgLoop.loopDgType == IG.ModeType.DUNGEON_MINE)
        {
            int dg_indx = GameDatabase.GetInstance().chartDB.list_cdb_dungeon_mine.FindIndex(x => int.Equals(x.nbr, dgLoop.loopDgNbr));
            if (dg_indx >= 0)
            {
                GameMng.GetInstance().ChangeMode_Mine(dgLoop.loopDgNbr);
                GameDatabase.GetInstance().dailyMissionDB.ASetInCount(GameDatabase.DailyMissionDB.Nbr.nbr10, 1); // 일일미션, nbr10 던전 : 광산 진행하기! 
            }
            else
            {
                PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("입장 가능한 던전 정보가 잘못되었습니다.");
            }
        }
        else if (dgLoop.loopDgType == IG.ModeType.DUNGEON_RAID)
        {
            int dg_indx = GameDatabase.GetInstance().chartDB.list_cdb_dungeon_raid.FindIndex(x => int.Equals(x.nbr, dgLoop.loopDgNbr));
            if (dg_indx >= 0)
            {
                GameMng.GetInstance().ChangeMode_Raid(dgLoop.loopDgNbr);
                GameDatabase.GetInstance().dailyMissionDB.ASetInCount(GameDatabase.DailyMissionDB.Nbr.nbr11, 1); // 일일미션, nbr11 던전 : 레이드 진행하기! 
            }
            else
            {
                PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("입장 가능한 던전 정보가 잘못되었습니다.");
            }
        }

        gameObject.SetActive(false);
    }

    // ###########################################
    /// <summary> 결과 팝업 </summary>
    public async void SetData(IG.ModeType _mdTy, int _inNbr, bool _isClear, float _clrSec, bool sweep = false, int sweep_cnt = 1)
    {
        Loading.Full(false, true);
        LogPrint.EditorPrint("결과 SetData _mdTy : " + _mdTy + ", _nbr : " + _inNbr + ", _isClear : " + _isClear + ", _clrSec : " + _clrSec + ", sweep : " + sweep);
        isSweep = sweep;
        rwdMty = _mdTy;
      
        btnDgTopNextOrRe.gameObject.SetActive(false);
        go_close.SetActive(false);
        goLoopBtn.SetActive(false);
        resultRewards.Clear();
        initOnStart.InitStart(sweep);

        ui.tx_DungeonName.text = string.Format("{0} 던전 {1}보상",
            _mdTy == IG.ModeType.DUNGEON_TOP ? string.Format(LanguageGameData.GetInstance().GetString("text.dungeon.top.stage"), _inNbr + 1) :
            _mdTy == IG.ModeType.DUNGEON_MINE ? LanguageGameData.GetInstance().GetString(string.Format("text.dungeon.mine.stage_{0}", _inNbr)) :
            _mdTy == IG.ModeType.DUNGEON_RAID ? LanguageGameData.GetInstance().GetString(string.Format("text.dungeon.raid.stage_{0}", _inNbr)) : string.Empty,
            sweep == true ? "소탕하기 " : "");

        string rank_dgName = _mdTy == IG.ModeType.DUNGEON_TOP ? "TOP" : _mdTy == IG.ModeType.DUNGEON_MINE ? "MINE" : "RAID";

        // ##### 클리어 랭크 #####
        string clrStrRank = _isClear == true && sweep == false ? GameDatabase.GetInstance().dungeonDB.GetClearRank(_clrSec, rank_dgName) : sweep == true ? "S" : "";

        await GameDatabase.GetInstance().tableDB.ConsumDungeonTicket(_mdTy, sweep_cnt); // 입장권 차감 

        // 도전의 탑 
        if (_mdTy == IG.ModeType.DUNGEON_TOP)
        {
            int rwd_indx = GameDatabase.GetInstance().chartDB.list_cdb_dungeon_top.FindIndex(x => x.nbr == _inNbr);
            LogPrint.EditorPrint("도전의탑 nbr : " + _inNbr + " 클리어 : " + _isClear + ", rwd_indx : " + rwd_indx);
            if (rwd_indx >= 0)
            {
                if(_isClear && !sweep) 
                    PlayerPrefs.SetFloat(string.Format(PrefsKeys.prky_LastClearSecDungeonTopNbr, _inNbr), _clrSec);

                RewardSend(GameDatabase.GetInstance().chartDB.list_cdb_dungeon_top[rwd_indx].reward, _mdTy, _inNbr, _isClear, _clrSec, clrStrRank, sweep, sweep_cnt);

                if (_isClear)
                {
                    int lastClrNbr = GameDatabase.GetInstance().tableDB.GetUserInfo().GetDgNbr(IG.ModeType.DUNGEON_TOP);
                    LogPrint.EditorPrint("<color=red>--------lastClrNbr----2------- : " + lastClrNbr + ", _nbr : " + _inNbr + "</color>");
                    GameDatabase.GetInstance().achievementsDB.ASetInCount(GameDatabase.AchievementsDB.Nbr.nbr20, _inNbr + 1, false); // 업적, nbr20 도전의 탑 5~35층 클리어하기! 
                }
            }
            else
            {
                PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("던전 정보가 잘못되었습니다.");
                GameMng.GetInstance().Routin_ChangeMode(MainUI.GetInstance().tapDungeon.saveMdTy, false, 0);
            }
        }
        // 광산 
        else if (_mdTy == IG.ModeType.DUNGEON_MINE)
        {
            int rwd_indx = GameDatabase.GetInstance().chartDB.list_cdb_dungeon_mine.FindIndex(x => x.nbr == _inNbr);
            LogPrint.EditorPrint("광산 nbr : " + _inNbr + " 클리어 : " + _isClear + ", rwd_indx : " + rwd_indx);
            if (rwd_indx >= 0)
            {
                if (_isClear && !sweep)
                    PlayerPrefs.SetFloat(string.Format(PrefsKeys.prky_LastClearSecDungeonMineNbr, _inNbr), _clrSec);

                RewardSend(GameDatabase.GetInstance().chartDB.list_cdb_dungeon_mine[rwd_indx].reward, _mdTy, _inNbr, _isClear, _clrSec, clrStrRank, sweep, sweep_cnt);
            }
            else
            {
                PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("던전 정보가 잘못되었습니다.");
                GameMng.GetInstance().Routin_ChangeMode(MainUI.GetInstance().tapDungeon.saveMdTy, false, 0);
            }
        }
        // 레이드 
        else if (_mdTy == IG.ModeType.DUNGEON_RAID)
        {
            int rwd_indx = GameDatabase.GetInstance().chartDB.list_cdb_dungeon_raid.FindIndex(x => x.nbr == _inNbr);
            LogPrint.EditorPrint("레이드 nbr : " + _inNbr + " 클리어 : " + _isClear + ", rwd_indx : " + rwd_indx);
            if (rwd_indx >= 0)
            {
                if (_isClear && !sweep)
                    PlayerPrefs.SetFloat(string.Format(PrefsKeys.prky_LastClearSecDungeonRaidNbr, _inNbr), _clrSec);

                RewardSend(GameDatabase.GetInstance().chartDB.list_cdb_dungeon_raid[rwd_indx].reward, _mdTy, _inNbr, _isClear, _clrSec, clrStrRank, sweep, sweep_cnt);
            }
            else
            {
                PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("던전 정보가 잘못되었습니다.");
                GameMng.GetInstance().Routin_ChangeMode(MainUI.GetInstance().tapDungeon.saveMdTy, false, 0);
            }
        }
    }

    // ###########################################
    /// <summary> 클리어 던전 넘버 정보 전송 </summary>
    async Task SendDungeonInfo(IG.ModeType md_ty, int _nbr)
    {
        var uInfo = GameDatabase.GetInstance().tableDB.GetUserInfo();
        if (md_ty == IG.ModeType.DUNGEON_TOP)
        {
            int dgStgCnt = GameDatabase.GetInstance().chartDB.list_cdb_dungeon_top.Count;
            int lastNbr = GameDatabase.GetInstance().chartDB.list_cdb_dungeon_top[dgStgCnt - 1].nbr;
            LogPrint.Print("<color=magenta>lastNbr : " + lastNbr + ", dgStgCnt : " + dgStgCnt + ", _nbr : " + _nbr + ", lastNbr : " + lastNbr + " </color>");
            //if (_nbr <= lastNbr)
            //{
                uInfo.m_dg_top_nbr = _nbr;
                await GameDatabase.GetInstance().tableDB.SetUpdate_UserInfo(uInfo);
            //}
        }
        else if (md_ty == IG.ModeType.DUNGEON_MINE)
        {
            int dgStgCnt = GameDatabase.GetInstance().chartDB.list_cdb_dungeon_mine.Count;
            int lastNbr = GameDatabase.GetInstance().chartDB.list_cdb_dungeon_mine[dgStgCnt - 1].nbr;
            LogPrint.Print("<color=magenta>lastNbr : " + lastNbr + ", dgStgCnt : " + dgStgCnt + ", _nbr : " + _nbr + ", lastNbr : " + lastNbr + " </color>");
            //if (_nbr <= lastNbr)
            //{
                uInfo.m_dg_mine_nbr = _nbr;
                await GameDatabase.GetInstance().tableDB.SetUpdate_UserInfo(uInfo);
            //}
        }
        else if (md_ty == IG.ModeType.DUNGEON_RAID)
        {
            int dgStgCnt = GameDatabase.GetInstance().chartDB.list_cdb_dungeon_raid.Count;
            int lastNbr = GameDatabase.GetInstance().chartDB.list_cdb_dungeon_raid[dgStgCnt - 1].nbr;
            LogPrint.Print("<color=magenta>lastNbr : " + lastNbr + ", dgStgCnt : " + dgStgCnt + ", _nbr : " + _nbr + ", lastNbr : " + lastNbr + " </color>");
            //if (_nbr <= lastNbr)
            //{
                uInfo.m_dg_raid_nbr = _nbr;
                await GameDatabase.GetInstance().tableDB.SetUpdate_UserInfo(uInfo);
            //}
        }
    }

    // ###########################################
    /// <summary> 클리어 UI 정보 </summary>
    async Task SendDungeonRankAndUI(IG.ModeType _mdTy, int _nbr, bool _isClear, float _clrSec, string _clrStrRank, bool isSweep = false)
    {
        LogPrint.Print("<color=red>SendDungeonRankAndUI</color> _nbr : " + _nbr + ", _isClear :" + _isClear + ", _clrSec : " + _clrSec + ", _clrStrRank : " + _clrStrRank);
        #region ##### 랭크 보여주기 #####
        string rank_dgName = _mdTy == IG.ModeType.DUNGEON_TOP ? "TOP" : _mdTy == IG.ModeType.DUNGEON_MINE ? "MINE" : "RAID";
        float hg_sec = GameDatabase.GetInstance().dungeonDB.GetClearTime(_mdTy, _nbr); // 내 최고 기록 
        if (_isClear == false)
        {
            ui.tx_ClearRanKTitle.text = LanguageGameData.GetInstance().GetString("text.failure.dungeon"); // "도전 실패";
            ui.tx_ClearTime.text = "[--:--.--]";
            ui.tx_ClearHighRank.text = string.Format("내 최고 랭크 [<size=40>{0}</size>]", GameDatabase.GetInstance().dungeonDB.GetClearRank(hg_sec, rank_dgName));
            ui.tx_ClearHighTime.text = "[--:--.--]";
        }
        else
        {
            string strClearRank = LanguageGameData.GetInstance().GetString("text.dg.clear.rank"); // 클리어 랭크 
            ui.tx_ClearRanKTitle.text = string.Format("{0} [<size=60>{1}</size>]", strClearRank, _clrStrRank);

            float sec = _clrSec;  // 클리어 시간 
            bool isNewRecord = sec < hg_sec || hg_sec == 0;
            if(isNewRecord || hg_sec == 0)
                hg_sec = sec;

            // 클리어 UI 
            if (!isSweep)
            {
                int minutes = (int)(System.Math.Floor((double)(sec / 60)));
                int second = (int)(System.Math.Floor((double)(sec % 3600 % 60)));
                string[] msec = sec.ToString("N2").Split('.');
                ui.tx_ClearTime.text = string.Format("기록 [{0:00}:{1:00}.{2}]", minutes, second, msec[1]);
            }
            else
            {
                ui.tx_ClearTime.text = "";
            }

            // 내 최고 클리어 UI (기록 경신)
            if (hg_sec > 0)
            {
                int minutes2 = (int)(System.Math.Floor((double)(hg_sec / 60)));
                int second2 = (int)(System.Math.Floor((double)(hg_sec % 3600 % 60)));
                string[] msec2 = hg_sec.ToString("N2").Split('.');
                ui.tx_ClearHighRank.text = string.Format("내 최고 랭크 [<size=40>{0}</size>]", GameDatabase.GetInstance().dungeonDB.GetClearRank(hg_sec, rank_dgName));
                ui.tx_ClearHighTime.text = string.Format("내 최고 기록 [{0:00}:{1:00}.{2}]", minutes2, second2, msec2[1]);
            }
            else
            {
                ui.tx_ClearHighRank.text = "내 최고 랭크 [-]";
                ui.tx_ClearHighTime.text = "내 최고 기록 [--:--.--]";
            }

            LogPrint.PrintError("SendDungeonRankAndUI sec : " + sec + ", hg_sec : " + hg_sec);

            if (_isClear && isNewRecord == true && !isSweep)
            {
                // 서버로 새로 경신된 클리어 기록 전송 
                await GameDatabase.GetInstance().dungeonDB.ASendScoreData(_mdTy, string.Format("ctmNbr{0}", _nbr), sec);
            }
        }
        #endregion
    }

    // ###########################################
    /// <summary> 던전 보상 데이터 전송 </summary>
    async void RewardSend(DungeonReward dg_rwds, IG.ModeType _mdTy, int _inNbr, bool _isClear, float _clrSec, string _clrStrRank, bool sweep = false, int sweep_cnt = 1)
    {
        LogPrint.EditorPrint(" 111 결과 RewardSend _mdTy : " + _mdTy + ", _nbr : " + _inNbr + ", _isClear : " + _isClear + ", _clrSec : " + _clrSec + ", sweep : " + sweep);

        var dg_mdty = GameMng.GetInstance().mode_type;
        List<Task> awaitTasks = new List<Task>();
        bool isRwdSkill = false;

        awaitTasks.Add(SendDungeonRankAndUI(_mdTy, _inNbr, _isClear, _clrSec, _clrStrRank, sweep)); // ##### 클리어 랭크 타임 전송 및 클리어 랭크 UI 

        // ##### 골드 (기본 보상) 
        for (int i = 0; i < sweep_cnt; i++)
        {
            GameDatabase.GetInstance().tableDB.SetUpdateGoods("gold", GameDatabase.GetInstance().questDB.GetQuestMaxSecondRewardGold(dg_rwds.qst_gold), "+", true);
        }
        
        resultRewards.Add(new ResultReward() 
        { rwd_type = RwdType.GOLD, rwdGold = new ResultReward.RwdGold() { cnt = dg_rwds.qst_gold * sweep_cnt } });

        if (_isClear) // 성공시에만 
        {
            int lastClearNbr = GameDatabase.GetInstance().tableDB.GetUserInfo().GetDgNbr(_mdTy);
            bool isTopNewClear = _mdTy == IG.ModeType.DUNGEON_TOP && lastClearNbr == _inNbr;
            bool isDgTop = _mdTy == IG.ModeType.DUNGEON_TOP;

            LogPrint.Print("<color=red>마지막 클리어 넘버 lastClearNbr : " + lastClearNbr + ", 입장 클리어 넘버 _inNbr : " + _inNbr + ", isTopNewClear : " + isTopNewClear + ", isDgTop : " + isDgTop + " </color>");
            if (lastClearNbr == _inNbr)
                awaitTasks.Add(SendDungeonInfo(_mdTy, _inNbr + 1)); // ##### 던전 클리어 넘버 전송 (다음 넘버)

            #region ##### 클리어 성공 - 보상 전송 #####
            // ##### 장비  <클리어 최초 1회 보상>
            if (dg_rwds.rw_eq_rt > 0 && (isTopNewClear || !isDgTop))
            {
                var rwdEqDb = GameDatabase.GetInstance().tableDB.GetNewEquipmentData(dg_rwds.rw_eq_ty, dg_rwds.rw_eq_rt, dg_rwds.rw_eq_id);
                if (rwdEqDb.eq_rt >= 7)
                {
                    // 서버 전송 
                    long unused_uid = GameDatabase.GetInstance().tableDB.GetUnusedUID();
                    rwdEqDb.indate = GameDatabase.GetInstance().tableDB.GetUIDSearchToInDate(unused_uid);
                    string send_type = string.IsNullOrEmpty(rwdEqDb.indate) ? "insert" : "change";

                    awaitTasks.Add(GameDatabase.GetInstance().tableDB.SendDataEquipment(rwdEqDb, send_type));
                    if (!string.IsNullOrEmpty(rwdEqDb.indate))
                        GameDatabase.GetInstance().tableDB.SetUnusedInDateToEmpty(unused_uid);
                }
                else GameDatabase.GetInstance().tableDB.UpdateClientDB_Equip(rwdEqDb); // client 

                NotificationIcon.GetInstance().CheckNoticeAutoWear(rwdEqDb, false);
                GameDatabase.GetInstance().equipmentEncyclopediaDB.DropAcquisitionAdded(rwdEqDb.eq_ty, rwdEqDb.eq_rt, rwdEqDb.eq_id);

                resultRewards.Add(new ResultReward()
                { rwd_type = RwdType.EQUIP, rwdEquip = new ResultReward.RwdEquip() { eq_ty = dg_rwds.rw_eq_ty, eq_rt = dg_rwds.rw_eq_rt, eq_id = dg_rwds.rw_eq_id } });
            }

            // ##### 장신구  <클리어 최초 1회 보상>
            if (dg_rwds.rw_eqac_rt > 0 && (isTopNewClear || !isDgTop))
            {
                var rwdEqAcDB = GameDatabase.GetInstance().tableDB.GetNewEquipmentData(dg_rwds.rw_eqac_ty, dg_rwds.rw_eqac_rt, dg_rwds.rw_eqac_id);
                if (rwdEqAcDB.eq_rt >= 5)
                {
                    // 서버 전송 
                    long unused_uid = GameDatabase.GetInstance().tableDB.GetUnusedUID();
                    rwdEqAcDB.indate = GameDatabase.GetInstance().tableDB.GetUIDSearchToInDate(unused_uid);
                    string send_type = string.IsNullOrEmpty(rwdEqAcDB.indate) ? "insert" : "change";

                    awaitTasks.Add(GameDatabase.GetInstance().tableDB.SendDataEquipment(rwdEqAcDB, send_type));
                    if (!string.IsNullOrEmpty(rwdEqAcDB.indate))
                        GameDatabase.GetInstance().tableDB.SetUnusedInDateToEmpty(unused_uid);
                }
                else GameDatabase.GetInstance().tableDB.UpdateClientDB_Equip(rwdEqAcDB); // client 

                NotificationIcon.GetInstance().CheckNoticeAutoWear(rwdEqAcDB, false);
                GameDatabase.GetInstance().equipmentEncyclopediaDB.DropAcquisitionAdded(rwdEqAcDB.eq_ty, rwdEqAcDB.eq_rt, rwdEqAcDB.eq_id);
                resultRewards.Add(new ResultReward()
                { rwd_type = RwdType.EQUIP_AC, rwdEquip = new ResultReward.RwdEquip() { eq_ty = dg_rwds.rw_eqac_ty, eq_rt = dg_rwds.rw_eqac_rt, eq_id = dg_rwds.rw_eqac_id } });
            }

            bool is_dgRaid_eqac_true = false;
            // ##### 장신구 (랜덤 보상)
            if (dg_rwds.rw_pct_eq_ac.Count > 0)
            {
                for (int fswp_cnt = 0; fswp_cnt < sweep_cnt; fswp_cnt++)
                {
                    GameDatabase.TableDB.Equipment rnd_rwdEqAcDB = new GameDatabase.TableDB.Equipment();
                    float pct = 0f, r_pct = GameDatabase.GetInstance().GetRandomPercent();
                    foreach (var item in dg_rwds.rw_pct_eq_ac)
                    {
                        pct += item.pct;
                        if (r_pct <= pct)
                        {
                            if (item.rwds[0].eq_rt > 0)
                            {
                                rnd_rwdEqAcDB = GameDatabase.GetInstance().tableDB.GetNewEquipmentData(Random.Range(8, 11), item.rwds[0].eq_rt, item.rwds[0].eq_id);
                                is_dgRaid_eqac_true = true;
                            }
                            break;
                        }
                    }

                    if (is_dgRaid_eqac_true)
                    {
                        if (rnd_rwdEqAcDB.eq_rt >= 5)
                        {
                            // 서버 전송 
                            long unused_uid = GameDatabase.GetInstance().tableDB.GetUnusedUID();
                            rnd_rwdEqAcDB.indate = GameDatabase.GetInstance().tableDB.GetUIDSearchToInDate(unused_uid);
                            string send_type = string.IsNullOrEmpty(rnd_rwdEqAcDB.indate) ? "insert" : "change";

                            await GameDatabase.GetInstance().tableDB.SendDataEquipment(rnd_rwdEqAcDB, send_type);
                            //awaitTasks.Add(GameDatabase.GetInstance().tableDB.SendDataEquipment(rnd_rwdEqAcDB, send_type));

                            if (!string.IsNullOrEmpty(rnd_rwdEqAcDB.indate))
                                GameDatabase.GetInstance().tableDB.SetUnusedInDateToEmpty(unused_uid);
                        }
                        else GameDatabase.GetInstance().tableDB.UpdateClientDB_Equip(rnd_rwdEqAcDB); // client 

                        NotificationIcon.GetInstance().CheckNoticeAutoWear(rnd_rwdEqAcDB, false);
                        GameDatabase.GetInstance().equipmentEncyclopediaDB.DropAcquisitionAdded(rnd_rwdEqAcDB.eq_ty, rnd_rwdEqAcDB.eq_rt, rnd_rwdEqAcDB.eq_id);
                        resultRewards.Add(new ResultReward()
                        { rwd_type = RwdType.EQUIP_AC, rwdEquip = new ResultReward.RwdEquip() { eq_ty = rnd_rwdEqAcDB.eq_ty, eq_rt = rnd_rwdEqAcDB.eq_rt, eq_id = rnd_rwdEqAcDB.eq_id } });
                    }
                }
            }

            // ##### 장비 (랜덤 보상)
            if (dg_rwds.rw_pct_eq.Count > 0)
            {
                for (int fswp_cnt = 0; fswp_cnt < sweep_cnt; fswp_cnt++)
                {
                    List<GameDatabase.TableDB.Equipment> rnd_rwdEqDb = new List<GameDatabase.TableDB.Equipment>();
                    int forCnt = is_dgRaid_eqac_true == true ? 2 : 3;
                    for (int i = 0; i <= forCnt; i++)
                    {
                        float pct = 0f, r_pct = GameDatabase.GetInstance().GetRandomPercent();
                        foreach (var item in dg_rwds.rw_pct_eq)
                        {
                            pct += item.pct;
                            if (r_pct <= pct)
                            {
                                if (item.rwds[0].eq_rt > 0)
                                {
                                    rnd_rwdEqDb.Add(GameDatabase.GetInstance().tableDB.GetNewEquipmentData(Random.Range(0, 8), item.rwds[0].eq_rt, item.rwds[0].eq_id));
                                }
                                break;
                            }
                        }
                    }

                    List<Task> arr_bkSend_task = new List<Task>();
                    foreach (var item in rnd_rwdEqDb)
                    {
                        var tmp_eqDB = item;
                        if (tmp_eqDB.eq_rt >= 7)
                        {
                            // 서버 전송 
                            long unused_uid = GameDatabase.GetInstance().tableDB.GetUnusedUID();
                            tmp_eqDB.indate = GameDatabase.GetInstance().tableDB.GetUIDSearchToInDate(unused_uid);
                            string send_type = string.IsNullOrEmpty(tmp_eqDB.indate) ? "insert" : "change";

                            await GameDatabase.GetInstance().tableDB.SendDataEquipment(tmp_eqDB, send_type);
                            //awaitTasks.Add(GameDatabase.GetInstance().tableDB.SendDataEquipment(tmp_eqDB, send_type));

                            if (!string.IsNullOrEmpty(tmp_eqDB.indate))
                                GameDatabase.GetInstance().tableDB.SetUnusedInDateToEmpty(unused_uid);
                        }
                        else GameDatabase.GetInstance().tableDB.UpdateClientDB_Equip(tmp_eqDB); // client 

                        NotificationIcon.GetInstance().CheckNoticeAutoWear(tmp_eqDB, false);
                        GameDatabase.GetInstance().equipmentEncyclopediaDB.DropAcquisitionAdded(tmp_eqDB.eq_ty, tmp_eqDB.eq_rt, tmp_eqDB.eq_id);
                    }

                    for (int i = 0; i < rnd_rwdEqDb.Count; i++)
                    {
                        resultRewards.Add(new ResultReward()
                        { rwd_type = RwdType.EQUIP, rwdEquip = new ResultReward.RwdEquip() { eq_ty = rnd_rwdEqDb[i].eq_ty, eq_rt = rnd_rwdEqDb[i].eq_rt, eq_id = rnd_rwdEqDb[i].eq_id } });
                    }
                }
            }

            // ##### 스킬 (확정 보상)
            if (dg_rwds.rw_sk_id > 0)
            {
                var temp = GameDatabase.GetInstance().tableDB.GetSkill(dg_rwds.rw_sk_id);
                if (temp.idx > 0)
                {
                    // 서버 전송 
                    isRwdSkill = true;
                    temp.count += dg_rwds.rw_sk_cnt * sweep_cnt;
                    awaitTasks.Add(GameDatabase.GetInstance().tableDB.SendDataSkill(temp));
                    resultRewards.Add(new ResultReward()
                    { rwd_type = RwdType.SKILL, rwdSkill = new ResultReward.RwdSkill() { sk_id = dg_rwds.rw_sk_id, sk_cnt = dg_rwds.rw_sk_cnt * sweep_cnt } });
                }
            }

            // ##### 장비 조각 (확정 보상)
            if (dg_rwds.rw_eq_pce_rt > 0)
            {
                // 서버 전송
                var temp = GameDatabase.GetInstance().tableDB.GetItem(28, dg_rwds.rw_eq_pce_rt);
                temp.count += dg_rwds.rw_eq_pce_cnt * sweep_cnt;
                awaitTasks.Add(GameDatabase.GetInstance().tableDB.SendDataItem(temp));
                resultRewards.Add(new ResultReward()
                { rwd_type = RwdType.EQUIP_PIECE, rwdEquipPiece = new ResultReward.RwdEquipPiece() { pce_rt = dg_rwds.rw_eq_pce_rt, pce_cnt = dg_rwds.rw_eq_pce_cnt * sweep_cnt } });
            }

            // ##### 장신구 조각 (확정 보상)
            if (dg_rwds.rw_eqac_pce_rt > 0)
            {
                // 서버 전송
                var temp = GameDatabase.GetInstance().tableDB.GetItem(29, dg_rwds.rw_eqac_pce_rt);
                temp.count += dg_rwds.rw_eqac_pce_cnt * sweep_cnt;
                awaitTasks.Add(GameDatabase.GetInstance().tableDB.SendDataItem(temp));
                resultRewards.Add(new ResultReward()
                { rwd_type = RwdType.EQUIP_AC_PIECE, rwdEquipAcPiece = new ResultReward.RwdEquipAcPiece() { pce_rt = dg_rwds.rw_eqac_pce_rt, pce_cnt = dg_rwds.rw_eqac_pce_cnt * sweep_cnt } });
            }

            // ##### 장비 조각 (랭크 확정 보상)
            if (dg_rwds.rw_eq_pces.Count > 0)
            {
                foreach (var item in dg_rwds.rw_eq_pces)
                {
                    if (Equals(item.rnk, _clrStrRank))
                    {
                        for (int i = 0; i < item.rwds.Count; i++)
                        {
                            if (item.rwds[i].rt > 0)
                            {
                                var temp = GameDatabase.GetInstance().tableDB.GetItem(28, item.rwds[i].rt);
                                temp.count += item.rwds[i].cnt * sweep_cnt;
                                awaitTasks.Add(GameDatabase.GetInstance().tableDB.SendDataItem(temp));
                                resultRewards.Add(new ResultReward()
                                { rwd_type = RwdType.EQUIP_PIECE, rwdEquipPiece = new ResultReward.RwdEquipPiece() { pce_rt = item.rwds[i].rt, pce_cnt = item.rwds[i].cnt * sweep_cnt } });
                            }
                        }
                        break;
                    }
                }
            }

            // ##### 장신구 조각 (랭크 확정 보상)
            if (dg_rwds.rw_eqac_pces.Count > 0)
            {
                foreach (var item in dg_rwds.rw_eqac_pces)
                {
                    if (Equals(item.rnk, _clrStrRank))
                    {
                        for (int i = 0; i < item.rwds.Count; i++)
                        {
                            if (item.rwds[i].rt > 0)
                            {
                                var temp = GameDatabase.GetInstance().tableDB.GetItem(29, item.rwds[i].rt);
                                temp.count += item.rwds[i].cnt * sweep_cnt;
                                awaitTasks.Add(GameDatabase.GetInstance().tableDB.SendDataItem(temp));
                                resultRewards.Add(new ResultReward()
                                { rwd_type = RwdType.EQUIP_AC_PIECE, rwdEquipAcPiece = new ResultReward.RwdEquipAcPiece() { pce_rt = item.rwds[i].rt, pce_cnt = item.rwds[i].cnt * sweep_cnt } });
                            }
                        }
                        break;
                    }
                }
            }

            // ##### 장비 강화석 (랭크 확정 보상)
            if (dg_rwds.rw_eq_ehnt_ston.Count > 0)
            {
                foreach (var item in dg_rwds.rw_eq_ehnt_ston)
                {
                    if (Equals(item.rnk, _clrStrRank))
                    {
                        for (int i = 0; i < item.rwds.Count; i++)
                        {
                            if (item.rwds[i].rt > 0)
                            {
                                var temp = GameDatabase.GetInstance().tableDB.GetItem(21, item.rwds[i].rt);
                                temp.count += item.rwds[i].cnt * sweep_cnt;
                                awaitTasks.Add(GameDatabase.GetInstance().tableDB.SendDataItem(temp));
                                resultRewards.Add(new ResultReward()
                                { rwd_type = RwdType.EQUIP_ENHANT_STON, rwdEquipEnhantSton = new ResultReward.RwdEquipEnhantSton() { eq_stn_rt = item.rwds[i].rt, eq_stn_cnt = item.rwds[i].cnt * sweep_cnt } });
                            }
                        }
                        break;
                    }
                }
            }

            // ##### 장신구 강화석 (랭크 확정 보상)
            if (dg_rwds.rw_eqac_ehnt_ston.Count > 0)
            {
                foreach (var item in dg_rwds.rw_eqac_ehnt_ston)
                {
                    if (Equals(item.rnk, _clrStrRank))
                    {
                        for (int i = 0; i < item.rwds.Count; i++)
                        {
                            if (item.rwds[i].rt > 0)
                            {
                                var temp = GameDatabase.GetInstance().tableDB.GetItem(27, item.rwds[i].rt);
                                temp.count += item.rwds[i].cnt * sweep_cnt;
                                awaitTasks.Add(GameDatabase.GetInstance().tableDB.SendDataItem(temp));
                                resultRewards.Add(new ResultReward()
                                { rwd_type = RwdType.EQUIP_AC_ENHANT_STON, rwdEquipAcEnhantSton = new ResultReward.RwdEquipAcEnhantSton() { eqac_stn_rt = item.rwds[i].rt, eqac_stn_cnt = item.rwds[i].cnt * sweep_cnt } });
                            }
                        }
                        break;
                    }
                }
            }

            // ##### 강화 축복 주문서
            if (dg_rwds.rw_ehnt_bless_rt > 0)
            {
                var temp = GameDatabase.GetInstance().tableDB.GetItem(22, dg_rwds.rw_ehnt_bless_rt);
                temp.count += dg_rwds.rw_ehnt_bless_cnt * sweep_cnt;
                awaitTasks.Add(GameDatabase.GetInstance().tableDB.SendDataItem(temp));
                resultRewards.Add(new ResultReward()
                { rwd_type = RwdType.ENHANT_BLESS, rwdEnhantBless = new ResultReward.RwdEnhantBless() { bls_rt = dg_rwds.rw_ehnt_bless_rt, bls_cnt = dg_rwds.rw_ehnt_bless_cnt * sweep_cnt } });
            }

            // ##### 에테르 <클리어 최초 1회 보상>
            if (dg_rwds.rw_equip_ac_crystal.rw_goods_ether_cnt > 0 && (isTopNewClear || !isDgTop))
            {
                var temp_gd = GameDatabase.GetInstance().tableDB.GetTableDB_Goods();
                temp_gd.m_ether += dg_rwds.rw_equip_ac_crystal.rw_goods_ether_cnt;
                awaitTasks.Add(GameDatabase.GetInstance().tableDB.SetUpdateGoods(temp_gd));
                resultRewards.Add(new ResultReward()
                { rwd_type = RwdType.GOODS_ETHER, rwdGoodsEther = new ResultReward.RwdGoodsEther() { ether_cnt = dg_rwds.rw_equip_ac_crystal.rw_goods_ether_cnt } });
            }

            // ##### 루비 

            #endregion
        }

        // 서버와 통신이 끝날때까지 대기 
        while (awaitTasks.Count > 0)
        {
            bool isAllComplete = true;
            for (int i = 0; i < awaitTasks.Count; i++)
            {
                isAllComplete = awaitTasks[i].IsCompleted;
                if (!isAllComplete)
                    break;
            }

            if (isAllComplete)
                break;
            else await Task.Delay(250);
        }

        // 정보 새로 고침 
        initOnStart.InitStart(sweep);
        
        NotificationIcon.GetInstance().CheckNoticeContentsTicket();

        if (isRwdSkill) // 스킬 보상 
        {
            NotificationIcon.GetInstance().CheckNoticeSkillLevelUp(true);
        }

        // 반복 진행 체크 
        dgLoop = MainUI.GetInstance().tapDungeon.dgLoop;
        dgLoop.loopCnt--;
        dgLoop.isLoop = dgLoop.loopCnt > 0;
        MainUI.GetInstance().tapDungeon.dgLoop = dgLoop;
        goLoopBtn.SetActive(dgLoop.isLoop);
        if (dgLoop.isLoop == true)
        {
            StopCoroutine("IELoopContinue");
            StartCoroutine("IELoopContinue");
        }
        else
        {
            // 도전의 탑 -> 다음 도전or 재도전 
            if (dg_mdty == IG.ModeType.DUNGEON_TOP)
            {
                int befor_inNbr = (int)MainUI.GetInstance().tapDungeon.dgTopNbr;
                int new_inNbr = GameDatabase.GetInstance().tableDB.GetUserInfo().GetDgNbr(IG.ModeType.DUNGEON_TOP);
                int tikCnt = GameDatabase.GetInstance().tableDB.GetItemDungeonTicket(IG.ModeType.DUNGEON_TOP);
                int dg_indx = GameDatabase.GetInstance().chartDB.list_cdb_dungeon_top.FindIndex(x => int.Equals(x.nbr, new_inNbr));
                btnDgTopNextOrRe.gameObject.SetActive(dg_indx >= 0 && tikCnt > 0);
                if (btnDgTopNextOrRe.gameObject.activeSelf)
                {
                    btnDgTopNextOrRe.onClick.RemoveAllListeners();
                    if(_isClear)
                        btnDgTopNextOrRe.onClick.AddListener(DgTopNext);
                    else
                        btnDgTopNextOrRe.onClick.AddListener(DgTopRe);

                    tx_dg_top_next.text = _isClear == true ?
                        string.Format(LanguageGameData.GetInstance().GetString("text.dungeon.top.go"), new_inNbr + 1) :   /*{0}층 도전*/
                        string.Format(LanguageGameData.GetInstance().GetString("text.dungeon.top.rego"), befor_inNbr + 1);  /*{0}층 재도전*/
                }
            }
            else btnDgTopNextOrRe.gameObject.SetActive(false);
        }

        go_close.SetActive(true);

        StopCoroutine("IEndPop");
        StartCoroutine("IEndPop");
        Loading.Full(true);
    }

    IEnumerator IELoopContinue()
    {
        yield return null;
        float end = Time.time + 5;
        while (end - Time.time > 0)
        {
            txLoopBtn.text = string.Format("-자동 반복 진행중 (<color=red>{0}회</color> 남음)-\n{1}초 뒤에 자동 진행됩니다.", dgLoop.loopCnt, ((int)(end - Time.time)).ToString());
            yield return null;
        }

        LoopInDungeon();
    }

    IEnumerator IEndPop()
    {
        yield return null;
        float end = Time.time + 60;
        while (end - Time.time > 0)
        {
            ui.tx_CloseTime.text = string.Format("확인 ({0})", ((int)(end - Time.time)).ToString());
            yield return null;
        }

        Click_EndPop();
    }
}
