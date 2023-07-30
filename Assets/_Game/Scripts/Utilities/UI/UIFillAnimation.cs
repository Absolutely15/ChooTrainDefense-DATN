using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIFillAnimation : MonoBehaviour
{
    public float duration = 1f;
    [SerializeField] private Image imageFill;

    private float currentValue;
    private float targetValue;
    
    private Tween tween;
    private void OnValidate()
    {
        if (imageFill == null)
        {
            imageFill = GetComponentInChildren<Image>();
        }
    }
    public void OnDisable()
    {
        if (Math.Abs(currentValue - targetValue) > 0.1)
            SetValue(targetValue);
    }

    protected void SetTargetValue(float newTarget, bool force = false)
    {
        targetValue = newTarget;

        tween?.Kill();
        if (force)
        {
            currentValue = targetValue;
            SetValue(targetValue);
        }
        else
        {
            tween = DOVirtual.Float(currentValue, targetValue, duration, SetValue).OnComplete(() => SetValue(targetValue));
        }
    }

    private void SetValue(float amount)
    {
        currentValue = amount;
        imageFill.fillAmount = amount;
    }
}
