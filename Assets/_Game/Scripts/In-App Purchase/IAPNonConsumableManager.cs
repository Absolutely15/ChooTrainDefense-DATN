using System;
using System.Collections.Generic;
using ATSoft;
using ATSoft.Ads;
using NVTT;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Purchasing;

public class IAPNonConsumableManager : Singleton<IAPNonConsumableManager>
{
    #region Properties
    public IAPNonConsumableSaveData iapNonConsumableSaveData;
    
    [HideInInspector] public UnityEvent<int> onPurchaseComplete = new UnityEvent<int>();

    [SerializeField] private List<string> nonConsumableProductId = new List<string>();

    private Dictionary<string, Action> nonConsumableDictionary;
    #endregion
    
    #region Init
    private void OnValidate()
    {
        if (nonConsumableProductId.Count == 0 || nonConsumableProductId == null)
            nonConsumableProductId = IAPUtilities.GetProductsIDType(ProductType.NonConsumable);
    }

    private void Start()
    {
        Init();
    }
    private void Init()
    {
        nonConsumableDictionary = new Dictionary<string, Action>
        {
            { nonConsumableProductId[IAPNonConsumableBundle.Vip], UnlockVipPackage },
            { nonConsumableProductId[IAPNonConsumableBundle.Weapons], UnlockWeaponPackage },
            { nonConsumableProductId[IAPNonConsumableBundle.Characters], UnlockCharacterPackage },
            { nonConsumableProductId[IAPNonConsumableBundle.NoAds], UnlockNoAdsPackage }
        };

        var tempSaveData = PlayerSave.DeserializeIAPNonConsumableSaveData();
        if (tempSaveData != null)
            iapNonConsumableSaveData = tempSaveData;
    }
    #endregion
    
    #region Purchase
    public void ProcessPurchase(Product product)
    {
        nonConsumableDictionary[product.definition.id].Invoke();
    }
    #endregion
    
    #region Unlock Package
    private void SerializeIAPNonConsumableSaveData() => PlayerSave.SerializeIAPNonConsumableSaveData(iapNonConsumableSaveData);
    private void ProductOnCompletePurchase(int id)
    {
        onPurchaseComplete.Invoke(id);
        iapNonConsumableSaveData.isOwned[id] = true;
        SerializeIAPNonConsumableSaveData();
    }

    private void UnlockVipPackage()
    {
        PlayerSave.SetSkinState(SkinID.Cowboy, SkinState.Unlocked);
        PlayerSave.SetWeaponState((int)WeaponID.LaserGun, WeaponState.Unlocked);
        PlayerSave.Diamond += 500;
        Advertisements.Instance.RemoveAds(true);
        AnalyticsManager.LogEventDiamondEarn(500, "unlock_starter_pack");
        
        ProductOnCompletePurchase(IAPNonConsumableBundle.Vip);
        ProductOnCompletePurchase(IAPNonConsumableBundle.NoAds);
    }

    private void UnlockWeaponPackage()
    {
        PlayerSave.SetWeaponState((int)WeaponID.LaserGun, WeaponState.Unlocked);
        PlayerSave.SetWeaponState((int)WeaponID.QuadRocket, WeaponState.Unlocked);
        PlayerSave.SetWeaponState((int)WeaponID.Flamethrower, WeaponState.Unlocked);
        
        ProductOnCompletePurchase(IAPNonConsumableBundle.Weapons);
    }

    private void UnlockCharacterPackage()
    {
        PlayerSave.SetSkinState(SkinID.Cyborg, SkinState.Unlocked);
        PlayerSave.SetSkinState(SkinID.Police, SkinState.Unlocked);
        PlayerSave.SetSkinState(SkinID.Gangster, SkinState.Unlocked);
        
        ProductOnCompletePurchase(IAPNonConsumableBundle.Characters);
    }

    private void UnlockNoAdsPackage()
    {
        Advertisements.Instance.RemoveAds(true);
        
        ProductOnCompletePurchase(IAPNonConsumableBundle.NoAds);
    }
    #endregion
}
