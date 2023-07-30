using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AppsFlyerSDK;
using UnityEngine;
using UnityEngine.Events;
using UniRx;


//  ---------------------------------------------
//  Author:     DatDz <steven@atsoft.io> 
//  Copyright (c) 2022 AT Soft
// ----------------------------------------------

#if AD_TYPE_IS
namespace ATSoft.Ads
{
    public class CustomIronSource : BaseAds, ICustomAds
    {
        private bool bannerLoaded;
        
        private const float reloadTime = 2;
        private bool rewardedVideoCompleted;
        private bool directedForChildren = true;
        private bool enableDebugger;

        private string appKey;

#if UNITY_ANDROID
        private string ID_TEST_APP = "1764abf15";
#elif UNITY_IOS
        private string ID_TEST_APP = "17455d1ed";
#else
        private string ID_TEST_APP = "unexpected_platform";
#endif

        /// <summary>
        /// Initializing IronSource
        /// </summary>
        /// <param name="platformSettings">contains all required settings for this publisher</param>
        public void InitializeAds(List<AdvertiserSettings> platformSettings, bool isTestAd, bool enableDebugger, bool enableConsent)
        {
            if (initialized == false)
            {
                Debug.Log("IronSource Start Initialization");
                
                //get settings
#if UNITY_ANDROID
                AdvertiserSettings settings =
                    platformSettings.First(cond => cond.platform == SupportedPlatforms.Android);
#elif UNITY_IOS
                AdvertiserSettings settings = platformSettings.First(cond => cond.platform == SupportedPlatforms.IOS);
#else
                AdvertiserSettings settings = new AdvertiserSettings(SupportedPlatforms.Windows,"", "", "","");
#endif
                //apply settings
                appKey = !isTestAd ? settings.appId : ID_TEST_APP;
                this.enableConsent = enableConsent;

                if (enableConsent)
                {
                    var acceptConsent = PlayerPrefs.GetInt("ACCEPT_CONSENT", -1);
                    if (acceptConsent >= 0)
                    {
                        StartCoroutine(WaitingConsent(acceptConsent >= 1));
                    }
                    else
                    {
                        var consentDialog = DialogManager.Instance.ConsentDialog;
                        consentDialog.Show();
                    }
                }
                else
                {
                    StartCoroutine(WaitingConsent());
                }
                
            }
        }

        public void ContinueConsentFlow()
        {
            var acceptConsent = PlayerPrefs.GetInt("ACCEPT_CONSENT", -1);
            StartCoroutine(WaitingConsent(acceptConsent >= 1));
        }

        IEnumerator WaitingConsent(bool acceptConsent = true)
        {
            yield return new WaitForSeconds(1f);
            
            //Dynamic config example
            IronSourceConfig.Instance.setClientSideCallbacks(true);
            IronSource.Agent.shouldTrackNetworkState (true);

            //Debug validate
            //https://developers.is.com/ironsource-mobile/unity/integration-helper-unity/
            if (enableDebugger)
            {
                //verify settings
                Debug.Log("IronSource plugin Version: " + IronSource.pluginVersion());
                string id = IronSource.Agent.getAdvertiserId();
                Debug.Log("IronSource GetAdvertiserId : " + id);
                
                IronSource.Agent.validateIntegration();
            }

            if (enableConsent)
            {
                //Update consent
                // IronSource.Agent.setMetaData("is_child_directed", acceptConsent ? "true" : "false");
                // IronSource.Agent.setMetaData("do_not_sell", acceptConsent ? "true" : "false"); //Set up on dashboard
                IronSource.Agent.setConsent(acceptConsent);
                Debug.Log("IronSource -> appKey: " + appKey + " - acceptConsent: " + acceptConsent);
            }

            //Add Init Event
            IronSourceEvents.onSdkInitializationCompletedEvent += SdkInitializationCompletedEvent;
            
            IronSource.Agent.init(appKey);
            
            //IronSource Ad Quality
            // AdQualitySdkInit adQualitySdkInit = new AdQualitySdkInit();
            // ISAdQualityConfig adQualityConfig = new ISAdQualityConfig {
            //     AdQualityInitCallback = adQualitySdkInit,
            // };
            // //adQualityConfig.UserId = userId;
            // // The default user id is Ad Quality internal id.
            // // The only allowed characters for user id are: letters, numbers, @, -, :, =,_ and /.
            // // The user id cannot be null and must be between 2 and 100 characters, otherwise it will be blocked.
            //
            // //adQualityConfig.TestMode = true;
            // // The default is false - set to true only to test your Ad Quality integration
            //
            // //adQualityConfig.LogLevel = ISAdQualityLogLevel.INFO;
            // // There are 5 different log levels:
            // // ERROR, WARNING, INFO, DEBUG, VERBOSE
            // // The default is INFO
            // IronSourceAdQuality.Initialize(appKey, adQualityConfig);
        }

