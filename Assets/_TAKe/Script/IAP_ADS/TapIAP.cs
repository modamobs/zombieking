using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using System.Threading.Tasks;
using BackEnd;
using LitJson;
using UnityEngine.UI;
using System;

public class TapIAP : MonoBehaviour
{
    [SerializeField] TapObject tapObject;

    //▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
    // UI 탭 - 상점 - 
    //▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
    #region
    [SerializeField] List<UI> tapShops = new List<UI>();
    public TapShopDiamond tapShopDiamond;
    public TapShopPackageEtc tapPackageEtc;
    public TapShopLuck tapShopLuck;
    public TapShopItem tapShopItem;
    public TapShopExchange tapShopExchange;

    [System.Serializable]
    struct UI
    {
        public string sTapShopName; // [0]: 다이아 상점, [1] : 골드 상점, [2] : 행운 상점, [3] : 아이템 상점, [4] : 교환 상점 
        public GameObject goTap;
        public Image imBtn;
    }

    Color coAlpha1 = new Color(1, 1, 1, 1), coAlpha0 = new Color(0, 0, 0, 0);

    void OnEnable()
    {
        tapObject.aniIcon.Play("MainButtonActiveOnScale");
        tapObject.txName.fontStyle = FontStyle.Bold;
        tapObject.txName.color = tapObject.onCorSelect;
        tapObject.goOutline.SetActive(true);
    }

    void OnDisable()
    {
        tapObject.txName.fontStyle = FontStyle.Normal;
        tapObject.txName.color = tapObject.noCorSelect;
        tapObject.goOutline.SetActive(false);
    }

    public void TopTapChange(GameObject goOnTap)
    {
        Click_TapChange(goOnTap);
    }

    public void Click_TapChange(GameObject goOnTap)
    {
        if (goOnTap == null)
            return;

        foreach (var tap in tapShops)
        {
            tap.imBtn.color = GameObject.Equals(tap.goTap, goOnTap) ? coAlpha1 : coAlpha0;
            tap.goTap.SetActive(GameObject.Equals(tap.goTap, goOnTap));
        }
    }
    #endregion

    void Awake()
    {
        //LogPrint.Print("22222222222222222222");
        //foreach (var tap in tapShops)
        //{
        //    tap.goTap.SetActive(string.Equals(tap.sTapShopName, "diamond"));
        //    if(string.Equals(tap.sTapShopName, "diamond"))
        //    {
        //        Click_TapChange(tap.goTap);
        //    }
        //}
    }

    public void OnPurchaseComplete(IAPButton ib)
    {
        LogPrint.EditorPrint("<color=yellow>----------</color>OnPurchaseComplete productId : " + ib.productId);
    }


    public void OnPurchaseFailed(IAPButton ib)
    {
        LogPrint.EditorPrint("<color=yellow>----------</color>OnPurchaseFailed productId : " + ib.productId);
    }

    //▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
    // 상품 구매
    //▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
    #region
    // 테스트
    public void BuyTest(IAPButton ib) => IAPMng.GetInstance().ABuyProduct(ib.productId, ABuyTestPurchaseResult);

    // TBC (화이트 다이아) 충전 
    public void BuyTBC(IAPButton ib) => IAPMng.GetInstance().ABuyTBC(ib.productId);

    public void BuyItem(IAPButton ib) => IAPMng.GetInstance().ABuyProduct(ib.productId, ABuyTestPurchaseResult);

    //광고 제거 
    public void BuyAdsRemoval(IAPButton ib) => IAPMng.GetInstance().ABuyProduct(ib.productId, ABuyAdsRemovalPurchaseResult);

    public void BuyEquipEnhantPackage(IAPButton ib) => IAPMng.GetInstance().ABuyProduct(ib.productId, ABuyCompleteEquipEnhantPackage);
    public void BuyAcceEnhantPackage(IAPButton ib) => IAPMng.GetInstance().ABuyProduct(ib.productId, ABuyCompleteAcceEnhantPackage);

