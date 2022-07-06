using UnityEngine;
using BackEnd;
using System;
using UnityEngine.SceneManagement;
using BackEnd.Tcp;
using System.Threading.Tasks;

public class ChatManager : MonoBehaviour
{
    private ChatItem chatItem;
    public GameObject chatObject;

    int sceneIdx;
    bool ChatJoind = false;

    // Use this for initialization
    void Start()
    {
        Backend.Initialize(ChatHandlers);
        sceneIdx = SceneManager.GetActiveScene().buildIndex;
        GameDatabase.GetInstance().chat.GetUserBlockList();
        QueueLoop();
    }

    async void QueueLoop()
    {
        while (Application.isPlaying)
        {
            Backend.Chat.Poll();
            await Task.Delay(250);
        }
    }

    //protected void Update()
    //{
    //    Backend.Chat.Poll();
    //    //if (Input.GetKeyDown(KeyCode.Escape))
    //    //{
    //    //    if (ChatJoind)
    //    //    {
    //    //        Backend.Chat.LeaveChannel(ChannelType.Public);
    //    //    }
    //    //    else
    //    //    {
    //    //        SceneManager.LoadScene(sceneIdx - 1);
    //    //    }
    //    //}
    //}

    void ChatHandlers()
    {
        Backend.Chat.SetRepeatedChatBlockMessage("도배하면 안돼요.");
        //Backend.Chat.SetTimeoutMessage("챗 안해서 쫒아냅니다.");
        Backend.Chat.SetTimeoutMessage("채팅 서버와 세션이 만료되었습니다.");

        ChatScroll chatScroll = ChatScroll.Instance();
        ChatParticipantsScroll participantsScroll = ChatParticipantsScroll.Instance();

        /// <summary>
        /// ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
        /// 채널에 입장시 해당 채널에 접속해있는 모든 게이머들의 정보이며, 입장시 최초 한번 콜백
        /// ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
        /// </summary>
        #region 
        Backend.Chat.OnSessionListInChannel = (args) =>
        {
            participantsScroll.public_participants.Clear();
            // 게이머 정보를 참여자 리스트에 추가
            foreach (SessionInfo session in args.SessionList)
            {
                participantsScroll.public_participants.Add(session);
            }
            // 참여자 목록 출력
            participantsScroll.PublicPopulateList();
        };
        #endregion

        /// <summary>
        /// ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
        /// 채널에 입장시 해당 길드 채널에 접속해있는 모든 게이머들의 정보이며, 입장시 최초 한번 콜백
        /// ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
        /// </summary>
        #region 
        Backend.Chat.OnSessionListInGuildChannel = (args) =>
        {
            participantsScroll.guild_participants.Clear();
            // 게이머 정보를 참여자 리스트에 추가
            foreach (SessionInfo session in args.SessionList)
            {
                participantsScroll.guild_participants.Add(session);
            }
            // 참여자 목록 출력
            participantsScroll.GuildPopulateList();
        };
        #endregion

        /// <summary>
        /// ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
        /// 공개 채팅 룸에 입장 혹은 다른 게이머가 룸에 입장하면 콜백
        /// ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
        /// </summary>
        #region 
        Backend.Chat.OnJoinChannel = (JoinChannelEventArgs args) =>
        {
            Debug.Log(string.Format("OnJoinChannel {0}", args.ErrInfo));
            Debug.Log("args.Session.IsRemote : " + args.Session.IsRemote);
            if (args.ErrInfo == ErrorInfo.Success)
            {
                // 내가 접속한 경우 
                if (!args.Session.IsRemote)
                {
                    chatObject.SetActive(true);
                    ChatJoind = true;

                    // 접속할 때 마다 필터링 리스트 확인
                    Backend.Chat.SetFilterUse(ChatScript.Instance().publicFilteringOn.isOn);

                }

                // 접속한 세션이 참여자에 존재하지 않을 때 -> 추가
                if (!participantsScroll.public_participants.Contains(args.Session))
                {
                    participantsScroll.public_participants.Add(args.Session);
                    participantsScroll.PublicPopulate(args.Session);

                    // 입장 안내 메세지 view
                    //chatItem = new ChatItem(SessionInfo.None, chatScroll.infoText, string.Format("{0}이 입장했습니다.", args.Session.NickName), false);
                    //chatScroll.PopulatePublicChat(chatItem);
                }
            }
            else
            {
                ShowModal(string.Format("OnJoinChannel: {0}", args.ErrInfo.Reason));
            }
        };
        #endregion

        /// <summary>
        /// ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
        /// 길드 채팅 룸에 입장 혹은 다른 게이머가 룸에 입장하면 콜백
        /// ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
        /// </summary>
        #region 
        Backend.Chat.OnJoinGuildChannel = (JoinChannelEventArgs args) =>
        {
            Debug.Log(string.Format("OnJoinGuildChannel {0}", args.ErrInfo));

            if (args.ErrInfo == ErrorInfo.Success)
            {
                // 접속한 세션이 참여자에 존재하지 않을 때 -> 추가
                if (!participantsScroll.guild_participants.Contains(args.Session))
                {
                    participantsScroll.guild_participants.Add(args.Session);
                    participantsScroll.GuildPopulate(args.Session);

                    //chatItem = new ChatItem(SessionInfo.None, chatScroll.infoText, string.Format("{0}이 입장했습니다.", args.Session.NickName), false);
                    //chatScroll.PopulateGuildChat(chatItem);
                }
            }
            else
            {
                ChatScript.Instance().guild_chat_alert.gameObject.SetActive(true);
                ChatScript.Instance().guild_chat_panel.SetActive(false);
                ShowModal(string.Format("OnJoinGuildChannel: {0}", args.ErrInfo.Reason));
            }
        };
        #endregion

        /// <summary>
        /// ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
        /// 공개 채널에서 퇴장 혹은 다른 게이머가 채널에서 퇴장하면 콜백
        /// ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
        /// </summary>
        #region
        Backend.Chat.OnLeaveChannel = (LeaveChannelEventArgs args) =>
        {
            Debug.Log(string.Format("<color=yellow>OnLeaveChannel {0}</color>", args.ErrInfo));
            // 퇴장한 사람을 참여자 목록에서 삭제
            participantsScroll.PublicDePopulate(args.Session);

            // 내가 채널에서 퇴장한 경우
            Debug.Log("<color=yellow>--------내가 채널에서 퇴장한 경우---------</color>");
            if (!args.Session.IsRemote)
            {
                try
                {
                    Debug.Log("<color=yellow>--------1---------</color>");
                    //활성상태인 모달이 존재하는 경우, 모두 닫기 
                    ParticipantsModal.Instance().CloseAll();
                }
                finally
                {
                    Debug.Log("<color=yellow>--------2---------</color>");
                    ChatScript.Instance().PublicTab.isOn = true;
                    chatObject.SetActive(false);
                    ChatJoind = false;
                    Debug.Log("<color=yellow>--------3---------</color>");
                    chatScroll.RemoveAllPublicListViewItem();
                    Debug.Log("<color=yellow>--------4---------</color>");
                    ChannelListScript.Instance().GetChatStatus();// 재입장 
                    Debug.Log("<color=yellow>--------5---------</color>");
                }
            }
            else // 다른사람이 채널에서 퇴장한 경우
            {
                LogPrint.EditorPrint("<color=yellow>--------다른사람이 채널에서 퇴장한 경우---------</color>" + string.Format("{0}이 퇴장했습니다.", args.Session.NickName));
                //chatItem = new ChatItem(SessionInfo.None, chatScroll.infoText, string.Format("{0}이 퇴장했습니다.", args.Session.NickName), false);
                //chatScroll.PopulatePublicChat(chatItem);
            }
        };
        #endregion

        /// <summary>
        /// ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
        /// 길드 채널에서 퇴장 혹은 다른 게이머가 채널에서 퇴장하면 콜백
        /// ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
        /// </summary>
        #region
        Backend.Chat.OnLeaveGuildChannel = (LeaveChannelEventArgs args) =>
        {
            Debug.Log(string.Format("OnLeaveGuildChannel {0}", args.ErrInfo));
            // 퇴장한 사람을 참여자 목록에서 삭제
            participantsScroll.GuildDePopulate(args.Session);
            // 내가 채널에서 퇴장한 경우
            if (!args.Session.IsRemote)
            {
                chatScroll.RemoveAllGuildListViewItem();


            }
            // 다른사람이 채널에서 퇴장한 경우
            else
            {
                //chatItem = new ChatItem(SessionInfo.None, chatScroll.infoText, string.Format("{0}이 퇴장했습니다.", args.Session.NickName), false);
                //chatScroll.PopulateGuildChat(chatItem);
            }
        };
        #endregion

        /// <summary>
        /// ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
        /// //클라이언트의 네트워크 상황이 좋지않거나, 어떠한 이유로 인해 Poll 함수가 주기적으로 호출되지 못한 경우 서버에서 해당 유저를 오프라인하고 이 때 콜백되는 함수
        /// ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
        /// </summary>
        #region 
        Backend.Chat.OnSessionOfflineChannel = (SessionOfflineEventArgs args) =>
        {
            Debug.Log(string.Format("OnSessionOfflineChannel {0}", args.ErrInfo));
            // 퇴장한 사람을 참여자 목록에서 삭제
            participantsScroll.PublicDePopulate(args.Session);
            // 내가 채널에서 팅겼을 경우
            if (!args.Session.IsRemote)
            {
                //chatItem = new ChatItem(SessionInfo.None, chatScroll.infoText, "OnSessionOfflineChannel : 일시적으로 접속이 끊겼습니다", false);
                //chatScroll.PopulatePublicChat(chatItem);
                ChannelListScript.Instance().GetChatStatus();// 재입장 
            }
            // 다른사람이 채널에서 팅겼을 경우
            else
            {
                //chatItem = new ChatItem(SessionInfo.None, chatScroll.infoText, string.Format("{0}이 OnSessionOfflineChannel로 퇴장했습니다.", args.Session.NickName), false);
                //chatScroll.PopulatePublicChat(chatItem);
            }
        };
        #endregion

        /// <summary>
        /// ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
        /// 오프라인 상태에서 클라이언트의 네트워크 상황이 다시 좋아지거나, Poll 함수가 다시 주기적으로 호출되는 경우 서버에서 해당 유저를 온라인 처리하고 콜백되는 함수
        /// ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
        /// </summary>
        #region 
        Backend.Chat.OnSessionOnlineChannel = (SessionOnlineEventArgs args) =>
        {
            Debug.Log(string.Format("OnSessionOnlineChannel {0}", args.ErrInfo));
            participantsScroll.PublicDePopulate(args.Session);
            // 내가 채널에서 팅겼을 경우
            if (!args.Session.IsRemote)
            {

                //chatItem = new ChatItem(SessionInfo.None, chatScroll.infoText, "OnSessionOnlineChannel : 접속되었습니다", false);
                //chatScroll.PopulatePublicChat(chatItem);

            }
            // 다른사람이 채널에서 팅겼을 경우
            else
            {
                //chatItem = new ChatItem(SessionInfo.None, chatScroll.infoText, string.Format("{0}이 OnSessionOnlineChannel로 접속되었습니다.", args.Session.NickName), false);
                //chatScroll.PopulatePublicChat(chatItem);
            }
        };
        #endregion

        /// <summary>
        /// ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
        /// 공개 채팅 왔을 때
        /// ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
        /// </summary>
        #region 
        Backend.Chat.OnChat = (ChatEventArgs args) =>
        {
#if UNITY_EDITOR
            Debug.Log(string.Format("OnChat {0}, {1}", DateTime.Now.TimeOfDay, args.From.NickName, args.Message));
#endif

            if (args.ErrInfo == ErrorInfo.Success)
            {
                chatItem = new ChatItem(args.From, args.From.NickName, args.Message, args.From.IsRemote);
                chatScroll.PopulatePublicChat(chatItem);
            }
            else if (args.ErrInfo.Category == ErrorCode.BannedChat)
            {
                // 도배방지 메세지 
                if (args.ErrInfo.Detail == ErrorCode.BannedChat)
                {
                    chatItem = new ChatItem(SessionInfo.None, chatScroll.infoText, args.ErrInfo.Reason, false);
                    chatScroll.PopulatePublicChat(chatItem);
                }
            }
        };
        #endregion

        /// <summary>
        /// ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
        /// 길드 채팅 왔을 때
        /// ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
        /// </summary>
        #region 
        Backend.Chat.OnGuildChat = (ChatEventArgs args) =>
        {
            //Debug.Log(string.Format("OnChat {0}", args.ErrInfo));
            if (args.ErrInfo == ErrorInfo.Success)
            {
                chatItem = new ChatItem(args.From, args.From.NickName, args.Message, args.From.IsRemote);
                chatScroll.PopulateGuildChat(chatItem);
            }
            else if (args.ErrInfo.Category == ErrorCode.BannedChat)
            {
                // 도배방지 메세지 
                if (args.ErrInfo.Detail == ErrorCode.BannedChat)
                {
                    chatItem = new ChatItem(args.From, chatScroll.infoText, args.ErrInfo.Reason, false);
                    chatScroll.PopulateGuildChat(chatItem);
                }
            }

        };
        #endregion

        /// <summary>
        /// ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
        /// 알림(공지, 운영자) 채팅 왔을 때
        /// ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
        /// </summary>
        #region
        Backend.Chat.OnNotification = (NotificationEventArgs args) =>
        {
            // 공지 
            string notificationNick = string.Format(LanguageGameData.GetInstance ().GetString("chat.notification"), args.Subject);
            string msg = string.Format("<color=#00FF22>{0}</color>", args.Message);
            chatItem = new ChatItem(SessionInfo.None, notificationNick, msg, false, false, true);
            chatScroll.PopulateAll(chatItem);

        };

        //string GlobalChatNick;
        //Backend.Chat.OnGlobalChat = (GlobalChatEventArgs args) =>
        //{
        //    if (args.ErrInfo == ErrorInfo.Success)
        //    {
        //        GlobalChatNick = string.Format("[User Notification] {0}", args.From.NickName);
        //    }
        //    else
        //    {
        //        GlobalChatNick = string.Format("[Error] {0}", "운영자가 아닙니다");
        //    }

        //    chatItem = new ChatItem(SessionInfo.None, GlobalChatNick, args.Message, false, false, true);
        //    chatScroll.PopulatePublicChat(chatItem);

        //};
        #endregion

        /// <summary>
        /// ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
        /// 귓속말 / 보내거나 받거나 
        /// ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
        /// </summary>
        #region 
        string whisperNick;
        Backend.Chat.OnWhisper = (WhisperEventArgs args) =>
        {
            //Debug.Log(string.Format("OnWhisper {0}", args.ErrInfo));

            if (args.ErrInfo == ErrorInfo.Success)
            {
                //Debug.Log(string.Format("OnWhisper: from {0} to {1} : message {2}", args.From.NickName, args.To.NickName, args.Message));

                // 내가 보낸 귓속말인 경우
                if (!args.From.IsRemote)
                {
                    whisperNick = string.Format("to {0}", args.To.NickName);
                    chatItem = new ChatItem(SessionInfo.None, whisperNick, args.Message, true, true);
                }
                // 내가 받은 귓속말인 경우
                else
                {
                    whisperNick = string.Format("from {0}", args.From.NickName);
                    chatItem = new ChatItem(args.From, whisperNick, args.Message, true, false);
                }

                chatScroll.PopulateAll(chatItem);
            }
            else if (args.ErrInfo.Category == ErrorCode.BannedChat)
            {
                // 도배방지 메세지 
                if (args.ErrInfo.Detail == ErrorCode.BannedChat)
                {
                    chatItem = new ChatItem(SessionInfo.None, chatScroll.infoText, args.ErrInfo.Reason, false);
                    chatScroll.PopulateAll(chatItem);
                }
            }
        };
        #endregion

        // Exception 발생 시
        Backend.Chat.OnException = (Exception e) =>
        {
            Debug.LogError(e);
        };
    }


