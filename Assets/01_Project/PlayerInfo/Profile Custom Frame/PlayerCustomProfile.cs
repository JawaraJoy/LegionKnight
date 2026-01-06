using System.Collections.Generic;
using UnityEngine;

namespace LegionKnight
{
    public class PlayerCustomProfile : CustomProfile
    {
        
    }

    public partial class Player
    {
        [SerializeField]
        private PlayerCustomProfile m_CustomProfile = null;
        public PlayerCustomProfile CustomProfile => m_CustomProfile;
    }
}
