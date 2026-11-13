using System;
using System.Collections.Generic;
using LegionKnight;
using UnityEngine;

namespace Rush
{
    public sealed class ProfileBadgeListView : UIView
    {
        [Serializable]
        private sealed class DialogField
        {
            [SerializeField] private BadgeInfoPanel m_Panel;
            [SerializeField] private GameObject m_StandardDetails;
            [SerializeField] private HeroBadgeDetailsView m_HeroPrefab;
            [SerializeField] private LootedPanel m_Rewards;
            public BadgeInfoPanel Panel => m_Panel;
            public GameObject StandardDetails => m_StandardDetails;
            public HeroBadgeDetailsView HeroPrefab => m_HeroPrefab;
            public LootedPanel Rewards => m_Rewards;
        }

        [SerializeField] private Transform m_BadgeContainer;
        [SerializeField] private ProfileBadgeListItem m_CardPrefab;
        [SerializeField] private DialogField m_Dialog;
        private readonly Dictionary<BadgeConfig, ProfileBadgeListItem> m_Cards = new();
        private BadgeManager m_Manager;
        private HeroProfileBadgeTracker m_Tracker;
        private HeroBadgeDetailsView m_HeroView;

        protected override void ShowInternal()
        {
            base.ShowInternal();
            if (!IsShown) return;
            if (m_Manager == null)
            {
                m_Manager = Player.Instance.BadgeManager;
                m_Manager.Changed += RefreshInternal;
                if (m_Manager.TryGetComponent(out m_Tracker)) m_Tracker.ProgressChanged += RefreshInternal;
            }
            m_Manager.RefreshMilestones();
            RefreshInternal();
        }

        private void OnDestroy()
        {
            if (m_Manager != null) m_Manager.Changed -= RefreshInternal;
            if (m_Tracker != null) m_Tracker.ProgressChanged -= RefreshInternal;
        }

        private void RefreshInternal()
        {
            if (m_HeroView != null && m_HeroView.isActiveAndEnabled) m_HeroView.RefreshDetails();
            if (!IsShown || !m_Content.activeInHierarchy) return;
            foreach (var entry in m_Manager.GetAllBadges())
            {
                var config = entry?.Config;
                if (config == null || config.Upgrade.Length == 0) continue;
                if (config is BossDefeatBadgeConfig bossBadge && bossBadge.HiddenUntilDefeated &&
                    !entry.IsUnlocked && !entry.CanClaim) continue;
                if (config.Goal == ProfileBadgeGoal.Event && config is not HeroProfileBadgeConfig &&
                    !entry.IsUnlocked && !entry.CanClaim) continue;

                if (m_Cards.TryGetValue(config, out var card)) card.RefreshCard();
                else
                {
                    card = Instantiate(m_CardPrefab, m_BadgeContainer, false);
                    card.Bind(this, config);
                    m_Cards.Add(config, card);
                }
            }
        }

        public void OpenDetails(BadgeConfig config)
        {
            if (!m_Manager.HasBadge(config, out var entry)) return;
            bool hero = config is HeroProfileBadgeConfig;
            m_Dialog.StandardDetails.SetActive(!hero);
            if (hero)
            {
                if (m_HeroView == null)
                    m_HeroView = Instantiate(m_Dialog.HeroPrefab, m_Dialog.Panel.Content.transform, false);
                m_HeroView.gameObject.SetActive(true);
                m_HeroView.Bind(this, (HeroProfileBadgeConfig)config, entry);
            }
            else
            {
                if (m_HeroView != null) m_HeroView.gameObject.SetActive(false);
                m_Dialog.Panel.Init(config);
            }
            m_Dialog.Panel.Show();
        }

        public void CloseDetails() => m_Dialog.Panel.Hide();

        public void Claim(BadgeConfig config)
        {
            if (m_Dialog.Rewards.IsShown || !m_Manager.TryClaimReward(config, out var rewards)) return;
            m_Dialog.Rewards.ShowLoot(rewards);
        }
    }
}
