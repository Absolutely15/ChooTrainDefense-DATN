using UnityEngine;
using TMPro;
using DG.Tweening;

public class UITextCountAnimation : MonoBehaviour
{
	public bool useSeparator;
	[SerializeField] TextMeshProUGUI tmgui;

	private int currentValue;
	private int targetValue;

	private Tween tween;

    protected void OnValidate()
    {
		if (tmgui == null)
			tmgui = GetComponent<TextMeshProUGUI>();
	}
    
    
    protected virtual void OnDisable()
    {
	    if (currentValue != targetValue)
		    SetText(targetValue);
    }

    protected void SetTargetValue(int newTarget, bool force = false)
	{
		targetValue = newTarget;

		tween?.Kill();
		if (force)
		{
			currentValue = targetValue;
			SetText(targetValue);
		}
		else
		{
			tween = DOVirtual.Float(currentValue, targetValue, 1, t =>
			{
				SetText(Mathf.RoundToInt(t));
			}).SetUpdate(true).OnComplete(() => SetText(targetValue));
		}
	}

    private void SetText(int number)
    {
	    currentValue = number;
	    tmgui.text = useSeparator ? $"{number:#,###0}".Replace(".", ",") : number.ToString();
    }

	
}
