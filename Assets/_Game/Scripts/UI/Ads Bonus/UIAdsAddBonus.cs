using ATSoft;
using ATSoft.Ads;
using TMPro;
using UnityBase.DesignPattern;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIAdsAddBonus : MonoBehaviour
{
    [SerializeField] private CurrencyType currencyType;
    [SerializeField] private DotweenScale dotweenScale;
    [Space]
    [SerializeField] private Button earnBonusBtn;
    [SerializeField] private Button noThanksBtn;
    [SerializeField] private TextMeshProUGUI textDesc;
    [SerializeField] private TextMeshProUGUI bonusAmountText;
    [SerializeField] private int bonusAmount;
    
    private void OnEnable()
    {
        GPExecutor.Instance.PauseGame();
    }
    private void Start()
    {
        Init();
    }

    private void Init()
    {
        textDesc.text = $"Watch a video to earn {bonusAmount} free";
        bonusAmountText.text = $"+{bonusAmount}";
        
        earnBonusBtn.onClick.AddListener(() =>
        {
            UnityAction<bool> actionComplete = delegate(bool isSuccess)
            {
                AddBonus();
                OnClick();
            };
            Advertisements.Instance.ShowRewardedVideo(actionComplete, $"Add_Bonus_{currencyType}");
        });
        noThanksBtn.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        dotweenScale.OnClose();
        Observer.Instance.Notify(EventID.DismissBonusCurrency);
        
        if (UIAdsManager.AdsAddBonusResumeConditionCheck()) return;
        GPExecutor.Instance.ResumeGame();
    }

    private void AddBonus()
    {
        switch (currencyType)
        {
            case CurrencyType.Gold:
                PlayerSave.Gold += bonusAmount;
                AnalyticsManager.LogEventGoldEarn(bonusAmount, "ads_add_bonus");
                break;
            case CurrencyType.Diamond:
                PlayerSave.Diamond += bonusAmount;
                AnalyticsManager.LogEventDiamondEarn(bonusAmount, "ads_add_bonus");
                break;
        }
    }
}
