using static NVTT.Utilities;
using UnityEngine;

public class DroneController : MonoBehaviour
{
    public DroneWeapon droneWeapon;
    
    [SerializeField] private Transform droneTarget;
    [SerializeField] private float moveSpeed;
    
    private void DoMove()
    {
        transform.position = Vector3.Lerp(transform.position, droneTarget.position,  moveSpeed * Time.deltaTime);

        if (Player.direction == Vector3.zero) return;
        if (droneWeapon.WeaponTarget == null)
        {
            transform.LookAtDirection(Player.direction, 2f);
        }
    }
    
    private void Update()
    {
        DoMove();
    }
}
