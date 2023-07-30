using ATSoft.Ads;
using Coffee.UIEffects;
using TMPro;
using UnityEngine;

public class UIVipPackExtra : MonoBehaviour
{    
    [SerializeField] private Sprite ownedSprite;
    [SerializeField] private UIShiny uiShiny;
    [SerializeField] private IAPProductButton iapProductBtn;
    [SerializeField] private TextMeshProUGUI realPrice;
    [SerializeField] private TextMeshProUGUI fakePrice;

    private void Awake()
    {
        if (IAPNonConsumableManager.Instance.iapNonConsumableSaveData.isOwned[IAPNonConsumableBundle.Vip])
            UIOnPurchaseComplete(IAPNonConsumableBundle.Vip);
        else
            IAPNonConsumableManager.Instance.onPurchaseComplete.AddListener(UIOnPurchaseComplete);
    }

    private void UIOnPurchaseComplete(int id)
    {
        if (id != IAPNonConsumableBundle.Vip) return;
        uiShiny.enabled = false;
        iapProductBtn.enabled = false;
        iapProductBtn.purchaseBtn.interactable = false;
        iapProductBtn.purchaseBtn.image.sprite = ownedSprite;
        realPrice.text = "Owned";
        fakePrice.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        if (!UIManager.Instance.iapManagerPanel.gameObject.activeInHierarchy &&
            IAPNonConsumableManager.Instance.iapNonConsumableSaveData.isOwned[IAPNonConsumableBundle.Vip])
        {
            gameObject.SetActive(false);
            return;
        }

        
        if (!UIManager.Instance.iapManagerPanel.gameObject.activeInHierarchy)
            GPExecutor.Instance.PauseGame();
    }

    private void OnDisable()
    {
        if (!UIManager.Instance.iapManagerPanel.gameObject.activeInHierarchy)
        {
            GPExecutor.Instance.ResumeGame();
            Advertisements.Instance.ShowInterstitial(default, "Close_Starter_Pack");
        }
        
        gameObject.SetActive(false);
    }
}
