using System.Collections.Generic;
using NVTT;
using UnityEngine;
using UnityEngine.Purchasing;

public class IAPSubscriptionManager : Singleton<IAPSubscriptionManager>
{
    [SerializeField] private List<string> subscriptionProductId = new List<string>();

    private void OnValidate()
    {
        if (subscriptionProductId.Count == 0 || subscriptionProductId == null)
            subscriptionProductId = IAPUtilities.GetProductsIDType(ProductType.Subscription);
    }
    public void ProcessPurchase(Product product)
    {
        
    }
}
