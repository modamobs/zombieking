using Coffee.UIExtensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct EquipViewUI
{
    public GameObject go_Root;
    public Image image_Icon;
    public Image image_iconRatingBg;
    public GameObject go_IconLock;
    public UIGradient uigd_LineEffect;
    public Text text_RatingOL, text_Rating_;
    public Text text_LevelOL, text_Level;
    public Text text_Name;
    public Text text_AllCombatPower;        // 전투력 총합

    public Text text_MainStatCombatPower;   // 매인스탯 전투력 
    public Text text_MainStatVal;           // 매인 스탯 값 
    public Text text_MainStatName;          // 매인 스탯 이름 

    public Text text_OpCombatPower;         // 옵션 스탯 전투력 
    public Text[] text_OpStatVal;           // 옵션 스탯 값 
    public Text[] text_OpStatName_select;    // 옵션 스탯 이름 

    public Text text_AcceSpOpCombatPower;   // 장신구 전용 옵션 전투력 
    public Text text_AccSpOpStatName;       // 장신구 전용 옵션 이름
    public Text text_AccSpOpStatVal;        // 장신구 전용 옵션 값 

    public Image image_LockBtn;
    public Text text_LockReTime; // 잠금 기능 재시간 

    //                장착           해제           강화            강화 이전                옵션 스탯 변경 
    public GameObject go_BtnWearing, go_BtnRelease, go_BtnUpLevel_, go_BtnLevelTransmission, go_BtnChangeOpStat;
    public GameObject go_RootAcceSpOption; // 장신구 전용 옵션 root 
}

[System.Serializable]
public struct EquipmentIconUI
{
    public Image eqIcon;            // 장비의 아이콘 
    public Image eqRatingBg;        // 장비의 등급 배경
    public Text eqRating;           // 장비의 등급 
    public Text eqRatingOutLine;    // 장비의 등급 라인 
    public Text eqEnhantLevel;      // 장비의 강화 레벨 
    public Text eqName;             // 장비의 이름 
    public GameObject eqLabel;      // 신규, 장착, 등 
    public GameObject eqLock;       // 장금 상태 아이콘 
}

[System.Serializable]
public struct EquipmentStatUI
{
    public Text text_TotalCombatPower; // 전투력 총합 
    public Text text_StatCombatPower; // 스탯 + 전투력 
    public Text text_StatName; // 스탯 이름
    public Text text_StatValue; // 스탯 값 
    public Text text_OptionStatCombatPower; // 옵션 + 전투력 
    public Text[] text_OptionNames; // 옵션 이름 
    public Text[] text_OptionValues; // 옵션 값 

    public Text text_AcceSpOptionCombatPower; // 장신구 전용 옵션 전투력 
    public Text text_AcceSpOptionName; // 장신구 전용 옵션 이름
    public Text text_AcceSpOptionValue; // 장신구 전용 옵션 값 
}

[System.Serializable]
public struct SkillIconUI
{
    public Image image_Icon; // 아이콘 
    public Image image_RatingOutLine; // 등급 아웃 라인 컬러 
    public Text text_Rating; // 등급 
    public Text text_Level; // 레벨 
    public Text text_CountAndUpCount; // 현재수량 / 업 수량 
    public Image image_CountFillAmount; // 스킬 레벨업 Fill Amount 
    public Image[] image_sPoint; // 스킬 포인트 1~3 
    public Text text_LockScript; // 잠김 이유 
    public CanvasGroup cngpLock;
    public Image imLock;
    public GameObject go_Empty;
    public GameObject go_Root;
}

[System.Serializable]
public struct SkillInfoIconUI
{
    public Image image_icon;
    public Image image_BgRating; // 등급 배경 
    public Text text_Rating; // 등급 
    public Text text_RatingOL; // 등급 아웃 라인 
    public Text text_Level; // 레벨 및 카운트 
    public Text text_LevelOL;
    public Text text_Name;
    public Text text_Sub;
    public Image[] image_sPoint; // 스킬 포인트 1~3 
}

