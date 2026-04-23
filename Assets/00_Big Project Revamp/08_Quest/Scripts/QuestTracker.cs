using LegionKnight;
using System;
using UnityEngine;

namespace Rush
{
    public class QuestTracker : MonoBehaviour
    {
        private const string KeyCount = "Quest_Count_";
        private const string KeyClaimed = "Quest_Claimed_";
        private const string KeyLastReset = "Quest_LastReset_";
        private const string DateFormat = "yyyy-MM-dd HH:mm:ss";

        // ── Count ─────────────────────────────────────────────────────────────

        public int GetCount(QuestTaskConfig task)
        {
            string key = KeyCount + task.BaseInfo.Id;
            return UnityService.Instance.HasData(key)
                ? UnityService.Instance.GetData<int>(key) : 0;
        }

        public void SaveCount(QuestTaskConfig task, int count) =>
            UnityService.Instance.SaveData(KeyCount + task.BaseInfo.Id, count);

        // ── Claimed ───────────────────────────────────────────────────────────

        public bool IsClaimed(QuestTaskConfig task)
        {
            string key = KeyClaimed + task.BaseInfo.Id;
            return UnityService.Instance.HasData(key)
                && UnityService.Instance.GetData<bool>(key);
        }

        public void SaveClaimed(QuestTaskConfig task, bool claimed) =>
            UnityService.Instance.SaveData(KeyClaimed + task.BaseInfo.Id, claimed);

        // ── Last Reset ────────────────────────────────────────────────────────

        public DateTime? GetLastResetTime(QuestTaskConfig task)
        {
            string key = KeyLastReset + task.BaseInfo.Id;
            if (!UnityService.Instance.HasData(key)) return null;
            string raw = UnityService.Instance.GetData<string>(key);
            return DateTime.TryParseExact(raw, DateFormat,
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None, out var dt)
                ? dt : (DateTime?)null;
        }

        public void SaveLastResetTime(QuestTaskConfig task, DateTime time) =>
            UnityService.Instance.SaveData(
                KeyLastReset + task.BaseInfo.Id,
                time.ToString(DateFormat));

        // ── Reset ─────────────────────────────────────────────────────────────

        public void ResetTask(QuestTaskConfig task)
        {
            SaveCount(task, 0);
            SaveClaimed(task, false);
            SaveLastResetTime(task, DateTime.Now);
        }
    }
}