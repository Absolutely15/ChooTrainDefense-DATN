using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Skin Data")]
public class SkinData : ItemData
{
    [Title("Unlock")]
    public int isDiscoveredInWorld;
    public int unlockDiamondCost;
    public List<SkinBonus> skinBonusValues;
}

[Serializable]
public class SkinBonus
{
    public SkinBonusType skinBonusType;
    public float skinBonusValue = 1f;
}

public enum SkinState
{
    Undiscovered,
    Unlockable,
    Unlocked
}
public enum SkinBonusType
{
    Health,
    Speed,
    Damage,
    FireRate,
    Gold
}

public static class SkinID
{
    public const int Default = 0;
    public const int Cyborg = 1;
    public const int Cowboy = 2;
    public const int Police = 3;
    public const int PinkGuy = 4;
    public const int Gangster = 5;
    public const int IronMan = 6;
}
