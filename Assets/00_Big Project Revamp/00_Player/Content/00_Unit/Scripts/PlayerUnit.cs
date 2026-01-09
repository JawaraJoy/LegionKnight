using UnityEngine;

namespace Rush
{
    public partial class PlayerUnit : Unit
    {
        
    }
    public partial class RushPlayer
    {
        [SerializeField]
        private PlayerUnit m_Unit;
        public PlayerUnit Unit => m_Unit;
    }
}
