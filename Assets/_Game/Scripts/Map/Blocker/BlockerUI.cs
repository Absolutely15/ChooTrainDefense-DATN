using System;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BlockerUI : MonoBehaviour
{
    [Title("UI")]
    public RectTransform goldFlyDes;
    [SerializeField] private Image goldRequiredFill;
    [SerializeField] private TextMeshProUGUI goldRequiredText;

    [Title("Tutorial")]
    [SerializeField] private GameObject canvas;
    [SerializeField] private Collider boxCollider;

    private int goldRequire;
    private int goldReceivedUI;

    public void Init(int goldReceived, int goldRequired, int goldRemain)
    {
        goldRequire = goldRequired;
        goldReceivedUI = goldReceived;
        goldRequiredFill.fillAmount = (float) goldReceived/ goldRequired;
        goldRequiredText.text = goldRemain.ToString();
    }

    public void UIUpdate(Action action)
    {
        goldReceivedUI++;
        goldRequiredText.text = (goldRequire - goldReceivedUI).ToString();
        goldRequiredFill.fillAmount = (float)goldReceivedUI / goldRequire;
        
        //Event invoke
        if (goldReceivedUI != goldRequire) return;
        action.Invoke();
    }
    
    public void SetBlockerStatus(bool isActive)
    {
        canvas.gameObject.SetActive(isActive);
        boxCollider.enabled = isActive;
    }
}
