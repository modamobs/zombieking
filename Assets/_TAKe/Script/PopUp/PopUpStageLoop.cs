using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpStageLoop : MonoBehaviour
{
    [SerializeField] InitOnStartStageLoop initOnStartStageLoop;

    public void SetData()
    {
        initOnStartStageLoop.SetInit();
    }
}
