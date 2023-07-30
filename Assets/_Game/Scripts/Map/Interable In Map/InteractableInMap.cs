using DG.Tweening;
using UnityBase.DesignPattern;
using UnityEngine;
using static NVTT.Utilities;

public class InteractableInMap : MonoBehaviour
{
    [HideInInspector] public DotweenScale interactableArea;
    
    [SerializeField] private bool deActiveWhenCombat;
    [SerializeField] private GameObject frameHighlight;
    
    protected bool InInteractableZone;
    private void Awake()
    {
        Init();
        InitObserver();
    }

    private void OnEnable()
    {
        if (deActiveWhenCombat && Gameplay.IsInGameplay) interactableArea.OnClose();
    }

    protected virtual void Init()
    {
        interactableArea = GetComponent<DotweenScale>();
    }

    protected virtual void InitObserver()
    {
        if (!deActiveWhenCombat) return;
        Observer.Instance.AddObserver(EventID.StartGameLevel, OnStartGameLevel);
        Observer.Instance.AddObserver(EventID.EndGameLevel, OnEndGameLevel);
    }

    private void OnStartGameLevel() => interactableArea.OnClose();
    
    private void OnEndGameLevel() => DOVirtual.DelayedCall(3f, () => interactableArea.gameObject.SetActive(true));
    
    protected virtual void InInteractableZoneResponse()
    {
        InInteractableZone = true;
        frameHighlight.SetActive(true);
        Observer.Instance.Notify(EventID.InUpgradeZone);
    }

    protected virtual void OutInteractableZoneResponse()
    {
        InInteractableZone = false;
        frameHighlight.SetActive(false);
        Observer.Instance.Notify(EventID.OutUpgradeZone);
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Player")) return;
        InInteractableZoneResponse();
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Player")) return;
        OutInteractableZoneResponse();
    }
}
