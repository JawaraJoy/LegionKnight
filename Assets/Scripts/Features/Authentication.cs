using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using TMPro;
using System;
using UnityEngine.SocialPlatforms.Impl;
using CloudOnce;

public class Authentication : MonoBehaviour
{

    private void Start()
    {
        Cloud.OnInitializeComplete += OnCloudeInitComplete;
        Cloud.Initialize(false, true, false);
    }
    void OnCloudeInitComplete()
    {
        Cloud.OnInitializeComplete -= OnCloudeInitComplete;
        Debug.Log("Initialized");
        Debug.Log("Signin as : " + Cloud.PlayerDisplayName);

    }

    private void Cloud_OnCloudSaveComplete(bool arg0)
    {

    }

    private void Cloud_OnCloudLoadComplete(bool arg0)
    {

    }

}
