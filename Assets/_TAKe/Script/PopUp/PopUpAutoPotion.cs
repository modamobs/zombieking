using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopUpAutoPotion : MonoBehaviour
{
    [SerializeField] ToggleGroup toggles_potion;
    [SerializeField] ToggleGroup toggles_percent;
    [SerializeField] Text txRt1Cnt, txRt2Cnt, txRt3Cnt;
    public void Init()
    {
        // 물약 등급 
        int idx = 1;
        int useRt = ConvenienceFunctionMng.GetInstance().convenFun.cfAutoPosion.iUseRating;
        Toggle[] tog = toggles_potion.transform.GetComponentsInChildren<Toggle>();
        foreach (var item in tog)
        {
            item.isOn = idx == useRt;
            idx++;
        }

        txRt1Cnt.text = string.Format("x{0}", GameDatabase.GetInstance().tableDB.GetItem(20, 1).count);
        txRt2Cnt.text = string.Format("x{0}", GameDatabase.GetInstance().tableDB.GetItem(20, 2).count);
        txRt3Cnt.text = string.Format("x{0}", GameDatabase.GetInstance().tableDB.GetItem(20, 3).count);

        // 사용 HP 기준
        int loadPcr = PlayerPrefs.GetInt(PrefsKeys.key_AutoPotionUsePercent);
        if (loadPcr == 0)
            loadPcr = 30;

        Toggle[] tog2 = toggles_percent.transform.GetComponentsInChildren<Toggle>();
        foreach (var item in tog2)
        {
            Text text = item.transform.GetComponentInChildren<Text>();
            if (text.text == "30%" && loadPcr == 30)
            {
                item.isOn = true;
                break;
            }
            else if(text.text == "50%" && loadPcr == 50)
            {
                item.isOn = true;
                break;
            }
            else if(text.text == "70%" && loadPcr == 70)
            {
                item.isOn = true;
                break;
            }
        }
    }

    public void Click_Confirm()
    {
        // 물약 등급 
        int useRt = 1;
        foreach (Toggle toggle in toggles_potion.ActiveToggles())
        {
            Text t = toggle.GetComponentInChildren<Text>();
            for (int i = 1; i < 8; i++)
            {
                string srtRt = LanguageGameData.GetInstance().GetString(string.Format("item.rating.string.{0}", i));
                if(string.Equals(srtRt, t.text))
                {
                    useRt = i;
                    break;
                }
            }
        }

        // 사용 HP 기준
        int usePct = 30;
        foreach (Toggle toggle in toggles_percent.ActiveToggles())
        {
            Text t = toggle.GetComponentInChildren<Text>();
            if (t.text == "30%")
            {
                usePct = 30;
                break;
            }
            else if (t.text == "50%")
            {
                usePct = 50;
                break;
            }
            else if (t.text == "70%")
            {
                usePct = 70;
                break;
            }
        }

        if (useRt > 0)
        {
            // 선택한 물약 수량 체크 
            int poCnt = GameDatabase.GetInstance().tableDB.GetItem(20, useRt).count;
            if (poCnt > 0)
            {
                ConvenienceFunctionMng.GetInstance().ConvenienceAutoPotion(useRt, usePct);
                gameObject.SetActive(false);
            }
            else
            {
                string corRating = GameDatabase.StringFormat.GetRatingColorText(useRt);
                if (GameMng.GetInstance ().mode_type == IG.ModeType.CHAPTER_CONTINUE || GameMng.GetInstance().mode_type == IG.ModeType.CHAPTER_LOOP)
                {
                    PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener(string.Format("{0} 물약이 부족합니다.\n물약은 필드에서 몬스터 처치 시 드롭되기도 합니다.\n확인 버튼을 누르면 물약 구매 탭으로 이동됩니다.", corRating), MainUI.GetInstance().Listener_MoveItemShop);
                }
                else
                {
                    PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox(string.Format("{0} 물약이 부족합니다.", corRating));
                    gameObject.SetActive(false);
                }
            }
        }
        else PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("물약 등급이 잘못되었습니다. 다시 선택해주세요.");
    }
}
