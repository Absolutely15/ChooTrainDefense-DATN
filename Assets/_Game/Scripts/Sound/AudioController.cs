using UnityBase.DesignPattern;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    private void Start()
    {
        Observer.Instance.AddObserver(EventID.Dead, b => AudioManager.Instance.PlayAudio(AudioType.PlayerDeath));
        Observer.Instance.AddObserver(EventID.Revive, b => AudioManager.Instance.PlayAudio(AudioType.PlayerRevive));
        Observer.Instance.AddObserver(EventID.RebuildBarrier, b => AudioManager.Instance.PlayAudio(AudioType.RebuildBarrier));
        Observer.Instance.AddObserver(EventID.RebuildAllBarrier, b => AudioManager.Instance.PlayAudio(AudioType.RebuildAllBarrier));
        Observer.Instance.AddObserver(EventID.EndGameLevel, b => AudioManager.Instance.PlayAudio(AudioType.CollectAllCoin));
    }
}
