using BackEnd;
using BackEnd.Tcp;
using LitJson;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ChatScript : MonoBehaviour
{

    ModalPanel modalPanel;

    public GameObject guild_chat_alert_panel;
    public GameObject guild_chat_panel;
    public Text guild_chat_alert;

    public Toggle publicFilteringOn;
    //public Toggle guildFilteringOn;

    public Toggle WhisperTab;
    public Toggle PublicTab;

    public InputField publicMessage;
    public InputField guildMessage;
    public InputField whisperNickname;
    public InputField whisperMessage;
    int maxLength = 225;

    static string BlockFailMsg = "<color=red>존재하지 않는 닉네임입니다.</color>";
    static string BlockSuccessMsg = "<color=red>{0}님의 채팅을 차단합니다.</color>";
    static string UnBlockFailMsg = "<color=red>차단 목록에 존재하지 않는 닉네임입니다.</color>";
    static string UnBlockSuccessMsg = "<color=red>{0}님을 차단해제합니다.</color>";

    static string CmdErrMsg = "<color=red>잘못된 명령어 입니다.</color>";
    static string NullMsg = "<color=red>대상을 입력해주세요.</color>";
    static string WhisperNicknameNullMsg = "귓속말 대상을 입력해주세요.</color>";
    static string MessageNullMsg = "메세지를 입력해주세요.";
    static string MessageLengthExceedMsg = "입력 제한 글자수를 초과하였습니다.";
    static string WhisperNicknameErrorMsg = "자기자신에게 귓속말을 할 수 없습니다.";
    static string GlobalChatErrorMsg = "운영자가 아닙니다.";
    string mNickname;

    private static ChatScript chatScript;

    public static ChatScript Instance()
    {
        if (!chatScript)
        {
            chatScript = FindObjectOfType(typeof(ChatScript)) as ChatScript;
            if (!chatScript)
                Debug.LogWarning("There needs to be one active chatScript script on a GameObject in your scene.");
        }

        return chatScript;
    }
    // Use this for initialization
    void Start()
    {
        modalPanel = ModalPanel.Instance();
        BackendReturnObject bro = Backend.BMember.GetUserInfo();
        if (bro.IsSuccess())
        {
            JsonData nicknameJson = bro.GetReturnValuetoJSON()["row"]["nickname"];
            if (nicknameJson != null)
                mNickname = nicknameJson.ToString();
        }

        Backend.Chat.SetFilterReplacementChar('뀨');

        publicFilteringOn.onValueChanged.AddListener(value =>
        {
            Debug.Log(value);
            Backend.Chat.SetFilterUse(value);
        });

        publicMessage.onEndEdit.AddListener(delegate {
                PublicMessageEnterChat();
        });
        guildMessage.onEndEdit.AddListener(delegate { PublicMessageEnterChat(); });
        whisperNickname.onEndEdit.AddListener(delegate { PublicMessageEnterChat(); });
        whisperMessage.onEndEdit.AddListener(delegate { PublicMessageEnterChat(); });

        StartCoroutine("Loop");
    }
    IEnumerator Loop()
    {
        WaitForSeconds wait = new WaitForSeconds(0.25f);
        if (guild_chat_alert_panel.activeInHierarchy)
        {
            // guild chat unavailable
            if (PlayerPrefs.GetInt("guildStatus") == 0)
            {
                guild_chat_alert.gameObject.SetActive(true);
                guild_chat_panel.SetActive(false);
            }
            else
            {
                guild_chat_alert.gameObject.SetActive(false);
                guild_chat_panel.SetActive(true);
            }
            yield return null;
        }
    }


    //void Update()
    //{
    //    //if (Input.GetKeyDown(KeyCode.Escape))
    //    //{
    //    //    Debug.Log("LeaveChannel");
    //    //    LeaveChannel();
    //    //}

    //    if (guild_chat_alert_panel.activeInHierarchy)
    //    {
    //        // guild chat unavailable
    //        if (PlayerPrefs.GetInt("guildStatus") == 0)
    //        {
    //            guild_chat_alert.gameObject.SetActive(true);
    //            guild_chat_panel.SetActive(false);
    //        }
    //        else
    //        {
    //            guild_chat_alert.gameObject.SetActive(false);
    //            guild_chat_panel.SetActive(true);
    //        }
    //    }

    //    // 엔터 
    //    //if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.JoystickButton10))
    //    //{
    //    //    PublicMessageEnterChat();
    //    //}
    //}


    public void PublicChat()
    {
        Chat(ChannelType.Public, publicMessage.text);
    }

    /// <summary>
    /// 유저 직접 채팅 엔터 메시지 
    /// </summary>
    public void PublicMessageEnterChat()
    {
#if UNITY_EDITOR
        LogPrint.Print("PublicMessageEnterChat 유저 직접 채팅 엔터 메시지 ");
#endif

        Chat(ChannelType.Public, publicMessage.text, 1);
    }

    /// <summary>
    /// 아이템 획득 메시지 
    /// </summary>
    public void PublicEquipDropChat (string itemDropMsg)
    {
        Chat(ChannelType.Public, itemDropMsg, 2);
    }

    public void GuildChat()
    {
        Chat(ChannelType.Guild, guildMessage.text);
    }

    public void WhisperChat()
    {
        if (whisperNickname.text.Length > 0)
        {
            Chat(ChannelType.Public, whisperMessage.text, whisperNickname.text, true);
        }
        else
        {
            if (IsBlockCmd(whisperMessage.text) || IsUnblockCmd(whisperMessage.text))
                Chat(ChannelType.Public, whisperMessage.text, whisperNickname.text, true);
            else
                ShowModal(WhisperNicknameNullMsg);
        }
    }

    // public / guild chat
    private void Chat(ChannelType type, string message)
    {
        Chat(type, message, string.Empty, false);
    }

    private void Chat(ChannelType type, string message, int msgType)
    {
        Chat(type, message, string.Empty, false, msgType);
    }

    /// <summary>
    /// 채팅 보낼 때 
    /// </summary>
    private void Chat(ChannelType type, string message, string toNickname, bool IsWhisper, int msgType = 0)
    {
        if (Backend.IsInitialized)
        {
            // 글자수가 최대 글자수 넘는지 확인
            if (GetStringByte(message) > maxLength)
            {
                // 오류 모달
                ShowModal(MessageLengthExceedMsg);
            }
            // 글자수가 0 이상인 경우 채팅을 보냄
            else if (message.Length > 0)
            {
                string json_msg = GameDatabase.GetInstance().chat.ChatSendMessageJson(msgType, message);
                string send_message = JsonUtility.ToJson(new ChatMsgItem() { MsgType = msgType, Contents = json_msg }); ;

                if (IsWhisper)
                    whisperMessage.text = string.Empty;
                else if (type == ChannelType.Public)
                    publicMessage.text = string.Empty;
                else
                    guildMessage.text = string.Empty;

                // command 명령어인 경우
                if (message.StartsWith("/"))
                {
                    string[] messageSplit = message.Split(' ');

                    // public / guild 채널에서 명령어로 귓속말을 입력한 경우
                    if (IsWhisperCmd(messageSplit[0]))
                    {
                        //string[] messageSplit = message.Split(' ');
                        if (messageSplit.Length > 2)
                        {
                            string nickname = messageSplit[1];
                            if (nickname.Equals(mNickname))
                            {
                                ChatItem chatItem = new ChatItem(SessionInfo.None, ChatScroll.Instance().infoText, WhisperNicknameErrorMsg, false);
                                ChatScroll.Instance().PopulateAll(chatItem);
                                return;
                            }
                            else
                            {
                                var wMesssageStart = messageSplit[0].Length + messageSplit[1].Length + 2;
                                if (wMesssageStart < message.Length)
                                {
                                    string wMessage = message.Substring(wMesssageStart);
                                    Backend.Chat.Whisper(nickname, wMessage);
                                    return;
                                }
                            }
                        }
                        CmdError(NullMsg);
                        return;
                    }
                    //글로벌쳇 보내기
                    if (IsGlobalChatCmd(messageSplit[0]))
                    {
                        if (messageSplit.Length > 1)
                        {
                            string nickname = messageSplit[1];

                            Backend.Chat.ChatToGlobal(messageSplit[1]); // 운영자가 아니면 OnGlobalChat에서 에러로 리턴됩니다
                            return;
                        }
                    }


                    // 명령어로 차단해제한 경우 
                    if (IsUnblockCmd(messageSplit[0]))
                    {
                        //string[] messageSplit = message.Split(' ');
                        if (messageSplit.Length > 1)
                        {
                            string nickname = messageSplit[1];
                            if (!string.IsNullOrEmpty(nickname))
                            {
                                UnBlockUser(nickname);
                                return;
                            }
                        }
                        CmdError(NullMsg);
                        return;
                    }

                    // 명령어로 차단한 경우 
                    if (IsBlockCmd(messageSplit[0]))
                    {
                        //string[] messageSplit = message.Split(' ');
                        if (messageSplit.Length > 1)
                        {
                            string nickname = messageSplit[1];
                            if (!string.IsNullOrEmpty(nickname))
                            {
                                BlockUser(nickname);
                                return;
                            }
                        }
                        CmdError(NullMsg);
                        return;
                    }

                    // 명령어로 신고한 경우
                    if (IsReportCmd(messageSplit[0]))
                    {
                        if (messageSplit.Length > 1)
                        {
                            string nickname = messageSplit[1];
                            if (!string.IsNullOrEmpty(nickname))
                            {
                                ReportUser(nickname);
                                return;
                            }
                        }
                        CmdError(NullMsg);
                        return;
                    }

                    // 존재하지 않는 명령어를 입력한 경우
                    CmdError(CmdErrMsg);
                    return;
                }

                // whisper pannel 에서 입력한 경우 
                if (!string.IsNullOrEmpty(toNickname))
                {
                    Backend.Chat.Whisper(toNickname, send_message);
                    return;
                }

                // public / guild pannel 에서 일반 메세지를 입력한 경우
                switch (type)
                {
                    case ChannelType.Public:
                        Backend.Chat.ChatToChannel(ChannelType.Public, send_message);
                        break;
                    case ChannelType.Guild:
                        Backend.Chat.ChatToChannel(ChannelType.Guild, send_message);
                        break;
                }
            }
            // 메세지를 입력하지 않은 경우 
            else
            {
                // 오류 모달
                ShowModal(MessageNullMsg);
            }
        }
    }

    private bool IsWhisperCmd(string message)
    {
        return message.Equals("/ㄱ")
                    || message.Equals("/w")
                    || message.Equals("/귓");
    }

    private bool IsBlockCmd(string message)
    {
        return message.Equals("/b")
                    || message.Equals("/차단");
    }

    private bool IsUnblockCmd(string message)
    {
        return message.Equals("/ub")
                      || message.Equals("/차단해제");
    }

    private bool IsReportCmd(string message)
    {
        return message.Equals("/신고");
    }
    private bool IsGlobalChatCmd(string message)
    {
        return message.Equals("/all");
    }

    private void CmdError(string message)
    {
        ChatItem chatItem = new ChatItem(SessionInfo.None, ChatScroll.Instance().infoText, message, false);
        ChatScroll.Instance().PopulateAll(chatItem);
    }

    // 귓속말 탭으로 이동 (nickname 설정)
    internal void SetWhisperTabOn(string nickname)
    {
        WhisperTab.isOn = true;
        whisperNickname.text = nickname;
    }

    private void ShowModal(string message)
    {
        modalPanel.AlertShow(message);
    }

    private int GetStringByte(string message)
    {
        return System.Text.Encoding.Unicode.GetByteCount(message);
    }

    // 채널 나가기 
    public void LeaveChannel()
    {
        // 길드가 존재하는 경우 -> LeaveGuildChannel;
        if (PlayerPrefs.GetInt("guildStatus") != 0)
            Backend.Chat.LeaveChannel(ChannelType.Guild);
        Backend.Chat.LeaveChannel(ChannelType.Public);
        publicMessage.text = guildMessage.text = whisperNickname.text = whisperMessage.text = string.Empty;
    }

    internal void BlockUser(string nickname)
    {
        LogPrint.Print("BlockUser : " + nickname);
        ChatItem chatItem;
        Backend.Chat.BlockUser(nickname, blockCallback =>
        {
            // 성공
            if (blockCallback)
            {
                chatItem = new ChatItem(SessionInfo.None, ChatScroll.Instance().infoText, string.Format(BlockSuccessMsg, nickname), false);
                GameDatabase.GetInstance().chat.UserBlock(nickname);
            }
            // 실패
            else
            {
                chatItem = new ChatItem(SessionInfo.None, ChatScroll.Instance().infoText, BlockFailMsg, false);
            }

            ChatScroll.Instance().PopulateAll(chatItem);
        });
    }

    internal void UnBlockUser(string nickname)
    {
        LogPrint.Print("UnBlockUser : " + nickname);
        ChatItem chatItem;

        // 성공
        if (Backend.Chat.UnblockUser(nickname))
        {
            chatItem = new ChatItem(SessionInfo.None, ChatScroll.Instance().infoText, string.Format(UnBlockSuccessMsg, nickname), false);
        }
        // 실패
        else
        {
            chatItem = new ChatItem(SessionInfo.None, ChatScroll.Instance().infoText, UnBlockFailMsg, false);
        }

        GameDatabase.GetInstance().chat.UserUnBlock(nickname);
        ChatScroll.Instance().PopulateAll(chatItem);
    }

    internal void ReportUser(string nickname)
    {
        LogPrint.Print("ReportUser : " + nickname);
        ChatItem chatItem;
        Backend.Social.GetGamerIndateByNickname(nickname, callback =>
        {
            if (callback.IsSuccess())
            {
                LitJson.JsonData rows = callback.GetReturnValuetoJSON()["rows"];
                if (rows.Count > 0)
                {
                    ParticipantsModal.Instance().SetSelectedNickname(nickname);
                    ParticipantsModal.Instance().OnReport();
                }
                else
                {
                    chatItem = new ChatItem(SessionInfo.None, ChatScroll.Instance().infoText, BlockFailMsg, false);
                    ChatScroll.Instance().PopulateAll(chatItem);
                }
            }
            else
            {
                chatItem = new ChatItem(SessionInfo.None, ChatScroll.Instance().infoText, callback.GetMessage(), false);
                ChatScroll.Instance().PopulateAll(chatItem);
            }
        });
    }
}

