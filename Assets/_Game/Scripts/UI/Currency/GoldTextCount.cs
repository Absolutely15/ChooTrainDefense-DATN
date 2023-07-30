using UnityBase.DesignPattern;

public class GoldTextCount : UITextCountAnimation
{
    private void Start()
    {
        SetTargetValue(PlayerSave.Gold, true);
        Observer.Instance.AddObserver(EventID.GoldChange, g => SetTargetValue(PlayerSave.Gold));
    }
}
