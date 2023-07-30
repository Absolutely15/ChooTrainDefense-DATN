using System.Collections.Generic;
using Firebase.Analytics;
using UnityEngine;
#if PUB_RK || PUB_AD1 || PUB_FAL
using AppsFlyerSDK;
#endif
#if USE_GA
using GameAnalyticsSDK;
#endif

namespace ATSoft
{
    public class FirebaseStringEvent
    {
#if PUB_FAL //Fal
        public const string level_engage = "level_engage";
        public const string level_engage_passed = "level_engage_passed";
        public const string impdau_inter_passed = "impdau_inter_passed";
        public const string impdau_reward_passed = "impdau_reward_passed";

        public const string level_start = "level_start";
        public const string level_passed = "level_complete";
        public const string level_failed = "level_fail";
        
        public const string show_interstitial_ads = "ad_inter_show";
        public const string interstitial_ads_available = "ad_inter_load";
        public const string interstitial_ads_fail_show = "ad_inter_fail";
        public const string ad_click_interstitial = "ad_inter_click";
        
        public const string show_rewarded_ads = "ads_reward_show";
        public const string ad_click_reward = "ads_reward_click";
        public const string rewarded_ads_available = "ads_reward_offer";
        public const string rewarded_ads_fail_show = "ads_reward_fail";
        
        public const string ad_click_banner = "ad_click_banner";
        
#elif PUB_RK
        public const string level_engage = "level_engage";
        public const string level_engage_passed = "level_engage_passed";
        public const string impdau_inter_passed = "impdau_inter_passed";
        public const string impdau_reward_passed = "impdau_reward_passed";
        public const string ad_impression_max = "ad_impression_rocket_max";
        public const string ad_impression_ironsource = "ad_impression_rocket_ironsource";

        public const string level_start = "level_start";
        public const string level_win = "level_win";
        public const string level_lose = "level_lose";

        public const string interstitial_show = "interstitial_show";
        public const string interstitial_ads_available = "ad_inter_load";
        public const string interstitial_ads_fail_show = "ad_inter_fail";
        public const string ad_click_interstitial = "ad_click_interstitial";

        public const string rewarded_video_show = "rewarded_video_show";
        public const string rewarded_ads_available = "ads_reward_offer";
        public const string rewarded_ads_fail_show = "ads_reward_fail";
        public const string ad_click_reward = "ad_click_reward";
        
        public const string ad_click_banner = "ad_click_banner";
#else
        public const string level_engage = "level_engage";
        public const string level_engage_passed = "level_engage_passed";
        public const string impdau_inter_passed = "impdau_inter_passed";
        public const string impdau_reward_passed = "impdau_reward_passed";
        public const string ad_impression_max = "ad_impression_max";
        public const string ad_impression_ironsource = "ad_impression_ironsource";

        public const string level_start = "level_start";
        public const string level_passed = "level_passed";
        public const string level_failed = "level_failed";

        public const string show_interstitial_ads = "show_interstitial_ads";
        public const string interstitial_ads_available = "ad_inter_load";
        public const string interstitial_ads_fail_show = "ad_inter_fail";
        public const string ad_click_interstitial = "ad_click_interstitial";

        public const string show_rewarded_ads = "show_rewarded_ads";
        public const string rewarded_ads_available = "ads_reward_offer";
        public const string rewarded_ads_fail_show = "ads_reward_fail";
        public const string ad_click_reward = "ad_click_reward";
        
        public const string ad_click_banner = "ad_click_banner";
#endif

        public const string level_reach = "level_reach";
        public const string days_playing = "days_playing";
        public const string ad_click_AOA = "ad_OnAdDidRecordImpression_aoa";

        #region Gameplay String

        public const string gold_earn = "gold_earn";
        public const string gold_spend = "gold_spend";
        
        public const string diamond_earn = "diamond_earn";
        public const string diamond_spend = "diamond_spend";

        #endregion
    }

    public class AppsFlyerStringEvent
    {
#if PUB_RK
        public const string interstitial_show = "interstitial_show";
        public const string rewarded_video_show = "rewarded_video_show";
        public const string rewarded_video_completed = "rewarded_video_completed";
#else
        public const string af_inters_api_called = "af_inters_api_called";
        public const string af_inters_ad_eligible = "af_inters_ad_eligible";
        public const string af_inters_displayed = "af_inters_displayed";
        