    public void BuyRubyGold(IAPButton ib) => IAPMng.GetInstance().ABuyProduct(ib.productId, ABuyCompleteRubyGold);
    public void BuyEtherGold(IAPButton ib) => IAPMng.GetInstance().ABuyProduct(ib.productId, ABuyCompleteEtherGold);
    #endregion

    //▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
    // 상품 구매 결과 리턴 
    //▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
    #region 
    // TBC 
    [System.Serializable]
    struct TBCRow
    {
        public int usedTBC;
        public int amountTBC;
    }

    /// <summary>
    /// # 테스트 # 구글 & 애플 구매 결과 및 상품 데이터 처리 
    /// </summary>
    private async void ABuyTestPurchaseResult(string returnStr)
    {
        LogPrint.EditorPrint("<color=white>ABuyTestPurchaseResult----------returnStr : " + returnStr + "</color>");

        try
        {
            //string productID = returnStr.Split(',')[0].ToString();
            //bool isSuccess = bool.Parse(returnStr.Split(',')[1].ToString());
            //var db = GameDatabase.GetInstance().tableDB.GetTableDB_Goods();
            //db.m_gold += 1000;

            //Task tsk1 = GameDatabase.GetInstance().tableDB.SetUpdateGoods(db);
            //while (Loading.Bottom(tsk1.IsCompleted) == false) await Task.Delay(100);
        }
        catch (System.Exception)
        {

        }
    }

    // 광고 제거 구매 
    private async void ABuyAdsRemovalPurchaseResult(string returnStr)
    {
        try
        {
            var userinfo_db = GameDatabase.GetInstance().tableDB.GetUserInfo();
            userinfo_db.m_ad_removal = 1;
            Task tsk1 = GameDatabase.GetInstance().tableDB.SetUpdate_UserInfo(userinfo_db);
            while (Loading.Full(tsk1.IsCompleted) == false) await Task.Delay(100);
            MainUI.GetInstance().tapIAP.tapPackageEtc.AdsRemoval();
        }
        catch (System.Exception)
        { }

        LogPrint.Print("<color=white>ABuyTestPurchaseResult----------returnStr : " + returnStr + "</color>");
    }

    private async void ABuyCompleteEquipEnhantPackage(string returnStr)
    {
        int rwd_ruby = GameDatabase.GetInstance ().chartDB.GetDicBalance("shop.eq.enhant.pack.ruby").val_int;
        int rwd_rt1_eq_ston = GameDatabase.GetInstance().chartDB.GetDicBalance("shop.eq.enhant.pack.rt1.ston").val_int;
        int rwd_rt2_eq_ston = GameDatabase.GetInstance().chartDB.GetDicBalance("shop.eq.enhant.pack.rt2.ston").val_int;
        int rwd_rt3_eq_ston = GameDatabase.GetInstance().chartDB.GetDicBalance("shop.eq.enhant.pack.rt3.ston").val_int;
        GameDatabase.GetInstance().tableDB.SetUpdateGoods("ruby", rwd_ruby, "+");
        var item1 = GameDatabase.GetInstance().tableDB.GetItem(21, 1);
        var item2 = GameDatabase.GetInstance().tableDB.GetItem(21, 2);
        var item3 = GameDatabase.GetInstance().tableDB.GetItem(21, 3);
        item1.count += rwd_rt1_eq_ston;
        item2.count += rwd_rt2_eq_ston;
        item3.count += rwd_rt3_eq_ston;
        Task<bool> tsk1 = GameDatabase.GetInstance().tableDB.SendDataItem(item1);
        Task<bool> tsk2 = GameDatabase.GetInstance().tableDB.SendDataItem(item2);
        Task<bool> tsk3 = GameDatabase.GetInstance().tableDB.SendDataItem(item3);
        while (Loading.Full(tsk1.IsCompleted, tsk2.IsCompleted, tsk3.IsCompleted) == false)
            await Task.Delay(250);
    }               

