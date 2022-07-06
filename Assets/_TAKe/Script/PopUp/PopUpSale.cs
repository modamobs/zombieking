using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BackEnd;
using static BackEnd.BackendAsyncClass;
using System.Threading.Tasks;

public class PopUpSale : MonoBehaviour
{
    private List<GameDatabase.TableDB.Equipment> clnt_sale_equips = new List<GameDatabase.TableDB.Equipment>();
    private List<GameDatabase.TableDB.Equipment> bknd_sale_equips = new List<GameDatabase.TableDB.Equipment>();

    [SerializeField] SaleData saleData = new SaleData();
    [SerializeField] SaleInfo saleInfo = new SaleInfo();

    [System.Serializable]
    class SaleData
    {
        public List<bool> isOnEquipTypes = new List<bool>() { false, false, false };
        public List<bool> isOnEquipRatings = new List<bool>() { false, false, false, false, false, false, false };
        public List<bool> isOnAcceRatings = new List<bool>() { false, false, false, false, false, false, false };
    }

    [System.Serializable]
    struct SaleInfo
    {
        public List<Toggle> togglesEquipType;
        public List<Toggle> togglesEquipRating;
        public List<Toggle> togglesAcceRating;
        public Text tx_SaleCntTxt, tx_SaleRewardGold, tx_BunhaeCntTxt, tx_BunhaeRewardRuby, tx_BunhaeRewardEther; // 판매,분해 보상 및 수량 
        public Image im_SaleBtnBg, img_BunhaeBtnBg;
        public Text tx_SaleEquipCnt_WpnShd, tx_SaleEquipCnt_Armor, tx_SaleEquipCnt_Acce; // 장비 별 판매,분해 될 수량 
    }

    bool init_toggle = false;
    public void SetData ()
    {
         init_toggle = false;
        string str_SaleInfo = PlayerPrefs.GetString(PrefsKeys.prky_auto_sale_info);
        LogPrint.PrintError("SetData str_SaleInfo : " + str_SaleInfo);

        if (str_SaleInfo.Length > 0)
            saleData = JsonUtility.FromJson<SaleData>(str_SaleInfo);
        else saleData = new SaleData();

        if (saleData.isOnEquipTypes.FindIndex((bool b) => b == true) == -1)
            for (int i = 0; i < saleData.isOnEquipTypes.Count; i++)
                saleInfo.togglesEquipType[i].isOn = false;
        else
            for (int i = 0; i < saleData.isOnEquipTypes.Count; i++)
                saleInfo.togglesEquipType[i].isOn = saleData.isOnEquipTypes[i];

        if (saleData.isOnEquipRatings.FindIndex((bool b) => b == true) == -1)
            for (int i = 0; i < saleData.isOnEquipRatings.Count; i++)
                saleInfo.togglesEquipRating[i].isOn = false;
        else
            for (int i = 0; i < saleData.isOnEquipRatings.Count; i++)
                saleInfo.togglesEquipRating[i].isOn = saleData.isOnEquipRatings[i];

        if (saleData.isOnAcceRatings.FindIndex((bool b) => b == true) == -1)
            for (int i = 0; i < saleData.isOnAcceRatings.Count; i++)
                saleInfo.togglesAcceRating[i].isOn = false;
        else
            for (int i = 0; i < saleData.isOnAcceRatings.Count; i++)
                saleInfo.togglesAcceRating[i].isOn = saleData.isOnAcceRatings[i];

        init_toggle = true;

        SaleEquipDataList();
    }

    void SaveSaleData()
    {
        string jsn_SaleData = JsonUtility.ToJson(saleData);
        PlayerPrefs.SetString(PrefsKeys.prky_auto_sale_info, jsn_SaleData);

        LogPrint.PrintError("SaveSaleData jsn_SaleData : " + jsn_SaleData);
    }

    public void Click_SelectEquipType(int nbr)
    {
        if(!init_toggle)
            return;

        saleData.isOnEquipTypes[nbr] = saleInfo.togglesEquipType[nbr].isOn;
        SaleEquipDataList();
        SaveSaleData();
    }

    public void Click_SelectEquipRating (int nbr)
    {
        if (!init_toggle)
            return;

        saleData.isOnEquipRatings[nbr] = saleInfo.togglesEquipRating[nbr].isOn;
        SaleEquipDataList();
        SaveSaleData();
    }

    public void Click_SelectAcceRating(int nbr)
    {
        if (!init_toggle)
            return;

        saleData.isOnAcceRatings[nbr] = saleInfo.togglesAcceRating[nbr].isOn;
        SaleEquipDataList();
        SaveSaleData();
    }

