using UnityEngine;

namespace Rush
{
    [System.Serializable]
    public class DailySignInRewardEntry
    {
        [SerializeField] private CollectibleConfig m_Collectible;
        [SerializeField] private int m_Amount = 1;

        // Optional display override — if null, use collectible's default icon/name
        [SerializeField] private Sprite m_OverrideIcon;

        public CollectibleConfig Collectible => m_Collectible;
        public int Amount => m_Amount;
        public Sprite DisplayIcon => m_OverrideIcon != null
            ? m_OverrideIcon
            : m_Collectible?.CollectibleField?.Icon;
    }
}