    private async void ABuyCompleteAcceEnhantPackage(string returnStr)
    {
        int rwd_ether = GameDatabase.GetInstance().chartDB.GetDicBalance("shop.ac.enhant.pack.ether").val_int;
        int rwd_rt1_eq_ston = GameDatabase.GetInstance().chartDB.GetDicBalance("shop.ac.enhant.pack.rt1.ston").val_int;
        int rwd_rt2_eq_ston = GameDatabase.GetInstance().chartDB.GetDicBalance("shop.ac.enhant.pack.rt2.ston").val_int;
        int rwd_rt3_eq_ston = GameDatabase.GetInstance().chartDB.GetDicBalance("shop.ac.enhant.pack.rt3.ston").val_int;
        GameDatabase.GetInstance().tableDB.SetUpdateGoods("ether", rwd_ether, "+");
        var item1 = GameDatabase.GetInstance().tableDB.GetItem(27, 1);
        var item2 = GameDatabase.GetInstance().tableDB.GetItem(27, 2);
        var item3 = GameDatabase.GetInstance().tableDB.GetItem(27, 3);
        item1.count += rwd_rt1_eq_ston;
        item2.count += rwd_rt2_eq_ston;
        item3.count += rwd_rt3_eq_ston;
        Task<bool> tsk1 = GameDatabase.GetInstance().tableDB.SendDataItem(item1);
        Task<bool> tsk2 = GameDatabase.GetInstance().tableDB.SendDataItem(item2);
        Task<bool> tsk3 = GameDatabase.GetInstance().tableDB.SendDataItem(item3);
        while (Loading.Full(tsk1.IsCompleted, tsk2.IsCompleted, tsk3.IsCompleted) == false)
            await Task.Delay(250);
    }

    void ABuyCompleteRubyGold(string returnStr)
    {
        int rwd_ruby = GameDatabase.GetInstance().chartDB.GetDicBalance("shop.ruby.gold.pack1").val_int;
        long rwd_gold = GameDatabase.GetInstance().chartDB.GetDicBalance("shop.ruby.gold.pack2").val_long;
        GameDatabase.GetInstance().tableDB.SetUpdateGoods("ruby", rwd_ruby, "+");
        GameDatabase.GetInstance().tableDB.SetUpdateGoods("gold", rwd_gold, "+");
        PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox(string.Format("루비 x{0:#,0}, 골드 {1:#,0}을 구매하였습니다.", rwd_ruby, rwd_gold));
    }
    void ABuyCompleteEtherGold(string returnStr)
    {
        int rwd_ether = GameDatabase.GetInstance().chartDB.GetDicBalance("shop.ether.gold.pack1").val_int;
        long rwd_gold =  GameDatabase.GetInstance().chartDB.GetDicBalance("shop.ether.gold.pack2").val_long;
        GameDatabase.GetInstance().tableDB.SetUpdateGoods("ether", rwd_ether, "+");
        GameDatabase.GetInstance().tableDB.SetUpdateGoods("gold", rwd_gold, "+");
        PopUpMng.GetInstance().popupNotice.OpenCloseConmfirmNoticeBox(string.Format("에테르 x{0:#,0}, 골드 {1:#,0}을 구매하였습니다.", rwd_ether, rwd_gold));
    }

    /// <summary>
    /// 장비 (ALL) 상자 구매 
    /// </summary>
    private async void ABuyEquipAllBox(string returnString)
    {

    }
    
    /// <summary>
    /// 장비 - 무기/방어구 상자 구매 
    /// </summary>
    private async void ABuyEquipWeaponBox (string returnStr)
    {

    }

    /// <summary>
    /// 장비 - 방어구 상자 구매 
    /// </summary>
    private async void ABuyEquipArm()
    {

    }
    
    /// <summary>
    /// 장비 - 장신구 상자 구매 
    /// </summary>
    private async void ABuyEquipAcce()
    {

    }

    #endregion
}

