using ATSoft.Ads;
using TMPro;
using UnityBase.DesignPattern;
using UnityEngine;
using UnityEngine.UI;
using static NVTT.Utilities;
using static NVTT.UIUtilities;

public class RebuildAllBarrier : MonoBehaviour
{
    [SerializeField] private Button rebuildBtn;
    [SerializeField] private TextMeshProUGUI rebuildCost;
    [SerializeField] private Image iconCurrency;
    [SerializeField] private Sprite[] icon;

    private void Start()
    {
        Observer.Instance.AddObserver(EventID.DiamondChange, d =>
        {
            if (!gameObject.activeInHierarchy) return;
            UIResponse();
        });
        
        rebuildBtn.onClick.AddListener(() =>
        {
            rebuildBtn.transform.InteractableBtnDiamondResponse(
                GameDB.barrierData.rebuildAllDiamondCost[PlayerSave.BarrierLevel],
                Advertisements.Instance.IsRewardVideoAvailable(),
                () =>
                {
                    Observer.Instance.Notify(EventID.RebuildAllBarrier);
                    gameObject.SetActive(false);
                }, "Rebuild_Barrier");
        });
    }

    private void OnEnable()
    {
        UIResponse();
    }

    private void UIResponse()
    {
        if (IsEnoughDiamond(GameDB.barrierData.rebuildAllDiamondCost[PlayerSave.BarrierLevel]))
        {
            iconCurrency.sprite = icon[0];
            EnoughDiamondTextResponse(GameDB.barrierData.rebuildAllDiamondCost[PlayerSave.BarrierLevel], rebuildCost);
            return;
        }
        
        if (!IsPassBasicTutorial)
            return;

        iconCurrency.sprite = icon[1];
        rebuildCost.text = "Ads";
    }
}
