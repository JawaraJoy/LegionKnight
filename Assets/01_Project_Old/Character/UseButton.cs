using System;
using MoreMountains.Tools;
using Rush;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LegionKnight
{
    public class UseButton : MonoBehaviour, IUpdater
    {
        private const string TrialAdsTimeKey = "TRIAL_ADS_TIME";

        [Header("Trial Reset")]
        [SerializeField]
        private int m_ResetAfterHours = 24;

        [SerializeField]
        private Button m_UseButton;

        [SerializeField]
        private Button m_WatchToUTryOnceButton;

        [SerializeField]
        private TextMeshProUGUI m_UseText;

        [SerializeField]
        private TextMeshProUGUI m_WatchToTryOnceText;

        [SerializeField, MMReadOnly]
        private HeroUnit m_HeroUnit;

        public bool IsActive => gameObject.activeInHierarchy;

        private void Start()
        {
            m_UseButton.onClick.AddListener(Use);
            m_WatchToUTryOnceButton.onClick.AddListener(WatchToTryOnce);

            UnityService.Instance.LoadRewardedAd();

            UpdateBank.Instance.RegisterUpdateTick(gameObject, this);
        }

        public void Init(HeroUnitConfig heroConfig)
        {
            HeroUnit unit = Player.Instance.HeroesCollection.GetHeroUnit(heroConfig);
            m_HeroUnit = unit;

            Refresh();
        }

        private void Refresh()
        {
            bool isHeroUnlocked = m_HeroUnit.Owned;
            bool isCharacterUsed = m_HeroUnit.IsUsed;
            bool isOnTrial = m_HeroUnit.OnTrial;

            // =========================
            // USE BUTTON
            // =========================

            m_UseButton.interactable = !isCharacterUsed && (isHeroUnlocked || isOnTrial);

            if (!isHeroUnlocked && !isOnTrial)
            {
                m_UseText.text = "Locked";
            }
            else
            {
                m_UseText.text = isCharacterUsed ? "Used" : "Use";
            }

            // =========================
            // WATCH TO TRY BUTTON
            // =========================

            m_WatchToUTryOnceButton.gameObject.SetActive(!isHeroUnlocked);

            Tick();
        }

        public void Tick()
        {
            bool isInCooldown = IsInCooldown();

            m_WatchToUTryOnceButton.interactable = !isInCooldown;

            if (isInCooldown)
            {
                int remainingHours = GetRemainingHours();

                m_WatchToTryOnceText.text = $"{remainingHours}H Left";
            }
            else
            {
                m_WatchToTryOnceText.text = "Watch Trial";
            }
        }

        private void Use()
        {
            Player.Instance.HeroesCollection.SetUsedHero();
            Refresh();
        }

        private void WatchToTryOnce()
        {
            UnityService.Instance.ShowRewardedAd(UnlockHero);
        }

        private void UnlockHero()
        {
            SaveCurrentTime();

            m_HeroUnit.SetTrial(true);

            Use();

            Refresh();
        }

        private void SaveCurrentTime()
        {
            long currentTicks = DateTime.UtcNow.Ticks;

            PlayerPrefs.SetString(TrialAdsTimeKey, currentTicks.ToString());
            PlayerPrefs.Save();
        }

        private bool IsInCooldown()
        {
            string savedTime = PlayerPrefs.GetString(TrialAdsTimeKey, string.Empty);

            if (string.IsNullOrEmpty(savedTime))
            {
                return false;
            }

            if (!long.TryParse(savedTime, out long ticks))
            {
                return false;
            }

            DateTime lastWatchTime = new DateTime(ticks, DateTimeKind.Utc);

            TimeSpan elapsed = DateTime.UtcNow - lastWatchTime;

            return elapsed.TotalHours < m_ResetAfterHours;
        }

        private int GetRemainingHours()
        {
            string savedTime = PlayerPrefs.GetString(TrialAdsTimeKey, string.Empty);

            if (string.IsNullOrEmpty(savedTime))
            {
                return 0;
            }

            if (!long.TryParse(savedTime, out long ticks))
            {
                return 0;
            }

            DateTime lastWatchTime = new DateTime(ticks, DateTimeKind.Utc);

            DateTime resetTime = lastWatchTime.AddHours(m_ResetAfterHours);

            TimeSpan remaining = resetTime - DateTime.UtcNow;

            int remainingHours = Mathf.CeilToInt((float)remaining.TotalHours);

            return Mathf.Max(0, remainingHours);
        }
    }
}