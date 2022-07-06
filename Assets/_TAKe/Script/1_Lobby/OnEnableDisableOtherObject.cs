using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnEnableDisableOtherObject : MonoBehaviour
{
    public List<GameObject> onObject;
    public List<GameObject> offObject;

    void OnEnable()
    {
        foreach (var item in onObject)
        {
            item.SetActive(true);
        }
    }

    void OnDisable()
    {
        foreach (var item in offObject)
        {
            item.SetActive(false);
        }
    }
}
