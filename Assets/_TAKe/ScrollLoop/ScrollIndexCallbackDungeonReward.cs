using UnityEngine;
using UnityEngine.UI;

public class ScrollIndexCallbackDungeonReward : MonoBehaviour
{
    [SerializeField] bool is_preview = false;
    [SerializeField] Image img_Icon, img_RatingBg;
    [SerializeField] Text tx_Rating, tx_Count, tx_RwdName, tx_RwdRate;
    [SerializeField] Text tx_FirstOneReward; // 1회 보상 아이템 -> (0/1)
    [SerializeField] GameObject go_SkillWear;

    [SerializeField] PopUpDungeonReward.ResultReward temp = new PopUpDungeonReward.ResultReward();

    void ScrollCellIndex(int idx)
    {
        PopUpDungeonReward.ResultReward dg_result_rwd = new PopUpDungeonReward.ResultReward();
        if(is_preview == false) // 실제 보상 창에서 
        {
            var md_ty = GameMng.GetInstance().gameUIObject.dungeonTop.pu_DungeonReward.rwdMty;
            if (md_ty == IG.ModeType.DUNGEON_TOP)
            {
                dg_result_rwd = GameMng.GetInstance().gameUIObject.dungeonTop.pu_DungeonReward.resultRewards[idx];
            }
            else if (md_ty == IG.ModeType.DUNGEON_MINE)
            {
                dg_result_rwd = GameMng.GetInstance().gameUIObject.dungeonMine.pu_DungeonReward.resultRewards[idx];
            }
            else if (md_ty == IG.ModeType.DUNGEON_RAID)
            {
                dg_result_rwd = GameMng.GetInstance().gameUIObject.dungeonRaid.pu_DungeonReward.resultRewards[idx];
            }
        }
        else // 미리보기 탭에서 
        {
            dg_result_rwd = MainUI.GetInstance().tapDungeon.resultRewardPreview[idx];
        }

        if (dg_result_rwd.rwd_type == PopUpDungeonReward.RwdType.SKILL)
        {
            bool isWear = false;
            var useSkill = GameMng.GetInstance().myPZ.igp.playerSkillAction.playerSkill.useSkill;
            foreach (var item in useSkill)
            {
                if (!isWear)
                    isWear = (int)item.number == dg_result_rwd.rwdSkill.sk_id;

                if (isWear)
                    break;
            }

            if(go_SkillWear != null) 
                go_SkillWear.SetActive(isWear);
        }
        else
        {
            if(go_SkillWear != null)
                go_SkillWear.SetActive(false);
        }

        temp = dg_result_rwd;
        if (dg_result_rwd.rwd_type != PopUpDungeonReward.RwdType.NONE)
        {
            switch (dg_result_rwd.rwd_type)
            {
                case PopUpDungeonReward.RwdType.GOLD:
                    SetRewardUI_Gold(GameDatabase.GetInstance().questDB.GetQuestMaxSecondRewardGold(dg_result_rwd.rwdGold.cnt));
                    break;
                case PopUpDungeonReward.RwdType.EQUIP:
                    SetRewardUI_EquipOrAcce(dg_result_rwd.rwdEquip.eq_ty, dg_result_rwd.rwdEquip.eq_rt, dg_result_rwd.rwdEquip.eq_id, dg_result_rwd.rwdEquip.pct );
                    break;
                case PopUpDungeonReward.RwdType.EQUIP_AC:
                    SetRewardUI_EquipOrAcce(dg_result_rwd.rwdEquip.eq_ty, dg_result_rwd.rwdEquip.eq_rt, dg_result_rwd.rwdEquip.eq_id, dg_result_rwd.rwdEquip.pct);
                    break;
                case PopUpDungeonReward.RwdType.SKILL:
                    SetRewardUI_Skill(dg_result_rwd.rwdSkill.sk_id, dg_result_rwd.rwdSkill.sk_cnt);
                    break;
                case PopUpDungeonReward.RwdType.EQUIP_PIECE:
                    SetRewardUI_EquipPiece(dg_result_rwd.rwdEquipPiece.pce_rt, dg_result_rwd.rwdEquipPiece.pce_cnt);
                    break;
                case PopUpDungeonReward.RwdType.EQUIP_AC_PIECE:
                    SetRewardUI_EquipAcPiece(dg_result_rwd.rwdEquipAcPiece.pce_rt, dg_result_rwd.rwdEquipAcPiece.pce_cnt);
                    break;
                case PopUpDungeonReward.RwdType.EQUIP_ENHANT_STON:
                    SetRewardUI_EquipEnhantSton(dg_result_rwd.rwdEquipEnhantSton.eq_stn_rt, dg_result_rwd.rwdEquipEnhantSton.eq_stn_cnt);
                    break;
                case PopUpDungeonReward.RwdType.EQUIP_AC_ENHANT_STON:
                    SetRewardUI_EquipAcEnhantSton(dg_result_rwd.rwdEquipAcEnhantSton.eqac_stn_rt, dg_result_rwd.rwdEquipAcEnhantSton.eqac_stn_cnt);
                    break;
                case PopUpDungeonReward.RwdType.ENHANT_BLESS:
                    SetRewardUI_EnhantBless(dg_result_rwd.rwdEnhantBless.bls_rt, dg_result_rwd.rwdEnhantBless.bls_cnt);
                    break;
                case PopUpDungeonReward.RwdType.GOODS_ETHER:
                    SetRewardUI_EquipAcCrystal(dg_result_rwd.rwdGoodsEther.ether_cnt);
                    break;
            }

            // 1회 보상 아이템 체크 
            if (tx_FirstOneReward != null)
            {
                if (MainUI.GetInstance().tapDungeon.dgTap == TapDungeon.DgTap.TOP && is_preview)
                {
                    if (dg_result_rwd.rwd_type == PopUpDungeonReward.RwdType.EQUIP ||
                        dg_result_rwd.rwd_type == PopUpDungeonReward.RwdType.EQUIP_AC ||
                        dg_result_rwd.rwd_type == PopUpDungeonReward.RwdType.GOODS_ETHER)
                    {
                        int lastClrNbr = GameDatabase.GetInstance().tableDB.GetUserInfo().GetDgNbr(IG.ModeType.DUNGEON_TOP);
                        int nbr = (int)MainUI.GetInstance().tapDungeon.dgTopNbr;
                        LogPrint.Print("lastClrNbr : " + lastClrNbr + ", nbr : " + nbr + ", isCompleteReward : " + (lastClrNbr > nbr));
                        bool isCompleteReward = lastClrNbr > nbr; // 클리어 완료해서 보상을 받았다. 
                        tx_FirstOneReward.text = isCompleteReward == false ? "(0/1)" : "(1/1)";
                    }
                    else
                    {
                        tx_FirstOneReward.text = "";
                    }
                }
                else
                {
                    tx_FirstOneReward.text = "";
                }
            }
        }
    }

