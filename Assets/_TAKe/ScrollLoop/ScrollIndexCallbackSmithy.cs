using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.U2D;
using Coffee.UIExtensions;

public class ScrollIndexCallbackSmithy : MonoBehaviour
{
    [SerializeField] int cellIndex;
    [SerializeField] private GameDatabase.TableDB.Equipment cell_equip_data = new GameDatabase.TableDB.Equipment();
    [SerializeField] private GameDatabase.TableDB.Item cell_item_data = new GameDatabase.TableDB.Item();
    [SerializeField] private int nowCellIdx;
    [SerializeField] GameObject go_Root;
    [SerializeField] Image imageIcon;
    [SerializeField] Image imageRatingBg;
    [SerializeField] Text textRating;
    [SerializeField] Text textEqNormalLv;
    [SerializeField] Text textEqEhnLvOrCount;
    [SerializeField] GameObject go_Lock;
    [SerializeField] GameObject goWearLabel, goNew;

    [SerializeField] Image imRtOutline;
    [SerializeField] UIGradient grRtOutline;
    [SerializeField] UIShiny shRtOutline;

    [SerializeField] GameObject goBlack; // 블랙 처리 

    bool isNull = false;
    public void EquipCellRefresh()
    {
        LogPrint.Print("---------------REFRER ");
        AScrollCellIndex(nowCellIdx);
    }

    public void ItemCellRefresh()
    {
        LogPrint.Print("---------------REFRER ");
        AScrollCellIndex(nowCellIdx);
    }

    public void ItemCellRefresh(long uid1, long uid2)
    {
        LogPrint.Print("---------------REFRER ");
        if(cell_item_data.aInUid == uid1 || cell_item_data.aInUid == uid2)
            AScrollCellIndex(nowCellIdx);
    }

    public void CellIndex(int idx)
    {
        AScrollCellIndex((idx * 5) + cellIndex);
    }

