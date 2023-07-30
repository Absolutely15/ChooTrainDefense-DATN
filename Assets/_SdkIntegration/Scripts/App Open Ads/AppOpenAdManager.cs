using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

//  ---------------------------------------------
//  Author:     DatDz <steven@atsoft.io> 
//              AVT
//  Copyright (c) 2022 AT Soft
// ----------------------------------------------
namespace ATSoft.Ads
{
    public class AppOpenAdManager : Singleton<AppOpenAdManager>, IService
    {
        [SerializeField] private bool isTestAds;
        [SerializeField] private SupportedAdvertisers typeAdvertiser = SupportedAdvertisers.None;

        [Header("=== Settings Ads ===")] 
        public List<AppOpenAdvertiserSettings> appOpenAdvertiserSettings;
        
        [HideInInspector] public bool ResumeFromAds = false;
        [HideInInspector] public bool initialized;

        private AppOpenAdvertiser advertiser;
        private ICustomAppOpenAds selectedAdvertiser;
        private int countLoadAOA;
        private float timer;

        public void Initialize()
        {
            Debug.Log("Initialize " + (typeof(AppOpenAdManager)));

            if (initialized == false)
            {
#if UNITY_ANDROID
                AppOpenAdvertiserSettings settings =
                    appOpenAdvertiserSettings.FirstOrDefault(cond => cond.platform == SupportedPlatforms.Android);
#elif UNITY_IOS
                AppOpenAdvertiserSettings settings =
                    appOpenAdvertiserSettings.FirstOrDefault(cond => cond.platform == SupportedPlatforms.IOS);
#else
                AppOpenAdvertiserSettings settings = new AppOpenAdvertiserSettings(SupportedPlatforms.Windows,"", "", "","");
#endif
                switch (typeAdvertiser)
                {
                    case SupportedAdvertisers.Admob:
#if AOA_TYPE_ADMOB
                        var admobComp = gameObject.AddComponent<CustomAppOpenAdmob>();
                        advertiser = new AppOpenAdvertiser(admobComp, settings);
#endif
                        break;
                    case SupportedAdvertisers.AppLovin:
#if AOA_TYPE_APPLOVIN
                        var appLovinComp = gameObject.AddComponent<CustomAppOpenAppLovin>();
                        advertiser = new AppOpenAdvertiser(appLovinComp, settings);
#endif
                        break;
                    default:
                        Debug.LogWarning("AT Soft - Failed to Set up AOA Ads - UnKnow type Advertiser");
                        break;
                }

                if (advertiser != null)
                {
                    selectedAdvertiser = advertiser.advertiserScript;
                    selectedAdvertiser.InitializeAds(settings, isTestAds);
                }
                
                initialized = true;
            }
        }

        private ICustomAppOpenAds GetAppOpenAdvertiser()
        {
            return advertiser?.advertiserScript;
        }

        public bool IsAppOpenAdAvailable()
        {
            return selectedAdvertiser != null && selectedAdvertiser.IsAppOpenAdAvailable();
        }

        public bool ShowAppOpenAd(UnityAction appOpenAdClosed = null)
        {
            if (!SdkAutoInitializer.Instance.enableAppOpenAd)
            {
                appOpenAdClosed?.Invoke();
                return true;
            }
            
            if(!initialized)
            {
                return false;
            }
            
            if (selectedAdvertiser != null)
            {
                Debug.Log("AOA loaded from " + selectedAdvertiser);
                
                var showSuccess = selectedAdvertiser.ShowAppOpenAd(appOpenAdClosed);
                return showSuccess;
            }
            
            return false;
        }

        private bool didShowAoALoadingTime;
        private float lastCheck = 0;

        public void ShowAOAIfLoadedInLoadingTime()
        {
            if(Time.time <= 5f) return;
            
            if (didShowAoALoadingTime || lastCheck + 0.5f > Time.time)
                return;

            lastCheck = Time.time;
            didShowAoALoadingTime = ShowAppOpenAd();
        }

        private void OnApplicationPause(bool pause)
        {
            if (!pause && SdkAutoInitializer.Instance.enableAppOpenAd)
            {
                ShowAppOpenAd();
            }
        }

        
#if UNITY_EDITOR
        public void CheckFinal()
        {
            ChangeTypeAdvertiserHandle();
        }
        
        private void ChangeTypeAdvertiserHandle()
        {
            var buildTargetGroup = BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget);

            switch (typeAdvertiser)
            {
                case SupportedAdvertisers.None:
                    AddPreprocessorDirectiveAdType("", buildTargetGroup);
                    break;
                case SupportedAdvertisers.Admob:
                    AddPreprocessorDirectiveAdType("AOA_TYPE_ADMOB", buildTargetGroup);
                    break;
                case SupportedAdvertisers.AppLovin:
                    AddPreprocessorDirectiveAdType("AOA_TYPE_APPLOVIN", buildTargetGroup);
                    isTestAds = false; //Applovin not working with Test Ad
                    break;
                default:
                    AddPreprocessorDirectiveAdType("", buildTargetGroup);
                    break;
            }
        }

        private void AddPreprocessorDirectiveAdType(string directive, BuildTargetGroup target)
        {
            string textToWrite = PlayerSettings.GetScriptingDefineSymbolsForGroup(target);

            switch (directive)
            {
                case "AOA_TYPE_ADMOB":
                    textToWrite = textToWrite.Replace("AOA_TYPE_APPLOVIN", "");
                    break;
                case "AOA_TYPE_APPLOVIN":
                    textToWrite = textToWrite.Replace("AOA_TYPE_ADMOB", "");
                    break;
                default:
                    textToWrite = textToWrite.Replace("AOA_TYPE_ADMOB", "").Replace("AOA_TYPE_APPLOVIN", "");
                    break;
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

            PlayerSettings.SetScriptingDefineSymbolsForGroup(target, textToWrite);
        }
#endif
    }
}

public enum AppOpenAdState
{
    NONE,
    OPENING,
    CLOSED,
    TIME_OUT
}