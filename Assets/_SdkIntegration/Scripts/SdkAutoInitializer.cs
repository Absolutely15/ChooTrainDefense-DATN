using System;
using System.Collections;
using UnityEngine;
using ATSoft.Ads;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Sirenix.OdinInspector;
using UniRx;
using UnityEditor;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
#if USE_GA
using GameAnalyticsSDK;
#endif

namespace ATSoft
{
    public class SdkAutoInitializer : Singleton<SdkAutoInitializer>
    {
        [Title("SDK Initialization")]
        public bool enableAppTrackingTransparency = true;
        public bool enableAppOpenAd;
        public bool enableAds;
        public bool enableFirebase;
        public bool enableNotification;
        public bool enableGameAnalytics;
        public bool enableDebugLog = true;
        
        [Title("Game Config")]
        public bool enableInAppReview = false;
        public string packageName;
        public string appleAppId;
        public string policyLink; 
        
        [Title("UI Loading")]
        [SerializeField] private SceneName sceneNameToLoad = SceneName.Main;
        [SerializeField] private float timeLoading = 8f;
        [SerializeField] private UnityEvent<float> onLoadingProgress = new UnityEvent<float>();
        //[SerializeField] private Slider progressBar;
        
        //private Subject<bool> InternetSubject = new Subject<bool>();
        private IObservable<bool> internetObservable;
        private TweenerCore<float, float, FloatOptions> _tween;
        
        IEnumerator Start()
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            Application.targetFrameRate = 60;
            Debug.unityLogger.logEnabled = enableDebugLog;

            CheckInternet();
            
            float value = 0f;
            bool progressBarDone = false;
            _tween?.Kill();
            _tween = DOTween.To(() => value, x => value = x, 1, timeLoading)
                .OnUpdate(() =>
                {
                    onLoadingProgress.Invoke(value);
                })
                .OnComplete(() =>
                {
                    progressBarDone = true;
                });
            
            Debug.Log("=== Step ATT ===");
            if (enableAppTrackingTransparency)
            {
                AppTrackingTransparency.Instance.Initialize();
                yield return new WaitUntil(() => AppTrackingTransparency.Instance.initialized);
            }

            Debug.Log("=== Step AppsFlyers ===");
            AppsFlyerManager.Instance.Initialize();
            yield return new WaitUntil(() => AppsFlyerManager.Instance.initialized);

            Debug.Log("=== Step LoadSceneAsync ===");
            var sceneName = sceneNameToLoad.ToString();
            AsyncOperation async = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
            async.allowSceneActivation = false;
            
            Debug.Log("=== Step Firebase ===");
            if (enableFirebase)
            {
                FirebaseManager.Instance.Initialize();
                yield return new WaitUntil(() => FirebaseManager.Instance.IsFirebaseReady());
            }
            
            Debug.Log("=== Step AOA ===");
            if (enableAppOpenAd)
            {
                AppOpenAdManager.Instance.Initialize();
                yield return new WaitUntil(() => AppOpenAdManager.Instance.initialized);
            }

            Debug.Log("=== Step Ads ===");
            if (enableAds)
            {
                Advertisements.Instance.Initialize();
                yield return new WaitUntil(() => Advertisements.Instance.initialized);
            }

            Debug.Log("=== Step LoadSceneAsync Done ===");
            while (!async.isDone)
            {
                // Check if the load has finished
                yield return null;
                if (async.progress >= 0.9f)
                {
                    break;
                }
            }
            
            Debug.Log("=== Step Show AOA ===");
            yield return new WaitForSeconds(2f);
            UnityAction actionAoaComplete = () =>
            {
                Advertisements.Instance.ShowBanner();
#if USE_NOTIFICATION
                    Debug.Log("=== Step Notification ===");
                    if (enableNotification)
                    {
                        NotificationManager.Instance.Initialize();
                    }
#endif
                async.allowSceneActivation = true;
            };
            if (AppOpenAdManager.Instance.IsAppOpenAdAvailable())
            {
                if (FirebaseRemoteConfigManager.enableShowAOAOpenGame)
                {
                    AppOpenAdManager.Instance.ShowAppOpenAd(() =>
                    {
                        _tween?.Complete();
                        actionAoaComplete.Invoke();
                    });
                }
                else
                {
                    _tween?.Complete();
                    actionAoaComplete.Invoke();
                }
            }
            else
            {
                yield return new WaitUntil(() => progressBarDone);
                if (FirebaseRemoteConfigManager.enableShowAOAOpenGame)
                {
                    if (AppOpenAdManager.Instance.IsAppOpenAdAvailable())
                    {
                        AppOpenAdManager.Instance.ShowAppOpenAd(() =>
                        {
                            actionAoaComplete.Invoke();
                        });
                    }
                    else
                    {
                        actionAoaComplete.Invoke();
                    }
                }
                else
                {
                    actionAoaComplete.Invoke();
                }
            }
        }

        private void CheckInternet()
        {
            internetObservable = Observable.Interval(TimeSpan.FromSeconds(2), Scheduler.MainThreadIgnoreTimeScale)
                .Select(_ => InternetAvailable);

            internetObservable.Where(haveInternet => !haveInternet).Subscribe(_ =>
            {
                //Debug.LogError("Internet is not Ready !!!");
                InternetNotAvailable.Instance.ShowLoading();
            });
            
            internetObservable.Where(haveInternet => haveInternet).Subscribe(_ =>
            {
                //Debug.Log("Internet is Ready !!!");
                InternetNotAvailable.Instance.HideLoading();
            });
        }
        