    /// <summary>
    ///  스크롤 리스트 
    /// </summary>
    public void AScrollCellIndex(int idx)
    {
        nowCellIdx = idx;
        var data = GameDatabase.GetInstance().tableDB.GetTempList(idx);
        var smytype = MainUI.GetInstance().tapSmithy.smithyType;

        goNew.gameObject.SetActive(data.equipment.client_add_sp > MainUI.GetInstance().tapSmithy.tapOpenSP);

        if (data.default0Eq1It2Sk3 == 1) // 장비 
        {
            isNull = false;
            cell_equip_data = data.equipment;
            if (!go_Root.activeSelf)
                go_Root.SetActive(true);

            goWearLabel.gameObject.SetActive(GameDatabase.GetInstance().tableDB.GetNowWearingEquipPartsData(cell_equip_data.eq_ty).aInUid == cell_equip_data.aInUid); // 현재 착용중인 장비가 셀의 장비인가 

            // 선택한 장비나 아이템을 리스트에서 블랙 처리 
            if (smytype == SmithyTapType.Enhancement) // 강화 
            {
                goBlack.SetActive(cell_equip_data.aInUid == MainUI.GetInstance().tapSmithy.equipmentEnhancement.GetSelectEquipUid);
            }
            else if (smytype == SmithyTapType.LevelTransfer) // 강화 레벨 전승 
            {
                var mainEquip = MainUI.GetInstance().tapSmithy.equipmentLevelTransfer.GetSelectMain;
                if (mainEquip.aInUid == 0)
                    goBlack.SetActive(false);
                else ReleaseEnhantTransferMat(mainEquip);
            }
            else if (smytype == SmithyTapType.OrnamentChangeOptions) // 장신구 옵션 변경 
            {
                if (GameDatabase.GetInstance().tableDB.GetIsPartsTypeAcce(cell_equip_data.eq_ty) == true)
                {
                    if (cell_equip_data.eq_rt >= 3) // 고급 등급부터 옵션 변경 가능 
                    {
                        var selEquipAcce = MainUI.GetInstance().tapSmithy.equipmentChangeOrnamentOptions.GetSelectEquipAcce();
                        goBlack.SetActive(long.Equals(selEquipAcce.aInUid, cell_equip_data.aInUid));
                    }
                    else goBlack.SetActive(true);
                }
                else
                {
                    if (cell_equip_data.eq_rt >= 7 && cell_equip_data.eq_legend == 1) // 장비는 전설 진화된 장비 부터 가능 
                    {
                        var selEquipAcce = MainUI.GetInstance().tapSmithy.equipmentChangeOrnamentOptions.GetSelectEquipAcce();
                        goBlack.SetActive(long.Equals(selEquipAcce.aInUid, cell_equip_data.aInUid));
                    }
                    else goBlack.SetActive(true);
                }
            }
            else if (smytype == SmithyTapType.OrnamentSynthesis) // 장신구 합성 
            {
                long sel_mat1_uid = MainUI.GetInstance().tapSmithy.ornamentSynthesis.GetNowSelectEquipMat1().aInUid;
                long sel_mat2_uid = MainUI.GetInstance().tapSmithy.ornamentSynthesis.GetNowSelectEquipMat2().aInUid;
                goBlack.SetActive(sel_mat1_uid == cell_equip_data.aInUid || sel_mat2_uid == cell_equip_data.aInUid || cell_equip_data.eq_rt != 5 || cell_equip_data.m_state > 0);
            }
            else if(smytype == SmithyTapType.OrnamentAdvancement)
            {
                long sel_mat0_uid = MainUI.GetInstance().tapSmithy.ornamentAdvancement.GetNowSelectEquipMat0().aInUid;
                long sel_mat1_uid = MainUI.GetInstance().tapSmithy.ornamentAdvancement.GetNowSelectEquipMat1().aInUid;
                long sel_mat2_uid = MainUI.GetInstance().tapSmithy.ornamentAdvancement.GetNowSelectEquipMat2().aInUid;
                CellRefreshSelectAcceAdvancement(sel_mat0_uid, sel_mat1_uid, sel_mat2_uid);
                //bool isNoEmpty = sel_mat0_uid == 0 && sel_mat1_uid == 0 && sel_mat2_uid == 0;
                //if (isNoEmpty)
                //{
                //    goBlack.SetActive(cell_equip_data.eq_rt != 6);
                //}
                //else
                //{
                //    goBlack.SetActive(cell_equip_data.eq_rt != 6 || cell_equip_data.m_state > 0 || sel_mat0_uid == cell_equip_data.aInUid || sel_mat2_uid == cell_equip_data.aInUid || sel_mat2_uid == cell_equip_data.aInUid);
                //}

                //goBlack.SetActive(sel_mat0_uid == cell_equip_data.aInUid || sel_mat1_uid == cell_equip_data.aInUid || sel_mat2_uid == cell_equip_data.aInUid || cell_equip_data.eq_rt != 6 || cell_equip_data.m_state == 0);
            }
            else goBlack.SetActive(false);

            int parts_type = cell_equip_data.eq_ty;
            int parts_rating = cell_equip_data.eq_rt;
            int parts_idx = cell_equip_data.eq_id;
            int normal_level = cell_equip_data.m_norm_lv;
            int enhant_level = cell_equip_data.m_ehnt_lv;
            int state_lock = cell_equip_data.m_lck;
            imageIcon.sprite = SpriteAtlasMng.GetInstance().GetSpriteEquip(parts_type, parts_rating, parts_idx);
            imageRatingBg.sprite = SpriteAtlasMng.GetInstance().GetSpriteRatingBg(parts_rating, cell_equip_data.eq_legend);
            textRating.color = ResourceDatabase.GetInstance().GetItemColor(parts_rating);
            textRating.text = parts_rating > 0 ? LanguageGameData.GetInstance().GetString(string.Format("item.rating.string.{0}", parts_rating)) : "";

            textEqNormalLv.text = string.Format("Lv.{0}", normal_level);
            textEqEhnLvOrCount.text = string.Format("+{0}", enhant_level);
            go_Lock.SetActive(state_lock == 1);
        }
        else if (data.default0Eq1It2Sk3 == 2) // 아이템 
        {
            isNull = false;
            cell_item_data = data.item;
            int item_ty = cell_item_data.type;
            int item_rt = cell_item_data.rating;
            int item_cnt = cell_item_data.count;

            go_Root.SetActive(true);
            if (smytype == SmithyTapType.StonEvolution)
            {
                goBlack.SetActive(item_ty != GameDatabase.TableDB.item_type_eq_ston && item_ty != GameDatabase.TableDB.item_type_ac_ston);
            }
            else goBlack.SetActive(false);

            imageIcon.sprite = SpriteAtlasMng.GetInstance().GetItemSprite(item_ty, item_rt);
            imageRatingBg.sprite = SpriteAtlasMng.GetInstance().GetSpriteRatingBg(item_rt);
            textRating.color = ResourceDatabase.GetInstance().GetItemColor(item_rt);
            textRating.text = item_rt > 0 ? LanguageGameData.GetInstance().GetString(string.Format("item.rating.string.{0}", item_rt)) : "";

            textEqNormalLv.text = "";
            textEqEhnLvOrCount.text = string.Format("x{0}", item_cnt);

            goWearLabel.SetActive(false);
            go_Lock.SetActive(false);
        }
        else
        {
            isNull = true;
            if (go_Root.activeSelf)
                go_Root.SetActive(false);
            imageRatingBg.sprite = SpriteAtlasMng.GetInstance().GetSpriteRatingBg(0);
        }
    }

