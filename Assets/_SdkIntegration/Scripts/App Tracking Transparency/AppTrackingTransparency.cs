using System;
using System.Collections;
using UniRx;
using UnityEngine;

namespace ATSoft
{
    public class AppTrackingTransparency : Singleton<AppTrackingTransparency>, IService
    {
        [HideInInspector] public bool initialized;
        public void Initialize()
        {
            initialized = false;
            
#if UNITY_IOS
            //App Tracking Transparency
            var status = Unity.Advertisement.IosSupport.ATTrackingStatusBinding.GetAuthorizationTrackingStatus();

            if (status == Unity.Advertisement.IosSupport.ATTrackingStatusBinding.AuthorizationTrackingStatus.NOT_DETERMINED)
            {
                Unity.Advertisement.IosSupport.ATTrackingStatusBinding.RequestAuthorizationTracking();
            }
#endif 
            StartCoroutine(WaitForConsent());
        }

        private IEnumerator WaitForConsent()
        {
#if UNITY_IOS && !UNITY_EDITOR
        Version ver = Version.Parse(UnityEngine.iOS.Device.systemVersion);
        if (ver.Major >= 14)
        {
            var status = Unity.Advertisement.IosSupport.ATTrackingStatusBinding.GetAuthorizationTrackingStatus();
            while (status == Unity.Advertisement.IosSupport.ATTrackingStatusBinding.AuthorizationTrackingStatus.NOT_DETERMINED)
            {
                status = Unity.Advertisement.IosSupport.ATTrackingStatusBinding.GetAuthorizationTrackingStatus();
                yield return null;
            }

            switch (status)
            {
                case Unity.Advertisement.IosSupport.ATTrackingStatusBinding.AuthorizationTrackingStatus.AUTHORIZED:
                    //SetUserConsent(true);
                    break;
                case Unity.Advertisement.IosSupport.ATTrackingStatusBinding.AuthorizationTrackingStatus.DENIED:
                    //SetUserConsent(false);
                    break;
                default:
                    //SetUserConsent(true);
                    break;
            }

            initialized = true;
        }
        else
        {
            initialized = true;
        }
#else
            initialized = true;
#endif
            yield return null;
        }
    }
}