    // 골드 보상 UI 
    void SetRewardUI_Gold(long val)
    {
        img_Icon.sprite = SpriteAtlasMng.GetInstance().GetSpriteGold();
        img_RatingBg.sprite = SpriteAtlasMng.GetInstance().GetSpriteRatingBg(0);
        tx_Rating.text = string.Empty;
        tx_Count.text = string.Format("x{0:#,0}", val);
        tx_RwdName.text = "골드";
        if (tx_RwdRate != null)
            tx_RwdRate.text = "100%";
    }
    
    // 장비, 장신구 보상 UI
    void SetRewardUI_EquipOrAcce (int eq_ty, int eq_rt, int eq_id, float drp_pct)
    {
        bool isAcce = GameDatabase.GetInstance().tableDB.GetIsPartsTypeAcce(eq_ty);
        tx_Rating.text = GameDatabase.StringFormat.GetRatingColorText(eq_rt, false);
        tx_Count.text = string.Empty;
        tx_RwdName.text = is_preview == true ? (isAcce == true ? "장신구" : "장비") : GameDatabase.StringFormat.GetEquipName(eq_ty, eq_rt, eq_id);
        img_RatingBg.sprite = SpriteAtlasMng.GetInstance().GetSpriteRatingBg(eq_rt);
        if (is_preview)
        {
            if (tx_RwdRate != null)
                tx_RwdRate.text = drp_pct > 0.0f ? string.Format("{0:0.000}%", drp_pct) : "100%";
        }

        if (!is_preview)
        {
            img_Icon.sprite = SpriteAtlasMng.GetInstance().GetSpriteEquip(eq_ty, eq_rt, eq_id);
        }
        else
        {
            if (isAcce)
                img_Icon.sprite = SpriteAtlasMng.GetInstance().GetSpriteTypeEquipAcce();
            else
            {
                if (MainUI.GetInstance().tapDungeon.dgTap == TapDungeon.DgTap.RAID)
                    img_Icon.sprite = SpriteAtlasMng.GetInstance().GetSpriteTypeEquipAll();
                else
                    img_Icon.sprite = eq_ty <= 1 ? SpriteAtlasMng.GetInstance().GetSpriteTypeEquipWeaponShield() : SpriteAtlasMng.GetInstance().GetSpriteTypeEquipArmor();
            }
        }
    }

