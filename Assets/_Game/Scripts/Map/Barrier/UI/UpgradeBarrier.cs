using UnityEngine;
using NVTT;

public class UpgradeBarrier : MonoBehaviour, IUpgradeStat
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
        LevelMax = Utilities.MapManager.mapLimitation.barrierLevel;
        UpgradeLevel = PlayerSave.BarrierLevel;
        EventID = EventID.UpgradeBarrier;
        Description = "BARRICADE";
        IsInit = true;
    }

    public void LoadData()
    {
        if (!IsInit) return;
        PlayerSave.BarrierLevel = UpgradeLevel;
        CurLevelUI = PlayerSave.BarrierLevel + 1;
        if (!UIUtilities.IsMaxLevel(CurLevelUI, LevelMax))
            GoldCost = Utilities.GameDB.barrierData.upgradeCost[CurLevelUI];
    }

    public void UpgradeSuccess()
    {
        
    }
}
