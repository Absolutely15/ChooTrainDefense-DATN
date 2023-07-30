using static NVTT.Utilities;
using UnityEngine;

public class DealDamageKnockBack : MonoBehaviour, IDealDamage
{
    public float forceMagnitude;
    public void DealDamage(Health target, int amount, float invincibilityDuration)
    {
        throw new System.NotImplementedException();
    }

    public void DealDamage(Health target, int amount)
    {
        if (target.currentHealth < 0) return;
        target.SufferDamage(amount);
        var targetTf = target.transform;
        var position = targetTf.position;
        var direction = Player.transform.position - position;
        position = Vector3.MoveTowards(position,direction * -forceMagnitude, 5f * Time.deltaTime);
        targetTf.position = position;
    }
}
