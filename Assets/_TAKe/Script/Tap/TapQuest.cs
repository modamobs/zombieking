using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class TapQuest : MonoBehaviour
{
    public Animation aniTap;
    public LevelType levelType = LevelType.TYPE_1Lv;
    [SerializeField] UIInfo uiInfo;
    [System.Serializable]
    class UIInfo
    {
        public ScrollCellQuest[] scrCellQuest;
    }

    public enum LevelType
    {
       TYPE_1Lv = 0, TYPE_10LV = 1, TYPE_100LV = 2
    }
    
    void Start()
    {
        InvokeRepeating("RefreshCheck", 0.1f, 1.0f);
    }

    public void Init()
    {
        levelType = (LevelType)PlayerPrefs.GetInt(PrefsKeys.key_quest_leve_lup_type);

        foreach (var cell in uiInfo.scrCellQuest)
        {
            cell.ScrollCellIndex(true);
        }
    }

    /// <summary>
    /// 레벨업 타입 변굥 100LV, 10LV, 1LV 
    /// </summary>
    public void Click_LevelUpType(int type)
    {
        levelType = (LevelType)type;
        PlayerPrefs.SetInt(PrefsKeys.key_quest_leve_lup_type, (int)levelType);

        foreach (var cell in uiInfo.scrCellQuest)
            cell.ScrollCellIndex(false);
    }

    public void RefreshCheck()
    {
        foreach (var cell in uiInfo.scrCellQuest)
            cell.CheckUpBtn();
    }
}