/// <summary>
/// 스킬 슬롯 정보 
/// </summary>
[System.Serializable]
public struct PrefabSkillSlot
{
    public int main_slot_num;
    public int use; // 0:미사용, 1:사용
    public Slot[] slot;
    [System.Serializable]
    public struct Slot
    {
        public long aInUid;
    }
}


/// <summary>
/// 인 게임 배틀 UI 
/// </summary>
[System.Serializable]
public class BattleInfo
{
    public Text text_Name; // 유저(몬스터) 네임 
    public Text text_HpValue; // 체력 값 
    public Image image_HpFillAmount, image_HpBgFillAmount; // 체력 바 
    public Animator[] anis_Point;
    public GameObject[] gos_OnType0_PointEff;
    public GameObject[] gos_OnType1_PointEff;
    public GameObject[] gos_OnType2_PointEff;
    public GameObject goCardRoot;
    public Color cardOnColor, cardOffColor;
    public CardsSkillInfo[] cardsSkillInfo;
    public List<BuffAndDebuff> buffAndDebuff;
    public ActiveSkilLabel activeSkilLabel;

    [System.Serializable]
    public class CardsSkillInfo
    {
        public Image image_SkillIcon;
        public Image image_RatingOL;
        public Text text_Rating;
        public Text text_Level;
        public Animator[] anis_Point;
        public Image[] im_Point;
        public GameObject[] gos_PointOnEff;
        public Animator anitor;
        public GameObject go_ActiveEffect;
        public GameObject go_RandomLabel;
    }

    [System.Serializable]
    public class BuffAndDebuff
    {
        public Value val;
        [System.Serializable]
        public struct Value
        {
            public int i_skill_idx;
            public int i_end_atk_atv_cnt;
            public long l_set_time; // 발동 second 
            public string bdf_type; // 버프or디버프 
            public string owner; // 스킬 시전자 
            public string chk_bdf_atcker; // 적용 대상의 공격 카운트 동안 
        }

        public GObject go;
        [System.Serializable]
        public struct GObject
        {
            public GameObject go_Root;
            public Image image_sk_icon;
            public Text text_atk_cnt_OL;
            public Text text_atk_cnt;
            public Image image_BufDbuf_OL;
            public Image image_BdfAttacker;
        }
    }

    // 스킬 발동시 라벨 
    [System.Serializable]
    public struct ActiveSkilLabel
    {
        public GameObject go_Root;
        public Animation ani;
        public Text txEng;
        public Text txInfo;
    }
}

// 밸런스 차트 
public struct Balance
{
    public string balance_id;
    public long val_long;
    public int val_int;
    public List<int> val_int_array;
    public int val_int_idx;
    public int val_int_level;
    public float val_float;
    public List<float> val_float_array;
    public float val_float_second;
    public float val_float_percent;
    public string val_string;
}

// 전용 옵션 : 장신구
public struct cdb_stat_special_Acce
{
    public int eRating, eIdx;
    public float min_sop_id1,   max_sop_id1;
    public float min_sop_id2,   max_sop_id2;
    public float min_sop_id3,   max_sop_id3;
    public float min_sop_id4,   max_sop_id4;
    public float min_sop_id5,   max_sop_id5;
    public float min_sop_id6,   max_sop_id6;
    public float min_sop_id7,   max_sop_id7;
    public float min_sop_id8,   max_sop_id8;
    public float min_sop_id9,   max_sop_id9;
    public float min_sop_id10,  max_sop_id10;
}


// 스킬 스탯 차트 db 
public struct cdb_stat_skill
{
    public int s_idx; // 번호 
    public int s_type; // 타입 30 고정  
    public int s_rating; // 등급 
    public int s_pnt; // 발동 포인트 
    public int s_lck_opn_dgTopNbr; // 도전의 탑 클리어시 스킬 오픈 

