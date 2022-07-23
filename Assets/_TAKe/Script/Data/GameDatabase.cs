using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using LitJson;
using BackEnd;
using static BackEnd.BackendAsyncClass;
using System.Threading.Tasks;
using System;
using System.Xml;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using System.Runtime.InteropServices;
using TMPro.Examples;
using System.Reflection;
using UnityEngine.SocialPlatforms.Impl;
using Newtonsoft.Json;
using System.Security.Cryptography.X509Certificates;
using System.ComponentModel;
using System.Linq.Expressions;
using UnityEngine.UI;
using TMPro;
#if UNITY_EDITOR
using UnityEditor.Purchasing;
#endif
using CodeStage.AntiCheat.ObscuredTypes;

public class GameDatabase
{
    private static GameDatabase m_Instance;
    public static GameDatabase GetInstance()
    {
        if (m_Instance == null)
        {
            m_Instance = new GameDatabase();
        }

        return m_Instance;
    }

    public const int equip_max_level = 35;

    // List 직렬화 (리스트를 뒤끝서버로 보낼때 사용함)
    // JsonUtility.ToJson(list)
    [Serializable]
    public class Serialization<T>
    {
        [SerializeField]
        List<T> rows;
        public List<T> ToList() { return rows; }

        public Serialization(List<T> target)
        {
            this.rows = target;
        }
    }

    /// <summary>
    /// ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
    /// 선언 
    /// ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
    /// </summary>
#region 
    private static readonly TableDB _tableDB = new TableDB();
    public TableDB tableDB = _tableDB;

    private static readonly ChartDB _chartDB = new ChartDB();
    public ChartDB chartDB = _chartDB;

    private static readonly ChartProbabilityDB _chartProbabilityDB = new ChartProbabilityDB();
    public ChartProbabilityDB chartProbabilityDB = _chartProbabilityDB;
    
    private static readonly SmithyDB _smithyDB = new SmithyDB();
    public SmithyDB smithyDB = _smithyDB;
    
    private static readonly InventoryDB _inventoryDB = new InventoryDB();
    public InventoryDB inventoryDB = _inventoryDB;

    private static readonly CharacterDB _characterDB = new CharacterDB();
    public CharacterDB characterDB = _characterDB;

    private static readonly MonsterDB _monsterDB = new MonsterDB();
    public MonsterDB monsterDB = _monsterDB;

    private static readonly DungeonDB _dungeonDB = new DungeonDB();
    public DungeonDB dungeonDB = _dungeonDB;

    private static readonly RankDB _rankDB = new RankDB();
    public RankDB rankDB = _rankDB;
    
    private static readonly PublicContentDB _publicContentDB = new PublicContentDB();
    public PublicContentDB publicContentDB = _publicContentDB;

    private static readonly AttendanceDB _attendanceDB = new AttendanceDB();
    public AttendanceDB attendanceDB = _attendanceDB;

    private static readonly MailDB _mailDB = new MailDB();
    public MailDB mailDB = _mailDB;
     
    private static readonly Chat _chat = new Chat();
    public Chat chat = _chat;

    private static readonly PvPBattleRecord _pvpBattleRecord = new PvPBattleRecord();
    public PvPBattleRecord pvpBattleRecord = _pvpBattleRecord;

    private static readonly PvPBattle _pvpBattle = new PvPBattle();
    public PvPBattle pvpBattle = _pvpBattle;


    private static readonly AchievementsDB _achievementsDB = new AchievementsDB();
    public AchievementsDB achievementsDB = _achievementsDB;
    private static readonly DailyMissionDB _dailyMissionDB = new DailyMissionDB();
    public DailyMissionDB dailyMissionDB = _dailyMissionDB;

    private static readonly QuestDB _questDB = new QuestDB();
    public QuestDB questDB = _questDB;

    private static readonly ConvenienceFunctionDB _convenienceFunctionDB = new ConvenienceFunctionDB();
    public ConvenienceFunctionDB convenienceFunctionDB = _convenienceFunctionDB;

    private static readonly ShopDB _shopDB = new ShopDB();
    public ShopDB shopDB = _shopDB;

    private static readonly EquipmentEncyclopediaDB _equipmentEncyclopediaDB = new EquipmentEncyclopediaDB();
    public EquipmentEncyclopediaDB equipmentEncyclopediaDB = _equipmentEncyclopediaDB;

    #endregion

    /// <summary>
    /// ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
    /// 기타
    /// ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
    /// </summary>
#region
    //private int ntmp_1 = 10;
    private long lastUniqIdx = 0;
    public long GetUniqueIDX(int count = 1)
    {
        //ntmp_1++;
        //if (ntmp_1 >= 10)
        //    ntmp_1 = 1;

        //string s_date = BackendGpgsMng.GetInstance().GetNowTime().ToString();
        ////string s_date = string.Format("{0:u}", BackendGpgsMng.GetInstance().GetNowTime());
        //string strTmp = Regex.Replace(s_date, @"\D", "").Substring(2);
        //long nTmp = long.Parse(strTmp);
        //nTmp += ntmp_1;
        //return nTmp;
        
        string id = string.Format("{0}{1}{2}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
        string[] split = DateTime.Now.TimeOfDay.ToString().Split(new Char[] { ':', '.' });
        for (int i = 0; i < split.Length; i++)
            id += split[i];

        long val = System.Convert.ToInt64(id.Substring(2, 16));
        if (lastUniqIdx >= val)
            lastUniqIdx++;
        else lastUniqIdx = val;

        return lastUniqIdx;
    }

    /// 클라이언트에서 뒤끝에서 받아온 해당 테이블 아이템의 InDate값을 찾는다 
    public string GetFindInDate(string _table_name, string _uid)
    {
        string _indate = string.Empty;
        switch (_table_name)
        {
            case BackendGpgsMng.tableName_Goods:        _indate = tableDB.GetTableDB_Goods().indate; break;
            //case BackendGpgsMng.tableName_Character:    _indate = myWeapons.Find((DataWeapon obj) => string.Equals(obj.UniqIdx, _uniq_idx)).InDate; break;
            case BackendGpgsMng.tableName_Equipment:    _indate = tableDB.GetAllEquipment ().Find((TableDB.Equipment obj) => string.Equals(obj.aInUid, _uid)).indate; break;
            //case BackendGpgsMng.tableName_Item:         _indate = myArmors.Find((DataArmor obj) => string.Equals(obj.UniqIdx, _uniq_idx)).InDate; break;
            //case BackendGpgsMng.tableName_Skill:        _indate = myWings.Find((DataWing obj) => string.Equals(obj.UniqIdx, _uniq_idx)).InDate; break;
            default: _indate = "table_error"; break;
        }

        if (string.Equals(_indate, "table_error"))
        {
            Debug.LogError("존재하지 않는 테이블명입니다.");
        }
        else
        {
            if (!string.IsNullOrEmpty(_indate))
                Debug.Log("<color=yellow> [client], " + _table_name + " 해당 아이템의 InDate값을 찾았습니다. : " + _indate + "</color>");
            else Debug.Log("<color=red> [client], " + _table_name + "해당 아이템의 InDate값을 찾지 못했습니다. : NULL </color>");
        }

        return _indate;
    }

    public delegate void LanguageChange();
    public LanguageChange lanChange;
    public string GetGameString(string id) =>LanguageGameData.GetInstance().GetString(id).Trim();
    public string GetSystemString(string id) => LanguageSystemData.GetInstance().GetString(id).Trim();

    public int GetStatRandomLevel()
    {
        /* => (int)(UnityEngine.Random.value * 10 + 1);*/
        float r = GetRandomPercent();
        float rpct = 0.0f;
        for (int i = 10; i > 0; i--)
        {
            float rv = chartDB.GetDicBalance(string.Format("stat.random.lv_{0}", i)).val_float;
            if (rv > 0f)
            {
                rpct += rv;
                if (r < rpct)
                {
                    return i;
                }
            }
        }

        return 1;
    }
    public float GetRandomPercent() => UnityEngine.Random.value * 100.0f;
    public float GetRandomPercent(float max_pct) => UnityEngine.Random.Range(0, max_pct);

    /// <summary>
    /// int 값이 0 -> ON , 1 -> OFF
    /// </summary>
    public Option option;
    [System.Serializable]
    public struct Option
    {
        public int notice_server_push;
        public int notice_local_push;
        public int notice_inventory_max;
        public int notice_equip_send_rt5;
        public int notice_equip_send_rt6;
        public int notice_equip_send_rt7;
        public int notice_equip_recv_rt5;
        public int notice_equip_recv_rt6;
        public int notice_equip_recv_rt7;

        public int setting_bgm_snd;
        public int setting_sfx_snd;
        public int setting_equip_vib;
        public int setting_damage_txt;
        public int setting_quality;
    }

    #endregion

    /// <summary>
    /// ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
    /// #테이블 데이터 (비공개)
    /// ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
    /// </summary>
    public class TableDB : ChartDB
    {
        // ########################################################################################################
        // ################################ TBC < 재화 > ##########################################################
        // ########################################################################################################
#region ######## TBC ########################################################
        List<IAPMng.TheBackendProduct> productList = new List<IAPMng.TheBackendProduct>();

        /// <summary>
        /// TBC 상품 리스트 
        /// </summary>
        public void SetTBCProduct (JsonData jsonData)
        {
            for (int i = 0; i < jsonData["rows"].Count; i++)
            {
                var prodcutUuid = jsonData["rows"][i]["uuid"]["S"].ToString();
                var productName = jsonData["rows"][i]["name"]["S"].ToString();
                productList.Add(new IAPMng.TheBackendProduct(prodcutUuid, productName));
            }
        }

        /// <summary>
        /// 뒤끝 캐시 (TBC -> 인게임 화이트다이아)
        /// </summary>
        public async Task<int> GetMyTBC()
        {
            BackendReturnObject bro1 = null;
            SendQueue.Enqueue(Backend.TBC.GetTBC, callback => { bro1 = callback; });
            while (bro1 == null) { await Task.Delay(100); }

            if (bro1.IsSuccess())
            {
                ObscuredInt itbc = (ObscuredInt)System.Convert.ToInt32(bro1.GetReturnValuetoJSON()["amountTBC"].ToString());
                var goods_db = GetInstance().tableDB.GetTableDB_Goods();

                itbc -= goods_db.m_30DayTbc > 0 ? goods_db.m_30DayTbc : (ObscuredInt)0;
                itbc -= goods_db.m_7DayTbc > 0 ? goods_db.m_7DayTbc : (ObscuredInt)0;

                if(MainUI.GetInstance() != null)
                    MainUI.GetInstance().topUI.InfoViewTBC(itbc);

                return itbc;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// TBC 차감 -> 서버에 등록한 아이템 키값 
        /// </summary>
        public async Task<string> DeductionTBC(string tbcItemKey)
        {
            int uuid_indx = productList.FindIndex(x => x.name == tbcItemKey);
            string uuid = "";
            if (uuid_indx >= 0)
                uuid = productList[uuid_indx].uuid;

            BackendReturnObject bro1 = null;
            SendQueue.Enqueue(Backend.TBC.UseTBC, uuid, "DeductionTBC", callback => { bro1 = callback; });
            while (Loading.Full(bro1) == false) { await Task.Delay(100); }

            LogPrint.Print("DeductionTBC tbcItemKey : " + tbcItemKey + ", bro1 : " + bro1);
            if (bro1.IsSuccess())
            {
                MainUI.GetInstance().topUI.GetInfoViewTBC();
                return "success";
            }
            else return bro1.GetMessage();
        }

        /// <summary>
        /// TBC 차감 (1 ~ 100, 100 단위로 1500까지) 
        /// </summary>
        public async Task<bool> DeductionTBC(int dedCnt)
        {
            LogPrint.Print("DeductionTBC dedCnt : " + dedCnt);
            if (dedCnt <= 0)
                return true;

            if(productList.Count == 0)
            {
                PopUpMng.GetInstance().Open_MessageError("상품 리스트 정보가 잘못되었습니다.");
            }
            else
            {
                int tbc = await GetMyTBC();
                List<int> dedList = new List<int>();
                while (dedCnt > 0)
                {
                    LogPrint.Print("DeductionTBC 11111 -dedCnt : " + dedCnt);
                    if (dedCnt >= 100)
                    {
                        if (dedCnt >= 1500)
                        {
                            dedList.Add(1500); dedCnt -= 1500;
                        }
                        else if (dedCnt >= 1400)
                        {
                            dedList.Add(1400); dedCnt -= 1400;
                        }
                        else if (dedCnt >= 1300)
                        {
                            dedList.Add(1300); dedCnt -= 1300;
                        }
                        else if (dedCnt >= 1200)
                        {
                            dedList.Add(1200); dedCnt -= 1200;
                        }
                        else if (dedCnt >= 1100)
                        {
                            dedList.Add(1100); dedCnt -= 1100;
                        }
                        else if (dedCnt >= 1000)
                        {
                            dedList.Add(1000); dedCnt -= 1000;
                        }
                        else if (dedCnt >= 900)
                        {
                            dedList.Add(900); dedCnt -= 900;
                        }
                        else if (dedCnt >= 800)
                        {
                            dedList.Add(800); dedCnt -= 800;
                        }
                        else if (dedCnt >= 700)
                        {
                            dedList.Add(700); dedCnt -= 700;
                        }
                        else if (dedCnt >= 600)
                        {
                            dedList.Add(600); dedCnt -= 600;
                        }
                        else if (dedCnt >= 500)
                        {
                            dedList.Add(500); dedCnt -= 500;
                        }
                        else if (dedCnt >= 400)
                        {
                            dedList.Add(400); dedCnt -= 400;
                        }
                        else if (dedCnt >= 300)
                        {
                            dedList.Add(300); dedCnt -= 300;
                        }
                        else if (dedCnt >= 200)
                        {
                            dedList.Add(200); dedCnt -= 200;
                        }
                        else if (dedCnt >= 100)
                        {
                            dedList.Add(100); dedCnt -= 100;
                        }

                        LogPrint.Print("DeductionTBC 22222 -dedCnt : " + dedCnt);
                    }
                    else
                    {
                        LogPrint.Print("DeductionTBC 33333 -dedCnt : " + dedCnt);
                        dedList.Add(dedCnt);
                        dedCnt = 0;
                        LogPrint.Print("DeductionTBC 44444 -dedCnt : " + dedCnt);
                    }

                    LogPrint.Print("DeductionTBC 55555 -dedCnt : " + dedCnt);
                }

                LogPrint.Print("DeductionTBC 66666 productList.count : " + productList.Count);
                for (int i = 0; i < dedList.Count; i++)
                {   
                    int uuid_indx = productList.FindIndex(x => x.name == dedList[i].ToString());
                    if (uuid_indx >= 0)
                    {
                        LogPrint.Print("DeductionTBC 77777 uuid_indx : " + productList[uuid_indx].uuid);
                        BackendReturnObject bro1 = null;
                        SendQueue.Enqueue(Backend.TBC.UseTBC, productList[uuid_indx].uuid, "DeductionTBC", callback => { bro1 = callback; });
                        while (Loading.Bottom(bro1) == false) { await Task.Delay(100); }
                        LogPrint.Print("DeductionTBC 88888 [ i : " + i +" ] bro1 : " + bro1);
                    }
                }
            }

            await Task.Delay(1000);
            MainUI.GetInstance().topUI.GetInfoViewTBC();
            return true;
        }

        public string GetProductUUID(string tbcProductName)
        {
            int uuid_indx = productList.FindIndex(x => x.name == tbcProductName);
            if (uuid_indx >= 0)
            {
                return productList[uuid_indx].uuid;
            }
            else return "";
        }

        /// <summary>
        /// 자동 판매 구매 
        /// </summary>
        public async Task<bool> AutoSaleAddDay(int day)
        {
            if (day >= 1 || day <= 30)
            {
                var userInfo_db = GameDatabase.GetInstance().tableDB.GetUserInfo();
                DateTime nDate = BackendGpgsMng.GetInstance().GetNowTime(), tryDate;
                if (DateTime.TryParse(userInfo_db.m_auto_eq_sale_date, out tryDate) == false)
                    tryDate = nDate;

                if (tryDate <= nDate)
                    tryDate = nDate;

                userInfo_db.m_auto_eq_sale_date = tryDate.AddDays(GetInstance().chartDB.GetDicBalance(string.Format("auto.sale.add.{0}day", day)).val_int).ToString();

                Task tsk1 = GetInstance().tableDB.SetUpdate_UserInfo(userInfo_db);
                while (Loading.Bottom(tsk1.IsCompleted) == false) await Task.Delay(100);

                MainUI.GetInstance().topUI.GetInfoViewTBC();
                return true;
            }
            else return false;
        }
        #endregion

        // ########################################################################################################
        // ################################ Goods < 재화 > ########################################################
        // ########################################################################################################
#region ######## Goods < 재화 > ########################################################
        [System.Serializable]
        public struct Goods
        {
            public string indate;
            public ObscuredInt m_tbc;
            public ObscuredInt m_dia; // 게임 다이아 
            public ObscuredLong m_gold; // 게임 골드 
            public ObscuredInt m_ether; // 에테르 : 장신구 뽑기용 
            public ObscuredInt m_ruby; // 루비 : 장비 뽑기용 일반~전설 
            public ObscuredInt m_battle_coin; // 배틀코인 : pvp승리시 획득, 상점에서 장비 + 장신구 전설까지 등장 뽑기에 사용 

            // 기간제 TBC 
            public ObscuredInt m_30DayTbc; // 30일 매일 다이아 사품 구입시 TBC로에 한번에 지급한뒤에 UI상에서 보여주거나 TBC사용할때 이 변수값을 뺀 나머지로 TBC확률 할 수 있도록, 다음날 접속시 차감 
            public string m_30DayTbcDailyRewardDate; // 30일 매일 다이아 오늘 받았는지 날짜
            public string m_30DayTbcStartDate; // 30일 매일 다이아 구매 날짜 
            public string m_30DayTbcEndDate; // 30일 매일 다이아 종료 날짜 
            public ObscuredInt m_7DayTbc;
            public string m_7DayTbcDailyRewardDate; // 30일 매일 다이아 오늘 받았는지 날짜
            public string m_7DayTbcStartDate; // 30일 매일 다이아 구매 날짜 
            public string m_7DayTbcEndDate; // 7일 매일 다이아 종료 날짜 

            public ObscuredInt m_fr_ticket_dg_top;
            public ObscuredInt m_fr_ticket_dg_mine;
            public ObscuredInt m_fr_ticket_dg_raid;
            public ObscuredInt m_fr_ticket_pvp_arena;
        }

        // 아이템 목록
        //  ty	        rt	        name
        //  10	        0	        골드
        //  11	        0	        다이아
        //  12	        0	        에테르
        //  13	        0	        루비


        private static readonly Goods goods = new Goods();
        private Goods _goods = goods;
        DateTime dateGoodsLastUpdateAll;
        public Goods GetTableDB_Goods() => _goods; // 재화 데이터 

        public void Goods_RandomizeCryptoKey()
        {
            _goods.m_dia.RandomizeCryptoKey();
            _goods.m_tbc.RandomizeCryptoKey();
            _goods.m_gold.RandomizeCryptoKey();
            _goods.m_ether.RandomizeCryptoKey();
            _goods.m_ruby.RandomizeCryptoKey();
            _goods.m_battle_coin.RandomizeCryptoKey();
            _goods.m_30DayTbc.RandomizeCryptoKey();
            _goods.m_7DayTbc.RandomizeCryptoKey();
            _goods.m_fr_ticket_dg_top.RandomizeCryptoKey();
            _goods.m_fr_ticket_dg_mine.RandomizeCryptoKey();
            _goods.m_fr_ticket_dg_raid.RandomizeCryptoKey();
            _goods.m_fr_ticket_pvp_arena.RandomizeCryptoKey();
        }

        public Param ParamAddGoods()
        {
            Param pam = new Param();
            pam.Add("m_tbc", _goods.m_tbc);
            pam.Add("m_dia", _goods.m_dia);
            pam.Add("m_gold", _goods.m_gold);
            pam.Add("m_ether", _goods.m_ether);
            pam.Add("m_ruby", _goods.m_ruby);
            pam.Add("m_battle_coin", _goods.m_battle_coin);
            pam.Add("m_30DayTbc", _goods.m_30DayTbc);
            pam.Add("m_30DayTbcDailyRewardDate", _goods.m_30DayTbcDailyRewardDate);
            pam.Add("m_30DayTbcStartDate", _goods.m_30DayTbcStartDate);
            pam.Add("m_30DayTbcEndDate", _goods.m_30DayTbcEndDate);
            pam.Add("m_7DayTbc", _goods.m_7DayTbc);
            pam.Add("m_7DayTbcDailyRewardDate", _goods.m_7DayTbcDailyRewardDate);
            pam.Add("m_7DayTbcStartDate", _goods.m_7DayTbcStartDate);
            pam.Add("m_7DayTbcEndDate", _goods.m_7DayTbcEndDate);
            pam.Add("m_fr_ticket_dg_top", _goods.m_fr_ticket_dg_top);
            pam.Add("m_fr_ticket_dg_mine", _goods.m_fr_ticket_dg_mine);
            pam.Add("m_fr_ticket_dg_raid", _goods.m_fr_ticket_dg_raid);
            pam.Add("m_fr_ticket_pvp_arena", _goods.m_fr_ticket_pvp_arena);

            return pam;
        }

        public void SetInitDB_Goods(Goods val)
        {
            _goods = val; // 재화 데이터 초기화 
            dateGoodsLastUpdateAll = BackendGpgsMng.GetInstance().GetNowTime().AddMinutes(30);
        }
        private void SetUpdateGoodsGold(ObscuredLong val) => _goods.m_gold = val;
        private void SetUpdateGoodsRuby(ObscuredInt val) => _goods.m_ruby = val;
        private void SetUpdateGoodsEther(ObscuredInt val) => _goods.m_ether = val;

        public async void SetUpdateGoods(string m_name, long val, string cal, bool send_server = false, string send_param_name = "")
        {
            if (string.Equals(m_name, "gold"))
                SetUpdateGoodsGold(string.Equals(cal, "+") ? _goods.m_gold + (ObscuredLong)val: _goods.m_gold - (ObscuredLong)val);
            else if (string.Equals(m_name, "ruby"))
                SetUpdateGoodsRuby(string.Equals(cal, "+") ? _goods.m_ruby + (ObscuredInt)val : _goods.m_ruby - (ObscuredInt)val);
            else if (string.Equals(m_name, "ether"))
                SetUpdateGoodsEther(string.Equals(cal, "+") ? _goods.m_ether + (ObscuredInt)val : _goods.m_ether - (ObscuredInt)val);

            MainUI.GetInstance().topUI.SetGoodsView();
            if (dateGoodsLastUpdateAll < BackendGpgsMng.GetInstance().GetNowTime() || send_server)
            {
                if(string.Equals(send_param_name, "gold"))
                {
                    await SetUpdateGoods(_goods, false, "gold");
                }
                else await SetUpdateGoods(_goods, true);

                dateGoodsLastUpdateAll = BackendGpgsMng.GetInstance().GetNowTime().AddMinutes(30);
            }

            if (string.Equals(m_name, "ruby") || string.Equals(m_name, "ether"))
            {
                if (MainUI.GetInstance().tapIAP.tapShopLuck.gameObject.activeSelf)
                {
                    MainUI.GetInstance().tapIAP.tapShopLuck.GoodsView();
                }
            }
        }

        /// <summary>
        /// Goods 데이터 업데이트  
        /// </summary>
        public async Task<bool> SetUpdateGoods(Goods val, bool send_param_all = false, string send_param_name = "")
        {
            LogPrint.Print("기존 데이터를 Update한다.");
            Goods temp_goods = _goods;
            Param param_upd = new Param();
            if (temp_goods.m_gold != val.m_gold || (send_param_all == true || string.Equals(send_param_name, "gold")))
                param_upd.Add("m_gold", val.m_gold.ToString());

            if (temp_goods.m_dia != val.m_dia || (send_param_all == true || string.Equals(send_param_name, "dia")))
            {
                if (val.m_dia < 0)
                    val.m_dia = 0;

                param_upd.Add("m_dia", val.m_dia);
            }
                
            if (temp_goods.m_ether != val.m_ether || (send_param_all == true || string.Equals(send_param_name, "m_ether")))
                param_upd.Add("m_ether", val.m_ether);

            if (temp_goods.m_ruby != val.m_ruby || (send_param_all == true || string.Equals(send_param_name, "m_ruby")))
                param_upd.Add("m_ruby", val.m_ruby);

            if (temp_goods.m_battle_coin != val.m_battle_coin || (send_param_all == true || string.Equals(send_param_name, "m_battle_coin")))
                param_upd.Add("m_battle_coin", val.m_battle_coin);

            // 기간제 TBC(화이트 다이아) 
            if (temp_goods.m_30DayTbc != val.m_30DayTbc)
                param_upd.Add("m_30DayTbc", val.m_30DayTbc);

            if (temp_goods.m_30DayTbcDailyRewardDate != val.m_30DayTbcDailyRewardDate) 
                param_upd.Add("m_30DayTbcDailyRewardDate", val.m_30DayTbcDailyRewardDate);

            if (temp_goods.m_30DayTbcStartDate != val.m_30DayTbcStartDate)
                param_upd.Add("m_30DayTbcStartDate", val.m_30DayTbcStartDate);

            if (temp_goods.m_30DayTbcEndDate != val.m_30DayTbcEndDate)
                param_upd.Add("m_30DayTbcEndDate", val.m_30DayTbcEndDate);
            
            if (temp_goods.m_7DayTbc != val.m_7DayTbc)
                param_upd.Add("m_7DayTbc", val.m_7DayTbc);

            if (temp_goods.m_7DayTbcDailyRewardDate != val.m_7DayTbcDailyRewardDate)
                param_upd.Add("m_7DayTbcDailyRewardDate", val.m_7DayTbcDailyRewardDate);

            if (temp_goods.m_7DayTbcStartDate != val.m_7DayTbcStartDate)
                param_upd.Add("m_7DayTbcStartDate", val.m_7DayTbcStartDate);

            if (temp_goods.m_7DayTbcEndDate != val.m_7DayTbcEndDate)
                param_upd.Add("m_7DayTbcEndDate", val.m_7DayTbcEndDate);

            if (temp_goods.m_fr_ticket_dg_top != val.m_fr_ticket_dg_top)   
                param_upd.Add("m_fr_ticket_dg_top", val.m_fr_ticket_dg_top);

            if (temp_goods.m_fr_ticket_dg_mine != val.m_fr_ticket_dg_mine) 
                param_upd.Add("m_fr_ticket_dg_mine", val.m_fr_ticket_dg_mine);

            if (temp_goods.m_fr_ticket_dg_raid != val.m_fr_ticket_dg_raid)
                param_upd.Add("m_fr_ticket_dg_raid", val.m_fr_ticket_dg_raid);

            if (temp_goods.m_fr_ticket_pvp_arena != val.m_fr_ticket_pvp_arena)
                param_upd.Add("m_fr_ticket_pvp_arena", val.m_fr_ticket_pvp_arena);

            if (param_upd.GetValue().Count > 0)
            {
                LogPrint.Print("SetUpdateGoods goods param : " + param_upd.GetJson());
                bool _task = await BackendGpgsMng.GetInstance().TaskUpdateTableData("Goods", val.indate, param_upd);
                if (_task)
                {
                    _goods = val;
                    MainUI.GetInstance().topUI.SetGoodsView();
                }
            }

            return true;
        }

        /// <summary>드랍 골드 </summary>
        public void DropGold(int drpGold, float bnsRate)
        {
            int bns_gold = Mathf.RoundToInt(drpGold * (bnsRate * 0.01f));
            PopUpMng.GetInstance().Open_DropGold(drpGold, bns_gold);
            GetInstance().tableDB.SetUpdateGoods("gold", drpGold + bns_gold, "+");
        }
        #endregion

        // ########################################################################################################
        // ################################ UserInfo <유저 정보> ##################################################
        // ########################################################################################################
#region ######## UserInfo <유저 정보> ##################################################
        [System.Serializable]
        public struct UserInfo
        {
            public string indate;
            public long cheat_gotch_bitch;
            public string m_access_ymd;
            public string m_game_end_date; // 게임 종료 Date 

            public int m_attend_nbr; // 출석 카운트 
            public string m_attend_ymd; // 출석 날짜, ex) 2020-09-08

            public int m_char_lv;  // 메인 캐릭터 레벨 

            public ObscuredInt m_chpt_stg_nbr;   // 진행 챕터 ex) 12 : 1-2 챕터, 30 : 3-0챕터 
            public ObscuredInt m_clar_chpt; // 클리어 챕터 

            public ObscuredInt m_eq_lgnd_upgrd_rt7_p; // 전설 장비 진화 실패 보너스 
            public ObscuredInt m_ac_synt_rt6_p; // 장신구 고대 합성시 실패 보너스 성공률 5%, 15% 씩 
            public ObscuredInt m_ac_advc_rt7_p; // 장신구 고대 합성시 실패 보너스 성공률 5%, 15% 씩 
            public ObscuredInt m_ac_synt_fail_cnt; // 장신구 영웅 -> 고대, 합성 실패 횟수 
            public ObscuredInt m_ac_sync_succ_cnt; // 장신구 영웅 -> 고대, 합성 성공 횟수 
            public ObscuredInt m_ac_advc_fail_cnt; // 장신구 고대 -> 전설, 전설 승급 실패 횟수 
            public ObscuredInt m_ac_advc_succ_cnt; // 장신구 고대 -> 전설, 전설 승급 성공 횟수 
            public ObscuredInt m_invn_lv; // 인벤토리 레벨 

          

            public int m_auto_eq_sale_permanent;   // 장비 자동 판매/분해 영구 구매(true), 미구매(false) 
            public string m_auto_eq_sale_date;     // 장비 자동 판매/분해 구매 (구매할때마다 AddDay(1 or 7 or 30)

            public ObscuredInt m_auto_eq_sale_video_lv;  // 장비 자동 판매/분해 광고레벨 (max5) 
            public string m_auto_eq_sale_video_date; // 장비 자동 판매/분해 광고(광고 플레이 할때마다 AddHour(?) 

            public ObscuredInt m_dg_top_nbr_ret; // 도전의 탑 리셋 
            public ObscuredInt m_dg_top_nbr;  // 도전의 탑 최대 클리어 넘버 
            public ObscuredInt m_dg_mine_nbr; // 광산 최대 클리어 넘버 
            public ObscuredInt m_dg_raid_nbr; // 레이드 최대 클리어 넘버 

            // 던전 티켓 일일 구매한 수량
            public ObscuredInt m_daily_buy_ticket_dg_top;
            public ObscuredInt m_daily_buy_ticket_dg_mine;
            public ObscuredInt m_daily_buy_ticket_dg_raid;
            public ObscuredInt m_daily_buy_ticket_dg_pvp;

            // 아이템 일일 구매한 수량 
            public ObscuredInt m_daily_buy_eq_ehnt_ston_rt1; // 장비 강화석 
            public ObscuredInt m_daily_buy_eq_ehnt_ston_rt2;
            public ObscuredInt m_daily_buy_eq_ehnt_ston_rt3;
            public ObscuredInt m_daily_buy_ac_ehnt_ston_rt1; // 장신구 강화석 
            public ObscuredInt m_daily_buy_ac_ehnt_ston_rt2;
            public ObscuredInt m_daily_buy_ac_ehnt_ston_rt3;
            public ObscuredInt m_daily_buy_ehnt_bless_rt1; // 강화 축복 주문서 
            public ObscuredInt m_daily_buy_ehnt_bless_rt2;
            public ObscuredInt m_daily_buy_ehnt_bless_rt3;

            public ObscuredInt m_pet_synt_bns_pct; // 펫 보너스 성공률 
            public ObscuredInt m_pet_opch_red_cnt; // 펫 레드다야 옵션 변경 횟수 
            public ObscuredInt m_pet_opch_blue_cnt; // 펫 블루다야 옵션 변경 횟수 
            public ObscuredInt m_pet_synt_red_cnt; // 펫 레드다야 조합 횟수 
            public ObscuredInt m_pet_synt_blue_cnt; // 펫 블루다야 조합 횟수 

            public int acheive_pet_rt3;
            public int acheive_pet_rt4;
            public int acheive_pet_rt5;
            public int acheive_pet_rt6;
            public int acheive_pet_rt7;

            // 광고 제거
            public ObscuredInt m_ad_removal;

            public string m_ad_ruby_date, m_ad_ether_date;

            public string m_free_acce_sohwan; // 무료 다이아 장신구 소환 
            public string m_free_equip_sohwan; // 무료 다이아 장비 소환 
            public string m_free_pet_sohwan; // 무료 펫 소환 
            public string m_free_pet_sohwan_redate; // 무료 펫 소환 

            public bool m_pvp_in; // 입장할때 true로 변경, 종료시 false로 변경, 이후 게임 접속시 true라면 강종으로 인식하고 점수를 -30점 차감 
            public int m_pvp_win_streak; // pvp 연승 횟수 
            public ObscuredInt m_pvp_score;
            public ObscuredInt m_pvp_today_score; // pvp 일일 점수 (pvp 일일 랭킹)
            public int m_combat_score;

            public bool GetAdRemoval() => m_ad_removal > 0;

            public int GetChapterStageNbr() => m_chpt_stg_nbr;
            /// <summary> 챕터 -> m_chpt_stg_nbr값이 15일 경우 현재 챕터는 1으로 반환 </summary>
            public int GetChapterDvsNbr() => (int)Math.Floor((float)m_chpt_stg_nbr / 10.0f);
            public int GetRankChapterNbr(int val) => (int)Math.Floor((float)val / 10.0f);
            /// <summary> 챕터의 스테이지, 1~10 </summary>
            public int GetStageNbr() => (m_chpt_stg_nbr % 10) + 1;
            public int GetDgNbr(IG.ModeType dgMdTy)
            {
                if (object.Equals(dgMdTy, IG.ModeType.DUNGEON_TOP))
                    return m_dg_top_nbr;
                else if (object.Equals(dgMdTy, IG.ModeType.DUNGEON_MINE))
                    return m_dg_mine_nbr;
                else if (object.Equals(dgMdTy, IG.ModeType.DUNGEON_RAID))
                    return m_dg_raid_nbr;
                return 0;
            }
        }
        private UserInfo _userInfo = new UserInfo();

        public void UserInfo_RandomizeCryptoKey()
        {
            _userInfo.m_chpt_stg_nbr.RandomizeCryptoKey();
            _userInfo.m_clar_chpt.RandomizeCryptoKey();
            _userInfo.m_eq_lgnd_upgrd_rt7_p.RandomizeCryptoKey();
            _userInfo.m_ac_synt_rt6_p.RandomizeCryptoKey();
            _userInfo.m_ac_advc_rt7_p.RandomizeCryptoKey();
            _userInfo.m_ac_synt_fail_cnt.RandomizeCryptoKey();
            _userInfo.m_ac_sync_succ_cnt.RandomizeCryptoKey();
            _userInfo.m_ac_advc_fail_cnt.RandomizeCryptoKey();
            _userInfo.m_ac_advc_succ_cnt.RandomizeCryptoKey();
            _userInfo.m_pet_synt_bns_pct.RandomizeCryptoKey();

            _userInfo.m_invn_lv.RandomizeCryptoKey();
            _userInfo.m_auto_eq_sale_video_lv.RandomizeCryptoKey();
            _userInfo.m_dg_top_nbr_ret.RandomizeCryptoKey();
            _userInfo.m_dg_top_nbr.RandomizeCryptoKey();
            _userInfo.m_dg_mine_nbr.RandomizeCryptoKey();
            _userInfo.m_dg_raid_nbr.RandomizeCryptoKey();
            _userInfo.m_daily_buy_ticket_dg_top.RandomizeCryptoKey();
            _userInfo.m_daily_buy_ticket_dg_mine.RandomizeCryptoKey();
            _userInfo.m_daily_buy_ticket_dg_raid.RandomizeCryptoKey();
            _userInfo.m_daily_buy_ticket_dg_pvp.RandomizeCryptoKey();
            _userInfo.m_daily_buy_eq_ehnt_ston_rt1.RandomizeCryptoKey();
            _userInfo.m_daily_buy_eq_ehnt_ston_rt2.RandomizeCryptoKey();
            _userInfo.m_daily_buy_eq_ehnt_ston_rt3.RandomizeCryptoKey();
            _userInfo.m_daily_buy_ac_ehnt_ston_rt1.RandomizeCryptoKey();
            _userInfo.m_daily_buy_ac_ehnt_ston_rt2.RandomizeCryptoKey();
            _userInfo.m_daily_buy_ac_ehnt_ston_rt3.RandomizeCryptoKey();
            _userInfo.m_daily_buy_ehnt_bless_rt1.RandomizeCryptoKey();
            _userInfo.m_daily_buy_ehnt_bless_rt2.RandomizeCryptoKey();
            _userInfo.m_daily_buy_ehnt_bless_rt3.RandomizeCryptoKey();
            _userInfo.m_ad_removal.RandomizeCryptoKey();
            _userInfo.m_pvp_score.RandomizeCryptoKey();
            _userInfo.m_pvp_today_score.RandomizeCryptoKey();
        }

        /// <summary>
        /// 자신의 챕터 스테이지가 차트의 최대 값 보다 넘지 못하도록 
        /// </summary>
        public void InitChapterNbr()
        {
            int v = _userInfo.GetChapterDvsNbr();
            int indx = list_cdb_chpt_mnst_stat.FindIndex(x => x.chpt_dvs_nbr == v);
            if (indx == -1)
            {
                int last_chpt_indx = list_cdb_chpt_mnst_stat.Count - 1;
                _userInfo.m_chpt_stg_nbr = (int)(list_cdb_chpt_mnst_stat[last_chpt_indx].chpt_dvs_nbr * 10);
            }
        }
       
        public void SetInitTableDB_UserInfo(UserInfo val) => _userInfo = val;

        public UserInfo GetUserInfo() => _userInfo;
        /// <summary> 스테이지 1증가 </summary>
        public void SetStageIncrease()
        {
            bool isStageBossMnst = GameMng.GetInstance().stage_type == IG.StageType.BOSS_MONSTER;
            if (GameMng.GetInstance().mode_type == IG.ModeType.CHAPTER_CONTINUE)
            {
                //_userInfo.m_chpt_stg_nbr += isStageBossMnst == true ? 10 : 1;
                if (isStageBossMnst == false)
                    _userInfo.m_chpt_stg_nbr++;
                else
                {
                    int v = GameDatabase.GetInstance().tableDB.GetUserInfo().GetRankChapterNbr((int)_userInfo.m_chpt_stg_nbr + 10);
                    int indx = list_cdb_chpt_mnst_stat.FindIndex(x => x.chpt_dvs_nbr == v);
                    if (indx >= 0)
                    {
                        _userInfo.m_chpt_stg_nbr = (int)(list_cdb_chpt_mnst_stat[indx].chpt_dvs_nbr * 10);
                    }
                }
            }
            else if (GameMng.GetInstance().mode_type == IG.ModeType.CHAPTER_LOOP)
            {
                int lp_stg_nbr = GameMng.GetInstance().loopChapter.chpt_stg_nbr;
                if (lp_stg_nbr >= GameMng.GetInstance().loopChapter.chpt_stg_nbr_max)
                {
                    lp_stg_nbr = GameMng.GetInstance().loopChapter.chpt_stg_nbr_min;
                }
                else
                {
                    lp_stg_nbr += isStageBossMnst == true ? 10 : 1;
                }

                GameMng.GetInstance().loopChapter.chpt_stg_nbr = lp_stg_nbr;
            }
        }

        /// <summary> 
        /// 스테이지 보스 (10/10) 보스 처치 실패시 : 뒤로 or 점프 
        /// 반복모드는 뒤로 가지 않음 이어서 계속 
        /// </summary>
        public void ChapterLoseNextProgress(bool isBoss)
        {
            if (GameMng.GetInstance().mode_type == IG.ModeType.CHAPTER_CONTINUE) // 진행 : 뒤로 
            {
                _userInfo.m_chpt_stg_nbr = _userInfo.GetChapterDvsNbr() * 10;
            }
            else if (GameMng.GetInstance().mode_type == IG.ModeType.CHAPTER_LOOP) // 반복 : 점프 (보스 점프)
            {
                if(isBoss)
                    GetInstance().tableDB.SetStageIncrease();
            }
        }

        public Param ParamAddUserInfo(UserInfo val)
        {
            Param pam = new Param();
            // 접속 날짜 
            if (!string.Equals(_userInfo.m_access_ymd, val.m_access_ymd))
                pam.Add("m_access_ymd", val.m_access_ymd);

            // 종료 시간 
            pam.Add("m_game_end_date", BackendGpgsMng.GetInstance().GetNowTime().ToString());

            // 출석 관련 
            if (!string.Equals(_userInfo.m_attend_ymd, val.m_attend_ymd))
                pam.Add("m_attend_ymd", val.m_attend_ymd);
            if (!int.Equals(_userInfo.m_attend_nbr, val.m_attend_nbr))
                pam.Add("m_attend_nbr", val.m_attend_nbr);
            
            if (_userInfo.cheat_gotch_bitch != val.cheat_gotch_bitch)
                pam.Add("cheat_gotch_bitch", val.cheat_gotch_bitch);

            if (_userInfo.m_eq_lgnd_upgrd_rt7_p != val.m_eq_lgnd_upgrd_rt7_p)
                pam.Add("m_eq_lgnd_upgrd_rt7_p", val.m_eq_lgnd_upgrd_rt7_p);

            if (_userInfo.m_ac_synt_rt6_p != val.m_ac_synt_rt6_p)
                pam.Add("m_ac_synt_rt6_p", val.m_ac_synt_rt6_p);

            if (_userInfo.m_ac_advc_rt7_p != val.m_ac_advc_rt7_p)
                pam.Add("m_ac_advc_rt7_p", val.m_ac_advc_rt7_p);

            if (_userInfo.m_ac_sync_succ_cnt != val.m_ac_sync_succ_cnt) 
                pam.Add("m_ac_sync_succ_cnt", val.m_ac_sync_succ_cnt);
            if (_userInfo.m_ac_synt_fail_cnt != val.m_ac_synt_fail_cnt) 
                pam.Add("m_ac_synt_fail_cnt", val.m_ac_synt_fail_cnt);

            if (_userInfo.m_ac_advc_succ_cnt != val.m_ac_advc_succ_cnt)
                pam.Add("m_ac_advc_succ_cnt", val.m_ac_advc_succ_cnt);
            if (_userInfo.m_ac_advc_fail_cnt != val.m_ac_advc_fail_cnt)
                pam.Add("m_ac_advc_fail_cnt", val.m_ac_advc_fail_cnt);

            if (_userInfo.m_pet_synt_bns_pct != val.m_pet_synt_bns_pct)
                pam.Add("m_pet_synt_bns_pct", val.m_pet_synt_bns_pct);

            if (_userInfo.m_pet_opch_red_cnt != val.m_pet_opch_red_cnt)
                pam.Add("m_pet_opch_red_cnt", val.m_pet_opch_red_cnt);

            if (_userInfo.m_pet_opch_blue_cnt != val.m_pet_opch_blue_cnt)
                pam.Add("m_pet_opch_blue_cnt", val.m_pet_opch_blue_cnt);

            if (_userInfo.m_pet_synt_red_cnt != val.m_pet_synt_red_cnt)
                pam.Add("m_pet_synt_red_cnt", val.m_pet_synt_red_cnt);

            if (_userInfo.m_pet_synt_blue_cnt != val.m_pet_synt_blue_cnt)
                pam.Add("m_pet_synt_blue_cnt", val.m_pet_synt_blue_cnt);


            if (_userInfo.m_invn_lv != val.m_invn_lv)                   
                pam.Add("m_invn_lv", val.m_invn_lv);
            if (_userInfo.m_char_lv != val.m_char_lv)                   
                pam.Add("m_char_lv", val.m_char_lv);

            // 던전 
            if (_userInfo.m_dg_top_nbr_ret != val.m_dg_top_nbr_ret)
                pam.Add("m_dg_top_nbr_ret", val.m_dg_top_nbr_ret);
            if (_userInfo.m_dg_top_nbr != val.m_dg_top_nbr)             
                pam.Add("m_dg_top_nbr", val.m_dg_top_nbr);
            if (_userInfo.m_dg_mine_nbr != val.m_dg_mine_nbr)           
                pam.Add("m_dg_mine_nbr", val.m_dg_mine_nbr);
            if (_userInfo.m_dg_raid_nbr != val.m_dg_raid_nbr)           
                pam.Add("m_dg_raid_nbr", val.m_dg_raid_nbr);

            // 던전 티켓 일일 구매 수량 
            if (_userInfo.m_daily_buy_ticket_dg_top != val.m_daily_buy_ticket_dg_top)
                pam.Add("m_daily_buy_ticket_dg_top", val.m_daily_buy_ticket_dg_top);
            if (_userInfo.m_daily_buy_ticket_dg_mine != val.m_daily_buy_ticket_dg_mine)
                pam.Add("m_daily_buy_ticket_dg_mine", val.m_daily_buy_ticket_dg_mine);
            if (_userInfo.m_daily_buy_ticket_dg_raid != val.m_daily_buy_ticket_dg_raid)
                pam.Add("m_daily_buy_ticket_dg_raid", val.m_daily_buy_ticket_dg_raid);
            if (_userInfo.m_daily_buy_ticket_dg_pvp != val.m_daily_buy_ticket_dg_pvp)
                pam.Add("m_daily_buy_ticket_dg_pvp", val.m_daily_buy_ticket_dg_pvp);

            // 아이템 일일 구매 수량 
            // 장비 강화석 
            if (_userInfo.m_daily_buy_eq_ehnt_ston_rt1 != val.m_daily_buy_eq_ehnt_ston_rt1)
                pam.Add("m_daily_buy_eq_ehnt_ston_rt1", val.m_daily_buy_eq_ehnt_ston_rt1);
            if (_userInfo.m_daily_buy_eq_ehnt_ston_rt2 != val.m_daily_buy_eq_ehnt_ston_rt2)
                pam.Add("m_daily_buy_eq_ehnt_ston_rt2", val.m_daily_buy_eq_ehnt_ston_rt2);
            if (_userInfo.m_daily_buy_eq_ehnt_ston_rt3 != val.m_daily_buy_eq_ehnt_ston_rt3)
                pam.Add("m_daily_buy_eq_ehnt_ston_rt3", val.m_daily_buy_eq_ehnt_ston_rt3);
            // 장신구 강화석 
            if (_userInfo.m_daily_buy_ac_ehnt_ston_rt1 != val.m_daily_buy_ac_ehnt_ston_rt1)
                pam.Add("m_daily_buy_ac_ehnt_ston_rt1", val.m_daily_buy_ac_ehnt_ston_rt1);
            if (_userInfo.m_daily_buy_ac_ehnt_ston_rt2 != val.m_daily_buy_ac_ehnt_ston_rt2)
                pam.Add("m_daily_buy_ac_ehnt_ston_rt2", val.m_daily_buy_ac_ehnt_ston_rt2);
            if (_userInfo.m_daily_buy_ac_ehnt_ston_rt3 != val.m_daily_buy_ac_ehnt_ston_rt3)
                pam.Add("m_daily_buy_ac_ehnt_ston_rt3", val.m_daily_buy_ac_ehnt_ston_rt3);
            // 강화 축복 주문서 
            if (_userInfo.m_daily_buy_ehnt_bless_rt1 != val.m_daily_buy_ehnt_bless_rt1)
                pam.Add("m_daily_buy_ehnt_bless_rt1", val.m_daily_buy_ehnt_bless_rt1);
            if (_userInfo.m_daily_buy_ehnt_bless_rt2 != val.m_daily_buy_ehnt_bless_rt2)
                pam.Add("m_daily_buy_ehnt_bless_rt2", val.m_daily_buy_ehnt_bless_rt2);
            if (_userInfo.m_daily_buy_ehnt_bless_rt3 != val.m_daily_buy_ehnt_bless_rt3)
                pam.Add("m_daily_buy_ehnt_bless_rt3", val.m_daily_buy_ehnt_bless_rt3);

            // 자동 판매 (결제) 
            if (_userInfo.m_auto_eq_sale_permanent != val.m_auto_eq_sale_permanent)  // 영구 구매 
                pam.Add("m_auto_eq_sale_permanent", val.m_auto_eq_sale_permanent);

            if (_userInfo.m_auto_eq_sale_date != val.m_auto_eq_sale_date)           
                pam.Add("m_auto_eq_sale_date", val.m_auto_eq_sale_date);

            // 자동 판매 (광고) 
            if (_userInfo.m_auto_eq_sale_video_lv != val.m_auto_eq_sale_video_lv)
                pam.Add("m_auto_eq_sale_video_lv", val.m_auto_eq_sale_video_lv);

            if (_userInfo.m_auto_eq_sale_video_date != val.m_auto_eq_sale_video_date)
                pam.Add("m_auto_eq_sale_video_date", val.m_auto_eq_sale_video_date);

            // 장신구 무료 소환 
            if (_userInfo.m_free_acce_sohwan != val.m_free_acce_sohwan)
                pam.Add("m_free_acce_sohwan", val.m_free_acce_sohwan);

            if (_userInfo.m_free_equip_sohwan != val.m_free_equip_sohwan)
                pam.Add("m_free_equip_sohwan", val.m_free_equip_sohwan);

            if (_userInfo.m_free_pet_sohwan != val.m_free_pet_sohwan)
                pam.Add("m_free_pet_sohwan", val.m_free_pet_sohwan);

            if (_userInfo.m_free_pet_sohwan_redate != val.m_free_pet_sohwan_redate)
                pam.Add("m_free_pet_sohwan_redate", val.m_free_pet_sohwan_redate);

            if (_userInfo.m_pvp_in != val.m_pvp_in)       // pvp 랭킹        
                pam.Add("m_pvp_in", val.m_pvp_in);

            if (_userInfo.m_pvp_win_streak != val.m_pvp_win_streak)       // pvp 연승         
                pam.Add("m_pvp_win_streak", val.m_pvp_win_streak);

            if (_userInfo.m_ad_removal != val.m_ad_removal) // 광고 제거          
                pam.Add("m_ad_removal", val.m_ad_removal);

            if (_userInfo.m_ad_ruby_date != val.m_ad_ruby_date) // 광고 루비           
                pam.Add("m_ad_ruby_date", val.m_ad_ruby_date);

            if (_userInfo.m_ad_ether_date != val.m_ad_ether_date) // 광고 에테르 
                pam.Add("m_ad_ether_date", val.m_ad_ether_date);

            if (_userInfo.acheive_pet_rt3 != val.acheive_pet_rt3)
                pam.Add("acheive_pet_rt3", val.acheive_pet_rt3);
            if (_userInfo.acheive_pet_rt4 != val.acheive_pet_rt4)
                pam.Add("acheive_pet_rt4", val.acheive_pet_rt4);
            if (_userInfo.acheive_pet_rt5 != val.acheive_pet_rt5)
                pam.Add("acheive_pet_rt5", val.acheive_pet_rt5);
            if (_userInfo.acheive_pet_rt6 != val.acheive_pet_rt6)
                pam.Add("acheive_pet_rt6", val.acheive_pet_rt6);
            if (_userInfo.acheive_pet_rt7 != val.acheive_pet_rt7)
                pam.Add("acheive_pet_rt7", val.acheive_pet_rt7);

            // 누적 pvp 점수 
            //if (_userInfo.m_pvp_score != val.m_pvp_score)       // pvp 점수 -> 실시간 랭킹에서 자동으로 변경됨         
            //    pam.Add("m_pvp_score", val.m_pvp_score);
            // 일일 pvp 점수  
            //if (_userInfo.m_pvp_today_score != val.m_pvp_today_score)       // pvp 점수 -> 실시간 랭킹에서 자동으로 변경됨         
            //    pam.Add("m_pvp_today_score", val.m_pvp_today_score);

            if (_userInfo.m_combat_score != val.m_combat_score) // 전투력 랭킹     
                pam.Add("m_combat_score", val.m_combat_score);

            if (_userInfo.m_chpt_stg_nbr != val.m_chpt_stg_nbr) // 스테이지 랭킹, 현재 진행중인 챕터 스테이지 
                pam.Add("m_chpt_stg_nbr", val.m_chpt_stg_nbr);
          
            // 랭킹(리스트는 안보여주고, 챕터 보스에 사용됨)
            if (_userInfo.m_clar_chpt != val.m_clar_chpt)  // 마지막으로 클리어한 챕터  
                pam.Add("m_clar_chpt", val.m_clar_chpt);

            return pam;
        }

        /// <summary>
        /// 유저 정보 데이터 업데이트  
        /// </summary>
        public async Task<bool> SetUpdate_UserInfo(UserInfo val, bool isOnlyCliend = false)
        {
            Param pam = ParamAddUserInfo(val);
            LogPrint.Print("기존 데이터를 Update한다. pam.GetValue().Count : " + pam.GetValue().Count);
            if (pam.GetValue().Count > 0 && isOnlyCliend == false)
            {
                await BackendGpgsMng.GetInstance().TaskUpdateTableData(BackendGpgsMng.tableName_UserInfo, val.indate, pam);
               
                bool isRfsh_InvnNbr = !int.Equals(_userInfo.m_invn_lv, val.m_invn_lv); // 데이터 새로고침 하기 전에 인벤 레벨에 변화가 있는지 체크 
                _userInfo = val;
                if (isRfsh_InvnNbr)
                {
                    MainUI.GetInstance().InventoryItemTotalCountRefresh();
                }
            }
            else
            {
                _userInfo = val;
            }

            return true;
        }

        public void UpdateClientDB(UserInfo val) => _userInfo = val;

        /// <summary>
        /// 일일 리셋 정보 
        /// </summary>
        public async Task<bool> ResetDailyUserInfo(string ymd)
        {
            var userinfo_db = GetInstance().tableDB.GetUserInfo();

            // 접속 날짜 
            userinfo_db.m_access_ymd = ymd;

            // 던전 입장권 일일 구매량 리셋 
            userinfo_db.m_daily_buy_ticket_dg_top = 0;
            userinfo_db.m_daily_buy_ticket_dg_mine = 0;
            userinfo_db.m_daily_buy_ticket_dg_raid = 0;
            userinfo_db.m_daily_buy_ticket_dg_pvp = 0;

            // 강화석 일일 구매량 리셋 
            userinfo_db.m_daily_buy_eq_ehnt_ston_rt1 = 0;
            userinfo_db.m_daily_buy_eq_ehnt_ston_rt2 = 0;
            userinfo_db.m_daily_buy_eq_ehnt_ston_rt3 = 0;
            userinfo_db.m_daily_buy_ac_ehnt_ston_rt1 = 0;
            userinfo_db.m_daily_buy_ac_ehnt_ston_rt2 = 0;
            userinfo_db.m_daily_buy_ac_ehnt_ston_rt3 = 0;

            // 강화 축복 일일 구매량 리셋 
            userinfo_db.m_daily_buy_ehnt_bless_rt1 = 0;
            userinfo_db.m_daily_buy_ehnt_bless_rt2 = 0;
            userinfo_db.m_daily_buy_ehnt_bless_rt3 = 0;

            var db_goods = GetInstance().tableDB.GetTableDB_Goods();
            db_goods.m_fr_ticket_dg_top = 3;
            db_goods.m_fr_ticket_dg_mine = 3;
            db_goods.m_fr_ticket_dg_raid = 3;
            db_goods.m_fr_ticket_pvp_arena = 5;

            await GetInstance().tableDB.SetUpdateGoods(db_goods);
            return await GetInstance().tableDB.SetUpdate_UserInfo(userinfo_db);
        }
        #endregion

        // ########################################################################################################
        // ################################ Backend Transaction 장비,아이템,스킬 ##################################
        // ########################################################################################################
#region ######## Backend Transaction 장비,아이템,스킬 ##################################
        //public struct TransactionParams
        //{
        //    public string tb_name;
        //    public string row_indate;
        //    public Param param;
        //}

        //protected List<TransactionParams> transaction_item = new List<TransactionParams>();
        //protected List<TransactionParams> transaction_skill = new List<TransactionParams>();

        ///// <summary> [Add] 게임 정보 트랜잭션 보관 : 트랜잭션은 각 읽기 작업 최대 5개와 쓰기 작업 최대 5개를 한 번에 모두 수행 </summary>
        //public void AddTransaction(string tname, string idte, Param p)
        //{
        //    if (string.Equals(tname, BackendGpgsMng.tableName_Item))
        //    {
        //        int indx = transaction_item.FindIndex((TransactionParams tp) => tp.row_indate == idte && tp.row_indate != "delete");
        //        if (indx >= 0)
        //        {
        //            var data_ovrlp = transaction_item[indx];
        //            data_ovrlp.row_indate = "delete";
        //            transaction_item[indx] = data_ovrlp;
        //        }

        //        LogPrint.PrintError("Add  Item tb_name : " + tname + ", row_indate : " + idte + ", param : " + p.GetJson());
        //        transaction_item.Add(new TransactionParams() { tb_name = tname, row_indate = idte, param = p });
        //        UpdateTransaction_Item();
        //    }
        //    else if (string.Equals(tname, BackendGpgsMng.tableName_Skill))
        //    {
        //        int indx = transaction_skill.FindIndex((TransactionParams tp) => tp.row_indate == idte && tp.row_indate != "delete");
        //        if (indx >= 0)
        //        {
        //            var data_ovrlp = transaction_skill[indx];
        //            data_ovrlp.row_indate = "delete";
        //            transaction_skill[indx] = data_ovrlp;
        //        }

        //        LogPrint.PrintError("Add  skill tb_name : " + tname + ", row_indate : " + idte + ", param : " + p.GetJson());
        //        transaction_skill.Add(new TransactionParams() { tb_name = tname, row_indate = idte, param = p });
        //        UpdateTransaction_Skill();
        //    }
        //}

        ///// <summary> [Item]게임 정보 트랜잭션 Update : 트랜잭션은 각 읽기 작업 최대 5개와 쓰기 작업 최대 5개를 한 번에 모두 수행 </summary>
        //public async void UpdateTransaction_Item(bool isAuto = false) // isAuto true : 5분? 자동저장일때, false : 획득시 수량 체크 후 세이브 
        //{
        //    if (isAuto)
        //    {
        //        int trnsc_pam_cnt = transaction_item.Count;
        //        int forCnt = isAuto == false ?
        //            trnsc_pam_cnt >= 5 ? 5 : -1 :
        //            trnsc_pam_cnt >= 5 ? 5 : trnsc_pam_cnt;

        //        LogPrint.PrintError(" item -0- : " + transaction_item.Count + ", forCnt : " + forCnt);
        //        if (forCnt == -1)
        //            return;

        //        TransactionParam tParam = new TransactionParam();
        //        for (int i = 0; i < forCnt; i++)
        //        {
        //            var tr_item = transaction_item[i];
        //            if (string.Equals(tr_item.row_indate, "delete"))
        //            {
        //                LogPrint.PrintError("error tr_item.row_indate : " + tr_item.row_indate);
        //            }
        //            else
        //            {
        //                LogPrint.PrintError("ok tr_item.row_indate : " + tr_item.row_indate + ", param : " + tr_item.param.GetJson());
        //                List<BackEnd.WRITE> writes = new List<BackEnd.WRITE> { new WRITE { Action = TransactionAction.Update, Param = tr_item.param } };
        //                tParam.AddUpdateList(tr_item.tb_name, tr_item.row_indate, writes);
        //            }
        //        }

        //        BackendReturnObject _bro = new BackendReturnObject();
        //        Task _task = BackendAsync(Backend.GameInfo.TransactionWrite, tParam, callback => { _bro = callback; });
        //        await _task;
        //        if (_bro.IsSuccess())
        //        {
        //            //for (int i = 0; i < forCnt; i++)
        //            //transaction_item.RemoveAt(0);
        //            transaction_item.RemoveRange(0, forCnt);
        //        }
        //        LogPrint.PrintError(" -2- <color=yellow>완료 item 남은 수량 </color> : " + transaction_item.Count + ", 서버에 보낸 갯수 : " + forCnt + ", IsSuccess : " + _bro.IsSuccess() + ", GetReturnValue : " + _bro.GetReturnValue());
        //    }

        //    if (isAuto && transaction_item.Count > 0)
        //    {
        //        UpdateTransaction_Item(true);
        //    }
        //}

        ///// <summary> [Skill] 게임 정보 트랜잭션 Update : 트랜잭션은 각 읽기 작업 최대 5개와 쓰기 작업 최대 5개를 한 번에 모두 수행 </summary>
        //public async void UpdateTransaction_Skill(bool isAuto = false)
        //{
        //    if (isAuto)
        //    {
        //        int trnsc_pam_cnt = transaction_skill.Count;
        //        int forCnt = isAuto == false ?
        //            trnsc_pam_cnt >= 5 ? 5 : -1 :
        //            trnsc_pam_cnt >= 5 ? 5 : trnsc_pam_cnt;

        //        LogPrint.PrintError(" skill -0- : " + transaction_skill.Count + ", forCnt : " + forCnt);
        //        if (forCnt == -1)
        //            return;

        //        TransactionParam tParam = new TransactionParam();
        //        for (int i = 0; i < forCnt; i++)
        //        {
        //            var tr_skill = transaction_skill[i];
        //            if (string.Equals(tr_skill.row_indate, "delete"))
        //            {
        //                LogPrint.PrintError("error tr_skill.row_indate : " + tr_skill.row_indate);
        //            }
        //            else
        //            {
        //                LogPrint.PrintError("ok tr_skill.row_indate : " + tr_skill.row_indate + ", param : " + tr_skill.param.GetJson());
        //                List<BackEnd.WRITE> writes = new List<BackEnd.WRITE> { new WRITE { Action = TransactionAction.Update, Param = tr_skill.param } };
        //                tParam.AddUpdateList(tr_skill.tb_name, tr_skill.row_indate, writes);
        //            }
        //        }

        //        BackendReturnObject _bro = new BackendReturnObject();
        //        Task _task = BackendAsync(Backend.GameInfo.TransactionWrite, tParam, callback => { _bro = callback; });
        //        await _task;
        //        if (_bro.IsSuccess())
        //        {
        //            //for (int i = 0; i < forCnt; i++)
        //            //    transaction_skill.RemoveAt(0);
        //            transaction_skill.RemoveRange(0, forCnt);
        //        }
        //        LogPrint.PrintError(" -2- <color=yellow>완료 skill 남은 수량  </color>  : " + transaction_skill.Count + ", 서버에 보낸 갯수 : " + forCnt + ", IsSuccess : " + _bro.IsSuccess() + ", GetReturnValue : " + _bro.GetReturnValue());
        //    }

        //    if (isAuto && transaction_skill.Count > 0)
        //    {
        //        UpdateTransaction_Skill(true);
        //    }
        //}
        #endregion

        // ########################################################################################################
        // ################################ 장비,아이템,스킬 공용 #################################################
        // ########################################################################################################
#region ######## 장비,아이템,스킬 공용 #################################################
        /// <summary> 장비, 아이템, 스킬 획득 </summary>
        public async void DropRewardAcquire(int chpt_id, int drop_rt, float drop_accum_pct)
        {
            bool isRefInven = false;
            // 장비 드롭 
            var chrt_eq = GetInstance().chartDB.GetChapterDropEquipResult(chpt_id, drop_rt);
            if (chrt_eq != null)
            {
                float accum_pct = 0f;
                float r_pct = GetInstance().GetRandomPercent(drop_accum_pct);
                foreach (var cht_db in chrt_eq)
                {
                    if (cht_db.chpt_pct > 0)
                    {
                        accum_pct += cht_db.chpt_pct;
                        if (r_pct < accum_pct)
                        {
                            //if(cht_db.ty >= 8 && cht_db.ty <= 10 && cht_db.rt >= 5)
                            //{
                            //    LogPrint.EditorPrint("<color=yellow>DropRewardAcquire r_pct : " + r_pct + ", accum_pct : " + accum_pct + ", cht_db : " + JsonUtility.ToJson(cht_db) + "</color>");
                            //}
                            
                            if(cht_db.ty >= 0 && cht_db.rt > 0 && cht_db.id > 0)
                            {
                                isRefInven = SetAcquireEquip(cht_db);
                            }
                            break;
                        }
                    }
                }
            }

            // 아이템 드롭 
            float pet_sop2_value = GameMng.GetInstance().myPZ != null ? GameMng.GetInstance().myPZ.igp.statValue.petSpOpTotalFigures.sop2_value * 0.01f : 0;// 펫 전용 옵션 : 2.아이템 드랍률 증가 
            float item_drop_rate = GetInstance().chartDB.GetDicBalance("field.drop.item").val_int;
            bool drop_r_equip_item = GetInstance().GetRandomPercent() <= (item_drop_rate + (item_drop_rate * pet_sop2_value));
            if (drop_r_equip_item)
            {
                float accum_pct = 0f;
                float r_pct = GetInstance().GetRandomPercent();
                var chrt_it = GetInstance().chartDB.GetChapterDropItemResult();
                foreach (var cht_db in chrt_it)
                {
                    if (cht_db.pct > 0)
                    {
                        accum_pct += cht_db.pct;
                        if (r_pct < accum_pct)
                        {
                            await GetInstance().tableDB.SetAcquireItem(cht_db.it_ty, cht_db.it_rt, cht_db.it_cn, true);
                            isRefInven = true;
                            break;
                        }
                    }
                }
            }

            if (isRefInven)
            {
                MainUI.GetInstance().NewEquipItemInventortRefresh();
            }
        }
        #endregion

        // ########################################################################################################
        // ################################ Equipment <장비> ######################################################
        // ########################################################################################################
#region ######## Equipment <장비> ######################################################
        public const int equip_OpStatCount = 9; // 장비(장신구포함) 스탯 갯수 
        public const int equipSpecialOpStatCount = 5;
        public const int acce_SpecialOpStatCount = 10; // 장신구 전용 옵션 갯수 
        
        [System.Serializable]
        public struct Equipment
        {            
            public string indate;
            public long aInUid; // 장비 고유 
            /// <summary> -1:판매하였음, 0:보유중(인벤), 1: 1번 슬롯 착용중, 2: 2번 슬롯 착용중, 3: 3번 슬롯 착용중 </summary>
            public int m_state; 
            public int m_lck; // 잠금 상태 
            public int eq_ty; // 파츠 
            public int eq_rt; // 장비 등급 
            public int eq_id; // 장비 번호 
            public int eq_legend; // 전설 배경, 1 : 진화된상태, 0:진화 않된상태 
            public int eq_legend_sop_id;
            public int eq_legend_sop_rlv;

            public int ma_st_id; // 매인 스탯 id (1~9번) m_st_id
            public int ma_st_rlv; // 장비 획득시 부여, 1.Random.Range(1, 11) => ex)5 / 2. min_** + (((max_** - min_**) / 10) * stat_rnd_lv)) = 값 
            public int m_norm_lv; // 일반 레벨 
            public int m_ehnt_lv; // 강화 레벨 

            [SerializeField] public StatOp st_sop_ac;
            [SerializeField] public EquipStatOp st_op;
            [SerializeField] public EquipMagicSocket st_ms;

            public long client_add_sp; // 획득한 TimeSpan 
        }

        [System.Serializable] public struct StatOp
        {
            public int id;
            public int rlv;
        }
        [System.Serializable] public struct EquipStatOp
        {
            public StatOp op1;
            public StatOp op2;
            public StatOp op3;
            public StatOp op4;
        }

        [System.Serializable]
        public struct PetStatOp
        {
            public StatOp op1;
            public StatOp op2;
            public StatOp op3;
            public StatOp op4;
            public StatOp op5;
            public StatOp op6;
            public StatOp op7;
            public StatOp op8;
        }

        [System.Serializable]
        public struct MagicSocket
        {
            public int ms_id;
            public int ms_ty;
        }
        [System.Serializable] public struct EquipMagicSocket
        {
            public MagicSocket ms1;
            public MagicSocket ms2;
            public MagicSocket ms3;
        }

        private static readonly List<Equipment> list = new List<Equipment>();
        protected List<Equipment> _equipment = list;

        private Equipment wearingEquip_weapon    = new Equipment(); // party_type : 0
        private Equipment wearingEquip_shield    = new Equipment(); // party_type : 1
        private Equipment wearingEquip_helmet    = new Equipment(); // party_type : 2
        private Equipment wearingEquip_shoulder  = new Equipment(); // party_type : 3
        private Equipment wearingEquip_armor     = new Equipment(); // party_type : 4
        private Equipment wearingEquip_arm       = new Equipment(); // party_type : 5
        private Equipment wearingEquip_pants     = new Equipment(); // party_type : 6
        private Equipment wearingEquip_boots     = new Equipment(); // party_type : 7
        private Equipment wearingEquip_necklace  = new Equipment(); // party_type : 8
        private Equipment wearingEquip_earring   = new Equipment(); // party_type : 9
        private Equipment wearingEquip_ring      = new Equipment(); // party_type : 10

        // ########################################################################################################
        // ######################### 장비 : New / Get / Set / IS ##################################################
        #region ########################### 장비 : New / Get / Set / IS #########################
        /// <summary> 착용 중인 각 장비 데이터 세팅 </summary>
        public void SetttingEquipWearingData(int oneChangeType = -1)
        {
            int eq_use_slot = GetUseEquipSlot();

            if (oneChangeType == -1 || oneChangeType == 0)
                wearingEquip_weapon = _tableDB.GetFindUseEquipment(0, eq_use_slot);
            if (oneChangeType == -1 || oneChangeType == 1)
                wearingEquip_shield = _tableDB.GetFindUseEquipment(1, eq_use_slot);
            if (oneChangeType == -1 || oneChangeType == 2)
                wearingEquip_helmet = _tableDB.GetFindUseEquipment(2, eq_use_slot);
            if (oneChangeType == -1 || oneChangeType == 3)
                wearingEquip_shoulder = _tableDB.GetFindUseEquipment(3, eq_use_slot);
            if (oneChangeType == -1 || oneChangeType == 4)
                wearingEquip_armor = _tableDB.GetFindUseEquipment(4, eq_use_slot);
            if (oneChangeType == -1 || oneChangeType == 5)
                wearingEquip_arm = _tableDB.GetFindUseEquipment(5, eq_use_slot);
            if (oneChangeType == -1 || oneChangeType == 6)
                wearingEquip_pants = _tableDB.GetFindUseEquipment(6, eq_use_slot);
            if (oneChangeType == -1 || oneChangeType == 7)
                wearingEquip_boots = _tableDB.GetFindUseEquipment(7, eq_use_slot);
            if (oneChangeType == -1 || oneChangeType == 8)
                wearingEquip_necklace = _tableDB.GetFindUseEquipment(8, eq_use_slot);
            if (oneChangeType == -1 || oneChangeType == 9)
                wearingEquip_earring = _tableDB.GetFindUseEquipment(9, eq_use_slot);
            if (oneChangeType == -1 || oneChangeType == 10)
                wearingEquip_ring = _tableDB.GetFindUseEquipment(10, eq_use_slot);
        }

        /// <summary>
        /// 장비 숙련도 레벨에 변화가 있을 경우 
        /// </summary>
        public void ProficiencyUpdate(bool isAll = false)
        {
            // 장비 m_normal_lv 가 변화가 있는지 체크 후 있다면 서버 전송 
            var wear_equip_transaction = GameDatabase.GetInstance().tableDB.ParamTransactionEquipNormalLevel(PopUpMng.GetInstance().popUpProficiency.isUpdateEquipDB, isAll);
            if (wear_equip_transaction.Count > 0)
            {
                foreach (var db in wear_equip_transaction)
                {
                    Backend.GameInfo.TransactionWrite(db);
                }
            }
        }

        private List<TransactionParam> ParamTransactionEquipNormalLevel(bool[] isUpdateEquipDB, bool isAll)
        {
            int addCnt = 0;
            int addListCnt = 0;
            TransactionParam tParam = new TransactionParam();
            List<TransactionParam> tParamList = new List<TransactionParam>();
            for(int f_eq_ty = 0; f_eq_ty <= 10; f_eq_ty++)
            {
                if(isUpdateEquipDB[f_eq_ty] == true || isAll)
                {
                    var db = GetNowWearingEquipPartsData(f_eq_ty);
                    List<BackEnd.WRITE> writes = new List<BackEnd.WRITE> { new WRITE { Action = TransactionAction.Update, Param = ParamT.Collection(new ParamT.P[] { new ParamT.P() { k = "m_norm_lv", v = db.m_norm_lv } }) } };
                    tParam.AddUpdateList(BackendGpgsMng.tableName_Equipment, db.indate, writes);

                    addCnt++;
                    if (addCnt >= 10)
                    {
                        tParamList.Add(tParam);
                        tParam = new TransactionParam();
                        addCnt = 0;
                        addListCnt++;
                    }
                }

                PopUpMng.GetInstance().popUpProficiency.isUpdateEquipDB[f_eq_ty] = false;
            }

            if (addCnt > 0 && addCnt < 10)
            {
                tParamList.Add(tParam);
            }

            return tParamList;
        }

        /// <summary>
        /// 착용 장비 평균 등급 
        /// </summary>
        public int GetEquipRatingAverage()
        {
            int aver = 0;
            for (int i = 0; i <= 10; i++)
            {
                aver += GetNowWearingEquipPartsData(i).eq_rt;
            }

            return (int)Math.Floor((float)(aver / 11));
        }
        public int GetEquipRatingAverage(List<int> rts)
        {
            int aver = 0;
            foreach (int rt in rts)
            {
                aver += rt;
            }

            return (int)Math.Floor((float)(aver / 11));
        }

        /// <summary> 장비 ALL </summary>
        public List<Equipment> GetAllEquipment() => _equipment;
        /// <summary> 인벤 데이터</summary>
        public List<Equipment> GetInvenSave_Equip() => _equipment.FindAll((Equipment eq) => string.IsNullOrEmpty(eq.indate) && int.Equals(eq.m_state, 0));
        
        /// <summary> 장비 기본 세팅 값 </summary>
        public Equipment GetEquipDefault(int get_partype)
        {
            Equipment default_equip = 
            new Equipment() { indate = string.Empty, aInUid = GetInstance().GetUniqueIDX(),
                eq_ty = get_partype,
                ma_st_id = _chartDB.GetPartyMainStatId(get_partype),
                ma_st_rlv = 1,
            };

            return default_equip;
        }

        /// <summary> 착용 장비 데이터 </summary>
        public Equipment GetFindUseEquipment(int pt_type, int eq_slot)
        {
            var eqParts = _equipment.FindAll((Equipment eq) => int.Equals(eq.eq_ty, pt_type) && eq.m_state > 0 && int.Equals(eq.m_state, eq_slot));
            if (eqParts.Count > 0)
            {
                return eqParts[0];
            }
            else
            {
                LogPrint.Print(pt_type + "착용 장비데이터 null");
                return _tableDB.GetEquipDefault(pt_type);
            }
        }

        public Equipment GetFindEquipUID(long uid)
        {
            int indx = _equipment.FindIndex((Equipment eq) => long.Equals(eq.aInUid, uid));
            if (indx >= 0)
            {
                return _equipment[indx];
            }
            else return default;
        }

        /// <summary> 현재 슬롯에 착용중인 장비인가 </summary>
        public bool IsNowPartsWear (int pty)
        {
            int eq_use_slot = GetUseEquipSlot();
            int state = _tableDB.GetFindUseEquipment(pty, eq_use_slot).m_state;
            return state > 0;
        }

        /// <summary>  현재 장비는 사용중 슬롯중에 장착중인 장비인가 </summary>
        public bool IsNowUseSlotWearEquip(Equipment val)
        {
            int indx = _equipment.FindIndex(x => int.Equals(x.aInUid, val.aInUid));
            if (indx >= 0)
                return _equipment[indx].m_state == GetUseEquipSlot();
            else return false;
        }

        /// <summary> 현재 장비는 모든 슬롯중에 장착중인 장비인가 </summary>
        public bool IsNowAllUseSlotWearEquip(Equipment val)
        {
            int indx = _equipment.FindIndex(x => int.Equals(x.aInUid, val.aInUid));
            if (indx >= 0)
                return _equipment[indx].m_state > 0;
            else return false;
        }

        /// <summary> 현재 선택한 장비를 판매할 수 있는가? 현 타입의 장비는 착용중 제외 최고 1개이상 가지고 있어야 판매 가능 </summary>
        public bool GetNowEquipSellableCheck(int pty)
        {
            int ty_cnt = _equipment.FindAll((Equipment eq) => object.Equals(eq.eq_ty, pty) && string.Equals(eq.m_state, 0)).Count;
            return ty_cnt >= 2 || (IsNowPartsWear(pty) == true && ty_cnt >= 1);
        }

        /// <summary> 장비 타입의 모든 데이터를 리스트로 리턴 </summary>
        public List<Equipment> GetTypeEquipment(int _parts_type) => _equipment.FindAll((Equipment e) => e.eq_ty == _parts_type);

        /// <summary> 현재 사용중인 장비의 슬롯 </summary>
        public int GetUseEquipSlot() => 1;

        /// <summary> 장비중 indate가 존재하는 데이터중 판매or인벤에 있는 데이터의 UID를 리턴 </summary>
        public long GetUnusedUID(long _except = -1)
        {
            int indx = _equipment.FindIndex(eq => !string.IsNullOrEmpty(eq.indate) && eq.m_state < 0 && !long.Equals(eq.aInUid, _except));
            if (indx >= 0)
                return _equipment[indx].aInUid;
            else return 0;
        }

        /// <summary> uid를 가지고 서버에 사용되지 않는 않는(판매,인벤) indate를 비워준다 </summary>
        public void SetUnusedInDateToEmpty(long _uid)
        {
            if(_uid > 0)
            {
                int indx = _equipment.FindIndex(eq => long.Equals(eq.aInUid, _uid));
                if (indx >= 0)
                {
                    Equipment eqDb = _equipment[indx];
                    eqDb.indate = string.Empty;
                    _equipment[indx] = eqDb;
                }
            }
        }

        /// <summary> UID로 장비의 InDate를 리턴 </summary>
        public string GetUIDSearchToInDate(long _uid)
        {
            if (_uid <= 0)
                return string.Empty;

            int indx = _equipment.FindIndex((Equipment obj) => long.Equals(obj.aInUid, _uid));
            if (indx >= 0)
                return _equipment[indx].indate;
            else return string.Empty;
        }

        /// <summary> 영웅 등급 이상 장비가 몇개인가, InDate 값이 있으면 서버에 존재하는 장비임 </summary>
        public int GetBackendEquipRating6Count () => _equipment.FindAll(x => string.IsNullOrEmpty(x.indate)).Count;

        /// <summary>
        /// 착용한 장비를 제외한 현재 부위의 m_state값이 1이상이면 0으로 리셋 
        /// </summary> UpdateMStateServerDB_Equip
        public async Task UpdateMStateServerDB_Equip(GameDatabase.TableDB.Equipment wear_eqDb)
        {
            int index = _equipment.FindIndex(eq => long.Equals(eq.aInUid, wear_eqDb.aInUid));
            if (index >= 0)
            {
                if (_equipment[index].m_state <= 0)
                    _equipment[index] = wear_eqDb;
            }

            // 착용한 장비가 아닌게 m_state가 1이면 서버로 0으로 바꿔서 전송 
            var eqTyDb = GetInstance().tableDB.GetAllEquipment().FindAll((Equipment obj) => !long.Equals(obj.aInUid, wear_eqDb.aInUid) && obj.eq_ty == wear_eqDb.eq_ty && obj.m_state > 0);
            foreach (var item in eqTyDb)
            {
                int index2 = _equipment.FindIndex(obj => long.Equals(obj.aInUid, item.aInUid));
                if (index2 >= 0)
                {
                    LogPrint.Print("-------->---------- " + item.aInUid + " --------<----------");
                    var temp_db = item;
                    if(string.IsNullOrEmpty(temp_db.indate) == false) // 서버에 존재하는 장비인 경우 
                    {
                        BackendReturnObject bro1 = null;
                        SendQueue.Enqueue(Backend.GameInfo.Update, BackendGpgsMng.tableName_Equipment, temp_db.indate, ParamT.Collection(new ParamT.P[] { new ParamT.P() { k = "m_state", v = 0 } }), callback => { bro1 = callback; });
                        while (bro1 == null) { await Task.Delay(100); }
                    }

                    temp_db.m_state = 0;
                    _equipment[index2] = temp_db;
                }
            }
        }

        /// <summary>
        /// 현재 파츠별 장착중인 장비의 데이터를 리턴, 착용중인 장비가 없으면 기본값 리턴 
        /// </summary>
        public Equipment GetNowWearingEquipPartsData (int parts_type)
        {
            switch(parts_type)
            {
                case 0:  return wearingEquip_weapon;
                case 1:  return wearingEquip_shield;
                case 2:  return wearingEquip_helmet;
                case 3:  return wearingEquip_shoulder;
                case 4:  return wearingEquip_armor;
                case 5:  return wearingEquip_arm;
                case 6:  return wearingEquip_pants;
                case 7:  return wearingEquip_boots;
                case 8:  return wearingEquip_necklace;
                case 9:  return wearingEquip_earring;
                case 10: return wearingEquip_ring;
            }

            return GetEquipDefault(parts_type);
        }

        /// <summary>
        /// 현재 착용중인 장비중 장비 레벨이 제일 낮은 장비 DB 
        /// </summary>
        /// <returns></returns>
        public Equipment GetNowWearingEquipMinNormalLevel()
        {
            Equipment return_eqDB = default;
            return_eqDB.m_norm_lv = -1;
            int norLv = GameDatabase.GetInstance().chartDB.GetDicBalanceEquipMaxNormalLevel();
            for (int f_ty = 0; f_ty <= 10; f_ty++)
            {
                var wear_eqDB = GetNowWearingEquipPartsData(f_ty);
                if (wear_eqDB.m_norm_lv < norLv)
                {
                    norLv = wear_eqDB.m_norm_lv;
                    return_eqDB = wear_eqDB;
                }
            }
            
            return return_eqDB;
        }

        /// <summary> 새로운 장비 데이터를 리턴 </summary>
        public Equipment GetNewEquipmentData(int pt_ty, int rat, int idx)
        {
            // 장비 데이터 세팅 
            Equipment new_eq = GetInstance().tableDB.GetEquipDefault(pt_ty);
            new_eq.eq_id = idx;
            new_eq.ma_st_id = GameDatabase.GetInstance().chartDB.GetPartyMainStatId(pt_ty);
            new_eq.ma_st_rlv = GetInstance().GetStatRandomLevel(); // 매인 스탯 rnd 레벨 
            new_eq.eq_rt = rat;

            new_eq.st_op = GetEquipRandomOption(new_eq.eq_ty, new_eq.eq_rt);
            if (GameDatabase.GetInstance().chartDB.GetIsPartsTypeAcce(pt_ty) == true && rat >= 3) // 장신구 전용 옵션은 고급 등급 부터 (rating 3)
                new_eq.st_sop_ac = GetEquipAcceRandomSpecialOption(true);

            new_eq.st_ms = new EquipMagicSocket();
            new_eq.client_add_sp = GameDatabase.GetInstance().GetUniqueIDX();
            return new_eq;
        }

        ///<summary> 새로운 장비 획득 </summary>
        private bool SetAcquireEquip(cdb_r_chapter_equip_drop_result drop_eqDB)
        {
            if (drop_eqDB.ty >= 0 && drop_eqDB.rt > 0 && drop_eqDB.id > 0)
            {
                GameDatabase.GetInstance().equipmentEncyclopediaDB.DropAcquisitionAdded(drop_eqDB.ty, drop_eqDB.rt, drop_eqDB.id);
                if (ConvenienceFunctionMng.GetInstance().AutoSaleOnEquipType(drop_eqDB.ty, drop_eqDB.rt, drop_eqDB.id) == false) // 자동 판매 
                {
                    if (GetInstance().inventoryDB.CheckIsEmpty() == true) // 인벤토리 공간 체크 
                    {
                        var new_eqDB = GetNewEquipmentData(drop_eqDB.ty, drop_eqDB.rt, drop_eqDB.id);
                        UpdateClientDB_Equip(new_eqDB); // client DropViewEquipQueue

                        NotificationIcon.GetInstance().CheckNoticeAutoWear(new_eqDB, false);
                        PopUpMng.GetInstance().Open_DropEquip(new_eqDB);
                        GetInstance().chat.ChatSendItemMessage("equip", new_eqDB.eq_ty, new_eqDB.eq_rt, new_eqDB.eq_id);
                        if (new_eqDB.eq_rt >= 5)
                        {
                            if (GameDatabase.GetInstance().option.setting_equip_vib == 0 && !BackendGpgsMng.isEditor)
                                Handheld.Vibrate();
                        }

                        return true;
                    }
                    else
                    {
                        if (GameDatabase.GetInstance().option.notice_inventory_max == 0)
                            PopUpMng.GetInstance().Open_MessageError("인벤토리에 저장 공간이 부족합니다.");
                    }
                }
            }

            return false;
        }

        /// <summary> 장신구 전용 옵션 생성 </summary>
        public StatOp GetEquipAcceRandomSpecialOption (bool isAcce)
        {
            float r_pc = GetInstance().GetRandomPercent(), n_pc = 0f;
            if (isAcce)
            {
                foreach (var item in lst_cdb_acce_special_op)
                {
                    n_pc += item.pct;
                    if (r_pc <= n_pc)
                    {
                        return new StatOp() { id = item.ac_sop_id, rlv = GetInstance().GetStatRandomLevel() };
                    }
                }
            }
            else
            {
                for (int i = 0; i < 10; i++)
                {
                    int pct = GameDatabase.GetInstance().chartDB.GetDicBalance(string.Format("equip.sop.pct_id_{0}", i)).val_int;
                    if (pct > 0)
                    {
                        n_pc += pct;
                        if (r_pc <= n_pc)
                        {
                            return new StatOp() { id = GameDatabase.GetInstance().chartDB.GetDicBalance(string.Format("equip.sop.id_{0}", i)).val_int, rlv = GetInstance().GetStatRandomLevel() };
                        }
                    }
                }
            }

            return new StatOp();
        }

        /// <summary>
        /// 좀비 : 장비 옵션 랜덤 생성 or 고정 생성(몬스터전용 : st_id < 0 or 1 > , st_rlv )
        /// 옵션 스탯에 사용될 옵션 
        /// 무기   : 공격력 (stat id : 1) 
        /// 방패   : 방어력 (stat id : 2) 
        /// 어깨   : 치명타 공격력 (stat id : 4) 
        /// 갑옷   : 체력 (stat id : 5) 
        /// 부츠   : 치명타 방어력 (stat id : 8) 
        /// </summary>
        public EquipStatOp GetEquipRandomOption (int pt_ty, int rat, int st_id = -1, int st_rlv = -1)
        {
            if (dic_cdb_equip_op.TryGetValue(pt_ty, out cdb_equip_op ops))
            {
                var temp_so = new EquipStatOp();
                string[] op_st1 = ops.op1.Split(','); // 랜덤 옵션 1
                string[] op_st2 = ops.op2.Split(','); // 랜덤 옵션 2
                string[] op_st3 = ops.op3.Split(','); // 랜덤 옵션 3
                string[] op_st4 = ops.op4.Split(','); // 랜덤 옵션 4
                float eq_bns_st_pct = GetInstance().GetRandomPercent();//RowPaser.FloatPaser(row_random_probability, "num"); // 0 ~ 100 사이 랜덤 값 (장비 등급별로 특정 확률로 옵션 스탯 적용)
                switch (rat)
                {
                    case 1:
                    case 2: // 일반, 중급 각 1개 확정 = 최대 1개  
                        temp_so.op1 = new StatOp() { id = int.Parse(op_st1[st_id == -1 ? UnityEngine.Random.Range(0, op_st1.Length) : st_id]), rlv = st_rlv == -1 ? GetInstance().GetStatRandomLevel() : st_rlv };
                        break;
                    case 3: // 고급 1개 확정 1개 랜덤 = 최대 2개 
                        temp_so.op1 = new StatOp() { id = int.Parse(op_st1[st_id == -1 ? UnityEngine.Random.Range(0, op_st1.Length) : st_id]), rlv = st_rlv == -1 ? GetInstance().GetStatRandomLevel() : st_rlv };
                        if (eq_bns_st_pct < 15 || st_id != -1)
                            temp_so.op2 = new StatOp() { id = int.Parse(op_st2[st_id == -1 ? UnityEngine.Random.Range(0, 2) : st_id]), rlv = st_rlv == -1 ? GetInstance().GetStatRandomLevel() : st_rlv };
                        break;
                    case 4: // 희귀 2개 확정 = 최대 2개 
                        temp_so.op1 = new StatOp() { id = int.Parse(op_st1[st_id == -1 ? UnityEngine.Random.Range(0, op_st1.Length) : st_id]), rlv = st_rlv == -1 ? GetInstance().GetStatRandomLevel() : st_rlv };
                        temp_so.op2 = new StatOp() { id = int.Parse(op_st2[st_id == -1 ? UnityEngine.Random.Range(0, op_st2.Length) : st_id]), rlv = st_rlv == -1 ? GetInstance().GetStatRandomLevel() : st_rlv };
                        break;
                    case 5: // 영웅 2개 확정 1개 랜덤 = 최대 3개 
                        temp_so.op1 = new StatOp() { id = int.Parse(op_st1[st_id == -1 ? UnityEngine.Random.Range(0, op_st1.Length) : st_id]), rlv = st_rlv == -1 ? GetInstance().GetStatRandomLevel() : st_rlv };
                        temp_so.op2 = new StatOp() { id = int.Parse(op_st2[st_id == -1 ? UnityEngine.Random.Range(0, op_st2.Length) : st_id]), rlv = st_rlv == -1 ? GetInstance().GetStatRandomLevel() : st_rlv };
                        if (eq_bns_st_pct < 15 || st_id != -1)
                            temp_so.op3 = new StatOp() { id = int.Parse(op_st3[st_id == -1 ? UnityEngine.Random.Range(0, op_st3.Length) : st_id]), rlv = st_rlv == -1 ? GetInstance().GetStatRandomLevel() : st_rlv };
                        break;
                    case 6: // 고대 3개 확정 1개 랜덤 = 최대 4개 
                        temp_so.op1 = new StatOp() { id = int.Parse(op_st1[st_id == -1 ? UnityEngine.Random.Range(0, op_st1.Length) : st_id]), rlv = st_rlv == -1 ? GetInstance().GetStatRandomLevel() : st_rlv };
                        temp_so.op2 = new StatOp() { id = int.Parse(op_st2[st_id == -1 ? UnityEngine.Random.Range(0, op_st2.Length) : st_id]), rlv = st_rlv == -1 ? GetInstance().GetStatRandomLevel() : st_rlv };
                        temp_so.op3 = new StatOp() { id = int.Parse(op_st3[st_id == -1 ? UnityEngine.Random.Range(0, op_st3.Length) : st_id]), rlv = st_rlv == -1 ? GetInstance().GetStatRandomLevel() : st_rlv };
                        if (eq_bns_st_pct < 15 || st_id != -1)
                            temp_so.op4 = new StatOp() { id = int.Parse(op_st4[st_id == -1 ? UnityEngine.Random.Range(0, op_st4.Length) : st_id]), rlv = st_rlv == -1 ? GetInstance().GetStatRandomLevel() : st_rlv };
                        break;
                    case 7: // 전설 4개확정 = 최대 4개 
                        temp_so.op1 = new StatOp() { id = int.Parse(op_st1[st_id == -1 ? UnityEngine.Random.Range(0, op_st1.Length) : st_id]), rlv = st_rlv == -1 ? GetInstance().GetStatRandomLevel() : st_rlv };
                        temp_so.op2 = new StatOp() { id = int.Parse(op_st2[st_id == -1 ? UnityEngine.Random.Range(0, op_st2.Length) : st_id]), rlv = st_rlv == -1 ? GetInstance().GetStatRandomLevel() : st_rlv };
                        temp_so.op3 = new StatOp() { id = int.Parse(op_st3[st_id == -1 ? UnityEngine.Random.Range(0, op_st3.Length) : st_id]), rlv = st_rlv == -1 ? GetInstance().GetStatRandomLevel() : st_rlv };
                        temp_so.op4 = new StatOp() { id = int.Parse(op_st4[st_id == -1 ? UnityEngine.Random.Range(0, op_st4.Length) : st_id]), rlv = st_rlv == -1 ? GetInstance().GetStatRandomLevel() : st_rlv };
                        break;
                }

                return temp_so;
            }
            return new EquipStatOp();
        }

        /// <summary>
        /// 몬스터 : 장비 옵션 랜덤 생성 or 고정 생성(몬스터전용 : st_id < 0 or 1 > , st_rlv )
        /// 옵션 스탯에 사용될 옵션 
        /// 무기   : 공격력 (stat id : 1) 
        /// 방패   : 방어력 (stat id : 2) 
        /// 어깨   : 치명타 공격력 (stat id : 4) 
        /// 갑옷   : 체력 (stat id : 5) 
        /// 부츠   : 치명타 방어력 (stat id : 8) 
        /// </summary>
        public EquipStatOp GetMonsterEquipRandomOption(int eq_ty, int eq_rt, bool isRndOpSt)
        {
            if (dic_cdb_equip_op.TryGetValue(eq_ty, out cdb_equip_op ops))
            {
                //isRndOpSt = true;
                var temp_so = new EquipStatOp();
                string[] op_st1 = ops.op1.Split(','); // 랜덤 옵션 1
                string[] op_st2 = ops.op2.Split(','); // 랜덤 옵션 2
                string[] op_st3 = ops.op3.Split(','); // 랜덤 옵션 3
                string[] op_st4 = ops.op4.Split(','); // 랜덤 옵션 4
                float eq_bns_st_pct = GetInstance().GetRandomPercent();//RowPaser.FloatPaser(row_random_probability, "num"); // 0 ~ 100 사이 랜덤 값 (장비 등급별로 특정 확률로 옵션 스탯 적용)
                switch (eq_rt)
                {
                    case 1:
                    case 2: // 일반, 중급 각 1개 확정 = 최대 1개  
                        temp_so.op1 = new StatOp() { id = isRndOpSt ? int.Parse(op_st1[UnityEngine.Random.Range(0, op_st1.Length)]) : 1, rlv = GetInstance().GetStatRandomLevel() };
                        break;
                    case 3: // 고급 1개 확정 1개 랜덤 = 최대 2개 
                        temp_so.op1 = new StatOp() { id = isRndOpSt ? int.Parse(op_st1[UnityEngine.Random.Range(0, op_st1.Length)]) : 1, rlv = GetInstance().GetStatRandomLevel() };
                        if (eq_bns_st_pct < 15 || isRndOpSt)
                        temp_so.op2 = new StatOp() { id = isRndOpSt ? int.Parse(op_st2[UnityEngine.Random.Range(0, op_st2.Length)]) : 5, rlv = GetInstance().GetStatRandomLevel() };
                        break;
                    case 4: // 희귀 2개 확정 = 최대 2개 
                        temp_so.op1 = new StatOp() { id = isRndOpSt ? int.Parse(op_st1[UnityEngine.Random.Range(0, op_st1.Length)]) : 1, rlv = GetInstance().GetStatRandomLevel() };
                        temp_so.op2 = new StatOp() { id = isRndOpSt ? int.Parse(op_st2[UnityEngine.Random.Range(0, op_st2.Length)]) : 5, rlv = GetInstance().GetStatRandomLevel() };
                        break;
                    case 5: // 영웅 2개 확정 1개 랜덤 = 최대 3개 
                        temp_so.op1 = new StatOp() { id = isRndOpSt ? int.Parse(op_st1[UnityEngine.Random.Range(0, op_st1.Length)]) : 1, rlv = GetInstance().GetStatRandomLevel() };
                        temp_so.op2 = new StatOp() { id = isRndOpSt ? int.Parse(op_st2[UnityEngine.Random.Range(0, op_st2.Length)]) : 5, rlv = GetInstance().GetStatRandomLevel() };
                        if (eq_bns_st_pct < 15 || isRndOpSt)
                        temp_so.op3 = new StatOp() { id = isRndOpSt ? int.Parse(op_st3[UnityEngine.Random.Range(0, op_st3.Length)]) : 4, rlv = GetInstance().GetStatRandomLevel() };
                        break;
                    case 6: // 고대 3개 확정 1개 랜덤 = 최대 4개 
                        temp_so.op1 = new StatOp() { id = isRndOpSt ? int.Parse(op_st1[UnityEngine.Random.Range(0, op_st1.Length)]) : 1, rlv = GetInstance().GetStatRandomLevel() };
                        temp_so.op2 = new StatOp() { id = isRndOpSt ? int.Parse(op_st2[UnityEngine.Random.Range(0, op_st2.Length)]) : 5, rlv = GetInstance().GetStatRandomLevel() };
                        temp_so.op3 = new StatOp() { id = isRndOpSt ? int.Parse(op_st3[UnityEngine.Random.Range(0, op_st3.Length)]) : 4, rlv = GetInstance().GetStatRandomLevel() };
                        if (eq_bns_st_pct < 15 || isRndOpSt)
                        temp_so.op4 = new StatOp() { id = isRndOpSt ? int.Parse(op_st4[UnityEngine.Random.Range(0, op_st4.Length)]) : 2, rlv = GetInstance().GetStatRandomLevel() };
                        break;
                    case 7: // 전설 4개확정 = 최대 4개 
                        //temp_so.op1 = new StatOp() { id = int.Parse(op_st1[isRndOpSt ? UnityEngine.Random.Range(0, op_st1.Length) : 1]),rlv = GetInstance().GetStatRandomLevel() };
                        temp_so.op1 = new StatOp() { id = isRndOpSt ? int.Parse(op_st1[UnityEngine.Random.Range(0, op_st1.Length)]) : 5, rlv = isRndOpSt ? GetInstance().GetStatRandomLevel() : 10 };
                        temp_so.op2 = new StatOp() { id = isRndOpSt ? int.Parse(op_st2[UnityEngine.Random.Range(0, op_st2.Length)]) : 5, rlv = isRndOpSt ? GetInstance().GetStatRandomLevel() : 10 };
                        temp_so.op3 = new StatOp() { id = isRndOpSt ? int.Parse(op_st3[UnityEngine.Random.Range(0, op_st3.Length)]) : 5, rlv = isRndOpSt ? GetInstance().GetStatRandomLevel() : 10 };
                        temp_so.op4 = new StatOp() { id = isRndOpSt ? int.Parse(op_st4[UnityEngine.Random.Range(0, op_st4.Length)]) : 5, rlv = isRndOpSt ? GetInstance().GetStatRandomLevel() : 10 };
                        break;
                }

                return temp_so;
            }
            return new EquipStatOp();
        }

        public Param EquipParamCollection(Equipment eqDb)
        {
            Param pam = new Param();
            pam.Add("aInUid", eqDb.aInUid);
            pam.Add("m_state", eqDb.m_state);
            pam.Add("m_lck", eqDb.m_lck);
            pam.Add("eq_ty", eqDb.eq_ty);
            pam.Add("eq_rt", eqDb.eq_rt);
            pam.Add("eq_id", eqDb.eq_id);
            pam.Add("eq_legend", eqDb.eq_legend);
            pam.Add("eq_legend_sop_id", eqDb.eq_legend_sop_id);
            pam.Add("eq_legend_sop_rlv", eqDb.eq_legend_sop_rlv);
            pam.Add("m_norm_lv", eqDb.m_norm_lv);
            pam.Add("m_ehnt_lv", eqDb.m_ehnt_lv);
            pam.Add("ma_st_id", eqDb.ma_st_id);
            pam.Add("ma_st_rlv", eqDb.ma_st_rlv);
            pam.Add("st_sop_ac", JsonUtility.ToJson(eqDb.st_sop_ac));
            pam.Add("st_op", JsonUtility.ToJson(eqDb.st_op));
            pam.Add("st_ms", JsonUtility.ToJson(eqDb.st_ms));
            return pam;
        }

        /// <summary>
        /// backend 장비 테이블에 데이터 추가 
        /// </summary>
        public async Task<bool> SendDataEquipment(Equipment val, string type, Param upd_pam = null)
        {
            Param add_pam = new Param();
            if ((string.Equals(type, "insert") || string.Equals(type, "first_insert") || string.Equals(type, "change")) && upd_pam == null)
            {
                add_pam = EquipParamCollection(val);
            }

            // 새로운 장비 새로 추가 
            if ((type == "insert" || type == "first_insert") && string.IsNullOrEmpty(val.indate))
            {
                Task<string> _taskInsert = BackendGpgsMng.GetInstance().TaskInsertTableData(BackendGpgsMng.tableName_Equipment, add_pam);
                await _taskInsert;
                if (_taskInsert.Result != string.Empty)
                {
                    val.indate = _taskInsert.Result;
                    UpdateClientDB_Equip(val);
                    return true;
                }
            }
            else if (string.Equals(type, "update") || string.Equals(type, "delete") || string.Equals(type, "change")) // 기존 장비 업데이트 
            {
                if (!string.IsNullOrEmpty(val.indate)) // indate가 있다. (서버에 데이터가 존재함) 
                {
                    Param p2 = string.Equals(type, "change") ? add_pam : upd_pam;
                    Task<bool> _taskUpdate = BackendGpgsMng.GetInstance().TaskUpdateTableData(BackendGpgsMng.tableName_Equipment, val.indate, p2);
                    await _taskUpdate;
                    if (_taskUpdate.Result)
                    {
                        UpdateClientDB_Equip(val, string.Equals(type, "delete"));
                    }
                }
                else // Equipment 테이블에 없는 정보 
                {
                    UpdateClientDB_Equip(val, string.Equals(type, "delete"));
                }
            }

            return true;
        }

        /// <summary> 서버와 정상 통신 후 -> 클라이언트에 데이터 변경 또는 추가 </summary>
        public void UpdateClientDB_Equip(Equipment eqDb, bool _delete = false)
        {
            if (eqDb.aInUid == 0)
                return;

            if (_delete == false)
            {
                int index = _equipment.FindIndex(obj => long.Equals(obj.aInUid, eqDb.aInUid));
                if (index >= 0)
                    _equipment[index] = eqDb;
                else
                    _equipment.Add(eqDb);

                if (eqDb.m_state > 0) // 착용중 장비면 
                    SetttingEquipWearingData(eqDb.eq_ty);
            }
            else
            {
                int indx = _equipment.FindIndex(obj => long.Equals(obj.aInUid, eqDb.aInUid));
                if (string.IsNullOrEmpty(eqDb.indate))
                {
                    if (indx >= 0)
                        _equipment.RemoveAt(indx);
                }
                else
                {
                    eqDb.m_state = -1;
                    _equipment[indx] = eqDb;
                }
            }

            if (BackendGpgsMng.GetInstance ().GetSceneActiveIndex() == 1)
            {
                if (MainUI.GetInstance().inventory.inventoryType != Inventory.InventoryType.Disable || MainUI.GetInstance().tapSmithy.smithyListType != SmithyListType.Disable)
                {
                    GameDatabase.GetInstance().tableDB.SetTempEquipDbChange(eqDb);
                }
            }
        }

        /// <summary>
        /// 데이터 추가 
        /// </summary>
        public async Task<bool> SetInsertNewEquipmentData(TableDB.Equipment get_eq)
        {
            int saveRating = 5;
            Task<bool> bTask = SendDataEquipment(get_eq, "insert");
            await bTask;
            if (bTask.Result)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 데이터 변경 
        /// </summary>
        public async Task<bool> SetUpdateChangeEquipmentData(TableDB.Equipment get_eq, Param param_update)
        {
            Task<bool> bTask = SendDataEquipment(get_eq, "update", param_update);
            await bTask;
            if (bTask.Result)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 처음 접속시 (일반 rating1, idx1) 장비 
        /// </summary>
        public async Task<bool> SetInitFirstEquip ()
        {
            //new_eq.m_norm_lv = GetInstance().tableDB.GetNowWearingEquipPartsData(pt_ty).m_norm_lv;
            for (int f_eq_ty = 0; f_eq_ty <= 10; f_eq_ty++)
            {
                var eq_data = GameDatabase.GetInstance().tableDB.GetEquipDefault(f_eq_ty);
                eq_data.m_state = 1;
                eq_data.eq_rt = 1;
                eq_data.eq_id = 1;
                eq_data.st_op = GameDatabase.GetInstance().tableDB.GetEquipRandomOption(eq_data.eq_ty, eq_data.eq_rt);
                eq_data.st_sop_ac = new StatOp();
                eq_data.st_ms = new EquipMagicSocket();

                Task<bool> bTask = SendDataEquipment(eq_data, "first_insert");
                await bTask;
            }

            return true;
        }

        /// <summary> 장비 정렬 </summary>
        public void SortEquipment(string sort_type = "default")
        {
            _equipment.Sort
                (delegate (Equipment a, Equipment b)
                {
                    if (sort_type == "default")
                    {
                        if (a.eq_ty > b.eq_ty)
                            return 1;
                        else if (a.eq_ty < b.eq_ty)
                            return -1;
                    }

                    if (sort_type == "rating_high")
                    {
                        if (a.eq_rt > b.eq_rt)
                            return 1;
                        else if (a.eq_rt < b.eq_rt)
                            return -1;
                    }
                    else if (sort_type == "rating_low")
                    {
                        if (a.eq_rt < b.eq_rt)
                            return 1;
                        else if (a.eq_rt > b.eq_rt)
                            return -1;
                    }

                    return 0;
                });
        }
#endregion

        // ########################################################################################################
        // ######################## 장비 : 전투력 #################################################################
        #region ########################### 장비 : 전투력 ###########################
        Cdb_stat combat_default = new Cdb_stat();
        /// <summary>
        /// 각 스탯별 전투력 측정 곱하기 값 
        /// </summary>
        public float GetCombatXValue(int stat_idx)
        {
            if(combat_default.min == 0)
            {
                combat_default = list_cdb_stat.Find(x => x.eRating == 0 && x.eIdx == 0);
            }
            
            //LogPrint.Print("typ0 : " + (float)default_stat.min * ((float)default_stat.min / (float)default_stat.min_typ0)); // 1.65 
            //LogPrint.Print("typ1 : " + (float)default_stat.min * ((float)default_stat.min / (float)default_stat.min_typ1));
            //LogPrint.Print("typ2 : " + (float)default_stat.min * ((float)default_stat.min / (float)default_stat.min_typ2));
            //LogPrint.Print("typ3 : " + (float)default_stat.min * ((float)default_stat.min / (float)default_stat.min_typ3));
            //LogPrint.Print("typ4 : " + (float)default_stat.min * ((float)default_stat.min / (float)default_stat.min_typ4));
            //LogPrint.Print("typ5 : " + (float)default_stat.min * ((float)default_stat.min / (float)default_stat.min_typ5));
            //LogPrint.Print("typ6 : " + (float)default_stat.min * ((float)default_stat.min / (float)default_stat.min_typ6));
            //LogPrint.Print("typ7 : " + (float)default_stat.min * ((float)default_stat.min / (float)default_stat.min_typ7));
            //LogPrint.Print("typ9 : " + (float)default_stat.min * ((float)default_stat.min / (float)default_stat.min_typ9));
            switch (stat_idx)
            {
                case 1: return 2.0f;// combat_default.min * (combat_default.min / (float)combat_default.min_typ1);
                case 2: return 1.0f;// combat_default.min * (combat_default.min / (float)combat_default.min_typ2);
                case 3: return 1.0f;// combat_default.min * (combat_default.min / (float)combat_default.min_typ3);
                case 4: return 2.0f;// combat_default.min * (combat_default.min / (float)combat_default.min_typ4);
                case 5: return 0.25f;// combat_default.min * (combat_default.min / (float)combat_default.min_typ5);
                case 6: return 1.0f;// combat_default.min * (combat_default.min / (float)combat_default.min_typ6);
                case 7: return 1.0f;// combat_default.min * (combat_default.min / (float)combat_default.min_typ7);
                case 8: return 2.0f;// combat_default.min * (combat_default.min / (float)combat_default.min_typ8);
                case 9: return 1.0f;// combat_default.min * (combat_default.min / (float)combat_default.min_typ9);
                default: return 1;
            }
        }

        /// <summary>
        /// 현재 장비 데이터의 전투력 총 합 
        /// all : 전체 전투력, main 스탯 전투력, op : 옵션 전투력, sop : 장신구 전용 옵션 전투력 
        /// </summary>
        public long GetEquipCombatPower(Equipment _data, string returnType, int norLv = 0)
        {
            if (string.IsNullOrEmpty(returnType))
                return 0;

            if(norLv > 0)
                _data.m_norm_lv = norLv;

            long total_combat_power = 0;
            // 매인 스탯 전투력 
            if(System.Object.Equals(returnType, "total") || System.Object.Equals(returnType, "main"))
            {
                int main_stat_idx = _data.ma_st_id;
                if(main_stat_idx > 0)
                {
                    object[] stat_val = GetInstance().chartDB.GetMainStatValue(_data);
                    total_combat_power += stat_val[0].GetType() == typeof(float) ? (long)Math.Floor((float)stat_val[0] * GetCombatXValue(main_stat_idx)) : (long)Math.Floor((long)stat_val[0] * GetCombatXValue(main_stat_idx));
                }
            }

            // 옵션 스탯 전투력 
            if (System.Object.Equals(returnType, "total") || System.Object.Equals(returnType, "op"))
            {
                var statOp = _data.st_op;
                for (int i = 0; i < 4; i++)
                {
                    var sod = i == 0 ? statOp.op1 : i == 1 ? statOp.op2 : i == 2 ? statOp.op3 : i == 3 ? statOp.op4 : new GameDatabase.TableDB.StatOp();
                    int so_id = sod.id;
                    if (so_id > 0)
                    {
                        int so_rlv = sod.rlv;
                        object op_stat_val = GameDatabase.GetInstance().chartDB.GetEquipOptionStatValue(so_id, so_rlv, _data.eq_rt, _data.eq_id, _data.m_ehnt_lv, true, _data.eq_legend, _data.eq_legend_sop_id, _data.eq_legend_sop_rlv);
                        if (op_stat_val.GetType() == typeof(float))
                        {
                            total_combat_power += (int)Math.Floor((float)op_stat_val * GetCombatXValue(so_id));
                        }
                        else
                        {
                            total_combat_power += (long)Math.Floor((long)op_stat_val * GetCombatXValue(so_id));
                        }
                    }
                }
            }

            return total_combat_power;
        }

        /// <summary>
        /// 매인 전투력 비교 
        /// </summary>
        public string GetCombatCompare(long wear_val, long select_val)
        {
            if (wear_val > 0 && select_val > 0)
            {
                if (wear_val > select_val)
                {
                    long cmpr_val = wear_val - select_val; // 1:#,###
                    return string.Format("<size=20><color=#FF0000>▼{0:#,0}</color></size>", cmpr_val);
                }
                else if (wear_val < select_val)
                {
                    long cmpr_val = select_val - wear_val;
                    return string.Format("<size=20><color=#00FF18>▲{0:#,0}</color></size>", cmpr_val);
                }
            }
            return "<size=20><color=#FFAF00>〓</color></size>";
        }

        /// <summary> 스탯 비교 (매인, 옵션) </summary>
        public string GetStatCompare(object wear_val, object select_val, int wear_op_id = 0, int invn_op_id = 0)
        {
            bool isOpStat = wear_op_id > 0 || invn_op_id > 0;
            Type val_type = wear_val.GetType();
            if (val_type == typeof(float))
            {
                float f_wear_val = (float)wear_val;
                float f_select_val = (float)select_val;
                if (f_wear_val > 0.0f && f_select_val > 0.0f)
                {
                    if (f_wear_val > f_select_val)
                    {
                        float f_cmpr_val = f_wear_val - f_select_val; // 1:#,###
                        return string.Format("<size=20><color=#FF003B>▼{0:0.000}</color></size>", f_cmpr_val);
                    }
                    else if (f_wear_val < f_select_val)
                    {
                        float f_cmpr_val = f_select_val - f_wear_val;
                        return string.Format("<size=20><color=#00FF18>▲{0:0.000}</color></size>", f_cmpr_val);
                    }
                }
            }
            else if (val_type == typeof(long))
            {
                long i_wear_val = (long)wear_val;
                long i_select_val = (long)select_val;
                if (i_wear_val > 0 && i_select_val > 0)
                {
                    if (i_wear_val > i_select_val)
                    {
                        long i_cmpr_val = i_wear_val - i_select_val; // 1:#,###
                        return string.Format("<size=20><color=#FF003B>▼{0:#,0}</color></size>", i_cmpr_val);
                    }
                    else if (i_wear_val < i_select_val)
                    {
                        long i_cmpr_val = i_select_val - i_wear_val;
                        return string.Format("<size=20><color=#00FF18>▲{0:#,0}</color></size>", i_cmpr_val);
                    }
                }
            }

            return "<size=20><color=#FFAF00>〓</color></size>";
        }

        //public string GetEquip
#endregion

        // ########################################################################################################
        // ######################## 장비 : 임시 리스트 ############################################################
        #region ########################### 장비 : 임시 리스트 ###################
        public class temp
        {
            /// <summary> 0:null : 1:장비, 2:아이템 </summary>
            public int default0Eq1It2Sk3;
            public Equipment equipment;
            public Item item;
        }

        temp tempDefault = new temp();
        Dictionary<int, temp> temp_list = new Dictionary<int, temp>();
        public int GetTempCount() => temp_list.Count;


        public temp GetTempList (int k)
        {
            if (temp_list.ContainsKey(k))
            {
                return temp_list[k];
            }
            else return tempDefault;
        }

        /// <summary>
        /// 임시 리스트의 현재 장비의 데이터를 교체 
        /// </summary>
        public void SetTempEquipDbChange(Equipment eqDb)
        {
            //foreach (var item in temp_list.Keys)
            //{
            //    if(temp_list[item].equipment.aInUid == eqDb.aInUid)
            //    {
            //        temp_list[item].equipment = eqDb;
            //        break;
            //    }
            //}

            int index1 = _equipment.FindIndex(x => x.aInUid == eqDb.aInUid);
            if (index1 >= 0)
                _equipment[index1] = eqDb;

            TempInventoryList();
        }

        /// <summary>
        /// 임시 리스트의 장착중이었던 장비를 리스트에서 선택한 장비와 교체 
        /// </summary>
        public void SetTempEquipDbChange(Equipment eqDb1, Equipment eqDb2)
        {
            int index1 = _equipment.FindIndex(x => x.aInUid == eqDb1.aInUid);
            if (index1 >= 0)
                _equipment[index1] = eqDb2;

            int index2 = _equipment.FindIndex(x => x.aInUid == eqDb2.aInUid);
            if (index2 >= 0)
                _equipment[index1] = eqDb1;

            TempInventoryList();
        }

        /// <summary>
        /// 임시 리스트의 현재 아이템의 데이터를 교체 
        /// </summary>
        public void SetTempItemDbChange(Item eqDb)
        {
            int index1 = _item.FindIndex(x => x.aInUid == eqDb.aInUid);
            if (index1 >= 0)
                _item[index1] = eqDb;

            TempInventoryList();
        }

        Dictionary<int, temp> arrDicTempInventory = new Dictionary<int, temp>();
        public void TempInventoryList(bool _default = false)
        {
            LogPrint.Print("<color=magenta>------------------- 인벤토리 리스트 정렬 -------------------</color>");
            var inv_type = MainUI.GetInstance().inventory.inventoryType;
            var smt_type = MainUI.GetInstance().tapSmithy.smithyType;
            var smt_list_type = MainUI.GetInstance().tapSmithy.smithyListType;
            var invn_sort_type = PopUpMng.GetInstance().popUpInventorySort.enum_SortInventory;
            bool sortHighToLow = PopUpMng.GetInstance().popUpInventorySort.enum_SortInvnHighLow == SortInventorytHighLow.HIGH_TO_LOW;

            LogPrint.Print("<color=magenta> invn_sort_type : " + invn_sort_type + ", sortHighToLow : " + sortHighToLow + "</color>");

            // 인벤토리 탭 : 보유중인 장비를 보여줌
            // 대장간 탭 : 보유중, 착용중 장비를 보여줌 
            bool isInvTap = inv_type != Inventory.InventoryType.Disable && smt_list_type == SmithyListType.Disable;
            var temp_wpsh = _equipment.FindAll((Equipment eq) => (isInvTap == true ? eq.m_state == 0 : eq.m_state >= 0) && (eq.eq_ty == 0 || eq.eq_ty == 1));
            var temp_cost = _equipment.FindAll((Equipment eq) => (isInvTap == true ? eq.m_state == 0 : eq.m_state >= 0) && (eq.eq_ty == 2 || eq.eq_ty == 3 || eq.eq_ty == 4 || eq.eq_ty == 5 || eq.eq_ty == 6 || eq.eq_ty == 7));
            var temp_acce = _equipment.FindAll((Equipment eq) => (isInvTap == true ? eq.m_state == 0 : eq.m_state >= 0) && (eq.eq_ty == 8 || eq.eq_ty == 9 || eq.eq_ty == 10));
            var temp_item = inv_type == Inventory.InventoryType.Disable ? _item.FindAll((Item it) => it.type == 21 || it.type == 27) : _item;

            arrDicTempInventory.Clear();
            temp_list.Clear();
            int key = 0;
            if (invn_sort_type != SortInventory.TYPE)
            {
                // ### 무기, 방패 
                if ((inv_type == Inventory.InventoryType.ALL || inv_type == Inventory.InventoryType.EQUIP_WEAPON_SHIELD) || smt_list_type == SmithyListType.EQUIP_WEAPON_SHIELD)
                {
                    foreach (var eq_item in temp_wpsh)
                    {
                        if (eq_item.eq_id > 0)
                        {
                            arrDicTempInventory.Add(key, new temp() { default0Eq1It2Sk3 = 1, equipment = eq_item });
                            key++;
                        }
                    }
                }

                // ###  방어구 
                if ((inv_type == Inventory.InventoryType.ALL || inv_type == Inventory.InventoryType.EQUIP_COSTUME) || smt_list_type == SmithyListType.EQUIP_COSTUME)
                {
                    foreach (var eq_item in temp_cost)
                    {
                        if (eq_item.eq_id > 0)
                        {
                            arrDicTempInventory.Add(key, new temp() { default0Eq1It2Sk3 = 1, equipment = eq_item });
                            key++;
                        }
                    }
                }

                // ### 장신구 / 전설 진화 장비 
                if ((inv_type == Inventory.InventoryType.ALL || inv_type == Inventory.InventoryType.EQUIP_ACCE) || smt_list_type == SmithyListType.EQUIP_ACCE)
                {
                    foreach (var eq_item in temp_acce)
                    {
                        if (eq_item.eq_id > 0)
                        {
                            arrDicTempInventory.Add(key, new temp() { default0Eq1It2Sk3 = 1, equipment = eq_item });
                            key++;
                        }
                    }
                }

                // ### 아이템 
                if ((inv_type == Inventory.InventoryType.ALL || inv_type == Inventory.InventoryType.ITEM) || smt_list_type == SmithyListType.ITEM)
                {
                    foreach (var it_item in temp_item)
                    {
                        if (it_item.rating > 0)
                        {
                            arrDicTempInventory.Add(key, new temp() { default0Eq1It2Sk3 = 2, item = it_item });
                            key++;
                        }
                    }
                }
            }

            key = 0;
            // dict.OrderByDescending -> 내림 차순 정렬
            // dict.OrderBy -> 오름 차순 정렬 
            // ### 신규 정렬 ###
            if (invn_sort_type == SortInventory.NEW)
            {
                var dbs = sortHighToLow == true ?
                    arrDicTempInventory.OrderByDescending(x => x.Value.default0Eq1It2Sk3 == 1 ? x.Value.equipment.client_add_sp : x.Value.item.client_add_sp) :
                    arrDicTempInventory.OrderBy(x => x.Value.default0Eq1It2Sk3 == 1 ? x.Value.equipment.client_add_sp : x.Value.item.client_add_sp);
                foreach (var v in dbs)
                {
                    temp_list.Add(key, v.Value);
                    key++;
                }
            }
            else // ### 등급 정렬 ###
            if (invn_sort_type == SortInventory.RATING)
            {
                var dbs = sortHighToLow == true ?
                    arrDicTempInventory.OrderByDescending(x => x.Value.default0Eq1It2Sk3 == 1 ? (int)x.Value.equipment.eq_rt : x.Value.item.rating) :
                    arrDicTempInventory.OrderBy(x => x.Value.default0Eq1It2Sk3 == 1 ? (int)x.Value.equipment.eq_rt : x.Value.item.rating);
                foreach (var v in dbs)
                {
                    temp_list.Add(key, v.Value);
                    key++;
                }
            }
            else // ### 종류 정렬 ###
            if (invn_sort_type == SortInventory.TYPE)
            {
                temp_wpsh.Sort((Equipment x, Equipment y) => sortHighToLow == true ? x.eq_ty.CompareTo(y.eq_ty) : y.eq_ty.CompareTo(x.eq_ty));
                temp_cost.Sort((Equipment x, Equipment y) => sortHighToLow == true ? x.eq_ty.CompareTo(y.eq_ty) : y.eq_ty.CompareTo(x.eq_ty));
                temp_acce.Sort((Equipment x, Equipment y) => sortHighToLow == true ? x.eq_ty.CompareTo(y.eq_ty) : y.eq_ty.CompareTo(x.eq_ty));
                temp_item.Sort((Item x, Item y) => sortHighToLow == true ? x.type.CompareTo(y.type) : y.type.CompareTo(x.type));

                if (sortHighToLow == false && (inv_type == Inventory.InventoryType.ALL || inv_type == Inventory.InventoryType.ITEM))
                {
                    foreach (var varItDb in temp_item)
                    {
                        if (varItDb.type > 0)
                        {
                            arrDicTempInventory.Add(key, new temp() { default0Eq1It2Sk3 = 2, item = varItDb });
                            key++;
                        }
                    }
                }

                if (inv_type == Inventory.InventoryType.ALL || inv_type == Inventory.InventoryType.EQUIP_WEAPON_SHIELD)
                {
                    foreach (var varEqDb in temp_wpsh)
                    {
                        if (varEqDb.eq_id > 0)
                        {
                            arrDicTempInventory.Add(key, new temp() { default0Eq1It2Sk3 = 1, equipment = varEqDb });
                            key++;
                        }
                    }
                }

                if (inv_type == Inventory.InventoryType.ALL || inv_type == Inventory.InventoryType.EQUIP_COSTUME)
                {
                    foreach (var varEqDb in temp_cost)
                    {
                        if (varEqDb.eq_id > 0)
                        {
                            arrDicTempInventory.Add(key, new temp() { default0Eq1It2Sk3 = 1, equipment = varEqDb });
                            key++;
                        }
                    }
                }

                if (inv_type == Inventory.InventoryType.ALL || inv_type == Inventory.InventoryType.EQUIP_ACCE)
                {
                    foreach (var varEqDb in temp_acce)
                    {
                        if (varEqDb.eq_id > 0)
                        {
                            arrDicTempInventory.Add(key, new temp() { default0Eq1It2Sk3 = 1, equipment = varEqDb });
                            key++;
                        }
                    }
                }

                if (sortHighToLow == true && (inv_type == Inventory.InventoryType.ALL || inv_type == Inventory.InventoryType.ITEM))
                {
                    foreach (var varItDb in temp_item)
                    {
                        if (varItDb.type > 0)
                        {
                            arrDicTempInventory.Add(key, new temp() { default0Eq1It2Sk3 = 2, item = varItDb });
                            key++;
                        }
                    }
                }
                else
                {
                    arrDicTempInventory.OrderByDescending(x => x.Value.equipment.eq_ty);
                }

                key = 0;
                foreach (var item in arrDicTempInventory)
                {
                    temp_list.Add(key, item.Value);
                    key++;
                }
            }
            else // ### 일반 레벨 정렬 ###
            if (invn_sort_type == SortInventory.NORMAL_LEVEL)
            {
                var dbs = sortHighToLow == true ?
                    arrDicTempInventory.OrderByDescending(x => x.Value.equipment.m_norm_lv) :
                    arrDicTempInventory.OrderBy(x => x.Value.equipment.m_norm_lv);
                foreach (var v in dbs)
                {
                    temp_list.Add(key, v.Value);
                    key++;
                }
            }
            else // ### 강화 레벨 정렬 ###
            if (invn_sort_type == SortInventory.ENHANT_LEVEL)
            {
                var dbs = sortHighToLow == true ?
                    arrDicTempInventory.OrderByDescending(x => x.Value.equipment.m_ehnt_lv) :
                    arrDicTempInventory.OrderBy(x => x.Value.equipment.m_ehnt_lv);
                foreach (var v in dbs)
                {
                    temp_list.Add(key, v.Value);
                    key++;
                }
            }
            else // ### 전투력 정렬 ###
            if (invn_sort_type == SortInventory.COMBAT)
            {
                var dbs = sortHighToLow == true ?
                arrDicTempInventory.OrderByDescending(x => GameDatabase.GetInstance().tableDB.GetEquipCombatPower(x.Value.equipment, "total")) :
                arrDicTempInventory.OrderBy(x => GameDatabase.GetInstance().tableDB.GetEquipCombatPower(x.Value.equipment, "total"));
                foreach (var v in dbs)
                {
                    temp_list.Add(key, v.Value);
                    key++;
                }
            }
        }

        /// <summary>
        /// 신규 장비 표시 제거 
        /// </summary>
        public void SetEquipNewHide(Equipment get_eq)
        {
            get_eq.client_add_sp = 0;
            foreach (var v in temp_list.Keys)
            {
                if(string.Equals(temp_list[v].equipment.aInUid, get_eq.aInUid))
                {
                    LogPrint.Print(get_eq.aInUid);
                    temp_list[v] = new temp() { default0Eq1It2Sk3 = 1, equipment = get_eq };
                    break;
                }
            }

            int a_index = _equipment.FindIndex((Equipment eq) => eq.aInUid == get_eq.aInUid);
            if (a_index >= 0)
                _equipment[a_index] = get_eq;
        }

        /// <summary>
        /// 임시 인벤토리 리스트의 데이터를 변경 
        /// </summary>
        public void InitTempInventoryDataRefresh (Equipment get_eq)
        {
            foreach (var v in temp_list.Keys)
            {
                if (string.Equals(temp_list[v].equipment.aInUid, get_eq.aInUid))
                {
                    temp_list[v] = new temp() { equipment = get_eq };
                    break;
                }
            }
        }
        #endregion
        #endregion

        // ########################################################################################################
        // ################################ Item <아이템> #########################################################
        // ########################################################################################################
        #region ######## Item <아이템> #########################################################
        // 아이템 목록
        //  ty	        rt	        name
        //  20	        1	        일반 물약
        //  20	        2	        중급 물약
        //  20	        3	        고급 물약
        //  21	        1	        일반 장비 강화석
        //  21	        2	        중급 장비 강화석
        //  21	        3	        고급 장비 강화석
        //  21	        4	        희귀 장비 강화석
        //  22	        1	        일반 강화 축복 주문서
        //  22	        2	        중급 강화 축복 주문서
        //  22	        3	        고급 강화 축복 주문서
        //  23	        1	        입장권 : 도전의 탑 
        //  24	        1	        입장권 : 광산 
        //  25	        1	        입장권 : 레이드 
        //  26	        1	        부활석
        //  27	        1	        일반 장신구 강화석
        //  27	        2	        중급 장신구 강화석
        //  27	        3	        고급 장신구 강화석
        //  27	        4	        희귀 장신구 강화석
        //  28	        2	        중급 장비 조각
        //  28	        3	        고급 장비 조각
        //  28	        4	        희귀 장비 조각
        //  28	        5	        영웅 장비 조각
        //  28	        6	        고대 장비 조각
        //  28	        7	        전설 장비 조각
        //  29	        2	        중급 장신구 조각
        //  29	        3	        고급 장신구 조각
        //  29	        4	        희귀 장신구 조각
        //  29	        5	        영웅 장신구 조각
        //  29	        6	        고대 장신구 조각
        //  29	        7	        전설 장신구 조각
        //  30          1           입장권 : PvP 배틀 아레나 
        //  31          3           고급 펫 알
        //  31          4           희귀 펫 알
        //  31          5           영웅 펫 알



        public const int item_type_potion = 20; // 아이템 : 물약 타입 
        public const int item_type_eq_ston = 21; // 아이템 : 장비 강화석
        public const int item_type_ac_ston = 27; // 아이템 : 장신구 강화석
        public const int item_type_beless = 22; // 아이템 강화 축복 주문서 


        [System.Serializable]
        public struct Item
        {
            public string indate;
            public long aInUid;
            public int type; // 아이템 구분 
            public int rating; // 아이템 등급 
            public int count;   // 소지 수량

            public long client_add_sp; // 획득한 TimeSpan 
        }

        private static readonly List<Item> itemList = new List<Item>();
        public List<Item> _item = itemList;

        /// <summary> 아이템 리스트 </summary>
        public List<Item> GetItemAll() => _item;

        public List<TransactionParam> ParamTransactionItem()
        {
            int addCnt = 0;
            int addListCnt = 0;
            TransactionParam tParam = new TransactionParam();
            List<TransactionParam> tParamList = new List<TransactionParam>();
            foreach (var db in _item)
            {
                List<BackEnd.WRITE> writes = new List<BackEnd.WRITE> { new WRITE { Action = TransactionAction.Update, Param = ParamT.Collection(new ParamT.P[] { new ParamT.P() { k = "count", v = db.count } }) } };
                tParam.AddUpdateList(BackendGpgsMng.tableName_Item, db.indate, writes);

                addCnt++;
                if (addCnt >= 10)
                {
                    tParamList.Add(tParam);
                    tParam = new TransactionParam();
                    addCnt = 0;
                    addListCnt++;
                }
            }

            if(addCnt > 0 && addCnt < 10)
            {
                tParamList.Add(tParam);
            }

            return tParamList;
        }

        /// <summary>
        /// 아이템 정렬된 순서대로 리스트중 순차적 번호를 가지고 찾음 
        /// </summary>
        public Item GetListItemIndex(int index) => _item[index];

        private List<Item> temp_Item = new List<Item>();

        /// <summary>
        /// 선택 아이템 데이터 
        /// </summary>
        public Item GetItem(int it_ty, int it_rt)
        {
            int indx = _item.FindIndex((Item it) => int.Equals(it.type, it_ty) && int.Equals(it.rating, it_rt) && !string.IsNullOrEmpty(it.indate));
            if (indx >= 0)
            {
                return _item[indx];
            }
            else return new Item() { indate = string.Empty, aInUid = GetInstance().GetUniqueIDX(), type = it_ty, rating = it_rt, count = 0 };
        }

        /// <summary> 던전 입장권 </summary>
        public int GetItemDungeonTicket(IG.ModeType dg_mdty)
        {
            if (dg_mdty == IG.ModeType.DUNGEON_TOP)
            {
                int freTikCnt = GetInstance().tableDB.GetTableDB_Goods().m_fr_ticket_dg_top;
                return _item.Find(i => Equals(i.type, 23) && Equals(i.rating, 1)).count + freTikCnt;
            }
            else if (dg_mdty == IG.ModeType.DUNGEON_MINE)
            {
                int freTikCnt = GetInstance().tableDB.GetTableDB_Goods().m_fr_ticket_dg_mine;
                return _item.Find(i => Equals(i.type, 24) && Equals(i.rating, 1)).count + freTikCnt;
            }
            else if (dg_mdty == IG.ModeType.DUNGEON_RAID)
            {
                int freTikCnt = GetInstance().tableDB.GetTableDB_Goods().m_fr_ticket_dg_raid;
                return _item.Find(i => Equals(i.type, 25) && Equals(i.rating, 1)).count + freTikCnt;
            }
            else if (dg_mdty == IG.ModeType.PVP_BATTLE_ARENA)
            {
                int freTikCnt = GetInstance().tableDB.GetTableDB_Goods().m_fr_ticket_pvp_arena;
                return _item.Find(i => Equals(i.type, 30) && Equals(i.rating, 1)).count + freTikCnt;
            }

            return 0;
        }


        /// <summary>
        /// 입장 티켓
        /// type 23 : 도전의 탑, 24 : 광산, 25 : 레이드, 30 :pvp배틀 아레나
        /// </summary>
        public string GetStrDungeonTicket(IG.ModeType dg_mdty)
        {
            if (dg_mdty == IG.ModeType.DUNGEON_TOP)
            {
                int freTikCnt = GetInstance().tableDB.GetTableDB_Goods().m_fr_ticket_dg_top;
                int tikCnt = _item.Find(i => Equals(i.type, 23) && Equals(i.rating, 1)).count;
                return string.Format("x{0} (x{1})", tikCnt, freTikCnt);
            }
            else if (dg_mdty == IG.ModeType.DUNGEON_MINE)
            {
                int freTikCnt = GetInstance().tableDB.GetTableDB_Goods().m_fr_ticket_dg_mine;
                int tikCnt = _item.Find(i => Equals(i.type, 24) && Equals(i.rating, 1)).count;
                return string.Format("x{0} (x{1})", tikCnt, freTikCnt);
            }
            else if (dg_mdty == IG.ModeType.DUNGEON_RAID)
            {
                int freTikCnt = GetInstance().tableDB.GetTableDB_Goods().m_fr_ticket_dg_raid;
                int tikCnt = _item.Find(i => Equals(i.type, 25) && Equals(i.rating, 1)).count;
                return string.Format("x{0} (x{1})", tikCnt, freTikCnt);
            }
            else if (dg_mdty == IG.ModeType.PVP_BATTLE_ARENA)
            {
                int freTikCnt = GetInstance().tableDB.GetTableDB_Goods().m_fr_ticket_pvp_arena;
                int tikCnt = _item.Find(i => Equals(i.type, 30) && Equals(i.rating, 1)).count;
                return string.Format("x{0} (x{1})", tikCnt, freTikCnt);
            }

            return "x0 (x0)";
        }

        /// <summary> 던전 입장권 차감 </summary>
        public async Task<bool> ConsumDungeonTicket(IG.ModeType dg_mdty, int cnsm_cnt = 1)
        {
            LogPrint.EditorPrint(":cnsm_cnt : " + cnsm_cnt);
            LogPrint.Print("<color=red>입장권 차감 dg_mdty : " + dg_mdty + "</color>");

            var gds = GetInstance().tableDB.GetTableDB_Goods();
            if (dg_mdty == IG.ModeType.DUNGEON_TOP) // 도전의 탑 입장권 차감 
            {
                if (gds.m_fr_ticket_dg_top > 0)
                {
                    gds.m_fr_ticket_dg_top -= cnsm_cnt;
                    if(gds.m_fr_ticket_dg_top < 0)
                    {
                        cnsm_cnt = System.Math.Abs(gds.m_fr_ticket_dg_top);
                        gds.m_fr_ticket_dg_top = 0;
                    }

                    await GetInstance().tableDB.SetUpdateGoods(gds);
                    LogPrint.Print("도전의 탑 입장권 차감 gds.m_fr_ticket_dg_top : " + gds.m_fr_ticket_dg_top);
                }

                if(cnsm_cnt > 0)
                {
                    var it_db = _item.Find(i => i.type == 23 && i.rating == 1);
                    if (it_db.count > 0)
                    {
                        it_db.count -= cnsm_cnt;
                        await SendDataItem(it_db);
                        LogPrint.Print("도전의 탑 입장권 차감 it_db.count : " + it_db.count);
                    }
                }
            }
            else if (dg_mdty == IG.ModeType.DUNGEON_MINE) // 광산 입장권 차감 
            {
                if (gds.m_fr_ticket_dg_mine > 0)
                {
                    gds.m_fr_ticket_dg_mine -= cnsm_cnt;
                    if (gds.m_fr_ticket_dg_mine < 0)
                    {
                        cnsm_cnt = System.Math.Abs(gds.m_fr_ticket_dg_mine);
                        gds.m_fr_ticket_dg_mine = 0;
                    }

                    await GetInstance().tableDB.SetUpdateGoods(gds);
                    LogPrint.Print("광산 입장권 차감 it_db.count : " + gds.m_fr_ticket_dg_mine);
                }

                if (cnsm_cnt > 0)
                {
                    var it_db = _item.Find(i => i.type == 24 && i.rating == 1);
                    if (it_db.count > 0)
                    {
                        it_db.count -= cnsm_cnt;
                        await SendDataItem(it_db);
                        LogPrint.Print("광산 입장권 차감 it_db.count : " + it_db.count);
                    }
                }
            }
            else if (dg_mdty == IG.ModeType.DUNGEON_RAID) // 레이드 입장권 차감 
            {
                if (gds.m_fr_ticket_dg_raid > 0)
                {
                    gds.m_fr_ticket_dg_raid -= cnsm_cnt;
                    if (gds.m_fr_ticket_dg_raid < 0)
                    {
                        cnsm_cnt = System.Math.Abs(gds.m_fr_ticket_dg_raid);
                        gds.m_fr_ticket_dg_raid = 0;
                    }

                    await GetInstance().tableDB.SetUpdateGoods(gds);
                    LogPrint.Print("레이드 입장권 차감 gds.m_fr_ticket_dg_raid : " + gds.m_fr_ticket_dg_raid);
                }

                if (cnsm_cnt > 0)
                {
                    var it_db = _item.Find(i => i.type == 25 && i.rating == 1);
                    if (it_db.count > 0)
                    {
                        it_db.count -= cnsm_cnt;
                        await SendDataItem(it_db);
                        LogPrint.Print("레이드 입장권 차감 it_db.count : " + it_db.count);
                    }
                }
            }
            else if (dg_mdty == IG.ModeType.PVP_BATTLE_ARENA) // PvP 배틀 아레나 입장권 차감 
            {
                if (gds.m_fr_ticket_pvp_arena > 0)
                {
                    gds.m_fr_ticket_pvp_arena -= cnsm_cnt;
                    await GetInstance().tableDB.SetUpdateGoods(gds);
                    LogPrint.Print("PvP 배틀 아레나 입장권 차감 gds.m_fr_ticket_pvp_arena : " + gds.m_fr_ticket_pvp_arena);
                }
                else
                {
                    var it_db = _item.Find(i => i.type == 30 && i.rating == 1);
                    if (it_db.count > 0)
                    {
                        it_db.count -= cnsm_cnt;
                        await SendDataItem(it_db);
                        LogPrint.Print("PvP 배틀 아레나 입장권 차감 it_db.count : " + it_db.count);
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// 아이템 정렬 
        /// </summary>
        public void SortItem(string sort_type = "default")
        {
            _item.Sort((Item x, Item y) => x.type.CompareTo(y.type).CompareTo(x.rating.CompareTo(y.rating)));
            //_item.Sort((Item x, Item y) => x.type == y.type ? x.rating.CompareTo(y.rating) : (x.count > 0 ? -1 : 1));
        }

        /// <summary> 현재 아이템 데이터 정보 변경, 데이터가 없다면 새로 추가 </summary>
        public void UpdateClientDB_Item(Item itDb)
        {
            int indx = _item.FindIndex(it => string.Equals(it.indate, itDb.indate));
            if (indx >= 0)
                _item[indx] = itDb;
            else _item.Add(itDb);

            if (BackendGpgsMng.GetInstance().GetSceneActiveIndex() == 1)
            {
                if (MainUI.GetInstance().inventory.inventoryType != Inventory.InventoryType.Disable || MainUI.GetInstance().tapSmithy.smithyListType != SmithyListType.Disable)
                {
                    GameDatabase.GetInstance().tableDB.SetTempItemDbChange(itDb);
                }
            }
        }

        /// <summary>
        /// 장비 획득 해당 장비 테이블에 데이터 추가 및 업데이트 : void 
        /// </summary>
        public async Task<bool> SendDataItem(Item it_val)
        {
            var getItem = GetInstance().tableDB.GetItem(it_val.type, it_val.rating);
            int index = _item.FindIndex((Item it) => int.Equals(it.type, it_val.type) && int.Equals(it.rating, it_val.rating) && !string.IsNullOrEmpty(it.indate));
            if (index == -1) // 아이템 데이터 새로 추가 
            {
                LogPrint.Print("새로운 데이터를 Insert한다.");
                Param insert_param = new Param();
                insert_param.Add("aInUid", it_val.aInUid);
                insert_param.Add("type", it_val.type);
                insert_param.Add("rating", it_val.rating);
                insert_param.Add("count", it_val.count);
                string _insertInDate = await BackendGpgsMng.GetInstance().TaskInsertTableData(BackendGpgsMng.tableName_Item, insert_param);
                if (_insertInDate != string.Empty)
                {
                    it_val.client_add_sp = GameDatabase.GetInstance().GetUniqueIDX();
                    it_val.indate = _insertInDate;
                    UpdateClientDB_Item(it_val);
                }
            }
            else // 아이템 데이터 업데이트 
            {
                LogPrint.Print("기존 데이터를 Update한다.");
                var db = _item[index];
                if(db.count != it_val.count)
                {
                    Param update_param = new Param();
                    update_param.Add("count", it_val.count);

                    bool _taskResult = await BackendGpgsMng.GetInstance().TaskUpdateTableData(BackendGpgsMng.tableName_Item, it_val.indate, update_param);
                    if (_taskResult == true)
                    {
                        it_val.client_add_sp = GameDatabase.GetInstance().GetUniqueIDX();
                        UpdateClientDB_Item(it_val);
                    }
                }
            }

            return true;
        }

        /// <summary> 새로운 아이템 획득 </summary>
        public async Task SetAcquireItem(int it_ty, int it_rt, int it_cnt, bool is_field_drop)
        {
            int acq_type = it_ty;
            int acq_rating = it_rt;
            var temp = GetItem(acq_type, acq_rating);
            temp.count += it_cnt;

            if(string.IsNullOrEmpty(temp.indate))
            {
                Param pam = new Param();
                pam.Add("aInUid", temp.aInUid);
                pam.Add("type", temp.type);
                pam.Add("rating", temp.rating);
                pam.Add("count", temp.count);
                Task<string> _taskInsert = BackendGpgsMng.GetInstance().TaskInsertTableData(BackendGpgsMng.tableName_Item, pam);
                await _taskInsert;
                if (_taskInsert.Result != string.Empty)
                    temp.indate = _taskInsert.Result;
            }

            temp.client_add_sp = GameDatabase.GetInstance().GetUniqueIDX();
            UpdateClientDB_Item(temp);

            if (is_field_drop)
            {
                // UI Refresh 
                PopUpMng.GetInstance().Open_DropItem(temp);

                // 입장권 획득시 
                if(it_ty == 23 || it_ty == 24 || it_ty == 25 || it_ty == 30)
                    NotificationIcon.GetInstance().CheckNoticeContentsTicket();
            }
        }

        /// <summary>
        /// 아이템 판매 
        /// </summary>
        public async void SetSaleItem(Item val)
        {

        }

        /// <summary>
        /// 신규 아이템 표시 제거 
        /// </summary>
        public void SetItemNewHide(Item get_it)
        {
            get_it.client_add_sp = 0;
            foreach (var item in temp_list.Keys)
            {
                if (temp_list[item].item.type == get_it.type && temp_list[item].item.rating == get_it.rating)
                {
                    temp_list[item] = new temp() { default0Eq1It2Sk3 = 2, item = get_it };
                    break;
                }
            }

            int a_index = _item.FindIndex((Item it) => it.type == get_it.type && it.rating == get_it.rating);
            if (a_index >= 0)
                _item[a_index] = get_it;
        }
#endregion

        // ########################################################################################################
        // ################################ Skill <스킬> ##########################################################
        // ########################################################################################################
#region ######## Skill <스킬> ##########################################################
        [System.Serializable]
        public struct Skill
        {
            public string indate;
            public long aInUid;
            public int idx;     // 스킬 번호 
            public int level;   // 레벨 (소지수량가지고 레벨업) 
            public int count;   // 소지 수량 

            public int cliend_type; // 타입 = 스킬은 30 고정 
            public int cliend_rating; // 등급 (클라이언트 전용)
            public long client_add_sp; // 획득 당시 시간 (클라이언트 전용) 
        }

        public List<Skill> _skill = new List<Skill>();
        public List<Skill> GetSkillAll() => _skill;

        /// <summary> 현재 사용 매인 슬롯의 스킬을 가지고 내 전체 소지스킬중 중복안되는 스킬만 리턴 </summary> 
        /// <returns></returns>
        public List<Skill> GetUnusedSkillAll()
        {
            int useMainSlot = GetUseMainSlot();
            long[] lArr = new long[]
            {
                prefSkillSlot[useMainSlot].slot[0].aInUid,
                prefSkillSlot[useMainSlot].slot[1].aInUid,
                prefSkillSlot[useMainSlot].slot[2].aInUid,
                prefSkillSlot[useMainSlot].slot[3].aInUid,
                prefSkillSlot[useMainSlot].slot[4].aInUid,
                prefSkillSlot[useMainSlot].slot[5].aInUid,
            };

            return GetInstance().tableDB.GetSkillAll().FindAll((Skill s) => s.idx > 0 && (s.aInUid != lArr[0] && s.aInUid != lArr[1] && s.aInUid != lArr[2] && s.aInUid != lArr[3] && s.aInUid != lArr[4] && s.aInUid != lArr[5]));
        }

        public List<Skill> GetUseSkillAll()
        {
            int useMainSlot = GetUseMainSlot();
            long[] lArr = new long[]
            {
                prefSkillSlot[useMainSlot].slot[0].aInUid,
                prefSkillSlot[useMainSlot].slot[1].aInUid,
                prefSkillSlot[useMainSlot].slot[2].aInUid,
                prefSkillSlot[useMainSlot].slot[3].aInUid,
                prefSkillSlot[useMainSlot].slot[4].aInUid,
                prefSkillSlot[useMainSlot].slot[5].aInUid,
            };

            return GetInstance().tableDB.GetSkillAll().FindAll((Skill s) => s.aInUid != lArr[0] || s.aInUid != lArr[1] || s.aInUid != lArr[2] || s.aInUid != lArr[3] || s.aInUid != lArr[4] || s.aInUid != lArr[5]);
        }

        public Skill GetSkill(int sk_idx)
        {
            int indx = _skill.FindIndex((Skill sk) => sk.idx == sk_idx);
            if (indx >= 0)
                return _skill[indx];
            else return new Skill() { aInUid = GetInstance().GetUniqueIDX(), idx = sk_idx };
        }

        public Skill GetFindSkill(long uid)
        {
            int index = _skill.FindIndex((Skill sk) => long.Equals(sk.aInUid, uid));
            if (index >= 0)
                return _skill[index];
            else return new Skill();
        }

        /// <summary> 현재 스킬 데이터 정보 변경, 데이터가 없다면 새로 추가 </summary>
        public void UpdateClientDB_Skill(Skill val)
        {
            var chart = GameDatabase.GetInstance().chartDB.GetChartSkill_Data(val.idx);
            val.client_add_sp = GameDatabase.GetInstance().GetUniqueIDX();
            val.cliend_rating = chart.s_rating;
            val.cliend_type = chart.s_type;

            int index = _skill.FindIndex((Skill sk) => sk.idx == val.idx);
            if(index >= 0)
                _skill[index] = val;
            else _skill.Add(val);
        }

        /// <summary> 서버로 데이터 전송 </summary>
        public async Task<bool> SendDataSkill(Skill sednVal)
        {
            if (sednVal.idx > 0)
            {
                int index = _skill.FindIndex((Skill sk) => int.Equals(sk.idx, sednVal.idx));
                if (index == -1) // 아이템 데이터 새로 추가 
                {
                    LogPrint.Print("<color=yellow>Set DataSkill 새로운 데이터를 Insert한다.</color>");
                    Param param_ins = new Param();
                    param_ins.Add("aInUid", sednVal.aInUid);
                    param_ins.Add("idx", sednVal.idx);
                    param_ins.Add("level", sednVal.level);
                    param_ins.Add("count", sednVal.count);
                    Task<string> _taskInsert = BackendGpgsMng.GetInstance().TaskInsertTableData(BackendGpgsMng.tableName_Skill, param_ins);
                    await _taskInsert;
                    if (_taskInsert.Result != string.Empty)
                    {
                        sednVal.indate = _taskInsert.Result;
                        UpdateClientDB_Skill(sednVal);
                    }
                }
                // 데이터 업데이트 
                else
                {
                    var skillData = GetSkillAll()[index];
                    Param param_upd = new Param();
                    if (skillData.level != sednVal.level) param_upd.Add("level", sednVal.level);
                    if (skillData.count != sednVal.count) param_upd.Add("count", sednVal.count);

                    LogPrint.Print("<color=yellow>Set DataSkill 기존 데이터를 Update한다 Cnt : " + param_upd.GetValue().Count + ", " + param_upd.GetJson() + ".</color>");

                    Task<bool> _taskUpdate = BackendGpgsMng.GetInstance().TaskUpdateTableData(BackendGpgsMng.tableName_Skill, sednVal.indate, param_upd);
                    await _taskUpdate;
                    if (_taskUpdate.Result)
                    {
                        UpdateClientDB_Skill(sednVal);
                    }
                }
            }

            return true;
        }

        public bool GetNoticeCompleteCheckSkill()
        {
            foreach (var _sk in GetSkillAll())
            {
                bool isMaxLevel = _sk.level >= GameDatabase.GetInstance().chartDB.GetChartSkill_MaxLevel();
                if (!isMaxLevel)
                {
                    int upNeedCnt = GameDatabase.GetInstance().chartDB.GetChartSkill_UpNeedCount(_sk.level);
                    if(_sk.count >= upNeedCnt)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary> 스킬 드롭 획득( Insert or Update) </summary>
        //public async void SetAcquireSkill (cdb_r_field_chest fld_chest)
        //{
        //    if (GetInstance().inventoryDB.CheckIsEmpty() == false) // 인벤 부족   
        //    {
        //        PopUpMng.GetInstance().Open_MessageError("system.inventory.zero");
        //        return;
        //    }

        //    var temp = GetSkill(fld_chest.sk_id);
        //    temp.count += fld_chest.sk_cn;
        //    if(string.IsNullOrEmpty(temp.indate))
        //    {
        //        // 신규 스킬 -> 즉시 서버로 인서트 
        //        Param p1 = new Param();
        //        p1.Add("aInUid", temp.aInUid);
        //        p1.Add("idx", temp.idx);
        //        p1.Add("level", temp.level);
        //        p1.Add("count", temp.count);
        //        Task<string> t1 = BackendGpgsMng.GetInstance().TaskInsertTableData(BackendGpgsMng.tableName_Skill, p1);
        //        await t1;
        //        if (t1.Result != string.Empty)
        //        {
        //            temp.indate = t1.Result;
        //        }
        //    }

        //    temp.client_add_sp = GameDatabase.GetInstance().GetUniqueIDX();
        //    UpdateClientDB_Skill(temp);

        //    // UI Refresh 
        //    PopUpMng.GetInstance().OpenDropSkill(temp);
        //    MainUI.GetInstance().NewEquipItemInventortRefresh();
        //    MainUI.GetInstance().NewSkillScrollRefresh();
        //}

        //#################################################################################
        #region # 스킬 : 슬롯 #
        private static readonly Dictionary<int, PrefabSkillSlot> _prefabSkillSlot = new Dictionary<int, PrefabSkillSlot>();
        private Dictionary<int, PrefabSkillSlot> prefSkillSlot = _prefabSkillSlot;

        /// <summary> 현재 사용중인 매인 스킬 슬롯 </summary>
        public int GetUseMainSlot () => PlayerPrefs.GetInt(PrefsKeys.prefabKey_SkillUseMainSlot);
        public PrefabSkillSlot GetSkillSlot(int m_slot)
        {
            if(prefSkillSlot.ContainsKey(m_slot))
            {
                return prefSkillSlot[m_slot];
            }
            else
            return new PrefabSkillSlot();
        }

        /// <summary> 스킬 슬롯에 장착 </summary>
        public void SetSkillSlotChange(int use_main_slot, int wear_slot_num, long wear_uid, bool change)
        {
            LogPrint.Print("use_main_slot : " + use_main_slot + ", wear_slot_num : " + wear_slot_num + ", wear_uid : " + wear_uid);
            if (prefSkillSlot.ContainsKey(use_main_slot))
            {
                if (change == false)
                {
                    // 중복 방지 -> 착용하려는 스킬이 이미 있다면 먼저 제거 
                    for (int i = 0; i < prefSkillSlot[use_main_slot].slot.Length; i++)
                    {
                        if (prefSkillSlot[use_main_slot].slot[i].aInUid == wear_uid)
                        {
                            prefSkillSlot[use_main_slot].slot[i].aInUid = 0;
                        }
                    }

                    prefSkillSlot[use_main_slot].slot[wear_slot_num].aInUid = wear_uid;
                }
                else
                {
                    int now_sk_slot = -1;
                    long now_sk_slot_uid = prefSkillSlot[use_main_slot].slot[wear_slot_num].aInUid;
                    for (int i = 0; i < prefSkillSlot[use_main_slot].slot.Length; i++)
                    {
                        if (prefSkillSlot[use_main_slot].slot[i].aInUid == wear_uid)
                        {
                            now_sk_slot = i;
                            break;
                        }
                    }

                    if(now_sk_slot >= 0 && now_sk_slot_uid > 0)
                    {
                        LogPrint.Print(" 111 chng_temp_uid [ " + now_sk_slot + " ] : " + now_sk_slot_uid);
                        LogPrint.Print(" 222 chng_temp_uid [ " + wear_slot_num + " ] : " + wear_uid);

                        prefSkillSlot[use_main_slot].slot[now_sk_slot].aInUid = now_sk_slot_uid;
                        prefSkillSlot[use_main_slot].slot[wear_slot_num].aInUid = wear_uid;
                    }
                }
                
                string mapper = JsonMapper.ToJson(prefSkillSlot.ToArray());
                PlayerPrefs.SetString(PrefsKeys.prefabKey_SkillSlot, mapper);
            }

            LogPrint.Print(PlayerPrefs.GetString(PrefsKeys.prefabKey_SkillSlot));
        }

        public void SetSkilRelease (int use_main_slot, long rel_uid)
        {
            if (prefSkillSlot.ContainsKey(use_main_slot))
            {
                for (int i = 0; i < prefSkillSlot[use_main_slot].slot.Length; i++)
                {
                    if (prefSkillSlot[use_main_slot].slot[i].aInUid == rel_uid)
                    {
                        prefSkillSlot[use_main_slot].slot[i].aInUid = 0;
                        break;
                    }
                }

                string mapper = JsonMapper.ToJson(prefSkillSlot.ToArray());
                PlayerPrefs.SetString(PrefsKeys.prefabKey_SkillSlot, mapper);
            }
        }

        public int GetIsNowWearing(int use_main_slot, long uid)
        {
            if(prefSkillSlot.ContainsKey(use_main_slot))
            {
                int slot = 0;
                foreach (var item in prefSkillSlot[use_main_slot].slot)
                {
                    slot++;
                    if (item.aInUid == uid)
                        return slot;
                }
            }

            return -1;
        }

        public void InitSkillSlot()
        {
            prefSkillSlot.Clear();
            JsonData loadJsn = JsonMapper.ToObject(PlayerPrefs.GetString(PrefsKeys.prefabKey_SkillSlot));
            if (string.IsNullOrEmpty(loadJsn.ToJson()) == true) // 새로 세팅 
            {
                for (int i = 0; i < 3; i++)
                {
                    PrefabSkillSlot product = new PrefabSkillSlot()
                    {
                        main_slot_num = i,
                        use = i == 0 ? 1 : 0,
                        slot = new PrefabSkillSlot.Slot[6]
                    };
                    prefSkillSlot.Add(i, product);
                }

                loadJsn = JsonMapper.ToObject(JsonMapper.ToJson(prefSkillSlot.ToArray()));
            }
            
            foreach (JsonData jd in loadJsn)
            {
                int key = System.Convert.ToInt32(jd["Key"].ToString());
                long aInUid_0 = System.Convert.ToInt64(jd["Value"]["slot"][0]["aInUid"].ToString());
                long aInUid_1 = System.Convert.ToInt64(jd["Value"]["slot"][1]["aInUid"].ToString());
                long aInUid_2 = System.Convert.ToInt64(jd["Value"]["slot"][2]["aInUid"].ToString());
                long aInUid_3 = System.Convert.ToInt64(jd["Value"]["slot"][3]["aInUid"].ToString());
                long aInUid_4 = System.Convert.ToInt64(jd["Value"]["slot"][4]["aInUid"].ToString());
                long aInUid_5 = System.Convert.ToInt64(jd["Value"]["slot"][5]["aInUid"].ToString());

                PrefabSkillSlot pss = new PrefabSkillSlot()
                {
                    main_slot_num = System.Convert.ToInt32(jd["Value"]["main_slot_num"].ToString()),
                    use = System.Convert.ToInt32(jd["Value"]["use"].ToString()),
                    slot = new PrefabSkillSlot.Slot[]
                     {
                         new PrefabSkillSlot.Slot() { aInUid = string.Equals(aInUid_0, GetFindSkill(aInUid_0).aInUid) ? aInUid_0 : 0 },
                         new PrefabSkillSlot.Slot() { aInUid = string.Equals(aInUid_1, GetFindSkill(aInUid_1).aInUid) ? aInUid_1 : 0 },
                         new PrefabSkillSlot.Slot() { aInUid = string.Equals(aInUid_2, GetFindSkill(aInUid_2).aInUid) ? aInUid_2 : 0 },
                         new PrefabSkillSlot.Slot() { aInUid = string.Equals(aInUid_3, GetFindSkill(aInUid_3).aInUid) ? aInUid_3 : 0 },
                         new PrefabSkillSlot.Slot() { aInUid = string.Equals(aInUid_4, GetFindSkill(aInUid_4).aInUid) ? aInUid_4 : 0 },
                         new PrefabSkillSlot.Slot() { aInUid = string.Equals(aInUid_5, GetFindSkill(aInUid_5).aInUid) ? aInUid_5 : 0 },
                     },
                };

                prefSkillSlot[key] = pss;
            }

            string save_mapper = JsonMapper.ToJson(prefSkillSlot.ToArray());
            PlayerPrefs.SetString(PrefsKeys.prefabKey_SkillSlot, save_mapper);
        }
        #endregion
        // END 슬롯 

        #endregion

        // ########################################################################################################
        // ################################ Skill <스킬> ##########################################################
        // ########################################################################################################
#region ######## Pet 펫, PetEncy 펫 도감 ##########################################################
        [System.Serializable]
        public struct Pet
        {
            public string indate;
            public long aInUid; // 장비 고유 ID 
            /// <summary> -1:판매하였음, 0:보유중, 1: 1번 슬롯 착용중, 2: 2번 슬롯 착용중, 3: 3번 슬롯 착용중 </summary>
            public int m_state;
            public int p_id; // 펫 번호 
            public int p_rt; // 펫 등급 
            public int p_lv; // 펫 레벨 
            public float p_lv_residual; // 레벨업 남은 값 
            public int m_lck; // 잠금 
            public StatOp sOp1; // 전용 옵션 1
            public StatOp sOp2; // 전용 옵션 2
            public StatOp sOp3; // 전용 옵션 3
            public PetStatOp statOp; // 옵션 스텟 
        }

        [System.Serializable]
        public struct PetEncy
        {
            public string indate;
            public long aInUid; // 장비 고유 ID 
            public int p_rt;
            public int p_id;
            public int incr_cnt; // 획득 카운트
        }

        private static readonly List<Pet> pet_list = new List<Pet>();
        protected List<Pet> _pet_list = pet_list;

        private static readonly List<PetEncy> pet_encylist = new List<PetEncy>();
        protected List<PetEncy> _pet_encylist = pet_encylist;

        private List<Pet> pet_list_Management = new List<Pet>();
        private List<Pet> pet_list_LevelUp = new List<Pet>();
        private List<Pet> pet_list_OptionChange = new List<Pet>();
        private List<Pet> pet_list_Synthesis = new List<Pet>();

        public void SetPetTypeAddSort(PetTapType petTapType)
        {
            if (petTapType == PetTapType.Management)
            {
                pet_list_Management.Clear();
                foreach (var item in _pet_list)
                {
                    if(item.m_state >= 0)
                        pet_list_Management.Add(item);
                }

                pet_list_Management.Sort((x, y) => y.p_rt.CompareTo(x.p_rt));
            }
            else if (petTapType == PetTapType.LevelUp)
            {
                pet_list_LevelUp.Clear();
                Pet ingnrPetDB = MainUI.GetInstance().tapPet.NowPetDB;
                foreach (var item in _pet_list)
                {
                    if(item.aInUid != ingnrPetDB.aInUid && item.m_state == 0 && item.p_rt >= 3 && item.p_rt <= ingnrPetDB.p_rt)
                        pet_list_LevelUp.Add(item);
                }

                pet_list_LevelUp.Sort((x, y) => x.p_rt.CompareTo(y.p_rt));
            }
            else if (petTapType == PetTapType.OptionChange)
            {
                pet_list_OptionChange.Clear();
                foreach (var item in _pet_list)
                {
                    if (item.m_state >= 0 && item.p_rt >= 3)
                        pet_list_OptionChange.Add(item);
                }

                pet_list_OptionChange.Sort((x, y) => y.p_rt.CompareTo(x.p_rt));
            }
            else if (petTapType == PetTapType.Synthesis)
            {
                pet_list_Synthesis.Clear();
                foreach (var item in _pet_list)
                {
                    var cdbPet = GetInstance().chartDB.GetCdbPet(item.p_rt, item.p_id);
                    if(item.m_state >= 0 /*&& item.p_lv >= cdbPet.max_lv*/ && item.p_rt >= 3 && item.p_rt < 7)
                        pet_list_Synthesis.Add(item);
                }

                pet_list_Synthesis.Sort((x, y) => y.p_lv.CompareTo(x.p_lv));
            }
            else if (petTapType == PetTapType.Encyclopedia)
            {
                _pet_encylist.Sort((x, y) => x.p_rt.CompareTo(y.p_rt));
            }
        }

        /// <summary>
        /// 보유 펫 : 리스트 idx로 찾기 
        /// </summary>
        public Pet GetIndexPet(PetTapType petTapType, int indx)
        {
            if (petTapType == PetTapType.Management)
                return pet_list_Management[indx];
            else if (petTapType == PetTapType.LevelUp)
                return pet_list_LevelUp[indx];
            else if (petTapType == PetTapType.OptionChange)
                return pet_list_OptionChange[indx];
            else if (petTapType == PetTapType.Synthesis)
                return pet_list_Synthesis[indx];

            return default;
        }

        /// <summary>
        /// 펫 도감 : 리스트 idx로 찾기 
        /// </summary>
        public PetEncy GetIndexPetEncy(PetTapType petTapType, int indx) => _pet_encylist[indx];

        /// <summary>
        /// 보유 펫 : 고유 idx로 찾기 
        /// </summary>
        public Pet GetUniqIdxPet(PetTapType petTapType, long uniq_idx)
        {
            if (petTapType == PetTapType.Management)
            {
                int indx = pet_list_Management.FindIndex(obj => obj.aInUid == uniq_idx);
                if (indx >= 0)
                    return pet_list_Management[indx];
            }
            else if (petTapType == PetTapType.LevelUp)
            {
                int indx = pet_list_LevelUp.FindIndex(obj => obj.aInUid == uniq_idx);
                if (indx >= 0)
                    return pet_list_LevelUp[indx];
            }
            else if (petTapType == PetTapType.OptionChange)
            {
                int indx = pet_list_OptionChange.FindIndex(obj => obj.aInUid == uniq_idx);
                if (indx >= 0)
                    return pet_list_OptionChange[indx];
            }
            else if (petTapType == PetTapType.Synthesis)
            {
                int indx = pet_list_Synthesis.FindIndex(obj => obj.aInUid == uniq_idx);
                if (indx >= 0)
                    return pet_list_Synthesis[indx];
            }

            return default;
        }
        /// <summary>
        /// 펫 도감 : rt, id로 찾기 
        /// </summary>
        public PetEncy GetRatingIdPetEncy(int rt, int id)
        {
            int indx = _pet_encylist.FindIndex(obj => obj.p_rt == rt && obj.p_id == id);
            if (indx >= 0)
                return _pet_encylist[indx];

            return default;
        }

        public List<Pet> GetAllPets() => _pet_list;

        public List<Pet> GetAllPet(PetTapType petTapType)
        {
            if (petTapType == PetTapType.Management)
            {
                return pet_list_Management;
            }
            else if (petTapType == PetTapType.LevelUp)
            {
                return pet_list_LevelUp;
            }
            else if (petTapType == PetTapType.OptionChange)
            {
                return pet_list_OptionChange;
            }
            else if (petTapType == PetTapType.Synthesis)
            {
                return pet_list_Synthesis;
            }

            return default;
        }

        public List<PetEncy> GetAllPetEncy()
        {
            return _pet_encylist;
        }

        public Param ParamCollectionPet(Pet _petDB)
        {
            Param pam = new Param();
            pam.Add("aInUid", _petDB.aInUid);
            pam.Add("m_state", _petDB.m_state);
            pam.Add("p_id", _petDB.p_id);
            pam.Add("p_rt", _petDB.p_rt);
            pam.Add("p_lv", _petDB.p_lv);
            pam.Add("p_lv_residual", _petDB.p_lv_residual);
            pam.Add("m_lck", _petDB.m_lck);
            pam.Add("sOp1", JsonUtility.ToJson(_petDB.sOp1));
            pam.Add("sOp2", JsonUtility.ToJson(_petDB.sOp2));
            pam.Add("sOp3", JsonUtility.ToJson(_petDB.sOp3));
            pam.Add("statOp", JsonUtility.ToJson(_petDB.statOp));
            return pam;
        }

        public Param ParamCollectionPet(PetEncy _petDB)
        {
            Param pam = new Param();
            pam.Add("aInUid", _petDB.aInUid);
            pam.Add("p_id", _petDB.p_id);
            pam.Add("p_rt", _petDB.p_rt);
            pam.Add("incr_cnt", _petDB.incr_cnt);
            return pam;
        }

        /// <summary>
        /// backend 장비 테이블에 데이터 추가 
        /// </summary>
        public async Task<bool> SendDataPet(Pet val, string type)
        {
            if(val.p_rt >= 5 || (string.IsNullOrEmpty(val.indate) && string.Equals(type, "update")))
            {
                if (string.IsNullOrEmpty(val.indate) && !string.Equals(type, "delete"))
                {
                    var findNotUsePetDBIndex = _pet_list.FindIndex(obj => obj.m_state == -1 && !string.IsNullOrEmpty(obj.indate));
                    if (findNotUsePetDBIndex >= 0)
                    {
                        type = "update";
                        val.indate = _pet_list[findNotUsePetDBIndex].indate;
                        _pet_list[findNotUsePetDBIndex] = val;
                    }
                }

                Param add_pam = ParamCollectionPet(val);
                if (string.Equals(type, "insert") && string.IsNullOrEmpty(val.indate))  // 새로운 장비 새로 추가 
                {
                    Task<string> _taskInsert = BackendGpgsMng.GetInstance().TaskInsertTableData(BackendGpgsMng.tableName_Pet, add_pam);
                    await _taskInsert;
                    if (_taskInsert.Result != string.Empty)
                    {
                        val.indate = _taskInsert.Result;
                        UpdateClientDB_Pet(val);
                        return true;
                    }
                }
                else if (string.Equals(type, "update") || string.Equals(type, "delete")) // 기존 장비 업데이트 
                {
                    if (!string.IsNullOrEmpty(val.indate)) // indate가 있다. (서버에 데이터가 존재함) 
                    {
                        Task<bool> _taskUpdate = BackendGpgsMng.GetInstance().TaskUpdateTableData(BackendGpgsMng.tableName_Pet, val.indate, add_pam);
                        await _taskUpdate;
                        if (_taskUpdate.Result)
                        {
                            UpdateClientDB_Pet(val, string.Equals(type, "delete"));
                        }
                    }
                    else // Equipment 테이블에 없는 정보 
                    {
                        UpdateClientDB_Pet(val, string.Equals(type, "delete"));
                    }
                }
            }
            else
            {
                UpdateClientDB_Pet(val, string.Equals(type, "delete"));
            }

            return true;
        }

        public void UpdateClientDB_Pet(Pet _petDB, bool _delete = false)
        {
            if (_petDB.aInUid == 0)
                return;

            if (_delete == false)
            {
                int index = _pet_list.FindIndex(obj => long.Equals(obj.aInUid, _petDB.aInUid));
                if (index >= 0)
                    _pet_list[index] = _petDB;
                else
                    _pet_list.Add(_petDB);
            }
            else
            {
                int indx = _pet_list.FindIndex(obj => long.Equals(obj.aInUid, _petDB.aInUid));
                if (string.IsNullOrEmpty(_petDB.indate))
                {
                    if (indx >= 0)
                        _pet_list.RemoveAt(indx);
                }
                else
                {
                    _petDB.m_state = -1;
                    _pet_list[indx] = _petDB;
                }
            }
        }

        public void UpdateClientDB_PetEncy(PetEncy _petDB, bool _delete = false)
        {
            if (_petDB.aInUid == 0)
                return;

            int index = _pet_encylist.FindIndex(obj => long.Equals(obj.aInUid, _petDB.aInUid));
            if (index >= 0)
                _pet_encylist[index] = _petDB;
            else
                _pet_encylist.Add(_petDB);
        }

        /// <summary>
        /// 각 탭별로 리스트 카운트 
        /// </summary>
        public int GetPetCount(PetTapType petTapType)
        {
            if (petTapType == PetTapType.Management)
            {
                return pet_list_Management.Count;
            }
            else if (petTapType == PetTapType.LevelUp)
            {
                return pet_list_LevelUp.Count;
            }
            else if (petTapType == PetTapType.OptionChange)
            {
                return pet_list_OptionChange.Count;
            }
            else if (petTapType == PetTapType.Synthesis)
            {
                return pet_list_Synthesis.Count;
            }
            else if (petTapType == PetTapType.Encyclopedia)
            {
                return GameDatabase.GetInstance().chartDB.GetCdbPetAll().Count;
            }

            return 0;
        }

        /// <summary>
        /// 고유 idx로 찾기 
        /// </summary>
        public Pet GetUniqIdxPet(long uniq_idx)
        {
            int indx = _pet_list.FindIndex(obj => obj.aInUid == uniq_idx);
            if (indx >= 0)
            {
                return _pet_list[indx];
            }
            else return default;
        }
        

        /// <summary>
        /// 사용중인 펫 
        /// </summary>
        public Pet GetUsePet()
        {
            int indx = _pet_list.FindIndex(obj => obj.m_state >= 1);
            if (indx >= 0)
                return _pet_list[indx];
            else return default;
        }

        /// <summary>
        /// 동행중인 펫은 먼저 m_state = 0 변경
        /// 동행할 펫은 m_state = 1 변경 
        /// 트랜젝션으로 서버 전송
        /// </summary>
        public async Task SetPetUseState (PetTapType ptt, long uniq_idx)
        {
            int indx = _pet_list.FindIndex(obj => long.Equals(obj.aInUid, uniq_idx));
            if(indx >= 0)
            {
                TransactionParam trct_prm = new TransactionParam();
                // 해제 : 먼저 동행중인 펫이 있으면 m_state = 0 변경 후 트랜젝션에 보관 
                var relse_tmps = _pet_list.FindAll(obj => obj.m_state > 0 && !long.Equals(obj.aInUid, uniq_idx));
                if(relse_tmps.Count > 0)
                {
                    foreach (var item in relse_tmps)
                    {
                        var tmp = item;
                        tmp.m_state = 0;
                        if (tmp.p_rt >= 5 || !string.IsNullOrEmpty(tmp.indate))
                        {
                            trct_prm.AddUpdateList(BackendGpgsMng.tableName_Pet, tmp.indate, new List<BackEnd.WRITE> { new WRITE { Action = TransactionAction.Update,
                                Param = ParamT.Collection(new ParamT.P[] { new ParamT.P() { k = "m_state", v = tmp.m_state } }) } });
                        }

                        UpdateClientDB_Pet(tmp);
                    }
                }

                // 동행 : 새로 동행 할 펫의 m_tate = 0 or 1 변경 
                var select_tmp = _pet_list[indx];
                select_tmp.m_state = string.Equals(select_tmp.aInUid, uniq_idx) && int.Equals(select_tmp.m_state, 1) ? 0 : 1;
                if (select_tmp.p_rt >= 5 || !string.IsNullOrEmpty(select_tmp.indate))
                {
                    trct_prm.AddUpdateList(BackendGpgsMng.tableName_Pet, select_tmp.indate, new List<BackEnd.WRITE> { new WRITE { Action = TransactionAction.Update,
                        Param = ParamT.Collection(new ParamT.P[] { new ParamT.P() { k = "m_state", v = select_tmp.m_state } }) } });
                }

                LogPrint.EditorPrint("trct_prm.GetWriteValues().count : " + trct_prm.GetWriteValues().Count);
                if(trct_prm.GetWriteValues().Count > 0)
                {
                    BackendReturnObject trct_bro1 = null;
                    SendQueue.Enqueue(Backend.GameInfo.TransactionWrite, trct_prm, callback => { trct_bro1 = callback; });
                    while (trct_bro1 == null) { await Task.Delay(100); }
                }

                UpdateClientDB_Pet(select_tmp);
                SetPetTypeAddSort(ptt);
            }
        }

        /// <summary>
        /// 펫 전용 옵션 
        /// </summary>
        public List<StatOp> GetPetRandomSpecialOption(int p_rt)
        {
            List<GameDatabase.TableDB.StatOp> ovrp_sopID = new List<GameDatabase.TableDB.StatOp>() { new StatOp(), new StatOp(), new StatOp() };
            float r_pc = GetInstance().GetRandomPercent(), n_pc = 0f;
            int forCnt = p_rt == 5 ? 2 : p_rt == 6 || p_rt == 7 ? 3 : 1;
            for (int i = 0; i < forCnt; i++)
            {
                bool ovrp = false;
                if(p_rt == 6 && i == 2)
                {
                    if (GameDatabase.GetInstance().GetRandomPercent() < 50)
                        ovrp = false;
                    else ovrp = true;
                }

                while (!ovrp)
                {
                    foreach (var item in list_cdb_pet_sop)
                    {
                        n_pc += item.r_pct;
                        if (r_pc <= n_pc)
                        {
                            if (ovrp_sopID.FindIndex(obj => obj.id == item.sop_id) == -1)
                            {
                                ovrp_sopID[i] = new StatOp() { id = item.sop_id, rlv = GetInstance().GetStatRandomLevel() };
                                ovrp = true;
                                break;
                            }
                        }
                    }
                }
            }

            return ovrp_sopID;
        }

        /// <summary>
        /// 옵션 스탯에 사용될 옵션 
        /// 공격력 (stat id : 1) 
        /// 방어력 (stat id : 2) 
        /// 치명타 공격력 (stat id : 4) 
        /// 체력 (stat id : 5) 
        /// 치명타 방어력 (stat id : 8) 
        /// </summary>
        public PetStatOp GetPetRandomOption(int rt, bool isSynySucs = false, GameDatabase.TableDB.PetStatOp tmpPetStOp = default)
        {
            if (dic_cdb_equip_op.TryGetValue(0, out cdb_equip_op ops))
            {
                var tmp_pso = new PetStatOp();
                string[] op_array = ops.op1.Split(','); // 랜덤 옵션 1
                switch (rt)
                {
                    case 3: // 고급 1개 확정
                        if (!isSynySucs)
                        {
                            tmp_pso.op1 = new StatOp() { id = int.Parse(op_array[UnityEngine.Random.Range(0, op_array.Length)]), rlv = GetInstance().GetStatRandomLevel() };
                        }
                        else tmp_pso = tmpPetStOp;
                        break;
                    case 4: // 희귀 2개 확정
                        if (!isSynySucs)
                        {
                            tmp_pso.op1 = new StatOp() { id = int.Parse(op_array[UnityEngine.Random.Range(0, op_array.Length)]), rlv = GetInstance().GetStatRandomLevel() };
                            tmp_pso.op2 = new StatOp() { id = int.Parse(op_array[UnityEngine.Random.Range(0, op_array.Length)]), rlv = GetInstance().GetStatRandomLevel() };
                        }
                        else
                        {
                            tmp_pso.op1 = tmpPetStOp.op1;
                            tmp_pso.op2 = new StatOp() { id = int.Parse(op_array[UnityEngine.Random.Range(0, op_array.Length)]), rlv = GetInstance().GetStatRandomLevel() };
                        }
                        break;
                    case 5: // 영웅 3개 확정 2개 랜덤 = 최대 5개 
                        if (!isSynySucs)
                        {
                            tmp_pso.op1 = new StatOp() { id = int.Parse(op_array[UnityEngine.Random.Range(0, op_array.Length)]), rlv = GetInstance().GetStatRandomLevel() };
                            tmp_pso.op2 = new StatOp() { id = int.Parse(op_array[UnityEngine.Random.Range(0, op_array.Length)]), rlv = GetInstance().GetStatRandomLevel() };
                            tmp_pso.op3 = new StatOp() { id = int.Parse(op_array[UnityEngine.Random.Range(0, op_array.Length)]), rlv = GetInstance().GetStatRandomLevel() };

                            if (GetInstance().GetRandomPercent() < 35)
                                tmp_pso.op4 = new StatOp() { id = int.Parse(op_array[UnityEngine.Random.Range(0, op_array.Length)]), rlv = GetInstance().GetStatRandomLevel() };
                            if (GetInstance().GetRandomPercent() < 35)
                                tmp_pso.op5 = new StatOp() { id = int.Parse(op_array[UnityEngine.Random.Range(0, op_array.Length)]), rlv = GetInstance().GetStatRandomLevel() };
                        }
                        else
                        {
                            tmp_pso.op1 = tmpPetStOp.op1;
                            tmp_pso.op2 = tmpPetStOp.op2;
                            if (tmp_pso.op3.id > 0)
                                tmp_pso.op3 = tmpPetStOp.op3;
                            else tmp_pso.op3 = new StatOp() { id = int.Parse(op_array[UnityEngine.Random.Range(0, op_array.Length)]), rlv = GetInstance().GetStatRandomLevel() };

                            if (tmpPetStOp.op4.id <= 0 && GetInstance().GetRandomPercent() < 35)
                                tmp_pso.op4 = new StatOp() { id = int.Parse(op_array[UnityEngine.Random.Range(0, op_array.Length)]), rlv = GetInstance().GetStatRandomLevel() };
                            if (tmpPetStOp.op5.id <= 0 && GetInstance().GetRandomPercent() < 35)
                                tmp_pso.op5 = new StatOp() { id = int.Parse(op_array[UnityEngine.Random.Range(0, op_array.Length)]), rlv = GetInstance().GetStatRandomLevel() };
                        }
                        break;
                    case 6: // 고대 4개 확정 2개 랜덤 = 최대 6개 
                        if (!isSynySucs)
                        {
                            tmp_pso.op1 = new StatOp() { id = int.Parse(op_array[UnityEngine.Random.Range(0, op_array.Length)]), rlv = GetInstance().GetStatRandomLevel() };
                            tmp_pso.op2 = new StatOp() { id = int.Parse(op_array[UnityEngine.Random.Range(0, op_array.Length)]), rlv = GetInstance().GetStatRandomLevel() };
                            tmp_pso.op3 = new StatOp() { id = int.Parse(op_array[UnityEngine.Random.Range(0, op_array.Length)]), rlv = GetInstance().GetStatRandomLevel() };
                            tmp_pso.op4 = new StatOp() { id = int.Parse(op_array[UnityEngine.Random.Range(0, op_array.Length)]), rlv = GetInstance().GetStatRandomLevel() };

                            if (GetInstance().GetRandomPercent() < 35)
                                tmp_pso.op5 = new StatOp() { id = int.Parse(op_array[UnityEngine.Random.Range(0, op_array.Length)]), rlv = GetInstance().GetStatRandomLevel() };
                            if (GetInstance().GetRandomPercent() < 35)
                                tmp_pso.op6 = new StatOp() { id = int.Parse(op_array[UnityEngine.Random.Range(0, op_array.Length)]), rlv = GetInstance().GetStatRandomLevel() };
                        }
                        else
                        {
                            tmp_pso.op1 = tmpPetStOp.op1;
                            tmp_pso.op2 = tmpPetStOp.op2;
                            tmp_pso.op3 = tmpPetStOp.op3;
                            if (tmp_pso.op4.id > 0)
                                tmp_pso.op4 = tmpPetStOp.op4;
                            else tmp_pso.op4 = new StatOp() { id = int.Parse(op_array[UnityEngine.Random.Range(0, op_array.Length)]), rlv = GetInstance().GetStatRandomLevel() };

                            if (tmpPetStOp.op5.id <= 0 && GetInstance().GetRandomPercent() < 35)
                                tmp_pso.op5 = new StatOp() { id = int.Parse(op_array[UnityEngine.Random.Range(0, op_array.Length)]), rlv = GetInstance().GetStatRandomLevel() };
                            if (tmpPetStOp.op6.id <= 0 && GetInstance().GetRandomPercent() < 35)
                                tmp_pso.op6 = new StatOp() { id = int.Parse(op_array[UnityEngine.Random.Range(0, op_array.Length)]), rlv = GetInstance().GetStatRandomLevel() };
                        }
                        break;
                    case 7: // 전설 6개확정 2개 랜덤 = 최대 8개 
                        if (!isSynySucs)
                        {
                            tmp_pso.op1 = new StatOp() { id = int.Parse(op_array[UnityEngine.Random.Range(0, op_array.Length)]), rlv = GetInstance().GetStatRandomLevel() };
                            tmp_pso.op2 = new StatOp() { id = int.Parse(op_array[UnityEngine.Random.Range(0, op_array.Length)]), rlv = GetInstance().GetStatRandomLevel() };
                            tmp_pso.op3 = new StatOp() { id = int.Parse(op_array[UnityEngine.Random.Range(0, op_array.Length)]), rlv = GetInstance().GetStatRandomLevel() };
                            tmp_pso.op4 = new StatOp() { id = int.Parse(op_array[UnityEngine.Random.Range(0, op_array.Length)]), rlv = GetInstance().GetStatRandomLevel() };
                            tmp_pso.op5 = new StatOp() { id = int.Parse(op_array[UnityEngine.Random.Range(0, op_array.Length)]), rlv = GetInstance().GetStatRandomLevel() };
                            tmp_pso.op6 = new StatOp() { id = int.Parse(op_array[UnityEngine.Random.Range(0, op_array.Length)]), rlv = GetInstance().GetStatRandomLevel() };

                            if (GetInstance().GetRandomPercent() < 35)
                                tmp_pso.op7 = new StatOp() { id = int.Parse(op_array[UnityEngine.Random.Range(0, op_array.Length)]), rlv = GetInstance().GetStatRandomLevel() };
                            if (GetInstance().GetRandomPercent() < 35)
                                tmp_pso.op8 = new StatOp() { id = int.Parse(op_array[UnityEngine.Random.Range(0, op_array.Length)]), rlv = GetInstance().GetStatRandomLevel() };
                        }
                        else
                        {
                            tmp_pso.op1 = tmpPetStOp.op1;
                            tmp_pso.op2 = tmpPetStOp.op2;
                            tmp_pso.op3 = tmpPetStOp.op3;
                            tmp_pso.op4 = tmpPetStOp.op4;
                            if (tmp_pso.op5.id > 0)
                                tmp_pso.op5 = tmpPetStOp.op5;
                            else tmp_pso.op5 = new StatOp() { id = int.Parse(op_array[UnityEngine.Random.Range(0, op_array.Length)]), rlv = GetInstance().GetStatRandomLevel() };
                            
                            if (tmp_pso.op6.id > 0)
                                tmp_pso.op6 = tmpPetStOp.op6;
                            else tmp_pso.op6 = new StatOp() { id = int.Parse(op_array[UnityEngine.Random.Range(0, op_array.Length)]), rlv = GetInstance().GetStatRandomLevel() };

                            if (tmpPetStOp.op7.id <= 0 && GetInstance().GetRandomPercent() < 35)
                                tmp_pso.op7 = new StatOp() { id = int.Parse(op_array[UnityEngine.Random.Range(0, op_array.Length)]), rlv = GetInstance().GetStatRandomLevel() };
                            if (tmpPetStOp.op8.id <= 0 && GetInstance().GetRandomPercent() < 35)
                                tmp_pso.op8 = new StatOp() { id = int.Parse(op_array[UnityEngine.Random.Range(0, op_array.Length)]), rlv = GetInstance().GetStatRandomLevel() };
                        }
                        break;
                }

                return tmp_pso;
            }
            return new PetStatOp();
        }

        /// <summary>
        /// 펫 옵션 스탯별(공,방,치공,체,치방) 종합 수치 ex) => 150.00% * 0.01 = 1.5%
        /// </summary>
        public PetOpStTotalFigures GetPetOpStTotalFigures(Pet _petDB)
        {
            PetOpStTotalFigures pOpTotal = new PetOpStTotalFigures();
            var statOp = _petDB.statOp;
            for (int i = 0; i < 8; i++)
            {
                var op = i == 0 ? statOp.op1 : i == 1 ? statOp.op2 : i == 2 ? statOp.op3 : i == 3 ? statOp.op4 : i == 4 ? statOp.op5 : i == 5 ? statOp.op6 : i == 6 ? statOp.op7 : i == 7 ? statOp.op8 : default;
                switch (op.id)
                {
                    case 1: pOpTotal.op1v += GetInstance().chartDB.GetCdbPetOptionValue(_petDB.p_rt, _petDB.p_id, 1, op.rlv, _petDB.p_lv) * 0.01f; break;
                    case 2: pOpTotal.op2v += GetInstance().chartDB.GetCdbPetOptionValue(_petDB.p_rt, _petDB.p_id, 2, op.rlv, _petDB.p_lv) * 0.01f; break;
                    case 4: pOpTotal.op4v += GetInstance().chartDB.GetCdbPetOptionValue(_petDB.p_rt, _petDB.p_id, 4, op.rlv, _petDB.p_lv) * 0.01f; break;
                    case 5: pOpTotal.op5v += GetInstance().chartDB.GetCdbPetOptionValue(_petDB.p_rt, _petDB.p_id, 5, op.rlv, _petDB.p_lv) * 0.01f; break;
                    case 8: pOpTotal.op8v += GetInstance().chartDB.GetCdbPetOptionValue(_petDB.p_rt, _petDB.p_id, 8, op.rlv, _petDB.p_lv) * 0.01f; break;
                }
            }

            return pOpTotal;
        }

        /// <summary>
        /// 펫 전용 옵션 값 
        /// </summary>
        public PetSpOpTotalFigures GetPetSpOpTotalFigures(Pet _petDB)
        {
            PetSpOpTotalFigures pSpOpTotal = new PetSpOpTotalFigures();
            for (int i = 0; i < 3; i++)
            {
                int sop_id = i == 0 ? _petDB.sOp1.id : i == 1 ? _petDB.sOp2.id : i == 2 ? _petDB.sOp3.id : 0;
                if(sop_id > 0)
                {
                    float sop_val = GameDatabase.GetInstance().chartDB.GetPetSpecialOptionStatValue(_petDB, i + 1);
                    switch (sop_id)
                    {
                        case 1: pSpOpTotal.sop1_value = sop_val;  break;
                        case 2: pSpOpTotal.sop2_value = sop_val;  break;
                        case 3: pSpOpTotal.sop3_value = sop_val;  break;
                        case 4: pSpOpTotal.sop4_value = sop_val;  break;
                        case 5: pSpOpTotal.sop5_value = sop_val;  break;
                        case 6: pSpOpTotal.sop6_value = sop_val;  break;
                        case 7: pSpOpTotal.sop7_value = sop_val;  break;
                        case 8: pSpOpTotal.sop8_value = sop_val;  break;
                        case 9: pSpOpTotal.sop9_value = sop_val;  break;
                    }
                }
            }

            return pSpOpTotal;
        }

        #endregion
    }

    /// <summary>
    /// ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
    /// #테이블 데이터 (공개)
    /// ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
    /// </summary>
    public class PublicContentDB
    {
        // ####################### 챕터 클리어 당시 캐릭터 데이터 #######################
        private string pubDB_ClearChapterChar_MyInDate; // 챕터 클리어 캐릭터 정보 InDate 
#region # 챕터 클리어 당시 캐릭터 정보 #
        public struct PubDB_ClearChapterChar_UseSkill
        {
            public int slot1_id, slot1_lv;
            public int slot2_id, slot2_lv;
            public int slot3_id, slot3_lv;
            public int slot4_id, slot4_lv;
            public int slot5_id, slot5_lv;
            public int slot6_id, slot6_lv;
        }
#endregion

        // ####################### 캐릭터 데이터 (pvp, 유저정보 보기 에 사용됨) #######################
        private string pubDB_NowChar_MyInDate; // 실시간 캐릭터 정보 InDate 
#region # 현재 캐릭터 정보 #
        [System.Serializable]
        public struct PubDB_NowChar_Equip
        {
            public int eq_ty;
            public int eq_rt;
            public int eq_id;
            public int eq_legend;
            public int eq_legend_sop_id;
            public int eq_legend_sop_rlv;
            public int ma_st_id, ma_st_rlv;
            public int eq_nor_lv, eq_ehn_lv;
            public TableDB.StatOp st_sop_ac; // 장신구 전용 옵션 
            public TableDB.EquipStatOp st_op; // 장비/장신구 옵션 
        }
#endregion


        //private Dictionary<string, int> pubTableSendDbCombat = new Dictionary<string, int>();
        public void InitSendCharacterDbCombat()
        {
            //int chpt_charDb_table_combat = PlayerPrefs.GetInt(PrefsKeys.key_CombatChpterClear);
            //int now_charDb_table_combat = PlayerPrefs.GetInt(PrefsKeys.key_CombatNow);
            //pubTableSendDbCombat.Add(BackendGpgsMng.tableName_Pub_ChapterClearCharData, chpt_charDb_table_combat);
            //pubTableSendDbCombat.Add(BackendGpgsMng.tableName_Pub_NowCharData, now_charDb_table_combat);

        }
        /// <summary> 
        /// 캐릭터 데이터를 퍼블릭 테이블에 데이터 전송 
        /// </summary>
        public async void ASetPub_CharData(string pub_tableName, bool isAsync = true)
        {
            // 이전 전투력과 현재 전투력을 비교하여 변화가 있다면 전송 없다면 멈춤 
            var myIgp = GameMng.GetInstance().myPZ.igp;
            IG.PlayerSkill.Skill[] use_skills = GameMng.GetInstance().myPZ.igp.playerSkillAction.playerSkill.useSkill;
            string skJsonStr = JsonUtility.ToJson(
                new PubDB_ClearChapterChar_UseSkill()
                {
                    slot1_id = (int)use_skills[0].number,
                    slot1_lv = (int)use_skills[0].stat.skLv,
                    slot2_id = (int)use_skills[1].number,
                    slot2_lv = (int)use_skills[1].stat.skLv,
                    slot3_id = (int)use_skills[2].number,
                    slot3_lv = (int)use_skills[2].stat.skLv,
                    slot4_id = (int)use_skills[3].number,
                    slot4_lv = (int)use_skills[3].stat.skLv,
                    slot5_id = (int)use_skills[4].number,
                    slot5_lv = (int)use_skills[4].stat.skLv,
                    slot6_id = (int)use_skills[5].number,
                    slot6_lv = (int)use_skills[5].stat.skLv,
                });

            bool isCombatSame = false;
            if (string.Equals(BackendGpgsMng.tableName_Pub_ChapterClearCharData, pub_tableName))
            {
                isCombatSame = PlayerPrefs.GetString(PrefsKeys.key_CombatChpterClear) == myIgp.statValue.combat_power.ToString();
                if (!isCombatSame)
                    PlayerPrefs.SetString(PrefsKeys.key_CombatChpterClear, myIgp.statValue.combat_power.ToString());
            }
            else if (string.Equals(BackendGpgsMng.tableName_Pub_NowCharData, pub_tableName))
            {
                isCombatSame = PlayerPrefs.GetString(PrefsKeys.key_CombatNow) == myIgp.statValue.combat_power.ToString();
                if (!isCombatSame)
                    PlayerPrefs.SetString(PrefsKeys.key_CombatNow, myIgp.statValue.combat_power.ToString());
            }

            if (isCombatSame)
                return;

            LogPrint.EditorPrint("ASetPub_CharData pub_tableName : " + pub_tableName);
            #region ##전송할 데이터 ###
            IG.PartsIdx parts = GameMng.GetInstance().myPZ.igp.parts;
            GameDatabase.CharacterDB.StatValue statValue = GameDatabase.GetInstance().characterDB.SetPlayerStatValue(false);// GameMng.GetInstance().myPZ.igp.statValue;

            Param pam = new Param();

            // ##### 유저 캐릭터 정보 공통 데이터 
            // 장비 파츠 rt, id 
            pam.Add("parts", JsonUtility.ToJson(parts));

            // 스탯 정보 
            pam.Add("statValue", JsonUtility.ToJson(statValue));

            // 스킬 정보 
            pam.Add("useSkill", skJsonStr);

            // ##### 챕터 클리어 당시 캐릭터 데이터 (챕터 유저 보스에서 사용됨) ##### 
            // ##### 전송 시기 -> 새로운 챕터 클리어 했을 경우 ? 
            if (pub_tableName == BackendGpgsMng.tableName_Pub_ChapterClearCharData)
            {
                pam.Add("m_clear_chpter", GetInstance().tableDB.GetUserInfo().GetChapterDvsNbr());
            }
            // ##### 현재 캐릭터 데이터 (랭킹에서 유저 정보 보여주거나, PvP에서 사용됨) ##### 
            // ##### 전송 시기 -> Pvp에서 승리하였을 경우 ? 
            else if (pub_tableName == BackendGpgsMng.tableName_Pub_NowCharData)
            {
                List<PubDB_NowChar_Equip> nEquips = new List<PubDB_NowChar_Equip>();
                int useEqSlot = GetInstance().tableDB.GetUseEquipSlot();
                for (int i = 0; i <= 10; i++)
                {
                    var use_equip = GetInstance().tableDB.GetFindUseEquipment(i, useEqSlot);
                    nEquips.Add(new PubDB_NowChar_Equip()
                    {
                        eq_ty = use_equip.eq_ty,
                        eq_rt = use_equip.eq_rt,
                        eq_id = use_equip.eq_id,
                        eq_legend = use_equip.eq_legend,
                        eq_legend_sop_id = use_equip.eq_legend_sop_id,
                        eq_legend_sop_rlv = use_equip.eq_legend_sop_rlv,
                        ma_st_id = use_equip.ma_st_id,
                        ma_st_rlv = use_equip.ma_st_rlv,
                        eq_nor_lv = use_equip.m_norm_lv,
                        eq_ehn_lv = use_equip.m_ehnt_lv,

                        st_sop_ac = use_equip.st_sop_ac,
                        st_op = use_equip.st_op
                    });
                }

                pam.Add("useEquip", JsonUtility.ToJson(new Serialization<PubDB_NowChar_Equip>(nEquips)));

                // 결투 코멘트 
                string pvp_comment = PlayerPrefs.GetString(PrefsKeys.key_PvPArenaComment);
                if (string.IsNullOrEmpty(pvp_comment) == false)
                    pam.Add("m_comment", pvp_comment);
            }
            #endregion

            // 퍼블릭 테이블의 indate 
            string pubDb_indate = string.Empty;
            if (pub_tableName == BackendGpgsMng.tableName_Pub_NowCharData)
                pubDb_indate = pubDB_NowChar_MyInDate;
            else if (pub_tableName == BackendGpgsMng.tableName_Pub_ChapterClearCharData)
                pubDb_indate = pubDB_ClearChapterChar_MyInDate;

            if (isAsync)
            {
                if (string.IsNullOrEmpty(pubDb_indate))
                {
                    LogPrint.EditorPrint("----------------A GetMyPublicContents pub_tableName : " + pub_tableName);
                    BackendReturnObject bro1 = null;
                    SendQueue.Enqueue(Backend.GameInfo.GetMyPublicContents, pub_tableName, callback => { bro1 = callback; });
                    while (bro1 == null) { await Task.Delay(100); }

                    JsonData returnData = bro1.GetReturnValuetoJSON()["rows"];
                    if (returnData.Count > 0)
                    {
                        LogPrint.EditorPrint("----------------A GetMyPublicContents pub_tableName : " + pub_tableName + ", _bro1 : " + bro1.GetReturnValuetoJSON());
                        pubDb_indate = RowPaser.StrPaser(returnData[0], "inDate");
                    }
                }

                LogPrint.EditorPrint("Async pubDb_indate : " + pubDb_indate);
                if (pam.GetValue().Count > 0)
                {
                    if (string.IsNullOrEmpty(pubDb_indate))
                    {
                        Task<string> t2 = BackendGpgsMng.GetInstance().TaskInsertTableData(pub_tableName, pam);
                        await t2;
                        if (t2.Result != string.Empty) { pubDb_indate = t2.Result; }
                    }
                    
                    await BackendGpgsMng.GetInstance().TaskUpdateTableData(pub_tableName, pubDb_indate, pam);

                    if (pub_tableName == BackendGpgsMng.tableName_Pub_NowCharData)
                        pubDB_NowChar_MyInDate = pubDb_indate;
                    else if (pub_tableName == BackendGpgsMng.tableName_Pub_ChapterClearCharData)
                        pubDB_ClearChapterChar_MyInDate = pubDb_indate;
                }
            }
            else
            {
                if (string.IsNullOrEmpty(pubDb_indate))
                {
                    LogPrint.EditorPrint("----------------A GetMyPublicContents pub_tableName : " + pub_tableName);
                    BackendReturnObject bro1 = Backend.GameInfo.GetMyPublicContents(pub_tableName);
                    JsonData returnData = bro1.GetReturnValuetoJSON()["rows"];
                    if (returnData.Count > 0)
                    {
                        LogPrint.EditorPrint("----------------A GetMyPublicContents pub_tableName : " + pub_tableName + ", _bro1 : " + bro1.GetReturnValuetoJSON());
                        pubDb_indate = RowPaser.StrPaser(returnData[0], "inDate");
                    }
                }

                LogPrint.EditorPrint("!Async pubDb_indate : " + pubDb_indate);
                if (pam.GetValue().Count > 0)
                {
                    if (string.IsNullOrEmpty(pubDb_indate))
                    {
                        BackendReturnObject bro1 = Backend.GameInfo.Insert(pub_tableName, pam);
                        if (bro1.IsSuccess())
                        {
                            //{ "inDate":"2020-06-10T09:26:21.738Z
                            pubDb_indate = bro1.GetReturnValuetoJSON()["inDate"].ToString();
                        }
                    }

                    if (!string.IsNullOrEmpty(pubDb_indate))
                    {
                        Backend.GameInfo.Update(pub_tableName, pubDb_indate, pam);

                        if (pub_tableName == BackendGpgsMng.tableName_Pub_NowCharData)
                            pubDB_NowChar_MyInDate = pubDb_indate;
                        else if (pub_tableName == BackendGpgsMng.tableName_Pub_ChapterClearCharData)
                            pubDB_ClearChapterChar_MyInDate = pubDb_indate;
                    }
                }
            }
        }

        /// <summary>
        /// 퍼블릭 테이블의 유저 indate값을 가지고 퍼블릭 테이블의 데이터를 리턴 
        /// </summary>
        public async Task<JsonData> AGetPubTableDataLoad(string pub_tableName, string gamerInDate)
        {
            if (string.IsNullOrEmpty(gamerInDate))
                return null;

            LogPrint.Print("----------------A AGetPubTableDataLoad pub_tableName : " + pub_tableName + ", gamerInDate : " + gamerInDate);
            BackendReturnObject bro1 = null;
            SendQueue.Enqueue(Backend.GameInfo.GetPublicContentsByGamerIndate, pub_tableName, gamerInDate, callback => { bro1 = callback; });
            while (bro1 == null) { await Task.Delay(100); }
            LogPrint.Print("----------------A AGetPubTableDataLoad pub_tableName : " + pub_tableName + ", gamerInDate : " + gamerInDate + ", _bro1 : " + bro1.GetReturnValuetoJSON().ToString());

            if (bro1.IsSuccess())
            {
                if (bro1.GetReturnValuetoJSON()["rows"].Count > 0)
                {
                    return bro1.GetReturnValuetoJSON()["rows"][0];
                }
            }

            return null;
        }
    }

    /// <summary>
    /// ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
    /// #차트 데이터 (일반 차트)
    /// ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
    /// </summary>
    public class ChartDB
    {
        // 차트 이름  
#region ----- 차트 이름 -----
        public const string chartName_balance = "cdb_balance"; // 밸런스 
        public const string chartName_stat = "cdb_stat";  // 장비 스탯 
        public const string chartName_equip_stat = "cdb_equip_stat";  // 장비 스탯 
        public const string charName_sop_acce = "cdb_stat_acce_sop"; // 장신구 전용 옵션 
        public const string chartName_skill = "cdb_stat_skill"; // 스킬 
        public const string chartName_MonsterSstat = "cdb_chpt_mnst_stat";  // 챕터 스테이지 몬스터 차트 네임
        public const string chartName_DungeonTop = "cdb_dungeon_top";       // 던전 : 도전의 탑 차트 네임
        public const string chartName_DungeonMine = "cdb_dungeon_mine";     // 던전 : 광산 차트 네임
        public const string chartName_DungeonRaid = "cdb_dungeon_raid";     // 던전 : 레이드 차트 네임

        public const string chartName_GgachaPercentage = "cdb_gacha_percentage"; // 가차 확률 표 
        public const string chartName_attendance_book = "cdb_attendance_book"; // 출석 보상 
        public const string chartName_achievements = "cdb_achievements"; // 업적 
        public const string chartName_daily_mission = "cdb_daily_mission"; // 미션(일일)
        public const string chartName_quest = "cdb_quest"; // 퀘스트 
        public const string chartName_offline_reward = "cdb_offline_reward"; // 오프라인 보상 

        public const string chartName_chapter_drop_rating = "chapter_drop_rating";
        public const string chartName_chapter_drop_db = "chapter_drop_db";
        public const string chartName_r_acce_special_op = "r_acce_special_op";
        public const string chartName_equip_option = "cdb_equip_option";
        public const string chartName_pet = "cdb_pet";
        public const string chartName_pet_sop = "cdb_pet_sop";

        private string[] chartNames = new string[]
        {
            chartName_stat, chartName_equip_stat, chartName_pet, chartName_pet_sop,  
            charName_sop_acce,      chartName_skill,

            chartName_balance, chartName_MonsterSstat, chartName_DungeonTop, chartName_DungeonMine, chartName_DungeonRaid,
            chartName_chapter_drop_rating, chartName_chapter_drop_db, 
            chartName_r_acce_special_op, chartName_equip_option, chartName_GgachaPercentage,

            chartName_attendance_book, chartName_achievements, chartName_daily_mission, chartName_quest, chartName_offline_reward
        };
        #endregion

        #region ----- 장비 -----

        // 장비 스탯 : 무기~방어구~장신구
        private static readonly List<Cdb_stat> _cdb_stat = new List<Cdb_stat>();
        public List<Cdb_stat> list_cdb_stat = _cdb_stat;

        /// <summary>
        /// 장비 스탯 
        /// </summary>
        public struct Cdb_stat // exch_per : 조각 교환시 ID별 확률 
        { 
            public int eRating; public int eIdx; public int exch_per; public float atk_spd; public float min; public float max;
            public int      min_typ0, max_typ0;
            public int      min_typ1, max_typ1;
            public float    min_typ2, max_typ2;
            public int      min_typ3, max_typ3;
            public int      min_typ4, max_typ4;
            public float    min_typ5, max_typ5;
            public float    min_typ6, max_typ6;
            public int      min_typ7, max_typ7;
            public float    min_typ8, max_typ8;
            public float    min_typ9, max_typ9;
            public int      min_typ10, max_typ10;
        };

        private static readonly List<Cdb_EquipStat> _list_cdb_equipstat = new List<Cdb_EquipStat>();
        public List<Cdb_EquipStat> list_cdb_equipstat = _list_cdb_equipstat;

        public struct Cdb_EquipStat
        {
            public int level;
            public long wea_r1, shi_r1, arm_r1, gau_r1, pan_r1;
            public long wea_r2, shi_r2, arm_r2, gau_r2, pan_r2;
            public long wea_r3, shi_r3, arm_r3, gau_r3, pan_r3;
            public long wea_r4, shi_r4, arm_r4, gau_r4, pan_r4;
            public long wea_r5, shi_r5, arm_r5, gau_r5, pan_r5;
            public long wea_r6, shi_r6, arm_r6, gau_r6, pan_r6;
            public long wea_r7, shi_r7, arm_r7, gau_r7, pan_r7;

            public float hel_r1, sho_r1, boo_r1, nec_r1, ear_r1, rin_r1;
            public float hel_r2, sho_r2, boo_r2, nec_r2, ear_r2, rin_r2;
            public float hel_r3, sho_r3, boo_r3, nec_r3, ear_r3, rin_r3;
            public float hel_r4, sho_r4, boo_r4, nec_r4, ear_r4, rin_r4;
            public float hel_r5, sho_r5, boo_r5, nec_r5, ear_r5, rin_r5;
            public float hel_r6, sho_r6, boo_r6, nec_r6, ear_r6, rin_r6;
            public float hel_r7, sho_r7, boo_r7, nec_r7, ear_r7, rin_r7;
        }

        // 장비 강화 스탯 
        public object GetEquipEnchantLevelStatValue(int lv, int eq_ty, int rt)
        {
            var find = list_cdb_equipstat.Find(obj => obj.level == lv);
            switch (eq_ty)
            {
                case 0: // 무기 : wea_r0 ~ wea_r7 
                    switch (rt) { case 1: return find.wea_r1; case 2: return find.wea_r2; case 3: return find.wea_r3; case 4: return find.wea_r4; case 5: return find.wea_r5; case 6: return find.wea_r6; case 7: return find.wea_r7; }
                    break;

                case 1: // 방패 
                    switch (rt) { case 1: return find.shi_r1; case 2: return find.shi_r2; case 3: return find.shi_r3; case 4: return find.shi_r4; case 5: return find.shi_r5; case 6: return find.shi_r6; case 7: return find.shi_r7; }
                    break;

                case 2: // 헬멧 %
                    switch (rt) { case 1: return find.hel_r1; case 2: return find.hel_r2; case 3: return find.hel_r3; case 4: return find.hel_r4; case 5: return find.hel_r5; case 6: return find.hel_r6; case 7: return find.hel_r7; }
                    break;

                case 3: // 어깨 %
                    switch (rt) { case 1: return find.sho_r1; case 2: return find.sho_r2; case 3: return find.sho_r3; case 4: return find.sho_r4; case 5: return find.sho_r5; case 6: return find.sho_r6; case 7: return find.sho_r7; }
                    break;

                case 4: // 갑옷 
                    switch (rt) { case 1: return find.arm_r1; case 2: return find.arm_r2; case 3: return find.arm_r3; case 4: return find.arm_r4; case 5: return find.arm_r5; case 6: return find.arm_r6; case 7: return find.arm_r7; }
                    break;

                case 5: // 팔 
                    switch (rt) { case 1: return find.gau_r1; case 2: return find.gau_r2; case 3: return find.gau_r3; case 4: return find.gau_r4; case 5: return find.gau_r5; case 6: return find.gau_r6; case 7: return find.gau_r7; }
                    break;

                case 6: // 바지 
                    switch (rt) { case 1: return find.pan_r1; case 2: return find.pan_r2; case 3: return find.pan_r3; case 4: return find.pan_r4; case 5: return find.pan_r5; case 6: return find.pan_r6; case 7: return find.pan_r7; }
                    break;

                case 7: // 신발 %
                    switch (rt) { case 1: return find.boo_r1; case 2: return find.boo_r2; case 3: return find.boo_r3; case 4: return find.boo_r4; case 5: return find.boo_r5; case 6: return find.boo_r6; case 7: return find.boo_r7; }
                    break;

                case 8: // 목걸이 %
                    switch (rt) { case 1: return find.nec_r1; case 2: return find.nec_r2; case 3: return find.nec_r3; case 4: return find.nec_r4; case 5: return find.nec_r5; case 6: return find.nec_r6; case 7: return find.nec_r7; }
                    break;

                case 9: // 귀고리 %
                    switch (rt) { case 1: return find.ear_r1; case 2: return find.ear_r2; case 3: return find.ear_r3; case 4: return find.ear_r4; case 5: return find.ear_r5; case 6: return find.ear_r6; case 7: return find.ear_r7; }
                    break;

                case 10: // 반지 %
                    switch (rt) { case 1: return find.rin_r1; case 2: return find.rin_r2; case 3: return find.rin_r3; case 4: return find.rin_r4; case 5: return find.rin_r5; case 6: return find.rin_r6; case 7: return find.rin_r7; }
                    break;
            }

            return 0;
        }

        #endregion

        #region ----- 능력치 스탯 업 -----
        // 능력치 스탯 
        public object GetAbilityLevelStatsValue(int lv, int eq_ty, int rt)
        {
            return 0;
        }
        #endregion

        #region ----- 출석부 #####
        private static readonly List<cdb_attendance_book> _list_cdb_attendance_book = new List<cdb_attendance_book>();
        protected List<cdb_attendance_book> list_cdb_attendance_book = _list_cdb_attendance_book;

        [System.Serializable]
        public struct cdb_attendance_book
        {
            public int nbr;
            public GameDatabase.MailDB.Item item;
        }
#endregion
        // ### 장신구 전용 옵션 스탯 
        /*
         1.PvE피해 증가
         2.PvP피해 증가
         3.PvE피해 감소
         4.PvP피해 감소
         5.골드 획득 증가
         6.장비 드랍률 증가
         7.보스 피해 증가
         8.체력 25% 회복(확률)
         9.버블 추가 획득(확률)
        10.상대 버블 차감(확률)
         */
        private static readonly List<cdb_stat_special_Acce> _list_cdb_stat_sop_Acce = new List<cdb_stat_special_Acce>();
        public List<cdb_stat_special_Acce> list_cdb_stat_sop_Acce = _list_cdb_stat_sop_Acce;

        // ### 스킬 
        private static readonly List<cdb_stat_skill> _cdb_stat_Skill = new List<cdb_stat_skill>(); // 스킬 
        public List<cdb_stat_skill> list_cdb_stat_Skill = _cdb_stat_Skill;

        // ### 챕터 드롭될 등급 
        private static readonly List<cdb_r_chapter_drop_rating> _cdb_r_field_chest_rt = new List<cdb_r_chapter_drop_rating>();
        protected List<cdb_r_chapter_drop_rating> list_cdb_r_field_chest_rt = _cdb_r_field_chest_rt;
        public cdb_r_chapter_drop_rating GetFieldDropRating(int chpt_id) => list_cdb_r_field_chest_rt.Find(obj => obj.chpt_id == chpt_id);

        // ### 챕터 드랍 -> 장비(장신구) 
        private static readonly Dictionary<string, List<cdb_r_chapter_equip_drop_result>> _cdb_r_field_drop_result = new Dictionary<string, List<cdb_r_chapter_equip_drop_result>>();
        protected Dictionary<string, List<cdb_r_chapter_equip_drop_result>> dic_cdb_r_field_drop_result = _cdb_r_field_drop_result;

        // ### 챕터 드랍 -> 아이템 
        private static readonly List<cdb_r_chapter_item_drop_result> _cdb_r_field_chest_item = new List<cdb_r_chapter_item_drop_result>();
        protected List<cdb_r_chapter_item_drop_result> dic_cdb_r_field_chest_item = _cdb_r_field_chest_item;
        
        // ### 장비/장신구 소환 가차 표 
        private static readonly List<cdb_gacha_percentage> _cdb_gacha_percentage = new List<cdb_gacha_percentage>();
        protected List<cdb_gacha_percentage> list_cdb_gacha_percentage = _cdb_gacha_percentage;
        public cdb_gacha_percentage Get_cdb_gacha_percentage(string gch_name)
        {
            int indx = list_cdb_gacha_percentage.FindIndex(obj => string.Equals(obj.gch_name, gch_name));
            if(indx >= 0)
            {
                return list_cdb_gacha_percentage[indx];
            }
            else return default;
        }

        public List<cdb_r_chapter_equip_drop_result> GetChapterDropEquipResult(int chpt_id, int rt)
        {
            if (dic_cdb_r_field_drop_result.TryGetValue(chpt_id.ToString(), out List<cdb_r_chapter_equip_drop_result> db))
            {
                var list = db.FindAll(obj => obj.rt == rt);
                return list.Count > 0 ? list : null;
            }
            else return null;
        }

        public List<cdb_r_chapter_item_drop_result> GetChapterDropItemResult()
        {
            if (dic_cdb_r_field_chest_item.Count > 0)
                return dic_cdb_r_field_chest_item;
            else return null;
        }

        // ### 장신구 전용 옵션 (랜덤 값)
        private static readonly List<cdb_acce_special_op> _cdb_acce_special_op = new List<cdb_acce_special_op>();
        protected List<cdb_acce_special_op> lst_cdb_acce_special_op = _cdb_acce_special_op;

        // ### 장신구 전용 옵션 (랜덤 값)
        private static readonly Dictionary<int, cdb_equip_op> _dic_cdb_equip_op = new Dictionary<int, cdb_equip_op>();
        protected Dictionary<int, cdb_equip_op> dic_cdb_equip_op = _dic_cdb_equip_op;

        // ### etc 벨런스 
        private static readonly Dictionary<string, Balance> _dic_cdb_balance = new Dictionary<string, Balance>();
        private Dictionary<string, Balance> dic_cdb_balance = _dic_cdb_balance;
        #region 차트 - 밸런스 -
        public Balance GetDicBalance(string id)
        {
            if (dic_cdb_balance.TryGetValue(id, out Balance ba))
                return ba;
            else return default;
        }

        public int GetDicBalanceEquipMaxNormalLevel() => GetDicBalance("eq.normal.max.level").val_int;
        public int GetDicBalanceEquipMaxEnhantLevel() => GetDicBalance("eq.enhant.max.level").val_int;
        public int GetDicBalanceStatMaxLevel () => GetInstance().chartDB.GetDicBalance("eq.stat.max.level").val_int;
        #endregion

        // ### 챕터 스테이지 
        private static readonly List<cdb_chpt_mnst_stat> _cdb_chpt_mnst_stat = new List<cdb_chpt_mnst_stat>();
        public List<cdb_chpt_mnst_stat> list_cdb_chpt_mnst_stat = _cdb_chpt_mnst_stat;
        /// <summary>
        /// 챕터  ID
        /// </summary>
        public int GetChapterDvsNbrFindChapterID(int dvs_nbr)
        {
            int indx = list_cdb_chpt_mnst_stat.FindIndex(x => x.chpt_dvs_nbr == dvs_nbr);
            if (indx >= 0)
                return list_cdb_chpt_mnst_stat[indx].chpt_id;
            else return list_cdb_chpt_mnst_stat[list_cdb_chpt_mnst_stat.Count - 1].chpt_id;
        }

        /// <summary>
        /// 스테이지 ID 
        /// </summary>
        public int GetChapterDvsNbrFindStageID(int dvs_nbr)
        {
            int indx = list_cdb_chpt_mnst_stat.FindIndex(x => x.chpt_dvs_nbr == dvs_nbr);
            if (indx >= 0)
                return list_cdb_chpt_mnst_stat[indx].stg_id;
            else return list_cdb_chpt_mnst_stat[list_cdb_chpt_mnst_stat.Count - 1].stg_id;
        }

        /// <summary>
        /// 현재 진행중인 나의 챕터 ID 
        /// </summary>
        public int GetMyCurrentChapterID()
        {
            int m_chpt_stg_nbr = GameDatabase.GetInstance().tableDB.GetUserInfo().GetChapterDvsNbr();
            return GetChapterDvsNbrFindChapterID(m_chpt_stg_nbr);
        }

        /// <summary>
        /// 마지막 단계의 chpt_dvs_nbr 
        /// </summary>
        public int GetChapterLastDvsNbr() => list_cdb_chpt_mnst_stat[list_cdb_chpt_mnst_stat.Count - 1].chpt_dvs_nbr;

        /// <summary>
        /// 채팅, 랭킹탭에서 유저의 현재 진행중인 스테이지를 표기 -> ?챕터, ?스테이지 
        /// </summary>
        public string GetRankChapterStageString (float rank_chpt_stage)
        {
            int v = GameDatabase.GetInstance().tableDB.GetUserInfo().GetRankChapterNbr((int)rank_chpt_stage); 
            int indx = list_cdb_chpt_mnst_stat.FindIndex(x => x.chpt_dvs_nbr == v);
            if(indx >= 0)
            {
                int chpt_id = list_cdb_chpt_mnst_stat[indx].chpt_id;
                int stg_id = list_cdb_chpt_mnst_stat[indx].stg_id;
                return string.Format("챕터 {0}, 스테이지 {1}", chpt_id, stg_id);
            }
            else
            {
                int last_indx = list_cdb_chpt_mnst_stat.Count - 1;
                int last_chpt_id = list_cdb_chpt_mnst_stat[last_indx].chpt_id;
                int last_stg_id = list_cdb_chpt_mnst_stat[last_indx].stg_id;
                return string.Format("챕터 {0}, 스테이지 {1}", last_chpt_id, last_stg_id);
            }
        }

        #region 챕터 반복
        // ### 챕터 반복 몬스터 db 
        private static readonly Dictionary<int, List<cdb_chpt_mnst_stat>> _cdb_chpt_mnst_loop = new Dictionary<int, List<cdb_chpt_mnst_stat>>();
        public Dictionary<int, List<cdb_chpt_mnst_stat>> dic_cdb_chpt_mnst_loop = _cdb_chpt_mnst_loop;

        public int GetChapterLoopIdCnt() => dic_cdb_chpt_mnst_loop.Keys.Count;
        public int GetChapterLoopStageCnt(int key) => dic_cdb_chpt_mnst_loop[key].Count;
        /// <summary>
        /// 반복 스테이지 : 몬스터 정보 
        /// </summary>
        public cdb_chpt_mnst_stat GetDicChapterLoopDb(int chpt_id, int chpt)
        {
            if (dic_cdb_chpt_mnst_loop.TryGetValue(chpt_id, out List<cdb_chpt_mnst_stat> ba))
                return ba.Find(x => x.chpt_dvs_nbr == chpt);
            else return default;
        }
        /// <summary>
        /// 반복 스테이지 : 몬스터 정보 처음 (챕터 몬스터 장비의 rt, id)
        /// </summary>
        public cdb_chpt_mnst_stat GetDicChapterLoopArrayFirst(int chpt_id)
        {
            if (dic_cdb_chpt_mnst_loop.TryGetValue(chpt_id, out List<cdb_chpt_mnst_stat> ba))
                return ba[0];
            else return default;
        }
        /// <summary>
        /// 반복 스테이지 : 몬스터 정보 마지막 (챕터 몬스터 장비의 rt, id)
        /// </summary>
        public cdb_chpt_mnst_stat GetDicChapterLoopArrayLast(int chpt_id)
        {
            if (dic_cdb_chpt_mnst_loop.TryGetValue(chpt_id, out List<cdb_chpt_mnst_stat> ba))
                return ba[ba.Count - 1];
            else return default;
        }
#endregion

        // ### 던전 : 도전의 탑 
        private static readonly List<cdb_dungeon_top> _cdb_dungeon_top = new List<cdb_dungeon_top>();
        public List<cdb_dungeon_top> list_cdb_dungeon_top = _cdb_dungeon_top;
        // ### 던전 : 광산
        private static readonly List<cdb_dungeon_mine> _cdb_dungeon_mine = new List<cdb_dungeon_mine>();
        public List<cdb_dungeon_mine> list_cdb_dungeon_mine = _cdb_dungeon_mine;
        // ### 던전 : 레이드 
        private static readonly List<cdb_dungeon_raid> _cdb_dungeon_raid = new List<cdb_dungeon_raid>();
        public List<cdb_dungeon_raid> list_cdb_dungeon_raid = _cdb_dungeon_raid;

        // 업적 및 일일 미션 
        private static readonly Dictionary<int, List<cdb_achievements>> _cdb_achievements = new Dictionary<int, List<cdb_achievements>> ();
        public Dictionary<int, List<cdb_achievements>> dic_cdb_achievements = _cdb_achievements;
        private static readonly List<cdb_achievements> _cdb_daily_mission = new List<cdb_achievements>();
        public List<cdb_achievements> list_cdb_daily_mission = _cdb_daily_mission;

        // 퀘스트 
        private static readonly List<cdb_quest> _cdb_quest = new List<cdb_quest>();
        public List<cdb_quest> cdb_quest = _cdb_quest;

        // 퀘스트 
        private static readonly List<cdb_offline_reward> _cdb_offline_reward = new List<cdb_offline_reward>();
        public List<cdb_offline_reward> cdb_offline_reward = _cdb_offline_reward;

        // 펫 정보 및 옵션  
        private static readonly List<cdb_pet> _cdb_pet = new List<cdb_pet>();
        public List<cdb_pet> list_cdb_pet = _cdb_pet;

        // 펫 전용 옵션 
        private static readonly List<cdb_pet_sop> _cdb_pet_sop = new List<cdb_pet_sop>();
        public List<cdb_pet_sop> list_cdb_pet_sop = _cdb_pet_sop;

        #region ##### 초기화 #####
        /// <summary>
        /// 차트 장비 스텟 데이터 세팅 
        /// </summary>
        public bool PaserChart()
        {
            LogPrint.Print("<color=magenta>--------------------------- START - SetChartParsing_cdb_stat ----------------------------</color>");

            list_cdb_stat_sop_Acce.Clear();
            list_cdb_stat_Skill.Clear();
            list_cdb_attendance_book.Clear();
            dic_cdb_balance.Clear();
            list_cdb_chpt_mnst_stat.Clear();
            list_cdb_r_field_chest_rt.Clear();
            dic_cdb_r_field_chest_item.Clear();
            dic_cdb_r_field_drop_result.Clear();

            lst_cdb_acce_special_op.Clear();
            dic_cdb_equip_op.Clear();
            list_cdb_pet.Clear();

            for (int i = 0; i < chartNames.Length; i++)
            {
                string cdb_key = chartNames[i];
                var cdb_data = JsonMapper.ToObject(Backend.Chart.GetLocalChartData(cdb_key));

#if UNITY_EDITOR
                // 에디터 차트 테스트 및 업데이트시 변경될 차트 ID 
                if (BackendGpgsMng.isEditor)
                {
                    if (string.Equals(cdb_key, chartName_chapter_drop_db))
                    {
                        BackendReturnObject bro = Backend.Chart.GetChartContents("21258");
                        cdb_data = JsonMapper.ToObject(bro.GetReturnValue());
                    }
                    else if (string.Equals(cdb_key, chartName_balance))
                    {
                        BackendReturnObject bro = Backend.Chart.GetChartContents("21370");
                        cdb_data = JsonMapper.ToObject(bro.GetReturnValue());
                    }
                    else if (string.Equals(cdb_key, chartName_pet))
                    {
                        BackendReturnObject bro = Backend.Chart.GetChartContents("21420");
                        cdb_data = JsonMapper.ToObject(bro.GetReturnValue());
                    }
                    else if (string.Equals(cdb_key, chartName_pet_sop))
                    {
                        BackendReturnObject bro = Backend.Chart.GetChartContents("21419");
                        cdb_data = JsonMapper.ToObject(bro.GetReturnValue());
                    }
                    else if (string.Equals(cdb_key, chartName_GgachaPercentage))
                    {
                        BackendReturnObject bro = Backend.Chart.GetChartContents("21383");
                        cdb_data = JsonMapper.ToObject(bro.GetReturnValue());
                    }
                    else if(string.Equals(cdb_key, chartName_equip_stat))
                    {
                        BackendReturnObject bro = Backend.Chart.GetChartContents("55038");
                        cdb_data = JsonMapper.ToObject(bro.GetReturnValue());
                    }
                }
#endif

            if (!string.IsNullOrEmpty(cdb_data.ToJson()))
                {
                    foreach (JsonData cdb_val in cdb_data["rows"])
                    {
                        switch (cdb_key)
                        {
                            case chartName_equip_stat:
                                list_cdb_equipstat.Add(
                                    new Cdb_EquipStat()
                                    {
                                        level = RowPaser.IntPaser(cdb_val, "level"),
                                        wea_r1 = RowPaser.LongPaser(cdb_val, "wea_r1"),
                                        wea_r2 = RowPaser.LongPaser(cdb_val, "wea_r2"),
                                        wea_r3 = RowPaser.LongPaser(cdb_val, "wea_r3"),
                                        wea_r4 = RowPaser.LongPaser(cdb_val, "wea_r4"),
                                        wea_r5 = RowPaser.LongPaser(cdb_val, "wea_r5"),
                                        wea_r6 = RowPaser.LongPaser(cdb_val, "wea_r6"),
                                        wea_r7 = RowPaser.LongPaser(cdb_val, "wea_r7"),
                                        
                                        shi_r1 = RowPaser.LongPaser(cdb_val, "shi_r1"),
                                        shi_r2 = RowPaser.LongPaser(cdb_val, "shi_r2"),
                                        shi_r3 = RowPaser.LongPaser(cdb_val, "shi_r3"),
                                        shi_r4 = RowPaser.LongPaser(cdb_val, "shi_r4"),
                                        shi_r5 = RowPaser.LongPaser(cdb_val, "shi_r5"),
                                        shi_r6 = RowPaser.LongPaser(cdb_val, "shi_r6"),
                                        shi_r7 = RowPaser.LongPaser(cdb_val, "shi_r7"),

                                        hel_r1 = RowPaser.FloatPaser(cdb_val, "hel_r1"),
                                        hel_r2 = RowPaser.FloatPaser(cdb_val, "hel_r2"),
                                        hel_r3 = RowPaser.FloatPaser(cdb_val, "hel_r3"),
                                        hel_r4 = RowPaser.FloatPaser(cdb_val, "hel_r4"),
                                        hel_r5 = RowPaser.FloatPaser(cdb_val, "hel_r5"),
                                        hel_r6 = RowPaser.FloatPaser(cdb_val, "hel_r6"),
                                        hel_r7 = RowPaser.FloatPaser(cdb_val, "hel_r7"),

                                        sho_r1 = RowPaser.FloatPaser(cdb_val, "sho_r1"),
                                        sho_r2 = RowPaser.FloatPaser(cdb_val, "sho_r2"),
                                        sho_r3 = RowPaser.FloatPaser(cdb_val, "sho_r3"),
                                        sho_r4 = RowPaser.FloatPaser(cdb_val, "sho_r4"),
                                        sho_r5 = RowPaser.FloatPaser(cdb_val, "sho_r5"),
                                        sho_r6 = RowPaser.FloatPaser(cdb_val, "sho_r6"),
                                        sho_r7 = RowPaser.FloatPaser(cdb_val, "sho_r7"),

                                        arm_r1 = RowPaser.LongPaser(cdb_val, "arm_r1"),
                                        arm_r2 = RowPaser.LongPaser(cdb_val, "arm_r2"),
                                        arm_r3 = RowPaser.LongPaser(cdb_val, "arm_r3"),
                                        arm_r4 = RowPaser.LongPaser(cdb_val, "arm_r4"),
                                        arm_r5 = RowPaser.LongPaser(cdb_val, "arm_r5"),
                                        arm_r6 = RowPaser.LongPaser(cdb_val, "arm_r6"),
                                        arm_r7 = RowPaser.LongPaser(cdb_val, "arm_r7"),

                                        gau_r1 = RowPaser.LongPaser(cdb_val, "gau_r1"),
                                        gau_r2 = RowPaser.LongPaser(cdb_val, "gau_r2"),
                                        gau_r3 = RowPaser.LongPaser(cdb_val, "gau_r3"),
                                        gau_r4 = RowPaser.LongPaser(cdb_val, "gau_r4"),
                                        gau_r5 = RowPaser.LongPaser(cdb_val, "gau_r5"),
                                        gau_r6 = RowPaser.LongPaser(cdb_val, "gau_r6"),
                                        gau_r7 = RowPaser.LongPaser(cdb_val, "gau_r7"),

                                        pan_r1 = RowPaser.LongPaser(cdb_val, "pan_r1"),
                                        pan_r2 = RowPaser.LongPaser(cdb_val, "pan_r2"),
                                        pan_r3 = RowPaser.LongPaser(cdb_val, "pan_r3"),
                                        pan_r4 = RowPaser.LongPaser(cdb_val, "pan_r4"),
                                        pan_r5 = RowPaser.LongPaser(cdb_val, "pan_r5"),
                                        pan_r6 = RowPaser.LongPaser(cdb_val, "pan_r6"),
                                        pan_r7 = RowPaser.LongPaser(cdb_val, "pan_r7"),

                                        boo_r1 = RowPaser.FloatPaser(cdb_val, "boo_r1"),
                                        boo_r2 = RowPaser.FloatPaser(cdb_val, "boo_r2"),
                                        boo_r3 = RowPaser.FloatPaser(cdb_val, "boo_r3"),
                                        boo_r4 = RowPaser.FloatPaser(cdb_val, "boo_r4"),
                                        boo_r5 = RowPaser.FloatPaser(cdb_val, "boo_r5"),
                                        boo_r6 = RowPaser.FloatPaser(cdb_val, "boo_r6"),
                                        boo_r7 = RowPaser.FloatPaser(cdb_val, "boo_r7"),

                                        nec_r1 = RowPaser.FloatPaser(cdb_val, "nec_r1"),
                                        nec_r2 = RowPaser.FloatPaser(cdb_val, "nec_r2"),
                                        nec_r3 = RowPaser.FloatPaser(cdb_val, "nec_r3"),
                                        nec_r4 = RowPaser.FloatPaser(cdb_val, "nec_r4"),
                                        nec_r5 = RowPaser.FloatPaser(cdb_val, "nec_r5"),
                                        nec_r6 = RowPaser.FloatPaser(cdb_val, "nec_r6"),
                                        nec_r7 = RowPaser.FloatPaser(cdb_val, "nec_r7"),

                                        ear_r1 = RowPaser.FloatPaser(cdb_val, "ear_r1"),
                                        ear_r2 = RowPaser.FloatPaser(cdb_val, "ear_r2"),
                                        ear_r3 = RowPaser.FloatPaser(cdb_val, "ear_r3"),
                                        ear_r4 = RowPaser.FloatPaser(cdb_val, "ear_r4"),
                                        ear_r5 = RowPaser.FloatPaser(cdb_val, "ear_r5"),
                                        ear_r6 = RowPaser.FloatPaser(cdb_val, "ear_r6"),
                                        ear_r7 = RowPaser.FloatPaser(cdb_val, "ear_r7"),

                                        rin_r1 = RowPaser.FloatPaser(cdb_val, "rin_r1"),
                                        rin_r2 = RowPaser.FloatPaser(cdb_val, "rin_r2"),
                                        rin_r3 = RowPaser.FloatPaser(cdb_val, "rin_r3"),
                                        rin_r4 = RowPaser.FloatPaser(cdb_val, "rin_r4"),
                                        rin_r5 = RowPaser.FloatPaser(cdb_val, "rin_r5"),
                                        rin_r6 = RowPaser.FloatPaser(cdb_val, "rin_r6"),
                                        rin_r7 = RowPaser.FloatPaser(cdb_val, "rin_r7"),
                                    });
                                break;

                            case chartName_stat:
                                if (RowPaser.IntPaser(cdb_val, "eRating") >= 0)
                                {
                                    list_cdb_stat.Add(
                                    new Cdb_stat()
                                    {
                                        eRating = RowPaser.IntPaser(cdb_val, "eRating"),
                                        eIdx = RowPaser.IntPaser(cdb_val, "eIdx"),
                                        exch_per = RowPaser.IntPaser(cdb_val, "exch_per"),
                                        atk_spd = RowPaser.FloatPaser(cdb_val, "atk_spd"),
                                        min = RowPaser.FloatPaser(cdb_val, "min"),
                                        max = RowPaser.FloatPaser(cdb_val, "max"),
                                        min_typ0 = RowPaser.IntPaser(cdb_val, "min_typ0"),// 공격력 (stat id : 1)
                                        max_typ0 = RowPaser.IntPaser(cdb_val, "max_typ0"),
                                        min_typ1 = RowPaser.IntPaser(cdb_val, "min_typ1"),// 방어력 (stat id : 2) 
                                        max_typ1 = RowPaser.IntPaser(cdb_val, "max_typ1"),
                                        min_typ2 = RowPaser.FloatPaser(cdb_val, "min_typ2"),// 명중력 (stat id : 3) 
                                        max_typ2 = RowPaser.FloatPaser(cdb_val, "max_typ2"),
                                        min_typ3 = RowPaser.IntPaser(cdb_val, "min_typ3"),// 치명타 공격력 (stat id : 4) 
                                        max_typ3 = RowPaser.IntPaser(cdb_val, "max_typ3"),
                                        min_typ4 = RowPaser.IntPaser(cdb_val, "min_typ4"),// 체력 (stat id : 5) 
                                        max_typ4 = RowPaser.IntPaser(cdb_val, "max_typ4"),
                                        min_typ5 = RowPaser.FloatPaser(cdb_val, "min_typ5"),// 치명타 성공률 (stat id : 6) 
                                        max_typ5 = RowPaser.FloatPaser(cdb_val, "max_typ5"),
                                        min_typ6 = RowPaser.FloatPaser(cdb_val, "min_typ6"),// 회피율 (stat id : 7) 
                                        max_typ6 = RowPaser.FloatPaser(cdb_val, "max_typ6"),
                                        min_typ7 = RowPaser.IntPaser(cdb_val, "min_typ7"), // 치명타 방어력 (stat id : 8) 
                                        max_typ7 = RowPaser.IntPaser(cdb_val, "max_typ7"),
                                        min_typ8 = RowPaser.FloatPaser(cdb_val, "min_typ8"),
                                        max_typ8 = RowPaser.FloatPaser(cdb_val, "max_typ8"),
                                        min_typ9 = RowPaser.FloatPaser(cdb_val, "min_typ9"),// 치명타 회피 (stat id : 9) 
                                        max_typ9 = RowPaser.FloatPaser(cdb_val, "max_typ9"),
                                        min_typ10 = RowPaser.IntPaser(cdb_val, "min_typ10"),
                                        max_typ10 = RowPaser.IntPaser(cdb_val, "max_typ10"),
                                    });
                                }
                                break;
                            case charName_sop_acce:
                                if(RowPaser.IntPaser(cdb_val, "eRating") > 0)
                                {
                                    cdb_stat_special_Acce cdb_Sop_acce = new cdb_stat_special_Acce()
                                    {
                                        eRating = RowPaser.IntPaser(cdb_val, "eRating"),
                                        eIdx = RowPaser.IntPaser(cdb_val, "eIdx"),
                                        min_sop_id1 = RowPaser.FloatPaser(cdb_val, "min_sop_id1"),
                                        max_sop_id1 = RowPaser.FloatPaser(cdb_val, "max_sop_id1"),
                                        min_sop_id2 = RowPaser.FloatPaser(cdb_val, "min_sop_id2"),
                                        max_sop_id2 = RowPaser.FloatPaser(cdb_val, "max_sop_id2"),
                                        min_sop_id3 = RowPaser.FloatPaser(cdb_val, "min_sop_id3"),
                                        max_sop_id3 = RowPaser.FloatPaser(cdb_val, "max_sop_id3"),
                                        min_sop_id4 = RowPaser.FloatPaser(cdb_val, "min_sop_id4"),
                                        max_sop_id4 = RowPaser.FloatPaser(cdb_val, "max_sop_id4"),
                                        min_sop_id5 = RowPaser.FloatPaser(cdb_val, "min_sop_id5"),
                                        max_sop_id5 = RowPaser.FloatPaser(cdb_val, "max_sop_id5"),
                                        min_sop_id6 = RowPaser.FloatPaser(cdb_val, "min_sop_id6"),
                                        max_sop_id6 = RowPaser.FloatPaser(cdb_val, "max_sop_id6"),
                                        min_sop_id7 = RowPaser.FloatPaser(cdb_val, "min_sop_id7"),
                                        max_sop_id7 = RowPaser.FloatPaser(cdb_val, "max_sop_id7"),
                                        min_sop_id8 = RowPaser.FloatPaser(cdb_val, "min_sop_id8"),
                                        max_sop_id8 = RowPaser.FloatPaser(cdb_val, "max_sop_id8"),
                                        min_sop_id9 = RowPaser.FloatPaser(cdb_val, "min_sop_id9"),
                                        max_sop_id9 = RowPaser.FloatPaser(cdb_val, "max_sop_id9"),
                                        min_sop_id10 = RowPaser.FloatPaser(cdb_val, "min_sop_id10"),
                                        max_sop_id10 = RowPaser.FloatPaser(cdb_val, "max_sop_id10"),
                                    };
                                    list_cdb_stat_sop_Acce.Add(cdb_Sop_acce);
                                }
                                break;

                            case chartName_skill:
                                cdb_stat_skill cdb_skill = new cdb_stat_skill()
                                {
                                    s_idx = RowPaser.IntPaser(cdb_val, "s_idx"),
                                    s_type = RowPaser.IntPaser(cdb_val, "s_type"),
                                    s_rating = RowPaser.IntPaser(cdb_val, "s_rating"),
                                    s_pnt = RowPaser.IntPaser(cdb_val, "s_pnt"),
                                    s_lck_opn_dgTopNbr = RowPaser.IntPaser(cdb_val, "s_lck_opn_dgTopNbr"),
                                    atk_atv_cnt = RowPaser.IntPaser(cdb_val, "atk_atv_cnt"),
                                    s_atk_cnt_check = RowPaser.StrPaser(cdb_val, "s_atk_cnt_check"),
                                    s_bdf_type = RowPaser.StrPaser(cdb_val, "s_bdf_type"),
                                    s_chk_bdf_atcker = RowPaser.StrPaser(cdb_val, "s_chk_bdf_atcker"),
                                    is_atv_hit_dmg = RowPaser.BoolPaser(cdb_val, "is_atv_hit_dmg"),
                                    is_atv_end_hit = RowPaser.BoolPaser(cdb_data, "is_atv_end_hit"),
                                    is_atk_range = RowPaser.BoolPaser(cdb_val, "is_atk_range"),
                                    f_mtp_val1 = RowPaser.FloatPaser(cdb_val, "f_mtp_val1"),
                                    f_mtp_val2 = RowPaser.FloatPaser(cdb_val, "f_mtp_val2"),
                                    f_mtp_val3 = RowPaser.FloatPaser(cdb_val, "f_mtp_val3"),
                                    f_mtp_pow1 = RowPaser.FloatPaser(cdb_val, "f_mtp_pow1"),
                                    f_mtp_pow2 = RowPaser.FloatPaser(cdb_val, "f_mtp_pow2"),
                                    f_mtp_pow3 = RowPaser.FloatPaser(cdb_val, "f_mtp_pow3")
                                };
                                list_cdb_stat_Skill.Add(cdb_skill);
                                break;

                            // 밸런스 
                            case chartName_balance:
                                string blcID = RowPaser.StrPaser(cdb_val, "balance_id");
                                if (!string.IsNullOrEmpty(blcID))
                                {
                                    dic_cdb_balance.Add(
                                    blcID,
                                    new Balance()
                                    {
                                        balance_id = RowPaser.StrPaser(cdb_val, "balance_id"),
                                        val_long = RowPaser.LongPaser(cdb_val, "val_long"),
                                        val_int = RowPaser.IntPaser(cdb_val, "val_int"),
                                        val_int_array = RowPaser.ListIntPaser(cdb_val, "val_int_array"),
                                        val_int_idx = RowPaser.IntPaser(cdb_val, "val_int_idx"),
                                        val_int_level = RowPaser.IntPaser(cdb_val, "val_int_level"),
                                        val_float = RowPaser.FloatPaser(cdb_val, "val_float"),
                                        val_float_array = RowPaser.ListFloatPaser(cdb_val, "val_float_array"),
                                        val_float_second = RowPaser.FloatPaser(cdb_val, "val_float_second"),
                                        val_float_percent = RowPaser.FloatPaser(cdb_val, "val_float_percent"),
                                        val_string = RowPaser.StrPaser(cdb_val, "val_string"),
                                    });
                                }
                                break;

                            // 몬스터 
                            case chartName_MonsterSstat:
                                int chpt_id = RowPaser.IntPaser(cdb_val, "chpt_id");
                                int stg_id = RowPaser.IntPaser(cdb_val, "stg_id");
                                int eq_rt = RowPaser.IntPaser(cdb_val, "eq_rat");
                                if (eq_rt > 0)
                                {
                                    if (chpt_id <= 30) // 30챕터 이전 
                                    {
                                        cdb_chpt_mnst_stat cdb_mnst_stat = new cdb_chpt_mnst_stat()
                                        {
                                            eq_rat = eq_rt,
                                            eq_id = RowPaser.IntPaser(cdb_val, "eq_id"),
                                            chpt_dvs_nbr = RowPaser.IntPaser(cdb_val, "chpt_dvs_nbr"),

                                            // 챕터 스테이지 
                                            chpt_norm_lv = RowPaser.IntPaser(cdb_val, "chpt_norm_lv"),
                                            chpt_ehnt_lv = RowPaser.IntPaser(cdb_val, "chpt_ehnt_lv"),
                                            chpt_m_st_rlv = RowPaser.IntPaser(cdb_val, "chpt_m_st_rlv"),

                                            // 챕터 반복 번호 
                                            chpt_id = RowPaser.IntPaser(cdb_val, "chpt_id"),
                                            stg_id = RowPaser.IntPaser(cdb_val, "stg_id"),
                                            map_nbr = RowPaser.IntPaser(cdb_val, "map_nbr"),
                                        };

                                        list_cdb_chpt_mnst_stat.Add(cdb_mnst_stat);

                                        // 반복 
                                        if (dic_cdb_chpt_mnst_loop.ContainsKey(cdb_mnst_stat.chpt_id) == false)
                                            dic_cdb_chpt_mnst_loop.Add(cdb_mnst_stat.chpt_id, new List<cdb_chpt_mnst_stat>());

                                        dic_cdb_chpt_mnst_loop[cdb_mnst_stat.chpt_id].Add(cdb_mnst_stat);
                                    }
                                    else
                                    {
                                        if (stg_id == 1)
                                        {
                                            int f_norm_lv_pow = RowPaser.IntPaser(cdb_val, "norm_lv_pow");
                                            int f_chpt_norm_lv = RowPaser.IntPaser(cdb_val, "chpt_norm_lv");
                                            for (int f_stg_id = 0; f_stg_id < 100; f_stg_id++)
                                            {
                                                int f_chpt_dvs_nbr = RowPaser.IntPaser(cdb_val, "chpt_dvs_nbr") + f_stg_id;
                                                cdb_chpt_mnst_stat cdb_mnst_stat = new cdb_chpt_mnst_stat()
                                                {
                                                    eq_rat = eq_rt,
                                                    eq_id = RowPaser.IntPaser(cdb_val, "eq_id"),
                                                    chpt_dvs_nbr = f_chpt_dvs_nbr, // RowPaser.IntPaser(cdb_val, "chpt_dvs_nbr"),

                                                    // 챕터 스테이지 
                                                    chpt_norm_lv = f_chpt_norm_lv + (f_norm_lv_pow * (f_stg_id + 1)), // RowPaser.IntPaser(cdb_val, "chpt_norm_lv"),
                                                    chpt_ehnt_lv = RowPaser.IntPaser(cdb_val, "chpt_ehnt_lv"),
                                                    chpt_m_st_rlv = RowPaser.IntPaser(cdb_val, "chpt_m_st_rlv"),

                                                    // 챕터 반복 번호 
                                                    chpt_id = RowPaser.IntPaser(cdb_val, "chpt_id"),
                                                    stg_id = f_stg_id + 1, // RowPaser.IntPaser(cdb_val, "stg_id"),
                                                    map_nbr = RowPaser.IntPaser(cdb_val, "map_nbr"),
                                                };

                                                list_cdb_chpt_mnst_stat.Add(cdb_mnst_stat);

                                                // 반복 
                                                if (dic_cdb_chpt_mnst_loop.ContainsKey(cdb_mnst_stat.chpt_id) == false)
                                                    dic_cdb_chpt_mnst_loop.Add(cdb_mnst_stat.chpt_id, new List<cdb_chpt_mnst_stat>());

                                                dic_cdb_chpt_mnst_loop[cdb_mnst_stat.chpt_id].Add(cdb_mnst_stat);
                                            }
                                        }
                                    }
                                }
                                break;

                            case chartName_DungeonTop:
                                int dgTopCnt = list_cdb_dungeon_top.Count;
                                int dgTopNbr = RowPaser.IntPaser(cdb_val, "nbr");
                                if (dgTopNbr > 0 || (dgTopNbr == 0 && dgTopCnt == 0))
                                {
                                    cdb_dungeon_top cdb_dg_top = new cdb_dungeon_top()
                                    {
                                        nbr = dgTopNbr,
                                        eq_rt = RowPaser.IntPaser(cdb_val, "eq_rt"),
                                        eq_id = RowPaser.IntPaser(cdb_val, "eq_id"),
                                        dg_top_norm_lv = RowPaser.IntPaser(cdb_val, "dg_top_norm_lv"),
                                        dg_top_ehnt_lv = RowPaser.IntPaser(cdb_val, "dg_top_ehnt_lv"),
                                        dg_top_m_st_rlv = RowPaser.IntPaser(cdb_val, "dg_top_m_st_rlv"),
                                        dg_top_op_st_rlv = RowPaser.IntPaser(cdb_val, "dg_top_op_st_rlv"),

                                        reward = new DungeonReward()
                                        {
                                            qst_gold = RowPaser.FloatPaser(cdb_val, "qst_gold"),

                                            rw_eq_ty = RowPaser.IntPaser(cdb_val, "rw_eq_ty"),
                                            rw_eq_rt = RowPaser.IntPaser(cdb_val, "rw_eq_rt"),
                                            rw_eq_id = RowPaser.IntPaser(cdb_val, "rw_eq_id"),

                                            rw_eqac_ty = RowPaser.IntPaser(cdb_val, "rw_eqac_ty"),
                                            rw_eqac_rt = RowPaser.IntPaser(cdb_val, "rw_eqac_rt"),
                                            rw_eqac_id = RowPaser.IntPaser(cdb_val, "rw_eqac_id"),

                                            rw_sk_id = RowPaser.IntPaser(cdb_val, "rw_sk_id"),
                                            rw_sk_cnt = RowPaser.IntPaser(cdb_val, "rw_sk_cnt"),

                                            rw_eq_pce_rt = RowPaser.IntPaser(cdb_val, "rw_eq_pce_rt"),
                                            rw_eq_pce_cnt = RowPaser.IntPaser(cdb_val, "rw_eq_pce_cnt"),
                                            rw_eqac_pce_rt = RowPaser.IntPaser(cdb_val, "rw_eqac_pce_rt"),
                                            rw_eqac_pce_cnt = RowPaser.IntPaser(cdb_val, "rw_eqac_pce_cnt"),

                                            rw_equip_ac_crystal = new DungeonReward.EquipAcCrystal()
                                            {
                                                rw_goods_ether_cnt = RowPaser.IntPaser(cdb_val, "rw_goods_ether_cnt"),
                                            },
                                        }
                                    };

                                    list_cdb_dungeon_top.Add(cdb_dg_top);
                                }
                                break;

                            case chartName_DungeonMine:
                                int dgMineCnt = list_cdb_dungeon_mine.Count;
                                int dgMineNbr = RowPaser.IntPaser(cdb_val, "nbr");
                                if (dgMineNbr > 0 || (dgMineNbr == 0 && dgMineCnt == 0))
                                {
                                    var mine_json_rw_eq_ehnt_ston = JSONObject.Create(RowPaser.StrPaser(cdb_val, "rw_eq_ehnt_ston"));
                                    var mine_json_rw_eqac_ehnt_ston = JSONObject.Create(RowPaser.StrPaser(cdb_val, "rw_eqac_ehnt_ston"));
                                    var mine_json_rw_eq_pces = JSONObject.Create(RowPaser.StrPaser(cdb_val, "rw_eq_pces"));
                                    var mine_json_rw_eqac_pces = JSONObject.Create(RowPaser.StrPaser(cdb_val, "rw_eqac_pces"));
                                    cdb_dungeon_mine cdb_dg_mine = new cdb_dungeon_mine()
                                    {
                                        nbr = dgMineNbr,
                                        eq_rt = RowPaser.IntPaser(cdb_val, "eq_rt"),
                                        eq_id = RowPaser.IntPaser(cdb_val, "eq_id"),
                                        dg_mine_norm_lv = RowPaser.IntPaser(cdb_val, "dg_mine_norm_lv"),
                                        dg_mine_ehnt_lv = RowPaser.IntPaser(cdb_val, "dg_mine_ehnt_lv"),
                                        dg_mine_m_st_rlv = RowPaser.IntPaser(cdb_val, "dg_mine_m_st_rlv"),
                                        dg_mine_op_st_rlv = RowPaser.IntPaser(cdb_val, "dg_mine_op_st_rlv"),

                                        reward = new DungeonReward()
                                        {
                                            qst_gold = RowPaser.FloatPaser(cdb_val, "qst_gold"),
                                            // 장비 강화석 보상
                                            rw_eq_ehnt_ston = new List<DungeonReward.EnhantSton>()
                                            {
                                                new DungeonReward.EnhantSton() {
                                                    rnk = mine_json_rw_eq_ehnt_ston.list[0]["rnk"].str,
                                                    rwds = new List<DungeonReward.EnhantSton.Rwd>()
                                                    {
                                                        JsonUtility.FromJson<DungeonReward.EnhantSton.Rwd> (mine_json_rw_eq_ehnt_ston.list[0]["rwd"][0].ToString()),
                                                        JsonUtility.FromJson<DungeonReward.EnhantSton.Rwd> (mine_json_rw_eq_ehnt_ston.list[0]["rwd"][1].ToString()),
                                                        JsonUtility.FromJson<DungeonReward.EnhantSton.Rwd> (mine_json_rw_eq_ehnt_ston.list[0]["rwd"][2].ToString()),
                                                        JsonUtility.FromJson<DungeonReward.EnhantSton.Rwd> (mine_json_rw_eq_ehnt_ston.list[0]["rwd"][3].ToString()),
                                                    }
                                                },
                                                new DungeonReward.EnhantSton() {
                                                    rnk = mine_json_rw_eq_ehnt_ston.list[1]["rnk"].str,
                                                    rwds = new List<DungeonReward.EnhantSton.Rwd>()
                                                    {
                                                        JsonUtility.FromJson<DungeonReward.EnhantSton.Rwd> (mine_json_rw_eq_ehnt_ston.list[1]["rwd"][0].ToString()),
                                                        JsonUtility.FromJson<DungeonReward.EnhantSton.Rwd> (mine_json_rw_eq_ehnt_ston.list[1]["rwd"][1].ToString()),
                                                        JsonUtility.FromJson<DungeonReward.EnhantSton.Rwd> (mine_json_rw_eq_ehnt_ston.list[1]["rwd"][2].ToString()),
                                                        JsonUtility.FromJson<DungeonReward.EnhantSton.Rwd> (mine_json_rw_eq_ehnt_ston.list[1]["rwd"][3].ToString()),
                                                    }
                                                },
                                                new DungeonReward.EnhantSton() {
                                                    rnk = mine_json_rw_eq_ehnt_ston.list[2]["rnk"].str,
                                                    rwds = new List<DungeonReward.EnhantSton.Rwd>()
                                                    {
                                                        JsonUtility.FromJson<DungeonReward.EnhantSton.Rwd> (mine_json_rw_eq_ehnt_ston.list[2]["rwd"][0].ToString()),
                                                        JsonUtility.FromJson<DungeonReward.EnhantSton.Rwd> (mine_json_rw_eq_ehnt_ston.list[2]["rwd"][1].ToString()),
                                                        JsonUtility.FromJson<DungeonReward.EnhantSton.Rwd> (mine_json_rw_eq_ehnt_ston.list[2]["rwd"][2].ToString()),
                                                        JsonUtility.FromJson<DungeonReward.EnhantSton.Rwd> (mine_json_rw_eq_ehnt_ston.list[2]["rwd"][3].ToString()),
                                                    }
                                                },
                                                new DungeonReward.EnhantSton() {
                                                    rnk = mine_json_rw_eq_ehnt_ston.list[3]["rnk"].str,
                                                    rwds = new List<DungeonReward.EnhantSton.Rwd>()
                                                    {
                                                        JsonUtility.FromJson<DungeonReward.EnhantSton.Rwd> (mine_json_rw_eq_ehnt_ston.list[3]["rwd"][0].ToString()),
                                                        JsonUtility.FromJson<DungeonReward.EnhantSton.Rwd> (mine_json_rw_eq_ehnt_ston.list[3]["rwd"][1].ToString()),
                                                        JsonUtility.FromJson<DungeonReward.EnhantSton.Rwd> (mine_json_rw_eq_ehnt_ston.list[3]["rwd"][2].ToString()),
                                                        JsonUtility.FromJson<DungeonReward.EnhantSton.Rwd> (mine_json_rw_eq_ehnt_ston.list[3]["rwd"][3].ToString()),
                                                    }
                                                },
                                            },
                                            // 장신구 강화석 보상
                                            rw_eqac_ehnt_ston = new List<DungeonReward.EnhantSton>()
                                            {
                                                new DungeonReward.EnhantSton() {
                                                    rnk = mine_json_rw_eq_ehnt_ston.list[0]["rnk"].str,
                                                    rwds = new List<DungeonReward.EnhantSton.Rwd>()
                                                    {
                                                        JsonUtility.FromJson<DungeonReward.EnhantSton.Rwd> (mine_json_rw_eqac_ehnt_ston.list[0]["rwd"][0].ToString()),
                                                        JsonUtility.FromJson<DungeonReward.EnhantSton.Rwd> (mine_json_rw_eqac_ehnt_ston.list[0]["rwd"][1].ToString()),
                                                        JsonUtility.FromJson<DungeonReward.EnhantSton.Rwd> (mine_json_rw_eqac_ehnt_ston.list[0]["rwd"][2].ToString()),
                                                        JsonUtility.FromJson<DungeonReward.EnhantSton.Rwd> (mine_json_rw_eqac_ehnt_ston.list[0]["rwd"][3].ToString()),
                                                    }
                                                },
                                                new DungeonReward.EnhantSton() {
                                                    rnk = mine_json_rw_eq_ehnt_ston.list[1]["rnk"].str,
                                                    rwds = new List<DungeonReward.EnhantSton.Rwd>()
                                                    {
                                                        JsonUtility.FromJson<DungeonReward.EnhantSton.Rwd> (mine_json_rw_eqac_ehnt_ston.list[1]["rwd"][0].ToString()),
                                                        JsonUtility.FromJson<DungeonReward.EnhantSton.Rwd> (mine_json_rw_eqac_ehnt_ston.list[1]["rwd"][1].ToString()),
                                                        JsonUtility.FromJson<DungeonReward.EnhantSton.Rwd> (mine_json_rw_eqac_ehnt_ston.list[1]["rwd"][2].ToString()),
                                                        JsonUtility.FromJson<DungeonReward.EnhantSton.Rwd> (mine_json_rw_eqac_ehnt_ston.list[1]["rwd"][3].ToString()),
                                                    }
                                                },
                                                new DungeonReward.EnhantSton() {
                                                    rnk = mine_json_rw_eq_ehnt_ston.list[2]["rnk"].str,
                                                    rwds = new List<DungeonReward.EnhantSton.Rwd>()
                                                    {
                                                        JsonUtility.FromJson<DungeonReward.EnhantSton.Rwd> (mine_json_rw_eqac_ehnt_ston.list[2]["rwd"][0].ToString()),
                                                        JsonUtility.FromJson<DungeonReward.EnhantSton.Rwd> (mine_json_rw_eqac_ehnt_ston.list[2]["rwd"][1].ToString()),
                                                        JsonUtility.FromJson<DungeonReward.EnhantSton.Rwd> (mine_json_rw_eqac_ehnt_ston.list[2]["rwd"][2].ToString()),
                                                        JsonUtility.FromJson<DungeonReward.EnhantSton.Rwd> (mine_json_rw_eqac_ehnt_ston.list[2]["rwd"][3].ToString()),
                                                    }
                                                },
                                                new DungeonReward.EnhantSton() {
                                                    rnk = mine_json_rw_eq_ehnt_ston.list[3]["rnk"].str,
                                                    rwds = new List<DungeonReward.EnhantSton.Rwd>()
                                                    {
                                                        JsonUtility.FromJson<DungeonReward.EnhantSton.Rwd> (mine_json_rw_eqac_ehnt_ston.list[3]["rwd"][0].ToString()),
                                                        JsonUtility.FromJson<DungeonReward.EnhantSton.Rwd> (mine_json_rw_eqac_ehnt_ston.list[3]["rwd"][1].ToString()),
                                                        JsonUtility.FromJson<DungeonReward.EnhantSton.Rwd> (mine_json_rw_eqac_ehnt_ston.list[3]["rwd"][2].ToString()),
                                                        JsonUtility.FromJson<DungeonReward.EnhantSton.Rwd> (mine_json_rw_eqac_ehnt_ston.list[3]["rwd"][3].ToString()),
                                                    }
                                                },
                                            },
                                            // 장비 조각 보상 (랭크 보상)
                                            rw_eq_pces = new List<DungeonReward.EquipPiece>()
                                            {
                                                new DungeonReward.EquipPiece() {
                                                    rnk = mine_json_rw_eq_pces.list[0]["rnk"].str,
                                                    rwds = new List<DungeonReward.EquipPiece.Rwd>()
                                                    {
                                                        JsonUtility.FromJson<DungeonReward.EquipPiece.Rwd> (mine_json_rw_eq_pces.list[0]["rwd"][0].ToString()),
                                                        JsonUtility.FromJson<DungeonReward.EquipPiece.Rwd> (mine_json_rw_eq_pces.list[0]["rwd"][1].ToString()),
                                                        JsonUtility.FromJson<DungeonReward.EquipPiece.Rwd> (mine_json_rw_eq_pces.list[0]["rwd"][2].ToString()),
                                                        JsonUtility.FromJson<DungeonReward.EquipPiece.Rwd> (mine_json_rw_eq_pces.list[0]["rwd"][3].ToString()),
                                                    }
                                                },
                                                new DungeonReward.EquipPiece() {
                                                    rnk = mine_json_rw_eq_pces.list[1]["rnk"].str,
                                                    rwds = new List<DungeonReward.EquipPiece.Rwd>()
                                                    {
                                                        JsonUtility.FromJson<DungeonReward.EquipPiece.Rwd> (mine_json_rw_eq_pces.list[1]["rwd"][0].ToString()),
                                                        JsonUtility.FromJson<DungeonReward.EquipPiece.Rwd> (mine_json_rw_eq_pces.list[1]["rwd"][1].ToString()),
                                                        JsonUtility.FromJson<DungeonReward.EquipPiece.Rwd> (mine_json_rw_eq_pces.list[1]["rwd"][2].ToString()),
                                                        JsonUtility.FromJson<DungeonReward.EquipPiece.Rwd> (mine_json_rw_eq_pces.list[1]["rwd"][3].ToString()),
                                                    }
                                                },
                                                new DungeonReward.EquipPiece() {
                                                    rnk = mine_json_rw_eq_pces.list[2]["rnk"].str,
                                                    rwds = new List<DungeonReward.EquipPiece.Rwd>()
                                                    {
                                                        JsonUtility.FromJson<DungeonReward.EquipPiece.Rwd> (mine_json_rw_eq_pces.list[2]["rwd"][0].ToString()),
                                                        JsonUtility.FromJson<DungeonReward.EquipPiece.Rwd> (mine_json_rw_eq_pces.list[2]["rwd"][1].ToString()),
                                                        JsonUtility.FromJson<DungeonReward.EquipPiece.Rwd> (mine_json_rw_eq_pces.list[2]["rwd"][2].ToString()),
                                                        JsonUtility.FromJson<DungeonReward.EquipPiece.Rwd> (mine_json_rw_eq_pces.list[2]["rwd"][3].ToString()),
                                                    }
                                                },
                                                new DungeonReward.EquipPiece() {
                                                    rnk = mine_json_rw_eq_pces.list[3]["rnk"].str,
                                                    rwds = new List<DungeonReward.EquipPiece.Rwd>()
                                                    {
                                                        JsonUtility.FromJson<DungeonReward.EquipPiece.Rwd> (mine_json_rw_eq_pces.list[3]["rwd"][0].ToString()),
                                                        JsonUtility.FromJson<DungeonReward.EquipPiece.Rwd> (mine_json_rw_eq_pces.list[3]["rwd"][1].ToString()),
                                                        JsonUtility.FromJson<DungeonReward.EquipPiece.Rwd> (mine_json_rw_eq_pces.list[3]["rwd"][2].ToString()),
                                                        JsonUtility.FromJson<DungeonReward.EquipPiece.Rwd> (mine_json_rw_eq_pces.list[3]["rwd"][3].ToString()),
                                                    }
                                                }
                                            },
                                            // 장신구 조각 보상 (랭크 보상)
                                            rw_eqac_pces = new List<DungeonReward.EquipPiece>()
                                            {
                                                new DungeonReward.EquipPiece() {
                                                    rnk = mine_json_rw_eqac_pces.list[0]["rnk"].str,
                                                    rwds = new List<DungeonReward.EquipPiece.Rwd>()
                                                    {
                                                        JsonUtility.FromJson<DungeonReward.EquipPiece.Rwd> (mine_json_rw_eqac_pces.list[0]["rwd"][0].ToString()),
                                                        JsonUtility.FromJson<DungeonReward.EquipPiece.Rwd> (mine_json_rw_eqac_pces.list[0]["rwd"][1].ToString()),
                                                        JsonUtility.FromJson<DungeonReward.EquipPiece.Rwd> (mine_json_rw_eqac_pces.list[0]["rwd"][2].ToString()),
                                                        JsonUtility.FromJson<DungeonReward.EquipPiece.Rwd> (mine_json_rw_eqac_pces.list[0]["rwd"][3].ToString()),
                                                    }
                                                },
                                                new DungeonReward.EquipPiece() {
                                                    rnk = mine_json_rw_eqac_pces.list[1]["rnk"].str,
                                                    rwds = new List<DungeonReward.EquipPiece.Rwd>()
                                                    {
                                                        JsonUtility.FromJson<DungeonReward.EquipPiece.Rwd> (mine_json_rw_eqac_pces.list[1]["rwd"][0].ToString()),
                                                        JsonUtility.FromJson<DungeonReward.EquipPiece.Rwd> (mine_json_rw_eqac_pces.list[1]["rwd"][1].ToString()),
                                                        JsonUtility.FromJson<DungeonReward.EquipPiece.Rwd> (mine_json_rw_eqac_pces.list[1]["rwd"][2].ToString()),
                                                        JsonUtility.FromJson<DungeonReward.EquipPiece.Rwd> (mine_json_rw_eqac_pces.list[1]["rwd"][3].ToString()),
                                                    }
                                                },
                                                new DungeonReward.EquipPiece() {
                                                    rnk = mine_json_rw_eqac_pces.list[2]["rnk"].str,
                                                    rwds = new List<DungeonReward.EquipPiece.Rwd>()
                                                    {
                                                        JsonUtility.FromJson<DungeonReward.EquipPiece.Rwd> (mine_json_rw_eqac_pces.list[2]["rwd"][0].ToString()),
                                                        JsonUtility.FromJson<DungeonReward.EquipPiece.Rwd> (mine_json_rw_eqac_pces.list[2]["rwd"][1].ToString()),
                                                        JsonUtility.FromJson<DungeonReward.EquipPiece.Rwd> (mine_json_rw_eqac_pces.list[2]["rwd"][2].ToString()),
                                                        JsonUtility.FromJson<DungeonReward.EquipPiece.Rwd> (mine_json_rw_eqac_pces.list[2]["rwd"][3].ToString()),
                                                    }
                                                },
                                                new DungeonReward.EquipPiece() {
                                                    rnk = mine_json_rw_eqac_pces.list[3]["rnk"].str,
                                                    rwds = new List<DungeonReward.EquipPiece.Rwd>()
                                                    {
                                                        JsonUtility.FromJson<DungeonReward.EquipPiece.Rwd> (mine_json_rw_eqac_pces.list[3]["rwd"][0].ToString()),
                                                        JsonUtility.FromJson<DungeonReward.EquipPiece.Rwd> (mine_json_rw_eqac_pces.list[3]["rwd"][1].ToString()),
                                                        JsonUtility.FromJson<DungeonReward.EquipPiece.Rwd> (mine_json_rw_eqac_pces.list[3]["rwd"][2].ToString()),
                                                        JsonUtility.FromJson<DungeonReward.EquipPiece.Rwd> (mine_json_rw_eqac_pces.list[3]["rwd"][3].ToString()),
                                                    }
                                                }
                                            }
                                        }
                                    };

                                    list_cdb_dungeon_mine.Add(cdb_dg_mine);
                                }
                                break;

                            case chartName_DungeonRaid:
                                int dgRaidCnt = list_cdb_dungeon_raid.Count;
                                int dgRaidNbr = RowPaser.IntPaser(cdb_val, "nbr");
                                if (dgRaidNbr > 0 || (dgRaidNbr == 0 && dgRaidCnt == 0))
                                {
                                    var raid_json_rw_eq = JSONObject.Create(RowPaser.StrPaser(cdb_val, "rw_pct_eq"));
                                    var raid_json_rw_eq_ac = JSONObject.Create(RowPaser.StrPaser(cdb_val, "rw_pct_eq_ac"));
                                    var raid_json_rw_eq_pces = JSONObject.Create(RowPaser.StrPaser(cdb_val, "rw_eq_pces"));
                                    var raid_json_rw_eqac_pces = JSONObject.Create(RowPaser.StrPaser(cdb_val, "rw_eqac_pces"));
                                    cdb_dungeon_raid cdb_dg_raid = new cdb_dungeon_raid()
                                    {
                                        nbr = dgRaidNbr,
                                        eq_rt = RowPaser.IntPaser(cdb_val, "eq_rt"),
                                        eq_id = RowPaser.IntPaser(cdb_val, "eq_id"),
                                        dg_raid_norm_lv = RowPaser.IntPaser(cdb_val, "dg_raid_norm_lv"),
                                        dg_raid_ehnt_lv = RowPaser.IntPaser(cdb_val, "dg_raid_ehnt_lv"),
                                        dg_raid_m_st_rlv = RowPaser.IntPaser(cdb_val, "dg_raid_m_st_rlv"),
                                        dg_raid_op_st_rlv = RowPaser.IntPaser(cdb_val, "dg_raid_op_st_rlv"),

                                        reward = new DungeonReward()
                                        {
                                            qst_gold = RowPaser.FloatPaser(cdb_val, "qst_gold"),
                                            // 강화 축복 주문서 
                                            rw_ehnt_bless_rt = RowPaser.IntPaser(cdb_val, "rw_ehnt_bless_rt"),
                                            rw_ehnt_bless_cnt = RowPaser.IntPaser(cdb_val, "rw_ehnt_bless_cnt"),

                                            // 장비 보상 (랜덤)
                                            rw_pct_eq = new List<DungeonReward.EquipRatingPercent>()
                                            {
                                                new DungeonReward.EquipRatingPercent ()
                                                {
                                                    pct =  System.Convert.ToSingle(raid_json_rw_eq.list[0]["pct"].ToString()),
                                                    rwds = new List<DungeonReward.EquipRatingPercent.Rwd>()
                                                    {
                                                        JsonUtility.FromJson<DungeonReward.EquipRatingPercent.Rwd> (raid_json_rw_eq.list[0]["rwd"][0].ToString()),
                                                    }
                                                },
                                                new DungeonReward.EquipRatingPercent ()
                                                {
                                                    pct =  System.Convert.ToSingle(raid_json_rw_eq.list[1]["pct"].ToString()),
                                                    rwds = new List<DungeonReward.EquipRatingPercent.Rwd>()
                                                    {
                                                        JsonUtility.FromJson<DungeonReward.EquipRatingPercent.Rwd> (raid_json_rw_eq.list[1]["rwd"][0].ToString()),
                                                    }
                                                },
                                                new DungeonReward.EquipRatingPercent ()
                                                {
                                                    pct =  System.Convert.ToSingle(raid_json_rw_eq.list[2]["pct"].ToString()),
                                                    rwds = new List<DungeonReward.EquipRatingPercent.Rwd>()
                                                    {
                                                        JsonUtility.FromJson<DungeonReward.EquipRatingPercent.Rwd> (raid_json_rw_eq.list[2]["rwd"][0].ToString()),
                                                    }
                                                }
                                            },
                                            // 장신구 보상(랜덤)
                                            rw_pct_eq_ac = new List<DungeonReward.EquipRatingPercent>()
                                            {
                                                new DungeonReward.EquipRatingPercent ()
                                                {
                                                    pct =  System.Convert.ToSingle(raid_json_rw_eq_ac.list[0]["pct"].ToString()),
                                                    rwds = new List<DungeonReward.EquipRatingPercent.Rwd>()
                                                    {
                                                        JsonUtility.FromJson<DungeonReward.EquipRatingPercent.Rwd> (raid_json_rw_eq_ac.list[0]["rwd"][0].ToString()),
                                                    }
                                                },
                                                new DungeonReward.EquipRatingPercent ()
                                                {
                                                    pct =  System.Convert.ToSingle(raid_json_rw_eq_ac.list[1]["pct"].ToString()),
                                                    rwds = new List<DungeonReward.EquipRatingPercent.Rwd>()
                                                    {
                                                        JsonUtility.FromJson<DungeonReward.EquipRatingPercent.Rwd> (raid_json_rw_eq_ac.list[1]["rwd"][0].ToString()),
                                                    }
                                                },
                                                new DungeonReward.EquipRatingPercent ()
                                                {
                                                    pct =  System.Convert.ToSingle(raid_json_rw_eq_ac.list[2]["pct"].ToString()),
                                                    rwds = new List<DungeonReward.EquipRatingPercent.Rwd>()
                                                    {
                                                        JsonUtility.FromJson<DungeonReward.EquipRatingPercent.Rwd> (raid_json_rw_eq_ac.list[2]["rwd"][0].ToString()),
                                                    }
                                                }
                                            },
                                            // 장비 조각 보상 (랭크 보상)
                                            rw_eq_pces = new List<DungeonReward.EquipPiece>()
                                            {
                                                new DungeonReward.EquipPiece()
                                                {
                                                    rnk = raid_json_rw_eq_pces.list[0]["rnk"].str,
                                                    rwds = new List<DungeonReward.EquipPiece.Rwd>()
                                                    {
                                                        JsonUtility.FromJson<DungeonReward.EquipPiece.Rwd> (raid_json_rw_eq_pces.list[0]["rwd"][0].ToString()),
                                                        JsonUtility.FromJson<DungeonReward.EquipPiece.Rwd> (raid_json_rw_eq_pces.list[0]["rwd"][1].ToString()),
                                                        JsonUtility.FromJson<DungeonReward.EquipPiece.Rwd> (raid_json_rw_eq_pces.list[0]["rwd"][2].ToString()),
                                                        JsonUtility.FromJson<DungeonReward.EquipPiece.Rwd> (raid_json_rw_eq_pces.list[0]["rwd"][3].ToString()),
                                                    }
                                                },
                                                new DungeonReward.EquipPiece()
                                                {
                                                    rnk = raid_json_rw_eq_pces.list[1]["rnk"].str,
                                                    rwds = new List<DungeonReward.EquipPiece.Rwd>()
                                                    {
                                                        JsonUtility.FromJson<DungeonReward.EquipPiece.Rwd> (raid_json_rw_eq_pces.list[1]["rwd"][0].ToString()),
                                                        JsonUtility.FromJson<DungeonReward.EquipPiece.Rwd> (raid_json_rw_eq_pces.list[1]["rwd"][1].ToString()),
                                                        JsonUtility.FromJson<DungeonReward.EquipPiece.Rwd> (raid_json_rw_eq_pces.list[1]["rwd"][2].ToString()),
                                                        JsonUtility.FromJson<DungeonReward.EquipPiece.Rwd> (raid_json_rw_eq_pces.list[1]["rwd"][3].ToString()),
                                                    }
                                                },
                                                new DungeonReward.EquipPiece()
                                                {
                                                    rnk = raid_json_rw_eq_pces.list[2]["rnk"].str,
                                                    rwds = new List<DungeonReward.EquipPiece.Rwd>()
                                                    {
                                                        JsonUtility.FromJson<DungeonReward.EquipPiece.Rwd> (raid_json_rw_eq_pces.list[2]["rwd"][0].ToString()),
                                                        JsonUtility.FromJson<DungeonReward.EquipPiece.Rwd> (raid_json_rw_eq_pces.list[2]["rwd"][1].ToString()),
                                                        JsonUtility.FromJson<DungeonReward.EquipPiece.Rwd> (raid_json_rw_eq_pces.list[2]["rwd"][2].ToString()),
                                                        JsonUtility.FromJson<DungeonReward.EquipPiece.Rwd> (raid_json_rw_eq_pces.list[2]["rwd"][3].ToString()),
                                                    }
                                                },
                                                new DungeonReward.EquipPiece()
                                                {
                                                    rnk = raid_json_rw_eq_pces.list[3]["rnk"].str,
                                                    rwds = new List<DungeonReward.EquipPiece.Rwd>()
                                                    {
                                                        JsonUtility.FromJson<DungeonReward.EquipPiece.Rwd> (raid_json_rw_eq_pces.list[3]["rwd"][0].ToString()),
                                                        JsonUtility.FromJson<DungeonReward.EquipPiece.Rwd> (raid_json_rw_eq_pces.list[3]["rwd"][1].ToString()),
                                                        JsonUtility.FromJson<DungeonReward.EquipPiece.Rwd> (raid_json_rw_eq_pces.list[3]["rwd"][2].ToString()),
                                                        JsonUtility.FromJson<DungeonReward.EquipPiece.Rwd> (raid_json_rw_eq_pces.list[3]["rwd"][3].ToString()),
                                                    }
                                                }
                                            },
                                            // 장신구 조각 보상 (랭크 보상)
                                            rw_eqac_pces = new List<DungeonReward.EquipPiece>()
                                            {
                                                new DungeonReward.EquipPiece()
                                                {
                                                    rnk = raid_json_rw_eqac_pces.list[0]["rnk"].str,
                                                    rwds = new List<DungeonReward.EquipPiece.Rwd>()
                                                    {
                                                        JsonUtility.FromJson<DungeonReward.EquipPiece.Rwd> (raid_json_rw_eqac_pces.list[0]["rwd"][0].ToString()),
                                                        JsonUtility.FromJson<DungeonReward.EquipPiece.Rwd> (raid_json_rw_eqac_pces.list[0]["rwd"][1].ToString()),
                                                        JsonUtility.FromJson<DungeonReward.EquipPiece.Rwd> (raid_json_rw_eqac_pces.list[0]["rwd"][2].ToString()),
                                                        JsonUtility.FromJson<DungeonReward.EquipPiece.Rwd> (raid_json_rw_eqac_pces.list[0]["rwd"][3].ToString()),
                                                    }
                                                },
                                                new DungeonReward.EquipPiece()
                                                {
                                                    rnk = raid_json_rw_eqac_pces.list[1]["rnk"].str,
                                                    rwds = new List<DungeonReward.EquipPiece.Rwd>()
                                                    {
                                                        JsonUtility.FromJson<DungeonReward.EquipPiece.Rwd> (raid_json_rw_eqac_pces.list[1]["rwd"][0].ToString()),
                                                        JsonUtility.FromJson<DungeonReward.EquipPiece.Rwd> (raid_json_rw_eqac_pces.list[1]["rwd"][1].ToString()),
                                                        JsonUtility.FromJson<DungeonReward.EquipPiece.Rwd> (raid_json_rw_eqac_pces.list[1]["rwd"][2].ToString()),
                                                        JsonUtility.FromJson<DungeonReward.EquipPiece.Rwd> (raid_json_rw_eqac_pces.list[1]["rwd"][3].ToString()),
                                                    }
                                                },
                                                new DungeonReward.EquipPiece()
                                                {
                                                    rnk = raid_json_rw_eqac_pces.list[2]["rnk"].str,
                                                    rwds = new List<DungeonReward.EquipPiece.Rwd>()
                                                    {
                                                        JsonUtility.FromJson<DungeonReward.EquipPiece.Rwd> (raid_json_rw_eqac_pces.list[2]["rwd"][0].ToString()),
                                                        JsonUtility.FromJson<DungeonReward.EquipPiece.Rwd> (raid_json_rw_eqac_pces.list[2]["rwd"][1].ToString()),
                                                        JsonUtility.FromJson<DungeonReward.EquipPiece.Rwd> (raid_json_rw_eqac_pces.list[2]["rwd"][2].ToString()),
                                                        JsonUtility.FromJson<DungeonReward.EquipPiece.Rwd> (raid_json_rw_eqac_pces.list[2]["rwd"][3].ToString()),
                                                    }
                                                },
                                                new DungeonReward.EquipPiece()
                                                {
                                                    rnk = raid_json_rw_eqac_pces.list[3]["rnk"].str,
                                                    rwds = new List<DungeonReward.EquipPiece.Rwd>()
                                                    {
                                                        JsonUtility.FromJson<DungeonReward.EquipPiece.Rwd> (raid_json_rw_eqac_pces.list[3]["rwd"][0].ToString()),
                                                        JsonUtility.FromJson<DungeonReward.EquipPiece.Rwd> (raid_json_rw_eqac_pces.list[3]["rwd"][1].ToString()),
                                                        JsonUtility.FromJson<DungeonReward.EquipPiece.Rwd> (raid_json_rw_eqac_pces.list[3]["rwd"][2].ToString()),
                                                        JsonUtility.FromJson<DungeonReward.EquipPiece.Rwd> (raid_json_rw_eqac_pces.list[3]["rwd"][3].ToString()),
                                                    }
                                                }
                                            }
                                        }
                                    };

                                    list_cdb_dungeon_raid.Add(cdb_dg_raid);
                                }
                                break;

                            // 챕터 장비 드랍 등급 
                            case chartName_chapter_drop_rating:
                                list_cdb_r_field_chest_rt.Add(
                                    new cdb_r_chapter_drop_rating()
                                    {
                                        chpt_id = RowPaser.IntPaser(cdb_val, "chpt_id"),
                                        drop_rt1 = RowPaser.FloatPaser(cdb_val, "drop_rt1"),
                                        drop_rt2 = RowPaser.FloatPaser(cdb_val, "drop_rt2"),
                                        drop_rt3 = RowPaser.FloatPaser(cdb_val, "drop_rt3"),
                                        drop_rt4 = RowPaser.FloatPaser(cdb_val, "drop_rt4"),
                                        drop_rt5 = RowPaser.FloatPaser(cdb_val, "drop_rt5"),
                                        drop_rt6 = RowPaser.FloatPaser(cdb_val, "drop_rt6"),
                                        drop_rt7 = RowPaser.FloatPaser(cdb_val, "drop_rt7"),
                                    }
                                );
                                break;

                            // 챕터 장비/아이템 드랍 DB
                            case chartName_chapter_drop_db:
                                string gach_type = RowPaser.StrPaser(cdb_val, "gach_type");
                                if (string.Equals(gach_type, "eq"))
                                {
                                    for (int chpt = 1; chpt <= 30; chpt++)
                                    {
                                        float pct = RowPaser.FloatPaser(cdb_val, string.Format("chpt{0}_pct", chpt));
                                        if (pct > 0)
                                        {
                                            if (dic_cdb_r_field_drop_result.ContainsKey(chpt.ToString()) == false)
                                                dic_cdb_r_field_drop_result.Add(chpt.ToString(), new List<cdb_r_chapter_equip_drop_result>());

                                            string drop_db_k = cdb_val.ContainsKey("eq_ty_rt_id") ? "eq_ty_rt_id" : string.Format("chpt{0}_eq_ty_rt_id", chpt);
                                            string[] v = RowPaser.StrPaser(cdb_val, drop_db_k).Split('_');
                                            dic_cdb_r_field_drop_result[chpt.ToString()].Add(
                                            new cdb_r_chapter_equip_drop_result()
                                            {
                                                chpt_pct = pct,
                                                ty = int.Parse(v[0]),
                                                rt = int.Parse(v[1]),
                                                id = int.Parse(v[2])
                                            });
                                        }
                                    }
                                }
                                else if (string.Equals(gach_type, "it"))
                                {
                                    LogPrint.Print("r_field_drop_result it cdb_key : " + cdb_key + ", cdb_val : " + cdb_val.ToJson());
                                    if (RowPaser.FloatPaser(cdb_val, "it_pct") > 0)
                                    {
                                        dic_cdb_r_field_chest_item.Add
                                        (
                                            new cdb_r_chapter_item_drop_result()
                                            {
                                                pct = RowPaser.FloatPaser(cdb_val, "it_pct"),
                                                it_ty = RowPaser.IntPaser(cdb_val, "it_type"),
                                                it_rt = RowPaser.IntPaser(cdb_val, "it_rating"),
                                                it_cn = RowPaser.IntPaser(cdb_val, "it_cnt"),
                                            }
                                        );
                                    }
                                }
                                break;
                            case chartName_GgachaPercentage:
                                list_cdb_gacha_percentage.Add(
                                   new cdb_gacha_percentage()
                                   {
                                       gch_name = RowPaser.StrPaser(cdb_val, "gacha_name"),
                                       rt1 = RowPaser.FloatPaser(cdb_val, "rt1"),
                                       rt2 = RowPaser.FloatPaser(cdb_val, "rt2"),
                                       rt3 = RowPaser.FloatPaser(cdb_val, "rt3"),
                                       rt4 = RowPaser.FloatPaser(cdb_val, "rt4"),
                                       rt5 = RowPaser.FloatPaser(cdb_val, "rt5"),
                                       rt6 = RowPaser.FloatPaser(cdb_val, "rt6"),
                                       rt7 = RowPaser.FloatPaser(cdb_val, "rt7"),
                                   }
                               );
                                break;

                            case chartName_r_acce_special_op:
                                lst_cdb_acce_special_op.Add(new cdb_acce_special_op()
                                {
                                    pct = RowPaser.FloatPaser(cdb_val, "pct"),
                                    ac_sop_id = RowPaser.IntPaser(cdb_val, "ac_sop_id"),
                                });
                                break;

                            case chartName_equip_option:
                                cdb_equip_op cdb_eq_op = new cdb_equip_op()
                                {
                                    pt_ty = RowPaser.IntPaser(cdb_val, "pt_ty"),
                                    op1 = RowPaser.StrPaser(cdb_val, "op1"),
                                    op2 = RowPaser.StrPaser(cdb_val, "op2"),
                                    op3 = RowPaser.StrPaser(cdb_val, "op3"),
                                    op4 = RowPaser.StrPaser(cdb_val, "op4"),
                                };
                                dic_cdb_equip_op.Add(cdb_eq_op.pt_ty, cdb_eq_op);
                                break;

                            case chartName_attendance_book:
                                _list_cdb_attendance_book.Add(
                                        new cdb_attendance_book()
                                        {
                                            nbr = RowPaser.IntPaser(cdb_val, "nbr"),
                                            item = new MailDB.Item()
                                            {
                                                item_name = RowPaser.StrPaser(cdb_val, "item_name"),
                                                gift_type = RowPaser.StrPaser(cdb_val, "gift_type"),
                                                ty = RowPaser.IntPaser(cdb_val, "ty"),
                                                rt = RowPaser.IntPaser(cdb_val, "rt"),
                                                idx = RowPaser.IntPaser(cdb_val, "idx"),
                                                count = RowPaser.IntPaser(cdb_val, "count"),
                                            }
                                        }
                                    );
                                break;
                            case chartName_achievements:
                                cdb_achievements __cdb_achievements = new cdb_achievements()
                                {
                                    nbr = RowPaser.IntPaser(cdb_val, "nbr"),
                                    lv = RowPaser.IntPaser(cdb_val, "lv"),
                                    name = RowPaser.StrPaser(cdb_val, "name"),
                                    prog_cmp_cnt = RowPaser.LongPaser(cdb_val, "prog_cmp_cnt"),
                                    prog_cnt_type = RowPaser.IntPaser(cdb_val, "prog_cnt_type"),
                                    prog_tap_move = RowPaser.BoolPaser(cdb_val, "prog_tap_move"),
                                    rwd_reset = RowPaser.BoolPaser(cdb_val, "rwd_reset"),
                                    item = new MailDB.Item()
                                    {
                                        item_name = RowPaser.StrPaser(cdb_val, "item_name"),
                                        gift_type = RowPaser.StrPaser(cdb_val, "gift_type"),
                                        ty = RowPaser.IntPaser(cdb_val, "ty"),
                                        rt = RowPaser.IntPaser(cdb_val, "rt"),
                                        idx = RowPaser.IntPaser(cdb_val, "idx"),
                                        count = RowPaser.IntPaser(cdb_val, "count"),
                                    },
                                };

                                int key = __cdb_achievements.nbr;
                                if (dic_cdb_achievements.ContainsKey(key) == false)
                                    dic_cdb_achievements.Add(__cdb_achievements.nbr, new List<cdb_achievements>());

                                dic_cdb_achievements[key].Add(__cdb_achievements);
                                break;
                            case chartName_daily_mission:
                                list_cdb_daily_mission.Add(
                                    new cdb_achievements()
                                    {
                                        nbr = RowPaser.IntPaser(cdb_val, "nbr"),
                                        lv = 0,
                                        name = RowPaser.StrPaser(cdb_val, "name"),
                                        prog_cmp_cnt = RowPaser.IntPaser(cdb_val, "prog_cmp_cnt"),
                                        prog_cnt_type = RowPaser.IntPaser(cdb_val, "prog_cnt_type"),
                                        prog_tap_move = RowPaser.BoolPaser(cdb_val, "prog_tap_move"),
                                        rwd_reset = RowPaser.BoolPaser(cdb_val, "rwd_reset"),
                                        item = new MailDB.Item()
                                        {
                                            item_name = RowPaser.StrPaser(cdb_val, "item_name"),
                                            gift_type = RowPaser.StrPaser(cdb_val, "gift_type"),
                                            ty = RowPaser.IntPaser(cdb_val, "ty"),
                                            rt = RowPaser.IntPaser(cdb_val, "rt"),
                                            idx = RowPaser.IntPaser(cdb_val, "idx"),
                                            count = RowPaser.IntPaser(cdb_val, "count"),
                                        },
                                    }
                                );
                                break;
                            case chartName_quest:
                                string nbrKey = RowPaser.StrPaser(cdb_val, "nbr");
                                if (!string.IsNullOrEmpty(nbrKey) && !string.Equals(nbrKey, string.Empty))
                                {
                                    cdb_quest.Add
                                    (
                                        new cdb_quest()
                                        {
                                            nbr = RowPaser.IntPaser(cdb_val, "nbr"),
                                            rwd_power = RowPaser.FloatPaser(cdb_val, "rwd_power"),
                                            up_power = RowPaser.FloatPaser(cdb_val, "up_power"),
                                            max_lv = RowPaser.IntPaser(cdb_val, "max_lv"),
                                            title = RowPaser.StrPaser(cdb_val, "title"),
                                            rwd_time = RowPaser.IntPaser(cdb_val, "rwd_time"),
                                            rwd_gold = RowPaser.IntPaser(cdb_val, "rwd_gold"),
                                            up_gold = RowPaser.LongPaser(cdb_val, "up_gold"),

                                            eq_rt = RowPaser.IntPaser(cdb_val, "eq_rt"),
                                            eq_id = RowPaser.IntPaser(cdb_val, "eq_id"),
                                            qst_mnst_drop_gold = RowPaser.IntPaser(cdb_val, "qst_mnst_drop_gold"),

                                            qst_eq_sale_gold = RowPaser.IntPaser(cdb_val, "qst_eq_sale_gold"),
                                            qst_eq_decomp_ruby = RowPaser.IntPaser(cdb_val, "qst_eq_decomp_ruby"),
                                            qst_eq_ac_decomp_ether = RowPaser.IntPaser(cdb_val, "qst_eq_ac_decomp_ether"),

                                            qst_eq_upgrade_gold = RowPaser.IntPaser(cdb_val, "qst_eq_upgrade_gold"),

                                            qst_eq_enhant_gold = RowPaser.IntPaser(cdb_val, "qst_eq_enhant_gold"),
                                            qst_eq_enhant_ruby = RowPaser.IntPaser(cdb_val, "qst_eq_enhant_ruby"),
                                            qst_eq_ac_enhant_ether = RowPaser.IntPaser(cdb_val, "qst_eq_ac_enhant_ether"),

                                            qst_ston_evol_gold = RowPaser.IntPaser(cdb_val, "qst_ston_evol_gold"),

                                            qst_eq_enhant_transfer_gold = RowPaser.LongPaser(cdb_val, "qst_eq_enhant_transfer_gold"),
                                            qst_eq_enhant_transfer_dia = RowPaser.IntPaser(cdb_val, "qst_eq_enhant_transfer_dia"),

                                            qst_eq_ac_op_change_gold = RowPaser.LongPaser(cdb_val, "qst_eq_ac_op_change_gold"),
                                            qst_eq_ac_op_change_dia = RowPaser.IntPaser(cdb_val, "qst_eq_ac_op_change_dia"),

                                            qst_eq_op_change_gold = RowPaser.LongPaser(cdb_val, "qst_eq_op_change_gold"),
                                            qst_eq_op_change_dia = RowPaser.IntPaser(cdb_val, "qst_eq_op_change_dia"),
                                            qst_eq_sop_change_dia = RowPaser.IntPaser(cdb_val, "qst_eq_sop_change_dia"),

                                            qst_eq_ac_synt_gold = RowPaser.LongPaser(cdb_val, "qst_eq_ac_synt_gold"),
                                            qst_eq_ac_synt_tbc_dia = RowPaser.IntPaser(cdb_val, "qst_eq_ac_synt_tbc_dia"),

                                            qst_skill_up_gold = RowPaser.LongPaser(cdb_val, "qst_skill_up_gold"),
                                        }
                                    );
                                }
                                break;
                            case chartName_offline_reward:
                                cdb_offline_reward.Add
                                (
                                   new cdb_offline_reward()
                                   {
                                       hour = RowPaser.IntPaser(cdb_val, "hour"),
                                       qst_gold = RowPaser.FloatPaser(cdb_val, "qst_gold"),
                                       ruby = RowPaser.IntPaser(cdb_val, "ruby"),
                                       ether = RowPaser.IntPaser(cdb_val, "ether"),
                                       piece_equip_rt5 = RowPaser.IntPaser(cdb_val, "piece_equip_rt5"),
                                       piece_equip_rt6 = RowPaser.IntPaser(cdb_val, "piece_equip_rt6"),
                                       piece_equip_rt7 = RowPaser.IntPaser(cdb_val, "piece_equip_rt7"),
                                       piece_acce_rt5 = RowPaser.IntPaser(cdb_val, "piece_acce_rt5"),
                                   }
                               );
                                break;
                            case chartName_pet:
                                if(RowPaser.IntPaser(cdb_val, "p_rt") > 0)
                                {
                                    list_cdb_pet.Add
                                   (
                                       new cdb_pet()
                                       {
                                           p_id = RowPaser.IntPaser(cdb_val, "p_id"),
                                           p_rt = RowPaser.IntPaser(cdb_val, "p_rt"),
                                           name = RowPaser.StrPaser(cdb_val, "name"),
                                           max_lv = RowPaser.IntPaser(cdb_val, "max_lv"),

                                           op1v_min = RowPaser.FloatPaser(cdb_val, "op1v_min"),
                                           op1v_max = RowPaser.FloatPaser(cdb_val, "op1v_max"),
                                           op2v_min = RowPaser.FloatPaser(cdb_val, "op2v_min"),
                                           op2v_max = RowPaser.FloatPaser(cdb_val, "op2v_max"),
                                           op4v_min = RowPaser.FloatPaser(cdb_val, "op4v_min"),
                                           op4v_max = RowPaser.FloatPaser(cdb_val, "op4v_max"),
                                           op5v_min = RowPaser.FloatPaser(cdb_val, "op5v_min"),
                                           op5v_max = RowPaser.FloatPaser(cdb_val, "op5v_max"),
                                           op8v_min = RowPaser.FloatPaser(cdb_val, "op8v_min"),
                                           op8v_max = RowPaser.FloatPaser(cdb_val, "op8v_max"),
                                       }
                                   );
                                }
                                break;
                            case chartName_pet_sop:
                                if(RowPaser.IntPaser(cdb_val, "p_rt") > 0)
                                {
                                    list_cdb_pet_sop.Add
                                   (
                                       new cdb_pet_sop()
                                       {
                                           r_pct = RowPaser.IntPaser(cdb_val, "r_pct"),
                                           sop_id = RowPaser.IntPaser(cdb_val, "sop_id"),
                                           sop_name = RowPaser.StrPaser(cdb_val, "sop_name"),

                                           p_rt = RowPaser.IntPaser(cdb_val, "p_rt"),
                                           p_id = RowPaser.IntPaser(cdb_val, "p_id"),

                                           sop1v_min = RowPaser.FloatPaser(cdb_val, "sop1v_min"),
                                           sop1v_max = RowPaser.FloatPaser(cdb_val, "sop1v_max"),
                                           sop2v_min = RowPaser.FloatPaser(cdb_val, "sop2v_min"),
                                           sop2v_max = RowPaser.FloatPaser(cdb_val, "sop2v_max"),
                                           sop3v_min = RowPaser.FloatPaser(cdb_val, "sop3v_min"),
                                           sop3v_max = RowPaser.FloatPaser(cdb_val, "sop3v_max"),
                                           sop4v_min = RowPaser.FloatPaser(cdb_val, "sop4v_min"),
                                           sop4v_max = RowPaser.FloatPaser(cdb_val, "sop4v_max"),
                                           sop5v_min = RowPaser.FloatPaser(cdb_val, "sop5v_min"),
                                           sop5v_max = RowPaser.FloatPaser(cdb_val, "sop5v_max"),
                                           sop6v_min = RowPaser.FloatPaser(cdb_val, "sop6v_min"),
                                           sop6v_max = RowPaser.FloatPaser(cdb_val, "sop6v_max"),
                                           sop7v_min = RowPaser.FloatPaser(cdb_val, "sop7v_min"),
                                           sop7v_max = RowPaser.FloatPaser(cdb_val, "sop7v_max"),
                                           sop8v_min = RowPaser.FloatPaser(cdb_val, "sop8v_min"),
                                           sop8v_max = RowPaser.FloatPaser(cdb_val, "sop8v_max"),
                                           sop9v_min = RowPaser.FloatPaser(cdb_val, "sop9v_min"),
                                           sop9v_max = RowPaser.FloatPaser(cdb_val, "sop9v_max"),
                                       }
                                   );
                                }
                               
                                break;
                        }
                    }
                }
                else
                {
                    return false;
                }
            }

            LogPrint.EditorPrint("<color=magenta>--------------------------- SUCCESS - SetChartParsing_cdb_stat ----------------------------</color>");
            return true;
        }
#endregion

        // 장비 type 스킬 idx 
        /*
        // 버블은 적에게 일반 타격을 성공하였을때 획득 
        // 전투시 일반 몬스터는 물약 회복이 가능하지만 보스와의 전투에서는 물약 회복 불가, 자신이 공격할때 체력이 일정% 이하일경우 공격하지않고 물약을 먹는다. 

        // ##### ------------- 장비 [ 매인 stat ] [강화:해당 매인 스탯만 증가됨(랜덤옵션스탯은 증가X이고, 획득시 1~10레벨이 랜덤으로 지정됨)] ------------- #####
        // -> 파츠 Type Idx : 0:무기      1:방패      2:헬멧      3:어깨            4:갑옷     5:팔                  6:바지    7:부츠            [8:목걸이]  [9:귀고리   ]  [10:반지]  
        // -> 스탯 Type Idx : 1:공격력,   2:방어력,   3:명중력,   4:치명타 공격력,  5:체력,    6:치명타 성공 확률,   7:회피,   8:치명타 방어력   [3:명중력]  [9:치명타 회피]  [1:공격력]
#           #                       #                 #                                          #

        // ##### ------------- 악세 [ 매인 스탯 ] [강화:해당 매인 스탯만 증가됨(랜덤옵션스탯은 증가X이고, 획득시 1~10레벨이 랜덤으로 지정됨)] ------------- #####
        // -> 파츠 Type Idx : 8:목걸이, 9:귀고리    10:반지     
        // -> 스탯 Type Idx : 3:명중력, 9:공격속도, 1:공격력,   

        // ##### 랜덤 옵션(일반 1개(1번 확정랜덤), 중급 1개(1번 확정랜덤), 고급 1~2개(1번 확정랜덤, 2번 랜덤), 희귀 2개(1번,2번 확정랜덤), 영웅 2~3개(1번,2번 확정랜덤, 3번 랜덤), 고대 3개(1,2,3번 확정 랜덤), 전설 3~4개(1,2,3번 확정랜덤, 4번 랜덤)
        // 옵션 슬롯별 스탯 id 랜범 배정
        // 각 등급별 장비 총 19개 
        // -> 무기0       : 1,2,4  //  1,5,8  // 1,2,4  //  1,5,8
        // -> 방패1       : 2,4,5  //  2,8,1  // 2,4,5  //  2,8,1
        // -> 헬멧2       : 4,5,8  //  4,1,2  // 4,5,8  //  4,1,2
        // -> 어깨3       : 5,8,1  //  5,2,4  // 5,8,1  //  5,2,4
        // -> 갑옷4       : 8,1,2  //  8,4,5  // 8,1,2  //  8,4,5
        // -> 팔  5         : 1,2,4  //  1,5,8  // 1,2,4  //  1,5,8
        // -> 바지6       : 2,4,5  //  2,8,1  // 2,4,5  //  2,8,1
        // -> 부츠7       : 4,5,8  //  4,1,2  // 4,5,8  //  4,1,2
        // -> 목걸8       : 5,8,1  //  5,2,4  // 5,8,1  //  5,2,4
        // -> 귀걸9       : 8,1,2  //  8,4,5  // 8,1,2  //  8,4,5
        // -> 반지10      : 1,2,4,5,8  //  1,2,4,5,8  //  1,2,4,5,8  //  1,2,4,5,8
        
        // ##### 장신구 전용 옵션
        // -> 1:pve피해 증가(공격력의 ?%), 2:pvp피해 증가(공격력의 ?%), 3:pve피해 감소(%), 4:pvp피해 감소(%) 5:골드 획득 증가, 6:장비 획득 증가, 
        //    7:보스 몬스터 피해 증가, 8:공격시 현재 체력의 ?%를 회복, 9:공격시 랜덤으로 버블 추가획득, 10:공격시 랜덤으로 상대 버블 차감,
        // ##### 랜덤 전용 옵션 갯수  
        // -> 일반 1개(1번 확정 랜덤), 중급 1개(1번 확정 랜덤), 고급 1~2개(1번 확정 랜덤, 2번 랜덤), 희귀 2개(1번~2번 확정 랜덤), 영웅 2~3개(1번~2번 확정랜덤, 3번 랜덤), 고대 3개(1~3번 확정랜덤), 전설 3~4개(1~3번 확정 랜덤, 4번 랜덤)
        // 랜덤 옵션 획득 1번 -> 1:pve피해 증가(공격력의 ?%), 2:pvp피해 증가(공격력의 ?%)
        // 랜덤 옵션 획득 2번 -> 3:pve피해 감소(%), 4:pvp피해 감소(%)
        // 랜덤 옵션 획득 3번 -> 5:골드 획득 증가, 6:장비 획득 증가, 7:보스 몬스터 피해 증가, 8:공격시 현재 체력의 ?%를 회복 
        // 랜덤 옵션 획득 4번 -> 9:공격시 랜덤으로 버블 추가획득, 10:공격시 랜덤으로 상대 버블 차감

        // ##### 추후 예정
        // ##### 무기 신석 옵션(영웅 등급부터 무기 장비 획득시 랜덤 부여)
        // -> 1:공격시 ?%확률로 ?초간 마비, 2:공격시 ?%확률로 ?초간 침묵, 3:공격시 ?%확률로 ?초간 실명, 
        //    4:공격시 ?%확률로 ?초간 지속성 대미지, 5:공격시 ?%확률로 상대 공격속도 저하, 6:공격시 ?%확률로 ?초간 받은 대미지의 ?%를 반사 
        //    6:공격시 ?%확률로 ?초간 상대 공격력 감소, 7:공격시 ?%확률로 ?초간 나의 공격력 증가, 8:공격시 ?%확률로 ?초간 상대 방어력 감소, 9:공격시 ?%확률로 ?초간 나의 방어력 증가,
        //    10::공격시 ?%확률로 ?초간 상대 치명타 성공 확률 감소, 11::공격시 ?%확률로 ?초간 나의 치명타 확률 증가, 12:공격시 ?%확률로 ?초간 상대 치명타 대미지 감소, 13:공격시 ?%확률로 ?초간 나의 치명타 대미지 증가 
        // ##### 무기 신석 옵션 갯수
        // -> 영웅 1개(1번 확정 랜덤), 고대 2~3개(1번~2번 확정 랜덤, 3번 랜덤), 전설 3~4개(1번~3번 랜덤 확정, 4번 랜덤)
        // 신석 옵션 획득 1번 -> 6:공격시 ?%확률로 ?초간 상대 공격력 감소, 7:공격시 ?%확률로 ?초간 나의 공격력 증가, 8:공격시 ?%확률로 ?초간 상대 방어력 감소, 9:공격시 ?%확률로 ?초간 나의 방어력 증가,
        // 신석 옵션 획득 2번 -> 10::공격시 ?%확률로 ?초간 상대 치명타 성공 확률 감소, 11::공격시 ?%확률로 ?초간 나의 치명타 확률 증가, 12:공격시 ?%확률로 ?초간 상대 치명타 대미지 감소, 13:공격시 ?%확률로 ?초간 나의 치명타 대미지 증가 
        // 신석 옵션 획득 3번 -> 4:공격시 ?%확률로 ?초간 지속성 대미지, 5:공격시 ?%확률로 상대 공격속도 저하, 6:공격시 ?%확률로 ?초간 받은 대미지의 ?%를 반사 
        // 신석 옵션 획득 4번 -> 1:공격시 ?%확률로 ?초간 마비, 2:공격시 ?%확률로 ?초간 침묵, 3:공격시 ?%확률로 ?초간 실명, 

        // 내가 적 공격시 -> 내 공격력[100] - (적의 방어력[33] - 나의 명중력[10]) = 77의 공격이들어감 
        */

        // ###################################################
        // ---------------------- START ----------------------
#region ##### Get 장비 /장신구 차트 데이터 ######
        /// <summary> 장비 파츠별로 매인 스탯 번호 </summary>
        public int GetPartyMainStatId(int parts_type) => GetDicBalance(string.Format("equip.parts.mainStatID_{0}", parts_type)).val_int;

        ///  <summary> 장비 파츠 타입 번호를가지고 장신구 인가 아닌가 체크 (true : 장신구, false : 장비) </summary>
        public bool GetIsPartsTypeAcce(int _getPartsType) => (_getPartsType >= 8 && _getPartsType <= 10) ? true : false;



        #endregion
        //END endregion --------------------------------------

        // ###################################################
        // ---------------------- START ----------------------
        #region ##### 장비 전용 옵션 #####
        /// <summary> 장비 장신구의 랜덤으로 지정된 전용옵션 번호를 가지고 전용 옵션 이름 리턴 </summary>
        public string GetEquipSpecialOptionName(int sop_id) => GameDatabase.GetInstance().chartDB.GetDicBalance(string.Format("equip.sop.id_{0}", sop_id)).val_string;

        public float GetEquipSpecialOptionValue(int eq_sop_id = 0, int eq_sop_rlv = 0)
        {
            if(eq_sop_id == 0 || eq_sop_rlv == 0)
                return 0;

            float f_min = 0f, f_max = 0f;
            switch (eq_sop_id)
            {
                case 1: f_min = GetDicBalance("equip.sop.id_1.min.val").val_float; f_max = GetDicBalance("equip.sop.id_1.max.val").val_float; break;
                case 2: f_min = GetDicBalance("equip.sop.id_2.min.val").val_float; f_max = GetDicBalance("equip.sop.id_2.max.val").val_float; break;
                case 4: f_min = GetDicBalance("equip.sop.id_4.min.val").val_float; f_max = GetDicBalance("equip.sop.id_4.max.val").val_float; break;
                case 5: f_min = GetDicBalance("equip.sop.id_5.min.val").val_float; f_max = GetDicBalance("equip.sop.id_5.max.val").val_float; break;
                case 8: f_min = GetDicBalance("equip.sop.id_8.min.val").val_float; f_max = GetDicBalance("equip.sop.id_8.max.val").val_float; break;
            }

            float f_rVal = (((f_max - f_min) / 10) * eq_sop_rlv) + f_min;
            return f_rVal;
        }

        /// <summary>
        /// 장비 전용 옵션에서 해당 장비 기준 최소 값과 최대 값을 리턴 
        /// </summary>
        public float[] GetEquipSpecialOptionMinMax(int get_ac_rt, int get_ac_id, int get_ac_sop_id)
        {
            float[] returnVal = new float[2] { 0f, 0f };
            float f_min = 0f, f_max = 0f;
            var ch_stat_sop = list_cdb_stat_sop_Acce.Find((cdb_stat_special_Acce obj) => obj.eRating == get_ac_rt && obj.eIdx == get_ac_id);
            switch (get_ac_sop_id)
            {
                case 1: f_min = GetDicBalance("equip.sop.id_1.min.val").val_float; f_max = GetDicBalance("equip.sop.id_1.max.val").val_float; break;
                case 2: f_min = GetDicBalance("equip.sop.id_2.min.val").val_float; f_max = GetDicBalance("equip.sop.id_2.max.val").val_float; break;
                case 4: f_min = GetDicBalance("equip.sop.id_4.min.val").val_float; f_max = GetDicBalance("equip.sop.id_4.max.val").val_float; break;
                case 5: f_min = GetDicBalance("equip.sop.id_5.min.val").val_float; f_max = GetDicBalance("equip.sop.id_5.max.val").val_float; break;
                case 8: f_min = GetDicBalance("equip.sop.id_8.min.val").val_float; f_max = GetDicBalance("equip.sop.id_8.max.val").val_float; break;
            }

            float f_dvi_val = ((f_max - f_min) / 10) * 10;/*랜덤 최대 레벨 10*/
            returnVal[0] = (f_min * 100) / 100;
            returnVal[1] = ((f_dvi_val + f_min) * 100) / 100;
            return returnVal;
        }

        #endregion
        // ###################################################
        // ---------------------- START ----------------------
        #region ##### 장신구 전용 옵션 #####
        /// <summary> 장비 장신구의 랜덤으로 지정된 전용옵션 번호를 가지고 전용 옵션 이름 리턴 </summary>
        public string GetAcceSpecialOptionName(int sop_id)
        {
            if (sop_id == 8)
            {
                float reco_hp = GameDatabase.GetInstance().chartDB.GetDicBalance("ac.sop.stat8.hp.recovery").val_float;
                return string.Format(LanguageGameData.GetInstance().GetString("acce.option.id_8"), reco_hp);
            }
            else return LanguageGameData.GetInstance().GetString(string.Format("acce.option.id_{0}", sop_id));
        }

        /// <summary>
        /// 장신구 데이터를 가지고 [전용 옵션] 값 리턴, 
        /// [0]기본 스탯값 + 강화 스탯값 = 총합, [1]: 강화 레벨에 따른 스탯 값 GetSpecialOptionStatValueAcce
        /// </summary>
        public float[] GetAcceSpecialOptionValue(TableDB.Equipment equip_ac_data) // returnVal[0] : 최종 값, returnVal[1] : 강화로 추가된 값 
        {
            float[] returnVal = new float[2] { 0f, 0f };
            int get_ac_id = equip_ac_data.eq_id;
            int get_ac_rt = equip_ac_data.eq_rt;
            int get_enhant_level = equip_ac_data.m_ehnt_lv;
            int get_ac_sop_id = equip_ac_data.st_sop_ac.id;
            int get_ac_sop_rlv = equip_ac_data.st_sop_ac.rlv; // acce_op_rnd_lv 최소 1, 최대 10 

            float f_min = 0f, f_max = 0f;//, f_dvVal, f_rVal = 0f, f_rVal2 = 0f;
            var ch_stat_sop = list_cdb_stat_sop_Acce.Find((cdb_stat_special_Acce obj) => int.Equals(obj.eRating, get_ac_rt) && int.Equals(obj.eIdx, get_ac_id));
            switch (get_ac_sop_id)
            {
                case 1: f_min = ch_stat_sop.min_sop_id1;    f_max = ch_stat_sop.max_sop_id1;    break;
                case 2: f_min = ch_stat_sop.min_sop_id2;    f_max = ch_stat_sop.max_sop_id2;    break;
                case 3: f_min = ch_stat_sop.min_sop_id3;    f_max = ch_stat_sop.max_sop_id3;    break;
                case 4: f_min = ch_stat_sop.min_sop_id4;    f_max = ch_stat_sop.max_sop_id4;    break;
                case 5: f_min = ch_stat_sop.min_sop_id5;    f_max = ch_stat_sop.max_sop_id5;    break;
                case 6: f_min = ch_stat_sop.min_sop_id6;    f_max = ch_stat_sop.max_sop_id6;    break;
                case 7: f_min = ch_stat_sop.min_sop_id7;    f_max = ch_stat_sop.max_sop_id7;    break;
                case 8: f_min = ch_stat_sop.min_sop_id8;    f_max = ch_stat_sop.max_sop_id8;    break;
                case 9: f_min = ch_stat_sop.min_sop_id9;    f_max = ch_stat_sop.max_sop_id9;    break;
                case 10: f_min = ch_stat_sop.min_sop_id10;  f_max = ch_stat_sop.max_sop_id10;   break;
            }

            float f_rVal = (((f_max - f_min) / 10) * get_ac_sop_rlv) + f_min;
            returnVal[0] = f_rVal;
            return returnVal;
        }

        /// <summary>
        /// 장신구 전용 옵션에서 해당 장비 기준 최소 값과 최대 값을 리턴 
        /// </summary>
        public float[] GetAcceSpecialOptionMinMax(int get_ac_rt, int get_ac_id, int get_ac_sop_id)
        {
            float[] returnVal = new float[2] { 0f, 0f };
            float f_min = 0f, f_max = 0f;
            var ch_stat_sop = list_cdb_stat_sop_Acce.Find((cdb_stat_special_Acce obj) => obj.eRating == get_ac_rt && obj.eIdx == get_ac_id);
            switch (get_ac_sop_id)
            {
                case 1: f_min = ch_stat_sop.min_sop_id1;    f_max = ch_stat_sop.max_sop_id1;    break;
                case 2: f_min = ch_stat_sop.min_sop_id2;    f_max = ch_stat_sop.max_sop_id2;    break;
                case 3: f_min = ch_stat_sop.min_sop_id3;    f_max = ch_stat_sop.max_sop_id3;    break;
                case 4: f_min = ch_stat_sop.min_sop_id4;    f_max = ch_stat_sop.max_sop_id4;    break;
                case 5: f_min = ch_stat_sop.min_sop_id5;    f_max = ch_stat_sop.max_sop_id5;    break;
                case 6: f_min = ch_stat_sop.min_sop_id6;    f_max = ch_stat_sop.max_sop_id6;    break;
                case 7: f_min = ch_stat_sop.min_sop_id7;    f_max = ch_stat_sop.max_sop_id7;    break;
                case 8: f_min = ch_stat_sop.min_sop_id8;    f_max = ch_stat_sop.max_sop_id8;    break;
                case 9: f_min = ch_stat_sop.min_sop_id9;    f_max = ch_stat_sop.max_sop_id9;    break;
                case 10: f_min = ch_stat_sop.min_sop_id10;  f_max = ch_stat_sop.max_sop_id10;   break;
            }

            float f_dvi_val = ((f_max - f_min) / 10) * 10;/*랜덤 최대 레벨 10*/
            returnVal[0] = (f_min * 100) / 100;
            returnVal[1] = ((f_dvi_val + f_min) * 100) / 100;
            return returnVal;
        }
#endregion
        //END endregion --------------------------------------

        // ###################################################
        // ---------------------- START ----------------------
#region ##### 장비(악세) 스탯 매인, 옵션 #####
        public float GetEqNormalLvStatIncr(int st_id) => GetInstance().chartDB.GetDicBalance(string.Format("equip.normal.lv.Incr.value.stat.id{0}", st_id)).val_float;
        public float GetEqEnhantLvStatIncr(int st_id) => GetInstance().chartDB.GetDicBalance(string.Format("equip.enhant.lv.Incr.value.stat.id{0}", st_id)).val_float;

        // 스탯 
        // 0무기    : 공격력 
        // 1방패    : 방어력 
        // 2헬멧%   : 피해량 감소
        // 3어깨%   : 체력
        // 4갑옷    : 체력 
        // 5팔      : 치명타 공격력
        // 6바지    : 공격력
        // 7부츠%   : 공격 속도

        // 8목걸이% : 치명타 발동률 
        // 9귀고리% : 방어력
        // 10반지%   : 공격력

        // 장신구 전용 옵션
        // 1:pve피해 증가(공격력의 ?%)
        // 2:pvp피해 증가(공격력의 ?%) 
        // 3:pve피해 감소(%) 
        // 4:pvp피해 감소(%) 
        // 5:골드 획득 증가 
        // 6:장비 획득 증가
        // 7:보스 몬스터 피해 증가
        // 8:공격시 현재 체력의 ?%를 회복 
        // 9:공격시 랜덤으로 버블 추가획득
        // 10:공격시 랜덤으로 상대 버블 차감

        /// <summary>
        /// 장비 매인 스텟 값 : [0]기본 스탯값 + 레벨,강화 스탯 총합, [1]: 레벨업 + 강화 레벨에 따른 스탯 값 
        /// </summary>
        public object[] GetMainStatValue(TableDB.Equipment equip_data, int chng_pt_ty = -1) // returnVal[0] : 최종 값, returnVal[1] : 강화로 추가된 값 
        {
            if(chng_pt_ty != -1)
                equip_data.eq_ty = chng_pt_ty;

            int get_eq_ty    = equip_data.eq_ty;
            int get_eq_rt    = equip_data.eq_rt;
            int get_eq_id    = equip_data.eq_id;
            int get_mast_id  = equip_data.ma_st_id;
            int get_mast_lv  = equip_data.ma_st_rlv <= 0 || equip_data.ma_st_rlv > 10 ? 1 : equip_data.ma_st_rlv; // get_stat_rnd_lv 최소 1, 최대 10 
            int get_nor_lv   = equip_data.m_norm_lv;
            int get_eht_lv   = equip_data.m_ehnt_lv;

            //float incr = GetInstance().chartDB.GetDicBalance(string.Format("equip.main.stat.Incr.value.eqty{0}", get_eq_ty)).val_float; // 장비ty -> 매인 스탯 증가 값 (기본,렙업,강화)
            switch (get_eq_ty)
            {
                case 0: // 무기 : 공격력 
                    return new object[]
                    {
                        //(long)(GetEquipWeaponStat(get_eq_rt, get_eq_id, get_mast_lv, 0, get_nor_lv, get_eht_lv) * incr),
                        //(long)(GetEquipWeaponStat(get_eq_rt, get_eq_id, 0, 0, get_nor_lv, get_eht_lv) * incr)
                        (long)GetEquipWeaponStat(get_eq_rt, get_eq_id, get_mast_lv, 0, get_nor_lv, get_eht_lv),
                        (long)GetEquipWeaponStat(get_eq_rt, get_eq_id, 0, 0, get_nor_lv, get_eht_lv)
                    };

                case 1: // 방패 : 방어력 
                    return new object[]
                    {
                        (long)GetEquipShieldStat(get_eq_rt, get_eq_id, get_mast_lv, 0, get_nor_lv, get_eht_lv),
                        (long)GetEquipShieldStat(get_eq_rt, get_eq_id, 0, 0, get_nor_lv, get_eht_lv)
                    };
                case 2: // 헬멧 : 피해량 감소%
                    return new object[]
                    {
                        (float)GetEquipHelmetStat(get_eq_rt, get_eq_id, get_mast_lv, 0, get_nor_lv, get_eht_lv),
                        (float)GetEquipHelmetStat(get_eq_rt, get_eq_id, 0, 0, get_nor_lv, get_eht_lv)
                    };
                case 3: // 어깨 : 체력%
                    return new object[]
                    {
                        (float)GetEquipShoulderStat(get_eq_rt, get_eq_id, get_mast_lv, 0, get_nor_lv, get_eht_lv),
                        (float)GetEquipShoulderStat(get_eq_rt, get_eq_id, 0, 0, get_nor_lv, get_eht_lv)
                    };
                case 4: // 갑옷 : 체력
                    return new object[]
                    {
                        (long)GetEquipArmorStat(get_eq_rt, get_eq_id, get_mast_lv, 0, get_nor_lv, get_eht_lv),
                        (long)GetEquipArmorStat(get_eq_rt, get_eq_id, 0, 0, get_nor_lv, get_eht_lv)
                    };
                case 5: // 팔 : 치명타 공격력 
                    return new object[]
                    {
                        (long)GetEquipGauntlets(get_eq_rt, get_eq_id, get_mast_lv, 0, get_nor_lv, get_eht_lv),
                        (long)GetEquipGauntlets(get_eq_rt, get_eq_id, 0, 0, get_nor_lv, get_eht_lv)
                    };
                case 6: // 바지 : 공격력 
                    return new object[]
                    {
                        (long)GetEquipPantsStat(get_eq_rt, get_eq_id, get_mast_lv, 0, get_nor_lv, get_eht_lv),
                        (long)GetEquipPantsStat(get_eq_rt, get_eq_id, 0, 0, get_nor_lv, get_eht_lv)
                    };
                case 7: // 부츠 : 공격 속도 %
                    return new object[]
                    {
                        (float)GetEquipBootsStat(get_eq_rt, get_eq_id, get_mast_lv, 0, get_nor_lv, get_eht_lv),
                        (float)GetEquipBootsStat(get_eq_rt, get_eq_id, 0, 0, get_nor_lv, get_eht_lv)
                    };
                case 8: // 목걸이 : 치명타 발동률%
                    return new object[]
                    {
                        (float)GetEquipNecklaceStat(get_eq_rt, get_eq_id, get_mast_lv, 0, get_nor_lv, get_eht_lv),
                        (float)GetEquipNecklaceStat(get_eq_rt, get_eq_id, 0, 0, get_nor_lv, get_eht_lv)
                    };
                case 9: // 귀고리 : 방어력%
                    return new object[]
                    {
                        (float)GetEquipEarringStat(get_eq_rt, get_eq_id, get_mast_lv, 0, get_nor_lv, get_eht_lv),
                        (float)GetEquipEarringStat(get_eq_rt, get_eq_id, 0, 0, get_nor_lv, get_eht_lv)
                    };
                case 10: // 반지 : 공격력%
                    return new object[]
                    {
                        (float)GetEquipRingStat(get_eq_rt, get_eq_id, get_mast_lv, 0, get_nor_lv, get_eht_lv),
                        (float)GetEquipRingStat(get_eq_rt, get_eq_id, 0, 0, get_nor_lv, get_eht_lv)
                    };
            }

            return new object[] { (long)0, (long)0 };
        }

        /// <summary> 무기, 공격력 </summary>
        public long GetEquipWeaponStat(int rt, int id, int mast_lv, int opst_lv, int nr_lv, int eh_lv, bool isEncy = false)
        {
            int eq_ty = 0;
            long value = (long)GetEquipEnchantLevelStatValue(eh_lv, eq_ty, rt);
            value += (long)GetAbilityLevelStatsValue(nr_lv, eq_ty, rt);
            //value += (long)GetEquipStatValue(opst_lv, eq_ty, rt);
            //value += (long)GetEquipStatValue(mast_lv, eq_ty, rt);

            return value;
        }

        /// <summary> 방패, 방어력 </summary>
        public long GetEquipShieldStat(int rt, int id, int mast_lv, int opst_lv, int nr_lv, int eh_lv, bool isEncy = false)
        {
            int eq_ty = 1;
            long value = (long)GetEquipEnchantLevelStatValue(eh_lv, eq_ty, rt);
            value += (long)GetAbilityLevelStatsValue(nr_lv, eq_ty, rt);
            //value += (long)GetEquipStatValue(opst_lv, eq_ty, rt);
            //value += (long)GetEquipStatValue(mast_lv, eq_ty, rt);

            return value;
        }

        // XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
        // 변경 스탯 : 명중력 -> 피해량 감소 
        // XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
        /// <summary> 헬멧, 피해량 감소% </summary>
        public float GetEquipHelmetStat(int rt, int id, int mast_lv, int opst_lv, int nr_lv, int eh_lv, bool isEncy = false)
        {
            int eq_ty = 2;
            float value = (float)GetEquipEnchantLevelStatValue(eh_lv, eq_ty, rt);
            value += (float)GetAbilityLevelStatsValue(nr_lv, eq_ty, rt);
            //value += (float)GetEquipStatValue(opst_lv, eq_ty, rt);
            //value += (float)GetEquipStatValue(mast_lv, eq_ty, rt);

            return value;
        }

        /// <summary> 어깨, 체력% </summary>
        public float GetEquipShoulderStat(int rt, int id, int mast_lv, int opst_lv, int nr_lv, int eh_lv, bool isEncy = false)
        {
            int eq_ty = 3;
            float value = (float)GetEquipEnchantLevelStatValue(eh_lv, eq_ty, rt);
            value += (float)GetAbilityLevelStatsValue(nr_lv, eq_ty, rt);
            //value += (float)GetEquipStatValue(opst_lv, eq_ty, rt);
            //value += (float)GetEquipStatValue(mast_lv, eq_ty, rt);

            return value;
        }

        /// <summary> 갑옷, 체력 </summary>
        public long GetEquipArmorStat(int rt, int id, int mast_lv, int opst_lv, int nr_lv, int eh_lv, bool isEncy = false)
        {
            int eq_ty = 4;
            long value = (long)GetEquipEnchantLevelStatValue(eh_lv, eq_ty, rt);
            value += (long)GetAbilityLevelStatsValue(nr_lv, eq_ty, rt);
            //value += (long)GetEquipStatValue(opst_lv, eq_ty, rt);
            //value += (long)GetEquipStatValue(mast_lv, eq_ty, rt);

            return value;
        }

        /// <summary> 팔, 치명타 공격력 </summary>
        public long GetEquipGauntlets(int rt, int id, int mast_lv, int opst_lv, int nr_lv, int eh_lv, bool isEncy = false)
        {
            int eq_ty = 5;
            long value = (long)GetEquipEnchantLevelStatValue(eh_lv, eq_ty, rt);
            value += (long)GetAbilityLevelStatsValue(nr_lv, eq_ty, rt);
            //value += (long)GetEquipStatValue(opst_lv, eq_ty, rt);
            //value += (long)GetEquipStatValue(mast_lv, eq_ty, rt);

            return value;
        }

        // XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
        // 변경 스탯 : 회피율 -> 공격력 
        // XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
        /// <summary> 바지, 공격력 </summary>
        public long GetEquipPantsStat(int rt, int id, int mast_lv, int opst_lv, int nr_lv, int eh_lv, bool isEncy = false)
        {
            int eq_ty = 6;
            long value = (long)GetEquipEnchantLevelStatValue(eh_lv, eq_ty, rt);
            value += (long)GetAbilityLevelStatsValue(nr_lv, eq_ty, rt);
            //value += (long)GetEquipStatValue(opst_lv, eq_ty, rt);
            //value += (long)GetEquipStatValue(mast_lv, eq_ty, rt);

            return value;
        }

        // XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
        // 변경 스탯 : 치명타 방어력 -> 공격 속도 
        // XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
        /// <summary> 부츠,공격 속도% </summary>
        public float GetEquipBootsStat(int rt, int id, int mast_lv, int opst_lv, int nr_lv, int eh_lv, bool isEncy = false)
        {
            int eq_ty = 7;
            float value = (float)GetEquipEnchantLevelStatValue(eh_lv, eq_ty, rt);
            value += (float)GetAbilityLevelStatsValue(nr_lv, eq_ty, rt);
            //value += (float)GetEquipStatValue(opst_lv, eq_ty, rt);
            //value += (float)GetEquipStatValue(mast_lv, eq_ty, rt);

            return value;
        }

        /// <summary> 목걸이, 치명타 발동률% </summary>
        public float GetEquipNecklaceStat(int rt, int id, int mast_lv, int opst_lv, int nr_lv, int eh_lv, bool isEncy = false)
        {
            int eq_ty = 8;
            float value = (float)GetEquipEnchantLevelStatValue(eh_lv, eq_ty, rt);
            value += (float)GetAbilityLevelStatsValue(nr_lv, eq_ty, rt);
            //value += (float)GetEquipStatValue(opst_lv, eq_ty, rt);
            //value += (float)GetEquipStatValue(mast_lv, eq_ty, rt);

            return value;
        }

        // XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
        // 변경 스탯 : 치명타 회피율 -> 방어력 % 
        // XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
        /// <summary> 귀고리, 방어력% </summary>
        public float GetEquipEarringStat(int rt, int id, int mast_lv, int opst_lv, int nr_lv, int eh_lv, bool isEncy = false)
        {
            int eq_ty = 9;
            float value = (float)GetEquipEnchantLevelStatValue(eh_lv, eq_ty, rt);
            value += (float)GetAbilityLevelStatsValue(nr_lv, eq_ty, rt);
            //value += (float)GetEquipStatValue(opst_lv, eq_ty, rt);
            //value += (float)GetEquipStatValue(mast_lv, eq_ty, rt);

            return value;
        }

        /// <summary> 반지, 공격력% </summary>
        public float GetEquipRingStat(int rt, int id, int mast_lv, int opst_lv, int nr_lv, int eh_lv, bool isEncy = false)
        {
            int eq_ty = 10;
            float value = (float)GetEquipEnchantLevelStatValue(eh_lv, eq_ty, rt);
            value += (float)GetAbilityLevelStatsValue(nr_lv, eq_ty, rt);
            //value += (float)GetEquipStatValue(opst_lv, eq_ty, rt);
            //value += (float)GetEquipStatValue(mast_lv, eq_ty, rt);

            return value;
        }

        /// <summary>
        /// 도감 스탯 값 : 도감 강화 레벨에 따른 스탯 전체 스탯값 리턴 
        /// </summary>
        public CharacterDB.StatValue GetEncycloStatValue()
        {
            int max_enhant_lv = GameDatabase.GetInstance().equipmentEncyclopediaDB.GetMaxEnhantLevel();
            CharacterDB.StatValue stat = new CharacterDB.StatValue();
            var getEncycloDB = GetInstance().equipmentEncyclopediaDB.GetAll();
            foreach (var ecyKey in getEncycloDB.Keys)
            {
                var encyDBs = getEncycloDB[ecyKey];
                for (int i = 0; i < encyDBs.Count; i++)
                {
                    var encyDB = encyDBs[i];
                    if (encyDB.eh_lv > 0)
                    {
                        if (encyDB.eh_lv > max_enhant_lv)
                            encyDB.eh_lv = max_enhant_lv;

                        int opst_id = GetInstance().chartDB.GetDicBalance(string.Format("ency.option.stat.id.equip.ty.{0}", encyDB.ty)).val_int;
                        //float IncrVal = GetInstance().chartDB.GetDicBalance(string.Format("equip.ency.main.stat.Incr.value.eqty{0}", encyDB.ty)).val_float; // 장비ty ->도감 장비 매인 스탯 증가 값
                        switch (opst_id)
                        {
                            // 공격력 (stat id : 1) 
                            case 1: //공격력
                                stat.p0_wea_attackPower += (long)GetEquipWeaponStat(encyDB.rt, encyDB.id, 0, 0, 0, encyDB.eh_lv, true); 
                                break;
                            case 2: //방어력
                                stat.p1_shi_defance += (long)GetEquipShieldStat(encyDB.rt, encyDB.id, 0, 0, 0, encyDB.eh_lv, true); 
                                break;
                            case 4: //치명타 공격력
                                stat.p5_gau_criticalPower += (long)GetEquipShoulderStat(encyDB.rt, encyDB.id, 0, 0, 0, encyDB.eh_lv, true);
                                break;
                            case 5: //체력
                                stat.p4_arm_health += (long)GetEquipArmorStat(encyDB.rt, encyDB.id, 0, 0, 0, encyDB.eh_lv, true);
                                break;
                            //case 8: //치명타 방어력
                            //    stat.stat8_valCriDefense += (long)GetEquipBootsStat(encyDB.rt, encyDB.id, 0, 0, 0, encyDB.eh_lv, true);
                            //    break;
                        }
                    }
                }
            }

            return stat;
        }

        /// <summary>
        /// 장비별 옵션 스탯 값 
        /// </summary>
        public long GetEquipOptionStatValue(int opst_id, int opst_rlv, int eq_rt, int eq_id, int eq_eht_lv, bool isZb, int eq_legend = 0, int eq_lgnd_sop_id = 0, int eq_lgnd_sop_rlv = 0)
        {
            eq_eht_lv++;
            float incr_eht_calc = ((eq_eht_lv * eq_eht_lv) * 0.005f) + 1;
            float incr = GetInstance().chartDB.GetDicBalance(string.Format("equip.option.Incr.value.stat.id{0}", opst_id)).val_float; // 옵션id -> 옵션 스탯 증가 값 
            if (isZb)
            {
                float incr_eq_legend = 1.0f, incr_lgnd_bns = 0.0f;
                if (eq_legend == 1 && int.Equals(opst_id, eq_lgnd_sop_id))
                {
                    incr_eq_legend += eq_legend;
                    float lgnd_sop_val = GameDatabase.GetInstance().chartDB.GetEquipSpecialOptionValue(eq_lgnd_sop_id, eq_lgnd_sop_rlv);
                    if (lgnd_sop_val > 0.0f)
                        incr_lgnd_bns += lgnd_sop_val * 0.01f;
                }

                switch (opst_id)
                {
                    case 1:  return (long)((((GetEquipWeaponStat(eq_rt, eq_id, 0, opst_rlv, 0, 0) * incr) * incr_eht_calc) * incr_eq_legend) * (incr_lgnd_bns > 0.0f ? incr_lgnd_bns : 1)); /*공격력(stat id: 1)*/
                    case 2:  return (long)((((GetEquipShieldStat(eq_rt, eq_id, 0, opst_rlv, 0, 0) * incr) * incr_eht_calc) * incr_eq_legend) * (incr_lgnd_bns > 0.0f ? incr_lgnd_bns : 1)); /*방어력(stat id: 2) */
                    case 4:  return (long)((((GetEquipShoulderStat(eq_rt, eq_id, 0, opst_rlv, 0, 0) * incr) * incr_eht_calc) * incr_eq_legend) * (incr_lgnd_bns > 0.0f ? incr_lgnd_bns : 1)); /*치명타 공격력(stat id : 4*/
                    case 5:  return (long)((((GetEquipArmorStat(eq_rt, eq_id, 0, opst_rlv, 0, 0) * incr) * incr_eht_calc) * incr_eq_legend) * (incr_lgnd_bns > 0.0f ? incr_lgnd_bns : 1)); /*체력(stat id: 5)*/
                    case 8:  return (long)((((GetEquipBootsStat(eq_rt, eq_id, 0, opst_rlv, 0, 0) * incr) * incr_eht_calc) * incr_eq_legend) * (incr_lgnd_bns > 0.0f ? incr_lgnd_bns : 1)); /*치명타 방어력(stat id : 8)*/
                    default: return (long)0;
                }
            }
            else
            {
                switch (opst_id)
                {
                    case 1:  return (long)(GetEquipWeaponStat(eq_rt, eq_id, 0, opst_rlv, 0, 0) * incr); /*공격력(stat id: 1)*/
                    case 2:  return (long)(GetEquipShieldStat(eq_rt, eq_id, 0, opst_rlv, 0, 0) * incr); /*방어력(stat id: 2) */
                    case 4:  return (long)(GetEquipShoulderStat(eq_rt, eq_id, 0, opst_rlv, 0, 0) * incr); /*치명타 공격력(stat id : 4*/
                    case 5:  return (long)(GetEquipArmorStat(eq_rt, eq_id, 0, opst_rlv, 0, 0) * incr); /*체력(stat id: 5)*/
                    case 8:  return (long)(GetEquipBootsStat(eq_rt, eq_id, 0, opst_rlv, 0, 0) * incr); /*치명타 방어력(stat id : 8)*/
                    default: return (long)0;
                }
            }
        }
        #endregion
        //END endregion ----------------------

        // ###################################################
        // ---------------------- START ----------------------
        #region ##### 펫 전용 옵션, 옵션 #####
        /// <summary>
        /// 각 옵션 별 펫 레벨,옵션 id,rlv, lv로 1개 값 리턴 
        /// </summary>
        public float GetCdbPetOptionValue(int p_rt, int p_id, int opid, int oplv, int p_lv)
        {
            cdb_pet cdbPet = GetCdbPet(p_rt, p_id);
            switch (opid)
            {
                case 1: return (float)((((cdbPet.op1v_max - cdbPet.op1v_min) * 0.1) * oplv) + cdbPet.op1v_min) + (p_lv + (p_rt * p_rt * p_rt * 0.01f)); // 공 
                case 2: return (float)((((cdbPet.op2v_max - cdbPet.op2v_min) * 0.1) * oplv) + cdbPet.op2v_min) + (p_lv + (p_rt * p_rt * p_rt * 0.01f)); // 방
                case 4: return (float)((((cdbPet.op4v_max - cdbPet.op4v_min) * 0.1) * oplv) + cdbPet.op4v_min) + (p_lv + (p_rt * p_rt * p_rt * 0.01f)); // 치.공
                case 5: return (float)((((cdbPet.op5v_max - cdbPet.op5v_min) * 0.1) * oplv) + cdbPet.op5v_min) + (p_lv + (p_rt * p_rt * p_rt * 0.01f)); // 체
                case 8: return (float)((((cdbPet.op8v_max - cdbPet.op8v_min) * 0.1) * oplv) + cdbPet.op8v_min) + (p_lv + (p_rt * p_rt * p_rt * 0.01f)); // 치.방 
                default: return 1.0f;                                                                             
            }
        }
       
        /// <summary>
        /// 펫 옵션 
        /// </summary>
        public float[] GetPetOptionStatValue(TableDB.Pet _petDB)
        {
            float[] val = // [8]
            {
                GetCdbPetOptionValue(_petDB.p_rt, _petDB.p_id, _petDB.statOp.op1.id, _petDB.statOp.op1.rlv, _petDB.p_lv),
                GetCdbPetOptionValue(_petDB.p_rt, _petDB.p_id, _petDB.statOp.op2.id, _petDB.statOp.op2.rlv, _petDB.p_lv),
                GetCdbPetOptionValue(_petDB.p_rt, _petDB.p_id, _petDB.statOp.op3.id, _petDB.statOp.op3.rlv, _petDB.p_lv),
                GetCdbPetOptionValue(_petDB.p_rt, _petDB.p_id, _petDB.statOp.op4.id, _petDB.statOp.op4.rlv, _petDB.p_lv),
                GetCdbPetOptionValue(_petDB.p_rt, _petDB.p_id, _petDB.statOp.op5.id, _petDB.statOp.op5.rlv, _petDB.p_lv),
                GetCdbPetOptionValue(_petDB.p_rt, _petDB.p_id, _petDB.statOp.op6.id, _petDB.statOp.op6.rlv, _petDB.p_lv),
                GetCdbPetOptionValue(_petDB.p_rt, _petDB.p_id, _petDB.statOp.op7.id, _petDB.statOp.op7.rlv, _petDB.p_lv),
                GetCdbPetOptionValue(_petDB.p_rt, _petDB.p_id, _petDB.statOp.op8.id, _petDB.statOp.op8.rlv, _petDB.p_lv),
            };

            return val;
        }

        /// <summary>
        /// 펫 전용 옵션 
        /// </summary>
        public float GetPetSpecialOptionStatValue(TableDB.Pet _petDB, int sop_num)
        {
            TableDB.StatOp Sop = sop_num == 1 ? _petDB.sOp1 : sop_num == 2 ? _petDB.sOp2 : sop_num == 3 ? _petDB.sOp3 : default;
            if (Sop.id > 0)
            {
                float f_min = 0f, f_max = 0f;
                var cdb = GetCdbPetSop(_petDB.p_rt, _petDB.p_id);
                switch (Sop.id)
                {
                    case 1: f_min = cdb.sop1v_min; f_max = cdb.sop1v_max; break;
                    case 2: f_min = cdb.sop2v_min; f_max = cdb.sop2v_max; break;
                    case 3: f_min = cdb.sop3v_min; f_max = cdb.sop3v_max; break;
                    case 4: f_min = cdb.sop4v_min; f_max = cdb.sop4v_max; break;
                    case 5: f_min = cdb.sop5v_min; f_max = cdb.sop5v_max; break;
                    case 6: f_min = cdb.sop6v_min; f_max = cdb.sop6v_max; break;
                    case 7: f_min = cdb.sop7v_min; f_max = cdb.sop7v_max; break;
                    case 8: f_min = cdb.sop8v_min; f_max = cdb.sop8v_max; break;
                    case 9: f_min = cdb.sop9v_min; f_max = cdb.sop9v_max; break;
                }

                return (((f_max - f_min) / 10) * Sop.rlv) + f_min;
            }
            else
            {
                return 1.0f;
            }
        }
        #endregion
        //END endregion ----------------------

        // ###################################################
        // ---------------------- START ----------------------
        #region ##### 스킬 #####
        public List<cdb_stat_skill> GetChartSkill_DataAll() => list_cdb_stat_Skill;
        public cdb_stat_skill GetChartSkill_Data(int sk_idx)
        {
            int index = list_cdb_stat_Skill.FindIndex((cdb_stat_skill sk) => sk.s_idx == sk_idx);
            if (index >= 0)
                return list_cdb_stat_Skill[index];

            return default;
        }

        /// <summary> 스킬 맥스 레벨 </summary>
        public int GetChartSkill_MaxLevel() => GetDicBalance("skill.max.level").val_int;
        /// <summary> 현재의 스킬 레벨업에 필요한 수량 </summary>
        public int GetChartSkill_UpNeedCount(int nwLv) => GetDicBalance(string.Format("skill.up.need.count.level{0}", nwLv)).val_int;

        /// <summary>
        /// 레벨업에 필요한 골드 
        /// </summary>
        public long GetChartSkillLeveUpGoldPrice(int nwLv) => GetDicBalance(string.Format("skill.up.gold.level{0}", nwLv)).val_long;

        /// <summary> 스킬 능력치 정보 </summary>
        public string GetInfoSkillDescription(int id, int lv)
        {
            var cdb = GetInstance().chartDB.GetChartSkill_Data(id);
            string sfmt = LanguageGameData.GetInstance().GetString(string.Format("skill.description_{0}", id));
            switch (id)
            {
                case 1:
                case 2:
                case 3:
                case 5:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                case 22:
                case 24:
                case 25:
                case 26:
                    //(float)Math.Round(cdb.f_mtp_val1 * Mathf.Pow(cdb.f_mtp_pow1, lv), 2)
                    return string.Format(sfmt, string.Format("{0:0.##}", Math.Round(cdb.f_mtp_val1 * Mathf.Pow(cdb.f_mtp_pow1, lv), 2) * 100));
               
                case 4: 
                case 6: 
                case 14:                
                case 15:         
                case 16:          
                case 17:           
                case 18:            
                case 19:
                case 21:
                    return string.Format(sfmt,
                        string.Format("{0:0.##}", Math.Round(cdb.f_mtp_val1 * Mathf.Pow(cdb.f_mtp_pow1, lv), 2) * 100),
                        string.Format("{0:0.##}", Math.Round(cdb.f_mtp_val2 * Mathf.Pow(cdb.f_mtp_pow2, lv), 2) * 100));
               
                case 12:
                case 13:
                case 20:
                case 23:
                    return string.Format(sfmt,
                     string.Format("{0:0.##}", Math.Round(cdb.f_mtp_val1 * Mathf.Pow(cdb.f_mtp_pow1, lv), 2) * 100),
                     string.Format("{0:0.##}", Math.Round(cdb.f_mtp_val2 * Mathf.Pow(cdb.f_mtp_pow2, lv), 2) * 100),
                     string.Format("{0:0.##}", Math.Round(cdb.f_mtp_val3 * Mathf.Pow(cdb.f_mtp_pow3, lv), 2) * 100));
            }

            return sfmt;
        }

        /// <summary> 스킬 발동시 타격 대미지 </summary> 
        public long GetValueSkillAttackPower(int id, int lv, long _df_dmg)
        {
            float mtp_pls = GetValue_SkillAttackPowerMultiply(id, lv);
            if (mtp_pls > 0)
                return (long)(_df_dmg * mtp_pls);
            else 
                return _df_dmg;
        }

        /// <summary> 스킬 발동시 타격 포함 스킬의 공격 퍼센트 값 </summary>
        private float GetValue_SkillAttackPowerMultiply(int id, int lv)
        {
            var cdb = GetInstance().chartDB.GetChartSkill_Data(id);
            switch (id)
            {
                case 1:  return (float)Math.Round(cdb.f_mtp_val1 * Mathf.Pow(cdb.f_mtp_pow1, lv), 2);
                case 4:  return (float)Math.Round(cdb.f_mtp_val1 * Mathf.Pow(cdb.f_mtp_pow1, lv), 2);
                case 6:  return (float)Math.Round(cdb.f_mtp_val1 * Mathf.Pow(cdb.f_mtp_pow1, lv), 2);
                case 7:  return (float)Math.Round(cdb.f_mtp_val1 * Mathf.Pow(cdb.f_mtp_pow1, lv), 2);
                case 12: return UnityEngine.Random.Range((float)Math.Round(cdb.f_mtp_val1 * Mathf.Pow(cdb.f_mtp_pow1, lv), 2), (float)Math.Round(cdb.f_mtp_val2 * Mathf.Pow(cdb.f_mtp_pow2, lv), 2));
                case 13: return UnityEngine.Random.Range((float)Math.Round(cdb.f_mtp_val1 * Mathf.Pow(cdb.f_mtp_pow1, lv), 2), (float)Math.Round(cdb.f_mtp_val2 * Mathf.Pow(cdb.f_mtp_pow2, lv), 2));
                case 14: return (float)Math.Round(cdb.f_mtp_val1 * Mathf.Pow(cdb.f_mtp_pow1, lv), 2);
                case 15: return (float)Math.Round(cdb.f_mtp_val1 * Mathf.Pow(cdb.f_mtp_pow1, lv), 2);
                case 16: return (float)Math.Round(cdb.f_mtp_val1 * Mathf.Pow(cdb.f_mtp_pow1, lv), 2);
                case 17: return (float)Math.Round(cdb.f_mtp_val1 * Mathf.Pow(cdb.f_mtp_pow1, lv), 2);
                case 18: return (float)Math.Round(cdb.f_mtp_val1 * Mathf.Pow(cdb.f_mtp_pow1, lv), 2);
                case 19: return (float)Math.Round(cdb.f_mtp_val1 * Mathf.Pow(cdb.f_mtp_pow1, lv), 2);
                case 20: return UnityEngine.Random.Range((float)Math.Round(cdb.f_mtp_val1 * Mathf.Pow(cdb.f_mtp_pow1, lv), 2), (float)Math.Round(cdb.f_mtp_val2 * Mathf.Pow(cdb.f_mtp_pow2, lv), 2));
                case 21: return UnityEngine.Random.Range((float)Math.Round(cdb.f_mtp_val1 * Mathf.Pow(cdb.f_mtp_pow1, lv), 2), (float)Math.Round(cdb.f_mtp_val2 * Mathf.Pow(cdb.f_mtp_pow2, lv), 2));
                case 24: return (float)Math.Round(cdb.f_mtp_val1 * Mathf.Pow(cdb.f_mtp_pow1, lv), 2);
                case 25: return (float)Math.Round(cdb.f_mtp_val1 * Mathf.Pow(cdb.f_mtp_pow1, lv), 2);
            }

            return 0;
        }

        /// <summary>스킬 2번 : 상대에게 폭탄의 폭발 대미지 </summary>
        public long GetSkillAbility2_BombDamage(int lv, long _df_dmg)
        {
            var cdb = GetInstance().chartDB.GetChartSkill_Data(2);
            float mtp = (float)Math.Round(cdb.f_mtp_val1 * Mathf.Pow(cdb.f_mtp_pow1, lv), 2);
            return (long)(_df_dmg * mtp);
        }

        /// <summary> 스킬 3번 : 보호막 적용된 대미지 </summary>
        public long GetSkillAbility3_ShieldDamage(int lv, long _rlt_dmg)
        {
            var cdb = GetInstance().chartDB.GetChartSkill_Data(3);
            float mtp = cdb.f_mtp_val1 + (lv * 0.03f);
            long rlt_dmg = _rlt_dmg - (long)(_rlt_dmg * mtp);
            if (rlt_dmg <= 0)
                rlt_dmg = 0;

            return rlt_dmg;
        }

        /// <summary> 스킬 4번 : 기절 발동 확률 </summary>
        public bool GetSkillAbility4_Stunned(int lv)
        {
            var cdb = GetInstance().chartDB.GetChartSkill_Data(4);
            return UnityEngine.Random.Range(0f, 100f) < (100f * cdb.f_mtp_val2 + (lv * 0.03f));
        }
        /// <summary> 스킬 5번 : 반사 대미지 </summary>
        public long GetSkillAbility5_ReflectDamage (int lv, long _df_dmg)
        {
            var cdb = GetInstance().chartDB.GetChartSkill_Data(5);
            float mtp = cdb.f_mtp_val1 + (lv * 0.03f);
            return (long)(_df_dmg * mtp);
        }

        /// <summary> 스킬 6번 : 출혈 도트 대미지 (기본 공격력의) </summary>
        public int GetSkillAbility6_DotDamage (int lv, int _df_dmg)
        {
            var cdb = GetInstance().chartDB.GetChartSkill_Data(6);
            float mtp = cdb.f_mtp_val1 + (lv * 0.03f);
            return Mathf.RoundToInt(_df_dmg * mtp);
        }

        /// <summary> 스킬 8번 : 자신의 남은 체력 기준 체력 회복 량 </summary>
        public long GetSkillAbility8_RecoveryHealth (int lv, long now_hp)
        {
            var cdb = GetInstance().chartDB.GetChartSkill_Data(8);
            float mtp = cdb.f_mtp_val1 + (lv * 0.03f);
            return (long)(now_hp * mtp);
        }

        /// <summary> 스킬 9번 : 시전자의 공격력이 증가 값 </summary>
        public float  GetSkillAbility9_AttackPower(int lv)
        {
            var cdb = GetInstance().chartDB.GetChartSkill_Data(9);
            float mtp = cdb.f_mtp_val1 + (lv * 0.03f);
            return mtp;
            //return Mathf.RoundToInt(_df_dmg * mtp);
        }

        /// <summary> 스킬 10번 : 시전자의 명중률이 증가 </summary>
        public float GetSkillAbility10_Accuracy(int lv, float _df_Acur)
        {
            var cdb = GetInstance().chartDB.GetChartSkill_Data(10);
            float mtp = cdb.f_mtp_val1 + (lv * 0.03f);
            //return Mathf.Round(_df_Acur * mtp);
            return (float)(Math.Truncate((_df_Acur * mtp) * 1000.0f) / 1000.0f);
        }

        /// <summary> 스킬 11번 : 시전자의 방어력 증가 </summary>
        public float GetSkillAbility11_UpDefense(int lv)
        {
            var cdb = GetInstance().chartDB.GetChartSkill_Data(11);
            float mtp = cdb.f_mtp_val3 + (float)(lv * 0.03f);
            return mtp;
        }

        /// <summary> 스킬 12번 : 상대가 기절 상태일 경우 기본 고정 공격력 x 증가 대미지  </summary>
        public long GetSkillAbility12_StunnedBonusDamage(int lv, long _atk_power)
        {
            var cdb = GetInstance().chartDB.GetChartSkill_Data(12);
            float mtp = cdb.f_mtp_val3 + (lv * 0.03f);
            return (long)(_atk_power * mtp);
        }

        /// <summary> 스킬 13번 : 타격한 대미지의 일정량을 체력으로 회복 </summary>
        public long GetSkillAbility13_RecoveryHealth(int lv, long _hit_dmg)
        {
            var cdb = GetInstance().chartDB.GetChartSkill_Data(13);
            float mtp = cdb.f_mtp_val3 + (lv * 0.03f);
            return (long)(_hit_dmg * mtp);
        }

        /// <summary> 스킬 14번 : 시전자가 상대방의 방어력 감소 -> 상대의 방어력을 가져와서 계산 후 리턴 </summary>
        public float GetSkillAbility14_DownDefense(int lv) // , int _df_defnse)
        {
            var cdb = GetInstance().chartDB.GetChartSkill_Data(14);
            float mtp = cdb.f_mtp_val2 + (lv * 0.03f);
            return mtp;

            //LogPrint.Print("###### 스킬 14번 mtp : " + mtp + ", _df_defnse : " + _df_defnse + ", rlt_defnse : " + Mathf.RoundToInt(_df_defnse - (_df_defnse * mtp)));
            //return Mathf.RoundToInt(_df_defnse - (_df_defnse * mtp));
        }

        /// <summary> 스킬 15번 : 시전자가 상대방의 공격력 감소 -> 상대의 공격 대미지를 가져와서 계산 후 리턴 </summary>
        public float GetSkillAbility15_DownDefense(int lv)
        {
            var cdb = GetInstance().chartDB.GetChartSkill_Data(15);
            float mtp = cdb.f_mtp_val2 + (lv * 0.03f);
            return mtp;
            //return Mathf.RoundToInt(_df_dmg - (_df_dmg * mtp));
        }

        /// <summary> 스킬 16번 : 시전자가 상대방의 명중률 감소 -> 상대의 명중률을 가져와서 계산 후 리턴 </summary>
        public float GetSkillAbility16_DownDefense(int lv, float _df_acur)
        {
            var cdb = GetInstance().chartDB.GetChartSkill_Data(16);
            float mtp = cdb.f_mtp_val2 + (lv * 0.03f);

            LogPrint.Print("###### 스킬 16번 mtp : " + mtp +", _df_acur : " + _df_acur + ", _rlt_acur : " + (_df_acur - (float)(Math.Truncate((_df_acur * mtp) * 1000.0f) / 1000.0f)));
            return _df_acur - (float)(Math.Truncate((_df_acur * mtp) * 1000.0f) / 1000.0f);
        }

        /// <summary> 스킬 17번 : 시전자의 다음 1회 공격 동안 크리성공100%, 크리 대미지 증가 </summary>
        public float GetSkillAbility17_UpCriticalAttackPower(int lv)
        {
            var cdb = GetInstance().chartDB.GetChartSkill_Data(17);
            float mtp = cdb.f_mtp_val2 + (lv * 0.03f);
            return mtp;
            //return Mathf.RoundToInt(_df_cri_dmg * mtp);
        }

        /// <summary> 스킬 18번 : 시전자의 공격시 체력 회복 </summary>
        public long GetSkillAbility18_RecoveryHealth(int lv, long max_hp)
        {
            var cdb = GetInstance().chartDB.GetChartSkill_Data(18);
            float mtp = cdb.f_mtp_val2 + (lv * 0.03f);
            return (long)(max_hp * mtp);
        }

        /// <summary> 스킬 19번 : 방어자 ?회 공격 동안 크리 성공률 50%감소, 크리 대미지 감소 </summary>
        public float GetSkillAbility19_DwCriticalAttackPower(int lv)
        {
            var cdb = GetInstance().chartDB.GetChartSkill_Data(19);
            float mtp = cdb.f_mtp_val1 + (lv * 0.03f);
            return mtp;
            //return Mathf.RoundToInt(_df_cri_dmg * mtp);
        }

        /// <summary> 스킬 20번 : 방어자의 공격 속도가 감소 </summary>
        public float GetSkillAbility20_DwAttackSpeed(int lv, float _df_aSp)
        {
            //var cdb = GetInstance().chartDB.GetChartSkill_Data(20);
            //float mtp = cdb.f_mtp_val3 + (float)(lv * 0.03f);
            //LogPrint.Print("스킬 20번 : 방어자의 공격 속도가 감소 lv : " + lv + ", cdb.f_mtp_val3 : " + cdb.f_mtp_val3 + ", f : " + ((float)(lv * 0.03f)) + ", mtp : " + mtp + ", resunt : " + (_df_aSp - (Math.Truncate((_df_aSp * mtp) * 1000.0f) / 1000.0f)));
            //return _df_aSp - (float)(Math.Truncate((_df_aSp * mtp) * 1000.0f) / 1000.0f);

            var cdb = GetInstance().chartDB.GetChartSkill_Data(20);
            float mtp = cdb.f_mtp_val3 + (float)(lv * 0.03f);
            return System.Math.Clamp(_df_aSp - mtp, 0.25f, 1f);
            //return _df_aSp - (float)(Math.Truncate((_df_aSp * mtp) * 1000.0f) / 1000.0f);
        }

        /// <summary> 스킬 22번 : 상대로부터 마지막으로 받은 대미지를 나의 hg로 회복 </summary>
        public int GetSkillAbility22_GetLastDamageHpUp (int lv, float _last_dmg)
        {
            var cdb = GetInstance().chartDB.GetChartSkill_Data(22);
            float mtp = cdb.f_mtp_val1 + (lv * 0.03f);
            return Mathf.RoundToInt(_last_dmg * mtp);
        }

        /// <summary> 스킬 23번 : 방어력 증가값 </summary>
        public float GetSkillAbility23_UpDefense (int lv)
        {
            var cdb = GetInstance().chartDB.GetChartSkill_Data(23);
            float mtp = cdb.f_mtp_val1 + (lv * 0.03f);
            return mtp;
        }
        /// <summary> 스킬 23번 : 회피율 증가값 </summary>
        public float GetSkillAbility23_UpEvasion(int lv)
        {
            var cdb = GetInstance().chartDB.GetChartSkill_Data(23);
            float mtp = cdb.f_mtp_val2 + (lv * 0.03f);
            return mtp;
        }
        /// <summary> 스킬 23번 : 치명타 방어력 증가값 </summary>
        public float GetSkillAbility23_UpCriticalDefense(int lv)
        {
            var cdb = GetInstance().chartDB.GetChartSkill_Data(23);
            float mtp = cdb.f_mtp_val3 + (lv * 0.03f);
            return mtp;
        }

        /// <summary> 스킬 24번 : 기절 연장 횟수 </summary>
        public int GetSkillAbility24_StunnedExtension()
        {
            return 3;
        }

        /// <summary> 스킬 26번 : 스킬 대미지 감소 </summary>
        public long GetSkillAbility26_DwSkillDamage (int lv, long _rlt_dmg)
        {
            var cdb = GetInstance().chartDB.GetChartSkill_Data(26);
            float mtp = cdb.f_mtp_val1 + (lv * 0.03f);
            long rlt_dmg = _rlt_dmg - (long)(_rlt_dmg * mtp);
            if (rlt_dmg <= 0)
                rlt_dmg = 0;

            return rlt_dmg;
        }

        #endregion
        //END endregion --------------------------------------

        // ###################################################
        // ---------------------- START ----------------------
        #region ##### 펫 #####
        public cdb_pet GetCdbPet(int _pRt, int _pId)
        {
            int indx = list_cdb_pet.FindIndex(obj => obj.p_rt == _pRt && obj.p_id == _pId);
            if (indx >= 0)
                return list_cdb_pet[indx];
            else return default;
        }

        public List<cdb_pet> GetCdbPetRatingAll(int _pRt)
        {
            return list_cdb_pet.FindAll(obj => obj.p_rt == _pRt);
        }

        private cdb_pet_sop GetCdbPetSop(int p_rt, int p_id)
        {
            int indx = list_cdb_pet_sop.FindIndex(obj => obj.p_rt == p_rt && obj.p_id == p_id);
            LogPrint.EditorPrint("p_rt : " + p_rt + ", p_id : " + p_id + ", indx : " + indx);
            if (indx >= 0)
            {
                return list_cdb_pet_sop[indx];
            }

            return default;
        }

        public List<cdb_pet> GetCdbPetAll() => list_cdb_pet;
        public cdb_pet GetCdbPet(int indx) => list_cdb_pet[indx];
        /// <summary>
        /// 펫 전용 옵션이름 
        /// </summary>
        public string GetCdbPetSopName(int sop_id)
        {
            int indx = list_cdb_pet_sop.FindIndex(obj => obj.sop_id == sop_id);
            if (indx >= 0)
            {
                return list_cdb_pet_sop[indx].sop_name;
            }
            return "-";
        }

        /// <summary>
        /// 펫 전용 옵션 값 min, max 
        /// </summary>
        public float[] GetCdbPetSopValue(int sop_id, int p_rt, int p_id)
        {
            int indx = list_cdb_pet_sop.FindIndex(obj => obj.p_rt == p_rt && obj.p_id == p_id);
            if (indx >= 0)
            {
                if (sop_id == 1)
                    return new float[] { list_cdb_pet_sop[indx].sop1v_min, list_cdb_pet_sop[indx].sop1v_max };
                if (sop_id == 2)
                    return new float[] { list_cdb_pet_sop[indx].sop2v_min, list_cdb_pet_sop[indx].sop2v_max };
                if (sop_id == 3)
                    return new float[] { list_cdb_pet_sop[indx].sop3v_min, list_cdb_pet_sop[indx].sop3v_max };
                if (sop_id == 4)
                    return new float[] { list_cdb_pet_sop[indx].sop4v_min, list_cdb_pet_sop[indx].sop4v_max };
                if (sop_id == 5)
                    return new float[] { list_cdb_pet_sop[indx].sop5v_min, list_cdb_pet_sop[indx].sop5v_max };
                if (sop_id == 6)
                    return new float[] { list_cdb_pet_sop[indx].sop6v_min, list_cdb_pet_sop[indx].sop6v_max };
                if (sop_id == 7)
                    return new float[] { list_cdb_pet_sop[indx].sop7v_min, list_cdb_pet_sop[indx].sop7v_max };
                if (sop_id == 8)
                    return new float[] { list_cdb_pet_sop[indx].sop8v_min, list_cdb_pet_sop[indx].sop8v_max };
                if (sop_id == 9)
                    return new float[] { list_cdb_pet_sop[indx].sop9v_min, list_cdb_pet_sop[indx].sop9v_max };
            }
            return new float[] { 1, 2 };
        }
        /// <summary>
        /// 펫 옵션 값 min, max 
        /// </summary>
        public float[] GetCdbPetMinMaxStOpValue(bool is_min, int p_rt, int p_id, int p_lv)
        {
            if (is_min)
            {
                float[] val = // [5]
                {
                    GetCdbPetOptionValue(p_rt, p_id, 1, 1, p_lv),
                    GetCdbPetOptionValue(p_rt, p_id, 2, 1, p_lv),
                    GetCdbPetOptionValue(p_rt, p_id, 4, 1, p_lv),
                    GetCdbPetOptionValue(p_rt, p_id, 5, 1, p_lv),
                    GetCdbPetOptionValue(p_rt, p_id, 8, 1, p_lv),
                };
                return val;
            }
            else
            {
                float[] val = // [5]
                {
                    GetCdbPetOptionValue(p_rt, p_id, 1, 10, p_lv),
                    GetCdbPetOptionValue(p_rt, p_id, 2, 10, p_lv),
                    GetCdbPetOptionValue(p_rt, p_id, 4, 10, p_lv),
                    GetCdbPetOptionValue(p_rt, p_id, 5, 10, p_lv),
                    GetCdbPetOptionValue(p_rt, p_id, 8, 10, p_lv),
                };
                return val;
            }
        }
        #endregion
    }

    /// <summary>
    /// ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
    /// #차트 데이터 (확률 차트)
    /// ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
    /// </summary>
    public class ChartProbabilityDB
    {
        private List<GachyaFileId> probabilityID = new List<GachyaFileId>();
        private struct GachyaFileId
        {
            public string probabilityName;
            public string selectedProbabilityFileId;
        }

        public void SetGachyaChartParsingID(BackendReturnObject bro)
        {
            LogPrint.Print("<color=magenta>---------------------------- SetGachyaChartParsingEquipmentReinforced ----------------------------</color>");
            foreach (JsonData jd in bro.GetReturnValuetoJSON()["rows"])
            {
                GachyaFileId gachyaID = new GachyaFileId()
                {
                    probabilityName = RowPaser.StrPaser(jd, "probabilityName"),
                    selectedProbabilityFileId = RowPaser.IntPaser(jd, "selectedProbabilityFileId").ToString(),
                };
                probabilityID.Add(gachyaID);
            }
            LogPrint.Print("<color=magenta>SUCCESS---------------------------- SetGachyaChartParsingEquipmentReinforced ----------------------------</color>");
        }

        /// <summary> 확률명을 가지고 확률 차트 데이터를 가져옴 </summary>
        public string GetSelectedProbabilityFileId(string _chart_name) => probabilityID.Find((GachyaFileId obj) => string.Equals(obj.probabilityName, _chart_name)).selectedProbabilityFileId;
    }

    /// <summary>
    /// ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
    /// #좀비 캐릭터 관련 데이터 
    /// ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
    /// </summary>
    public class CharacterDB : TableDB
    {
        [System.Serializable]
        public struct StatValue
        {
            // 매인 스탯 + 옵션 스탯 
            public long combat_power;

            /// <summary> 무기 : 공격력 </summary>
            public long p0_wea_attackPower;
            /// <summary> 방패 : 방어력 </summary>
            public long p1_shi_defance;
            /// <summary> 헬멧 : 피해량 감소% </summary>
            public float p2_hel_damageReduction;
            /// <summary> 어깨 : 체력% </summary>
            public float p3_sho_health;
            /// <summary> 갑옷 : 체력 </summary>
            public long p4_arm_health;
            /// <summary> 장갑 : 크리티컬 공격력 </summary>
            public long p5_gau_criticalPower;
            /// <summary> 바지 : 공격력 </summary>
            public long p6_pan_attackPower;
            /// <summary> 부츠 : 공격 속도% </summary>
            public float p7_boo_attackSpeed;
            /// <summary> 목걸이 : 크리티컬 확률% </summary>
            public float p8_nec_criticalRate;
            /// <summary> 귀걸이 : 방어력% </summary>
            public float p9_ear_defance;
            /// <summary> 반지 : 공격력% </summary>
            public float p10_rin_attackPower;

            // 장신구 전용 옵션
            /// <summary> #1.PvE피해 증가 </summary> 
            public float sop1_val;
            /// <summary> #2.PvP피해 증가 </summary> 
            public float sop2_val;
            /// <summary> #3.PvE피해 감소 </summary> 
            public float sop3_val;
            /// <summary> #4.PvP피해 감소 </summary> 
            public float sop4_val;
            /// <summary> #5.골드 획득 증가 </summary> 
            public float sop5_val;
            /// <summary> #6.장비 드랍률 증가 </summary> 
            public float sop6_val;
            /// <summary> #7.보스 몬스터 피해 증가 </summary> 
            public float sop7_val;
            /// <summary> #8.최대 체력의 5% 회복 (확률) </summary> 
            public float sop8_val;
            /// <summary> #9.버블 추가 획득(확률) </summary> 
            public float sop9_val;
            /// <summary> #10.상대 버블 차감(확률) </summary> 
            public float sop10_val;

            // 전설 장비 전용 옵션 
            /// <summary> #1.공격력 옵션 증가 </summary> 
            public float eq_sop1_val;
            /// <summary> #2.방어력 옵션 증가 </summary> 
            public float eq_sop2_val;
            /// <summary> #3.치명타 공격력 증가 </summary> 
            public float eq_sop3_val;
            /// <summary> #4.체력 증가 </summary> 
            public float eq_sop4_val;
            /// <summary> #5.치명타 방어력 증가 </summary> 
            public float eq_sop5_val;

            // 펫 전용 옵션
            public PetSpOpTotalFigures petSpOpTotalFigures;

            // 몬스터에만 사용 
            public int eq_rt;
            public int eq_id;
        }

        private StatValue stat = new StatValue();
        public StatValue GetStat() => stat;
        public void SetStat(StatValue val) => stat = val;

        /// <summary> 전체 스탯 총합 세팅 </summary>
        public StatValue SetPlayerStatValue(bool isOnlyPvE = true) // GetMonsterStatValue
        {
            if(isOnlyPvE)
                _tableDB.SetttingEquipWearingData();

            StatValue stat = new StatValue();
            // --------------------------------------------------------------------------------------------------------------------------------------------
            // --------------------------------------------------------------------------------------------------------------------------------------------
            // # 매인 스탯 #
            var wpEqDb = _tableDB.GetNowWearingEquipPartsData(0);

            stat.p0_wea_attackPower = (long)_chartDB.GetMainStatValue(_tableDB.GetNowWearingEquipPartsData(0))[0];
            stat.p1_shi_defance = (long)_chartDB.GetMainStatValue(_tableDB.GetNowWearingEquipPartsData(1))[0];
            stat.p2_hel_damageReduction = (float)_chartDB.GetMainStatValue(_tableDB.GetNowWearingEquipPartsData(2))[0];
            stat.p3_sho_health = (float)_chartDB.GetMainStatValue(_tableDB.GetNowWearingEquipPartsData(3))[0];
            stat.p4_arm_health = (long)_chartDB.GetMainStatValue(_tableDB.GetNowWearingEquipPartsData(4))[0];
            stat.p5_gau_criticalPower = (long)_chartDB.GetMainStatValue(_tableDB.GetNowWearingEquipPartsData(5))[0];
            stat.p6_pan_attackPower = (long)_chartDB.GetMainStatValue(_tableDB.GetNowWearingEquipPartsData(6))[0];
            stat.p7_boo_attackSpeed = (float)_chartDB.GetMainStatValue(_tableDB.GetNowWearingEquipPartsData(7))[0];

            stat.p8_nec_criticalRate = (float)_chartDB.GetMainStatValue(_tableDB.GetNowWearingEquipPartsData(8))[0];
            stat.p9_ear_defance = (float)_chartDB.GetMainStatValue(_tableDB.GetNowWearingEquipPartsData(9))[0];
            stat.p10_rin_attackPower = (float)_chartDB.GetMainStatValue(_tableDB.GetNowWearingEquipPartsData(10))[0];

//            // --------------------------------------------------------------------------------------------------------------------------------------------
//            // --------------------------------------------------------------------------------------------------------------------------------------------
//            // # 도감 스탯 #
//            var encycloStatValue = _chartDB.GetEncycloStatValue();
//            stat.p0_wea_attackPower += encycloStatValue.p0_wea_attackPower;     // 1.공격력    : 무기 / 반지 
//            stat.p1_shi_defance += encycloStatValue.p1_shi_defance;             // 2.방어력    : 방패 
//            stat.p5_gau_criticalPower += encycloStatValue.p5_gau_criticalPower; // 4.치명타    : 공격력 : 어깨 
//            stat.p4_arm_health += encycloStatValue.p4_arm_health;               // 5.체력      : 갑옷 
//
//            int lgnd_st1_combat_divi = 0;
//            long lgnd_st2_combat_divi = 0;
//            int lgnd_st4_combat_divi = 0;
//            int lgnd_st5_combat_divi = 0;
//            int lgnd_st8_combat_divi = 0;
//            // --------------------------------------------------------------------------------------------------------------------------------------------
//            // --------------------------------------------------------------------------------------------------------------------------------------------
//            // # 옵션 스탯 #
//            for (int eq_ty = 0; eq_ty <= 10; eq_ty++)
//            {
//                var eqDB = _tableDB.GetNowWearingEquipPartsData(eq_ty);
//                var statOp = eqDB.st_op;
//                for (int opNbr = 0; opNbr <= 3; opNbr++)
//                {
//                    var opStDB = opNbr == 0 ? statOp.op1 : opNbr == 1 ? statOp.op2 : opNbr == 2 ? statOp.op3 : opNbr == 3 ? statOp.op4 : default;
//                    if (opStDB.id > 0)
//                    {
//                        long iVal = (long)GetInstance().chartDB.GetEquipOptionStatValue(opStDB.id, opStDB.rlv, eqDB.eq_rt, eqDB.eq_id, eqDB.m_ehnt_lv, true, eqDB.eq_legend, eqDB.eq_legend_sop_id, eqDB.eq_legend_sop_rlv);
//                        switch (opStDB.id)
//                        {
//                            case 1: 
//                                stat.p0_wea_attackPower += iVal; // 1.공격력 
//                                break;
//                            case 2: 
//                                stat.p1_shi_defance += iVal; // 2.방어력
//                                if (eqDB.eq_legend == 1)
//                                {
//                                    if (eqDB.eq_legend_sop_id == 2)
//                                    {
//                                        lgnd_st2_combat_divi += (long)(iVal / (750/350));
//                                    }
//                                }
//                                break;
//                            case 4: 
//                                stat.p5_gau_criticalPower += iVal;  // 4.치명타 공격력
//                                break;
//                            case 5: 
//                                stat.p4_arm_health += iVal; // 5.체력
//                                break;
//                        }
//                    }
//                }
//            }
//
//            // --------------------------------------------------------------------------------------------------------------------------------------------
//            // --------------------------------------------------------------------------------------------------------------------------------------------
//            #region  # 펫 옵션 스탯 #
//            // 동행중인 펫 
//            var usePetDB = GameDatabase.GetInstance().tableDB.GetUsePet();
//           
//            var petOpStTotalFigures = GameDatabase.GetInstance().tableDB.GetPetOpStTotalFigures(usePetDB); // 펫 옵션 
//            var petSpOpTotalFigures = GameDatabase.GetInstance().tableDB.GetPetSpOpTotalFigures(usePetDB); // 펫 전용 옵션 
//            stat.petSpOpTotalFigures = petSpOpTotalFigures;
//
//            LogPrint.EditorPrint("11111 petOpStTotalFigures.op1v : " + petOpStTotalFigures.op1v); // 공격력 증가 
//            LogPrint.EditorPrint("11111 petOpStTotalFigures.op2v : " + petOpStTotalFigures.op2v); // 방어력 증가 
//            LogPrint.EditorPrint("11111 petOpStTotalFigures.op3v : " + petOpStTotalFigures.op4v); // 치 공 증가 
//            LogPrint.EditorPrint("11111 petOpStTotalFigures.op4v : " + petOpStTotalFigures.op5v); // 체력 증가 
//            LogPrint.EditorPrint("11111 petOpStTotalFigures.op5v : " + petOpStTotalFigures.op8v); // 치 방 증가 
//
//            if (isOnlyPvE)
//            {
//                // 공격력 옵션 증가 (PvE전용)
//                if (petSpOpTotalFigures.sop5_value > 0.0f)
//                    petOpStTotalFigures.op1v += petOpStTotalFigures.op1v * (petSpOpTotalFigures.sop5_value * 0.01f);
//
//                // 방어력 옵션 증가 (PvE전용)
//                if (petSpOpTotalFigures.sop6_value > 0.0f)
//                    petOpStTotalFigures.op2v += petOpStTotalFigures.op2v * (petSpOpTotalFigures.sop6_value * 0.01f);
//
//                // 체력 옵션 증가 (PvE전용)
//                if (petSpOpTotalFigures.sop7_value > 0.0f)
//                    petOpStTotalFigures.op5v += petOpStTotalFigures.op5v * (petSpOpTotalFigures.sop7_value * 0.01f);
//
//                // 치.공 옵션 증가 (PvE전용)
//                if (petSpOpTotalFigures.sop8_value > 0.0f)
//                    petOpStTotalFigures.op4v += petOpStTotalFigures.op4v * (petSpOpTotalFigures.sop8_value * 0.01f);
//
//                // 치.방 옵션 증가 (PvE전용)
//                if (petSpOpTotalFigures.sop9_value > 0.0f)
//                    petOpStTotalFigures.op8v += petOpStTotalFigures.op8v * (petSpOpTotalFigures.sop9_value * 0.01f);
//            }
//
//            LogPrint.EditorPrint("22222 petOpStTotalFigures.op1v : " + petOpStTotalFigures.op1v); // 공격력 증가 
//            LogPrint.EditorPrint("22222 petOpStTotalFigures.op2v : " + petOpStTotalFigures.op2v); // 방어력 증가 
//            LogPrint.EditorPrint("22222 petOpStTotalFigures.op3v : " + petOpStTotalFigures.op4v); // 치 공 증가 
//            LogPrint.EditorPrint("22222 petOpStTotalFigures.op4v : " + petOpStTotalFigures.op5v); // 체력 증가 
//            LogPrint.EditorPrint("22222 petOpStTotalFigures.op5v : " + petOpStTotalFigures.op8v); // 치 방 증가 
//
//            stat.p0_wea_attackPower += (long)(stat.p0_wea_attackPower * petOpStTotalFigures.op1v);
//            stat.p1_shi_defance += (long)(stat.p1_shi_defance * petOpStTotalFigures.op2v);
//            stat.p5_gau_criticalPower += (long)(stat.p5_gau_criticalPower * petOpStTotalFigures.op4v);
//            stat.p4_arm_health += (long)(stat.p4_arm_health * petOpStTotalFigures.op5v);
//            //stat.stat8_valCriDefense += (long)(stat.stat8_valCriDefense * petOpStTotalFigures.op8v);
//
//            LogPrint.EditorPrint("petSpOpTotalFigures.sop1_value : " + petSpOpTotalFigures.sop1_value);// 퀘스트/장비 판매 골드 획득 증가
//            LogPrint.EditorPrint("petSpOpTotalFigures.sop2_value : " + petSpOpTotalFigures.sop2_value);// 아이템 드랍률 증가
//            LogPrint.EditorPrint("petSpOpTotalFigures.sop3_value : " + petSpOpTotalFigures.sop3_value);// 장비 드랍률 증가
//            LogPrint.EditorPrint("petSpOpTotalFigures.sop4_value : " + petSpOpTotalFigures.sop4_value);// 몬스터 공격력 감소
//            LogPrint.EditorPrint("petSpOpTotalFigures.sop5_value : " + petSpOpTotalFigures.sop5_value);// 공격력 옵션 증가 (PvE전용)
//            LogPrint.EditorPrint("petSpOpTotalFigures.sop6_value : " + petSpOpTotalFigures.sop6_value);// 방어력 옵션 증가 (PvE전용)
//            LogPrint.EditorPrint("petSpOpTotalFigures.sop7_value : " + petSpOpTotalFigures.sop7_value);// 체력 옵션 증가 (PvE전용)
//            LogPrint.EditorPrint("petSpOpTotalFigures.sop8_value : " + petSpOpTotalFigures.sop8_value);// 치명타 공격력 옵션 증가 (PvE전용)
//            LogPrint.EditorPrint("petSpOpTotalFigures.sop9_value : " + petSpOpTotalFigures.sop9_value);// 치명타 방어력 옵션 증가 (PvE전용)
//            #endregion
//
//            // --------------------------------------------------------------------------------------------------------------------------------------------
//            // --------------------------------------------------------------------------------------------------------------------------------------------
//            #region  # 장신구 전용 옵션 스탯 #
//            var eq_necklace = _tableDB.GetNowWearingEquipPartsData(8);
//            stat.sop1_val = eq_necklace.st_sop_ac.id == 1 ? GetInstance().chartDB.GetAcceSpecialOptionValue(eq_necklace)[0] : 0f;
//            stat.sop2_val = eq_necklace.st_sop_ac.id == 2 ? GetInstance().chartDB.GetAcceSpecialOptionValue(eq_necklace)[0] : 0f;
//            stat.sop3_val = eq_necklace.st_sop_ac.id == 3 ? GetInstance().chartDB.GetAcceSpecialOptionValue(eq_necklace)[0] : 0f;
//            stat.sop4_val = eq_necklace.st_sop_ac.id == 4 ? GetInstance().chartDB.GetAcceSpecialOptionValue(eq_necklace)[0] : 0f;
//            stat.sop5_val = eq_necklace.st_sop_ac.id == 5 ? GetInstance().chartDB.GetAcceSpecialOptionValue(eq_necklace)[0] : 0f;
//            stat.sop6_val = eq_necklace.st_sop_ac.id == 6 ? GetInstance().chartDB.GetAcceSpecialOptionValue(eq_necklace)[0] : 0f;
//            stat.sop7_val = eq_necklace.st_sop_ac.id == 7 ? GetInstance().chartDB.GetAcceSpecialOptionValue(eq_necklace)[0] : 0f;
//            stat.sop8_val = eq_necklace.st_sop_ac.id == 8 ? GetInstance().chartDB.GetAcceSpecialOptionValue(eq_necklace)[0] : 0f;
//            stat.sop9_val = eq_necklace.st_sop_ac.id == 9 ? GetInstance().chartDB.GetAcceSpecialOptionValue(eq_necklace)[0] : 0f;
//            stat.sop10_val = eq_necklace.st_sop_ac.id == 10 ? GetInstance().chartDB.GetAcceSpecialOptionValue(eq_necklace)[0] : 0f;
//
//            var eq_earring = _tableDB.GetNowWearingEquipPartsData(9);
//            stat.sop1_val += eq_earring.st_sop_ac.id == 1 ? GetInstance().chartDB.GetAcceSpecialOptionValue(eq_earring)[0] : 0f;
//            stat.sop2_val += eq_earring.st_sop_ac.id == 2 ? GetInstance().chartDB.GetAcceSpecialOptionValue(eq_earring)[0] : 0f;
//            stat.sop3_val += eq_earring.st_sop_ac.id == 3 ? GetInstance().chartDB.GetAcceSpecialOptionValue(eq_earring)[0] : 0f;
//            stat.sop4_val += eq_earring.st_sop_ac.id == 4 ? GetInstance().chartDB.GetAcceSpecialOptionValue(eq_earring)[0] : 0f;
//            stat.sop5_val += eq_earring.st_sop_ac.id == 5 ? GetInstance().chartDB.GetAcceSpecialOptionValue(eq_earring)[0] : 0f;
//            stat.sop6_val += eq_earring.st_sop_ac.id == 6 ? GetInstance().chartDB.GetAcceSpecialOptionValue(eq_earring)[0] : 0f;
//            stat.sop7_val += eq_earring.st_sop_ac.id == 7 ? GetInstance().chartDB.GetAcceSpecialOptionValue(eq_earring)[0] : 0f;
//            stat.sop8_val += eq_earring.st_sop_ac.id == 8 ? GetInstance().chartDB.GetAcceSpecialOptionValue(eq_earring)[0] : 0f;
//            stat.sop9_val += eq_earring.st_sop_ac.id == 9 ? GetInstance().chartDB.GetAcceSpecialOptionValue(eq_earring)[0] : 0f;
//            stat.sop10_val += eq_earring.st_sop_ac.id == 10 ? GetInstance().chartDB.GetAcceSpecialOptionValue(eq_earring)[0] : 0f;
//
//            var eq_ring = _tableDB.GetNowWearingEquipPartsData(10);
//            stat.sop1_val += eq_ring.st_sop_ac.id == 1 ? GetInstance().chartDB.GetAcceSpecialOptionValue(eq_ring)[0] : 0f;
//            stat.sop2_val += eq_ring.st_sop_ac.id == 2 ? GetInstance().chartDB.GetAcceSpecialOptionValue(eq_ring)[0] : 0f;
//            stat.sop3_val += eq_ring.st_sop_ac.id == 3 ? GetInstance().chartDB.GetAcceSpecialOptionValue(eq_ring)[0] : 0f;
//            stat.sop4_val += eq_ring.st_sop_ac.id == 4 ? GetInstance().chartDB.GetAcceSpecialOptionValue(eq_ring)[0] : 0f;
//            stat.sop5_val += eq_ring.st_sop_ac.id == 5 ? GetInstance().chartDB.GetAcceSpecialOptionValue(eq_ring)[0] : 0f;
//            stat.sop6_val += eq_ring.st_sop_ac.id == 6 ? GetInstance().chartDB.GetAcceSpecialOptionValue(eq_ring)[0] : 0f;
//            stat.sop7_val += eq_ring.st_sop_ac.id == 7 ? GetInstance().chartDB.GetAcceSpecialOptionValue(eq_ring)[0] : 0f;
//            stat.sop8_val += eq_ring.st_sop_ac.id == 8 ? GetInstance().chartDB.GetAcceSpecialOptionValue(eq_ring)[0] : 0f;
//            stat.sop9_val += eq_ring.st_sop_ac.id == 9 ? GetInstance().chartDB.GetAcceSpecialOptionValue(eq_ring)[0] : 0f;
//            stat.sop10_val += eq_ring.st_sop_ac.id == 10 ? GetInstance().chartDB.GetAcceSpecialOptionValue(eq_ring)[0] : 0f;
//            #endregion
//
//            stat.combat_power += (long)(stat.p0_wea_attackPower * GetInstance().tableDB.GetCombatXValue(1));
//            stat.combat_power += (long)(stat.p1_shi_defance * GetInstance().tableDB.GetCombatXValue(2));
//            stat.combat_power += (long)(stat.p2_hel_damageReduction * GetInstance().tableDB.GetCombatXValue(3));
//            stat.combat_power += (long)(stat.p3_sho_health* GetInstance().tableDB.GetCombatXValue(4));
//            stat.combat_power += (long)(stat.p4_arm_health* GetInstance().tableDB.GetCombatXValue(5));
//            stat.combat_power += (long)(stat.p5_gau_criticalPower* GetInstance().tableDB.GetCombatXValue(6)); 
//            stat.combat_power += (long)(stat.p6_pan_attackPower* GetInstance().tableDB.GetCombatXValue(7));
//            stat.combat_power += (long)(stat.p7_boo_attackSpeed* GetInstance().tableDB.GetCombatXValue(8));
//            stat.combat_power += (long)(stat.p8_nec_criticalRate * GetInstance().tableDB.GetCombatXValue(9));
//            stat.combat_power += (long)(stat.p9_ear_defance * GetInstance().tableDB.GetCombatXValue(9));
//            stat.combat_power += (long)(stat.p10_rin_attackPower * GetInstance().tableDB.GetCombatXValue(9));

            if (isOnlyPvE)
                SetStat(stat);

            return stat;
        }
    }

    /// <summary>
    /// ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
    /// #대장간 관련 데이터 
    /// ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
    /// </summary>
    public class SmithyDB : TableDB
    {
        /// <summary>
        /// 특정 장비 부위의 체일 높은 장비의 UID 
        /// </summary>
        public long GetEquipTypeToHighEnhantLevelUID(int _parts_type, int _compare_ehtLv)
        {
            var arr_parts = GameDatabase.GetInstance().tableDB.GetTypeEquipment(_parts_type).FindAll((TableDB.Equipment eq) => eq.m_ehnt_lv > _compare_ehtLv);
            if (arr_parts.Count > 0)
            {
                arr_parts.Sort((x, y) => y.m_ehnt_lv.CompareTo(x.m_ehnt_lv));
                return arr_parts[0].aInUid;
            }

            return 0;
        }

        /// <summary>
        /// 장비 / 장신구 레벨업 가격 
        /// </summary>
        public long GetEquipNormalUpGold(int rt, int now_nor_lv, int up_cnt)
        {
            //int eqMaxLv = GameDatabase.GetInstance().chartDB.GetDicBalance("eq.normal.max.level").val_int;
            string blcID = string.Format("equip.level.up.gold.rt{0}", rt);
            int blcVal = GameDatabase.GetInstance().chartDB.GetDicBalance(blcID).val_int;
            long val = 0;
            for (int i = now_nor_lv; i < up_cnt; i++)
            {
                val += blcVal * (i + 1);
            }

            LogPrint.Print("<color=magenta> --------- rt : " + rt + ", now_nor_lv : " + now_nor_lv + ", up_cnt : " + up_cnt + ", val : " + val + " </color>");
            return val;
        }

        // ------------------------ 강화 ------------------------
        // ------------------------ 강화 ------------------------
        // ------------------------ 강화 ------------------------

        /// <summary> 현재 장비 강화에 필요한 강화석 수량 </summary>
        public int GetEnhantStonCount(int ehntLv) => GetInstance().chartDB.GetDicBalance(string.Format("ehnt.ston.need.cnt_{0}", ehntLv)).val_int;

        /// <summary> 장비의 강화 레벨에 따라 강화석 등급 리턴 </summary>
        public int GetEnhantStonTypeRating(int ehntLv) => GetInstance().chartDB.GetDicBalance(string.Format("ehnt.ston.need.rt_{0}", ehntLv)).val_int;

        /// <summary> 장비의 강화석 종류에 따라 현재 내가 소지중인 강화석 갯수를 리턴 </summary>
        public int GetEnhantStonTypeRatingCount(int _eq_ty, int _ston_rt)
        {
            bool isAcce = GetInstance().tableDB.GetIsPartsTypeAcce(_eq_ty);
            int it_ty = GetInstance().tableDB.GetIsPartsTypeAcce(_eq_ty) == true ? item_type_ac_ston : item_type_eq_ston;
            return _tableDB._item.Find((Item it) => Equals(it.type, it_ty) && Equals(it.rating, _ston_rt)).count; // _ston_rt : 1-일반, 2-중급, 3-고급, 4-희귀 
        }

        /// <summary>
        /// 강화석 이름 
        /// </summary>
        public string GetStonName(int ston_ty, int ston_rt)
        {
            if (ston_ty == GameDatabase.TableDB.item_type_eq_ston)
            {
                return LanguageGameData.GetInstance().GetString(string.Format("item.name.item_21_{0}", ston_rt));
            }
            else if (ston_ty == GameDatabase.TableDB.item_type_ac_ston)
            {
                return LanguageGameData.GetInstance().GetString(string.Format("item.name.item_27_{0}", ston_rt));
            }
            return string.Empty;
        }

        /// <summary>
        /// 강화 축복 주문서 사용시 확률 증가 값 
        /// </summary>
        public float GetEnhantBlessingRate(int useRating, float basicProbability)
        {
            float p = GameDatabase.GetInstance().chartDB.GetDicBalance(string.Format("equip.enhant.bonus.rate.rt{0}", useRating)).val_float;
            if (p > 0)
                return basicProbability += basicProbability * p;
            else return basicProbability;
        }

        /// <summary>
        /// 장비 강화 레벨에 따른 기본 확률 
        /// </summary>
        public float GetEnhantRate(int _eq_level) => GetInstance().chartDB.GetDicBalance(string.Format("equip.enhancement_{0}", _eq_level)).val_float_percent;

        // ------------------------ 강화석 진화 ------------------------
        // ------------------------ 강화석 진화 ------------------------
        // ------------------------ 강화석 진화 ------------------------

        /// <summary> 강화석 진화시 다음 강화석 등급 리턴 </summary>
        public int GetEvolutionStonNextRating(int _nStonRat)
        {
            _nStonRat++;
            if (_nStonRat > 4)
            {
                return -1;
            }

            return _nStonRat;
        }

        // ------------------------ 강화 레벨 이전 ------------------------
        // ------------------------ 강화 레벨 이전 ------------------------
        // ------------------------ 강화 레벨 이전 ------------------------

        // ------------------------ 장신구 옵션 변경 ------------------------
        // ------------------------ 장신구 옵션 변경 ------------------------
        // ------------------------ 장신구 옵션 변경 ------------------------

        // ------------------------ 장신구 합성 ------------------------
        // ------------------------ 장신구 합성 ------------------------
        // ------------------------ 장신구 합성 ------------------------

        /// <summary>
        /// 기본 성공률 
        /// </summary>
        public float GetAcceSyntRate () => GetInstance().chartDB.GetDicBalance("acce.synthesis.probability").val_float;

        public float GetAcceAdvcRate () => GetInstance().chartDB.GetDicBalance("acce.advancement.probability").val_float;
    }

    /// <summary>
    /// ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
    /// #인벤토리 관련 데이터 
    /// ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
    /// </summary>
    public class InventoryDB 
    {
        private InvenBackUp _invenBackUp = new InvenBackUp();
        public InvenBackUp GetInvenBackUp() => _invenBackUp;
        public void SetInvenBackUp(InvenBackUp ibp)
        {
            _invenBackUp = ibp;
        }

        /// <summary>
        /// 접속시 인벤에 들어있던 데이터 장비 클라에 세팅  
        /// </summary>
        public async Task GetLoadToSetting(InvenData invndata)
        {
            LogPrint.Print("<color=yellow> GetLoadToSetting invndata.equips.Count : " + invndata.equips.Count + "</color>");
            if(invndata.equips != null)
            {
                foreach (TableDB.Equipment equipBkupDB in invndata.equips)
                {
                    if (string.IsNullOrEmpty(equipBkupDB.indate))
                    {
                        int ovr_indx = GetInstance().tableDB.GetAllEquipment().FindIndex(x => long.Equals(x.aInUid, equipBkupDB.aInUid));
                        if (ovr_indx == -1)
                        {
                            GetInstance().tableDB.UpdateClientDB_Equip(equipBkupDB);
                        }
                    }
                }
            }

            if(invndata.pets != null)
            {
                foreach (TableDB.Pet petBkupDB in invndata.pets)
                {
                    if (string.IsNullOrEmpty(petBkupDB.indate))
                    {
                        int ovr_indx = GetInstance().tableDB.GetAllPets().FindIndex(x => long.Equals(x.aInUid, petBkupDB.aInUid));
                        if (ovr_indx == -1)
                        {
                            GetInstance().tableDB.UpdateClientDB_Pet(petBkupDB);
                        }
                    }
                }
            }

            if(invndata.petsEncy != null)
            {
                foreach (TableDB.PetEncy petEncyBkupDB in invndata.petsEncy)
                {
                    if (string.IsNullOrEmpty(petEncyBkupDB.indate))
                    {
                        int ovr_indx = GetInstance().tableDB.GetAllPetEncy().FindIndex(x => long.Equals(x.aInUid, petEncyBkupDB.aInUid));
                        if (ovr_indx == -1)
                        {
                            GetInstance().tableDB.UpdateClientDB_PetEncy(petEncyBkupDB);
                        }
                    }
                }
            }
        }

        /// <summary> 인벤토리 레벨업(인벤 확장) 비용 </summary>
        public int GetLevelUpPrice() => GameDatabase.GetInstance().chartDB.GetDicBalance("price.invn.up").val_int;

        /// <summary> 인벤토리 레벨 </summary>
        public int GetLevel() => GetInstance().tableDB.GetUserInfo().m_invn_lv + 1;

        /// <summary> 인벤토리 레벨에 따른 인벤토링 칸 </summary>
        public int GetLevelInvenCount() => 50 + (GetLevel() * 50);

        /// <summary> 인벤토리에 빈 공간이 있는가? </summary>
        public bool CheckIsEmpty(int inCnt = 0)
        {
            bool isEmp = GetInItemsCount() + inCnt < GetLevelInvenCount();
            if (isEmp) // 인벤 공간이 있다 
                NotificationIcon.GetInstance().OffInventoryMax(); // 인벤토리 공간 부족 알림 아이콘 
            else // 부족 
                NotificationIcon.GetInstance().OnInventoryMax(); // 인벤토리 공간 부족 알림 아이콘 
            
            return isEmp;
        }

        /// <summary>인벤에 들어있는 장비 및 스킬, 아이템등 수량 </summary>
        public int GetInItemsCount()
        {
            int eq_cnt = GetInstance().tableDB.GetAllEquipment().FindAll((GameDatabase.TableDB.Equipment eq) => eq.m_state == 0).Count;
            int it_cnt = GetInstance().tableDB.GetItemAll().Count;
            return eq_cnt + it_cnt;
        }
    }

    /// <summary>
    /// ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
    /// # 몬스터 관련 데이터 
    /// ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
    /// </summary>
    public class MonsterDB :ChartDB
    {
        public cdb_chpt_mnst_stat GetMonsterStatDb(int chpt_dvs_nbr)
        {
            int indx = list_cdb_chpt_mnst_stat.FindIndex((cdb_chpt_mnst_stat obj) => obj.chpt_dvs_nbr == chpt_dvs_nbr);
            if (indx >= 0)
                return list_cdb_chpt_mnst_stat[indx];
            else return list_cdb_chpt_mnst_stat[list_cdb_chpt_mnst_stat.Count - 1];
        }
        public List<cdb_chpt_mnst_stat> GetLoopStageMonsterStatDBs(int loop_pnt) => list_cdb_chpt_mnst_stat.FindAll(x => x.loop_pnt == loop_pnt);
        /// <summary>
        /// 챕터 스테이지의 보스 전투력 
        /// </summary>
        public long GetChapterMonsterCombat(int chpt_dvs_nbr)
        {
            var cdb_mnst_stat = GetInstance().monsterDB.GetMonsterStatDb(chpt_dvs_nbr);
            TableDB.Equipment eqdata = new TableDB.Equipment()
            {
                eq_rt = cdb_mnst_stat.eq_rat,
                eq_id = cdb_mnst_stat.eq_id,
                m_norm_lv = cdb_mnst_stat.chpt_norm_lv,
                m_ehnt_lv = cdb_mnst_stat.chpt_ehnt_lv,
                ma_st_rlv = cdb_mnst_stat.chpt_m_st_rlv,
            };

            CharacterDB.StatValue stat = GetInstance().monsterDB.SetMonsterStatValue(eqdata, 0, true, cdb_mnst_stat.chpt_id <= 23 ? true : false);

            return stat.combat_power;
        }

        /// <summary>
        /// 던전 몬스터 스탯 
        /// </summary>
        public CharacterDB.StatValue GetDungeonMonsterStatValue(IG.ModeType md_ty, int mon_id, int dg_nbr = 0)
        {
            bool isRndOpSt = true;
            CharacterDB.StatValue mnst_statDB = new CharacterDB.StatValue();
            TableDB.Equipment mnst_eqDB = new TableDB.Equipment();
            if (md_ty == IG.ModeType.DUNGEON_TOP)
            {
                var cdb_dg_top = list_cdb_dungeon_top.Find((cdb_dungeon_top obj) => obj.nbr == dg_nbr);
                isRndOpSt = cdb_dg_top.nbr <= 29 ? true : false;
                mnst_eqDB = new TableDB.Equipment()
                {
                    eq_rt = cdb_dg_top.eq_rt,
                    eq_id = cdb_dg_top.eq_id,
                    m_norm_lv = cdb_dg_top.dg_top_norm_lv,
                    m_ehnt_lv = cdb_dg_top.dg_top_ehnt_lv,
                    ma_st_rlv = cdb_dg_top.dg_top_m_st_rlv,
                };
            }
            else if (md_ty == IG.ModeType.DUNGEON_MINE)
            {
                var cdb_dg_mine = list_cdb_dungeon_mine.Find((cdb_dungeon_mine obj) => obj.nbr == dg_nbr);
                isRndOpSt = cdb_dg_mine.nbr <= 3 ? true : false;
                mnst_eqDB = new TableDB.Equipment()
                {
                    eq_rt = cdb_dg_mine.eq_rt,
                    eq_id = cdb_dg_mine.eq_id,
                    m_norm_lv = cdb_dg_mine.dg_mine_norm_lv,
                    m_ehnt_lv = cdb_dg_mine.dg_mine_ehnt_lv,
                    ma_st_rlv = cdb_dg_mine.dg_mine_m_st_rlv,
                };
            }
            else if (md_ty == IG.ModeType.DUNGEON_RAID)
            {
                var cdb_dg_raid = list_cdb_dungeon_raid.Find((cdb_dungeon_raid obj) => obj.nbr == dg_nbr);
                isRndOpSt = cdb_dg_raid.nbr <= 3 ? true : false;
                mnst_eqDB = new TableDB.Equipment()
                {
                    eq_rt = cdb_dg_raid.eq_rt,
                    eq_id = cdb_dg_raid.eq_id,
                    m_norm_lv = cdb_dg_raid.dg_raid_norm_lv,
                    m_ehnt_lv = cdb_dg_raid.dg_raid_ehnt_lv,
                    ma_st_rlv = cdb_dg_raid.dg_raid_m_st_rlv,
                };
            }

            // # 스탯 #
            mnst_statDB = GetInstance().monsterDB.SetMonsterStatValue(mnst_eqDB, mon_id, true, isRndOpSt);

            return mnst_statDB;
        }

        public Dictionary<int, int[]> GetMonsterRandomSkills()
        {
            Dictionary<int, int[]> skill_data = new Dictionary<int, int[]>();
            var playerSkillAll = GameDatabase.GetInstance().tableDB.GetSkillAll();
            if (playerSkillAll.Count > 0)
            {
                List<int> random = new List<int>();
                for (int i = 0; i < playerSkillAll.Count; i++)
                    random.Add(i);

                int slot = 0;
                List<int> shuffled = new List<int>(Utility.ShuffleArray(random, (int)Time.time));
                foreach (int idx in shuffled)
                {
                    if (slot < 6)
                    {
                        var sdb = playerSkillAll[idx];
                        var cdb = GameDatabase.GetInstance().chartDB.GetChartSkill_Data(sdb.idx);
                        int sk_id = cdb.s_idx;
                        int sk_lv = UnityEngine.Random.Range(0, sdb.level + 1);
                        int sk_rt = cdb.s_rating;
                        int sk_pt = cdb.s_pnt;
                        int sk_rn = 0;
                        skill_data.Add(slot, new int[] { sk_id, sk_lv, sk_rt, sk_pt, sk_rn });
                        slot++;
                    }
                    else break;
                }
            }

            return skill_data;
        }

        /// <summary>
        /// 몬스터 스탯 
        /// </summary>
        public CharacterDB.StatValue SetMonsterStatValue(TableDB.Equipment mnst_eqDB, int mon_id, bool isMonsterOrUserZB, bool isRndOpSt)
        {
            CharacterDB.StatValue monster_stat = new CharacterDB.StatValue();
            monster_stat.p0_wea_attackPower = (long)_chartDB.GetMainStatValue(mnst_eqDB, 0)[0];
            monster_stat.p1_shi_defance = (long)_chartDB.GetMainStatValue(mnst_eqDB, 1)[0];
            monster_stat.p2_hel_damageReduction = (float)_chartDB.GetMainStatValue(mnst_eqDB, 2)[0];
            monster_stat.p3_sho_health = (float)_chartDB.GetMainStatValue(mnst_eqDB, 3)[0];
            monster_stat.p4_arm_health = (long)_chartDB.GetMainStatValue(mnst_eqDB, 4)[0] / (mon_id + 1);
            monster_stat.p5_gau_criticalPower = (long)_chartDB.GetMainStatValue(mnst_eqDB, 5)[0];
            monster_stat.p6_pan_attackPower = (long)_chartDB.GetMainStatValue(mnst_eqDB, 6)[0];
            monster_stat.p7_boo_attackSpeed = (float)_chartDB.GetMainStatValue(mnst_eqDB, 7)[0];

            monster_stat.p8_nec_criticalRate = (float)_chartDB.GetMainStatValue(mnst_eqDB, 8)[0];
            monster_stat.p9_ear_defance = (float)_chartDB.GetMainStatValue(mnst_eqDB, 9)[0];
            monster_stat.p10_rin_attackPower = (float)_chartDB.GetMainStatValue(mnst_eqDB, 10)[0];

            monster_stat.combat_power += (long)(monster_stat.p0_wea_attackPower * GetInstance().tableDB.GetCombatXValue(1));
            monster_stat.combat_power += (long)(monster_stat.p1_shi_defance * GetInstance().tableDB.GetCombatXValue(2));
            monster_stat.combat_power += (long)(monster_stat.p2_hel_damageReduction * GetInstance().tableDB.GetCombatXValue(3));
            monster_stat.combat_power += (long)(monster_stat.p3_sho_health * GetInstance().tableDB.GetCombatXValue(4));
            monster_stat.combat_power += (long)(monster_stat.p4_arm_health * GetInstance().tableDB.GetCombatXValue(5));
            monster_stat.combat_power += (long)(monster_stat.p5_gau_criticalPower * GetInstance().tableDB.GetCombatXValue(6));
            monster_stat.combat_power += (long)(monster_stat.p6_pan_attackPower * GetInstance().tableDB.GetCombatXValue(7));
            monster_stat.combat_power += (long)(monster_stat.p7_boo_attackSpeed * GetInstance().tableDB.GetCombatXValue(8));
            monster_stat.combat_power += (long)(monster_stat.p8_nec_criticalRate * GetInstance().tableDB.GetCombatXValue(9));
            monster_stat.combat_power += (long)(monster_stat.p9_ear_defance * GetInstance().tableDB.GetCombatXValue(9));
            monster_stat.combat_power += (long)(monster_stat.p10_rin_attackPower * GetInstance().tableDB.GetCombatXValue(9));

            monster_stat.eq_rt = mnst_eqDB.eq_rt;
            monster_stat.eq_id = mnst_eqDB.eq_id;

            return monster_stat;
        }
    }

    /// <summary>
    /// ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
    /// #던전 데이터 
    /// ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
    /// </summary>
    public class DungeonDB
    {
#region ----- struct -----
        // 도전의 탑  
        private Top _top = new Top();
        public struct Top
        {
            public string indate;
            public float
                    ctmNbr0, ctmNbr1, ctmNbr2, ctmNbr3, ctmNbr4, ctmNbr5, ctmNbr6, ctmNbr7, ctmNbr8, ctmNbr9, ctmNbr10,
                    ctmNbr11, ctmNbr12, ctmNbr13, ctmNbr14, ctmNbr15, ctmNbr16, ctmNbr17, ctmNbr18, ctmNbr19, ctmNbr20,
                    ctmNbr21, ctmNbr22, ctmNbr23, ctmNbr24, ctmNbr25, ctmNbr26, ctmNbr27, ctmNbr28, ctmNbr29, ctmNbr30,
                    ctmNbr31, ctmNbr32, ctmNbr33, ctmNbr34, ctmNbr35, ctmNbr36, ctmNbr37, ctmNbr38, ctmNbr39, ctmNbr40;
        }

        // 광산  
        private Mine _mine = new Mine();
        public class Mine
        {
            public string indate = string.Empty;
            public float ctmNbr0, ctmNbr1, ctmNbr2, ctmNbr3;
        }

        // 레이드 
        private Raid _raid = new Raid();
        public class Raid
        {
            public string indate = string.Empty;
            public float ctmNbr0, ctmNbr1, ctmNbr2, ctmNbr3, ctmNbr4, ctmNbr5, ctmNbr6;
        }
#endregion

#region ----- struct get / set -----
        public Top GetTop() => _top;
        public void SetTop(JsonData returnData)
        {
            if (returnData != null)
            {
                if (returnData.Keys.Contains("rows"))
                {
                    JsonData row = returnData["rows"][0];
                    _top = new Top()
                    {
                        indate = RowPaser.StrPaser(row, "inDate"),
                        ctmNbr0 = RowPaser.FloatPaser(row, "ctmNbr0"),
                        ctmNbr1 = RowPaser.FloatPaser(row, "ctmNbr1"),
                        ctmNbr2 = RowPaser.FloatPaser(row, "ctmNbr2"),
                        ctmNbr3 = RowPaser.FloatPaser(row, "ctmNbr3"),
                        ctmNbr4 = RowPaser.FloatPaser(row, "ctmNbr4"),
                        ctmNbr5 = RowPaser.FloatPaser(row, "ctmNbr5"),
                        ctmNbr6 = RowPaser.FloatPaser(row, "ctmNbr6"),
                        ctmNbr7 = RowPaser.FloatPaser(row, "ctmNbr7"),
                        ctmNbr8 = RowPaser.FloatPaser(row, "ctmNbr8"),
                        ctmNbr9 = RowPaser.FloatPaser(row, "ctmNbr9"),
                        ctmNbr10 = RowPaser.FloatPaser(row, "ctmNbr10"),
                        ctmNbr11 = RowPaser.FloatPaser(row, "ctmNbr11"),
                        ctmNbr12 = RowPaser.FloatPaser(row, "ctmNbr12"),
                        ctmNbr13 = RowPaser.FloatPaser(row, "ctmNbr13"),
                        ctmNbr14 = RowPaser.FloatPaser(row, "ctmNbr14"),
                        ctmNbr15 = RowPaser.FloatPaser(row, "ctmNbr15"),
                        ctmNbr16 = RowPaser.FloatPaser(row, "ctmNbr16"),
                        ctmNbr17 = RowPaser.FloatPaser(row, "ctmNbr17"),
                        ctmNbr18 = RowPaser.FloatPaser(row, "ctmNbr18"),
                        ctmNbr19 = RowPaser.FloatPaser(row, "ctmNbr19"),
                        ctmNbr20 = RowPaser.FloatPaser(row, "ctmNbr20"),
                        ctmNbr21 = RowPaser.FloatPaser(row, "ctmNbr21"),
                        ctmNbr22 = RowPaser.FloatPaser(row, "ctmNbr22"),
                        ctmNbr23 = RowPaser.FloatPaser(row, "ctmNbr23"),
                        ctmNbr24 = RowPaser.FloatPaser(row, "ctmNbr24"),
                        ctmNbr25 = RowPaser.FloatPaser(row, "ctmNbr25"),
                        ctmNbr26 = RowPaser.FloatPaser(row, "ctmNbr26"),
                        ctmNbr27 = RowPaser.FloatPaser(row, "ctmNbr27"),
                        ctmNbr28 = RowPaser.FloatPaser(row, "ctmNbr28"),
                        ctmNbr29 = RowPaser.FloatPaser(row, "ctmNbr29"),
                        ctmNbr30 = RowPaser.FloatPaser(row, "ctmNbr30"),
                        ctmNbr31 = RowPaser.FloatPaser(row, "ctmNbr31"),
                        ctmNbr32 = RowPaser.FloatPaser(row, "ctmNbr32"),
                        ctmNbr33 = RowPaser.FloatPaser(row, "ctmNbr33"),
                        ctmNbr34 = RowPaser.FloatPaser(row, "ctmNbr34"),
                        ctmNbr35 = RowPaser.FloatPaser(row, "ctmNbr35"),
                        ctmNbr36 = RowPaser.FloatPaser(row, "ctmNbr36"),
                        ctmNbr37 = RowPaser.FloatPaser(row, "ctmNbr37"),
                        ctmNbr38 = RowPaser.FloatPaser(row, "ctmNbr38"),
                        ctmNbr39 = RowPaser.FloatPaser(row, "ctmNbr39"),
                        ctmNbr40 = RowPaser.FloatPaser(row, "ctmNbr40"),
                    };
                }
            }
            else Debug.Log("contents has no data");
        }
        public Mine GetMine() => _mine;
        public void SetMine(JsonData returnData)
        {
            if (returnData.Keys.Contains("rows"))
            {
                JsonData row = returnData["rows"][0];
                _mine = new Mine()
                {
                    indate = RowPaser.StrPaser(row, "inDate"),
                    ctmNbr0 = RowPaser.FloatPaser(row, "ctmNbr0"),
                    ctmNbr1 = RowPaser.FloatPaser(row, "ctmNbr1"),
                    ctmNbr2 = RowPaser.FloatPaser(row, "ctmNbr2"),
                    ctmNbr3 = RowPaser.FloatPaser(row, "ctmNbr3"),
                };
            }
        }

        public Raid GetRaid() => _raid;
        public void SetRaid(JsonData returnData)
        {
            if (returnData.Keys.Contains("rows"))
            {
                JsonData row = returnData["rows"][0];
                _raid = new Raid()
                {
                    indate = RowPaser.StrPaser(row, "inDate"),
                    ctmNbr0 = RowPaser.FloatPaser(row, "ctmNbr0"),
                    ctmNbr1 = RowPaser.FloatPaser(row, "ctmNbr1"),
                    ctmNbr2 = RowPaser.FloatPaser(row, "ctmNbr2"),
                    ctmNbr3 = RowPaser.FloatPaser(row, "ctmNbr3"),
                    ctmNbr4 = RowPaser.FloatPaser(row, "ctmNbr4"),
                    ctmNbr5 = RowPaser.FloatPaser(row, "ctmNbr5"),
                    ctmNbr6 = RowPaser.FloatPaser(row, "ctmNbr6"),
                };
            }
        }

        public void SetData(IG.ModeType md_ty, string _key, float _val)
        {
            LogPrint.PrintError("md_ty : " + md_ty + ", _key : " + _key + ", _val : " + _val);
            if (md_ty == IG.ModeType.DUNGEON_TOP)
            {
                switch (_key)
                {
                    case "ctmNbr0": _top.ctmNbr0 = _val; break;
                    case "ctmNbr1": _top.ctmNbr1 = _val; break;
                    case "ctmNbr2": _top.ctmNbr2 = _val; break;
                    case "ctmNbr3": _top.ctmNbr3 = _val; break;
                    case "ctmNbr4": _top.ctmNbr4 = _val; break;
                    case "ctmNbr5": _top.ctmNbr5 = _val; break;
                    case "ctmNbr6": _top.ctmNbr6 = _val; break;
                    case "ctmNbr7": _top.ctmNbr7 = _val; break;
                    case "ctmNbr8": _top.ctmNbr8 = _val; break;
                    case "ctmNbr9": _top.ctmNbr9 = _val; break;
                    case "ctmNbr10": _top.ctmNbr10 = _val; break;
                    case "ctmNbr11": _top.ctmNbr11 = _val; break;
                    case "ctmNbr12": _top.ctmNbr12 = _val; break;
                    case "ctmNbr13": _top.ctmNbr13 = _val; break;
                    case "ctmNbr14": _top.ctmNbr14 = _val; break;
                    case "ctmNbr15": _top.ctmNbr15 = _val; break;
                    case "ctmNbr16": _top.ctmNbr16 = _val; break;
                    case "ctmNbr17": _top.ctmNbr17 = _val; break;
                    case "ctmNbr18": _top.ctmNbr18 = _val; break;
                    case "ctmNbr19": _top.ctmNbr19 = _val; break;
                    case "ctmNbr20": _top.ctmNbr20 = _val; break;
                    case "ctmNbr21": _top.ctmNbr21 = _val; break;
                    case "ctmNbr22": _top.ctmNbr22 = _val; break;
                    case "ctmNbr23": _top.ctmNbr23 = _val; break;
                    case "ctmNbr24": _top.ctmNbr24 = _val; break;
                    case "ctmNbr25": _top.ctmNbr25 = _val; break;
                    case "ctmNbr26": _top.ctmNbr26 = _val; break;
                    case "ctmNbr27": _top.ctmNbr27 = _val; break;
                    case "ctmNbr28": _top.ctmNbr28 = _val; break;
                    case "ctmNbr29": _top.ctmNbr29 = _val; break;
                    case "ctmNbr30": _top.ctmNbr30 = _val; break;
                    case "ctmNbr31": _top.ctmNbr31 = _val; break;
                    case "ctmNbr32": _top.ctmNbr32 = _val; break;
                    case "ctmNbr33": _top.ctmNbr33 = _val; break;
                    case "ctmNbr34": _top.ctmNbr34 = _val; break;
                    case "ctmNbr35": _top.ctmNbr35 = _val; break;
                    case "ctmNbr36": _top.ctmNbr36 = _val; break;
                    case "ctmNbr37": _top.ctmNbr37 = _val; break;
                    case "ctmNbr38": _top.ctmNbr38 = _val; break;
                    case "ctmNbr39": _top.ctmNbr39 = _val; break;
                    case "ctmNbr40": _top.ctmNbr40 = _val; break;
                }
            }
            else if (md_ty == IG.ModeType.DUNGEON_MINE)
            {
                switch (_key)
                {
                    case "ctmNbr0": _mine.ctmNbr0 = _val; break;
                    case "ctmNbr1": _mine.ctmNbr1 = _val; break;
                    case "ctmNbr2": _mine.ctmNbr2 = _val; break;
                    case "ctmNbr3": _mine.ctmNbr3 = _val; break;
                }
            }
            else if (md_ty == IG.ModeType.DUNGEON_RAID)
            {
                switch (_key)
                {
                    case "ctmNbr0": _raid.ctmNbr0 = _val; break;
                    case "ctmNbr1": _raid.ctmNbr1 = _val; break;
                    case "ctmNbr2": _raid.ctmNbr2 = _val; break;
                    case "ctmNbr3": _raid.ctmNbr3 = _val; break;
                    case "ctmNbr4": _raid.ctmNbr4 = _val; break;
                    case "ctmNbr5": _raid.ctmNbr5 = _val; break;
                    case "ctmNbr6": _raid.ctmNbr6 = _val; break;
                }
            }
        }

        /// <summary>
        /// 각 던전 클리어 타임 
        /// </summary>
        public float GetClearTime(IG.ModeType mdty, int nbr)
        {
            if (mdty == IG.ModeType.DUNGEON_TOP)
            {
                float sco = GetTopClearTime(nbr);
                return sco > 0 ? RankDB.max_time - sco : 0;
            }
            else if (mdty == IG.ModeType.DUNGEON_MINE)
            {
                float sco = GetMineClearTime(nbr);
                return sco > 0 ? RankDB.max_time - sco : 0;
            }
            else if (mdty == IG.ModeType.DUNGEON_RAID)
            {
                float sco = GetRaidClearTime(nbr);
                return sco > 0 ? RankDB.max_time - sco : 0;
            }

            return 0f;
        }

        float GetTopClearTime(int nbr)
        {
            switch (nbr)
            {
                case 0: return _top.ctmNbr0;
                case 1: return _top.ctmNbr1;
                case 2: return _top.ctmNbr2;
                case 3: return _top.ctmNbr3;
                case 4: return _top.ctmNbr4;
                case 5: return _top.ctmNbr5;
                case 6: return _top.ctmNbr6;
                case 7: return _top.ctmNbr7;
                case 8: return _top.ctmNbr8;
                case 9: return _top.ctmNbr9;
                case 10: return _top.ctmNbr10;
                case 11: return _top.ctmNbr11;
                case 12: return _top.ctmNbr12;
                case 13: return _top.ctmNbr13;
                case 14: return _top.ctmNbr14;
                case 15: return _top.ctmNbr15;
                case 16: return _top.ctmNbr16;
                case 17: return _top.ctmNbr17;
                case 18: return _top.ctmNbr18;
                case 19: return _top.ctmNbr19;
                case 20: return _top.ctmNbr20;
                case 21: return _top.ctmNbr21;
                case 22: return _top.ctmNbr22;
                case 23: return _top.ctmNbr23;
                case 24: return _top.ctmNbr24;
                case 25: return _top.ctmNbr25;
                case 26: return _top.ctmNbr26;
                case 27: return _top.ctmNbr27;
                case 28: return _top.ctmNbr28;
                case 29: return _top.ctmNbr29;
                case 30: return _top.ctmNbr30;
                case 31: return _top.ctmNbr31;
                case 32: return _top.ctmNbr32;
                case 33: return _top.ctmNbr33;
                case 34: return _top.ctmNbr34;
                case 35: return _top.ctmNbr35;
                case 36: return _top.ctmNbr36;
                case 37: return _top.ctmNbr37;
                case 38: return _top.ctmNbr38;
                case 39: return _top.ctmNbr39;
                case 40: return _top.ctmNbr40;
            }
            return 0;
        }

        float GetMineClearTime(int nbr)
        {
            switch (nbr)
            {
                case 0: return _mine.ctmNbr0;
                case 1: return _mine.ctmNbr1;
                case 2: return _mine.ctmNbr2;
                case 3: return _mine.ctmNbr3;
            }
            return 0;
        }

        float GetRaidClearTime(int nbr)
        {
            switch (nbr)
            {
                case 0: return _raid.ctmNbr0;
                case 1: return _raid.ctmNbr1;
                case 2: return _raid.ctmNbr2;
                case 3: return _raid.ctmNbr3;
                case 4: return _raid.ctmNbr4;
                case 5: return _raid.ctmNbr5;
                case 6: return _raid.ctmNbr6;
            }
            return 0;
        }

        string GetInDate(IG.ModeType md_ty)
        {
            if (md_ty == IG.ModeType.DUNGEON_TOP)
                return _top.indate;
            else if (md_ty == IG.ModeType.DUNGEON_MINE)
                return _mine.indate;
            else if (md_ty == IG.ModeType.DUNGEON_RAID)
                return _raid.indate;

            return string.Empty;
        }

#endregion

        /// <summary>
        /// 각 던전 클리어 타임 전송 
        /// </summary>
        public async Task ASendScoreData(IG.ModeType md_ty, string key, float val)
        {
            LogPrint.Print("<color=red> ASendScoreData </color>");
            string tblName = 
                md_ty == IG.ModeType.DUNGEON_TOP ? BackendGpgsMng.tableName_DungeonTop : 
                md_ty == IG.ModeType.DUNGEON_MINE ? BackendGpgsMng.tableName_DungeonMine :
                md_ty == IG.ModeType.DUNGEON_RAID ? BackendGpgsMng.tableName_DungeonRaid : string.Empty;
            if(!string.IsNullOrEmpty(tblName))
            {
                float score = GameDatabase.RankDB.max_time - val;
                if(score > 0)
                {
                    Param p = new Param();
                    p.Add(key, score);
                    LogPrint.Print("tblName : " + tblName +", md_ty : " + md_ty + ", key : " + key + ", val : " +val + ", param : " + p.GetJson());
                    await BackendGpgsMng.GetInstance().TaskUpdateTableData(tblName, GetInDate(md_ty), p);
                    SetData(md_ty, key, score);
                }
            }
        }

        // 던전 클리어 랭크 
        List<RankTime> rankTime = new List<RankTime>(); struct RankTime { public float time; public string rank; }

        /// <summary> 던전 클리어 랭크 </summary>
        public string GetClearRank(float _clrSec, string dgName)
        {
            if (_clrSec <= 0)
                return "-";

            //string dg_ty = MainUI.GetInstance().tapDungeon.GetDungeonTypeStr();
            LogPrint.Print(string.Format("dg.clear.rank.{0}", dgName));
            string valStrRankTime = GameDatabase.GetInstance().chartDB.GetDicBalance(string.Format("dg.clear.rank.{0}", dgName)).val_string;
            var sRankTime = JSONObject.Create(valStrRankTime);
            LogPrint.Print("valStrRankTime : " + valStrRankTime + ", sRankTime : " + sRankTime);
            rankTime = new List<RankTime>()
            {
               JsonUtility.FromJson<RankTime>(sRankTime.list[0]["rank_time"][0].ToString()),
               JsonUtility.FromJson<RankTime>(sRankTime.list[0]["rank_time"][1].ToString()),
               JsonUtility.FromJson<RankTime>(sRankTime.list[0]["rank_time"][2].ToString()),
               JsonUtility.FromJson<RankTime>(sRankTime.list[0]["rank_time"][3].ToString()),
            };

            foreach (var item in rankTime)
            {
                if (_clrSec <= item.time)
                {
                    return item.rank.ToString();
                }
            }

            return string.Empty;
        }
    }

    /// <summary>
    /// ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
    /// #출석부 데이터 
    /// ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
    /// </summary>
    public class AttendanceDB : ChartDB
    {
        public string DailyYmd() => BackendGpgsMng.GetInstance().GetNowTime().ToString("yyyy/MM/dd");

        /// <summary> 출석 보상 받을 수 있는지 체크 </summary>
        public async Task<bool> GetIsCheckAttendance()
        {
            var tResult = await BackendGpgsMng.GetInstance().GetBackendTime();
            var nwDate = DateTime.Parse(tResult);
            DateTime day_ymd = DateTime.Parse(nwDate.ToString("yyyy/MM/dd"));
            string str_old_ymd = GetInstance().tableDB.GetUserInfo().m_attend_ymd;

            try 
            {
                DateTime old_ymd = DateTime.Parse(str_old_ymd);
                int result = DateTime.Compare(old_ymd, day_ymd);
                NotificationIcon.GetInstance().CheckNoticeAttend(result < 0 && result != 0);
                return result < 0 && result != 0;
            }
            catch (Exception) 
            { 
                LogPrint.Print("첫 출석이므로 보상 받을 수 있음");
                NotificationIcon.GetInstance().CheckNoticeAttend(true);
                return true;
            }
        }

        /// <summary> 보상 받기 확인 </summary>
        public async Task SetConfirmAttendance()
        {
            var uinfo = GetInstance().tableDB.GetUserInfo();
            int rwd_nbr = (int)(uinfo.m_attend_nbr % 30);
            uinfo.m_attend_ymd = DailyYmd().ToString();
            uinfo.m_attend_nbr++;

            Task tsk1 = GetInstance().tableDB.SetUpdate_UserInfo(uinfo);
            while (Loading.Bottom(tsk1.IsCompleted) == false) await Task.Delay(100);

            await GetInstance().dailyMissionDB.ASetInCount(DailyMissionDB.Nbr.nbr0, 1); // 일일미션, nbr0 오늘 출석 체크 하기 
            await GetInstance().mailDB.ASetGiftRewarded(GetFindNbrCdb(rwd_nbr).item);
        }

        public cdb_attendance_book GetFindIndexCdb(int indx)
        {
            try { return list_cdb_attendance_book[indx]; }
            catch (Exception) { return default; }
        }

        public cdb_attendance_book GetFindNbrCdb(int nbr)
        {
            try 
            {
                return list_cdb_attendance_book.Find(x => x.nbr == nbr);
            }
            catch (Exception) { return default; }
        }

        public int GetCount() => list_cdb_attendance_book.Count;
    }

    /// <summary>
    /// ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
    /// #메일 데이터 
    /// ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
    /// </summary>
    public class MailDB : ChartDB
    {
        private List<Data> mail_data = new List<Data>();
        [System.Serializable]
        public struct Data
        {
            public bool isFromAdmin; // true : 관리자가 보낸 우편이다., false : 유저가 보낸 우편이다. 
            public string indate;
            public string expirationDate; // // 만료 일시
            public string senderNickname; // 발신인(보낸사람) 
            public string title; // 우편 제목 
            public string content; // 우편 내용 
            
            public Item item;
        }

        [System.Serializable]
        public struct Item
        {
            public string item_name;
            public string gift_type; // 보상 아이템 타입, cash, equip, item, skill ......
            public int ty;
            public int rt;
            public int idx;     // 스킬 idx 
            public int count;   // 수량 
        }

        /// <summary> 우편함을 뒤끝에서 받아옴 </summary>
        public async Task AGetAll()
        {
            mail_data.Clear();
            JsonData jd = await BackendGpgsMng.GetInstance().AGetMail();
            LogPrint.Print("jd : " + jd.ToJson());

            if(jd["fromAdmin"].Count > 0)
            {
                foreach (var fromKey in jd.Keys)
                {
                    foreach (JsonData row in jd[fromKey])
                    {
                        JsonData jd_item = row["item"]["M"];
                        Data data = new Data()
                        {
                            isFromAdmin = string.Equals(fromKey, "fromAdmin"),
                            indate = RowPaser.StrPaser(row, "inDate"),
                            expirationDate = RowPaser.StrPaser(row, "expirationDate"),
                            senderNickname = string.Equals(fromKey, "fromAdmin") ? "Admin" : RowPaser.StrPaser(row, "senderNickname"),
                            title = RowPaser.StrPaser(row, "title"),
                            content = RowPaser.StrPaser(row, "content"),
                            
                            item = new Item()
                            {
                                gift_type = RowPaser.StrPaser(jd_item, "gift_type"),
                                ty = RowPaser.IntPaser(jd_item, "ty"),
                                rt = RowPaser.IntPaser(jd_item, "rt"),
                                idx = RowPaser.IntPaser(jd_item, "idx"),
                                count = RowPaser.IntPaser(row, "itemCount"),
                                item_name = RowPaser.StrPaser(jd_item, "item_name"),
                            }
                        };

                        mail_data.Add(data);
                    }
                }
            }
        }


        /// <summary> 보상 받기 버튼 누름 </summary>
        public async Task AGetRewarded(string indate)
        {
            LogPrint.EditorPrint("----------------A GetRewarded indate : " + indate);
            BackendReturnObject bro1 = null;
            SendQueue.Enqueue(Backend.Social.Post.ReceiveAdminPostItemV2, indate, callback => { bro1 = callback; });
            while (bro1 == null) { await Task.Delay(100); }
            LogPrint.EditorPrint("----------------A GetRewarded indate : " + indate + ", _bro1 : " + bro1);
            
            if (bro1.IsSuccess())
            {
                int delete_indx = mail_data.FindIndex(x => x.indate == indate);
                if (delete_indx >= 0)
                {
                    await ASetGiftRewarded(mail_data[delete_indx].item);
                    mail_data.RemoveAt(delete_indx);
                }
                else LogPrint.Print("삭제할 메일이 없음.");
            }

            NotificationIcon.GetInstance().CheckNoticeMail(mail_data.Count > 0);
        }

        /// <summary> 전체 보상 받기 버튼 누름 </summary>
        public async Task AGetAllRewarded()
        {
            LogPrint.Print("----------------A GetAllRewarded");
            BackendReturnObject bro1 = null;
            SendQueue.Enqueue(Backend.Social.Post.ReceiveAdminPostAllV2, callback => { bro1 = callback; });
            while (bro1 == null) { await Task.Delay(100); }
            LogPrint.Print("----------------A GetAllRewarded indate _bro1 : " + bro1);

            List<Item> allRewardItems = new List<Item>();
            if (bro1.IsSuccess())
            {
                mail_data.Clear();
                foreach (JsonData row in bro1.GetReturnValuetoJSON()["items"])
                {
                    JsonData jd_item = row["item"]["M"];
                    string _gift_type = RowPaser.StrPaser(jd_item, "gift_type");
                    if (string.IsNullOrEmpty(_gift_type) == false)
                    {
                        int ovrp_indx = allRewardItems.FindIndex(obj => string.Equals(obj.gift_type, _gift_type) &&
                                                                    int.Equals(obj.ty, RowPaser.IntPaser(jd_item, "ty")) &&
                                                                    int.Equals(obj.rt, RowPaser.IntPaser(jd_item, "rt")) &&
                                                                    int.Equals(obj.idx, RowPaser.IntPaser(jd_item, "idx")));

                        if (ovrp_indx >= 0)
                        {
                            var tmp_item = allRewardItems[ovrp_indx];
                            tmp_item.count += RowPaser.IntPaser(row, "itemCount");
                            allRewardItems[ovrp_indx] = tmp_item;
                        }
                        else
                        {
                            allRewardItems.Add
                            (
                                new Item()
                                {
                                    gift_type = RowPaser.StrPaser(jd_item, "gift_type"),
                                    ty = RowPaser.IntPaser(jd_item, "ty"),
                                    rt = RowPaser.IntPaser(jd_item, "rt"),
                                    idx = RowPaser.IntPaser(jd_item, "idx"),
                                    count = RowPaser.IntPaser(row, "itemCount"),
                                    item_name = RowPaser.StrPaser(jd_item, "item_name"),
                                }
                            );
                        }
                    }
                }

                foreach (var item in allRewardItems)
                {
                    await ASetGiftRewarded(item, true);
                }
            }

            NotificationIcon.GetInstance().CheckNoticeMail(mail_data.Count > 0);
        }

        /// <summary>
        /// 보상 전송 (메일, 출석 ,,,,,) 
        /// </summary>
        /// <returns></returns>
        public async Task ASetGiftRewarded(Item m_item, bool isMailAllReward = false)
        {
            long rwd_cnt = 0;
            string successNoticeText = "";
            if (string.Equals(m_item.gift_type, "user_info"))
            {
                switch (m_item.ty)
                {
                    case 31:// 편의 기능 
                        rwd_cnt = m_item.count;
                        Task<bool> task = GameDatabase.GetInstance().tableDB.AutoSaleAddDay(m_item.count);
                        while (Loading.Full(task.IsCompleted) == false) await Task.Delay(100);

                        if (task.Result == true)
                        {
                            ConvenienceFunctionMng.GetInstance().InitConvenienceAutoSale();
                            if (GameDatabase.GetInstance().convenienceFunctionDB.GetUseingConvenFunAutoSale())
                                PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox(string.Format("편의 기능 : 장비 판매/분해 <color=yellow>{0}일</color> 이용권을 추가 완료하였습니다.", rwd_cnt));
                            else
                                PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox(string.Format("편의 기능 : 장비 판매/분해 <color=yellow>{0}일</color> 이용권을 구매 완료하였습니다.", rwd_cnt));

                            await Task.Delay(300);
                            if (ConvenienceFunctionMng.GetInstance().convenFun.cfAutoSale.onOff == IG.ConvenienceFunction.OnOff.OFF)
                                PopUpMng.GetInstance().Open_AutoSale();
                        }
                        break;
                }
            }
            else if (string.Equals(m_item.gift_type, "item"))
            {
                var item = GameDatabase.GetInstance().tableDB.GetItem(m_item.ty, m_item.rt);
                item.count += m_item.count;
                rwd_cnt = m_item.count;
                Task task = GetInstance().tableDB.SendDataItem(item);
                while (Loading.Full(task.IsCompleted) == false) await Task.Delay(100);

                switch (item.type)
                {
                    case 20: // 물약 
                    case 21: // 장비 강화석 
                    case 22: // 강화 축복 주문서 
                    case 23: // 입장권 :던전 알수없는 탑 
                    case 24: // 입장권 :던전 광산 
                    case 25: // 입장권 : 던전 레이드 
                    case 26: // 부활석 (X) 
                    case 27: // 장신구 강화석 
                    case 28: // 장비 조각 
                    case 29: // 장신구 조각 
                    case 30: // 입장권 : PVP 배틀 아레나
                    case 31: // 펫 소환 고급, 희귀, 영웅 
                        if (item.type == 20)
                            ConvenienceFunctionMng.GetInstance().UIConvenienceAutoPosion();

                        if(int.Equals(item.type, 23) || int.Equals(item.type, 24) || int.Equals(item.type, 25) || int.Equals(item.type, 30))
                        {
                            if(MainUI.GetInstance().tapDungeon.gameObject.activeSelf)
                                MainUI.GetInstance().tapDungeon.Ticket();
                        } 

                        if (MainUI.GetInstance().inventory.gameObject.activeSelf)
                            MainUI.GetInstance().inventory.initOnStartInventoryAll.SetInit(true);
                        break;
                }

                successNoticeText = string.Format(LanguageGameData.GetInstance().GetString("string.format.one.type.reward.confirm"), m_item.item_name, rwd_cnt);
            }
            else if (string.Equals(m_item.gift_type, "goods"))
            {
                var goods = GetInstance().tableDB.GetTableDB_Goods();
                switch (m_item.ty)
                {
                    case 10: 
                        goods.m_gold += GameDatabase.GetInstance().questDB.GetQuestMaxSecondRewardGold(m_item.count);
                        rwd_cnt = GameDatabase.GetInstance().questDB.GetQuestMaxSecondRewardGold(m_item.count);
                        break;
                    case 11:
                        goods.m_dia += m_item.count;
                        rwd_cnt = m_item.count;
                        break;
                    case 12:
                        goods.m_ether += m_item.count;
                        rwd_cnt = m_item.count;
                        break;
                    case 13:
                        goods.m_ruby += m_item.count;
                        rwd_cnt = m_item.count;
                        break;
                }

                Task task = GetInstance().tableDB.SetUpdateGoods(goods);
                while (Loading.Full(task.IsCompleted) == false) await Task.Delay(100);
                successNoticeText = string.Format(LanguageGameData.GetInstance().GetString("string.format.one.type.reward.confirm"), m_item.item_name, rwd_cnt);
            }
            else if (string.Equals(m_item.gift_type, "skill"))
            {
                var skill = GetInstance().tableDB.GetSkill(m_item.idx);
                if(skill.idx > 0)
                {
                    skill.count += m_item.count;
                    rwd_cnt = m_item.count;
                    Task task = GetInstance().tableDB.SendDataSkill(skill);
                    while (Loading.Full(task.IsCompleted) == false) await Task.Delay(100);
                }
                successNoticeText = string.Format(LanguageGameData.GetInstance().GetString("string.format.one.type.reward.confirm"), m_item.item_name, rwd_cnt);
            }
            else if (string.Equals(m_item.gift_type, "chest"))
            {

            }
            else PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("출석 보상 아이템의 타입이 잘못되었습니다.");

            if (string.Equals(successNoticeText, "") == false && isMailAllReward == false)
                PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox(successNoticeText);
        }

        public Data GetFindIndex(int indx) => mail_data[indx];

        public int GetCount() => mail_data.Count;
    }

    /// <summary>
    /// ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
    /// #랭킹 데이터 
    /// ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
    /// </summary>
    public class RankDB
    {
        public const float max_time = 300;
        struct DungeonRankLoad
        {
            public int nbr;
            public int hour_key;
            public bool isLoadSuccess;
        }

        /// <summary>
        /// 어느 시점에 보낼 수 있도록 획득 점수 세이브 
        /// </summary>
        public void ChapterBossKillAddPvPScoreSave()
        {
            //int now_chpt_stg_nbr = GameDatabase.GetInstance().tableDB.GetUserInfo().GetChapterStageNbr();
            //int v = GameDatabase.GetInstance().tableDB.GetUserInfo().GetRankChapterNbr(now_chpt_stg_nbr);
            //int indx = _chartDB.list_cdb_chpt_mnst_stat.FindIndex(x => x.chpt_dvs_nbr == v);
            //if (indx >= 0)
            //{
            //    _userInfo.m_chpt_stg_nbr = (int)(list_cdb_chpt_mnst_stat[indx].chpt_dvs_nbr * 10);
            //}

            int last_chpt_dvs_nbr = GameDatabase.GetInstance().chartDB.GetChapterLastDvsNbr();
            int now_chpt_dvs_nbr = GameDatabase.GetInstance().tableDB.GetUserInfo().GetChapterDvsNbr();
            if (now_chpt_dvs_nbr < last_chpt_dvs_nbr)
            {
                var userinfo_db = GameDatabase.GetInstance().tableDB.GetUserInfo();
                userinfo_db.m_pvp_score += 5;
                userinfo_db.m_pvp_today_score += 5;
                GameDatabase.GetInstance().tableDB.UpdateClientDB(userinfo_db);
            }
        }

        //############################################################
        #region # 선언 #
        private string RankName_RT_ChptBoss = "RT_ChptBoss"; // 챕터 보스 매칭용
        private string RankName_RT_ChptStgTop50 = "RT_ChptStgTop50"; // 실시간, 스테이지 랭킹 
        private string RankName_RT_PvpTop100 = "RT_PvpTop100"; // 실시간, PvP 랭킹 

        private int rankLast_RT_ChptBoss = -1;
        private int rankLast_RT_ChptStgTop50 = -1;

        // struct 
        private RankUUID rankUUID = new RankUUID();
        struct RankUUID
        {
            public string uuid_RT_ChptBoss;     // 실시간 챕터 보스 UUID (유저 보스 등장용)
            public string uuid_RT_ChptStgTop50; // 실시간 챕터 스테이지 TOP 50 UUID 
            public string uuid_RT_PvpTop100;     // 실시간 PvP Top50 UUID (컨텐츠 PvP 배틀 아레나에서 사용) 
        }

        // Enum 
        public Enum_RTRankType stageRankType = Enum_RTRankType.None; // 스테이지 랭킹 타입 top or around 
        

        // 실시간 랭킹 타입 
        public enum Enum_RTRankType
        {
            None,
            Rank_RT_ChptBossAround, // 챕터 스테이지 보스 유저 랭킹 (리스트는 안보여주고 스테이지의 보스시에 데이터로 사용함)
            Rank_RT_ChptStgTop50, // 실시간 챕터 스테이지 Top50 랭킹
            Rank_RT_ChptStgTop50MyAround10, // 실시간 챕터 스테이지 Top50 랭킹 (내주변 +-랭킹)
            Rank_RT_PvpTop50, // pvv 점수 top 50 
            Rank_RT_PvpTop50MyAround10,// pvv 점수 top 50 (내주변 +-랭킹)
        }

        public Dictionary<Enum_RTRankType, List<RankInfo>> m_Dic_RTRank = new Dictionary<Enum_RTRankType, List<RankInfo>>();
        [System.Serializable]
        public struct RankInfo
        {
            public string gamer_indate;
            public bool isMyself;
            public int rank;
            public string nickName;
            public float score;

            public bool isAI;
            public int ai_cpht_dvs_nbr;
        }
#endregion

        //############################################################
#region # 랭킹 리스트 UUID 세팅 및 랭킹 딕셔너리 초기화 진행 #
        /// <summary> 실시간 랭킹 리스트, uuid 및 딕셔너리 세팅</summary>
        public void CSetRTRankList(JsonData jdRTRank)
        {
            foreach (JsonData row in jdRTRank["rows"])
            {
                string title = RowPaser.StrPaser(row, "title");
                string uuid = RowPaser.StrPaser(row, "uuid");
                if (string.Equals(title, RankName_RT_ChptBoss))
                {
                    rankUUID.uuid_RT_ChptBoss = uuid;
                    m_Dic_RTRank[Enum_RTRankType.Rank_RT_ChptBossAround] = new List<RankInfo>();
                }
                else if (string.Equals(title, RankName_RT_ChptStgTop50))
                {
                    rankUUID.uuid_RT_ChptStgTop50 = uuid;
                    m_Dic_RTRank[Enum_RTRankType.Rank_RT_ChptStgTop50] = new List<RankInfo>();
                    m_Dic_RTRank[Enum_RTRankType.Rank_RT_ChptStgTop50MyAround10] = new List<RankInfo>();

                }
                else if (string.Equals(title, RankName_RT_PvpTop100))
                {
                    rankUUID.uuid_RT_PvpTop100 = uuid;
                    m_Dic_RTRank[Enum_RTRankType.Rank_RT_PvpTop50] = new List<RankInfo>();
                    m_Dic_RTRank[Enum_RTRankType.Rank_RT_PvpTop50MyAround10] = new List<RankInfo>();

                    m_Dic_RTRankPvPBattleArena[Enum_RTRankType.Rank_RT_PvpTop50] = new List<RankInfo>();
                    m_Dic_RTRankPvPBattleArena[Enum_RTRankType.Rank_RT_PvpTop50MyAround10] = new List<RankInfo>();
                }
            }
        }
#endregion

        //############################################################
#region # 랭킬 전송 #

        /// <summary> 실시간 랭킹, 챕터 스테이지 클리어 등록 (유저 보스 사용 전용) </summary>
        public void ASetRTRank_RT_ChptBoss()
        {
            // 랭킹 등록 전송 
            var uInfo = GetInstance().tableDB.GetUserInfo();
            if (rankLast_RT_ChptBoss != uInfo.GetChapterDvsNbr())
            {
                rankLast_RT_ChptBoss = uInfo.GetChapterDvsNbr();
                BackendReturnObject bro1 = null;
                SendQueue.Enqueue(Backend.GameInfo.UpdateRTRankTable,BackendGpgsMng.tableName_UserInfo, "m_clar_chpt", (long)uInfo.GetChapterDvsNbr(), uInfo.indate, callback => { bro1 = callback; });
            }
        }
        /// <summary> 실시간 랭킹, 챕터 스테이지 등록 </summary>
        public void ASetRTRank_RT_ChptStgTop50()
        {
            // 랭킹 등록 전송 
            var uInfo = GetInstance().tableDB.GetUserInfo();
            if (rankLast_RT_ChptStgTop50 != uInfo.m_chpt_stg_nbr)
            {
                rankLast_RT_ChptStgTop50 = uInfo.m_chpt_stg_nbr;
                BackendReturnObject bro1 = null;
                SendQueue.Enqueue(Backend.GameInfo.UpdateRTRankTable, BackendGpgsMng.tableName_UserInfo, "m_chpt_stg_nbr", (long)uInfo.m_chpt_stg_nbr, uInfo.indate, callback => { bro1 = callback; });
            }
        }

        private long lastPvpSendScore = -1;
        private long lastPvpToDaySendScore = -1;
        /// <summary> 실시간 랭킹, Pvp 점수 등록 </summary>
        public async Task ASendRTRank_PvpTop100(int pvp_win_streak, int pvpScore, int pvpToDayScore)
        {
            LogPrint.Print("<color=magenta> 11111 </color> 실시간 누적 랭킹, Pvp 점수 등록 lastPvpSendScore : " + lastPvpSendScore + ", pvpScore : " + pvpScore);
            LogPrint.Print("<color=magenta> 11111 </color> 실시간 일일 랭킹, Pvp 점수 등록 lastPvpToDaySendScore : " + lastPvpToDaySendScore + ", pvpToDayScore : " + pvpToDayScore);
            var userInfo_db = GetInstance().tableDB.GetUserInfo();
            if (pvpScore < 0)
                pvpScore = 0;
            if (pvpToDayScore < 0)
                pvpToDayScore = 0;

            BackendReturnObject broA = null, broB = null;
            if (lastPvpSendScore != pvpScore)
                SendQueue.Enqueue(Backend.GameInfo.UpdateRTRankTable, BackendGpgsMng.tableName_UserInfo, "m_pvp_score", (long)pvpScore, userInfo_db.indate, callback => { broA = callback; });
            else broB = default;

            if (lastPvpToDaySendScore != pvpToDayScore)
                SendQueue.Enqueue(Backend.GameInfo.UpdateRTRankTable, BackendGpgsMng.tableName_UserInfo, "m_pvp_today_score", (long)pvpToDayScore, userInfo_db.indate, callback => { broB = callback; });
            else broB = default;

            GetInstance().publicContentDB.ASetPub_CharData(BackendGpgsMng.tableName_Pub_NowCharData); // 캐릭터 데이터 전송 

            if(pvp_win_streak != -1)
                userInfo_db.m_pvp_win_streak = pvp_win_streak;

            userInfo_db.m_pvp_score = pvpScore;
            userInfo_db.m_pvp_today_score = pvpToDayScore;
            userInfo_db.m_pvp_in = false;
            Task tsk1 = GameDatabase.GetInstance().tableDB.SetUpdate_UserInfo(userInfo_db);
            while (Loading.Bottom(broA, broB) == false && Loading.Bottom(tsk1.IsCompleted) == false) await Task.Delay(100);
            while (broA == null || broB == null || tsk1.IsCompleted == false) await Task.Delay(100);

            LogPrint.Print("<color=yellow> 11111 </color> 실시간 누적 랭킹, Pvp 점수 등록 lastPvpSendScore : " + lastPvpSendScore + ", pvpScore : " + pvpScore);
            LogPrint.Print("<color=yellow> 11111 </color> 실시간 일일 랭킹, Pvp 점수 등록 lastPvpToDaySendScore : " + lastPvpToDaySendScore + ", pvpToDayScore : " + pvpToDayScore);
            
            if (lastPvpSendScore != pvpScore)
                lastPvpSendScore = pvpScore;

            if (lastPvpToDaySendScore != pvpToDayScore)
                lastPvpToDaySendScore = pvpToDayScore;
        }
#endregion

        // #########################################################################################################
#region # 실시간 랭킹 로드 #
        /// <summary> 실시간 랭킹 정보 로드 </summary>
        public async Task AGetRTRank(Enum_RTRankType rank_type)
        {
            string rank_uuid = string.Empty;
            Enum_RTRankType[] rank_topOrAround = new Enum_RTRankType[2];
            if (rank_type == Enum_RTRankType.Rank_RT_ChptStgTop50 || rank_type == Enum_RTRankType.Rank_RT_ChptStgTop50MyAround10)
            {
                rank_uuid = rankUUID.uuid_RT_ChptStgTop50;
                rank_topOrAround = new Enum_RTRankType[] { Enum_RTRankType.Rank_RT_ChptStgTop50, Enum_RTRankType.Rank_RT_ChptStgTop50MyAround10 };
            }
            else if (rank_type == Enum_RTRankType.Rank_RT_PvpTop50 || rank_type == Enum_RTRankType.Rank_RT_PvpTop50MyAround10)
            {
                rank_uuid = rankUUID.uuid_RT_PvpTop100;
                rank_topOrAround = new Enum_RTRankType[] { Enum_RTRankType.Rank_RT_PvpTop50, Enum_RTRankType.Rank_RT_PvpTop50MyAround10 };
            }

            if (string.IsNullOrEmpty(rank_uuid))
                return;

            foreach (var item in rank_topOrAround)
                m_Dic_RTRank[item].Clear();

            BackendReturnObject[] _bro = { null, null };
            LogPrint.Print("----------------A GetRTRank ");
            SendQueue.Enqueue(Backend.RTRank.GetRTRankByUuid, rank_uuid, 50, callback => { _bro[0] = callback; });
            SendQueue.Enqueue(Backend.RTRank.GetMyRTRank, rank_uuid, 5, callback => { _bro[1] = callback; });
            while (_bro[0] == null || _bro[1] == null) { await Task.Delay(100); }
            LogPrint.Print("----------------A GetRTRank Top50 rank_type : " + rank_type +", _bro : " + _bro[0]);
            LogPrint.Print("----------------A GetRTRank Around  rank_type : " + rank_type + ", _bro : " + _bro[1]);

            for (int i = 0; i < _bro.Length; i++)
            {
                if (_bro[i].IsSuccess())
                {
                    JsonData jdRows = _bro[i].GetReturnValuetoJSON()["rows"];
                    if (jdRows != null)
                    {
                        foreach (JsonData row in jdRows)
                        {
                            float _score = RowPaser.FloatPaser(row, "score");
                            if (_score > 0)
                            {
                                m_Dic_RTRank[rank_topOrAround[i]].Add(
                                new RankInfo()
                                {
                                    gamer_indate = row["gamerInDate"].ToString(),
                                    isMyself = string.Equals(row["nickname"].ToString(), BackendGpgsMng.backendUserInfo.m_nickname),
                                    nickName = row["nickname"].ToString(),
                                    rank = RowPaser.IntPaser(row, "rank"),
                                    score = _score
                                });
                            }
                        }
                    }
                }
            }
        }

        /// <summary> 실시간, 클라이언트에 저장되어있는 타입별 랭킹 순위 리턴 </summary>
        public List<RankInfo> GetDicRTRank(Enum_RTRankType rtt = Enum_RTRankType.None)
        {
            if (m_Dic_RTRank.TryGetValue(rtt == Enum_RTRankType.None ? stageRankType : rtt, out List<RankInfo> ba))
                return ba;

            return null;
        }

        /// <summary>
        /// 스테이지 자신의 랭킹 
        /// </summary>
        public int GetMyStageRank()
        {
            if (m_Dic_RTRank.TryGetValue(Enum_RTRankType.Rank_RT_ChptStgTop50MyAround10, out List<RankInfo> ba))
                return ba.Find(x => x.isMyself).rank;

            return -1;
        }
#endregion

        // #########################################################################################################
#region 실시간 랭킹 / 컨텐츠 PvP 배틀 아레나 
        // 컨텐츠 PvP 배틀 아레나 탭(랭킹100명, 매칭유저 100명<랜덤으로 뽑아서 mListPvpBTLMatching에 넣음)
        public Dictionary<Enum_RTRankType, List<RankInfo>> m_Dic_RTRankPvPBattleArena = new Dictionary<Enum_RTRankType, List<RankInfo>>();

        // 컨텐츠 : PvP 배틀 아레나 매칭 상대 리스트 (자신 기준 m_Dic_RTRankPvPBattleArena 의 Around키값 내의 리스트에서 랜덤으로 뽑아서 넣음)
        private List<RankInfo> mListPvpBTLMatching = new List<RankInfo>();
        public List<RankInfo> GetPvpBTLMatching() => mListPvpBTLMatching;

        /// <summary>
        /// Pvp 배틀 아레나 실시간 랭킹을 불러온다 
        /// </summary>
        /// <returns></returns>
        public async Task AGetRTRankPvpBTLArenaRank(bool getA, bool getB)
        {
            string rank_uuid = rankUUID.uuid_RT_PvpTop100;
            LogPrint.Print("----------------A GetRTRankPvpBTLArena : " + rank_uuid);
            Enum_RTRankType[] rank_topOrAround = new Enum_RTRankType[] { Enum_RTRankType.Rank_RT_PvpTop50, Enum_RTRankType.Rank_RT_PvpTop50MyAround10 };

            if (string.IsNullOrEmpty(rank_uuid))
                return;

            foreach (var item in rank_topOrAround)
            {
                if(m_Dic_RTRankPvPBattleArena.ContainsKey(item))
                    m_Dic_RTRankPvPBattleArena[item].Clear();
            }

            LogPrint.Print("----------------A GetRTRankPvpBTLArena ");

            BackendReturnObject 
                broA = getA == true ? null : new BackendReturnObject(),
                broB = getB == true ? null : new BackendReturnObject();

            SendQueue.Enqueue(Backend.RTRank.GetRTRankByUuid, rank_uuid, 100, callback => { broA = callback; });
            SendQueue.Enqueue(Backend.RTRank.GetMyRTRank, rank_uuid, 100, callback => { broB = callback; });

            while (Loading.Bottom(broA, broB) == false) { await Task.Delay(100); }

            LogPrint.EditorPrint("----------------A GetRTRankPvpBTLArena Top broA : " + broA);
            LogPrint.EditorPrint("----------------A GetRTRankPvpBTLArena Around  _bro : " + broB);

            for (int i = 0; i < 2; i++)
            {
                BackendReturnObject bro = i == 0 ? broA : broB;
                if (bro != null && bro.IsSuccess())
                {
                    JsonData jdRows = bro.GetReturnValuetoJSON()["rows"];
                    if (jdRows != null)
                    {
                        foreach (JsonData row in jdRows)
                        {
                            float _score = RowPaser.FloatPaser(row, "score");
                            if (_score > 0)
                            {
                                m_Dic_RTRankPvPBattleArena[rank_topOrAround[i]].Add(
                                new RankInfo()
                                {
                                    gamer_indate = row["gamerInDate"].ToString(),
                                    isMyself = string.Equals(row["nickname"].ToString(), BackendGpgsMng.backendUserInfo.m_nickname),
                                    nickName = row["nickname"].ToString(),
                                    rank = RowPaser.IntPaser(row, "rank"),
                                    score = _score
                                });
                            }
                        }
                    }
                }
            }
        }

        /// <summary> 컨텐츠 : 결투 자신의 랭킹 +-50중 랜덤으로 20명 매칭 상대 세팅 </summary>
        public async Task AGetRTRankPvBTLArenaMatchingList()
        {
            int matching_user_cnt = 20; // 매칭 상대 유저 카운트 
            string rank_uuid = rankUUID.uuid_RT_PvpTop100;
            if (string.IsNullOrEmpty(rank_uuid))
                return;

            if (m_Dic_RTRankPvPBattleArena.ContainsKey(Enum_RTRankType.Rank_RT_PvpTop50MyAround10))
                m_Dic_RTRankPvPBattleArena[Enum_RTRankType.Rank_RT_PvpTop50MyAround10].Clear();

            LogPrint.Print("----------------A GetRTRankPvBTLArenaMatchingList ");
            BackendReturnObject bro1 = null;
            SendQueue.Enqueue(Backend.RTRank.GetMyRTRank, rank_uuid, 50, callback => { bro1 = callback; });
            while (Loading.Bottom(bro1) == false) { await Task.Delay(100); }
            LogPrint.Print("----------------A GetRTRankPvBTLArenaMatchingList Around  _bro1 : " + bro1);

            if (bro1.IsSuccess())
            {
                JsonData jdRows = bro1.GetReturnValuetoJSON()["rows"];
                if (jdRows != null)
                {
                    int ir = 0;
                    foreach (JsonData row in jdRows)
                    {
                        float _score = RowPaser.FloatPaser(row, "score");
                        if (_score > 0)
                        {
                            m_Dic_RTRankPvPBattleArena[Enum_RTRankType.Rank_RT_PvpTop50MyAround10].Add(
                            new RankInfo()
                            {
                                gamer_indate = row["gamerInDate"].ToString(),
                                isMyself = string.Equals(row["nickname"].ToString(), BackendGpgsMng.backendUserInfo.m_nickname),
                                nickName = row["nickname"].ToString(),
                                rank = RowPaser.IntPaser(row, "rank"),
                                score = _score,

                                isAI = false,
                                ai_cpht_dvs_nbr = -1,
                            });
                        }
                    }
                }

                // 자신의 주변 유저들 100명중 무작위 랜덤 20명 리스트 생성 
                int addCnt = 0;
                mListPvpBTLMatching.Clear();
                var myArouldRanKUser = m_Dic_RTRankPvPBattleArena[Enum_RTRankType.Rank_RT_PvpTop50MyAround10];
                if (myArouldRanKUser.Count > 1)
                {
                    List<int> random = new List<int>();
                    for (int i = 0; i < myArouldRanKUser.Count; i++)
                        random.Add(i);

                    List<int> shuffled = new List<int>(Utility.ShuffleArray(random, (int)Time.time));
                    foreach (int idx in shuffled)
                    {
                        if (addCnt < matching_user_cnt)
                        {
                            if (myArouldRanKUser[idx].isMyself == false)
                            {
                                mListPvpBTLMatching.Add(myArouldRanKUser[idx]);
                                addCnt++;
                            }
                        }
                        else break;
                    }
                }

                // AI입력, 매칭 상대 수를 지정한 수보다 낮으면 자신의 전투력??비슷하게 랜덤으로 AI데이터 입력 
                if (addCnt < matching_user_cnt)
                {
                    int ai_cnt = matching_user_cnt - addCnt;
                    List<int> ai_shuffled_chpt_dvs_nbr = AIMatchingChapterDvsNbr(ai_cnt);
                    LogPrint.Print(" 111 RANK - USER - [addCnt : " + mListPvpBTLMatching.Count + "] 매칭 상대 수를 지정한 수보다 낮으면 자신의 전투력??비슷하게 랜덤으로 AI데이터 입력");
                    for (int i = 0; i < ai_shuffled_chpt_dvs_nbr.Count; i++)
                    {
                        mListPvpBTLMatching.Add(
                            new RankInfo()
                            {
                                gamer_indate = GameDatabase.GetInstance ().GetUniqueIDX().ToString(),
                                isMyself = false,
                                nickName = string.Format("No.{0} ZB", ai_shuffled_chpt_dvs_nbr[i]),
                                rank = -1,
                                score = -1,

                                isAI = true,
                                ai_cpht_dvs_nbr = ai_shuffled_chpt_dvs_nbr[i]
                            });
                        addCnt++;
                    }
                    LogPrint.Print(" 222 RANK - USER - [addCnt : " + mListPvpBTLMatching.Count + "] 매칭 상대 수를 지정한 수보다 낮으면 자신의 전투력??비슷하게 랜덤으로 AI데이터 입력");
                }

                if (mListPvpBTLMatching.Count > 0)
                {
                    mListPvpBTLMatching.Sort((RankInfo x, RankInfo y) => y.score.CompareTo(x.score));
                }
            }
        }

        /// <summary>
        /// 자신이 진행중입 +- 챕터의랜덤 AI의 chpt_dvs_nbr 
        /// </summary>
        List<int> AIMatchingChapterDvsNbr(int ai_cnt)
        {
            int pvp_arena_ai_chpt_plus = GameDatabase.GetInstance().chartDB.GetDicBalance("pvp.arena.ai.chpt.plus").val_int;
            int chpt_dvs_nbr = GameDatabase.GetInstance().tableDB.GetUserInfo().GetChapterDvsNbr();
            int last_chpt_dvs_nbr = GameDatabase.GetInstance().monsterDB.GetChapterLastDvsNbr();
            List<int> random = new List<int>();
            while (ai_cnt > 0)
            {
                int rnd_chpt_dvs_nbr = UnityEngine.Random.Range(chpt_dvs_nbr, chpt_dvs_nbr + pvp_arena_ai_chpt_plus);
                if(rnd_chpt_dvs_nbr <= last_chpt_dvs_nbr)
                {
                    if (random.FindIndex(x => x == rnd_chpt_dvs_nbr) == -1)
                    {
                        random.Add(rnd_chpt_dvs_nbr);
                        ai_cnt--;
                    }
                }
            }

            return new List<int>(Utility.ShuffleArray(random, (int)Time.time));
        }

        /// <summary> 실시간 PvP배틀 아레나, 클라이언트에 저장되어있는 타입별 랭킹 순위 리턴 </summary>
        public List<RankInfo> GetListRTRankPvpBTLArenaRank()
        {
            if (m_Dic_RTRankPvPBattleArena.TryGetValue(Enum_RTRankType.Rank_RT_PvpTop50, out List<RankInfo> ba))
                return ba;

            return null;
        }
        public List<RankInfo> GetListRTRankPvpBTLArenaMatching()
        {
            if (m_Dic_RTRankPvPBattleArena.TryGetValue(Enum_RTRankType.Rank_RT_PvpTop50MyAround10, out List<RankInfo> ba))
                return ba;

            return null;
        }

        /// <summary> 나의 PvP 배틀 아레나 랭킹 </summary>
        public RankInfo GetMyRankPvPBTLArena ()
        {
            int indx = GetInstance().rankDB.GetListRTRankPvpBTLArenaMatching().FindIndex(x => x.isMyself == true);
            if(indx >= 0)
            {
                return GetInstance().rankDB.GetListRTRankPvpBTLArenaMatching()[indx];
            }

            return new RankInfo() { rank = -1 };
        }

#endregion

        // #########################################################################################################
#region # backend 랭킹에서 유저 데이터 찾기 #
        /// <summary> 실시간 데이터, 챕터 클리어 랭킹에서 특정 챕터의 유저 데이터 받기 </summary>
        public async Task<JsonData> AGetChapterRTRankUser(int chpt_nbr)
        {
            LogPrint.PrintError("특정 챕터의 유저 데이터 받기  chpt_nbr:" + chpt_nbr);
            string gamer_indate = string.Empty;
            BackendReturnObject bro1 = null;
            SendQueue.Enqueue(Backend.RTRank.GetRTRankByScore, GetInstance().rankDB.rankUUID.uuid_RT_ChptBoss, (long)chpt_nbr, callback => { bro1 = callback; });
            while (Loading.Bottom(bro1) == false) { await Task.Delay(100); }

            if (bro1.IsSuccess())
            {
                JsonData returnData = bro1.GetReturnValuetoJSON();
                if (returnData["rows"].Count > 0)
                {
                    var rJd = returnData["rows"][UnityEngine.Random.Range(0, returnData["rows"].Count)];
                    gamer_indate = rJd["gamerInDate"].ToString();
                }
            }

            if (!string.IsNullOrEmpty(gamer_indate))
            {
                LogPrint.PrintError("r gamer_indate : " + gamer_indate);
                Task<JsonData> t2 = GetInstance().publicContentDB.AGetPubTableDataLoad(BackendGpgsMng.tableName_Pub_ChapterClearCharData, gamer_indate);
                await t2;
                return t2.Result;
            }

            return null;
        }
        
#endregion
    }

    /// <summary>
    /// ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
    /// #PvP 배틀 아레나 기록 메시지   
    /// ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
    /// </summary>
    public class PvPBattleRecord
    {
#region
        public const int max_storage = 10;

        public StructData structData;
        [System.Serializable]
        public struct StructData
        {
            public Match match;
            public Rank rank;
            public Record record;

            [System.Serializable]
            public struct Match
            {
                public DateTime nextLoadTime;
            }

            [System.Serializable]
            public struct Rank
            {
                public DateTime nextLoadTIme;
            }

            [System.Serializable]
            public struct Record
            {
                public bool isNewReceivNotif; // 새로운 알림이 있는가 ? 
                public bool isNewSent; // 내가 전투 결과를 상대에게 보냈는가 ?
                public DateTime nextLoadTIme; // 다음 새로고침 시간 
            }
        }

        /// <summary> 결투 결과를 상대에게 보낼때 태워 보내는 정보 (message 란에 json string 형태로 바꿔서 태워 보냄) </summary>
        [System.Serializable]
        public struct SendRecordContents
        {
            /// <summary>
            /// 결투 기록 보내는 사람의 InDate 값 
            /// </summary>
            public string sent_indate;
            /// <summary>
            /// 결투 승리자의 indate 
            /// </summary>
            public string winner_indate;

            public bool isAI;
            public int pvp_rank;
            public float pvp_score;
            public string msg;
        }

        /// <summary> 전투 결과 데이터 </summary>
        [System.Serializable]
        public struct ReceiveRecord
        {
            public string inDate; // 기록(쪽지)의 indate (매칭한 시간으로 활용)
            public string sender; // 보내는 사람의 InDate 
            public string receiver; // 받는 사람의 InDate 

            public string sender_nickname; // 보내는 사람의 닉네임 
            public string receiver_nickname; // 받는 사람의 닉네임 
            public bool isRead; // 결투 보상 받기 완료 (상대가 나와결투를 해서 내가 승리했을 경우에만)
            public SendRecordContents rcodJSONContents; // 상대 결투 결과 
        }
        // 전투 결과 기록 보관 
        private List<ReceiveRecord> _records = new List<ReceiveRecord>(); // 결투 기록 보낸것, 결투 기록 받은 것 (RecordJSONContents.i_sent_it == true ->>> 자신이 보낸것)
        private List<ReceiveRecord> _sent_client_records = new List<ReceiveRecord>();

        public int PvpBTLRecordCount => _records.Count;
        public ReceiveRecord GetPvpBTLRecordIndexDb(int index)
        {
            try
            {
                return _records[index];
            }
            catch (Exception e)
            {
                return new ReceiveRecord();
            }
        }

        /// <summary>
        /// 내가 보낸 기록 (클라이언트에 저장되있는 데이터)
        /// </summary>
        public void GetClientLoadMySentRecord()
        {
            _sent_client_records.Clear();
            // 내가 보낸것
            JSONObject rows = JSONObject.Create(PlayerPrefs.GetString(PrefsKeys.key_PvPBTLMySentRecord));
           
            if (rows["rows"] != null)
            {
                for (int i = 0; i < rows["rows"].Count; i++)
                {
                    LogPrint.Print("내가 보낸 기록 rows : " + rows["rows"][i].ToString());
                    ReceiveRecord db = JsonUtility.FromJson<ReceiveRecord>(rows["rows"][i].ToString());
                    SendRecordContents contents = JsonUtility.FromJson<SendRecordContents>(rows["rows"][i]["rcodJSONContents"].ToString());
                    ReceiveRecord rcd = new ReceiveRecord() { inDate = db.inDate };
                    try
                    {
                        rcd.sender = db.sender;
                        rcd.receiver = db.receiver;
                        rcd.sender_nickname = db.sender_nickname;
                        rcd.receiver_nickname = db.receiver_nickname;
                        rcd.isRead = db.isRead;
                        rcd.rcodJSONContents = contents;
                    }
                    catch (Exception e)
                    { LogPrint.PrintError("sentRows e : " + e); }

                    _sent_client_records.Add(rcd);
                }
            }
        }

        /// <summary>
        /// 보낸 기록(클라), 받은 기록 모두 로드 
        /// received == true : 받은 기록들만 로드 
        /// sent == true : 보낸 기록들만 로드 
        /// </summary>
        public async Task<bool> AGetLoadRecord(bool loadReceived = true, bool loadSent = true)
        {
            LogPrint.Print("!");
            BackendReturnObject bro1 = loadReceived == true ? null : new BackendReturnObject();
            if(loadReceived)
                SendQueue.Enqueue(Backend.Social.Message.GetReceivedMessageList, callback => { bro1 = callback; });

            while (Loading.Bottom(bro1) == false) { await Task.Delay(100); }
            LogPrint.Print("A GetLoadRecord bro1 : " + bro1);

            JsonData receivedRows = loadReceived == true ? bro1.GetReturnValuetoJSON()["rows"] : null;

            string my_indate = BackendGpgsMng.backendUserInfo.m_indate;
            List<ReceiveRecord> received_backup = new List<ReceiveRecord>(); // 받은 기록들 임시 보관 
            List<ReceiveRecord> sent_backup = new List<ReceiveRecord>(); // 보낸 기록들 임시 보관 
            if (loadReceived == true && loadSent == true)
            {
                if (receivedRows.Count > 0)
                    _records.Clear();
            }
            else
            {
                // 받은 기록만 다시 불러올것이다 -> 보낸 기록들 백업 
                if (loadReceived == true && loadSent == false)
                {
                    foreach (ReceiveRecord item in _records)
                    {
                        if (string.Equals(item.rcodJSONContents.sent_indate, my_indate) == true)
                            sent_backup.Add(item);
                    }
                }
                // 보낸 기록만 다시 불러올것이다 -> 받은 기록들 백업 
                else
                {
                    foreach (ReceiveRecord item in _records)
                    {
                        if (string.Equals(item.rcodJSONContents.sent_indate, my_indate) == false)
                            received_backup.Add(item);
                    }
                }

                _records.Clear();
                if (received_backup.Count > 0)
                    _records = received_backup;
                else if (sent_backup.Count > 0)
                    _records = sent_backup;
            }

            // 상대가 보낸것 
            if (receivedRows != null && receivedRows.Count > 0 && loadReceived == true)
            {
                foreach (JsonData row in receivedRows)
                {
                    LogPrint.Print("받은 기록 row : " + row.ToJson());
                    ReceiveRecord rcd = new ReceiveRecord() { inDate = RowPaser.StrPaser(row, "inDate") };
                    try
                    {
                        rcd.sender = RowPaser.StrPaser(row, "sender");
                        rcd.receiver = RowPaser.StrPaser(row, "receiver");
                        rcd.sender_nickname = RowPaser.StrPaser(row, "senderNickname");
                        rcd.receiver_nickname = RowPaser.StrPaser(row, "receiverNickname");
                        rcd.isRead = RowPaser.BoolPaser(row, "isRead");
                        rcd.rcodJSONContents = JsonUtility.FromJson<SendRecordContents>(RowPaser.StrPaser(row, "content"));
                    }
                    catch (Exception e)
                    { LogPrint.PrintError("receivedRows e : " + e); }

                    _records.Add(rcd);
                }
            }

            if (loadSent)
            {
                // 내가 보낸것
                if(_sent_client_records.Count > 0)
                {
                    foreach (ReceiveRecord rRcd in _sent_client_records)
                    {
                        _records.Add(rRcd);
                    }
                }
            }

            // 최근 순으로 내림차순 정렬 
            _records.Sort((ReceiveRecord x, ReceiveRecord y) => DateTime.Parse(y.inDate).CompareTo(DateTime.Parse(x.inDate)));

            LogPrint.Print("기록 보관 갯수 _records.Count : " + _records.Count);
            if(loadReceived)
            {
                DateTime next = BackendGpgsMng.GetInstance().GetNowTime().AddMinutes(5);
                GetInstance().pvpBattleRecord.structData.record.nextLoadTIme = next;
                PlayerPrefs.SetString(PrefsKeys.key_PvPArenaRecordNextLoadTime, next.ToString());
                GetInstance().pvpBattleRecord.structData.record.isNewReceivNotif = false;
            }

            if(loadSent)
            {
                GetInstance().pvpBattleRecord.structData.record.isNewSent = false;
            }

            return true;
        }

        /// <summary> 전투 결과 상대에게 메시지 보냄 </summary>
        public async Task<bool> SendRecord(string gamer_indate, SendRecordContents jmsg)
        {
            string jsonDb_content = JsonUtility.ToJson(jmsg);
            BackendReturnObject bro1 = null;
            SendQueue.Enqueue(Backend.Social.Message.SendMessage, gamer_indate, jsonDb_content, callback => { bro1 = callback; });
            while (Loading.Bottom(bro1) == false) { await Task.Delay(100); }
            LogPrint.Print("SendMessage : " + bro1);
            GameDatabase.GetInstance().pvpBattleRecord.structData.record.isNewSent = true;
            return true;
        }

        /// <summary> 클라이언트에 보낸 메시지 저장 </summary>
        public void SaveClientMySentMsg(SendRecordContents _contents, string _senderIndt, string _receiverIndt, string _sentNick, string _recvNick)
        {
            LogPrint.Print("SaveClientSentMsg");
            DateTime nt = BackendGpgsMng.GetInstance().GetNowTime();
            ReceiveRecord rr = new ReceiveRecord()
            {
                inDate = nt.ToString(),
                sender = _senderIndt,
                receiver = _receiverIndt,
                sender_nickname = _sentNick,
                receiver_nickname = _recvNick,
                isRead = false,
                rcodJSONContents = _contents
            };

            _sent_client_records.Add(rr);
            PlayerPrefs.SetString(PrefsKeys.key_PvPBTLMySentRecord, JsonUtility.ToJson(new Serialization<ReceiveRecord>(_sent_client_records)));
            LogPrint.Print("_sent_client_records.Count : " + _sent_client_records.Count);
        }

        /// <summary>
        /// 전투기록 메시지 읽기 (보상받기)
        /// </summary>
        /// <param name="record_indate"></param>
        public async Task<bool> ReceivedMessage(bool i_sent, string record_indate)
        {
            if (i_sent) // 내가 보낸것 (클라) (내가 공격한 기록)
            {
                int indx = _records.FindIndex(x => x.inDate == record_indate);
                if(indx >= 0)
                {
                    var temp = _records[indx];
                    temp.isRead = true;
                    _records[indx] = temp;

                    int indx2 = _sent_client_records.FindIndex(x => x.inDate == record_indate);
                    if(indx2 >= 0)
                    {
                        var temp2 = _sent_client_records[indx2];
                        temp2.isRead = true;
                        _sent_client_records[indx2] = temp2;
                        PlayerPrefs.SetString(PrefsKeys.key_PvPBTLMySentRecord, JsonUtility.ToJson(new Serialization<ReceiveRecord>(_sent_client_records)));
                    }

                    // -> 리워드 지급 
                    var goods = GetInstance().tableDB.GetTableDB_Goods();
                    goods.m_dia += GetInstance().chartDB.GetDicBalance("pvp.match.atk.win.reward").val_int;
                    await GetInstance().tableDB.SetUpdateGoods(goods);
                }
            }
            else
            {
                BackendReturnObject bro1 = null;
                SendQueue.Enqueue(Backend.Social.Message.GetReceivedMessage, record_indate, (callback) => { bro1 = callback; });
                while (Loading.Bottom(bro1) == false) { await Task.Delay(100); }
                if (bro1.IsSuccess())
                {
                    int indx = _records.FindIndex(x => x.inDate == record_indate);
                    if(indx >= 0)
                    {
                        var temp = _records[indx];
                        temp.isRead = true;
                        _records[indx] = temp;

                        // -> 리워드 지급 
                        var goods = GetInstance().tableDB.GetTableDB_Goods();
                        goods.m_dia += GetInstance().chartDB.GetDicBalance("pvp.match.dfn.win.reward").val_int;
                        await GetInstance().tableDB.SetUpdateGoods(goods);
                    }
                }
            }

            return true;
        }

        /// <summary> 받음 보관 가능한 메시지중 최근 20개를 제외한 이후 모두 삭제 </summary>
        public async Task<bool> DeleteRecordLastFrom20(bool deleteAll = false)
        {
            LogPrint.Print("DeleteRecordLastFrom20 start _records.Count : " + _records.Count);
            int start = deleteAll == false ? max_storage : 0;
            if (_records.Count > start || deleteAll == true)
            {
                List<ReceiveRecord> remove_records = new List<ReceiveRecord>();
                for (int i = start; i < _records.Count; i++)
                    remove_records.Add(_records[i]);

                if(remove_records.Count > 0)
                {
                    BackendReturnObject bro1 = null;
                    foreach (ReceiveRecord item in remove_records)
                    {
                        bro1 = null;
                        // 서버의 받은 기록 삭제 
                        if (string.Equals(item.rcodJSONContents.sent_indate, BackendGpgsMng.backendUserInfo.m_indate) == false)
                        {
                            SendQueue.Enqueue(Backend.Social.Message.DeleteReceivedMessage, item.inDate, (callback) => { bro1 = callback; });
                            while (Loading.Bottom(bro1) == false) { await Task.Delay(100); }
                        }
                    }

                    foreach (ReceiveRecord rmv_rdc in remove_records)
                    {
                        _sent_client_records.Remove(rmv_rdc);  // 클라의 보낸 기록 삭제 
                        _records.Remove(rmv_rdc);
                    }

                    PlayerPrefs.SetString(PrefsKeys.key_PvPBTLMySentRecord, JsonUtility.ToJson(new Serialization<ReceiveRecord>(_sent_client_records)));
                    LogPrint.Print("DeleteRecordLastFrom20 end _records.Count : " + _records.Count + ", _sent_client_records.count : " + _sent_client_records.Count);
                }
            }

            return true;
        }
#endregion
    }

    /// <summary>
    /// ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
    /// #PvP 배틀 데이터   
    /// ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
    /// </summary>
    public class PvPBattle
    {
        private Data pvpDbOr;
        public Data GetDataBattleDbOr() => pvpDbOr;
        public struct Data
        {
            public GamerInfo gamerInfo;
            public List<TableDB.Equipment> eqList;
            public IG.PartsIdx parts;
            public CharacterDB.StatValue statValue;
            public Dictionary<int, int[]> skillData;

            public struct GamerInfo
            {
                public string gamer_indate;
                public string gamer_nickName;
                public int gamer_rank;
                public float gamer_score;
                public string gamer_comment;
                public bool isAI;
                public int ai_cpht_dvs_nbr;
            }
        }

        private List<StructMatchUserData> rerMatchUseList = new List<StructMatchUserData>();
        private StructMatchUserData structRerMatchUseList;
        [System.Serializable]
        struct StructMatchUserData
        {
            public string indate; // 결투 상대 InDate 
            public string reMatchDate; // 다시 결투 지핼할 수 있는 시간 
        }

        /// <summary> 이전에 결투해서 승리한 유저인가? 그렇다면 1시간 텀이 지났나? </summary>
        public bool GetIsCheckMatch(string gmrIndate) 
        {
            int indx = rerMatchUseList.FindIndex(x => x.indate == gmrIndate);
            if (indx >= 0)
            {
                var db = rerMatchUseList[indx];
                DateTime reDate = DateTime.Parse(db.reMatchDate);
                DateTime nowDate = BackendGpgsMng.GetInstance().GetNowTime();
                if ((reDate - nowDate).TotalSeconds > 0)
                {
                    return false;
                }
                else return true;
            }
            return true;
        }

        /// <summary> 다시 결투할수 있는 종료 시간 </summary>
        public DateTime GetReMatchDate(string gmrIndate)
        {
            int indx = rerMatchUseList.FindIndex(x => x.indate == gmrIndate);
            if(indx >= 0)
            {
                return DateTime.Parse(rerMatchUseList[indx].reMatchDate);
            }

            return BackendGpgsMng.GetInstance().GetNowTime();
        }
        /// <summary> 리매치 유저 Add </summary>
        void ReMatchUserAdd (string gmrIndate)
        {
            DateTime reDate = BackendGpgsMng.GetInstance().GetNowTime().AddHours(1);
            LogPrint.Print("gmrIndate : " + gmrIndate + ", next re date : " + reDate);
            rerMatchUseList.Add(new StructMatchUserData() { indate = gmrIndate, reMatchDate = reDate.ToString() });
            PlayerPrefs.SetString(PrefsKeys.key_PvPReMatchUserList, JsonUtility.ToJson(new Serialization<StructMatchUserData>(rerMatchUseList)));

            LogPrint.Print(" 리매치 유저 Add : " + PlayerPrefs.GetString(PrefsKeys.key_PvPReMatchUserList));
            LogPrint.Print(" 리매치 유저 Add : " + JsonUtility.ToJson(new Serialization<StructMatchUserData>(rerMatchUseList)));
        }

        /// <summary> 다시 결투 할 수있는 유저 리스트 </summary>
        public void ReMatchUserLoad()
        {
            LogPrint.Print("다시 결투 할 수있는 유저 리스트 : " + PlayerPrefs.GetString(PrefsKeys.key_PvPReMatchUserList));
            rerMatchUseList.Clear();
            JSONObject rows = JSONObject.Create(PlayerPrefs.GetString(PrefsKeys.key_PvPReMatchUserList));
            LogPrint.Print("다시 결투 할 수있는 유저 리스트 rows : " + rows);
            if (rows["rows"] != null)
            {
                for (int i = 0; i < rows["rows"].Count; i++)
                {
                    LogPrint.Print("리매치 rows : " + rows["rows"][i].ToString());
                    try
                    {
                        StructMatchUserData db = JsonUtility.FromJson<StructMatchUserData>(rows["rows"][i].ToString());
                        if((DateTime.Parse(db.reMatchDate) - BackendGpgsMng.GetInstance().GetNowTime()).TotalSeconds > 0)
                        {
                            rerMatchUseList.Add(db);
                        }
                    }
                    catch (Exception e)
                    { LogPrint.PrintError("e : " + e); }
                }

                PlayerPrefs.SetString(PrefsKeys.key_PvPReMatchUserList, JsonUtility.ToJson(new Serialization<StructMatchUserData>(rerMatchUseList)));
            }
        }

#region 
        /// <summary> PvP 배틀에 사용될 결투 상대 데이터 </summary>
        public void SetBattleData(Data.GamerInfo gmrInfo, JsonData row)
        {
            List<TableDB.Equipment> eq_data_list = new List<TableDB.Equipment>();
            IG.PartsIdx parts_data = new IG.PartsIdx();
            CharacterDB.StatValue stat_data = new CharacterDB.StatValue();
            Dictionary<int, int[]> skill_data = new Dictionary<int, int[]>();

            try
            {
                if (gmrInfo.isAI == true) // AI 몬스터 
                {
                    var mnst_db = GameDatabase.GetInstance().monsterDB.list_cdb_chpt_mnst_stat;
                    int mnst_db_indx = mnst_db.FindIndex((cdb_chpt_mnst_stat obj) => obj.chpt_dvs_nbr == gmrInfo.ai_cpht_dvs_nbr);
                    var ai_chpt_db = mnst_db_indx >= 0 ? mnst_db[mnst_db_indx] : mnst_db[mnst_db.Count - 1];

                    #region --- AI 장비 정보 ---
                    if (mnst_db_indx >= 0)
                    {
                        for (int eq_ty = 0; eq_ty <= 10; eq_ty++)
                        {
                            TableDB.Equipment eq_new_db = new TableDB.Equipment()
                            {
                                eq_ty = eq_ty,
                                eq_rt = ai_chpt_db.eq_rat,
                                eq_id = ai_chpt_db.eq_id,
                                m_norm_lv = ai_chpt_db.chpt_norm_lv,
                                m_ehnt_lv = ai_chpt_db.chpt_ehnt_lv
                            };
                            eq_new_db.st_op = GameDatabase.GetInstance().tableDB.GetMonsterEquipRandomOption(eq_new_db.eq_ty, eq_new_db.eq_rt, ai_chpt_db.chpt_id <= 23 ? true : false);
                            if (GameDatabase.GetInstance().chartDB.GetIsPartsTypeAcce(eq_new_db.eq_ty) == true && eq_new_db.eq_rt >= 3) // 장신구 전용 옵션은 고급 등급 부터 (rating 3)
                                eq_new_db.st_sop_ac = GameDatabase.GetInstance().tableDB.GetEquipAcceRandomSpecialOption(true);

                            eq_data_list.Add(eq_new_db);
                        }
                    }
                    #endregion

                    #region --- AI 장비 파츠 ID 정보 ---
                    parts_data = new IG.PartsIdx()
                    {
                        ty0_weapon_rt = eq_data_list[0].eq_rt,
                        ty0_weapon_id = eq_data_list[0].eq_id,
                        ty1_shield_rt = eq_data_list[1].eq_rt,
                        ty1_shield_id = eq_data_list[1].eq_id,
                        ty2_helmet_rt = eq_data_list[2].eq_rt,
                        ty2_helmet_id = eq_data_list[2].eq_id,
                        ty3_shoulder_l_rt = eq_data_list[3].eq_rt,
                        ty3_shoulder_l_id = eq_data_list[3].eq_id,
                        ty3_shoulder_r_rt = eq_data_list[3].eq_rt,
                        ty3_shoulder_r_id = eq_data_list[3].eq_id,
                        ty4_armor_rt = eq_data_list[4].eq_rt,
                        ty4_armor_id = eq_data_list[4].eq_id,
                        ty5_arm_rt = eq_data_list[5].eq_rt,
                        ty5_arm_id = eq_data_list[5].eq_id,
                        ty6_pants_rt = eq_data_list[6].eq_rt,
                        ty6_pants_id = eq_data_list[6].eq_id,
                        ty7_boots_rt = eq_data_list[7].eq_rt,
                        ty7_boots_id = eq_data_list[7].eq_id
                    };
                    #endregion

                    #region --- AI 스탯 정보 ---
                    stat_data = GameDatabase.GetInstance().monsterDB.SetMonsterStatValue(eq_data_list[0], 0, true, ai_chpt_db.chpt_id <= 23 ? true : false);
                    #endregion

                    #region --- AI 스킬 정보 ---
                    skill_data = GameDatabase.GetInstance().monsterDB.GetMonsterRandomSkills();
                    #endregion
                }
                else
                {
                    #region --- 유저 장비 정보 ---
                    JSONObject db_useEquip = JSONObject.Create(RowPaser.StrPaser(row, "useEquip"));
                    if (db_useEquip["rows"] != null)
                    {
                        for (int i = 0; i < db_useEquip["rows"].Count; i++)
                        {
                            var eq_psr = JsonUtility.FromJson<PublicContentDB.PubDB_NowChar_Equip>(db_useEquip["rows"][i].ToString());
                            eq_data_list.Add(new TableDB.Equipment()
                            {
                                eq_ty = eq_psr.eq_ty,
                                eq_rt = eq_psr.eq_rt,
                                eq_id = eq_psr.eq_id,
                                eq_legend = eq_psr.eq_legend,
                                m_norm_lv = eq_psr.eq_nor_lv,
                                m_ehnt_lv = eq_psr.eq_ehn_lv,
                            });
                        }
                    }
                    #endregion

                    #region --- 유저 장비 파츠 ID 정보 ---
                    parts_data = JsonUtility.FromJson<IG.PartsIdx>(RowPaser.StrPaser(row, "parts"));
                    #endregion

                    #region --- 유저 스탯 정보 ---
                    stat_data = JsonUtility.FromJson<CharacterDB.StatValue>(RowPaser.StrPaser(row, "statValue"));
                    #endregion

                    #region --- 유저 스킬 정보 ---
                    var pubDb_skill = JsonUtility.FromJson<GameDatabase.PublicContentDB.PubDB_ClearChapterChar_UseSkill>(RowPaser.StrPaser(row, "useSkill"));
                    int n_mSlotNum = GameDatabase.GetInstance().tableDB.GetUseMainSlot();
                    for (int i = 0; i < 6; i++)
                    {
                        int sk_id = 0; // 번호 
                        int sk_lv = 0; // 레벨 
                        switch (i)
                        {
                            case 0: sk_id = pubDb_skill.slot1_id; sk_lv = pubDb_skill.slot1_lv; break;
                            case 1: sk_id = pubDb_skill.slot2_id; sk_lv = pubDb_skill.slot2_lv; break;
                            case 2: sk_id = pubDb_skill.slot3_id; sk_lv = pubDb_skill.slot3_lv; break;
                            case 3: sk_id = pubDb_skill.slot4_id; sk_lv = pubDb_skill.slot4_lv; break;
                            case 4: sk_id = pubDb_skill.slot5_id; sk_lv = pubDb_skill.slot5_lv; break;
                            case 5: sk_id = pubDb_skill.slot6_id; sk_lv = pubDb_skill.slot6_lv; break;
                        }

                        var cdb = GameDatabase.GetInstance().chartDB.GetChartSkill_Data(sk_id);
                        int sk_rt = cdb.s_rating; // 등급 
                        int sk_pt = cdb.s_pnt; // 발동 포인트 
                        int sk_rn = 0; // 착용중 or 랜덤 

                        skill_data.Add(i, new int[] { sk_id, sk_lv, sk_rt, sk_pt, sk_rn });
                    }
                    #endregion
                }

                LogPrint.Print("LoadPvPBattleOpponentData parts_data : " + JsonUtility.ToJson(parts_data));

                pvpDbOr = new Data()
                {
                    eqList = eq_data_list,
                    parts = parts_data,
                    statValue = stat_data,
                    skillData = skill_data,
                    gamerInfo = gmrInfo,
                };

                PopUpMng.GetInstance().Open_PvpBattleOpponentInfo(); // 상대할 유저의 데이터를 팝업 
            }
            catch (Exception)
            {
                PopUpMng.GetInstance().Close_PvpBattleOpponent();
                throw;
            }
        }

        /// <summary> PvP 기록 결과 전송 </summary>
        public async Task SendPvPBattleRecord(bool iWin)
        {
            try
            {
                string ndate = BackendGpgsMng.GetInstance().GetNowTime().ToString();
                var myRankInfo = GetInstance().rankDB.GetMyRankPvPBTLArena();
                string my_nick = BackendGpgsMng.backendUserInfo.m_nickname;
                string my_indate = BackendGpgsMng.backendUserInfo.m_indate;
                int my_rank = myRankInfo.rank;
                float my_score = myRankInfo.score;
                string my_comment = PlayerPrefs.GetString(PrefsKeys.key_PvPArenaComment);
               
                string gmr_nick = pvpDbOr.gamerInfo.gamer_nickName;
                string gmr_indate = pvpDbOr.gamerInfo.gamer_indate;
                int gmr_rank = pvpDbOr.gamerInfo.gamer_rank;
                float gmr_score = pvpDbOr.gamerInfo.gamer_score;
                string gmr_comment = pvpDbOr.gamerInfo.gamer_comment;
                bool gmr_isAI = pvpDbOr.gamerInfo.isAI;

                if (iWin) // 리매치 유저 Add 
                {
                    ReMatchUserAdd(gmr_indate);
                }

                if (!string.IsNullOrEmpty(gmr_indate))
                {
                    PvPBattleRecord.SendRecordContents contents = new PvPBattleRecord.SendRecordContents()
                    {
                        sent_indate = my_indate,
                        winner_indate = iWin == true ? my_indate : gmr_indate,
                        pvp_rank = my_rank,
                        pvp_score = my_score,
                        msg = iWin == true ? my_comment : gmr_comment,
                        isAI = gmr_isAI
                    };

                    await GetInstance().pvpBattleRecord.SendRecord(gmr_indate, contents);

                    // 클라이언트에 저잘될 결투 결과 
                    PvPBattleRecord.SendRecordContents clasave_contents = new PvPBattleRecord.SendRecordContents()
                    {
                        sent_indate = my_indate,
                        winner_indate = iWin == true ? my_indate : gmr_indate,
                        pvp_rank = gmr_rank,
                        pvp_score = gmr_score,
                        msg = iWin == true ? my_comment : gmr_comment,
                        isAI = gmr_isAI
                    };
                    GetInstance().pvpBattleRecord.SaveClientMySentMsg(clasave_contents, my_indate, gmr_indate, my_nick, gmr_nick);
                }
            }
            catch (Exception e)
            {
                LogPrint.Print("e : " + e);
                //return false;
            }
           
            //return true;
        }
#endregion
    }

    /// <summary>
    /// ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
    /// #채팅 
    /// ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
    /// </summary>
    public class Chat
    {
        #region 
        // 채팅 메시지 스트링 포멧 
        private static string DEFAULT_CHAT_RANK     = "{0} [{1}, {2}등]"; // 닉네임, 스테이지-넘버, 반복or진행, 스테이지 순위
        private static string DEFAULT_CHAT_NO_RANK  = "{0} [{1}, 순위없음]"; // 닉네임, 스테이지-넘버, 반복or진행, 스테이지 순위
        private static string DATE_TOP = "[{0}]";

        /// <summary>
        /// 일반 채팅 메시지 받은 내용을 MsgType 타입에 맞게 title, contents로 각각 나눠서 리턴 
        /// </summary>
        public ChatMsg ChatCellText(string nickName, string msg)
        {
            try
            {
                string title = string.Empty, contents = string.Empty;
                ChatMsgItem cmi = JsonUtility.FromJson<ChatMsgItem>(msg);
                if (cmi.MsgType == 1) // 유저 채팅 메시지 (유저정보가 json 형식으로 메시지와 함께 Contentes 존재)
                {
                    ChatPaserMsgUserInfo cmui = JsonUtility.FromJson<ChatPaserMsgUserInfo>(cmi.Contents);
                    if (cmui.cs_rank > 0)
                    {
                        title = string.Format(DEFAULT_CHAT_RANK, nickName, GameDatabase.GetInstance().monsterDB.GetRankChapterStageString(cmui.csn), cmui.cs_rank);
                    }
                    else
                    {
                        title = string.Format(DEFAULT_CHAT_NO_RANK, nickName, GameDatabase.GetInstance().monsterDB.GetRankChapterStageString(cmui.csn));
                    }

                    contents = cmui.Message;
                }
                else if (cmi.MsgType == 2) // 장비 드랍 정보 (장비 드랍 정보가 json형식으로 Contents에 존재)
                {
                    title = string.Format(DATE_TOP, BackendGpgsMng.GetInstance().GetNowTime());
                    ChatPaserMsgItemInfo cmui = JsonUtility.FromJson<ChatPaserMsgItemInfo>(cmi.Contents);
                    if(cmui.ItemType == "equip") // 장비 드랍 
                    {
                        // 알림 OFF 상태인지 체크 
                        if ((GameDatabase.GetInstance().option.notice_equip_recv_rt5 == 1 && cmui.Rating == 5) || 
                            (GameDatabase.GetInstance().option.notice_equip_recv_rt6 == 1 && cmui.Rating == 6) || 
                            (GameDatabase.GetInstance().option.notice_equip_recv_rt7 == 1 && cmui.Rating == 7))
                        {
                            cmi.MsgType = -1;
                        }
                        else 
                        {
                            contents = StringFormat.ChatEquipDropText(nickName, cmui.Type, cmui.Rating, cmui.Idx, true);
                        }
                    }
                }
                else
                {
                    // 시스템 메시지 (시스템 메시지가 Contentes에 존재)
                    title = string.Format(DATE_TOP, BackendGpgsMng.GetInstance().GetNowTime());
                    contents = cmi.Contents;
                }

                return new ChatMsg() { MsgType = cmi.MsgType, TItle = title, Contents = contents };
            }
            catch (Exception)
            {
                string eTitle = string.Format("{0} {1}", string.Format(DATE_TOP, BackendGpgsMng.GetInstance().GetNowTime()), nickName);
                return new ChatMsg() { MsgType = 0, TItle = eTitle, Contents = msg };
            }
        }

        /// <summary>
        /// 채팅 메시지를 전송할때 msgType 타입 번호에 따라 json으로 msgType타입에 맞게 변환 리턴 
        /// </summary>
        public string ChatSendMessageJson(int msgType, string message)
        {
            string jsn_msg = message;
            if (msgType == 0) // 기본 메시지 (공지, 시스템...)
            {
                jsn_msg = message;
            }
            else if (msgType == 1) // 일반 메시지 -> 유저 정보 포함 
            {
                int chapter_stage_nbr = GameDatabase.GetInstance().tableDB.GetUserInfo().GetChapterStageNbr();
                int chapter_stage_nbr_rank = GetInstance().rankDB.GetMyStageRank();
                var equip = GetInstance().tableDB.GetNowWearingEquipPartsData(2);
                int hel_ty = equip.eq_ty;
                int hel_rt = equip.eq_rt;
                int hel_id = equip.eq_id;
                int hel_eht_lv = equip.m_ehnt_lv;
                string guild = "";

                jsn_msg = JsonUtility.ToJson(new ChatPaserMsgUserInfo() 
                {   
                    csn = chapter_stage_nbr,
                    cs_rank = chapter_stage_nbr_rank,
                    hel_ty = hel_ty, 
                    hel_rt = hel_rt, 
                    hel_id = hel_id, 
                    hel_eht_lv = hel_eht_lv,
                    Guild = guild, 
                    Message = message 
                });
            }
            else if (msgType == 2) // 아이템 획득 정보 메시지 
            {
                jsn_msg = message;
            }

            return jsn_msg;
        }

        /// <summary>
        /// 아이템 획득 채팅 메시지 전송 
        /// </summary>
        public void ChatSendItemMessage(string itemType, int ty, int rt, int id)
        {
            if(rt >= 5)
            {
                if((GameDatabase.GetInstance().option.notice_equip_send_rt5 == 0 && rt == 5) || 
                    (GameDatabase.GetInstance().option.notice_equip_send_rt6 == 0 && rt == 6) || 
                    (GameDatabase.GetInstance().option.notice_equip_send_rt7 == 0 && rt == 7))
                {
                    ChatScript.Instance().PublicEquipDropChat(JsonUtility.ToJson(new ChatPaserMsgItemInfo() { ItemType = itemType, Type = ty, Rating = rt, Idx = id }));
                }
            }
        }

        // ###################################
#region # 유저 채팅 차단 #
        List<Block> blocks = new List<Block>();
        [System.Serializable]
        public struct Block
        {
            public string nick;
            public string blockDate;
        }

        public int GetBlockCount() => blocks.Count;
        public Block GetBlock(int indx) => blocks[indx];
        
        public void GetUserBlockList()
        {
            //ChatScript.Instance().UnBlockUser("lg-01");

            JSONObject rows = JSONObject.Create(PlayerPrefs.GetString(PrefsKeys.prky_ChatBlockNickName));
            if (rows["rows"] != null)
            {
                for (int i = 0; i < rows["rows"].Count; i++)
                {
                    var db = JsonUtility.FromJson<Block>(rows["rows"][i].ToString());
                    blocks.Add(new Block() { nick = db.nick, blockDate = db.blockDate });
                }
            }

            LogPrint.Print("UserBlock : " + blocks.Count + ", " + JsonUtility.ToJson(new Serialization<Block>(blocks)));
        }

        public void UserBlock (string nickName)
        {
            blocks.Add(new Block() { nick = nickName, blockDate = BackendGpgsMng.GetInstance ().GetNowTime().ToString() });
            LogPrint.Print("UserBlock : " + blocks .Count + ", " + JsonUtility.ToJson(new Serialization<Block>(blocks)));

            string save = JsonUtility.ToJson(new Serialization<Block>(blocks));
            PlayerPrefs.SetString(PrefsKeys.prky_ChatBlockNickName, save);
        }

        public void UserUnBlock(string nickName)
        {
            int indx = blocks.FindIndex(x => x.nick == nickName);
            if(indx >= 0)
            {
                blocks.RemoveAt(indx);
            }

            LogPrint.Print("UserBlock : " + blocks.Count + ", " + JsonUtility.ToJson(new Serialization<Block>(blocks)));
            string save = JsonUtility.ToJson(new Serialization<Block>(blocks));
            PlayerPrefs.SetString(PrefsKeys.prky_ChatBlockNickName, save);
        }
#endregion
#endregion
    }

    /// <summary>
    /// ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
    /// #업적  
    /// ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
    /// </summary>
    public class AchievementsDB
    {
        public enum Nbr
        {
            nbr0,
            nbr1,
            nbr2,
            nbr3,
            nbr4,
            nbr5,
            nbr6,
            nbr7,
            nbr8,
            nbr9,
            nbr10,
            nbr11,
            nbr12,
            nbr13,
            nbr14,
            nbr15,
            nbr16,
            nbr17,
            nbr18,
            nbr19,
            nbr20,
            nbr21,
            nbr22,
            nbr23,
        }

        private Achievements achievement_db = new Achievements();
        [System.Serializable]
        public struct Achievements
        {
            public string indate;
            public JsonDB nbr0;
            public JsonDB nbr1;
            public JsonDB nbr2;
            public JsonDB nbr3;
            public JsonDB nbr4;
            public JsonDB nbr5;
            public JsonDB nbr6;
            public JsonDB nbr7;
            public JsonDB nbr8;
            public JsonDB nbr9;
            public JsonDB nbr10;
            public JsonDB nbr11;
            public JsonDB nbr12;
            public JsonDB nbr13;
            public JsonDB nbr14;
            public JsonDB nbr15;
            public JsonDB nbr16;
            public JsonDB nbr17;
            public JsonDB nbr18;
            public JsonDB nbr19;
            public JsonDB nbr20;
            public JsonDB nbr21;
            public JsonDB nbr22;
            public JsonDB nbr23;
        }
        [System.Serializable]
        public struct JsonDB
        {
            public int progLv; // 업적 진행 레벨 
            public long progCnt; // 업적 진행 카운트 
        }

        public string GetInDate() => achievement_db.indate;
        public int GetChartCount() => _chartDB.dic_cdb_achievements.Keys.Count;

        public Param GetParamAddAchievements()
        {
            Param parm = new Param();
            for (int i = 0; i < GetChartCount(); i++)
            {
                parm.Add(string.Format("nbr{0}", i), JsonUtility.ToJson(Get((Nbr)i)));
            }

            return parm;
        }

        public void SetInitialDataSetting(Achievements a) => achievement_db = a;
        public void Set(JsonData returnData)
        {
            if (returnData != null)
            {
                if (returnData.Keys.Contains("rows"))
                {
                    string defaultJDb = JsonUtility.ToJson(new JsonDB());
                    JsonData row = returnData["rows"][0];
                    achievement_db = new Achievements()
                    {
                        indate = RowPaser.StrPaser(row, "inDate"),
                        nbr0 = JsonUtility.FromJson<JsonDB>(string.IsNullOrEmpty(RowPaser.StrPaser(row, "nbr0")) ? defaultJDb : RowPaser.StrPaser(row, "nbr0")),
                        nbr1 = JsonUtility.FromJson<JsonDB>(string.IsNullOrEmpty(RowPaser.StrPaser(row, "nbr1")) ? defaultJDb : RowPaser.StrPaser(row, "nbr1")),
                        nbr2 = JsonUtility.FromJson<JsonDB>(string.IsNullOrEmpty(RowPaser.StrPaser(row, "nbr2")) ? defaultJDb : RowPaser.StrPaser(row, "nbr2")),
                        nbr3 = JsonUtility.FromJson<JsonDB>(string.IsNullOrEmpty(RowPaser.StrPaser(row, "nbr3")) ? defaultJDb : RowPaser.StrPaser(row, "nbr3")),
                        nbr4 = JsonUtility.FromJson<JsonDB>(string.IsNullOrEmpty(RowPaser.StrPaser(row, "nbr4")) ? defaultJDb : RowPaser.StrPaser(row, "nbr4")),
                        nbr5 = JsonUtility.FromJson<JsonDB>(string.IsNullOrEmpty(RowPaser.StrPaser(row, "nbr5")) ? defaultJDb : RowPaser.StrPaser(row, "nbr5")),
                        nbr6 = JsonUtility.FromJson<JsonDB>(string.IsNullOrEmpty(RowPaser.StrPaser(row, "nbr6")) ? defaultJDb : RowPaser.StrPaser(row, "nbr6")),
                        nbr7 = JsonUtility.FromJson<JsonDB>(string.IsNullOrEmpty(RowPaser.StrPaser(row, "nbr7")) ? defaultJDb : RowPaser.StrPaser(row, "nbr7")),
                        nbr8 = JsonUtility.FromJson<JsonDB>(string.IsNullOrEmpty(RowPaser.StrPaser(row, "nbr8")) ? defaultJDb : RowPaser.StrPaser(row, "nbr8")),
                        nbr9 = JsonUtility.FromJson<JsonDB>(string.IsNullOrEmpty(RowPaser.StrPaser(row, "nbr9")) ? defaultJDb : RowPaser.StrPaser(row, "nbr9")),
                        nbr10 = JsonUtility.FromJson<JsonDB>(string.IsNullOrEmpty(RowPaser.StrPaser(row, "nbr10")) ? defaultJDb : RowPaser.StrPaser(row, "nbr10")),
                        nbr11 = JsonUtility.FromJson<JsonDB>(string.IsNullOrEmpty(RowPaser.StrPaser(row, "nbr11")) ? defaultJDb : RowPaser.StrPaser(row, "nbr11")),
                        nbr12 = JsonUtility.FromJson<JsonDB>(string.IsNullOrEmpty(RowPaser.StrPaser(row, "nbr12")) ? defaultJDb : RowPaser.StrPaser(row, "nbr12")),
                        nbr13 = JsonUtility.FromJson<JsonDB>(string.IsNullOrEmpty(RowPaser.StrPaser(row, "nbr13")) ? defaultJDb : RowPaser.StrPaser(row, "nbr13")),
                        nbr14 = JsonUtility.FromJson<JsonDB>(string.IsNullOrEmpty(RowPaser.StrPaser(row, "nbr14")) ? defaultJDb : RowPaser.StrPaser(row, "nbr14")),
                        nbr15 = JsonUtility.FromJson<JsonDB>(string.IsNullOrEmpty(RowPaser.StrPaser(row, "nbr15")) ? defaultJDb : RowPaser.StrPaser(row, "nbr15")),
                        nbr16 = JsonUtility.FromJson<JsonDB>(string.IsNullOrEmpty(RowPaser.StrPaser(row, "nbr16")) ? defaultJDb : RowPaser.StrPaser(row, "nbr16")),
                        nbr17 = JsonUtility.FromJson<JsonDB>(string.IsNullOrEmpty(RowPaser.StrPaser(row, "nbr17")) ? defaultJDb : RowPaser.StrPaser(row, "nbr17")),
                        nbr18 = JsonUtility.FromJson<JsonDB>(string.IsNullOrEmpty(RowPaser.StrPaser(row, "nbr18")) ? defaultJDb : RowPaser.StrPaser(row, "nbr18")),
                        nbr19 = JsonUtility.FromJson<JsonDB>(string.IsNullOrEmpty(RowPaser.StrPaser(row, "nbr19")) ? defaultJDb : RowPaser.StrPaser(row, "nbr19")),
                        nbr20 = JsonUtility.FromJson<JsonDB>(string.IsNullOrEmpty(RowPaser.StrPaser(row, "nbr20")) ? defaultJDb : RowPaser.StrPaser(row, "nbr20")),
                        nbr21 = JsonUtility.FromJson<JsonDB>(string.IsNullOrEmpty(RowPaser.StrPaser(row, "nbr21")) ? defaultJDb : RowPaser.StrPaser(row, "nbr21")),
                        nbr22 = JsonUtility.FromJson<JsonDB>(string.IsNullOrEmpty(RowPaser.StrPaser(row, "nbr22")) ? defaultJDb : RowPaser.StrPaser(row, "nbr22")),
                        nbr23 = JsonUtility.FromJson<JsonDB>(string.IsNullOrEmpty(RowPaser.StrPaser(row, "nbr23")) ? defaultJDb : RowPaser.StrPaser(row, "nbr23")),
                    };

                    //for (int i = 0; i <= this.GetChartCount(); i++)
                    //{
                    //    var db = Get((Nbr)i);
                    //    if (db.progCnt <= 0)
                    //    {
                    //        string strCnt = PlayerPrefs.GetString(PrefsKeys.prky_achievement(((Nbr)i).ToString()));
                    //        if (i == 0)
                    //            strCnt = "0";

                    //        db.progCnt = System.Convert.ToInt64(string.IsNullOrEmpty(strCnt) ? "0" : strCnt);
                    //        SetUpdate((Nbr)i, db);
                    //    }
                    //}
                }
            } 
            else Debug.Log("contents has no data");
        }

        public JsonDB Get(Nbr nbr)
        {
            switch (nbr)
            {
                case Nbr.nbr0: return achievement_db.nbr0;
                case Nbr.nbr1: return achievement_db.nbr1;
                case Nbr.nbr2: return achievement_db.nbr2;
                case Nbr.nbr3: return achievement_db.nbr3;
                case Nbr.nbr4: return achievement_db.nbr4;
                case Nbr.nbr5: return achievement_db.nbr5;
                case Nbr.nbr6: return achievement_db.nbr6;
                case Nbr.nbr7: return achievement_db.nbr7;
                case Nbr.nbr8: return achievement_db.nbr8;
                case Nbr.nbr9: return achievement_db.nbr9;
                case Nbr.nbr10: return achievement_db.nbr10;
                case Nbr.nbr11: return achievement_db.nbr11;
                case Nbr.nbr12: return achievement_db.nbr12;
                case Nbr.nbr13: return achievement_db.nbr13;
                case Nbr.nbr14: return achievement_db.nbr14;
                case Nbr.nbr15: return achievement_db.nbr15;
                case Nbr.nbr16: return achievement_db.nbr16;
                case Nbr.nbr17: return achievement_db.nbr17;
                case Nbr.nbr18: return achievement_db.nbr18;
                case Nbr.nbr19: return achievement_db.nbr19;
                case Nbr.nbr20: return achievement_db.nbr20;
                case Nbr.nbr21: return achievement_db.nbr21;
                case Nbr.nbr22: return achievement_db.nbr22;
                case Nbr.nbr23: return achievement_db.nbr23;
            }

            return default;
        }

        void SetUpdate(Nbr nbr, AchievementsDB.JsonDB db)
        {
            switch (nbr)
            {
                case Nbr.nbr0: achievement_db.nbr0 = db; break;
                case Nbr.nbr1: achievement_db.nbr1 = db; break;
                case Nbr.nbr2: achievement_db.nbr2 = db; break;
                case Nbr.nbr3: achievement_db.nbr3 = db; break;
                case Nbr.nbr4: achievement_db.nbr4 = db; break;
                case Nbr.nbr5: achievement_db.nbr5 = db; break;
                case Nbr.nbr6: achievement_db.nbr6 = db; break;
                case Nbr.nbr7: achievement_db.nbr7 = db; break;
                case Nbr.nbr8: achievement_db.nbr8 = db; break;
                case Nbr.nbr9: achievement_db.nbr9 = db; break;
                case Nbr.nbr10: achievement_db.nbr10 = db; break;
                case Nbr.nbr11: achievement_db.nbr11 = db; break;
                case Nbr.nbr12: achievement_db.nbr12 = db; break;
                case Nbr.nbr13: achievement_db.nbr13 = db; break;
                case Nbr.nbr14: achievement_db.nbr14 = db; break;
                case Nbr.nbr15: achievement_db.nbr15 = db; break;
                case Nbr.nbr16: achievement_db.nbr16 = db; break;
                case Nbr.nbr17: achievement_db.nbr17 = db; break;
                case Nbr.nbr18: achievement_db.nbr18 = db; break;
                case Nbr.nbr19: achievement_db.nbr19 = db; break;
                case Nbr.nbr20: achievement_db.nbr20 = db; break;
                case Nbr.nbr21: achievement_db.nbr21 = db; break;
                case Nbr.nbr22: achievement_db.nbr22 = db; break;
                case Nbr.nbr23: achievement_db.nbr23 = db; break;
            }
        }

        List<Nbr> sortChartDBKey = new List<Nbr>();
        public Nbr GetSortIndex(int idx)
        {
            return sortChartDBKey[idx];
        }
        
        public int GetLastAchieLv(Nbr nbrKey)
        {
            int last = _chartDB.dic_cdb_achievements[(int)nbrKey].Count - 1;
            return _chartDB.dic_cdb_achievements[(int)nbrKey][last].lv;
        }

        /// <summary>
        /// 진행 완료 / 진행중 / 보상 받기 완료 
        /// </summary>
        public void Sort()
        {
            sortChartDBKey.Clear();
            foreach (var nbrKey in _chartDB.dic_cdb_achievements.Keys)
            {
                var cmp_indx = _chartDB.dic_cdb_achievements[nbrKey].FindIndex(x => Get((Nbr)nbrKey).progCnt >= x.prog_cmp_cnt && Get((Nbr)nbrKey).progLv <= x.lv);
                if (cmp_indx >= 0)
                {
                    sortChartDBKey.Add((Nbr)nbrKey);
                }
            }
            foreach (var nbrKey in _chartDB.dic_cdb_achievements.Keys)
            {
                var ing_indx = _chartDB.dic_cdb_achievements[nbrKey].FindIndex(x => Get((Nbr)nbrKey).progCnt < x.prog_cmp_cnt && Get((Nbr)nbrKey).progLv <= x.lv);
                if (ing_indx >= 0)
                {
                    if(sortChartDBKey.Contains((Nbr)nbrKey) == false)
                        sortChartDBKey.Add((Nbr)nbrKey);
                }
            }
            foreach (var nbrKey in _chartDB.dic_cdb_achievements.Keys)
            {
                var end_indx = _chartDB.dic_cdb_achievements[nbrKey].FindIndex(x => Get((Nbr)nbrKey).progLv > GetLastAchieLv((Nbr)nbrKey));
                if (end_indx >= 0)
                {
                    if(sortChartDBKey.Contains((Nbr)nbrKey) == false)
                        sortChartDBKey.Add((Nbr)nbrKey);
                }
            }
        }

        /// <summary>
        /// 업적 카운트 증가 
        /// </summary>
        public async Task ASetInCount(Nbr nbr, long inCnt, bool increase = true, bool isDirectSend = false)
        {
            var db = this.Get(nbr);
            long cdb_progCompCnt = _chartDB.dic_cdb_achievements[(int)nbr].Find(x => x.nbr == (int)nbr && x.lv == db.progLv).prog_cmp_cnt;
            bool db_compCnt = db.progCnt >= cdb_progCompCnt;

            if (increase == true)
                db.progCnt += inCnt;
            else
            {
                if (nbr == Nbr.nbr11 || nbr == Nbr.nbr12 || nbr == Nbr.nbr13 || nbr == Nbr.nbr14 || nbr == Nbr.nbr15 || nbr == Nbr.nbr16 || nbr == Nbr.nbr17)
                {
                    // 장비 도감 강화 업적 
                    switch (nbr)
                    {
                        case Nbr.nbr11: db.progCnt = GameDatabase.GetInstance().equipmentEncyclopediaDB.GetEncyProgressEnhantLevel(1); break; // 
                        case Nbr.nbr12: db.progCnt = GameDatabase.GetInstance().equipmentEncyclopediaDB.GetEncyProgressEnhantLevel(2); break; // 
                        case Nbr.nbr13: db.progCnt = GameDatabase.GetInstance().equipmentEncyclopediaDB.GetEncyProgressEnhantLevel(3); break; // 
                        case Nbr.nbr14: db.progCnt = GameDatabase.GetInstance().equipmentEncyclopediaDB.GetEncyProgressEnhantLevel(4); break; // 
                        case Nbr.nbr15: db.progCnt = GameDatabase.GetInstance().equipmentEncyclopediaDB.GetEncyProgressEnhantLevel(5); break; // 
                        case Nbr.nbr16: db.progCnt = GameDatabase.GetInstance().equipmentEncyclopediaDB.GetEncyProgressEnhantLevel(6); break; // 
                        case Nbr.nbr17: db.progCnt = GameDatabase.GetInstance().equipmentEncyclopediaDB.GetEncyProgressEnhantLevel(7); break; // 
                    }
                }
                else
                {
                    if (db.progCnt < inCnt)
                    {
                        db.progCnt = inCnt;
                    }
                }
            }

            this.SetUpdate((Nbr)nbr, db);
            //PlayerPrefs.SetString(PrefsKeys.prky_achievement(nbr.ToString()), db.progCnt.ToString());
            PopUpMng.GetInstance().Refresh_Achievemet();

            if (db_compCnt == false || isDirectSend == true)
            {
                // 완료 카운트 충족시 서버로 1회 전송 
                if (db.progCnt >= cdb_progCompCnt && !int.Equals(nbr, (int)Nbr.nbr1))
                {
                    LogPrint.EditorPrint("---------------- A SetInCount Achievements" + nbr + ", inCnt : " + inCnt + ", increase : " + increase + ", isDirectSend : " + isDirectSend);
                    Param pram = new Param();
                    pram.Add(nbr.ToString(), JsonUtility.ToJson(db));
                    BackendReturnObject bro1 = null;
                    SendQueue.Enqueue(Backend.GameInfo.Update, BackendGpgsMng.tableName_Achievements, achievement_db.indate, pram, callback => { bro1 = callback; });
                    while (bro1 == null) { await Task.Delay(100); }
                    LogPrint.EditorPrint("---------------- A SetInCount Achievements " + bro1);
                }
            }

            NotificationIcon.GetInstance().CheckNoticeAchievement();
        }

        public async Task AReward(Nbr nbr, MailDB.Item item)
        {
            var db = Get(nbr);
            if(_chartDB.dic_cdb_achievements[(int)nbr].Find(x => x.nbr == (int)nbr && x.lv == db.progLv).rwd_reset)
            {
                db.progCnt = 0;
            }
                
            db.progLv++;
            if (nbr == Nbr.nbr18)
            {
                db.progCnt = GameDatabase.GetInstance().questDB.GetLevel(db.progLv);
            }

            Param pram = new Param();
            pram.Add(nbr.ToString(), JsonUtility.ToJson(db));

            LogPrint.Print("----------------A SetReward Achievements");
            BackendReturnObject bro1 = null;
            SendQueue.Enqueue(Backend.GameInfo.Update, BackendGpgsMng.tableName_Achievements, achievement_db.indate, pram, callback => { bro1 = callback; });
            while (Loading.Bottom(bro1) == false) { await Task.Delay(100); }
            LogPrint.Print("----------------A SetReward Achievements " + bro1);

            await GetInstance().mailDB.ASetGiftRewarded(item);
            SetUpdate(nbr, db);
        }

        public bool GetNoticeCompleteCheck()
        {
            for (int nbr = 0; nbr < _chartDB.dic_cdb_achievements.Count; nbr++)
            {
                var db = Get((Nbr)nbr);
                int lastLv = GetLastAchieLv((Nbr)nbr);
                bool isRwdComp = db.progLv > lastLv;
                var cdb = GameDatabase.GetInstance().chartDB.dic_cdb_achievements[(int)nbr][isRwdComp == true ? lastLv : db.progLv];
                if (!isRwdComp && db.progCnt >= cdb.prog_cmp_cnt)
                    return true;
            }

            return false;
        }
    }

    /// <summary>
    /// ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
    /// #미션 (일일)
    /// ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
    /// </summary>
    public class DailyMissionDB 
    {
        public enum Nbr
        {
            /// <summary> 일일 미션 : 출석 체크하기 </summary>
            nbr0,
            nbr1,
            nbr2,
            nbr3,
            nbr4,
            nbr5,
            nbr6,
            nbr7,
            nbr8,
            nbr9,
            nbr10,
            nbr11,
            nbr12,
        }

        private DailyMission daily_mission_db = new DailyMission();
        [System.Serializable]
        public struct DailyMission
        {
            public string indate;
            public string dailyYmd;
            public AchievementsDB.JsonDB nbr0;
            public AchievementsDB.JsonDB nbr1;
            public AchievementsDB.JsonDB nbr2;
            public AchievementsDB.JsonDB nbr3;
            public AchievementsDB.JsonDB nbr4;
            public AchievementsDB.JsonDB nbr5;
            public AchievementsDB.JsonDB nbr6;
            public AchievementsDB.JsonDB nbr7;
            public AchievementsDB.JsonDB nbr8;
            public AchievementsDB.JsonDB nbr9;
            public AchievementsDB.JsonDB nbr10;
            public AchievementsDB.JsonDB nbr11;
            public AchievementsDB.JsonDB nbr12;
        }

        //public struct JsonDB
        //{
        //    public int progLv; // 보상 완료 (받기완료 1, 받기 미완료 0) 
        //    public int progCnt;
        //}

        public string GetInDate() => daily_mission_db.indate;
        public int GetChartCount() => _chartDB.list_cdb_daily_mission.Count;
        public Param GetParamAddDailyMission()
        {
            Param parm = new Param();
            for(int i = 0; i < GetChartCount(); i++)
                parm.Add(string.Format("nbr{0}", i), JsonUtility.ToJson(Get((Nbr)i)));

            return parm;
        }

        public void SetInitialDataSetting(DailyMission a) => daily_mission_db = a;
        public void Set(JsonData load_rows)
        {
            if (load_rows != null)
            {
                if (load_rows.Keys.Contains("rows"))
                {
                    string defaultJDb = JsonUtility.ToJson(new AchievementsDB.JsonDB());
                    JsonData row = load_rows["rows"][0];
                    daily_mission_db = new DailyMission()
                    {
                        indate = RowPaser.StrPaser(row, "inDate"),
                        dailyYmd = RowPaser.StrPaser(row, "dailyYmd"),
                        nbr0 = JsonUtility.FromJson<AchievementsDB.JsonDB>(string.IsNullOrEmpty(RowPaser.StrPaser(row, "nbr0")) ? defaultJDb : RowPaser.StrPaser(row, "nbr0")),
                        nbr1 = JsonUtility.FromJson<AchievementsDB.JsonDB>(string.IsNullOrEmpty(RowPaser.StrPaser(row, "nbr1")) ? defaultJDb : RowPaser.StrPaser(row, "nbr1")),
                        nbr2 = JsonUtility.FromJson<AchievementsDB.JsonDB>(string.IsNullOrEmpty(RowPaser.StrPaser(row, "nbr2")) ? defaultJDb : RowPaser.StrPaser(row, "nbr2")),
                        nbr3 = JsonUtility.FromJson<AchievementsDB.JsonDB>(string.IsNullOrEmpty(RowPaser.StrPaser(row, "nbr3")) ? defaultJDb : RowPaser.StrPaser(row, "nbr3")),
                        nbr4 = JsonUtility.FromJson<AchievementsDB.JsonDB>(string.IsNullOrEmpty(RowPaser.StrPaser(row, "nbr4")) ? defaultJDb : RowPaser.StrPaser(row, "nbr4")),
                        nbr5 = JsonUtility.FromJson<AchievementsDB.JsonDB>(string.IsNullOrEmpty(RowPaser.StrPaser(row, "nbr5")) ? defaultJDb : RowPaser.StrPaser(row, "nbr5")),
                        nbr6 = JsonUtility.FromJson<AchievementsDB.JsonDB>(string.IsNullOrEmpty(RowPaser.StrPaser(row, "nbr6")) ? defaultJDb : RowPaser.StrPaser(row, "nbr6")),
                        nbr7 = JsonUtility.FromJson<AchievementsDB.JsonDB>(string.IsNullOrEmpty(RowPaser.StrPaser(row, "nbr7")) ? defaultJDb : RowPaser.StrPaser(row, "nbr7")),
                        nbr8 = JsonUtility.FromJson<AchievementsDB.JsonDB>(string.IsNullOrEmpty(RowPaser.StrPaser(row, "nbr8")) ? defaultJDb : RowPaser.StrPaser(row, "nbr8")),
                        nbr9 = JsonUtility.FromJson<AchievementsDB.JsonDB>(string.IsNullOrEmpty(RowPaser.StrPaser(row, "nbr9")) ? defaultJDb : RowPaser.StrPaser(row, "nbr9")),
                        nbr10 = JsonUtility.FromJson<AchievementsDB.JsonDB>(string.IsNullOrEmpty(RowPaser.StrPaser(row, "nbr10")) ? defaultJDb : RowPaser.StrPaser(row, "nbr10")),
                        nbr11 = JsonUtility.FromJson<AchievementsDB.JsonDB>(string.IsNullOrEmpty(RowPaser.StrPaser(row, "nbr11")) ? defaultJDb : RowPaser.StrPaser(row, "nbr11")),
                        nbr12 = JsonUtility.FromJson<AchievementsDB.JsonDB>(string.IsNullOrEmpty(RowPaser.StrPaser(row, "nbr12")) ? defaultJDb : RowPaser.StrPaser(row, "nbr12")),
                    };

                    for (int i = 0; i < this.GetChartCount(); i++)
                    {
                        var db = Get((Nbr)i);
                        if (db.progCnt == 0)
                        {
                            string strCnt = PlayerPrefs.GetString(PrefsKeys.prky_daily_mission(((Nbr)i).ToString()));
                            db.progCnt = System.Convert.ToInt64(string.IsNullOrEmpty(strCnt) ? "0" : strCnt);
                            SetUpdate((Nbr)i, db);
                        }
                    }
                }
            }
            else Debug.Log("contents has no data");
        }
        
        public AchievementsDB.JsonDB Get(Nbr nbr)
        {
            switch (nbr)
            {
                case Nbr.nbr0: return daily_mission_db.nbr0;
                case Nbr.nbr1: return daily_mission_db.nbr1;
                case Nbr.nbr2: return daily_mission_db.nbr2;
                case Nbr.nbr3: return daily_mission_db.nbr3;
                case Nbr.nbr4: return daily_mission_db.nbr4;
                case Nbr.nbr5: return daily_mission_db.nbr5;
                case Nbr.nbr6: return daily_mission_db.nbr6;
                case Nbr.nbr7: return daily_mission_db.nbr7;
                case Nbr.nbr8: return daily_mission_db.nbr8;
                case Nbr.nbr9: return daily_mission_db.nbr9;
                case Nbr.nbr10: return daily_mission_db.nbr10;
                case Nbr.nbr11: return daily_mission_db.nbr11;
                case Nbr.nbr12: return daily_mission_db.nbr12;
            }

            return default;
        }

        void SetUpdate(Nbr nbr, AchievementsDB.JsonDB db)
        {
            switch (nbr)
            {
                case Nbr.nbr0: daily_mission_db.nbr0 = db; break;
                case Nbr.nbr1: daily_mission_db.nbr1 = db; break;
                case Nbr.nbr2: daily_mission_db.nbr2 = db; break;
                case Nbr.nbr3: daily_mission_db.nbr3 = db; break;
                case Nbr.nbr4: daily_mission_db.nbr4 = db; break;
                case Nbr.nbr5: daily_mission_db.nbr5 = db; break;
                case Nbr.nbr6: daily_mission_db.nbr6 = db; break;
                case Nbr.nbr7: daily_mission_db.nbr7 = db; break;
                case Nbr.nbr8: daily_mission_db.nbr8 = db; break;
                case Nbr.nbr9: daily_mission_db.nbr9 = db; break;
                case Nbr.nbr10: daily_mission_db.nbr10 = db; break;
                case Nbr.nbr11: daily_mission_db.nbr11 = db; break;
                case Nbr.nbr12: daily_mission_db.nbr12 = db; break;
            }
        }

        /// <summary>
        /// 일일 미션 리셋
        /// </summary>
        public async Task<bool> ResetDailyMission()
        {
            string _indate = daily_mission_db.indate;
            string nwYmd = GetInstance().attendanceDB.DailyYmd();
            string inYmd = daily_mission_db.dailyYmd;
            if(string.Equals(nwYmd, inYmd) == false && !string.IsNullOrEmpty(_indate))
            {
                int mCnt = GetInstance().dailyMissionDB.GetChartCount();
                for (int i = 0; i < mCnt; i++)
                {
                    Nbr nbr = (Nbr)i;
                    PlayerPrefs.DeleteKey(PrefsKeys.prky_daily_mission(nbr.ToString()));
                }

                daily_mission_db = new DailyMission() { indate = _indate, dailyYmd = nwYmd };
                Task tsk1 = BackendGpgsMng.GetInstance().ASetResetDailyMission(nwYmd);
                while (Loading.Bottom(tsk1.IsCompleted) == false) await Task.Delay(100);
                PopUpMng.GetInstance().Refresh_Achievemet();
            }

            return true;
        }

        public void Sort()
        {
            var comp = _chartDB.list_cdb_daily_mission.FindAll(x => Get((Nbr)x.nbr).progCnt >= x.prog_cmp_cnt && Get((Nbr)x.nbr).progLv <= x.lv);
            var ing = _chartDB.list_cdb_daily_mission.FindAll(x => Get((Nbr)x.nbr).progCnt < x.prog_cmp_cnt && Get((Nbr)x.nbr).progLv <= x.lv);
            var end = _chartDB.list_cdb_daily_mission.FindAll(x => Get((Nbr)x.nbr).progLv > x.lv);
            _chartDB.list_cdb_daily_mission.Clear();

            foreach (var item in comp) _chartDB.list_cdb_daily_mission.Add(item);
            foreach (var item in ing)  _chartDB.list_cdb_daily_mission.Add(item);
            foreach (var item in end)  _chartDB.list_cdb_daily_mission.Add(item);
        }

        /// <summary>
        /// 미션 카운트 증가 
        /// </summary>
        public async Task ASetInCount(Nbr nbr, int inCnt)
        {
            var db = this.Get((Nbr)nbr);
            long cdb_progCompCnt = _chartDB.list_cdb_daily_mission.Find(x => x.nbr == (int)nbr).prog_cmp_cnt;
            bool db_compCnt = db.progCnt >= cdb_progCompCnt;
            db.progCnt += inCnt;

            this.SetUpdate((Nbr)nbr, db);
            PlayerPrefs.SetString(PrefsKeys.prky_daily_mission(nbr.ToString()), db.progCnt.ToString());
            PopUpMng.GetInstance().Refresh_Achievemet();

            if (db_compCnt == false)
            {
                // 완료 카운트 충족시 서버로 1회 전송 
                if (db.progCnt >= cdb_progCompCnt)
                {
                    LogPrint.EditorPrint("---------------- A SetInCount DailyMission");
                    Param pram = new Param();
                    pram.Add(nbr.ToString(), JsonUtility.ToJson(db));
                    BackendReturnObject bro1 = null;
                    SendQueue.Enqueue(Backend.GameInfo.Update, BackendGpgsMng.tableName_DailyMission, daily_mission_db.indate, pram, callback => { bro1 = callback; });
                    while (bro1 == null) { await Task.Delay(100); }
                    LogPrint.EditorPrint("---------------- A SetInCount DailyMission " + bro1);
                    
                }
            }

            NotificationIcon.GetInstance().CheckNoticeDailyMission();
        }

        /// <summary>
        /// 완료 보상 받기 
        /// </summary>
        public async Task AReward(Nbr nbr, MailDB.Item item)
        {
            var db = Get(nbr);
            db.progLv++;
            Param pram = new Param();
            pram.Add(nbr.ToString(), JsonUtility.ToJson(db));

            LogPrint.Print("----------------A SetReward DailyMission");
            BackendReturnObject bro1 = null;
            SendQueue.Enqueue(Backend.GameInfo.Update, BackendGpgsMng.tableName_DailyMission, daily_mission_db.indate, pram, callback => { bro1 = callback; });
            while (Loading.Bottom(bro1) == false) { await Task.Delay(100); }
            LogPrint.Print("----------------A SetReward DailyMission " + bro1);

            await GetInstance().mailDB.ASetGiftRewarded(item);
            SetUpdate(nbr, db);
        }

        public bool GetNoticeCompleteCheck()
        {
            foreach (var m_cdb in _chartDB.list_cdb_daily_mission)
            {
                var db = this.Get((Nbr)m_cdb.nbr);
                if (db.progCnt >= m_cdb.prog_cmp_cnt && db.progLv <= m_cdb.lv)
                    return true;
            }

            return false;
        }
    }

    /// <summary>
    /// ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
    /// #퀘스트 
    /// ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
    /// </summary>
    public class QuestDB
    {
        private Quest quest = new Quest();
        [System.Serializable]
        public struct Quest
        {
            public string indate;
            public int nbr0;
            public int nbr1;
            public int nbr2;
            public int nbr3;
            public int nbr4;
            public int nbr5;
            public int nbr6;
            public int nbr7;
            public int nbr8;
            public int nbr9;
            public int nbr10;
            public int nbr11;
            public int nbr12;
            public int nbr13;
            public int nbr14;
            public int nbr15;
            public int nbr16;
            public int nbr17;
            public int nbr18;
            public int nbr19;
            public int nbr20;
            public int nbr21;
            public int nbr22;
            public int nbr23;
            public int nbr24;
            public int nbr25;
            public int nbr26;
            public int nbr27;
            public int nbr28;
            public int nbr29;
        }

        /// <summary> 퀘스트 카운트 </summary>
        public int GetAllCount => _chartDB.cdb_quest.Count;
        public string GetInDate => quest.indate;

        /// <summary> 서버에서 로드한 데이터 </summary>
        public void Set(JsonData load_rows)
        {
            if (load_rows != null)
            {
                if (load_rows.Keys.Contains("rows"))
                {
                    JsonData row = load_rows["rows"][0];
                    List<int> nbrValue = new List<int>();
                    for (int i = 0; i < 30; i++)
                        nbrValue.Add(RowPaser.IntPaser(row, string.Format("nbr{0}", i)));

                    quest = new Quest()
                    {
                        indate = RowPaser.StrPaser(row, "inDate"),
                        nbr0 = nbrValue[0] > 1000 ? 1000 : nbrValue[0],
                        nbr1 = nbrValue[1] > 1000 ? 1000 : nbrValue[1],
                        nbr2 = nbrValue[2] > 1000 ? 1000 : nbrValue[2],
                        nbr3 = nbrValue[3] > 1000 ? 1000 : nbrValue[3],
                        nbr4 = nbrValue[4] > 1000 ? 1000 : nbrValue[4],
                        nbr5 = nbrValue[5] > 1000 ? 1000 : nbrValue[5],
                        nbr6 = nbrValue[6] > 1000 ? 1000 : nbrValue[6],
                        nbr7 = nbrValue[7] > 1000 ? 1000 : nbrValue[7],
                        nbr8 = nbrValue[8] > 1000 ? 1000 : nbrValue[8],
                        nbr9 = nbrValue[9] > 1000 ? 1000 : nbrValue[9],
                        nbr10 = nbrValue[10] > 1000 ? 1000 : nbrValue[10],
                        nbr11 = nbrValue[11] > 1000 ? 1000 : nbrValue[11],
                        nbr12 = nbrValue[12] > 1000 ? 1000 : nbrValue[12],
                        nbr13 = nbrValue[13] > 1000 ? 1000 : nbrValue[13],
                        nbr14 = nbrValue[14] > 1000 ? 1000 : nbrValue[14],
                        nbr15 = nbrValue[15] > 1000 ? 1000 : nbrValue[15],
                        nbr16 = nbrValue[16] > 1000 ? 1000 : nbrValue[16],
                        nbr17 = nbrValue[17] > 1000 ? 1000 : nbrValue[17],
                        nbr18 = nbrValue[18] > 1000 ? 1000 : nbrValue[18],
                        nbr19 = nbrValue[19] > 1000 ? 1000 : nbrValue[19],
                        nbr20 = nbrValue[20] > 1000 ? 1000 : nbrValue[20],
                        nbr21 = nbrValue[21] > 1000 ? 1000 : nbrValue[21],
                        nbr22 = nbrValue[22] > 1000 ? 1000 : nbrValue[22],
                        nbr23 = nbrValue[23] > 1000 ? 1000 : nbrValue[23],
                        nbr24 = nbrValue[24] > 1000 ? 1000 : nbrValue[24],
                        nbr25 = nbrValue[25] > 1000 ? 1000 : nbrValue[25],
                        nbr26 = nbrValue[26] > 1000 ? 1000 : nbrValue[26],
                        nbr27 = nbrValue[27] > 1000 ? 1000 : nbrValue[27],
                        nbr28 = nbrValue[28] > 1000 ? 1000 : nbrValue[28],
                        nbr29 = nbrValue[29] > 1000 ? 1000 : nbrValue[29],
                    };



                    LogPrint.Print("<color=white> Quest Set quest : " + JsonUtility.ToJson(quest) + "</color>");
                }
            }
            else LogPrint.PrintError("QuestDB Set Failled!!!!!");
        }

        /// <summary> 서버에 데이터 없을때 최초 1회 </summary>
        public void SetInitial(JsonData load_rows)
        {
            quest = new Quest()
            {
                indate = load_rows["inDate"].ToString(),
                nbr0 = 1
            };
        }

        /// <summary> 서버 전송 </summary>
        public async Task<bool> Send(int nbr, int val)
        {
            Param parm = ParamT.Collection(new ParamT.P[] { new ParamT.P() { k = string.Format("nbr{0}", nbr), v = val } });
            BackendReturnObject bro1 = null;
            SendQueue.Enqueue(Backend.GameInfo.Update, BackendGpgsMng.tableName_Quest, quest.indate, parm, callback => { bro1 = callback; });
            while (Loading.Bottom(bro1) == false) { await Task.Delay(100); }

            SetLevel(nbr, val);
            return bro1.IsSuccess();
        }

        /// <summary> 퀘스트 진행 레벨 </summary>
        public int GetLevel(int nbr)
        {
            switch (nbr)
            {
                case 0: /*return 1;*/ return quest.nbr0 <= 0 ? 1 : quest.nbr0;
                case 1: /*return 1;*/ return quest.nbr1;
                case 2: /*return 1;*/ return quest.nbr2;
                case 3: /*return 1;*/ return quest.nbr3;
                case 4: /*return 1;*/ return quest.nbr4;
                case 5: /*return 1;*/ return quest.nbr5;
                case 6: /*return 1;*/ return quest.nbr6;
                case 7: /*return 1;*/ return quest.nbr7;
                case 8: /*return 1;*/ return quest.nbr8;
                case 9: /*return 1;*/ return quest.nbr9;
                case 10: return quest.nbr10;
                case 11: return quest.nbr11;
                case 12: return quest.nbr12;
                case 13: return quest.nbr13;
                case 14: return quest.nbr14;
                case 15: return quest.nbr15;
                case 16: return quest.nbr16;
                case 17: return quest.nbr17;
                case 18: return quest.nbr18;
                case 19: return quest.nbr19;
                case 20: return quest.nbr20;
                case 21: return quest.nbr21;
                case 22: return quest.nbr22;
                case 23: return quest.nbr23;
                case 24: return quest.nbr24;
                case 25: return quest.nbr25;
                case 26: return quest.nbr26;
                case 27: return quest.nbr27;
                case 28: return quest.nbr28;
                case 29: return quest.nbr29;
                default: return -1;
            }
        }

        public void SetLevel(int nbr, int val)
        {
            LogPrint.Print("nbr : " + nbr + ", val :" + val);
            switch (nbr)
            {
                case 0:  quest.nbr0 = val; break;
                case 1:  quest.nbr1 = val; break;
                case 2:  quest.nbr2 = val; break;
                case 3:  quest.nbr3 = val; break;
                case 4:  quest.nbr4 = val; break;
                case 5:  quest.nbr5 = val; break;
                case 6:  quest.nbr6 = val; break;
                case 7:  quest.nbr7 = val; break;
                case 8:  quest.nbr8 = val; break;
                case 9:  quest.nbr9 = val; break;
                case 10: quest.nbr10 = val; break;
                case 11: quest.nbr11 = val; break;
                case 12: quest.nbr12 = val; break;
                case 13: quest.nbr13 = val; break;
                case 14: quest.nbr14 = val; break;
                case 15: quest.nbr15 = val; break;
                case 16: quest.nbr16 = val; break;
                case 17: quest.nbr17 = val; break;
                case 18: quest.nbr18 = val; break;
                case 19: quest.nbr19 = val; break;
                case 20: quest.nbr20 = val; break;
                case 21: quest.nbr21 = val; break;
                case 22: quest.nbr22 = val; break;
                case 23: quest.nbr23 = val; break;
                case 24: quest.nbr24 = val; break;
                case 25: quest.nbr25 = val; break;
                case 26: quest.nbr26 = val; break;
                case 27: quest.nbr27 = val; break;
                case 28: quest.nbr28 = val; break;
                case 29: quest.nbr29 = val; break;
            }
        }

        public Param GetAllParamQuest()
        {
            Param parm = new Param();
            for (int i = 0; i < GetAllCount; i++)
            {
                if (GetLevel(i) >= 1)
                {
                    parm.Add(string.Format("nbr{0}", i), GetLevel(i));
                }
            }

            return parm;
        }

        /// <summary> nbr 차트 </summary>
        public cdb_quest GetCdbQuest (int nbr)
        {
            int indx = _chartDB.cdb_quest.FindIndex(x => int.Equals(x.nbr, nbr));
            if (indx >= 0)
                return _chartDB.cdb_quest[indx];
            else return _chartDB.cdb_quest[_chartDB.cdb_quest.Count - 1];
        }

        /// <summary> 보상 골드 </summary>
        public long RewardGold(cdb_quest cdb, int nbr_lv)
        {
            float chpt_id = GameDatabase.GetInstance().achievementsDB.Get((GameDatabase.AchievementsDB.Nbr)1).progCnt + 1;
            long val = (long)((cdb.rwd_time * Math.Pow(nbr_lv + 1, 1.005f)) * (cdb.nbr + 1));
            return (long)(val += (long)(val * chpt_id));
        }

        /// <summary> 레벨업 골드 필요량 </summary>
        public long LevelUpGold(cdb_quest front_cdb, cdb_quest cdb, int nbr_lv)
        {
            long bfr_up_gold = cdb.nbr > 0 ? (long)((front_cdb.rwd_time * Math.Pow(101, 1.5f)) * Mathf.Pow(cdb.nbr + 1, 2.0f)) : 0;  // 이전 퀘스트의 100레벨 레벨업 골드 
            return (long)((cdb.rwd_time * Math.Pow(nbr_lv + 1, 1.5f)) * Mathf.Pow(cdb.nbr + 1, 2.0f)) + bfr_up_gold;

            //long bfr_up_gold = cdb.nbr > 0 ? (long)((front_cdb.rwd_time * Math.Pow(100, 1.5f)) * (front_cdb.nbr + 1)) : 0;  // 이전 퀘스트의 100레벨 레벨업 골드 
            //return (long)((cdb.rwd_time * Math.Pow(nbr_lv + 1, 2.0f)) * Mathf.Pow(cdb.nbr + 1, 2.0f)) + bfr_up_gold;
        }

        /// <summary>
        /// 퀘스트 100레벨 기준으로 골드보상
        /// ex) xCnt -> 1이면 각 퀘스트가 1시간 동안 벌수있는 골드 양을 모두 함친 금액 
        /// </summary>
        public long GetQuestMaxSecondRewardGold(float xCnt)
        {
            long val = 0;
            int last_qNbr = _chartDB.cdb_quest.Count - 1;
            int maxSec = _chartDB.cdb_quest[last_qNbr].rwd_time;
            int last_nbr = _chartDB.cdb_quest[last_qNbr].nbr;
            if (last_nbr >= 0)
            {
                int indx = _chartDB.cdb_quest.FindIndex(x => int.Equals(x.nbr, last_nbr));
                if (indx >= 0)
                {
                    int nbr = 0;
                    foreach (var item in _chartDB.cdb_quest)
                    {
                        if(nbr == 0)
                        {
                            int qNbrLv = GameDatabase.GetInstance().questDB.GetLevel(nbr) + 100;
                            val += (long)(RewardGold(item, GetRewardGoldLevelUnit(nbr, qNbrLv)) * (maxSec / item.rwd_time) * 0.5f);
                        }
                        else
                        {
                            int qNbrLv = GameDatabase.GetInstance().questDB.GetLevel(nbr);
                            if (qNbrLv > 0)
                                val += (long)(RewardGold(item, GetRewardGoldLevelUnit(nbr, qNbrLv)) * (maxSec / item.rwd_time) * 0.5f);
                        }

                        nbr++;
                    }
                }
            }

            return (long)(val * xCnt);
        }

        int GetRewardGoldLevelUnit(int nbr, int nbrLv)
        {
            if(nbr == 0)
            {
                if (nbrLv >= 200)
                    nbrLv = 200;
            }
            else
            {
                if (nbrLv >= 100)
                    nbrLv = 100;
            }

            return nbrLv;
            //if (nbrLv >= 1000)
            //    return 1000;
            //else if (nbrLv >= 900)
            //    return 900;
            //else if (nbrLv >= 800)
            //    return 800;
            //else if (nbrLv >= 700)
            //    return 700;
            //else if (nbrLv >= 600)
            //    return 600;
            //else if (nbrLv >= 500)
            //    return 500;
            //else if (nbrLv >= 400)
            //    return 400;
            //else if (nbrLv >= 300)
            //    return 300;
            //else if (nbrLv >= 200)
            //    return 200;
            //else if (nbrLv >= 100)
            //    return 100;
            //else
            //    return 1;
        }

        /// <summary>
        /// 퀘스트 기준 : 몬스터 처치 골드 , qst_mnst_drop_gold
        /// </summary>
        public int GetQuestMonsterDropGold(int eqRt, int eqId)
        {
            int indx = _chartDB.cdb_quest.FindIndex(obj => int.Equals(obj.eq_rt, eqRt) && int.Equals(obj.eq_id, eqId));
            if (indx >= 0)
                return _chartDB.cdb_quest[indx].qst_mnst_drop_gold;
            else return 0;
        }
        /// <summary>
        /// 퀘스트 기준 : 장비 판매 골드 , qst_sale_gold
        /// </summary>
        public int GetQuestEquipSaleGold(int eqRt, int eqId)
        {
            int indx = _chartDB.cdb_quest.FindIndex(obj => int.Equals(obj.eq_rt, eqRt) && int.Equals(obj.eq_id, eqId));
            if (indx >= 0)
                return _chartDB.cdb_quest[indx].qst_eq_sale_gold;
            else return 0;
        }
        /// <summary>
        /// 퀘스트 기준 : 장비 분해 루비 , qst_decomp_ruby
        /// </summary>
        public int GetQuestEquipDecompRuby(int eqRt, int eqId)
        {
            int indx = _chartDB.cdb_quest.FindIndex(obj => int.Equals(obj.eq_rt, eqRt) && int.Equals(obj.eq_id, eqId));
            if (indx >= 0)
                return _chartDB.cdb_quest[indx].qst_eq_decomp_ruby;
            else return 0;
        }
        /// <summary>
        /// 퀘스트 기준 : 장신구 분해 에테르 , qst_decomp_ether
        /// </summary>
        public int GetQuestEquipDecompEther(int eqRt, int eqId)
        {
            int indx = _chartDB.cdb_quest.FindIndex(obj => int.Equals(obj.eq_rt, eqRt) && int.Equals(obj.eq_id, eqId));
            if (indx >= 0)
                return _chartDB.cdb_quest[indx].qst_eq_ac_decomp_ether;
            else return 0;
        }
        /// <summary>
        ///  퀘스트 기준 : 장비 레벨 업 골드 , qst_eq_upgrade_gold
        ///  -> 사용 않함 (장비 숙련 레벨로 대체됨)
        /// </summary>
        public long GetQuestEquipLevelUpGold (int eqRt, int eqId, int eqNorLv, int eqUpCnt)
        {
            //int indx = _chartDB.cdb_quest.FindIndex(obj => int.Equals(obj.eq_rt, eqRt) && int.Equals(obj.eq_id, eqId));
            //if (indx >= 0)
            //{
            //    int blcVal = _chartDB.cdb_quest[indx].qst_eq_upgrade_gold;
            //    long val = eqUpCnt - eqNorLv > 0 ? blcVal * (eqUpCnt - eqNorLv) : 0;
            //    return val;
            //}
            
            return -1;
        }

        /// <summary>
        /// 장비 숙련도 레벨업 비용 
        /// </summary>
        public ObscuredLong GetQuestEquipProficiencyUpGold(int nowLv)
        {
            nowLv++;
            float eup_prf_lv_calc = GameDatabase.GetInstance().chartDB.GetDicBalance("equip.profic.up.gold.calc").val_float;
            int chrt_nbr_indx = _chartDB.cdb_quest.FindIndex(obj => int.Equals(obj.nbr, (int)(nowLv * 0.1f)));
            var cdb = chrt_nbr_indx == -1 ? _chartDB.cdb_quest[_chartDB.cdb_quest.Count - 1] : _chartDB.cdb_quest[chrt_nbr_indx];
            return (ObscuredLong)((((cdb.rwd_time * Math.Pow(nowLv + 1, 1.005f)) * (cdb.nbr + 1))) * (int)((nowLv * nowLv) * eup_prf_lv_calc));
        }

        /// <summary>
        ///  퀘스트 기준 : 장비 강화 골드 , qst_enhant_gold
        /// </summary>
        public long GetQuestEquipEnhantGold (int eqRt, int eqId, int eqEhtLv)
        {
            eqEhtLv++;
            int indx = _chartDB.cdb_quest.FindIndex(obj => int.Equals(obj.eq_rt, eqRt) && int.Equals(obj.eq_id, eqId));
            if (indx >= 0)
            {
                return _chartDB.cdb_quest[indx].qst_eq_enhant_gold * eqEhtLv;
            }

            return -1;
        }

        /// <summary>
        ///  퀘스트 기준 : 장비(장신구) 강화 루비or에테르 , qst_enhant_gold
        /// </summary>
        public int GetQuestEquipEnhantRubyOrEther(bool isAcc, int eqRt, int eqId)
        {
            int indx = _chartDB.cdb_quest.FindIndex(obj => int.Equals(obj.eq_rt, eqRt) && int.Equals(obj.eq_id, eqId));
            if (indx >= 0)
            {
                if (isAcc)
                    return _chartDB.cdb_quest[indx].qst_eq_ac_enhant_ether;
                else
                    return _chartDB.cdb_quest[indx].qst_eq_enhant_ruby;
            }
            else return 0;
        }
        /// <summary>
        ///  퀘스트 기준 : 강화석 진화 골드 , qst_ston_evol_gold
        /// </summary>
        public int GetQuestStonEvolutionGold(int eqRt)
        {
            int indx = _chartDB.cdb_quest.FindIndex(obj => int.Equals(obj.eq_rt, eqRt) && int.Equals(obj.eq_id, 1));
            if (indx >= 0)
                return _chartDB.cdb_quest[indx].qst_ston_evol_gold;
            else return 0;
        }
        /// <summary>
        ///  퀘스트 기준 : 장비 강화 레벨 전승 골드 , qst_enhant_transfer_gold
        /// </summary>
        public long GetQuestEquipTransferGold(int eqRt, int eqId, int eqEhtLv)
        {
            int indx = _chartDB.cdb_quest.FindIndex(obj => int.Equals(obj.eq_rt, eqRt) && int.Equals(obj.eq_id, eqId));
            if (indx >= 0)
                return _chartDB.cdb_quest[indx].qst_eq_enhant_transfer_gold * eqEhtLv;
            else return 0;
        }
        /// <summary>
        ///  퀘스트 기준 : 장비 강화 레벨 전승 다이아 , qst_eq_enhant_transfer_dia
        /// </summary>
        public int GetQuestEquipTransferDia(int eqRt, int eqId)
        {
            int indx = _chartDB.cdb_quest.FindIndex(obj => int.Equals(obj.eq_rt, eqRt) && int.Equals(obj.eq_id, eqId));
            if (indx >= 0)
                return _chartDB.cdb_quest[indx].qst_eq_enhant_transfer_dia;
            else return 0;
        }

        /// <summary>
        ///  퀘스트 기준 :장신구 옵션 변경 골드 , qst_acc_op_change_gold
        /// </summary>
        public long GetQuestAcceOptionChangeGold (int eqRt, int eqId)
        {
            int indx = _chartDB.cdb_quest.FindIndex(obj => int.Equals(obj.eq_rt, eqRt) && int.Equals(obj.eq_id, eqId));
            if (indx >= 0)
                return _chartDB.cdb_quest[indx].qst_eq_ac_op_change_gold * eqRt;
            else return 0;
        }

        /// <summary>
        ///  퀘스트 기준 :장신구 옵션 변경 다이아 , qst_acc_op_change_gold
        /// </summary>
        public int GetQuestAcceOptionChangeDia(int eqRt, int eqId)
        {
            int indx = _chartDB.cdb_quest.FindIndex(obj => int.Equals(obj.eq_rt, eqRt) && int.Equals(obj.eq_id, eqId));
            if (indx >= 0)
                return _chartDB.cdb_quest[indx].qst_eq_ac_op_change_dia;
            else return 0;
        }


        /// <summary>
        ///  퀘스트 기준 :전설 진화 장비 옵션 변경 골드 , qst_acc_op_change_gold
        /// </summary>
        public long GetQuestEquipOptionChangeGold(int eqRt, int eqId)
        {
            int indx = _chartDB.cdb_quest.FindIndex(obj => int.Equals(obj.eq_rt, eqRt) && int.Equals(obj.eq_id, eqId));
            if (indx >= 0)
                return _chartDB.cdb_quest[indx].qst_eq_op_change_gold * eqRt;
            else return 0;
        }

        /// <summary>
        ///  퀘스트 기준 :전설 진화 장비 옵션, 전용 옵션 변경 다이아 , 
        /// </summary>
        public int GetQuestEquipOptionChangeDia(int eqRt, int eqId, bool isSpOp)
        {
            if (isSpOp)
            {
                int indx = _chartDB.cdb_quest.FindIndex(obj => int.Equals(obj.eq_rt, eqRt) && int.Equals(obj.eq_id, eqId));
                if (indx >= 0)
                    return _chartDB.cdb_quest[indx].qst_eq_sop_change_dia;
                else return 0;
            }
            else
            {
                int indx = _chartDB.cdb_quest.FindIndex(obj => int.Equals(obj.eq_rt, eqRt) && int.Equals(obj.eq_id, eqId));
                if (indx >= 0)
                    return _chartDB.cdb_quest[indx].qst_eq_op_change_dia;
                else return 0;
            }
        }

        /// <summary>
        ///  퀘스트 기준 :장신구 합성/승급 골드 , qst_acc_synt_gold
        /// </summary>
        public long GetQuestEquipAcceSyntAdvcGold(int eqRt, int eqId)
        {
            int indx = _chartDB.cdb_quest.FindIndex(obj => int.Equals(obj.eq_rt, eqRt) && int.Equals(obj.eq_id, eqId));
            if (indx >= 0)
                return _chartDB.cdb_quest[indx].qst_eq_ac_synt_gold;
            else return 0;
        }

        /// <summary>
        ///  퀘스트 기준 :장신구 합성/승급 다이아 , qst_acc_synt_dia 
        /// </summary>
        public int GetQuestEquipAcceSyntAdvcTBCDia(int eqRt, int eqId)
        {
            int indx = _chartDB.cdb_quest.FindIndex(obj => int.Equals(obj.eq_rt, eqRt) && int.Equals(obj.eq_id, eqId));
            if (indx >= 0)
                return _chartDB.cdb_quest[indx].qst_eq_ac_synt_tbc_dia;
            else return 0;
        }

        /// <summary>
        ///  퀘스트 기준 : 스킬 레벨업 골드 , qst_skill_up_gold
        /// </summary>
        public long GetQuestSkillUpGold (int skRt, int skId, int skLv)
        {
            LogPrint.Print("GetQuestSkillUpGold skRt : " + skRt + ", skId : " + skId + ", skLv : " + skLv);
            int indx = _chartDB.cdb_quest.FindIndex(obj => int.Equals(obj.eq_rt, skRt) && int.Equals(obj.eq_id, skId));
            if (indx >= 0)
                return _chartDB.cdb_quest[indx].qst_skill_up_gold * skLv;
            else return 0;
        }

        public bool GetNoticeCompleteCheck()
        {
            int qLastNbr = _chartDB.cdb_quest[_chartDB.cdb_quest.Count - 1].nbr;
            for (int q_nbr = 0; q_nbr <= qLastNbr; q_nbr++)
            {
                int q_lv = GameDatabase.GetInstance().questDB.GetLevel(q_nbr);
                var cdb = GameDatabase.GetInstance().questDB.GetCdbQuest(q_nbr);
                if (q_lv < cdb.max_lv)
                {
                    cdb_quest front_cdb = q_nbr > 0 ? GameDatabase.GetInstance().questDB.GetCdbQuest(q_nbr - 1) : new cdb_quest();
                    long gold = GameDatabase.GetInstance().tableDB.GetTableDB_Goods().m_gold;
                    long price = GameDatabase.GetInstance().questDB.LevelUpGold(front_cdb, cdb, q_lv);
                    int fntNbr = q_nbr - 1;
                    int frtQstLevel = GameDatabase.GetInstance().chartDB.GetDicBalance("quest.front.level").val_int;
                    bool isFrontQuestLv100 = fntNbr < 0 ? true : GameDatabase.GetInstance().questDB.GetLevel(fntNbr) >= frtQstLevel;
                    if (isFrontQuestLv100 == true)
                    {
                        if (gold >= price)
                            return true;
                    }
                }
            }

            return false;
        }
    }

    /// <summary>
    /// ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
    /// #편의 기능 
    /// ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
    /// </summary>
    public class ConvenienceFunctionDB
    {
        /// <summary>
        /// 장비 자동 판매/분해 (결제) 종료 남은 시간 
        /// </summary> 
        public DateTime GetDate_ConvenFunAutoSale()
        {
            string sSale_date = GameDatabase.GetInstance().tableDB.GetUserInfo().m_auto_eq_sale_date;

            DateTime tryDate;
            if (DateTime.TryParse(sSale_date, out tryDate) == false)
                tryDate = BackendGpgsMng.GetInstance().GetNowTime();

            LogPrint.Print(" ################ AutoSale(결제) 종료 Date  : " + tryDate.ToString());
            return tryDate;
        }

        /// <summary>
        /// 장비 자동 판매/분해 (광고) 종료 남은 시간 
        /// </summary> 
        public DateTime GetDate_ConvenFunAutoSaleVideo()
        {
            string sSale_date = GameDatabase.GetInstance().tableDB.GetUserInfo().m_auto_eq_sale_video_date;

            DateTime tryDate;
            if (DateTime.TryParse(sSale_date, out tryDate) == false)
                tryDate = BackendGpgsMng.GetInstance().GetNowTime();

            LogPrint.Print(" ################ AutoSale(광고) 종료 Date  : " + tryDate.ToString());
            return tryDate;
        }

        public bool GetUseingConvenFunAutoSale()
        {
            DateTime nDate = BackendGpgsMng.GetInstance().GetNowTime();
            DateTime endDate = GetDate_ConvenFunAutoSale();
            if ((endDate - nDate).TotalSeconds <= 0)
                endDate = GetDate_ConvenFunAutoSaleVideo();

            return (endDate - nDate).TotalSeconds > 0;

        }

        /// <summary> 물약 등급별 체력 회복 % (일반15% 중급 30%, 고급 60%) </summary>
        public float AutoPotion_RecoveryPercent (int rt)
        {
            string blcStrKey = string.Format("potion.recovery.rt_{0}", rt);
            return GameDatabase.GetInstance().chartDB.GetDicBalance(blcStrKey).val_float;
        }
    }

    /// <summary>
    /// ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
    /// #상점
    /// ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
    /// </summary>
    public class ShopDB
    {
        List<LuckExchange> listLuckExchange = new List<LuckExchange>();
        public struct LuckExchange
        {
            public int exch_per;
            public int equip_id;
        }

        /// <summary>
        /// 조각 교환시 확률별 장비 ID 
        /// </summary>
        /// <returns></returns>
        public List<LuckExchange> GetLuckPieceExchangePerId(int rt)
        {
            listLuckExchange.Clear();

            var eqTy0_cdb = GetInstance().chartDB.list_cdb_stat.FindAll(x => x.eRating == rt);
            foreach (var item in eqTy0_cdb)
                listLuckExchange.Add(new LuckExchange() { exch_per = item.exch_per, equip_id = item.eIdx });

            return listLuckExchange;
        }
    }

    /// <summary>
    /// ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
    /// #도감
    /// ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
    /// </summary>
    public class EquipmentEncyclopediaDB
    {
        public ID_Rating idRating = ID_Rating.Rating1;
        public enum ID_Rating
        {
            Rating1 = 1,
            Rating2 = 2,
            Rating3 = 3,
            Rating4 = 4,
            Rating5 = 5,
            Rating6 = 6,
            Rating7 = 7,
        }

        private string inDate = "";
        private Dictionary<string, List<JsonDB>> dicListEncyclopedia = new Dictionary<string, List<JsonDB>>();
        List<string> changeDBKey = new List<string>();

        [System.Serializable]
        public struct JsonDB
        {
            public int ty, rt, id, cnt, eh_lv; // ins : 획득, eh_lv : 강화 레벨 
        }

        public void DefaultSetting()
        {
            LogPrint.Print("-----DefaultSetting-------");
            dicListEncyclopedia.Clear();
            for (int fRt = 1; fRt <= 7; fRt++)
            {
                List<JsonDB> temp = new List<JsonDB>();
                for (int i_typ = 0; i_typ <= 10; i_typ++)
                {
                    foreach (var item in GetInstance().chartDB.list_cdb_stat.FindAll(x => x.eRating == fRt))
                        temp.Add(new JsonDB() { ty = i_typ, rt = item.eRating, id = item.eIdx, cnt = 0, eh_lv = 0 });
                }

                dicListEncyclopedia.Add(string.Format("Rating{0}", fRt), temp);
            }
        }

        public void Set(JsonData returnData)
        {
            if (returnData != null)
            {
                if (returnData.Keys.Contains("inDate"))
                    inDate = returnData["inDate"].ToString();
                else if (returnData.Keys.Contains("rows"))
                    inDate = RowPaser.StrPaser(returnData["rows"][0], "inDate");
                else LogPrint.PrintError("도감의 indate가 null입니다.");

                if (returnData.Keys.Contains("rows"))
                {
                    //JsonUtility.FromJson<IG.PartsIdx>(RowPaser.StrPaser(ubsJd, "parts"));
                    JsonData rows = returnData["rows"][0];
                    foreach (var dbKey in rows.Keys)
                    {
                        if (dicListEncyclopedia.ContainsKey(dbKey))
                        {
                            JSONObject jsonDB = JSONObject.Create(RowPaser.StrPaser(rows, dbKey))["rows"];
                            for (int i = 0; i < jsonDB.Count; i++)
                            {
                                string[] vArr = jsonDB[i].ToString().Replace("\"", "").ToString().Split(',');
                                JsonDB addDB = new JsonDB()
                                {
                                    ty = System.Convert.ToInt32(vArr[0]),
                                    rt = System.Convert.ToInt32(vArr[1]),
                                    id = System.Convert.ToInt32(vArr[2]),
                                    cnt = System.Convert.ToInt32(vArr[3]),
                                    eh_lv = System.Convert.ToInt32(vArr[4])
                                };

                                int fIndx = dicListEncyclopedia[dbKey].FindIndex(x => int.Equals(x.ty, addDB.ty) && int.Equals(x.rt, addDB.rt) && int.Equals(x.id, addDB.id));
                                if (fIndx >= 0)
                                {
                                    dicListEncyclopedia[dbKey][fIndx] = addDB;
                                }
                            }
                        }
                    }
                }
            }
            else Debug.Log("contents has no data");
        }

        /// <summary>
        /// 장비 드롭시 도감 db에 값 추가 
        /// </summary>
        public void DropAcquisitionAdded(int eqTy, int eqRt, int eqId)
        {
            string dbKey = string.Format("Rating{0}", eqRt);
            if (dicListEncyclopedia.ContainsKey(dbKey))
            {
                int dicIndex = dicListEncyclopedia[dbKey].FindIndex(obj => obj.ty == eqTy && obj.rt == eqRt && obj.id == eqId);
                if (dicIndex >= 0)
                {
                    var encyDB = dicListEncyclopedia[dbKey][dicIndex];
                    encyDB.cnt += 1;
                    UpdateClientDB(encyDB);
                    PopUpMng.GetInstance().EquipEncyclopediaTapNotice();
                    NotificationIcon.GetInstance().CheckNoticeEncylo();
                    PopUpMng.GetInstance().RefreshCells_EquipEncyclope(eqRt);
                    GameDatabase.GetInstance().dailyMissionDB.ASetInCount(GameDatabase.DailyMissionDB.Nbr.nbr3, 1); // 일일미션, nbr3 장비 획득 
                }
            }
        }
         
        public void UpdateClientDB(JsonDB db)
        {
            string dbKey = string.Format("Rating{0}", db.rt);
            if(dicListEncyclopedia.ContainsKey(dbKey))
            {
                int dicIndex = dicListEncyclopedia[dbKey].FindIndex(x => x.ty == db.ty && x.rt == db.rt && x.id == db.id);
                if (dicIndex >= 0)
                    dicListEncyclopedia[dbKey][dicIndex] = db;
            }

            if (changeDBKey.Contains(dbKey) == false)
                changeDBKey.Add(dbKey);
        }

        public async void SendUpdateTableDB(bool Async = true)
        {
            if(changeDBKey.Count > 0)
            {
                if (string.IsNullOrEmpty(inDate))
                {
                    await BackendGpgsMng.GetInstance().ASetInsertEquipmentEncyclopedia();
                }

                if (string.IsNullOrEmpty(inDate) == false)
                {
                    //Param param = new Param();
                    //foreach (var dbKey in changeDBKey)
                    //{
                    //    if (dicListEncyclopedia.ContainsKey(dbKey))
                    //    {
                    //        List<string> strDBs = new List<string>();
                    //        foreach (var fdb in dicListEncyclopedia[dbKey])
                    //        {
                    //            strDBs.Add(string.Format("{0},{1},{2},{3},{4}", fdb.ty, fdb.rt, fdb.id, fdb.cnt, fdb.eh_lv));
                    //        }

                    //        param.Add(dbKey, JsonUtility.ToJson(new Serialization<string>(strDBs)));
                    //    }
                    //}

                    var dbEncy = GameDatabase.GetInstance().equipmentEncyclopediaDB.GetAll();
                    Param param = new Param();
                    for (int fEqRt = 1; fEqRt <= 7; fEqRt++)
                    {
                        string dbKey = string.Format("Rating{0}", fEqRt);
                        if (dbEncy.ContainsKey(dbKey))
                        {
                            List<string> strDBs = new List<string>();
                            foreach (var fdb in dbEncy[dbKey])
                            {
                                string dbStr = string.Format("{0},{1},{2},{3},{4}", fdb.ty, fdb.rt, fdb.id, fdb.cnt, fdb.eh_lv);
                                strDBs.Add(dbStr);
                            }
                            param.Add(dbKey, JsonUtility.ToJson(new Serialization<string>(strDBs)));
                        }
                    }

                    LogPrint.Print("send db : " + param.GetJson());
                    if (Async)
                    {
                        BackendReturnObject bro1 = null;
                        SendQueue.Enqueue(Backend.GameInfo.Update, BackendGpgsMng.tableName_EquipmentEncyclopedia, inDate, param, callback => { bro1 = callback; });
                        while (bro1 == null) { await Task.Delay(100); }
                    }
                    else
                    {
                        Backend.GameInfo.Update(BackendGpgsMng.tableName_EquipmentEncyclopedia, inDate, param);
                    }

                    changeDBKey.Clear();
                }
                else PopUpMng.GetInstance().Open_MessageError("데이터 식별ID값 오류입니다.");
            }
        }

        /// <summary>
        /// sortId 0 : 강화 가능 순서 
        /// sortId 1 : 강화 레벨 높은 순서
        /// sortId 2 : 강화 레벨 낮은 순서 
        /// sortId 3 : 장비 순서 (무기, 방패, 헬멧........반지)
        /// </summary>
        public void Sort(int sortId)
        {
            LogPrint.EditorPrint("Sort sortId : " + sortId);
            string[] kk = { "Rating1", "Rating2", "Rating3", "Rating4", "Rating5", "Rating6", "Rating7" };
            foreach (var k in kk)
            {
                int rt = int.Parse(Regex.Replace(k.ToString(), @"\D", ""));
                switch (sortId)
                {
                    case 0:
                        dicListEncyclopedia[k].Sort((JsonDB x, JsonDB y) => x.ty.CompareTo(y.ty));
                        dicListEncyclopedia[k].Sort((JsonDB x, JsonDB y) => x.eh_lv.CompareTo(y.eh_lv));
                        for (int i = 0; i < dicListEncyclopedia[k].Count; i++)
                        {
                            var tmp = dicListEncyclopedia[k][i];
                            if (tmp.cnt >= GetNeedCount(rt) && tmp.eh_lv < GetMaxEnhantLevel())
                            {
                                int id = -1;
                                for(int y = 0; y < dicListEncyclopedia[k].Count; y++)
                                {
                                    if(dicListEncyclopedia[k][y].cnt < GetNeedCount(rt))
                                    {
                                        id = y;
                                        break;
                                    }
                                }

                                if(id >= 0)
                                {
                                    var tmp0 = dicListEncyclopedia[k][id];
                                    dicListEncyclopedia[k][id] = tmp;
                                    dicListEncyclopedia[k][i] = tmp0;
                                }
                            }
                        }
                        break;
                    case 1: 
                        dicListEncyclopedia[k].Sort((JsonDB x, JsonDB y) => y.eh_lv.CompareTo(x.eh_lv));
                        break;
                    case 2:
                        dicListEncyclopedia[k].Sort((JsonDB x, JsonDB y) => x.eh_lv.CompareTo(y.eh_lv));
                        break;
                    case 3: 
                        dicListEncyclopedia[k].Sort((JsonDB x, JsonDB y) => x.ty.CompareTo(y.ty));
                        break;
                }
            }
        }

        public int GetCount(int rt)
        {
            idRating = (ID_Rating)rt;
            return dicListEncyclopedia[string.Format("Rating{0}", rt)].Count;
        }

        public int GetNeedCount(int rt)
        {
            return (GameDatabase.GetInstance().chartDB.GetDicBalance("eq.max.rating").val_int + 1) - rt;
        }

        public int GetMaxEnhantLevel() => GameDatabase.GetInstance().chartDB.GetDicBalance("eq.ency.enhant.max.level").val_int;

        public JsonDB Get(int idx)
        {
            int rt = (int)idRating;
            string dbKey = string.Format("Rating{0}", rt);
            return dicListEncyclopedia[dbKey][idx];
        }
        public JsonDB Get(int rt, int idx)
        {
            string dbKey = string.Format("Rating{0}", rt);
            return dicListEncyclopedia[dbKey][idx];
        }

        public Dictionary<string, List<JsonDB>> GetAll() => dicListEncyclopedia; // string dbKey = string.Format("{0}", ((ID_Rating)db.rt).ToString());
        public List<JsonDB> GetRatingAll(int rt)
        {
            string dbKey = string.Format("Rating{0}", rt);
            if (dicListEncyclopedia.ContainsKey(dbKey))
                return dicListEncyclopedia[dbKey];

            return null;
        }

        public bool[] GetCheckNotifNeedCount()
        {
            int ency_max_lv = GameDatabase.GetInstance().equipmentEncyclopediaDB.GetMaxEnhantLevel();
            bool[] isBool = new bool[8];
            foreach (string key in dicListEncyclopedia.Keys)
            {
                string strTmp = Regex.Replace(key, @"\D", "");
                int nTmp = int.Parse(strTmp);
                int nedCnt = (GameDatabase.GetInstance().chartDB.GetDicBalance("eq.max.rating").val_int + 1) - nTmp;
                isBool[nTmp] = dicListEncyclopedia[key].FindIndex(x => x.cnt >= nedCnt && x.eh_lv < ency_max_lv) >= 0;
            }
            return isBool;
        }

        public bool GetNoticeCompleteCheck() 
        {
            int ency_max_lv = GameDatabase.GetInstance().equipmentEncyclopediaDB.GetMaxEnhantLevel();
            foreach (string key in dicListEncyclopedia.Keys)
            {
                string strTmp = Regex.Replace(key, @"\D", "");
                int nTmp = int.Parse(strTmp);
                int nedCnt = (GameDatabase.GetInstance().chartDB.GetDicBalance("eq.max.rating").val_int + 1) - nTmp;
                if (dicListEncyclopedia[key].FindIndex(x => x.cnt >= nedCnt && x.eh_lv < ency_max_lv) >= 0)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 해당 등급 장비 도감 총 레벨
        /// </summary>
        public int GetEncyProgressEnhantLevel(int rt, int ach_lv = -1)
        {
            int encyMaxLv = GameDatabase.GetInstance().chartDB.GetDicBalance(string.Format("encyclope.max.lv{0}", ach_lv >= 0 ? ach_lv : 0)).val_int;
            var getEncycloDB = GetInstance().equipmentEncyclopediaDB.GetRatingAll(rt);
            if(getEncycloDB.Count > 0)
            {
                int nEncyEnhantLv = 0;
                var db = getEncycloDB.FindAll(x => x.eh_lv > 0);
                foreach (var item in getEncycloDB)
                {
                    //if(ach_lv == 0) // 업적 15레벨 100% 완성하기
                    //{
                    //    nEncyEnhantLv += item.eh_lv > encyMaxLv ? encyMaxLv : item.eh_lv;
                    //}
                    //else if(ach_lv == 1) // 업적 35레벨 100% 완성하기 
                    //{
                    //    nEncyEnhantLv += item.eh_lv > encyMaxLv ? encyMaxLv : item.eh_lv;
                    //}
                    //else 
                        nEncyEnhantLv += item.eh_lv > encyMaxLv ? encyMaxLv : item.eh_lv;
                }

                return nEncyEnhantLv;
            }

            return 0;
        }

        /// <summary>
        /// 도감 등급별 진행률 (업적 : ?)
        /// </summary>
        public float GetEncycCompletionRate(int rt)
        {
            int encyMaxLv = GameDatabase.GetInstance().tableDB.GetDicBalanceEquipMaxEnhantLevel(); //GameDatabase.GetInstance().chartDB.GetDicBalance("encyclope.max.lv").val_int;
            var getEncycloDB = GetInstance().equipmentEncyclopediaDB.GetRatingAll(rt);
            if(getEncycloDB.Count > 0)
            {
                int encyCnt = getEncycloDB.Count;
                int totalEncyEnhantLv = encyCnt * encyMaxLv;
                int nEncyEnhantLv = 0;
                var db = getEncycloDB.FindAll(x => x.eh_lv > 0);
                foreach (var item in getEncycloDB)
                {
                    nEncyEnhantLv += item.eh_lv > encyMaxLv ? encyMaxLv : item.eh_lv;
                }

                return (float)((float)nEncyEnhantLv / (float)totalEncyEnhantLv);
            }

            return 0;
        }
    }

    public static class StringFormat
    {
        // 장비 획득 채팅 메시지 
        private static string ITE_DROP_MSG = LanguageGameData.GetInstance().GetString("chat.drop.msg.equip"); // <color={0}>[{1}님이 {2} 등급 [{3}]을(를) 획득하였습니다.</color>

        public static string GetGoodsNameext(int gdsType)
        {
            switch (gdsType)
            {
                case 10: return LanguageGameData.GetInstance().GetString("goods.gold"); // 골드 
                case 11: return LanguageGameData.GetInstance().GetString("goods.dia"); // 블루 다이아 
                case 12: return LanguageGameData.GetInstance().GetString("goods.ether"); // 에테르 
                case 13: return LanguageGameData.GetInstance().GetString("goods.ruby"); // 루비 
            }

            return "";
        }

        public static string GetGoodsDescriptText(int gdsType)
        {
            switch (gdsType)
            {
                case 10: return LanguageGameData.GetInstance().GetString("goods.descript.gold"); // 골드 
                case 11: return LanguageGameData.GetInstance().GetString("goods.descript.dia"); // 블루 다이아 
                case 12: return LanguageGameData.GetInstance().GetString("goods.descript.ether"); // 에테르 
                case 13: return LanguageGameData.GetInstance().GetString("goods.descript.ruby"); // 루비 
            }

            return "";
        }

        public static string ChatEquipDropText(string nickName, int ty, int rt, int id, bool isColor)
        {
            string hexColor = ResourceDatabase.GetInstance().GetItemHexColor(rt);
            string equipRating = LanguageGameData.GetInstance().GetString(string.Format("item.rating.string.{0}", rt));
            string equipName = GameDatabase.StringFormat.GetEquipName(ty, rt, id);
            return string.Format(ITE_DROP_MSG, hexColor, nickName, equipRating, equipName);
        }

        /// <summary>
        /// 등급 컬러 텍스트 
        /// </summary>
        public static string GetRatingColorText(int rt, bool isBracket = false)
        {
            string strRt = LanguageGameData.GetInstance().GetString(string.Format("item.rating.string.{0}", rt));
            if (rt == 0)
                strRt = "";

            string strCr = ResourceDatabase.GetInstance().GetItemHexColor(rt);
            if (isBracket)
            {
                return string.Format("<color={0}>({1})</color>", strCr, strRt);
            }
            else
            {
                return string.Format("<color={0}>{1}</color>", strCr, strRt);
            }
        }

        public static string GetStringRating (int rt)
        {
            return LanguageGameData.GetInstance().GetString(string.Format("item.rating.string.{0}", rt));
        }

        /// <summary>  
        /// 장비 이름 
        /// </summary>
        public static string GetEquipName(int ty, int rt, int id)
        {
            string nameF = LanguageGameData.GetInstance().GetString(string.Format("equip.name.rating{0}_{1}", rt, id));
            string nameB = LanguageGameData.GetInstance().GetString(string.Format("equip.name.type{0}", ty));
            return string.Format("{0} {1}", nameF, nameB);
        }

        public static string GetEquipFrontName(int ty) => LanguageGameData.GetInstance().GetString(string.Format("equip.type.{0}", ty));

        /// <summary>
        /// 장비/장신구 조각 등급 이름 
        /// </summary>
        public static string GetPieceName(bool isAcce, int rt)
        {
            if (isAcce)
                return LanguageGameData.GetInstance().GetString(string.Format("item.name.item_29_{0}", rt));
            else
                return LanguageGameData.GetInstance().GetString(string.Format("item.name.item_28_{0}", rt));
        }

        public static string GetSkillName(int id) => LanguageGameData.GetInstance().GetString(string.Format("skill.name_{0}", id));

        /// <summary> 장비의 매인 스텟 이름 </summary>
        public static string GetEquipMainStatName(int parts_ty) => LanguageGameData.GetInstance().GetString(string.Format("stat.name.id_{0}", GameDatabase.GetInstance().tableDB.GetPartyMainStatId(parts_ty)));

        /// <summary> 장비의 타입 번호를가지고 각 스탯 이름을 리턴 </summary>
        public static string GetEquipStatName(int get_main_stat_id) => LanguageGameData.GetInstance().GetString(string.Format("stat.name.id_{0}", get_main_stat_id));
    }
}