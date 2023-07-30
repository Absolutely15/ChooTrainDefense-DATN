using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class UINonConsumablePack : MonoBehaviour
{
    [SerializeField] private IAPProductButton iapProductBtn;
    [SerializeField] private GameObject confetti;
    [SerializeField] private Sprite ownedSprite;
    [SerializeField] private TextMeshProUGUI realPrice;
    [SerializeField] private TextMeshProUGUI ownedTxt;
    [SerializeField] private string[] price;
    
    private IUINonConsumable iUINonConsumable;

    private void Awake()
    {
        iUINonConsumable = GetComponent<IUINonConsumable>();
        iUINonConsumable.Init();

        if (IAPNonConsumableManager.Instance.iapNonConsumableSaveData.isOwned[iUINonConsumable.ID])
            UIOnPurchaseComplete(iUINonConsumable.ID);
        else
            IAPNonConsumableManager.Instance.onPurchaseComplete.AddListener(UIOnPurchaseComplete);
    }

    private void OnEnable()
    {
        confetti.SetActive(false);
    }

    private void UIOnPurchaseComplete(int id)
    {
        if (id != iUINonConsumable.ID) return;
        iUINonConsumable.UIOnPurchaseComplete();

        confetti.SetActive(true);
        
        iapProductBtn.enabled = false;
        iapProductBtn.purchaseBtn.interactable = false;
        iapProductBtn.purchaseBtn.image.sprite = ownedSprite;
        
        realPrice.gameObject.SetActive(false);
        ownedTxt.gameObject.SetActive(true);
    }

    [Button]
    private void SetText()
    {
        if (price.Length != 1)
        {
            realPrice.SetText(price[0]);
            iUINonConsumable = GetComponent<IUINonConsumable>();
            iUINonConsumable.SetText(price[1]);
        }
        else
            realPrice.SetText(price[0]);
    }
}
