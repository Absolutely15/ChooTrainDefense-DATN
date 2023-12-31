using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.Events;

//  ---------------------------------------------
//  Author:     DatDz <steven@atsoft.io> 
//  Copyright (c) 2022 AT Soft
// ----------------------------------------------

#if AD_TYPE_APPLOVIN
namespace ATSoft.Ads
{
    public class CustomAppLovin : BaseAds, ICustomAds
    {
        private bool rewardedVideoCompleted;
        private bool directedForChildren;
        private bool enableDebugger;

        /// <summary>
        /// Initializing AppLovin
        /// </summary>
        /// <param name="consent">user consent -> if true show personalized ads</param>
        /// <param name="platformSettings">contains all required settings for this publisher</param>
        public void InitializeAds(List<AdvertiserSettings> platformSettings, bool isTestAd, bool enableMediationDebugger, bool enableConsent)
        {
            if (initialized == false)
            {
                //get settings
#if UNITY_ANDROID
                AdvertiserSettings settings = platformSettings.First(cond => cond.platform == SupportedPlatforms.Android);
#elif UNITY_IOS
                AdvertiserSettings settings = platformSettings.First(cond => cond.platform == SupportedPlatforms.IOS);
#else
                AdvertiserSettings settings = new AdvertiserSettings(SupportedPlatforms.Windows,"", "", "","");
#endif

                //preparing AppLovin SDK for initialization
                Debug.Log("APPID: " + settings.appId);

                //Request target for children
                // directedForChildren = settings.directedForChildren;
                
                //apply settings
                interstitialId = settings.idInterstitial;
                bannerId = settings.idBanner;
                rewardedVideoId = settings.idRewarded;
                enableDebugger = enableMediationDebugger;

                MaxSdkCallbacks.OnSdkInitializedEvent += ApplovinInitialized;

#if !AOA_TYPE_APPLOVIN
                //Initialize the SDK
                MaxSdk.SetSdkKey(settings.appId);
                //Set UserId by AppsFlyer
#if PUB_RK || PUB_AD1 || PUB_FAL
                MaxSdk.SetUserId(AppsFlyer.getAppsFlyerId());
#endif
                MaxSdk.InitializeSdk();

                Debug.Log(this + " " + "Start Initialization");
                Debug.Log(this + " SDK key: " + settings.appId);
#endif
            }
        }

        public void ContinueConsentFlow()
        {
            // var acceptConsent = PlayerPrefs.GetInt("ACCEPT_CONSENT", -1);
            // StartCoroutine(WaitingConsent(acceptConsent >= 1));
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            StopAllCoroutines();
        }

        private void ApplovinInitialized(MaxSdk.SdkConfiguration obj)
        {
            Debug.Log(this + " Init Complete");

            initialized = true;
            if (enableDebugger)
            {
                // Show Mediation Debugger
                MaxSdk.ShowMediationDebugger();
                MaxSdk.SetVerboseLogging(true);
            }

            if (Advertisements.Instance.CanShowAds())
            {
                InitBanner();
                InitInterstitial();
            }
           
            InitRewardVideo();
        }

        private void InitBanner()
        {
            if (string.IsNullOrEmpty(bannerId)) return;

            MaxSdkCallbacks.Banner.OnAdLoadedEvent += OnBannerAdLoadedEvent;
            MaxSdkCallbacks.Banner.OnAdLoadFailedEvent += OnBannerAdFailedEvent;
            MaxSdkCallbacks.Banner.OnAdClickedEvent += OnBannerAdClickedEvent;
            MaxSdkCallbacks.Banner.OnAdExpandedEvent += OnBannerAdExpandedEvent;
            //MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent += OnBannerAdRevenuePaidEvent;
            MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent += OnRevenuePaidEvent;

            ShowBanner();
        }
        
        private void InitInterstitial()
        {
            if (string.IsNullOrEmpty(interstitialId)) return;
            
            // Attach callbacks
            MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += OnInterstitialLoadedEvent;
            MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += OnInterstitialFailedEvent;
            MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += InterstitialFailedToDisplayEvent;
            MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += OnInterstitialDismissedEvent;
            MaxSdkCallbacks.Interstitial.OnAdClickedEvent += OnInterstitialClickedEvent;
            //MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent += OnInterstitialRevenuePaidEvent;
            MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent += OnRevenuePaidEvent;

            LoadInterstitial();
        }


