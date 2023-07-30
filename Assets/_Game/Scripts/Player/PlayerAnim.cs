using UnityBase.DesignPattern;
using UnityEngine;
using static NVTT.Utilities;

public class PlayerAnim : MonoBehaviour
{
    #region Properties
    [SerializeField] private Animator anim;
    
    private Vector3 faceDirect;
    private Vector2 moveDirect;
    private float angle;
    
    private const float BASE_SPEED_ANIM = 1.25f;
    private static readonly int Speed = Animator.StringToHash("Speed");
    private static readonly int RunSpeed = Animator.StringToHash("RunSpeed");
    private static readonly int Shoot = Animator.StringToHash("Shoot");
    private static readonly int Death = Animator.StringToHash("Death");
    private static readonly int Revive = Animator.StringToHash("Revive");
    private static readonly int InputX = Animator.StringToHash("InputX");
    private static readonly int InputY = Animator.StringToHash("InputY");
    private static readonly int ExitTeleport = Animator.StringToHash("ExitTeleport");
    #endregion

    #region Unity Functions
    private void OnValidate()
    {   
        if (anim == null)
            anim = GetComponentInChildren<Animator>();
    }

    private void Awake()
    {
        Init();
        InitEvent();
    }
    #endregion

    #region Init
    private void Init()
    {
        GetAnimStat();
    }

    private void InitEvent()
    {
        Observer.Instance.AddObserver(EventID.UpgradeStat, GetAnimStat);
        //Observer.Instance.AddObserver(EventID.BonusSpeed, GetAnimStat);
        Observer.Instance.AddObserver(EventID.EndGameLevel, GetAnimStat);
        Observer.Instance.AddObserver(EventID.Dead, a => anim.SetTrigger(Death));
        Observer.Instance.AddObserver(EventID.Revive, a => anim.SetTrigger(Revive));
        Observer.Instance.AddObserver(EventID.EnterTeleport, AnimEnterTeleport);
        Observer.Instance.AddObserver(EventID.ExitTeleport, AnimExitTeleport);
    }
    #endregion

    #region Animation
    private void GetAnimStat() => anim.SetFloat(RunSpeed, GameDB.playerUpgradeData.speedAnimStat[PlayerSave.PlayerSpeedLevel]/* * Player.bonusSpeed*/);
    public void ExitTeleportAnim(bool isExit) => anim.SetBool(ExitTeleport, isExit);
    public void TriggerAnimShoot() => anim.SetTrigger(Shoot);

    private void AnimEnterTeleport()
    {
        anim.updateMode = AnimatorUpdateMode.UnscaledTime;
    }

    private void AnimExitTeleport()
    {
        anim.updateMode = AnimatorUpdateMode.Normal;
        ExitTeleportAnim(false);
    }

    public void OnVelocity(float vel)
    {
        vel /= BASE_SPEED_ANIM;
        anim.SetFloat(Speed, vel);
    }

    public void SetFloatAnimRun(float x, float y)
    {
        anim.SetFloat(InputX, x);
        anim.SetFloat(InputY, y);
    }

    public void SetAnimRunAndGun()
    {
        faceDirect = transform.forward;
        moveDirect.Set(PlayerController.VariableJoystick.Horizontal, PlayerController.VariableJoystick.Vertical);
        angle = Vector3.SignedAngle(Vector3.forward, faceDirect, Vector3.up);
        moveDirect = moveDirect.Rotate(angle);
        SetFloatAnimRun(moveDirect.x, moveDirect.y);
    }
    #endregion
}
