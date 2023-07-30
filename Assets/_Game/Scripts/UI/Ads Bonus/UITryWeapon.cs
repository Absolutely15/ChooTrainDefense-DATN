using System.Collections.Generic;
using ATSoft.Ads;
using DG.Tweening;
using UnityBase.DesignPattern;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static NVTT.Utilities;

public class UITryWeapon : MonoBehaviour
{
    [SerializeField] private List<WeaponLocalData> weaponsUnlockable;
    [SerializeField] private Button tryWeaponBtn;
    [SerializeField] private Image imageFill;
    [SerializeField] private Image weaponIcon;
    [Space]
    [SerializeField] private bool isTryingWeapon;
    [SerializeField] private int randomWeaponID;
    [SerializeField] private int currentUsingWeaponID;
    private void Start()
    {
        Observer.Instance.AddObserver(EventID.EndGameLevelData, SetBackCurrentWeapon);
        tryWeaponBtn.onClick.AddListener(() =>
        {
            UnityAction<bool> actionComplete = delegate(bool isSuccess)
            {
                TryWeapon();
            };
            Advertisements.Instance.ShowRewardedVideo(actionComplete, "Try_Weapon");
        });
    }
    
    private void OnEnable()
    {
        GetWeaponUnlockableList();
        if (weaponsUnlockable.Count == 0)
        {
            gameObject.SetActive(false);
            return;
        }
        GetWeaponTryingID();
    }
    
    private void OnDestroy()
    {
        if (!isTryingWeapon) return;
        PlayerSave.SetWeaponIsUsing(currentUsingWeaponID);
    }

    private void GetWeaponTryingID()
    {
        currentUsingWeaponID = PlayerSave.GetWeaponIsUsing();
        randomWeaponID = weaponsUnlockable.Rand(0).id;

        //UI
        weaponIcon.sprite = Player.weaponController.weaponList[randomWeaponID].weaponLocalData.icon;
        
        DOVirtual.Float(1, 0, 5, t =>
        {
            imageFill.fillAmount = t;
        }).OnComplete(() => gameObject.SetActive(false));
    }
    
    private void TryWeapon()
    {
        isTryingWeapon = true;
        PlayerSave.SetWeaponIsUsing(randomWeaponID);
        Player.weaponController.EquipWeapon(Player.CurrentWeapon);
        gameObject.SetActive(false);
        Observer.Instance.Notify(EventID.TryWeapon);
    }

    private void SetBackCurrentWeapon()
    {
        if (!isTryingWeapon) return;
        isTryingWeapon = false;
        PlayerSave.SetWeaponIsUsing(currentUsingWeaponID);
        Player.weaponController.EquipWeapon(Player.CurrentWeapon);
    }

    private void GetWeaponUnlockableList()
    {
        weaponsUnlockable.Clear();
        foreach (var wData in GameDB.weaponList)
        {
            if (wData.GetWeaponState() == WeaponState.Unlockable)
                weaponsUnlockable.Add(wData);
        }
    }
}
