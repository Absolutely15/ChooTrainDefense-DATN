using UnityEngine;
using NVTT;
using static NVTT.Utilities;
using static NVTT.UIUtilities;

public class UpgradeHealth : MonoBehaviour, IUpgradeStat
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
        LevelMax = Utilities.MapManager.mapLimitation.playerHealthLevel;
        UpgradeLevel = PlayerSave.PlayerHealthLevel;
        EventID = EventID.UpgradeStat;
        Description = "HEALTH";
        IsInit = true;
    }

    public void LoadData()
    {
        if (!IsInit) return;
        PlayerSave.PlayerHealthLevel = UpgradeLevel;
        CurLevelUI = PlayerSave.PlayerHealthLevel + 1;
        if (!IsMaxLevel(CurLevelUI, LevelMax))
            GoldCost = GameDB.playerUpgradeData.healthCost[PlayerSave.PlayerHealthLevel];
    }

    public void UpgradeSuccess()
    {
        
    }
}
