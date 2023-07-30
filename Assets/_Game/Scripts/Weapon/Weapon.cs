using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Events;

public class Weapon : MonoBehaviour
{
    [Title("Event")]
    public UnityEvent<Weapon> onFireEvent = new UnityEvent<Weapon>();
    public UnityEvent<Weapon> onFireProjectile = new UnityEvent<Weapon>();

    [Title("Ref")]
    public WeaponLocalData weaponLocalData;

    [Title("Weapon Global Stats")]
    public float globalDamage = 1;
    public float globalFireRate = 1;
    
    [Title("Weapon Local Stats")]
    public int localDamage;
    public float localFireRate;
    
    [Title("Projectile Pool")]
    public Transform projectileSpawnPoint;
    public Projectile projectilePrefab;

    [HideInInspector] public Projectile projectile;
    [HideInInspector] public WeaponAutoAim weaponAutoAim;
    [HideInInspector] public WeaponSkin weaponSkin;
    public Transform WeaponTarget => weaponAutoAim.target;
    protected float LastFired;
    
    private readonly List<Projectile> projectilePool = new List<Projectile>();
    private const int PRE_COUNT = 20;
    private int weaponLevel;
    
    private bool isInit;
    private bool isProjectileInit;
    public void OnValidate()
    {
        if (weaponAutoAim == null)
            weaponAutoAim = GetComponent<WeaponAutoAim>();
        if (weaponSkin == null)
            weaponSkin = GetComponent<WeaponSkin>();
    }

    public void Init(Transform owner, bool ignoreObstacles = false)
    {
        if (isInit) return;
        weaponAutoAim.Init(owner, weaponLocalData, ignoreObstacles);
        isInit = true;
    }

    public void InitProjectile()
    {
        //projectile pool
        if (projectilePrefab == null || isProjectileInit) return;
        isProjectileInit = true;
        //Code lai preCount sau vi dua vao fire rate va thoi gian projectile dispose
        for (var i = 0; i < PRE_COUNT; i++)
        {
            var clone = Instantiate(projectilePrefab, transform);
            clone.Init(weaponLocalData);
            clone.gameObject.SetActive(false);
            projectilePool.Add(clone);
        }
    }
    //Player Weapon
    public virtual void GetWeaponStat()
    {
        weaponLevel = weaponLocalData.GetWeaponLevel();
        localDamage = weaponLocalData.stats[weaponLevel].damage;
        localFireRate = weaponLocalData.stats[weaponLevel].fireRate;
    }

    public virtual void Update()
    {
        if (WeaponTarget != null) Shoot();
    }

    protected virtual void Shoot()
    {
        if (!(Time.time > LastFired + 1f / (globalFireRate * localFireRate))) return;
        LastFired = Time.time;

        projectile = GetProjectileFromPool();
        projectile.destination = WeaponTarget.position + Vector3.up * 0.3f;
        onFireProjectile.Invoke(this);
        onFireEvent.Invoke(this);
    }

    protected Projectile GetProjectileFromPool()
    {
        foreach (var t in projectilePool)
        {
            if (t.gameObject.activeInHierarchy) continue;
            t.gameObject.SetActive(true);
            return t;
        }
        return null;
    }
}