        #region Init callback handlers

        void SdkInitializationCompletedEvent()
        {
            Debug.Log("unity-script: I got SdkInitializationCompletedEvent");
            
            //Add ImpressionSuccess Event
            IronSourceEvents.onImpressionDataReadyEvent += ImpressionDataReadyEvent;
            
            //Add Banner Event
            IronSourceBannerEvents.onAdLoadedEvent += BannerAdLoadedEvent;
            IronSourceBannerEvents.onAdLoadFailedEvent += BannerAdLoadFailedEvent;
            IronSourceBannerEvents.onAdClickedEvent += BannerAdClickedEvent;
            // IronSourceEvents.onBannerAdLoadedEvent += BannerAdLoadedEvent;
            // IronSourceEvents.onBannerAdLoadFailedEvent += BannerAdLoadFailedEvent;
            // IronSourceEvents.onBannerAdClickedEvent += BannerAdClickedEvent;
            
            if (Advertisements.Instance.CanShowAds())
            {
                ShowBanner();
                InitInterstitial();
            }

            InitRewardVideo();
            initialized = true;
            Debug.Log(this + " Init Complete: ");
        }

        #endregion
        
        #region ImpressionSuccess callback handler

        void ImpressionDataReadyEvent(IronSourceImpressionData impressionData)
        {
            Debug.Log("unity - script: I got ImpressionSuccessEvent ToString(): " + impressionData.ToString());
            Debug.Log("unity - script: I got ImpressionSuccessEvent allData: " + impressionData.allData);
            
            // Ad revenue paid. Use this callback to track user revenue.
            Debug.Log("Ad revenue paid");

            // Ad revenue
            double revenue = (double)impressionData.revenue;

            // Miscellaneous data
            string countryCode = impressionData.country; // "US" for the United States, etc - Note: Do not confuse this with currency code which is "USD" in most cases!
            string networkName = impressionData.adNetwork; // Display name of the network that showed the ad (e.g. "AdColony")
            string adUnitIdentifier = impressionData.auctionId; // The MAX Ad Unit ID
            string placement = impressionData.placement; // The placement this ad's postbacks are tied to

            var data = new ImpressionData
            {
                AdFormat = impressionData.adUnit,
                AdUnitIdentifier = adUnitIdentifier,
                CountryCode = countryCode,
                NetworkName = networkName,
                Placement = placement,
                Revenue = revenue
            };

            SendEventImpression(data);
            
            Dictionary<string, string> dic = new Dictionary<string, string>
            {
                { "ad_platform", "IronSource" },
                { "ad_source", networkName },
                { "ad_unit_name", adUnitIdentifier },
                { "currency", "USD" },
                { "value", $"{revenue}" },
                { "placement", placement },
                { "country_code", countryCode },
                { "ad_format", impressionData.adUnit }
            };
            AppsFlyerAdRevenue.logAdRevenue("IronSource",AppsFlyerAdRevenueMediationNetworkType.AppsFlyerAdRevenueMediationNetworkTypeIronSource, revenue, "USD", dic);
        }

