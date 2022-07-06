using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 대장간 타입 : 장비 강화, 강화석 진화, 강화 레벨 이전, 장비 옵션 변경, 장신구 합성 
/// </summary>
public enum SmithyTapType
{
    None,
    /// <summary> 대장간 -> 장비 강화 </summary>
    Enhancement,
    /// <summary> 대장간 -> 강화석 진화 </summary>
    StonEvolution,
    /// <summary> 대장간 -> 강화 레벨 전승 </summary>
    LevelTransfer,
    /// <summary> 대장간 -> 장신구 옵션 변경 </summary>
    OrnamentChangeOptions,
    /// <summary> 대장간 -> 장신구 합성 </summary>
    OrnamentSynthesis,
    /// <summary> 대장간 -> 장신구 승급 </summary>
    OrnamentAdvancement
}

public enum SmithyListType
{
    Disable,
    EQUIP_WEAPON_SHIELD,
    EQUIP_COSTUME,
    EQUIP_ACCE,
    ITEM,
    ETC
}

public class TapSmithy : MonoBehaviour
{
    [SerializeField] TapObject tapObject;

    public long tapOpenSP = 0;
    // 스크롤 셀 새로 고침 
    public delegate void SmithyScrollRefreshCellEquip();
    public SmithyScrollRefreshCellEquip smithyScrollRefreshCellEquip;
    public delegate void SmithyScrollRefreshCellItem();
    public SmithyScrollRefreshCellItem smithyScrollRefreshCellItem;


    public SmithyTapType smithyType = SmithyTapType.Enhancement;
    public SmithyListType smithyListType = SmithyListType.EQUIP_WEAPON_SHIELD;
    public InitOnStartSmithy initOnStartSmithy;

    public TapSmithyEquipmentEnhancement equipmentEnhancement;// 장비 강화 
    public TapSmithyEquipmentStonEvolution equipmentStonEvolution; // 강화석 진화 
    public TapSmithyEquipmentLevelTransfer equipmentLevelTransfer; // 강화 레벨 이전 
    public TapSmithyEquipmentChangeOrnamentOptions equipmentChangeOrnamentOptions; // 장신구 옵션 변경 
    public TapSmithyOrnamentSynthesis ornamentSynthesis; // 장신구 합성 
    public TapSmithyOrnamentAdvancement ornamentAdvancement; // 장신구 전설 승급 

    [SerializeField] Animation an_EquipAll, an_WeaponShield, an_EquipCostume, an_Acce, an_Item;

    [SerializeField] private LeftBtn[] leftBtn = new LeftBtn[5];
    [SerializeField] Color alpho_0;
    [SerializeField] Color alpho_1;
    [System.Serializable]
    public struct LeftBtn
    {
        public Image bg;
        public Image outbox;
    }

    void OnEnable()
    {
        tapObject.aniIcon.Play("MainButtonActiveOnScale");
        tapObject.txName.fontStyle = FontStyle.Bold;
        tapObject.txName.color = tapObject.onCorSelect;
        tapObject.goOutline.SetActive(true);
        tapOpenSP = GameDatabase.GetInstance().GetUniqueIDX();
    }

    void OnDisable()
    {
        tapObject.txName.fontStyle = FontStyle.Normal;
        tapObject.txName.color = tapObject.noCorSelect;
        tapObject.goOutline.SetActive(false);
        smithyType = SmithyTapType.None;
        smithyListType = SmithyListType.Disable;
    }

    public void LeftBtnState (int _num)
    {
        for (int i = 0; i < leftBtn.Length; i++)
        {
            if(i == _num)
            {
                leftBtn[i].bg.color = alpho_1;
                leftBtn[i].outbox.color = Color.red;
            }
            else
            {
                leftBtn[i].bg.color = alpho_0;
                leftBtn[i].outbox.color = Color.gray;
            }
        }
    }

