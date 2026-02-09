using UnityEngine;

namespace LegionKnight
{
    public partial class BadgeManager : BadgeHandler
    {
        
    }
    public partial class Player
    {
        [SerializeField]
        private BadgeManager m_BadgeManager;
        public BadgeManager BadgeManager => m_BadgeManager;
    }
}
