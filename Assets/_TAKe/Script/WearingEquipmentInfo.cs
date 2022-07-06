using Coffee.UIExtensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WearingEquipmentInfo : MonoBehaviour
{
    [System.Serializable]
    private struct WearingInfo
    {
        public GameDatabase.TableDB.Equipment eqDB;
        public Image image_Icon;
        public Text text_Rating;
        public Image image_RatingBg;
        public Text text_EnhantLevel;
        public Text text_NormalLevel;
        public GameObject go_Lock;

        public GameObject goRootNorLv100;
        public Image imRtOutlineNorLv100;
        public ParticleSystem psNorLv100;

        public GameObject goRootEnhant30;
        public Image imRtOutlineEnhant30;
        public UIGradient ugEnhant30;
        public Animation ani;
    }

    [SerializeField] WearingInfo wearingInfo_weapon;
    [SerializeField] WearingInfo wearingInfo_shield;
    [SerializeField] WearingInfo wearingInfo_helmet;
    [SerializeField] WearingInfo wearingInfo_shoulder;
    [SerializeField] WearingInfo wearingInfo_armor;
    [SerializeField] WearingInfo wearingInfo_arm;
    [SerializeField] WearingInfo wearingInfo_pants;
    [SerializeField] WearingInfo wearingInfo_boots;
    [SerializeField] WearingInfo wearingInfo_neckace;
    [SerializeField] WearingInfo wearingInfo_earring;
    [SerializeField] WearingInfo wearingInfo_ring;

    [SerializeField] private bool isOthersInfo = false;

    private Vector2[] vec3_sizeDelta = new Vector2[] { new Vector2(42, 42), new Vector2(100, 100) };

    void OnEnable ()
    {
        //SetWearingView(true, -1);
    }

    /// <summary>
    /// 각 파츠별로 착용중인 장비 보여주기 새로 고침 
    /// </summary>
    public void SetWearEquipView(int party_type = -1, bool is_others_view = false, GameDatabase.TableDB.Equipment others_equip = default)
    {
        isOthersInfo = is_others_view;
        bool isAll = party_type == -1;
        // 무기 
        if (isAll == true || party_type == 0)
        {
            wearingInfo_weapon.eqDB = is_others_view == false ? GameDatabase.GetInstance().tableDB.GetNowWearingEquipPartsData(0) : others_equip;
            wearingInfo_weapon.image_Icon.sprite = SpriteAtlasMng.GetInstance().GetSpriteEquip(wearingInfo_weapon.eqDB.eq_ty, wearingInfo_weapon.eqDB.eq_rt, wearingInfo_weapon.eqDB.eq_id);
            wearingInfo_weapon.image_Icon.rectTransform.sizeDelta = vec3_sizeDelta[wearingInfo_weapon.eqDB.eq_rt == 0 ? 0 : 1]; // icon size 
            wearingInfo_weapon.image_RatingBg.sprite = SpriteAtlasMng.GetInstance().GetSpriteRatingBg(wearingInfo_weapon.eqDB.eq_rt, wearingInfo_weapon.eqDB.eq_legend);
            wearingInfo_weapon.go_Lock.SetActive(wearingInfo_weapon.eqDB.m_lck == 1);
            wearingInfo_weapon.text_Rating.text = GameDatabase.StringFormat.GetRatingColorText(wearingInfo_weapon.eqDB.eq_rt, false);

            wearingInfo_weapon.text_EnhantLevel.text = wearingInfo_weapon.eqDB.eq_rt > 0 ? string.Format("+{0}", wearingInfo_weapon.eqDB.m_ehnt_lv) : "";
            wearingInfo_weapon.text_NormalLevel.text = wearingInfo_weapon.eqDB.eq_rt > 0 ? string.Format("Lv.{0}", wearingInfo_weapon.eqDB.m_norm_lv) : "";

            if (wearingInfo_weapon.goRootEnhant30.activeSelf == !(wearingInfo_weapon.eqDB.m_ehnt_lv >= 30))
                wearingInfo_weapon.goRootEnhant30.SetActive(wearingInfo_weapon.eqDB.m_ehnt_lv >= 30);

            if (wearingInfo_weapon.eqDB.m_ehnt_lv >= 30)
            {
                wearingInfo_weapon.imRtOutlineEnhant30.color = ResourceDatabase.GetInstance().GetItemColor(wearingInfo_weapon.eqDB.eq_rt);
                wearingInfo_weapon.ugEnhant30.color1 = ResourceDatabase.GetInstance().GetItemGradientColor(wearingInfo_weapon.eqDB.eq_rt);
                wearingInfo_weapon.ugEnhant30.color2 = ResourceDatabase.GetInstance().GetItemGradientColor(wearingInfo_weapon.eqDB.eq_rt);
            }

            if (wearingInfo_weapon.goRootNorLv100.activeSelf == !(wearingInfo_weapon.eqDB.m_norm_lv >= 100))
                wearingInfo_weapon.goRootNorLv100.SetActive(wearingInfo_weapon.eqDB.m_norm_lv >= 100);

            if (wearingInfo_weapon.eqDB.m_norm_lv >= 100)
            {
                wearingInfo_weapon.imRtOutlineNorLv100.color = ResourceDatabase.GetInstance().GetItemColor(wearingInfo_weapon.eqDB.eq_rt);
                wearingInfo_weapon.psNorLv100.startColor = ResourceDatabase.GetInstance().GetItemColor(wearingInfo_weapon.eqDB.eq_rt);
            }
        }

        // 방패 
        if (isAll == true || party_type == 1)
        {
            wearingInfo_shield.eqDB = is_others_view == false ? GameDatabase.GetInstance().tableDB.GetNowWearingEquipPartsData(1) : others_equip;
            Color coTextRating = ResourceDatabase.GetInstance().GetItemColor(wearingInfo_shield.eqDB.eq_rt);
            wearingInfo_shield.image_Icon.sprite = SpriteAtlasMng.GetInstance().GetSpriteEquip(wearingInfo_shield.eqDB.eq_ty, wearingInfo_shield.eqDB.eq_rt, wearingInfo_shield.eqDB.eq_id);
            wearingInfo_shield.image_Icon.rectTransform.sizeDelta = vec3_sizeDelta[wearingInfo_shield.eqDB.eq_rt == 0 ? 0 : 1]; // icon size 
            wearingInfo_shield.image_RatingBg.sprite = SpriteAtlasMng.GetInstance().GetSpriteRatingBg(wearingInfo_shield.eqDB.eq_rt, wearingInfo_shield.eqDB.eq_legend);
            wearingInfo_shield.text_Rating.color = coTextRating;
            wearingInfo_shield.go_Lock.SetActive(wearingInfo_shield.eqDB.m_lck == 1);
            if (wearingInfo_shield.eqDB.eq_rt > 0)
                wearingInfo_shield.text_Rating.text = LanguageGameData.GetInstance().GetString(string.Format("item.rating.string.{0}", wearingInfo_shield.eqDB.eq_rt));
            else wearingInfo_shield.text_Rating.text = "";

            wearingInfo_shield.text_EnhantLevel.text = wearingInfo_shield.eqDB.eq_rt > 0 ? string.Format("+{0}", wearingInfo_shield.eqDB.m_ehnt_lv) : "";
            wearingInfo_shield.text_NormalLevel.text = wearingInfo_shield.eqDB.eq_rt > 0 ? string.Format("Lv.{0}", wearingInfo_shield.eqDB.m_norm_lv) : "";

            if (wearingInfo_shield.goRootEnhant30.activeSelf == !(wearingInfo_shield.eqDB.m_ehnt_lv >= 30))
                wearingInfo_shield.goRootEnhant30.SetActive(wearingInfo_shield.eqDB.m_ehnt_lv >= 30);

            if (wearingInfo_shield.eqDB.m_ehnt_lv >= 30)
            {
                wearingInfo_shield.imRtOutlineEnhant30.color = ResourceDatabase.GetInstance().GetItemColor(wearingInfo_shield.eqDB.eq_rt);
                wearingInfo_shield.ugEnhant30.color1 = ResourceDatabase.GetInstance().GetItemGradientColor(wearingInfo_shield.eqDB.eq_rt);
                wearingInfo_shield.ugEnhant30.color2 = ResourceDatabase.GetInstance().GetItemGradientColor(wearingInfo_shield.eqDB.eq_rt);
            }

            if (wearingInfo_shield.goRootNorLv100.activeSelf == !(wearingInfo_shield.eqDB.m_norm_lv >= 100))
                wearingInfo_shield.goRootNorLv100.SetActive(wearingInfo_shield.eqDB.m_norm_lv >= 100);

            if (wearingInfo_shield.eqDB.m_norm_lv >= 100)
            {
                wearingInfo_shield.imRtOutlineNorLv100.color = ResourceDatabase.GetInstance().GetItemColor(wearingInfo_shield.eqDB.eq_rt);
                wearingInfo_shield.psNorLv100.startColor = ResourceDatabase.GetInstance().GetItemColor(wearingInfo_shield.eqDB.eq_rt);
            }
        }

        // 헬멧 
        if (isAll == true || party_type == 2)
        {
            wearingInfo_helmet.eqDB = is_others_view == false ? GameDatabase.GetInstance().tableDB.GetNowWearingEquipPartsData(2) : others_equip;
            Color coTextRating = ResourceDatabase.GetInstance().GetItemColor(wearingInfo_helmet.eqDB.eq_rt);
            wearingInfo_helmet.image_Icon.sprite = SpriteAtlasMng.GetInstance().GetSpriteEquip(wearingInfo_helmet.eqDB.eq_ty, wearingInfo_helmet.eqDB.eq_rt, wearingInfo_helmet.eqDB.eq_id);
            wearingInfo_helmet.image_Icon.rectTransform.sizeDelta = vec3_sizeDelta[wearingInfo_helmet.eqDB.eq_rt == 0 ? 0 : 1]; // icon size 
            wearingInfo_helmet.image_RatingBg.sprite = SpriteAtlasMng.GetInstance().GetSpriteRatingBg(wearingInfo_helmet.eqDB.eq_rt, wearingInfo_helmet.eqDB.eq_legend);
            wearingInfo_helmet.text_Rating.color = coTextRating;
            wearingInfo_helmet.go_Lock.SetActive(wearingInfo_helmet.eqDB.m_lck == 1);
            if (wearingInfo_helmet.eqDB.eq_rt > 0)
                wearingInfo_helmet.text_Rating.text = LanguageGameData.GetInstance().GetString(string.Format("item.rating.string.{0}", wearingInfo_helmet.eqDB.eq_rt));
            else wearingInfo_helmet.text_Rating.text = "";

            wearingInfo_helmet.text_EnhantLevel.text = wearingInfo_helmet.eqDB.eq_rt > 0 ? string.Format("+{0}", wearingInfo_helmet.eqDB.m_ehnt_lv) : "";
            wearingInfo_helmet.text_NormalLevel.text = wearingInfo_helmet.eqDB.eq_rt > 0 ? string.Format("Lv.{0}", wearingInfo_helmet.eqDB.m_norm_lv) : "";

            if (wearingInfo_helmet.goRootEnhant30.activeSelf == !(wearingInfo_helmet.eqDB.m_ehnt_lv >= 30))
                wearingInfo_helmet.goRootEnhant30.SetActive(wearingInfo_helmet.eqDB.m_ehnt_lv >= 30);

            if (wearingInfo_helmet.eqDB.m_ehnt_lv >= 30)
            {
                wearingInfo_helmet.imRtOutlineEnhant30.color = ResourceDatabase.GetInstance().GetItemColor(wearingInfo_helmet.eqDB.eq_rt);
                wearingInfo_helmet.ugEnhant30.color1 = ResourceDatabase.GetInstance().GetItemGradientColor(wearingInfo_helmet.eqDB.eq_rt);
                wearingInfo_helmet.ugEnhant30.color2 = ResourceDatabase.GetInstance().GetItemGradientColor(wearingInfo_helmet.eqDB.eq_rt);
            }

            if (wearingInfo_helmet.goRootNorLv100.activeSelf == !(wearingInfo_helmet.eqDB.m_norm_lv >= 100))
                wearingInfo_helmet.goRootNorLv100.SetActive(wearingInfo_helmet.eqDB.m_norm_lv >= 100);

            if (wearingInfo_helmet.eqDB.m_norm_lv >= 100)
            {
                wearingInfo_helmet.imRtOutlineNorLv100.color = ResourceDatabase.GetInstance().GetItemColor(wearingInfo_helmet.eqDB.eq_rt);
                wearingInfo_helmet.psNorLv100.startColor = ResourceDatabase.GetInstance().GetItemColor(wearingInfo_helmet.eqDB.eq_rt);
            }
        }

        // 어깨 
        if (isAll == true || party_type == 3)
        {
            wearingInfo_shoulder.eqDB = is_others_view == false ? GameDatabase.GetInstance().tableDB.GetNowWearingEquipPartsData(3) : others_equip;
            Color coTextRating = ResourceDatabase.GetInstance().GetItemColor(wearingInfo_shoulder.eqDB.eq_rt);
            wearingInfo_shoulder.image_Icon.sprite = SpriteAtlasMng.GetInstance().GetSpriteEquip(wearingInfo_shoulder.eqDB.eq_ty, wearingInfo_shoulder.eqDB.eq_rt, wearingInfo_shoulder.eqDB.eq_id);
            wearingInfo_shoulder.image_Icon.rectTransform.sizeDelta = vec3_sizeDelta[wearingInfo_shoulder.eqDB.eq_rt == 0 ? 0 : 1]; // icon size 
            wearingInfo_shoulder.image_RatingBg.sprite = SpriteAtlasMng.GetInstance().GetSpriteRatingBg(wearingInfo_shoulder.eqDB.eq_rt, wearingInfo_shoulder.eqDB.eq_legend);
            wearingInfo_shoulder.text_Rating.color = coTextRating;
            wearingInfo_shoulder.go_Lock.SetActive(wearingInfo_shoulder.eqDB.m_lck == 1);
            if (wearingInfo_shoulder.eqDB.eq_rt > 0)
                wearingInfo_shoulder.text_Rating.text = LanguageGameData.GetInstance().GetString(string.Format("item.rating.string.{0}", wearingInfo_shoulder.eqDB.eq_rt));
            else wearingInfo_shoulder.text_Rating.text = "";

            wearingInfo_shoulder.text_EnhantLevel.text = wearingInfo_shoulder.eqDB.eq_rt > 0 ? string.Format("+{0}", wearingInfo_shoulder.eqDB.m_ehnt_lv) : "";
            wearingInfo_shoulder.text_NormalLevel.text = wearingInfo_shoulder.eqDB.eq_rt > 0 ? string.Format("Lv.{0}", wearingInfo_shoulder.eqDB.m_norm_lv) : "";

            if (wearingInfo_shoulder.goRootEnhant30.activeSelf == !(wearingInfo_shoulder.eqDB.m_ehnt_lv >= 30))
                wearingInfo_shoulder.goRootEnhant30.SetActive(wearingInfo_shoulder.eqDB.m_ehnt_lv >= 30);

            if (wearingInfo_shoulder.eqDB.m_ehnt_lv >= 30)
            {
                wearingInfo_shoulder.imRtOutlineEnhant30.color = ResourceDatabase.GetInstance().GetItemColor(wearingInfo_shoulder.eqDB.eq_rt);
                wearingInfo_shoulder.ugEnhant30.color1 = ResourceDatabase.GetInstance().GetItemGradientColor(wearingInfo_shoulder.eqDB.eq_rt);
                wearingInfo_shoulder.ugEnhant30.color2 = ResourceDatabase.GetInstance().GetItemGradientColor(wearingInfo_shoulder.eqDB.eq_rt);
            }

            if (wearingInfo_shoulder.goRootNorLv100.activeSelf == !(wearingInfo_shoulder.eqDB.m_norm_lv >= 100))
                wearingInfo_shoulder.goRootNorLv100.SetActive(wearingInfo_shoulder.eqDB.m_norm_lv >= 100);

            if (wearingInfo_shoulder.eqDB.m_norm_lv >= 100)
            {
                wearingInfo_shoulder.imRtOutlineNorLv100.color = ResourceDatabase.GetInstance().GetItemColor(wearingInfo_shoulder.eqDB.eq_rt);
                wearingInfo_shoulder.psNorLv100.startColor = ResourceDatabase.GetInstance().GetItemColor(wearingInfo_shoulder.eqDB.eq_rt);
            }
        }

        // 갑옷 
        if (isAll == true || party_type == 4)
        {
            wearingInfo_armor.eqDB = is_others_view == false ? GameDatabase.GetInstance().tableDB.GetNowWearingEquipPartsData(4) : others_equip;
            Color coTextRating = ResourceDatabase.GetInstance().GetItemColor(wearingInfo_armor.eqDB.eq_rt);
            wearingInfo_armor.image_Icon.sprite = SpriteAtlasMng.GetInstance().GetSpriteEquip(wearingInfo_armor.eqDB.eq_ty, wearingInfo_armor.eqDB.eq_rt, wearingInfo_armor.eqDB.eq_id);
            wearingInfo_armor.image_Icon.rectTransform.sizeDelta = vec3_sizeDelta[wearingInfo_armor.eqDB.eq_rt == 0 ? 0 : 1]; // icon size 
            wearingInfo_armor.image_RatingBg.sprite = SpriteAtlasMng.GetInstance().GetSpriteRatingBg(wearingInfo_armor.eqDB.eq_rt, wearingInfo_armor.eqDB.eq_legend);
            wearingInfo_armor.text_Rating.color = coTextRating;
            wearingInfo_armor.go_Lock.SetActive(wearingInfo_armor.eqDB.m_lck == 1);
            if (wearingInfo_armor.eqDB.eq_rt > 0)
                wearingInfo_armor.text_Rating.text = LanguageGameData.GetInstance().GetString(string.Format("item.rating.string.{0}", wearingInfo_armor.eqDB.eq_rt));
            else wearingInfo_armor.text_Rating.text = "";

            wearingInfo_armor.text_EnhantLevel.text = wearingInfo_armor.eqDB.eq_rt > 0 ? string.Format("+{0}", wearingInfo_armor.eqDB.m_ehnt_lv) : "";
            wearingInfo_armor.text_NormalLevel.text = wearingInfo_armor.eqDB.eq_rt > 0 ? string.Format("Lv.{0}", wearingInfo_armor.eqDB.m_norm_lv) : "";

            if (wearingInfo_armor.goRootEnhant30.activeSelf == !(wearingInfo_armor.eqDB.m_ehnt_lv >= 30))
                wearingInfo_armor.goRootEnhant30.SetActive(wearingInfo_armor.eqDB.m_ehnt_lv >= 30);

            if (wearingInfo_armor.eqDB.m_ehnt_lv >= 30)
            {
                wearingInfo_armor.imRtOutlineEnhant30.color = ResourceDatabase.GetInstance().GetItemColor(wearingInfo_armor.eqDB.eq_rt);
                wearingInfo_armor.ugEnhant30.color1 = ResourceDatabase.GetInstance().GetItemGradientColor(wearingInfo_armor.eqDB.eq_rt);
                wearingInfo_armor.ugEnhant30.color2 = ResourceDatabase.GetInstance().GetItemGradientColor(wearingInfo_armor.eqDB.eq_rt);
            }

            if (wearingInfo_armor.goRootNorLv100.activeSelf == !(wearingInfo_armor.eqDB.m_norm_lv >= 100))
                wearingInfo_armor.goRootNorLv100.SetActive(wearingInfo_armor.eqDB.m_norm_lv >= 100);

            if (wearingInfo_armor.eqDB.m_norm_lv >= 100)
            {
                wearingInfo_armor.imRtOutlineNorLv100.color = ResourceDatabase.GetInstance().GetItemColor(wearingInfo_armor.eqDB.eq_rt);
                wearingInfo_armor.psNorLv100.startColor = ResourceDatabase.GetInstance().GetItemColor(wearingInfo_armor.eqDB.eq_rt);
            }
        }

        // 팔 
        if (isAll == true || party_type == 5)
        {
            wearingInfo_arm.eqDB = is_others_view == false ? GameDatabase.GetInstance().tableDB.GetNowWearingEquipPartsData(5) : others_equip;
            Color coTextRating = ResourceDatabase.GetInstance().GetItemColor(wearingInfo_arm.eqDB.eq_rt);
            wearingInfo_arm.image_Icon.sprite = SpriteAtlasMng.GetInstance().GetSpriteEquip(wearingInfo_arm.eqDB.eq_ty, wearingInfo_arm.eqDB.eq_rt, wearingInfo_arm.eqDB.eq_id);
            wearingInfo_arm.image_Icon.rectTransform.sizeDelta = vec3_sizeDelta[wearingInfo_arm.eqDB.eq_rt == 0 ? 0 : 1]; // icon size 
            wearingInfo_arm.image_RatingBg.sprite = SpriteAtlasMng.GetInstance().GetSpriteRatingBg(wearingInfo_arm.eqDB.eq_rt, wearingInfo_arm.eqDB.eq_legend);
            wearingInfo_arm.text_Rating.color = coTextRating;
            wearingInfo_arm.go_Lock.SetActive(wearingInfo_arm.eqDB.m_lck == 1);
            if (wearingInfo_arm.eqDB.eq_rt > 0)
                wearingInfo_arm.text_Rating.text = LanguageGameData.GetInstance().GetString(string.Format("item.rating.string.{0}", wearingInfo_arm.eqDB.eq_rt));
            else wearingInfo_arm.text_Rating.text = "";

            wearingInfo_arm.text_EnhantLevel.text = wearingInfo_arm.eqDB.eq_rt > 0 ? string.Format("+{0}", wearingInfo_arm.eqDB.m_ehnt_lv) : "";
            wearingInfo_arm.text_NormalLevel.text = wearingInfo_arm.eqDB.eq_rt > 0 ? string.Format("Lv.{0}", wearingInfo_arm.eqDB.m_norm_lv) : "";

            if (wearingInfo_arm.goRootEnhant30.activeSelf == !(wearingInfo_arm.eqDB.m_ehnt_lv >= 30))
                wearingInfo_arm.goRootEnhant30.SetActive(wearingInfo_arm.eqDB.m_ehnt_lv >= 30);

            if (wearingInfo_arm.eqDB.m_ehnt_lv >= 30)
            {
                wearingInfo_arm.imRtOutlineEnhant30.color = ResourceDatabase.GetInstance().GetItemColor(wearingInfo_arm.eqDB.eq_rt);
                wearingInfo_arm.ugEnhant30.color1 = ResourceDatabase.GetInstance().GetItemGradientColor(wearingInfo_arm.eqDB.eq_rt);
                wearingInfo_arm.ugEnhant30.color2 = ResourceDatabase.GetInstance().GetItemGradientColor(wearingInfo_arm.eqDB.eq_rt);
            }

            if (wearingInfo_arm.goRootNorLv100.activeSelf == !(wearingInfo_arm.eqDB.m_norm_lv >= 100))
                wearingInfo_arm.goRootNorLv100.SetActive(wearingInfo_arm.eqDB.m_norm_lv >= 100);

            if (wearingInfo_arm.eqDB.m_norm_lv >= 100)
            {
                wearingInfo_arm.imRtOutlineNorLv100.color = ResourceDatabase.GetInstance().GetItemColor(wearingInfo_arm.eqDB.eq_rt);
                wearingInfo_arm.psNorLv100.startColor = ResourceDatabase.GetInstance().GetItemColor(wearingInfo_arm.eqDB.eq_rt);
            }
        }

        // 바지 
        if (isAll == true || party_type == 6)
        {
            wearingInfo_pants.eqDB = is_others_view == false ? GameDatabase.GetInstance().tableDB.GetNowWearingEquipPartsData(6) : others_equip;
            Color coTextRating = ResourceDatabase.GetInstance().GetItemColor(wearingInfo_pants.eqDB.eq_rt);
            wearingInfo_pants.image_Icon.sprite = SpriteAtlasMng.GetInstance().GetSpriteEquip(wearingInfo_pants.eqDB.eq_ty, wearingInfo_pants.eqDB.eq_rt, wearingInfo_pants.eqDB.eq_id);
            wearingInfo_pants.image_Icon.rectTransform.sizeDelta = vec3_sizeDelta[wearingInfo_pants.eqDB.eq_rt == 0 ? 0 : 1]; // icon size 
            wearingInfo_pants.image_RatingBg.sprite = SpriteAtlasMng.GetInstance().GetSpriteRatingBg(wearingInfo_pants.eqDB.eq_rt, wearingInfo_pants.eqDB.eq_legend);
            wearingInfo_pants.text_Rating.color = coTextRating;
            wearingInfo_pants.go_Lock.SetActive(wearingInfo_pants.eqDB.m_lck == 1);
            if (wearingInfo_pants.eqDB.eq_rt > 0)
                wearingInfo_pants.text_Rating.text = LanguageGameData.GetInstance().GetString(string.Format("item.rating.string.{0}", wearingInfo_pants.eqDB.eq_rt));
            else wearingInfo_pants.text_Rating.text = "";

            wearingInfo_pants.text_EnhantLevel.text = wearingInfo_pants.eqDB.eq_rt > 0 ? string.Format("+{0}", wearingInfo_pants.eqDB.m_ehnt_lv) : "";
            wearingInfo_pants.text_NormalLevel.text = wearingInfo_pants.eqDB.eq_rt > 0 ? string.Format("Lv.{0}", wearingInfo_pants.eqDB.m_norm_lv) : "";

            if (wearingInfo_pants.goRootEnhant30.activeSelf == !(wearingInfo_pants.eqDB.m_ehnt_lv >= 30))
                wearingInfo_pants.goRootEnhant30.SetActive(wearingInfo_pants.eqDB.m_ehnt_lv >= 30);

            if (wearingInfo_pants.eqDB.m_ehnt_lv >= 30)
            {
                wearingInfo_pants.imRtOutlineEnhant30.color = ResourceDatabase.GetInstance().GetItemColor(wearingInfo_pants.eqDB.eq_rt);
                wearingInfo_pants.ugEnhant30.color1 = ResourceDatabase.GetInstance().GetItemGradientColor(wearingInfo_pants.eqDB.eq_rt);
                wearingInfo_pants.ugEnhant30.color2 = ResourceDatabase.GetInstance().GetItemGradientColor(wearingInfo_pants.eqDB.eq_rt);
            }

            if (wearingInfo_pants.goRootNorLv100.activeSelf == !(wearingInfo_pants.eqDB.m_norm_lv >= 100))
                wearingInfo_pants.goRootNorLv100.SetActive(wearingInfo_pants.eqDB.m_norm_lv >= 100);

            if (wearingInfo_pants.eqDB.m_norm_lv >= 100)
            {
                wearingInfo_pants.imRtOutlineNorLv100.color = ResourceDatabase.GetInstance().GetItemColor(wearingInfo_pants.eqDB.eq_rt);
                wearingInfo_pants.psNorLv100.startColor = ResourceDatabase.GetInstance().GetItemColor(wearingInfo_pants.eqDB.eq_rt);
            }
        }

        // 부츠 
        if(isAll == true || party_type == 7)
        {
            wearingInfo_boots.eqDB = is_others_view == false ? GameDatabase.GetInstance().tableDB.GetNowWearingEquipPartsData(7) : others_equip;
            Color coTextRating = ResourceDatabase.GetInstance().GetItemColor(wearingInfo_boots.eqDB.eq_rt);
            wearingInfo_boots.image_Icon.sprite = SpriteAtlasMng.GetInstance().GetSpriteEquip(wearingInfo_boots.eqDB.eq_ty, wearingInfo_boots.eqDB.eq_rt, wearingInfo_boots.eqDB.eq_id);
            wearingInfo_boots.image_Icon.rectTransform.sizeDelta = vec3_sizeDelta[wearingInfo_boots.eqDB.eq_rt == 0 ? 0 : 1]; // icon size 
            wearingInfo_boots.image_RatingBg.sprite = SpriteAtlasMng.GetInstance().GetSpriteRatingBg(wearingInfo_boots.eqDB.eq_rt, wearingInfo_boots.eqDB.eq_legend);
            wearingInfo_boots.text_Rating.color = coTextRating;
            wearingInfo_boots.go_Lock.SetActive(wearingInfo_boots.eqDB.m_lck == 1);
            if (wearingInfo_boots.eqDB.eq_rt > 0)
                wearingInfo_boots.text_Rating.text = LanguageGameData.GetInstance().GetString(string.Format("item.rating.string.{0}", wearingInfo_boots.eqDB.eq_rt));
            else wearingInfo_boots.text_Rating.text = "";

            wearingInfo_boots.text_EnhantLevel.text = wearingInfo_boots.eqDB.eq_rt > 0 ? string.Format("+{0}", wearingInfo_boots.eqDB.m_ehnt_lv) : "";
            wearingInfo_boots.text_NormalLevel.text = wearingInfo_boots.eqDB.eq_rt > 0 ? string.Format("Lv.{0}", wearingInfo_boots.eqDB.m_norm_lv) : "";

            if (wearingInfo_boots.goRootEnhant30.activeSelf == !(wearingInfo_boots.eqDB.m_ehnt_lv >= 30))
                wearingInfo_boots.goRootEnhant30.SetActive(wearingInfo_boots.eqDB.m_ehnt_lv >= 30);

            if (wearingInfo_boots.eqDB.m_ehnt_lv >= 30)
            {
                wearingInfo_boots.imRtOutlineEnhant30.color = ResourceDatabase.GetInstance().GetItemColor(wearingInfo_boots.eqDB.eq_rt);
                wearingInfo_boots.ugEnhant30.color1 = ResourceDatabase.GetInstance().GetItemGradientColor(wearingInfo_boots.eqDB.eq_rt);
                wearingInfo_boots.ugEnhant30.color2 = ResourceDatabase.GetInstance().GetItemGradientColor(wearingInfo_boots.eqDB.eq_rt);
            }

            if (wearingInfo_boots.goRootNorLv100.activeSelf == !(wearingInfo_boots.eqDB.m_norm_lv >= 100))
                wearingInfo_boots.goRootNorLv100.SetActive(wearingInfo_boots.eqDB.m_norm_lv >= 100);

            if (wearingInfo_boots.eqDB.m_norm_lv >= 100)
            {
                wearingInfo_boots.imRtOutlineNorLv100.color = ResourceDatabase.GetInstance().GetItemColor(wearingInfo_boots.eqDB.eq_rt); 
                wearingInfo_boots.psNorLv100.startColor = ResourceDatabase.GetInstance().GetItemColor(wearingInfo_boots.eqDB.eq_rt);
            }
        }

        // 목걸이 
        if (isAll == true || party_type == 8)
        {
            wearingInfo_neckace.eqDB = is_others_view == false ? GameDatabase.GetInstance().tableDB.GetNowWearingEquipPartsData(8) : others_equip;
            Color coTextRating = ResourceDatabase.GetInstance().GetItemColor(wearingInfo_neckace.eqDB.eq_rt);
            wearingInfo_neckace.image_Icon.sprite = SpriteAtlasMng.GetInstance().GetSpriteEquip(wearingInfo_neckace.eqDB.eq_ty, wearingInfo_neckace.eqDB.eq_rt, wearingInfo_neckace.eqDB.eq_id);
            wearingInfo_neckace.image_Icon.rectTransform.sizeDelta = vec3_sizeDelta[wearingInfo_neckace.eqDB.eq_rt == 0 ? 0 : 1]; // icon size 
            wearingInfo_neckace.image_RatingBg.sprite = SpriteAtlasMng.GetInstance().GetSpriteRatingBg(wearingInfo_neckace.eqDB.eq_rt, wearingInfo_neckace.eqDB.eq_legend);
            wearingInfo_neckace.text_Rating.color = coTextRating;
            wearingInfo_neckace.go_Lock.SetActive(wearingInfo_neckace.eqDB.m_lck == 1);
            if (wearingInfo_neckace.eqDB.eq_rt > 0)
                wearingInfo_neckace.text_Rating.text = LanguageGameData.GetInstance().GetString(string.Format("item.rating.string.{0}", wearingInfo_neckace.eqDB.eq_rt));
            else wearingInfo_neckace.text_Rating.text = "";

            wearingInfo_neckace.text_EnhantLevel.text = wearingInfo_neckace.eqDB.eq_rt > 0 ? string.Format("+{0}", wearingInfo_neckace.eqDB.m_ehnt_lv) : "";
            wearingInfo_neckace.text_NormalLevel.text = wearingInfo_neckace.eqDB.eq_rt > 0 ? string.Format("Lv.{0}", wearingInfo_neckace.eqDB.m_norm_lv) : "";

            if (wearingInfo_neckace.goRootEnhant30.activeSelf == !(wearingInfo_neckace.eqDB.m_ehnt_lv >= 30))
                wearingInfo_neckace.goRootEnhant30.SetActive(wearingInfo_neckace.eqDB.m_ehnt_lv >= 30);

            if (wearingInfo_neckace.eqDB.m_ehnt_lv >= 30)
            {
                wearingInfo_neckace.imRtOutlineEnhant30.color = ResourceDatabase.GetInstance().GetItemColor(wearingInfo_neckace.eqDB.eq_rt);
                wearingInfo_neckace.ugEnhant30.color1 = ResourceDatabase.GetInstance().GetItemGradientColor(wearingInfo_neckace.eqDB.eq_rt);
                wearingInfo_neckace.ugEnhant30.color2 = ResourceDatabase.GetInstance().GetItemGradientColor(wearingInfo_neckace.eqDB.eq_rt);
            }

            if (wearingInfo_neckace.goRootNorLv100.activeSelf == !(wearingInfo_neckace.eqDB.m_norm_lv >= 100))
                wearingInfo_neckace.goRootNorLv100.SetActive(wearingInfo_neckace.eqDB.m_norm_lv >= 100);

            if (wearingInfo_neckace.eqDB.m_norm_lv >= 100)
            {
                wearingInfo_neckace.imRtOutlineNorLv100.color = ResourceDatabase.GetInstance().GetItemColor(wearingInfo_neckace.eqDB.eq_rt);
                wearingInfo_neckace.psNorLv100.startColor = ResourceDatabase.GetInstance().GetItemColor(wearingInfo_neckace.eqDB.eq_rt);
            }
        }

        // 귀고리 
        if (isAll == true || party_type == 9)
        {
            wearingInfo_earring.eqDB = is_others_view == false ? GameDatabase.GetInstance().tableDB.GetNowWearingEquipPartsData(9) : others_equip;
            Color coTextRating = ResourceDatabase.GetInstance().GetItemColor(wearingInfo_earring.eqDB.eq_rt);
            wearingInfo_earring.image_Icon.sprite = SpriteAtlasMng.GetInstance().GetSpriteEquip(wearingInfo_earring.eqDB.eq_ty, wearingInfo_earring.eqDB.eq_rt, wearingInfo_earring.eqDB.eq_id);
            wearingInfo_earring.image_Icon.rectTransform.sizeDelta = vec3_sizeDelta[wearingInfo_earring.eqDB.eq_rt == 0 ? 0 : 1]; // icon size 
            wearingInfo_earring.image_RatingBg.sprite = SpriteAtlasMng.GetInstance().GetSpriteRatingBg(wearingInfo_earring.eqDB.eq_rt, wearingInfo_earring.eqDB.eq_legend);
            wearingInfo_earring.text_Rating.color = coTextRating;
            wearingInfo_earring.go_Lock.SetActive(wearingInfo_earring.eqDB.m_lck == 1);
            if (wearingInfo_earring.eqDB.eq_rt > 0)
                wearingInfo_earring.text_Rating.text = LanguageGameData.GetInstance().GetString(string.Format("item.rating.string.{0}", wearingInfo_earring.eqDB.eq_rt));
            else wearingInfo_earring.text_Rating.text = "";

            wearingInfo_earring.text_EnhantLevel.text = wearingInfo_earring.eqDB.eq_rt > 0 ? string.Format("+{0}", wearingInfo_earring.eqDB.m_ehnt_lv) : "";
            wearingInfo_earring.text_NormalLevel.text = wearingInfo_earring.eqDB.eq_rt > 0 ? string.Format("Lv.{0}", wearingInfo_earring.eqDB.m_norm_lv) : "";

            if (wearingInfo_earring.goRootEnhant30.activeSelf == !(wearingInfo_earring.eqDB.m_ehnt_lv >= 30))
                wearingInfo_earring.goRootEnhant30.SetActive(wearingInfo_earring.eqDB.m_ehnt_lv >= 30);

            if (wearingInfo_earring.eqDB.m_ehnt_lv >= 30)
            {
                wearingInfo_earring.imRtOutlineEnhant30.color = ResourceDatabase.GetInstance().GetItemColor(wearingInfo_earring.eqDB.eq_rt);
                wearingInfo_earring.ugEnhant30.color1 = ResourceDatabase.GetInstance().GetItemGradientColor(wearingInfo_earring.eqDB.eq_rt);
                wearingInfo_earring.ugEnhant30.color2 = ResourceDatabase.GetInstance().GetItemGradientColor(wearingInfo_earring.eqDB.eq_rt);
            }

            if (wearingInfo_earring.goRootNorLv100.activeSelf == !(wearingInfo_earring.eqDB.m_norm_lv >= 100))
                wearingInfo_earring.goRootNorLv100.SetActive(wearingInfo_earring.eqDB.m_norm_lv >= 100);

            if (wearingInfo_earring.eqDB.m_norm_lv >= 100)
            {
                wearingInfo_earring.imRtOutlineNorLv100.color = ResourceDatabase.GetInstance().GetItemColor(wearingInfo_earring.eqDB.eq_rt);
                wearingInfo_earring.psNorLv100.startColor = ResourceDatabase.GetInstance().GetItemColor(wearingInfo_earring.eqDB.eq_rt);

            }
        }

        // 반지 
        if (isAll == true || party_type == 10)
        {
            wearingInfo_ring.eqDB = is_others_view == false ? GameDatabase.GetInstance().tableDB.GetNowWearingEquipPartsData(10) : others_equip;
            Color coTextRating = ResourceDatabase.GetInstance().GetItemColor(wearingInfo_ring.eqDB.eq_rt);
            wearingInfo_ring.image_Icon.sprite = SpriteAtlasMng.GetInstance().GetSpriteEquip(wearingInfo_ring.eqDB.eq_ty, wearingInfo_ring.eqDB.eq_rt, wearingInfo_ring.eqDB.eq_id);
            wearingInfo_ring.image_Icon.rectTransform.sizeDelta = vec3_sizeDelta[wearingInfo_ring.eqDB.eq_rt == 0 ? 0 : 1]; // icon size 
            wearingInfo_ring.image_RatingBg.sprite = SpriteAtlasMng.GetInstance().GetSpriteRatingBg(wearingInfo_ring.eqDB.eq_rt, wearingInfo_ring.eqDB.eq_legend);
            wearingInfo_ring.text_Rating.color = coTextRating;
            wearingInfo_ring.go_Lock.SetActive(wearingInfo_ring.eqDB.m_lck == 1);
            if (wearingInfo_ring.eqDB.eq_rt > 0)
                wearingInfo_ring.text_Rating.text = LanguageGameData.GetInstance().GetString(string.Format("item.rating.string.{0}", wearingInfo_ring.eqDB.eq_rt));
            else wearingInfo_ring.text_Rating.text = "";

            wearingInfo_ring.text_EnhantLevel.text = wearingInfo_ring.eqDB.eq_rt > 0 ? string.Format("+{0}", wearingInfo_ring.eqDB.m_ehnt_lv) : "";
            wearingInfo_ring.text_NormalLevel.text = wearingInfo_ring.eqDB.eq_rt > 0 ? string.Format("Lv.{0}", wearingInfo_ring.eqDB.m_norm_lv) : "";

            if (wearingInfo_ring.goRootEnhant30.activeSelf == !(wearingInfo_ring.eqDB.m_ehnt_lv >= 30))
                wearingInfo_ring.goRootEnhant30.SetActive(wearingInfo_ring.eqDB.m_ehnt_lv >= 30);

            if (wearingInfo_ring.eqDB.m_ehnt_lv >= 30)
            {
                wearingInfo_ring.imRtOutlineEnhant30.color = ResourceDatabase.GetInstance().GetItemColor(wearingInfo_ring.eqDB.eq_rt);
                wearingInfo_ring.ugEnhant30.color1 = ResourceDatabase.GetInstance().GetItemGradientColor(wearingInfo_ring.eqDB.eq_rt);
                wearingInfo_ring.ugEnhant30.color2 = ResourceDatabase.GetInstance().GetItemGradientColor(wearingInfo_ring.eqDB.eq_rt);
            }

            if (wearingInfo_ring.goRootNorLv100.activeSelf == !(wearingInfo_ring.eqDB.m_norm_lv >= 100))
                wearingInfo_ring.goRootNorLv100.SetActive(wearingInfo_ring.eqDB.m_norm_lv >= 100);

            if (wearingInfo_ring.eqDB.m_norm_lv >= 100)
            {
                wearingInfo_ring.imRtOutlineNorLv100.color = ResourceDatabase.GetInstance().GetItemColor(wearingInfo_ring.eqDB.eq_rt);
                wearingInfo_ring.psNorLv100.startColor = ResourceDatabase.GetInstance().GetItemColor(wearingInfo_ring.eqDB.eq_rt);
            }
        }
    }

    public void PlayNormalLevelAni(int eq_ty)
    {
        switch (eq_ty)
        {
            case 0: wearingInfo_weapon.ani.Play("QuestLevelUp"); break;
            case 1: wearingInfo_shield.ani.Play("QuestLevelUp"); break;
            case 2: wearingInfo_helmet.ani.Play("QuestLevelUp"); break;
            case 3: wearingInfo_shoulder.ani.Play("QuestLevelUp"); break;
            case 4: wearingInfo_armor.ani.Play("QuestLevelUp"); break;
            case 5: wearingInfo_arm.ani.Play("QuestLevelUp"); break;
            case 6: wearingInfo_pants.ani.Play("QuestLevelUp"); break;
            case 7: wearingInfo_boots.ani.Play("QuestLevelUp"); break;
            case 8: wearingInfo_neckace.ani.Play("QuestLevelUp"); break;
            case 9: wearingInfo_earring.ani.Play("QuestLevelUp"); break;
            case 10: wearingInfo_ring.ani.Play("QuestLevelUp");  break;
        }
    }

    private GameDatabase.TableDB.Equipment GetWearNowDb (int eq_ty)
    {
        switch (eq_ty)
        {
            case 0: return wearingInfo_weapon.eqDB;
            case 1: return wearingInfo_shield.eqDB;
            case 2: return wearingInfo_helmet.eqDB;
            case 3: return wearingInfo_shoulder.eqDB;
            case 4: return wearingInfo_armor.eqDB;
            case 5: return wearingInfo_arm.eqDB;
            case 6: return wearingInfo_pants.eqDB;
            case 7: return wearingInfo_boots.eqDB;
            case 8: return wearingInfo_neckace.eqDB;
            case 9: return wearingInfo_earring.eqDB;
            case 10: return wearingInfo_ring.eqDB;
        }

        return default;
    }

    public void ClickWearingEquipInfo(int eq_ty)
    {
        if (isOthersInfo == true) // 다른 사람 정보 
        {
            var _eqdb = GetWearNowDb(eq_ty);
            LogPrint.EditorPrint(JsonUtility.ToJson(_eqdb));
            if (_eqdb.ma_st_id > 0)
                PopUpMng.GetInstance().Open_ViewItemInfo(GetWearNowDb(eq_ty), true, null, false, false);
            else
                LogPrint.EditorPrint("유저의 장비 정보가 갱신되지 않았습니다.");
        }
        else // 내 정보 
        {
            var _eqdb = GameDatabase.GetInstance().tableDB.GetNowWearingEquipPartsData(eq_ty);
            LogPrint.EditorPrint("내정보 : " + JsonUtility.ToJson(_eqdb));
            if (_eqdb.eq_rt > 0) // || (eq_data.eq_ty == 0 || eq_data.eq_ty == 1)) // 장비 등급이 일반등급이어야하고 착용장비 팝어오픈가능하고, 예외적으로 무기, 방패는 기본 장비도 팝업가능. 
            {
                PopUpMng.GetInstance().Open_ViewItemInfo(_eqdb, false, null, false, true);
            }
        }
    }
}