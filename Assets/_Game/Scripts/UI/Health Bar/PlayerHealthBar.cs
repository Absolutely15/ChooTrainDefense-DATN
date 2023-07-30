using System.Collections.Generic;
using UnityBase.DesignPattern;
using UnityEngine;
using UnityEngine.UI;
using static NVTT.Utilities;

public class PlayerHealthBar : UIFillAnimation, IHealthBar
{
    [SerializeField] private Image healthBarBG;
    [SerializeField] private List<Image> health;

    private static Health PlayerHealth => Player.health;
    private float length;
    private float sizeY;
    public void Init()
    {
        length = healthBarBG.rectTransform.sizeDelta.x;
        SplitHealthBar();
        SetTargetValue((float) PlayerHealth.currentHealth / PlayerHealth.maximumHealth, true);
        
        Observer.Instance.AddObserver(EventID.UpgradeStat, SplitHealthBar);
        Observer.Instance.AddObserver(EventID.ChangeSkin, SplitHealthBar);
    }
    

    public void ShowHP(float hp, float hpMax)
    {
        SetTargetValue((float) PlayerHealth.currentHealth / PlayerHealth.maximumHealth);
    }

    private void SplitHealthBar()
    {
        for (var i = 0; i < health.Count; i++)
        {
            health[i].gameObject.SetActive(i < PlayerHealth.maximumHealth - 1);
        }
        
        for (var i = 0; i < health.Count; i++)
        {
            if (!health[i].gameObject.activeInHierarchy) continue;
            var pos = health[i].rectTransform.anchoredPosition;
            pos.x = (i + 1) * length / PlayerHealth.maximumHealth;
            health[i].rectTransform.anchoredPosition = pos;
        }
    }
}
