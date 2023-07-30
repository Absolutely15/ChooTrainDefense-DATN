using ATSoft.Ads;
using DG.Tweening;
using UnityBase.DesignPattern;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIAdsBonusSpeed : MonoBehaviour
{
    [SerializeField] private Image imageFill;
    [SerializeField] private Button bonusSpeedBtn;
    private void Awake()
    {
        bonusSpeedBtn.onClick.AddListener(() =>
        {
            UnityAction<bool> actionComplete = delegate(bool isSuccess)
            {
                BonusSpeed();
            };
            Advertisements.Instance.ShowRewardedVideo(actionComplete, "Ads_Bonus_Speed");
        });
        
    }

    private void OnEnable()
    {
        DOVirtual.Float(1, 0, 5, t => imageFill.fillAmount = t).OnComplete(DismissBonusSpeed);
    }

    private void BonusSpeed()
    {
        Observer.Instance.Notify(EventID.BonusSpeed);
        gameObject.SetActive(false);
    }

    private void DismissBonusSpeed()
    {
        Observer.Instance.Notify(EventID.DismissBonusSpeed);
        gameObject.SetActive(false);
    }
}
