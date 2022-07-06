using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SG;
using System.Threading.Tasks;

[RequireComponent(typeof(UnityEngine.UI.LoopScrollRect))]
[DisallowMultipleComponent]

public class InitOnStartStageLoop : MonoBehaviour
{
    [SerializeField] LoopScrollRect ls;
    public int totalCount = -1;

    public async void SetInit()
    {
        while (GameMng.GetInstance().mode_type == IG.ModeType.CHANGE_WAIT)
            await Task.Delay(100);

        totalCount = GameDatabase.GetInstance().chartDB.GetChapterLoopIdCnt();
        ls.totalCount = totalCount;
        ls.RefillCells();
    }
}
