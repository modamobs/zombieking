using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using BackEnd;
using static BackEnd.BackendAsyncClass;
using System.Threading.Tasks;
using System;
using LitJson;


public enum SkillMenuWindow
{
    W_INFO_USE_AND_LEVEL_UP,    // 스킬 장착 + 레벨 업 창
    W_INFO_REL_AND_LEVEL_UP,    // 스킬 해제 + 레벨 업 창
}

public class PopUpSkillInfoMenu : MonoBehaviour
{
    public SkillMenuWindow skillMenu = SkillMenuWindow.W_INFO_USE_AND_LEVEL_UP;

    [SerializeField] GameDatabase.TableDB.Skill temp_data;
    public GameDatabase.TableDB.Skill GetNowInfoSkillDate => temp_data;

    [SerializeField] SkillIconUI skillIconUI;
    public GameObject go_puInfo, go_puWearingSelect;
    [SerializeField] GameObject go_BtnWearing, go_BtnRelease;
    [SerializeField] Image im_BtnLevelUp;
    [SerializeField] Text text_Name, text_NowDscription, text_NexDscription, text_LevelUpGold; // 레벨업 필요 골드 ;
    [SerializeField] GameObject goNextDscrp;
    [SerializeField] GameObject goSlider;

    int nowMainSlotNum = 0;

    public void Click_TestLevel(int lv)
    {
        temp_data.level = lv;
        SetData(temp_data, true);
    }

