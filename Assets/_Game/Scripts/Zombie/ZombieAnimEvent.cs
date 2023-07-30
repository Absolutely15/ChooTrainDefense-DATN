using UnityEngine;
using UnityEngine.Events;

public class ZombieAnimEvent : MonoBehaviour
{
    public UnityEvent onDoAttack = new UnityEvent();
    public void DoAttack()
    {
        onDoAttack.Invoke();    
    }
}
