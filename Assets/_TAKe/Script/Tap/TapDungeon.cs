using BackEnd;
using CodeStage.AntiCheat.ObscuredTypes;
using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class TapDungeon : MonoBehaviour
{
    [SerializeField] TapObject tapObject;

    /// ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
    /// 컨텐츠 탭 UI 정보 
    /// ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
    #region ##### struct #####
    public DgTap dgTap;
    [System.Serializable]
    public enum DgTap
    {
        CONTENTS_CELL = 0, TOP = 1, MINE = 2, RAID = 3, PVP_ARENA = 4
    }
    public string GetDungeonTypeStr() => dgTap.ToString();

    public DgNbrTop dgTopNbr = DgNbrTop.Nbr0;
    public enum DgNbrTop
    {
        Nbr0, Nbr1, Nbr2, Nbr3, Nbr4, Nbr5, Nbr6, Nbr7, Nbr8, Nbr9,
        Nbr10, Nbr11, Nbr12, Nbr13, Nbr14, Nbr15, Nbr16, Nbr17, Nbr18, Nbr19,
        Nbr20, Nbr21, Nbr22, Nbr23, Nbr24, Nbr25, Nbr26, Nbr27, Nbr28, Nbr29,
        Nbr30, Nbr31, Nbr32, Nbr33, Nbr34, Nbr35, Nbr36, Nbr37, Nbr38, Nbr39,
        Nbr40, Nbr41, Nbr42, Nbr43, Nbr44,
    }

    public DgNbrMine dgMineNbr = DgNbrMine.Nbr1;

    public enum DgNbrMine
    {
        Nbr0, Nbr1, Nbr2, Nbr3, Nbr4, Nbr5
    }

    public DgNbrRaid dgRaidNbr = DgNbrRaid.Nbr1;
    public enum DgNbrRaid
    {
        Nbr0, Nbr1, Nbr2, Nbr3, Nbr4, Nbr5, Nbr6
    }

    public UIContentsCell uiContentsCell;
    [System.Serializable]
    public struct UIContentsCell
    {
        public GameObject goTap;
        public Text txDgTopTicket, txDgMineTicket, txDgRaidTicket, txPvPArentTicket;
    }

    // 반복 진행 
    public DgLoop dgLoop;
    [System.Serializable]
    public struct DgLoop
    {
        public IG.ModeType loopDgType;
        public bool isLoop;
        public int loopCnt;
        public int loopDgNbr;
    }

    // ui 도전의 탑 
    public UIDgTop uiDgTop;
    [System.Serializable]
    public struct UIDgTop
    {
        public GameObject go_Tap;
        public InitOnStartDungeonReward initOnStart;
        public GameObject go_FrontBtn, go_BackBtn;
        public Text tx_Floor, tx_BtnFloorFrontNbr, tx_BtnFloorBackNbr;
        public Text tx_ClearState;
        public Text tx_AdviseCombat, tx_MyCombat;
        public Image im_BtnBotmSltBg;
        public Text tx_Ticket;

        public Text tx_HighClearRankStr;
        public Text tx_HighSec;
        public Text tx_LastSec;

        public Image imSweepBtnBg;
    }

    // ui 광산 
    public UIDgMine uiDgMine;
    [System.Serializable]
    public struct UIDgMine
    {
        public GameObject go_Tap;
        public InitOnStartDungeonReward initOnStart;
        public Image[] im_BtnBg, im_BtnNbrBox;
        public GameObject[] go_BtnLock;
        public Text tx_AdviseCombat, tx_MyCombat;
        public Image im_BtnBotmSltBg;
        public long n_combat_power;
        public Text tx_Ticket;

        public Text tx_HighClearRankStr;
        public Text tx_HighSec;
        public Text tx_LastSec;

        public Image imSweepBtnBg;
    }

    // ui 레이드 
    public UIDgRaid uiDgRaid;
    [System.Serializable]
    public struct UIDgRaid
    {
        public GameObject go_Tap;
        public InitOnStartDungeonReward initOnStart;
        public Image[] im_BtnBg, im_BtnNbrBox;
        public GameObject[] go_BtnLock;
        public Text tx_AdviseCombat, tx_MyCombat;
        public Image im_BtnBotmSltBg;
        public long n_combat_power;
        public Text tx_Ticket;

        public Text tx_HighClearRankStr;
        public Text tx_HighSec;
        public Text tx_LastSec;

        public Image imSweepBtnBg;
    }

    // ui PvP 배틀 아레나 
    public UIPvPBattleArena uiPvPBattleArena;
    [System.Serializable]
    public class UIPvPBattleArena
    {
        public bool is_matchFirstLoad, is_recordFirstLoad;
        public Image imConBtnBg; // 컨텐츠 탭에서 입장 버튼 
        public Text txConBtn;
        public GameObject go_Tap, goSubTapMatching, goSubTapRank, goSubTapRecord;
        public Image imSubTapMatchingBtnBg, imSubTapRankBtnBg, imSubTapRecordBtnBg;
        public InitOnStartPvPBattleArenaMatching initOnStartPvPBattleArenaMatching;
        public InitOnStartPvPBattleArenaRank initOnStartPvPBattleArenaRank;
        public InitOnStartPvPBattleArenaRecord initOnStartPvPBattleArenaRecord;

        public Color coAlphaZero, coWhite;
        public Text tx_Ticket;
        public GameObject goNoMatchingUser; // 매칭 유저가 없다. 
        public Text txWinComment;
        public Image imRankMyRankSpr;
        public Text txRankMyRank, txRankMyName, txRankMyScore;
        public TimeRemaining timeRemaining_user_battle_list;
    }
    #endregion

    public List<PopUpDungeonReward.ResultReward> resultRewardPreview = new List<PopUpDungeonReward.ResultReward>();
    public IG.ModeType saveMdTy;

    void OnEnable()
    {
        tapObject.aniIcon.Play("MainButtonActiveOnScale");
        tapObject.txName.fontStyle = FontStyle.Bold;
        tapObject.txName.color = tapObject.onCorSelect;
        tapObject.goOutline.SetActive(true);
    }

    void OnDisable()
    {
        tapObject.txName.fontStyle = FontStyle.Normal;
        tapObject.txName.color = tapObject.noCorSelect;
        tapObject.goOutline.SetActive(false);
    }

    void Awake()
    {
        LogPrint.Print("Awake");
        uiDgTop.go_Tap.SetActive(false);
        uiDgMine.go_Tap.SetActive(false);
        uiDgRaid.go_Tap.SetActive(false);
        uiPvPBattleArena.go_Tap.SetActive(false);

        uiPvPBattleArena.goSubTapMatching.SetActive(true);
        uiPvPBattleArena.goSubTapRank.SetActive(false);
        uiPvPBattleArena.goSubTapRecord.SetActive(false);

        uiContentsCell.goTap.SetActive(true);
    }

    void Start()
    {
        LogPrint.Print("Start");
        GameDatabase.GetInstance().pvpBattleRecord.GetClientLoadMySentRecord();
    }

    /// ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
    /// 컨텐츠 탭 UI 정보 
    /// ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
    #region ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
    // 컨텐츠 리스트 셀 UI 
    public void InfoContents()
    {
        LogPrint.Print("--InfoContents--");
        dgTap = DgTap.CONTENTS_CELL;
        uiDgTop.go_Tap.SetActive(false);
        uiDgMine.go_Tap.SetActive(false);
        uiDgRaid.go_Tap.SetActive(false);
        uiPvPBattleArena.go_Tap.SetActive(false);

        Ticket();

        // 배틀 아레나 입장 가능 스텡지ㅣ 
        int chpt_dvs_nbr = GameDatabase.GetInstance().tableDB.GetUserInfo().GetChapterDvsNbr();
        int open_chpt_dvs_nbr = GameDatabase.GetInstance().chartDB.GetDicBalance("pvp.battle.arena.lock.open.chpt_dvs_nbr").val_int;
        uiPvPBattleArena.imConBtnBg.sprite = SpriteAtlasMng.GetInstance().GetSpriteButtonRedOrGray(chpt_dvs_nbr >= open_chpt_dvs_nbr);
        uiPvPBattleArena.txConBtn.color = ResourceDatabase.GetInstance().GetColorButtonText(chpt_dvs_nbr >= open_chpt_dvs_nbr);
    }

    public void Ticket()
    {
        uiContentsCell.txDgTopTicket.text = GameDatabase.GetInstance().tableDB.GetStrDungeonTicket(IG.ModeType.DUNGEON_TOP);
        uiContentsCell.txDgMineTicket.text = GameDatabase.GetInstance().tableDB.GetStrDungeonTicket(IG.ModeType.DUNGEON_MINE);
        uiContentsCell.txDgRaidTicket.text = GameDatabase.GetInstance().tableDB.GetStrDungeonTicket(IG.ModeType.DUNGEON_RAID);
        uiContentsCell.txPvPArentTicket.text = GameDatabase.GetInstance().tableDB.GetStrDungeonTicket(IG.ModeType.PVP_BATTLE_ARENA);

        if (uiDgTop.go_Tap.gameObject.activeSelf)
            uiDgTop.tx_Ticket.text = GameDatabase.GetInstance().tableDB.GetStrDungeonTicket(IG.ModeType.DUNGEON_TOP);

        if (uiDgMine.go_Tap.gameObject.activeSelf)
            uiDgMine.tx_Ticket.text = GameDatabase.GetInstance().tableDB.GetStrDungeonTicket(IG.ModeType.DUNGEON_MINE);

        if (uiDgRaid.go_Tap.gameObject.activeSelf)
            uiDgRaid.tx_Ticket.text = GameDatabase.GetInstance().tableDB.GetStrDungeonTicket(IG.ModeType.DUNGEON_RAID);

        if (uiPvPBattleArena.go_Tap.gameObject.activeSelf)
            uiPvPBattleArena.tx_Ticket.text = GameDatabase.GetInstance().tableDB.GetStrDungeonTicket(IG.ModeType.PVP_BATTLE_ARENA);
    }

    //도전의 탑 UI 
    void InfoDungeonTop(int _nbr = -1)
    {
        dgLoop = new DgLoop();
        uiDgTop.go_Tap.SetActive(true);
        resultRewardPreview.Clear();
        int lastClrNbr = GameDatabase.GetInstance().tableDB.GetUserInfo().GetDgNbr(IG.ModeType.DUNGEON_TOP);
        int lastStgNbr = GameDatabase.GetInstance().chartDB.list_cdb_dungeon_top[GameDatabase.GetInstance().chartDB.list_cdb_dungeon_top.Count - 1].nbr;
        if (lastClrNbr > lastStgNbr)
            lastClrNbr = lastStgNbr;

        dgTopNbr = _nbr == -1 ? (DgNbrTop)lastClrNbr : (DgNbrTop)_nbr;

        LogPrint.Print("<color=red> ------------ 도전의 탑 UI ------------------- [_nbr : " + _nbr + ", lastClrNbr : " + lastClrNbr + ", dgTopNbr : " + dgTopNbr + "] </color>");

        // 내 최고 기록 
        float hg_sec = GameDatabase.GetInstance().dungeonDB.GetClearTime(IG.ModeType.DUNGEON_TOP, (int)dgTopNbr); // 내 최고 기록 
        if (hg_sec > 0)
        {
            int minutes = (int)(System.Math.Floor((double)(hg_sec / 60)));
            int second = (int)(System.Math.Floor((double)(hg_sec % 3600 % 60)));
            string[] msec = hg_sec.ToString("N2").Split('.');

            uiDgTop.tx_HighClearRankStr.text = GameDatabase.GetInstance().dungeonDB.GetClearRank(hg_sec, "TOP");
            uiDgTop.tx_HighSec.text = string.Format("{0:00}:{1:00}.{2}", minutes, second, msec[1]);

            float last_sec = PlayerPrefs.GetFloat(string.Format(PrefsKeys.prky_LastClearSecDungeonTopNbr, (int)dgTopNbr));
            int minutes2 = (int)(System.Math.Floor((double)(last_sec / 60)));
            int second2 = (int)(System.Math.Floor((double)(last_sec % 3600 % 60)));
            string[] msec2 = last_sec.ToString("N2").Split('.');
            uiDgTop.tx_LastSec.text = string.Format("{0:00}:{1:00}.{2}", minutes2, second2, msec2[1]);
        }
        else
        {
            uiDgTop.tx_HighClearRankStr.text = "---";
            uiDgTop.tx_HighSec.text = "--:--.--";
            uiDgTop.tx_LastSec.text = "--:--.--";
        }

        uiDgTop.imSweepBtnBg.sprite = string.Equals(uiDgTop.tx_HighClearRankStr.text, "S") ? SpriteAtlasMng.GetInstance().GetSpriteButtonBox("blue") : SpriteAtlasMng.GetInstance().GetSpriteButtonBox("gray");

        // 입장권 
        uiDgTop.tx_Ticket.text = GameDatabase.GetInstance().tableDB.GetStrDungeonTicket(IG.ModeType.DUNGEON_TOP);

        //현재 층
        uiDgTop.tx_Floor.text = string.Format(LanguageGameData.GetInstance().GetString("text.dungeon.top.floor"), (int)dgTopNbr + 1);
        uiDgTop.tx_Floor.color = lastClrNbr == (int)dgTopNbr ? Color.yellow : lastClrNbr < (int)dgTopNbr ? Color.red : Color.white;

        // 내 전투력 
        uiDgTop.tx_MyCombat.text = string.Format("{0:#,0}", GameDatabase.GetInstance().characterDB.GetStat().combat_power);

        var cdb = GameDatabase.GetInstance().chartDB.list_cdb_dungeon_top;
        int rwd_indx = cdb.FindIndex(x => x.nbr == (int)dgTopNbr);
        if (rwd_indx >= 0)
        {
            uiDgTop.tx_AdviseCombat.text = string.Format("{0:#,0}", GameDatabase.GetInstance().monsterDB.GetDungeonMonsterStatValue(IG.ModeType.DUNGEON_TOP, 0, (int)dgTopNbr).combat_power);

            RewardView(cdb[rwd_indx].reward);

            // 앞 층 미리 보기 
            if ((int)dgTopNbr + 1 > lastStgNbr)
                uiDgTop.go_FrontBtn.SetActive(false);
            else
            {
                uiDgTop.go_FrontBtn.SetActive(true);
                uiDgTop.tx_BtnFloorFrontNbr.text = string.Format("{0}▶", ((int)dgTopNbr + 1) + 1);
            }

            // 뒤 층 미리 보기 
            if ((int)dgTopNbr - 1 < 0)
                uiDgTop.go_BackBtn.SetActive(false);
            else
            {
                uiDgTop.go_BackBtn.SetActive(true);
                uiDgTop.tx_BtnFloorBackNbr.text = string.Format("◀{0}", ((int)dgTopNbr + 1) - 1);
            }
        }
        else
        {
            uiDgTop.tx_AdviseCombat.text = "-----";
            uiDgTop.go_BackBtn.SetActive(false);
            uiDgTop.go_BackBtn.SetActive(false);
        }

        uiDgTop.initOnStart.InitStart();
    }

    //광산 UI 
    void InfoDungeonMine(int _nbr = -1)
    {
        dgLoop = new DgLoop();
        uiDgMine.go_Tap.SetActive(true);
        resultRewardPreview.Clear();
        int lastClrNbr = GameDatabase.GetInstance().tableDB.GetUserInfo().GetDgNbr(IG.ModeType.DUNGEON_MINE);
        int lastStgNbr = GameDatabase.GetInstance().chartDB.list_cdb_dungeon_mine[GameDatabase.GetInstance().chartDB.list_cdb_dungeon_mine.Count - 1].nbr;
        if (lastClrNbr > lastStgNbr)
            lastClrNbr = lastStgNbr;

        dgMineNbr = _nbr == -1 ? (DgNbrMine)lastClrNbr : (DgNbrMine)_nbr;

        // 내 최고 기록 
        float hg_sec = GameDatabase.GetInstance().dungeonDB.GetClearTime(IG.ModeType.DUNGEON_MINE, (int)dgMineNbr); // 내 최고 기록 
        LogPrint.Print("lastClrNbr : " + lastClrNbr + ", _nbr : " + _nbr + ", 내 최고 기록 ; " + hg_sec);
        if (hg_sec > 0)
        {
            int minutes = (int)(System.Math.Floor((double)(hg_sec / 60)));
            int second = (int)(System.Math.Floor((double)(hg_sec % 3600 % 60)));
            string[] msec = hg_sec.ToString("N2").Split('.');

            uiDgMine.tx_HighClearRankStr.text = GameDatabase.GetInstance().dungeonDB.GetClearRank(hg_sec, "MINE");
            uiDgMine.tx_HighSec.text = string.Format("{0:00}:{1:00}.{2}", minutes, second, msec[1]);

            float last_sec = PlayerPrefs.GetFloat(string.Format(PrefsKeys.prky_LastClearSecDungeonMineNbr, (int)dgMineNbr));
            int minutes2 = (int)(System.Math.Floor((double)(last_sec / 60)));
            int second2 = (int)(System.Math.Floor((double)(last_sec % 3600 % 60)));
            string[] msec2 = last_sec.ToString("N2").Split('.');
            uiDgMine.tx_LastSec.text = string.Format("{0:00}:{1:00}.{2}", minutes2, second2, msec2[1]);
        }
        else
        {
            uiDgMine.tx_HighClearRankStr.text = "---";
            uiDgMine.tx_HighSec.text = "--:--.--";
            uiDgMine.tx_LastSec.text = "--:--.--";
        }

        uiDgMine.imSweepBtnBg.sprite = string.Equals(uiDgMine.tx_HighClearRankStr.text, "S") ? SpriteAtlasMng.GetInstance().GetSpriteButtonBox("blue") : SpriteAtlasMng.GetInstance().GetSpriteButtonBox("gray");

        // 입장권
        uiDgMine.tx_Ticket.text = GameDatabase.GetInstance().tableDB.GetStrDungeonTicket(IG.ModeType.DUNGEON_MINE);
        for (int i = 0; i < uiDgMine.im_BtnNbrBox.Length; i++)
        {
            uiDgMine.im_BtnBg[i].sprite = (int)dgMineNbr == i ? SpriteAtlasMng.GetInstance().GetSpriteRatingBg(5) : SpriteAtlasMng.GetInstance().GetSpriteRatingBg(0);
            uiDgMine.im_BtnNbrBox[i].enabled = (int)dgMineNbr == i;
            uiDgMine.go_BtnLock[i].SetActive(i > lastClrNbr);
        }

        // 내 전투력 
        uiDgMine.tx_MyCombat.text = string.Format("{0:#,0}", GameDatabase.GetInstance().characterDB.GetStat().combat_power);

        var cdb = GameDatabase.GetInstance().chartDB.list_cdb_dungeon_mine;
        int rwd_indx = cdb.FindIndex(x => x.nbr == (int)dgMineNbr);
        if (rwd_indx >= 0)
        {
            uiDgMine.n_combat_power = GameDatabase.GetInstance().monsterDB.GetDungeonMonsterStatValue(IG.ModeType.DUNGEON_MINE, 0, (int)dgMineNbr).combat_power;
            uiDgMine.tx_AdviseCombat.text = string.Format("{0:#,0}", uiDgMine.n_combat_power);

            RewardView(cdb[rwd_indx].reward);
        }
        else
        {
            uiDgMine.tx_AdviseCombat.text = "-----";
        }

        uiDgMine.initOnStart.InitStart();
    }

    //레이드 UI 
    void InfoDungeonRaid(int _nbr = -1)
    {
        dgLoop = new DgLoop();
        uiDgRaid.go_Tap.SetActive(true);
        resultRewardPreview.Clear();
        int lastClrNbr = GameDatabase.GetInstance().tableDB.GetUserInfo().GetDgNbr(IG.ModeType.DUNGEON_RAID);
        int dgCnt = GameDatabase.GetInstance().chartDB.list_cdb_dungeon_raid.Count;
        int lastStgNbr = GameDatabase.GetInstance().chartDB.list_cdb_dungeon_raid[dgCnt - 1].nbr;
        if (lastClrNbr > lastStgNbr)
            lastClrNbr = lastStgNbr;

        dgRaidNbr = _nbr == -1 ? (DgNbrRaid)lastClrNbr : (DgNbrRaid)_nbr;

        // 내 최고 기록 
        float hg_sec = GameDatabase.GetInstance().dungeonDB.GetClearTime(IG.ModeType.DUNGEON_RAID, (int)dgRaidNbr); // 내 최고 기록 
        LogPrint.Print("_nbr : " + _nbr + ", 내 최고 기록 ; " + hg_sec);
        if (hg_sec > 0)
        {
            int minutes = (int)(System.Math.Floor((double)(hg_sec / 60)));
            int second = (int)(System.Math.Floor((double)(hg_sec % 3600 % 60)));
            string[] msec = hg_sec.ToString("N2").Split('.');

            uiDgRaid.tx_HighClearRankStr.text = GameDatabase.GetInstance().dungeonDB.GetClearRank(hg_sec, "RAID");
            uiDgRaid.tx_HighSec.text = string.Format("{0:00}:{1:00}.{2}", minutes, second, msec[1]);

            float last_sec = PlayerPrefs.GetFloat(string.Format(PrefsKeys.prky_LastClearSecDungeonRaidNbr, (int)dgRaidNbr));
            int minutes2 = (int)(System.Math.Floor((double)(last_sec / 60)));
            int second2 = (int)(System.Math.Floor((double)(last_sec % 3600 % 60)));
            string[] msec2 = last_sec.ToString("N2").Split('.');
            uiDgRaid.tx_LastSec.text = string.Format("{0:00}:{1:00}.{2}", minutes2, second2, msec2[1]);
        }
        else
        {
            uiDgRaid.tx_HighClearRankStr.text = "---";
            uiDgRaid.tx_HighSec.text = "--:--.--";
            uiDgRaid.tx_LastSec.text = "--:--.--";
        }

        uiDgRaid.imSweepBtnBg.sprite = string.Equals(uiDgRaid.tx_HighClearRankStr.text, "S") ? SpriteAtlasMng.GetInstance().GetSpriteButtonBox("blue") : SpriteAtlasMng.GetInstance().GetSpriteButtonBox("gray");

        // 입장권
        uiDgRaid.tx_Ticket.text = GameDatabase.GetInstance().tableDB.GetStrDungeonTicket(IG.ModeType.DUNGEON_RAID);
        for (int i = 0; i < uiDgRaid.im_BtnNbrBox.Length; i++)
        {
            uiDgRaid.im_BtnBg[i].gameObject.SetActive(i < dgCnt);
            if (i < dgCnt)
            {
                uiDgRaid.im_BtnBg[i].sprite = (int)dgRaidNbr == i ? SpriteAtlasMng.GetInstance().GetSpriteRatingBg(5) : SpriteAtlasMng.GetInstance().GetSpriteRatingBg(0);
                uiDgRaid.im_BtnNbrBox[i].enabled = (int)dgRaidNbr == i;
                uiDgRaid.go_BtnLock[i].SetActive(i > lastClrNbr);
            }
        }

        // 내 전투력 
        uiDgRaid.tx_MyCombat.text = string.Format("{0:#,0}", GameDatabase.GetInstance().characterDB.GetStat().combat_power);

        var cdb = GameDatabase.GetInstance().chartDB.list_cdb_dungeon_raid;
        int rwd_indx = cdb.FindIndex(x => x.nbr == (int)dgRaidNbr);
        if (rwd_indx >= 0)
        {
            uiDgRaid.n_combat_power = GameDatabase.GetInstance().monsterDB.GetDungeonMonsterStatValue(IG.ModeType.DUNGEON_RAID, 0, (int)dgRaidNbr).combat_power;
            uiDgRaid.tx_AdviseCombat.text = string.Format("{0:#,0}", uiDgRaid.n_combat_power);

            RewardView(cdb[rwd_indx].reward);
        }
        else
        {
            uiDgRaid.tx_AdviseCombat.text = "-----";
        }

        uiDgRaid.initOnStart.InitStart();
    }

    /// ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
    /// 각 던전(탑,광산,레이드) 반복 진행 
    /// ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
    #region 
    public void SettingDungeonLoop(IG.ModeType dgType, int inLoopCnt)
    {
        if (dgType == IG.ModeType.DUNGEON_TOP)
        {
            dgLoop.loopDgType = IG.ModeType.DUNGEON_TOP;
            dgLoop.isLoop = true;
            dgLoop.loopCnt = inLoopCnt;
            dgLoop.loopDgNbr = (int)dgTopNbr;
            //InDungeonTop((int)dgTopNbr);
        }
        else if (dgType == IG.ModeType.DUNGEON_MINE)
        {
            dgLoop.loopDgType = IG.ModeType.DUNGEON_MINE;
            dgLoop.isLoop = true;
            dgLoop.loopCnt = inLoopCnt;
            dgLoop.loopDgNbr = (int)dgMineNbr;
            //InDungeonMine((int)dgMineNbr);
        }
        else if (dgType == IG.ModeType.DUNGEON_RAID)
        {
            dgLoop.loopDgType = IG.ModeType.DUNGEON_RAID;
            dgLoop.isLoop = true;
            dgLoop.loopCnt = inLoopCnt;
            dgLoop.loopDgNbr = (int)dgRaidNbr;
            //InDungeonRaid((int)dgRaidNbr);
        }
    }
    #endregion

    //PvP 배틀 아레나 UI 
    async void InfoPvPBattleArena()
    {
        LogPrint.Print("<color=magenta>InfoPvPBattleArena</color>");
        uiPvPBattleArena.go_Tap.SetActive(true);
        bool isTapOn_matching = uiPvPBattleArena.goSubTapMatching.activeSelf;
        bool isTapOn_rank = uiPvPBattleArena.goSubTapRank.activeSelf;
        bool isTapOn_record = uiPvPBattleArena.goSubTapRecord.activeSelf;
        uiPvPBattleArena.imSubTapMatchingBtnBg.color = isTapOn_matching ? uiPvPBattleArena.coWhite : uiPvPBattleArena.coAlphaZero;
        uiPvPBattleArena.imSubTapRankBtnBg.color = isTapOn_rank ? uiPvPBattleArena.coWhite : uiPvPBattleArena.coAlphaZero;
        uiPvPBattleArena.imSubTapRecordBtnBg.color = isTapOn_record ? uiPvPBattleArena.coWhite : uiPvPBattleArena.coAlphaZero;
        // 입장권
        uiPvPBattleArena.tx_Ticket.text = GameDatabase.GetInstance().tableDB.GetStrDungeonTicket(IG.ModeType.PVP_BATTLE_ARENA);

        DateTime nDate = BackendGpgsMng.GetInstance().GetNowTime();
        // 결투 탭 
        if (isTapOn_matching)
        {
            int matchUserCnt = GameDatabase.GetInstance().rankDB.GetListRTRankPvpBTLArenaRank().Count;
            Refresh_Commnet();
            if (uiPvPBattleArena.is_matchFirstLoad == false || matchUserCnt < 1)
            {
                GameDatabase.GetInstance().pvpBattle.ReMatchUserLoad();
                await GameDatabase.GetInstance().rankDB.AGetRTRankPvBTLArenaMatchingList();

                //DateTime nDt = BackendGpgsMng.GetInstance().GetNowTime().AddMinutes(10);
                //GameDatabase.GetInstance().pvpBattleRecord.structData.match.nextLoadTime = nDt;
                //PlayerPrefs.SetString(PrefsKeys.key_PvPArenaMatchNextLoadTime, nDt.ToString());
                uiPvPBattleArena.is_matchFirstLoad = true;
            }

            uiPvPBattleArena.initOnStartPvPBattleArenaMatching.SetInit();
            uiPvPBattleArena.timeRemaining_user_battle_list.PvpMatchUserLise();
        }
        // 랭킹 탭 
        else if (isTapOn_rank)
        {
            LogPrint.Print("1");
            DateTime nlt = GameDatabase.GetInstance().pvpBattleRecord.structData.rank.nextLoadTIme;
            if (nDate >= nlt)
            {
                LogPrint.Print("2");
                await GameDatabase.GetInstance().rankDB.AGetRTRankPvpBTLArenaRank(true, true);
                LogPrint.Print("3");
                LogPrint.Print(Backend.Rank.GetRankRewardList());

                nlt = nDate.AddMinutes(5);
                GameDatabase.GetInstance().pvpBattleRecord.structData.rank.nextLoadTIme = nlt;
                PlayerPrefs.SetString(PrefsKeys.key_PvPArenaRankNextLoadTime, nlt.ToString());
            }

            var myRInfo = GameDatabase.GetInstance().rankDB.GetMyRankPvPBTLArena();
            if (myRInfo.rank != -1)
            {
                uiPvPBattleArena.txRankMyRank.text = myRInfo.rank.ToString();
                uiPvPBattleArena.txRankMyName.text = myRInfo.nickName.ToString();
                uiPvPBattleArena.txRankMyScore.text = myRInfo.score.ToString();
                uiPvPBattleArena.imRankMyRankSpr.sprite = SpriteAtlasMng.GetInstance().GetSpriteRankTrophy(myRInfo.rank);
            }
            else
            {
                uiPvPBattleArena.txRankMyRank.text = "-";
                uiPvPBattleArena.txRankMyName.text = "-";
                uiPvPBattleArena.txRankMyScore.text = "-";
                uiPvPBattleArena.imRankMyRankSpr.sprite = SpriteAtlasMng.GetInstance().GetSpriteRankTrophy(0);
            }

            uiPvPBattleArena.initOnStartPvPBattleArenaRank.SetInit();
        }
        // 기록 탭 
        else if (isTapOn_record)
        {
            bool is_ReceivNewNotif = GameDatabase.GetInstance().pvpBattleRecord.structData.record.isNewReceivNotif || uiPvPBattleArena.is_recordFirstLoad == false;
            bool is_SentNew = GameDatabase.GetInstance().pvpBattleRecord.structData.record.isNewSent || uiPvPBattleArena.is_recordFirstLoad == false;
            DateTime nlt = GameDatabase.GetInstance().pvpBattleRecord.structData.record.nextLoadTIme;
            // 최초 1회 무조건 로드 || 
            if (uiPvPBattleArena.is_recordFirstLoad == false || (is_SentNew == true || (is_ReceivNewNotif == true && nDate > nlt)))
            {
                await GameDatabase.GetInstance().pvpBattleRecord.AGetLoadRecord(is_ReceivNewNotif, is_SentNew);
                await GameDatabase.GetInstance().pvpBattleRecord.DeleteRecordLastFrom20();

                NotificationIcon.GetInstance().OffPvpReceiveRecord();
                uiPvPBattleArena.is_recordFirstLoad = true;
            }

            uiPvPBattleArena.initOnStartPvPBattleArenaRecord.SetInit();
        }
    }
    #endregion

    /// ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
    /// 던전 소탕하기 
    /// ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
    #region ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
    public void ClickSweep(string dg_name)
    {
        int inNbr = -1;
        IG.ModeType mty = IG.ModeType.DUNGEON_TOP;
        float highRankSec = 0.0f;
        bool isSRank = false;
        int tikCnt = 0;
        switch (dg_name)
        {
            case "top":
                int fbNbr1 = (int)dgTopNbr;
                int inNbr1 = GameDatabase.GetInstance().tableDB.GetUserInfo().GetDgNbr(IG.ModeType.DUNGEON_TOP);
                tikCnt = GameDatabase.GetInstance().tableDB.GetItemDungeonTicket(IG.ModeType.DUNGEON_TOP);
                if (fbNbr1 <= inNbr1)
                {
                    if(tikCnt > 0)
                    {
                        mty = IG.ModeType.DUNGEON_TOP;
                        isSRank = string.Equals(uiDgTop.tx_HighClearRankStr.text, "S");
                        int indx1 = GameDatabase.GetInstance().chartDB.list_cdb_dungeon_top.FindIndex(x => int.Equals(x.nbr, (int)dgTopNbr));
                        if (indx1 >= 0)
                        {
                            inNbr = GameDatabase.GetInstance().chartDB.list_cdb_dungeon_top[indx1].nbr;
                            highRankSec = GameDatabase.GetInstance().dungeonDB.GetClearTime(IG.ModeType.DUNGEON_TOP, inNbr); // 내 최고 기록 
                        }
                    }
                    else
                    {
                        PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener("알 수 없는 탑 입장권이 부족합니다.\n입장권 구매 탭으로 이동됩니다.", MainUI.GetInstance().Listener_MoveItemShop);
                        return;
                    }
                }
                else
                {
                    PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("이전 난이도 클리어 랭크 S 등급 달성 후 소탕 가능합니다.");
                    return;
                }
                break;
            case "mine":
                int fbNbr2 = (int)dgMineNbr;
                int inNbr2 = GameDatabase.GetInstance().tableDB.GetUserInfo().GetDgNbr(IG.ModeType.DUNGEON_MINE);
                tikCnt = GameDatabase.GetInstance().tableDB.GetItemDungeonTicket(IG.ModeType.DUNGEON_MINE);
                if (fbNbr2 <= inNbr2)
                {
                    if (tikCnt > 0)
                    {
                        mty = IG.ModeType.DUNGEON_MINE;
                        isSRank = string.Equals(uiDgMine.tx_HighClearRankStr.text, "S");
                        int indx2 = GameDatabase.GetInstance().chartDB.list_cdb_dungeon_mine.FindIndex(x => int.Equals(x.nbr, (int)dgMineNbr));
                        if (indx2 >= 0)
                        {
                            inNbr = GameDatabase.GetInstance().chartDB.list_cdb_dungeon_mine[indx2].nbr;
                            highRankSec = GameDatabase.GetInstance().dungeonDB.GetClearTime(IG.ModeType.DUNGEON_TOP, inNbr); // 내 최고 기록 
                        }
                    }
                    else
                    {
                        PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener("광산 입장권이 부족합니다.\n입장권 구매 탭으로 이동됩니다.", MainUI.GetInstance().Listener_MoveItemShop);
                        return;
                    }
                }
                else
                {
                    PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("이전 난이도 클리어 랭크 S 등급 달성 후 소탕 가능합니다.");
                    return;
                }
                break;
            case "raid":
                int fbNbr3 = (int)dgRaidNbr;
                int inNbr3 = GameDatabase.GetInstance().tableDB.GetUserInfo().GetDgNbr(IG.ModeType.DUNGEON_RAID);
                tikCnt = GameDatabase.GetInstance().tableDB.GetItemDungeonTicket(IG.ModeType.DUNGEON_RAID);
                if (fbNbr3 <= inNbr3)
                {
                    if (tikCnt > 0)
                    {
                        mty = IG.ModeType.DUNGEON_RAID;
                        isSRank = string.Equals(uiDgRaid.tx_HighClearRankStr.text, "S");
                        int indx3 = GameDatabase.GetInstance().chartDB.list_cdb_dungeon_raid.FindIndex(x => int.Equals(x.nbr, (int)dgRaidNbr));
                        if (indx3 >= 0)
                        {
                            inNbr = GameDatabase.GetInstance().chartDB.list_cdb_dungeon_raid[indx3].nbr;
                            highRankSec = GameDatabase.GetInstance().dungeonDB.GetClearTime(IG.ModeType.DUNGEON_TOP, inNbr); // 내 최고 기록 
                        }
                    }
                    else
                    {
                        PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener("레이드 입장권이 부족합니다.\n입장권 구매 탭으로 이동됩니다.", MainUI.GetInstance().Listener_MoveItemShop);
                        return;
                    }
                }
                else
                {
                    PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("이전 난이도 클리어 랭크 S 등급 달성 후 소탕 가능합니다.");
                    return;
                }
                break;
        }
       
        if (isSRank && inNbr >= 0)
        {
            if(tikCnt > 0)
            {
                PopUpMng.GetInstance().Open_PopUpSweep(mty, inNbr, highRankSec, tikCnt);
            }
            else
            {
                PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("소탕권(던전 입장권)이 부족합니다.");
            }
            
            //if (string.Equals(dg_name, "top"))
            //    GameDatabase.GetInstance().dailyMissionDB.ASetInCount(GameDatabase.DailyMissionDB.Nbr.nbr9, 1); // 일일미션, nbr9 던전 : 탑 진행하기! 
            //else if (string.Equals(dg_name, "mine"))
            //    GameDatabase.GetInstance().dailyMissionDB.ASetInCount(GameDatabase.DailyMissionDB.Nbr.nbr10, 1); // 일일미션, nbr10 던전 : 광산 진행하기! 
            //else if (string.Equals(dg_name, "raid"))
            //    GameDatabase.GetInstance().dailyMissionDB.ASetInCount(GameDatabase.DailyMissionDB.Nbr.nbr11, 1); // 일일미션, nbr11 던전 : 레이드 진행하기! 

            //GameMng.GetInstance().gameUIObject.dungeonTop.pu_DungeonReward.gameObject.SetActive(true);
            //GameMng.GetInstance().gameUIObject.dungeonTop.pu_DungeonReward.SetData(mty, inNbr, true, highRankSec, true);
        }
        else
        {
            if (!isSRank)
            {
                PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("소탕하기는 클리어 랭크 S 등급부터 가능합니다.");
            }
        }
    }
    #endregion

    /// ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
    /// PvP 배틀 아레나 
    /// ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
    #region ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
    /// <summary> 결투할 상대의 데이터를 불러와서 GameDatabase.PvPBattle 에 세팅한다.  </summary>
    public async void LoadPvPBattleOpponentData(GameDatabase.PvPBattle.Data.GamerInfo _gmrInfo)
    {
        LogPrint.Print("LoadPvPBattleOpponentData _gmrInfo : " + JsonUtility.ToJson(_gmrInfo));
        if(_gmrInfo.isAI == false) // 실제 유저 데이터 인경우 
        {
            BackendReturnObject bro1 = null;
            SendQueue.Enqueue(Backend.GameInfo.GetPublicContentsByGamerIndate, BackendGpgsMng.tableName_Pub_NowCharData, _gmrInfo.gamer_indate, callback => { bro1 = callback; });
            while (Loading.Bottom(bro1) == false) { await Task.Delay(100); }
            if (bro1.IsSuccess())
            {
                JsonData rows = bro1.GetReturnValuetoJSON()["rows"]; //await GameDatabase.GetInstance().publicContentDB.AGetPubTableDataLoad(BackendGpgsMng.tableName_Pub_NowCharData, gamerInDate);
                if (rows == null || rows.Count == 0)
                {
                    PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("결투 상대 데이터 정보 오류입니다.");
                    PopUpMng.GetInstance().Close_PvpBattleOpponent();
                }
                else
                {
                    LogPrint.Print("LoadPvPBattleOpponentData 결투 상대 db rows : " + bro1);
                    try // 유저 장비 
                    {
                        JsonData row = rows[0];
                        _gmrInfo.gamer_comment = RowPaser.StrPaser(row, "m_comment");
                        GameDatabase.GetInstance().pvpBattle.SetBattleData(_gmrInfo, row);// _eqList, db_stat, _gmrInfo);
                    }
                    catch (Exception)
                    {
                        PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("결투 상대 데이터 정보 오류입니다.");
                        PopUpMng.GetInstance().Close_PvpBattleOpponent();
                    }
                }
            }
            else
            {
                PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("결투 상대 데이터 정보 오류입니다.");
                PopUpMng.GetInstance().Close_PvpBattleOpponent();
            }
        }
        else // AI 데이터 인경우 
        {
            if(_gmrInfo.ai_cpht_dvs_nbr > 0)
            {
                GameDatabase.GetInstance().pvpBattle.SetBattleData(_gmrInfo, null);// _eqList, db_stat, _gmrInfo);
            }
            else
            {
                PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("결투 상대 데이터 정보 오류입니다.");
                PopUpMng.GetInstance().Close_PvpBattleOpponent();
            }
        }
    }

    public void LoadPvPBattleOpponentData()
    {

    }

    /// <summary>  결투 승리 멘트 변경 팝업 </summary>
    public void Click_PopComment() => PopUpMng.GetInstance().Open_PopUpPvpWinnerComment();
    public void Refresh_Commnet() => uiPvPBattleArena.txWinComment.text = PlayerPrefs.GetString(PrefsKeys.key_PvPArenaComment);

    /// 결투 상대 갱신 
    public void Click_RefreshMatch()
    {
        bool isFree = (GameDatabase.GetInstance().pvpBattleRecord.structData.match.nextLoadTime - BackendGpgsMng.GetInstance().GetNowTime()).TotalSeconds < 0;
        if (isFree)
            PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener("결투 상대 리스트를 새로고침하시겠습니까?\n<color=#698FFF>다이아 x0 소모</color>", Listener_FreeRefreshMatchList);
        else
            PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener(string.Format("결투 상대 리스트를 새로고침하시겠습니까?\n<color=#698FFF>다이아 x{0} 소모</color>", 5), Listener_RefreshMatchList);
    }

    void Listener_FreeRefreshMatchList() => RefreshMatch(true);
    void Listener_RefreshMatchList() => RefreshMatch(false);


    async void RefreshMatch(bool isFree)
    {
        bool succ = false;
        // 무료로 갱신 
        if (isFree == true)
        {
            DateTime nDt = BackendGpgsMng.GetInstance().GetNowTime().AddMinutes(10);
            GameDatabase.GetInstance().pvpBattleRecord.structData.match.nextLoadTime = nDt;
            PlayerPrefs.SetString(PrefsKeys.key_PvPArenaMatchNextLoadTime, nDt.ToString());
            uiPvPBattleArena.is_matchFirstLoad = false;
            InfoPvPBattleArena();
        }
        // 캐쉬로 갱신 
        else
        {
            int diaPrice = GameDatabase.GetInstance().chartDB.GetDicBalance("pvp.matching.refresh").val_int;
            var goods_db = GameDatabase.GetInstance().tableDB.GetTableDB_Goods();
            bool isBlueDiaLack = goods_db.m_dia < diaPrice;
            int tbc = isBlueDiaLack == true ? await GameDatabase.GetInstance().tableDB.GetMyTBC() : 0;
            int blue_dia = goods_db.m_dia;
            if (blue_dia + tbc >= diaPrice)
            {
                int dedDia = goods_db.m_dia -= diaPrice; // 내 현재 블루 다이아 차감
                int dedTbc = dedDia < 0 ? Math.Abs(dedDia) : 0;

                Task tsk1 = GameDatabase.GetInstance().tableDB.SetUpdateGoods(goods_db);
                Task tsk2 = GameDatabase.GetInstance().tableDB.DeductionTBC(dedTbc);
                while (Loading.Full(tsk1.IsCompleted, tsk2.IsCompleted) == false) await Task.Delay(100);
                
                DateTime nDt = BackendGpgsMng.GetInstance().GetNowTime().AddMinutes(10);
                GameDatabase.GetInstance().pvpBattleRecord.structData.match.nextLoadTime = nDt;
                PlayerPrefs.SetString(PrefsKeys.key_PvPArenaMatchNextLoadTime, nDt.ToString());
                uiPvPBattleArena.is_matchFirstLoad = false;
                InfoPvPBattleArena();
            }
            else
            {
                PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener("다이아가 부족합니다.\n다이아 구매 탭으로 이동됩니다.", MainUI.GetInstance().Listener_MoveTbcShop);
            }
        }
    }

    #endregion

    /// ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
    /// 던전 클리어 보상 미리 보그 
    /// ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
    #region ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
    // 클리어 보상 미리 보기 
    void RewardView(DungeonReward dg_rwds)
    {
        if (dg_rwds.qst_gold > 0)
        {
            resultRewardPreview.Add(new PopUpDungeonReward.ResultReward()
            { rwd_type = PopUpDungeonReward.RwdType.GOLD, rwdGold = new PopUpDungeonReward.ResultReward.RwdGold() { cnt = dg_rwds.qst_gold } });
        }

        // ##### 장비 조각 (1개 등급 고정)
        if (dg_rwds.rw_eq_pce_rt > 0)
        {
            resultRewardPreview.Add(new PopUpDungeonReward.ResultReward()
            { rwd_type = PopUpDungeonReward.RwdType.EQUIP_PIECE, rwdEquipPiece = new PopUpDungeonReward.ResultReward.RwdEquipPiece() { pce_rt = dg_rwds.rw_eq_pce_rt, pce_cnt = dg_rwds.rw_eq_pce_cnt } });
        }

        // ##### 장신구 조각 (1개 등급 고정)
        if (dg_rwds.rw_eqac_pce_rt > 0)
        {
            resultRewardPreview.Add(new PopUpDungeonReward.ResultReward()
            { rwd_type = PopUpDungeonReward.RwdType.EQUIP_AC_PIECE, rwdEquipAcPiece = new PopUpDungeonReward.ResultReward.RwdEquipAcPiece() { pce_rt = dg_rwds.rw_eqac_pce_rt, pce_cnt = dg_rwds.rw_eqac_pce_cnt } });
        }

        // ##### 장비 조각 (다중)
        if (dg_rwds.rw_eq_pces.Count > 0)
        {
            var item = dg_rwds.rw_eq_pces[0];
            for (int i = 0; i < item.rwds.Count; i++)
            {
                if (item.rwds[i].rt > 0)
                {
                    resultRewardPreview.Add(new PopUpDungeonReward.ResultReward()
                    { rwd_type = PopUpDungeonReward.RwdType.EQUIP_PIECE, rwdEquipPiece = new PopUpDungeonReward.ResultReward.RwdEquipPiece() { pce_rt = item.rwds[i].rt, pce_cnt = item.rwds[i].cnt } });
                }
            }
        }

        // ##### 장신구 조각 (다중)
        if (dg_rwds.rw_eqac_pces.Count > 0)
        {
            var item = dg_rwds.rw_eqac_pces[0];
            for (int i = 0; i < item.rwds.Count; i++)
            {
                if (item.rwds[i].rt > 0)
                {
                    resultRewardPreview.Add(new PopUpDungeonReward.ResultReward()
                    { rwd_type = PopUpDungeonReward.RwdType.EQUIP_AC_PIECE, rwdEquipAcPiece = new PopUpDungeonReward.ResultReward.RwdEquipAcPiece() { pce_rt = item.rwds[i].rt, pce_cnt = item.rwds[i].cnt } });
                }
            }
        }

        // ##### 장비 
        if (dg_rwds.rw_eq_rt > 0)
        {
            resultRewardPreview.Add(new PopUpDungeonReward.ResultReward()
            { rwd_type = PopUpDungeonReward.RwdType.EQUIP, rwdEquip = new PopUpDungeonReward.ResultReward.RwdEquip() { eq_ty = dg_rwds.rw_eq_ty, eq_rt = dg_rwds.rw_eq_rt, eq_id = dg_rwds.rw_eq_id } });
        }

        // ##### 장신구 
        if (dg_rwds.rw_eqac_rt > 0)
        {
            resultRewardPreview.Add(new PopUpDungeonReward.ResultReward()
            { rwd_type = PopUpDungeonReward.RwdType.EQUIP_AC, rwdEquip = new PopUpDungeonReward.ResultReward.RwdEquip() { eq_ty = dg_rwds.rw_eqac_ty, eq_rt = dg_rwds.rw_eqac_rt, eq_id = dg_rwds.rw_eqac_id } });
        }

        // 레이드 한정 
        // ##### 장신구 (랜덤)
        if (dg_rwds.rw_pct_eq_ac.Count > 0)
        {
            foreach (var item in dg_rwds.rw_pct_eq_ac)
            {
                if (item.rwds[0].eq_rt > 0)
                {
                    resultRewardPreview.Add(new PopUpDungeonReward.ResultReward()
                    { rwd_type = PopUpDungeonReward.RwdType.EQUIP_AC, rwdEquip = new PopUpDungeonReward.ResultReward.RwdEquip() { eq_ty = 8, eq_rt = item.rwds[0].eq_rt, eq_id = item.rwds[0].eq_id, pct = item.pct } });
                }
            }
        }

        // ##### 장비 (랜덤)
        if (dg_rwds.rw_pct_eq.Count > 0)
        {
            foreach (var item in dg_rwds.rw_pct_eq)
            {
                if (item.rwds[0].eq_rt > 0)
                {
                    resultRewardPreview.Add(new PopUpDungeonReward.ResultReward()
                    { rwd_type = PopUpDungeonReward.RwdType.EQUIP, rwdEquip = new PopUpDungeonReward.ResultReward.RwdEquip() { eq_ty = 0, eq_rt = item.rwds[0].eq_rt, eq_id = item.rwds[0].eq_id, pct = item.pct } });
                }
            }
        }

        // ##### 스킬 
        if (dg_rwds.rw_sk_id > 0)
        {
            resultRewardPreview.Add(new PopUpDungeonReward.ResultReward()
            { rwd_type = PopUpDungeonReward.RwdType.SKILL, rwdSkill = new PopUpDungeonReward.ResultReward.RwdSkill() { sk_id = dg_rwds.rw_sk_id, sk_cnt = dg_rwds.rw_sk_cnt } });
        }

        // ##### 장비 강화석
        if (dg_rwds.rw_eq_ehnt_ston.Count > 0)
        {
            var item = dg_rwds.rw_eq_ehnt_ston[0];
            for (int i = 0; i < item.rwds.Count; i++)
            {
                if (item.rwds[i].rt > 0)
                {
                    resultRewardPreview.Add(new PopUpDungeonReward.ResultReward()
                    { rwd_type = PopUpDungeonReward.RwdType.EQUIP_ENHANT_STON, rwdEquipEnhantSton = new PopUpDungeonReward.ResultReward.RwdEquipEnhantSton() { eq_stn_rt = item.rwds[i].rt, eq_stn_cnt = item.rwds[i].cnt } });
                }
            }
        }

        // ##### 장신구 강화석 
        if (dg_rwds.rw_eqac_ehnt_ston.Count > 0)
        {
            var item = dg_rwds.rw_eqac_ehnt_ston[0];
            for (int i = 0; i < item.rwds.Count; i++)
            {
                if (item.rwds[i].rt > 0)
                {
                    resultRewardPreview.Add(new PopUpDungeonReward.ResultReward()
                    { rwd_type = PopUpDungeonReward.RwdType.EQUIP_AC_ENHANT_STON, rwdEquipAcEnhantSton = new PopUpDungeonReward.ResultReward.RwdEquipAcEnhantSton() { eqac_stn_rt = item.rwds[i].rt, eqac_stn_cnt = item.rwds[i].cnt } });
                }
            }
        }

        // ##### 강화 축복 주문서
        if (dg_rwds.rw_ehnt_bless_rt > 0)
        {
            resultRewardPreview.Add(new PopUpDungeonReward.ResultReward()
            { rwd_type = PopUpDungeonReward.RwdType.ENHANT_BLESS, rwdEnhantBless = new PopUpDungeonReward.ResultReward.RwdEnhantBless() { bls_rt = dg_rwds.rw_ehnt_bless_rt, bls_cnt = dg_rwds.rw_ehnt_bless_cnt } });
        }

        // ##### 에테르 
        if (dg_rwds.rw_equip_ac_crystal.rw_goods_ether_cnt > 0)
        {
            resultRewardPreview.Add(new PopUpDungeonReward.ResultReward()
            { rwd_type = PopUpDungeonReward.RwdType.GOODS_ETHER, rwdGoodsEther = new PopUpDungeonReward.ResultReward.RwdGoodsEther() { ether_cnt = dg_rwds.rw_equip_ac_crystal.rw_goods_ether_cnt } });
        }
    }
    #endregion

    /// ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
    /// 컨텐츠 입장
    /// ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
    #region ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
    /// <summary>
    /// 도전의 탑 입장
    /// </summary>
    public void Click_InDungeon_Top()
    {
        int fbNbr = (int)dgTopNbr;
        int inNbr = GameDatabase.GetInstance().tableDB.GetUserInfo().GetDgNbr(IG.ModeType.DUNGEON_TOP);
        int tikCnt = GameDatabase.GetInstance().tableDB.GetItemDungeonTicket(IG.ModeType.DUNGEON_TOP);

        if (fbNbr <= inNbr)
        {
            if (tikCnt > 0)
            {
                PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener(string.Format(string.Format("알 수 없는 탑 : {0}층에 입장합니다.\n<color=#698FFF>입장권 x1장 소모</color>", fbNbr + 1)), Listener_InDungeonTop);
            }
            else PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener("알 수 없는 탑 입장권이 부족합니다.\n입장권 구매 탭으로 이동됩니다.", MainUI.GetInstance().Listener_MoveItemShop);
        }
        else PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox(string.Format("{0}층 클리어를 하면 입장할 수 있습니다.", fbNbr));
    }

    public void Listener_InDungeonTop() => InDungeonTop((int)dgTopNbr);

    void InDungeonTop(int inNbr)
    {
        var mt = GameMng.GetInstance().mode_type;
        if ((mt == IG.ModeType.CHAPTER_CONTINUE || mt == IG.ModeType.CHAPTER_LOOP) && GameMng.GetInstance().mode_type != IG.ModeType.CHANGE_WAIT)
        {
            int dg_indx = GameDatabase.GetInstance().chartDB.list_cdb_dungeon_top.FindIndex(x => int.Equals(x.nbr, inNbr));
            if (dg_indx >= 0)
            {
                saveMdTy = mt;
                GameMng.GetInstance().ChangeMode_Top(inNbr);
                GameDatabase.GetInstance().dailyMissionDB.ASetInCount(GameDatabase.DailyMissionDB.Nbr.nbr9, 1); // 일일미션, nbr9 던전 : 도전의 탑 진행하기! 
            }
            else
            {
                PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("입장 가능한 던전 정보가 잘못되었습니다.");
            }
        }
    }

    /// <summary>
    /// 광산 입장 
    /// </summary>
    public void Click_InDungeon_Mine()
    {
        int fbNbr = (int)dgMineNbr;
        int inNbr = GameDatabase.GetInstance().tableDB.GetUserInfo().GetDgNbr(IG.ModeType.DUNGEON_MINE);
        int tikCnt = GameDatabase.GetInstance().tableDB.GetItemDungeonTicket(IG.ModeType.DUNGEON_MINE);
        if (tikCnt > 0)
        {
            var mt = GameMng.GetInstance().mode_type;
            if ((mt == IG.ModeType.CHAPTER_CONTINUE || mt == IG.ModeType.CHAPTER_LOOP) && GameMng.GetInstance().mode_type != IG.ModeType.CHANGE_WAIT)
            {
                if (fbNbr <= inNbr)
                {
                    string rtTxt = LanguageGameData.GetInstance().GetString(string.Format("text.dungeon.difficulty.nbr{0}", (int)dgMineNbr));
                    string txt = string.Format(string.Format("광산 : 난이도 [{0}] 입장합니다.\n<color=#698FFF>입장권 x1장 소모</color>", rtTxt));
                    PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener(txt, Listener_InDungeonMine);
                }
                else PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("이전 난이도 성공 후 입장 가능");
            }
        }
        else PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener("버려진 광산 입장권이 부족합니다.\n입장권 구매 탭으로 이동됩니다.", MainUI.GetInstance().Listener_MoveItemShop);
    }
    public void Listener_InDungeonMine() => InDungeonMine((int)dgMineNbr);

    public void InDungeonMine(int inNbr)
    {
        var mt = GameMng.GetInstance().mode_type;
        if ((mt == IG.ModeType.CHAPTER_CONTINUE || mt == IG.ModeType.CHAPTER_LOOP) && GameMng.GetInstance().mode_type != IG.ModeType.CHANGE_WAIT)
        {
            int dg_indx = GameDatabase.GetInstance().chartDB.list_cdb_dungeon_mine.FindIndex(x => int.Equals(x.nbr, inNbr));
            if (dg_indx >= 0)
            {
                saveMdTy = mt;
                GameMng.GetInstance().ChangeMode_Mine(inNbr);
                GameDatabase.GetInstance().dailyMissionDB.ASetInCount(GameDatabase.DailyMissionDB.Nbr.nbr10, 1); // 일일미션, nbr10 던전 : 광산 진행하기! 
            }
            else
            {
                PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("입장 가능한 던전 정보가 잘못되었습니다.");
            }
        }
    }

    /// <summary>
    /// 레이드 입장 
    /// </summary>
    public void Click_InDungeon_Raid()
    {
        int fbNbr = (int)dgRaidNbr;
        int inNbr = GameDatabase.GetInstance().tableDB.GetUserInfo().GetDgNbr(IG.ModeType.DUNGEON_RAID);
        int tikCnt = GameDatabase.GetInstance().tableDB.GetItemDungeonTicket(IG.ModeType.DUNGEON_RAID);
        if (tikCnt > 0)
        {
            var mt = GameMng.GetInstance().mode_type;
            if ((mt == IG.ModeType.CHAPTER_CONTINUE || mt == IG.ModeType.CHAPTER_LOOP) && GameMng.GetInstance().mode_type != IG.ModeType.CHANGE_WAIT)
            {
                if (fbNbr <= inNbr)
                {
                    string rtTxt = LanguageGameData.GetInstance().GetString(string.Format("text.dungeon.difficulty.nbr{0}", (int)dgRaidNbr));
                    PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener(string.Format(string.Format("레이드 : 난이도 [{0}] 입장합니다.\n<color=#698FFF>입장권 x1장 소모</color>", rtTxt)), Listener_InDungeonRaid);
                }
                else PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("이전 난이도 성공 후 입장 할 수 있습니다.");
            }
        }
        else PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener("레이드 입장권이 부족합니다.\n입장권 구매 탭으로 이동됩니다.", MainUI.GetInstance().Listener_MoveItemShop);
    }

    public void Listener_InDungeonRaid() => InDungeonRaid((int)dgRaidNbr);

    public void InDungeonRaid(int inNbr)
    {
        var mt = GameMng.GetInstance().mode_type;
        if ((mt == IG.ModeType.CHAPTER_CONTINUE || mt == IG.ModeType.CHAPTER_LOOP) && GameMng.GetInstance().mode_type != IG.ModeType.CHANGE_WAIT)
        {
            int dg_indx = GameDatabase.GetInstance().chartDB.list_cdb_dungeon_raid.FindIndex(x => int.Equals(x.nbr, inNbr));
            if (dg_indx >= 0)
            {
                saveMdTy = mt;
                GameMng.GetInstance().ChangeMode_Raid(inNbr);
                GameDatabase.GetInstance().dailyMissionDB.ASetInCount(GameDatabase.DailyMissionDB.Nbr.nbr11, 1); // 일일미션, nbr11 던전 : 레이드 진행하기! 
            }
            else
            {
                PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("입장 가능한 던전 정보가 잘못되었습니다.");
            }
        }
    }

    public void Listener_LoopInDungeonRaid()
    {

    }

    /// <summary>
    /// PvP 배틀 아레나 입장 
    /// </summary>
    public void Click_InPvPBattleArena()
    {

    }
    #endregion

    /// ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
    /// 컨텐츠 탭 변경 
    /// ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
    #region ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
    // 컨텐츠 탭 변경
    // 0:컨텐츠 리스트 셀, 1:도전의탑, 2:광산, 3:레이드, 4:PvP 배틀 아레나 
    public void Click_TapOpen(int tapNbr)
    {
        if(tapNbr == 4)
        {
            int chpt_dvs_nbr = GameDatabase.GetInstance().tableDB.GetUserInfo().GetChapterDvsNbr();
            int open_chpt_dvs_nbr = GameDatabase.GetInstance().chartDB.GetDicBalance("pvp.battle.arena.lock.open.chpt_dvs_nbr").val_int;
            if (chpt_dvs_nbr < open_chpt_dvs_nbr)
            {
                int chpt_id = GameDatabase.GetInstance().monsterDB.GetChapterDvsNbrFindChapterID(open_chpt_dvs_nbr);
                int stg_id = GameDatabase.GetInstance().monsterDB.GetChapterDvsNbrFindStageID(open_chpt_dvs_nbr);
                PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox(string.Format("{0}챕터, {1}스테이지부터 PvP 배틀 아레나에 입장할 수 있습니다.", chpt_id, stg_id));
                return;
            }
        }

        dgTap = (DgTap)tapNbr;
        uiContentsCell.goTap.SetActive(dgTap == DgTap.CONTENTS_CELL);
        uiDgTop.go_Tap.SetActive(dgTap == DgTap.TOP);
        uiDgMine.go_Tap.SetActive(dgTap == DgTap.MINE);
        uiDgRaid.go_Tap.SetActive(dgTap == DgTap.RAID);
        uiPvPBattleArena.go_Tap.SetActive(dgTap == DgTap.PVP_ARENA);

        TapInfo();
    }

    public void TapInfo()
    {
        LogPrint.Print("<color=yellow> EEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEE </color>");

        if (uiContentsCell.goTap.activeSelf)
            InfoContents();
        else if (uiDgTop.go_Tap.activeSelf)
            InfoDungeonTop();
        else if (uiDgMine.go_Tap.activeSelf)
            InfoDungeonMine();
        else if (uiDgRaid.go_Tap.activeSelf)
            InfoDungeonRaid();
        else if (uiPvPBattleArena.go_Tap.activeSelf)
            InfoPvPBattleArena();
    }

    // 컨텐츠 리스트 탭으로 돌아가기 
    public void Click_ReturnContentsCell()
    {
        dgTap = DgTap.CONTENTS_CELL;
        uiContentsCell.goTap.SetActive(dgTap == DgTap.CONTENTS_CELL);
        InfoContents();
    }

    // PvP 배틀 아래나 서브탭 변경, tapNbr / 0:결투, 1:랭킹, 2:결투 기록 
    public void Click_PvPSubTapOpen(int tapNbr)
    {
        if (tapNbr == 0 && uiPvPBattleArena.goSubTapMatching.activeSelf)
            return;
        else if (tapNbr == 1 && uiPvPBattleArena.goSubTapRank.activeSelf)
            return;
        else if (tapNbr == 2 && uiPvPBattleArena.goSubTapRecord.activeSelf)
            return;

        uiPvPBattleArena.goSubTapMatching.SetActive(tapNbr == 0);
        uiPvPBattleArena.goSubTapRank.SetActive(tapNbr == 1);
        uiPvPBattleArena.goSubTapRecord.SetActive(tapNbr == 2);
        InfoPvPBattleArena();
    }
    #endregion

    /// ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
    /// 컨텐츠 : 던전 난이도 변경 및 보상 보기 
    /// ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
    #region ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
    // 도전의 탑 층 난이도 및 보상보기, fb : f-> 앞층 보기(+1), b->뒤층 보기(-1)
    public void Click_SelectFrontOrBack(int nbr_fb) => InfoDungeonTop((int)dgTopNbr + nbr_fb);

    //광산 난이도 및 보상보기 
    public void Click_SelectMineNumber(int nbr) => InfoDungeonMine(nbr);

    //레이드 난이도 및 보상보기
    public void Click_SelectRaidNumber(int nbr) => InfoDungeonRaid(nbr);
    #endregion

    /// <summary>
    /// 입장권 구매 
    /// </summary>
     #region ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
    public void Click_TicketPurchase(string dgName)
    {
        int price = GameDatabase.GetInstance().chartDB.GetDicBalance("shop.dungeon.ticket.price").val_int;
        int dailyPurMaxCnt = GameDatabase.GetInstance().chartDB.GetDicBalance(string.Format("shop.dungeon.{0}.ticket.buy.max", dgName)).val_int;
        int dailyPurCnt = string.Equals(dgName, "top") ?
            GameDatabase.GetInstance().tableDB.GetUserInfo().m_daily_buy_ticket_dg_top :
            string.Equals(dgName, "mine") ?
            GameDatabase.GetInstance().tableDB.GetUserInfo().m_daily_buy_ticket_dg_mine :
            string.Equals(dgName, "raid") ?
            GameDatabase.GetInstance().tableDB.GetUserInfo().m_daily_buy_ticket_dg_raid :
            string.Equals(dgName, "pvp") ?
            GameDatabase.GetInstance().tableDB.GetUserInfo().m_daily_buy_ticket_dg_pvp : (ObscuredInt)dailyPurMaxCnt;

        string strDgName = LanguageGameData.GetInstance().GetString(string.Format("text.dungeon.{0}", dgName));
        string Txt = string.Format("<color=#FFA500>[ {1} 입장권 x1 ]</color> 구매합니다.\n<color=#00BEFF>다이아 x10 </color>소모 / 일일 최대 구매량 ({2}/{3})", price, strDgName, dailyPurCnt, dailyPurMaxCnt);

        if (string.Equals(dgName, "top"))
        {
            if (GameDatabase.GetInstance().tableDB.GetUserInfo().m_daily_buy_ticket_dg_top < GameDatabase.GetInstance().chartDB.GetDicBalance("shop.dungeon.top.ticket.buy.max").val_int)
                PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener(Txt, APurchaseTicketTop);
            else PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("일일 구매 수량이 초과하였습니다. 다음날 출석체크 후 구매 가능합니다.");
        }
        else
        if (string.Equals(dgName, "mine"))
        {
            if (GameDatabase.GetInstance().tableDB.GetUserInfo().m_daily_buy_ticket_dg_mine < GameDatabase.GetInstance().chartDB.GetDicBalance("shop.dungeon.mine.ticket.buy.max").val_int)
                PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener(Txt, APurchaseTicketMine);
            else PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("일일 구매 수량이 초과하였습니다. 다음날 출석체크 후 구매 가능합니다.");
        }
        else
        if (string.Equals(dgName, "raid"))
        {
            if (GameDatabase.GetInstance().tableDB.GetUserInfo().m_daily_buy_ticket_dg_raid < GameDatabase.GetInstance().chartDB.GetDicBalance("shop.dungeon.raid.ticket.buy.max").val_int)
                PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener(Txt, APurchaseTicketRaid);
            else PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("일일 구매 수량이 초과하였습니다. 다음날 출석체크 후 구매 가능합니다.");
        }
        else
        if (string.Equals(dgName, "pvp"))
        {
            if (GameDatabase.GetInstance().tableDB.GetUserInfo().m_daily_buy_ticket_dg_pvp < GameDatabase.GetInstance().chartDB.GetDicBalance("shop.dungeon.pvp.ticket.buy.max").val_int)
                PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener(Txt, APurchaseTicketPvpBattleArena);
            else PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("일일 구매 수량이 초과하였습니다. 다음날 출석체크 후 구매 가능합니다.");
        }
    }

    public void APurchaseTicketTop() => APurchaseTicket("top", 23, GameDatabase.GetInstance().chartDB.GetDicBalance("shop.dungeon.top.ticket.price").val_int);
    public void APurchaseTicketMine() => APurchaseTicket("mine", 24, GameDatabase.GetInstance().chartDB.GetDicBalance("shop.dungeon.mine.ticket.price").val_int);
    public void APurchaseTicketRaid() => APurchaseTicket("raid", 25, GameDatabase.GetInstance().chartDB.GetDicBalance("shop.dungeon.raid.ticket.price").val_int);
    public void APurchaseTicketPvpBattleArena() => APurchaseTicket("pvp", 30, GameDatabase.GetInstance().chartDB.GetDicBalance("shop.dungeon.pvp.ticket.price").val_int);

    private async void APurchaseTicket(string tkName, int item_type_id, int price)
    {
        var goods_db = GameDatabase.GetInstance().tableDB.GetTableDB_Goods();
        bool isBlueDiaLack = goods_db.m_dia < price;
        int tbc = isBlueDiaLack == true ? await GameDatabase.GetInstance().tableDB.GetMyTBC() : 0;
        int blue_dia = goods_db.m_dia;
        if (blue_dia + tbc >= price)
        {
            var userInfo_db = GameDatabase.GetInstance().tableDB.GetUserInfo();
            var item_db = GameDatabase.GetInstance().tableDB.GetItem(item_type_id, 1);

            int dedDia = goods_db.m_dia -= price; // 내 현재 블루 다이아 차감
            int dedTbc = dedDia < 0 ? Math.Abs(dedDia) : 0;

            switch (tkName)
            {
                case "top":
                    userInfo_db.m_daily_buy_ticket_dg_top++;
                    break;
                case "mine":
                    userInfo_db.m_daily_buy_ticket_dg_mine++;
                    break;
                case "raid":
                    userInfo_db.m_daily_buy_ticket_dg_raid++;
                    break;
                case "pvp":
                    userInfo_db.m_daily_buy_ticket_dg_pvp++;
                    break;
            }

            item_db.count++;
            Task tsk1 = GameDatabase.GetInstance().tableDB.SetUpdateGoods(goods_db);
            Task tsk2 = GameDatabase.GetInstance().tableDB.DeductionTBC(dedTbc);
            Task tsk3 = GameDatabase.GetInstance().tableDB.SetUpdate_UserInfo(userInfo_db);
            Task tsk4 = GameDatabase.GetInstance().tableDB.SendDataItem(item_db);

            while (Loading.Bottom(tsk1.IsCompleted, tsk2.IsCompleted, tsk3.IsCompleted, tsk4.IsCompleted) == false) await Task.Delay(100);

            if (dgTap == DgTap.CONTENTS_CELL)
            {
                InfoContents();
            }
            else
            {
                if (string.Equals(tkName, "top"))
                { uiDgTop.tx_Ticket.text = GameDatabase.GetInstance().tableDB.GetStrDungeonTicket(IG.ModeType.DUNGEON_TOP); }
                else
                           if (string.Equals(tkName, "mine"))
                { uiDgMine.tx_Ticket.text = GameDatabase.GetInstance().tableDB.GetStrDungeonTicket(IG.ModeType.DUNGEON_MINE); }
                else
                           if (string.Equals(tkName, "raid"))
                { uiDgRaid.tx_Ticket.text = GameDatabase.GetInstance().tableDB.GetStrDungeonTicket(IG.ModeType.DUNGEON_RAID); }
                else
                           if (string.Equals(tkName, "pvp"))
                { uiPvPBattleArena.tx_Ticket.text = GameDatabase.GetInstance().tableDB.GetStrDungeonTicket(IG.ModeType.PVP_BATTLE_ARENA); }
            }

            string ntfMsg = string.Format(LanguageGameData.GetInstance().GetString(string.Format("str.frm.shop.item.dg.{0}.ticket.buy.success", tkName)), 1);
            PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox(ntfMsg); // [아이템] 도전의 탑 입장권 x{0}개를 구매하였습니다.
            NotificationIcon.GetInstance().CheckNoticeContentsTicket();
        }
    }
    #endregion

    /// <summary>
    /// 컨텐츠 탭에서 -> 리스트에서 던전 보상 미리보기 팝업 
    /// </summary>
    #region ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
    Sprite sprContPrev;
    public void Click_ContentsRewardPreviewSprite(GameObject goBtn)
    {
        sprContPrev = goBtn.transform.GetChild(0).GetComponent<Image>().sprite;
    }
    public void Click_ContentsRewardPreview(string rwd_name)
    {
        PopUpMng.GetInstance().Open_ContentsRewardItemInfo(sprContPrev, rwd_name);
    }

    /// <summary>
    /// 컨텐츠 탭에서 -> 던전 보상 미리보기 팝업 
    /// </summary>
    public void Click_ContentsRewardMaxEquipPiecePreview(int rt)
    {
        PopUpMng.GetInstance().Open_ContentsRewardItemInfo(sprContPrev, string.Format("equip_piece_max_rt{0}", rt));
    }
    public void Click_ContentsRewardMaxAccePiecePreview(int rt)
    {
        PopUpMng.GetInstance().Open_ContentsRewardItemInfo(sprContPrev, string.Format("acce_piece_max_rt{0}", rt));
    }
    public void Click_ContentsRewardMaxEquipEnhantStonPreview(int rt)
    {
        PopUpMng.GetInstance().Open_ContentsRewardItemInfo(sprContPrev, string.Format("equip_enhant_ston_max_rt{0}", rt));
    }
    public void Click_ContentsRewardMaxAcceEnhantStonPreview(int rt)
    {
        PopUpMng.GetInstance().Open_ContentsRewardItemInfo(sprContPrev, string.Format("acce_enhant_ston_max_rt{0}", rt));
    }
    public void Click_ContentsRewardMaxBlessPreview(int rt)
    {
        PopUpMng.GetInstance().Open_ContentsRewardItemInfo(sprContPrev, string.Format("enhant_bless_max_rt{0}", rt));
    }

    #endregion

    /// <summary>
    /// 던전 탭에서 ->  도전의 탑 , 광산, 레이드, Pvp 보상 미리보기 팝업 
    /// </summary>
    #region ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
    public void Click_DungeonRewardPreview()
    {

    }
    #endregion
}
