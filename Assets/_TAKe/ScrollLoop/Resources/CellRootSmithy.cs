using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class CellRootSmithy : MonoBehaviour
{
    [SerializeField] ScrollIndexCallbackSmithy[] cell;
    //async void ScrollCellIndex(int idx)
    //{
    //    for (int i = 0; i < cell.Length; i++)
    //    {
    //        cell[i].CellIndex(idx);
    //        await Task.Delay(1);
    //    }
    //}

    void ScrollCellIndex(int idx)
    {
        for (int i = 0; i < cell.Length; i++)
        {
            cell[i].CellIndex(idx);
        }
    }
}
