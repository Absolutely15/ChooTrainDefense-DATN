using ATSoft.Ads;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIInAppPurchaseAdsGems : MonoBehaviour
{
    [SerializeField] private int bonusAmount = 10;
    [SerializeField] private Button adsBonusGemBtn;

    private void OnValidate()
    {
        adsBonusGemBtn = GetComponent<Button>();
    }

    private void Start()
    {
        adsBonusGemBtn.onClick.AddListener(AdsAddBonus);
    }

    private void AdsAddBonus()
    {
        UnityAction<bool> actionComplete = delegate(bool isSuccess)
        {
            PlayerSave.Diamond += bonusAmount;
        };
        Advertisements.Instance.ShowRewardedVideo(actionComplete, "Iap_Ads_Gem");
    }
}
