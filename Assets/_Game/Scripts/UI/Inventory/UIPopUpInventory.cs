using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Sirenix.OdinInspector;
using NVTT;
using UnityEngine.Events;

public class UIPopUpInventory : MonoBehaviour
{
    public TextMeshProUGUI unlockCost;
    public Button unlockBtn;
    public Button equipBtn;

    public UnityEvent equipGun = new UnityEvent();
    [SerializeField] private RectTransform uiPopUp;

    private WeaponLocalData curWeaponData;
    private void OnValidate()
    {
        if (uiPopUp == null)
            uiPopUp = GetComponent<RectTransform>();
    }

    private void Start()
    {
        unlockBtn.onClick.AddListener(() => UnlockGun(curWeaponData));
        equipBtn.onClick.AddListener(() => EquipGun(curWeaponData));
    }

    private void OnEnable()
    {
        uiPopUp.anchoredPosition = new Vector2(0, -350f);
    }


    [Button]
    public void MoveIn(WeaponLocalData weaponLocalData)
    {
        curWeaponData = weaponLocalData;
        var state = curWeaponData.GetWeaponState();
        if (state == WeaponState.Undiscovered)
            return;

        uiPopUp.DOAnchorPosY(-350, 0.25f).SetUpdate(true).OnComplete(() =>
            {
                Display(curWeaponData, state);
                uiPopUp.DOAnchorPosY(0, 0.25f).SetUpdate(true);
            });
    }
    

    [Button]
    public void MoveOut()
    {
        if (uiPopUp.anchoredPosition.y <= -350f)
            return;
        uiPopUp.DOAnchorPosY(-350, 0.35f).SetUpdate(true);
    }

    private void Display(WeaponLocalData weaponLocalData, WeaponState state)
    {
        unlockBtn.gameObject.SetActive(state == WeaponState.Unlockable);
        equipBtn.gameObject.SetActive(state == WeaponState.Unlocked);
        
        unlockCost.text = weaponLocalData.unlockGoldCost.ToString();
    }

    private void UnlockGun(WeaponLocalData weaponLocalData)
    {
        if (PlayerSave.Gold >= weaponLocalData.unlockGoldCost)
        {
            PlayerSave.Gold -= weaponLocalData.unlockGoldCost;
            weaponLocalData.SetWeaponState(WeaponState.Unlocked);
            EquipGun(weaponLocalData);
        }
        else
        {
            unlockBtn.transform.NotInteractableResponse();
        }
    }

    private void EquipGun(WeaponLocalData weaponLocalData)
    {
        weaponLocalData.SetWeaponIsUsing();
        MoveOut();
        equipGun.Invoke();
    }
    
}
