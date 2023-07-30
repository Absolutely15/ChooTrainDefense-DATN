using UnityEngine;

public class TrapUI : MonoBehaviour
{
    [SerializeField] private GameObject notUnlockedUI;
    [SerializeField] private GameObject unlockedUI;

    public void Init(bool isUnlocked)
    {
        notUnlockedUI.gameObject.SetActive(!isUnlocked);
        unlockedUI.gameObject.SetActive(isUnlocked);
    }
}
