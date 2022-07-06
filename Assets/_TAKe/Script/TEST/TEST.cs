using BackEnd;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System;

public class TEST : MonoBehaviour
{
    [SerializeField] TEST_EquipChange testEquipChange;

    public void Tutirial()
    {
        TutorialMng.GetInstance().SetStartMainTop();
    }

    public void ChangeEquip()
    {
        testEquipChange.gameObject.SetActive(true);
    }

    public void Achievment()
    {
        var a = GameDatabase.GetInstance().tableDB.GetTableDB_Goods();
        a.m_dia += 100;
        BackEnd.Param p = ParamT.Collection(new ParamT.P[]
        {
            new ParamT.P() { k = "a int", v = (int)123 },
            new ParamT.P() { k = "a long", v = (long)123 },
            new ParamT.P() { k = "a float", v = (float)123 },
            new ParamT.P() { k = "a double", v = (double)123 },
            new ParamT.P() { k = "a string", v = "123" },
        });
        LogPrint.Print(p.GetJson());

        return;
        GameDatabase.AchievementsDB.Nbr nbr = (GameDatabase.AchievementsDB.Nbr)UnityEngine.Random.Range(0, GameDatabase.GetInstance().achievementsDB.GetChartCount() + 1);
        int inCnt = 1;
        GameDatabase.GetInstance().achievementsDB.ASetInCount(nbr, inCnt);
    }

    public void DailyMission()
    {
        GameDatabase.DailyMissionDB.Nbr nbr = (GameDatabase.DailyMissionDB.Nbr)UnityEngine.Random.Range(0, GameDatabase.GetInstance().dailyMissionDB.GetChartCount() + 1);
        int inCnt = 1;
        GameDatabase.GetInstance().dailyMissionDB.ASetInCount(nbr, inCnt);
    }

    public async void SendPvPBTLMessage()
    {
        //GameDatabase.GetInstance().pvpBattle.SendPvPBattleRecord(UnityEngine.Random.Range(0, 100) < 50);
        // custom_1 - 2020-09-18T06:44:41.412Z
        // custom_2 - 2020-09-18T06:55:14.354Z
        // custom_3 - 2020-09-18T07:00:31.526Z
        // 상두     - 2020-09-10T13:01:16.839Z

        //string ndate = BackendGpgsMng.GetInstance().GetNowTime().ToString();
        //LogPrint.Print("ndate : " + ndate + ", m_indate : " + BackendGpgsMng.backendUserInfo.m_indate);
        //string my_nick = BackendGpgsMng.backendUserInfo.m_nickname;
        //string my_indate = BackendGpgsMng.backendUserInfo.m_indate;
        //string gamer_nick = "상두";
        //string gamer_indate = "2020-09-10T13:01:16.839Z";
        //if (!string.IsNullOrEmpty(gamer_indate))
        //{
        //    string winner_indate = UnityEngine.Random.Range(0, 10) < 5 ? my_indate : gamer_indate;
        //    string winner_comment = PlayerPrefs.GetString(PrefsKeys.key_PvPArenaComment);
        //    var myRankInfo = GameDatabase.GetInstance().rankDB.GetMyRankPvPBTLArena();
        //    GameDatabase.PvPBattleRecord.SendRecordContents contents = new GameDatabase.PvPBattleRecord.SendRecordContents()
        //    {
        //        sent_indate = my_indate,
        //        winner_indate = winner_indate,
        //        pvp_rank = myRankInfo.rank,
        //        pvp_score = myRankInfo.score,
        //        msg = winner_comment
        //    };

        //    LogPrint.Print(JsonUtility.ToJson(contents).Length);
        //    await GameDatabase.GetInstance().pvpBattleRecord.SendRecord(gamer_indate, contents);
        //    GameDatabase.GetInstance().pvpBattleRecord.SaveClientMySentMsg(contents, my_indate, gamer_indate, my_nick, gamer_nick);
        //}
    }

    public async void GetAllPvPBTLMessage()
    {
        GameDatabase.GetInstance().pvpBattleRecord.AGetLoadRecord(true, true);
    }

