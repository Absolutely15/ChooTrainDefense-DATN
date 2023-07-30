using Sirenix.OdinInspector;
using UnityEngine;

public class LaserGun : Weapon
{
    #region Properties

    [Title("Laser Gun")]
    [SerializeField] private Laser laserLine;
    #endregion
    public override void Update()
    {
        if (laserLine.zombie == null) return;
        DealDamage(laserLine.zombie, (int)(globalDamage * localDamage));
    }

    private void DealDamage(Health target, int amount)
    {
        if (!(Time.time > LastFired + localFireRate * 1f / globalFireRate)) return;
        LastFired = Time.time;
        target.SufferDamage(amount);
    }
    
}
