using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Globalvar : MonoBehaviour
{
    #region FIREBASE ANALYTICS
    /// <summary>
    /// Insert the analytics
    /// </summary>
    /// <param name="_parameterName">Parameter name</param>
    public static void CallAnalytics(string _parameterName)
    {
        //hanling exception for firebase
        try
        {
            Debug.Log($"Calling {_parameterName}");
            if (Application.isMobilePlatform)
            {
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }
    #endregion
}