        private void SendEventImpression(ImpressionData data)
        {
            Firebase.Analytics.Parameter[] AdParameters =
            {
                new Firebase.Analytics.Parameter("ad_platform", "IronSource"),
                new Firebase.Analytics.Parameter("ad_source", data.NetworkName),
                new Firebase.Analytics.Parameter("ad_unit_name", data.AdUnitIdentifier),
                new Firebase.Analytics.Parameter("currency", "USD"),
                new Firebase.Analytics.Parameter("value", data.Revenue),
                new Firebase.Analytics.Parameter("placement", data.Placement),
                new Firebase.Analytics.Parameter("country_code", data.CountryCode),
                new Firebase.Analytics.Parameter("ad_format", data.AdFormat),
            };

            Firebase.Analytics.FirebaseAnalytics.LogEvent(FirebaseStringEvent.ad_impression_ironsource, AdParameters);
            
            // Dictionary<string, string> paramas = new Dictionary<string, string>
            // {
            //     { AFInAppEvents.QUANTITY, "1" },
            //     { "rocket", "1" },
            //     { AFInAppEvents.REVENUE, data.Revenue.ToString("N5") }
            // };
            // AppsFlyer.sendEvent(FirebaseStringEvent.ad_impression_ironsource, paramas);
        }

        #endregion

        #region === INTERSTITIAL ===

        /// <summary>
        /// Check if IronSource interstitial is available
        /// </summary>
        /// <returns>true if an interstitial is available</returns>
        public bool IsInterstitialAvailable()
        {
            return initialized && IronSource.Agent.isInterstitialReady() && interstitialReadyTime <= Time.time;
        }
        
        private bool isInitedInterstitial;

        private void InitInterstitial()
        {
            if (!Advertisements.Instance.CanShowAds()) return;
            if (isInitedInterstitial) return;
            
            isInitedInterstitial = true;
            IronSourceInterstitialEvents.onAdReadyEvent += InterstitialAdReadyEvent;
            IronSourceInterstitialEvents.onAdLoadFailedEvent += InterstitialAdLoadFailedEvent;
            IronSourceInterstitialEvents.onAdShowSucceededEvent += InterstitialAdShowSucceededEvent;
            IronSourceInterstitialEvents.onAdShowFailedEvent += InterstitialAdShowFailedEvent;
            IronSourceInterstitialEvents.onAdClickedEvent += InterstitialAdClickedEvent;
            IronSourceInterstitialEvents.onAdClosedEvent += InterstitialAdClosedEvent;
            
            // IronSourceEvents.onInterstitialAdReadyEvent += InterstitialAdReadyEvent;
            // IronSourceEvents.onInterstitialAdLoadFailedEvent += InterstitialAdLoadFailedEvent;
            // IronSourceEvents.onInterstitialAdShowSucceededEvent += InterstitialAdShowSucceededEvent;
            // IronSourceEvents.onInterstitialAdShowFailedEvent += InterstitialAdShowFailedEvent;
            // IronSourceEvents.onInterstitialAdClickedEvent += InterstitialAdClickedEvent;
            // //IronSourceEvents.onInterstitialAdOpenedEvent += InterstitialAdOpenedEvent;
            // IronSourceEvents.onInterstitialAdClosedEvent += InterstitialAdClosedEvent;
            LoadInterstitial();
        }
        
        /// <summary>
        /// Loads IronSource interstitial
        /// </summary>
        private void LoadInterstitial()
        {
            Debug.Log("IronSource Start Loading Interstitial");

            IronSource.Agent.loadInterstitial();
        }

