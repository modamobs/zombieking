using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TopMessageLog : MonoBehaviour
{
    public GameObject popBoard;
    public Text txt;

    void Start()
    {
        HidePop();
    }

    public void ShowPop(string _str, float _time = 4f)
    {
        txt.text = _str;
        popBoard.SetActive(true);
        CancelInvoke();
        Invoke("HidePop", _time);
    }

    public void HidePop()
    {
        CancelInvoke();
        popBoard.SetActive(false);
    }
}