    int ToggleEquipType (int eq_rt)
    {
        if(eq_rt <= 1)
            return 0;
        else
        if (eq_rt >= 2 && eq_rt <= 7)
            return 1;
        else
        if (eq_rt >= 8 && eq_rt <= 10)
            return 2;

        return -1;
    }

    int ToggleRating (int eq_rt)
    {
        return eq_rt - 1;
    }

    /// <summary> 장비 판매 골드 </summary>
    int RewardGold ()
    {
        float pet_sop1_value = GameMng.GetInstance().myPZ.igp.statValue.petSpOpTotalFigures.sop1_value * 0.01f;
        int rwd = 0;
        foreach (var item in clnt_sale_equips)
            rwd += GameDatabase.GetInstance().questDB.GetQuestEquipSaleGold(item.eq_rt, item.eq_id);

        foreach (var item in bknd_sale_equips)
            rwd += GameDatabase.GetInstance().questDB.GetQuestEquipSaleGold(item.eq_rt, item.eq_id);

        int result_rwd = rwd + (int)(rwd * pet_sop1_value);

        return result_rwd;
    }

    /// <summary> 장비 분해 보상 </summary>
    int RewardRuby ()
    {
        int rwd = 0;
        foreach (var item in clnt_sale_equips)
        {
            if (!GameDatabase.GetInstance().tableDB.GetIsPartsTypeAcce(item.eq_ty))
                rwd += GameDatabase.GetInstance().questDB.GetQuestEquipDecompRuby(item.eq_rt, item.eq_id);
        }

        foreach (var item in bknd_sale_equips)
        {
            if (!GameDatabase.GetInstance().tableDB.GetIsPartsTypeAcce(item.eq_ty))
                rwd += GameDatabase.GetInstance().questDB.GetQuestEquipDecompRuby(item.eq_rt, item.eq_id);
        }

        return rwd;
    }

    /// <summary> 장신구 분해 보상 </summary>
    int RewardEther ()
    {
        int rwd = 0;
        foreach (var item in clnt_sale_equips)
        {
            if (GameDatabase.GetInstance().tableDB.GetIsPartsTypeAcce(item.eq_ty))
                rwd += GameDatabase.GetInstance().questDB.GetQuestEquipDecompEther(item.eq_rt, item.eq_id);
        }

        foreach (var item in bknd_sale_equips)
        {
            if (GameDatabase.GetInstance().tableDB.GetIsPartsTypeAcce(item.eq_ty))
                rwd += GameDatabase.GetInstance().questDB.GetQuestEquipDecompEther(item.eq_rt, item.eq_id);
        }

        return rwd;
    }


