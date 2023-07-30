using DG.Tweening;
using Sirenix.OdinInspector;
using UnityBase.DesignPattern;
using UnityEngine;
using UnityEngine.Events;
using NVTT;

public class Trap : MonoBehaviour
{
    #region Properties
    [Title("State")]
    public bool isUnlocked;
    public int id;

    [Title("Reference")]
    [SerializeField] private TrapData trapData;
    [SerializeField] private GameObject model;
    [SerializeField] private GameObject spike;
    [SerializeField] private Collider trapCollider;
    [SerializeField] private TrapUI trapUI;
    [SerializeField] private TrapUnlockedUI trapUnlockedUI;

    [Title("Event")]
    public UnityEvent<Trap> updateData = new UnityEvent<Trap>();
    
    private IDealDamage dealAreaDamage;
    private int damage;
    private bool isCharging;
    private bool isDoingTween;
    #endregion

    #region Query
    public int UnlockCost => trapData.upgradeCost[0];
    private void GetData() => damage = trapData.damageUpgradeStat[PlayerSave.TrapLevel];
    #endregion

    #region Unity Functions

    private void Start()
    {
        Init();
        GetData();
    }
    #endregion

    #region Init

    private void Init()
    {
        Observer.Instance.AddObserver(EventID.UpgradeTrap, UpgradeTrap);
        dealAreaDamage = GetComponent<IDealDamage>();
        trapUI.Init(isUnlocked);
        model.SetActive(isUnlocked);
        trapCollider.enabled = isUnlocked;
    }
    private void UpgradeTrap()
    {
        Observer.Instance.Notify(EventID.UpgradeDefense);
        GetData();
        if (isUnlocked) transform.UpgradeFeedback();
    }

    public void UnlockTrap()
    {
        isUnlocked = true;
        model.SetActive(true);
        trapCollider.enabled = true;
        updateData.Invoke(this);
        trapUI.Init(true);
    }
    #endregion

    #region Spike Function

    private void SpikeDotween()
    {
        isDoingTween = true;
        
        var spikeSeq = DOTween.Sequence();
        spikeSeq.Append(spike.transform.DOLocalMoveY(0.1f, 0.25f))
            .AppendInterval(4.5f)
            .Append(spike.transform.DOLocalMoveY(-0.04f, 0.25f))
            .AppendCallback(RechargeSpike);
    }

    private void RechargeSpike()
    {
        isCharging = true;
        trapUnlockedUI.SpikeDotween(() =>
        {
            isDoingTween = false;
            isCharging = false;
        });
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isCharging) return;
        
        var enemies = other.GetComponent<Health>();
        if (enemies == null) return;
        
        dealAreaDamage.DealDamage(enemies, damage);
        
        if (isDoingTween) return;
        SpikeDotween();
    }
    #endregion
}