    // 스킬 보상 UI
    void SetRewardUI_Skill (int sk_id, int cnt)
    {
        int sk_rt = GameDatabase.GetInstance().chartDB.GetChartSkill_Data(sk_id).s_rating;
        img_Icon.sprite = SpriteAtlasMng.GetInstance().GetSpriteSkill(sk_id);
        img_RatingBg.sprite = SpriteAtlasMng.GetInstance().GetSpriteRatingBg(0);
        tx_Rating.text = string.Empty;
        tx_Count.text = string.Format("x{0}", cnt);
        tx_RwdName.text = GameDatabase.StringFormat.GetSkillName(sk_id);
        if (tx_RwdRate != null)
            tx_RwdRate.text = "100%";
    }

    // 장비 조각 보상 UI 
    void SetRewardUI_EquipPiece(int eq_pce_rt, int eq_pce_cnt)
    {
        img_Icon.sprite = SpriteAtlasMng.GetInstance().GetItemSprite(28, eq_pce_rt);
        img_RatingBg.sprite = SpriteAtlasMng.GetInstance().GetSpriteRatingBg(eq_pce_rt);
        tx_Rating.text = GameDatabase.StringFormat.GetRatingColorText(eq_pce_rt, false);
        tx_Count.text = string.Format("x{0}", eq_pce_cnt);
        tx_RwdName.text = "장비 조각";
        if (tx_RwdRate != null)
            tx_RwdRate.text = "100%";
    }

    // 장신구 조각 보상 UI
    void SetRewardUI_EquipAcPiece(int eqac_pce_rt, int eqac_pce_cnt)
    {
        img_Icon.sprite = SpriteAtlasMng.GetInstance().GetItemSprite(29, eqac_pce_rt);
        img_RatingBg.sprite = SpriteAtlasMng.GetInstance().GetSpriteRatingBg(eqac_pce_rt);
        tx_Rating.text = GameDatabase.StringFormat.GetRatingColorText(eqac_pce_rt, false);
        tx_Count.text = string.Format("x{0}", eqac_pce_cnt);
        tx_RwdName.text = "장신구 조각";
        if (tx_RwdRate != null)
            tx_RwdRate.text = "100%";
    }

