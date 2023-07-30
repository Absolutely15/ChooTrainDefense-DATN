using UnityEngine;
using static NVTT.Utilities;

public class UpgradeGunInMap : MonoBehaviour, IUpgradeInMap
{
    public UpgradeButton upgradeDamageBtn;
    public UpgradeButton upgradeFireRateBtn;
    public EventID EventID { get; set; }

    public void Init()
    {
        EventID = EventID.UpgradeWeaponGlobal;
    }

    public void LoadData()
    {
        if (Gameplay.IsInGameplay) return;
        upgradeDamageBtn.LoadData();
        upgradeFireRateBtn.LoadData();
    }
}
