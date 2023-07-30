using ATSoft.Ads;
using TMPro;
using UnityBase.DesignPattern;
using UnityEngine;
using UnityEngine.UI;
using static NVTT.Utilities;
using static NVTT.UIUtilities;

public class UpgradeButton : MonoBehaviour
{
    public RectTransform upgradeTransform;
    public Button upgradeBtn;
    public TextMeshProUGUI upgradeLevel;
    [Space]
    public Image upgradeAvailable;
    public TextMeshProUGUI upgradeGold;
    [Space]
    public Image upgradeMax;
    [Space]
    public Image upgradeAds;
    
    private IUpgradeStat iUpgradeStat;
    private void Awake()
    {
        Init();
        iUpgradeStat.Init();
    }

    private void Init()
    {
        iUpgradeStat = GetComponent<IUpgradeStat>();
        upgradeBtn.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        upgradeTransform.InteractableBtnResponse(iUpgradeStat.GoldCost, iUpgradeStat.CurLevelUI, iUpgradeStat.LevelMax, Advertisements.Instance.IsRewardVideoAvailable(),
            () =>
            {
                iUpgradeStat.UpgradeLevel++;
                iUpgradeStat.UpgradeSuccess();
                Observer.Instance.Notify(iUpgradeStat.EventID);
                AudioManager.Instance.PlayAudio(AudioType.ClickUpgrade, true);
            }, iUpgradeStat.EventID.ToString());
    }
    
    public void LoadData()
    {
        iUpgradeStat.LoadData();
        upgradeLevel.text = $"{iUpgradeStat.Description} \n Level {iUpgradeStat.CurLevelUI}";

        if (IsMaxLevel(iUpgradeStat.CurLevelUI, iUpgradeStat.LevelMax))
        {
            upgradeGold.text = "MAX LEVEL";
            upgradeBtn.targetGraphic = upgradeMax;
            UIActiveType(UpgradeState.Max);
            return;
        }

        if (IsEnoughGold(iUpgradeStat.GoldCost) || !IsShowUpgradeRewardAds)
        {
            upgradeBtn.targetGraphic = upgradeAvailable;
            EnoughGoldTextResponse(iUpgradeStat.GoldCost, upgradeGold);
            UIActiveType(UpgradeState.Available);
            return;
        }

        upgradeBtn.targetGraphic = upgradeAds;
        UIActiveType(UpgradeState.Ads);
    }

    private void UIActiveType(int i)
    {
        upgradeAvailable.gameObject.SetActive(i == UpgradeState.Available);
        upgradeMax.gameObject.SetActive(i == UpgradeState.Max);
        upgradeAds.gameObject.SetActive(i == UpgradeState.Ads);
    }

    private static class UpgradeState
    {
        public const int Available = 0;
        public const int Max = 1;
        public const int Ads = 2;
    }
}
