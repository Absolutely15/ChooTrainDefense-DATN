using DG.Tweening;
using UnityEngine;
public class UnlockTrap : MonoBehaviour, IUnlockObject
{
    public int UnlockCost { get; set; }
    
    [SerializeField] private Trap trap;

    public void Init()
    {
        UnlockCost = trap.UnlockCost;
    }

    public void UnlockSuccessfully()
    {
        trap.UnlockTrap();
        transform.DOScale(0, 0.25f).SetEase(Ease.InSine).OnComplete(() => gameObject.SetActive(false));
    }
}
