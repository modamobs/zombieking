using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using System;
using BackEnd;
using LitJson;
using System.Linq;
using System.Threading.Tasks;

public class IAPMng : MonoSingleton<IAPMng>, IStoreListener
{
    private static IStoreController m_StoreController;          // The Unity Purchasing system.
    private static IExtensionProvider m_StoreExtensionProvider; // The store-specific Purchasing subsystems.

    // Product identifiers for all products capable of being purchased: 
    // "convenience" general identifiers for use with Purchasing, and their store-specific identifier 
    // counterparts for use with and outside of Unity Purchasing. Define store-specific identifiers 
    // also on each platform's publisher dashboard (iTunes Connect, Google Play Developer Console, etc.)

    // General product identifiers for the consumable, non-consumable, and subscription products.
    // Use these handles in the code to reference which product to purchase. Also use these values 
    // when defining the Product Identifiers on the store. Except, for illustration purposes, the 
    // kProductIDSubscription - it has custom Apple and Google identifiers. We declare their store-
    // specific mapping to Unity Purchasing's AddProduct, below.

    public static string kProductIDConsumable = "consumable";
    public static string kProductIDNonConsumable = "nonconsumable";
    public static string kProductIDSubscription = "subscription";

    // Apple App Store-specific product identifier for the subscription product. 
    private static string kProductNameAppleSubscription = "com.unity3d.subscription.new";

    // Google Play Store-specific product identifier subscription product.
    private static string kProductNameGooglePlaySubscription = "com.unity3d.subscription.original";

    bool isAsync = false;
    bool isTBC = false;
    List<TheBackendProduct> productList = new List<TheBackendProduct>();

    /// <summary>
    /// 상품 구매 성공 or 실패
    /// return message : success or failed 
    /// </summary>
    public delegate void ResultPurchase(string _message);
    public event ResultPurchase resultPurchase;     // 상품 구매시 
    public event ResultPurchase resultTBCPurchase;  // TBC 캐시 구매시 

    void Start()
    {
        if (m_StoreController == null)
        {
            // Begin to configure our connection to Purchasing
            InitializePurchasing();
        }
    }

