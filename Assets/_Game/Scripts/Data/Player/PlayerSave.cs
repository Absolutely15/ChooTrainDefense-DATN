using UnityBase.DesignPattern;
using UnityEngine;

public static class PlayerSave
{
    #region Key Save
    private const string KEY_ALREADY_INIT = "already_init";
    private const string KEY_ALREADY_INIT_MAP = "already_init_map_data";
    
    private const string KEY_GOLD = "gold";
    private const string KEY_DIAMOND = "diamond";
    private const string KEY_CURRENT_GAME_LEVEL = "current_game_level";
    
    private const string KEY_SAVE_MAP = "map_save_data";
    private const string KEY_BARRIER = "barrier";
    private const string KEY_TRAP = "trap";
    private const string KEY_DRONE = "drone";
    
    private const string KEY_PLAYER_HEALTH = "player_health";
    private const string KEY_PLAYER_SPEED = "player_speed";
    private const string KEY_WEAPON_GLOBAL_DAMAGE = "weapon_global_damage";
    private const string KEY_WEAPON_GLOBAL_FIRE_RATE = "weapon_global_firerate";

    private const string KEY_WEAPON_USING = "weapon_using";
    private const string KEY_WEAPON_LEVEL = "weapon_level";
    private const string KEY_WEAPON_STATE = "weapon_state";

    private const string KEY_SKIN_USING = "skin_using";
    private const string KEY_SKIN_STATE = "skin_state";

    private const string KEY_IAP_NON_CONSUMABLE = "iap_non_consumable";

    #endregion

    #region Init

    public static void InitDefault()
    {
        CurrentGameLevel = CurrentGameLevel;
        Gold = Gold;
        Diamond = Diamond;
        PlayerHealthLevel = PlayerHealthLevel;
        PlayerSpeedLevel = PlayerSpeedLevel;
        
        //Weapon data
        WeaponGlobalDamageLevel = WeaponGlobalDamageLevel;
        WeaponGlobalFireRateLevel = WeaponGlobalFireRateLevel;

        //Map
        BarrierLevel = BarrierLevel;
        TrapLevel = TrapLevel;
        
        if (AlreadyInit == false)
        {
            //Inventory
            SetSkinState(SkinID.Default, SkinState.Unlocked);
            SetWeaponState((int)WeaponID.Rifle, WeaponState.Unlocked);
            for (var i = 1; i < 8; i++)
            {
                if (GameDatabase.Instance.weaponList[i].GetWeaponState() == WeaponState.Undiscovered)
                    SetWeaponState(i, WeaponState.Unlockable);
            }

            AlreadyInit = true;
        }
        
        PlayerPrefs.Save();
    }


    #endregion

    #region Get/Set Properties

    public static bool AlreadyInit
    {
        get => PlayerPrefs.GetInt(KEY_ALREADY_INIT,0) == 1;
        set => PlayerPrefs.SetInt(KEY_ALREADY_INIT, value ? 1 : 0);
    }
    public static bool AlreadyInitMapData
    {
        get => PlayerPrefs.GetInt(KEY_ALREADY_INIT_MAP,0) == 1;
        set => PlayerPrefs.SetInt(KEY_ALREADY_INIT_MAP, value ? 1 : 0);
    }

    public static int CurrentGameLevel
    {
        get => PlayerPrefs.GetInt(KEY_CURRENT_GAME_LEVEL, 0);
        set => PlayerPrefs.SetInt(KEY_CURRENT_GAME_LEVEL, value);
    }

    public static int Gold
    {
        get => PlayerPrefs.GetInt(KEY_GOLD, 0);
        set
        {
            PlayerPrefs.SetInt(KEY_GOLD, value);
            Observer.Instance.Notify(EventID.GoldChange, Gold);
        }
    }
    public static int Diamond
    {
        get => PlayerPrefs.GetInt(KEY_DIAMOND, 0);
        set
        {
            PlayerPrefs.SetInt(KEY_DIAMOND, value);
            Observer.Instance.Notify(EventID.DiamondChange, Diamond);
        }
    }

    public static int PlayerHealthLevel
    {
        get => PlayerPrefs.GetInt(KEY_PLAYER_HEALTH, 0);
        set => PlayerPrefs.SetInt(KEY_PLAYER_HEALTH, value);
    }
    
    public static int PlayerSpeedLevel
    {
        get => PlayerPrefs.GetInt(KEY_PLAYER_SPEED, 0);
        set => PlayerPrefs.SetInt(KEY_PLAYER_SPEED, value);
    }

    public static int WeaponGlobalDamageLevel
    {
        get => PlayerPrefs.GetInt(KEY_WEAPON_GLOBAL_DAMAGE, 0);
        set => PlayerPrefs.SetInt(KEY_WEAPON_GLOBAL_DAMAGE, value);
    }
    
