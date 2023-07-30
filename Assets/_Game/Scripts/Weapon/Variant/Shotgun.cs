using Sirenix.OdinInspector;
using UnityEngine;
using NVTT;

public class Shotgun : Weapon
{
    [Title("Shotgun")]
    [SerializeField] private int bulletPerShoot;
    [SerializeField] private Vector2 offset;
    [SerializeField] private float centerAngle; 
    [SerializeField] private float betweenAngle = 10f;

    private float baseAngle;
    private Transform trans;

    public override void GetWeaponStat()
    {
        base.GetWeaponStat();
        bulletPerShoot = (int)localFireRate;
        baseAngle = bulletPerShoot % 2 == 0 ? centerAngle - betweenAngle / 2 : centerAngle;
    }

    protected override void Shoot()
    {
        if (!(Time.time > LastFired + 1f / (globalFireRate * 1.5f))) return;
        LastFired = Time.time;
        for (var i = 0; i < bulletPerShoot; i++)
        {
            var angle = Utilities.GetShiftedAngle(i, baseAngle, betweenAngle);
            projectile = GetProjectileFromPool();
            trans = transform;
            projectile.destination = WeaponTarget.position + Vector3.up * offset.y + trans.right * angle + trans.forward * offset.x;
            onFireProjectile.Invoke(this);
        }
        onFireEvent.Invoke(this);
    }
}
