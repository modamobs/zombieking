using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollIndexCallbackAttendance : MonoBehaviour
{
    [SerializeField] UIInfo uiInfo;
    [System.Serializable]
    struct UIInfo
    {
        public Text tx_Nbr;
        public Text tx_Cnt;
        public Image im_Icon;
        public Image im_RatingBg;
        public Text tx_Rating;
        public GameObject go_Receiving;
        public GameObject go_Day;
    }

    [SerializeField] GameDatabase.ChartDB.cdb_attendance_book data;
    void ScrollCellIndex(int idx)
    {
        data = GameDatabase.GetInstance().attendanceDB.GetFindIndexCdb(idx);
        int cell_nbr = data.nbr;
        int m_nbr = GameDatabase.GetInstance().tableDB.GetUserInfo().m_attend_nbr % 30;
        bool m_reward_today = PopUpMng.GetInstance().GetIsRewardToDay();
        long rwd_cnt = 0;
        switch (data.item.gift_type) 
        {
            case "item":
                uiInfo.im_Icon.sprite = SpriteAtlasMng.GetInstance().GetItemSprite(data.item.ty, data.item.rt);
                uiInfo.tx_Rating.text = LanguageGameData.GetInstance().GetString(string.Format("item.rating.string.{0}", data.item.rt));
                rwd_cnt = data.item.count;
                break;
            case "goods":
                uiInfo.im_Icon.sprite = SpriteAtlasMng.GetInstance().GetSpriteGoods(data.item.ty);
                uiInfo.tx_Rating.text = string.Empty;
                if(data.item.ty == 10) // gold 
                {
                    rwd_cnt = GameDatabase.GetInstance().questDB.GetQuestMaxSecondRewardGold(data.item.count);
                }
                else
                {
                    rwd_cnt = data.item.count;
                }
                break;
            case "skill":
                uiInfo.im_Icon.sprite = SpriteAtlasMng.GetInstance().GetSpriteSkill(data.item.idx);
                uiInfo.tx_Rating.text = LanguageGameData.GetInstance().GetString(string.Format("item.rating.string.{0}", data.item.rt));
                rwd_cnt = data.item.count;
                break;
            case "chest":
                uiInfo.im_Icon.sprite = SpriteAtlasMng.GetInstance().GetItemSprite(data.item.ty, data.item.rt);
                uiInfo.tx_Rating.text = LanguageGameData.GetInstance().GetString(string.Format("item.rating.string.{0}", data.item.rt));
                rwd_cnt = data.item.count;
                break;
        }
            
        uiInfo.im_RatingBg.sprite = SpriteAtlasMng.GetInstance().GetSpriteRatingBg(data.item.rt);
        uiInfo.tx_Rating.color = ResourceDatabase.GetInstance().GetItemColor(data.item.rt);
        uiInfo.tx_Nbr.text = ((data.nbr % 30) + 1).ToString();
        uiInfo.tx_Cnt.text = string.Format("x{0:#,0}", rwd_cnt).ToString();
        uiInfo.go_Receiving.SetActive(m_nbr > cell_nbr);
        uiInfo.go_Day.SetActive(m_nbr == cell_nbr && m_reward_today);
    }

    public void Click_Info()
    {
        if(string.Equals(data.item.gift_type, "item"))
        {
            PopUpMng.GetInstance().Open_ViewItemInfo(new GameDatabase.TableDB.Item() { type = data.item.ty, rating = data.item.rt, count = data.item.count });
        }
        else if(string.Equals(data.item.gift_type, "goods"))
        {
            PopUpMng.GetInstance().Open_ViewGoodsInfo(data.item.ty, data.item.count);
        }
    }
}
