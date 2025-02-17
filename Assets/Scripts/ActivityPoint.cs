using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ActivityPoint : MonoBehaviour
{
    public static ActivityPoint Instance;

    public int activityPoint;
    public delegate void OnActivitypointChange(int activityPoint);
    public OnActivitypointChange onActivitypointChange;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    private void Start()
    {
        DataContainer data = DataManager.instance.LoadActivityTime();
        if(data != null)
        {
            AddOrSubActivityPoint(data.activityPoint);
        }
    }

    public void AddOrSubActivityPoint(int _activityPoint)
    {
        Mathf.Clamp(activityPoint += _activityPoint, 0, 100);
        DataManager.instance.SaveData(activityPoint);
        onActivitypointChange?.Invoke(activityPoint);
    }
}

