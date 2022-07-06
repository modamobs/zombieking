using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollCellEquipLegend : MonoBehaviour
{
    [SerializeField] GameDatabase.TableDB.Equipment _eqdb;
    [SerializeField] PopUpViewEquipmentInfo.InfoViewEquip.Info_icon info_icon;

    [SerializeField] Image imCheck;

    public void ScrollCellIndex(int indx)
    {
        _eqdb = PopUpMng.GetInstance().popUpEquipLegendUpdrage.Get(indx);

        // 아이콘 정보
        info_icon.imIcon.sprite = SpriteAtlasMng.GetInstance().GetSpriteEquip(_eqdb.eq_ty, _eqdb.eq_rt, _eqdb.eq_id);
        info_icon.imRatingBg.sprite = SpriteAtlasMng.GetInstance().GetSpriteRatingBg(_eqdb.eq_rt, _eqdb.eq_legend);
        info_icon.txRating.text = GameDatabase.StringFormat.GetRatingColorText(_eqdb.eq_rt, false);
        info_icon.goLock.SetActive(_eqdb.m_lck == 1);
        info_icon.txEnhantLevel.text = string.Format("+{0}", _eqdb.m_ehnt_lv);
        info_icon.txLevel.text = string.Format("Lv.{0}", _eqdb.m_norm_lv);
        //info_icon.txEquipName.text = GameDatabase.StringFormat.GetEquipName(_eqdb.eq_ty, _eqdb.eq_rt, _eqdb.eq_id);
        //info_etc.imLockBig.color = wearEquipValue.m_lock == 1 ? info_icon.coOkLock : info_icon.coNoLock;

        //// 레벨 이펙트 
        //if (info_icon.goRootEnhant30.activeSelf == !(_eqdb.m_ehnt_lv >= 30))
        //    info_icon.goRootEnhant30.SetActive(_eqdb.m_ehnt_lv >= 30);

        //if (_eqdb.m_ehnt_lv >= 30)
        //{
        //    info_icon.imRtOutlineEnhant30.color = ResourceDatabase.GetInstance().GetItemColor(_eqdb.eq_rt);
        //    info_icon.ugEnhant30.color1 = ResourceDatabase.GetInstance().GetItemGradientColor(_eqdb.eq_rt);
        //    info_icon.ugEnhant30.color2 = ResourceDatabase.GetInstance().GetItemGradientColor(_eqdb.eq_rt);
        //}

        //if (info_icon.goRootNorLv100.activeSelf == !(_eqdb.m_norm_lv >= 100))
        //    info_icon.goRootNorLv100.SetActive(_eqdb.m_norm_lv >= 100);

        //if (_eqdb.m_norm_lv >= 100)
        //{
        //    info_icon.imRtOutlineNorLv100.color = ResourceDatabase.GetInstance().GetItemColor(_eqdb.eq_rt);
        //    //info_icon.psNorLv100.startColor = ResourceDatabase.GetInstance().GetItemColor(_eqdb.eq_rt);
        //}

        Check();
    }

    public void Click_Check()
    {
        PopUpMng.GetInstance().popUpEquipLegendUpdrage.CheckSelectEquipDB(_eqdb);
        Check();
    }

    void Check()
    {
        bool isSelectCheck = PopUpMng.GetInstance().popUpEquipLegendUpdrage.IsCheckMat(_eqdb.aInUid);
        imCheck.sprite = isSelectCheck ? SpriteAtlasMng.GetInstance().GetCheckerMakk() : SpriteAtlasMng.GetInstance().GetTransparency();
    }
}