    void NowCell()
    {
        LogPrint.Print("nowCellIdx:" + nowCellIdx);
        AScrollCellIndex(nowCellIdx);
        MainUI.GetInstance().tapSmithy.initOnStartSmithy.cellAction = NowCell;
    }

    #region 장비 강화
    /// <summary>
    /// 장비 강화 : 셀에서 장비 선택했을 때 셀 블랙처리 
    /// </summary>
    public void CellSelectEnhant(GameDatabase.TableDB.Equipment eqDb)
    {
        goBlack.SetActive(eqDb.aInUid == cell_equip_data.aInUid);
    }

    /// <summary>
    /// 장비 강화 : 선택된 장비 해재 눌렀을 때 
    /// </summary>
    public void SelectRelease()
    {
        if(goBlack.activeSelf)
            goBlack.SetActive(false);
    }
    #endregion

    #region 강화석 진화 
    public void CellSelectSton(GameDatabase.TableDB.Equipment eqDb)
    {
        goBlack.SetActive(eqDb.aInUid == cell_equip_data.aInUid);
    }
    public void ReleaseSton()
    {
        if(goBlack.activeSelf)
            goBlack.SetActive(false);
    }
    #endregion

    #region 장비 강화 레벨 전승 
    /// <summary>
    /// 강화 레벨 전승 : 왼쪽, 전승할 재료로 쓰일 장비를 해ㅑ제누렀을 때
    /// </summary>
    public void ReleaseEnhantTransferMat(GameDatabase.TableDB.Equipment main_eqDb)
    {
        if (main_eqDb.eq_ty == cell_equip_data.eq_ty && main_eqDb.eq_rt == cell_equip_data.eq_rt)
        {
            if (main_eqDb.aInUid == cell_equip_data.aInUid)
            {
                if(!goBlack.activeSelf)
                    goBlack.SetActive(true);
            }
            else
            {
                if (main_eqDb.m_ehnt_lv < cell_equip_data.m_ehnt_lv)
                {
                    if(goBlack.activeSelf)
                        goBlack.SetActive(false);
                }
                else
                {
                    if (!goBlack.activeSelf)
                        goBlack.SetActive(true);
                }
            }
        }
        else goBlack.SetActive(true);
    }

    /// <summary>
    /// 강화 레벨 전승 : 오른쪽, 강화 전승받을 재료를 해제했을때 
    /// </summary>
    public void SelectReleaseEnhantTransfer()
    {
        if(goBlack.activeSelf)
            goBlack.SetActive(false);
    }
    #endregion

    #region 장신구 옵션 변경
    public void CellRefreshSelectAcceChangeOption(GameDatabase.TableDB.Equipment eqDb)
    {
        if (GameDatabase.GetInstance().tableDB.GetIsPartsTypeAcce(eqDb.eq_ty))
        {
            goBlack.SetActive(eqDb.aInUid == cell_equip_data.aInUid || cell_equip_data.eq_rt < 3);
        }
        else 
        {
            goBlack.SetActive(eqDb.aInUid == cell_equip_data.aInUid || (cell_equip_data.eq_rt < 7 || cell_equip_data.eq_legend == 0));
        }
    }

    public void RefreshSelectReleaseAcceChangeOption()
    {
        if (GameDatabase.GetInstance().tableDB.GetIsPartsTypeAcce(cell_equip_data.eq_ty))
        {
            goBlack.SetActive(cell_equip_data.eq_rt < 3);
        }
        else
        {
            goBlack.SetActive(cell_equip_data.eq_rt < 7 || cell_equip_data.eq_legend == 0);
        }
    }
    #endregion

