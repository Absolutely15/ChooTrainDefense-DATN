using System.Collections.Generic;
using UnityBase.DesignPattern;
using UnityEngine;
using static NVTT.Utilities;
public class PlayerWeapon : MonoBehaviour
{
    public List<Weapon> weaponList;
    public Weapon CurrentWeapon => weaponList[PlayerSave.GetWeaponIsUsing()];
    private void Awake()
    {
        Init();
        InitEvent();
    }

    private void Init()
    {
        EquipWeapon(CurrentWeapon);
        SetCurrentGunActive();
    }

    private void InitEvent()
    {
        Observer.Instance.AddObserver(EventID.UpgradeWeaponGlobal, g => GetWeaponGlobalStat(CurrentWeapon));
        Observer.Instance.AddObserver(EventID.ChangeSkin, g => GetWeaponGlobalStat(CurrentWeapon));
        Observer.Instance.AddObserver(EventID.ChangeWeapon, SetCurrentGunActive);
    }
    
    public void EquipWeapon(Weapon weapon)
    {
        GetWeaponGlobalStat(weapon);
        GetWeaponSkinState(weapon);
        weapon.Init(transform);
        weapon.InitProjectile();
        weapon.GetWeaponStat();
        Observer.Instance.Notify(EventID.ChangeWeapon);
    }

    private void SetCurrentGunActive()
    {
        for (var i = 0; i < weaponList.Count; i++)
        {
            weaponList[i].gameObject.SetActive(i == PlayerSave.GetWeaponIsUsing());
        }
    }
    private static void GetWeaponGlobalStat(Weapon weapon)
    {
        weapon.globalDamage = GameDB.GetGlobalDamageUpgradeAt(PlayerSave.WeaponGlobalDamageLevel) * Player.skinController.GetSkinBonusStat(SkinBonusType.Damage, 1);
        weapon.globalFireRate = GameDB.GetFireRateUpgradeAt(PlayerSave.WeaponGlobalFireRateLevel) * Player.skinController.GetSkinBonusStat(SkinBonusType.FireRate, 1);
    }

    private static void GetWeaponSkinState(Weapon weapon)
    {
        weapon.weaponSkin.GetCurrentMat(weapon);
    }
}
