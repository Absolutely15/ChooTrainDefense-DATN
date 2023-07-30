using UnityEngine;

public class UpgradeGuardInMap : MonoBehaviour, IUpgradeInMap
{
    public UpgradeButton upgradeGuardBtn;
    public EventID EventID { get; set; }

    public void Init()
    {
        EventID = EventID.None;
    }

    public void LoadData()
    {
        upgradeGuardBtn.LoadData();
    }
}
