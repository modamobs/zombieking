using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SG;

[RequireComponent(typeof(UnityEngine.UI.LoopScrollRect))]
[DisallowMultipleComponent]
public class InitOnStartEquipmentEncyclopedia : MonoBehaviour
{
    public int onRt = 0;
    [SerializeField] LoopScrollRect ls;
    public int totalCount = -1;

    public void Init(int rt = 0)
    {
        if(rt == 0 && onRt == 0)
            onRt = 1;
        else if(rt >= 1)
            onRt = rt;

        totalCount = GameDatabase.GetInstance().equipmentEncyclopediaDB.GetCount(onRt);
        ls.totalCount = totalCount;
        ls.RefillCells();
    }

    public void RefreshCells(bool isRefreshCells)
    {
        RefreshCells(onRt, isRefreshCells);
    }

    /// <summary>
    /// 위치 변화 없이 셀 새로고침 
    /// </summary>
    public void RefreshCells(int rt, bool isRefreshCells)
    {
        if (onRt == rt)
        {
            if (isRefreshCells == false)
                ls.RefillCells();
            else ls.RefreshCells();

            PopUpMng.GetInstance().popUpEquipmentEncyclopedia.AllEnhantBtn(true);
        }
    }
}