    private bool IsInitialized()
    {
        // Only say we are initialized if both the Purchasing references are set.
        return m_StoreController != null && m_StoreExtensionProvider != null;

    }
    public void InitializePurchasing()
    {
        // If we have already connected to Purchasing ...
        if (IsInitialized())
        {
            // ... we are done here.
            return;
        }

        Debug.Log("-------InitializePurchasing---------");
        // Create a builder, first passing in a suite of Unity provided stores.
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        // Add a product to sell / restore by way of its identifier, associating the general identifier
        // with its store-specific identifiers.
        //builder.AddProduct(kProductIDConsumable, ProductType.Consumable);

        builder.AddProduct(
            "tbc_7day", ProductType.Consumable, new IDs() {
                { "red.dia.tbc_7day", GooglePlay.Name },
            }
        );
        builder.AddProduct(
            "tbc_30day", ProductType.Consumable, new IDs() {
                { "red.dia.tbc_30day", GooglePlay.Name },
            }
        );
        builder.AddProduct(
            "tbc1_100", ProductType.Consumable, new IDs() {
                { "red.dia.tbc1_100", GooglePlay.Name },
            }
        );
        builder.AddProduct(
           "tbc_525", ProductType.Consumable, new IDs() {
                { "red.dia.tbc_525", GooglePlay.Name },
           }
        );
        builder.AddProduct(
            "tbc_1100", ProductType.Consumable, new IDs() {
                { "red.dia.tbc_1100", GooglePlay.Name },
            }
        );
        builder.AddProduct(
            "tbc_2875", ProductType.Consumable, new IDs() {
                { "red.dia.tbc_2875", GooglePlay.Name },
            }
        );
        builder.AddProduct(
            "tbc_6250", ProductType.Consumable, new IDs() {
                { "red.dia.tbc_6250", GooglePlay.Name },
            }
        );
        builder.AddProduct(
           "tbc_14000", ProductType.Consumable, new IDs() {
                { "red.dia.tbc_14000", GooglePlay.Name },
           }
       );
        builder.AddProduct(
           "eq_enhant_pack", ProductType.Consumable, new IDs() {
                { "ssap.zb.eq.enhant.pack", GooglePlay.Name },
           }
       );
        builder.AddProduct(
           "ac_enhant_pack", ProductType.Consumable, new IDs() {
                { "ssap.zb.ac.enhant.pack", GooglePlay.Name },
           }
       );
        builder.AddProduct(
           "eq_enhant_pack", ProductType.Consumable, new IDs() {
                { "ssap.zb.eq.ruby.gold", GooglePlay.Name },
           }
       );
        builder.AddProduct(
           "ac_enhant_pack", ProductType.Consumable, new IDs() {
                { "ssap.zb.ac.ether.gold", GooglePlay.Name },
           }
       );
        builder.AddProduct(
          "ruby_gold", ProductType.Consumable, new IDs() {
                { "ssap.zb.ruby.gold.pack", GooglePlay.Name },
          }
      );
        builder.AddProduct(
           "ether_gold", ProductType.Consumable, new IDs() {
                { "ssap.zb.ether.gold.pack", GooglePlay.Name },
           }
       ); // 
        builder.AddProduct(
           "ad_removal2", ProductType.Consumable, new IDs() {
                { "ssap.zb.ad.removal2", GooglePlay.Name },
           }
       );

        // Continue adding the non-consumable product.
        //builder.AddProduct(kProductIDNonConsumable, ProductType.NonConsumable);
        // And finish adding the subscription product. Notice this uses store-specific IDs, illustrating
        // if the Product ID was configured differently between Apple and Google stores. Also note that
        // one uses the general kProductIDSubscription handle inside the game - the store-specific IDs 
        // must only be referenced here. 
        builder.AddProduct(kProductIDSubscription, ProductType.Subscription, new IDs(){
            { kProductNameAppleSubscription, AppleAppStore.Name },
            { kProductNameGooglePlaySubscription, GooglePlay.Name },
        });

        // Kick off the remainder of the set-up with an asynchrounous call, passing the configuration 
        // and this class' instance. Expect a response either in OnInitialized or OnInitializeFailed.
        UnityPurchasing.Initialize(this, builder);

        Debug.LogError("UnityPurchasing initialize");
    }

    public void ChargeTBC()
    {
        isAsync = false;
        isTBC = true;
        BuyConsumable();
    }

    public void AChargeTBC()
    {
        isAsync = true;
        isTBC = true;
        BuyConsumable();
    }

    public void IsValidateGooglePurchase()
    {
        isAsync = false;
        isTBC = false;
        BuyConsumable();
    }

