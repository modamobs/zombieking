using BackEnd;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class ScrollCellPotionConversion : MonoBehaviour
{
    [SerializeField] Image imNowIcon;
    [SerializeField] Image imNowRatingBg;
    [SerializeField] Image imNowRatingOutLine;
    [SerializeField] Text txNowRating;
    [SerializeField] Text txNowCount;

    [SerializeField] Image imAftIcon;
    [SerializeField] Image imAftRatingBg;
    [SerializeField] Image imAftRatingOutLine;
    [SerializeField] Text txAftRating;
    [SerializeField] Text txAftCount;
    public Animation anComplete;

    bool isPressOn = false, isPlus = false;
    [SerializeField] int exch_min, exch_max, exch_cnt, n_it_ty, n_rt, conver_rt, n_cnt, a_cnt;
    [SerializeField] float one_conver_cnt;
    [SerializeField] Text txUseExchCnt;
    [SerializeField] Image imBtnConfirmBg, imMinusBtnBg, imPlusBtnBg, imMinBtnBg, imMaxBtnBg;

    void ScrollCellIndex(int idx)
    {
        anComplete.gameObject.SetActive(false);
        n_it_ty = 20;
        n_rt = PopUpMng.GetInstance().popUpPotionConversion.n_rt;
        n_cnt = GameDatabase.GetInstance().tableDB.GetItem(20, n_rt).count;
        
        conver_rt = PopUpMng.GetInstance().popUpPotionConversion.GetAfterRating(n_rt, idx);
        one_conver_cnt = PopUpMng.GetInstance().popUpPotionConversion.GetRatio(n_rt, idx);
        a_cnt = GameDatabase.GetInstance().tableDB.GetItem(n_it_ty, conver_rt).count;

        exch_min = n_cnt >= one_conver_cnt ? 1 : 0;
        exch_max = n_cnt >= one_conver_cnt ? (int)(n_cnt / one_conver_cnt) : 0;
        exch_cnt = exch_min;

        imNowIcon.sprite = SpriteAtlasMng.GetInstance().GetItemSprite(n_it_ty, n_rt);
        imNowRatingBg.sprite = SpriteAtlasMng.GetInstance().GetSpriteRatingBg(n_rt);
        imNowRatingOutLine.color = ResourceDatabase.GetInstance().GetItemShinyColor(n_rt);
        txNowRating.text = GameDatabase.StringFormat.GetRatingColorText(n_rt);
        txNowCount.text = string.Format("x{0:#,0}", n_cnt);

        imAftIcon.sprite = SpriteAtlasMng.GetInstance().GetItemSprite(n_it_ty, conver_rt);
        imAftRatingBg.sprite = SpriteAtlasMng.GetInstance().GetSpriteRatingBg(conver_rt);
        imAftRatingOutLine.color = ResourceDatabase.GetInstance().GetItemShinyColor(conver_rt);
        txAftRating.text = GameDatabase.StringFormat.GetRatingColorText(conver_rt);
        txAftCount.text = string.Format("<color=#00E0FF>+{0:#,0}</color>\nx{1:#,0}", exch_cnt, a_cnt);

        MinMaxButton();
    }

    public async void Click_ConfirmConversion()
    {
        if (exch_cnt > 0)
        {
            Loading.Bottom(false, true);
            var use_item = GameDatabase.GetInstance().tableDB.GetItem(n_it_ty, n_rt);
            var cvr_item = GameDatabase.GetInstance().tableDB.GetItem(n_it_ty, conver_rt);
            use_item.count -= (int)(exch_cnt * one_conver_cnt);
            cvr_item.count += exch_cnt;
            GameDatabase.GetInstance().tableDB.UpdateClientDB_Item(use_item);
            GameDatabase.GetInstance().tableDB.UpdateClientDB_Item(cvr_item);
            anComplete.gameObject.SetActive(true);
            anComplete.Play("QuestLevelUp");

            if (string.IsNullOrEmpty(cvr_item.indate))
                await GameDatabase.GetInstance().tableDB.SendDataItem(cvr_item);

            await Task.Delay(100);
            PopUpMng.GetInstance().popUpPotionConversion.SetInitCellCall();
            MainUI.GetInstance().tapIAP.tapShopExchange.SetData();
            Loading.Bottom(true, true);
        }
    }

    public void UpPress_Minus() => isPressOn = false;
    public void DwPress_Minus()
    {
        if (exch_cnt > exch_min)
        {
            if (exch_cnt == 1)
            {
                PopUpMng.GetInstance().popUpPotionConversion.SetInitCellCall();
            }
            isPressOn = true;
            isPlus = false;
            StopCoroutine("Routin_OnPress");
            StartCoroutine("Routin_OnPress");
        }
    }

    public void UpPress_Plus() => isPressOn = false;
    public void DwPress_Plus()
    {
        if (exch_cnt >= exch_min && exch_cnt < exch_max)
        {
            if (exch_cnt == 1)
            {
                PopUpMng.GetInstance().popUpPotionConversion.SetInitCellCall();
            }

            isPressOn = true;
            isPlus = true;
            StopCoroutine("Routin_OnPress");
            StartCoroutine("Routin_OnPress");
        }
    }

    public void Click_Min()
    {
        if (exch_min > 0)
        {
            if (exch_cnt == 1)
            {
                PopUpMng.GetInstance().popUpPotionConversion.SetInitCellCall();
            }

            exch_cnt = exch_min;
            txUseExchCnt.text = string.Format("{0:#,0} / {1:#,0}\n모두 변환", exch_cnt, exch_max);
            MinMaxButton();
        }
    }
    public void Click_Max()
    {
        if (exch_cnt < exch_max)
        {
            if (exch_cnt == 1)
            {
                PopUpMng.GetInstance().popUpPotionConversion.SetInitCellCall();
            }
            exch_cnt = exch_max;
            txUseExchCnt.text = string.Format("{0:#,0} / {1:#,0}\n모두 변환", exch_cnt, exch_max);
            MinMaxButton();
        }
    }

    IEnumerator Routin_OnPress()
    {
        float press_time = 0.1f;
        yield return null;
        while (isPressOn)
        {
            int bCnt = exch_cnt;
            if (isPlus)
            {
                if (exch_cnt < exch_max)
                {
                    exch_cnt++;
                    MinMaxButton();
                }
            }
            else
            {
                if (exch_cnt > exch_min)
                {
                    exch_cnt--;
                    MinMaxButton();
                }
            }

            if (bCnt != exch_cnt)
            {
                MinMaxButton();
                yield return new WaitForSeconds(press_time);
                if (press_time > 0.05f)
                    press_time -= 0.2f;
                else if (press_time > 0.025f)
                    press_time -= 0.05f;
                else press_time = 0.025f;
            }
            else break;
        }
    }

    void MinMaxButton()
    {
        imBtnConfirmBg.sprite = exch_cnt <= 0 ? SpriteAtlasMng.GetInstance().GetSpriteButtonRedOrGray(false) : SpriteAtlasMng.GetInstance().GetSpriteButtonRedOrGray(true);
        imMinusBtnBg.sprite = exch_cnt >= 2 ? SpriteAtlasMng.GetInstance().GetSpriteButtonBox("purple") : SpriteAtlasMng.GetInstance().GetSpriteButtonRedOrGray(false);
        imPlusBtnBg.sprite = exch_cnt < exch_max ? SpriteAtlasMng.GetInstance().GetSpriteButtonBox("blue") : SpriteAtlasMng.GetInstance().GetSpriteButtonRedOrGray(false);
        imMinBtnBg.sprite = exch_cnt >= 2 ? SpriteAtlasMng.GetInstance().GetSpriteButtonBox("purple") : SpriteAtlasMng.GetInstance().GetSpriteButtonRedOrGray(false);
        imMaxBtnBg.sprite = exch_cnt < exch_max ? SpriteAtlasMng.GetInstance().GetSpriteButtonBox("blue") : SpriteAtlasMng.GetInstance().GetSpriteButtonRedOrGray(false);
        txUseExchCnt.text = string.Format("{0:#,0} / {1:#,0}\n변환", exch_cnt, exch_max);
        txAftCount.text = string.Format("<color=#00E0FF>+{0:#,0}</color>\nx{1:#,0}", exch_cnt, a_cnt);
    }
}
