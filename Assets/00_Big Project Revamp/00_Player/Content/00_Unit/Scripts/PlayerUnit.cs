using UnityEngine;

namespace Rush
{
    public partial class PlayerUnit : Unit
    {
        
    }
    public partial class Player
    {
        [SerializeField]
        private PlayerUnit m_Unit;
        public PlayerUnit Unit => m_Unit;
    }
}
