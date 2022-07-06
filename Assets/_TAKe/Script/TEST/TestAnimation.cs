using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public class TestAnimation : MonoBehaviour
{
    int ID = 1;
    [SerializeField] Animator ani;
    [SerializeField] Text strId;

    void Start()
    {
        ani.SetBool("Walk", false);
        ani.Play("Idle");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            ani.Play("SKILL_TEST");
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            ID--;
            if (ID <= 0)
                ID = 1;

            ani.Play("SKILL_" + ID);
            strId.text = string.Format("ATTACK_{0}", ID);
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            ani.Play("SKILL_" + ID);
            strId.text = string.Format("ATTACK_{0}", ID);
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            ID++;
            if (ID > 40)
                ID = 1;

            ani.Play("SKILL_" + ID);
            strId.text = string.Format("ATTACK_{0}", ID);
        }
    }
}
