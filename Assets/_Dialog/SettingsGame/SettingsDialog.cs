using UnityEngine;
using UnityEngine.UI;

namespace ATSoft
{
    public class SettingsDialog : BaseDialog
    {
        private static GameObject instance;
        private const string SOUND_ON = "SOUND_ON";
        private const string VIBRATION_ON = "VIBRATION_ON";

        [SerializeField] private Toggle soundToggle;
        [SerializeField] private Toggle vibrationToggle;
        [SerializeField] GameObject soundToggleOffGraphic;
        [SerializeField] GameObject soundToggleOnGraphic;
        [SerializeField] GameObject vibrationToggleOffGraphic;
        [SerializeField] GameObject vibrationToggleOnGraphic;

        private static bool SoundOn
        {
            get => PlayerPrefs.GetInt(SOUND_ON, 1) == 1;
            set
            {
                PlayerPrefs.SetInt(SOUND_ON, value ? 1 : 0);
                PlayerPrefs.Save();
            }
        }

        private static bool VibrationOn
        {
            get => PlayerPrefs.GetInt(VIBRATION_ON, 1) == 1;
            set
            {
                PlayerPrefs.SetInt(VIBRATION_ON, value ? 1 : 0);
                PlayerPrefs.Save();
            }
        }

        public static SettingsDialog Setup()
        {
            if (instance == null)
            {
                instance = Instantiate(Resources.Load(PathPrefabs.SETTING_DIALOG) as GameObject);
            }

            return instance.GetComponent<SettingsDialog>();
        }

        protected override void Start()
        {
            base.Start();
            soundToggle.onValueChanged.AddListener(SetSoundToggle);
            vibrationToggle.onValueChanged.AddListener(SetVibrationToggle);
        }

        protected override void OnStart()
        {
            base.OnStart();
            SetSoundToggle(SoundOn);
            SetVibrationToggle(VibrationOn);
        }

        private void SetSoundToggle(bool value)
        {
            SoundOn = value;
            soundToggleOffGraphic.SetActive(!SoundOn);
            soundToggleOnGraphic.SetActive(SoundOn);
            AudioListener.volume = value ? 1 : 0;
        }

        private void SetVibrationToggle(bool value)
        {
            VibrationOn = value;
            vibrationToggleOffGraphic.SetActive(!VibrationOn);
            vibrationToggleOnGraphic.SetActive(VibrationOn);
#if MOREMOUNTAINS_NICEVIBRATIONS
            MoreMountains.NiceVibrations.MMVibrationManager.SetHapticsActive(value);
#endif
        }
    }
}