using ATSoft.Ads;
using DG.Tweening;
using TMPro;
using UnityBase.DesignPattern;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static NVTT.Utilities;

public class GameOverPanel : MonoBehaviour
{
    public Button revive;
    public Button noThanks;
    public TextMeshProUGUI level;

    private void Start()
    {
        revive.onClick.AddListener(() =>
        {
            UnityAction<bool> actionComplete = delegate(bool isSuccess)
            {
                Revive();
            };
            Advertisements.Instance.ShowRewardedVideo(actionComplete, "Revive");
        });
        noThanks.onClick.AddListener(() =>
        {
            noThanks.interactable = false;
            Advertisements.Instance.ShowInterstitial(Replay, "No_Thanks", false);
        });
    }

    private void OnEnable()
    {
        level.text = $"Level {PlayerSave.CurrentGameLevel + 1}";
        DOVirtual.DelayedCall(3f,() =>
        {
            noThanks.interactable = true;
            noThanks.gameObject.SetActive(true);
        });
    }

    private void OnDisable()
    {
        noThanks.gameObject.SetActive(false);
    }

    private void Revive()
    {
        if (Player.health.revivable)
        {
            Observer.Instance.Notify(EventID.Revive);
        }
    }

    private void Replay()
    {
        GPExecutor.Instance.Replay();
    }
}
