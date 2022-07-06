using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RectScreenHeightTop : MonoBehaviour
{
    [SerializeField] RectTransform RectBottom;
    [SerializeField] float defaultTop;

    void Start()
    {
        float fScaleWidth = ((float)Screen.width / (float)Screen.height) / ((float)9 / (float)16);
        float top = (defaultTop * (defaultTop / (defaultTop * fScaleWidth)));
        RectBottom.offsetMax = new Vector2(RectBottom.offsetMax.x, -top);
    }
}
