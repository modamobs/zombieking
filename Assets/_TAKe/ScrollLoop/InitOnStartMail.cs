using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SG;

[RequireComponent(typeof(UnityEngine.UI.LoopScrollRect))]
[DisallowMultipleComponent]

public class InitOnStartMail : MonoBehaviour
{
    public int totalCount = -1;

    public void SetInit(int toCnt)
    {
        totalCount = toCnt;
        var ls = GetComponent<LoopScrollRect>();
        ls.totalCount = totalCount;
        ls.RefreshCells();
    }
}
