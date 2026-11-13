using LegionKnight;
using UnityEngine;
using UnityEngine.UI;

namespace Rush
{
    // The existing card draws both hero and regular badges. The active profile supplies its navigation.
    public sealed class ProfileBadgeListItem : MonoBehaviour
    {
        [SerializeField] private BadgeView m_View;
        [SerializeField] private Button m_Details;
        [SerializeField] private Button m_Claim;

        public void Bind(ProfileBadgeListView list, BadgeConfig badge)
        {
            // Called synchronously on a fresh instance, before the legacy card installs its Start listeners.
            m_View.enabled = false;
            m_View.Init(badge);
            m_Details.onClick.AddListener(() => list.OpenDetails(badge));
            m_Claim.onClick.AddListener(() => list.Claim(badge));
        }

        public void RefreshCard() => m_View.Init(m_View.Definition);
    }
}
