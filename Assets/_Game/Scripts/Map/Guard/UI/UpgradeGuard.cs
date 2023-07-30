using UnityEngine;
using NVTT;
using static NVTT.Utilities;
using static NVTT.UIUtilities;

[DefaultExecutionOrder(1)]
public class UpgradeGuard : MonoBehaviour, IUpgradeStat
{
    public EventID EventID { get; set; }
    public string Description { get; set; }
    public int LevelMax { get; set; }
    public int CurLevelUI { get; set; }
    public int GoldCost { get; set; }
    public int UpgradeLevel { get; set; }
    public bool IsInit { get; set; }
    public bool IsGuardMaxLevel() => IsMaxLevel(CurLevelUI, LevelMax);
    
    [SerializeField] private GuardController guard;
    public void Init()
    {
        LevelMax = Utilities.MapManager.mapLimitation.guardLevel;
        UpgradeLevel = guard.level;
        EventID = EventID.None;
        Description = "GUARD";
        IsInit = true;
    }

    public void LoadData()
    {
        if (!IsInit) return;
        guard.level = UpgradeLevel;
        CurLevelUI = guard.level;
        if (!IsGuardMaxLevel())
            GoldCost = GameDB.guardUpgradeData.upgradeCost[CurLevelUI];
    }

    public void UpgradeSuccess()
    {
        guard.onUpgrade.Invoke(guard);
    }
}
