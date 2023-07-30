using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Player Upgrade Data")]
public class PlayerUpgradeData : ScriptableObject
{
    [Title("Statistics")]
    public List<int> healthStat;
    public List<float> speedStat;
    public List<float> speedAnimStat;
    [Title("Cost")]
    public List<int> healthCost;
    public List<int> speedCost;
}
