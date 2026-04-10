using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public class PlayerEnergy : EnergyController
    {
        
    }

    public partial class Player
    {
        [SerializeField]
        private PlayerEnergy m_EnergyController;
        public PlayerEnergy EnergyController => m_EnergyController;
    }
}
