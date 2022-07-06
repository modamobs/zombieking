using BackEnd;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class PopUpMail : MonoBehaviour
{
    [SerializeField] InitOnStartMail initOnStartMail;

    [SerializeField] UIInfo uiInfo;
    [System.Serializable]
    struct UIInfo
    {
        public bool isReward;
        public GameObject go_NoMail;
        public Image im_GetAllOkBtnBg1;
        public Color co_ok, co_no;
        public GameObject goBtnAllReward;
    }

    public void SetData()
    {
        int mCnt = GameDatabase.GetInstance().mailDB.GetCount();
        uiInfo.isReward = mCnt > 0;
        uiInfo.im_GetAllOkBtnBg1.color = uiInfo.isReward ? uiInfo.co_ok : uiInfo.co_no;
        uiInfo.go_NoMail.SetActive(!uiInfo.isReward);
        initOnStartMail.SetInit(mCnt);
        uiInfo.goBtnAllReward.SetActive(mCnt > 0);
    }

    // 모두 받기 
    public void OnClick_GetAll()
    {
        if (uiInfo.isReward)
        {

        }
    }

    /// <summary>
    /// 메일 보상 받기 
    /// </summary>
    public async void GetRewarded(string indate)
    {
        Task tsk1 = GameDatabase.GetInstance().mailDB.AGetRewarded(indate);
        while (Loading.Full(tsk1.IsCompleted) == false) await Task.Delay(100);

        SetData();
    }

    public async void GetAllRewarded()
    {
        Task tsk1 = GameDatabase.GetInstance().mailDB.AGetAllRewarded();
        while (Loading.Full(tsk1.IsCompleted) == false) await Task.Delay(100);

        SetData();
    }
}
