using UnityEngine;

namespace LegionKnight
{
    public partial class BadgeManager : BadgeHandler
    {
        private void OnEnable()
        {
            Rush.PlatformHandler.PerfectStreakReached += RecordStreak;
            Rush.UnitConfig.BossDefeated += RecordBossDefeat;
            Rush.UnitConfig.BossKilled += RecordBoss;
            SpinWheel.SpinConsumed += RecordSpin;
            Energy.EnergySpent += RecordEnergy;
        }

        private void OnDisable()
        {
            Rush.PlatformHandler.PerfectStreakReached -= RecordStreak;
            Rush.UnitConfig.BossDefeated -= RecordBossDefeat;
            Rush.UnitConfig.BossKilled -= RecordBoss;
            SpinWheel.SpinConsumed -= RecordSpin;
            Energy.EnergySpent -= RecordEnergy;
        }

        private void RecordStreak(int count) => RecordProgress(ProfileBadgeGoal.PerfectStreak, count);
        private void RecordBoss() => RecordProgress(ProfileBadgeGoal.BossKills, 1);
        private void RecordSpin() => RecordProgress(ProfileBadgeGoal.Spins, 1);
        private void RecordEnergy(int amount) => RecordProgress(ProfileBadgeGoal.EnergySpent, amount);
    }
    public partial class Player
    {
        [SerializeField]
        private BadgeManager m_BadgeManager;
        public BadgeManager BadgeManager => m_BadgeManager;
    }
}
