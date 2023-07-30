using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class TrapUnlockedUI : MonoBehaviour
{
    public Image circleBG;
    public Image circleFill;
    private void Start()
    {
        circleFill.fillAmount = 0;
        circleBG.gameObject.SetActive(false);
    }

    public void SpikeDotween(Action action)
    {
        circleBG.gameObject.SetActive(true);
        DOVirtual.Float(0, 1, 5f, t => { circleFill.fillAmount = t; }).OnComplete(() =>
        {
            circleBG.gameObject.SetActive(false);
            action();
        });
    }
}
