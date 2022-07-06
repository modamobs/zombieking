using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using BackEnd;
using LitJson;
using System.Threading.Tasks;

public class PopUpChatBlockList : MonoBehaviour
{
    public InitOnStartChatBlock initOnStartChatBlock;
    public void SetData()
    {
        initOnStartChatBlock.SetInit();
    }

    public void ASetUnBlock(string nickName)
    {
        if (!string.IsNullOrEmpty(nickName))
        {
            ChatScript.Instance().UnBlockUser(nickName);
            SetData();
        }
    }
}