        private bool InternetAvailable => Application.internetReachability != NetworkReachability.NotReachable;
        
#if UNITY_EDITOR
        [Title("Final Checking")]
        [Button(ButtonSizes.Large, ButtonStyle.Box, Name = "=== FINAL CHECKING ===")]
        public void CheckFinal()
        {
            AddPreprocessorDirectiveNotification();
            AddPreprocessorDirectiveGA();
            AddPreprocessorDirectiveNiceVibration();
            AddPreprocessorDirectiveGoogleInAppReview();

            // if (progressBar == null)
            // {
            //     Selection.activeObject = this;
            //     EditorUtility.DisplayDialog("Final Checking Failed", "Check Progress Loading Bar Empty!!!", "Go Go");
            //     return;
            // }

#if UNITY_IOS
            enableInAppReview = false;
            if (string.IsNullOrEmpty(appleAppId))
            {
                Selection.activeObject = this;
                EditorUtility.DisplayDialog("Final Checking Failed", "Check Game Config AppID IOS Empty!!!", "Go Go");
                return;
            }
#else
            if (string.IsNullOrEmpty(packageName))
            {
                Selection.activeObject = this;
                EditorUtility.DisplayDialog("Final Checking Failed", "Check Game Config PackageName Empty!!!", "Go Go");
                return;
            }
#endif

            var appsFlyer = transform.GetComponentInChildren<AppsFlyerManager>();
            if (appsFlyer != null)
            {
                if (string.IsNullOrEmpty(appsFlyer.devKey))
                {
                    Selection.activeObject = appsFlyer;
                    EditorUtility.DisplayDialog("Final Checking Failed", "Check AppsFlyer Dev Key Empty!!!", "Go Go");
                    return;
                }
#if UNITY_IOS
                if (string.IsNullOrEmpty(appsFlyer.appID))
                {
                    Selection.activeObject = appsFlyer;
                    EditorUtility.DisplayDialog("Final Checking Failed", "Check AppsFlyer AppID IOS Empty!!!", "Go Go");
                    return;
                }
#endif
            }

            var advert = transform.GetComponentInChildren<Advertisements>();
            if (advert != null)
            {
                advert.CheckFinal();
                if (advert.TypeAdvertiser == SupportedAdvertisers.IronSource)
                {
                    advert.enableConsentFlow = true;
                }
                policyLink = advert.Pulisher switch
                {
                    ATPublisher.RK => "http://rocketstudio.com.vn/space_shooter/policy.html",
                    ATPublisher.Ad1 => "https://adone.net/privacy.html",
                    _ => "https://atsoft.io"
                };
            }
            
            var appOpenAd = transform.GetComponentInChildren<AppOpenAdManager>();
            if (appOpenAd != null)
            {
                appOpenAd.CheckFinal();
            }

#if !UNITY_IOS
            EditorUtility.DisplayDialog("Final Checking", "Checker ~DatDz~ success <3", "OK");
#else
            Debug.Log("<color=yellow>=== Checker ~DatDz~ success <3 ===</color>");
#endif
        }
        private void AddPreprocessorDirectiveNotification()
        {
            var buildTargetGroup = BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget);

            string textToWrite = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);

            string directive = enableNotification ? "USE_NOTIFICATION" : string.Empty;
            
            if(!enableNotification)
            {
                textToWrite = textToWrite.Replace("USE_NOTIFICATION", "");
            }
            
            if (!textToWrite.Contains(directive))
            {
                if (textToWrite == "")
                {
                    textToWrite += directive;
                }
                else
                {
                    textToWrite += ";" + directive;
                }
            }

            PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, textToWrite);
        }
        
        private void AddPreprocessorDirectiveGA()
        {
            var buildTargetGroup = BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget);

            string textToWrite = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);

            string directive = enableGameAnalytics ? "USE_GA" : string.Empty;
            
            if(!enableGameAnalytics)
            {
                textToWrite = textToWrite.Replace("USE_GA", "");
            }
            
            if (!textToWrite.Contains(directive))
            {
                if (textToWrite == "")
                {
                    textToWrite += directive;
                }
                else
                {
                    textToWrite += ";" + directive;
                }
            }

            PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, textToWrite);
        }
        
        private void AddPreprocessorDirectiveNiceVibration()
        {
            var buildTargetGroup = BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget);

            string textToWrite = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);

            string directive = "MOREMOUNTAINS_NICEVIBRATIONS";
            
            if (!textToWrite.Contains(directive))
            {
                if (textToWrite == "")
                {
                    textToWrite += directive;
                }
                else
                {
                    textToWrite += ";" + directive;
                }
            }

            PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, textToWrite);
        }
        
        private void AddPreprocessorDirectiveGoogleInAppReview()
        {
            var buildTargetGroup = BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget);

            string textToWrite = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);

            string directive = enableInAppReview ? "USE_IN_APP_REVIEW" : string.Empty;
            
            if(!enableInAppReview)
            {
                textToWrite = textToWrite.Replace("USE_IN_APP_REVIEW", "");
            }
            
            if (!textToWrite.Contains(directive))
            {
                if (textToWrite == "")
                {
                    textToWrite += directive;
                }
                else
                {
                    textToWrite += ";" + directive;
                }
            }

            PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, textToWrite);
        }
#endif
    }
}
