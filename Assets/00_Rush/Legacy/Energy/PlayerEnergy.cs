using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public class PlayerEnergy : EnergyController
    {
        
    }
    // for you know this is singleton
    // you can get this just RushGameManager.Instance.EnergyController to get it
    public partial class Player
    {
        [SerializeField]
        private PlayerEnergy m_EnergyController;
        public PlayerEnergy EnergyController => m_EnergyController;
    }
}
