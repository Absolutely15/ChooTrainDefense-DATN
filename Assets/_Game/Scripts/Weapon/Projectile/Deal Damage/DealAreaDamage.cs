using UnityEngine;

public class DealAreaDamage : MonoBehaviour, IDealDamage
{
    public float radius;
    public bool drawDebugBox;
    
    private LayerMask enemies;
    private Collider[] hitColliders;
    private const int MAX_COLLIDERS = 20;

    private void OnDrawGizmos()
    {
        if (!drawDebugBox) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere (transform.position, radius);
    }
    private void Start()
    {
        hitColliders = new Collider[MAX_COLLIDERS];
        enemies = LayerMask.GetMask("Enemies");
    }
    
    //Target is where the explosion happened
    public void DealDamage(Health target, int amount)
    {
        var numColliders = Physics.OverlapSphereNonAlloc(target.transform.position, radius, hitColliders, enemies);
        if (numColliders == 0) return;
        for (var i = 0; i < numColliders; i++)
        {
            var targetInRange = hitColliders[i].GetComponent<Health>();
            if (targetInRange == null) return;
            if (targetInRange.currentHealth < 0) return;
            targetInRange.SufferDamage(amount);
        }
    }
    //Target is the game object that gonna take damage
    public void DealDamage(Health target, int amount, float invincibilityDuration)
    {
        if (target.currentHealth >= 0)
        {
            target.SufferDamage(amount);
        }
    }
}
