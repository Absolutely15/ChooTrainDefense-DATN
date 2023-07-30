using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Purchasing;
using Unity.Services.Core;
using Unity.Services.Core.Environments;

public class InAppPurchaseManager : Singleton<InAppPurchaseManager>, IStoreListener
{
    #region Properties
    public bool enableDebugLog = true;
    public UnityEvent<bool, string> onTransactionsRestored = new UnityEvent<bool, string>();

    public static bool InitializationComplete;
    
    private static IStoreController _storeController;
    private static IExtensionProvider _extensionProvider;

    private ProductCatalog catalog;
    private ConfigurationBuilder builder;
    
    private readonly WaitForSeconds waitInterval = new WaitForSeconds(5);
    #endregion

    #region Init
    private void Awake()
    {
        try
        {
            var options = new InitializationOptions()
#if UNITY_EDITOR
                .SetEnvironmentName("test");
#else
            .SetEnvironmentName("production");
#endif
            UnityServices.InitializeAsync(options).ContinueWith(task => MyDebug("Unity Services Initialized"));
        }
        catch (Exception e)
        {
            MyDebug($"Unity Gaming Services failed to initialize with error: {e}.");
        }

        LoadCatalog();
        InitializePurchasing();
    }

    private void LoadCatalog()
    {
        catalog = ProductCatalog.LoadDefaultCatalog();
        MyDebug($"Loaded catalog with {catalog.allProducts.Count} items");
    }
    
    private void InitializePurchasing()
    {
        if (InitializationComplete)
            return;
        
        var module = StandardPurchasingModule.Instance();
        module.useFakeStoreUIMode = FakeStoreUIMode.StandardUser;

        builder = ConfigurationBuilder.Instance(module);
        
        IAPConfigurationHelper.PopulateConfigurationBuilder(ref builder, catalog);
        MyDebug($"Initializing Unity IAP with {builder.products.Count} products");
        
        UnityPurchasing.Initialize(this, builder);
        
        StartCoroutine(InitChecking());
    }

    private IEnumerator InitChecking()
    {
        while (true)
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
                yield return waitInterval;
            else
            {
                switch (InitializationComplete)
                {
                    case true:
                        yield break;
                    case false:
                        UnityPurchasing.Initialize(this, builder);
                        break;
                }
                yield return waitInterval;
            }
        }
    }
    #endregion

    #region Query
    public bool HasProductInCatalog(string productID)
    {
        foreach (var product in catalog.allProducts)
        {
            if (product.id == productID)
            {
                return true;
            }
        }
        return false;
    }

    public Product GetProduct(string productID)
    {
        if (_storeController != null && !string.IsNullOrEmpty(productID))
            return _storeController.products.WithID(productID);
        
        MyDebug($"Unknown product {productID}");
        return null;
    }

    public List<Product> ProductsPurchased()
    {
        var temp = new List<Product>();
        foreach (var item in _storeController.products.all)
        {
            if (item.hasReceipt)
                temp.Add(item);
        }
        return temp;
    }

    public List<Product> ProductsAvailableToPurchase()
    {
        var temp = new List<Product>();
        foreach (var item in _storeController.products.all)
        {
            if (item.availableToPurchase)
                temp.Add(item);
        }
        return temp;
    }
    #endregion

    #region Buy/Restore
    public void BuyProductID(string productId)
    {
        if (InitializationComplete)
        {
            var product = _storeController.products.WithID(productId);
            if (product != null && product.availableToPurchase)
            {
                MyDebug($"Purchasing product: {product.definition.id}");
                _storeController.InitiatePurchase(product);
            }

            else
                MyDebug("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
        }
        else
            MyDebug("BuyProductID FAIL. Not initialized.");
    }

    //This method is for iOS platform (Android platform will auto restore when reinstalling)
    public void RestorePurchases()
    {
        _extensionProvider.GetExtension<IAppleExtensions>().RestoreTransactions(OnTransactionsRestored);
    }

    private void OnTransactionsRestored(bool success, string error)
    {
        onTransactionsRestored.Invoke(success, error);

        MyDebug(success ? "Restore purchases succeeded." : $"Restore purchases failed because of {error}");
    }
    #endregion
    
    #region API
    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        _storeController = controller;
        _extensionProvider = extensions;
        
        InitializationComplete = true;
        MyDebug("OnInitialized: PASS");
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        OnInitializeFailed(error, null);
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        MyDebugError(message == null
            ? $"Purchasing failed to initialize. Reason: {error}."
            : $"Purchasing failed to initialize. Reason: {error}. More details: {message}");
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
    {
        var product = purchaseEvent.purchasedProduct;

        switch (product.definition.type)
        {
            case ProductType.Consumable:
                IAPConsumableManager.Instance.ProcessPurchase(product);
                break;
            case ProductType.NonConsumable:
                IAPNonConsumableManager.Instance.ProcessPurchase(product);
                break;
            case ProductType.Subscription:
                //IAPSubscriptionManager.ProcessPurchase(product);
                break;
        }
        MyDebug($"ProcessPurchase: Complete, Product: {product.definition.id} - {product.transactionID}");
        return PurchaseProcessingResult.Complete;
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        MyDebug($"OnPurchaseFailed: FAIL. Product: {product.definition.storeSpecificId}, PurchaseFailureReason: {failureReason}");
    }
    #endregion

    #region Debug
    public void MyDebug(string debug)
    {
        if (enableDebugLog)
            Debug.Log(debug);
    }

    public void MyDebugWarning(string debug)
    {
        if (enableDebugLog)
            Debug.LogWarning(debug);
    }

    public void MyDebugError(string debug)
    {
        if (enableDebugLog)
            Debug.LogError(debug);
    }
    #endregion
}