        /// <summary>
        /// Show IronSource interstitial
        /// </summary>
        /// <param name="InterstitialClosed">callback called when user closes interstitial</param>
        public bool ShowInterstitial(UnityAction InterstitialClosed, string Placement, bool autoDisableLoading  = true, bool enableLoadingCanvas = true)
        {
            if (IsInterstitialAvailable())
            {
                OnInterstitialClosed = InterstitialClosed;
                autoDisableLoadingInter = autoDisableLoading;
                if(enableLoadingCanvas) LoadingAdsCanvas.Instance.ShowLoading();
                obInter?.Dispose();
                obInter = Observable.Timer(TimeSpan.FromSeconds(0.5f), Scheduler.MainThreadIgnoreTimeScale)
                    .Take(1)
                    .Subscribe(_ =>
                    {
                        AppOpenAdManager.Instance.ResumeFromAds = true;
                        IronSource.Agent.showInterstitial(Placement);
                        Time.timeScale = 0; // Pause game
                    }).AddTo(this);
                return true;
            }
            
            Debug.Log("Interstitial not available or not ready Capping-Time");
            InterstitialClosed?.Invoke();
            LoadInterstitial();
            return false;
        }

        /// <summary>
        /// IronSource specific event triggered after an interstitial was loaded
        /// </summary>
        private void InterstitialAdReadyEvent(IronSourceAdInfo info)
        {
            Debug.Log("IronSource Interstitial Loaded");

            currentRetryInterstitial = 0;
            OnInterstitialAvailable();
        }
        
        //IronSource: Invoked right before the Interstitial screen is about to open.
        void InterstitialAdShowSucceededEvent(IronSourceAdInfo info)
        {
            Debug.Log("IronSource show success Interstitial");
            currentRetryInterstitial = 0;
        }

        /// <summary>
        /// IronSource: Invoked when the interstitial ad closed and the user goes back to the application screen.
        /// </summary>
        private void InterstitialAdClosedEvent(IronSourceAdInfo info)
        {
            Debug.Log("IronSource Closed Interstitial");

            Time.timeScale = 1; // Resume game
            interstitialReadyTime = Time.time + FirebaseRemoteConfigManager.interstitialCappingTime;

            //trigger complete event
            StartCoroutine(CompleteMethodInterstitial());
        }

        /// <summary>
        ///  Because IronSource has some problems when used in multi threading applications with Unity a frame needs to be skipped before returning to application
        /// </summary>
        /// <returns></returns>
        private IEnumerator CompleteMethodInterstitial()
        {
            yield return new WaitForSeconds(1f);
            
            AppOpenAdManager.Instance.ResumeFromAds = false;

            if (OnInterstitialClosed != null)
            {
                OnInterstitialClosed();
                OnInterstitialClosed = null;
            }
            
            //reload interstitial
            LoadInterstitial();
            
            if (autoDisableLoadingInter)
            {
                LoadingAdsCanvas.Instance.HideLoading();
            }
        }

        /// <summary>
        /// IronSource: Invoked when the initialization process has failed.
        /// </summary>
        /// <param name="error"></param>
        private void InterstitialAdLoadFailedEvent(IronSourceError error)
        {
            Debug.Log(this + " Interstitial Failed To Load " + error.getCode() + "-" + error.getDescription());

            //try again to load a rewarded video
            if (currentRetryInterstitial < maxRetryCount)
            {
                currentRetryInterstitial++;
                Debug.Log("IronSource RETRY " + currentRetryInterstitial);
                StartCoroutine(ReloadInterstitial(reloadTime));
            }
        }

        /// <summary>
        /// IronSource specific event triggered if an interstitial failed to show
        /// </summary>
        /// <param name="error"></param>
        /// <param name="info"></param>
        private void InterstitialAdShowFailedEvent(IronSourceError error, IronSourceAdInfo info)
        {
            Debug.Log(this + " Interstitial Failed To Show " + error.getCode() + "-" + error.getDescription());
            
            Time.timeScale = 1; // Resume game
            LoadingAdsCanvas.Instance.HideLoading();
            AppOpenAdManager.Instance.ResumeFromAds = false;
            
            OnInterstitialFailedToShow(error.getDescription());
            
            //try again to load a rewarded video
            if (currentRetryInterstitial < maxRetryCount)
            {
                currentRetryInterstitial++;
                Debug.Log("IronSource RETRY " + currentRetryInterstitial);
                StartCoroutine(ReloadInterstitial(reloadTime));
            }
        }
        