    public string s_bdf_type;
    public string s_chk_bdf_atcker;
    public int atk_atv_cnt; // 스킬 발동시 유지되는 공격 카운트 
    public string s_atk_cnt_check; // 공격 카운트 체크 (공격 전 : bf, 공격 후 : af)
    public bool is_atv_hit_dmg; // 스킬 발동시 공격 대미지를 주는 가 
    public bool is_atv_end_hit; // 스킬 끝날 시점에 대미지를 주는가 
    public bool is_atk_range; // 랜덤한 대미지를 주는 스킬인가 
    public float f_mtp_val1;
    public float f_mtp_val2;
    public float f_mtp_val3;
    public float f_mtp_pow1;
    public float f_mtp_pow2;
    public float f_mtp_pow3;
}

/// <summary> 업적, 미션(일일) 차트 db </summary>
#if UNITY_EDITOR
[System.Serializable]
#endif
public class cdb_achievements
{
    public int nbr;
    public int lv;
    public string name;
    public long prog_cmp_cnt; // 완료 카운트 
    public int prog_cnt_type; // 진행 카운트 %로 표시 => 0, 일반 수 => 1
    public bool prog_tap_move;// 진행탭으로 이동 가능 
    public bool rwd_reset; // 보상 후 업적 진행 카운트 리셋 
    public GameDatabase.MailDB.Item item;
}

/// <summary> 퀘스트 차트 db </summary>
#if UNITY_EDITOR
[System.Serializable]
#endif
public class cdb_quest
{
    public int nbr;
    public float rwd_power;
    public float up_power;
    public int max_lv;
    public string title;
    public int rwd_time;
    public int rwd_gold;
    public long up_gold;

    public int eq_rt;
    public int eq_id;
    public int qst_mnst_drop_gold;
    public int qst_eq_sale_gold;
    public int qst_eq_upgrade_gold;

    // 장비 강화 
    public int qst_eq_enhant_gold;
    public int qst_eq_enhant_ruby;
    public int qst_eq_ac_enhant_ether;

    // 강화석 진화 
    public int qst_ston_evol_gold;

    // 강화 레벨 전승 
    public long qst_eq_enhant_transfer_gold;
    public int qst_eq_enhant_transfer_dia;

    // 장신구 옵션 변경 
    public long qst_eq_ac_op_change_gold;
    public int qst_eq_ac_op_change_dia;

    // 전설 진화 장비 옵션 변경 다이아 
    public long qst_eq_op_change_gold;
    public int qst_eq_sop_change_dia;
    public int qst_eq_op_change_dia;

    // 장신구 합성 
    public long qst_eq_ac_synt_gold;
    public int qst_eq_ac_synt_tbc_dia;

    // 스킬 레벨업 
    public long qst_skill_up_gold;

    // 판매 or 분해 
    public int qst_eq_decomp_ruby;
    public int qst_eq_ac_decomp_ether;
}

/// <summary> 오프라인 보상  </summary>
#if UNITY_EDITOR
[System.Serializable]
#endif
public struct cdb_offline_reward 
{
    public int hour;
    public float qst_gold;
    public int ruby;
    public int ether;
    public int piece_equip_rt5;
    public int piece_equip_rt6;
    public int piece_equip_rt7;
    public int piece_acce_rt5;
}

/// <summary> 펫 정보 및 옵션 </summary>
#if UNITY_EDITOR
[System.Serializable]
#endif
public struct cdb_pet
{
    public int p_rt;
    public int p_id;
    public string name;
    public int max_lv;

    public float op1v_min, op1v_max;
    public float op2v_min, op2v_max;
    public float op4v_min, op4v_max;
    public float op5v_min, op5v_max;
    public float op8v_min, op8v_max;
}


/// <summary>
/// 펫 옵션 스탯 종합 수치 
/// </summary>
public struct PetOpStTotalFigures
{
    public float op1v;
    public float op2v;
    public float op4v;
    public float op5v;
    public float op8v;
}

/// <summary> 펫 고유 옵션  </summary>
#if UNITY_EDITOR
[System.Serializable]
#endif
public struct cdb_pet_sop
{
    public int r_pct;
    public int sop_id;
    public string sop_name;

    public int p_rt, p_id;

