public class TableVip : InteractableInMap
{
    protected override void InInteractableZoneResponse()
    {
        base.InInteractableZoneResponse();
        
        if (IAPNonConsumableManager.Instance.iapNonConsumableSaveData.isOwned[IAPNonConsumableBundle.Vip])
            UIManager.Instance.iapManagerPanel.gameObject.SetActive(true);
        else
            UIManager.Instance.starterPackPanel.gameObject.SetActive(true);
    }
}
