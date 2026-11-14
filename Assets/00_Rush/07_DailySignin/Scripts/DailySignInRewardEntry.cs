using UnityEngine;

namespace Rush
{
    [System.Serializable]
    public class DailySignInRewardEntry
    {
        [SerializeField] private CollectibleConfig m_Collectible;
        [SerializeField] private int m_Amount = 1;

        // A custom icon takes precedence on both the calendar and the featured reward.
        [SerializeField] private Sprite m_OverrideIcon;

        [SerializeField] private ShopBundleEntry[] m_AdditionalRewards;

        internal bool TryBuildClaim(int dayIndex, out DailySignInClaimResult result)
        {
            result = null;
            if (m_Collectible == null || m_Amount <= 0) return false;
            var claim = new DailySignInClaimResult(dayIndex + 1);
            claim.AddEntry(m_Collectible, m_Amount);
            if (m_AdditionalRewards != null)
            {
                foreach (var reward in m_AdditionalRewards)
                {
                    if (reward?.Collectible == null || reward.Amount <= 0) return false;
                    claim.AddEntry(reward.Collectible, reward.Amount);
                }
            }
            result = claim;
            return true;
        }

        public CollectibleConfig Collectible => m_Collectible;
        public int Amount => m_Amount;
        public Sprite DisplayIcon => m_OverrideIcon != null
            ? m_OverrideIcon
            : m_Collectible?.CollectibleField?.Icon;

        public Sprite DisplaySplash => m_OverrideIcon != null
            ? m_OverrideIcon
            : m_Collectible?.CollectibleField?.SplashImage ?? DisplayIcon;
    }
    internal sealed class DailySignInClaimResult : CollectibleResultData
    {
        internal int DayNumber { get; }
        internal DailySignInClaimResult(int dayNumber) => DayNumber = dayNumber;
    }
}