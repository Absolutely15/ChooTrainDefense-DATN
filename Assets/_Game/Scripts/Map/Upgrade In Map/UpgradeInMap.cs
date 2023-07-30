using UnityBase.DesignPattern;

public class UpgradeInMap : InteractableInMap
{
    public DotweenScale groupBtn;
    private IUpgradeInMap iUpgradeInMap;
    protected override void Init()
    {
        base.Init();
        iUpgradeInMap = GetComponent<IUpgradeInMap>();
        iUpgradeInMap.Init();
    }
    
    protected override void InitObserver()
    {
        base.InitObserver();

        if (iUpgradeInMap.EventID != EventID.None)
            Observer.Instance.AddObserver(iUpgradeInMap.EventID, LoadDataInZone);
        
        Observer.Instance.AddObserver(EventID.GoldChange, LoadDataInZone);

        //Next 2 line is extra feature
        // Observer.Instance.AddObserver(EventID.BonusSpeed, CheckInUpgradeZone);
        // Observer.Instance.AddObserver(EventID.DismissBonusSpeed, CheckInUpgradeZone);
    }

    private void LoadDataInZone()
    {
        if (!InInteractableZone) return;
        iUpgradeInMap.LoadData();
    }

    protected override void InInteractableZoneResponse()
    {
        base.InInteractableZoneResponse();
        groupBtn.gameObject.SetActive(true);
        iUpgradeInMap.LoadData();
        
        //Ads Extra
        UIAdsManager.Instance.PopUpFloatingGold();
    }

    protected override void OutInteractableZoneResponse()
    {
        base.OutInteractableZoneResponse();
        groupBtn.OnClose();
    }

    //Ads Extra
    // private void CheckInUpgradeZone()
    // {
    //     if (iUpgradeInMap.InUpgradeZone)
    //         InUpgradeZoneResponse();
    // }
}