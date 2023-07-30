using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AVT;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityBase.DesignPattern;
using UnityEngine;
using UnityEngine.Events;
using static NVTT.Utilities;

public class Blocker : MonoBehaviour
{
    #region Properties
    [Title("Edit In Hierarchy")]
    public int ID;
    public int nextRoomID;
    public int goldRequired;
    public GameObject nextRoom;
    public List<Barrier> nextRoomBarrier;
    [HideInInspector] public int goldReceived;

    [Title("UI")]
    public BlockerUI blockerUI;

    [Title("Event")]
    public UnityEvent<Blocker> onUnlockedRoom = new UnityEvent<Blocker>();
    public UnityEvent<Blocker> onGoldUnlockRoomChange = new UnityEvent<Blocker>();
    public UnityEvent inTriggerZone = new UnityEvent();
    public UnityEvent outTriggerZone = new UnityEvent();
    private int GoldRemain => goldRequired - goldReceived;
    private bool isPlayerEntered;
    #endregion
    
    #region Init
    private void Start()
    {
       blockerUI.Init(goldReceived, goldRequired, GoldRemain);
    }
    
    private void OnValidate()
    {
        if (nextRoomBarrier.Count != 0 || nextRoom == null) return;
        nextRoomBarrier = nextRoom.GetComponentsInChildren<Barrier>().ToList();
        nextRoomID = int.Parse(nextRoom.gameObject.name.Substring(4));
    }
    #endregion

    #region Spawn Gold
    private IEnumerator DelayGoldSpawn()
    {
        while (isPlayerEntered && GoldRemain > 0)
        {
            if (!Gameplay.IsInGameplay && PlayerSave.Gold == 0 && goldReceived >= goldRequired / 2)
                Observer.Instance.Notify(EventID.PopUpBonusGold);
            
            if (PlayerSave.Gold == 0 || GoldRemain == 0) yield break;
            goldReceived++;
            
            //Database
            onGoldUnlockRoomChange.Invoke(this);

            SpawnCoin();
            yield return GetWfs(0.005f);
        }
    }
    
    private void SpawnCoin()
    {
        var gold = ObjectPooler.Instance.GetPooledObject(ObjectPoolType.Gold);
        gold.transform.position = Player.transform.position + Vector3.up * 0.5f;
        gold.SetActive(true);

        var deltaPos = Random.insideUnitSphere * 0.1f;
        deltaPos.y = 0;
        
        var goldFlySeq = DOTween.Sequence();
        goldFlySeq.Append(gold.transform.DoParabolaMotion(gold.transform.position + deltaPos, 0.1f, 0.2f))
            .Append(gold.transform.DOMove(blockerUI.goldFlyDes.position, 0.1f))
            .AppendCallback(() =>
            {
                gold.SetActive(false);
                blockerUI.UIUpdate(() => onUnlockedRoom.Invoke(this));
            });
    }
    #endregion

    #region On Trigger
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Player")) return;
        isPlayerEntered = true;

        if (goldRequired >= 100)
        {
            for (var i = 0; i < 5; i++)
            {
                StartCoroutine(DelayGoldSpawn());
            }
        }
        else
        {
            StartCoroutine(DelayGoldSpawn());
        }

        if (PlayerSave.Gold == 0) return;
        inTriggerZone.Invoke();
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Player")) return;
        isPlayerEntered = false;
        DOVirtual.DelayedCall(0.5f, outTriggerZone.Invoke);
    }
    #endregion
}
