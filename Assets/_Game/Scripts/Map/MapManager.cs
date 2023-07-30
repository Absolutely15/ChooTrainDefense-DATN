using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Sirenix.OdinInspector;
using Unity.AI.Navigation;
using UnityBase.DesignPattern;
using UnityEngine;

[DefaultExecutionOrder(-1)]
public class MapManager : Singleton<MapManager>
{
    #region Properties
    [Title("Objects")] 
    public List<Barrier> barrierListSpawn;
    public List<Barrier> barrierListSpawnable;
    public List<Health> barrierListEvent;
    public List<GameObject> room;
    public List<Blocker> blocker;
    public List<GuardController> guard;
    public List<Trap> trap;
    public List<Chest> chest;
    public MapSaveData mapSaveData;
    [Title("Refs")]
    public int id;
    public MapObjectLevelLimitation mapLimitation;
    public ZombieSpawner zombieSpawner;
    
    [SerializeField] private NavMeshSurface navMesh;
    [SerializeField] private AnimationCurve blockerAnimCurve;
    [SerializeField] private AnimationCurve roomAnimCurve;
    
    private WaitForEndOfFrame waitForEndOfFrame;
    #endregion

    #region Unity Functions
    private void OnValidate()
    {
        if (navMesh == null)
            navMesh = GetComponent<NavMeshSurface>();
        if (zombieSpawner == null)
            zombieSpawner = GetComponent<ZombieSpawner>();
    }

    private void Awake()
    {
        InitParam();
        InitEvent();
        LoadMapData();
    }

    private void Start()
    {
        GetSpawnableBarrier();
    }
    #endregion

    #region Init
    private void InitParam()
    {
        waitForEndOfFrame = new WaitForEndOfFrame();
    }
    private void InitEvent()
    {
        //Event RebuildNavMesh
        foreach (var barrierHealth in barrierListEvent)
        {
            barrierHealth.onDeath.AddListener(RebuildNavMesh);
        }

        //Event unlock room
        foreach (var b in blocker)
        {
            b.onGoldUnlockRoomChange.AddListener( bl => UpdateBlockerGoldChange(bl.ID));
            b.onUnlockedRoom.AddListener(bl => UnlockRoomSuccess(bl.ID));
        }
        //Event unlock & upgrade guard
        for (var i = 0; i < guard.Count; i++)
        {
            var i1 = i;
            guard[i].updateData.AddListener(g => UpgradeGuard(i1));
        }
        
        //Event unlock trap
        for (var i = 0; i < trap.Count; i++)
        {
            var i1 = i;
            trap[i].updateData.AddListener(t => UnlockTrapSuccess(i1));
        }
        
        //Event open chest
        for (var i = 0; i < chest.Count; i++)
        {
            var i1 = i;
            chest[i].onOpen.AddListener(c => OpenChestSuccess(i1));
        }
        
        //Observer rebuild barrier
        Observer.Instance.AddObserver(EventID.RebuildBarrier, RebuildNavMesh);
        Observer.Instance.AddObserver(EventID.RebuildAllBarrier, RebuildNavMesh);
    }
    
    private void LoadMapData()
    {
        //Load data
        mapSaveData = PlayerSave.DeserializeMapSaveData();
        var r = mapSaveData.roomSaveDatas.Count;
        var b = mapSaveData.blockerGoldReceived.Count;
        var g = mapSaveData.guardSaveDatas.Count;
        var t = mapSaveData.trapSaveDatas.Count;
        var c = mapSaveData.chestSaveDatas.Count;

        // Room
        for (var i = 0; i < r; i++)
        {
            room[i].SetActive(mapSaveData.roomSaveDatas[i].isUnlocked);
        }
        
        //Barrier
        for (var i = 0; i < r - 1; i++)
        {
            if (!blocker[i].nextRoom.activeInHierarchy) continue;

            blocker[i].gameObject.SetActive(false);

            AddBarrierSpawn(i);
        }
        
        for (var i = 0; i < b; i++)
        {
            if (blocker[i].gameObject.activeInHierarchy)
                blocker[i].goldReceived = mapSaveData.blockerGoldReceived[i];
        }

        //Trap
        for (var i = 0; i < t; i++)
        {
            trap[i].isUnlocked = mapSaveData.trapSaveDatas[i].isUnlocked;
        }
        
        //Chest
        for (var i = 0; i < c; i++)
        {
            chest[i].isOpened = mapSaveData.chestSaveDatas[i].isOpened;
        }
        
        //Guard
        for (var i = 0; i < g; i++)
        {
            if (mapSaveData.guardSaveDatas[i].level <= 0) continue;
            guard[i].level = mapSaveData.guardSaveDatas[i].level;
        }
        
        RebuildNavMesh();
    }
    #endregion

