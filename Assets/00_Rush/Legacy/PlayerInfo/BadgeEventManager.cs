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
        }
    }
}