    /// <summary>
    /// 판매 데이터 및 정보 
    /// </summary>
    private void SaleEquipDataList ()
    {
        clnt_sale_equips.Clear();
        bknd_sale_equips.Clear();
        var eq_all = GameDatabase.GetInstance().tableDB.GetAllEquipment().FindAll(obj => obj.eq_rt > 0 && obj.m_state == 0 && obj.m_lck == 0);
        var clnt_eq1 = eq_all.FindAll(
            obj => string.IsNullOrEmpty(obj.indate) && ToggleEquipType(obj.eq_ty) == 0 && saleData.isOnEquipTypes[ToggleEquipType(obj.eq_ty)] == true && saleData.isOnEquipRatings[ToggleRating(obj.eq_rt)] == true);
        var clnt_eq2 = eq_all.FindAll(
            obj => string.IsNullOrEmpty(obj.indate) && ToggleEquipType(obj.eq_ty) == 1 && saleData.isOnEquipTypes[ToggleEquipType(obj.eq_ty)] == true && saleData.isOnEquipRatings[ToggleRating(obj.eq_rt)] == true);
        var clnt_ac = eq_all.FindAll(
            obj => string.IsNullOrEmpty(obj.indate) && ToggleEquipType(obj.eq_ty) == 2 && saleData.isOnEquipTypes[ToggleEquipType(obj.eq_ty)] == true && saleData.isOnAcceRatings[ToggleRating(obj.eq_rt)] == true);
        foreach (var item in clnt_eq1) clnt_sale_equips.Add(item);
        foreach (var item in clnt_eq2) clnt_sale_equips.Add(item);
        foreach (var item in clnt_ac) clnt_sale_equips.Add(item);

        var back_eq1 = eq_all.FindAll(
            obj => !string.IsNullOrEmpty(obj.indate) && ToggleEquipType(obj.eq_ty) == 0 && saleData.isOnEquipTypes[ToggleEquipType(obj.eq_ty)] == true && saleData.isOnEquipRatings[ToggleRating(obj.eq_rt)] == true);
        var back_eq2 = eq_all.FindAll(
            obj => !string.IsNullOrEmpty(obj.indate) && ToggleEquipType(obj.eq_ty) == 1 && saleData.isOnEquipTypes[ToggleEquipType(obj.eq_ty)] == true && saleData.isOnEquipRatings[ToggleRating(obj.eq_rt)] == true);
        var back_ac = eq_all.FindAll(
            obj => !string.IsNullOrEmpty(obj.indate) && ToggleEquipType(obj.eq_ty) == 2 && saleData.isOnEquipTypes[ToggleEquipType(obj.eq_ty)] == true && saleData.isOnAcceRatings[ToggleRating(obj.eq_rt)] == true);
        foreach (var item in back_eq1) bknd_sale_equips.Add(item);
        foreach (var item in back_eq2) bknd_sale_equips.Add(item);
        foreach (var item in back_ac) bknd_sale_equips.Add(item);


        // 각 장비 파츠별 수량 
        if (saleData.isOnEquipTypes[0] == true)
            saleInfo.tx_SaleEquipCnt_WpnShd.text = string.Format("x{0}", (clnt_sale_equips.FindAll((e) => ToggleEquipType(e.eq_ty) == 0).Count + bknd_sale_equips.FindAll((e) => ToggleEquipType(e.eq_ty) == 0).Count));

        if (saleData.isOnEquipTypes[1] == true)
            saleInfo.tx_SaleEquipCnt_Armor.text = string.Format("x{0}", (clnt_sale_equips.FindAll((e) => ToggleEquipType(e.eq_ty) == 1).Count + bknd_sale_equips.FindAll((e) => ToggleEquipType(e.eq_ty) == 1).Count));

        if (saleData.isOnEquipTypes[2] == true)
            saleInfo.tx_SaleEquipCnt_Acce.text = string.Format("x{0}", (clnt_sale_equips.FindAll((e) => ToggleEquipType(e.eq_ty) == 2).Count + bknd_sale_equips.FindAll((e) => ToggleEquipType(e.eq_ty) == 2).Count));
        
        int sale_cnt = clnt_sale_equips.Count + bknd_sale_equips.Count;
        string str_sale_fTxt = LanguageGameData.GetInstance().GetString("text.equip.sale.question");
        string str_bunhae_fTxt = LanguageGameData.GetInstance().GetString("text.equip.bunhae.question");
        
        // 판매 
        saleInfo.tx_SaleCntTxt.text = string.Format(str_sale_fTxt, sale_cnt);
        saleInfo.tx_SaleRewardGold.text = string.Format("{0:#,0}", RewardGold().ToString()); // 골드 
        saleInfo.im_SaleBtnBg.sprite = SpriteAtlasMng.GetInstance().GetSpriteButtonRedOrGray(sale_cnt > 0);

        // 분해 - 루비(무가,방패,방어구)
        saleInfo.tx_BunhaeCntTxt.text = string.Format(str_bunhae_fTxt, sale_cnt);
        saleInfo.tx_BunhaeRewardRuby.text = string.Format("{0:#,0}", RewardRuby().ToString());     //  루비(무기,방패,방어구)
        saleInfo.tx_BunhaeRewardEther.text = string.Format("{0:#,0}", RewardEther().ToString());     //  에테르(장신구)
        saleInfo.img_BunhaeBtnBg.sprite = SpriteAtlasMng.GetInstance().GetSpriteButtonRedOrGray(sale_cnt > 0);
    }

    /// <summary> 판매 불가 체크 </summary>
    bool IsNotForSale(bool isSale)
    {
        if (saleData.isOnEquipTypes.FindIndex((b) => b == true) == -1)
        {
            PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("판매/분해 할 장비 타입을 선택해 주세요.");
            return true;
        }
        else
        if (saleData.isOnEquipRatings.FindIndex((b) => b == true) == -1 && saleData.isOnAcceRatings.FindIndex((b) => b == true) == -1)
        {
            PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("판매/분해 할 등급을 선택해 주세요.");
            return true;
        }

        if (clnt_sale_equips.Count + bknd_sale_equips.Count == 0)
        {
            PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox(isSale == true ? "판매 할 장비가 없습니다." : "분해 할 장비가 없습니다.");
            return true;
        }

        return false;
    }

