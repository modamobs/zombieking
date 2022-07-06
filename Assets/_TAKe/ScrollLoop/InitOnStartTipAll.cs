using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SG;
using System.Threading.Tasks;

[RequireComponent(typeof(UnityEngine.UI.LoopScrollRect))]
[DisallowMultipleComponent]
public class InitOnStartTipAll : MonoBehaviour
{
    [SerializeField] LoopScrollRect ls;
    public int totalCount = -1;

    public void Start()
    {
        totalCount = MainUI.GetInstance().tapNoticeTipMessage.GetTipCount();
        ls.totalCount = totalCount;
        ls.RefillCells();
    }
}
