using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class SpriteAtlasMng : MonoSingleton<SpriteAtlasMng>
{
    public SpriteAtlas mAtlas_equip_skill;
    public Texture[] texSkill;

    public Sprite GetTransparency() => mAtlas_equip_skill.GetSprite("Transparency");
    public Sprite GetSpriteEquip(int eq_ty, int eq_rt, int eq_id)
    {
        switch (eq_ty)
        {
            case 0: return mAtlas_equip_skill.GetSprite(string.Format("eq_weapon_{0}_{1}", eq_rt, eq_id));
            case 1: return mAtlas_equip_skill.GetSprite(string.Format("eq_shield_{0}_{1}", eq_rt, eq_id));
            case 2: return mAtlas_equip_skill.GetSprite(string.Format("eq_helmet_{0}_{1}", eq_rt, eq_id));
            case 3: return mAtlas_equip_skill.GetSprite(string.Format("eq_shoulder_{0}_{1}", eq_rt, eq_id));
            case 4: return mAtlas_equip_skill.GetSprite(string.Format("eq_armor_{0}_{1}", eq_rt, eq_id));
            case 5: return mAtlas_equip_skill.GetSprite(string.Format("eq_arm_{0}_{1}", eq_rt, eq_id));
            case 6: return mAtlas_equip_skill.GetSprite(string.Format("eq_pants_{0}_{1}", eq_rt, eq_id));
            case 7: return mAtlas_equip_skill.GetSprite(string.Format("eq_boots_{0}_{1}", eq_rt, eq_id));
            case 8: return mAtlas_equip_skill.GetSprite(string.Format("ac_necklace_{0}_{1}", eq_rt, eq_id));
            case 9: return mAtlas_equip_skill.GetSprite(string.Format("ac_earring_{0}_{1}", eq_rt, eq_id));
            case 10: return mAtlas_equip_skill.GetSprite(string.Format("ac_ring_{0}_{1}", eq_rt, eq_id));
        }

        return GetTransparency();
    }

    public Sprite GetItemSprite(int ty, int rt = 0)
    {
        switch (ty)
        {
            case 20: return mAtlas_equip_skill.GetSprite(string.Format("it_{0}_{1}_potion", ty, rt)); // 물약 
            case 21: return mAtlas_equip_skill.GetSprite(string.Format("it_{0}_{1}_equip_enchant_stone", ty, rt)); // 장비 강화석 
            case 22: return mAtlas_equip_skill.GetSprite(string.Format("it_{0}_{1}_enchant_blessing", ty, rt)); // 강화 축복  
            case 23: return mAtlas_equip_skill.GetSprite(string.Format("it_{0}_{1}_ticket_dungeon_top", ty, rt)); // 도전 TOP 입장권 
            case 24: return mAtlas_equip_skill.GetSprite(string.Format("it_{0}_{1}_ticket_mine", ty, rt)); // 광산 입장권 
            case 25: return mAtlas_equip_skill.GetSprite(string.Format("it_{0}_{1}_ticket_raid", ty, rt)); // 레이드 입장권  
            case 26: return mAtlas_equip_skill.GetSprite(string.Format("it_{0}_{1}_resurrectionStone", ty, rt)); // 부활석 
            case 27: return mAtlas_equip_skill.GetSprite(string.Format("it_{0}_{1}_acce_enchant_stone", ty, rt)); // 장신구 강화석 
            case 28: return mAtlas_equip_skill.GetSprite(string.Format("it_{0}_{1}_equip_piece", ty, rt)); // 장비 조각 
            case 29: return mAtlas_equip_skill.GetSprite(string.Format("it_{0}_{1}_equip_ac_piece", ty, rt)); // 장신구 조각 
            case 30: return mAtlas_equip_skill.GetSprite(string.Format("it_{0}_{1}_ticket_pvp_arena", ty, rt)); // Pvp 배틀 아레나 입장권 
            case 31: return mAtlas_equip_skill.GetSprite(string.Format("it_{0}_{1}_pet_al", ty, rt)); // 펫 소환권

        }
        return GetTransparency();
    }
    
    public Sprite GetMailUseItemSprite(int ty, int rt = 0)
    {
        switch (ty)
        {
            case 31: return mAtlas_equip_skill.GetSprite("auto_sale_card"); // 장비 판매(분해) 1일 이용권 
        }

        return GetTransparency();
    }

    public Sprite GetPvpRewarCellIconSprite(string pvp_icon_name)
    {
        switch (pvp_icon_name)
        {
            case "win_score":
            case "lose_score":
                return mAtlas_equip_skill.GetSprite("battle_score"); // 점수 
            case "battle_coin":
                return mAtlas_equip_skill.GetSprite("goods_battle_coin"); // 배틀 코인 
        }

        return GetTransparency();
    }

    // 스텟 아이콘 
    public Sprite GetSpriteStatIcon(int st_id) => mAtlas_equip_skill.GetSprite(string.Format("stat{0}", st_id));
    public Sprite GetSpriteEquipMainStatIcon(int parts_ty) => mAtlas_equip_skill.GetSprite(string.Format("stat{0}", GameDatabase.GetInstance().tableDB.GetPartyMainStatId(parts_ty)));
    // 장신구 전용 옵션 아이콘 
    public Sprite GetSpriteAcceSpOpStatIcon(int ac_st_id) => mAtlas_equip_skill.GetSprite(string.Format("acce_stat{0}", ac_st_id));


    public Sprite GetSpriteItemStonType(int eq_ty, int st_rt)
    {
        bool isAcc = GameDatabase.GetInstance().tableDB.GetIsPartsTypeAcce(eq_ty);
        if (isAcc)
            return mAtlas_equip_skill.GetSprite(string.Format("it_27_{0}_acce_enchant_stone", st_rt)); // 장신구 강화석 
        else
            return mAtlas_equip_skill.GetSprite(string.Format("it_21_{0}_equip_enchant_stone", st_rt)); // 장비 강화석 
    }

    public Sprite GetSpriteSkill(int sk_idx) => mAtlas_equip_skill.GetSprite(string.Format("icon_skill_{0}", sk_idx));
    public Sprite GetSprite(string spr_name) => mAtlas_equip_skill.GetSprite(spr_name);

    /// <summary>
    /// 잠금 
    /// </summary>
    public Sprite GetSpriteEquipLock(int state_lock)
    {
        return mAtlas_equip_skill.GetSprite(string.Format("lock_{0}", state_lock));
    }

    /// <summary>
    /// 등급 배경 
    /// </summary>
    public Sprite GetSpriteRatingBg(int rating, int rt7_lengend = 0)
    {
        if (rating == 0)
            return mAtlas_equip_skill.GetSprite("rating_bg_0");

        if(rt7_lengend == 1)
        {
            return mAtlas_equip_skill.GetSprite(string.Format("rating_bg_{0}_{1}", rating, rt7_lengend));
        }
        else
        {
            return mAtlas_equip_skill.GetSprite(string.Format("rating_bg_{0}", rating));
        }
    }

    public Sprite GetSpriteButtonRedOrGray(bool isRed)
    {
        if (isRed)
            return mAtlas_equip_skill.GetSprite("btn_box_red");
        else
            return mAtlas_equip_skill.GetSprite("gray_box");
    }
    

    /// <summary>
    /// corName -> blue, green, purple, red, yellow, gray
    /// </summary>
    public Sprite GetSpriteButtonBox(string corName) => mAtlas_equip_skill.GetSprite(string.Format("btn_box_{0}", corName));

    public Sprite GetSpriteGoods(int ty)
    {
        switch (ty)
        {
            case 10: return GetSpriteGold();
            case 11: return GetSpriteDia();
            case 12: return GetSpriteEther();
            case 13: return GetSpriteRuby();
        }

        return GetTransparency();
    }

    public Sprite GetPetIcon(int rt, int id) => mAtlas_equip_skill.GetSprite(string.Format("pet-{0}-{1}", rt, id));

    public Sprite GetSpriteCancelBtn(bool isColor) => isColor == true ? mAtlas_equip_skill.GetSprite("cancel") : mAtlas_equip_skill.GetSprite("cancel-gray");

    public Sprite GetSpriteGold() => mAtlas_equip_skill.GetSprite("goods_gold");
    public Sprite GetSpriteDia() => mAtlas_equip_skill.GetSprite("goods_blue_dia");
    public Sprite GetSpriteTBC() => mAtlas_equip_skill.GetSprite("goods_white_dia_TBC");
    public Sprite GetSpriteEther() => mAtlas_equip_skill.GetSprite("goods_ether");// 에테르 : 장신구 뽑기에 사용됨 
    public Sprite GetSpriteRuby() => mAtlas_equip_skill.GetSprite("goods_ruby"); // 루비 : 무기,방패,방어구 뽑기에 사용됨 
    public Sprite GetSpriteVideo() => mAtlas_equip_skill.GetSprite("video-play");
    public Sprite GetSpriteTypeEquipAll() => mAtlas_equip_skill.GetSprite("type_equip_all");
    public Sprite GetSpriteTypeEquipWeaponShield() => mAtlas_equip_skill.GetSprite("type_equip_wpn_shd");
    public Sprite GetSpriteTypeEquipArmor() => mAtlas_equip_skill.GetSprite("type_equip_armor");
    public Sprite GetSpriteTypeEquipAcce() => mAtlas_equip_skill.GetSprite("type_equip_acce");

    /// <summary> 오프라인 보상  </summary>
    public Sprite GetOfflineRewardIcon(string rwd_name)
    {
        switch(rwd_name)
        {
            case "qst_gold": return GetSpriteGold();
            case "ruby": return GetSpriteRuby();
            case "ether": return GetSpriteEther();
            case "piece_equip_rt5": return mAtlas_equip_skill.GetSprite("it_28_5_equip_piece");
            case "piece_equip_rt6": return mAtlas_equip_skill.GetSprite("it_28_6_equip_piece");
            case "piece_equip_rt7": return mAtlas_equip_skill.GetSprite("it_28_7_equip_piece");
            case "piece_acce_rt5": return mAtlas_equip_skill.GetSprite("it_29_5_equip_ac_piece");
        }

        return null;
    }
    public Sprite GetSpriteEquipType(string eq_ty_key)
    {
        if (string.Equals(eq_ty_key, "wp"))
            return mAtlas_equip_skill.GetSprite("type_equip_wpn_shd");
        else if (string.Equals(eq_ty_key, "ar"))
            return mAtlas_equip_skill.GetSprite("type_equip_armor");
        else if (string.Equals(eq_ty_key, "ac"))
            return mAtlas_equip_skill.GetSprite("type_equip_acce");

        return GetTransparency();
    }

    public Sprite GetGrayRoundBox(bool isGray)
    {
        if (isGray)
            return mAtlas_equip_skill.GetSprite("gray_round_box");
        else
            return mAtlas_equip_skill.GetSprite("ui_round_box");
    }

    /// <summary> 랭킹 1~3위 트로피 </summary>
    public Sprite GetSpriteRankTrophy(int rank)
    {
        switch (rank)
        {
            case 1: return mAtlas_equip_skill.GetSprite("rank_1st");
            case 2: return mAtlas_equip_skill.GetSprite("rank_2st");
            case 3: return mAtlas_equip_skill.GetSprite("rank_3st");
            default: return mAtlas_equip_skill.GetSprite("rank_etc");
        }
    }

    /// <summary> PvP 배틀 아레나 기록 리스트에서 공격자 or 방어자 아이콘 </summary>
    public Sprite GetPvpAttackerOrDefender(bool isAtker)
    {
        if (isAtker)
            return mAtlas_equip_skill.GetSprite("crossed-swords"); // pvp_btl_attacker
        else
            return mAtlas_equip_skill.GetSprite("skull-shield"); // pvp_btl_defender
    }

    /// <summary> 퀘스트 아이콘 </summary>
    public Sprite GetQuestIcon(int nbr)
    {
        return mAtlas_equip_skill.GetSprite(string.Format("quest_icon_nbr{0}", nbr));
    }

    /// <summary> 홈 오른쪽 옵션 버튼 isOn : 현재 상태가 열림 상태</summary>
    public Sprite GetHomeOptionButton(bool isOn)
    {
        if (isOn)
            return mAtlas_equip_skill.GetSprite("expand"); // 열기 이미지 
        else return mAtlas_equip_skill.GetSprite("contract"); // 닫기 이미지 
    }

    /// <summary>
    /// 컨텐츠 매인 탭에서 보상 아이콘 
    /// </summary>
    public Sprite GetContentsRewardIcon (string rwd_name)
    {
        switch (rwd_name)
        {
            case "gold": return mAtlas_equip_skill.GetSprite("goods_gold");
            case "weapon_shield_top":
            case "weapon_shield": return mAtlas_equip_skill.GetSprite("type_equip_wpn_shd");
            case "armor_top":
            case "armor": return mAtlas_equip_skill.GetSprite("type_equip_armor");
            case "acce_top":
            case "acce": return mAtlas_equip_skill.GetSprite("type_equip_acce");
            case "skill": return mAtlas_equip_skill.GetSprite("icon_skill_1");
            case "equip_piece": return mAtlas_equip_skill.GetSprite("it_28_7_equip_piece");
            case "acce_piece_rt5": return mAtlas_equip_skill.GetSprite("it_29_5_equip_ac_piece");
            case "acce_piece_rt7": return mAtlas_equip_skill.GetSprite("it_29_7_equip_ac_piece");
            case "ether": return mAtlas_equip_skill.GetSprite("goods_ether");
            case "equip_enhant_ston": return mAtlas_equip_skill.GetSprite("it_21_4_equip_enchant_stone");
            case "acce_enhant_ston": return mAtlas_equip_skill.GetSprite("it_27_4_acce_enchant_stone");
            case "enhant_bless_rt2": return mAtlas_equip_skill.GetSprite("it_22_2_enchant_blessing");
            case "enhant_bless_rt3": return mAtlas_equip_skill.GetSprite("it_22_3_enchant_blessing");
            case "potion": return mAtlas_equip_skill.GetSprite("it_20_3_potion");
            case "key_all": return mAtlas_equip_skill.GetSprite("key_all");
        }
        return null;
    }

    public Sprite GetCheckerMakk () => mAtlas_equip_skill.GetSprite("check-mark");
}
