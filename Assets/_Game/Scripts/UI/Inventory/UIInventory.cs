using System.Collections.Generic;
using ATSoft.Ads;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using static NVTT.Utilities;
public class UIInventory : MonoBehaviour
{
    #region Properties
    [Title("Weapons")]
    [SerializeField] private UIInventoryWeaponTab uiInventoryWeaponTab;
    [Title("Skins")]
    [SerializeField] private UIInventorySkinTab uiInventorySkinTab;
    
    [Title("Settings")]
    [SerializeField] private Button closeBtn;
    [SerializeField] private List<Toggle> tabToggles;
    [SerializeField] private List<GameObject> tab;
    
    private int lastTab;
    private int itemID;
    #endregion

    #region Init
    private void Awake()
    {
        uiInventoryWeaponTab.Init();
        uiInventorySkinTab.Init();
        //Settings
        for (var i = 0; i < tabToggles.Count; i++)
        {
            var i1 = i;
            tabToggles[i].onValueChanged.AddListener(b => OnTabValueChange(i1, b));
        }
        //closeBtn.onClick.AddListener(() => Advertisements.Instance.ShowInterstitial(CloseBtn, "Close_Shop"));
        closeBtn.onClick.AddListener(CloseBtn);
    }

    private void OnEnable()
    {
        GPExecutor.Instance.PauseGame();
        Refresh(lastTab);
    }

    private void OnTabValueChange(int tabID, bool status)
    {
        if (status)
        {
            Refresh(tabID);
        }
    }
    
    private void Refresh(int tabID)
    {
        for (var i = 0; i < tab.Count; i++)
        {
            tab[i].SetActive(i == tabID);
        }
        uiInventoryWeaponTab.uiStats.gameObject.SetActive(tabID == Inventory.Guns);
        
        switch (tabID)
        {
            case Inventory.Guns:
                uiInventoryWeaponTab.Refresh();
                break;
            case Inventory.Skins:
                uiInventorySkinTab.Refresh();
                break;
            case Inventory.Drone:
                break;
        }

        lastTab = tabID;
    }

    private void CloseBtn()
    {
        Player.weaponController.EquipWeapon(Player.CurrentWeapon);
        Player.skinController.EquipSkin();
        
        gameObject.SetActive(false);
        GPExecutor.Instance.ResumeGame();
    }
    
    #endregion

    #region Open Tab
    public void OpenSkinTab()
    {
        lastTab = Inventory.Skins;
        tabToggles[Inventory.Skins].isOn = true;
        gameObject.SetActive(true);
    }
    #endregion
}

#region Inventory
public static class Inventory
{
    public const int Guns = 0;
    public const int Skins = 1;
    public const int Drone = 2;
}
#endregion
