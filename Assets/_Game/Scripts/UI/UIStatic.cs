using UnityEngine;
using UnityEngine.UI;

public class UIStatic : MonoBehaviour
{
    [SerializeField] private GameObject addBonusGold;
    [SerializeField] private Button addGold;
    [SerializeField] private Button addDiamond;

    private void Start()
    {
        addGold.onClick.AddListener(() => addBonusGold.SetActive(true));
        addDiamond.onClick.AddListener(UIManager.Instance.ScrollToGemsPack);
    }
}
