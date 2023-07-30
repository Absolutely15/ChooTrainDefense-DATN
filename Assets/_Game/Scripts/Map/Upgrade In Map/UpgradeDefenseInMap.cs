using UnityEngine;
using static NVTT.Utilities;

public class UpgradeDefenseInMap : MonoBehaviour, IUpgradeInMap
{
    public UpgradeButton upgradeBarrierBtn;
    public UpgradeButton upgradeTrapBtn;
    public EventID EventID { get; set; }
    public void Init()
    {
        EventID = EventID.UpgradeDefense;
    }

    public void LoadData()
    {
        if (Gameplay.IsInGameplay) return;
        upgradeBarrierBtn.LoadData();
        upgradeTrapBtn.LoadData();
    }
}
