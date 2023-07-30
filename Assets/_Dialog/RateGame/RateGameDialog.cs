using System;
using System.Collections;
using System.Collections.Generic;
#if USE_IN_APP_REVIEW
using Google.Play.Review;
#endif
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace ATSoft
{
    public class RateGameDialog : BaseDialog
    {
        private static GameObject instance;
        private const string CAN_SHOW_RATE = "CAN_SHOW_RATE";
        private readonly CompositeDisposable _disposes = new CompositeDisposable();
        private List<IObservable<int>> obsList;
        private bool canRateStore;
        
        [SerializeField] private Button[] starButton;
        [SerializeField] private Sprite yellowStar;
        [SerializeField] private Sprite grayStar;

        private static bool CanShowRate
        {
            get => PlayerPrefs.GetInt(CAN_SHOW_RATE, 0) == 0;
            set
            {
                PlayerPrefs.SetInt(CAN_SHOW_RATE, value ? 0 : 1);
                PlayerPrefs.Save();
            }
        }
        
        public static RateGameDialog Setup()
        {
            if (instance == null)
            {
                instance = Instantiate(Resources.Load(PathPrefabs.RATE_GAME_DIALOG) as GameObject);
            }
            instance.gameObject.SetActive(false);
            return instance.GetComponent<RateGameDialog>();
        }

        protected override void Start()
        {
            base.Start();
            obsList = new List<IObservable<int>>();
            for (int i = 0; i < starButton.Length; i++)
            {
                var index = i;
                var starObservable = starButton[i]
                    .OnClickAsObservable()
                    .Select(_ => index);
                obsList.Add(starObservable);
            }

            obsList.Merge()
                .Do(UpdateView)
                .Subscribe(OnCheckStarNumber)
                .AddTo(_disposes);
        }

        protected override void OnStart()
        {
            base.OnStart();
            for (int i = 0; i < starButton.Length; i++)
            {
                starButton[i].image.sprite = yellowStar;
            }

            canRateStore = true;
        }

        protected override void OnDestroy()
        {
            _disposes.Dispose();
        }

        public override void Show()
        {
            if (!CanShowRate)
            {
                Debug.Log("=== CanShowRate: false ===");
                return;
            }
            base.Show();
        }

        public void OnClickRate()
        {
            if (!canRateStore)
            {
                Hide();
                GPExecutor.Instance.ResumeGame();
                return;
            }

#if USE_IN_APP_REVIEW && !UNITY_IOS
            if (!SdkAutoInitializer.Instance.enableInAppReview)
            {
#if UNITY_EDITOR
                var OPEN_LINK_RATE = "https://play.google.com/store/apps/details?id=" + SdkAutoInitializer.Instance.packageName;
#else
                var OPEN_LINK_RATE = "market://details?id=" + SdkAutoInitializer.Instance.packageName;
#endif
                Application.OpenURL(OPEN_LINK_RATE);
                CanShowRate = false;
                Hide();
            }
            else
            {
                StartCoroutine(RequestInAppReview());
            }
#else
#if UNITY_EDITOR
            var OPEN_LINK_RATE = "https://play.google.com/store/apps/details?id=" + SdkAutoInitializer.Instance.packageName;
#elif UNITY_ANDROID
            var OPEN_LINK_RATE = "market://details?id=" + SdkAutoInitializer.Instance.packageName;
#else
            var OPEN_LINK_RATE = "itms-apps://itunes.apple.com/app/id" + SdkAutoInitializer.Instance.appleAppId;
#endif
            Application.OpenURL(OPEN_LINK_RATE);
            CanShowRate = false;
            Hide();
            GPExecutor.Instance.ResumeGame();
#endif
        }

        public void OnClickMaybeLater()
        {
            Hide();
            GPExecutor.Instance.ResumeGame();
        }

        private void UpdateView(int starIndex)
        {
            for (int i = 0; i < starButton.Length; i++)
                starButton[i].image.sprite = i <= starIndex ? yellowStar : grayStar;
        }

        private void OnCheckStarNumber(int starIndex)
        {
            var starRequirement = starButton.Length - 2;
            canRateStore = starIndex >= starRequirement;
        }
        
#if USE_IN_APP_REVIEW
        #region In-App Review

        private ReviewManager reviewManager;
        private PlayReviewInfo playReviewInfo;
        private IEnumerator RequestInAppReview()
        {
#if UNITY_EDITOR
            var OPEN_LINK_RATE = "https://play.google.com/store/apps/details?id=" + SdkAutoInitializer.Instance.packageName;
#elif UNITY_ANDROID
            var OPEN_LINK_RATE = "market://details?id=" + SdkAutoInitializer.Instance.packageName;
#endif
            AppOpenAdManager.Instance.ResumeFromAds = true;
            reviewManager = new ReviewManager();
            var requestFlowOperation = reviewManager.RequestReviewFlow();
            yield return requestFlowOperation;
            if (requestFlowOperation.Error != ReviewErrorCode.NoError)
            {
                // Log error. For example, using requestFlowOperation.Error.ToString().
                Debug.LogError(requestFlowOperation.Error);
                Hide();
                AppOpenAdManager.Instance.ResumeFromAds = false;
                Application.OpenURL(OPEN_LINK_RATE);
                yield break;
            }
            playReviewInfo = requestFlowOperation.GetResult();
            var launchFlowOperation = reviewManager.LaunchReviewFlow(playReviewInfo);
            yield return launchFlowOperation;
            playReviewInfo = null; // Reset the object
            if (launchFlowOperation.Error != ReviewErrorCode.NoError)
            {
                // Log error. For example, using requestFlowOperation.Error.ToString().
                Debug.LogError(launchFlowOperation.Error);
                Hide();
                AppOpenAdManager.Instance.ResumeFromAds = false;
                Application.OpenURL(OPEN_LINK_RATE);
                yield break;
            }
            // The flow has finished. The API does not indicate whether the user
            // reviewed or not, or even whether the review dialog was shown. Thus, no
            // matter the result, we continue our app flow.
            CanShowRate = false;
            Hide();
            
            yield return new WaitForSeconds(2f);
            AppOpenAdManager.Instance.ResumeFromAds = false;
        }
        
        #endregion
#endif
    }
}