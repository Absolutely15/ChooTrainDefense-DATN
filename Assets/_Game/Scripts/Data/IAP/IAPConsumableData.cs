using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/IAP Consumable Data")]
public class IAPConsumableData : ScriptableObject
{
    public List<int> diamondsPackData = new List<int>();
}
