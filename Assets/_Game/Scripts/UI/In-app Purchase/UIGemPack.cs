using DG.Tweening;
using UnityEngine;

public class UIGemPack : MonoBehaviour
{
    [SerializeField] private int id;
    [SerializeField] private GameObject confetti;

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        IAPConsumableManager.Instance.onPurchaseComplete.AddListener(VFXResponse);
    }

    private void VFXResponse(int index)
    {
        if (index != id) return;
        confetti.SetActive(true);
        DOVirtual.DelayedCall(3, () => confetti.SetActive(false));
    }
}