    public void TapOpen(SmithyTapType _smhy_type, GameDatabase.TableDB.Equipment _equip = default, GameDatabase.TableDB.Item _item = default)
    {
        if (_smhy_type == SmithyTapType.StonEvolution) // 강화석 진화 
            PopUpMng.GetInstance().popUpInventorySort.enum_SortInvnHighLow = SortInventorytHighLow.LOW_TO_HIGH;
        else
            PopUpMng.GetInstance().popUpInventorySort.enum_SortInvnHighLow = SortInventorytHighLow.HIGH_TO_LOW;

        equipmentEnhancement.gameObject.SetActive(_smhy_type == SmithyTapType.Enhancement);
        equipmentStonEvolution.gameObject.SetActive(_smhy_type == SmithyTapType.StonEvolution);
        equipmentLevelTransfer.gameObject.SetActive(_smhy_type == SmithyTapType.LevelTransfer);
        equipmentChangeOrnamentOptions.gameObject.SetActive(_smhy_type == SmithyTapType.OrnamentChangeOptions);
        ornamentSynthesis.gameObject.SetActive(_smhy_type == SmithyTapType.OrnamentSynthesis);
        ornamentAdvancement.gameObject.SetActive(_smhy_type == SmithyTapType.OrnamentAdvancement);

        if (_smhy_type == SmithyTapType.Enhancement) // 장비 강화 
        {
            smithyType = _smhy_type;
            ButtonAniPlayType(true, _equip.eq_ty);
            LeftBtnState(0);
            equipmentEnhancement.SetData(_equip);
        }
        else if (_smhy_type == SmithyTapType.StonEvolution) // 강화석 진화 
        {
            smithyType = _smhy_type;
            ButtonAniPlayType(false);
            LeftBtnState(1);
            equipmentStonEvolution.SetData(_item);
        }
        else if (_smhy_type == SmithyTapType.LevelTransfer) // 강화 레벨 전승 
        {
            smithyType = _smhy_type;
            ButtonAniPlayType(true, _equip.eq_ty);
            LeftBtnState(2);
            equipmentLevelTransfer.SetInitData(_equip);
        }
        else if (_smhy_type == SmithyTapType.OrnamentChangeOptions) // 장신구 옵션 변경 
        {
            smithyType = _smhy_type;
            ButtonAniPlayType(true, _equip.eq_ty);
            LeftBtnState(3);
            equipmentChangeOrnamentOptions.SetDatas(_equip);
        }
        else if (_smhy_type == SmithyTapType.OrnamentSynthesis) // 장신구 합성 
        {
            smithyType = _smhy_type;
            ButtonAniPlayType(true, _equip.eq_ty);
            LeftBtnState(4);
            ornamentSynthesis.SetDatas(default, true, -1);
        }
        else if (_smhy_type == SmithyTapType.OrnamentAdvancement) // 장신구 승급 
        {
            smithyType = _smhy_type;
            ButtonAniPlayType(true, _equip.eq_ty);
            LeftBtnState(5);
            ornamentAdvancement.SetDatas(default, true, -1);
        }
    }

    private void ButtonAniPlayType (bool _equipOrItem, int _partsType = -1)
    {
        LogPrint.Print("<color=magenta> ------------ _equipOrItem : " + _equipOrItem + ", _partsType : " + _partsType + " ------------ </color>");
        if(_equipOrItem) // 장비 
        {
            if (_partsType >= 0 && _partsType <= 1) // 무기, 방패 리스트 버튼 ON 
            {
                an_EquipAll.Play("UIInventoryRightButtonOff");
                an_WeaponShield.Play("UIInventoryRightButtonOn");
                an_EquipCostume.Play("UIInventoryRightButtonOff");
                an_Acce.Play("UIInventoryRightButtonOff");

                if (smithyType == SmithyTapType.StonEvolution)
                    an_Item.Play("UIInventoryRightButtonOn");
                else an_Item.Play("UIInventoryRightButtonDisable");
            }
            else if (_partsType >= 2 && _partsType <= 7) // 방어구 리스트 버튼 ON 
            {
                an_EquipAll.Play("UIInventoryRightButtonOff");
                an_WeaponShield.Play("UIInventoryRightButtonOff");
                an_EquipCostume.Play("UIInventoryRightButtonOn");
                an_Acce.Play("UIInventoryRightButtonOff");

                if (smithyType == SmithyTapType.StonEvolution)
                    an_Item.Play("UIInventoryRightButtonOn");
                else an_Item.Play("UIInventoryRightButtonDisable");
            }
            else // 장신구 리스트 버튼 ON 
            {
                if(smithyType == SmithyTapType.OrnamentSynthesis || smithyType == SmithyTapType.OrnamentAdvancement)
                {
                    an_EquipAll.Play("UIInventoryRightButtonDisable");
                    an_WeaponShield.Play("UIInventoryRightButtonDisable");
                    an_EquipCostume.Play("UIInventoryRightButtonDisable");
                }
                else
                {
                    an_EquipAll.Play("UIInventoryRightButtonOff");
                    an_WeaponShield.Play("UIInventoryRightButtonOff");
                    an_EquipCostume.Play("UIInventoryRightButtonOff");
                }
                
                an_Acce.Play("UIInventoryRightButtonOn");

                if (smithyType == SmithyTapType.StonEvolution)
                    an_Item.Play("UIInventoryRightButtonOn");
                else an_Item.Play("UIInventoryRightButtonDisable");
            }
        }
        else // 아이템 및 기타 리스트 버튼 ON (강화석)
        {
            an_EquipAll.Play("UIInventoryRightButtonDisable");
            an_WeaponShield.Play("UIInventoryRightButtonDisable");
            an_EquipCostume.Play("UIInventoryRightButtonDisable");
            an_Acce.Play("UIInventoryRightButtonDisable");
            an_Item.Play("UIInventoryRightButtonOn");
        }
    }

