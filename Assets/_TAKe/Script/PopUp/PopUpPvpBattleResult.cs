using BackEnd;
using Coffee.UIExtensions;
using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class PopUpPvpBattleResult : MonoBehaviour
{
    public List<PvpReward> pvpRewards = new List<PvpReward>();
    public struct PvpReward
    {
        public string gch_type; // it:item, win_score or lose_score : 점수, battle_coin : 배틀 코인 
        public int it_type;
        public int it_rating;
        public int it_cnt;
        public string it_name;
    }

    [SerializeField] InitOnStartPvpReward initOnStartPvpReward;
    [SerializeField] UI ui;
    [System.Serializable]
    class UI
    {
        public Text txResult;
        public Text txWinCount;
        public Text txScore;
        public Text txCoin;
        public Text txBefScore;
        public Text txAftScore;

        public Text txEndPop;
        public Color coWin, coLose;
    }

    [SerializeField] GameObject goRoot;
    [SerializeField] GameObject goRewardRoot;
    [SerializeField] GameObject goBtnsRoot;

    public async void SetData(bool isMyWin, long myCombat)
    {
        goRoot.SetActive(false);
        goRewardRoot.SetActive(false);
        goBtnsRoot.SetActive(false);

        var goods_db = GameDatabase.GetInstance().tableDB.GetTableDB_Goods();
        var userInfo_db = GameDatabase.GetInstance().tableDB.GetUserInfo();
        int myPvpScore = userInfo_db.m_pvp_score;
        int myPvpToDayScore = userInfo_db.m_pvp_today_score;
        var dbOr = GameDatabase.GetInstance().pvpBattle.GetDataBattleDbOr();
        int pvp_difference_max = GameDatabase.GetInstance().chartDB.GetDicBalance("pvp.win.difference.score.max").val_int; // max 10 
        long difference = dbOr.statValue.combat_power < myCombat ? 0 : ((long)(dbOr.statValue.combat_power - myCombat) / (long)(myCombat * 0.1f));
        if (difference > pvp_difference_max)
            difference = pvp_difference_max;
         
        int win_valScore = GameDatabase.GetInstance().chartDB.GetDicBalance(string.Format("pvp.win.difference.score.{0}", difference)).val_int;
        int los_valScore = GameDatabase.GetInstance().chartDB.GetDicBalance(string.Format("pvp.lose.difference.score.{0}", difference)).val_int;

        int sendScore = 0, sendTodayScore = 0, sendRwdCoin = 0;
        ui.txResult.color = isMyWin == true ? ui.coWin : ui.coLose;
        GameDatabase.GetInstance().dailyMissionDB.ASetInCount(GameDatabase.DailyMissionDB.Nbr.nbr4, 1); // 일일미션, nbr4 배틀 아레나에 입장하여 결투하기! 

        if (isMyWin) // 승리시 
        {
            GameDatabase.GetInstance().achievementsDB.ASetInCount(GameDatabase.AchievementsDB.Nbr.nbr10, 1); // 업적, nbr10 PvP 배틀 아레나 승리 10회!
            GameDatabase.GetInstance().dailyMissionDB.ASetInCount(GameDatabase.DailyMissionDB.Nbr.nbr5, 1); // 일일미션, nbr5 배틀 아레나 입장하여 결투 승리하기!

            if (userInfo_db.m_pvp_win_streak < GameDatabase.GetInstance ().chartDB.GetDicBalance("pvp.win.streak.max.count").val_int)
            {
                userInfo_db.m_pvp_win_streak++;
            }

            if (userInfo_db.m_pvp_win_streak > 0)
            {
                GameDatabase.GetInstance().achievementsDB.ASetInCount(GameDatabase.AchievementsDB.Nbr.nbr21, userInfo_db.m_pvp_win_streak, false); // 업적, nbr21 배틀 아레나 2~10연승 하기!
            }

            sendScore = myPvpScore + win_valScore;
            sendTodayScore = myPvpToDayScore + win_valScore;
            sendRwdCoin = GameDatabase.GetInstance().chartDB.GetDicBalance(string.Format("pvp.difference.battle.coin.{0}", difference)).val_int;
            ui.txResult.text = "WIN<size=60>~!</size>";
            ui.txWinCount.text = string.Format("연승 : {0}", userInfo_db.m_pvp_win_streak.ToString());
            ui.txScore.text = string.Format("<color=#E3E1E3>+{0}</color>", win_valScore);
            ui.txCoin.text = string.Format("x{0}", sendRwdCoin);
            ui.txBefScore.text = string.Format("{0:#,0}", myPvpScore);
            ui.txAftScore.text = string.Format("<color=#00E6FF>{0:#,0}</color>", sendScore);
        }
        // 패배시 
        else
        {
            userInfo_db.m_pvp_win_streak = 0;
            sendScore = myPvpScore - los_valScore;
            sendTodayScore = myPvpToDayScore - los_valScore;
            sendRwdCoin = GameDatabase.GetInstance().chartDB.GetDicBalance("pvp.lose.battle.coin").val_int;
            //ui.txResult.text = string.Format("LOSE<size=60>...</size> <color=red><size=34>-{0}점  <color=white><size=34>[{1}점 <size=20>></size> {2}점]</size></color></size></color>", los_valScore, myPvpScore, sendScore);
            ui.txResult.text = "LOSE<size=60>...</size>";
            ui.txWinCount.text = "연승 : 0";
            ui.txScore.text = string.Format("<color=#FF0000>-{0}</color>", los_valScore);
            ui.txCoin.text = string.Format("x{0}", sendRwdCoin);
            ui.txBefScore.text = string.Format("{0:#,0}", myPvpScore);
            ui.txAftScore.text = string.Format("<color=#FF0000>{0:#,0}</color>", sendScore);
        }
        goods_db.m_battle_coin += sendRwdCoin;
        
        if(userInfo_db.m_pvp_win_streak > GameDatabase.GetInstance().chartDB.GetDicBalance("pvp.win.streak.max.count").val_int)
            userInfo_db.m_pvp_win_streak = GameDatabase.GetInstance().chartDB.GetDicBalance("pvp.win.streak.max.count").val_int;
        else if (userInfo_db.m_pvp_win_streak < 0)
            userInfo_db.m_pvp_win_streak = 0;
        
        Task tsk1 = GameDatabase.GetInstance().rankDB.ASendRTRank_PvpTop100(userInfo_db.m_pvp_win_streak, sendScore, sendTodayScore);
        Task tsk2 = GameDatabase.GetInstance().pvpBattle.SendPvPBattleRecord(isMyWin);
        Task tsk3 = GameDatabase.GetInstance().tableDB.SetUpdateGoods(goods_db);
        Task tsk4 = GameDatabase.GetInstance().tableDB.ConsumDungeonTicket(IG.ModeType.PVP_BATTLE_ARENA);
        while (Loading.Bottom(tsk1.IsCompleted, tsk2.IsCompleted, tsk3.IsCompleted, tsk4.IsCompleted) == false) await Task.Delay(100);

        goRoot.SetActive(true);
        GameDatabase.GetInstance().pvpBattleRecord.structData.rank.nextLoadTIme = BackendGpgsMng.GetInstance().GetNowTime();

        // 보상 -> 승패 상관없이 
        pvpRewards.Clear();
        if (isMyWin)
        {
            LogPrint.Print("win_score or : " + win_valScore);
            pvpRewards.Add(new PvpReward() { gch_type = "win_score", it_cnt = win_valScore, it_name = "점수" });
        }
        else
        {
            LogPrint.Print("lose_score or : " + los_valScore);
            pvpRewards.Add(new PvpReward() { gch_type = "lose_score", it_cnt = los_valScore, it_name = "점수" });
        }

        pvpRewards.Add(new PvpReward() { gch_type = "battle_coin", it_cnt = sendRwdCoin, it_name = "승리의 파편" });

        // 보상 -> 승리한 경우만 
        if (isMyWin)
        {
            string random_probability = GameDatabase.GetInstance().chartProbabilityDB.GetSelectedProbabilityFileId("r_pvp_win_reward");
            BackendReturnObject bro1 = null;
            SendQueue.Enqueue(Backend.Probability.GetProbabilitys, random_probability, userInfo_db.m_pvp_win_streak, callback => { bro1 = callback; });
            while (Loading.Bottom(bro1) == false) { await Task.Delay(100); }

            LogPrint.Print("##### bro1.IsSuccess() : " + bro1.IsSuccess() + ", GetProbability : " + bro1.GetReturnValuetoJSON().ToJson());
            if (bro1.IsSuccess())
            {
                var rows = bro1.GetReturnValuetoJSON()["elements"];
                foreach (JsonData item in rows)
                {
                    pvpRewards.Add(new PvpReward()
                    {
                        gch_type = "item",
                        it_type = RowPaser.IntPaser(item, "it_type"),
                        it_rating = RowPaser.IntPaser(item, "it_rating"),
                        it_cnt = RowPaser.IntPaser(item, "it_cnt"),
                        it_name = RowPaser.StrPaser(item, "it_name"),
                    });
                }
            }
        }

        goRewardRoot.SetActive(true);
        initOnStartPvpReward.SetData(pvpRewards);
        NotificationIcon.GetInstance().CheckNoticeContentsTicket();

        await SendReward();
        StopCoroutine("IEndPop");
        StartCoroutine("IEndPop");
    }

    Dictionary<int, TransactionParam> sendUpdateTransParam = new Dictionary<int, TransactionParam>();
    private async Task SendReward()
    {
        TransactionParam transactParam = new TransactionParam();
        List<GameDatabase.TableDB.Item> rwd_item = new List<GameDatabase.TableDB.Item>();
        foreach (var RWD in pvpRewards)
        {
            if (string.Equals(RWD.gch_type, "item"))
            {
                var temp_item_DB = GameDatabase.GetInstance().tableDB.GetItem(RWD.it_type, RWD.it_rating);
                int indx = rwd_item.FindIndex(obj => string.Equals(obj.type, RWD.it_type) && string.Equals(obj.rating, RWD.it_rating));
                if(indx >= 0) // 서버+클라에 아이템 존재 
                {
                    var tmp = rwd_item[indx];
                    tmp.count += RWD.it_cnt;
                    rwd_item[indx] = tmp;
                }
                else
                {
                    temp_item_DB.count += RWD.it_cnt;
                    rwd_item.Add(temp_item_DB);
                }
            }
        }

        if (rwd_item.Count > 0)
        {
            List<GameDatabase.TableDB.Item> insert_itemDB = new List<GameDatabase.TableDB.Item>();
            List<GameDatabase.TableDB.Item> update_itemDB = new List<GameDatabase.TableDB.Item>();
            foreach (var RWD in rwd_item)
            {
                if (string.IsNullOrEmpty(RWD.indate) == true)
                {
                    //LogPrint.EditorPrint("------ SendReward INSERT RWD ----- " + JsonUtility.ToJson(RWD));
                    transactParam.AddInsert(BackendGpgsMng.tableName_Item,
                    ParamT.Collection(new ParamT.P[]
                    {
                        new ParamT.P() { k = "aInUid", v = RWD.aInUid },
                        new ParamT.P() { k = "type", v = RWD.type },
                        new ParamT.P() { k = "rating", v = RWD.rating },
                        new ParamT.P() { k = "count", v = RWD.count },
                    }));
                    insert_itemDB.Add(RWD);
                }
                else
                {
                    //LogPrint.EditorPrint("------ SendReward UPDATE RWD ----- " + JsonUtility.ToJson(RWD));
                    List<BackEnd.WRITE> writes = new List<BackEnd.WRITE> { new WRITE { Action = TransactionAction.Update, Param = ParamT.Collection(new ParamT.P[] { new ParamT.P() { k = "count", v = RWD.count } }) } };
                    transactParam.AddUpdateList(BackendGpgsMng.tableName_Item, RWD.indate, writes);
                    update_itemDB.Add(RWD);
                }
            }

            BackendReturnObject bro = null;
            SendQueue.Enqueue(Backend.GameInfo.TransactionWrite, transactParam, callback => { bro = callback; });
            while (Loading.Full(bro) == false) await Task.Delay(100);

            //LogPrint.EditorPrint(" ----- SendReward 11111 ----- bro : " + bro);
            if (bro.GetReturnValuetoJSON() != null)
            {
                JsonData jdRows = bro.GetReturnValuetoJSON()["putItem"];
                if (jdRows.Count > 0 && insert_itemDB.Count > 0 && jdRows.Count == insert_itemDB.Count)
                {
                    for (int i = 0; i < jdRows.Count; i++)
                    {
                        //LogPrint.EditorPrint("SendReward 11111 [" + i + "] -> " + jdRows[i].ToJson());
                        string inDate = (string)jdRows[i]["inDate"];
                        if (string.IsNullOrEmpty(inDate) == false)
                        {
                            var tmp = insert_itemDB[i];
                            tmp.indate = inDate;
                            GameDatabase.GetInstance().tableDB.UpdateClientDB_Item(tmp);
                        }
                    }
                }
            }

            if(update_itemDB.Count > 0)
            {
                foreach (var db in update_itemDB)
                    GameDatabase.GetInstance().tableDB.UpdateClientDB_Item(db);
            }
        }

        goBtnsRoot.SetActive(true);

        //sendUpdateTransParam.Clear();
        //List<GameDatabase.TableDB.Item> update_item_temp = new List<GameDatabase.TableDB.Item>(); // UPDATE 같은 종류 중복 체크하여 합침 
        //List<GameDatabase.TableDB.Item> insert_item_temp = new List<GameDatabase.TableDB.Item>(); // INSERT 같은 종류 중복 체크하여 합침 
        //foreach (var RWD in pvpRewards)
        //{
        //    LogPrint.EditorPrint("------ RWD ----- " + JsonUtility.ToJson(RWD));

        //    if(string.Equals(RWD.gch_type, "item"))
        //    {
        //        var temp = GameDatabase.GetInstance().tableDB.GetItem(RWD.it_type, RWD.it_rating);
        //        if(string.IsNullOrEmpty(temp.indate) == true)
        //        {
        //            int indx2 = insert_item_temp.FindIndex(x => x.type == RWD.it_type && x.rating == RWD.it_rating);
        //            if (indx2 >= 0)
        //            {
        //                var addTemp = insert_item_temp[indx2];
        //                addTemp.count += RWD.it_cnt;
        //                insert_item_temp[indx2] = addTemp;
        //            }
        //            else
        //            {
        //                temp.count += RWD.it_cnt;
        //                insert_item_temp.Add(temp);
        //            }
        //        }
        //        else
        //        {
        //            int indx = update_item_temp.FindIndex(x => x.type == RWD.it_type && x.rating == RWD.it_rating);
        //            if (indx >= 0)
        //            {
        //                var addTemp = update_item_temp[indx];
        //                addTemp.count += RWD.it_cnt;
        //                update_item_temp[indx] = addTemp;
        //            }
        //            else
        //            {
        //                temp.count += RWD.it_cnt;
        //                update_item_temp.Add(temp);
        //            }
        //        }
        //    }
        //}

        //for (int i = 0; i <= (int)(update_item_temp.Count / 5) + 1; i++)
        //{
        //    if(sendUpdateTransParam.ContainsKey(i) == false)
        //        sendUpdateTransParam.Add(i, new TransactionParam());
        //}

        //// UPDATE 
        //int id = 0;
        //foreach (var item in update_item_temp)
        //{
        //    Param pam = new Param();
        //    pam.Add("count", item.count);
        //    List<BackEnd.WRITE> writes = new List<BackEnd.WRITE> { new WRITE { Action = TransactionAction.Update, Param = pam } };
        //    sendUpdateTransParam[id].AddUpdateList(BackendGpgsMng.tableName_Item, item.indate, writes);
        //    if (sendUpdateTransParam[id].GetWriteValues().Count >= 5)
        //        id++;
        //}

        //foreach (var key in sendUpdateTransParam.Keys)
        //{
        //    if(sendUpdateTransParam[key].GetWriteValues().Count > 0)
        //    {
        //        BackendReturnObject bro1 = null;
        //        SendQueue.Enqueue(Backend.GameInfo.TransactionWrite, sendUpdateTransParam[key], callback => { bro1 = callback; });
        //        while (Loading.Bottom(bro1) == false) { await Task.Delay(100); }
        //    }
        //}

        //// INSERT 
        //for (int i = 0; i < insert_item_temp.Count; i++)
        //{
        //    var item = insert_item_temp[i];
        //    await GameDatabase.GetInstance().tableDB.SetAcquireItem(item.type, item.rating, item.count, false);
        //}

        //sendUpdateTransParam.Clear();
        //goBtnsRoot.SetActive(true);
    }

    IEnumerator IEndPop()
    {
        yield return null;
        float end = Time.time + 60;
        while (end - Time.time > 0)
        {
            ui.txEndPop.text = string.Format("확인 ({0})", ((int)(end - Time.time)).ToString());
            yield return null;
        }

        yield return null;
        Click_EndPop();
    }

    public void Click_EndPop() 
    {
        //GameMng.GetInstance().Routin_ChangeMode(MainUI.GetInstance().tapDungeon.stageMdTy, true, isGoHomeTap == true ? 0 : 4);

        //while (GameMng.GetInstance().mode_type != IG.ModeType.CHANGE_WAIT) await Task.Delay(100);
        //while (GameMng.GetInstance().mode_type == IG.ModeType.CHANGE_WAIT) await Task.Delay(100);

        //if (!isGoHomeTap)
        //    MainUI.GetInstance().tapDungeon.uiPvPBattleArena.initOnStartPvPBattleArenaMatching.SetInit();

        //PopUpMng.GetInstance().Close_PvpBattleHome();
        //PopUpMng.GetInstance().Close_PvpBattleResult();


        GameMng.GetInstance().Routin_ChangeMode(MainUI.GetInstance().tapDungeon.saveMdTy, true, 5);
        PopUpMng.GetInstance().Close_PvpBattleHome();
        GameMng.GetInstance().zbCam.PlayZombieCamera();
        gameObject.SetActive(false);
    }
}