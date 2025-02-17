using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Notifications.Android;
using UnityEngine;

public class UnityNotification : MonoBehaviour
{
    public static UnityNotification Instance { get;private set; }

    private int identifier;
    private void Awake()
    {
        if (!Instance)
            Instance = this;
        else
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        AndroidNotificationCenter.OnNotificationReceived += AndroidNotificationCenter_OnNotificationReceived;
        var notifIntentData = AndroidNotificationCenter.GetLastNotificationIntent();
        if (notifIntentData != null)
        {
            Actions.OpenGameWithNotif.Invoke();
        }
    }
    private void OnDisable()
    {
        AndroidNotificationCenter.OnNotificationReceived -= AndroidNotificationCenter_OnNotificationReceived;

    }
    private void AndroidNotificationCenter_OnNotificationReceived(AndroidNotificationIntentData data)
    {
        AndroidNotificationCenter.CancelNotification(identifier);
    }

    public void SendNotification(string topic,string desc,DateTime dateTime)
    {
        var notifChannel = new AndroidNotificationChannel()
        {
            Id = topic,
            Name = topic,
            Description = "For Generic Notification",
            Importance = Importance.Default
        };
        AndroidNotificationCenter.RegisterNotificationChannel(notifChannel);

        AndroidNotification notification = new AndroidNotification()
        {
            Title = "Knight Jumper",
            Text = desc,
            SmallIcon = "default",
            LargeIcon = "default",
            FireTime = dateTime
        };
        identifier = AndroidNotificationCenter.SendNotification(notification, topic);
    }
    public void CancelNotification()
    {
        AndroidNotificationCenter.CancelNotification(identifier);

    }
}