    public async void GetRecvPvPBTLMessage()
    {
        GameDatabase.GetInstance().pvpBattleRecord.AGetLoadRecord(true, false);
    }

    public async void GetSentPvPBTLMessage()
    {
        GameDatabase.GetInstance().pvpBattleRecord.AGetLoadRecord(false, true);
    }

    public async void Delete20PvpBTLMessage()
    {
        GameDatabase.GetInstance().pvpBattleRecord.DeleteRecordLastFrom20();
    }

    public async void DeleteAllPvpBTLMessage()
    {
        GameDatabase.GetInstance().pvpBattleRecord.DeleteRecordLastFrom20(true);
    }

    // 캐릭터 공개 데이터 
    public async void ClickTest_ASetPubCharData()
    {
        GameDatabase.GetInstance().publicContentDB.ASetPub_CharData(BackendGpgsMng.tableName_Pub_NowCharData); // 캐릭터 데이터 전송 
        GameDatabase.GetInstance().publicContentDB.ASetPub_CharData(BackendGpgsMng.tableName_Pub_ChapterClearCharData); // 클리어 당시 캐릭터 데이터 전송 
    }

    // 챕터 클리어 데이터 전송 
    public async void ClickTest_ASetRTRankChapterClear()
    {
        //GameDatabase.GetInstance().rankDB.ASetRTRank_RT_ChptBoss(); // 챕터 클리어 랭킹 등록 
        //GameDatabase.GetInstance().rankDB.ASetRTRank_RT_ChptStgTop50(); // 챕터 스테이지 넘버 랭킹 등록 
        //GameDatabase.GetInstance().publicContentDB.ASetPub_CharData(BackendGpgsMng.tableName_Pub_ChapterClearCharData); // 클리어 당시 캐릭터 데이터 전송 
    }

    public void Click_GoodsGoldUp()
    {
        var db = GameDatabase.GetInstance().tableDB.GetTableDB_Goods();
        db.m_gold = 10000000;
        GameDatabase.GetInstance().tableDB.SetUpdateGoods(db);
    }

    public void Click_GoodsGoldZero()
    {
        var db = GameDatabase.GetInstance().tableDB.GetTableDB_Goods();
        db.m_gold = 0;
        GameDatabase.GetInstance().tableDB.SetUpdateGoods(db);
    }

    public void Click_GoodsDiaUp()
    {
        var db = GameDatabase.GetInstance().tableDB.GetTableDB_Goods();
        db.m_dia = 10000;
        GameDatabase.GetInstance().tableDB.SetUpdateGoods(db);
    }


    public async void ClickTest_ASetRTRAnkChapterClearRandom()
    {
        //int test11 = UnityEngine.Random.Range(1, 160);
        //GameDatabase.GetInstance().rankDB.ASetRTRank_RT_ChptBoss();
        //GameDatabase.GetInstance().rankDB.ASetRTRank_RT_ChptStgTop50();
    }

    public async void ClickTest_ASetRTRankPvpScore()
    {
        //int test_v = UnityEngine.Random.Range(1, 1000);
        //GameDatabase.GetInstance().rankDB.ASendRTRank_PvpTop100(-1, test_v, test_v);
    }

    public void ClickTEST_GoodsInventoryUp()
    {
        int max_invn = GameDatabase.GetInstance().chartDB.GetDicBalance("inventory.max.level").val_int;
        var uInfo = GameDatabase.GetInstance().tableDB.GetUserInfo();
        if (max_invn > uInfo.m_invn_lv)
        {
            uInfo.m_invn_lv++;
            GameDatabase.GetInstance().tableDB.SetUpdate_UserInfo(uInfo);
        }
        else
        {
            PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("인벤토리가 최대치까지 확장 완료된 상태입니다.");
        }
    }

    public async void ClickTEST_RandomEquipAdd()
    {
        var temp_gd = GameDatabase.GetInstance().tableDB.GetTableDB_Goods();
        temp_gd.m_ruby += 10;
        Task<bool> t1 = GameDatabase.GetInstance().tableDB.SetUpdateGoods(temp_gd);
        await t1;
    }

