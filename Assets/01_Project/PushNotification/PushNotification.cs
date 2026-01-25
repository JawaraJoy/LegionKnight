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

        [SerializeField]
        private bool m_IsAllowed = false;
        public bool IsAllowed => m_IsAllowed;

        private void Start()
        {
            bool hasKey = PlayerPrefs.HasKey("pushnotif");
            if (hasKey)
            {
                bool savedPref = PlayerPrefs.GetInt("pushnotif", 1) == 1;
                m_IsAllowed = savedPref;
            }
        }
        public void Init(bool allow)
        {
            m_IsAllowed = allow;
            PlayerPrefs.SetInt("pushnotif", m_IsAllowed ? 1 : 0);
            if (!m_IsAllowed)
            {
                return;
            }
            //Gley.Notifications.API.Initialize();
        }
        private void OnApplicationFocus(bool focus)
        {
            if (!m_IsAllowed)
            {
                return;
            }
            if (focus)
            {
                //Gley.Notifications.API.CancelAllNotifications();
            }
            else
            {
                TimeSpan delayFromNow = new(m_ScheduleInHour, m_ScheduleInMinutes, 0);
                //Gley.Notifications.API.SendNotification(m_GameTitle, m_Note, delayFromNow, m_SmallIconKey, m_LargeIconKey);
            }
        }
    }
}
