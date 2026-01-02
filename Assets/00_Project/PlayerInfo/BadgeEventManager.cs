using UnityEngine;

namespace LegionKnight
{
    public class BadgeEventManager : MonoBehaviour
    {
        [SerializeField]
        private BadgeContent[] m_Badges;

        private BadgeManager badgeManager;

        void Start()
        {
            badgeManager = GameObject.FindFirstObjectByType<BadgeManager>();

            if(badgeManager && m_Badges.Length > 0)
            {
                badgeManager.AddAdditionalBadges(m_Badges);
            }
        }
    }
}
