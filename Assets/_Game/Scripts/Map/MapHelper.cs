using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class MapHelper : MonoBehaviour
{
    [SerializeField] private List<Objects> objectsList;

    [Button]
    public void GetAllRedundantObject()
    {
        objectsList = gameObject.GetComponentsInChildren<Objects>().ToList();
    }

    [Button]
    public void SetActiveTrueAll(bool isActive)
    {
        foreach (var go in objectsList)
        {
            go.gameObject.SetActive(isActive);
        }
    }
}
