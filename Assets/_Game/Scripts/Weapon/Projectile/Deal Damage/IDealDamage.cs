public interface IDealDamage
{
    void DealDamage(Health target, int amount);
    void DealDamage(Health target, int amount, float invincibilityDuration);
}
