using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SG;

[RequireComponent(typeof(UnityEngine.UI.LoopScrollRect))]
[DisallowMultipleComponent]

public class InitOnStartPvPBattleArenaRank : MonoBehaviour
{
    [SerializeField] LoopScrollRect ls;
    [SerializeField] int totalCount = 0;
    public void SetInit()
    {
        var db = GameDatabase.GetInstance().rankDB.GetListRTRankPvpBTLArenaRank();
        totalCount = db == null ? 0 : db.Count;
        ls.totalCount = totalCount;
        ls.RefillCells();
    }
}