    /// <summary>
    /// TBC (화이트 다이아) 충전 결과 및 수량 새로 고침 
    /// </summary>
    public async void ABuyTBCResult(string returnStr)
    {
        try
        {
            DateTime nDate = BackendGpgsMng.GetInstance().GetNowTime();
            string productID = returnStr.Split(',')[0].ToString();
            bool isSuccess = bool.Parse(returnStr.Split(',')[1].ToString());
            LogPrint.EditorPrint("<color=yellow>ABuyTBCResult returnStr : " + returnStr + ", productID : " + productID + ", isSuccess : " + isSuccess + "</color>");
            if (isSuccess == true)
            {
                await BackendGpgsMng.GetInstance().SettingNowTime();
                nDate = BackendGpgsMng.GetInstance().GetNowTime();
                var goods_db = GameDatabase.GetInstance().tableDB.GetTableDB_Goods();
                if (string.Equals(productID, "red.dia.tbc_30day")) // 30일 매일 다이아 
                {
                    goods_db.m_30DayTbc = GameDatabase.GetInstance().chartDB.GetDicBalance("day30.daily.dia.reward.cnt").val_int * 30;
                    goods_db.m_30DayTbcDailyRewardDate = nDate.AddDays(-1).ToString();
                    goods_db.m_30DayTbcStartDate = nDate.ToString();
                    goods_db.m_30DayTbcEndDate = nDate.AddDays(29).ToString(/*"yyyy/MM/dd"*/);
                    Task tsk1 = GameDatabase.GetInstance().tableDB.SetUpdateGoods(goods_db);
                    while (Loading.Full(tsk1.IsCompleted) == false) await Task.Delay(100);

                    PopUpMng.GetInstance().OnInit_DailyProductReward();
                    if (MainUI.GetInstance().tapIAP.tapShopDiamond.gameObject.activeSelf)
                        MainUI.GetInstance().tapIAP.tapShopDiamond.DayProduct();
                }
                else if (string.Equals(productID, "red.dia.tbc_7day")) // 7일 매일 다이아 
                {
                    goods_db.m_7DayTbc = (int)(GameDatabase.GetInstance().chartDB.GetDicBalance("day7.daily.dia.reward.cnt").val_int * 7);
                    goods_db.m_7DayTbcDailyRewardDate = nDate.AddDays(-1).ToString();
                    goods_db.m_7DayTbcStartDate = nDate.ToString();
                    goods_db.m_7DayTbcEndDate = nDate.AddDays(6).ToString(/*"yyyy/MM/dd"*/);
                    Task tsk1 = GameDatabase.GetInstance().tableDB.SetUpdateGoods(goods_db);
                    while (Loading.Full(tsk1.IsCompleted) == false) await Task.Delay(100);

                    PopUpMng.GetInstance().OnInit_DailyProductReward();
                    if (MainUI.GetInstance().tapIAP.tapShopDiamond.gameObject.activeSelf)
                        MainUI.GetInstance().tapIAP.tapShopDiamond.DayProduct();
                }
                else
                {

                }

                //TBCRow rowTBC = JsonUtility.FromJson<TBCRow>(returnStr);
                //MainUI.GetInstance().topUI.InfoViewTBC(rowTBC.amountTBC);

                await Task.Delay(1000);
                MainUI.GetInstance().topUI.GetInfoViewTBC();
            }
            else
            {
                LogPrint.EditorPrint("<color=red>ABuyTBCResult error</color>");
            }
        }
        catch (System.Exception)
        {
            MainUI.GetInstance().topUI.GetInfoViewTBC();
        }
        LogPrint.EditorPrint("<color=white>ABuyTBCResult----------returnStr : " + returnStr + "</color>");
    }


    /// <summary>
    /// 뒤끝 TBC 충전 (뒤끝 전용 캐시 -> 화이트 다이아) 
    /// </summary>
    public void ABuyTBC(string productId)
    {
        Debug.Log("IsInitialized() : " + IsInitialized());
        m_StoreController.InitiatePurchase(productId);

        isAsync = true;
        isTBC = true;

        kProductIDConsumable = productId;
        resultTBCPurchase = ABuyTBCResult;
        LogPrint.EditorPrint("<color=yellow>ABuyTBC kProductIDConsumable : " + kProductIDConsumable + ", resultTBCPurchase.Count : " + resultTBCPurchase.GetInvocationList().Length + "</color>");
    }

    /// <summary>
    /// 구글, 애플 상품 구매 
    /// </summary>
    public void ABuyProduct(string productId, ResultPurchase rp)
    {
        isAsync = true;
        isTBC = false;
        resultPurchase = null;

        kProductIDConsumable = productId;
        resultPurchase = rp;
        LogPrint.EditorPrint("<color=yellow>ABuyProduct kProductIDConsumable : " + kProductIDConsumable + ", resultPurchase.Count : " + resultPurchase.GetInvocationList().Length + "</color>");
    }

    public void AIsValidateGooglePurchase()
    {
        isAsync = true;
        isTBC = false;
        BuyConsumable();
    }

    void BuyConsumable()
    {
        // Buy the consumable product using its general identifier. Expect a response either 
        //             through ProcessPurchase or OnPurchaseFailed asynchronously.
        BuyProductID(kProductIDConsumable);
    }


    public void BuyNonConsumable()
    {
        // Buy the non-consumable product using its general identifier. Expect a response either 
        // through ProcessPurchase or OnPurchaseFailed asynchronously.
        BuyProductID(kProductIDNonConsumable);
    }