    // 장비 강화석 UI
    void SetRewardUI_EquipEnhantSton(int eq_stn_rt, int eq_stn_cnt)
    {
        img_Icon.sprite = SpriteAtlasMng.GetInstance().GetItemSprite(21, eq_stn_rt);
        img_RatingBg.sprite = SpriteAtlasMng.GetInstance().GetSpriteRatingBg(eq_stn_rt);
        tx_Rating.text = GameDatabase.StringFormat.GetRatingColorText(eq_stn_rt, false);
        tx_Count.text = string.Format("x{0}", eq_stn_cnt);
        tx_RwdName.text = "장비 강화석";
        if (tx_RwdRate != null)
            tx_RwdRate.text = "100%";
    }

    // 장신구 강화석 UI
    void SetRewardUI_EquipAcEnhantSton(int eqac_stn_rt, int eqac_stn_cnt)
    {
        img_Icon.sprite = SpriteAtlasMng.GetInstance().GetItemSprite(27, eqac_stn_rt);
        img_RatingBg.sprite = SpriteAtlasMng.GetInstance().GetSpriteRatingBg(eqac_stn_rt);
        tx_Rating.text = GameDatabase.StringFormat.GetRatingColorText(eqac_stn_rt, false);
        tx_Count.text = string.Format("x{0}", eqac_stn_cnt);
        tx_RwdName.text = "장신구 강화석";
        if (tx_RwdRate != null)
            tx_RwdRate.text = "100%";
    }

    // 장비/장신구 강화 축복 주문서 UI
    void SetRewardUI_EnhantBless(int bls_rt, int bls_cnt)
    {
        img_Icon.sprite = SpriteAtlasMng.GetInstance().GetItemSprite(22, bls_rt);
        img_RatingBg.sprite = SpriteAtlasMng.GetInstance().GetSpriteRatingBg(bls_rt);
        tx_Rating.text = GameDatabase.StringFormat.GetRatingColorText(bls_rt, false);
        tx_Count.text = string.Format("x{0}", bls_cnt);
        tx_RwdName.text = "강화 축복 주문서";
        if (tx_RwdRate != null)
            tx_RwdRate.text = "100%";
    }

    void SetRewardUI_EquipAcCrystal(int ether_cnt)
    {
        img_Icon.sprite = SpriteAtlasMng.GetInstance().GetSpriteEther();
        img_RatingBg.sprite = SpriteAtlasMng.GetInstance().GetSpriteRatingBg(0);
        tx_Rating.text = string.Empty;
        tx_Count.text = string.Format("x{0}", ether_cnt);
        tx_RwdName.text = "에테르";
        if (tx_RwdRate != null)
            tx_RwdRate.text = "100%";
    }

