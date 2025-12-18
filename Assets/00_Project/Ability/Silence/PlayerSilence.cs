using UnityEngine;

namespace LegionKnight
{
    public class PlayerSilence : Silence
    {
        
    }

    public partial class Player
    {
        [SerializeField]
        private PlayerSilence m_Silence;
        public PlayerSilence Silence => m_Silence;
    }
}
