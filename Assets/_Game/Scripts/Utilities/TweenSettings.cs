using UnityEngine;
using DG.Tweening;

public class TweenSettings : MonoBehaviour
{
    [SerializeField] public int tweenersCapacity;
    [SerializeField] public int sequencesCapacity;
    private void Awake()
    {
        DOTween.SetTweensCapacity(tweenersCapacity, sequencesCapacity);
    }

    private void OnDestroy()
    {
        DOTween.KillAll();
    }
}
