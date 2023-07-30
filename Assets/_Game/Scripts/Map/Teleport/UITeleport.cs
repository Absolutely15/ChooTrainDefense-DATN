using DG.Tweening;
using NVTT;
using UnityEngine;
using UnityEngine.UI;

public class UITeleport : MonoBehaviour
{
    [SerializeField] private Image blackPanel;

    private void OnEnable()
    {
        FadeIn(1f);
    }

    public void FadeIn(float duration)
    {
        blackPanel.DOFade(1, duration).SetUpdate(true);
    }

    public void FadeOut(float duration)
    {
        blackPanel.Fade(0.25f);
        
        blackPanel.DOFade(0, duration).SetUpdate(true).OnComplete(() => gameObject.SetActive(false));
    }
}
