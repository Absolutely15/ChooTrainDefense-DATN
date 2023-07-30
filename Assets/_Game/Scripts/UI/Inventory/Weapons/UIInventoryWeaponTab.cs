using System.Collections.Generic;
using UnityEngine;
using static NVTT.Utilities;

public class UIInventoryWeaponTab : MonoBehaviour
{
    public UIStats uiStats;
    [SerializeField] private UIPreviewWeapon uiPreviewWeapon;
    [SerializeField] private UIWeaponEquipShared uiWeaponEquip;
    [SerializeField] private List<UIInventoryWeaponBtn> weaponItemPremium;
    [SerializeField] private List<UIInventoryWeaponBtn> weaponItemNormal;

    private List<WeaponLocalData> premiumWeapons = new List<WeaponLocalData>();
    private List<WeaponLocalData> normalWeapons = new List<WeaponLocalData>();

    private WeaponLocalData curWeaponData;
    private static WeaponLocalData WeaponIsUsing => GameDB.weaponList[PlayerSave.GetWeaponIsUsing()];
    private void OnEnable()
    {
        curWeaponData = WeaponIsUsing;
        
        uiStats.Display(curWeaponData);
        uiPreviewWeapon.DisplayGun(curWeaponData);
        uiWeaponEquip.Display(curWeaponData, curWeaponData.GetWeaponState());
    }

    public void Init()
    {
        //Weapons
        premiumWeapons = GameDB.GetWeaponDataListWithFilter(true);
        normalWeapons = GameDB.GetWeaponDataListWithFilter(default, true);
        
        WeaponBtnAddEvent(weaponItemPremium, premiumWeapons);
        WeaponBtnAddEvent(weaponItemNormal, normalWeapons);
        
        uiWeaponEquip.equipGun.AddListener(() => RefreshUI(WeaponIsUsing));
        uiWeaponEquip.goldenGun.AddListener(() => RefreshUI(WeaponIsUsing));
    }
    
    public void Refresh()
    {
        WeaponDisplay(weaponItemPremium, premiumWeapons, true);
        WeaponDisplay(weaponItemNormal, normalWeapons, false);
    }

    private static void WeaponDisplay(IReadOnlyList<UIInventoryWeaponBtn> weapons, IReadOnlyList<WeaponLocalData> weaponsData, bool isPremium)
    {
        for (var i = 0; i < weapons.Count; i++)
        {
            var hasData = i < weaponsData.Count;
            if (hasData)
                weapons[i].Display(weaponsData[i], isPremium);
            weapons[i].gameObject.SetActive(hasData);
        }
    }
    
    private void RefreshUI(WeaponLocalData wData)
    {
        uiStats.Display(wData);
        uiPreviewWeapon.DisplayGun(wData);
        Refresh();
    }

    private void GetLastItemClick(WeaponLocalData wData)
    {
        var state = wData.GetWeaponState();
        if (state == WeaponState.Undiscovered) return;
        uiStats.Display(wData);
        uiPreviewWeapon.DisplayGun(wData);
        uiWeaponEquip.Display(wData, state);
    }
    
    private void WeaponBtnAddEvent(IReadOnlyList<UIInventoryWeaponBtn> weapons, IReadOnlyList<WeaponLocalData> weaponsData)
    {
        for (var i = 0; i < weaponsData.Count; i++)
        {
            var i1 = i;
            weapons[i].btn.onClick.AddListener(() => GetLastItemClick(weaponsData[i1]));
            // weapons[i].uiWeaponEquip.equipGun.AddListener(() => EquipWeapon(weaponsData[i1]));
        }
    }
}
