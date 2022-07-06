using BackEnd;
using static BackEnd.BackendAsyncClass;
using Coffee.UIExtensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using System;
using LitJson;

/// <summary>
/// 장비 강화석 진화 
/// </summary>
public class TapSmithyEquipmentStonEvolution : MonoBehaviour
{
    [SerializeField] private GameDatabase.TableDB.Item temp_Item;

    [SerializeField] BlockScreen blockScreen;
    [SerializeField] Animator ani;
    [SerializeField] Image image_BefIcon, image_BefRatingBg, image_AftIcon, image_AftRatingBg; // 강화석 아이콘, 강화석 등급 배경 
    [SerializeField] Text text_BefRating, text_AftRating; // 강화석 등급 
    [SerializeField] Text text_BefCount, text_AftCount;
    [SerializeField] Text text_BefNameAndCnt, text_AftNameAndCnt; // 선택 강화석 이름 + 갯수, 진화될 강화석 이름 + 갯수 
    [SerializeField] Text text_PriceOne, text_PriceAll; // 1회 진화 비용, 모두 진화 비용 

    [SerializeField] GameObject go_BefRoot, go_AftRoot;
    [SerializeField] Image image_OntStartBtnBg, image_AllStartBtnBg;

    public void SetData(GameDatabase.TableDB.Item _item)
    {
        blockScreen.gameObject.SetActive(false);
        temp_Item = _item;

        // 아이템에 대한 정보가 기본값이거나 이미 최대 등급이 강화석을 선택했을 경우 
        if(string.IsNullOrEmpty(_item.indate) || _item.rating >= 4 || _item.count < 10)
        {
            text_PriceOne.text = "0";
            text_PriceAll.text = "0";
            image_OntStartBtnBg.enabled = false;
            image_AllStartBtnBg.enabled = false;
            go_BefRoot.SetActive(false);
            go_AftRoot.SetActive(false);
        }
        else
        {
            image_OntStartBtnBg.enabled = true;
            image_AllStartBtnBg.enabled = true;
            go_BefRoot.SetActive(true);
            go_AftRoot.SetActive(true);

            int bef_item_ty = _item.type;
            int bef_item_rt = _item.rating;
            int bef_item_cnt = _item.count;
            image_BefIcon.sprite = SpriteAtlasMng.GetInstance().GetItemSprite(bef_item_ty, bef_item_rt);
            image_BefRatingBg.sprite = SpriteAtlasMng.GetInstance().GetSpriteRatingBg(bef_item_rt);
            text_BefRating.color = ResourceDatabase.GetInstance().GetItemColor(bef_item_rt);
            if (bef_item_rt > 0)
            {
                text_BefRating.text = LanguageGameData.GetInstance().GetString(string.Format("item.rating.string.{0}", bef_item_rt));
                text_BefCount.text = bef_item_cnt.ToString();
            }
            else
            {
                text_BefRating.text = "";
                text_BefCount.text = "";
            }

            var goods = GameDatabase.GetInstance().tableDB.GetTableDB_Goods();
            string bef_ston_name = GameDatabase.GetInstance().smithyDB.GetStonName(bef_item_ty, bef_item_rt);
            text_BefNameAndCnt.text = string.Format("{0}\n{1}/10", bef_ston_name, bef_item_cnt);

            int aft_item_ty = _item.type;
            int aft_item_rt = GameDatabase.GetInstance().smithyDB.GetEvolutionStonNextRating(_item.rating);
            if (aft_item_rt > 0)
            {
                image_AftIcon.sprite = SpriteAtlasMng.GetInstance().GetItemSprite(aft_item_ty, aft_item_rt);
                image_AftRatingBg.sprite = SpriteAtlasMng.GetInstance().GetSpriteRatingBg(aft_item_rt);
                text_AftRating.color = ResourceDatabase.GetInstance().GetItemColor(aft_item_rt);
                text_AftRating.text = aft_item_rt > 0 ? LanguageGameData.GetInstance().GetString(string.Format("item.rating.string.{0}", aft_item_rt)) : "";
                text_AftCount.text = aft_item_rt > 0 ? "1" : "";

                string aft_ston_name = GameDatabase.GetInstance().smithyDB.GetStonName(aft_item_ty, aft_item_rt);
                text_AftNameAndCnt.text = string.Format("{0}\n<color=orange>{1}</color>", aft_ston_name, LanguageGameData.GetInstance().GetString("text.1acquisition"));

                // 진화 가격 
                int price_one = GameDatabase.GetInstance().questDB.GetQuestStonEvolutionGold(bef_item_rt);
                if (price_one > 0)
                {
                    text_PriceOne.text = string.Format("{0:#,0}", price_one);
                    int all_count_dvi = (int)Math.Truncate((double)(bef_item_cnt / 10));
                    int price_all = price_one * all_count_dvi;
                    text_PriceAll.text = string.Format("{0:#,0}", price_all);

                    image_OntStartBtnBg.sprite = SpriteAtlasMng.GetInstance().GetSpriteButtonRedOrGray(goods.m_gold >= price_one);
                    image_AllStartBtnBg.sprite = SpriteAtlasMng.GetInstance().GetSpriteButtonRedOrGray(goods.m_gold >= price_all);
                }
                else
                {
                    LogPrint.PrintError("강화 진화 가격 정보 잘못 됨");
                }
            }
            else
            {
                LogPrint.Print(aft_item_rt + " 다음 등급 강화석 없음");
            }
        }
    }

