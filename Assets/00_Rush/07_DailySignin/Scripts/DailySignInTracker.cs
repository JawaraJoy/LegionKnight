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

        private string ClaimedDaysKey(DailySignInConfig config) => "SignIn_ClaimedDays_" + config.BaseInfo.Id;

        private int[] ClaimedDaysInternal(DailySignInConfig config)
        {
            string key = ClaimedDaysKey(config);
            if (UnityService.Instance.HasData(key)) return UnityService.Instance.GetData<int[]>(key) ?? Array.Empty<int>();
            // One-time migration: old progression meant every earlier reward was claimed.
            int count = Mathf.Clamp(GetCurrentDay(config), 0, config.TotalDays);
            var days = new int[count];
            for (int i = 0; i < count; i++) days[i] = i;
            return days;
        }

        public bool IsDayClaimed(DailySignInConfig config, int dayIndex)
        {
            if (dayIndex < 0 || dayIndex >= config.TotalDays) return false;
            return Array.IndexOf(ClaimedDaysInternal(config), dayIndex) >= 0 || IsMissedDayClaimed(config, dayIndex);
        }

        public void SaveDayClaimed(DailySignInConfig config, int dayIndex)
        {
            var days = new System.Collections.Generic.List<int>(ClaimedDaysInternal(config));
            if (!days.Contains(dayIndex)) days.Add(dayIndex);
            UnityService.Instance.SaveData(ClaimedDaysKey(config), days.ToArray());
        }

        public void ResetCycle(DailySignInConfig config, DateTime cycleStart)
        {
            UnityService.Instance.LocalSave.ExecuteTransaction(() =>
            {
                SaveCurrentDay(config, 0);
                SaveCycleStartTime(config, cycleStart);
                UnityService.Instance.SaveData(ClaimedDaysKey(config), Array.Empty<int>());
                for (int i = 0; i < config.TotalDays; i++)
                {
                    string key = $"{config.BaseInfo.Id}_missed_{i}";

                    if (UnityService.Instance.HasData(key))
                    {
                        UnityService.Instance.DeleteData(key);
                    }
                }
            });
            // LastClaimed intentionally not reset — prevents same-day double claim after reset
        }
    }
}