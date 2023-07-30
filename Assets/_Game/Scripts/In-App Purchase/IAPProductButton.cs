using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;
using NVTT;

public class IAPProductButton : MonoBehaviour
{
    #region Properties
    [SerializeField] private ProductType productType;
    [ValueDropdown("GetProductsID")]
    [SerializeField] private string productId;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI priceText;
    
    public Button purchaseBtn;

    private Product product;
    #endregion

    #region Unity Functions
    private void OnValidate()
    {
        if (purchaseBtn == null)
            purchaseBtn = GetComponent<Button>();
    }

    private void Awake()
    {
        Init();
    }

    private void OnEnable()
    {
        if (InAppPurchaseManager.InitializationComplete)
            UpdateText(product);
    }

    #endregion

    #region Init
    private void Init()
    {
        product = InAppPurchaseManager.Instance.GetProduct(productId);
        
        purchaseBtn.onClick.AddListener(PurchaseProduct);
    }

    private void PurchaseProduct()
    {
        InAppPurchaseManager.Instance.BuyProductID(productId);
    }

    private List<string> GetProductsID() => IAPUtilities.GetProductsIDType(productType);

    private void UpdateText(Product production)
    {
        if (titleText != null)
            titleText.text = production.metadata.localizedTitle;

        if (descriptionText != null)
            descriptionText.text = production.metadata.localizedDescription;
            
        if (priceText != null)
            priceText.text = production.metadata.localizedPriceString;
    }
    #endregion
}
