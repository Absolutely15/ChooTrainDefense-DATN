using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Transitions : Singleton<Transitions>
{
    [SerializeField] private RectTransform mask, background;
    [SerializeField, HideInInspector] private Image maskable, bg;
    [SerializeField] private float fadeOutDuration = 3;
    [SerializeField] private float fadeInDuration = 2;

    private Tween fadeTween;
    private bool isFading;
    private void OnValidate()
    {
        if (mask != null && maskable == null)
            maskable = mask.GetComponent<Image>();
        
        if (background != null && bg == null)
            bg = background.GetComponent<Image>();
    }

    private void Start()
    {
        FadeOut();
    }

    private void LateUpdate()
    {
        if (!isFading) return;
        Smoothing(maskable, 1);
    }

    [Button]
    public void FadeOut()
    {
        fadeTween?.Kill();
        isFading = true;
        bg.raycastTarget = false;
        mask.transform.localScale = Vector3.one;
        fadeTween = mask.DOScale(10, fadeOutDuration).SetEase(Ease.OutQuad).OnComplete(CompleteFadeOut);
    }

    private void CompleteFadeOut()
    {
        isFading = false;
        background.gameObject.SetActive(false);
    }

    private void FadeInPreparation()
    {
        fadeTween?.Kill();
        isFading = true;
        bg.raycastTarget = true;
        background.gameObject.SetActive(true);
        mask.transform.localScale = Vector3.one * 10;
    }

    [Button]
    public void FadeAllIn(UnityAction actionComplete)
    {
        FadeInPreparation();
        fadeTween = mask.DOScale(0, fadeInDuration).SetEase(Ease.InQuad).OnComplete(() => CompleteFadeIn(actionComplete));
    }
    
    [Button]
    public void FadeIn(UnityAction actionComplete, float firstFadeInDuration = 1f, float secondFadeInDuration = 0.5f)
    {
        FadeInPreparation();
        fadeTween = DOTween.Sequence()
            .Append(mask.DOScale(1, firstFadeInDuration).SetEase(Ease.InQuad))
            .Append(mask.DOScale(2, secondFadeInDuration).SetEase(Ease.OutQuad))
            .Append(mask.DOScale(0, fadeInDuration - firstFadeInDuration - secondFadeInDuration).SetEase(Ease.InQuad))
            .AppendCallback(() => CompleteFadeIn(actionComplete));
    }

    private void CompleteFadeIn(UnityAction actionComplete)
    {
        isFading = false;
        actionComplete.Invoke();
    }
    
    private static void Smoothing(MaskableGraphic graphic, float smooth)
    {
        if (!graphic) return;
        
        var canvasRenderer = graphic.canvasRenderer;
        var currentColor = canvasRenderer.GetColor();
        var targetAlpha = 1f;
        if (graphic.maskable && 0 < smooth)
        {
            var currentAlpha = graphic.color.a * canvasRenderer.GetInheritedAlpha();
            if (0 < currentAlpha)
            {
                targetAlpha = Mathf.Lerp(0.01f, 0.002f, smooth) / currentAlpha;
            }
        }

        if (Mathf.Approximately(currentColor.a, targetAlpha)) return;
        currentColor.a = Mathf.Clamp01(targetAlpha);
        canvasRenderer.SetColor(currentColor);
    }
}
