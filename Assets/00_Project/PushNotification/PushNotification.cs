using System;
using UnityEngine;

namespace LegionKnight
{
    public class PushNotification : MonoBehaviour
    {
        [SerializeField]
        private string m_GameTitle = "Aether Jump";
        [SerializeField]
        private string m_Note;
        [SerializeField]
        private string m_SmallIconKey = "icon_0";
        [SerializeField]
        private string m_LargeIconKey = "icon_1";
        [SerializeField]
        private int m_ScheduleInHour = 12;
        [SerializeField]
        private int m_ScheduleInMinutes = 1;

        private void Start()
        {
            Gley.Notifications.API.Initialize();
        }
        private void OnApplicationFocus(bool focus)
        {
            if (focus)
            {
                Gley.Notifications.API.CancelAllNotifications();
            }
            else
            {
                TimeSpan delayFromNow = new(m_ScheduleInHour, m_ScheduleInMinutes, 0);  
                Gley.Notifications.API.SendNotification(m_GameTitle, m_Note, delayFromNow, m_SmallIconKey, m_LargeIconKey);
            }
        }
    }
}
