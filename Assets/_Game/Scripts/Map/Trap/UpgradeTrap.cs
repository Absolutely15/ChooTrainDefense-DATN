using UnityEngine;
using NVTT;

public class UpgradeTrap : MonoBehaviour, IUpgradeStat
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
        LevelMax = Utilities.MapManager.mapLimitation.trapLevel;
        UpgradeLevel = PlayerSave.TrapLevel;
        EventID = EventID.UpgradeTrap;
        Description = "TRAPS";
        IsInit = true;
    }

    public void LoadData()
    {
        if (!IsInit) return;
        PlayerSave.TrapLevel = UpgradeLevel;
        CurLevelUI = PlayerSave.TrapLevel + 1;
        if (!UIUtilities.IsMaxLevel(CurLevelUI, LevelMax))
            GoldCost = Utilities.GameDB.trapData.upgradeCost[CurLevelUI];
    }

    public void UpgradeSuccess()
    {
        
    }
}
