using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAutoShoot : MonoBehaviour
{
    public float delayBeforeShootingAfterAcquiringTarget = 0f;

    private WeaponAutoAim weaponAutoAim;
    private Weapon weapon;
    private float targerAcquiredAt;
    private Transform lastTarget;

    public void OnValidate()
    {
        if (weaponAutoAim == null)
        {
            weaponAutoAim = GetComponent<WeaponAutoAim>();
        }

        if (weapon == null)
        {
            weapon = GetComponent<Weapon>();
        }
    }

    // protected void Update()
    // {
    //     HandleAutoShoot();
    // }
    //
    // private void HandleAutoShoot()
    // {
    //     if (weaponAutoAim.target != null)
    //     {
    //         if (lastTarget != weaponAutoAim.target)
    //         {
    //             targerAcquiredAt = Time.time;
    //         }
    //
    //         if (Time.time - targerAcquiredAt >= delayBeforeShootingAfterAcquiringTarget)
    //         {
    //             weapon.SpawnProjectile();
    //         }
    //
    //         lastTarget = weaponAutoAim.target;
    //     }
    // }
}
