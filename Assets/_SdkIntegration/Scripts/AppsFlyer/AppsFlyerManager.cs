using System.Collections.Generic;
using UnityEngine;
using AppsFlyerSDK;

namespace ATSoft
{
    public class AppsFlyerManager : Singleton<AppsFlyerManager>, IService, IAppsFlyerConversionData
    {
        // These fields are set from the editor so do not modify!
        //******************************//
        public string devKey;
        public string appID;
        public bool isDebug;
        public bool getConversionData;
        //******************************//
        
        [HideInInspector] public bool initialized;
        
        public void Initialize()
        {
            Debug.Log("Initialize " + (typeof(AppsFlyerObjectScript)));
            
            // These fields are set from the editor so do not modify!
            //******************************//
            AppsFlyer.setIsDebug(isDebug);
#if PUB_AD1
            AppsFlyer.setHost("", "appsflyersdk.com");
#endif
            AppsFlyer.initSDK(devKey, appID, getConversionData ? this : null);
            //******************************//

            AppsFlyer.startSDK();
            AppsFlyerAdRevenue.start();
            initialized = true;
        }

        // Mark AppsFlyer CallBacks
        public void onConversionDataSuccess(string conversionData)
        {
            AppsFlyer.AFLog("didReceiveConversionData", conversionData);
            Dictionary<string, object> conversionDataDictionary = AppsFlyer.CallbackStringToDictionary(conversionData);
            // add deferred deeplink logic here
        }

        public void onConversionDataFail(string error)
        {
            AppsFlyer.AFLog("didReceiveConversionDataWithError", error);
        }

        public void onAppOpenAttribution(string attributionData)
        {
            AppsFlyer.AFLog("onAppOpenAttribution", attributionData);
            Dictionary<string, object> attributionDataDictionary = AppsFlyer.CallbackStringToDictionary(attributionData);
            // add direct deeplink logic here
        }

        public void onAppOpenAttributionFailure(string error)
        {
            AppsFlyer.AFLog("onAppOpenAttributionFailure", error);
        }
    }
}