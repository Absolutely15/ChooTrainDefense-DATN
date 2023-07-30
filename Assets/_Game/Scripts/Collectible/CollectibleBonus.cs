using NVTT;
using UnityBase.DesignPattern;
using static NVTT.Utilities;

public class CollectibleBonus : CollectibleDrop
{
    private void Start()
    {
        RandomSpreadRange = 0.3f;
        Observer.Instance.AddObserver(EventID.EndGameLevel, OnEndGameLevel);
    }

    private void OnEndGameLevel()
    {
        DiamondsDropAmount = PlayerSave.CurrentGameLevel < GameDB.currencyBonusData.diamondBonusEachLevel.Count ? GameDB.currencyBonusData.diamondBonusEachLevel[PlayerSave.CurrentGameLevel] : 3;
        DropDiamond(Utilities.MapManager.zombieSpawner.lastZombieKilled.transform.position);
    }
}
