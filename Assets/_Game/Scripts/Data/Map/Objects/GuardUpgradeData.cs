using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Guard Upgrade Data")]
public class GuardUpgradeData : MapDefenseObjectData
{
    public List<Material> upgradeMaterials;
}