    private void ShowModal(string message)
    {
        ChannelListScript.Instance().showmodal(message);
    }
}

public struct ChatMsg
{
    /// <summary>
    /// // 0 : 일반 시스템 메시지(Contents에 메시지내용) 1 : 유저 채팅 메시지(Contents에 Json형식으로 정보와 메시지가 포함), 2 : 아이템 드롭 메시지(Contents에 Json형식으로 정보가 포함)
    /// </summary>
    public int MsgType;
    public string TItle;
    public string Contents;
}

/// <summary>
/// 채팅 메시지 : 공개 채팅 메시지에 함께 태워보낼 값 (유저 정보, 메시지) json 형식 string 
/// </summary>
internal struct ChatMsgItem
{
    /// <summary>
    /// // 0 : 일반 시스템 메시지(Contents에 메시지내용) 1 : 유저 채팅 메시지(Contents에 Json형식으로 정보와 메시지가 포함), 2 : 아이템 드롭 메시지(Contents에 Json형식으로 정보가 포함)
    /// </summary>
    public int MsgType; 
    public string Contents;
}

/// <summary>
/// 일반 공개 채팅에서 유저 정보 
/// </summary>
internal struct ChatPaserMsgUserInfo
{
    public int csn; // 챕터 스테이지 총 넘버 
    public int cs_rank; // 챕터 스테이지 랭킹 
    public int hel_ty, hel_rt, hel_id, hel_eht_lv;
    public string Guild;
    public string Message;
}

