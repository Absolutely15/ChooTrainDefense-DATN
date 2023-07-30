using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Purchasing;
using NVTT;

public class IAPProductText : MonoBehaviour
{
    #region Properties

    [SerializeField] private ProductType productType;
    [ValueDropdown("GetProductsID")]
    [SerializeField] private string productId;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI priceText;

    private Product product;
    #endregion

    #region Unity Functions
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
    }
    
    private void UpdateText(Product production)
    {
        if (titleText != null)
            titleText.text = production.metadata.localizedTitle;

        if (descriptionText != null)
            descriptionText.text = production.metadata.localizedDescription;
            
        if (priceText != null)
            priceText.text = production.metadata.localizedPriceString;
    }
    
    private List<string> GetProductsID() => IAPUtilities.GetProductsIDType(productType);
    #endregion
}
