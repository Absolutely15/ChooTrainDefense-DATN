using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class UIStats : MonoBehaviour
{
    [SerializeField] private StarDisplay starDisplay;
    [SerializeField] private TextMeshProUGUI nameLabel;
    [SerializeField] private TextMeshProUGUI damage;
    [SerializeField] private TextMeshProUGUI fireRate;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private TextMeshProUGUI damagePreview;
    [SerializeField] private TextMeshProUGUI fireRatePreview;
    [SerializeField] private TextMeshProUGUI descriptionPreview;
    
    [Button]
    public void Display(WeaponLocalData weaponLocalData)
    {
        var weaponLevel = weaponLocalData.GetWeaponLevel();
        starDisplay.GetActiveStar(weaponLevel);
        nameLabel.text = weaponLocalData.itemName;
        damage.text = $"{weaponLocalData.stats[weaponLevel].damage}";
        fireRate.text = $"{weaponLocalData.stats[weaponLevel].fireRate}";
        description.text = weaponLocalData.description + new string('+', weaponLevel);
        descriptionPreview.gameObject.SetActive(false);
        //zombie kill amount
    }

    public void SetPreview(float dmgAmount, float fireRateAmount)
    {
        damagePreview.text = $"{dmgAmount}";
        fireRatePreview.text = $"{fireRateAmount}";
        descriptionPreview.text = description.text + '+';
    }
}
