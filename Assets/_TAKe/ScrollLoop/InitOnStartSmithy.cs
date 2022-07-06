using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(UnityEngine.UI.LoopScrollRect))]
[DisallowMultipleComponent]
public class InitOnStartSmithy : MonoBehaviour
{
    public UnityAction cellAction;

    /// <summary>
    /// 셀 새로 고침 
    /// </summary>
    public delegate void RefreshCell();
    public RefreshCell regreshCell;

    public int totalLineCount = -1;
    [SerializeField] LoopScrollRect ls;

    public void SetInit(bool _listRefresh = true)
    {
        cellAction = null;
        MainUI.GetInstance().inventory.inventoryType = Inventory.InventoryType.Disable;

        if (_listRefresh)
        {
            GameDatabase.GetInstance().tableDB.TempInventoryList();
        }

        totalLineCount = (int)System.Math.Ceiling((double)GameDatabase.GetInstance().tableDB.GetTempCount() / 5);
        if (totalLineCount < 5)
            totalLineCount = 5;

        ls.totalCount = totalLineCount;
        ls.RefillCells();
    }


    //ScrollIndexCallbackSmithy[] findVars;
    //void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.B))
    //    {
    //        GameDatabase.GetInstance().tableDB.TempInventoryList();
    //    }

    //    if (Input.GetKeyDown(KeyCode.N))
    //    {
    //        totalLineCount = GameDatabase.GetInstance().tableDB.GetTempCount();
    //        ls.totalCount = totalLineCount;
    //        ls.RefillCells();
    //    }

    //    if (Input.GetKeyDown(KeyCode.M))
    //    {
    //        LogPrint.Print(findVars == null);
    //        if (findVars == null)
    //            findVars = FindObjectsOfType<ScrollIndexCallbackSmithy>();

    //        RefeshCell();

    //        //StartCoroutine("RCell");
    //    }
    //}

    //IEnumerator RCell()
    //{
    //    int i = 0;
    //    foreach (var item in findVars)
    //    {
    //        item.AScrollCellIndex(i);
    //        i++;
    //        yield return null;
    //    }
    //}

    //async void RefeshCell()
    //{
    //    int i = 0;
    //    foreach (var item in findVars)
    //    {
    //        item.SendMessage("ScrollCellIndex", i);
    //        //item.ScrollCellIndex(i);
    //        i++;
    //        await Task.Delay(1);
    //    }
    //}
}
