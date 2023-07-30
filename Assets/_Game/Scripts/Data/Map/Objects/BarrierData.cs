using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Barrier Data")]
public class BarrierData : MapDefenseObjectData
{
    public List<int> healthUpgradeStat;
    public List<int> rebuildAllDiamondCost;
}