        private void InitRewardVideo()
        {
            if (string.IsNullOrEmpty(rewardedVideoId)) return;
            
            // Attach callbacks
            MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += OnRewardedAdLoadedEvent;
            MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += OnRewardedAdFailedEvent;
            MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnRewardedAdFailedToDisplayEvent;
            //MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += OnRewardedAdDisplayedEvent;
            MaxSdkCallbacks.Rewarded.OnAdClickedEvent += OnRewardedAdClickedEvent;
            MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnRewardedAdDismissedEvent;
            MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;
            //MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnRewardedAdRevenuePaidEvent;
            MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnRevenuePaidEvent;

            //start loading ads
            LoadRewardedVideo();
        }

        #region === INTERSTITIAL ===

        /// <summary>
        /// Check if AppLovin interstitial is available
        /// </summary>
        /// <returns>true if an interstitial is available</returns>
        public bool IsInterstitialAvailable()
        {
            return MaxSdk.IsInterstitialReady(interstitialId) && interstitialReadyTime <= Time.time;
        }

        /// <summary>
        /// Preload an interstitial ad before showing
        /// if it fails for maxRetryCount times do not try anymore
        /// </summary>
        void LoadInterstitial()
        {
            if (currentRetryInterstitial < maxRetryCount)
            {
                Debug.Log(this + " Load Interstitial");

                currentRetryInterstitial++;
                MaxSdk.LoadInterstitial(interstitialId);
            }
        }

        /// <summary>
        /// Show AppLovin interstitial
        /// </summary>
        /// <param name="InterstitialClosed">callback called when user closes interstitial</param>
        public bool ShowInterstitial(UnityAction InterstitialClosed, string Placement, bool autoDisableLoading = true, bool enableLoadingCanvas = true)
        {
            if (IsInterstitialAvailable())
            {
                OnInterstitialClosed = InterstitialClosed;
                this.autoDisableLoadingInter = autoDisableLoading;
                if(enableLoadingCanvas) LoadingAdsCanvas.Instance.ShowLoading();
                obInter?.Dispose();
                obInter = Observable.Timer(TimeSpan.FromSeconds(0.5f), Scheduler.MainThreadIgnoreTimeScale)
                    .Take(1)
                    .Subscribe(_ =>
                    {
                        AppOpenAdManager.Instance.ResumeFromAds = true;
                        MaxSdk.ShowInterstitial(interstitialId);
                        Time.timeScale = 0; // Pause game
                    }).AddTo(this);
                return true;
            }
            
            Debug.Log("Interstitial not available or not ready Capping-Time");
            LoadInterstitial();
            InterstitialClosed?.Invoke();
            return false;
        }

        private void OnInterstitialLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Debug.Log(this + " interstitial ad was loaded");

            currentRetryInterstitial = 0;
            OnInterstitialAvailable();
        }

        private void OnInterstitialFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            Debug.Log(this + " interstitial ad failed to load: " + errorInfo.Code);
            
