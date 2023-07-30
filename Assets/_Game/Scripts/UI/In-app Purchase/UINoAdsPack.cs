using UnityEngine;

public class UINoAdsPack : MonoBehaviour, IUINonConsumable
{
    public int ID { get; set; }

    public void SetText(string text)
    {
        
    }

    public void Init()
    {
        ID = IAPNonConsumableBundle.NoAds;
    }

    public void UIOnPurchaseComplete()
    {
        
    }
}
