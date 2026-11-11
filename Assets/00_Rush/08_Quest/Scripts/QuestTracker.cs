using LegionKnight;
using System;
using UnityEngine;

namespace Rush
{
    public class QuestTracker : MonoBehaviour
    {
        private const string KeyCount = "Quest_Count_";
        private const string KeyClaimed = "Quest_Claimed_";
        private const string KeyLastReset = "Quest_CatalogReset_";  // per catalog
        private const string DateFormat = "yyyy-MM-dd HH:mm:ss";

        // ── Task Count ────────────────────────────────────────────────────────

        public int GetCount(QuestTaskConfig task)
        {
            string key = KeyCount + task.BaseInfo.Id;
            return UnityService.Instance.HasData(key)
                ? UnityService.Instance.GetData<int>(key) : 0;
        }

        public void SaveCount(QuestTaskConfig task, int count) =>
            UnityService.Instance.SaveData(KeyCount + task.BaseInfo.Id, count);

        // ── Task Claimed ──────────────────────────────────────────────────────

        public bool IsClaimed(QuestTaskConfig task)
        {
            string key = KeyClaimed + task.BaseInfo.Id;
            return UnityService.Instance.HasData(key)
                && UnityService.Instance.GetData<bool>(key);
        }

        public void SaveClaimed(QuestTaskConfig task, bool claimed) =>
            UnityService.Instance.SaveData(KeyClaimed + task.BaseInfo.Id, claimed);

        // ── Catalog Reset Time ────────────────────────────────────────────────

        public DateTime? GetLastResetTime(QuestCatalogConfig catalog)
        {
            string key = KeyLastReset + catalog.BaseInfo.Id;
            if (!UnityService.Instance.HasData(key)) return null;
            string raw = UnityService.Instance.GetData<string>(key);
            return DateTime.TryParseExact(raw, DateFormat,
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None, out var dt)
                ? dt : (DateTime?)null;
        }

        public void SaveLastResetTime(QuestCatalogConfig catalog, DateTime time) =>
            UnityService.Instance.SaveData(
                KeyLastReset + catalog.BaseInfo.Id,
                time.ToString(DateFormat));

        // ── Reset all tasks in catalog ────────────────────────────────────────

        public void ResetCatalog(QuestCatalogConfig catalog)
        {
            if (catalog.Tasks == null) return;
            foreach (var task in catalog.Tasks)
            {
                if (task == null) continue;
                SaveCount(task, 0);
                SaveClaimed(task, false);
            }
            SaveLastResetTime(catalog, DateTime.Now);
        }
    }
}