using System.Collections.Generic;
using UnityEngine;
using NVTT;
using Sirenix.OdinInspector;

public class WeaponAutoAim : MonoBehaviour
{
    #region Properties
    [HideInInspector] public Transform target;
    [Title("Layer Masks")] 
    [SerializeField] private LayerMask targetMask;
    [SerializeField] private LayerMask obstacleMask;
    
    [Title("Scan for Targets")]
    [SerializeField] private float scanRadius = 15;
    [SerializeField] private float durationBetweenScans = 1f;

    [Title("3D")]
    [SerializeField] private bool drawDebugRadius = true;
    [SerializeField] private int overlapMaximum = 10;
    
    private Vector3 aimDirection;
    private Vector3 raycastDirection;
    private Collider[] hits;
    private List<Transform> potentialTargets;
    private Transform ownerTF;
    private float lastScanTimestamp;
    #endregion
    
    #region Unity Functions
    private void Update()
    {
        ScanIfNeeded();
        SetCurrentAim();
    }
    private void OnDrawGizmos()
    {
        if (!drawDebugRadius || ownerTF == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(ownerTF.position, scanRadius);
    }
    #endregion

    #region Basic Feature
    public void Init(Transform owner, WeaponLocalData weaponLocalData, bool ignoreObstacle = false)
    {
        targetMask = LayerMask.GetMask("Enemies");
        obstacleMask =  ignoreObstacle ? 0 : LayerMask.GetMask("Obstacles");
        scanRadius = weaponLocalData.fireRange;
        potentialTargets = new List<Transform>();
        hits = new Collider[10];
        
        ownerTF = owner;
    }
    
    private void SetCurrentAim()
    {
        if (target == null) return;
        aimDirection = target.transform.position - ownerTF.position;
        ownerTF.LookAtDirection(aimDirection, 10f, true);
    }
    #endregion
    
    #region Scan Feature
    private void ScanIfNeeded()
    {
        if (!(Time.time - lastScanTimestamp > durationBetweenScans)) return;
        ScanForTargets();
        lastScanTimestamp = Time.time;
    }

    private bool ScanForTargets()
    {
        target = null;
        var numberOfHits = Physics.OverlapSphereNonAlloc(ownerTF.position, scanRadius, hits, targetMask);
        if (numberOfHits == 0)
        {
            return false;
        }
        potentialTargets.Clear();

        var min = Mathf.Min(overlapMaximum, numberOfHits);
        for (var i = 0; i < min; i++)
        {
            if (hits[i] == null)
                continue;
            if (hits[i].gameObject == gameObject || hits[i].transform.IsChildOf(transform))
            {
                continue;
            }
            potentialTargets.Add(hits[i].gameObject.transform);
        }
        potentialTargets.Sort((a, b) => Vector3.Distance(ownerTF.position, a.transform.position)
            .CompareTo(Vector3.Distance(ownerTF.position, b.transform.position)));
        
        foreach (var t in potentialTargets)
        {
            raycastDirection = t.position - ownerTF.position;
            var hit = Raycast3D(ownerTF.position, raycastDirection, raycastDirection.magnitude, obstacleMask,
                Color.red, true);
            if (hit.collider != null) continue;
            target = t;
            return true;
        }

        return false;
    }
    private static RaycastHit Raycast3D(Vector3 rayOriginPoint, Vector3 rayDirection, float rayDistance, LayerMask mask, Color color,bool drawGizmo=false, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
    {
        if (drawGizmo)
        {
            Debug.DrawRay(rayOriginPoint, rayDirection * rayDistance, color);
        }

        Physics.Raycast(rayOriginPoint, rayDirection, out var hit, rayDistance, mask, queryTriggerInteraction);
        return hit;
    }
    #endregion
}
