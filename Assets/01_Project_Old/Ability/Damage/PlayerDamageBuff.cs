using UnityEngine;

namespace LegionKnight
{
    public class PlayerDamageBuff : DamageBuff
    {
        
    }

    public partial class Player
    {
        [SerializeField]
        private PlayerDamageBuff m_PlayerDamageBuff;
        public PlayerDamageBuff GetPlayerDamageBuff()
        {
            return m_PlayerDamageBuff;
        }
    }
}
