using UnityEngine;
using Rush;

namespace LegionKnight
{
    [CreateAssetMenu(fileName = "New Energy", menuName = "Legion Knight/Energy", order = 1)]
    public class EnergyConfig : CollectibleConfig
    {
        [SerializeField]
        private bool m_CanBreakMaxAmount = false; // Optional, if true, allows exceeding max amount temporarily
        [SerializeField]
        private int m_MaxAmount;
        [SerializeField]
        private bool m_CanRegen = false;
        [SerializeField]
        private int m_RegenEverySeconds = 1;
        [SerializeField]
        private int m_RegenAmount = 1;
        public int MaxAmount => m_MaxAmount;
        public bool CanBreakMaxAmount => m_CanBreakMaxAmount;
        public int RegenEverEverySeconds => m_RegenEverySeconds;
        public int RegenAmount => m_RegenAmount;
        public bool CanRegen => m_CanRegen;

        public void AddEnergy(int amount)
        {
            Player.Instance.AddEnergy(this, amount);
        }
        public void SetEnergy(int amount)
        {
            Player.Instance.SetEnergy(this, amount);
        }
    }
}
