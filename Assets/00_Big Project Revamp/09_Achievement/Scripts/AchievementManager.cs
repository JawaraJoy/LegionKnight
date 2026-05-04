using UnityEngine;

namespace Rush
{
    public class AchievementManager : AchievementHandler { }

    public partial class RushPlayer
    {
        [SerializeField] private AchievementManager m_AchievementManager;
        public AchievementManager AchievementManager => m_AchievementManager;
    }
}