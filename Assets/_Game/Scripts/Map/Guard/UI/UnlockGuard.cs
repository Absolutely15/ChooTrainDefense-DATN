using DG.Tweening;
using UnityEngine;
using static NVTT.Utilities;

public class UnlockGuard : MonoBehaviour, IUnlockObject
{
    public int UnlockCost { get; set; }
    
    [SerializeField] private GuardController guard;

    public void Init()
    {
        UnlockCost = GameDB.guardUpgradeData.upgradeCost[0];
    }

    public void UnlockSuccessfully()
    {
        Unlock();
        transform.DOScale(0, 0.25f).SetEase(Ease.InSine).OnComplete(() => gameObject.SetActive(false));
    }

    private void Unlock()
    {
        guard.level++;
        guard.guardUI.UnlockGuard(guard.level);
        guard.model.SetActive(true);
        guard.updateData.Invoke(guard);
    }
}
