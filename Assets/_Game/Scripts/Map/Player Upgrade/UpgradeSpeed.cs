using UnityEngine;
using NVTT;
using static NVTT.Utilities;
using static NVTT.UIUtilities;

public class UpgradeSpeed : MonoBehaviour, IUpgradeStat
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
        LevelMax = Utilities.MapManager.mapLimitation.playerSpeedLevel;
        UpgradeLevel = PlayerSave.PlayerSpeedLevel;
        EventID = EventID.UpgradeStat;
        Description = "SPEED";
        IsInit = true;
    }

    public void LoadData()
    {
        if (!IsInit) return;
        PlayerSave.PlayerSpeedLevel = UpgradeLevel;
        CurLevelUI = PlayerSave.PlayerSpeedLevel + 1;
        if (!IsMaxLevel(CurLevelUI, LevelMax))
            GoldCost = GameDB.playerUpgradeData.speedCost[PlayerSave.PlayerSpeedLevel];
    }

    public void UpgradeSuccess()
    {
        
    }
}
