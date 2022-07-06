using Coffee.UIExtensions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IG
{
    [System.Serializable]
    public struct ImGameUIObject
    {
        public ChapterStage chapterStage;
        public DungeonTop dungeonTop;
        public DungeonMine dungeonMine;
        public DungeonRaid dungeonRaid;
        public PvpBattleArena pvpBattleArena;

        [System.Serializable]
        public class ChapterStage
        {
            public GameObject go_Root;
            public Text tx_ChapterStage, tx_MonCnt;
            public Slider sd_Progress;

            public GameObject go_BtnChptLoop, go_BtnChptCntu;
            public GameObject go_TapChptLoop;
        }

        [System.Serializable]
        public class DungeonTop
        {
            public GameObject go_Root;
            public PopUpDungeonReward pu_DungeonReward;
            public Text tx_Timer;
            public Text tx_Nbr;
            public Slider sd_pgrs;
            public GameObject[] go_ClearLabel;
        }

        [System.Serializable]
        public class DungeonMine
        {
            public GameObject go_Root;
            public PopUpDungeonReward pu_DungeonReward;
            public Text tx_Timer;
            public Text tx_Nbr;
            public Slider sd_pgrs;
            public GameObject[] go_ClearLabel;
        }

        [System.Serializable]
        public class DungeonRaid
        {
            public GameObject go_Root;
            public PopUpDungeonReward pu_DungeonReward;
            public Text tx_Timer;
            public Text tx_Nbr;
        }

        [System.Serializable]
        public class PvpBattleArena
        {
            public GameObject go_Root;
        }
    }

    #region ##### 좀비 플레이어 관련 #####
    [System.Serializable]
    public class Player
    {
        public ZombieState state;
        public IG.PartsIdx parts;
        public GameDatabase.CharacterDB.StatValue statValue;
        public EtcZbData etcZbData = new EtcZbData();
        public PlayerSkillAction playerSkillAction = new PlayerSkillAction();
        public List<SkillNumber> activateSkills = new List<SkillNumber>(); // 활성중인 스킬들 
        public Dictionary<int, int> activateExtended = new Dictionary<int, int>();
        public List<IG.SkillNumber> activeSkillCancel = new List<SkillNumber>(); // 활성중인 스킬 캔슬 시킬때 사용
        public Transform parent;
        public Transform respwanPos;
    }

    public struct EtcZbData
    {
        public int attack_count; // 공격한 횟수 
        public int last_attack_count; // 마지막 공격 횟수 
        public long last_taken_damage; // 상대로 부터 받은 마지막 대미지 
        public float accuracy_correction; //명중률 보정값 
    }

    /// <summary>
    /// 좀비의 현재 상태
    /// </summary>
    public enum ZombieState
    {
        Ready,
        MOVE_POS, // 파이트 위치로 이동중
        FIGHT, // 파이트 중 
        LOSER, // 패배자 
        WINNER, // 승리자 
    }
    #endregion 

    #region ##### 장비 파츠 #####
    [System.Serializable]
    public struct PartsIdx
    {
        public int ty0_weapon_rt, ty1_shield_rt, ty2_helmet_rt, ty3_shoulder_r_rt, ty3_shoulder_l_rt, ty4_armor_rt, ty5_arm_rt, ty6_pants_rt, ty7_boots_rt;   // 파츠 등급 
        public int ty0_weapon_id, ty1_shield_id, ty2_helmet_id, ty3_shoulder_r_id, ty3_shoulder_l_id, ty4_armor_id, ty5_arm_id, ty6_pants_id, ty7_boots_id;   // 파츠 번호 
    }

    [System.Serializable]
    public class Parts
    {
        [SerializeField] string header;
        public int partyType;
        public string partsName;
        public List<PartsArray> parArray = new List<PartsArray>();
    }
    [System.Serializable]
    public struct PartsArray
    {
        [SerializeField] string header;
        public List<PartsObj> partObj;
    }

    [System.Serializable]
    public struct PartsObj
    {
        public int idx;
        public GameObject mesh;
    }
    #endregion

    #region ##### 스킬 #####
    [System.Serializable]
    public class PlayerSkill
    {
        public int readyActiveSkillSlot = 0; // 발동 준비중인 스킬 슬롯 번호 
        public int nowBubbleCount; // 현재 스킬 버블 카운트 
        public Skill[] useSkill = new Skill[6];
        [System.Serializable]
        public struct Skill
        {
            public SkillActiveState activeState; // 스킬 발동 상태 
            public SkillNumber number; // 스킬 IDX 
            public SkillStat stat;
            public cdb_stat_skill chart;
        }
    }

    // 스킬 스탯 
    [System.Serializable]
    public struct SkillStat
    {
        public int skLv;
        public int atvBubCount; // 스킬 발동이 가능한 버블 갯수 
    }

    // 스킬 번호 
    public enum SkillNumber
    {
        EMPTY = 0,
        NUMBER_1,
        NUMBER_2,
        NUMBER_3,
        NUMBER_4,
        NUMBER_5,
        NUMBER_6,
        NUMBER_7,
        NUMBER_8,
        NUMBER_9,
        NUMBER_10,
        NUMBER_11,
        NUMBER_12,
        NUMBER_13,
        NUMBER_14,
        NUMBER_15,
        NUMBER_16,
        NUMBER_17,
        NUMBER_18,
        NUMBER_19,
        NUMBER_20,
        NUMBER_21,
        NUMBER_22,
        NUMBER_23,
        NUMBER_24,
        NUMBER_25,
        NUMBER_26,
    }

    // 스킬 발동 상태 
    public enum SkillActiveState
    {
        WAITING, // 대기중 
        READY_ACTIVE, // 발동 준비 
        ACTION_SKILL, // 스킬 액션중 
        ACTIVE_END, // 발동 완료 
    }
    #endregion

    #region ##### 게임 모드 #####
    [System.Serializable]
    public enum ModeType
    {
        //GAME_PAUSE, //일시 중지 
        CHANGE_WAIT,
        
        CHAPTER_CONTINUE,   // 챕터 진행 
        CHAPTER_LOOP,       // 챕터 루프(파밍) 
        
        DUNGEON_TOP,        // 도전의 탑 
        DUNGEON_MINE,       // 광산 
        DUNGEON_RAID,       // 레이드 
        //DUNGEON_EQUIP,    // 장비 던전 

        PVP_BATTLE_ARENA,         // PVP 배틀 
    }

    /// <summary>
    /// 스테이지 진행 타입 일반진행 or 보스만 진행 
    /// </summary>
    [System.Serializable]
    public enum StageType
    {
        NORMAL_MONSTER,
        BOSS_MONSTER
    }

    [System.Serializable]
    public enum MonsterType
    {
        NONE,
        MINE, // 플레이어 자신 
        NORM_MONSTER, // 일반 몬스터 
        BOSS_MONSTER, // 보스 몬스터
        BOSS_USER, // 보스 유저
        PVP_USER, // Pvp 배틀 아레나 상대 유저 
        NORM_DGN_MONSTER, // 던전 일반 몬스터 
        BOSS_DGN_MONSTER, // 던전 보스 몬스터 
    }
    #endregion

    #region 편의 기능 ON / OFF
    [System.Serializable]
    public struct ConvenienceFunction
    {
        public ConvenienceAutoSkill cfAutoSkill;        // 자동 스킬 -> 초기값 ON 
        public ConvenienceAutoAutoPosion cfAutoPosion;  // 자동 물약 -> 초기값 OFF 
        public ConvenienceGameSpeed cfGameSpeed;        // 게임 속도 -> 초기값 OFF 
        public ConvenienceAutoSale cfAutoSale;          // 장비 자동 판매 

        [System.Serializable]
        public struct ConvenienceAutoSkill
        {
            public OnOff onOff;
            public Image imBtnBg;
            public Text txBtn;
        }

        [System.Serializable]
        public struct ConvenienceAutoAutoPosion
        {
            public OnOff onOff;
            public Image imBtnBg;
            public Text txBtn;
            public GameObject onUIShiny;
            public Text txOnCnt;
            public Text txReUseTime;
            public GameObject goOnObject;
            public GameObject goNotUse; // 사용 금지 

            public float recoPcr; // 회복량 15% 30% 60%
            public float usePcr; // 물약 사용 기준 30% 50% 70%
            public int iUseRating; // 자동으로 사용중인 물약 등급 
            public DateTime reUseDate; // 재사용 시간 
        }

        [System.Serializable]
        public struct ConvenienceGameSpeed
        {
            public OnOffSpeed onOffSpeed;
            public Image imBtnBg;
            public Text txBtn;

            public enum OnOffSpeed
            {
                OFF = 0,
                ONx2 = 1,
                ONx3 = 2
            }
        }

        [System.Serializable]
        public struct ConvenienceAutoSale
        {
            public OnOff onOff;
            public Image imBtnBg;
            public Text txBtn;
            public GameObject onUIShiny;

            public bool isPremanent;
            public DateTime endDate;
            public Text txEndDate;

            public SaleSetting saleSetting;
            [System.Serializable]
            public struct SaleSetting
            {
                public bool[] saleEquipType;
                public bool[] saleEquipRating;
                public bool[] saleAcceRating;
                public bool[] saleOrDecomp;
            }
        }

        public enum OnOff
        {
            OFF,
            ON
        }
    }

   
    #endregion
}


/// <summary>
/// 인벤토리 정렬 
/// </summary>
public enum SortInventorytHighLow
{
    HIGH_TO_LOW = 0,
    LOW_TO_HIGH = 1
}

public enum SortInventory
{
    NEW = 0,    // 신규
    RATING = 1, // 등급
    TYPE = 2,   // 종류(부위) 순서 장비,아이템 순 
    NORMAL_LEVEL = 3,  // 일반 레벨
    COMBAT = 4, // 전투력 
    ENHANT_LEVEL = 5, // 강화 레벨 
}