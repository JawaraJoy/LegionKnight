using System;
using UnityEngine;
using UnityEngine.Events;

namespace Rush
{
    public class DailyCheckIn : MonoBehaviour
    {
        [Header("Reset Time (24h Format)")]
        [SerializeField] private int m_ResetHour = 4; // Jam reset (contoh: 4 = jam 04:00)
        [SerializeField] private int m_ResetMinute = 0;

        [Header("Events")]
        [SerializeField] private UnityEvent m_OnFirstCheckInToday;

        private const string PREF_KEY = "DAILY_CHECK_IN_LAST_TIME";

        private void Start()
        {
            CheckIn();
        }

        public void CheckIn()
        {
            DateTime now = DateTime.Now;

            DateTime lastCheckInTime = GetLastCheckInTime();

            DateTime todayResetTime = GetTodayResetTime(now);

            // Kalau sekarang masih sebelum reset hari ini, berarti reset terakhir itu kemarin
            if (now < todayResetTime)
            {
                todayResetTime = todayResetTime.AddDays(-1);
            }

            // Jika last check-in sebelum reset terakhir → berarti belum check-in hari ini
            if (lastCheckInTime < todayResetTime)
            {
                // Trigger event
                m_OnFirstCheckInToday?.Invoke();

                // Save waktu check-in sekarang
                SaveCheckInTime(now);
            }
        }

        private DateTime GetTodayResetTime(DateTime currentTime)
        {
            return new DateTime(
                currentTime.Year,
                currentTime.Month,
                currentTime.Day,
                m_ResetHour,
                m_ResetMinute,
                0
            );
        }

        private void SaveCheckInTime(DateTime time)
        {
            PlayerPrefs.SetString(PREF_KEY, time.ToString("o")); // ISO format
            PlayerPrefs.Save();
        }

        private DateTime GetLastCheckInTime()
        {
            if (!PlayerPrefs.HasKey(PREF_KEY))
                return DateTime.MinValue;

            string saved = PlayerPrefs.GetString(PREF_KEY);

            if (DateTime.TryParse(saved, out DateTime result))
                return result;

            return DateTime.MinValue;
        }
    }
}