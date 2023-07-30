using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class QuadRocket : Weapon
{
    [Title("Quad Rocket")]
    [SerializeField] private int bulletPerShoot;
    [SerializeField] private float offset;

    protected override void Shoot()
    {
        if (!(Time.time > LastFired + 1f / (globalFireRate * localFireRate))) return;
        LastFired = Time.time;
        DelayBullet(0.05f);
        onFireEvent.Invoke(this);
    }
    
    private void DelayBullet(float delayInterval)
    {
        for (var i = 0; i < bulletPerShoot; i++)
        {
            if (WeaponTarget == null) break;
            DOVirtual.DelayedCall(delayInterval * i, () =>
            {
                projectile = GetProjectileFromPool();
                projectile.destination = WeaponTarget.position +
                                         WeaponTarget.transform.right * Random.Range(-offset, offset) +
                                         Vector3.up * 0.05f;
                onFireProjectile.Invoke(this);
            }).SetId("QuadRocket");
        }
    }

    private void OnDisable()
    {
        DOTween.Kill("QuadRocket");
    }
}

