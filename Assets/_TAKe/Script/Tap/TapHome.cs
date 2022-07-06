using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TapHome : MonoBehaviour
{
    [SerializeField] TapObject tapObject;


    void Awake()
    {

    }

    void OnEnable()
    {
        tapObject.aniIcon.Play("MainButtonActiveOnScale");
        tapObject.txName.fontStyle = FontStyle.Bold;
        tapObject.txName.color = tapObject.onCorSelect;
        tapObject.goOutline.SetActive(true);
    }

    void OnDisable()
    {
        tapObject.txName.fontStyle = FontStyle.Normal;
        tapObject.txName.color = tapObject.noCorSelect;
        tapObject.goOutline.SetActive(false);
    }
}
