using System.Collections.Generic;
using BackEnd.Tcp;
using UnityEngine;
using UnityEngine.UI;

public class ChatParticipantsScroll : MonoBehaviour
{
    private ModalPanel modalPanel;

    public GameObject prefab; 

    public RectTransform publicContent;
    public RectTransform guildContent;

    internal List<SessionInfo> public_participants = new List<SessionInfo>();
    internal List<SessionInfo> guild_participants = new List<SessionInfo>();

    private static ChatParticipantsScroll participantsScroll;

    public static ChatParticipantsScroll Instance()
    {
        if (!participantsScroll)
        {
            participantsScroll = FindObjectOfType(typeof(ChatParticipantsScroll)) as ChatParticipantsScroll;
            if (!participantsScroll)
                Debug.LogWarning("There needs to be one active ChatParticipantsScroll script on a GameObject in your scene.");
        }

        return participantsScroll;
    }

    private void Awake()
    {
        modalPanel = ModalPanel.Instance();
    }

    internal void PublicPopulateList()
    {
        PopulateList(ChannelType.Public);
    }

    internal void GuildPopulateList()
    {
        PopulateList(ChannelType.Guild);
    }

    void PopulateList(ChannelType type)
    {
        // 존재하던 리스트를 삭제 (reset)
        RemoveAllListViewItem(type);

        switch(type)
        {
            case ChannelType.Public:
                foreach (SessionInfo participant in public_participants)
                {
                    PublicPopulate(participant);
                }
                break;
            case ChannelType.Guild:
                foreach (SessionInfo participant in guild_participants)
                {
                    GuildPopulate(participant);
                }
                break;
        }
    }

    /*
     * 한명의 참여자를 리스트에 추가 출력
     */
    GameObject newObj;
    internal void PublicPopulate(SessionInfo participant)
    {
        Populate(ChannelType.Public, participant);
    }

    internal void GuildPopulate(SessionInfo participant)
    {
        Populate(ChannelType.Guild, participant);
    }

    void Populate(ChannelType type, SessionInfo participant)
    {
        switch(type)
        {
            case ChannelType.Public:
                newObj = (GameObject)Instantiate(prefab, publicContent.transform);
                break;

            case ChannelType.Guild:
                newObj = (GameObject)Instantiate(prefab, guildContent.transform);
                break;
        }

        newObj.name = participant.GetHashCode().ToString();

        // participants NickName 출력
        Text nickname = newObj.GetComponent<Text>();
        nickname.text = participant.NickName;
    }

    internal void PublicDePopulate(SessionInfo participant)
    {
        DePopulate(ChannelType.Public, participant);
    }

    internal void GuildDePopulate(SessionInfo participant)
    {
        DePopulate(ChannelType.Guild, participant);
    }

    void DePopulate(ChannelType type, SessionInfo participant)
    {
        RectTransform content = null;
        List<SessionInfo> participants = null;

        switch (type)
        {
            case ChannelType.Public:
                content = publicContent;
                participants = public_participants;
                break;

            case ChannelType.Guild:
                content = guildContent;
                participants = guild_participants;
                break;
        }

        if(participants!=null && content!=null)
        {
            // 리스트에 속한 경우 -> 리스트에서 삭제
            if (participants.Contains(participant))
            {
                participants.Remove(participant);

                foreach (Transform child in content.transform)
                {
                    if (child.gameObject.name.Equals(participant.GetHashCode().ToString()))
                    {
                        Destroy(child.gameObject);
                    }
                }
            }
        }
    }

    private void RemoveAllListViewItem(ChannelType type)
    {
        switch (type)
        {
            case ChannelType.Public:
                foreach (Transform child in publicContent.transform)
                {
                    if(child!=null)
                        Destroy(child.gameObject);
                }

                break;

            case ChannelType.Guild:
                foreach (Transform child in guildContent.transform)
                {
                    if (child != null)  
                        Destroy(child.gameObject);
                }
                break;
        }
    }
}

