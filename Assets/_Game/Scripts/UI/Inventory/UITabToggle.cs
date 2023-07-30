using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UITabToggle : MonoBehaviour
{
    public Toggle toggle;
    [SerializeField] private Vector3 scaleOnSelect;
    [SerializeField] private Image curImg;
    [SerializeField] private HorizontalLayoutGroup hg;
    [SerializeField] private Sprite[] img;
    private void OnValidate()
    {
        if (toggle == null)
            toggle = GetComponent<Toggle>();
        if (hg == null)
            hg = GetComponentInParent<HorizontalLayoutGroup>();
        if (curImg == null)
            curImg = GetComponent<Image>();
    }
    private void OnEnable()
    {
        OnToggleValueChange(toggle.isOn);
    }
    public void OnToggleValueChange(bool value)
    {
        transform.DOScale(value ? scaleOnSelect : Vector3.one, 0.5f).SetUpdate(true).onUpdate = () => hg.SetLayoutHorizontal();
        curImg.sprite = value ? img[0] : img[1];
    }
    
}
