using System;
using UnityEngine;
using UnityEngine.Events;
using LegionKnight;

namespace Rush
{
    public class DailyCheckIn : MonoBehaviour
    {

        [Header("Reset Time (24h Format)")]
        [SerializeField] private int m_ResetHour = 4;
        [SerializeField] private int m_ResetMinute = 0;

        [Header("Events")]
        [SerializeField] private UnityEvent m_OnFirstCheckInToday;

        private const string SAVE_KEY = "DAILY_CHECK_IN_LAST_TIME";


        public void CheckIn()
        {
            DateTime now = DateTime.Now;

            DateTime lastCheckInTime = GetLastCheckInTime();

            DateTime todayResetTime = GetTodayResetTime(now);

            // Kalau sekarang masih sebelum reset hari ini → reset terakhir adalah kemarin
            if (now < todayResetTime)
            {
                todayResetTime = todayResetTime.AddDays(-1);
            }

            // Jika belum check-in hari ini
            if (lastCheckInTime < todayResetTime)
            {
                m_OnFirstCheckInToday?.Invoke();
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
            // simpan sebagai string ISO
            UnityService.Instance.SaveData(SAVE_KEY, time.ToString("o"));
        }

        private DateTime GetLastCheckInTime()
        {
            if (!UnityService.Instance.HasData(SAVE_KEY))
                return DateTime.MinValue;

            string saved = UnityService.Instance.GetData<string>(SAVE_KEY);

            if (DateTime.TryParse(saved, out DateTime result))
                return result;

            return DateTime.MinValue;
        }
    }
}