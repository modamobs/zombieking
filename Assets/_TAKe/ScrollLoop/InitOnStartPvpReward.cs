using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SG;
using LitJson;

[RequireComponent(typeof(UnityEngine.UI.LoopScrollRect))]
[DisallowMultipleComponent]

public class InitOnStartPvpReward : MonoBehaviour
{
    [SerializeField] LoopScrollRect ls;
    public int totalCount = -1;

    public void SetData (List<PopUpPvpBattleResult.PvpReward> rewards)
    {
        totalCount = rewards.Count;
        ls.totalCount = totalCount;
        ls.RefillCells();
    }
}