    // 고유 옵션 별 최소 ~ 최대치 값 
    public float sop1v_min, sop1v_max;
    public float sop2v_min, sop2v_max;
    public float sop3v_min, sop3v_max;
    public float sop4v_min, sop4v_max;
    public float sop5v_min, sop5v_max;
    public float sop6v_min, sop6v_max;
    public float sop7v_min, sop7v_max;
    public float sop8v_min, sop8v_max;
    public float sop9v_min, sop9v_max;
}

/// <summary>
/// 펫 전용 옵션 스탯 수치 
/// </summary>
#if UNITY_EDITOR
[System.Serializable]
#endif
public struct PetSpOpTotalFigures
{
    public float sop1_value;
    public float sop2_value;
    public float sop3_value;
    public float sop4_value;
    public float sop5_value;
    public float sop6_value;
    public float sop7_value;
    public float sop8_value;
    public float sop9_value;
}

/// <summary> 몬스터 스탯 (일반 필드 스테이지 기준) </summary>
#if UNITY_EDITOR
[System.Serializable]
#endif
public struct cdb_chpt_mnst_stat   // ###### 무기 - 매인 스탯 
{
    /// <summary> 10 /10 = 1 -> chpt_divi_nbr의 1번 값 </summary>
    public int chpt_dvs_nbr;
    public int eq_rat;
    public int eq_id;

    public int norm_lv_pow;
    public int chpt_norm_lv;        // 일반 레벨 
    public int chpt_ehnt_lv;        // 강화 레벨(챕터 스테이지 전용)
    public int chpt_m_st_rlv;       // 매인 스탯 랜덤 레벨 

    public int chpt_id; // 챕터 번호 
    public int stg_id; // 스테이지 번호 
    public int map_nbr; // 맵 번호 
    public int loop_pnt;
    public int min_loop_pnt;
    public int max_loop_pnt;
};

#region ##### 던전 #####
// 도전의 탑, 몬스터 정보
public struct cdb_dungeon_top
{
    public int nbr;  // 도전의 탑 number 
    public int eq_rt;
    public int eq_id;
    public int dg_top_norm_lv;      // 일반 레벨 
    public int dg_top_ehnt_lv;      // 강화 레벨(도전의 탑 전용)
    public int dg_top_m_st_rlv;     // 매인 스탯 랜덤 레벨 
    public int dg_top_op_st_rlv;    // 옵션 스탯 랜덤 레벨 

    public DungeonReward reward;
}

// 광산, 몬스터 정보
public struct cdb_dungeon_mine
{
    public int nbr; // 광산 number 
    public int eq_rt;
    public int eq_id;
    public int dg_mine_norm_lv;     // 일반 레벨 
    public int dg_mine_ehnt_lv;     // 강화 레벨(광산 전용)
    public int dg_mine_m_st_rlv;    // 매인 스탯 랜덤 레벨 
    public int dg_mine_op_st_rlv;   // 옵션 스탯 랜덤 레벨 

    public DungeonReward reward;
}

// 레이드, 몬스터 정보
public struct cdb_dungeon_raid
{
    public int nbr; // 레이드 number 
    public int eq_rt;
    public int eq_id;
    public int dg_raid_norm_lv;     // 일반 레벨 
    public int dg_raid_ehnt_lv;     // 강화 레벨(레이드 전용) 
    public int dg_raid_m_st_rlv;    // 매인 스탯 랜덤 레벨 
    public int dg_raid_op_st_rlv;   // 옵션 스탯 랜덤 레벨 

    public DungeonReward reward;
}

// 보상 
public class DungeonReward
{
    // 골드 
    public float qst_gold;

    // 장비 
    public int rw_eq_ty, rw_eq_rt, rw_eq_id;

    // 장신구 
    public int rw_eqac_ty, rw_eqac_rt, rw_eqac_id;

    // 스킬 
    public int rw_sk_id, rw_sk_cnt;

    // 장비 조각(고정)
    public int rw_eq_pce_rt, rw_eq_pce_cnt;
    // 장신구 조각(고정)
    public int rw_eqac_pce_rt, rw_eqac_pce_cnt;

