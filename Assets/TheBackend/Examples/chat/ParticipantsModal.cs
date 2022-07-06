using BackEnd;
using BackEnd.Tcp;
using UnityEngine;
using UnityEngine.UI;

public class ParticipantsModal : MonoBehaviour
{
    public GameObject UserPanel;
    public GameObject UserPanelModal;

    public GameObject ReportPanel;
    public Text ReportNickname;
    public ToggleGroup reportReasons;
    public InputField reportDetails;
    public Button reportCancel;

    public string SelectedNickname { get; private set; }
    private static string ReportSuccessMsg = "신고가 완료되었습니다. 내역 검토 후 적절한 조치가 취해집니다.";
    private static string ReportDetailsLengthMsg = "신고내용을 500자 이내로 작성해주세요.";
    private static string ReportNicknameErrorMsg = "잘못된 닉네임 입니다.";
    private static string ReportDetailEmptyMsg = "신고내용을 입력해주세요.";
    private static ParticipantsModal modalPanel;
    private Text blockText;
    private static string BlockMsg = "차단";
    private static string UnBlockMsg = "차단해제";

    private Toggle[] toggles = null;

    public static ParticipantsModal Instance()
    {
        if (!modalPanel)
        {
            modalPanel = FindObjectOfType(typeof(ParticipantsModal)) as ParticipantsModal;
            if (!modalPanel)
                Debug.LogWarning("There needs to be one active ParticipantsModal script on a GameObject in your scene.");
        }
        return modalPanel;
    }

    public void participantPanelShow(string nickname, Vector2 mousePos)
    {
        PopUpMng.GetInstance().OpenUserInfo(nickname, false, Backend.Chat.IsUserBlocked(nickname));

        /*
        LogPrint.Print("participantPanelShow nickname : " + nickname);
        // set position 
        var modalWidth = UserPanelModal.transform.GetComponent<RectTransform>().rect.size.x;
        var modalHeight = UserPanelModal.transform.GetComponent<RectTransform>().rect.size.y / 2;
        //mousePos = new Vector2(mousePos.x + modalWidth, mousePos.y - modalHeight);
        //UserPanelModal.transform.position = mousePos;

        // set blockText 
        if (blockText == null)
        {
            foreach (Text text in UserPanelModal.GetComponentsInChildren<Text>())
            {
                if (text.name.Equals("block"))
                {
                    blockText = text;
                    break;
                }
            }
        }

        if (blockText != null)
        {
            // check user has been blocked and set the message
            if (Backend.Chat.IsUserBlocked(nickname))
            {
                blockText.text = UnBlockMsg;
            }
            else
            {
                blockText.text = BlockMsg;
            }
        }


        UserPanel.SetActive(true);
        SelectedNickname = nickname;
        */
    }

    public void OnWhisper()
    {
        ChatScript.Instance().SetWhisperTabOn(SelectedNickname);
        CloseParticipantPanel();
    }

    public void SetSelectedNickname(string nickname)
    {
        SelectedNickname = string.Copy(nickname);
    }

    public void OnReport()
    {
        //CloseParticipantPanel();
        //ReportNickname.text = SelectedNickname;
        //ReportPanel.SetActive(true);

        //reportCancel.onClick.AddListener(CloseReportPanel);

        //if (toggles == null)
        //{
        //    toggles = reportReasons.GetComponentsInChildren<Toggle>();
        //}
    }

    public void ReportUser()
    {
        string reason = string.Empty;
        foreach (Toggle toggle in reportReasons.ActiveToggles())
        {
            Text text = toggle.GetComponentInChildren<Text>();
            if (!string.IsNullOrEmpty(text.text))
            {
                reason = text.text;
                break;
            }
        }

        if (reportDetails.text.Length > 500)
        {
            ModalPanel.Instance().AlertShow(ReportDetailsLengthMsg);
        }
        else
        {
            Backend.Chat.ReportUser(SelectedNickname, reason, reportDetails.text, report =>
           {
               Debug.Log(report);
               if (report.IsSuccess())
               {
                   ChatItem chatItem = new ChatItem(SessionInfo.None, ChatScroll.Instance().infoText, ReportSuccessMsg, true);
                   ChatScroll.Instance().PopulateAll(chatItem);
                   CloseReportPanel();
               }
               else
               {
                   Debug.Log(report);
                   string alertMsg = string.Empty;
                   if (report.GetStatusCode().Equals("404"))
                   {
                       alertMsg = ReportNicknameErrorMsg;
                   }
                   else if (report.GetStatusCode().Equals("400"))
                   {
                       if (report.GetMessage().Contains("bad details of report"))
                       {
                           alertMsg = ReportDetailEmptyMsg;
                       }
                       else if (report.GetMessage().Contains("bad details"))
                       {
                           alertMsg = ReportDetailsLengthMsg;
                       }
                   }

                   if (!string.IsNullOrEmpty(alertMsg))
                   {
                       ModalPanel.Instance().AlertShow(alertMsg);
                   }
                   else
                   {
                       ModalPanel.Instance().AlertShow(report.GetMessage());
                   }

               }
           });

        }
    }

    private void CloseReportPanel()
    {
        //if (ReportPanel != null)
        //{
        //    // reason : defualt 설정
        //    if (toggles.Length > 0)
        //    {
        //        toggles[toggles.Length - 1].isOn = true;
        //    }
        //    reportDetails.text = string.Empty;
        //    ReportPanel.SetActive(false);
        //}
    }

    public void OnBlock()
    {
        CloseParticipantPanel();
        if (blockText != null)
        {
            if (blockText.text.Equals(BlockMsg))
            {
                ChatScript.Instance().BlockUser(SelectedNickname);
            }
            else
            {
                ChatScript.Instance().UnBlockUser(SelectedNickname);
            }
        }
        else
            ChatScript.Instance().BlockUser(SelectedNickname);
    }

    // 이외의 바깥쪽 터치시 모달패널 닫힘
    public void CloseParticipantPanel()
    {
        //if (UserPanel != null)
        //{
        //    UserPanel.SetActive(false);
        //}
    }

    public void CloseAll()
    {
        CloseParticipantPanel();
        CloseReportPanel();
    }

}