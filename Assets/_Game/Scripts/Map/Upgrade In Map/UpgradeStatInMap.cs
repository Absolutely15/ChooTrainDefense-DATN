using UnityEngine;
using static NVTT.Utilities;

public class UpgradeStatInMap : MonoBehaviour, IUpgradeInMap
{
    public UpgradeButton upgradeHealthBtn;
    public UpgradeButton upgradeSpeedBtn;
    public EventID EventID { get; set; }

    public void Init()
    {
        EventID = EventID.UpgradeStat;
    }
    
    public void LoadData()
    {
        if (Gameplay.IsInGameplay) return;
        upgradeHealthBtn.LoadData();
        upgradeSpeedBtn.LoadData();
    }
}
