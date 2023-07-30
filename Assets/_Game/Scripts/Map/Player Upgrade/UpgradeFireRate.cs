using UnityEngine;
using NVTT;
using static NVTT.Utilities;
using static NVTT.UIUtilities;

public class UpgradeFireRate : MonoBehaviour, IUpgradeStat
{
    public EventID EventID { get; set; }
    public string Description { get; set; }
    public int LevelMax { get; set; }
    public int CurLevelUI { get; set; }
    public int GoldCost { get; set; }
    public int UpgradeLevel { get; set; }
    public bool IsInit { get; set; }

    public void Init()
    {
        LevelMax = Utilities.MapManager.mapLimitation.gunFireRateLevel;
        UpgradeLevel = PlayerSave.WeaponGlobalFireRateLevel;
        EventID = EventID.UpgradeWeaponGlobal;
        Description = "FIRE RATE";
        IsInit = true;
    }

    public void LoadData()
    {
        if (!IsInit) return;
        PlayerSave.WeaponGlobalFireRateLevel = UpgradeLevel;
        CurLevelUI = PlayerSave.WeaponGlobalFireRateLevel + 1;
        if (!IsMaxLevel(CurLevelUI, LevelMax))
            GoldCost = GameDB.weaponUpgradeData.fireRateUpgradeCost[CurLevelUI];
    }

    public void UpgradeSuccess()
    {
        
    }
}