using System;
using System.Collections.Generic;
using LegionKnight;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Rush
{
    public sealed class HeroBadgeDetailsView : MonoBehaviour
    {
        [Serializable]
        private sealed class HeroField
        {
            [SerializeField] private TMP_Text m_Name;
            [SerializeField] private TMP_Text m_Rarity;
            [SerializeField] private TMP_Text m_Count;
            [SerializeField] private Image m_Art;
            [SerializeField] private Image m_Rim;

            public void Display(HeroProfileBadgeConfig config, int claimed)
            {
                var hero = config.Hero.CollectibleField;
                var rarity = hero.RarityConfig;
                m_Name.text = config.BaseInfo.Name;
                m_Rarity.text = rarity != null ? rarity.BaseInfo.Name.ToUpperInvariant() : string.Empty;
                m_Rim.color = rarity != null ? rarity.Color : Color.white;
                m_Rarity.color = Color.Lerp(m_Rim.color, Color.white, .55f);
                m_Art.sprite = hero.SplashImage != null ? hero.SplashImage : hero.Icon;
                m_Count.text = $"{claimed} of {config.Upgrade.Length} claimed";
            }
        }

        [Serializable]
        private sealed class ActionField
        {
            [SerializeField] private Button m_Close;
            [SerializeField] private Button m_Claim;
            [SerializeField] private TMP_Text m_Label;
            public Button Close => m_Close;
            public Button Claim => m_Claim;
            public TMP_Text Label => m_Label;
        }

        [SerializeField] private HeroField m_Hero;
        [SerializeField] private ActionField m_Action;
        [SerializeField] private HeroBadgeRewardRow m_Template;
        private readonly List<HeroBadgeRewardRow> m_Rows = new();
        private ProfileBadgeListView m_List;
        private HeroProfileBadgeConfig m_Config;
        private BadgeContent m_Entry;

        private void Awake()
        {
            m_Action.Claim.onClick.AddListener(ClaimCurrent);
            m_Action.Close.onClick.AddListener(Close);
        }

        public void Bind(ProfileBadgeListView list, HeroProfileBadgeConfig config, BadgeContent entry)
        {
            m_List = list;
            m_Config = config;
            m_Entry = entry;
            RefreshInternal();
        }

        public void RefreshDetails() => RefreshInternal();

        private void RefreshInternal()
        {
            if (m_Entry == null) return;
            var tiers = m_Config.Upgrade;
            int claimed = m_Entry.IsUnlocked ? m_Entry.CurrentUpgradeLevel + 1 : 0;
            int earned = claimed + m_Entry.UnClaimedReward;
            m_Hero.Display(m_Config, claimed);
            for (int i = 0; i < tiers.Length; i++)
            {
                if (i == m_Rows.Count)
                    m_Rows.Add(Instantiate(m_Template, m_Template.transform.parent, false));
                var row = m_Rows[i];
                row.gameObject.SetActive(true);
                float height = 1f / Mathf.Max(2, tiers.Length);
                float top = .5f + (tiers.Length * .5f - i) * height;
                var rect = (RectTransform)row.transform;
                rect.anchorMin = new Vector2(0f, top - height);
                rect.anchorMax = new Vector2(1f, top);
                row.Display(m_Config, i, claimed, earned, m_Entry.CanClaim);
            }
            for (int i = tiers.Length; i < m_Rows.Count; i++) m_Rows[i].gameObject.SetActive(false);

            bool ready = m_Entry.CanClaim;
            m_Action.Claim.interactable = ready;
            m_Action.Label.text = ready ? "Claim " + m_Rows[m_Entry.DisplayLevel].RewardName :
                claimed == tiers.Length ? "All rewards claimed" : "Locked";
        }

        private void Close() => m_List.CloseDetails();
        private void ClaimCurrent() => m_List.Claim(m_Config);
    }
}

