using System;
using System.Collections.Generic;
using ATSoft;
using UnityEngine;
using UnityEngine.Purchasing;
using NVTT;
using UnityEngine.Events;

public class IAPConsumableManager : Singleton<IAPConsumableManager>
{
    #region Properties

    [HideInInspector] public UnityEvent<int> onPurchaseComplete = new UnityEvent<int>();
    [SerializeField] private IAPConsumableData iapConsumableData;
    [SerializeField] private List<string> consumableProductId = new List<string>();

    private readonly  Dictionary<string, Action> consumableDictionary = new Dictionary<string, Action>();
    #endregion

    #region Init
    private void OnValidate()
    {
        if (consumableProductId.Count == 0 || consumableProductId == null)
            consumableProductId = IAPUtilities.GetProductsIDType(ProductType.Consumable);
    }

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        for (var i = 0; i < consumableProductId.Count; i++)
        {
            var i1 = i;
            consumableDictionary.Add(consumableProductId[i], () =>
            {
                AddDiamondAmount(iapConsumableData.diamondsPackData[i1]);
                ProductOnCompletePurchase(i1);
            });
        }
    }
    #endregion

    #region Purchase
    public void ProcessPurchase(Product product)
    {
        consumableDictionary[product.definition.id].Invoke();
    }

    private void ProductOnCompletePurchase(int id)
    {
        onPurchaseComplete.Invoke(id);
    }

    private static void AddDiamondAmount(int amount)
    {
        PlayerSave.Diamond += amount;
        AnalyticsManager.LogEventDiamondEarn(amount, "in_app_purchase");
    }
    #endregion

}
