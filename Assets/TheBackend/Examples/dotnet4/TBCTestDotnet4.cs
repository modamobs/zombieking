using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using System;
using BackEnd;
using LitJson;

public class TBCTestDotnet4 : MonoBehaviour, IStoreListener
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

    public static string kProductIDConsumable = "test1";
    public static string kProductIDNonConsumable = "nonconsumable";
    public static string kProductIDSubscription = "subscription";

    // Apple App Store-specific product identifier for the subscription product. 
    private static string kProductNameAppleSubscription = "com.unity3d.subscription.new";

    // Google Play Store-specific product identifier subscription product.
    private static string kProductNameGooglePlaySubscription = "com.unity3d.subscription.original";

    string id = "id1";
    string pw = "thebackend";
    bool isAsync = false;
    bool isTBC = false;
    List<TheBackendProduct> productList = new List<TheBackendProduct>();
    BackendReturnObject bro = new BackendReturnObject();
    bool isSuccess = false;

    void Start()
    {
        // If we haven't set up the Unity Purchasing reference
        if (m_StoreController == null)
        {
            // Begin to configure our connection to Purchasing
            InitializePurchasing();
        }

        //Backend.Initialize(BRO =>
        //{
        //    Debug.Log("Backend.Initialize " + BRO);
        //    // 성공
        //    if (BRO.IsSuccess())
        //    {
        //        Backend.BMember.CustomLogin(id, pw, callback =>
        //        {
        //            Debug.Log("CustomLogin " + callback);
        //            isSuccess = callback.IsSuccess();
        //            bro = callback;
        //        });
        //    }
        //    // 실패
        //    else
        //    {
        //        Debug.LogError("Failed to initialize the backend");
        //    }
        //});

        Backend.Initialize(HandleBackendCallback);
    }

    void HandleBackendCallback()
    {
        if (Backend.IsInitialized)
        {
            // 구글 해시키 획득 
            if (!Backend.Utils.GetGoogleHash().Equals(""))
                Debug.Log(Backend.Utils.GetGoogleHash());

            // 서버시간 획득
            Debug.Log(Backend.Utils.GetServerTime());

            // 로그인
            Backend.BMember.CustomLogin(id, pw, callback =>
           {
               Debug.Log("CustomLogin " + callback);
               isSuccess = callback.IsSuccess();
               bro = callback;
           });
        }
        // 실패
        else
        {
            Debug.LogError("Failed to initialize the backend");
        }
    }

    private void Update()
    {
        //if (isSuccess)
        //{
        //    Debug.Log("-------------Update(SaveToken)-------------");
        //    BackendReturnObject saveToken = Backend.BMember.SaveToken(bro);
        //    if (saveToken.IsSuccess())
        //    {
        //        Debug.Log("로그인 성공");
        //    }
        //    else
        //    {
        //        Debug.Log("로그인 실패: " + saveToken.ToString());
        //    }
        //    isSuccess = false;
        //    bro.Clear();
        //}
    }

    // deprecated
    // 초기화 함수 이후에 실행되는 callback 
    //void backendCallback()
    //{
    //    // 초기화 성공한 경우 실행
    //    if (Backend.IsInitialized)
    //    {
    //    }
    //    // 초기화 실패한 경우 실행 
    //    else
    //    {
    //    }
    //}

    public void InitializePurchasing()
    {
        // If we have already connected to Purchasing ...
        if (IsInitialized())
        {
            // ... we are done here.
            return;
        }

        // Create a builder, first passing in a suite of Unity provided stores.
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        // Add a product to sell / restore by way of its identifier, associating the general identifier
        // with its store-specific identifiers.
        builder.AddProduct(kProductIDConsumable, ProductType.Consumable);
        // Continue adding the non-consumable product.
        builder.AddProduct(kProductIDNonConsumable, ProductType.NonConsumable);
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


    private bool IsInitialized()
    {
        // Only say we are initialized if both the Purchasing references are set.
        return m_StoreController != null && m_StoreExtensionProvider != null;
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


    //  
    // --- IStoreListener
    //

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

#if UNITY_ANDROID
        // 동기
        if (!isAsync)
        { 
            if(isTBC)
            {
                Debug.Log("-----------------ChargeTBC-----------------");
                // 영수증 검증 & TBC 충전
                Debug.Log(Backend.TBC.ChargeTBC(args.purchasedProduct.receipt, ""));
            }
            else
            {
                Debug.Log("-----------------IsValidateGooglePurchase-----------------");
                // 영수증 검증 
                Debug.Log(Backend.Receipt.IsValidateGooglePurchase(args.purchasedProduct.receipt, "영수증 description", false));
            }
        }
        // 비동기
        else
        {
            if(isTBC)
            {
                Debug.Log("-----------------AChargeTBC-----------------");
                // 영수증 검증 & TBC 충전
                Backend.TBC.ChargeTBC(args.purchasedProduct.receipt, "charge description", bro =>
                {
                    Debug.Log(bro);
                });
            }
            else
            {
                Debug.Log("-----------------AIsValidateGooglePurchase-----------------");
                // 영수증 검증
                Backend.Receipt.IsValidateGooglePurchase( args.purchasedProduct.receipt, "", false, bro =>
                {
                    Debug.Log(bro);
                });
            }

        }
#elif UNITY_IOS
        if (!isAsync)
        { 
            if(isTBC)
            {
                Debug.Log("-----------------ChargeTBC-----------------");
                // 영수증 검증 & TBC 충전
                Debug.Log(Backend.TBC.ChargeTBC(args.purchasedProduct.receipt, ""));
            }
            else
            {
                Debug.Log("-----------------IsValidateGooglePurchase-----------------");
                // 영수증 검증 
                Debug.Log(Backend.Receipt.IsValidateApplePurchase(args.purchasedProduct.receipt, "영수증 description"));
            }
        }
        // 비동기
        else
        {
            if(isTBC)
            {
                Debug.Log("-----------------AChargeTBC-----------------");
                // 영수증 검증 & TBC 충전
                Backend.TBC.ChargeTBC,args.purchasedProduct.receipt( "charge description", bro =>
                {
                    Debug.Log(bro);
                });
            }
            else
            {
                Debug.Log("-----------------AIsValidateGooglePurchase-----------------");
                // 영수증 검증
                Backend.Receipt.IsValidateApplePurchase( args.purchasedProduct.receipt, "", bro =>
                {
                    Debug.Log(bro);
                });
            }

        }
#endif

        // Return a flag indicating whether this product has completely been received, or if the application needs 
        // to be reminded of this purchase at next app launch. Use PurchaseProcessingResult.Pending when still 
        // saving purchased products to the cloud, and when that save is delayed. 
        return PurchaseProcessingResult.Complete;
    }


    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        // A product purchase attempt did not succeed. Check failureReason for more detail. Consider sharing 
        // this reason with the user to guide their troubleshooting actions.
        Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
    }

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
            Debug.Log(name + " : " + uuid);
        }
        public string uuid;
        public string name;
    }
}