using UnityEngine;
using Sirenix.OdinInspector;
using UnityBase.DesignPattern;
using UnityEngine.AI;
using UnityEngine.Events;
using static NVTT.Utilities;

public class PlayerController : MonoBehaviour
{
    #region Properties
    [Title("Ref")]
    public PlayerWeapon weaponController;
    public PlayerSkin skinController;
    public PlayerAnim animController;
    public Health health;
    public NavMeshAgent navMeshAgent;
    
    [Title("Main Stats")]
    public float moveSpeed;
    //public float bonusSpeed = 1f;
    [HideInInspector] public Vector3 direction;
    [HideInInspector] public UnityEvent isMoving = new UnityEvent();
    public Weapon CurrentWeapon => weaponController.CurrentWeapon;
    public static VariableJoystick VariableJoystick => Gameplay.variableJoystick;
    
    #endregion
    
    #region Unity Functions
    private void Awake()
    {
        Init();
        InitEvent();
    }
    
    private void Update()
    {
        DoMove();
    }
    #endregion
    
    #region Init
    private void Init()
    {
        GetBasicStat();
    }

    private void InitEvent()
    {
        //Core
        health.onDeath.AddListener(DeathAction);
        Observer.Instance.AddObserver(EventID.UpgradeStat, GetBasicStat);
        Observer.Instance.AddObserver(EventID.ChangeSkin, GetBasicStat);
        Observer.Instance.AddObserver(EventID.EndGameLevel, OnEndGameStat);
        Observer.Instance.AddObserver(EventID.Revive, Revive);
        Observer.Instance.AddObserver(EventID.EnterTeleport, EnterTeleport);
        Observer.Instance.AddObserver(EventID.ExitTeleport, ExitTeleport);
        //Ads
        //Observer.Instance.AddObserver(EventID.RestoreSingleHealth, RestoreSingleHealth);
        Observer.Instance.AddObserver(EventID.PopUpBonusGold, DisableControl);
        Observer.Instance.AddObserver(EventID.DismissBonusCurrency, EnableControl);
        //Observer.Instance.AddObserver(EventID.BonusSpeed, GetBonusSpeed);
    }
    #endregion
    
    #region Stat

    private void GetBasicStat()
    {
        health.SetUpgradeHealth(GameDB.playerUpgradeData.healthStat[PlayerSave.PlayerHealthLevel] + (int)skinController.GetSkinBonusStat(SkinBonusType.Health, 0));

        moveSpeed = GameDB.playerUpgradeData.speedStat[PlayerSave.PlayerSpeedLevel] * skinController.GetSkinBonusStat(SkinBonusType.Speed, 1)/* * bonusSpeed*/;
    }
    
    // private void GetBonusSpeed()
    // {
    //     bonusSpeed = 1.1f;
    //     GetBasicStat();
    // }
    // private void RestoreSingleHealth()
    // {
    //     health.RestoreHealth(health.currentHealth + 1);
    // }
    private void RestoreFullHealth()
    {
        if (health.currentHealth < health.maximumHealth)
            Observer.Instance.Notify(EventID.RestoreFullHealth);
        
        health.RestoreHealth(health.maximumHealth);
    }
    private void OnEndGameStat()
    {
        // bonusSpeed = 1f;
        // moveSpeed = GameDB.playerUpgradeData.speedStat[PlayerSave.PlayerSpeedLevel] *
        //             skinController.GetSkinBonusStat(SkinBonusType.Speed, 1);
        RestoreFullHealth();
    }
    #endregion

    #region Movement
    private void DoMove()
    {
        direction = Vector3.forward * VariableJoystick.Vertical + Vector3.right * VariableJoystick.Horizontal;
        transform.position += direction.normalized * (moveSpeed * Time.deltaTime);
        if (direction != Vector3.zero)
        {
            if (CurrentWeapon.weaponAutoAim.target == null)
            {
                transform.LookAtDirection(direction, 10f);
                animController.SetFloatAnimRun(0, 1);
            }
            else
            {
                animController.SetAnimRunAndGun();
            }
            
            animController.OnVelocity(moveSpeed);
            isMoving.Invoke();
        }
        else
        {
            animController.OnVelocity(0);
        }
    }
    #endregion

    #region Death / Revive
    private void DeathAction(Health h)
    {
        Observer.Instance.Notify(EventID.Dead);
        health.revivable = true;
        DisableControl();
    }
    private void Revive()
    {
        health.Revive();
        EnableControl();
    }

    public void EnableControl()
    {
        CurrentWeapon.enabled = true;
        VariableJoystick.gameObject.SetActive(true);
    }

    public void DisableControl()
    {
        CurrentWeapon.enabled = false;
        VariableJoystick.Release();
        VariableJoystick.gameObject.SetActive(false);
    }
    #endregion

    #region Teleport

    private void EnterTeleport()
    {
        DisableControl();
        navMeshAgent.enabled = false;
    }

    private void ExitTeleport()
    {
        EnableControl();
        navMeshAgent.enabled = true;
    }
    #endregion
}