    public void BuySubscription()
    {
        // Buy the subscription product using its the general identifier. Expect a response either 
        // through ProcessPurchase or OnPurchaseFailed asynchronously.
        // Notice how we use the general product identifier in spite of this ID being mapped to
        // custom store-specific identifiers above.
        BuyProductID(kProductIDSubscription);
    }

    void ABuyProductID(string productId)
    {
        // If Purchasing has been initialized ...
        if (IsInitialized())
        {
            // ... look up the Product reference with the general product identifier and the Purchasing 
            // system's products collection.
            Product product = m_StoreController.products.WithID(productId);

            // If the look up found a product for this device's store and that product is ready to be sold ... 
            if (product != null && product.availableToPurchase)
            {
                Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
                // ... buy the product. Expect a response either through ProcessPurchase or OnPurchaseFailed 
                // asynchronously.
                m_StoreController.InitiatePurchase(product);
            }
            // Otherwise ...
            else
            {
                // ... report the product look-up failure situation  
                Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
            }
        }
        // Otherwise ...
        else
        {
            // ... report the fact Purchasing has not succeeded initializing yet. Consider waiting longer or 
            // retrying initiailization.
            Debug.Log("BuyProductID FAIL. Not initialized.");
        }
    }

    void BuyProductID(string productId)
    {
        // If Purchasing has been initialized ...
        if (IsInitialized())
        {
            // ... look up the Product reference with the general product identifier and the Purchasing 
            // system's products collection.
            Product product = m_StoreController.products.WithID(productId);

            // If the look up found a product for this device's store and that product is ready to be sold ... 
            if (product != null && product.availableToPurchase)
            {
                Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
                // ... buy the product. Expect a response either through ProcessPurchase or OnPurchaseFailed 
                // asynchronously.
                m_StoreController.InitiatePurchase(product);
            }
            // Otherwise ...
            else
            {
                // ... report the product look-up failure situation  
                Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
            }
        }
        // Otherwise ...
        else
        {
            // ... report the fact Purchasing has not succeeded initializing yet. Consider waiting longer or 
            // retrying initiailization.
            Debug.Log("BuyProductID FAIL. Not initialized.");
        }
    }


    // Restore purchases previously made by this customer. Some platforms automatically restore purchases, like Google. 
    // Apple currently requires explicit purchase restoration for IAP, conditionally displaying a password prompt.
    public void RestorePurchases()
    {
        // If Purchasing has not yet been set up ...
        if (!IsInitialized())
        {
            // ... report the situation and stop restoring. Consider either waiting longer, or retrying initialization.
            Debug.Log("RestorePurchases FAIL. Not initialized.");
            return;
        }

        // If we are running on an Apple device ... 
        if (Application.platform == RuntimePlatform.IPhonePlayer ||
            Application.platform == RuntimePlatform.OSXPlayer)
        {
            // ... begin restoring purchases
            Debug.Log("RestorePurchases started ...");

            // // Fetch the Apple store-specific subsystem.
            // var apple = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();
            // // Begin the asynchronous process of restoring purchases. Expect a confirmation response in 
            // // the Action<bool> below, and ProcessPurchase if there are previously purchased products to restore.
            // apple.RestoreTransactions((result) => {
            //     // The first phase of restoration. If no more responses are received on ProcessPurchase then 
            //     // no purchases are available to be restored.
            //     Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
            // });
        }
        // Otherwise ...
        else
        {
            // We are not running on an Apple device. No work is necessary to restore purchases.
            Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
        }
    }



    /// <summary>
    /// ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
    /// IStoreListener
    /// ▣▣▣▣▣▣▣▣▣▣▣▣▣▣▣
    /// </summary>
    #region 
    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        // Purchasing has succeeded initializing. Collect our Purchasing references.
        Debug.Log("OnInitialized: PASS");

