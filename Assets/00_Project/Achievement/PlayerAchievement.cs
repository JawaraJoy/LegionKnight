using System.Collections.Generic;
using UnityEngine;

namespace LegionKnight
{
    public class PlayerAchievement : AchievementHandler
    {
        
    }

    public partial class Player
    {
        [SerializeField]
        private PlayerAchievement m_Achievement;

        public string GetAchievementLabel(string id)
        {
            return m_Achievement.GetAchievementLabel(id);
        }
        public string GetAchievementUnclockedMessage(string id)
        {
            return m_Achievement.GetAchievementUnclockedMessage(id);
        }
        public bool IsAchievementUnlocked(string id)
        {
            return m_Achievement.IsAchievementUnlocked(id);
        }
        public void AddAchievementScore(string id, int score)
        {
            m_Achievement.AddScore(id, score);
        }
        public void SetAchievementScore(string id, int score)
        {
            m_Achievement.SetScore(id, score);
        }
        public void InitPlayerAchievement()
        {
            m_Achievement.Init();
        }
        public void ApplyAchievementGroup()
        {
            m_Achievement.ApplyAchievementGroup();
        }
        public void ApplyAchievement(AchieveDefinition achievement)
        {
            m_Achievement.ApplyAchievement(achievement);
        }
        public Achievement GetAchievementHold(AchieveDefinition defi)
        {
            return m_Achievement.GetAchievementHold(defi);
        }
        public List<Achievement> GetAchievementHolds()
        {
            return m_Achievement.GetAchievementHolds();
        }
    }
}