    #region 장신구 합성, 영웅 -> 고대 
    // 셀에서 선택 
    public void CellRefreshSelectAcceSynthesis(GameDatabase.TableDB.Equipment eqDbMat1, GameDatabase.TableDB.Equipment eqDbMat2)
    {
        if (eqDbMat1.aInUid == cell_equip_data.aInUid || eqDbMat2.aInUid == cell_equip_data.aInUid)
            goBlack.SetActive(true);
        else
        {
            if(cell_equip_data.eq_rt == 5 && cell_equip_data.m_state == 0)
            {
                if(goBlack.activeSelf)
                    goBlack.SetActive(false);
            }
            else 
            {
                if (!goBlack.activeSelf)
                    goBlack.SetActive(true);
            }
        }
    }
    // 탭에서 해제 
    public void RefreshSelectReleaseAcceSynthesis(GameDatabase.TableDB.Equipment eqDb)
    {
        if(eqDb.aInUid == cell_equip_data.aInUid)
        {
            goBlack.SetActive(false);
        }
    }
    #endregion

    #region 장신구 승급 고대 -> 전설 
    // 셀에서 선택 
    public void CellRefreshSelectAcceAdvancement(long eqDbUniqId0, long eqDbUniqId1, long eqDbUniqId2)
    {
        if (eqDbUniqId0 == cell_equip_data.aInUid || eqDbUniqId1 == cell_equip_data.aInUid || eqDbUniqId2 == cell_equip_data.aInUid)
            goBlack.SetActive(true);
        else
        {
            if(eqDbUniqId0 > 0) // 매인 선택된상태 
            {
                goBlack.SetActive(cell_equip_data.m_state > 0 || cell_equip_data.eq_rt != 6); // 착용중이거나, 고대 빼고 블럭 
            }
            else
            {
                goBlack.SetActive(cell_equip_data.eq_rt != 6);
            }
        }
    }
    // 탭에서 해제 
    public void RefreshSelectReleaseAcceAdvancement(GameDatabase.TableDB.Equipment eqDb)
    {
        if (eqDb.aInUid == cell_equip_data.aInUid)
        {
            goBlack.SetActive(false);
        }
    }
    #endregion

