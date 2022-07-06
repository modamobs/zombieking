using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollIndexCallbackChatBlock : MonoBehaviour
{
    [SerializeField] Text txNickName;
    [SerializeField] Text txDate;

    void ScrollCellIndex(int idx)
    {
        var db = GameDatabase.GetInstance().chat.GetBlock(idx);
        txNickName.text = db.nick;
        txDate.text = db.blockDate;
    }

    public void Click_UnBlock (Text nickName)
    {
        PopUpMng.GetInstance().ChatBlockList_UnBlock(nickName.text);
    }
}
