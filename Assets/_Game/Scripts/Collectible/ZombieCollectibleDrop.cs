using UnityEngine;
using NVTT;

public class ZombieCollectibleDrop : CollectibleDrop
{
    [SerializeField] private ZombieData zombieData;
    private void Start()
    {
        GoldsDropAmount = zombieData.goldDropPerUnit[Utilities.MapManager.id];
        DiamondsDropAmount = zombieData.diamondDropPerUnit;
        RandomSpreadRange = zombieData.randomSpreadRange;
    }
}