    public void ClickRightTapChange(int _btnNum)
    {
        LogPrint.EditorPrint(" -------------- _btnNum " + _btnNum + "---------------");
        if (_btnNum == 1) // 무기, 방패    // 장비 강화, 강화 레벨 이전 일때에만 클릭 됨 
        {
            if (smithyType == SmithyTapType.Enhancement || smithyType == SmithyTapType.LevelTransfer || smithyType == SmithyTapType.OrnamentChangeOptions)
            {
                if(smithyListType != SmithyListType.EQUIP_WEAPON_SHIELD)
                {
                    ButtonAniPlayType(true, 0);
                    smithyListType = SmithyListType.EQUIP_WEAPON_SHIELD;
                    MainUI.GetInstance().tapSmithy.initOnStartSmithy.SetInit();
                }
            }
        }
        else if (_btnNum == 2) // 방어구   // 장비 강화, 강화 레벨 이전 일때에만 클릭 됨 
        {
            if (smithyType == SmithyTapType.Enhancement || smithyType == SmithyTapType.LevelTransfer || smithyType == SmithyTapType.OrnamentChangeOptions)
            {
                if (smithyListType != SmithyListType.EQUIP_COSTUME)
                {
                    ButtonAniPlayType(true, 2);
                    smithyListType = SmithyListType.EQUIP_COSTUME;
                    MainUI.GetInstance().tapSmithy.initOnStartSmithy.SetInit();
                }
            }
        }
        else if (_btnNum == 3) // 장신구    // 장비 강화, 강화 레벨 이전, 장비 옵션, 장신구 합성 일때에만 클릭 됨 
        {
            if (smithyType == SmithyTapType.Enhancement || smithyType == SmithyTapType.LevelTransfer || smithyType == SmithyTapType.OrnamentChangeOptions || smithyType == SmithyTapType.OrnamentSynthesis)
            {
                if (smithyListType != SmithyListType.EQUIP_ACCE)
                {
                    ButtonAniPlayType(true, 8);
                    smithyListType = SmithyListType.EQUIP_ACCE;
                    MainUI.GetInstance().tapSmithy.initOnStartSmithy.SetInit();
                }
            }
        }
        else if (_btnNum == 4) // 아이템 및 기타  // 강화석 진화 일때에만 클릭 됨  
        {
            if (smithyType == SmithyTapType.StonEvolution) 
            {
                if (smithyListType != SmithyListType.ITEM)
                {
                    ButtonAniPlayType(false, -1);
                    smithyListType = SmithyListType.ITEM;
                    MainUI.GetInstance().tapSmithy.initOnStartSmithy.SetInit();
                }
            }
        }
    }

    public void ClickBottomSmithyTypeTap (int _iSmy)
    {
        if (_iSmy == 1) // 대장간 타입 : [장비 강화] 탭으로 변경 
        {
            if(smithyType != SmithyTapType.Enhancement)
            {
                smithyListType = SmithyListType.EQUIP_WEAPON_SHIELD;
                TapOpen(SmithyTapType.Enhancement, new GameDatabase.TableDB.Equipment() { eq_ty = 0 }, default);
                MainUI.GetInstance().tapSmithy.initOnStartSmithy.SetInit();
            }
        }
        else
        if (_iSmy == 2) // 대장간 타입 : [강화석 진화] 탭으로 변경 
        {
            if(smithyType != SmithyTapType.StonEvolution)
            {
                smithyListType = SmithyListType.ITEM;
                TapOpen(SmithyTapType.StonEvolution, new GameDatabase.TableDB.Equipment() { eq_ty = 0 }, default);
                MainUI.GetInstance().tapSmithy.initOnStartSmithy.SetInit();
            }
        }
        else if (_iSmy == 3) // 대장간 타입 : [장비 레벨 이전] 탭으로 변경 
        {
            if (smithyType != SmithyTapType.LevelTransfer)
            {
                smithyListType = SmithyListType.EQUIP_WEAPON_SHIELD;
                TapOpen(SmithyTapType.LevelTransfer, new GameDatabase.TableDB.Equipment() { eq_ty = 0 }, default);
                MainUI.GetInstance().tapSmithy.initOnStartSmithy.SetInit();
            }
        }
        else if (_iSmy == 4) // 대장간 타입 [장신구 옵션 변경] 탭으로 변경 
        {
            if (smithyType != SmithyTapType.OrnamentChangeOptions)
            {
                smithyListType = SmithyListType.EQUIP_ACCE;
                TapOpen(SmithyTapType.OrnamentChangeOptions, new GameDatabase.TableDB.Equipment() { eq_ty = 8 }, default);
                MainUI.GetInstance().tapSmithy.initOnStartSmithy.SetInit();
            }
        }
        else if (_iSmy == 5) // 대장간 타입 : [장신구 합성] 탭으로 변경 
        {
            if (smithyType != SmithyTapType.OrnamentSynthesis)
            {
                smithyListType = SmithyListType.EQUIP_ACCE;
                TapOpen(SmithyTapType.OrnamentSynthesis, new GameDatabase.TableDB.Equipment() { eq_ty = 8 }, default);
                MainUI.GetInstance().tapSmithy.initOnStartSmithy.SetInit();
            }
        }
        else if (_iSmy == 6)
        {
            if (smithyType != SmithyTapType.OrnamentAdvancement)
            {
                smithyListType = SmithyListType.EQUIP_ACCE;
                TapOpen(SmithyTapType.OrnamentAdvancement, new GameDatabase.TableDB.Equipment() { eq_ty = 8 }, default);
                MainUI.GetInstance().tapSmithy.initOnStartSmithy.SetInit();
            }
        }
    }
}
