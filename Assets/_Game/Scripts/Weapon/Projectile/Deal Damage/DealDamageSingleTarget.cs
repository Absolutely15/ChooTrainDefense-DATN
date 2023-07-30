using UnityEngine;

public class DealDamageSingleTarget : MonoBehaviour, IDealDamage
{
    public void DealDamage(Health target, int amount, float invincibilityDuration)
    {
        if (target.currentHealth >= 0)
        {
            target.SufferDamage(amount, invincibilityDuration);
        }
    }

    public void DealDamage(Health target, int amount)
    {
        if (target.currentHealth >= 0)
        {
            target.SufferDamage(amount);
        }
    }
}