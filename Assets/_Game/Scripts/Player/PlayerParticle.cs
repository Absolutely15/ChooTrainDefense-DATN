using System.Collections.Generic;
using UnityBase.DesignPattern;
using UnityEngine;

public class PlayerParticle : MonoBehaviour
{
    [SerializeField] private List<ParticleSystem> playerVFX;
    private void Start()
    {
        Observer.Instance.AddObserver(EventID.Revive, PlayHealBigParticle);
        Observer.Instance.AddObserver(EventID.RestoreFullHealth, PlayHealBigParticle);
        //Observer.Instance.AddObserver(EventID.RestoreSingleHealth, PlayHealSmallParticle);
        //Observer.Instance.AddObserver(EventID.BonusSpeed, PlayBonusSpeedParticle);
    }

    private void PlayHealBigParticle() => playerVFX[0].Play();
    
    private void PlayHealSmallParticle() => playerVFX[1].Play();
    
    private void PlayBonusSpeedParticle() => playerVFX[2].Play();

}