            //wait and load another
            double retryDelay = Math.Pow(2, Math.Min(6, currentRetryInterstitial));
            Debug.Log(this + " reloading " + currentRetryInterstitial + " in " + currentRetryInterstitial + " sec");
            Invoke("LoadInterstitial", (float)retryDelay);
        }

        private void InterstitialFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo,
            MaxSdkBase.AdInfo adInfo)
        {
            Debug.LogError(this + " interstitial ad failed to display: " + errorInfo.Code);
            Time.timeScale = 1; // Resume game
            AppOpenAdManager.Instance.ResumeFromAds = false;
            LoadingAdsCanvas.Instance.HideLoading();
            OnInterstitialFailedToShow(errorInfo.Code.ToString());
            LoadInterstitial();
        }

        private void OnInterstitialDismissedEvent(string arg1, MaxSdkBase.AdInfo arg2)
        {
            Debug.Log(this + " interstitial ad was closed");
            
            Time.timeScale = 1; // Resume game
            interstitialReadyTime = Time.time + FirebaseRemoteConfigManager.interstitialCappingTime;

            //MainThreadDispatcher.StartUpdateMicroCoroutine(CompleteMethodInterstitial());
            StartCoroutine(CompleteMethodInterstitial());
        }
        
        private IEnumerator CompleteMethodInterstitial()
        {
            yield return new WaitForSeconds(1f);
            
            AppOpenAdManager.Instance.ResumeFromAds = false;
            
            //trigger closed callback
            if (OnInterstitialClosed != null)
            {
                OnInterstitialClosed();
                OnInterstitialClosed = null;
            }

            //load another ad
            LoadInterstitial();

            if (autoDisableLoadingInter)
            {
                LoadingAdsCanvas.Instance.HideLoading();
            }
        }

        private void OnInterstitialClickedEvent(string arg1, MaxSdkBase.AdInfo arg2)
        {
            AnalyticsManager.LogEventClickInterAds();
        }

        private void OnInterstitialRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            // Ad revenue
            double revenue = adInfo.Revenue;

            // Miscellaneous data
            string
                countryCode = MaxSdk.GetSdkConfiguration()
                        .CountryCode; // "US" for the United States, etc - Note: Do not confuse this with currency code which is "USD" in most cases!
            string
                networkName =
                    adInfo.NetworkName; // Display name of the network that showed the ad (e.g. "AdColony")
            string adUnitIdentifier = adInfo.AdUnitIdentifier; // The MAX Ad Unit ID
            string Placement = adInfo.Placement; // The Placement this ad's postbacks are tied to

            Debug.Log(this + " revenue " + revenue + " countryCode " + countryCode + " networkName " + networkName +
                      " adUnitIdentifier " + adUnitIdentifier + " Placement " + Placement);
            
            var impressionParameters = new[] {
                new Firebase.Analytics.Parameter("ad_platform", "AppLovin"),
                new Firebase.Analytics.Parameter("ad_source", networkName),
                new Firebase.Analytics.Parameter("ad_unit_name", adUnitIdentifier),
                //new Firebase.Analytics.Parameter("ad_format", adUnitIdentifier),
                new Firebase.Analytics.Parameter("value", revenue),
                new Firebase.Analytics.Parameter("currency", "USD"), // All AppLovin revenue is sent in USD
            };
            Firebase.Analytics.FirebaseAnalytics.LogEvent("ad_impression", impressionParameters);
        }

        #endregion

        #region === REWARDEDVIDEO ===

        /// <summary>
        /// Check if AppLovin rewarded video is available
        /// </summary>
        /// <returns>true if a rewarded video is available</returns>
        public bool IsRewardVideoAvailable()
        {
            return MaxSdk.IsRewardedAdReady(rewardedVideoId);
        }

        /// <summary>
        /// preload a rewarded video ad before showing
        /// if it fails for maxRetryCount times do not try anymore
        /// </summary>
        void LoadRewardedVideo()
        {
            if (currentRetryRewardedVideo < maxRetryCount)
            {
                Debug.Log(this + " Load Rewarded Video");

                currentRetryRewardedVideo++;
                MaxSdk.LoadRewardedAd(rewardedVideoId);
            }
        }

        /// <summary>
        /// Show AppLovin rewarded video
        /// </summary>
        /// <param name="CompleteMethod">callback called when user closes the rewarded video -> if true, video was not skipped</param>
        public bool ShowRewardVideo(UnityAction<bool> CompleteMethod, string Placement)
        {
            if (IsRewardVideoAvailable())
            {
                OnRewardVideoCompleteMethod = CompleteMethod;
                rewardedVideoCompleted = false;
                AppOpenAdManager.Instance.ResumeFromAds = true;
                MaxSdk.ShowRewardedAd(rewardedVideoId);
                Time.timeScale = 0; // Pause game
                return true;
            }

            LoadRewardedVideo();
            ShowMessage("Ads not ready yet!");
            return false;
        }

        private void OnRewardedAdLoadedEvent(string arg1, MaxSdkBase.AdInfo arg2)
        {
            Debug.Log(this + " rewarded video was successfully loaded");

            currentRetryRewardedVideo = 0;
            OnRewardVideoAvailable();
        }

        private void OnRewardedAdFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            Debug.Log(this + " rewarded video failed to load " + errorInfo.Code + " " + errorInfo.Message);

            //wait and load another
            double retryDelay = Math.Pow(2, Math.Min(6, currentRetryRewardedVideo));
            Debug.Log(this + " reloading " + currentRetryRewardedVideo + " in " + retryDelay + " sec");
            Invoke("LoadRewardedVideo", (float)retryDelay);
        }

        private void OnRewardedAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo,
            MaxSdkBase.AdInfo adInfo)
        {
            Debug.Log(this + " rewarded video failed to display " + errorInfo.Code + " " + errorInfo.Message);

            Time.timeScale = 1; // Resume game
            AppOpenAdManager.Instance.ResumeFromAds = false;
            OnRewardVideoFailedToShow(errorInfo.Code.ToString());
            LoadRewardedVideo();
        }

        private void OnRewardedAdDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Debug.Log(this + " rewarded video displayed");
        }

        private void OnRewardedAdClickedEvent(string arg1, MaxSdkBase.AdInfo arg2)
        {
            Debug.Log(this + " rewarded video clicked");
            
            AnalyticsManager.LogEventClickRewardedVideoAds();
        }

        private void OnRewardedAdDismissedEvent(string arg1, MaxSdkBase.AdInfo arg2)
        {
            Debug.Log(this + " rewarded video was closed");
            
            Time.timeScale = 1; // Resume game

            //trigger rewarded video completed callback method
            if (OnRewardVideoCompleteMethod != null)
            {
                OnRewardVideoCompleteMethod(rewardedVideoCompleted);
                OnRewardVideoCompleteMethod = null;
            }

            //load another rewarded video
            LoadRewardedVideo();
            
            StartCoroutine(CompleteMethodRewardedVideo());
        }

        private IEnumerator CompleteMethodRewardedVideo()
        {
            yield return new WaitForSeconds(1.0f);
            
            AppOpenAdManager.Instance.ResumeFromAds = false;
        }

        private void OnRewardedAdReceivedRewardEvent(string adUnitId, MaxSdk.Reward reward, MaxSdkBase.AdInfo adInfo)
        {
            Debug.Log("Rewarded video was completed: " + reward.Amount + " " + reward.Label);

            rewardedVideoCompleted = true;
            OnRewardVideoCompleted();
        }

        private void OnRewardedAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            // Ad revenue
            double revenue = adInfo.Revenue;

            // Miscellaneous data
            string
                countryCode =
                    MaxSdk.GetSdkConfiguration()
                        .CountryCode; // "US" for the United States, etc - Note: Do not confuse this with currency code which is "USD" in most cases!
            string networkName = adInfo.NetworkName; // Display name of the network that showed the ad (e.g. "AdColony")
            string adUnitIdentifier = adInfo.AdUnitIdentifier; // The MAX Ad Unit ID
            string Placement = adInfo.Placement; // The Placement this ad's postbacks are tied to

            Debug.Log(this + " revenue " + revenue + " countryCode " + countryCode + " networkName " + networkName +
                      " adUnitIdentifier " + adUnitIdentifier + " Placement " + Placement);
            
            var impressionParameters = new[] {
                new Firebase.Analytics.Parameter("ad_platform", "AppLovin"),
                new Firebase.Analytics.Parameter("ad_source", networkName),
                new Firebase.Analytics.Parameter("ad_unit_name", adUnitIdentifier),
                //new Firebase.Analytics.Parameter("ad_format", adUnitIdentifier),
                new Firebase.Analytics.Parameter("value", revenue),
                new Firebase.Analytics.Parameter("currency", "USD"), // All AppLovin revenue is sent in USD
            };
            Firebase.Analytics.FirebaseAnalytics.LogEvent("ad_impression", impressionParameters);
        }

        #endregion

        #region === BANNER ===

        public bool IsBannerAvailable()
        {
            return true;
        }

        /// <summary>
        /// Show AppLovin banner
        /// </summary>
        /// <param name="position">can be TOP of BOTTOM</param>
        /// <param name="bannerType">it is not used in AppLovin, this param is used just in Admob implementation</param>
        public void ShowBanner(BannerPosition position = BannerPosition.BOTTOM, BannerType bannerType = BannerType.SmartBanner,
            UnityAction<bool, BannerPosition, BannerType> DisplayResult = null)
        {
            this.bannerType = bannerType;
            this.DisplayResult = DisplayResult;

            LoadBanner(position, bannerType);
            MaxSdk.ShowBanner(bannerId);

            bannerPosition = position;
        }


        private void LoadBanner(BannerPosition position, BannerType bannerType)
        {
            MaxSdk.DestroyBanner(bannerId);
            if (position == BannerPosition.TOP)
            {
                MaxSdk.CreateBanner(bannerId, MaxSdkBase.BannerPosition.TopCenter);
            }
            else
            {
                MaxSdk.CreateBanner(bannerId, MaxSdkBase.BannerPosition.BottomCenter);
            }

            // Set background or background color for banners to be fully functional.
            MaxSdk.SetBannerBackgroundColor(bannerId, Color.black);
        }

        /// <summary>
        /// Hides AppLovin banner
        /// </summary>
        public void HideBanner()
        {
            MaxSdk.HideBanner(bannerId);
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

        private void OnBannerAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Debug.Log(this + " banner ad loaded");

            BannerShown();
        }

        private void OnBannerAdFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            Debug.Log(this + " banner ad failed to load " + errorInfo.Code + " " + errorInfo.Message);
            
            if (DisplayResult != null)
            {
                DisplayResult(false, bannerPosition, bannerType);
                DisplayResult = null;
            }
            
            ShowBanner();
        }
        
        private void OnBannerAdExpandedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Debug.Log(this + " banner ad Expanded");

            BannerShown();
        }

        private void OnBannerAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Debug.Log(this + " banner ad clicked");
            
            AnalyticsManager.LogEventClickBannerAds();
        }


        private void OnBannerAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            Debug.Log(this + " banner ad revenue paid");
        }

        #endregion

        private void OnRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            // Ad revenue paid. Use this callback to track user revenue.
            Debug.Log("Ad revenue paid");

            // Ad revenue
            double revenue = adInfo.Revenue;

            // Miscellaneous data
            string
                countryCode =
                    MaxSdk.GetSdkConfiguration()
                        .CountryCode; // "US" for the United States, etc - Note: Do not confuse this with currency code which is "USD" in most cases!
            string networkName = adInfo.NetworkName; // Display name of the network that showed the ad (e.g. "AdColony")
            string adUnitIdentifier = adInfo.AdUnitIdentifier; // The MAX Ad Unit ID
            string placement = adInfo.Placement; // The placement this ad's postbacks are tied to
            string adFormat = adInfo.AdFormats;

            var data = new ImpressionData
            {
                AdFormat = adFormat,
                AdUnitIdentifier = adUnitIdentifier,
                CountryCode = countryCode,
                NetworkName = networkName,
                Placement = placement,
                Revenue = revenue
            };

            SendEventImpression(data);
        }

         private void SendEventImpression(ImpressionData data)
         {
             Firebase.Analytics.Parameter[] AdParameters =
             {
                 new Firebase.Analytics.Parameter("ad_platform", "applovin"),
                 new Firebase.Analytics.Parameter("ad_source", data.NetworkName),
                 new Firebase.Analytics.Parameter("ad_unit_name", data.AdUnitIdentifier),
                 new Firebase.Analytics.Parameter("currency", "USD"),
                 new Firebase.Analytics.Parameter("value", data.Revenue),
                 new Firebase.Analytics.Parameter("placement", data.Placement),
                 new Firebase.Analytics.Parameter("country_code", data.CountryCode),
                 new Firebase.Analytics.Parameter("ad_format", data.AdFormat),
             };

             Firebase.Analytics.FirebaseAnalytics.LogEvent(FirebaseStringEvent.ad_impression_max, AdParameters);
         }

        private void OnApplicationFocus(bool focus)
        {
            if (focus == true)
            {
                if (IsInterstitialAvailable() == false)
                {
                    if (currentRetryInterstitial == maxRetryCount)
                    {
                        currentRetryInterstitial--;
                        LoadInterstitial();
                    }
                }

                if (IsRewardVideoAvailable() == false)
                {
                    if (currentRetryRewardedVideo == maxRetryCount)
                    {
                        currentRetryRewardedVideo--;
                        LoadRewardedVideo();
                    }
                }
            }
        }
    }
}
#endif