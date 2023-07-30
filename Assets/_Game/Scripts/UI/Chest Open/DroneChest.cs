using UnityEngine;
using static NVTT.Utilities;

public class DroneChest : MonoBehaviour, IChest
{
    public ChestType ChestType { get; set; }

    public void Init()
    {
        ChestType = ChestType.DroneChest;
    }

    public void Open()
    {
        PlayerSave.IsDroneUnlocked = true;
    }

    public void Claim()
    {
        Gameplay.DroneUnlockedCheck();
    }
}
