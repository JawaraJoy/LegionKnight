using System.Linq;
using UnityEngine;

namespace LegionKnight
{
    public class BadgeEventManager : MonoBehaviour
    {
        [SerializeField]
        private BadgeContent[] m_Badges;

        private BadgeManager m_BadgeManager;

        void Start()
        {
            if (m_BadgeManager == null)
            {
                m_BadgeManager = Player.Instance.BadgeManager;
            }

            if(m_BadgeManager && m_Badges.Length > 0)
            {
                m_BadgeManager.AddAdditionalBadges(m_Badges);
            }
        }
    }

    public partial class BadgeHandler
    {
        public void AddAdditionalBadges(BadgeContent[] additionalBadges)
        {
            m_Badges = m_Badges.Concat(additionalBadges).ToArray();
        }
    }
}
