using BackEnd;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class PopUpShopItemPurchase : MonoBehaviour
{
    
    public enum PurchaseType
    {
        None,
        EquipEnhantSton = 21,
        AcceEnhantSton = 27,
        EnhantBlessing = 22
    }

    [SerializeField] UI ui;
    [System.Serializable]
    struct UI
    {
        public PurchaseType purchaseType;
        public Text txTitle;
        public Image imIconRt1, imIconRt2, imIconRt3;
        public Text txBuyCntMaxCntRt1, txBuyCntMaxCntRt2, txBuyCntMaxCntRt3;
        public Text txPriceRt1, txPriceRt2, txPriceRt3;
        public Text txTotalPrice;
        public Image imRt1ResetBtnBg, imRt2ResetBtnBg, imRt3ResetBtnBg;
    }

    //구매 데이터 
    private BuyDB buyRt1DB = new BuyDB();   // 일반 아이템 
    private BuyDB buyRt2DB = new BuyDB();   // 중급 아이템 
    private BuyDB buyRt3DB = new BuyDB();   // 고급 아이템 
    private struct BuyDB
    {
        public int daily_buy_cnt; // 오늘 구매한 수량 
        public int daily_buy_max_cnt; // 일일 최대 구매 수량 
        public int one_price; // 구매 가격 
        public int buy_cnt; // 구매할 수량 
        public int buy_price; // 구매 비용 
    }

    bool isBuyPressOn = false;
    private Sprite spCancel, spCancelGray;
    string success_buy_eq_ston = "";
    string success_buy_ac_ston = "";
    string success_buy_eh_bles = "";

    void Awake()
    {
        spCancel = SpriteAtlasMng.GetInstance().GetSpriteCancelBtn(true);
        spCancelGray = SpriteAtlasMng.GetInstance().GetSpriteCancelBtn(false);
        success_buy_eq_ston = LanguageGameData.GetInstance().GetString("str.frm.shop.item.eq.ston.buy.success");        // [아이템] 장비 강화석 {0} x{1}개를 구매하였습니다.
        success_buy_ac_ston = LanguageGameData.GetInstance().GetString("str.frm.shop.item.ac.ston.buy.success");        // [아이템] 장신구 강화석 {0} x{1}개를 구매하였습니다.
        success_buy_eh_bles = LanguageGameData.GetInstance().GetString("str.frm.shop.item.enhant.bles.buy.success");    // [아이템] 강화 축복 주문서 {0} x{1}개를 구매하였습니다.
    }

    /// <summary>
    /// 초기화 
    /// </summary>
    public void Init(PurchaseType purType)
    {
        if (purType == PurchaseType.None)
            return;

        StopCoroutine("Routin_Rt1Buy");
        StopCoroutine("Routin_Rt2Buy");
        StopCoroutine("Routin_Rt3Buy");
        isBuyPressOn = false;

        ui.purchaseType = purType;
        buyRt1DB = new BuyDB() { buy_cnt = 0 };
        buyRt2DB = new BuyDB() { buy_cnt = 0 };
        buyRt3DB = new BuyDB() { buy_cnt = 0 };

        var userInfo_db = GameDatabase.GetInstance().tableDB.GetUserInfo();
        if (purType == PurchaseType.EquipEnhantSton)
        {
            buyRt1DB = new BuyDB()
            {
                one_price = GameDatabase.GetInstance().chartDB.GetDicBalance("shop.item.eq.enhant.ston.price.rt1").val_int,
                daily_buy_max_cnt = GameDatabase.GetInstance().chartDB.GetDicBalance("shop.item.eq.enhant.ston.max.rt1").val_int,
                daily_buy_cnt = userInfo_db.m_daily_buy_eq_ehnt_ston_rt1,
                buy_cnt = 0,
                buy_price = 0
            };
            buyRt2DB = new BuyDB()
            {
                one_price = GameDatabase.GetInstance().chartDB.GetDicBalance("shop.item.eq.enhant.ston.price.rt2").val_int,
                daily_buy_max_cnt = GameDatabase.GetInstance().chartDB.GetDicBalance("shop.item.eq.enhant.ston.max.rt2").val_int,
                daily_buy_cnt = userInfo_db.m_daily_buy_eq_ehnt_ston_rt2,
                buy_cnt = 0,
                buy_price = 0
            };
            buyRt3DB = new BuyDB()
            {
                one_price = GameDatabase.GetInstance().chartDB.GetDicBalance("shop.item.eq.enhant.ston.price.rt3").val_int,
                daily_buy_max_cnt = GameDatabase.GetInstance().chartDB.GetDicBalance("shop.item.eq.enhant.ston.max.rt3").val_int,
                daily_buy_cnt = userInfo_db.m_daily_buy_eq_ehnt_ston_rt3,
                buy_cnt = 0,
                buy_price = 0
            };
        }
        else if (purType == PurchaseType.AcceEnhantSton)
        {
            buyRt1DB = new BuyDB()
            {
                one_price = GameDatabase.GetInstance().chartDB.GetDicBalance("shop.item.ac.enhant.ston.price.rt1").val_int,
                daily_buy_max_cnt = GameDatabase.GetInstance().chartDB.GetDicBalance("shop.item.ac.enhant.ston.max.rt1").val_int,
                daily_buy_cnt = userInfo_db.m_daily_buy_ac_ehnt_ston_rt1,
                buy_cnt = 0,
                buy_price = 0
            };
            buyRt2DB = new BuyDB()
            {
                one_price = GameDatabase.GetInstance().chartDB.GetDicBalance("shop.item.ac.enhant.ston.price.rt2").val_int,
                daily_buy_max_cnt = GameDatabase.GetInstance().chartDB.GetDicBalance("shop.item.ac.enhant.ston.max.rt2").val_int,
                daily_buy_cnt = userInfo_db.m_daily_buy_ac_ehnt_ston_rt2,
                buy_cnt = 0,
                buy_price = 0
            };
            buyRt3DB = new BuyDB()
            {
                one_price = GameDatabase.GetInstance().chartDB.GetDicBalance("shop.item.ac.enhant.ston.price.rt3").val_int,
                daily_buy_max_cnt = GameDatabase.GetInstance().chartDB.GetDicBalance("shop.item.ac.enhant.ston.max.rt3").val_int,
                daily_buy_cnt = userInfo_db.m_daily_buy_ac_ehnt_ston_rt3,
                buy_cnt = 0,
                buy_price = 0
            };

        }
        else if (purType == PurchaseType.EnhantBlessing)
        {
            buyRt1DB = new BuyDB()
            {
                one_price = GameDatabase.GetInstance().chartDB.GetDicBalance("shop.item.enhant.bless.price.rt1").val_int,
                daily_buy_max_cnt = GameDatabase.GetInstance().chartDB.GetDicBalance("shop.item.enhant.bless.max.rt1").val_int,
                daily_buy_cnt = userInfo_db.m_daily_buy_ehnt_bless_rt1,
                buy_cnt = 0,
                buy_price = 0
            };
            buyRt2DB = new BuyDB()
            {
                one_price = GameDatabase.GetInstance().chartDB.GetDicBalance("shop.item.enhant.bless.price.rt2").val_int,
                daily_buy_max_cnt = GameDatabase.GetInstance().chartDB.GetDicBalance("shop.item.enhant.bless.max.rt2").val_int,
                daily_buy_cnt = userInfo_db.m_daily_buy_ehnt_bless_rt2,
                buy_cnt = 0,
                buy_price = 0
            };
            buyRt3DB = new BuyDB()
            {
                one_price = GameDatabase.GetInstance().chartDB.GetDicBalance("shop.item.enhant.bless.price.rt3").val_int,
                daily_buy_max_cnt = GameDatabase.GetInstance().chartDB.GetDicBalance("shop.item.enhant.bless.max.rt3").val_int,
                daily_buy_cnt = userInfo_db.m_daily_buy_ehnt_bless_rt3,
                buy_cnt = 0,
                buy_price = 0
            };
        }
        
        ui.txPriceRt1.text = string.Format("x{0:#,0}", buyRt1DB.one_price);
        ui.txPriceRt2.text = string.Format("x{0:#,0}", buyRt2DB.one_price);
        ui.txPriceRt3.text = string.Format("x{0:#,0}", buyRt3DB.one_price);
        BuyInfo();


        if(purType == PurchaseType.EquipEnhantSton)
        {
            ui.txTitle.text = LanguageGameData.GetInstance().GetString("shop.item.eq.enhant.ston");
        }
        else if(purType == PurchaseType.AcceEnhantSton)
        {
            ui.txTitle.text = LanguageGameData.GetInstance().GetString("shop.item.ac.enhant.ston");
        }
        else if(purType == PurchaseType.EnhantBlessing)
        {
            ui.txTitle.text = LanguageGameData.GetInstance().GetString("shop.item.enhant.blessing");
        }

        ui.imIconRt1.sprite = SpriteAtlasMng.GetInstance().GetItemSprite((int)purType, 1);
        ui.imIconRt2.sprite = SpriteAtlasMng.GetInstance().GetItemSprite((int)purType, 2);
        ui.imIconRt3.sprite = SpriteAtlasMng.GetInstance().GetItemSprite((int)purType, 3);
    }

    void BuyInfo()
    {
        ui.txBuyCntMaxCntRt1.text = string.Format("({0} / {1})", (buyRt1DB.daily_buy_cnt + buyRt1DB.buy_cnt).ToString(), buyRt1DB.daily_buy_max_cnt);
        ui.txBuyCntMaxCntRt2.text = string.Format("({0} / {1})", (buyRt2DB.daily_buy_cnt + buyRt2DB.buy_cnt).ToString(), buyRt2DB.daily_buy_max_cnt);
        ui.txBuyCntMaxCntRt3.text = string.Format("({0} / {1})", (buyRt3DB.daily_buy_cnt + buyRt3DB.buy_cnt).ToString(), buyRt3DB.daily_buy_max_cnt);

        ui.txBuyCntMaxCntRt1.color = ((buyRt1DB.buy_cnt + buyRt1DB.daily_buy_cnt) < buyRt1DB.daily_buy_max_cnt) ? Color.white : Color.red;
        ui.txBuyCntMaxCntRt2.color = ((buyRt2DB.buy_cnt + buyRt2DB.daily_buy_cnt) < buyRt2DB.daily_buy_max_cnt) ? Color.white : Color.red;
        ui.txBuyCntMaxCntRt3.color = ((buyRt3DB.buy_cnt + buyRt3DB.daily_buy_cnt) < buyRt3DB.daily_buy_max_cnt) ? Color.white : Color.red;

        int total_price = (int)(buyRt1DB.buy_price + buyRt2DB.buy_price + buyRt3DB.buy_price);
        if (total_price > 0)
            ui.txTotalPrice.text = string.Format("x{0:#,0}", total_price);
        else ui.txTotalPrice.text = "x0";

        ui.imRt1ResetBtnBg.sprite = buyRt1DB.buy_cnt > 0 ? spCancel : spCancelGray;
        ui.imRt2ResetBtnBg.sprite = buyRt2DB.buy_cnt > 0 ? spCancel : spCancelGray;
        ui.imRt3ResetBtnBg.sprite = buyRt3DB.buy_cnt > 0 ? spCancel : spCancelGray;
    }

    public void ClickReset (int rt)
    {
        if(rt == 1)
        {
            if(buyRt1DB.buy_cnt > 0)
            {
                buyRt1DB.buy_cnt = 0;
                buyRt1DB.buy_price = 0;
                BuyInfo();
            }
        }
        else if(rt == 2)
        {
            if (buyRt2DB.buy_cnt > 0)
            {
                buyRt2DB.buy_cnt = 0;
                buyRt2DB.buy_price = 0;
                BuyInfo();
            }
        }
        else if(rt == 3)
        {
            if (buyRt3DB.buy_cnt > 0)
            {
                buyRt3DB.buy_cnt = 0;
                buyRt3DB.buy_price = 0;
                BuyInfo();
            }
        }
    }

    /// <summary>
    /// 구매 
    /// </summary>
    public void DownPress_Buy(int rt)
    {
        isBuyPressOn = true;
        if(rt == 1)
        {
            if ((buyRt1DB.buy_cnt + buyRt1DB.daily_buy_cnt) < buyRt1DB.daily_buy_max_cnt)
            {
                StopCoroutine("Routin_Rt1Buy");
                StartCoroutine("Routin_Rt1Buy");
            } 
            else PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("일일 최대 구매량을 초과하였습니다.");
        }
        else if(rt == 2)
        {
            if ((buyRt2DB.buy_cnt + buyRt2DB.daily_buy_cnt) < buyRt2DB.daily_buy_max_cnt)
            {
                StopCoroutine("Routin_Rt2Buy");
                StartCoroutine("Routin_Rt2Buy");
            }
            else PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("일일 최대 구매량을 초과하였습니다.");
        }
        else if(rt == 3)
        {
            if ((buyRt3DB.buy_cnt + buyRt3DB.daily_buy_cnt) < buyRt3DB.daily_buy_max_cnt)
            {
                StopCoroutine("Routin_Rt3Buy");
                StartCoroutine("Routin_Rt3Buy");
            } 
            else PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("일일 최대 구매량을 초과하였습니다.");
        }
    }

    IEnumerator Routin_Rt1Buy()
    {
        float press_time = 0.5f;
        while (isBuyPressOn && (buyRt1DB.buy_cnt + buyRt1DB.daily_buy_cnt) < buyRt1DB.daily_buy_max_cnt)
        {
            buyRt1DB.buy_cnt++;
            buyRt1DB.buy_price = (int)(buyRt1DB.buy_cnt * buyRt1DB.one_price);
            BuyInfo();

            yield return new WaitForSeconds(press_time);
            if (press_time > 0.05f)
                press_time -= 0.2f;
            else if (press_time > 0.025f)
                press_time -= 0.05f;
            else press_time = 0.025f;
        }
    }

    IEnumerator Routin_Rt2Buy()
    {
        float press_time = 0.5f;
        while (isBuyPressOn && (buyRt2DB.buy_cnt + buyRt2DB.daily_buy_cnt) < buyRt2DB.daily_buy_max_cnt)
        {
            buyRt2DB.buy_cnt++;
            buyRt2DB.buy_price = (int)(buyRt2DB.buy_cnt * buyRt2DB.one_price);
            BuyInfo();

            yield return new WaitForSeconds(press_time);
            if (press_time > 0.05f)
                press_time -= 0.2f;
            else if (press_time > 0.025f)
                press_time -= 0.05f;
            else press_time = 0.025f;
        }
    }

    IEnumerator Routin_Rt3Buy()
    {
        float press_time = 0.5f;
        while (isBuyPressOn && (buyRt3DB.buy_cnt + buyRt3DB.daily_buy_cnt) < buyRt3DB.daily_buy_max_cnt)
        {
            buyRt3DB.buy_cnt++;
            buyRt3DB.buy_price = (int)(buyRt3DB.buy_cnt * buyRt3DB.one_price);
            BuyInfo();

            yield return new WaitForSeconds(press_time);
            if (press_time > 0.05f)
                press_time -= 0.2f;
            else if (press_time > 0.025f)
                press_time -= 0.05f;
            else press_time = 0.025f;
        }
    }

    public void UpPress_Buy(int rt)
    {
        isBuyPressOn = false;
    }

    List<string> ntfMsg = new List<string>();
    /// <summary>
    /// 구매한것 서버로 보냄 
    /// </summary>
    public async void BuySend()
    {
        var userInfo_db = GameDatabase.GetInstance().tableDB.GetUserInfo();
        if (ui.purchaseType == PurchaseType.EquipEnhantSton)
        {
            userInfo_db.m_daily_buy_eq_ehnt_ston_rt1 += buyRt1DB.buy_cnt;
            userInfo_db.m_daily_buy_eq_ehnt_ston_rt2 += buyRt2DB.buy_cnt;
            userInfo_db.m_daily_buy_eq_ehnt_ston_rt3 += buyRt3DB.buy_cnt;
        }
        else if(ui.purchaseType == PurchaseType.AcceEnhantSton)
        {
            userInfo_db.m_daily_buy_ac_ehnt_ston_rt1 += buyRt1DB.buy_cnt;
            userInfo_db.m_daily_buy_ac_ehnt_ston_rt2 += buyRt2DB.buy_cnt;
            userInfo_db.m_daily_buy_ac_ehnt_ston_rt3 += buyRt3DB.buy_cnt;
        }
        else if(ui.purchaseType == PurchaseType.EnhantBlessing)
        {
            userInfo_db.m_daily_buy_ehnt_bless_rt1 += buyRt1DB.buy_cnt;
            userInfo_db.m_daily_buy_ehnt_bless_rt2 += buyRt2DB.buy_cnt;
            userInfo_db.m_daily_buy_ehnt_bless_rt3 += buyRt3DB.buy_cnt;
        }

        if(buyRt1DB.buy_cnt + buyRt2DB.buy_cnt + buyRt3DB.buy_cnt <= 0)
        {
            PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox("구매 할 아이템의 수량이 0입니다.");
            return;
        }

        int price = buyRt1DB.buy_price + buyRt2DB.buy_price + buyRt3DB.buy_price;
        var goods_db = GameDatabase.GetInstance().tableDB.GetTableDB_Goods();
        bool isBlueDiaLack = goods_db.m_dia < price;
        int tbc = isBlueDiaLack == true ? await GameDatabase.GetInstance().tableDB.GetMyTBC() : 0;
        int blue_dia = goods_db.m_dia;
        if (blue_dia + tbc >= price)
        {
            int dedDia = goods_db.m_dia -= price;
            int dedTbc = dedDia < 0 ? System.Math.Abs(dedDia) : 0;
            var item_rt1 = GameDatabase.GetInstance().tableDB.GetItem((int)ui.purchaseType, 1);
            var item_rt2 = GameDatabase.GetInstance().tableDB.GetItem((int)ui.purchaseType, 2);
            var item_rt3 = GameDatabase.GetInstance().tableDB.GetItem((int)ui.purchaseType, 3);
            item_rt1.count += buyRt1DB.buy_cnt;
            item_rt2.count += buyRt2DB.buy_cnt;
            item_rt3.count += buyRt3DB.buy_cnt;

            // 서버 전송 
            Task tsk1 = GameDatabase.GetInstance().tableDB.SetUpdateGoods(goods_db);
            Task tsk2 = GameDatabase.GetInstance().tableDB.DeductionTBC(dedTbc);
            Task tsk3 = GameDatabase.GetInstance().tableDB.SetUpdate_UserInfo(userInfo_db);
            while (Loading.Bottom(tsk1.IsCompleted, tsk2.IsCompleted, tsk3.IsCompleted) == false) await Task.Delay(100);

            Task tsk4 = GameDatabase.GetInstance().tableDB.SendDataItem(item_rt1);
            Task tsk5 = GameDatabase.GetInstance().tableDB.SendDataItem(item_rt2);
            Task tsk6 = GameDatabase.GetInstance().tableDB.SendDataItem(item_rt3);
            while (Loading.Bottom(tsk4.IsCompleted, tsk5.IsCompleted, tsk6.IsCompleted) == false) await Task.Delay(100);

            // 구매 완료 메시지 
            ntfMsg.Clear();
            if (ui.purchaseType == PurchaseType.EquipEnhantSton)
            {
                if (buyRt1DB.buy_cnt > 0)
                    ntfMsg.Add(string.Format(success_buy_eq_ston, GameDatabase.StringFormat.GetRatingColorText(1, false), buyRt1DB.buy_cnt));
                if (buyRt2DB.buy_cnt > 0)
                    ntfMsg.Add(string.Format(success_buy_eq_ston, GameDatabase.StringFormat.GetRatingColorText(2, false), buyRt2DB.buy_cnt));
                if (buyRt3DB.buy_cnt > 0)
                    ntfMsg.Add(string.Format(success_buy_eq_ston, GameDatabase.StringFormat.GetRatingColorText(3, false), buyRt3DB.buy_cnt));
            }
            else if(ui.purchaseType == PurchaseType.AcceEnhantSton)
            {
                if (buyRt1DB.buy_cnt > 0)
                    ntfMsg.Add(string.Format(success_buy_ac_ston, GameDatabase.StringFormat.GetRatingColorText(1, false), buyRt1DB.buy_cnt));
                if (buyRt2DB.buy_cnt > 0)
                    ntfMsg.Add(string.Format(success_buy_ac_ston, GameDatabase.StringFormat.GetRatingColorText(2, false), buyRt2DB.buy_cnt));
                if (buyRt3DB.buy_cnt > 0)
                    ntfMsg.Add(string.Format(success_buy_ac_ston, GameDatabase.StringFormat.GetRatingColorText(3, false), buyRt3DB.buy_cnt));
            }
            else if(ui.purchaseType == PurchaseType.EnhantBlessing)
            {
                if (buyRt1DB.buy_cnt > 0)
                    ntfMsg.Add(string.Format(success_buy_eh_bles, GameDatabase.StringFormat.GetRatingColorText(1, false), buyRt1DB.buy_cnt));
                if (buyRt2DB.buy_cnt > 0)
                    ntfMsg.Add(string.Format(success_buy_eh_bles, GameDatabase.StringFormat.GetRatingColorText(2, false), buyRt2DB.buy_cnt));
                if (buyRt3DB.buy_cnt > 0)
                    ntfMsg.Add(string.Format(success_buy_eh_bles, GameDatabase.StringFormat.GetRatingColorText(3, false), buyRt3DB.buy_cnt));
            }

            foreach (var sMsg in ntfMsg)
                PopUpMng.GetInstance().Open_MessageNotif(sMsg); // "[아이템] {0} {1} x{2}개를 구매하였습니다"

            // 리스트 새로 고침 
            if (ui.purchaseType == PurchaseType.EquipEnhantSton)
                MainUI.GetInstance().tapIAP.tapShopItem.InitInfoEquipEnhantSton(userInfo_db);
            else if (ui.purchaseType == PurchaseType.AcceEnhantSton)
                MainUI.GetInstance().tapIAP.tapShopItem.InitInfoAcceEnhantSton(userInfo_db);
            else if (ui.purchaseType == PurchaseType.EnhantBlessing)
                MainUI.GetInstance().tapIAP.tapShopItem.InitInfoEnhantBlessing(userInfo_db);

            buyRt1DB = new BuyDB();
            buyRt2DB = new BuyDB();
            buyRt3DB = new BuyDB();
            gameObject.SetActive(false);
        }
        else PopUpMng.GetInstance().popupNotice.OpenAskNoticeBoxListener("다이아가 부족합니다.\n다이아 구매 탭으로 이동됩니다.", MainUI.GetInstance().Listener_MoveTbcShop);
    }
}
