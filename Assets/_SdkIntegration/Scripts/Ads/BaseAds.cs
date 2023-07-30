using System;
using UnityEngine;
using UnityEngine.Events;

namespace ATSoft.Ads
{
    public class BaseAds : MonoBehaviour
    {
        protected bool initialized;
        protected UnityAction<bool, BannerPosition, BannerType> DisplayResult;
        protected UnityAction OnInterstitialClosed;
        protected UnityAction<bool> OnRewardVideoCompleteMethod;
        protected string bannerId;
        protected string interstitialId;
        protected string rewardedVideoId;
        protected BannerPosition bannerPosition;
        protected BannerType bannerType;
        protected int currentRetryInterstitial;
        protected int currentRetryRewardedVideo;
        protected bool autoDisableLoadingInter;
        protected float interstitialReadyTime;
        protected IDisposable obInter;
        protected readonly int maxRetryCount = 10;
        public float interstitialStartLevel = 0;
        protected bool enableConsent;

        public float InterstitialReadyTime => interstitialReadyTime;
        protected void Start()
        {
            interstitialReadyTime = 0;
        }

        protected virtual void OnDestroy()
        {
            
        }

        protected void ShowMessage(string msg)
        {
            AdsNotAvailable.Instance.Show();
            return;
#if !UNITY_EDITOR && UNITY_ANDROID
        AndroidJavaObject @static =
 new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject androidJavaObject = new AndroidJavaClass("android.widget.Toast");
        androidJavaObject.CallStatic<AndroidJavaObject>("makeText", new object[]
        {
                @static,
                msg,
                androidJavaObject.GetStatic<int>("LENGTH_SHORT")
        }).Call("show", Array.Empty<object>());
#endif
        }

        protected void OnInterstitialAvailable()
        {
            //LogEvent
#if PUB_RK //Rocket
#elif PUB_AD1 //Ad1
#elif PUB_FAL //Fal
            AnalyticsManager.LogEventInterstitialAvailable();
#else //In-house
            AnalyticsManager.LogEventInterstitialAvailable();
#endif
            Debug.Log(gameObject.name + "OnInterstitialAvailable()");
        }

        protected void OnInterstitialFailedToShow(string err)
        {
            //LogEvent
#if PUB_RK //Rocket
#elif PUB_AD1 //Ad1
#elif PUB_FAL //Fal 1
            AnalyticsManager.LogEventInterstitialFailToShow(err);
#else //In-house
            AnalyticsManager.LogEventInterstitialFailToShow(err);
#endif
            Debug.Log(gameObject.name + "LogEventInterstitialFailToShow() - " + err);
        }

        protected void OnRewardVideoAvailable()
        {
            //LogEvent
#if PUB_RK //Rocket
#elif PUB_AD1 //Ad1
#elif PUB_FAL //Fal
            AnalyticsManager.LogEventRewardVideoAvailable();
#else //In-house
            AnalyticsManager.LogEventRewardVideoAvailable();
#endif
            Debug.Log(gameObject.name + "OnRewardVideoAvailable()");
        }

        protected void OnRewardVideoFailedToShow(string err)
        {
            //LogEvent
#if PUB_RK //Rocket
#elif PUB_AD1 //Ad1
#elif PUB_FAL //Fal 1
            AnalyticsManager.LogEventRewardVideoFailToShow(err);
#else //In-house
            AnalyticsManager.LogEventRewardVideoFailToShow(err);
#endif
            Debug.Log(gameObject.name + "LogEventRewardVideoFailToShow() - " + err);
        }

        protected void OnRewardVideoCompleted()
        {
            //LogEvent
#if PUB_RK //Rocket
#elif PUB_AD1 //Ad1
#elif PUB_FAL //Fal 1
            AnalyticsManager.LogEventRewardAdComplete();
#else //In-house
            AnalyticsManager.LogEventRewardAdComplete();
#endif
            Debug.Log(gameObject.name + "LogEventRewardAdComplete()");
        }
    }
}

public class ImpressionData
{
    public string CountryCode;
    public string NetworkName;
    public string AdUnitIdentifier;
    public string Placement;
    public double Revenue;
    public string AdFormat;

}

public enum AdFormat
{
    interstitial,
    video_rewarded,
    banner
}