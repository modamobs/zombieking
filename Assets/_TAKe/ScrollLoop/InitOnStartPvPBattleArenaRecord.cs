using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SG;

[RequireComponent(typeof(UnityEngine.UI.LoopScrollRect))]
[DisallowMultipleComponent]

public class InitOnStartPvPBattleArenaRecord : MonoBehaviour
{
    [SerializeField] LoopScrollRect ls;
    [SerializeField] int totalCount = 0;

    public void SetInit()
    {
        LogPrint.Print("PvpBTLRecordCount : " + GameDatabase.GetInstance().pvpBattleRecord.PvpBTLRecordCount);
        int rcd_cnt = GameDatabase.GetInstance().pvpBattleRecord.PvpBTLRecordCount;
        totalCount = rcd_cnt > 20 ? GameDatabase.PvPBattleRecord.max_storage : rcd_cnt;
        ls.totalCount = totalCount;
        ls.RefillCells();
    }
}
