using UnityEngine;
using UnityEngine.UI;

public class UIMainMenu : MonoBehaviour
{
    [SerializeField] private Button inventoryBtn;
    [SerializeField] private Button shopBtn;

    private void Start()
    {
        inventoryBtn.onClick.AddListener(() => ButtonOnClick(UIManager.Instance.inventoryPanel.gameObject));
        shopBtn.onClick.AddListener(() => ButtonOnClick(UIManager.Instance.iapManagerPanel.gameObject));
    }

    private static void ButtonOnClick(GameObject go)
    {
        go.SetActive(true);
    }
}
