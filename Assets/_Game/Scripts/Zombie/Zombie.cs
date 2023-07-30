using System.Collections.Generic;
using NVTT;
using Sirenix.OdinInspector;
using UnityBase.DesignPattern;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class Zombie : MonoBehaviour
{
    #region Properties
    [SerializeField] protected ZombieState zombieState;
    [Title("Ref")]
    [SerializeField] protected ZombieData zombieData;
    [SerializeField] protected NavMeshAgent navMeshAgent;
    [SerializeField] protected Animator anim;
    [SerializeField] protected ZombieAnimEvent animEvent;
    [SerializeField] protected Health health;

    public Barrier ClosestBarrier { private get; set; }
    protected IDealDamage DealDamage;
    protected int MapID;

    private Dictionary<ZombieState, UnityAction> stateActionDict;

    private float atkDoorCoolDown;
    private float lastAttack;

    private int damageDealtToDoor;
    private int damageDealtToPlayer;

    #region Anim Properties
    private static readonly int Attack = Animator.StringToHash("Attack");
    private static readonly int Move = Animator.StringToHash("Move");
    private static readonly int PlayerDeath = Animator.StringToHash("PlayerDeath");
    private static readonly int PlayerRevive = Animator.StringToHash("PlayerRevive");
    private static readonly int Dead = Animator.StringToHash("Dead");
    #endregion

    #endregion

    #region Unity Functions
    protected void OnValidate()
    {
        if (anim == null)
            anim = GetComponentInChildren<Animator>();
        if (animEvent == null)
            animEvent = GetComponentInChildren<ZombieAnimEvent>();
    }

    protected void Awake()
    {
        Init();
        InitData();
        InitEvent();
    }
    
    protected void OnEnable()
    {
        InitState();
    }

    protected void Update()
    {
        stateActionDict[zombieState].Invoke();
    }
    #endregion
    
    #region Init
    protected virtual void InitData()
    {
        MapID = Utilities.MapManager.id;
        //Data
        health.maximumHealth = zombieData.health[MapID];
        navMeshAgent.speed = zombieData.moveSpeed[MapID];
        damageDealtToDoor = zombieData.damageDealtToDoor[MapID];
        damageDealtToPlayer = zombieData.damageDealtToPlayer;
        atkDoorCoolDown = zombieData.atkDoorCD[MapID];
    }
    
    private void Init()
    {
        DealDamage = GetComponent<IDealDamage>();
        //State Action
        stateActionDict = new Dictionary<ZombieState, UnityAction>
        {
            {ZombieState.Smashing, StateSmashing},
            {ZombieState.Seeking, StateSeeking},
            {ZombieState.Die, StateDie},
            {ZombieState.PlayerDeath, StatePlayerDeath}
        };
    }

    private void InitEvent()
    {
        Observer.Instance.AddObserver(EventID.Dead, ChangeStatePlayerDead);
        Observer.Instance.AddObserver(EventID.Revive, ChangeStatePlayerRevive);
        
        animEvent.onDoAttack.AddListener(AttackDoor);
        health.onDeath.AddListener(ChangeStateDie);
    }

    private void InitState()
    {
        gameObject.layer = LayerMask.NameToLayer("Enemies");
        zombieState = ZombieState.Smashing;

        //closest barrier
        if (ClosestBarrier == null) return;
        navMeshAgent.SetDestination(ClosestBarrier.transform.position);
    }
    #endregion
    
    #region State Action
    private void StateSmashing()
    {
        if (ClosestBarrier.model.activeInHierarchy)
        {
            if (!IsTargetInRange(ClosestBarrier.transform, zombieData.attackRange) ||
                !(Time.time > lastAttack + atkDoorCoolDown)) return;
            lastAttack = Time.time;
            anim.SetTrigger(Attack);
        }
        else
        {
            ChangeStateSeeking();
        }
    }
    private void StateSeeking()
    {
        navMeshAgent.SetDestination(Utilities.Player.transform.position);
        if (!IsTargetInRange(Utilities.Player.transform, zombieData.attackRange) ||
            Utilities.Player.health.currentHealth <= 0) return;
        AttackPlayer();
    }
    private static void StateDie() { }
    private static void StatePlayerDeath() { }
    #endregion

    #region State Switch
    private void ChangeStateSeeking()
    {
        if (!gameObject.activeInHierarchy) return;
        zombieState = ZombieState.Seeking;
        anim.SetTrigger(Move);
    }
    protected virtual void ChangeStateDie(Health h)
    {
        if (!gameObject.activeInHierarchy) return;
        zombieState = ZombieState.Die;
        anim.SetTrigger(Dead);
        navMeshAgent.isStopped = true;
        gameObject.layer = LayerMask.NameToLayer("Default");
        Observer.Instance.Notify(EventID.ZombieDead, this);
    }

    private void ChangeStatePlayerDead()
    {
        if (zombieState == ZombieState.Die || !gameObject.activeInHierarchy) return;
        zombieState = ZombieState.PlayerDeath;
        anim.SetTrigger(PlayerDeath);
        navMeshAgent.isStopped = true;
    }

    private void ChangeStatePlayerRevive()
    {
        if (zombieState == ZombieState.Die || !gameObject.activeInHierarchy) return;
        zombieState = ZombieState.Smashing;
        anim.SetTrigger(PlayerRevive);
        navMeshAgent.isStopped = false;
        health.SetDeath();
    }
    #endregion

    #region Attack
    protected bool IsTargetInRange(Transform target, float range) => (transform.position - target.position).sqrMagnitude <= range * range;
    private void AttackDoor() => DealDamage.DealDamage(ClosestBarrier.health, damageDealtToDoor);
    protected void AttackPlayer() => DealDamage.DealDamage(Utilities.Player.health, damageDealtToPlayer, Utilities.Player.health.invincibilityDur);
    #endregion
}