    /// <summary>
    /// 선택 해제 
    /// </summary>
    public void ClickItemRelease()
    {
        SetData(default);
        foreach (var item in GameObject.FindObjectsOfType<ScrollIndexCallbackSmithy>())
        {
            item.ReleaseSton();
        }
    }

    /// <summary>
    /// 강화석 진화 시작 
    /// </summary>
    public async void ClickStartEnhancement(bool _isOne)
    {
        var goods = GameDatabase.GetInstance().tableDB.GetTableDB_Goods();
        int price_one = GameDatabase.GetInstance().questDB.GetQuestStonEvolutionGold(temp_Item.rating);
        int bef_item_count = temp_Item.count;
        if (bef_item_count >= 10)
        {
            int evolu_cnt = 1; // 진화한 갯수 
            if (_isOne == false) // 모두 진화 
                evolu_cnt = (int)Math.Truncate((double)(bef_item_count / 10));

            if (goods.m_gold < price_one * evolu_cnt)
            {
                PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener("골드가 부족합니다.\n골드 구매 탭으로 이동됩니다.", MainUI.GetInstance().Listener_MoveItemShop);
            }
            else if (goods.m_gold >= price_one * evolu_cnt)
            {
                blockScreen.gameObject.SetActive(true);
                // 진화 가격 차감 
                goods.m_gold -= price_one * evolu_cnt;
                Task<bool> task1 = GameDatabase.GetInstance().tableDB.SetUpdateGoods(goods);

                // 진화한 강화석 재료 강화석 차감
                var bef_item_ston = GameDatabase.GetInstance().tableDB.GetItem(temp_Item.type, temp_Item.rating);
                bef_item_ston.count -= evolu_cnt * 10;
                Task<bool> task2 = GameDatabase.GetInstance().tableDB.SendDataItem(bef_item_ston);

                // 진화된 강화석 증가
                var aft_item_ston = GameDatabase.GetInstance().tableDB.GetItem(temp_Item.type, temp_Item.rating + 1);
                aft_item_ston.count += evolu_cnt;
                Task<bool> task3 = GameDatabase.GetInstance().tableDB.SendDataItem(aft_item_ston);

                while (task1.IsCompleted == false || task2.IsCompleted == false || task3.IsCompleted == false) await Task.Delay(100);

                LogPrint.PrintError("강화석 진화 모두 정상 처리됨");

                temp_Item.count -= evolu_cnt * 10;

                blockScreen.CenterAlphaZero();
                ani.Play("StonEvolutionStart");
                while (ani.GetCurrentAnimatorStateInfo(0).IsName("StonEvolutionStart") == false)
                    await Task.Delay(100);

                while (ani.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
                {
                    LogPrint.Print(ani.GetCurrentAnimatorStateInfo(0).normalizedTime);
                    await Task.Delay(100);
                }

                // 셀 새로 고침
                foreach (var item in GameObject.FindObjectsOfType<ScrollIndexCallbackSmithy>())
                    item.ItemCellRefresh(bef_item_ston.aInUid, aft_item_ston.aInUid);

                SetData(temp_Item);
                ani.Play("StonEvolutionReady");
                blockScreen.gameObject.SetActive(false);
            }
        }
        else
        {
            PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener("강화석이 부족합니다.\n강화석 구매 탭으로 이동됩니다.", MainUI.GetInstance().Listener_MoveItemShop);
        }
    }
}
