using UnityEngine;
using UnityEngine.UI;

public class SettingsPanel : MonoBehaviour
{
    [SerializeField] private Button restoreBtn;

    private void Awake()
    {
#if UNITY_IOS
        restoreBtn.gameObject.SetActive(true);
        restoreBtn.onClick.AddListener(InAppPurchaseManager.Instance.RestorePurchases);
#endif
    }
}
