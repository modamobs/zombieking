using System.Collections.Generic;
using BackEnd.Tcp;
using Coffee.UIExtensions;
using UnityEngine;
using UnityEngine.UI;

public class ChatScroll : MonoBehaviour
{
    public GameObject[] prefabsMsg;
    public GameObject prefabMsg; // This is our prefab object that will be exposed in the inspector

    public RectTransform publicContent;
    public RectTransform guildContent;
    public RectTransform whisperContent;
    public Color whisperColor;
    private Color32 infoTextColor = new Color32(158, 72, 28, 255);

    internal List<ChatItem> publicChats = new List<ChatItem>();

    private static ChatScroll chatScroll;

    internal string infoText = "안내";
    private static string CHAT_NICK = "[{0}]";

    public static ChatScroll Instance()
    {
        if (!chatScroll)
        {
            chatScroll = FindObjectOfType(typeof(ChatScroll)) as ChatScroll;
            if (!chatScroll)
                Debug.LogWarning("There needs to be one active ChatScroll script on a GameObject in your scene.");
        }

        return chatScroll;
    }

    internal void PopulatePublicList()
    {
        // 존재하던 리스트를 삭제 (reset)
        RemoveAllPublicListViewItem();
        foreach (ChatItem item in publicChats)
        {
            PopulatePublicChat(item);
        }
    }


    // 채팅 아이템 출력
    GameObject newObj;
    internal void PopulatePublicChat(ChatItem item)
    {
        PopulateChat(ChannelType.Public, item);
    }

    internal void PopulateGuildChat(ChatItem item)
    {
        PopulateChat(ChannelType.Guild, item);
    }

    internal void PopulateAll(ChatItem item)
    {
        PopulatePublicChat(item);
        PopulateGuildChat(item);
        PopulateWhisperChat(item);
    }

    /// <summary>
    /// ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
    /// 귓속말 내용 표시 
    /// ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
    /// </summary>
    #region 
    private void PopulateWhisperChat(ChatItem item)
    {
        LogPrint.Print("PopulateWhisperChat");
        // Create new instances of our prefab until we've created as many as we specified
        newObj = (GameObject)Instantiate(prefabMsg, whisperContent.transform);

        Text[] texts = newObj.GetComponentsInChildren<Text>();

        var msg = GameDatabase.GetInstance().chat.ChatCellText(item.Nickname, item.Contents);
        texts[0].text = msg.TItle;
        texts[1].text = msg.Contents;

        // 안내 메세지 색상
        if (item.Nickname.Equals(infoText))
        {
            foreach (Text text in texts)
            {
                text.color = infoTextColor;
            }
        }
        // 귓속말 메세지 색상
        else
        {
            foreach (Text text in texts)
            {
                text.color = whisperColor;
            }
        }

        if (item.session != SessionInfo.None && item.session.IsRemote)
        {
            Button button = newObj.GetComponentInChildren<Button>();
            button.onClick.AddListener(delegate { ParticipantsModal.Instance().participantPanelShow(item.session.NickName, Input.mousePosition); });
        }
    }
    #endregion

