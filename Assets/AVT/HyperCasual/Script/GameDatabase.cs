using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Game Database")]
public class GameDatabase : SingletonScriptableObject<GameDatabase>
{
    #region Properties
    [Title("Player")]
    public PlayerUpgradeData playerUpgradeData;

    [Title("Weapon")]
    public WeaponUpgradeData weaponUpgradeData;
    public List<WeaponLocalData> weaponList;

    [Title("Skin")]
    public List<SkinData> skinDatas;

    [Title("Map")]
    public MapData mapData;
    public BarrierData barrierData;
    public TrapData trapData;
    public GuardUpgradeData guardUpgradeData;

    [Title("Currency")]
    public CurrencyBonusData currencyBonusData;
    #endregion

    public float GetGlobalDamageUpgradeAt(int level)
    {
        return weaponUpgradeData.globalBaseStat.damage * Mathf.Pow(1.15f, level);
    }
    public float GetFireRateUpgradeAt(int level)
    {
        return weaponUpgradeData.globalBaseStat.fireRate * Mathf.Pow(1.15f, level);
    }

    public List<WeaponLocalData> GetWeaponDataListWithFilter(bool isPremiumOnly = false, bool isNormalOnly = false)
    {
        var res = new List<WeaponLocalData>();
        foreach (var data in weaponList)
        {
            if (isPremiumOnly)
            {
                if (data.isPremium)
                    res.Add(data);
            }
            else if (isNormalOnly)
            {
                if (!data.isPremium)
                    res.Add(data);
            }
        }

        return res;
    }
}

