using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 스킬 리스트 셀 
/// </summary>
public class CellSkill : MonoBehaviour
{
    [SerializeField] bool isOthers = false;
    [SerializeField] bool isWearingCell = false;
    [SerializeField] int SLOT_IDX;
    [SerializeField] Text text_Number;
    [SerializeField] SkillIconUI skillIconUI;
    [SerializeField] GameDatabase.TableDB.Skill temp_data;
    [SerializeField] GameObject goNotice;
    cdb_stat_skill _cdb_stat_skill;


    public void SetWearingIdx (int number)
    {
        SLOT_IDX = number;
        if(text_Number!= null)
            text_Number.text = (number + 1).ToString();
    }

    [SerializeField] int a = 0, b = 0;

    public void SetData(cdb_stat_skill _chartSkill = default, bool srollCell = true, int useMainSlot = 0)
    {
        _cdb_stat_skill = _chartSkill;
        temp_data = GameDatabase.GetInstance().tableDB.GetSkill(_chartSkill.s_idx);
        int sk_idx = temp_data.idx;
        int sk_level = temp_data.level;
        int sk_count = temp_data.count;
        int sk_use = 0;

        int m_dg_top_nbr = GameDatabase.GetInstance().tableDB.GetUserInfo().m_dg_top_nbr;
        int lck_opn_dgTopNbr = _chartSkill.s_lck_opn_dgTopNbr;
       
        if (isWearingCell == true) // 착용 슬롯 (6개) 
        {
            skillIconUI.cngpLock.alpha = 0;
        }
        else
        {
            int dg_reset = GameDatabase.GetInstance().tableDB.GetUserInfo().m_dg_top_nbr_ret;
            int useSlotNum = GameDatabase.GetInstance().tableDB.GetIsNowWearing(useMainSlot, temp_data.aInUid); // 현재 사용중인 슬롯 1, 2, 3 중에서 하나 
            if (lck_opn_dgTopNbr <= m_dg_top_nbr || dg_reset > 0) // 던전 클리어 및 던전 리셋 한적이 있는가 
            {
                if(useSlotNum > 0) // 장착중 
                {
                    skillIconUI.text_LockScript.text = string.Format("<color=orange>{0}번</color> 슬롯 장착중", useSlotNum);
                    skillIconUI.cngpLock.alpha = 1;
                    skillIconUI.imLock.color = Color.white;
                }
                // 소지중 
                else skillIconUI.cngpLock.alpha = 0;

            }
            else
            {
                if (lck_opn_dgTopNbr > m_dg_top_nbr && dg_reset == 0) // 미소지 -> 도전의탑 클리어시 소지로 변경됨, 던전 리셋한적 없다 
                {
                    skillIconUI.text_LockScript.text = string.Format("알 수 없는 탑{0}\n클리어시 사용 가능", lck_opn_dgTopNbr);
                    skillIconUI.cngpLock.alpha = 1;
                    skillIconUI.imLock.color = Color.red;
                }
                // 소지중 
                else skillIconUI.cngpLock.alpha = 0;
            }
        }

        if(sk_idx > 0)
        {
            skillIconUI.go_Root.SetActive(true);
            skillIconUI.go_Empty.SetActive(false);
            //Color ratingColor = ResourceDatabase.GetInstance().GetItemColor(_chartSkill.s_rating);
            int upNeedCnt = GameDatabase.GetInstance().chartDB.GetChartSkill_UpNeedCount(sk_level);
            bool isMaxLevel = temp_data.level >= GameDatabase.GetInstance().chartDB.GetChartSkill_MaxLevel();

            skillIconUI.image_Icon.sprite = SpriteAtlasMng.GetInstance().GetSpriteSkill(_chartSkill.s_idx);
            //skillIconUI.text_Rating.color = ratingColor;
            //skillIconUI.text_Rating.text = LanguageGameData.GetInstance().GetString(string.Format("item.rating.string.{0}", _chartSkill.s_rating));
            //skillIconUI.image_RatingOutLine.color = ratingColor;
            for (int i = 0; i < skillIconUI.image_sPoint.Length; i++)
            {
                skillIconUI.image_sPoint[i].gameObject.SetActive(i < _chartSkill.s_pnt);
            }
            
            skillIconUI.text_CountAndUpCount.text = string.Format("{0}/{1}", sk_count, upNeedCnt > 0 ? upNeedCnt.ToString() : "???");
            skillIconUI.image_CountFillAmount.fillAmount = (float)sk_count / (float)upNeedCnt;
            skillIconUI.text_Level.text = string.Format("LEVEL.{0}{1}", sk_level, isMaxLevel == true ? "<color=red>(MAX)</color>" : "");

            if(goNotice != null)
                goNotice.SetActive(sk_count >= upNeedCnt);

            if (sk_use != 0)
            {

            }
        }
        else
        {
            skillIconUI.go_Root.SetActive(false);
            skillIconUI.go_Empty.SetActive(true);
            skillIconUI.image_RatingOutLine.color = ResourceDatabase.GetInstance().GetItemColor(0);
        }
    }

