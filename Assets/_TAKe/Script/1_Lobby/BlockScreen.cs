using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class BlockScreen : MonoBehaviour
{
    [SerializeField] GameObject goCenter;
    [SerializeField] GameObject goText;
    [SerializeField] CanvasGroup cavg_Center;

    public void OnEnable ()
    {
        goCenter.SetActive(true);
        goText.SetActive(false);

        if(cavg_Center)
            cavg_Center.alpha = 1;
    }

    public void CenterAlphaZero()
    {
        if(cavg_Center)
            cavg_Center.alpha = 0;
    }

    public void CenterObjectEnable()
    {
        goCenter.SetActive(true);
    }

    public void CenterObjectDisable()
    {
        goCenter.SetActive(false);
    }

    public async void OnText(float delay)
    {
        await Task.Delay((int)(delay * 1000));
        goText.SetActive(true);
    }

    public void OnCloseText()
    {
        goText.SetActive(false);
    }
}
