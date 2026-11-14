using System.Collections;
using System.Collections.Generic;
using LegionKnight;
using MoreMountains.Feedbacks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Rush
{
    public sealed class BadgeUnlockedBanner : PanelView
    {
        [System.Serializable]
        private sealed class BannerField
        {
            [SerializeField] internal Image icon, rarityRim, rewardIcon;
            [SerializeField] internal TMP_Text title, reward;
            [SerializeField] internal CanvasGroup surface;
            [SerializeField] internal MMF_Player fade;
            [SerializeField] internal CanvasGroup bossHud;
            [SerializeField] internal RectTransform card;
            [SerializeField] internal float topMargin = 60f, topMarginWithBoss = 166f;
            [SerializeField, Min(0)] internal float readingSeconds = 4.5f;
        }

        [SerializeField] private BadgeUnlockChannel m_Channel;
        [SerializeField] private BannerField m_Banner;
        private readonly Queue<(BadgeContent badge, int tier)> m_Queue = new();
        private Coroutine m_Playback;

        private void OnEnable() => m_Channel.Earned += OnEarnedInternal;

        private void OnDisable()
        {
            m_Channel.Earned -= OnEarnedInternal;
            if (m_Playback != null) StopCoroutine(m_Playback);
            m_Playback = null;
            m_Queue.Clear();
            m_Content.SetActive(false);
        }

        private void LateUpdate()
        {
            if (m_Playback == null || !m_Content.activeSelf) return;
            PositionInternal();
        }

        private void PositionInternal()
        {
            bool bossVisible = m_Banner.bossHud.isActiveAndEnabled && m_Banner.bossHud.alpha > 0f;
            m_Banner.card.anchoredPosition = Vector2.down * (bossVisible ?
                m_Banner.topMarginWithBoss : m_Banner.topMargin);
        }

        private void OnEarnedInternal(BadgeContent badge, int tier)
        {
            m_Queue.Enqueue((badge, tier));
            if (m_Playback == null) m_Playback = StartCoroutine(PlayQueueInternal());
        }

        private IEnumerator PlayQueueInternal()
        {
            // Defer until battle and badge callbacks have completed, including Profile claims.
            yield return null;
            while (m_Queue.Count > 0)
            {
                var (badge, tierIndex) = m_Queue.Dequeue();
                if (badge.IsUnlocked && badge.CurrentUpgradeLevel >= tierIndex) continue;

                var config = badge.Config;
                var tier = config.Upgrade[tierIndex];
                var art = config.CollectibleField;
                m_Banner.title.text = string.IsNullOrWhiteSpace(tier.Label) ? config.BaseInfo.Name : tier.Label;
                m_Banner.icon.sprite = tier.Icon != null ? tier.Icon : art.Icon;
                m_Banner.icon.enabled = m_Banner.icon.sprite != null;
                m_Banner.rarityRim.color = art.RarityConfig != null ? art.RarityConfig.Color : Color.white;

                if (tier.RewardOnUnlock == null) continue;
                foreach (var row in tier.RewardOnUnlock)
                {
                    if (row?.ItemLoot == null || row.Amount < 1) continue;
                    if (badge.IsUnlocked && badge.CurrentUpgradeLevel >= tierIndex) break;
                    var item = row.ItemLoot;
                    m_Banner.rewardIcon.sprite = item.CollectibleField.Icon;
                    m_Banner.rewardIcon.enabled = m_Banner.rewardIcon.sprite != null;
                    var name = item.BaseInfo.Name;
                    if (config is HeroProfileBadgeConfig && item is CustomImageDefinition photo && photo.Type == CustomImageType.Icon)
                        name = config.BaseInfo.Name + " Photo";
                    else if (name == "Diamond" && row.Amount > 1)
                        name = "Diamonds";
                    m_Banner.reward.text = row.Amount == 1 ? name : $"{row.Amount:N0} {name}";
                    PositionInternal();
                    m_Banner.surface.alpha = 1f;
                    m_Content.SetActive(true);
                    yield return new WaitForSecondsRealtime(m_Banner.readingSeconds);
                    if (m_Banner.fade.gameObject.activeInHierarchy)
                    {
                        m_Banner.fade.PlayFeedbacks();
                        yield return new WaitForSecondsRealtime(m_Banner.fade.TotalDuration);
                    }
                    m_Content.SetActive(false);
                }
            }
            m_Playback = null;
        }
    }
}
