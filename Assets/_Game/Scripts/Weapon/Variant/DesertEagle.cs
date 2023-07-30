using Sirenix.OdinInspector;
using UnityEngine;

public class DesertEagle : Weapon
{
    [Title("Desert Eagle")] [SerializeField]
    private float offset;
    protected override void Shoot()
    {
        if (!(Time.time > LastFired + 1f / (globalFireRate * localFireRate))) return;
        LastFired = Time.time;

        projectile = GetProjectileFromPool();
        projectile.destination = WeaponTarget.position + Vector3.up * 0.3f + transform.forward * offset;
        onFireProjectile.Invoke(this);
        onFireEvent.Invoke(this);
    }
}
