using DG.Tweening;
using UnityEngine;
using NVTT;
using static NVTT.Utilities;
using UnityBase.DesignPattern;
using UnityEngine.Events;

public class Teleport : MonoBehaviour
{
    #region Properties
    public BoxCollider boxCollider;
    [SerializeField] private Transform gate;
    [SerializeField] private Transform movePos;
    [SerializeField] private UITeleport uiTeleport;
    [SerializeField] private Teleport otherTeleport;

    private bool isGateUp;
    #endregion

    #region Unity Functions
    private void Start()
    {
        Observer.Instance.AddObserver(EventID.EndGameLevel, OnEndGameLevel);
    }

    private void OnEnable()
    {
        OnGameObjectActive();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Player")) return;
        TeleportSequence();
    }
    #endregion

    #region Basic Functions
    private Tween GateUp(float duration = 0f, UnityAction action = null)
    {
        isGateUp = true;
        return gate.transform.DOLocalMoveY(0.3f, duration).SetUpdate(true).OnComplete(() => action?.Invoke());
    }

    private Tween GateDown(float duration = 0f, UnityAction action = null)
    {
        isGateUp = false;
        return gate.transform.DOLocalMoveY(-0.25f, duration).SetUpdate(true).OnComplete(() => action?.Invoke());
    }
    
    private void OnGameObjectActive()
    {
        if (!otherTeleport.gameObject.activeInHierarchy) return;
        GateDown(0.5f, () => Utilities.MapManager.RebuildNavMesh());
        otherTeleport.GateDown(0.5f);
    }

    private void OnEndGameLevel()
    {
        if (!isGateUp || !otherTeleport.gameObject.activeInHierarchy) return;
        GateDown(0.5f, () => Utilities.MapManager.RebuildNavMesh());
    }
    #endregion

    #region Core Sequence
    private void TeleportSequence()
    {
        var teleportSeq = DOTween.Sequence().SetUpdate(true);
        teleportSeq.AppendCallback(EnterTeleport)
            .AppendInterval(1f)
            .AppendCallback(InsideTeleport);
    }

    private void EnterTeleport()
    {
        Observer.Instance.Notify(EventID.EnterTeleport);
        GPExecutor.Instance.PauseGame();
        
        GateUp(0.5f);
        otherTeleport.GateUp();
        
        uiTeleport.gameObject.SetActive(true);
        otherTeleport.boxCollider.enabled = false;
    }

    private void InsideTeleport()
    {
        uiTeleport.FadeOut(1f);
        
        Player.transform.localEulerAngles = Vector3.up * 180f;
        var insideTeleportSeq = DOTween.Sequence().SetUpdate(true);
        insideTeleportSeq.Append(Player.transform.DOMove(otherTeleport.transform.position, 0f).SetUpdate(true))
            .Append(otherTeleport.GateDown(0.5f))
            .AppendCallback(() => Player.animController.ExitTeleportAnim(true))
            .Append(Player.transform.DOMove(otherTeleport.movePos.position, 0.5f).SetUpdate(true))
            .AppendCallback(ExitTeleport);
    }

    private void ExitTeleport()
    {
        Observer.Instance.Notify(EventID.ExitTeleport);
        GPExecutor.Instance.ResumeGame();
        
        otherTeleport.boxCollider.enabled = true;

        if (Gameplay.IsInGameplay)
        {
            otherTeleport.GateUp(0.5f);
            GateUp(0.5f, () => Utilities.MapManager.RebuildNavMesh());
        }
        else
        {
            GateDown(0.5f);
        }
    }
    #endregion
}
