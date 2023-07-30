using ATSoft.Ads;
using DG.Tweening;
using UnityBase.DesignPattern;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIAdsRestoreHealth : MonoBehaviour
{
    [SerializeField] private Button restoreHealthBtn;
    [SerializeField] private Image imageFill;

    private void Start()
    {
        Observer.Instance.AddObserver(EventID.EndGameLevel, DismissGameObject);
        Observer.Instance.AddObserver(EventID.Dead, DismissGameObject);
        
        restoreHealthBtn.onClick.AddListener(() =>
        {
            UnityAction<bool> actionComplete = delegate(bool isSuccess)
            {
                RestoreSingleHealth();
            };
            Advertisements.Instance.ShowRewardedVideo(actionComplete, "Restore_Single_Health");
        });
    }
    private void OnEnable()
    {
        DOVirtual.Float(1, 0, 5, t =>
        {
            imageFill.fillAmount = t;
        }).OnComplete(() => gameObject.SetActive(false));
    }

    private void DismissGameObject()
    {
        if (gameObject.activeInHierarchy)
            gameObject.SetActive(false);
    } 
    
    private void RestoreSingleHealth()
    {
        Observer.Instance.Notify(EventID.RestoreSingleHealth);
        DismissGameObject();
    }
}
