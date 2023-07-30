using System.Collections;
using System.Collections.Generic;
using ATSoft.Ads;
using UnityEngine;
using UnityEngine.UI;

namespace ATSoft
{
    public class ConsentDialog : BaseDialog
    {
        private static GameObject instance;

        private const string ACCEPT_CONSENT = "ACCEPT_CONSENT";
        private const string CAN_SHOW_CONSENT = "CAN_SHOW_CONSENT";

        [SerializeField] private Button buttonYes, buttonNo, buttonPrivacyPolicy;

        private static int AcceptConsent
        {
            get => PlayerPrefs.GetInt(ACCEPT_CONSENT, -1); //-1: Not show yet, 0: Deny, 1: Accepted
            set
            {
                PlayerPrefs.SetInt(ACCEPT_CONSENT, value);
                PlayerPrefs.Save();
            }
        }

        public static ConsentDialog Setup()
        {
            if (instance == null)
            {
                instance = Instantiate(Resources.Load(PathPrefabs.CONSENT_DIALOG) as GameObject);
            }
            instance.gameObject.SetActive(false);
            return instance.GetComponent<ConsentDialog>();
        }

        protected override void Start()
        {
            base.Start();
            buttonYes.onClick.AddListener(OnClickYes);
            buttonNo.onClick.AddListener(OnClickNo);
            buttonPrivacyPolicy.onClick.AddListener(OnClickPrivacyPolicy);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if(AppOpenAdManager.Instance != null) AppOpenAdManager.Instance.ResumeFromAds = true;
            Time.timeScale = 0;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if(AppOpenAdManager.Instance != null) AppOpenAdManager.Instance.ResumeFromAds = false;
        }

        private void OnClickYes()
        {
            AcceptConsent = 1;
            PlayerPrefs.SetInt(CAN_SHOW_CONSENT, 1);
            Time.timeScale = 1;
            Hide();
             Advertisements.Instance.ContinueConsentFlow();
        }

        private void OnClickNo()
        {
            AcceptConsent = 0;
            PlayerPrefs.SetInt(CAN_SHOW_CONSENT, 1);
            Time.timeScale = 1;
            Hide();
            Advertisements.Instance.ContinueConsentFlow();
        }

        private void OnClickPrivacyPolicy()
        {
            Application.OpenURL(!string.IsNullOrEmpty(SdkAutoInitializer.Instance.policyLink)
                ? SdkAutoInitializer.Instance.policyLink
                : "https://atsoft.io");
        }
    }
}
