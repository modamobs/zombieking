using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SG;

[RequireComponent(typeof(UnityEngine.UI.LoopScrollRect))]
[DisallowMultipleComponent]
public class InitOnStartInventoryAll : MonoBehaviour
{
    [SerializeField] LoopScrollRect ls;
    /// <summary>
    /// 셀 새로 고침 
    /// </summary>
    public delegate void RefreshCell();
    public RefreshCell regreshCell;

    public int totalCount = -1;

    public void SetInit(bool isRefillCells = false) // isRefillCells : true -> 셀 전체 새로 고침, false : 셀 현재 위치에서 새로 고침 
    {
        LogPrint.Print("enum_SortInventory : " + PopUpMng.GetInstance().popUpInventorySort.enum_SortInventory + ", enum_SortInvnHighLow : " + PopUpMng.GetInstance().popUpInventorySort.enum_SortInvnHighLow);

        MainUI.GetInstance().tapSmithy.smithyListType = SmithyListType.Disable;
        GameDatabase.GetInstance().tableDB.TempInventoryList();
        totalCount = GameDatabase.GetInstance().inventoryDB.GetLevelInvenCount() / 5;
        ls.totalCount = totalCount;

        if (isRefillCells)
            ls.RefillCells();
        else
            ls.RefreshCells();

        MainUI.GetInstance().inventory.InventoryLevelCount();
    }
}
