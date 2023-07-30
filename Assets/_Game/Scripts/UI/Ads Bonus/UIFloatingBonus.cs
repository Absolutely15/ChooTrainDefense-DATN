using ATSoft;
using ATSoft.Ads;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;

public class UIFloatingBonus : MonoBehaviour
{
    public DotweenScale dotweenScale;
    [SerializeField] private CurrencyType currencyType;
    [SerializeField] private RectTransform rectTransform;
    [Space]
    [SerializeField] private RectTransform startPos;
    [SerializeField] private RectTransform endPos;
    [Space]
    [SerializeField] private Button bonusBtn;
    [SerializeField] private TextMeshProUGUI amountText;
    [SerializeField] private int minBonusAmount;
    [SerializeField] private int maxBonusAmount;


    private Tween sinTween;
    private Vector3 newPos;
    private int bonusAmount;
    private void Start()
    {
        Init();
    }

    private void OnEnable()
    {
        GetBonusData();
        DotweenFeedback();
    }

    private void Init()
    {
        bonusBtn.onClick.AddListener(() =>
        {
            UnityAction<bool> actionComplete = delegate(bool isSuccess)
            {
                AddBonus();
                dotweenScale.OnClose();
            };
            Advertisements.Instance.ShowRewardedVideo(actionComplete, $"Floating_Bonus_{currencyType}");
        });
    }

    private void DotweenFeedback()
    {
        sinTween?.Kill();
        sinTween = DOVirtual.Float(0, 1, 5, t =>
        {
            newPos.Set(Mathf.Lerp(startPos.position.x, endPos.position.x, t), startPos.position.y + Mathf.Sin(t * 15) * 100, 0);
            rectTransform.position = newPos;
        }).SetEase(Ease.Linear).OnComplete(() => gameObject.SetActive(false));
    }

    private void GetBonusData()
    {
        bonusAmount = Random.Range(minBonusAmount, maxBonusAmount + 1);
        amountText.text = $"+{bonusAmount}";
    }
    private void AddBonus()
    {
        switch (currencyType)
        {
            case CurrencyType.Gold:
                PlayerSave.Gold += bonusAmount;
                AnalyticsManager.LogEventGoldEarn(bonusAmount, "floating_bonus_gold");
                break;
            case CurrencyType.Diamond:
                PlayerSave.Diamond += bonusAmount;
                AnalyticsManager.LogEventDiamondEarn(bonusAmount, "floating_bonus_diamond");
                break;
        }
    }
}
