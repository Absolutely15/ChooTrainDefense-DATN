using System.Collections.Generic;
using UnityEngine;

public class RoomObject : MonoBehaviour
{
    [SerializeField] private GameObject refGameObject;
    [SerializeField] private List<GameObject> popUpObjects;
    [SerializeField] private List<GameObject> hiddenObjects;
    private void OnEnable()
    {
        if (!refGameObject.activeInHierarchy) return;
        CheckGOStatus();
        CheckBlockerStatus();
    }

    private static void SetGameObjectActive(List<GameObject> listGO, bool active)
    {
        if (listGO.Count == 0) return;
        foreach (var go in listGO)
        {
            go.gameObject.SetActive(active);
        }
    }

    private void CheckBlockerStatus()
    {
        foreach (var go in popUpObjects)
        {
            if (!go.gameObject.TryGetComponent(out Blocker blocker)) continue;
            if (!blocker.nextRoom.activeInHierarchy) continue;
            go.gameObject.SetActive(false);
        }
    }

    public void CheckGOStatus()
    {
        SetGameObjectActive(popUpObjects, true);
        SetGameObjectActive(hiddenObjects, false);
    }
}
