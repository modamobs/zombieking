using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoockAtZombieCamera : MonoBehaviour
{
    void Update()
    {
        //transform.LookAt(Camera.main.transform.position);
        transform.LookAt(GameMng.GetInstance ().zbCam.trCam.position);
    }
}
