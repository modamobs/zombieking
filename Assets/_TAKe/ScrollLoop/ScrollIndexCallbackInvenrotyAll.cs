using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.U2D;
using Coffee.UIExtensions;

public class ScrollIndexCallbackInvenrotyAll : MonoBehaviour
{
    [SerializeField] int cellIndex;
    [SerializeField] private GameDatabase.TableDB.Equipment equip_data = new GameDatabase.TableDB.Equipment();
    [SerializeField] private GameDatabase.TableDB.Item item_data = new GameDatabase.TableDB.Item();
    [SerializeField] private int nowCellIdx;

    [SerializeField] GameObject go_Root;
    public Text text;
    [SerializeField] Image imageIcon;
    [SerializeField] Image imageRatingBg;
    [SerializeField] Text textRating;
    [SerializeField] Text text_EhntLvOrCount, text_NormLvOrCount;
    [SerializeField] GameObject go_Lock;
    [SerializeField] GameObject go_Label;

    [SerializeField] Image imRtOutline;
    [SerializeField] UIGradient grRtOutline;
    [SerializeField] UIShiny shRtOutline;

    [SerializeField] private string cell_type;

    [SerializeField] Sprite spNone;

    Color[] cArrColor = new Color[8];
    string[] sArrRating = new string[8];

    void Awake()
    {
        for (int i = 0; i < sArrRating.Length; i++)
        {
            cArrColor[i] = ResourceDatabase.GetInstance().GetItemColor(i);
            sArrRating[i] = LanguageGameData.GetInstance().GetString(string.Format("item.rating.string.{0}", i));
        }
    }

    public void Refresh ()
    {
        AScrollCellIndex(nowCellIdx);
        MainUI.GetInstance().inventory.initOnStartInventoryAll.regreshCell -= this.Refresh;
    }


    public void CellIndex(int idx)
    {
        AScrollCellIndex((idx * 5) + cellIndex);
    }
    public void AScrollCellIndex(int idx)
    {
        nowCellIdx = idx;
        var data = GameDatabase.GetInstance().tableDB.GetTempList(idx);
        if (data.default0Eq1It2Sk3 == 1) // equipment 
        {
            cell_type = "eq";
            equip_data = data.equipment;
            item_data = default;
             if (!go_Root.activeSelf)
                go_Root.SetActive(true);

            int parts_type = equip_data.eq_ty;
            int parts_rating = equip_data.eq_rt;
            int parts_idx = equip_data.eq_id;
            int enhant_level = equip_data.m_ehnt_lv;
            int normal_level = equip_data.m_norm_lv;
            int state_lock = equip_data.m_lck;

            imageIcon.sprite = SpriteAtlasMng.GetInstance().GetSpriteEquip(parts_type, parts_rating, parts_idx);
            text_EhntLvOrCount.text = string.Format("+{0}", enhant_level);
            text_NormLvOrCount.text = string.Format("Lv.{0}", normal_level);
            imageRatingBg.sprite = SpriteAtlasMng.GetInstance().GetSpriteRatingBg(parts_rating, equip_data.eq_legend);
            textRating.color = cArrColor[parts_rating];
            textRating.text = sArrRating[parts_rating];

            go_Label.SetActive(equip_data.client_add_sp > MainUI.GetInstance().inventory.tapOpenSP);
            go_Lock.SetActive(state_lock == 1);

            //imRtOutline.enabled = normal_level >= 100 || enhant_level >= 30;
            //if (imRtOutline.enabled)
            //{
            //    imRtOutline.color = ResourceDatabase.GetInstance().GetItemColor(parts_rating);
            //    grRtOutline.enabled = enhant_level >= 30;
            //    shRtOutline.enabled = enhant_level >= 30;
            //    if (grRtOutline.enabled)
            //    {
            //        grRtOutline.color1 = ResourceDatabase.GetInstance().GetItemGradientColor(parts_rating);
            //        grRtOutline.color2 = ResourceDatabase.GetInstance().GetItemGradientColor(parts_rating);
            //    }
            //}
            //else
            //{
            //    grRtOutline.enabled = false;
            //    shRtOutline.enabled = false;
            //}
        }
        else if (data.default0Eq1It2Sk3 == 2) // item 
        {
            cell_type = "it";
            equip_data = default;
            item_data = data.item;
            if(!go_Root.activeSelf)
                go_Root.SetActive(true);

            int item_type = item_data.type;
            int item_rating = item_data.rating;
            int item_count = item_data.count;

            imageIcon.sprite = SpriteAtlasMng.GetInstance().GetItemSprite(item_type, item_rating);
            text_EhntLvOrCount.text = string.Format("x{0}", item_count);
            text_NormLvOrCount.text = "";
            imageRatingBg.sprite = SpriteAtlasMng.GetInstance().GetSpriteRatingBg(item_rating);
            textRating.color = cArrColor[item_rating];
            textRating.text = sArrRating[item_rating];

             go_Label.SetActive(item_data.client_add_sp > MainUI.GetInstance().inventory.tapOpenSP);

             if (go_Lock.activeSelf)
                go_Lock.SetActive(false);

             //if(imRtOutline.enabled)
             //   imRtOutline.enabled = false;
        }
        else
        {
            if (go_Root.activeSelf)
                go_Root.SetActive(false);

            cell_type = "null";
            imageRatingBg.sprite = spNone;
        }
    }

    public void ClickItemView ()
    {
        if(string.Equals(cell_type, "null"))
            return;

        if(string.Equals(cell_type, "eq")) // !string.IsNullOrEmpty(equip_data.indate))
        {
            PopUpMng.GetInstance().Open_ViewItemInfo(equip_data, false, InfoViewLevelUpSetData, false, true);
            if (equip_data.client_add_sp > MainUI.GetInstance().inventory.tapOpenSP)
            {
                GameDatabase.GetInstance().tableDB.SetEquipNewHide(equip_data);
                Refresh();
            }
        }
        else if (string.Equals(cell_type, "it")) // !string.IsNullOrEmpty(item_data.indate))
        {
            PopUpMng.GetInstance().Open_ViewItemInfo(item_data);
            if (item_data.client_add_sp > MainUI.GetInstance().inventory.tapOpenSP)
            {
                GameDatabase.GetInstance().tableDB.SetItemNewHide(item_data);
                Refresh();
            }
        } 

        MainUI.GetInstance().inventory.initOnStartInventoryAll.regreshCell += this.Refresh;
    }

    void InfoViewLevelUpSetData()
    {
        AScrollCellIndex(nowCellIdx);
    }
}
