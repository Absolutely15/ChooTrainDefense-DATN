using System;
using System.Collections;
using System.Collections.Generic;
using ATSoft;
using ATSoft.Ads;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class AdvertisementSample : MonoBehaviour
{
    void Start ()
    {
        //StartCoroutine (DetectCountry());
    }

    IEnumerator DetectCountry ()
    {
        UnityWebRequest request = UnityWebRequest.Get ("https://extreme-ip-lookup.com/json");
        request.chunkedTransfer = false;
        yield return request.Send ();
        Debug.Log("Locating...");

        if (request.isNetworkError) {
            Debug.Log("error : " + request.error);
        } else {
            if (request.isDone) {
                Debug.Log(request.downloadHandler.text);
                Country res = JsonUtility.FromJson<Country> (request.downloadHandler.text);
                Debug.Log(res.country);
            }
        }
    }
    
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.A))
        {
            UnityAction actionComplete = delegate()
            {
                Debug.Log("ShowInterstitial");
                SceneName sceneNameToLoad = SceneName.Menu;
                SceneManager.LoadScene(sceneNameToLoad.ToString(), LoadSceneMode.Single);
            };
            Advertisements.Instance.ShowInterstitial(actionComplete);
        }
        else if (Input.GetKeyUp(KeyCode.B))
        {
            Advertisements.Instance.ShowBanner(BannerPosition.BOTTOM, BannerType.SmartBanner);
        }
        else if (Input.GetKeyUp(KeyCode.C))
        {
            UnityAction<bool> actionComplete = delegate(bool isSuccess)
            {
                Debug.Log("ShowRewardedVideo " + isSuccess);
            };
            Advertisements.Instance.ShowRewardedVideo(actionComplete, "placement");
        }
    }

    public void ShowBanner()
    {
        Advertisements.Instance.ShowBanner(BannerPosition.BOTTOM, BannerType.SmartBanner);
    }
    
    public void ShowSmartBanner()
    {
        Advertisements.Instance.ShowBanner(BannerPosition.BOTTOM, BannerType.SmartBanner);
    }
    
    public void ShowAdaptiveBanner()
    {
        Advertisements.Instance.ShowBanner(BannerPosition.BOTTOM, BannerType.Adaptive);
    }

    public void ShowInter()
    {
        UnityAction actionComplete = delegate()
        {
            Debug.Log("ShowInterstitial");
            SceneName sceneNameToLoad = SceneName.Menu;
            SceneManager.LoadScene(sceneNameToLoad.ToString(), LoadSceneMode.Single);
        };
        Advertisements.Instance.ShowInterstitial(actionComplete, "menu_1");
    }
    
    public void ShowInter2()
    {
        Advertisements.Instance.ShowInterstitial(null, "menu_2");
    }


    public void ShowRewardVideo()
    {
        UnityAction<bool> actionComplete = delegate(bool isSuccess)
        {
            Debug.Log("ShowRewardedVideo " + isSuccess);
        };
        Advertisements.Instance.ShowRewardedVideo(actionComplete, "placement");
    }
}

[Serializable]
public class Country
{

    public string businessName;
    public string businessWebsite;
    public string city;
    public string continent;
    public string country;
    public string countryCode;
    public string ipName;
    public string ipType;
    public string isp;
    public string lat;
    public string lon;
    public string org;
    public string query;
    public string region;
    public string status;

}