using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPet : MonoBehaviour
{
    [SerializeField] GameObject goRoot;
    [SerializeField] Transform tr;
    [SerializeField] Transform target = null;
    [SerializeField] Animator ani;
    [Header("펫 3d 목록")]
    // 펫 목록 
    public List<Pets> pets = new List<Pets>();

    IEnumerator Start()
    {
        Setting();
        WaitForSeconds sec1 = new WaitForSeconds(0.1f);
        while (true)
        {
            if(GameObject.Equals(GameMng.GetInstance().myPZ, null))
            {
                if (goRoot.activeSelf)
                    goRoot.SetActive(false);
            }
            else
            {
                if (!goRoot.activeSelf)
                    goRoot.SetActive(true);

                tr.position = GameMng.GetInstance().myPZ.tr.trfPetInitPos.position;
                target = GameMng.GetInstance().myPZ.tr.transf;
                while (!GameObject.Equals(target, null) && target.gameObject.activeSelf && ani != null)
                {
                    if(GameMng.GetInstance().myPZ.zbType == IG.MonsterType.NONE)
                    {
                        if (ani.GetCurrentAnimatorStateInfo(0).IsName("Die") == false)
                        {
                            SetInt("animation,5");
                            target = null;
                        }
                    }
                    else
                    {
                        if (GameMng.GetSqr(tr.position, target.position) > 1.5f) // 좀비를 따라다닌다. 
                        {
                            if (ani.GetCurrentAnimatorStateInfo(0).IsName("Move") == false)
                            {
                                SetInt("animation,2");
                            }

                            tr.position = Vector3.Lerp(tr.position, target.position, GameMng.GetInstance().GameDeltaTime);
                        }
                        else
                        {
                            if (ani.GetCurrentAnimatorStateInfo(0).IsName("Idle") == false)
                            {
                                SetInt("animation,1");
                            }
                        }
                    }

                    yield return null;
                }
            }

            yield return sec1;
        }
    }

    public void Setting()
    {
        var usePet = GameDatabase.GetInstance().tableDB.GetUsePet();
        // 펫 3d on off 
        foreach (var item in pets)
        {
            if (int.Equals(item.pet_rt, usePet.p_rt))
            {
                foreach (var item2 in item.petIdObjs)
                {
                    if(int.Equals(item2.pet_id, usePet.p_id))
                    {
                        item2.goPet.SetActive(true);
                        ani = item2.aniPet;
                        SetInt("animation,1");
                    }
                    else
                    {
                        item2.goPet.SetActive(false);
                    }
                }
            }
            else
            {
                foreach (var item2 in item.petIdObjs)
                {
                    if (item2.goPet.activeSelf)
                        item2.goPet.SetActive(false);
                }
            }
        }
    }

    public void SetFloat(string parameter = "key,value")
    {
        if (goRoot.activeSelf)
        {
            char[] separator = { ',', ';' };
            string[] param = parameter.Split(separator);
            string name = param[0];
            float value = (float)Convert.ToDouble(param[1]);
            ani.SetFloat(name, value);
        }
    }
    public void SetInt(string parameter = "key,value")
    {
        if (goRoot.activeSelf)
        {
            char[] separator = { ',', ';' };
            string[] param = parameter.Split(separator);
            string name = param[0];
            int value = Convert.ToInt32(param[1]);
            ani.SetInteger(name, value);
        }
    }

    public void SetBool(string parameter = "key,value")
    {
        if (goRoot.activeSelf)
        {
            char[] separator = { ',', ';' };
            string[] param = parameter.Split(separator);
            string name = param[0];
            bool value = Convert.ToBoolean(param[1]);
            ani.SetBool(name, value);
        }
    }

    public void SetTrigger(string parameter = "key,value")
    {
        if (goRoot.activeSelf)
        {
            char[] separator = { ',', ';' };
            string[] param = parameter.Split(separator);
            string name = param[0];
            ani.SetTrigger(name);
        }
    }
}