    /// <summary>
    /// ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
    /// 채팅 내용 표시 
    /// ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
    /// </summary>
    #region 
    private void PopulateChat(ChannelType ch_type, ChatItem item)
    {
        var msg = GameDatabase.GetInstance().chat.ChatCellText(item.Nickname, item.Contents);
        if (msg.MsgType == -1)
        {
            LogPrint.Print("<color=red> ---------------------- ( msg.MsgType == -1 ) ---------------------- </color>");
            return;
        }

        switch (ch_type)
        {
            case ChannelType.Public:
                if (publicContent != null && publicContent.transform != null)
                {
                    // Create new instances of our prefab until we've created as many as we specified
                    Transform t = publicContent.transform;
                    //Debug.Log(publicContent.transform.position.y + newObj.GetComponent<RectTransform>().rect.height);
                    newObj = (GameObject)Instantiate(prefabsMsg[msg.MsgType],  publicContent.transform);
                    newObj.transform.position = new Vector3(newObj.transform.position.x,newObj.transform.position.y+20,0);
                }
                break;
            case ChannelType.Guild:
                if (guildContent != null && guildContent.transform != null)
                {
                    // Create new instances of our prefab until we've created as many as we specified
                    newObj = (GameObject)Instantiate(prefabMsg, guildContent.transform);
                }
                break;
            default:
                return;
        }

        Text[] texts = newObj.GetComponentsInChildren<Text>();
        texts[0].text = msg.TItle;
        texts[1].text = msg.Contents;

        PopUpMng.GetInstance().popUpChat.OnTopChatLineMsg(msg);

        // 유저 아이콘 정보 - 유저가 채팅을 보낸경우 
        if (msg.MsgType == 1)
        {
            try
            {
                ChatMsgItem cmi = JsonUtility.FromJson<ChatMsgItem>(item.Contents);
                if (cmi.MsgType == 1)
                {
                    ChatPaserMsgUserInfo cmui = JsonUtility.FromJson<ChatPaserMsgUserInfo>(cmi.Contents);
                    Image[] images = newObj.GetComponentsInChildren<Image>();
                    if (images.Length > 0)
                    {
                        images[1].sprite = SpriteAtlasMng.GetInstance().GetSpriteRatingBg(cmui.hel_rt);
                        images[2].sprite = SpriteAtlasMng.GetInstance().GetSpriteEquip(cmui.hel_ty, cmui.hel_rt, cmui.hel_id);
                        images[3].color = ResourceDatabase.GetInstance().GetItemColor(cmui.hel_rt);
                        if (cmui.hel_eht_lv >= 30)
                        {
                            images[3].GetComponent<UIShiny>().enabled = true;
                        }
                    }

                    if (texts[0] != null)
                    {
                        texts[0].color = ResourceDatabase.GetInstance().GetChatRankColor(cmui.cs_rank); // 랭킹 순위에 닉네임 텍스트 컬러 다르게 변경 
                    }
                }
            }
            catch (System.Exception)
            {

            }
        }

        if ( (item.session!=SessionInfo.None && item.session.IsRemote) )
        {
            LogPrint.Print("item.session.NickName : " + item.session.NickName);
            Button button = newObj.GetComponent<Button>();
            button.onClick.AddListener(delegate { ParticipantsModal.Instance().participantPanelShow(item.session.NickName, Input.mousePosition); });
        }

        // 안내 메세지 색상
        if (item.Nickname.Equals(infoText))
        {
            foreach (Text text in texts)
            {
                text.color = infoTextColor;
            }
        }

        // 귓속말 색상
        else if (item.isWhisper)
        {
            foreach (Text text in texts)
            {
                text.color = whisperColor;
            }
        }
    }
    #endregion

    internal void RemoveAllPublicListViewItem()
    {
        RemoveAllListViewItem(ChannelType.Public);
    }

    internal void RemoveAllGuildListViewItem()
    {
        RemoveAllListViewItem(ChannelType.Guild);
    }

    private void RemoveAllListViewItem(ChannelType type)
    {
        RectTransform content = null;
        switch (type)
        {
            case ChannelType.Public:
                content = publicContent;
                break;
            case ChannelType.Guild:
                content = guildContent;
                break;
        }

        if(content!=null)
        {
            foreach (Transform child in content.transform)
            {
                if (child != null)
                    Destroy(child.gameObject);
            }
        }

        // 퍼블릭 채널 퇴장 시, 귓속말도 초기화
        if(type == ChannelType.Public)
        {
            if (whisperContent != null)
            {
                foreach (Transform child in whisperContent.transform)
                {
                    if (child != null)
                        Destroy(child.gameObject);
                }
            }

        }
    }
}