    int ts = 7;
    public async void ClickTEST_RandomSkillAdd()
    {
        if (ts <= 26)
        {
            int nwIdx = ts;
            int nwCount = 1;

            var temp = GameDatabase.GetInstance().tableDB.GetSkill(nwIdx);
            temp.count += nwCount;
            Task<bool> tb = GameDatabase.GetInstance().tableDB.SendDataSkill(temp);
            await tb;
            if (tb.Result)
            {
                ts++;
            }
        }

        //GameDatabase.GetInstance().tableDB.AddAcquireNewSkills();
    }

    // 자동 장비 판매 -> 시간 구매  
    public async void AddAutoSale_1hour(int hour)
    {
        if (hour <= 0)
            return;

        DateTime nDate = BackendGpgsMng.GetInstance().GetNowTime();
        var db = GameDatabase.GetInstance().tableDB.GetUserInfo();
        DateTime tryDate;
        if(DateTime.TryParse(db.m_auto_eq_sale_date, out tryDate) == false)
            tryDate = nDate;

        LogPrint.Print("1_date : " + tryDate + ", _date.add : " + tryDate.AddHours(1));
        db.m_auto_eq_sale_date = tryDate.AddHours(GameDatabase.GetInstance().chartDB.GetDicBalance(string.Format("auto.sale.add.{0}hour", hour)).val_int).ToString();
        await GameDatabase.GetInstance().tableDB.SetUpdate_UserInfo(db);
        LogPrint.Print("2 db.m_auto_eq_sale_date  : " + db.m_auto_eq_sale_date);
        ConvenienceFunctionMng.GetInstance().InitConvenienceAutoSale();
    }

    // 자동 장비 판매 -> 일 구매  
    public async void AddAutoSale_day(int day)
    {
        if (day <= 0)
            return;

        DateTime nDate = BackendGpgsMng.GetInstance().GetNowTime();
        var db = GameDatabase.GetInstance().tableDB.GetUserInfo();
        DateTime tryDate;
        if (DateTime.TryParse(db.m_auto_eq_sale_date, out tryDate) == false)
        {
            tryDate = nDate;
        }

        LogPrint.Print("1_date : " + tryDate);
        db.m_auto_eq_sale_date = tryDate.AddDays(GameDatabase.GetInstance().chartDB.GetDicBalance(string.Format("auto.sale.add.{0}day", day)).val_int).ToString();
        await GameDatabase.GetInstance().tableDB.SetUpdate_UserInfo(db);
        LogPrint.Print("2 db.m_auto_eq_sale_date  : " + db.m_auto_eq_sale_date);
        ConvenienceFunctionMng.GetInstance().InitConvenienceAutoSale();
    }

    /// <summary>
    /// 전설 장비 부위별 모두 획득 
    /// </summary>
    public void AddEquipRating7()
    {
        int r_ty = UnityEngine.Random.Range(0, 10);
        int rt = 7;
        int id = 1;
        var new_eqDB = GameDatabase.GetInstance().tableDB.GetNewEquipmentData(r_ty, rt, id);
        new_eqDB.m_norm_lv = 100;
        new_eqDB.m_ehnt_lv = 30;
        new_eqDB.m_state = 0;
        GameDatabase.GetInstance().equipmentEncyclopediaDB.DropAcquisitionAdded(new_eqDB.eq_ty, new_eqDB.eq_rt, new_eqDB.eq_id);
        GameDatabase.GetInstance().tableDB.UpdateClientDB_Equip(new_eqDB); // client DropViewEquipQueue
        PopUpMng.GetInstance().Open_DropEquip(new_eqDB);
        GameDatabase.GetInstance().chat.ChatSendItemMessage("equip", new_eqDB.eq_ty, new_eqDB.eq_rt, new_eqDB.eq_id);
        MainUI.GetInstance().NewEquipItemInventortRefresh();
    }
}
