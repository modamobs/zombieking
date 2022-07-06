using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChannelGridScroll : MonoBehaviour
{
    public RectTransform content;
    public GameObject prefab; 

    // singleton 
    private static ChannelGridScroll channelGrid;
    public static ChannelGridScroll Instance()
    {
        if (!channelGrid)
        {
            channelGrid = FindObjectOfType(typeof(ChannelGridScroll)) as ChannelGridScroll;
            if (!channelGrid)
                Debug.LogWarning("There needs to be one active ChannelGridScroll script on a GameObject in your scene.");
        }

        return channelGrid;
    }

    internal void PopulatePublic(List<ChannelNodeObject> channelList)
    {
        RemoveAllListViewItem();

        GameObject newObj; 

        foreach (ChannelNodeObject channelNode in channelList)
        {
            newObj = (GameObject)Instantiate(prefab, transform);

            Text[] texts = newObj.GetComponentsInChildren<Text>();
            // channel alias
            texts[0].text = channelNode.alias;
            // participants
            texts[1].text = string.Format("{0}/{1}", channelNode.joinedUserCount, channelNode.maxUserCount);
            // 꽉찬 경우 색 설정
            if(channelNode.joinedUserCount == channelNode.maxUserCount)
            {
                texts[1].color = new Color32 (218,75,75,255);
            }

            // 접속 버튼
            Button button = newObj.GetComponentInChildren<Button>();
            button.onClick.AddListener(delegate { ChannelListScript.Instance().JoinChannel(channelNode); });
        }
    }

    internal void RemoveAllListViewItem()
    {
        foreach (Transform child in content.transform)
        {
            if(child!=null)
            {
                Destroy(child.gameObject);
            }

        }
    }
}
