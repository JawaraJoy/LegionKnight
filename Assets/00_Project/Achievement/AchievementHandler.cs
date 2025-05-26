using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    [System.Serializable]
    public class Achievement
    {
        [SerializeField]
        private AchieveDefinition m_Definition;
        private bool m_IsAchieved;
        [SerializeField]
        private UnityEvent<Achievement> m_OnUnlocked;

        private int m_CurrentScore;
        public AchieveDefinition Definition => m_Definition;
        public bool IsAchieved => m_IsAchieved;

        public string Id => m_Definition.Id;
        public string Label => m_Definition.Label;
        public string UnclockedMessage => m_Definition.UnclockedMessage;
        public int UnlockScore => m_Definition.AchieveScore;

        private string CurrentScoreKey => $"Ach{m_Definition.Id}Sco";
        private string IsAchievedKey => $"Ach{m_Definition.Id}IsAch";

        public void Init()
        {
            if (UnityService.Instance.HasData(CurrentScoreKey))
            {
                m_CurrentScore = UnityService.Instance.GetData<int>(CurrentScoreKey);
            }
            else
            {
                m_CurrentScore = 0;
            }
            if (UnityService.Instance.HasData(IsAchievedKey))
            {
                m_IsAchieved = UnityService.Instance.GetData<bool>(IsAchievedKey);
            }
            else
            {
                m_IsAchieved = false;
            }
        }
        public void SetScore(int score)
        {
            m_CurrentScore = score;
            UnityService.Instance.SaveData(CurrentScoreKey, m_CurrentScore);
            if (m_CurrentScore >= m_Definition.AchieveScore)
            {
                OnAchievedInvoke();
            }
        }
        public void AddScore(int score)
        {
            m_CurrentScore += score;
            UnityService.Instance.SaveData(CurrentScoreKey, m_CurrentScore);
            if (m_CurrentScore >= m_Definition.AchieveScore)
            {
                OnAchievedInvoke();
            }
        }
        private void OnAchievedInvoke()
        {
            if (m_IsAchieved) return;
            m_OnUnlocked?.Invoke(this);
            m_IsAchieved = true;
            UnityService.Instance.SaveData(IsAchievedKey, m_IsAchieved);
        }
    }
    public class AchievementHandler : MonoBehaviour
    {
        [SerializeField]
        private Achievement[] m_Achievements;

        private readonly List<Achievement> m_AchievementHolds = new();

        //[SerializeField]
        //private UnityEvent<Achievement> m_OnAchievementUnlocked;

        [SerializeField]
        private UnityEvent<Achievement> m_OnAchievementApplied;
        [SerializeField]
        private UnityEvent<List<Achievement>> m_OnAchievementAppliedGroup;

        public void Init()
        {
            foreach (var achievement in m_Achievements)
            {
                achievement.Init();
            }
        }
        private Achievement GetAchievementHoldInternal(AchieveDefinition defi)
        {
            return m_AchievementHolds.Find(a => a.Definition.Id == defi.Id);
        }
        public Achievement GetAchievementHold(AchieveDefinition defi)
        {
            return GetAchievementHoldInternal(defi);
        }
        public List<Achievement> GetAchievementHolds()
        {
            return new List<Achievement>(m_AchievementHolds);
        }
        public void ApplyAchievementGroup()
        {
            if (m_AchievementHolds.Count > 0)
            {
                m_OnAchievementAppliedGroup?.Invoke(m_AchievementHolds);
                foreach (var achievement in m_AchievementHolds)
                {
                    m_OnAchievementApplied?.Invoke(achievement);
                }
                ClearAchievementHold();
            }
        }
        public void ApplyAchievement(AchieveDefinition defi)
        {
            Achievement holdAchievement = GetAchievementHoldInternal(defi);
            if (holdAchievement != null)
            {
                m_OnAchievementApplied?.Invoke(holdAchievement);
                RemoveAchievementHold(holdAchievement);
            }
            else
            {
                holdAchievement = GetAchievement(defi.Id);
                if (holdAchievement != null && holdAchievement.IsAchieved)
                {
                    m_OnAchievementApplied?.Invoke(holdAchievement);
                }
            }
        }
        private void AddAchievementHold(Achievement achievement)
        {
            if (!m_AchievementHolds.Contains(achievement))
            {
                m_AchievementHolds.Add(achievement);
            }
        }
        private void RemoveAchievementHold(Achievement achievement)
        {
            if (m_AchievementHolds.Contains(achievement))
            {
                m_AchievementHolds.Remove(achievement);
            }
        }
        private void ClearAchievementHold()
        {
            m_AchievementHolds.Clear();
        }
        private Achievement GetAchievement(string id)
        {
            foreach (var achievement in m_Achievements)
            {
                if (achievement.Id == id)
                {
                    return achievement;
                }
            }
            return null;
        }
        public string GetAchievementLabel(string id)
        {
            var achievement = GetAchievement(id);
            if (achievement != null)
            {
                return achievement.Label;
            }
            return string.Empty;
        }
        public string GetAchievementUnclockedMessage(string id)
        {
            var achievement = GetAchievement(id);
            if (achievement != null)
            {
                return achievement.UnclockedMessage;
            }
            return string.Empty;
        }
        public bool IsAchievementUnlocked(string id)
        {
            var achievement = GetAchievement(id);
            if (achievement != null)
            {
                return achievement.IsAchieved;
            }
            return false;
        }
        public void AddScore(string id, int score)
        {
            var achievement = GetAchievement(id);
            if (achievement != null)
            {
                achievement.AddScore(score);
                if (achievement.IsAchieved)
                {
                    AddAchievementHold(achievement);
                }
            }
        }
        public void SetScore(string id, int score)
        {
            var achievement = GetAchievement(id);
            if (achievement != null)
            {
                achievement.SetScore(score);
                if (achievement.IsAchieved)
                {
                    AddAchievementHold(achievement);
                }
            }
        }
    }
}
