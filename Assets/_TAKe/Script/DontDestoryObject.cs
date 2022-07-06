using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestoryObject : MonoBehaviour
{
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }
}
