using ATSoft;
using NVTT;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UISkinEquip : MonoBehaviour
{
    public UnityEvent equipSkin = new UnityEvent();
    
    [SerializeField] private Button unlockBtn;
    [SerializeField] private Button equipBtn;
    [Space]
    [SerializeField] private Image equipImage;
    [Space]
    [SerializeField] private TextMeshProUGUI equipText;
    [SerializeField] private TextMeshProUGUI unlockCost;
    [Space]
    [SerializeField] private Sprite[] sprite;
    
    private SkinData thisSkinData;

    private void Start()
    {
        unlockBtn.onClick.AddListener(() => UnlockSkin(thisSkinData));
        equipBtn.onClick.AddListener(() => EquipSkin(thisSkinData));
    }

    public void Display(SkinData sData, SkinState state)
    {
        thisSkinData = sData;
        unlockBtn.gameObject.SetActive(state == SkinState.Unlockable);
        equipBtn.gameObject.SetActive(state == SkinState.Unlocked);
        
        if (state == SkinState.Unlockable)
            unlockCost.text = sData.unlockDiamondCost.ToString();

        if (state != SkinState.Unlocked) return;
        if (sData.id == PlayerSave.GetSkinIsUsing())
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
    private void UnlockSkin(SkinData sData)
    {
        if (PlayerSave.Diamond >= sData.unlockDiamondCost)
        {
            PlayerSave.Diamond -= sData.unlockDiamondCost;
            sData.SetSkinState(SkinState.Unlocked);
            EquipSkin(sData);

            //Ads
            AnalyticsManager.LogEventDiamondSpend(sData.unlockDiamondCost, "unlock_skin", $"{sData.itemName}");
        }
        else
        {
            unlockBtn.transform.NotInteractableResponse();
            AudioManager.Instance.PlayAudio(AudioType.Click, true);
            
            //Ads
            UIAdsManager.Instance.PopUpNotEnoughDiamond();
        }
    }

    private void EquipSkin(SkinData sData)
    {
        sData.SetSkinIsUsing();
        Display(sData, sData.GetSkinState());
        AudioManager.Instance.PlayAudio(AudioType.Equip, true);
        equipSkin.Invoke();
    }
}
