using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityBase.DesignPattern;
using UnityEngine;

public class Test : MonoBehaviour
{
    public EventID eventID;

    [Button]
    public void Notify()
    {
        Observer.Instance.Notify(eventID);
    }
}
