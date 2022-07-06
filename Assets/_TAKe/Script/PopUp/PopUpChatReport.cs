using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using BackEnd;
using LitJson;
using System.Threading.Tasks;

public class PopUpChatReport : MonoBehaviour
{
    [SerializeField] Text txNickName;
    [SerializeField] InputField inputField;
    [SerializeField] ToggleGroup toggles;


    public void SetData(string nickName)
    {
        txNickName.text = nickName;
        inputField.text = "";
        foreach(Toggle toggle in toggles.ActiveToggles())
        {
            toggle.isOn = false;
        }
    }

    public async void Click_ReportSend()
    {
        string reason = string.Empty;
        foreach (Toggle toggle in toggles.ActiveToggles())
        {
            Text text = toggle.GetComponentInChildren<Text>();
            if (!string.IsNullOrEmpty(text.text))
            {
                reason = text.text;
                break;
            }
        }

        if (string.IsNullOrEmpty(reason))
        {
            PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("신고 사유를 체크해주세요.");
            return;
        }

        if (string.IsNullOrEmpty(txNickName.text))
        {
            PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("신고 대상이 없습니다.");
            return;
        }

        if (inputField.text.Length <= 0)
        {
            PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("신고 내용을 작성해주세요.");
            return;
        }

        BackendReturnObject bro1 = null;
        SendQueue.Enqueue(Backend.Chat.ReportUser, txNickName.text, reason, inputField.text, callback => { bro1 = callback; });
        while (Loading.Bottom(bro1) == false) { await Task.Delay(100); }
        if (bro1.IsSuccess())
        {
            PopUpMng.GetInstance().Open_MessageNotif("신고 내용이 정상적으로 전송되었습니다.");
            gameObject.SetActive(false);
        }
        else
        {
            PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("오류가 발생하였습니다. 다시 시도해주세요.");
        }
    }
}
