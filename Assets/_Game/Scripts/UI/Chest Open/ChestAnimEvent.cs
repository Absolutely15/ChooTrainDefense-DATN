using UnityEngine;
using UnityEngine.Events;

public class ChestAnimEvent : MonoBehaviour
{
    public UnityEvent onFinishAnimation = new UnityEvent();

    public void EventInvoke()
    {
        onFinishAnimation.Invoke();
    }
}
