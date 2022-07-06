using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollCellTipAll : MonoBehaviour
{
    [SerializeField] Text txt;

    void ScrollCellIndex(int idx)
    {
        txt.text = string.Format("<color=yellow>TIP{0}.</color> {1}", idx + 1, MainUI.GetInstance().tapNoticeTipMessage.GetTip(idx));
    }
}
