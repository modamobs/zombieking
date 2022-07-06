using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class PopUpAttendanceBook : MonoBehaviour
{
    [SerializeField] InitOnStartAttendance initOnStartAttendance;

    [SerializeField] UIInfo uiInfo;
    [System.Serializable]
    struct UIInfo
    {
        public bool isRewardToDay;
        public Image im_OkBtnBg1;
        public Color co_ok, co_no;
    }

    public bool GetRewrdToDay() => uiInfo.isRewardToDay;

    public async void SetData()
    {
        Task<bool> tsk1 = GameDatabase.GetInstance().attendanceDB.GetIsCheckAttendance();
        while (Loading.Full(tsk1.IsCompleted) == false) await Task.Delay(100);

        uiInfo.isRewardToDay = tsk1.Result;
        uiInfo.im_OkBtnBg1.sprite = SpriteAtlasMng.GetInstance().GetSpriteButtonRedOrGray(uiInfo.isRewardToDay);
        initOnStartAttendance.SetInit();
    }

    /// <summary>
    /// 출석 보상 받기 
    /// </summary>
    public async void Click_AttendanceOk ()
    {
        if (uiInfo.isRewardToDay)
        {
            Task tsk1 = GameDatabase.GetInstance().attendanceDB.SetConfirmAttendance();
            while (Loading.Full(tsk1.IsCompleted) == false) await Task.Delay(100);

            SetData();
        }
    }
}
