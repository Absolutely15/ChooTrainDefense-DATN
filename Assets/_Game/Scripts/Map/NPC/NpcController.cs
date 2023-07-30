using static NVTT.Utilities;
using UnityBase.DesignPattern;
using UnityEngine;

[DefaultExecutionOrder(-2)]
public class NpcController : MonoBehaviour
{
    [SerializeField] private Animator anim;
    private static readonly int IsInCombat = Animator.StringToHash("IsInCombat");

    private void OnValidate()
    {
        anim = GetComponentInChildren<Animator>();
    }

    private void OnEnable()
    {
        if (!Gameplay.IsInGameplay) return;
        anim.SetBool(IsInCombat, true);
    }

    private void Start()
    {
        Observer.Instance.AddObserver(EventID.StartGameLevel, StartLevelAnim);
        Observer.Instance.AddObserver(EventID.EndGameLevel, EndLevelAnim);
    }

    private void StartLevelAnim() => anim.SetBool(IsInCombat, true);
    private void EndLevelAnim() => anim.SetBool(IsInCombat, false);
}
