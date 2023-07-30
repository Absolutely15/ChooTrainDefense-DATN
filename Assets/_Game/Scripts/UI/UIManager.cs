using System.Linq;
using Sirenix.OdinInspector;
using TMPro;
using UnityBase.DesignPattern;
using UnityEngine;
using DG.Tweening;
using NVTT;
using static NVTT.Utilities;
public class UIManager : Singleton<UIManager>
{
    #region Properties
    [Title("Main Menu Top")]
    public GameObject progressBar;
    public GameObject shopBtn;
    public GameObject inventoryBtn;
    public DotweenScale environmentProgress;
    public DotweenScale settingBtn;

    [Title("Main Menu Bot")]
    public DotweenScale playBtn;
    public DotweenScale rebuildBtn;

    [Title("Inventory")]
    public UIInventory inventoryPanel;
    public UIInAppPurchase iapManagerPanel;
    public UIVipPackExtra starterPackPanel;
    
    [Title("Pop-up Victory")]
    public DotweenScale victoryGroup;
    public TextMeshProUGUI victoryLevelText;
    public ParticleSystem confetti;

    [Title("Pop-up Game Over")]
    public GameObject gameOverPanel;

    [HideInInspector] public bool isDoingBarrierTut;
    #endregion

    #region Init
    private void Start()
    {
        // Observer
        Observer.Instance.AddObserver(EventID.StartGameLevel, OnStartGameLevel);
        Observer.Instance.AddObserver(EventID.EndGameLevel, OnEndGameLevel);
        Observer.Instance.AddObserver(EventID.InUpgradeZone, InUpgradeZone);
        Observer.Instance.AddObserver(EventID.OutUpgradeZone, OutUpgradeZone);
        Observer.Instance.AddObserver(EventID.RebuildBarrier, CheckBrokenBarrier);
        Observer.Instance.AddObserver(EventID.Dead, OnPlayerDeath);
        Observer.Instance.AddObserver(EventID.Revive, OnPlayerRevive);
    }
    #endregion
    
    #region Start / End Game Level
    private void OnStartGameLevel()
    {
        playBtn.OnClose();
        rebuildBtn.OnClose();
        environmentProgress.OnClose();
        settingBtn.OnClose();
        DOVirtual.DelayedCall(0.5f, () => progressBar.SetActive(true));
    }

    private void OnEndGameLevel()
    {
        victoryLevelText.text = $"Level {PlayerSave.CurrentGameLevel + 1}";

        var victorySeq = DOTween.Sequence();
        victorySeq.AppendCallback(() =>
            {
                confetti.Play();
                progressBar.SetActive(false);
                victoryGroup.gameObject.SetActive(true);
            })
            .AppendInterval(victoryGroup.scaleUpDuration)
            .AppendCallback(victoryGroup.OnClose)
            .AppendInterval(victoryGroup.scaleDownDuration)
            .AppendCallback(() =>
            {
                Observer.Instance.Notify(EventID.EndGameLevelData);

                CheckBrokenBarrier();
                environmentProgress.gameObject.SetActive(true);
                settingBtn.gameObject.SetActive(true);
                
                if (IsPassLevelTutAndRebuildTut) playBtn.gameObject.SetActive(true);
            });
    }
    #endregion
    
    #region Player Death / Revive
    private void OnPlayerDeath()
    {
        DOVirtual.DelayedCall(1f, () =>
        {
            gameOverPanel.SetActive(true);
            progressBar.SetActive(false);
            inventoryBtn.SetActive(false);
            shopBtn.SetActive(false);
        });
    }

    private void OnPlayerRevive()
    {
        gameOverPanel.SetActive(false);
        progressBar.SetActive(true);
        if (IsPassBasicTutorial) shopBtn.SetActive(true);
        if (IsInventoryBtnActive) inventoryBtn.SetActive(true);
    }
    
    #endregion
    
    #region Upgrade Zone
    private void InUpgradeZone()
    {
        if (Gameplay.IsInGameplay) return;
        playBtn.gameObject.SetActive(false);
        rebuildBtn.gameObject.SetActive(false);
    }

    private void OutUpgradeZone()
    {
        if (Gameplay.IsInGameplay || isDoingBarrierTut) return;
        SetPlayAndRebuildBtnActive();
    }

    public void SetPlayAndRebuildBtnActive()
    {
        playBtn.gameObject.SetActive(true);
        CheckBrokenBarrier();
    }
    #endregion

    #region Barrier
    private void CheckBrokenBarrier()
    {
        if (!IsPassBasicTutorial) return;
        rebuildBtn.gameObject.SetActive(Utilities.MapManager.barrierListSpawnable.Any(t => !t.health.IsFullHealth()));
    }
    #endregion

    #region In-app Purchase

    public void ScrollToGemsPack()
    {
        if (!iapManagerPanel.GemsPack())
            iapManagerPanel.gameObject.SetActive(true);
    }
    #endregion

}