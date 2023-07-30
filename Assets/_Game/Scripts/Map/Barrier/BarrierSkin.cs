using System.Collections.Generic;
using UnityBase.DesignPattern;
using UnityEngine;
using NVTT;

public class BarrierSkin : MonoBehaviour
{
    [SerializeField] private List<GameObject> modelSkin;

    private void Start()
    {
        InitObserver();
        SetCurrentBarrierSkin();
    }

    private void InitObserver()
    {
        Observer.Instance.AddObserver(EventID.UpgradeBarrier, UpgradeBarrier);
    }

    private void SetCurrentBarrierSkin()
    {
        for (var i = 0; i < modelSkin.Count; i++)
        {
            modelSkin[i].SetActive(i == PlayerSave.BarrierLevel);
        }
    }
    private void UpgradeBarrier() => transform.UpgradeFeedback(SetCurrentBarrierSkin);
    
}
