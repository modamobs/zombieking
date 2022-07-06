using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadSceneMng : MonoSingleton<LoadSceneMng>
{
    public SceneIndex sceneIndex = SceneIndex.Init;
    public GameObject imProgressBarRoot;
    public Image imProgressBar;
    public Text txProgressPerTxt;
    public Text txProgressSubTxt;

    public enum SceneIndex
    {
        Init,
        BattleMode
    }

    void Awake ()
    {
        DontDestroyOnLoad(this);
    }

    public void LoadScene()
    {
        StartCoroutine("CoLoadScene");
    }

    IEnumerator CoLoadScene()
    {
        yield return null;
        imProgressBar.fillAmount = 0f;
        imProgressBarRoot.gameObject.SetActive(false);
        if (SceneManager.GetActiveScene().buildIndex == (int)sceneIndex)
        {
            Debug.LogError("현재buildIndex == 로드sceneIndex");
            yield break;
        }

        yield return new WaitForSeconds(.25f);
        float timer = 0.0f;
        imProgressBar.fillAmount = 0f;
        imProgressBarRoot.gameObject.SetActive(true);
        txProgressPerTxt.text = "";

        AsyncOperation op = SceneManager.LoadSceneAsync((int)sceneIndex);
        op.allowSceneActivation = false;

        while (!op.isDone)
        {
            yield return null;

            timer += Time.deltaTime;
            if (SceneManager.GetActiveScene().buildIndex != (int)sceneIndex)
            {
                txProgressPerTxt.text = string.Format("{0}%", (imProgressBar.fillAmount * 100f).ToString("N2"));
                if (op.progress < 0.9f)
                {
                    imProgressBar.fillAmount = Mathf.Lerp(imProgressBar.fillAmount, op.progress, timer);
                    if (imProgressBar.fillAmount >= op.progress)
                    {
                        timer = 0f;
                    }
                }
                else
                {
                    imProgressBar.fillAmount = Mathf.Lerp(imProgressBar.fillAmount, 1f, timer);
                    if (imProgressBar.fillAmount >= 1.0f)
                    {
                        txProgressPerTxt.text = "100%";
                        yield return new WaitForSeconds(.25f);
                        op.allowSceneActivation = true;
                        yield break;
                    }
                }
            }
        }
    }
}
