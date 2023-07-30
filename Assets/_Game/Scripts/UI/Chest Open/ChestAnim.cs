using DG.Tweening;
using Spine.Unity;
using UnityEngine;

public class ChestAnim : MonoBehaviour
{
    public ChestAnimEvent chestAnimEvent;
    
    [SerializeField] private SkeletonGraphic chestAnim;
    [SerializeField] private Animator openedAnim;
    [SerializeField] private Animator chestTypeAnim;

    [SerializeField] private ParticleSystem openedParticle;
    [SerializeField] private ParticleSystem chestDropParticle;
    
    private Spine.AnimationState chestAnimState;

    private void Start()
    {
        chestAnimState = chestAnim.AnimationState;
        chestAnimEvent.onFinishAnimation.AddListener(ChestReward);
    }
    
    public void OpenChestResponse()
    {
        //Chest Anim
        chestAnimState.SetAnimation(0, "xuathien", false);
        chestAnimState.AddAnimation(0, "idle", true, 0f);
        chestAnimState.AddAnimation(0, "open", false, 2f);
        chestAnimState.AddAnimation(0, "open_idle", true, 3f);

        //Particle Response
        DOVirtual.DelayedCall(2 / 3f, ChestFall);
        
        DOVirtual.DelayedCall(4f, ChestOpening);
    }

    private void ChestFall()
    {
        chestDropParticle.gameObject.SetActive(true);
        AudioManager.Instance.PlayAudio(AudioType.ChestFall);
    }

    private void ChestOpening()
    {
        openedAnim.enabled = true;
        openedParticle.gameObject.SetActive(true);
        AudioManager.Instance.PlayAudio(AudioType.ChestOpening);
    }

    private void ChestReward()
    {
        chestTypeAnim.enabled = true;
        AudioManager.Instance.PlayAudio(AudioType.ChestReward);
    }
}
