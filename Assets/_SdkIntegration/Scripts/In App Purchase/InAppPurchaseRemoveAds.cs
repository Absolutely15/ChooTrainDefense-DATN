using System;
using System.Collections;
using System.Collections.Generic;
using ATSoft.Ads;
using UnityEngine.Purchasing;
using UnityEngine;
using UnityEngine.UI;

public class InAppPurchaseRemoveAds : MonoBehaviour
{
    [SerializeField] private string keyRemoveAds;
    private Button button;
    
    private void Start()
    {
        button = GetComponent<Button>();
        if (button != null)
        {
            var canShowAds = Advertisements.Instance.CanShowAds();
            button.gameObject.SetActive(canShowAds);
        }
    }

    public void OnPurchaseComplete(Product product)
    {
        if(string.CompareOrdinal(product.definition.id,keyRemoveAds) == 0)
        {
            Advertisements.Instance.RemoveAds(true);
            
            if (button != null)
            {
                button.interactable = false;
                StartCoroutine(DisableButton());
            }
        }    
    }

    IEnumerator DisableButton()
    {
        yield return new WaitForEndOfFrame();
        
        if (button != null)
        {
            button.gameObject.SetActive(false);
        }
    }
    
    public void OnPurchaseFailed(Product product, PurchaseFailureReason reason)
    {
        Debug.Log("Purchase of " + product.definition.id + "  failed due to " + reason); 
    }
}
