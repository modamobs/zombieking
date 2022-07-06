using BackEnd;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class PopUpOfflineReward : MonoBehaviour
{
    private float rwd_hour = -1f;
    private bool isCompleteReward = false;
    [SerializeField] Text txOfflineHour;
    [SerializeField] Text txConfirmTime;

    [SerializeField] UI[] ui;
    [System.Serializable] class UI
    {
        public GameObject goObj;
        public Image imRewardIcon;
        public Text txRewardName;
        public Text txRewardCnt;
    }

    public void SetData(System.DateTime nDate, System.DateTime endDate)
    {
        LogPrint.Print("-----PopUpOfflineReward----- endDate : " + endDate + ", nDate : " + nDate + ", Hours : " + (nDate - endDate).Hours);
        isCompleteReward = false;
        int totalSec = (int)(nDate - endDate).TotalSeconds > 36000 ? 36000 : (int)(nDate - endDate).TotalSeconds;
        int hours = -1, minute, second;

        totalSec = totalSec % (24 * 3600);
        hours = totalSec / 3600;

        totalSec %= 3600;
        minute = totalSec / 60;

        totalSec %= 60;
        second = totalSec;

        txOfflineHour.text = string.Format("오프라인 시간 {0:00}:{1:00}:{2:00}", hours, minute, second);
        LogPrint.Print("hours : " + hours +", totalSec:" + totalSec);
        if(hours >= 0)
        {
            rwd_hour = hours;
            var cdb = GameDatabase.GetInstance().chartDB.cdb_offline_reward.Find((cdb_offline_reward cor) => cor.hour == hours);
            LogPrint.Print("-----PopUpOfflineReward----- : " + JsonUtility.ToJson(cdb));

            ui[0].goObj.SetActive(cdb.qst_gold > 0);
            ui[1].goObj.SetActive(cdb.ruby > 0);
            ui[2].goObj.SetActive(cdb.ether > 0);
            ui[3].goObj.SetActive(cdb.piece_equip_rt5 > 0);
            ui[4].goObj.SetActive(cdb.piece_equip_rt6 > 0);
            ui[5].goObj.SetActive(cdb.piece_equip_rt7 > 0);
            ui[6].goObj.SetActive(cdb.piece_acce_rt5 > 0);

            ui[0].imRewardIcon.sprite = SpriteAtlasMng.GetInstance().GetOfflineRewardIcon("qst_gold");
            ui[1].imRewardIcon.sprite = SpriteAtlasMng.GetInstance().GetOfflineRewardIcon("ruby");
            ui[2].imRewardIcon.sprite = SpriteAtlasMng.GetInstance().GetOfflineRewardIcon("ether");
            ui[3].imRewardIcon.sprite = SpriteAtlasMng.GetInstance().GetOfflineRewardIcon("piece_equip_rt5");
            ui[4].imRewardIcon.sprite = SpriteAtlasMng.GetInstance().GetOfflineRewardIcon("piece_equip_rt6");
            ui[5].imRewardIcon.sprite = SpriteAtlasMng.GetInstance().GetOfflineRewardIcon("piece_equip_rt7");
            ui[6].imRewardIcon.sprite = SpriteAtlasMng.GetInstance().GetOfflineRewardIcon("piece_acce_rt5");

            ui[0].txRewardName.text = LanguageGameData.GetInstance().GetString("offline.reward.name.qst_gold");
            ui[1].txRewardName.text = LanguageGameData.GetInstance().GetString("offline.reward.name.ruby");
            ui[2].txRewardName.text = LanguageGameData.GetInstance().GetString("offline.reward.name.ether");
            ui[3].txRewardName.text = LanguageGameData.GetInstance().GetString("offline.reward.name.piece_equip_rt5");
            ui[4].txRewardName.text = LanguageGameData.GetInstance().GetString("offline.reward.name.piece_equip_rt6");
            ui[5].txRewardName.text = LanguageGameData.GetInstance().GetString("offline.reward.name.piece_equip_rt7");
            ui[6].txRewardName.text = LanguageGameData.GetInstance().GetString("offline.reward.name.piece_acce_rt5");

            ui[0].txRewardCnt.text = string.Format("{0:#,0}", GameDatabase.GetInstance().questDB.GetQuestMaxSecondRewardGold(cdb.qst_gold));
            ui[1].txRewardCnt.text = string.Format("x{0:#,0}", cdb.ruby);
            ui[2].txRewardCnt.text = string.Format("x{0:#,0}", cdb.ether);
            ui[3].txRewardCnt.text = string.Format("x{0:#,0}", cdb.piece_equip_rt5);
            ui[4].txRewardCnt.text = string.Format("x{0:#,0}", cdb.piece_equip_rt6);
            ui[5].txRewardCnt.text = string.Format("x{0:#,0}", cdb.piece_equip_rt7);
            ui[6].txRewardCnt.text = string.Format("x{0:#,0}", cdb.piece_acce_rt5);
        }

        StartCoroutine("AutoClose");
    }

    IEnumerator AutoClose()
    {
        WaitForSeconds sec = new WaitForSeconds(1);
        yield return null;
        int closeCount = 60;
        while (closeCount >= 0 && !isCompleteReward)
        {
            txConfirmTime.text = string.Format("{0}초 뒤에 자동으로 보상이 받아집니다.", closeCount);
            yield return sec;
            closeCount--;
        }

        if (!isCompleteReward)
            Reward(false);
    }

    public void Click_Reward()
    {
        Reward(false);
    }

    /// <summary>
    /// 광고 플레이 묻는 팝업 
    /// </summary>
    public void Click_VideoReward() => PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener("확인 버튼을 누르시면 짧은 광고 시청 후 보상을 2배로 획득합니다.", PlayVideo);

    /// <summary>
    /// 광고 플레이 하고 정상 완료후 리턴 이벤트로 보상 지급받을 함수를 태워 보냄 
    /// </summary>
    public void PlayVideo()
    {
        if (GameDatabase.GetInstance().tableDB.GetUserInfo().GetAdRemoval() == true)
        {
            PlayVideoResulReward("success");
        }
        else
        {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
            PlayVideoResulReward("success");
            return;
#endif

            VideoAdsMng.GetInstance().AdShow(PlayVideoResulReward);
        }
    }

    /// <summary>
    /// 광고 완료 보상
    /// 다이아 10 
    /// </summary>
    private void PlayVideoResulReward(string result) => Reward(true);

    /// <summary>
    /// 보상 지급 
    /// </summary>
    private async void Reward(bool isDoubleRwd)
    {
        if (rwd_hour >= 0)
        {
            int calc = isDoubleRwd == true ? 2 : 1;
            var cdb = GameDatabase.GetInstance().chartDB.cdb_offline_reward.Find((cdb_offline_reward cor) => cor.hour == rwd_hour);
            isCompleteReward = true;

            GameDatabase.GetInstance().tableDB.SetUpdateGoods("gold", GameDatabase.GetInstance().questDB.GetQuestMaxSecondRewardGold(cdb.qst_gold * calc), "+"); // 골드 
            GameDatabase.GetInstance().tableDB.SetUpdateGoods("ruby", cdb.ruby * calc, "+"); // 루비 
            GameDatabase.GetInstance().tableDB.SetUpdateGoods("ether", cdb.ether * calc, "+", true); // 에테르 

            // 장비 조각 
            List<GameDatabase.TableDB.Equipment> temp_eq_list = new List<GameDatabase.TableDB.Equipment>();
            TransactionParam tParam = new TransactionParam();
            for (int rt = 5; rt <= 7; rt++)
            {
                Param p = new Param();
                var item_db = GameDatabase.GetInstance().tableDB.GetItem(28, rt);
                if (rt == 5)
                    item_db.count += cdb.piece_equip_rt5 * calc;
                else if (rt == 6)
                    item_db.count += cdb.piece_equip_rt6 * calc;
                else if (rt == 7)
                    item_db.count += cdb.piece_equip_rt7 * calc;

                p.Add("count", item_db.count);
                List<BackEnd.WRITE> writes = new List<BackEnd.WRITE> { new WRITE { Action = TransactionAction.Update, Param = p } };
                tParam.AddUpdateList(BackendGpgsMng.tableName_Item, item_db.indate, writes);
                GameDatabase.GetInstance().tableDB.UpdateClientDB_Item(item_db);
            }

            // 장신구 조각 
            Param p2 = new Param();
            var item_db2 = GameDatabase.GetInstance().tableDB.GetItem(29, 5);
            item_db2.count += cdb.piece_acce_rt5 * calc;
            p2.Add("count", item_db2.count);
            tParam.AddUpdateList(BackendGpgsMng.tableName_Item, item_db2.indate, new List<WRITE>() { new WRITE { Action = TransactionAction.Update, Param = p2 } });
            GameDatabase.GetInstance().tableDB.UpdateClientDB_Item(item_db2);

            // 장비 조각, 장신구 조각 서버전송 
            BackendReturnObject bro1 = null;
            SendQueue.Enqueue(Backend.GameInfo.TransactionWrite, tParam, callback => { bro1 = callback; });

            // 이전 종료 시간을 현재 접속시간으로 변경 
            BackendReturnObject userinfo_bro = null;
            var userinfo_db = GameDatabase.GetInstance().tableDB.GetUserInfo();
            userinfo_db.m_game_end_date = await BackendGpgsMng.GetInstance().GetBackendTime();
            Param userinfo_prm = new Param();
            userinfo_prm.Add("m_game_end_date", userinfo_db.m_game_end_date);
            SendQueue.Enqueue(Backend.GameInfo.Update, BackendGpgsMng.tableName_UserInfo, userinfo_db.indate, userinfo_prm, callback => { userinfo_bro = callback; });
           
            while (Loading.Full(bro1, userinfo_bro) == false) { await Task.Delay(100); }
            GameDatabase.GetInstance().tableDB.SetUpdate_UserInfo(userinfo_db, true);

            gameObject.SetActive(false);
        }
    }
}
