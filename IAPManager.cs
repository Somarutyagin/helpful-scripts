using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;
using Firebase;

public class IAPManager : MonoBehaviour, IStoreListener
{
    IStoreController m_StoreController;

    public static string _60_diamonds = "60_diamonds"; //многоразовые - consumable
    public static string _50_energy = "60_diamonds"; //многоразовые - consumable
    public static string special_offer = "special_offer"; //многоразовые - consumable
    public static string support = "support"; //многоразовые - consumable
    public static string nanny = "nanny"; //одноразовые - nonconsumable
    public static string cat = "cat"; //одноразовые - nonconsumable
    public static string subscription = "premium"; //подписка - subscription

    public Firebase.FirebaseApp app { get; private set; }

    void Start()
    {
        InitializePurchasing();

        if (PlayerPrefs.HasKey("firstStart") == false)
        {
            PlayerPrefs.SetInt("firstStart", 1);
            RestoreMyProduct();
            //installation tracking
            Firebase.Analytics.FirebaseAnalytics.LogEvent("first_open", "count", 1);
        }
    }

    public bool IsSubscribed_()
    {
#if !UNITY_EDITOR
        var subscriptionProduct = m_StoreController.products.WithID(subscription);

        if (subscriptionProduct.receipt == null)
        {
            return false;
        }

        var subscriptionManager = new SubscriptionManager(subscriptionProduct, null);
        SubscriptionInfo subscriptionInfo = subscriptionManager.getSubscriptionInfo();

        if (subscriptionInfo.isSubscribed() == Result.True)
        {
            print("subscription result - True");
            PlayerPrefs.SetInt("subscribed", 1);
            return true;
        }
        else if (subscriptionInfo.isSubscribed() == Result.False)
        {
            print("subscription result - False");
            return false;
        }
        else if (subscriptionInfo.isSubscribed() == Result.Unsupported)
        {
            print("subscription result - Unsupported");
            return false;
        }

        return false;
#else
        return false;
#endif
    }
    void InitializePurchasing()
    {
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        builder.AddProduct(_60_diamonds, ProductType.Consumable);
        builder.AddProduct(_50_energy, ProductType.Consumable);
        builder.AddProduct(special_offer, ProductType.Consumable);
        builder.AddProduct(nanny, ProductType.NonConsumable);
        builder.AddProduct(cat, ProductType.NonConsumable);
        builder.AddProduct(support, ProductType.Consumable);
        builder.AddProduct(subscription, ProductType.Subscription);

        UnityPurchasing.Initialize(this, builder);
    }


    public void BuyProduct(string productName)
    {
        m_StoreController.InitiatePurchase(productName);
    }


    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        var product = args.purchasedProduct;
        Debug.Log($"Purchase Complete - Product: {product.definition.id}");

        switch (product.definition.id)
        {
            case nanny:
                Product_nanny();
                break;
            case cat:
                Product_cat();
                break;
            case energy50:
                Product_energy50();
                break;
            case _60_diamonds:
                Product_60_diamonds();
                break;
            case special_offer:
                Product_special_offer();
                break;
            case support:
                Product_support();
                break;
            case subscription:
                Product_subscription();
                break;
        }

        Debug.Log($"Purchase Complete - Product: {product.definition.id}");

        Firebase.Analytics.FirebaseAnalytics.LogEvent("All_Buy", "count", 1);

        return PurchaseProcessingResult.Complete;
    }

    private void Product_nanny()
    {
        if (PlayerPrefs.GetInt("nanny") == 0)
        {
            PlayerPrefs.SetInt("nanny", 1);
            GlobalVaribles.Energy += 50;
        }
    }

    private void Product_cat()
    {
        if (PlayerPrefs.GetInt("catBuy") == 0)
        {
            PlayerPrefs.SetInt("catBuy", 1);
            GlobalVaribles.Energy += 50;
        }
    }


    private void Awake()
    {
        GlobalVaribles.Init();
    }

    private void Product_60_diamonds()
    {
        GlobalVaribles.Diamonds += 60;
    }

    private void Product_special_offer()
    {
        GlobalVaribles.Diamonds += 220;
        GlobalVaribles.Energy += 150;
    }

    private void Product_energy50()
    {
        GlobalVaribles.Energy += 150;
    }
    private void Product_support()
    {
        //donation to support the developer
    }

    private void Product_subscription()
    {
        GlobalVaribles.Energy += 100;
        GlobalVaribles.Diamonds += 100;
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.Log($"In-App Purchasing initialize failed: {error}");
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.Log($"Purchase failed - Product: '{product.definition.id}', PurchaseFailureReason: {failureReason}");
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        Debug.Log("In-App Purchasing successfully initialized");
        m_StoreController = controller;
    }


    public void RestoreMyProduct()
    {
        if (CodelessIAPStoreListener.Instance.StoreController.products.WithID(nanny).hasReceipt)
        {
            Product_nanny();
        }
        if (CodelessIAPStoreListener.Instance.StoreController.products.WithID(cat).hasReceipt)
        {
            Product_cat();
        }
        if (CodelessIAPStoreListener.Instance.StoreController.products.WithID(subscription).hasReceipt)
        {
            Product_subscription();
        }
    }
}

public static class GlobalVaribles
{
    //Energy
    private const string EnergyValueChanged = "EnergyValueChanged";
    private const string keyEnergy = "Energy";
    private const string keyEnergyWas = "EnergyWas";
    private const int defoultEnergy = 200;
    private static int _valueEnergy;
    public static int Energy
    {
        get
        {
            Init();
            return _valueEnergy;
        }
        set
        {
            PlayerPrefs.SetInt(keyEnergyWas, PlayerPrefs.GetInt(keyEnergy, defoultEnergy));
            PlayerPrefs.SetInt(keyEnergy, value);
            _valueEnergy = value;
        }
    }
    //diamonds
    private const string keyDiamonds = "diamonds";
    private const int defoultDiamonds = 100;
    private static int _valueDiamonds;
    public static int Diamonds
    {
        get
        {
            Init();
            return _valueDiamonds;
        }
        set
        {
            PlayerPrefs.SetInt(keyDiamonds, value);
            _valueDiamonds = value;
        }
    }
    public static void Init()
    {
        _valueEnergy = PlayerPrefs.GetInt(keyEnergy, defoultEnergy);
        if (PlayerPrefs.GetInt("testMode_") == 0)
            _valueDiamonds = PlayerPrefs.GetInt(keyDiamonds, defoultDiamonds);
        else
            _valueDiamonds = PlayerPrefs.GetInt(keyDiamonds, 9000);
    }
}
