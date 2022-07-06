using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class PopUpAutoSale : MonoBehaviour
{
    [SerializeField] Toggle[] toggles_equipType;
    [SerializeField] Toggle[] toggles_equipRating;
    [SerializeField] Toggle[] toggles_acceRating;
    [SerializeField] Toggle[] toggles_saleType;

    [SerializeField]
    IG.ConvenienceFunction.ConvenienceAutoSale.SaleSetting saleSetting = new IG.ConvenienceFunction.ConvenienceAutoSale.SaleSetting();
    [SerializeField] ToggleGroup toggleGroup;
    [SerializeField] Text txConfirmType;

    public void Init()
    {
        //saleSetting = ConvenienceFunctionMng.GetInstance ().convenFun.cfAutoSale.saleSetting;
        string strSetting = PlayerPrefs.GetString(PrefsKeys.key_AutoSaleSetting);
        LogPrint.Print("strSetting : " + strSetting);

        try
        {
            saleSetting = JsonUtility.FromJson<IG.ConvenienceFunction.ConvenienceAutoSale.SaleSetting>(strSetting);
            if (saleSetting.saleEquipType.Length == 0)
                saleSetting.saleEquipType = new bool[3];
            if (saleSetting.saleEquipRating.Length == 0)
                saleSetting.saleEquipRating = new bool[7];
            if (saleSetting.saleAcceRating.Length == 0)
                saleSetting.saleAcceRating = new bool[7];
            if (saleSetting.saleOrDecomp.Length == 0)
                saleSetting.saleOrDecomp = new bool[2];
        }
        catch (System.Exception e)
        { 
            LogPrint.Print("e : " + e);
            saleSetting.saleEquipType = new bool[3];
            saleSetting.saleEquipRating = new bool[7];
            saleSetting.saleAcceRating = new bool[7];
            saleSetting.saleOrDecomp = new bool[2];
        }

        //foreach (Toggle toggle in toggleGroup.ActiveToggles())
        //    toggle.isOn = false;

        for (int i = 0; i < saleSetting.saleEquipType.Length; i++)
            toggles_equipType[i].isOn = saleSetting.saleEquipType[i];

        for (int i = 0; i < saleSetting.saleEquipRating.Length; i++)
            toggles_equipRating[i].isOn = saleSetting.saleEquipRating[i];

        for (int i = 0; i < saleSetting.saleAcceRating.Length; i++)
            toggles_acceRating[i].isOn = saleSetting.saleAcceRating[i];

        for (int i = 0; i < saleSetting.saleOrDecomp.Length; i++)
            toggles_saleType[i].isOn = saleSetting.saleOrDecomp[i];
    }

     public async void ClickToggle_ConfirmButtonText()
    {
        await Task.Delay(250);
        if (toggles_saleType[0].isOn == true)
        {
            txConfirmType.text = "자동 판매 하기";

        } 
        else if (toggles_saleType[1].isOn == true)
        {
            txConfirmType.text = "자동 분해 하기";
        }
    }

    public void Click_Confirm()
    {
        bool isNoSel_eqType = true, isNoSel_acType = true;
        for (int i = 0; i < toggles_equipType.Length; i++)
        {
            saleSetting.saleEquipType[i] = toggles_equipType[i].isOn;
            if(i <= 1)
            {
                if (isNoSel_eqType && saleSetting.saleEquipType[i] == true)
                    isNoSel_eqType = false;
            }
            else if(i == 2)
            {
                if (isNoSel_acType && saleSetting.saleEquipType[i] == true)
                    isNoSel_acType = false;
            }
        }

        bool isNoSel_EquipRating = true;
        for (int i = 0; i < toggles_equipRating.Length; i++)
        {
            saleSetting.saleEquipRating[i] = toggles_equipRating[i].isOn;
            if (isNoSel_EquipRating && saleSetting.saleEquipRating[i] == true)
            {
                isNoSel_EquipRating = false;
            }
        }

        bool isNoSel_AcceRating = true;
        for (int i = 0; i < toggles_acceRating.Length; i++)
        {
            saleSetting.saleAcceRating[i] = toggles_acceRating[i].isOn;
            if (isNoSel_AcceRating && saleSetting.saleAcceRating[i] == true)
            {
                isNoSel_AcceRating = false;
            }
        }

        if (isNoSel_eqType && isNoSel_acType)
        {
            PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("장비 타입을 선택해주세요.");
            return;
        }

        if (!isNoSel_EquipRating && isNoSel_eqType)
        {
            PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("장비 타입을 선태해주세요.");
            return;
        }
        else if (isNoSel_EquipRating && !isNoSel_eqType)
        {
            PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("장비 등급을 선택해주세요.");
            return;
        }

        if (!isNoSel_AcceRating && isNoSel_acType)
        {
            PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("장신구 타입을 선택해주세요.");
            return;
        }
        else if (isNoSel_AcceRating && !isNoSel_acType)
        {
            PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("장신구 등급을 선택해주세요.");
            return;
        }

        bool isSaleOrDecomp = true;
        for (int i = 0; i < toggles_saleType.Length; i++)
        {
            saleSetting.saleOrDecomp[i] = toggles_saleType[i].isOn;
            if (isSaleOrDecomp && saleSetting.saleOrDecomp[i] == true)
            {
                isSaleOrDecomp = false;
            }
        }

        if (isSaleOrDecomp)
        {
            PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("<color=#00C2FF>[자동 판매하기]</color> 또는 <color=#00C2FF>[자동 분해하기]</color>\n둘중 한가지를 선택해주세요.");
            return;
        }

        ConvenienceFunctionMng.GetInstance().ConvenienceAutoSale(saleSetting.saleEquipType, saleSetting.saleEquipRating, saleSetting.saleAcceRating, saleSetting.saleOrDecomp);
        gameObject.SetActive(false);
    }
}
