using LegionKnight;
using UnityEngine;

namespace Rush
{
    public class AchievementTracker : MonoBehaviour
    {
        private const string KeyCount = "Achievement_Count_";
        private const string KeyClaimed = "Achievement_Claimed_";

        public int GetCount(AchievementTaskConfig task)
        {
            string key = KeyCount + task.BaseInfo.Id;
            return UnityService.Instance.HasData(key)
                ? UnityService.Instance.GetData<int>(key) : 0;
        }

        public void SaveCount(AchievementTaskConfig task, int count) =>
            UnityService.Instance.SaveData(KeyCount + task.BaseInfo.Id, count);

        public bool IsClaimed(AchievementTaskConfig task)
        {
            string key = KeyClaimed + task.BaseInfo.Id;
            return UnityService.Instance.HasData(key)
                && UnityService.Instance.GetData<bool>(key);
        }

        public void SaveClaimed(AchievementTaskConfig task, bool claimed) =>
            UnityService.Instance.SaveData(KeyClaimed + task.BaseInfo.Id, claimed);
    }
}