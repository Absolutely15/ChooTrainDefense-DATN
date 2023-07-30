using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Weapon Local Data")]
public class WeaponLocalData : ItemData
{
    [PreviewField(100, ObjectFieldAlignment.Center)] public Sprite goldenIcon;
    [Title("Stats")]
    public List<Stats> stats;
    public float fireRange = 1f;
    public float projectileMoveSpeed = 10f;
    
    [Title("Unlock / Upgrade")]
    public List<int> upgradeCost;
    public int isDiscoveredInWorld;
    public int unlockGoldCost;
    public int unlockDiamondCost;
    public int goldenDiamondCost;
    public bool isPremium;

}

public enum WeaponState
{
    Undiscovered,
    Unlockable,
    Unlocked,
    Golden
}

[Serializable]
public struct Stats
{
    public int damage;
    public float fireRate;
}

public enum WeaponID
{
    Rifle = 0,
    Shotgun = 1,
    Uzi = 2,
    Rocket = 3,
    QuadRocket = 4,
    Flamethrower = 5,
    DesertEagle = 6,
    LaserGun = 7
}