        //IronSource: Invoked when end user clicked on the interstitial ad
        void InterstitialAdClickedEvent(IronSourceAdInfo info)
        {
            Debug.Log("IronSource Clicked Interstitial");
            AnalyticsManager.LogEventClickInterAds();
        }
        
        //IronSource: Invoked when the Interstitial Ad Unit has opened
        void InterstitialAdOpenedEvent()
        {
        }

        private IEnumerator ReloadInterstitial(float reloadTime)
        {
            yield return new WaitForSeconds(reloadTime);
            LoadInterstitial();
        }

        #endregion

        #region === REWARDED VIDEO ===

        /// <summary>
        /// Check if IronSource rewarded video is available
        /// </summary>
        /// <returns>true if a rewarded video is available</returns>
        public bool IsRewardVideoAvailable()
        {
            Debug.Log("IronSource -> Reward Video initialized: " + initialized + " - available: " + IronSource.Agent.isRewardedVideoAvailable());
            return initialized && IronSource.Agent.isRewardedVideoAvailable();
        }
        
        private bool isInitedRewardVideo;

        private void InitRewardVideo()
        {
            if (isInitedRewardVideo) return;
            
            isInitedRewardVideo = true;
            IronSourceRewardedVideoEvents.onAdReadyEvent += RewardedVideoReadyEvent;
            IronSourceRewardedVideoEvents.onAdShowFailedEvent += RewardedVideoAdShowFailedEvent;
            IronSourceRewardedVideoEvents.onAdClosedEvent += RewardedVideoAdClosedEvent;
            IronSourceRewardedVideoEvents.onAdRewardedEvent += RewardedVideoAdRewardedEvent;
            IronSourceRewardedVideoEvents.onAdClickedEvent += RewardedVideoAdClickedEvent;

            // IronSourceEvents.onRewardedVideoAdOpenedEvent += RewardedVideoAdOpenedEvent;
            // IronSourceEvents.onRewardedVideoAdStartedEvent += RewardedVideoAdStartedEvent;
            // IronSourceEvents.onRewardedVideoAdClosedEvent += RewardedVideoAdClosedEvent;
            // IronSourceEvents.onRewardedVideoAvailabilityChangedEvent += RewardedVideoAvailabilityChangedEvent;
            // IronSourceEvents.onRewardedVideoAdRewardedEvent += RewardedVideoAdRewardedEvent;
            // IronSourceEvents.onRewardedVideoAdShowFailedEvent += RewardedVideoAdShowFailedEvent;
            // IronSourceEvents.onRewardedVideoAdClickedEvent += RewardedVideoAdClickedEvent;
            LoadRewardedVideo();
        }

        private void LoadRewardedVideo()
        {
            //Please make sure you are not trying to load the RV ad manually if you are not going to use the manual RV loading flow.
            //Please note, that unlike Interstitials, the RV ads are being loaded automatically by default.
            //There is no use to call loadRewardedVideo() manually. Please remove this implementation
            return;
            IronSource.Agent.loadRewardedVideo();
        }

        public bool ShowRewardVideo(UnityAction<bool> CompleteMethod, string Placement)
        {
            if (IsRewardVideoAvailable())
            {
                OnRewardVideoCompleteMethod = CompleteMethod;
                AppOpenAdManager.Instance.ResumeFromAds = true;
                Time.timeScale = 0; // Pause game
                IronSource.Agent.showRewardedVideo(Placement);
                return true;
            }

            LoadRewardedVideo();
            ShowMessage("Ads not ready yet!");
            return false;
        }

