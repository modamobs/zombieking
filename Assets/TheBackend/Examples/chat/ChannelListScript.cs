using System.Collections.Generic;
using UnityEngine;
using BackEnd;
using LitJson;
using BackEnd.Tcp;
using UnityEngine.UI;

public class ChannelListScript : MonoBehaviour
{

    public GameObject channelScrollView;
    public Text public_channel_alias;
    public Text guild_channel_alias;

    public Text alert;
    public Button refresh;

    private ChannelNodeObject guildChannelNode = null;

    private List<ChannelNodeObject> channelList = new List<ChannelNodeObject>();
    internal bool chatStatus = false;
    private bool guildStatus = false;
    readonly string CHAT_INACTIVE = "채팅 서비스가 비활성화 상태입니다. 콘솔에서 활성화 시켜주세요.";

    private ModalPanel modalPanel;
    ChannelGridScroll populateGrid;
    // singleton

    bool isJoinChannelFinish = false;
    private static ChannelListScript channelListScript;
    public static ChannelListScript Instance()
    {
        if (!channelListScript)
        {
            channelListScript = FindObjectOfType(typeof(ChannelListScript)) as ChannelListScript;
            if (!channelListScript)
                Debug.LogWarning("There needs to be one active ChannelListScript script on a GameObject in your scene.");
        }

        return channelListScript;
    }

    // Use this for initialization
    void Start()
    {
        modalPanel = ModalPanel.Instance();

        Screen.SetResolution(Screen.width, Screen.height, true);

        populateGrid = ChannelGridScroll.Instance();

        if (!Backend.IsInitialized)
        {
            Backend.Initialize(GetChatStatus);
        }
        else
        {
            GetChatStatus();
        }
    }

    public void GetChatStatus()
    {
        BackendReturnObject chatStatusBRO = Backend.Chat.GetChatStatus();
        if (chatStatusBRO.IsSuccess())
        {
            alert.gameObject.SetActive(false);
            channelScrollView.SetActive(true);
            string yn = chatStatusBRO.GetReturnValuetoJSON()["chatServerStatus"]["chatServer"].ToString();
            chatStatus |= yn.Equals("y");
            Debug.Log("<color=yellow>chatStatus: " + chatStatus + "</color>");

            if (chatStatus)
            {
                alert.gameObject.SetActive(false);
                refresh.gameObject.SetActive(true);
                GetChannelList(ChannelType.Public);
                GetChannelList(ChannelType.Guild);
            }
            else
            {
                alert.text = CHAT_INACTIVE;
                alert.gameObject.SetActive(true);
            }
        }
        else
        {
            alert.text = chatStatusBRO.GetMessage();
            alert.gameObject.SetActive(true);
            populateGrid.RemoveAllListViewItem();
        }


    }

    public void GetPublicChannelList()
    {
        if (!chatStatus)
            GetChatStatus();
        else
            GetChannelList(ChannelType.Public);
    }

