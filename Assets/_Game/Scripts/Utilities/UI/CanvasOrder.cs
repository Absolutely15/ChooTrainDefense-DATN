using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class CanvasOrder : MonoBehaviour
{
    public List<Canvas> canvases;

    [Button]
    public void Sort()
    {
        for (var i = 0; i < canvases.Count; i++)
        {
            canvases[i].sortingOrder = i;
        }
    }
}
