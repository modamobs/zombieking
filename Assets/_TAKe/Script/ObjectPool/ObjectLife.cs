using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ObjectLife : MonoBehaviour
{
    [SerializeField] bool isNoReset = false;
    [SerializeField] bool init = false;
    [SerializeField] bool isPool = true;
    [SerializeField] public string opName;
    [SerializeField] private float lifeTime;
    [SerializeField] private TextMeshPro textMeshPro;
    [SerializeField] private TextMesh textMesh;
    [SerializeField] Animator anitor;

    private void OnEnable()
    {
        if (lifeTime > 0)
        {
            Invoke("VoLife", lifeTime);
        }

        if(init && isNoReset == false)
            ObjectPool.GetInstance().onActiveLife.Add(this);

        init = true;
    }

    public void ResetActive ()
    {
        ObjectPool.GetInstance().PushToPool(opName, this);
    }

    public void VoLife ()
    {
        if (gameObject.activeSelf && isPool)
        {
            if (init && isNoReset == false)
            {
                ObjectPool.GetInstance().onActiveLife.Remove(this);
            }
                
            ObjectPool.GetInstance().PushToPool(opName, this);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    public void TextView (string txt, Color _cor)
    {
        if (textMeshPro)
        {
            textMeshPro.text = txt;
            textMeshPro.color = _cor;
        }
        else
        {
            if (textMesh)
            {
                textMesh.text = txt;
                textMesh.color = _cor;
            }
        }
    }

    public Animator GetAnimator ()
    {
        if (anitor != null)
            return anitor;

        return null;
    }
}



