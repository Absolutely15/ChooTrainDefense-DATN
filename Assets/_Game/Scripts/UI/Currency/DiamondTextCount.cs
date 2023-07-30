using UnityBase.DesignPattern;

public class DiamondTextCount : UITextCountAnimation
{
    private void Start()
    {
        SetTargetValue(PlayerSave.Diamond, true);
        Observer.Instance.AddObserver(EventID.DiamondChange, g => SetTargetValue(PlayerSave.Diamond));
    }
}