using ATSoft;
using UnityBase.DesignPattern;
using UnityEngine;
using static NVTT.Utilities;

public class FirebaseLogEvent : MonoBehaviour
{
    private float timePlayedThisLevel;
    private void Start()
    {
        Init();
        InitObserver();
    }

    private void Update()
    {
        if (!Gameplay.IsInGameplay) return;
        timePlayedThisLevel += Time.deltaTime;
    }

    private static void Init()
    {
        foreach (var b in MapManager.Instance.blocker)
        {
            b.onGoldUnlockRoomChange.AddListener(t => AnalyticsManager.LogEventGoldSpend(1, "barrier", "unlock_room"));
        }
    }

    private void InitObserver()
    {
        Observer.Instance.AddObserver(EventID.StartGameLevel, OnStartGame);
        Observer.Instance.AddObserver(EventID.EndGameLevelData, OnEndGame);
        Observer.Instance.AddObserver(EventID.RebuildAllBarrier, LogRebuildBarrier);
        Observer.Instance.AddObserver(EventID.Dead, OnPlayerDead);
    }

    private static void OnStartGame()
    {
        AnalyticsManager.LogEventLevelStart(PlayerSave.CurrentGameLevel + 1);
    }

    private void OnEndGame()
    {
        AnalyticsManager.LogEventGoldEarn(CollectibleCollector.CollectedGoldThisLevel, "enemies");
        AnalyticsManager.LogEventDiamondEarn(CollectibleCollector.CollectedDiamondThisLevel, "enemies");

        AnalyticsManager.LogEventLevelPassed(PlayerSave.CurrentGameLevel + 1, (int)timePlayedThisLevel);
    }

    private void OnPlayerDead()
    {
        AnalyticsManager.LogEventLevelFailed(PlayerSave.CurrentGameLevel + 1, (int)timePlayedThisLevel);
    }

    private static void LogRebuildBarrier()
    {
        AnalyticsManager.LogEventDiamondSpend(GameDB.barrierData.rebuildAllDiamondCost[PlayerSave.BarrierLevel],
            "rebuild", "all_barrier");
    }
}
