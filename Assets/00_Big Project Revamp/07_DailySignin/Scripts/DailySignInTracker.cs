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
            // LastClaimed intentionally not reset — prevents same-day double claim after reset
        }
    }
}