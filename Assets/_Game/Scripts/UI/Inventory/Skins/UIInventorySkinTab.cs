using System.Collections.Generic;
using UnityEngine;
using static NVTT.Utilities;

public class UIInventorySkinTab : MonoBehaviour
{
    [SerializeField] private UIPreviewSkin uiPreviewSkin;
    [SerializeField] private UISkinEquip uiSkinEquip;
    [SerializeField] private List<UIInventorySkinBtn> skinItem;
    
    private static SkinData SkinIsUsing => GameDB.skinDatas[PlayerSave.GetSkinIsUsing()];
    private SkinData curSkinData;
    private void OnEnable()
    {
        curSkinData = SkinIsUsing;
        uiPreviewSkin.DisplaySkin(curSkinData, curSkinData.GetSkinState());
        uiSkinEquip.Display(curSkinData, SkinState.Unlocked);
    }
    
    public void Init()
    {
        SkinBtnAddEvent(skinItem, GameDB.skinDatas);
        uiSkinEquip.equipSkin.AddListener(() => EquipSkin(SkinIsUsing));
    }

    public void Refresh()
    {
        for (var i = 0; i < skinItem.Count; i++)
        {
            var hasData = i < GameDB.skinDatas.Count;
            if (hasData)
                skinItem[i].Display(GameDB.skinDatas[i]);
            skinItem[i].gameObject.SetActive(hasData);
        }
    }
    
    private void EquipSkin(SkinData sData)
    {
        uiPreviewSkin.DisplaySkin(sData, sData.GetSkinState());
        Refresh();
    }
    
    private void GetLastItemClick(SkinData sData)
    {
        var state = sData.GetSkinState();
        if (state == SkinState.Undiscovered) return;
        uiPreviewSkin.DisplaySkin(sData, state);
        uiSkinEquip.Display(sData, state);
    }
    
    private void SkinBtnAddEvent(IReadOnlyList<UIInventorySkinBtn> skins, IReadOnlyList<SkinData> skinsData)
    {
        for (var i = 0; i < skinsData.Count; i++)
        {
            var i1 = i;
            skins[i].btn.onClick.AddListener(() => GetLastItemClick(skinsData[i1]));
        }
    }
}
