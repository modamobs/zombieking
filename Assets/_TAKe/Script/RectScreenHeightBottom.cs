using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RectScreenHeightBottom : MonoBehaviour
{
    [SerializeField] RectTransform Rect;
    [SerializeField] float defaultBottom;
    void Start()
    {
        float fScaleWidth = ((float)Screen.width / (float)Screen.height) / ((float)9 / (float)16);
        float top = (defaultBottom * (defaultBottom / (defaultBottom * fScaleWidth)));
        Rect.offsetMin = new Vector2(Rect.offsetMax.x, top);
    }
}
