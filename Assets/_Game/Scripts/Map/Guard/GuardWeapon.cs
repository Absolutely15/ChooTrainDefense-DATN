using UnityBase.DesignPattern;
using UnityEngine;

public class GuardWeapon : MonoBehaviour
{
    public Transform guard;
    public Weapon weapon;
    
    public WeaponLocalData Data => weapon.weaponLocalData;
    private void OnValidate()
    {
        if (weapon == null)
            weapon = GetComponentInChildren<Weapon>();
    }

    private void Start()
    {
        InitEvent();
    }

    public void Init()
    {
        weapon.Init(guard, true);
        weapon.InitProjectile();
    }

    private void InitEvent()
    {
        Observer.Instance.AddObserver(EventID.Dead, w => SetWeaponEnable(false));
        Observer.Instance.AddObserver(EventID.Revive, w => SetWeaponEnable(true));
    }
    
    private void SetWeaponEnable(bool isEnabled)
    {
        weapon.enabled = isEnabled;
    }
    
    public void GetWeaponData(int lev)
    {
        weapon.localDamage = (int) (Data.stats[0].damage * Mathf.Pow(1.2f, lev - 1));
        weapon.localFireRate = Data.stats[0].fireRate * Mathf.Pow(1.2f, lev - 1);
    }
}
