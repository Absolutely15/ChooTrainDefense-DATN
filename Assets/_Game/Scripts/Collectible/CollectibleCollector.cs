using System.Collections.Generic;
using DG.Tweening;
using UnityBase.DesignPattern;
using UnityEngine;
using static NVTT.Utilities;

public class CollectibleCollector : Singleton<CollectibleCollector>
{
    public List<CollectibleMagnet> collectibleList;
    
    public static int CollectedGoldThisLevel;
    public static int CollectedDiamondThisLevel;
    
    [SerializeField] private float collectRange = 1.25f;
    [SerializeField] private float collectibleMoveSpeed = 3.5f;
    [SerializeField] private float collectibleEndLevelMS = 5f;
    
    private bool collectAll;

    private static bool IsInCollectRange(Transform target, Vector3 offset, float range)
    {
        return (Player.transform.position + offset - target.position).sqrMagnitude <= range * range;
    }

    private void Start()
    {
        Observer.Instance.AddObserver(EventID.StartGameLevel, OnStartGameLevel);
        Observer.Instance.AddObserver(EventID.EndGameLevel, OnEndGameLevel);
    }

    private void OnStartGameLevel()
    {
        collectAll = false;
        CollectedGoldThisLevel = 0;
        CollectedDiamondThisLevel = 0;
    }
    private void OnEndGameLevel()
    {
        DOVirtual.DelayedCall(2.5f, CollectAll);
    }

    private void CollectAll()
    {
        collectAll = true;
        foreach (var t in collectibleList)
        {
            t.moveSpeed = collectibleEndLevelMS;
            t.isMoving = true;
        }
    }
    
    private void Update()
    {
        if (collectibleList.Count == 0 || collectAll) return;
        
        foreach (var t in collectibleList)
        {
            if (!IsInCollectRange(t.transform, Vector3.zero, collectRange) || t.isMoving) continue;
            t.moveSpeed = collectibleMoveSpeed;
            t.isMoving = true;
        }
    }
}