        public const string af_rewarded_ad_eligible = "af_rewarded_ad_eligible";
        public const string af_rewarded_api_called = "af_rewarded_api_called"; 
        public const string af_rewarded_ad_displayed = "af_rewarded_ad_displayed";
        public const string af_rewarded_ad_completed = "af_rewarded_ad_completed";
#endif
    }

    public class AnalyticsManager : MonoBehaviour
    {
        private static FirebaseManager m_FirebaseManager => FirebaseManager.Instance;

        public static void SetUserPropertyDayPlaying()
        {
            // Day Playing
            var daysPlayed = PlayerPrefs.GetInt("days_played", 0);
            var latestDayUpdateCount = PlayerPrefs.GetInt("latest_day", -1);

            if (latestDayUpdateCount != System.DateTime.Now.DayOfYear)
            {
                daysPlayed += 1;
                latestDayUpdateCount = System.DateTime.Now.DayOfYear;
                
                PlayerPrefs.SetInt("days_played", daysPlayed);
                PlayerPrefs.SetInt("latest_day", latestDayUpdateCount);
                
                if (m_FirebaseManager != null) m_FirebaseManager.SetUserProperty(FirebaseStringEvent.days_playing, daysPlayed.ToString());
            }
        }

        #region Gameplay Event

        public static void LogEventGoldEarn(int volume, string position)
        {
            if (m_FirebaseManager != null)
                m_FirebaseManager.LogAnalyticsEvent(FirebaseStringEvent.gold_earn,
                    new Parameter("volume", volume.ToString()),
                    new Parameter("position", position));
        }
        
        public static void LogEventGoldSpend(int volume, string position, string item)
        {
            if (m_FirebaseManager != null)
                m_FirebaseManager.LogAnalyticsEvent(FirebaseStringEvent.gold_spend,
                    new Parameter("volume", volume.ToString()),
                    new Parameter("position", position),
                    new Parameter("item",item));
        }
        
        public static void LogEventDiamondEarn(int volume, string position)
        {
            if (m_FirebaseManager != null)
                m_FirebaseManager.LogAnalyticsEvent(FirebaseStringEvent.diamond_earn,
                    new Parameter("volume", volume.ToString()),
                    new Parameter("position", position));
        }
        
        public static void LogEventDiamondSpend(int volume, string position, string item)
        {
            if (m_FirebaseManager != null)
                m_FirebaseManager.LogAnalyticsEvent(FirebaseStringEvent.diamond_spend,
                    new Parameter("volume", volume.ToString()),
                    new Parameter("position", position),
                    new Parameter("item",item));
        }

        #endregion

        public static void LogEventLevelEngage(int level, bool isWin)
        {
            if (m_FirebaseManager != null)
                m_FirebaseManager.LogAnalyticsEvent(FirebaseStringEvent.level_engage,
                    new Parameter("level", level.ToString()),
                    new Parameter("result", isWin ? "win" : "lose"));
        }

        public static void LogEventLevelEngagePassed(int engageWithLevel)
        {
            if (m_FirebaseManager != null)
                m_FirebaseManager.LogAnalyticsEvent(FirebaseStringEvent.level_engage_passed,
                    new Parameter("EngageWithLevel", engageWithLevel));
        }
        
        public static void LogEventLevelStart(int level)
        {
            if (m_FirebaseManager != null)
                m_FirebaseManager.LogAnalyticsEvent(FirebaseStringEvent.level_start,
                    new Parameter("level", level.ToString()));
            
#if USE_GA
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, level.ToString());
#endif
        }

