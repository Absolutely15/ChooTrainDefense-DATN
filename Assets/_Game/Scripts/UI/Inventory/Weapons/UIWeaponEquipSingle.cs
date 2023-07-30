using NVTT;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIWeaponEquipSingle : MonoBehaviour
{
    public Button unlockBtn;
    public Button equipBtn;
    [Space]
    public Image equipImage;
    [Space]
    public TextMeshProUGUI equipText;
    public TextMeshProUGUI unlockCost;
    [Space]
    public Sprite[] sprite;
    
    public UnityEvent equipGun = new UnityEvent();

    private WeaponLocalData thisWeaponData;

    private void Start()
    {
        unlockBtn.onClick.AddListener(() => UnlockGun(thisWeaponData));
        equipBtn.onClick.AddListener(() => EquipGun(thisWeaponData));
    }

    public void Display(WeaponLocalData wData, WeaponState state)
    {
        thisWeaponData = wData;
        unlockBtn.gameObject.SetActive(state == WeaponState.Unlockable);
        equipBtn.gameObject.SetActive(state == WeaponState.Unlocked);
        
        if (state == WeaponState.Unlockable)
            unlockCost.text = wData.unlockDiamondCost.ToString();

        if (state != WeaponState.Unlocked) return;
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
            AudioManager.Instance.PlayAudio(AudioType.Equip, true);
        }
        else
        {
            unlockBtn.transform.NotInteractableResponse();
            AudioManager.Instance.PlayAudio(AudioType.Click, true);
        }
    }
    
    private void EquipGun(WeaponLocalData weaponLocalData)
    {
        weaponLocalData.SetWeaponIsUsing();
        equipGun.Invoke();
    }

}