    void GetChannelList(ChannelType type)
    {
        if (chatStatus)
        {
            BackendReturnObject bro = null;
            switch (type)
            {
                case ChannelType.Guild:
                    bro = Backend.Chat.GetGuildChannel();
                    if (bro.IsSuccess())
                    {
                        alert.gameObject.SetActive(false);
                        guildStatus = true;

                        JsonData data = bro.GetReturnValuetoJSON();

                        guildChannelNode = new ChannelNodeObject(type, data["uuid"].ToString(), (int)data["joinedUserCount"], (int)data["maxUserCount"],
                                                                data["serverHostName"].ToString(), data["serverPort"].ToString(), data["alias"].ToString());

                    }
                    // Client Error
                    else
                    {
                        guildStatus = false;

                        // 채팅을 뒤끝콘솔에서 활성화하지 않은 경우
                        if (bro.GetStatusCode().Equals("412"))
                        {
                            alert.text = CHAT_INACTIVE;
                            alert.gameObject.SetActive(true);
                        }
                    }

                    PlayerPrefs.SetInt("guildStatus", guildStatus ? 1 : 0);
                    break;

                case ChannelType.Public:
                    channelList.Clear();
                    bro = Backend.Chat.GetChannelList();
                    LogPrint.Print("-------------- GetChannelList " + bro.GetReturnValuetoJSON().ToJson());
                    if (bro.IsSuccess())
                    {
                        // 채널 리스트 보여주기 
                        /*    JsonData rows = bro.GetReturnValuetoJSON()["rows"];
                            ChannelNodeObject channelNode;
                            Debug.Log(rows.Count);
                            for (int i = 0; i < rows.Count; i++)
                            {
                                JsonData data = rows[i];
                                channelNode = new ChannelNodeObject(type, data["uuid"].ToString(), (int)data["joinedUserCount"], (int)data["maxUserCount"],
                                                                    data["serverHostName"].ToString(), data["serverPort"].ToString(), data["alias"].ToString());
                                channelList.Add(channelNode);
                            }
                            alert.gameObject.SetActive(false);
                            populateGrid.PopulatePublic(channelList);
                        */

                        // 인원 빈 채널에 자동 입장 
                        JsonData rows = bro.GetReturnValuetoJSON()["rows"];
                        ChannelNodeObject channelNode;
                        Debug.Log("<color=yellow>인원 빈 채널에 자동 입장 rows.Count : " + rows.Count + ", rows : " + rows.ToJson() + "</color>");
                        for (int i = 0; i < rows.Count; i++)
                        {
                            JsonData data = rows[i];
                            channelNode = new ChannelNodeObject(type, data["uuid"].ToString(), (int)data["joinedUserCount"], (int)data["maxUserCount"],
                                                                data["serverHostName"].ToString(), data["serverPort"].ToString(), data["alias"].ToString());
                            if(channelNode.joinedUserCount < channelNode.maxUserCount)
                            {
                                JoinChannel(channelNode);
                                break;
                            }
                        }
                        alert.gameObject.SetActive(false);

                    }
                    // Client Error
                    else
                    {
                        // 채팅을 뒤끝콘솔에서 활성화하지 않은 경우
                        if (bro.GetStatusCode() == "412")
                        {
                            alert.text = CHAT_INACTIVE;
                            alert.gameObject.SetActive(true);
                        }
                    }
                    break;
            }

            // Server Error
            if (bro.IsServerError())
            {
                Debug.Log("server error");
                alert.text = "server error";
                alert.gameObject.SetActive(true);
                populateGrid.RemoveAllListViewItem();
            }
            else if (type == ChannelType.Public && bro != null && !bro.IsSuccess())
            {
                alert.text = bro.GetMessage();
                alert.gameObject.SetActive(true);
                refresh.gameObject.SetActive(false);
                populateGrid.RemoveAllListViewItem();
            }
        }
    }

    internal void JoinChannel(ChannelNodeObject channelNode)
    {
        // 인원수 체크 
        if (channelNode.joinedUserCount >= channelNode.maxUserCount)
        {
            AlreadyFull();
        }
        else
        {
            ErrorInfo info;
            Backend.Chat.JoinChannel(channelNode.type, channelNode.host, channelNode.port, channelNode.channel_uuid, out info);
            switch (channelNode.type)
            {
                case ChannelType.Public:
                    public_channel_alias.text = channelNode.alias;
                    // public channel 입장할 때 -> guild Channel도 함께 입장
                    if (guildChannelNode != null)
                        Invoke("JoinGuildChannel", 1); // delay를 줘야 핸들러가 겹치지 않음
                    break;
                case ChannelType.Guild:
                    guild_channel_alias.text = channelNode.alias;
                    break;
            }
        }
    }
    void JoinGuildChannel()
    {
        JoinChannel(guildChannelNode);
    }

    void AlreadyFull()
    {
        showmodal("인원이 꽉찬 방입니다.");
    }

    internal void showmodal(string message)
    {
        modalPanel.AlertShow(message);
    }

}

public class ChannelNodeObject
{
    public ChannelType type;
    public string channel_uuid;
    public string participants;
    public int joinedUserCount;
    public int maxUserCount;
    public string host;
    public ushort port;
    public string alias;

    public ChannelNodeObject(ChannelType type, string uuid, int joinedUser, int maxUser, string host, string port, string alias)
    {
        this.type = type;
        this.channel_uuid = uuid;
        this.joinedUserCount = joinedUser;
        this.maxUserCount = maxUser;
        this.participants = joinedUser + "/" + maxUser;
        this.host = host;
        this.port = ushort.Parse(port);
        this.alias = alias;
    }
}
