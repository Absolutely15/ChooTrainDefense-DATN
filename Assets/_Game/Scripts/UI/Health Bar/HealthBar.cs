using UnityEngine;
using DG.Tweening;

public class HealthBar : MonoBehaviour, IHealthBar
{
    [SerializeField] private bool isShowing;
    [SerializeField] private float duration = 1f;

    [SerializeField] private SpriteRenderer healthBarBG;
    [SerializeField] private SpriteRenderer healthBarFill;

    private float length;
    private float sizeY;
    
    private Tween tween;

    public void Init()
    {
        var size = healthBarFill.size;
        length = size.x;
        sizeY = size.y;
    }

    private void OnDisable()
    {
        isShowing = false;
        SetTargetValue(length, true);
        gameObject.SetActive(false);
    }


    public void ShowHP(float hp, float hpMax)
    {
        SetTargetValue(length * (hp / hpMax));

        if (isShowing) return;
        isShowing = true;
        gameObject.SetActive(true);

    }

    private void SetTargetValue(float newTarget, bool force = false)
    {
        var targetValue = healthBarFill.size;
        targetValue.x = newTarget;
        
        tween?.Kill();
        
        if (force)
        {
            healthBarFill.size = targetValue;
            SetValue(targetValue.x);
        }
        else
        {
            tween = DOVirtual.Float(healthBarFill.size.x, targetValue.x, duration, SetValue)
                .OnComplete(() => SetValue(targetValue.x));
        }
    }
    
    private void SetValue(float amount)
    {
        healthBarFill.size = new Vector2(amount, sizeY);
    }
}