    /// <summary> 판매 </summary>
    public void Click_Sale(bool isSale)
    {

        if (IsNotForSale(isSale) == true)
            return;

        if (clnt_sale_equips.FindAll(x => x.eq_rt >= 5).Count > 0 || bknd_sale_equips.FindAll(x => x.eq_rt >= 5).Count > 0)
        {
            string txt = string.Format("{0}등급 이상의 장비 또는 장신구가 포함되어있습니다.\n<color=red>{1}</color>를 진행하시겠습니까?", GameDatabase.StringFormat.GetRatingColorText(5, false), isSale == true ? "판매" : "분해");
            if(isSale)
                PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener(txt, Listener_Sale);
            else PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener(txt, Listener_Decomp);
        }
        else
        {
            if (isSale)
                Listener_Sale();
            else Listener_Decomp();
        }
    }

    void Listener_Sale() => Listener_SaleDecomp(true);
    void Listener_Decomp() => Listener_SaleDecomp(false);

    async void Listener_SaleDecomp(bool isSale)
    {
        Loading.Full(false);
        try
        {
            List<TransactionParam> TransactionParamList = new List<TransactionParam>();
            TransactionParam tParam = new TransactionParam();
            for (int i = 0; i < bknd_sale_equips.Count; i++)
            {
                if (tParam.GetWriteValues().Count >= 10)
                {
                    TransactionParamList.Add(tParam);
                    tParam = new TransactionParam();
                }

                var tmp_db = bknd_sale_equips[i];
                tmp_db.m_state = -1;
                List<BackEnd.WRITE> writes = new List<BackEnd.WRITE> { new WRITE { Action = TransactionAction.Update, Param = ParamT.Collection(new ParamT.P[] { new ParamT.P() { k = "m_state", v = tmp_db.m_state } }) } };
                tParam.AddUpdateList(BackendGpgsMng.tableName_Equipment, tmp_db.indate, writes);
            }

            if (tParam.GetWriteValues().Count > 0)
            {
                TransactionParamList.Add(tParam);
                tParam = new TransactionParam();
            }

            foreach (var send_param in TransactionParamList)
            {
                BackendReturnObject bro = null;
                SendQueue.Enqueue(Backend.GameInfo.TransactionWrite, send_param, callback => { bro = callback; });
                while (bro != null) await Task.Delay(100);
            }

            // 클라이언트의 장비 삭제 
            if (clnt_sale_equips.Count > 0)
            {
                foreach (var db in clnt_sale_equips)
                    GameDatabase.GetInstance().tableDB.UpdateClientDB_Equip(db, true);
            }

            if (bknd_sale_equips.Count > 0)
            {
                foreach (var db in bknd_sale_equips)
                    GameDatabase.GetInstance().tableDB.UpdateClientDB_Equip(db, true);
            }

            var gd = GameDatabase.GetInstance().tableDB.GetTableDB_Goods();
            if (isSale)// 판매일 경우 
            {
                gd.m_gold += RewardGold();
                PopUpMng.GetInstance().Open_MessageNotif(string.Format("장비를 판매하여 {0:#,0} 골드 획득하였습니다.", RewardGold()));
            }
            // 분해일 경우 
            else
            {
                gd.m_ruby += RewardRuby();
                gd.m_ether += RewardEther();
                PopUpMng.GetInstance().Open_MessageNotif(string.Format("장비를 분해하여 <color=#FF0000>루비</color> {0:#,0} / <color=#00FF1E>에테르</color> {1:#,0}개를 획득하였습니다.", RewardRuby(), RewardEther()));
            }

            Task tsk1 = GameDatabase.GetInstance().tableDB.SetUpdateGoods(gd);
            while (tsk1.IsCompleted == false) await Task.Delay(100);

            GameDatabase.GetInstance().achievementsDB.ASetInCount(GameDatabase.AchievementsDB.Nbr.nbr3, clnt_sale_equips.Count + bknd_sale_equips.Count); // 업적, nbr3 장비 판매/분해!
            GameDatabase.GetInstance().dailyMissionDB.ASetInCount(GameDatabase.DailyMissionDB.Nbr.nbr12, clnt_sale_equips.Count + bknd_sale_equips.Count); // 일일미션, nbr12 장비 판매/분해 하기! 

            clnt_sale_equips.Clear();
            bknd_sale_equips.Clear();

            MainUI.GetInstance().inventory.initOnStartInventoryAll.SetInit(true);
            MainUI.GetInstance().InventoryItemTotalCountRefresh();

            GameDatabase.GetInstance().inventoryDB.CheckIsEmpty(); // 인벤토리 공간 체크 
        }
        catch (System.Exception e)
        {
            
        }
        
        Loading.Full(true);
        gameObject.SetActive(false);
    }
}
