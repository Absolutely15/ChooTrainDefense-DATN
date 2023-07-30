using System;
using ATSoft;
using TMPro;
using UnityBase.DesignPattern;
using UnityEngine;
using UnityEngine.UI;
using static NVTT.UIUtilities;
using static NVTT.Utilities;

[DefaultExecutionOrder(-2)]
public class GunFloat : MonoBehaviour
{
    [SerializeField] private WeaponID weaponID;
    [SerializeField] private DotweenScale weaponModel;
    [SerializeField] private DotweenScale canvasPopUp;
    [SerializeField] private RectTransform unlockBtnTransform;
    [SerializeField] private Button unlockBtn;
    [SerializeField] private Button equipBtn;
    [SerializeField] private TextMeshProUGUI weaponName;
    [SerializeField] private TextMeshProUGUI damageStat;
    [SerializeField] private TextMeshProUGUI fireRateStat;
    [SerializeField] private TextMeshProUGUI unlockCostText;

    private WeaponLocalData weaponFloating;
    private int weaponLevel;
    private int curWeaponIsUsing;
    private void OnEnable()
    {
        OnChangeWeapon();
    }

    private void Start()
    {
        Init();
        InitEvent();
    }

    private void Init()
    {
        weaponFloating = GameDB.weaponList[(int)weaponID];
        weaponLevel = weaponFloating.GetWeaponLevel();

        weaponName.text = $"{weaponFloating.itemName}";
        damageStat.text = $"{weaponFloating.stats[weaponLevel].damage}";
        fireRateStat.text = $"{weaponFloating.stats[weaponLevel].fireRate}";
        unlockCostText.text = $"{weaponFloating.unlockDiamondCost}";
        
        canvasPopUp.gameObject.SetActive(false);

    }

    private void InitEvent()
    {
        unlockBtn.onClick.AddListener(UnlockBtnOnClick);
        equipBtn.onClick.AddListener(EquipBtnOnClick);
        Observer.Instance.AddObserver(EventID.ChangeWeapon, OnChangeWeapon);
    }

    private void UnlockBtnOnClick()
    {
        if (unlockBtnTransform.InteractableBtnDiamondResponse(weaponFloating.unlockDiamondCost))
        {
            PlayerSave.Diamond -= weaponFloating.unlockDiamondCost;
            weaponFloating.SetWeaponState(WeaponState.Unlocked);
            EquipBtnOnClick();
            //Event firebase
            AnalyticsManager.LogEventDiamondSpend(weaponFloating.unlockDiamondCost, "unlock_gun_float",
                weaponID.ToString());
        }
        else
        {
            //Ads
            UIAdsManager.Instance.PopUpNotEnoughDiamond();
        }
    }

    private void EquipBtnOnClick()
    {
        weaponFloating.SetWeaponIsUsing();
        Player.weaponController.EquipWeapon(Player.weaponController.CurrentWeapon);
        canvasPopUp.OnClose();
        weaponModel.OnClose();
        AudioManager.Instance.PlayAudio(AudioType.Equip);
    }

    private void BtnActive(WeaponState weaponState)
    {
        unlockBtn.gameObject.SetActive(weaponState == WeaponState.Unlockable);
        equipBtn.gameObject.SetActive(weaponState == WeaponState.Unlocked || weaponState == WeaponState.Golden);
    }

    private void OnChangeWeapon()
    {
        curWeaponIsUsing = PlayerSave.GetWeaponIsUsing();
        
        if ((int) weaponID == curWeaponIsUsing)
            weaponModel.OnClose();
        else
            weaponModel.gameObject.SetActive(true);
    }
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Player") || (int) weaponID == curWeaponIsUsing) return;
        canvasPopUp.gameObject.SetActive(true);
        BtnActive(weaponFloating.GetWeaponState());
        
        //Ads
        UIAdsManager.Instance.PopUpFloatingDiamond();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Player") || (int) weaponID == curWeaponIsUsing) return;
        canvasPopUp.OnClose();
    }
}
