using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class ScrollCellQuest : MonoBehaviour
{
    [SerializeField] int qNbr;
    [SerializeField] cdb_quest CDBquest = new cdb_quest();
    [SerializeField] cdb_quest CDBFrontQuest = new cdb_quest();
    private DateTime endDate;
    private int lv;
    [SerializeField] UIInfo uiInfo;
    [System.Serializable]
    class UIInfo
    {
        public Text txNbr;
        public Text txLevel;
        public Text txTitle;
        public Image imIcon;
        public Slider sdBar;
        public Text txTime;
        public Text txRwdGold;

        public Animation aniReward;
        public Text txReward;

        public Image imBtnBgLvUp;
        public Text txBtnNextRwdGold;
        public Text txBtnUpPrice;
        public Color coOk, coNo, coFrtLvNo100, coMax;

        public Animation ani;
    }

    void Awake()
    {
        string strTmp = Regex.Replace(gameObject.name, @"\D", "");
        qNbr = int.Parse(strTmp);
        CDBquest = GameDatabase.GetInstance().questDB.GetCdbQuest(qNbr);

        if (qNbr > 0)
            CDBFrontQuest = GameDatabase.GetInstance().questDB.GetCdbQuest(qNbr - 1);
        else CDBFrontQuest = new cdb_quest();
    }

    public void ScrollCellIndex(bool isInit)
    {
        //PlayerPrefs.SetString(PrefsKeys.key_quest_completed_date(qNbr), BackendGpgsMng.GetInstance().GetNowTime().ToString());

        int qLv = GameDatabase.GetInstance().questDB.GetLevel(qNbr);
        uiInfo.txNbr.text = (CDBquest.nbr + 1).ToString();
        uiInfo.txLevel.text = string.Format("Lv.{0}", qLv);
        uiInfo.txTitle.text = CDBquest.title.ToString();
        uiInfo.imIcon.sprite = SpriteAtlasMng.GetInstance().GetQuestIcon(qNbr);

        // 레벨업 가격 
        if (qLv < CDBquest.max_lv)
            uiInfo.txBtnUpPrice.text = string.Format("{0:#,0}", GameDatabase.GetInstance().questDB.LevelUpGold(CDBFrontQuest, CDBquest, qLv));
        else uiInfo.txBtnUpPrice.text = "MAX";

        CheckUpBtn();

        if (qLv > 0)
        {
            uiInfo.txRwdGold.text = string.Format("{0:#,0}", GameDatabase.GetInstance().questDB.RewardGold(CDBquest, qLv));
            if(isInit == true)
            {
                bool first = false;
                DateTime nDate = BackendGpgsMng.GetInstance().GetNowTime();
                DateTime tryCompleteDate;
                if (DateTime.TryParse(PlayerPrefs.GetString(PrefsKeys.key_quest_completed_date(qNbr)), out tryCompleteDate) == false)
                {
                    first = true;
                    tryCompleteDate = nDate.AddSeconds(CDBquest.rwd_time);
                }

                int totalSec = (int)(tryCompleteDate - nDate).TotalSeconds;
                if (totalSec < 0)
                {
                    if (!first)
                        CompleteTimeRewardGold();

                    tryCompleteDate = nDate.AddSeconds(CDBquest.rwd_time);
                    PlayerPrefs.SetString(PrefsKeys.key_quest_completed_date(qNbr), tryCompleteDate.ToString());
                }

                endDate = tryCompleteDate;
            }

            StopCoroutine("RunRutine");
            StartCoroutine("RunRutine");
        }
        else
        {
            if(qLv <= 0)
            {
                uiInfo.sdBar.value = 0f;
                uiInfo.txTime.text = "";
                uiInfo.txRwdGold.text = "0";
            }
        }
    }

    IEnumerator RunRutine()
    {
        WaitForSeconds wait1Second = new WaitForSeconds(1f);
        int hours, minute, second;
        while (true)
        {
            DateTime nDate = BackendGpgsMng.GetInstance().GetNowTime();
            float totalSec = (float)(endDate - nDate).TotalSeconds;
            if(totalSec >= 0)
            {
                float sec = totalSec + 0.1f;
                hours = (int)(sec / 3600);
                minute = (int)((sec % 3600) / 60);
                second = (int)((sec % 3600) % 60);

                uiInfo.txTime.text = string.Format("{0:00}:{1:00}:{2:00}", hours, minute, second);
                uiInfo.sdBar.value = Mathf.Lerp(uiInfo.sdBar.value,1 - totalSec / CDBquest.rwd_time, Time.time);
            }
            else
            {
                CompleteTimeRewardGold(); // reward gold 
                CheckUpBtn();

                uiInfo.sdBar.value = 0.0f;
                endDate = nDate.AddSeconds(CDBquest.rwd_time);
                if ((endDate - nDate).TotalSeconds > 60)
                {
                    PlayerPrefs.SetString(PrefsKeys.key_quest_completed_date(qNbr), endDate.ToString());
                }
            }

            if (CDBquest.rwd_time < 60)
                yield return null;
            else yield return wait1Second;
        }
    }

    private void CompleteTimeRewardGold()
    {
        int nbrLv = GameDatabase.GetInstance().questDB.GetLevel(qNbr);
        float pet_sop1_value = GameMng.GetInstance().myPZ != null ? GameMng.GetInstance().myPZ.igp.statValue.petSpOpTotalFigures.sop1_value * 0.01f : 0;
        long rwdGold = GameDatabase.GetInstance().questDB.RewardGold(CDBquest, nbrLv);
        long result_rwdGold = rwdGold + (int)(rwdGold * pet_sop1_value);

        GameDatabase.GetInstance().tableDB.SetUpdateGoods("gold", result_rwdGold, "+");

        if (qNbr == 0)
        {
            NotificationIcon.GetInstance().CheckEquipProficiencyLevelUp();
            NotificationIcon.GetInstance().CheckNoticeQuestLevelUp();
        }

        uiInfo.txReward.text = string.Format("+{0:#,0}", result_rwdGold);
        uiInfo.aniReward.Play("TapQuestRewardGold");
        MainUI.GetInstance().tapQuest.RefreshCheck();
    }

    
    bool isPressOn = false;
    public void UpPress_LevelUp() => isPressOn = false;

    public void DwPress_LevelUp()
    {
        isPressOn = true;
        int fntNbr = qNbr - 1;
        int frtQstLevel = GameDatabase.GetInstance().chartDB.GetDicBalance("quest.front.level").val_int;
        bool isFrontQuestLv100 = fntNbr < 0 ? true : GameDatabase.GetInstance().questDB.GetLevel(fntNbr) >= frtQstLevel;

        if (isFrontQuestLv100 == false)
        {
            var cdb_frtQst = GameDatabase.GetInstance().questDB.GetCdbQuest(fntNbr);
            PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox(string.Format("{0}번 퀘스트 [{1}] <color=yellow>{2}Lv</color>을 달성해야 레벨을 올릴 수 있습니다.", cdb_frtQst.nbr + 1, cdb_frtQst.title, frtQstLevel));
        }
        else
        {
            StopCoroutine("Routin_LevelUp");
            StartCoroutine("Routin_LevelUp");
        }
    }

    IEnumerator Routin_LevelUp()
    {
        yield return null;
        float press_time = 0.5f;
        int nbrLv = GameDatabase.GetInstance().questDB.GetLevel(qNbr);
        bool isNewUp = nbrLv <= 0;
        int upCnt = 0;

        while (isPressOn)
        {
            bool checkUpMax = nbrLv >= CDBquest.max_lv;
            if (!checkUpMax)
            {
                var goods_db = GameDatabase.GetInstance().tableDB.GetTableDB_Goods();
                long price = GameDatabase.GetInstance().questDB.LevelUpGold(CDBFrontQuest, CDBquest, nbrLv);
                if (goods_db.m_gold >= price) // 가능 
                {
                    LogPrint.Print("-----upCnt------ : " + upCnt);
                    upCnt++;
                    nbrLv++;
                    if (nbrLv > CDBquest.max_lv)
                        nbrLv = CDBquest.max_lv;

                    uiInfo.ani.Play("QuestLevelUp");
                    GameDatabase.GetInstance().questDB.SetLevel(qNbr, nbrLv);
                    uiInfo.txRwdGold.text = string.Format("{0:#,0}", GameDatabase.GetInstance().questDB.RewardGold(CDBquest, nbrLv));
                    uiInfo.txLevel.text = string.Format("Lv.{0}", nbrLv);
                    if (nbrLv < CDBquest.max_lv)
                        uiInfo.txBtnUpPrice.text = string.Format("{0:#,0}", price);
                    else uiInfo.txBtnUpPrice.text = "MAX";

                    GameDatabase.GetInstance().tableDB.SetUpdateGoods("gold", price, "-");
                    yield return new WaitForSeconds(press_time);
                    if (press_time > 0.05f)
                        press_time -= 0.2f;
                    else if (press_time > 0.025f)
                        press_time -= 0.05f;
                    else press_time = 0.025f;
                }
                else
                {
                    bool isGoldBuyMove = GameMng.GetInstance().mode_type == IG.ModeType.CHAPTER_LOOP || GameMng.GetInstance().mode_type == IG.ModeType.CHAPTER_CONTINUE;
                    if (isGoldBuyMove)
                        PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener("골드가 부족합니다.\n골드 구매 탭으로 이동됩니다.", MainUI.GetInstance().Listener_MoveItemShop);
                    else PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("골드가 부족합니다.");
                    break;
                }
            }
            else
            {
                PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("최대 레벨에 도달하였습니다.");
                break;
            }
        }

        if (upCnt > 0)
        {
            //SendQuestLevel(isNewUp, nbrLv);
            GameDatabase.GetInstance().questDB.SetLevel(qNbr, nbrLv);
            ScrollCellIndex(isNewUp);
            var achi_db = GameDatabase.GetInstance().achievementsDB.Get(GameDatabase.AchievementsDB.Nbr.nbr18);
            if (achi_db.progLv == qNbr)
            {
                var achi_cdb = GameDatabase.GetInstance().chartDB.dic_cdb_achievements[(int)GameDatabase.AchievementsDB.Nbr.nbr18].Find(obj => obj.lv == achi_db.progLv);
                long ach_cnt = nbrLv >= achi_cdb.prog_cmp_cnt ? achi_cdb.prog_cmp_cnt : nbrLv;
                GameDatabase.GetInstance().achievementsDB.ASetInCount(GameDatabase.AchievementsDB.Nbr.nbr18, ach_cnt, false, false); // 업적, nbr18 1~20번 퀘스트 100레벨 달성하기!
            }
            NotificationIcon.GetInstance().CheckNoticeQuestLevelUp(true);
        }
    }

    /// <summary>
    /// 데이터 전송 
    /// </summary>
    async void SendQuestLevel (bool firNewUp, int send_nbrLv)
    {
        Task tsk1 = GameDatabase.GetInstance().questDB.Send(qNbr, send_nbrLv);
        while (Loading.Bottom(tsk1.IsCompleted) == false) await Task.Delay(100);

        ScrollCellIndex(firNewUp);

        var achi_db = GameDatabase.GetInstance().achievementsDB.Get(GameDatabase.AchievementsDB.Nbr.nbr18);
        if (achi_db.progLv == qNbr)
        {
            var achi_cdb = GameDatabase.GetInstance().chartDB.dic_cdb_achievements[(int)GameDatabase.AchievementsDB.Nbr.nbr18].Find(obj => obj.lv == achi_db.progLv);
            long ach_cnt = send_nbrLv >= achi_cdb.prog_cmp_cnt ? achi_cdb.prog_cmp_cnt : send_nbrLv;
            GameDatabase.GetInstance().achievementsDB.ASetInCount(GameDatabase.AchievementsDB.Nbr.nbr18, ach_cnt, false, false); // 업적, nbr18 1~20번 퀘스트 100레벨 달성하기!
        }
    }

    public void CheckUpBtn()
    {
        int nbrLv = GameDatabase.GetInstance().questDB.GetLevel(qNbr);
        if (nbrLv >= CDBquest.max_lv)
        {
            uiInfo.imBtnBgLvUp.color = uiInfo.coMax;
        }
        else
        {
            long gold = GameDatabase.GetInstance().tableDB.GetTableDB_Goods().m_gold;
            long price = GameDatabase.GetInstance().questDB.LevelUpGold(CDBFrontQuest, CDBquest, nbrLv);
            int fntNbr = qNbr - 1;
            int frtQstLevel = GameDatabase.GetInstance().chartDB.GetDicBalance("quest.front.level").val_int;
            bool isFrontQuestLv100 = fntNbr < 0 ? true : GameDatabase.GetInstance().questDB.GetLevel(fntNbr) >= frtQstLevel;
            if (isFrontQuestLv100 == true)
            {
                if (gold >= price)
                    uiInfo.imBtnBgLvUp.color = uiInfo.coOk;
                else uiInfo.imBtnBgLvUp.color = uiInfo.coNo;
            }
            else uiInfo.imBtnBgLvUp.color = uiInfo.coFrtLvNo100;
        }
    }
}