/// <summary>
/// 채팅 메시지 : 아이템 드롭 메시지 
/// </summary>
internal struct ChatPaserMsgItemInfo
{
    public string ItemType; // item, skill, equip 
    public int Type;
    public int Rating;
    public int Idx;
}

internal class ChatItem
{
    internal SessionInfo session { get; set; }
    internal bool IsRemote { get; set; }
    internal bool isWhisper { get; set; }
    internal bool isNotification { get; set; } // 운영자채팅도 포함
    internal string Nickname;
    internal string Contents;

    internal ChatItem(SessionInfo session, string nick, string cont, bool isWhisper, bool IsRemote)
    {
        this.session = session;
        Nickname = nick;
        Contents = cont;
        this.isWhisper = isWhisper;
        this.IsRemote = IsRemote;
    }
    internal ChatItem(SessionInfo session, string nick, string cont, bool isWhisper, bool IsRemote, bool isNotification)
    {
        this.session = session;
        Nickname = nick;
        Contents = cont;
        this.isWhisper = isWhisper;
        this.IsRemote = IsRemote;
        this.isNotification = isNotification;
    }

    internal ChatItem(SessionInfo session, string nick, string cont, bool IsRemote)
    {
        this.session = session;
        Nickname = nick;
        Contents = cont;
        isWhisper = false;
        this.IsRemote = IsRemote;
    }
}