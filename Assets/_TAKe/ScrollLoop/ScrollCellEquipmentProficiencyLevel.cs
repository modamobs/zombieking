using BackEnd;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class ScrollCellEquipmentProficiencyLevel : MonoBehaviour
{
    GameDatabase.TableDB.Equipment eqDb = new GameDatabase.TableDB.Equipment();
    public GameDatabase.TableDB.Equipment GetCellEquopDB() => eqDb;
    [SerializeField] int id = 0;
    [SerializeField]
    UI uI;
    [System.Serializable]
    class UI
    {
        public Image imIcon, imTypeIcon, imRatingBg, imButtonBg;
        public Text txTypeName, txTypeStatValue, txTypeLevel, txTypeLevelGold, txBtn;
        public Animation ani;
    }

    void ScrollCellIndex(int idx)
    {
        id = idx;
        eqDb = GameDatabase.GetInstance().tableDB.GetNowWearingEquipPartsData(idx);
        uI.imIcon.sprite = SpriteAtlasMng.GetInstance().GetSpriteEquip(eqDb.eq_ty, eqDb.eq_rt, eqDb.eq_id);
        uI.imTypeIcon.sprite = SpriteAtlasMng.GetInstance().GetSpriteEquipMainStatIcon(eqDb.eq_ty);
        uI.imRatingBg.sprite = SpriteAtlasMng.GetInstance().GetSpriteRatingBg(eqDb.eq_rt);
        uI.txTypeName.text = GameDatabase.StringFormat.GetEquipFrontName(eqDb.eq_ty);
        uI.txTypeLevel.text = string.Format("Lv.{0}", eqDb.m_norm_lv);

        var main_stat_value = GameDatabase.GetInstance().chartDB.GetMainStatValue(eqDb);
        Type val_type = main_stat_value[0].GetType();
        if (val_type == typeof(float))
            uI.txTypeStatValue.text = string.Format("{0} {1}", GameDatabase.StringFormat.GetEquipMainStatName(eqDb.eq_ty), string.Format("{0:0.000}(+{1:0.000})", (float)main_stat_value[0], (float)main_stat_value[1]));
        else if (val_type == typeof(long))
            uI.txTypeStatValue.text = string.Format("{0} {1}", GameDatabase.StringFormat.GetEquipMainStatName(eqDb.eq_ty), string.Format("{0:#,0}(+{1:#,0})", (long)main_stat_value[0], (long)main_stat_value[1]));

        // 버튼 
        int maxLv = GameDatabase.GetInstance().chartDB.GetDicBalanceEquipMaxNormalLevel();
        if (eqDb.m_norm_lv >= maxLv) // 최대 레벨 도달 
        {
            uI.imButtonBg.sprite = SpriteAtlasMng.GetInstance().GetSpriteButtonRedOrGray(false);
            uI.txBtn.text = "MAX";
            uI.txTypeLevelGold.gameObject.SetActive(false);
        }
        else
        {
            // 레벨 업 가격 
            long price_gold = GameDatabase.GetInstance().questDB.GetQuestEquipProficiencyUpGold(eqDb.m_norm_lv);
            long gold = GameDatabase.GetInstance().tableDB.GetTableDB_Goods().m_gold;
            uI.txTypeLevelGold.text = string.Format("{0:#,0}", price_gold);
            uI.imButtonBg.sprite = SpriteAtlasMng.GetInstance().GetSpriteButtonRedOrGray(gold >= price_gold);
            uI.txBtn.text = "레벨 업";
            uI.txTypeLevelGold.gameObject.SetActive(true);
        }
    }

    void Update()
    {

    }

    bool isPressOn = false;
    public void PressUp_LevelUp() => isPressOn = false;
    public void PressDw_LevelUp()
    {
        isPressOn = true;
        StopCoroutine("Routin_LevelUp");
        StartCoroutine("Routin_LevelUp");
    }

    IEnumerator Routin_LevelUp()
    {
        yield return null;
        float press_time = 0.5f;
        while (isPressOn)
        {
            int maxLv = GameDatabase.GetInstance().chartDB.GetDicBalanceEquipMaxNormalLevel();
            if (eqDb.m_norm_lv < maxLv)
            {
                long price_gold = GameDatabase.GetInstance().questDB.GetQuestEquipProficiencyUpGold(eqDb.m_norm_lv);
                long gold = GameDatabase.GetInstance().tableDB.GetTableDB_Goods().m_gold;
                if (gold >= price_gold)
                {
                    eqDb.m_norm_lv++;
                    GameDatabase.GetInstance().tableDB.UpdateClientDB_Equip(eqDb);
                    GameDatabase.GetInstance().tableDB.SetUpdateGoods("gold", price_gold, "-");
                    PopUpMng.GetInstance().popUpProficiency.initOnStartEquipmentProficiencyLevel.SetInit(); // 리스트 새로 고침 
                    PopUpMng.GetInstance().popUpProficiency.isUpdateEquipDB[eqDb.eq_ty] = true;
                    uI.ani.Play("QuestLevelUp");

                    MainUI.GetInstance().tapCharacter.wearingEquipmentInfo.SetWearEquipView(eqDb.eq_ty);
                    MainUI.GetInstance().tapCharacter.wearingEquipmentInfo.PlayNormalLevelAni(eqDb.eq_ty);
                }
                else
                {
                    PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener("골드가 부족합니다.\n골드 구매 탭으로 이동됩니다.", PopUpMng.GetInstance().popUpProficiency.Listener_ChangeMoveShopTap);
                    break;
                }
            }
            else
            {
                string type_name = GameDatabase.StringFormat.GetEquipFrontName(eqDb.eq_ty);
                PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox(string.Format("{0} 숙련도의 레벨이 최대 레벨에 도달하였습니다.", type_name));
                break;
            }

            yield return new WaitForSeconds(press_time);
            if (press_time > 0.1f)
            {
                press_time -= 0.1f;
            }
            else if (press_time > 0.05f)
            {
                press_time -= 0.01f;
            }
            else press_time = 0.05f;
        }

        //Param param = new Param();
        //param.Add("m_norm_lv", eqDb.m_norm_lv);
        //Task<bool> tsk1 = GameDatabase.GetInstance().tableDB.SetUpdateChangeEquipmentData(eqDb, param); // db 전송 
        //Task<bool> tsk2 = GameDatabase.GetInstance().tableDB.SetUpdateGoods(GameDatabase.GetInstance().tableDB.GetTableDB_Goods(), false, "gold");
        //while (Loading.Bottom(tsk1.IsCompleted, tsk2.IsCompleted) == false) { yield return null; }

        NotificationIcon.GetInstance().CheckEquipProficiencyLevelUp(true);
        GameDatabase.GetInstance().characterDB.SetPlayerStatValue(); // 스탯 
        MainUI.GetInstance().tapCharacter.SetStatView(GameDatabase.GetInstance().characterDB.GetStat());
        GameMng.GetInstance().myPZ.Setting(); // 스탯 
        //SendProficiencyLevel(isNewUp, nbrLv);
    }
}
