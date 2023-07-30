using ATSoft;
using ATSoft.Ads;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UINotEnoughCurrency : MonoBehaviour
{
    [SerializeField] private CurrencyType currencyType;
    [SerializeField] private DotweenScale dotweenScale;
    [Space]
    [SerializeField] private Button earnBonusBtn;
    [SerializeField] private Button moreCurrencyBtn;
    [SerializeField] private Button noThanksBtn;
    [SerializeField] private TextMeshProUGUI textDesc;
    [SerializeField] private TextMeshProUGUI bonusAmountText;
    [SerializeField] private int bonusAmount;

    private Tween tween;
    private void OnEnable()
    {
        GPExecutor.Instance.PauseGame();
        
        tween = DOVirtual.DelayedCall(3, () => noThanksBtn.gameObject.SetActive(true));
    }

    private void OnDisable()
    {
        tween?.Kill();
        noThanksBtn.gameObject.SetActive(false);
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
        moreCurrencyBtn.onClick.AddListener(MoreCurrencyOnClick);
        noThanksBtn.onClick.AddListener(OnClick);
    }

    private void MoreCurrencyOnClick()
    {
        dotweenScale.OnClose();
        UIManager.Instance.ScrollToGemsPack();
    }

    private void OnClick()
    {
        dotweenScale.OnClose();
        if (UIAdsManager.AdsAddBonusResumeConditionCheck()) return;
        GPExecutor.Instance.ResumeGame();
    }

    private void AddBonus()
    {
        switch (currencyType)
        {
            case CurrencyType.Gold:
                PlayerSave.Gold += bonusAmount;
                AnalyticsManager.LogEventGoldEarn(bonusAmount, "ads_not_enough_currency");
                break;
            case CurrencyType.Diamond:
                PlayerSave.Diamond += bonusAmount;
                AnalyticsManager.LogEventDiamondEarn(bonusAmount, "ads_not_enough_currency");
                break;
        }
    }
}