    /// <summary>
    /// 셀 선택 
    /// </summary>
    public void ClickItem()
    {
        if (isNull)
            return;

        switch (MainUI.GetInstance().tapSmithy.smithyType)
        {
            // ----- 장비 강화 -----
            case SmithyTapType.Enhancement:
                if (cell_equip_data.m_ehnt_lv >= GameDatabase.GetInstance ().chartDB.GetDicBalance("eq.enhant.max.level").val_int)
                {
                    PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("이미 최대 강화 레벨 상태입니다.");
                }
                else if (MainUI.GetInstance().tapSmithy.equipmentEnhancement.GetSelectEquipUid != cell_equip_data.aInUid)
                {
                    MainUI.GetInstance().tapSmithy.equipmentEnhancement.SetData(cell_equip_data);
                    foreach (var item in GameObject.FindObjectsOfType<ScrollIndexCallbackSmithy>())
                        item.CellSelectEnhant(cell_equip_data);

                    MainUI.GetInstance().tapSmithy.smithyScrollRefreshCellEquip = EquipCellRefresh;
                }
                break;

            // -----강화석 진화 -----
            case SmithyTapType.StonEvolution:
                if (cell_item_data.type == GameDatabase.TableDB.item_type_eq_ston || cell_item_data.type == GameDatabase.TableDB.item_type_ac_ston) // 아이템이 강화석(장비,장신구)일 때만 
                {
                    if(cell_item_data.count >= 10 && cell_item_data.rating < 4)
                    {
                        MainUI.GetInstance().tapSmithy.equipmentStonEvolution.SetData(cell_item_data);
                        foreach (var item in GameObject.FindObjectsOfType<ScrollIndexCallbackSmithy>())
                            item.CellSelectSton(cell_equip_data);
                    }
                    else
                    {
                        if (cell_item_data.rating >= 4)
                            PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("최대 등급의 강화석 입니다.\n강화석 진화가 불가능합니다.");
                        else if(cell_item_data.count < 10)
                            PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener("강화석이 부족합니다.\n강화석 구매 탭으로 이동됩니다.", MainUI.GetInstance().Listener_MoveItemShop);
                    }
                }
                break;

            // ----- 강화 레벨 전승 -----
            case SmithyTapType.LevelTransfer:
                if (goBlack.activeSelf || MainUI.GetInstance().tapSmithy.equipmentLevelTransfer.GetSelectMain.aInUid == cell_equip_data.aInUid ||
                    MainUI.GetInstance().tapSmithy.equipmentLevelTransfer.GetSelectMat.aInUid == cell_equip_data.aInUid)
                {

                }
                else
                {
                    MainUI.GetInstance().tapSmithy.equipmentLevelTransfer.SetInitData(cell_equip_data);
                    foreach (var item in GameObject.FindObjectsOfType<ScrollIndexCallbackSmithy>())
                        item.ReleaseEnhantTransferMat(cell_equip_data);
                }
                break;

            // ----- 장신구 옵션 변경 -----
            case SmithyTapType.OrnamentChangeOptions:
                if(GameDatabase.GetInstance().tableDB.GetIsPartsTypeAcce(cell_equip_data.eq_ty))
                {
                    if (cell_equip_data.eq_rt >= 3)
                    {
                        MainUI.GetInstance().tapSmithy.equipmentChangeOrnamentOptions.SetDatas(cell_equip_data);
                        var cell = GameObject.FindObjectsOfType<ScrollIndexCallbackSmithy>();
                        foreach (var item in cell)
                            item.CellRefreshSelectAcceChangeOption(cell_equip_data);
                    }
                    else PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("장신구 옵션 변경은 고급 등급부터 가능합니다.");
                }
                else
                {
                    if (cell_equip_data.eq_rt >= 7 && cell_equip_data.eq_legend == 1)
                    {
                        MainUI.GetInstance().tapSmithy.equipmentChangeOrnamentOptions.SetDatas(cell_equip_data);
                        var cell = GameObject.FindObjectsOfType<ScrollIndexCallbackSmithy>();
                        foreach (var item in cell)
                            item.CellRefreshSelectAcceChangeOption(cell_equip_data);
                    }
                    else PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("장비 옵션 변경은 진화된 전설 장비부터 가능합니다.");
                }
                break;

            // ----- 장신구 합성, 영웅 -> 고대 -----
            case SmithyTapType.OrnamentSynthesis:
                if (goBlack.activeSelf)
                    return;
                if (cell_equip_data.m_lck == 1)
                {
                    PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener("현재 장신구는 잠금 상태입니다.\n재료로 등록하시겠습니까?", Synthesis);
                }
                else
                {
                    Synthesis();
                }
                break;
            // ----- 장신구 승급, 고대 -> 전설 -----
            case SmithyTapType.OrnamentAdvancement:
                if (goBlack.activeSelf)
                    return;

                if(cell_equip_data.m_lck == 1)
                {
                    PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener("현재 장신구는 잠금 상태입니다.\n재료로 등록하시겠습니까?", Advancement);
                }
                else
                {
                    Advancement();
                }
                break;
        }
    }

    void Synthesis()
    {
        MainUI.GetInstance().tapSmithy.ornamentSynthesis.SetDatas(cell_equip_data);
        var eqDbMat1 = MainUI.GetInstance().tapSmithy.ornamentSynthesis.GetNowSelectEquipMat1();
        var eqDbMat2 = MainUI.GetInstance().tapSmithy.ornamentSynthesis.GetNowSelectEquipMat2();
        foreach (var item in GameObject.FindObjectsOfType<ScrollIndexCallbackSmithy>())
            item.CellRefreshSelectAcceSynthesis(eqDbMat1, eqDbMat2);
    }

    void Advancement()
    {
        MainUI.GetInstance().tapSmithy.ornamentAdvancement.SetDatas(cell_equip_data);
        var eqDbAdvMat0 = MainUI.GetInstance().tapSmithy.ornamentAdvancement.GetNowSelectEquipMat0();
        var eqDbAdvMat1 = MainUI.GetInstance().tapSmithy.ornamentAdvancement.GetNowSelectEquipMat1();
        var eqDbAdvMat2 = MainUI.GetInstance().tapSmithy.ornamentAdvancement.GetNowSelectEquipMat2();
        foreach (var item in GameObject.FindObjectsOfType<ScrollIndexCallbackSmithy>())
            item.CellRefreshSelectAcceAdvancement(eqDbAdvMat0.aInUid, eqDbAdvMat1.aInUid, eqDbAdvMat2.aInUid);
    }
}
