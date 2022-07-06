using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefsKeys
{

    public static string id = "";

    public static string prky_privacy_policy => string.Format("{0}_privacy_policy", id);

    public static string prefabKey_ChartVersion(string chart_name) => string.Format("{0}_Chart_{1}_Ver}", id, chart_name);
    /// <summary>
    /// 사용중인 매인 스킬 슬롯 0 ~ 2 중에서하나 
    /// </summary>
    public static string prefabKey_SkillUseMainSlot => id + "_pk_SkillUseMainSlot";
    public static string prefabKey_SkillSlot => id + "_pk_SkillSlot";
    public static string prky_SortInventorytHighLow_etc => id + "_SortInventorytHighLow_etc";
    public static string prky_SortInventorytHighLow_equip => id + "_SortInventorytHighLow_equip";
    public static string prky_SortInventory_etc => id + "_SortInventory_etc";
    public static string prky_SortInventory_equip => id + "_SortInventory_equip";
    /// <summary> 스테이지 반복모드 데이터 </summary>
    public static string prky_LoopChapterDb => string.Format("{0}_LoopChapterDb", id);
    /// <summary> 0:일반 진행모드, 1:보스 진행 모드 </summary>
    public static string prky_StageType = string.Format("{0}_StageType", id);
    public static string prky_LastClearSecDungeonTopNbr => id + "_LastClearSecDungeonTopNbr{0}";
    public static string prky_LastClearSecDungeonMineNbr => id + "_LastClearSecDungeonMineNbr{0}";
    public static string prky_LastClearSecDungeonRaidNbr => id + "_LastClearSecDungeonRaidNbr{0}";

    public static string prky_daily_mission(string nbr) => string.Format("{0}_daily_mission_nbr{1}", id, nbr);
    public static string prky_achievement(string nbr) => string.Format("{0}_achievement_nbr{1}", id, nbr);

    public static string prky_ChatBlockNickName => id + "_BlockNickName";

    /// <summary> PvP 배틀 아레나 랭킹로드 시간 (string -> DateTime Paser) </summary>
    public static string key_PvPArenaRankNextLoadTime => id + "PvPArenaRankNextLoadTime";
    /// <summary> PvP 배틀 아레나 매칭 상대 다음 무료 로드 시간 (string -> DateTime Paser) </summary>
    public static string key_PvPArenaMatchNextLoadTime => id + "PvPArenaMatchNextLoadTime";
    /// <summary> PvP 배틀 아레나 매칭 상대 다음 무료 로드 시간 (string -> DateTime Paser) </summary>
    public static string key_PvPArenaRecordNextLoadTime => id + "PvPArenaRecordNextLoadTime";

    /// <summary> 내가 보낸 결투 기록 </summary>
    public static string key_PvPBTLMySentRecord => id + "PvPBTLMySentRecord";
    /// <summary> 결투 승리 멘트 </summary>
    public static string key_PvPArenaComment => id + "PvPArenaComment";

    /// <summary> 결투를 이미 진행해서 승리한 상대와 다시 결투진행을 할 수 있는 시간 리스트 </summary>
    public static string key_PvPReMatchUserList => id + "PvPReMatchUserList";

    // 유저 편의 기능 
    public static string key_Convenience_OnfAutoSkill => id + "Convenience_OnfAutoSkill";   // 자동 스킬 -> 초기값 ON 
    public static string key_Convenience_OnfAutoPotion => id + "Convenience_OnfAutoPotion"; // 자동 물약 -> 초기값 OFF 
    public static string key_Convenience_OnfGameSpeed => id + "Convenience_OnfGameSpeed";   // 게임 속도 -> 초기값 OF
    public static string key_Convenience_OnfAutoSale => id + "Convenience_OnfAutoSale";     // 자동 판매 -> 초기값 OF
    public static string key_AutoPosionRating => id + "AutoPosionRating"; // 사용중인 자동 물약 등급 
    public static string key_AutoPotionUsePercent => id + "AutoPotionUsePercent"; // 물약 자동 사용 기준 
    public static string key_AutoSaleSetting => id + "AutoSaleSetting"; // 자동 판매 세팅값 

    public static string key_VideoFreeDate => id + "VideoFreeDate"; // 비디오 다이아 보상 시청 가능 
    public static string key_VideoDateAutoSale => id + "VideoDateAutoSale"; // 자동 판매 1시간 비디오 다음 시청 가능 시간 

    public static string key_VideoDateAcceFreeSoHwan => id + "VideoDateAcceFreeSoHwan"; // 장신구 무료 소환 광고 다음 시청 가능 시간 
    public static string key_VideoDateEquipFreeSoHwan => id + "VideoDateEquipFreeSoHwan"; // 장비 무료 소환 광고 다음 시청 가능 시간 

    public static string key_CombatChpterClear => id + "CombatChpterClear";
    public static string key_CombatNow => id + "CombatNow";
    public static string key_JsonStrSkillUseIdx => string.Format("{0}_JsonStrSkillUseId", id);
    public static string key_quest_completed_date(int nbr) => string.Format("{0}_quest_completed_date_nbr{1}", id, nbr);
    public static string key_quest_leve_lup_type => string.Format("{0}_level_up_type", id);

    public static string key_zb_cam_type = "zb_cam_type";
    public static string prky_OptionDb => string.Format("{0}_OptionDb", id);

    public static string prky_InventoryTapOpenSp => string.Format("{0}_InventoryTapOpenSp", id);

    public static string prky_buy_ads_re_time_rating3_potion => string.Format("{0}buy_ads_re_time_rating3_potion", id);
    public static string prky_buy_ads_re_time_gold => string.Format("{0}_buy_ads_re_time_gold", id);

    public static string prky_backend_notice_ymd => string.Format("{0}_backend_notice_ymd", id);

    public static string prky_auto_sale_info => string.Format("{0}_auto_sale_info", id);

}
