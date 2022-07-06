using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SG;

[RequireComponent(typeof(UnityEngine.UI.LoopScrollRect))]
[DisallowMultipleComponent]

public class InitOnStartRank : MonoBehaviour
{
    [SerializeField] bool isRTRank = false;
    [SerializeField] int totalCount = 0;
    public void SetInit()
    {
        totalCount = GameDatabase.GetInstance().rankDB.GetDicRTRank().Count;

        var ls = GetComponent<LoopScrollRect>();
        ls.totalCount = totalCount;
        ls.RefillCells();
    }
}
