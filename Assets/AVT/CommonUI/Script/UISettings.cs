using UnityEngine;
using UnityEngine.UI;

public class UISettings : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private GameObject settingsPanel;

    [Header("Settings")]
    [SerializeField] private Button settingsButton;
    [SerializeField] private Toggle soundToggle;
    [SerializeField] private Toggle vibrationToggle;

    private void Awake()
    {
        settingsButton.onClick.AddListener(() => settingsPanel.SetActive(true));
        
        soundToggle.onValueChanged.AddListener(SetSoundToggle);
        vibrationToggle.onValueChanged.AddListener(SetVibrationToggle);

        soundToggle.isOn = PlayerPrefs.GetInt("Sound On", 1) == 1;
        vibrationToggle.isOn = PlayerPrefs.GetInt("Vibration On", 1) == 1;
    }

	private void Start()
	{
        // Init on Start
        SetSoundToggle(PlayerPrefs.GetInt("Sound On", 1) == 1);
        SetVibrationToggle(PlayerPrefs.GetInt("Vibration On", 1) == 1);
    }

    #region Settings Functions

    public void SetSoundToggle(bool value)
    {
        PlayerPrefs.SetInt("Sound On", value ? 1 : 0);
        AudioListener.volume = value ? 1 : 0;
    }

    public void SetVibrationToggle(bool value)
    {
        PlayerPrefs.SetInt("Vibration On", value ? 1 : 0);
#if MOREMOUNTAINS_NICEVIBRATIONS
        MoreMountains.NiceVibrations.MMVibrationManager.SetHapticsActive(value);
#endif
    }

    #endregion
}