    // 장비/장신구 조각 (다중 리스트)
    public List<EquipPiece> rw_eq_pces = new List<EquipPiece>();
    public List<EquipPiece> rw_eqac_pces = new List<EquipPiece>();
    [System.Serializable]
    public struct EquipPiece
    {
        public string rnk; // s, a, b, c 
        public List<Rwd> rwds;
        [System.Serializable]
        public struct Rwd
        {
            public int rt, cnt;
        }
    }

    // 장비/장신구 강화석 
    public List<EnhantSton> rw_eq_ehnt_ston = new List<EnhantSton>();
    public List<EnhantSton> rw_eqac_ehnt_ston = new List<EnhantSton>();
    [System.Serializable]
    public struct EnhantSton
    {
        public string rnk; // s, a, b, c 
        public List<Rwd> rwds;
        [System.Serializable]
        public struct Rwd
        {
            public int rt, cnt;
        }
    }

    // 강화 축복 주문서 
    public int rw_ehnt_bless_rt;
    public int rw_ehnt_bless_cnt;

    // 장비 등급 확률 pct 확률, eq_rt 등급, eq_id 번호 
    public List<EquipRatingPercent> rw_pct_eq = new List<EquipRatingPercent>();
    public List<EquipRatingPercent> rw_pct_eq_ac = new List<EquipRatingPercent>();
    [System.Serializable]
    public struct EquipRatingPercent
    {
        public float pct;
        public List<Rwd> rwds;
        [System.Serializable]
        public struct Rwd
        {
            public int eq_rt, eq_id;
        }
    }

    // 에테르 (장신구 뽑기용) -> 고대 장신구까지 획득가능한 뽑기 시스템에 사용 
    public EquipAcCrystal rw_equip_ac_crystal;
    public struct EquipAcCrystal
    {
        public int rw_goods_ether_cnt;
    }
}


#endregion

/// <summary>
/// 몬스터 처치시 드롭될 등급 
/// </summary>
public struct cdb_r_chapter_drop_rating
{
    public int chpt_id;
    public float drop_rt1;
    public float drop_rt2;
    public float drop_rt3;
    public float drop_rt4;
    public float drop_rt5;
    public float drop_rt6;
    public float drop_rt7;
}

public class cdb_r_chapter_equip_drop_result
{
    public float chpt_pct;
    public int ty, rt, id;
}

public struct cdb_r_chapter_item_drop_result
{
    public float pct;
    public int it_ty, it_rt, it_cn;
}

public struct cdb_gacha_percentage
{
    public string gch_name;
    public float rt1, rt2, rt3, rt4, rt5, rt6, rt7;
}

/// <summary>
/// 장신구 전용 옵션 랜덤 
/// </summary>
public struct cdb_acce_special_op
{
    public float pct;
    public int ac_sop_id;
}

/// <summary>
/// 장비(장신구) 옵션
/// </summary>
public struct cdb_equip_op
{
    public int pt_ty;
    public string op1;
    public string op2;
    public string op3;
    public string op4;
}

/// 
/// <summary>
/// 구글 클라우드에 저장될 데이터 
/// GpgsCloudData
/// </summary>
[System.Serializable]
public struct InvenBackUp
{
    public string in_date;
    public InvenData inven_data;
}


[System.Serializable]
public struct InvenData
{
    public List<GameDatabase.TableDB.Equipment> equips;
    public List<GameDatabase.TableDB.Pet> pets;
    public List<GameDatabase.TableDB.PetEncy> petsEncy;
}

[System.Serializable]
public class TapObject
{
    public GameObject goOutline;
    public Animation aniIcon;
    public Text txName;
    public Color onCorSelect, noCorSelect;
}

[System.Serializable]
public class HomeOptionButtons
{
    public Animation ani;
    public Image imIcon;
    public Text txOnOff;
}

[System.Serializable]
public class StageTypeUI
{
    public Image imBtnBg;
    public Text txt;
}

// 결과 값
[System.Serializable]
public struct AcceResult
{
    public long tempUID;
    public bool isRtFixed; // 고정 등급 뽑기로 나온 결과인가 
    public int ac_type, ac_rt, ac_id;
}