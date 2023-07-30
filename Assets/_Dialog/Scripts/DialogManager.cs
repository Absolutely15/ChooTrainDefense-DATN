using Sirenix.OdinInspector;
using UnityBase.DesignPattern;

namespace ATSoft
{
    public class DialogManager : Singleton<DialogManager>
    {
        /*RateGame*/
        public RateGameDialog RateGameDialog => RateGameDialog.Setup();

        private void Start()
        {
            Observer.Instance.AddObserver(EventID.EndGameLevelData, ObserverOpenRateGame);
        }

        private void ObserverOpenRateGame()
        {
            if (!FirebaseRemoteConfigManager.levelRateGamePopUp.Contains(PlayerSave.CurrentGameLevel)) return;
            OpenRateGame();
        }

        [Button]
        public void OpenRateGame()
        {
            var rateGameDialog = DialogManager.Instance.RateGameDialog;
            rateGameDialog.Show();
            GPExecutor.Instance.PauseGame();
        }
        
        /*SettingsGame*/
        public SettingsDialog SettingsDialog => SettingsDialog.Setup();
        
        [Button]
        public void OpenSettingsGame()
        {
            var settingDialog = DialogManager.Instance.SettingsDialog;
            settingDialog.Show();
        }
        
        /*Consent flow*/
        public ConsentDialog ConsentDialog => ConsentDialog.Setup();
        
        [Button]
        public void OpenConsentFlow()
        {
            var consentDialog = DialogManager.Instance.ConsentDialog;
            consentDialog.Show();
        }
    }
    
    public class PathPrefabs
    {
        public const string CONFIRM_DIALOG = "UI/ConfirmDialog";
        public const string SETTING_DIALOG = "UI/SettingDialog";
        public const string RATE_GAME_DIALOG = "UI/RateGameDialog";
        public const string CONSENT_DIALOG = "UI/ConsentDialog";
        public const string WAITING_DIALOG = "UI/WaitingDialog";
    }
}