        //IronSource: Invoked when the Rewarded Video failed to show
        private void RewardedVideoAdShowFailedEvent(IronSourceError error, IronSourceAdInfo info)
        {
            Debug.LogError("IronSource Rewarded Video OnAdFailedToShow " + error.getCode() + "-" + error.getDescription());

            Time.timeScale = 1; // Resume game
            AppOpenAdManager.Instance.ResumeFromAds = false;
            
            OnRewardVideoFailedToShow(error.getDescription());
            LoadRewardedVideo();
        }

        //IronSource: Invoked when the video ad starts playing.
        private void RewardedVideoAdStartedEvent()
        {
            Debug.Log("IronSource Rewarded Video OnAdOpening");
        }
        
        //IronSource: Invoked when the RewardedVideo ad view has opened.
        void RewardedVideoAdOpenedEvent()
        {
            Debug.Log(this + "IronSource Rewarded Video Displayed");
        }

        /// <summary>
        ///IronSource: Invoked when the RewardedVideo ad view is about to be closed. Your activity will now regain its focus.
        /// </summary>
        private void RewardedVideoAdClosedEvent(IronSourceAdInfo info)
        {
            Debug.Log("IronSource Rewarded Video OnAdClosed");
            
            Time.timeScale = 1; // Resume game
            AppOpenAdManager.Instance.ResumeFromAds = false;
            
            //load another rewarded video
            LoadRewardedVideo();
        }

        /// <summary>
        /// IronSource: specific event triggered after a rewarded video was fully watched
        /// </summary>
        /// <param name="placement"></param>
        /// <param name="info"></param>
        private void RewardedVideoAdRewardedEvent(IronSourcePlacement placement, IronSourceAdInfo info)
        {
            Debug.Log($"IronSource Rewarded Video Watched: reward {placement.getRewardAmount()}");

            Time.timeScale = 1; // Resume game
            
            StartCoroutine(CompleteMethodRewardedVideo(true));
        }
        
        /// <summary>
        /// Because IronSource has some problems when used in multi-threading applications with Unity a frame needs to be skipped before returning to application
        /// </summary>
        /// <returns></returns>
        private IEnumerator CompleteMethodRewardedVideo(bool val)
        {
            yield return null;
            if (OnRewardVideoCompleteMethod != null)
            {
                OnRewardVideoCompleteMethod(val);
                OnRewardVideoCompleteMethod = null;
                OnRewardVideoCompleted();
            }

            yield return new WaitForSeconds(1.0f);
            
            AppOpenAdManager.Instance.ResumeFromAds = false;
        }

        /// <summary>
        /// IronSource: Invoked when there is a change in the ad availability status.
        /// </summary>
        /// <param name="available">value will change to true when rewarded videos are available</param>
        private void RewardedVideoAvailabilityChangedEvent(bool available)
        {
            if (available)
            {
                Debug.Log("IronSource Rewarded Video Loaded");
                //Value will change to true when rewarded videos are available
                OnRewardVideoAvailable();
            }
            else
            {
                Debug.Log("IronSource Rewarded Video Load - Failed");
                //Value will change to false when no videos are available
            }
        }

        private void RewardedVideoReadyEvent(IronSourceAdInfo info)
        {
            Debug.Log("IronSource Rewarded Video Ready");
            OnRewardVideoAvailable();
        }
        
        void RewardedVideoAdClickedEvent (IronSourcePlacement placement, IronSourceAdInfo info)
        {
            Debug.Log ("IronSource Rewarded Video Clicked, name = " + placement.getRewardName());
            
            AnalyticsManager.LogEventClickRewardedVideoAds();
        }

        #endregion

        #region === BANNER ===

        /// <summary>
        /// Check if IronSource banner is available
        /// </summary>
        /// <returns>true if a banner is available</returns>
        public bool IsBannerAvailable()
        {
            return true;
        }

