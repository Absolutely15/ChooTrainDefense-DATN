using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class DotweenScale : MonoBehaviour
{
    #region Properties

    public bool ignoreTimescale = true;
    
    #region Scale Up
    [Title("Pop Up")]
    public bool scaleToTarget;
    public bool customScaleUp;
    public float scaleUpDuration = 0.4f;
    
    public Vector3 startScale;
    public Vector3 targetScale = Vector3.one;
    public Vector3 customScale;
    
    public Ease easeTypeUp = Ease.OutQuad;

    #endregion

    #region Scale Down
    [Title("Dismiss")]
    public bool customEaseDown;
    public float scaleDownDuration = 0.3f;
    
    public GameObject go;

    [HideIf("customEaseDown")]
    public Ease easeTypeDown = Ease.InSine;

    [ShowIf("customEaseDown")]
    public AnimationCurve easeCustomDown;

    #endregion
    
    private Transform TF => transform;
    private Tween tweenUp;
    private Tween tweenUpExtra;
    private Tween tweenDown;
    
    #endregion

    #region Main Functions
    private void OnEnable()
    {
        TF.localScale = startScale;

        if (customScaleUp)
        {
            TweenUp(scaleUpDuration/2, easeTypeUp, customScale);
            return;
        }
        
        if (targetScale == Vector3.one || scaleToTarget)
        {
            TweenUp(scaleUpDuration, easeTypeUp);
        }
        else
        {
            TweenUp(scaleUpDuration / 2, easeTypeUp, Vector3.one);
        }
    }

    [Button]
    public void OnClose()
    {
        if (!go.activeInHierarchy)
            return;
        
        if (scaleDownDuration == 0)
        {
            go.SetActive(false);
            return;
        }
        
        if (customEaseDown)
        {
            TweenDown(easeCustomDown);
        }
        else
        {
            TweenDown(easeTypeDown);
        }
    }
    #endregion

    #region Tween
    private void TweenUp(float duration, Ease curve)
    {
        tweenUp?.Kill();
        tweenUp = TF.DOScale(targetScale, duration).SetUpdate(ignoreTimescale).SetEase(curve);
    }
    
    private void TweenUp(float duration, Ease curve, Vector3 target)
    {
        tweenUp?.Kill();
        tweenUp = TF.DOScale(targetScale, duration).SetUpdate(ignoreTimescale).SetEase(curve)
            .OnComplete(() => TweenUpExtra(duration, target));
    }

    private void TweenUpExtra(float duration, Vector3 target)
    {
        tweenUpExtra?.Kill();
        tweenUpExtra = TF.DOScale(target, duration).SetUpdate(ignoreTimescale).SetEase(easeTypeUp);
    }
    
    private void TweenDown(Ease curve)
    {
        tweenDown?.Kill();
        tweenDown = TF.DOScale(0, scaleDownDuration).SetEase(curve).SetUpdate(ignoreTimescale)
            .OnComplete(() => go.SetActive(false));
    }
    private void TweenDown(AnimationCurve curve)
    {
        tweenDown?.Kill();
        tweenDown = TF.DOScale(0, scaleDownDuration).SetEase(curve).SetUpdate(ignoreTimescale)
            .OnComplete(() => go.SetActive(false));
    }
    #endregion

}
