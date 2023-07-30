using NVTT;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class UIInventorySkinBtn : UIInventoryItem<SkinData>
{
    [Title("Background")] 
    [SerializeField] private Image frameBG;
    [SerializeField] private Image frameEquip;

    [Title("Undiscovered")]
    [SerializeField] private Image undiscovered;
    
    [Title("Unlockable")]
    [SerializeField] private GameObject unlockable;
    [SerializeField] private Image unlockableIcon;
    
    [Title("Unlocked")]
    [SerializeField] private Image unlocked;

    public override void Display(SkinData skinData)
    {
        base.Display(skinData);
        var state = skinData.GetSkinState();
        var isUsing = PlayerSave.GetSkinIsUsing();
        if (skinData.isDiscoveredInWorld <= Utilities.MapManager.id  && state == SkinState.Undiscovered)
        {
            state = SkinState.Unlockable;
            skinData.SetSkinState(state);
        }
        
        //Undiscovered
        undiscovered.gameObject.SetActive(state == SkinState.Undiscovered);
        
        //Unlockable
        unlockableIcon.sprite = skinData.icon;
        unlockable.SetActive(state == SkinState.Unlockable);
        
        //Unlocked
        unlocked.sprite = skinData.icon;
        frameEquip.gameObject.SetActive(isUsing == skinData.id);
        unlocked.gameObject.SetActive(state == SkinState.Unlocked);
    }

    public override void OnClick()
    {
        switch (data.GetSkinState())
        {
            case SkinState.Undiscovered:
                btn.transform.NotInteractableResponse();
                break;
            case SkinState.Unlockable:
                break;
            case SkinState.Unlocked:
                break;
        }
    }
}
