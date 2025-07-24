using UnityEngine;

namespace LegionKnight
{
    public class PlayerEnergy : EnergyController
    {
        
    }

    public partial class Player
    {
        [SerializeField]
        private PlayerEnergy m_EnergyController;
        public void AddEnergy(EnergyDefinition definition, int amount)
        {
            m_EnergyController.Add(definition, amount);
        }
        public void SetEnergy(EnergyDefinition definition, int amount)
        {
            m_EnergyController.Set(definition, amount);
        }
    }
}
