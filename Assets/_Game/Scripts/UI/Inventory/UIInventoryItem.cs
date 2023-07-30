using UnityEngine;
using UnityEngine.UI;

public class UIInventoryItem<T> : MonoBehaviour where T : ItemData
{
    public Button btn;
    
    [HideInInspector] public T data;
    private void OnValidate()
    {
        if (btn == null)
            btn = GetComponent<Button>();
    }

    private void Start()
    {
        btn.onClick.AddListener(OnClick);
    }

    public virtual void Display(T itemData)
    {
        data = itemData;
    }

    public virtual void Display(T itemData, bool isPremium)
    {
        data = itemData;
    }

    public virtual void OnClick()
    {
        
    }
}
