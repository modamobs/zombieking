using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SG;

[RequireComponent(typeof(UnityEngine.UI.LoopScrollRect))]
[DisallowMultipleComponent]

public class InitOnStartPet : MonoBehaviour
{
    public int totalCount = -1;

    public void SetInit(bool isRefillCells = true)
    {
        totalCount = GameDatabase.GetInstance().tableDB.GetPetCount(MainUI.GetInstance().tapPet.petTapType);
        var ls = GetComponent<LoopScrollRect>();
        ls.totalCount = totalCount;

        if (isRefillCells)
        {
            ls.RefillCells();
        }
        else
        {
            ls.RefreshCells();
        }
    }
}
