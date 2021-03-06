using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SG;

[RequireComponent(typeof(UnityEngine.UI.LoopScrollRect))]
[DisallowMultipleComponent]

public class InitOnStartAttendance : MonoBehaviour
{
    public int totalCount = -1;

    public void SetInit()
    {
        totalCount = GameDatabase.GetInstance().attendanceDB.GetCount();
        var ls = GetComponent<LoopScrollRect>();
        ls.totalCount = totalCount;
        ls.RefillCells();
    }
}
