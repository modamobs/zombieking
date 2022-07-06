using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SG;

[RequireComponent(typeof(UnityEngine.UI.LoopScrollRect))]
[DisallowMultipleComponent]
public class InitOnStartEquipLegend : MonoBehaviour
{
    [SerializeField] LoopScrollRect ls;

    public int totalCount = -1;

    public void SetInit() // isRefillCells : true -> 셀 전체 새로 고침, false : 셀 현재 위치에서 새로 고침 
    {
        //GameDatabase.GetInstance().tableDB.TempInventoryList();
        totalCount = PopUpMng.GetInstance().popUpEquipLegendUpdrage.GetCount;
        ls.totalCount = totalCount;
        ls.RefreshCells();
        //MainUI.GetInstance().inventory.InventoryLevelCount();
    }
}
