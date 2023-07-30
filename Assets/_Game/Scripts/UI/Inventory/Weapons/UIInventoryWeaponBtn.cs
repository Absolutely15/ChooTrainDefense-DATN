using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using NVTT;

public class UIInventoryWeaponBtn : UIInventoryItem<WeaponLocalData>
{
    [Title("Background")]
    [SerializeField] private Image frameBG;
    [SerializeField] private Image frameEquip;
    
    [Title("Undiscovered")]
    [SerializeField] private Image undiscovered;
    
    [Title("Unlockable")]
    [SerializeField] private GameObject unlockable;
    [SerializeField] private GameObject lockIcon;
    [SerializeField] private Image unlockableIcon;
    
    [Title("Unlocked")]
    [SerializeField] private StarDisplay starDisplay;
    [SerializeField] private Image unlocked;

    [Title("Golden")]
    [SerializeField] private Image golden;

    // [Space]
    // public UIWeaponEquip uiWeaponEquip;
    
    public override void Display(WeaponLocalData weaponLocalData, bool isPremium)
    {
        base.Display(weaponLocalData);
        
        var state = weaponLocalData.GetWeaponState();
        // var weaponLevel = weaponLocalData.GetWeaponLevel();
        var isUsing = PlayerSave.GetWeaponIsUsing();
        if (weaponLocalData.isDiscoveredInWorld <= Utilities.MapManager.id  && state == WeaponState.Undiscovered)
        {
            state = WeaponState.Unlockable;
            weaponLocalData.SetWeaponState(state);
        }
        
        // uiWeaponEquip.Display(weaponLocalData, state);
        
        if (isPremium && state == WeaponState.Unlockable)
        {
            unlockableIcon.sprite = weaponLocalData.icon;
            unlockableIcon.color = Color.white;
            lockIcon.SetActive(false);
            unlockable.SetActive(true);
            
            // starDisplay.GetActiveStar(weaponLevel);
            // starDisplay.gameObject.SetActive(state == WeaponState.Unlocked);
            frameEquip.gameObject.SetActive(isUsing == weaponLocalData.id);
        }
        else
        {
            //Undiscovered
            undiscovered.gameObject.SetActive(state == WeaponState.Undiscovered);
        
            //Unlockable
            unlockableIcon.sprite = weaponLocalData.icon;
            unlockable.SetActive(state == WeaponState.Unlockable);
        
            //Unlocked
            unlocked.sprite = weaponLocalData.icon;
            // starDisplay.GetActiveStar(weaponLevel);
            // starDisplay.gameObject.SetActive(state == WeaponState.Unlocked);
            frameEquip.gameObject.SetActive(isUsing == weaponLocalData.id);
            unlocked.gameObject.SetActive(state == WeaponState.Unlocked);
        
            //Golden
             golden.sprite = weaponLocalData.goldenIcon;
             golden.gameObject.SetActive(state == WeaponState.Golden);
        }
    }

    public override void OnClick()
    {
        switch (data.GetWeaponState())
        {
            case WeaponState.Undiscovered:
                //Pop up notification
                btn.transform.NotInteractableResponse();
                break;
            case WeaponState.Unlockable:
                //Pop up panel Unlock
                break;
            case WeaponState.Unlocked:
                //Pop up equip, upgrade, golden
                break;
            case WeaponState.Golden:
                //Pop up equip, upgrade
                break;
        }
    }
    
}