    #region Debug
    [Button]
    public void GetGoInHierarchy()
    {
        barrierListEvent = GetComponentsInChildren<Health>().ToList();
        blocker = GetComponentsInChildren<Blocker>().ToList();
        guard = GetComponentsInChildren<GuardController>().ToList();
        trap = GetComponentsInChildren<Trap>().ToList();
        chest = GetComponentsInChildren<Chest>().ToList();
    }

    [Button]
    public void SerializeMapSaveData()
    {
        PlayerSave.SerializeMapSaveData(mapSaveData);
    }
    #endregion

    #region NavMesh
    public void RebuildNavMesh(object data = null)
    {
        StartCoroutine(RebuildNavMeshCoroutine());
    }
    private IEnumerator RebuildNavMeshCoroutine()
    {
        yield return waitForEndOfFrame;
        navMesh.BuildNavMesh();
    }
    #endregion

    #region Unlock Room
    private void UpdateBlockerGoldChange(int blockerIndex)
    {
        mapSaveData.blockerGoldReceived[blockerIndex]++;
        PlayerSave.Gold--;
        SerializeMapSaveData();
    }
    
    private void UnlockRoomSuccess(int index)
    {
        //Save data
        mapSaveData.roomSaveDatas[blocker[index].nextRoomID].isUnlocked = true;
        SerializeMapSaveData();
        
        blocker[index].nextRoom.transform.localScale = Vector3.zero;
        blocker[index].nextRoom.SetActive(true);

        var unlockNewRoom = DOTween.Sequence();
        unlockNewRoom.Append(blocker[index].transform.DOScale(0, 0.25f).SetEase(blockerAnimCurve))
            .Append(blocker[index].nextRoom.transform.DOScale(1, 0.15f).SetEase(roomAnimCurve))
            .AppendCallback(() =>
            {
                AddBarrierSpawn(index);
                GetSpawnableBarrier();
                blocker[index].gameObject.SetActive(false);

                RebuildNavMesh();
            });
    }
    #endregion
    
    #region Barrier
    private void AddBarrierSpawn(int j)
    {
        if (blocker[j].nextRoomBarrier.Count == 0) return;
        foreach (var b in blocker[j].nextRoomBarrier)
        {
            barrierListSpawn.Add(b);
        }
    }

    private void GetSpawnableBarrier()
    {
        barrierListSpawnable.Clear();
        foreach (var b in barrierListSpawn)
        {
            if (b.gameObject.activeInHierarchy)
                barrierListSpawnable.Add(b);
        }
    }
    #endregion
    
    #region Unlock & Upgrade Guard
    private void UpgradeGuard(int j)
    {
        mapSaveData.guardSaveDatas[guard[j].id].level = guard[j].level;
        SerializeMapSaveData();
    }
    #endregion

    #region Unlock Trap
    private void UnlockTrapSuccess(int j)
    {
        mapSaveData.trapSaveDatas[trap[j].id].isUnlocked = true;
        SerializeMapSaveData();
    }
    #endregion

    #region Open Chest
    private void OpenChestSuccess(int j)
    {
        mapSaveData.chestSaveDatas[chest[j].id].isOpened = true;
        SerializeMapSaveData();
    }
    #endregion
}