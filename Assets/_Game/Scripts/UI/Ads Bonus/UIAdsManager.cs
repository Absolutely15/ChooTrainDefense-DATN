using System.Collections.Generic;
using ATSoft.Ads;
using Sirenix.OdinInspector;
using UnityBase.DesignPattern;
using UnityEngine;

public class UIAdsManager : Singleton<UIAdsManager>
{
    [Title("Ads Bonus")]
    public UIAdsAddBonus uiAdsBonusGold;
    public UINotEnoughCurrency uiNotEnoughDiamond;
    //public UIAdsBonusSpeed uiAdsBonusSpeed;
    
    [Title("End Game Level")]
    public UIEndLevelBonus uiEndLevelBonus;
    public UIFloatingBonus uiFloatingBonusDiamond;
    public UIFloatingBonus uiFloatingBonusGold;
    public UITryWeapon uiTryWeapon;

    private static readonly List<int> StartPackLevel = new List<int> {5, 9};

    private static readonly List<int> AdsInterEndLevel = new List<int> { 2, 3, 4, 8 };
    //private bool adsRHCheck;

    private const float COOLDOWN = 10f;
    private float readyTime;
    private void Start()
    {
        //Init();
        InitEvent();
    }

    private void Init()
    {
        // if (PlayerSave.CurrentGameLevel >= 1)
        //     uiAdsBonusSpeed.gameObject.SetActive(true);
    }

    private void InitEvent()
    {
        Observer.Instance.AddObserver(EventID.StartGameLevel, OnStartGameLevel);
        Observer.Instance.AddObserver(EventID.EndGameLevelData, OnEndGameLevel);
        Observer.Instance.AddObserver(EventID.PopUpBonusGold, PopUpBonusGold);
        
        // Player.health.onSufferDamage.AddListener((a, b) =>
        // {
        //     if (PlayerSave.CurrentGameLevel < 1 || adsRHCheck) return;
        //     adsRHCheck = true;
        //     uiAdsRestoreHealth.gameObject.SetActive(true);
        // });
    }

    private void OnStartGameLevel()
    {
        // if (uiAdsBonusSpeed.gameObject.activeInHierarchy)
        //     uiAdsBonusSpeed.gameObject.SetActive(false);

        if (PlayerSave.CurrentGameLevel < 2) return;
        uiEndLevelBonus.dotweenScale.OnClose();

        if (((PlayerSave.CurrentGameLevel + 1) % 2 == 1 ||
             GPRainbowDefense.CheckPoint.Contains(PlayerSave.CurrentGameLevel + 1)) &&
            PlayerSave.CurrentGameLevel + 1 >= 5)
            uiTryWeapon.gameObject.SetActive(true);

    }
    private void OnEndGameLevel()
    {
        if (PlayerSave.CurrentGameLevel >= 2) uiEndLevelBonus.gameObject.SetActive(true);
        
        if (PlayerSave.CurrentGameLevel > 10)
            Advertisements.Instance.ShowInterstitial(default, "End_Level");
        else
        {
            if (AdsInterEndLevel.Contains(PlayerSave.CurrentGameLevel))
                Advertisements.Instance.ShowInterstitial(default, "End_Level");
        
            if (StartPackLevel.Contains(PlayerSave.CurrentGameLevel))
                UIManager.Instance.starterPackPanel.gameObject.SetActive(true);
        }
        
        // if (PlayerSave.CurrentGameLevel < 1) return;
        //adsRHCheck = false;
        //uiAdsBonusSpeed.gameObject.SetActive(true);
    }

    public void PopUpNotEnoughDiamond() => uiNotEnoughDiamond.gameObject.SetActive(true);

    private void PopUpBonusGold() => uiAdsBonusGold.gameObject.SetActive(true);

    public void PopUpFloatingDiamond()
    {
        if (PlayerSave.CurrentGameLevel < 5) return;
        PopUpFloatingBonus(uiFloatingBonusDiamond.gameObject,
            uiFloatingBonusGold.gameObject, ref readyTime);
    }

    public void PopUpFloatingGold()
    {
        if (PlayerSave.CurrentGameLevel < 6) return;
        PopUpFloatingBonus(uiFloatingBonusGold.gameObject,
            uiFloatingBonusDiamond.gameObject, ref readyTime);
    }

    private static void PopUpFloatingBonus(GameObject go1, GameObject go2, ref float cappingTime)
    {
        if (go2.activeInHierarchy || cappingTime > Time.time) return;
        go1.SetActive(true);
        cappingTime = Time.time + COOLDOWN;
    }

    public static bool AdsAddBonusResumeConditionCheck()
    {
        return UIManager.Instance.inventoryPanel.gameObject.activeInHierarchy ||
               UIManager.Instance.iapManagerPanel.gameObject.activeInHierarchy;
    }
}
