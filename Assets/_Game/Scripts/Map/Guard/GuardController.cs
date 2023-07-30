using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class GuardController : MonoBehaviour
{
    #region Properties
    [Title("State")]
    public int level;
    public int id;
    
    [Title("Ref")]
    public GameObject model;
    public GuardUI guardUI;
    public GuardWeapon guardWeapon;
    public GuardSkin guardSkin;

    [Title("Event")]
    public UnityEvent<GuardController> onUpgrade = new UnityEvent<GuardController>();
    public UnityEvent<GuardController> updateData = new UnityEvent<GuardController>();
    
    private bool IsUnlocked => level > 0;
    #endregion

    #region Unity Functions
    private void Start()
    {
        Init();
        InitEvent();
    }
    #endregion

    #region Init
    private void Init()
    {
        model.SetActive(IsUnlocked);
        guardUI.Init(level, IsUnlocked);
        
        if (!IsUnlocked) return;
        guardWeapon.Init();
        GetData();
    }
    private void InitEvent()
    {
        updateData.AddListener(UpdateData);
    }
    #endregion

    #region Unlock & Upgrade
    private void UpdateData(GuardController g)
    {
        GetData();
        if (level != 1) return;
        guardWeapon.Init();
    }

    private void GetData()
    {
        guardWeapon.GetWeaponData(level);
        guardSkin.GetMaterialData(level);
    }

    public void UpgradeGuardFeedback()
    {
        //Feedback & save data
        var upgradeGuard = DOTween.Sequence();
        upgradeGuard.Append(model.transform.DOScale(0, 0.15f).SetEase(Ease.InSine))
            .AppendCallback(() =>
            {
                guardUI.SetLevelText(level);
                updateData.Invoke(this);
            })
            .Append(model.transform.DOScale(1, 0.15f).SetEase(Ease.OutQuad));
    }
    #endregion
}
