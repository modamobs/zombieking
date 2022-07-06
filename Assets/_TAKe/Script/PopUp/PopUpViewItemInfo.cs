using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopUpViewItemInfo : MonoBehaviour
{
    [SerializeField]
    InfoViewItem infoViewItem;
    [System.Serializable]
    struct InfoViewItem
    {
        public Image imIcon;
        public Image imRatingBg;
        public Text txRating;
        public Text txCount;
        public Text txName;
        public Text txDescrip;
    }

    /// <summary>
    /// 아이템 
    /// </summary>
    public void SetData (GameDatabase.TableDB.Item itDb)
    {
        infoViewItem.imIcon.sprite = SpriteAtlasMng.GetInstance().GetItemSprite(itDb.type, itDb.rating);
        infoViewItem.imRatingBg.sprite = SpriteAtlasMng.GetInstance().GetSpriteRatingBg(itDb.rating);

        infoViewItem.txRating.text = GameDatabase.StringFormat.GetRatingColorText(itDb.rating, false);
        infoViewItem.txCount.text = string.Format("x{0}", itDb.count);
        infoViewItem.txName.text = LanguageGameData.GetInstance().GetString(string.Format("item.name.item_{0}_{1}", itDb.type, itDb.rating));
        infoViewItem.txDescrip.text = LanguageGameData.GetInstance().GetString(string.Format("item.name.item.descript_{0}_{1}", itDb.type, itDb.rating));
    }

    /// <summary>
    /// Goods 재화
    /// </summary>
    public void SetData(int gdsType, int gdsCnt)
    {
        infoViewItem.imIcon.sprite = SpriteAtlasMng.GetInstance().GetSpriteGoods(gdsType);
        infoViewItem.imRatingBg.sprite = SpriteAtlasMng.GetInstance().GetSpriteRatingBg(0);

        infoViewItem.txRating.text = "";
        infoViewItem.txCount.text = string.Format("x{0}", gdsCnt);
        infoViewItem.txName.text = GameDatabase.StringFormat.GetGoodsNameext(gdsType);
        infoViewItem.txDescrip.text = GameDatabase.StringFormat.GetGoodsDescriptText(gdsType);
    }

    /// <summary>
    /// 컨텐츠 -> 아이템 획득품 보기 
    /// </summary>
    public void ContentsRewardPreview(Sprite rwd_icon, string rwd_name)
    {
        //string rn = rwd_name.Replace("_once", "").Replace("_rt1", "").Replace("_rt2", "").Replace("_rt3", "").Replace("_rt4", "").Replace("_rt5", "").Replace("_rt6", "").Replace("_rt7", "")
        //    .Replace("_max_rt1", "").Replace("_max_rt2", "").Replace("_max_rt3", "").Replace("_max_rt4", "").Replace("_max_rt5", "").Replace("_max_rt6", "").Replace("_max_rt7", "");

        string rn = rwd_name.Replace("_once", "").Replace("_max", "");
        for (int i = 0; i <= 7; i++)
            rn = rn.Replace(string.Format("_rt{0}", i), "");

        LogPrint.Print("<color=red> 111 rwd_name : " + rwd_name + ", rn:" + rn + "</color>");

        infoViewItem.imIcon.sprite = rwd_icon; //  SpriteAtlasMng.GetInstance().GetContentsRewardIcon(rwd_name);
        infoViewItem.imRatingBg.sprite = SpriteAtlasMng.GetInstance().GetSpriteRatingBg(0);
        infoViewItem.txRating.text = "";
        infoViewItem.txCount.text = "";
        infoViewItem.txName.text = LanguageGameData.GetInstance().GetString(string.Format("contents.reward.preview.name.{0}", rn));
        infoViewItem.txDescrip.text = LanguageGameData.GetInstance().GetString(string.Format("contents.reward.preview.descript.{0}", rwd_name));
    }

    /// <summary>
    /// 던전 -> 아이템 획득품 보기 
    /// </summary>
    public void ContentsDungeonRewardPreview(Sprite spr_icon, string rwd_name, int rt = -1)
    {
        //string rn = rwd_name.Replace("_once", "").Replace("_rt1", "").Replace("_rt2", "").Replace("_rt3", "").Replace("_rt4", "").Replace("_rt5", "").Replace("_rt6", "").Replace("_rt7", "")
        //            .Replace("_max_rt1", "").Replace("_max_rt2", "").Replace("_max_rt3", "").Replace("_max_rt4", "").Replace("_max_rt5", "").Replace("_max_rt6", "").Replace("_max_rt7", "");

        string rn = rwd_name.Replace("_once", "").Replace("_max", "");
        for (int i = 0; i <= 7; i++)
            rn = rn.Replace(string.Format("_rt{0}", i), "");


        LogPrint.Print("<color=red> 222 rwd_name : " + rwd_name + ", rn:" + rn + "</color>, rt : " + rt);

        infoViewItem.txCount.text = "";
        infoViewItem.imIcon.sprite = spr_icon == null ? SpriteAtlasMng.GetInstance().GetContentsRewardIcon(rwd_name) : spr_icon;
        if (rt == -1)
        {
            infoViewItem.txRating.text = "";
            infoViewItem.imRatingBg.sprite = SpriteAtlasMng.GetInstance().GetSpriteRatingBg(0);
            infoViewItem.txName.text = LanguageGameData.GetInstance().GetString(string.Format("contents.reward.preview.name.{0}", rn));
            infoViewItem.txDescrip.text = LanguageGameData.GetInstance().GetString(string.Format("contents.reward.preview.descript.{0}", rwd_name));
        }
        else
        {
            infoViewItem.txRating.text = LanguageGameData.GetInstance().GetString(string.Format("item.color.rating.string.{0}", rt));
            infoViewItem.imRatingBg.sprite = SpriteAtlasMng.GetInstance().GetSpriteRatingBg(0);
            infoViewItem.txName.text = LanguageGameData.GetInstance().GetString(string.Format(string.Format("contents.reward.preview.name.{0}_{1}", rn, rt)));
            infoViewItem.txDescrip.text = LanguageGameData.GetInstance().GetString(string.Format(string.Format("contents.reward.preview.descript.{0}_{1}", rwd_name, rt)));
        }
    }

    /// <summary>
    /// 던전 -> 스킬 획득품 보기 
    /// </summary>
    public void ContentsDungeonSkillRewardPreview (int sk_idx, int sk_cnt)
    {
        infoViewItem.imIcon.sprite = SpriteAtlasMng.GetInstance().GetSpriteSkill(sk_idx);
        infoViewItem.imRatingBg.sprite = SpriteAtlasMng.GetInstance().GetSpriteRatingBg(0);
        infoViewItem.txRating.text = "";
        infoViewItem.txCount.text = string.Format("x{0}", sk_cnt);
        infoViewItem.txName.text = LanguageGameData.GetInstance().GetString(string.Format("skill.name_{0}", sk_idx));
        infoViewItem.txDescrip.text = GameDatabase.GetInstance().chartDB.GetInfoSkillDescription(sk_idx, 0); // 현재 능력치 정보 
    }
}
