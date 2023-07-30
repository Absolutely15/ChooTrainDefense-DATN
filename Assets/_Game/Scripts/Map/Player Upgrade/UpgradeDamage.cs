using UnityEngine;
using NVTT;
using static NVTT.Utilities;
using static NVTT.UIUtilities;

public class UpgradeDamage : MonoBehaviour, IUpgradeStat
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
        LevelMax = Utilities.MapManager.mapLimitation.gunDamageLevel;
        UpgradeLevel = PlayerSave.WeaponGlobalDamageLevel;
        EventID = EventID.UpgradeWeaponGlobal;
        Description = "DAMAGE";
        IsInit = true;
    }

    public void LoadData()
    {
        if (!IsInit) return;
        PlayerSave.WeaponGlobalDamageLevel = UpgradeLevel;
        CurLevelUI = PlayerSave.WeaponGlobalDamageLevel + 1;
        if (!IsMaxLevel(CurLevelUI, LevelMax))
            GoldCost = GameDB.weaponUpgradeData.damageUpgradeCost[CurLevelUI];
    }

    public void UpgradeSuccess()
    {
        
    }
}