    public void SetData (GameDatabase.TableDB.Skill sk_data, bool isPopUse, bool isMainPreview = false, bool isOthers = false)
    {
        temp_data = sk_data;
        var cdb = GameDatabase.GetInstance().chartDB.GetChartSkill_Data(temp_data.idx);
        int max_sk_level = GameDatabase.GetInstance().chartDB.GetChartSkill_MaxLevel();
        int sk_idx = temp_data.idx;
        int sk_level = temp_data.level < max_sk_level ? temp_data.level : max_sk_level;
        int sk_count = temp_data.count;

        go_puInfo.SetActive(true);
        go_puWearingSelect.SetActive(false);

        LogPrint.EditorPrint("v : " + isOthers +", id : " + temp_data.idx +", lv : " + temp_data.level);
        if (isOthers)
        {
            go_BtnWearing.SetActive(false);
            go_BtnRelease.SetActive(false);
            goSlider.SetActive(false);
            goNextDscrp.SetActive(false);
            im_BtnLevelUp.gameObject.SetActive(false);

            Color ratingColor = ResourceDatabase.GetInstance().GetItemColor(temp_data.cliend_rating);
            skillIconUI.image_Icon.sprite = SpriteAtlasMng.GetInstance().GetSpriteSkill(temp_data.idx);
            skillIconUI.text_Rating.color = ratingColor;
            skillIconUI.text_Rating.text = LanguageGameData.GetInstance().GetString(string.Format("item.rating.string.{0}", temp_data.cliend_rating));
            skillIconUI.image_RatingOutLine.color = ratingColor;
            for (int i = 0; i < skillIconUI.image_sPoint.Length; i++)
            {
                skillIconUI.image_sPoint[i].gameObject.SetActive(i < cdb.s_pnt);
            }

            int upNeedCnt = GameDatabase.GetInstance().chartDB.GetChartSkill_UpNeedCount(sk_level);

            skillIconUI.image_CountFillAmount.fillAmount = (float)sk_count / (float)upNeedCnt;
            skillIconUI.text_Level.text = string.Format("<color=#FFBA00>LEVEL.{0}</color>", sk_level);
            text_Name.text = LanguageGameData.GetInstance().GetString(string.Format("skill.name_{0}", temp_data.idx));
            text_NowDscription.text = GameDatabase.GetInstance().chartDB.GetInfoSkillDescription(sk_idx, sk_level); // 현재 능력치 정보 
        }
        else
        {
            nowMainSlotNum = MainUI.GetInstance().tapSkill.nowSelectMainSlot;
            int m_dg_top_nbr = GameDatabase.GetInstance().tableDB.GetUserInfo().m_dg_top_nbr;
            int lck_opn_dgTopNbr = GameDatabase.GetInstance().chartDB.GetChartSkill_Data(temp_data.idx).s_lck_opn_dgTopNbr;
            if (isMainPreview == false)
            {
                goSlider.SetActive(true);
                if (m_dg_top_nbr >= lck_opn_dgTopNbr)
                {
                    int useSlotNum = GameDatabase.GetInstance().tableDB.GetIsNowWearing(nowMainSlotNum, temp_data.aInUid);
                    if (isPopUse && useSlotNum == -1) // 장착 팝업상태에서 Call
                    {
                        skillMenu = SkillMenuWindow.W_INFO_USE_AND_LEVEL_UP;
                        go_BtnWearing.SetActive(true);
                        go_BtnRelease.SetActive(false);
                    }
                    else // 해제 팝업상태에서 Call
                    {
                        skillMenu = SkillMenuWindow.W_INFO_REL_AND_LEVEL_UP;
                        go_BtnWearing.SetActive(false);
                        go_BtnRelease.SetActive(true);
                    }
                }
                else
                {
                    go_BtnWearing.SetActive(false);
                    go_BtnRelease.SetActive(false);
                }
            }
            else
            {
                go_BtnWearing.SetActive(false);
                go_BtnRelease.SetActive(false);
                goSlider.SetActive(false);
            }

           

            Color ratingColor = ResourceDatabase.GetInstance().GetItemColor(temp_data.cliend_rating);
            skillIconUI.image_Icon.sprite = SpriteAtlasMng.GetInstance().GetSpriteSkill(temp_data.idx);
            skillIconUI.text_Rating.color = ratingColor;
            skillIconUI.text_Rating.text = LanguageGameData.GetInstance().GetString(string.Format("item.rating.string.{0}", temp_data.cliend_rating));
            skillIconUI.image_RatingOutLine.color = ratingColor;
            for (int i = 0; i < skillIconUI.image_sPoint.Length; i++)
            {
                skillIconUI.image_sPoint[i].gameObject.SetActive(i < cdb.s_pnt);
            }

            int upNeedCnt = GameDatabase.GetInstance().chartDB.GetChartSkill_UpNeedCount(sk_level);

            skillIconUI.image_CountFillAmount.fillAmount = (float)sk_count / (float)upNeedCnt;
            text_Name.text = LanguageGameData.GetInstance().GetString(string.Format("skill.name_{0}", temp_data.idx));
            text_NowDscription.text = GameDatabase.GetInstance().chartDB.GetInfoSkillDescription(sk_idx, sk_level); // 현재 능력치 정보 

            if (isMainPreview == false)
            {
                if (sk_level < max_sk_level)
                {
                    var goods = GameDatabase.GetInstance().tableDB.GetTableDB_Goods();
                    long up_gold = GameDatabase.GetInstance().chartDB.GetChartSkillLeveUpGoldPrice(sk_level);
                    skillIconUI.text_Level.text = string.Format("<color=#FFBA00>LEVEL.{0}</color> ▶ <color=#00DCFF>{1}</color>", sk_level, sk_level + 1);
                    text_NexDscription.text = GameDatabase.GetInstance().chartDB.GetInfoSkillDescription(sk_idx, sk_level + 1); // 다음 능력치 정보 
                    goNextDscrp.SetActive(true);
                    im_BtnLevelUp.gameObject.SetActive(true);
                    im_BtnLevelUp.sprite = sk_count >= upNeedCnt && goods.m_gold >= up_gold ? SpriteAtlasMng.GetInstance().GetSpriteRatingBg(3) : SpriteAtlasMng.GetInstance().GetSpriteRatingBg(0);

                    if (goods.m_gold >= up_gold)
                        text_LevelUpGold.text = string.Format("{0:#,0}", GameDatabase.GetInstance().chartDB.GetChartSkillLeveUpGoldPrice(sk_level));
                    else
                        text_LevelUpGold.text = string.Format("<color=red>{0:#,0}</color>", GameDatabase.GetInstance().chartDB.GetChartSkillLeveUpGoldPrice(sk_level));

                    if (sk_count >= upNeedCnt)
                        skillIconUI.text_CountAndUpCount.text = string.Format("{0}/{1}", sk_count, upNeedCnt > 0 ? upNeedCnt.ToString() : "???");
                    else
                        skillIconUI.text_CountAndUpCount.text = string.Format("<color=red>{0}</color>/{1}", sk_count, upNeedCnt > 0 ? upNeedCnt.ToString() : "???");
                }
                else
                {
                    skillIconUI.text_Level.text = string.Format("<color=#FFBA00>LEVEL.{0} (MAX)</color>", sk_level);
                    goNextDscrp.SetActive(false);
                    im_BtnLevelUp.gameObject.SetActive(false);
                }
            }
            else
            {
                skillIconUI.text_Level.text = string.Format("<color=#FFBA00>LEVEL.{0}</color>", sk_level);
                goNextDscrp.SetActive(false);
                im_BtnLevelUp.gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// 스킬 정보 팝업에서 : 장착 
    /// </summary>
    public void ClickPopUse()
    {
        go_puInfo.SetActive(false);
        go_puWearingSelect.SetActive(true);
    }

    /// <summary>
    /// 슬롯 선택 팝업에서 : 취소 
    /// </summary>
    public void ClickCancelUse()
    {
        skillMenu = SkillMenuWindow.W_INFO_USE_AND_LEVEL_UP;
        go_puInfo.SetActive(true);
        go_puWearingSelect.SetActive(false);
    }

    /// <summary>
    /// 스킬 정보 팝업에서 : 해제 
    /// </summary>
    public void ClickRelease ()
    {
        GameDatabase.GetInstance().tableDB.SetSkilRelease(nowMainSlotNum, temp_data.aInUid);
        MainUI.GetInstance().tapSkill.Init(nowMainSlotNum);
        MainUI.GetInstance().tapGameBattleInfo.SkillChangeWaiting(nowMainSlotNum);
        PopUpMng.GetInstance().popUpSkillInfoMenu.gameObject.SetActive(false);
    }


    /// <summary>
    /// 사용중인 스킬일때 슬롯 변경 
    /// </summary>
    public void ClickWearSlotChange()
    {
        go_puInfo.SetActive(false);
        go_puWearingSelect.SetActive(true);
    }

    /// <summary>
    /// 스킬 레벨 업 팝업에서 바로 진행 
    /// </summary>
    public async void ClickPopLevelUp()
    {
        var data = temp_data;
        bool isMaxLevel = data.level >= GameDatabase.GetInstance().chartDB.GetChartSkill_MaxLevel();
        if (isMaxLevel)
        {
            PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("최대 레벨에 도달하였습니다.");
            return;
        }

        int sk_level = temp_data.level;
        int sk_count = temp_data.count;
        int upNeedCnt = GameDatabase.GetInstance().chartDB.GetChartSkill_UpNeedCount(sk_level);
        if (sk_count < upNeedCnt)
        {
            PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener("레벨업에 필요한 스킬의 수량이 부족합니다.\n스킬은 도전의 탑에서 획득하실 수 있습니다.\n확인 버튼을 누르시면 도전의 탑 탭으로 이동됩니다.", MainUI.GetInstance().Listener_MoveDungeonTop);
            return;
        }

        var goods = GameDatabase.GetInstance().tableDB.GetTableDB_Goods();
        long up_gold = GameDatabase.GetInstance().chartDB.GetChartSkillLeveUpGoldPrice(data.level);
        if (goods.m_gold < up_gold)
        {
            PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener("골드가 부족합니다.\n골드 구매 탭으로 이동됩니다.", MainUI.GetInstance().Listener_MoveItemShop);
            return;
        }

        LogPrint.Print(GameDatabase.GetInstance().chartDB.GetChartSkill_UpNeedCount(data.level));
        // 가격 차감 
        goods.m_gold -= up_gold;
        // 스킬 레벨 증가 
        data.count -= GameDatabase.GetInstance().chartDB.GetChartSkill_UpNeedCount(data.level);
        data.level++;
        
        Task<bool> tsk1 = GameDatabase.GetInstance().tableDB.SetUpdateGoods(goods);
        Task<bool> tsk2 = GameDatabase.GetInstance().tableDB.SendDataSkill(data);
        LogPrint.Print("--1--Time.time : " + Time.time + ", tsk1 : " + tsk1 + ", tsk2 : " + tsk2);

        while (Loading.Bottom(tsk1.IsCompleted, tsk2.IsCompleted) == false) await Task.Delay(100);

        LogPrint.Print("--2--Time.time : " + Time.time + ", tsk1 : " + tsk1.IsCompleted + ", tsk2 : " + tsk2.IsCompleted);

        SetData(data, true);
        MainUI.GetInstance().tapGameBattleInfo.SkillChangeWaiting(nowMainSlotNum);
        MainUI.GetInstance().tapSkill.Init(nowMainSlotNum);
        NotificationIcon.GetInstance().CheckNoticeSkillLevelUp(true);
    }

    /// <summary>
    /// 스킬 레벨업 팝업 + 레벨 업 팝업 : 레벨 업 
    /// </summary>
    public void ClickLevelUpConfirm()
    {

    }
}
