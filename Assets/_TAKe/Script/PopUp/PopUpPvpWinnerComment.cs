using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopUpPvpWinnerComment : MonoBehaviour
{
    [SerializeField] InputField inputField;
    public void SetData()
    {
        inputField.text = "";
    }

    /// <summary> 멘트 변경 </summary>
    public void Click_Confirm(InputField inpf)
    {
        string cmt = inpf.text.ToString();
        if (cmt.Length > 0)
        {
            LogPrint.Print("cmt.Length  : " + cmt.Length);
            if(cmt.Length > 20)
            {
                PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("멘트는 최대 20자 이내로 입력해주세요.");
            }
            else
            {
                PlayerPrefs.SetString(PrefsKeys.key_PvPArenaComment, inputField.text.ToString());
                MainUI.GetInstance().tapDungeon.Refresh_Commnet();
                gameObject.SetActive(false);
            }
        }
        else PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("멘트를 입력해주세요.");
    }
}
