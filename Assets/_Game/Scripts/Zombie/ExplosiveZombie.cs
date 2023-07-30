using NVTT;
using Sirenix.OdinInspector;
using UnityEngine;

public class ExplosiveZombie : Zombie
{
    [Title("Explosive")]
    [SerializeField] private ParticleSystem explosiveParticle;

    private int areaDamage;

    protected override void InitData()
    {
        base.InitData();
        areaDamage = zombieData.explosiveDamage[MapID];
    }

    protected override void ChangeStateDie(Health h)
    {
        base.ChangeStateDie(h);
        Explosive(h);
    }

    private void Explosive(Health h)
    {
        DealDamage.DealDamage(h, areaDamage);
        if (IsTargetInRange(Utilities.Player.transform, 1f) && Utilities.Player.health.currentHealth > 0)
            AttackPlayer();
        //VFX
        explosiveParticle.Play();
    }
}
