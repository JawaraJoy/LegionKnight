using UnityEngine;

namespace LegionKnight
{
    public partial class PlayerGuard : Guard
    {
        
    }

    public partial class Player
    {
        [SerializeField]
        private PlayerGuard m_PlayerGuard;

        public void ActiveGuard(Sprite sprite, float duration)
        {
            m_PlayerGuard.ActiveGuard(sprite, duration);
        }
        public void DeactiveGuard()
        {
            m_PlayerGuard.DeactiveGuard();
        }
        public void SetCanActive(bool set)
        {
            m_PlayerGuard.SetCanActive(set);
        }
    }
}
