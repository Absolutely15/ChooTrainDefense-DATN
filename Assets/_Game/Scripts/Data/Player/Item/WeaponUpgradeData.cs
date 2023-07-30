using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Weapon Upgrade Data")]
public class WeaponUpgradeData : ScriptableObject
{
    public BaseStat globalBaseStat;
    public List<int> damageUpgradeCost;
    public List<int> fireRateUpgradeCost;
    public List<Material> weaponMat;
}


[Serializable]
public class BaseStat
{
    public float damage;
    public float fireRate;
}