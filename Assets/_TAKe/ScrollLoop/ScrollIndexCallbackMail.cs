using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollIndexCallbackMail : MonoBehaviour
{
    [SerializeField] GameDatabase.MailDB.Data data = new GameDatabase.MailDB.Data();

    [SerializeField] UIInfo uiInfo = new UIInfo();
    [System.Serializable]
    struct UIInfo
    {
        public Image im_Icon; // 아이콘 
        public Text tx_ItemName_ItemCount; // 아이템 이름  + 수량 
        public Text tx_ExpirationDate; // // 만료 일시
        public Text tx_Title, tx_Content; // 제목, 내용 
        public Text tx_SendNickName; // 보낸 사람 
        public Text tx_Index;

        public Text tx_Rating;
        public Image im_RatingBg;
        public Text tx_SubCount;
    }
    void ScrollCellIndex(int idx)
    {
        data = GameDatabase.GetInstance().mailDB.GetFindIndex(idx);
        uiInfo.tx_Index.text = (idx + 1).ToString();
        //uiInfo.tx_ItemName_ItemCount.text = string.Format("{0} x{1}", data.item.item_name, string.Format("{0:#,###}", data.item.count));
        uiInfo.tx_Title.text = data.title.ToString();
        uiInfo.tx_Content.text = data.content.ToString();
        uiInfo.tx_ExpirationDate.text = string.Format("보관기간 {0}", System.DateTime.Parse(data.expirationDate.ToString()));
        uiInfo.tx_SendNickName.text = data.senderNickname.ToString();
        uiInfo.im_RatingBg.sprite = SpriteAtlasMng.GetInstance().GetSpriteRatingBg(data.item.rt);
        uiInfo.tx_Rating.color = ResourceDatabase.GetInstance().GetItemColor(data.item.rt);


        switch (data.item.gift_type)
        {
            case "user_info":
                uiInfo.im_Icon.sprite = SpriteAtlasMng.GetInstance().GetMailUseItemSprite(data.item.ty);
                uiInfo.tx_Rating.text = "";
                uiInfo.tx_SubCount.text = string.Format("x{0}", data.item.count);
                uiInfo.tx_ItemName_ItemCount.text = data.item.item_name.ToString();
                break;
            case "item":
                uiInfo.im_Icon.sprite = SpriteAtlasMng.GetInstance().GetItemSprite(data.item.ty, data.item.rt);
                uiInfo.tx_Rating.text = LanguageGameData.GetInstance().GetString(string.Format("item.rating.string.{0}", data.item.rt));
                uiInfo.tx_SubCount.text = string.Format("x{0}", data.item.count);
                uiInfo.tx_ItemName_ItemCount.text = data.item.item_name.ToString();
                break;
            case "goods":
                uiInfo.im_Icon.sprite = SpriteAtlasMng.GetInstance().GetSpriteGoods(data.item.ty);
                uiInfo.tx_Rating.text = string.Empty;
                if (data.item.ty == 10)
                {
                    uiInfo.tx_SubCount.text = string.Empty;
                    uiInfo.tx_ItemName_ItemCount.text = string.Format("{0}", data.item.item_name, string.Format("{0:#,0}", data.item.count));
                }
                else
                {
                    uiInfo.tx_SubCount.text = string.Format("x{0}", data.item.count);
                    uiInfo.tx_ItemName_ItemCount.text = data.item.item_name.ToString();
                }
                break;
            case "skill":
                uiInfo.im_Icon.sprite = SpriteAtlasMng.GetInstance().GetSpriteSkill(data.item.idx);
                uiInfo.tx_Rating.text = LanguageGameData.GetInstance().GetString(string.Format("item.rating.string.{0}", data.item.rt));
                uiInfo.tx_SubCount.text = string.Format("x{0}", data.item.count);
                uiInfo.tx_ItemName_ItemCount.text = data.item.item_name.ToString();
                break;
            case "chest":
                uiInfo.im_Icon.sprite = SpriteAtlasMng.GetInstance().GetMailUseItemSprite(data.item.ty, data.item.rt);
                uiInfo.tx_Rating.text = LanguageGameData.GetInstance().GetString(string.Format("item.rating.string.{0}", data.item.rt));
                uiInfo.tx_SubCount.text = string.Format("x{0}", data.item.count);
                uiInfo.tx_ItemName_ItemCount.text = data.item.item_name.ToString();
                break;
            default:
                uiInfo.im_Icon.sprite = SpriteAtlasMng.GetInstance().GetTransparency();
                uiInfo.tx_SubCount.text = string.Empty;
                uiInfo.tx_Rating.text = string.Empty;
                uiInfo.tx_ItemName_ItemCount.text = string.Empty;
                break;
        }
    }

    public void OnClick_Get()
    {
        if (string.IsNullOrEmpty(data.indate))
            return;

        PopUpMng.GetInstance().mail.GetRewarded(data.indate);
    }
}
