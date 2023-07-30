public class Locker : InteractableInMap
{
    protected override void InInteractableZoneResponse()
    {
        base.InInteractableZoneResponse();
        UIManager.Instance.inventoryPanel.OpenSkinTab();
    }
}
