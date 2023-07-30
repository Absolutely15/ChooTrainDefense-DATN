using TMPro;
using UnityEngine;

public class CurrencyChest : MonoBehaviour, IChest
{
    [SerializeField] private int goldRewardAmount;
    [SerializeField] private int diamondRewardAmount;
    [Space]
    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private TextMeshProUGUI diamondText;

    public ChestType ChestType { get; set; }

    public void Init()
    {
        ChestType = ChestType.CurrencyChest;
        goldText.text = $"{goldRewardAmount}";
        diamondText.text = $"{diamondRewardAmount}";
    }

    public void Open()
    {
        PlayerSave.Gold += goldRewardAmount;
        PlayerSave.Diamond += diamondRewardAmount;
    }

    public void Claim()
    {
        
    }
}
