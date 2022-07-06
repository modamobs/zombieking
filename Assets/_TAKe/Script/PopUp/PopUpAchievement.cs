using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopUpAchievement : MonoBehaviour
{
    [SerializeField] InitOnStartAchievement initOnStartAchievement;
    public Type type = Type.DailyMission;
    public enum Type
    {
        DailyMission,
        Achievement
    }

    [SerializeField] UI ui;
    [System.Serializable]
    struct UI
    {
        public Color coOkDaily, coOkAchie, coAlphaZero;
        public Image imBtnBgDaily;
        public Image imBtnBgAchie;
    }

    public void SetData(bool refresh = false)
    {
        if(type == Type.DailyMission)
        {
            ui.imBtnBgDaily.color = ui.coOkDaily;
            ui.imBtnBgAchie.color = ui.coAlphaZero;
            GameDatabase.GetInstance().dailyMissionDB.Sort();
        }
        else if(type == Type.Achievement)
        {
            ui.imBtnBgDaily.color = ui.coAlphaZero;
            ui.imBtnBgAchie.color = ui.coOkAchie;
            GameDatabase.GetInstance().achievementsDB.Sort();
        }

        initOnStartAchievement.SetInit(type, refresh);
    }

    public void Click_Daily()
    {
        if (type != Type.DailyMission)
        {
            type = Type.DailyMission;
            SetData();
        }
    }

    public void Click_Achie()
    {

        if (type != Type.Achievement)
        {
            type = Type.Achievement;
            SetData();
        }
    }
}
