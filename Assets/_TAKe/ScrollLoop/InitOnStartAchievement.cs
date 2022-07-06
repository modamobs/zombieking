using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SG;

[RequireComponent(typeof(UnityEngine.UI.LoopScrollRect))]
[DisallowMultipleComponent]

public class InitOnStartAchievement : MonoBehaviour
{
    public int totalCount = -1;

    public void SetInit(PopUpAchievement.Type type, bool refresh)
    {
        totalCount = type == PopUpAchievement.Type.DailyMission ? GameDatabase.GetInstance ().dailyMissionDB.GetChartCount() : GameDatabase.GetInstance ().achievementsDB.GetChartCount();
        var ls = GetComponent<LoopScrollRect>();
        ls.totalCount = totalCount;

        if (!refresh)
            ls.RefillCells();
        else
            ls.RefreshCells();
    }
}
