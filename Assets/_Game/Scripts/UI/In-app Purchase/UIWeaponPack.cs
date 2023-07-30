using Coffee.UIEffects;
using TMPro;
using UnityEngine;

public class UIWeaponPack : MonoBehaviour, IUINonConsumable
{
    [SerializeField] private UIShiny uiShiny;
    [SerializeField] private TextMeshProUGUI fakePrice;

    public int ID { get; set; }

    public void SetText(string text)
    {
        fakePrice.text = text;
    }

    public void Init()
    {
        ID = IAPNonConsumableBundle.Weapons;
    }

    public void UIOnPurchaseComplete()
    {
        uiShiny.enabled = false;
        fakePrice.gameObject.SetActive(false);
    }
}
