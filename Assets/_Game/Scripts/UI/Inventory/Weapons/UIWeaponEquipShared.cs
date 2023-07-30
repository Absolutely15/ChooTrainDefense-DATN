using ATSoft;
using NVTT;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIWeaponEquipShared : MonoBehaviour
{
    public UnityEvent equipGun = new UnityEvent();
    public UnityEvent goldenGun = new UnityEvent();
    
    [SerializeField] private Button unlockBtn;
    [SerializeField] private Button goldenBtn;
    [SerializeField] private Button equipBtn;
    [Space]
    [SerializeField] private Image equipImage;
    [Space]
    [SerializeField] private TextMeshProUGUI equipText;
    [SerializeField] private TextMeshProUGUI unlockCost;
    [SerializeField] private TextMeshProUGUI goldenCost;
    [Space]
    [SerializeField] private Sprite[] sprite;
    
    private WeaponLocalData thisWeaponData;

    private void Start()
    {
        unlockBtn.onClick.AddListener(() => UnlockGun(thisWeaponData));
        equipBtn.onClick.AddListener(() => EquipGun(thisWeaponData));
        goldenBtn.onClick.AddListener(() => GoldenGun(thisWeaponData));
    }

    public void Display(WeaponLocalData wData, WeaponState state)
    {
        thisWeaponData = wData;
        
        unlockBtn.gameObject.SetActive(state == WeaponState.Unlockable);
        equipBtn.gameObject.SetActive(state == WeaponState.Unlocked || state == WeaponState.Golden);
        goldenBtn.gameObject.SetActive(state == WeaponState.Unlocked);
        
        switch (state)
        {
            case WeaponState.Unlockable:
                unlockCost.text = wData.unlockDiamondCost.ToString();
                break;
            case WeaponState.Unlocked:
                goldenCost.text = wData.goldenDiamondCost.ToString();
                break;
        }

        if (state != WeaponState.Unlocked && state != WeaponState.Golden) return;
        if (wData.id == PlayerSave.GetWeaponIsUsing())
        {
            equipText.text = "Equipped";
            equipBtn.interactable = false;
            equipImage.sprite = sprite[1];
        }
        else
        {
            equipText.text = "Equip";
            equipBtn.interactable = true;
            equipImage.sprite = sprite[0];
        }
    }
    private void UnlockGun(WeaponLocalData wData)
    {
        if (PlayerSave.Diamond >= wData.unlockDiamondCost)
        {
            PlayerSave.Diamond -= wData.unlockDiamondCost;
            wData.SetWeaponState(WeaponState.Unlocked);
            EquipGun(wData);
            
            //Ads
            AnalyticsManager.LogEventDiamondSpend(wData.unlockDiamondCost, "unlock_weapon", $"{wData.itemName}");
        }
        else
        {
            unlockBtn.transform.NotInteractableResponse();
            AudioManager.Instance.PlayAudio(AudioType.Click, true);
            
            //Ads
            UIAdsManager.Instance.PopUpNotEnoughDiamond();
        }
    }
    private void EquipGun(WeaponLocalData weaponLocalData)
    {
        weaponLocalData.SetWeaponIsUsing();
        Display(weaponLocalData, weaponLocalData.GetWeaponState());
        equipGun.Invoke();
        AudioManager.Instance.PlayAudio(AudioType.Equip, true);
    }

    private void GoldenGun(WeaponLocalData wData)
    {
        if (PlayerSave.Diamond >= wData.goldenDiamondCost)
        {
            PlayerSave.Diamond -= wData.goldenDiamondCost;
            wData.SetWeaponState(WeaponState.Golden);
            EquipGun(wData);
            goldenGun.Invoke();
            
            //Ads
            AnalyticsManager.LogEventDiamondSpend(wData.goldenDiamondCost, "golden_weapon", $"{wData.itemName}");
        }
        else
        {
            goldenBtn.transform.NotInteractableResponse();
            AudioManager.Instance.PlayAudio(AudioType.Click, true);
            
            //Ads
            UIAdsManager.Instance.PopUpNotEnoughDiamond();
        }
    }
}
