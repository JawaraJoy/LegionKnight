using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallAds : MonoBehaviour
{
    void Update()
    {
        // Contoh pemanggilan ShowRewardedAds pada suatu kondisi
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ShowRewardedAds();
        }
    }

    public void ShowRewardedAds()
    {
        if (AdsManager.Instance != null)
        {
            AdsManager.Instance.ShowRewarded();
        }
        else
        {
            Debug.LogError("AdsManager instance is not assigned.");
        }
    }

    public void ShowInterstitialAds()
    {
        if (AdsManager.Instance != null)
        {
            AdsManager.Instance.LoadInterstitial();
        }
        else
        {
            Debug.LogError("AdsManager instance is not assigned.");
        }
    }
}
