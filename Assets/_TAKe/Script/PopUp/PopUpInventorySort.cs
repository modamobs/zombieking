using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 인벤토리의 정렬 팝업 
/// </summary>
public class PopUpInventorySort : MonoBehaviour
{
    public SortInventorytHighLow enum_SortInvnHighLow = SortInventorytHighLow.HIGH_TO_LOW;
    public SortInventory enum_SortInventory = SortInventory.NEW;

    //bool init_toggle = false;
    [SerializeField] Toggle to_high, to_low;
    [SerializeField] GameObject[] go_sortGroups; // 0:전체, 1:장비, 2:아이템(기타)

    [SerializeField] InfoSort[] infoSort;
    [System.Serializable]
    struct InfoSort
    {
        public Image[] im_sort_btn_bg;
        public Text[] tx_sort;
    }

    [SerializeField] Color cr_select, cr_none;

    [SerializeField] ToggleGroup togGroup;

    public void Init (bool _init = false)
    {
        var invn_type = MainUI.GetInstance().inventory.inventoryType;
        if (_init)
        {
            bool is_invn_type_equip = invn_type == Inventory.InventoryType.EQUIP_WEAPON_SHIELD || invn_type == Inventory.InventoryType.EQUIP_COSTUME || invn_type == Inventory.InventoryType.EQUIP_ACCE;
            // 정렬 내림 or 오름 차순 
            string sort_hl_key = is_invn_type_equip == true ? PrefsKeys.prky_SortInventorytHighLow_equip : PrefsKeys.prky_SortInventorytHighLow_etc;
            enum_SortInvnHighLow = (SortInventorytHighLow)PlayerPrefs.GetInt(sort_hl_key);
            to_high.isOn = enum_SortInvnHighLow == SortInventorytHighLow.HIGH_TO_LOW;
            to_low.isOn = enum_SortInvnHighLow == SortInventorytHighLow.LOW_TO_HIGH;

            // 정렬 종류 
            string sort_key = is_invn_type_equip ? PrefsKeys.prky_SortInventory_equip : PrefsKeys.prky_SortInventory_etc;
            enum_SortInventory = (SortInventory)PlayerPrefs.GetInt(sort_key);
        }

        int invn_type_nbr = invn_type == Inventory.InventoryType.ALL || invn_type == Inventory.InventoryType.ITEM ? 0 : 1;
        for (int i = 0; i < 2; i++)
            go_sortGroups[i].SetActive(i == invn_type_nbr);

        for (int i = 0; i < infoSort[invn_type_nbr].im_sort_btn_bg.Length; i++)
        {
            if(enum_SortInventory == (SortInventory)i)
            {
                infoSort[invn_type_nbr].tx_sort[i].color = Color.white;
                infoSort[invn_type_nbr].im_sort_btn_bg[i].color = cr_select;
            }
            else
            {
                infoSort[invn_type_nbr].tx_sort[i].color = Color.gray;
                infoSort[invn_type_nbr].im_sort_btn_bg[i].color = cr_none;
            }
        }

        //init_toggle = true;
    }

    public void ClickSort (int nbr)
    {
        enum_SortInventory = (SortInventory)nbr;
        Init();
    }

    public void ClickSort()
    {
        int chk = -1;
        foreach (Toggle toggle in togGroup.ActiveToggles())
        {
            if(chk == -1)
            {
                string strTmp = Regex.Replace(toggle.name, @"\D", "");
                if (!string.IsNullOrEmpty(strTmp))
                {
                    chk = int.Parse(strTmp);
                    enum_SortInvnHighLow = (SortInventorytHighLow)chk;
                    break;
                }
            }
        }

        var invn_type = MainUI.GetInstance().inventory.inventoryType;
        bool is_invn_type_equip = invn_type == Inventory.InventoryType.EQUIP_WEAPON_SHIELD || invn_type == Inventory.InventoryType.EQUIP_COSTUME || invn_type == Inventory.InventoryType.EQUIP_ACCE;

        string sort_hl_key = is_invn_type_equip == true ? PrefsKeys.prky_SortInventorytHighLow_equip : PrefsKeys.prky_SortInventorytHighLow_etc;
        PlayerPrefs.SetInt(sort_hl_key, (int)enum_SortInvnHighLow);
        
        string sort_key = is_invn_type_equip == true ? PrefsKeys.prky_SortInventory_equip : PrefsKeys.prky_SortInventory_etc;
        PlayerPrefs.SetInt(sort_key, (int)enum_SortInventory);

        MainUI.GetInstance().inventory.initOnStartInventoryAll.SetInit(true);
        gameObject.SetActive(false);
    }
}
