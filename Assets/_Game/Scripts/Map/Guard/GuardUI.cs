using NVTT;
using TMPro;
using UnityEngine;

public class GuardUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textLevel;
    [SerializeField] private GameObject notUnlockedUI;
    [SerializeField] private GameObject unlockedUI;
    [Space]
    [SerializeField] private UpgradeGuard upgradeGuard;
    [SerializeField] private UpgradeInMap upgradeInMap;


    public void Init(int level, bool isUnlocked)
    {
        //UI
        if (level == Utilities.MapManager.mapLimitation.guardLevel)
        {
            notUnlockedUI.gameObject.SetActive(false);
            unlockedUI.gameObject.SetActive(false);
        }
        else
        {
            notUnlockedUI.gameObject.SetActive(!isUnlocked);
            unlockedUI.gameObject.SetActive(isUnlocked);
        }

        textLevel.text = level.ToString();
    }

    public void UnlockGuard(int level)
    {
        textLevel.text = level.ToString();
        unlockedUI.gameObject.SetActive(true);
    }

    public void SetLevelText(int level)
    {
        textLevel.text = level.ToString();
    }
    public void MaxLevelFeedback()
    {
        if (!upgradeGuard.IsGuardMaxLevel()) return;
        upgradeInMap.groupBtn.OnClose();
        upgradeInMap.interactableArea.OnClose();
        
        if (Utilities.Gameplay.IsInGameplay) return;
        UIManager.Instance.SetPlayAndRebuildBtnActive();
    }
}