    public static int WeaponGlobalFireRateLevel
    {
        get => PlayerPrefs.GetInt(KEY_WEAPON_GLOBAL_FIRE_RATE, 0);
        set => PlayerPrefs.SetInt(KEY_WEAPON_GLOBAL_FIRE_RATE, value);
    }
    
    public static int BarrierLevel
    {
        get => PlayerPrefs.GetInt(KEY_BARRIER, 0);
        set => PlayerPrefs.SetInt(KEY_BARRIER, value);
    }
    
    public static int TrapLevel
    {
        get => PlayerPrefs.GetInt(KEY_TRAP, 0);
        set => PlayerPrefs.SetInt(KEY_TRAP, value);
    }
    
    public static bool IsDroneUnlocked
    {
        get => PlayerPrefs.GetInt(KEY_DRONE,0) == 1;
        set => PlayerPrefs.SetInt(KEY_DRONE, value ? 1 : 0);
    }
    
    #endregion

    #region Get/Set Weapon 
    //Weapon using
    public static int GetWeaponIsUsing() => PlayerPrefs.GetInt(KEY_WEAPON_USING, 0);
    public static void SetWeaponIsUsing(this WeaponLocalData weaponData) => SetWeaponIsUsing(weaponData.id);
    public static void SetWeaponIsUsing(int weaponId) => PlayerPrefs.SetInt(KEY_WEAPON_USING, weaponId);
    
    //Weapon unlock state
    public static WeaponState GetWeaponState(this WeaponLocalData weaponData) => GetWeaponState(weaponData.id);
    public static WeaponState GetWeaponState(int weaponId) => (WeaponState)PlayerPrefs.GetInt($"{KEY_WEAPON_STATE}_{weaponId}", 0);
    public static void SetWeaponState(this WeaponLocalData weaponData, WeaponState value) => SetWeaponState(weaponData.id, value);
    public static void SetWeaponState(int weaponId, WeaponState value) => PlayerPrefs.SetInt($"{KEY_WEAPON_STATE}_{weaponId}", (int)value);

    //Weapon Level
    public static int GetWeaponLevel(this WeaponLocalData weaponData) => GetWeaponLevel(weaponData.id);
    public static int GetWeaponLevel(int weaponId) => PlayerPrefs.GetInt($"{KEY_WEAPON_LEVEL}_{weaponId}", 0);
    public static void SetWeaponLevel(this WeaponLocalData weaponData, int level) => SetWeaponLevel(weaponData.id, level);
    public static void SetWeaponLevel(int weaponId, int level) => PlayerPrefs.SetInt($"{KEY_WEAPON_LEVEL}_{weaponId}", level);
    #endregion

    #region Get/Set Skin
    public static int GetSkinIsUsing() => PlayerPrefs.GetInt(KEY_SKIN_USING, 0);
    public static void SetSkinIsUsing(this SkinData skinData) => SetSkinIsUsing(skinData.id);
    public static void SetSkinIsUsing(int skinId) => PlayerPrefs.SetInt(KEY_SKIN_USING, skinId);
    
    //Skin unlock state
    public static SkinState GetSkinState(this SkinData skinData) => GetSkinState(skinData.id);
    public static SkinState GetSkinState(int skinId) => (SkinState)PlayerPrefs.GetInt($"{KEY_SKIN_STATE}_{skinId}", 0);
    public static void SetSkinState(this SkinData skinData, SkinState value) => SetSkinState(skinData.id, value);
    public static void SetSkinState(int skinId, SkinState value) => PlayerPrefs.SetInt($"{KEY_SKIN_STATE}_{skinId}", (int)value);

    #endregion
    
    #region Serialize/Deserialize Data
    public static void SerializeMapSaveData(MapSaveData mapSaveData)
    {
        var stringSave = JsonUtility.ToJson(mapSaveData);
        PlayerPrefs.SetString(KEY_SAVE_MAP, stringSave);
    }
    
    public static MapSaveData DeserializeMapSaveData()
    {
        var temp = PlayerPrefs.GetString(KEY_SAVE_MAP);
        var mapSaveData = JsonUtility.FromJson<MapSaveData>(temp);
        return mapSaveData;
    }
    
    public static void SerializeIAPNonConsumableSaveData(IAPNonConsumableSaveData iapNonConsumableSaveData)
    {
        var stringSave = JsonUtility.ToJson(iapNonConsumableSaveData);
        PlayerPrefs.SetString(KEY_IAP_NON_CONSUMABLE, stringSave);
    }
    
    public static IAPNonConsumableSaveData DeserializeIAPNonConsumableSaveData()
    {
        var temp = PlayerPrefs.GetString(KEY_IAP_NON_CONSUMABLE);
        var iapSaveData = JsonUtility.FromJson<IAPNonConsumableSaveData>(temp);
        return iapSaveData;
    }
    #endregion

}


