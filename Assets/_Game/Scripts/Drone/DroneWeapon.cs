using UnityBase.DesignPattern;
using UnityEngine;

public class DroneWeapon : MonoBehaviour
{
    public Weapon weapon;
    public Transform WeaponTarget => weapon.weaponAutoAim.target;
    private WeaponLocalData Data => weapon.weaponLocalData;
    
    private void EnableWeapon() => weapon.enabled = true;
    private void DisableWeapon() => weapon.enabled = false;
    
    private void OnValidate()
    {
        if (weapon == null)
            weapon = GetComponentInChildren<Weapon>();
    }

    private void Start()
    {
        InitWeapon();
        InitObserver();
    }
    private void InitObserver()
    {
        Observer.Instance.AddObserver(EventID.Dead, DisableWeapon);
        Observer.Instance.AddObserver(EventID.Revive, EnableWeapon);
    }
    
    private void InitWeapon()
    {
        weapon.Init(transform, true);
        weapon.InitProjectile();
        GetWeaponData();
    }

    private void GetWeaponData()
    {
        weapon.localDamage = Data.stats[0].damage;
        weapon.localFireRate = Data.stats[0].fireRate;
    }
}
