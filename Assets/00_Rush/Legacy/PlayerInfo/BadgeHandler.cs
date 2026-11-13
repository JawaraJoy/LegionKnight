using System;
using Rush;
using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public enum ProfileBadgeGoal { Event, PlayerLevel, PerfectStreak, LoginDays, OwnedHeroes, HeroLevel, BossKills, Spins, EnergySpent, DefeatSpecificBoss }

    public partial class BadgeHandler : MonoBehaviour
    {
        [SerializeField] private BadgeContent[] m_Badges;
        [SerializeField] private UnityEvent<BadgeContent> m_OnBadgeUnlocked;
        [SerializeField] private UnityEvent<BadgeContent> m_OnBadgeCurrentUpgradeLevelChanged;
        [SerializeField] private UnityEvent<LootField[]> m_OnRewardClaimed;
        public event Action Changed;

        public BadgeContent[] GetAllBadges() => m_Badges;
        public bool HasBadge(BadgeConfig definition, out BadgeContent content)
        {
            content = definition == null ? null : Array.Find(m_Badges, entry =>
                entry?.Config != null && entry.Config.BaseInfo.Id == definition.BaseInfo.Id);
            return content != null;
        }

        public void Init()
        {
            foreach (var badge in m_Badges) badge.Init();
            RecordLoginDay();
        }

        private void RecordLoginDay()
        {
            var save = UnityService.Instance;
            int today = (int)(DateTime.UtcNow.Date - new DateTime(1970, 1, 1)).TotalDays;
            const string lastDay = "ProfileBadge.LastLoginUtcDay";
            if (save.GetData<int>(lastDay) >= today) return;
            save.LocalSave.ExecuteTransaction(() =>
            {
                RecordProgress(ProfileBadgeGoal.LoginDays, 1);
                save.SaveData(lastDay, today);
            });
        }

        public void RecordProgress(ProfileBadgeGoal goal, int amount)
        {
            if (amount < 1) return;
            var save = UnityService.Instance;
            string key = "ProfileBadge.Progress." + goal;
            int previous = save.GetData<int>(key);
            int next = goal == ProfileBadgeGoal.PerfectStreak ? Mathf.Max(previous, amount) :
                (int)Math.Min(int.MaxValue, (long)previous + amount);
            if (next != previous) save.SaveData(key, next);
        }

        protected void RecordBossDefeat(BossUnitConfig boss)
        {
            foreach (var badge in m_Badges)
            {
                if (badge?.Config is not BossDefeatBadgeConfig target || target.Boss != boss) continue;
                if (badge.IsUnlocked || badge.CanClaim) return;
                badge.RefreshMilestones(1);
                Changed?.Invoke();
                return;
            }
        }

        public void RefreshMilestones()
        {
            RecordLoginDay();
            var player = Player.Instance;
            var heroes = player.HeroesCollection.HeroUnits;
            int owned = 0, highestLevel = 0;
            foreach (var hero in heroes)
            {
                if (!hero.Owned) continue;
                owned++;
                highestLevel = Mathf.Max(highestLevel, hero.Level);
            }
            foreach (var badge in m_Badges)
            {
                if (badge?.Config == null || badge.Config.Goal == ProfileBadgeGoal.Event ||
                    badge.Config.Goal == ProfileBadgeGoal.DefeatSpecificBoss) continue;
                int value = badge.Config.Goal switch
                {
                    ProfileBadgeGoal.PlayerLevel => player.GetPlayerLevel(),
                    ProfileBadgeGoal.OwnedHeroes => owned,
                    ProfileBadgeGoal.HeroLevel => highestLevel,
                    _ => UnityService.Instance.GetData<int>("ProfileBadge.Progress." + badge.Config.Goal)
                };
                badge.RefreshMilestones(value);
            }
        }

        public void SetCurrentUpgradeLevel(BadgeConfig defi, int level)
        {
            if (!HasBadge(defi, out var content)) return;
            content.SetCurrentUpgradeLevel(level);
            m_OnBadgeCurrentUpgradeLevelChanged?.Invoke(content);
            Changed?.Invoke();
        }

        public void ClaimReward(BadgeConfig defi) => TryClaimReward(defi, out _);

        public bool TryClaimReward(BadgeConfig defi, out LootField[] rewards)
        {
            rewards = null;
            if (!HasBadge(defi, out var badge)) return false;
            bool first = !badge.IsUnlocked;
            if (!badge.TryClaim(out rewards)) return false;
            if (first) m_OnBadgeUnlocked?.Invoke(badge);
            m_OnBadgeCurrentUpgradeLevelChanged?.Invoke(badge);
            m_OnRewardClaimed?.Invoke(rewards);
            Changed?.Invoke();
            return true;
        }

        public static LootField[] RewardOnUnlock(BadgeContent content) => content.Config.Upgrade[content.DisplayLevel].RewardOnUnlock;
    }

    [Serializable]
    public partial class BadgeContent
    {
        [SerializeField] private BadgeConfig m_Config;
        [SerializeField] private bool m_IsUnlocked;
        [SerializeField] private int m_CurrentUpgradeLevel;
        [SerializeField] private UnityEvent m_OnUnlocked;
        [SerializeField] private int m_UnlclaimedReward;
        private bool m_Claiming;
        public BadgeConfig Config => m_Config;
        public bool IsUnlocked => m_IsUnlocked;
        public int UnClaimedReward => m_UnlclaimedReward;
        public int CurrentUpgradeLevel => m_CurrentUpgradeLevel;
        private int ClaimedCount => m_IsUnlocked ? m_CurrentUpgradeLevel + 1 : 0;
        public bool CanClaim => !m_Claiming && m_UnlclaimedReward > 0 && ClaimedCount < m_Config.Upgrade.Length;
        public int DisplayLevel => CanClaim ? Mathf.Max(m_CurrentUpgradeLevel, ClaimedCount) : m_CurrentUpgradeLevel;
        private string UnlockKey => "isunlocked" + m_Config.BaseInfo.Id;
        private string PendingKey => "unclaimedreward" + m_Config.BaseInfo.Id;
        private string LevelKey => "currentupgradelevel" + m_Config.BaseInfo.Id;

        public BadgeContent(BadgeConfig badge) { m_Config = badge; }
        public void Init()
        {
            var save = UnityService.Instance;
            if (save.HasData(UnlockKey)) m_IsUnlocked = save.GetData<bool>(UnlockKey);
            if (save.HasData(LevelKey)) m_CurrentUpgradeLevel = save.GetData<int>(LevelKey);
            if (save.HasData(PendingKey)) m_UnlclaimedReward = save.GetData<int>(PendingKey);
            m_CurrentUpgradeLevel = Mathf.Clamp(m_CurrentUpgradeLevel, 0, Mathf.Max(0, m_Config.Upgrade.Length - 1));
            m_UnlclaimedReward = Mathf.Clamp(m_UnlclaimedReward, 0, m_Config.Upgrade.Length - ClaimedCount);
        }

        internal void RefreshMilestones(int value)
        {
            int earned = Array.FindAll(m_Config.Upgrade, tier => tier.Target > 0 && value >= tier.Target).Length;
            int pending = Mathf.Max(0, earned - ClaimedCount);
            if (pending > m_UnlclaimedReward) SetPending(pending);
        }

        private void SetPending(int count)
        {
            UnityService.Instance.SaveData(PendingKey, count);
            m_UnlclaimedReward = count;
        }

        public void AddUnClaimedReward(int count)
        {
            if (count <= 0) return;
            int next = (int)Math.Min((long)m_UnlclaimedReward + count, m_Config.Upgrade.Length - ClaimedCount);
            if (next == m_UnlclaimedReward) return;
            SetPending(next);
            CanvasManager.Instance.GetPanel<AchievementNotifPanel>()?.ShowNotif(this);
        }

        public void SetCurrentUpgradeLevel(int level)
        {
            level = Mathf.Clamp(level, 0, m_Config.Upgrade.Length - 1);
            if (level <= m_CurrentUpgradeLevel) return;
            UnityService.Instance.SaveData(LevelKey, level);
            m_CurrentUpgradeLevel = level;
        }

        public void ClaimReward() => Player.Instance.BadgeManager.ClaimReward(m_Config);

        internal bool TryClaim(out LootField[] rewards)
        {
            rewards = null;
            if (!CanClaim) return false;
            int tier = DisplayLevel;
            var payout = m_Config.Upgrade[tier].RewardOnUnlock;
            if (payout == null || payout.Length == 0 || Array.Exists(payout, row => row == null || row.ItemLoot == null || row.Amount <= 0)) return false;
            int pending = Mathf.Clamp(m_UnlclaimedReward - 1, 0, m_Config.Upgrade.Length - tier - 1);
            bool first = !m_IsUnlocked;
            m_Claiming = true;
            try
            {
                var result = new CollectibleResultData();
                foreach (var row in payout) result.AddEntry(row.ItemLoot, row.Amount);
                var save = UnityService.Instance;
                save.LocalSave.ExecuteTransaction(() =>
                {
                    result.GrantRewards(m_Config.BaseInfo.Id);
                    save.SaveData(LevelKey, tier);
                    save.SaveData(UnlockKey, true);
                    save.SaveData(PendingKey, pending);
                });
                m_CurrentUpgradeLevel = tier;
                m_IsUnlocked = true;
                m_UnlclaimedReward = pending;
                rewards = payout;
                if (first) m_OnUnlocked?.Invoke();
                return true;
            }
            finally { m_Claiming = false; }
        }
    }
}
