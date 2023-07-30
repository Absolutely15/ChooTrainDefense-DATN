using ATSoft;
using ATSoft.Ads;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;

public class UIEndLevelBonus : MonoBehaviour
{
    public DotweenScale dotweenScale;
    [SerializeField] private Button bonusBtn;
    [SerializeField] private Image barFill;
    [SerializeField] private TextMeshProUGUI goldBonusText;

    private void OnValidate()
    {
        if (dotweenScale == null)
            dotweenScale = GetComponent<DotweenScale>();
    }

    private void Awake()
    {
        bonusBtn.onClick.AddListener(() =>
        {
            UnityAction<bool> actionComplete = delegate(bool isSuccess)
            {
                PlayerSave.Gold += CollectibleCollector.CollectedGoldThisLevel * 2;
                AnalyticsManager.LogEventGoldEarn(CollectibleCollector.CollectedGoldThisLevel * 2, "end_level_bonus");
                dotweenScale.OnClose();
            };
            Advertisements.Instance.ShowRewardedVideo(actionComplete, "End_Level_Bonus");
        });
    }

    private void OnEnable()
    {
        AdsBonusFeedback();
    }

    private void AdsBonusFeedback()
    {
        barFill.fillAmount = 1;
        goldBonusText.text = $"+{CollectibleCollector.CollectedGoldThisLevel * 2}";
        DOVirtual.Float(1, 0, 5f, t =>
        {
            barFill.fillAmount = t;
        }).OnComplete(() => dotweenScale.OnClose());
    }
}