        public static void LogEventLevelPassed(int level)
        {
            if (m_FirebaseManager != null)
            {
                m_FirebaseManager.LogAnalyticsEvent(FirebaseStringEvent.level_win,
                    new Parameter("level", level.ToString()));
                m_FirebaseManager.SetUserProperty(FirebaseStringEvent.level_reach, level.ToString());
            }
#if USE_GA
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, level.ToString());
#endif
        }
        
        public static void LogEventLevelPassed(int level, int time_played = 0)
        {
            if (m_FirebaseManager != null)
            {
                m_FirebaseManager.LogAnalyticsEvent(FirebaseStringEvent.level_win,
                    new Parameter("level", level.ToString()),
                    new Parameter("time", time_played.ToString()));
                m_FirebaseManager.SetUserProperty(FirebaseStringEvent.level_reach, level.ToString());
            }
#if USE_GA
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, level.ToString());
#endif
        }

        public static void LogEventLevelFailed(int level)
        {
            if (m_FirebaseManager != null)
                m_FirebaseManager.LogAnalyticsEvent(FirebaseStringEvent.level_lose,
                    new Parameter("level", level.ToString()));

#if USE_GA
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Fail, level.ToString());
#endif
        }
        
        public static void LogEventLevelFailed(int level, float fail_count = 0)
        {
            if (m_FirebaseManager != null)
                m_FirebaseManager.LogAnalyticsEvent(FirebaseStringEvent.level_lose,
                    new Parameter("level", level.ToString()),
                    new Parameter("fail_count", fail_count.ToString()));
            
#if USE_GA
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Fail, level.ToString());
#endif
        }
        
        public static void LogEventLevelFailed(int level, int time_played = 0)
        {
            if (m_FirebaseManager != null)
                m_FirebaseManager.LogAnalyticsEvent(FirebaseStringEvent.level_lose,
                    new Parameter("level", level.ToString()),
                    new Parameter("time", time_played.ToString()));
            
#if USE_GA
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Fail, level.ToString());
#endif
        }


        public static void LogEventImpdauInterPassed()
        {
            int impdauPassed = PlayerPrefs.GetInt(FirebaseStringEvent.impdau_inter_passed, 0);
            impdauPassed++;
            PlayerPrefs.SetInt(FirebaseStringEvent.impdau_inter_passed, impdauPassed);

            if (m_FirebaseManager != null)
                m_FirebaseManager.LogAnalyticsEvent(FirebaseStringEvent.impdau_inter_passed,
                    new Parameter("ImpdauPassed", impdauPassed));
#if PUB_RK || PUB_AD1 || PUB_FAL
            // Log appsflyer
            var para = new Dictionary<string, string>();
            para.Add("ImpdauPassed", impdauPassed.ToString());
            AppsFlyer.sendEvent(FirebaseStringEvent.impdau_inter_passed, para);
#endif
        
        }

        public static void LogEventImpdauRewardPassed()
        {
            int impdauPassed = PlayerPrefs.GetInt(FirebaseStringEvent.impdau_reward_passed, 0);
            impdauPassed++;
            PlayerPrefs.SetInt(FirebaseStringEvent.impdau_reward_passed, impdauPassed);

            if (m_FirebaseManager != null)
                m_FirebaseManager.LogAnalyticsEvent(FirebaseStringEvent.impdau_reward_passed,
                    new Parameter("ImpdauPassed", impdauPassed));
#if PUB_RK || PUB_AD1 || PUB_FAL
            // Log appsflyer
            var para = new Dictionary<string, string>();
            para.Add("ImpdauPassed", impdauPassed.ToString());
            AppsFlyer.sendEvent(FirebaseStringEvent.impdau_reward_passed, para);
#endif
        }
        

        public static void LogEventClickInterAds()
        {
            if (m_FirebaseManager != null)
                m_FirebaseManager.LogAnalyticsEvent(FirebaseStringEvent.ad_click_interstitial);
            
#if USE_GA
            string adSdkName = "admob";
#if AD_TYPE_APPLOVIN
            adSdkName = "max";
#elif AD_TYPE_IS
            adSdkName = "ironsource";
#endif
            GameAnalytics.NewAdEvent(GAAdAction.Clicked, GAAdType.Interstitial, adSdkName, "");
#endif
        }
        
        public static void LogEventClickRewardedVideoAds()
        {
            if (m_FirebaseManager != null)
                m_FirebaseManager.LogAnalyticsEvent(FirebaseStringEvent.ad_click_reward);
#if USE_GA
            string adSdkName = "admob";
#if AD_TYPE_APPLOVIN
            adSdkName = "max";
#elif AD_TYPE_IS
            adSdkName = "ironsource";
#endif
            GameAnalytics.NewAdEvent(GAAdAction.Clicked, GAAdType.RewardedVideo, adSdkName, "");
#endif
        }
        
        public static void LogEventClickBannerAds()
        {
            if (m_FirebaseManager != null)
                m_FirebaseManager.LogAnalyticsEvent(FirebaseStringEvent.ad_click_banner);
        }
        
        public static void LogEventClickAOAAds()
        { 
            if (m_FirebaseManager != null)
                m_FirebaseManager.LogAnalyticsEvent(FirebaseStringEvent.ad_click_AOA);
        }

        public static void LogEventShowInterstitial(int level, string placement)
        {
            if (m_FirebaseManager != null)
            {
                m_FirebaseManager.LogAnalyticsEvent(FirebaseStringEvent.interstitial_show,
                    new Parameter("level", level.ToString()),
                    new Parameter("placement", placement));
            }
#if PUB_RK || PUB_AD1
             var para = new Dictionary<string, string>();
             AppsFlyer.sendEvent(FirebaseStringEvent.interstitial_show, para);
#endif

#if USE_ADJUST
             AdjustManager.Instance.TrackAdEvent(AdjustEventKeys.ad_inters_ad_eligible); 
             AdjustManager.Instance.TrackAdEvent(AdjustEventKeys.ad_inters_displayed);
#endif

#if USE_GA
            string adSdkName = "admob";
#if AD_TYPE_APPLOVIN
            adSdkName = "max";
#elif AD_TYPE_IS
            adSdkName = "ironsource";
#endif
            GameAnalytics.NewAdEvent(GAAdAction.Show, GAAdType.Interstitial, adSdkName, placement);
#endif
        }
        
        public static void LogEventShowInterstitial(bool hasAds, string placement)
        {
            if (m_FirebaseManager != null)
            {
                m_FirebaseManager.LogAnalyticsEvent(FirebaseStringEvent.interstitial_show,
                    new Parameter("has_ads", hasAds ? "Yes" : "No"),
                    new Parameter("internet_available", HasInternet()),
                    new Parameter("placement", placement));
            }
#if PUB_RK || PUB_AD1
             var para = new Dictionary<string, string>();
             AppsFlyer.sendEvent(FirebaseStringEvent.interstitial_show, para);
#endif

#if USE_ADJUST
             AdjustManager.Instance.TrackAdEvent(AdjustEventKeys.ad_inters_ad_eligible);
             if (hasAds)
             {
                 AdjustManager.Instance.TrackAdEvent(AdjustEventKeys.ad_inters_displayed);
             }
#endif

#if USE_GA
            string adSdkName = "admob";
#if AD_TYPE_APPLOVIN
            adSdkName = "max";
#elif AD_TYPE_IS
            adSdkName = "ironsource";
#endif
            GameAnalytics.NewAdEvent(GAAdAction.Show, GAAdType.Interstitial, adSdkName, placement);
#endif
        }
        
        public static void LogEventInterstitialAvailable()
        {
            if (m_FirebaseManager != null)
                m_FirebaseManager.LogAnalyticsEvent(FirebaseStringEvent.interstitial_ads_available);

#if PUB_RK || PUB_AD1 || PUB_FAL
            var para = new Dictionary<string, string>();
            AppsFlyer.sendEvent(AppsFlyerStringEvent.interstitial_show, para);
#endif
#if USE_ADJUST
             AdjustManager.Instance.TrackAdEvent(AdjustEventKeys.ad_inters_api_called);
#endif
        }
        
        public static void LogEventInterstitialFailToShow(string errormsg)
        {
            if (m_FirebaseManager != null)
                m_FirebaseManager.LogAnalyticsEvent(FirebaseStringEvent.interstitial_ads_fail_show,
                new Parameter("errormsg", errormsg));
            
#if USE_GA
            string adSdkName = "admob";
#if AD_TYPE_APPLOVIN
            adSdkName = "max";
#elif AD_TYPE_IS
            adSdkName = "ironsource";
#endif
            GameAnalytics.NewAdEvent(GAAdAction.FailedShow, GAAdType.Interstitial, adSdkName, "");
#endif
        }
        
        public static void LogEventShowReward(int level, string placement)
        {
            if (m_FirebaseManager != null)
            {
                m_FirebaseManager.LogAnalyticsEvent(FirebaseStringEvent.rewarded_video_show,
                    new Parameter("level", level.ToString()),
                    new Parameter("placement", placement));
            }
#if PUB_RK || PUB_AD1
            var para = new Dictionary<string, string>();
            AppsFlyer.sendEvent(FirebaseStringEvent.rewarded_video_show, para);
#endif
#if USE_ADJUST
            AdjustManager.Instance.TrackAdEvent(AdjustEventKeys.ad_rewarded_ad_eligible);
            if (hasAds)
            {
                AdjustManager.Instance.TrackAdEvent(AdjustEventKeys.ad_rewarded_displayed);
            }
#endif
            
#if USE_GA
            string adSdkName = "admob";
#if AD_TYPE_APPLOVIN
            adSdkName = "max";
#elif AD_TYPE_IS
            adSdkName = "ironsource";
#endif
            GameAnalytics.NewAdEvent(GAAdAction.Show, GAAdType.RewardedVideo, adSdkName, placement);
#endif
        }
        
        public static void LogEventShowReward(bool hasAds, string placement)
        {
            if (m_FirebaseManager != null)
            {
                m_FirebaseManager.LogAnalyticsEvent(FirebaseStringEvent.rewarded_video_show,
                    new Parameter("has_ads", hasAds ? "Yes" : "No"),
                    new Parameter("internet_available", HasInternet()),
                    new Parameter("placement", placement));
            }
#if PUB_RK || PUB_AD1
            var para = new Dictionary<string, string>();
            AppsFlyer.sendEvent(FirebaseStringEvent.rewarded_video_show, para);
#endif
#if USE_ADJUST
            AdjustManager.Instance.TrackAdEvent(AdjustEventKeys.ad_rewarded_ad_eligible);
            if (hasAds)
            {
                AdjustManager.Instance.TrackAdEvent(AdjustEventKeys.ad_rewarded_displayed);
            }
#endif
            
#if USE_GA
            string adSdkName = "admob";
#if AD_TYPE_APPLOVIN
            adSdkName = "max";
#elif AD_TYPE_IS
            adSdkName = "ironsource";
#endif
            GameAnalytics.NewAdEvent(GAAdAction.Show, GAAdType.RewardedVideo, adSdkName, placement);
#endif
        }
        
        public static void LogEventRewardVideoAvailable()
        {
            if (m_FirebaseManager != null)
                m_FirebaseManager.LogAnalyticsEvent(FirebaseStringEvent.rewarded_ads_available);
#if PUB_RK || PUB_AD1 || PUB_FAL
            var para = new Dictionary<string, string>();
            AppsFlyer.sendEvent(AppsFlyerStringEvent.rewarded_video_show, para);
#endif
#if USE_ADJUST
             AdjustManager.Instance.TrackAdEvent(AdjustEventKeys.ad_rewarded_api_called);
#endif
        }

        public static void LogEventRewardVideoFailToShow(string errormsg)
        {
            if (m_FirebaseManager != null)
                m_FirebaseManager.LogAnalyticsEvent(FirebaseStringEvent.rewarded_ads_fail_show,
                    new Parameter("errormsg", errormsg));

#if USE_GA
            string adSdkName = "admob";
#if AD_TYPE_APPLOVIN
            adSdkName = "max";
#elif AD_TYPE_IS
            adSdkName = "ironsource";
#endif
            GameAnalytics.NewAdEvent(GAAdAction.FailedShow, GAAdType.RewardedVideo, adSdkName, "");
#endif
        }
        
        public static void LogEventRewardAdComplete()
        {
#if PUB_RK || PUB_AD1 || PUB_FAL
            var para = new Dictionary<string, string>();
            AppsFlyer.sendEvent(AppsFlyerStringEvent.rewarded_video_completed, para);
#endif
        }

        private static string HasInternet()
        {
            return Application.internetReachability == NetworkReachability.NotReachable ? "No" : "Yes";
        }
        
        #region Game Analytics
        // public static void GameLogEventStartLevelExtra(int level)
        // {
        //     m_FirebaseManager?.LogAnalyticsEvent(
        //         $"{FirebaseStringEvent.start_extra_mode_level}{level}",
        //         new Parameter("Level", level));
        //
        //     var fields = new Dictionary<string, object> {{"Level", level}};
        //     GameAnalytics.NewDesignEvent (FirebaseStringEvent.start_extra_mode_level, fields);
        // }
        
        // public static void GameLogFreeSpin()
        // {
        //     m_FirebaseManager?.LogAnalyticsEvent(
        //         $"{FirebaseStringEvent.free_spin}");
        //     
        //     GameAnalytics.NewDesignEvent (FirebaseStringEvent.free_spin);
        // }
        #endregion
    }
}