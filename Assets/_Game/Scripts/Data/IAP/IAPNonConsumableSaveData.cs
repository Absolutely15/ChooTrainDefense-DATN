using System;
using System.Collections.Generic;

[Serializable]
public class IAPNonConsumableSaveData
{
    public List<bool> isOwned;
}

public static class IAPNonConsumableBundle
{
    public const int Vip = 0;
    public const int Weapons = 1;
    public const int Characters = 2;
    public const int NoAds = 3;
}
