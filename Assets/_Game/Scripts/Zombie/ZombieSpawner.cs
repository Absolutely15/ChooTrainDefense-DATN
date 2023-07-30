using DG.Tweening;
using NVTT;
using Sirenix.OdinInspector;
using UnityBase.DesignPattern;
using UnityEngine;
using static NVTT.Utilities;

public class ZombieSpawner : MonoBehaviour
{
    #region Properties
    public float interval;
    
    [HideInInspector] public Zombie lastZombieKilled;
    [HideInInspector] public int totalZombieInWave;

    private Barrier spawnBox;

    private bool isPlayerRevived;
    private int waveNumber;
    private int zombieSpawnedCount;
    private int zombieActiveCount;
    #endregion

    #region Unity Functions
    private void Start()
    {
        InitObserver();
    }
    private void Update()
    {
        if (!Gameplay.IsInGameplay)
            return;
        
        if (!isPlayerRevived && zombieSpawnedCount != totalZombieInWave)
            return;

        if (zombieActiveCount != 0)
            return;
        Observer.Instance.Notify(EventID.EndGameLevel);
    }
    #endregion

    #region Init
    private void InitObserver()
    {
        Observer.Instance.AddObserver(EventID.Dead, OnPlayerDeath);
        Observer.Instance.AddObserver(EventID.Revive, OnPlayerRevive);
        Observer.Instance.AddObserver(EventID.ZombieDead, z => OnZombieDead((Zombie)z[0]));
    }
    private void OnPlayerDeath()
    {
        StopSpawn();
    }

    private void OnPlayerRevive()
    {
        isPlayerRevived = true;
    }
    #endregion
    
    #region Query
    private void TotalZombieTypeInWave()
    {
        totalZombieInWave = 0;
        foreach (var i in GameDB.mapData.zombieSpawnAmount[waveNumber].zombieType)
            totalZombieInWave += i;
    }

    private void GetWaveNumber() =>
        waveNumber = PlayerSave.CurrentGameLevel < GameDB.mapData.zombieSpawnAmount.Count
            ? PlayerSave.CurrentGameLevel
            : Random.Range(GameDB.mapData.zombieSpawnAmount.Count - 5, GameDB.mapData.zombieSpawnAmount.Count);

    private void OnZombieDead(Zombie zombie)
    {
        lastZombieKilled = zombie;
        zombieActiveCount--;
    }

    #endregion
    
    #region Spawn
    public void StartSpawn()
    {
        //Tutorial
        if (PlayerSave.CurrentGameLevel == 1)
            interval = 0.5f;
        
        GetWaveNumber();
        TotalZombieTypeInWave();

        zombieSpawnedCount = 0;
        zombieActiveCount = 0;
        isPlayerRevived = false;

        for (var i = 0; i < GameDB.mapData.zombieSpawnAmount[waveNumber].zombieType.Count; i++)
        {
            DelaySpawn(i);
        }
    }

    [Button]
    public void StopSpawn()
    {
        DOTween.Kill("SpawnZombie");
    }

    private void DelaySpawn(int id)
    {
        for (var i = 0; i < GameDB.mapData.zombieSpawnAmount[waveNumber].zombieType[id]; i++)
        {
            SpawnZombie(id, interval * i);
        }
    }

    private void SpawnZombie(int id, float delay)
    {
        DOVirtual.DelayedCall(delay, () =>
        {
            spawnBox = Utilities.MapManager.barrierListSpawnable.Rand(0);

            var zombie = ZombieObjectPooler.Instance.GetPooledObject(id);
            zombie.ClosestBarrier = spawnBox;
            zombie.transform.SetPositionAndRotation(spawnBox.GetRandomPositionInsideBox(), Quaternion.LookRotation(-spawnBox.transform.forward));
            zombie.gameObject.SetActive(true);
            zombieSpawnedCount++;
            zombieActiveCount++;
        }).SetId("SpawnZombie");
    }
    #endregion
}


