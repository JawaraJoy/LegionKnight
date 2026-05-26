using LegionKnight;
using System;
using UnityEngine;

namespace Rush
{
    public class DailySignInTracker : MonoBehaviour
    {
        private const string KeyCurrentDay = "SignIn_Day_";
        private const string KeyLastClaimed = "SignIn_LastClaimed_";
        private const string KeyCycleStart = "SignIn_CycleStart_";
        private const string DateFormat = "yyyy-MM-dd HH:mm:ss";

        // ── Save / Load ───────────────────────────────────────────────────────

        public int GetCurrentDay(DailySignInConfig config)
        {
            string key = KeyCurrentDay + config.BaseInfo.Id;
            return UnityService.Instance.HasData(key)
                ? UnityService.Instance.GetData<int>(key) : 0;
        }

        public void SaveCurrentDay(DailySignInConfig config, int day)
        {
            UnityService.Instance.SaveData(KeyCurrentDay + config.BaseInfo.Id, day);
        }

        public DateTime? GetLastClaimedTime(DailySignInConfig config)
        {
            string key = KeyLastClaimed + config.BaseInfo.Id;
            if (!UnityService.Instance.HasData(key)) return null;
            string raw = UnityService.Instance.GetData<string>(key);
            return DateTime.TryParseExact(raw, DateFormat,
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None, out var dt)
                ? dt : (DateTime?)null;
        }

        public void SaveLastClaimedTime(DailySignInConfig config, DateTime time)
        {
            UnityService.Instance.SaveData(
                KeyLastClaimed + config.BaseInfo.Id,
                time.ToString(DateFormat));
        }
        public bool IsMissedDayClaimed(DailySignInConfig config, int dayIndex)
        {
            string key = $"{config.BaseInfo.Id}_missed_{dayIndex}";

            if (!UnityService.Instance.HasData(key))
                return false;

            return UnityService.Instance.GetData<bool>(key);
        }

        public void SaveMissedDayClaimed(
            DailySignInConfig config,
            int dayIndex)
        {
            string key = $"{config.BaseInfo.Id}_missed_{dayIndex}";

            UnityService.Instance.SaveData(key, true);
        }
        public DateTime? GetCycleStartTime(DailySignInConfig config)
        {
            string key = KeyCycleStart + config.BaseInfo.Id;
            if (!UnityService.Instance.HasData(key)) return null;
            string raw = UnityService.Instance.GetData<string>(key);
            return DateTime.TryParseExact(raw, DateFormat,
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None, out var dt)
                ? dt : (DateTime?)null;
        }

        public void SaveCycleStartTime(DailySignInConfig config, DateTime time)
        {
            UnityService.Instance.SaveData(
                KeyCycleStart + config.BaseInfo.Id,
                time.ToString(DateFormat));
        }

        public void ResetCycle(DailySignInConfig config, DateTime cycleStart)
        {
            SaveCurrentDay(config, 0);
            SaveCycleStartTime(config, cycleStart);
            for (int i = 0; i < config.TotalDays; i++)
            {
                string key = $"{config.BaseInfo.Id}_missed_{i}";

                if (UnityService.Instance.HasData(key))
                {
                    UnityService.Instance.DeleteData(key);
                }
            }
            // LastClaimed intentionally not reset — prevents same-day double claim after reset
        }
    }
}