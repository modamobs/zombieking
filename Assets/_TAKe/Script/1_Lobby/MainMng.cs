using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMng : MonoSingleton<MainMng>
{
    

    // Start is called before the first frame update
    void Start()
    {
        //QualitySettings.SetQualityLevel(2);
    }

    public void SetQualityLevel (Text txt)
    {
        int nc = QualitySettings.GetQualityLevel();
        nc++;

        if (nc > 5)
            nc = 0;

        QualitySettings.SetQualityLevel(nc);
        txt.text = string.Format("SetQualityLevel {0}", nc.ToString());
    }

    public void PlayBattleMode () 
    {
        LoadSceneMng.GetInstance().sceneIndex = LoadSceneMng.SceneIndex.BattleMode;
        LoadSceneMng.GetInstance().LoadScene();
    }

    
}
