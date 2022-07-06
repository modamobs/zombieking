using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnPop : MonoBehaviour
{
    public bool isLockEscape = false;
    void OnEnable() => PopUpMng.GetInstance().AddOnPop(this);

    void OnDisable() => PopUpMng.GetInstance().RmvOnPop(this);
}
