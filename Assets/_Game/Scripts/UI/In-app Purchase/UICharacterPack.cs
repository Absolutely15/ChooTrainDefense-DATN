using Coffee.UIEffects;
using TMPro;
using UnityEngine;

public class UICharacterPack : MonoBehaviour, IUINonConsumable
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
        ID = IAPNonConsumableBundle.Characters;
    }

    public void UIOnPurchaseComplete()
    {
        uiShiny.enabled = false;
        fakePrice.gameObject.SetActive(false);
    }
}
