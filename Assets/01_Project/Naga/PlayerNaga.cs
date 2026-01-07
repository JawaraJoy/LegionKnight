using LegionKnight;
using UnityEngine;

namespace Rush
{
    public class PlayerNaga : NagaAnimator
    {
        
    }
    public partial class Player
    {
        [SerializeField]
        private PlayerNaga m_Naga;
        public PlayerNaga Naga => m_Naga;
    }
}
