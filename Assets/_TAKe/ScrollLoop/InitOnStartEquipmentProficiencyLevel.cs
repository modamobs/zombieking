using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SG;

[RequireComponent(typeof(UnityEngine.UI.LoopScrollRect))]
[DisallowMultipleComponent]

public class InitOnStartEquipmentProficiencyLevel : MonoBehaviour
{
    public int totalCount = -1;

    public void SetInit()
    {
        totalCount = 11;
        var ls = GetComponent<LoopScrollRect>();
        ls.totalCount = totalCount;
        ls.RefreshCells();
    }
}