        /// <summary>
        /// Loads an IronSource banner
        /// </summary>
        /// <param name="position">display position</param>
        /// <param name="bannerType">can be normal banner or smart banner</param>
        private void LoadBanner(BannerPosition position = BannerPosition.BOTTOM, BannerType bannerType = BannerType.SmartBanner)
        {
            Debug.Log("IronSource Start Loading Banner");
            
            if(bannerLoaded) HideBanner();
            
            var bannerPosition = position == BannerPosition.BOTTOM ? IronSourceBannerPosition.BOTTOM : IronSourceBannerPosition.TOP;
            var bannerSize = IronSourceBannerSize.SMART;
            //var bannerSize = bannerType == BannerType.Banner ? IronSourceBannerSize.BANNER : IronSourceBannerSize.SMART;
            IronSource.Agent.loadBanner(bannerSize, bannerPosition);
        }

        /// <summary>
        /// Show IronSource banner
        /// </summary>
        /// <param name="position"> can be TOP or BOTTOM</param>
        ///  /// <param name="bannerType"> can be Banner or SmartBanner</param>
        public void ShowBanner(BannerPosition position = BannerPosition.BOTTOM, BannerType bannerType = BannerType.SmartBanner,
            UnityAction<bool, BannerPosition, BannerType> DisplayResult = null)
        {
            LoadBanner(position, bannerType);
        }
        
        void BannerShown()
        {
            Debug.Log(this + " banner ad shown");

            if (DisplayResult != null)
            {
                DisplayResult(true, bannerPosition, bannerType);
                DisplayResult = null;
            }
        }

        /// <summary>
        /// Hides IronSource banner
        /// </summary>
        public void HideBanner()
        {
            Debug.Log("IronSource Hide banner");
            
            if (DisplayResult != null)
            {
                DisplayResult(false, bannerPosition, bannerType);
                DisplayResult = null;
            }
            IronSource.Agent.destroyBanner();
        }

        /// <summary>
        /// Invoked once the banner has loaded
        /// </summary>
        private void BannerAdLoadedEvent(IronSourceAdInfo info)
        {
            Debug.Log("IronSource Banner Loaded");
            
            BannerShown();
        }

        /// <summary>
        /// IronSource specific event triggered after banner failed to load
        /// </summary>
        /// <param name="error"></param>
        private void BannerAdLoadFailedEvent(IronSourceError error)
        {
            //https://developers.is.com/ironsource-mobile/air/supersonic-sdk-error-codes/
            Debug.Log("IronSource Banner -> Error code: " + error.getCode() + "-" + error.getDescription());
            
            bannerLoaded = false;
            if (DisplayResult != null)
            {
                DisplayResult(false, bannerPosition, bannerType);
                DisplayResult = null;
            }
            
            StartCoroutine(RetryLoadBanner());
        }

        IEnumerator RetryLoadBanner()
        {
            yield return new WaitForSeconds(1f);
            
            LoadBanner();
        }
        
        //Invoked when end user clicks on the banner ad
        void BannerAdClickedEvent(IronSourceAdInfo info)
        {
            AnalyticsManager.LogEventClickBannerAds();
        }
        
        //Notifies the presentation of a full screen content following user click
        void BannerAdScreenPresentedEvent()
        {
        }

        //Notifies the presented screen has been dismissed
        void BannerAdScreenDismissedEvent()
        {
        }

        //Invoked when the user leaves the app
        void BannerAdLeftApplicationEvent()
        {
        }

        #endregion
        
        private void OnApplicationFocus(bool focus)
        {
            if (focus == true)
            {
                if (IsInterstitialAvailable() == false)
                {
                    if (currentRetryInterstitial == maxRetryCount)
                    {
                        LoadInterstitial();
                    }
                }
            }
        }
        
        private void OnApplicationPause(bool isPaused)
        {
            IronSource.Agent.onApplicationPause(isPaused);
        }
    }
}
#endif