        // Overall Purchasing system, configured with products for this application.
        m_StoreController = controller;
        // Store specific subsystem, for accessing device-specific store features.
        m_StoreExtensionProvider = extensions;
    }


    public void OnInitializeFailed(InitializationFailureReason error)
    {
        // Purchasing set-up has not succeeded. Check error for reason. Consider sharing this reason with the user.
        Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
    }


    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        bool isFailed = false;
        // A consumable product has been purchased by this user.
        if (String.Equals(args.purchasedProduct.definition.id, kProductIDConsumable, StringComparison.Ordinal))
        {
            Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
            // The consumable item has been successfully purchased, add 100 coins to the player's in-game score.
            // ScoreManager.score += 100;
        }
        // Or ... a non-consumable product has been purchased by this user.
        else if (String.Equals(args.purchasedProduct.definition.id, kProductIDNonConsumable, StringComparison.Ordinal))
        {
            Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
            // TODO: The non-consumable item has been successfully purchased, grant this item to the player.
        }
        // Or ... a subscription product has been purchased by this user.
        else if (String.Equals(args.purchasedProduct.definition.id, kProductIDSubscription, StringComparison.Ordinal))
        {
            Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
            // TODO: The subscription item has been successfully purchased, grant this to the player.
        }
        // Or ... an unknown product has been purchased by this user. Fill in additional products here....
        else
        {
            Debug.Log(string.Format("ProcessPurchase: FAIL. Unrecognized product: '{0}'", args.purchasedProduct.definition.id));
        }

        OnCompletePurchase(kProductIDConsumable, args.purchasedProduct.receipt);

        // Return a flag indicating whether this product has completely been received, or if the application needs 
        // to be reminded of this purchase at next app launch. Use PurchaseProcessingResult.Pending when still 
        // saving purchased products to the cloud, and when that save is delayed. 
        return PurchaseProcessingResult.Complete;
    }

    /// <summary>
    /// 구매 성공 
    /// </summary>
    private async void OnCompletePurchase(string productID, string receipt)
    {
#if UNITY_ANDROID
        BackendReturnObject bro1 = null;
        // 영수증 검증 & TBC 충전
        if (isTBC)
        {
            Debug.Log("-----------------AChargeTBC-----------------");
            SendQueue.Enqueue(Backend.TBC.ChargeTBC, receipt, "", callback => { bro1 = callback; });
            while (Loading.Bottom(bro1) == false) { await Task.Delay(100); }

            LogPrint.EditorPrint("영수증 검증 & TBC 충전 OnCompletePurchase bro1 : " + bro1);
            if (bro1.IsSuccess())
            {
                //resultTBCPurchase(bro1.GetReturnValuetoJSON().ToJson()); // 결과 델리게이트 (tbc 충전값, 보유값)
                resultTBCPurchase(string.Format("{0},{1}", productID, bro1.IsSuccess().ToString())); // 결과 델리게이트 (상품id, 성공or실패)
                LogPrint.EditorPrint("영수증 검증 & TBC 충전 OnCompletePurchase bro1 " + bro1.GetReturnValuetoJSON().ToJson());
            }
            else
            {
                LogPrint.EditorPrint("영수증 검증 & TBC 충전 OnCompletePurchase GetErrorCode : " + bro1.GetErrorCode());
            }
        }
        // 영수증 검증
        else
        {
            Debug.Log("-----------------AChargeTBC-----------------");
            SendQueue.Enqueue(Backend.Receipt.IsValidateGooglePurchase, receipt, "", false, callback => { bro1 = callback; });
            while (Loading.Bottom(bro1) == false) { await Task.Delay(100); }

            LogPrint.EditorPrint("영수증 검증 & TBC 충전 OnCompletePurchase bro1 : " + bro1);
            if (bro1.IsSuccess())
            {
                resultPurchase(string.Format("{0},{1}", productID, bro1.IsSuccess().ToString())); // 결과 델리게이트  
                LogPrint.EditorPrint("영수증 검증 OnCompletePurchase bro1 " + bro1.GetReturnValuetoJSON().ToJson());
            }
            else
            {
                LogPrint.EditorPrint("영수증 검증 OnCompletePurchase GetErrorCode : " + bro1.GetErrorCode());
            }
        }

#elif UNITY_IOS
        // 비동기
        // 영수증 검증 & TBC 충전
        if(isTBC)
        {
            Debug.Log("-----------------AChargeTBC-----------------");
            SendQueue.Enqueue(Backend.TBC.ChargeTBC, receipt, "charge description", callback => { bro1 = callback; });
            while (Loading.Bottom(bro1) == false) { await Task.Delay(100); }
            
            //resultTBCPurchase(bro1.GetReturnValuetoJSON().ToJson()); // 결과 델리게이트  (tbc 충전값, 보유값)
            resultTBCPurchase(string.Format("{0},{1}", productID, bro1.IsSuccess().ToString())); // 결과 델리게이트 (상품id, 성공or실패)
            LogPrint.EditorPrint("영수증 검증 & TBC 충전 OnCompletePurchase bro " + bro1.GetReturnValuetoJSON().ToJson());
        }
        // 영수증 검증
        else
        {
            Debug.Log("-----------------AIsValidateGooglePurchase-----------------");
            SendQueue.Enqueue(Backend.Receipt.IsValidateApplePurchase, receipt, "", false, callback => { bro1 = callback; });
            while (Loading.Bottom(bro1) == false) { await Task.Delay(100); }
            resultPurchase(string.Format("{0},{1}", productID, bro1.IsSuccess().ToString())); // 결과 델리게이트  
             LogPrint.EditorPrint("OnCompletePurchase OnCompletePurchase bro " + bro1.GetReturnValuetoJSON().ToJson());
        }
#endif
    }

    /// <summary>
    /// 구매 실패 
    /// </summary>
    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        // A product purchase attempt did not succeed. Check failureReason for more detail. Consider sharing 
        // this reason with the user to guide their troubleshooting actions.
        Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
    }
    #endregion

    public void GetTBC()
    {
        Debug.Log("-----------------GetTBC-----------------");
        Debug.Log(Backend.TBC.GetTBC());
    }

    public void AGetTBC()
    {
        Debug.Log("-----------------AGetTBC-----------------");
        Backend.TBC.GetTBC(callback =>
        {
            Debug.Log(callback);
        });
    }

    public void useTBC()
    {
        Debug.Log("-----------------useTBC-----------------");
        if (productList.Count > 0)
        {
            TheBackendProduct product = productList[0];
            var productUuid = product.uuid;
            Debug.Log(Backend.TBC.UseTBC(productUuid, "tbc"));
        }
        else
        {
            Debug.Log("get the product list");
        }
    }

    public void AuseTBC()
    {
        Debug.Log("-----------------AUseTBC-----------------");
        if (productList.Count > 0)
        {
            TheBackendProduct product = productList[0];
            var productUuid = product.uuid;
            Backend.TBC.UseTBC(productUuid, "atbc", callback =>
            {
                Debug.Log(callback);
            });
        }
        else
            Debug.Log("get the product list");
    }

    public void GetProducts()
    {
        Debug.Log("-----------------GetProducts-----------------");
        BackendReturnObject bro = Backend.TBC.GetProductList();
        Debug.Log(bro);
        if (bro.IsSuccess())
        {
            productList.Clear();
            JsonData jsonData = bro.GetReturnValuetoJSON();

            for (int i = 0; i < jsonData["rows"].Count; i++)
            {
                var prodcutUuid = jsonData["rows"][i]["uuid"]["S"].ToString();
                var productName = jsonData["rows"][i]["name"]["S"].ToString();
                productList.Add(new TheBackendProduct(prodcutUuid, productName));
            }
        }
    }

    public void AGetProducts()
    {
        Debug.Log("-----------------GetProducts-----------------");
        Backend.TBC.GetProductList(callback =>
        {
            Debug.Log(callback);

            if (callback.IsSuccess())
            {
                productList.Clear();
                JsonData jsonData = callback.GetReturnValuetoJSON();

                for (int i = 0; i < jsonData["rows"].Count; i++)
                {
                    var prodcutUuid = jsonData["rows"][i]["uuid"]["S"].ToString();
                    var productName = jsonData["rows"][i]["name"]["S"].ToString();
                    productList.Add(new TheBackendProduct(prodcutUuid, productName));
                }
            }
        });
    }

    internal class TheBackendProduct
    {
        public TheBackendProduct(string uuid, string name)
        {
            this.uuid = uuid;
            this.name = name;
        }
        public string uuid;
        public string name;
    }
}
