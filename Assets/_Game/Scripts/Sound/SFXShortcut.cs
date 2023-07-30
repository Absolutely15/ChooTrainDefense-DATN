using UnityEngine;

public class SFXShortcut : MonoBehaviour
{
    public AudioType audioType;
    public bool canReplayWhilePlaying = true;

    public void Play()
    {
        AudioManager.Instance.PlayAudio(audioType, canReplayWhilePlaying);
    }

    public void Stop()
    {
        AudioManager.Instance.StopAudio(audioType);
    }

    public void Restart()
    {
        AudioManager.Instance.RestartAudio(audioType);
    }
}