    /// <summary>
    /// 보상품 설명 팝업 
    /// </summary>
    public void Click_ItemPreview()
    {
        if (temp.rwdGold.cnt > 0)
        {
            LogPrint.Print(" --- rwdGold --- ");
            PopUpMng.GetInstance().Open_ContentsDungeonRewardItemInfo(img_Icon.sprite, "gold");
        }
        else if (temp.rwdEquip.eq_rt > 0)
        {
            LogPrint.Print(" --- rwdEquip --- ");
            if (temp.rwdEquip.eq_ty == 0 || temp.rwdEquip.eq_ty == 1)
            {
                if (MainUI.GetInstance().tapDungeon.dgTap == TapDungeon.DgTap.TOP)
                    PopUpMng.GetInstance().Open_ContentsDungeonRewardItemInfo(img_Icon.sprite, "weapon_shield_once");
                else if (MainUI.GetInstance().tapDungeon.dgTap == TapDungeon.DgTap.RAID)
                    PopUpMng.GetInstance().Open_ContentsDungeonRewardItemInfo(img_Icon.sprite, string.Format("equip_all_rt{0}", temp.rwdEquip.eq_rt));
                else PopUpMng.GetInstance().Open_ContentsDungeonRewardItemInfo(img_Icon.sprite, "weapon_shield");
            }
            else if (GameDatabase.GetInstance().tableDB.GetIsPartsTypeAcce(temp.rwdEquip.eq_ty))
            {
                if (MainUI.GetInstance().tapDungeon.dgTap == TapDungeon.DgTap.TOP)
                    PopUpMng.GetInstance().Open_ContentsDungeonRewardItemInfo(img_Icon.sprite, "acce_once");
                else if(MainUI.GetInstance().tapDungeon.dgTap == TapDungeon.DgTap.RAID)
                    PopUpMng.GetInstance().Open_ContentsDungeonRewardItemInfo(img_Icon.sprite, string.Format("acce_rt{0}", temp.rwdEquip.eq_rt)); 
                else PopUpMng.GetInstance().Open_ContentsDungeonRewardItemInfo(img_Icon.sprite, "acce");
            }
            else
            {
                if (MainUI.GetInstance().tapDungeon.dgTap == TapDungeon.DgTap.TOP)
                    PopUpMng.GetInstance().Open_ContentsDungeonRewardItemInfo(img_Icon.sprite, "armor_once");
                else if (MainUI.GetInstance().tapDungeon.dgTap == TapDungeon.DgTap.RAID)
                    PopUpMng.GetInstance().Open_ContentsDungeonRewardItemInfo(img_Icon.sprite, string.Format("equip_all_rt{0}", temp.rwdEquip.eq_rt));
                else PopUpMng.GetInstance().Open_ContentsDungeonRewardItemInfo(img_Icon.sprite, "armor");
            }
        }
        else if (temp.rwdSkill.sk_cnt > 0)
        {
            LogPrint.Print(" --- rwdSkill --- ");
            PopUpMng.GetInstance().Open_ContentsDungeonSkillRewardItemInfo(temp.rwdSkill.sk_id, temp.rwdSkill.sk_cnt);
        }
        else if (temp.rwdEquipPiece.pce_cnt > 0)
        {
            LogPrint.Print(" --- rwdEquipPiece --- ");
            PopUpMng.GetInstance().Open_ContentsDungeonRewardItemInfo(img_Icon.sprite, string.Format("equip_piece_rt{0}", temp.rwdEquipPiece.pce_rt));
        }
        else if (temp.rwdEquipAcPiece.pce_cnt > 0)
        {
            LogPrint.Print(" --- rwdEquipAcPiece --- ");
            PopUpMng.GetInstance().Open_ContentsDungeonRewardItemInfo(img_Icon.sprite, string.Format("acce_piece_rt{0}", temp.rwdEquipAcPiece.pce_rt));
        }
        else if (temp.rwdEquipEnhantSton.eq_stn_cnt > 0)
        {
            LogPrint.Print(" --- rwdEquipEnhantSton --- ");
            PopUpMng.GetInstance().Open_ContentsDungeonRewardItemInfo(img_Icon.sprite, "equip_enhant_ston");
        }
        else if (temp.rwdEquipAcEnhantSton.eqac_stn_cnt > 0)
        {
            LogPrint.Print(" --- rwdEquipAcEnhantSton --- ");
            PopUpMng.GetInstance().Open_ContentsDungeonRewardItemInfo(img_Icon.sprite, "acce_enhant_ston");
        }
        else if (temp.rwdEnhantBless.bls_cnt > 0)
        {
            LogPrint.Print(" --- rwdEnhantBless --- ");
            PopUpMng.GetInstance().Open_ContentsDungeonRewardItemInfo(img_Icon.sprite, "enhant_bless");
        }
        else if (temp.rwdGoodsEther.ether_cnt > 0)
        {
            LogPrint.Print(" --- rwdGoodsEther --- ");
            if (MainUI.GetInstance().tapDungeon.dgTap == TapDungeon.DgTap.TOP)
                PopUpMng.GetInstance().Open_ContentsDungeonRewardItemInfo(img_Icon.sprite, "ether_once");
            else PopUpMng.GetInstance().Open_ContentsDungeonRewardItemInfo(img_Icon.sprite, "ether");
        }
        
    }
}