    private int others_id, others_lv;
    public void SetOthers(int id, int lv)
    {
        others_id = id;
        others_lv = lv;
        skillIconUI.image_Icon.sprite = SpriteAtlasMng.GetInstance().GetSpriteSkill(id);
        skillIconUI.text_Level.text = string.Format("LEVEL.{0}", lv);
        cdb_stat_skill cdb = GameDatabase.GetInstance().chartDB.GetChartSkill_Data(id);
        for (int i = 0; i < skillIconUI.image_sPoint.Length; i++)
        {
            skillIconUI.image_sPoint[i].gameObject.SetActive(i < cdb.s_pnt);
        }
    }

    public async void ClickItem()
    {
        if (isOthers)
        {
            GameDatabase.TableDB.Skill sk = new GameDatabase.TableDB.Skill() { idx = others_id, level = others_lv };
            PopUpMng.GetInstance().PopUp_SkinnInfoMenu(sk, true, true, true);
        }
        else
        {
            int nowMainSlotNum = MainUI.GetInstance().tapSkill.nowSelectMainSlot;
            var poSkMenuType = PopUpMng.GetInstance().popUpSkillInfoMenu.skillMenu;
            if (string.IsNullOrEmpty(temp_data.indate) && temp_data.idx > 0)
            {
                int m_dg_top_nbr = GameDatabase.GetInstance().tableDB.GetUserInfo().m_dg_top_nbr;
                int lck_opn_dgTopNbr = _cdb_stat_skill.s_lck_opn_dgTopNbr;
                if (m_dg_top_nbr >= lck_opn_dgTopNbr)
                {
                    await GameDatabase.GetInstance().tableDB.SendDataSkill(temp_data);
                    temp_data = GameDatabase.GetInstance().tableDB.GetSkill(_cdb_stat_skill.s_idx);
                }
            }

            bool isSlotChange = temp_data.idx > 0 && PopUpMng.GetInstance().popUpSkillInfoMenu.gameObject.activeSelf && PopUpMng.GetInstance().popUpSkillInfoMenu.go_puWearingSelect.activeSelf;
            var temp = PopUpMng.GetInstance().popUpSkillInfoMenu.GetNowInfoSkillDate;
            if (temp_data.idx > 0) // 선택한 슬롯에 스킬이 빈 슬롯이 아니다 
            {
                LogPrint.Print("ClickItem 선택한 스킬이 빈 슬롯이 아니다.");
                if (PopUpMng.GetInstance().popUpSkillInfoMenu.gameObject.activeSelf == false)// 스킬 정보보기 팝업 
                {
                    LogPrint.Print("ClickItem 1 - 1 isWearingCell : " + isWearingCell);
                    if (isWearingCell) // 장착중인 스킬 아이콘 정보 리스트 셀에서 선택시 
                    {
                        LogPrint.Print("ClickItem 1 - 2");
                        PopUpMng.GetInstance().PopUp_SkinnInfoMenu(temp_data, false, false);
                    }
                    else
                    {
                        LogPrint.Print("ClickItem 1 - 3");
                        PopUpMng.GetInstance().PopUp_SkinnInfoMenu(temp_data, true, false);
                    }
                }
                else // 착용 리스트 클릭 -> 장착 
                {
                    LogPrint.Print("ClickItem 2 - 1");
                    if (poSkMenuType == SkillMenuWindow.W_INFO_USE_AND_LEVEL_UP) // 장착 팝업 상태 
                    {
                        LogPrint.Print("ClickItem 2 - 2");
                        GameDatabase.GetInstance().tableDB.SetSkillSlotChange(nowMainSlotNum, SLOT_IDX, temp.aInUid, false);
                        MainUI.GetInstance().tapSkill.Init(nowMainSlotNum);
                        PopUpMng.GetInstance().popUpSkillInfoMenu.gameObject.SetActive(false);
                        MainUI.GetInstance().tapGameBattleInfo.SkillChangeWaiting(nowMainSlotNum);
                    }
                    else if (poSkMenuType == SkillMenuWindow.W_INFO_REL_AND_LEVEL_UP) // 해제or슬롯 변경 팝업 상태 
                    {
                        LogPrint.Print("ClickItem 2 - 3");
                        GameDatabase.GetInstance().tableDB.SetSkillSlotChange(nowMainSlotNum, SLOT_IDX, temp.aInUid, true);
                        MainUI.GetInstance().tapSkill.Init(nowMainSlotNum);
                        PopUpMng.GetInstance().popUpSkillInfoMenu.gameObject.SetActive(false);
                        MainUI.GetInstance().tapGameBattleInfo.SkillChangeWaiting(nowMainSlotNum);
                    }
                }
            }
            else
            {
                LogPrint.Print("ClickItem 선택한 스킬이 빈 슬롯이다. isSlotChange : " + isSlotChange);
                if (PopUpMng.GetInstance().popUpSkillInfoMenu.gameObject.activeSelf)
                {
                    LogPrint.Print("ClickItem 3 - 1");
                    if (temp.aInUid != temp_data.aInUid)
                    {
                        if (poSkMenuType == SkillMenuWindow.W_INFO_USE_AND_LEVEL_UP) // 장착안되있는 스킬을 장착하려고 슬롯 선택시 
                        {
                            LogPrint.Print("ClickItem 3 - 2");
                            GameDatabase.GetInstance().tableDB.SetSkillSlotChange(nowMainSlotNum, SLOT_IDX, temp.aInUid, false);
                            MainUI.GetInstance().tapSkill.Init(MainUI.GetInstance().tapSkill.nowSelectMainSlot);
                            PopUpMng.GetInstance().popUpSkillInfoMenu.gameObject.SetActive(false);
                            MainUI.GetInstance().tapGameBattleInfo.SkillChangeWaiting(nowMainSlotNum);
                        }
                        else if (poSkMenuType == SkillMenuWindow.W_INFO_REL_AND_LEVEL_UP) // 이미 장착 되어있는 스킬을 다른 빈 슬롯에 장착하려고 슬롯 선택시 
                        {
                            LogPrint.Print("ClickItem 3 - 3");
                            GameDatabase.GetInstance().tableDB.SetSkillSlotChange(nowMainSlotNum, SLOT_IDX, temp.aInUid, false);
                            MainUI.GetInstance().tapSkill.Init(MainUI.GetInstance().tapSkill.nowSelectMainSlot);
                            PopUpMng.GetInstance().popUpSkillInfoMenu.gameObject.SetActive(false);
                            MainUI.GetInstance().tapGameBattleInfo.SkillChangeWaiting(nowMainSlotNum);
                        }
                    }
                    else
                    {
                        LogPrint.Print("ClickItem 현재 슬롯의 스킬과 착용하려는 스킬이 같다.");
                    }
                }
            }
        }
    }
}
