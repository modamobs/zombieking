using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(UnityEngine.UI.LoopScrollRect))]
[DisallowMultipleComponent]
public class InitOnStartChatBlock : MonoBehaviour
{
    public int totalCount = -1;
    public void SetInit()
    {
        totalCount = GameDatabase.GetInstance().chat.GetBlockCount();
        var ls = GetComponent<LoopScrollRect>();
        ls.totalCount = totalCount;
        ls.RefillCells();
    }
}