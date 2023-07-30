using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class PlayerHitEffect : MonoBehaviour
{
    [Title("VFX")]
    [SerializeField] private List<Renderer> preRends;
    [SerializeField] private Health player;
    [SerializeField] private Animator anim;
    [Title("SFX")]
    [SerializeField] private bool playAudioOnHit = true;
    [SerializeField] private AudioSource audioSource;

    private void Awake()
    {
        player.onSufferDamage.AddListener((a,b) =>
        {
            if (a.currentHealth >= 1)
                Play();
        });
        player.onRevive.AddListener(a=> PlayWithoutAudio());
    }

    [Button]
    public void Play() => Play(preRends);
    [Button]
    public void PlayWithoutAudio() => Play(preRends, false);

    private void Play(List<Renderer> renderers, bool playAudio = true)
    {
        foreach (var t in renderers)
        {
            PlaySingle(t);
        }

        if (playAudioOnHit && audioSource != null && playAudio)
        {
            audioSource.Play();
        }
    }

    private void PlaySingle(Component rend)
    {
        if (!rend.gameObject.activeInHierarchy) return;
        DOVirtual.Float(0, 1, player.invincibilityDur, t =>
        {
            anim.SetLayerWeight(1, 1);
        }).OnComplete(() =>
        {
            anim.SetLayerWeight(1, 0);
        });
    }
}
