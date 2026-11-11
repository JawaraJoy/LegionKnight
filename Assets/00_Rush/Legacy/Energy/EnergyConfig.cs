using Rush;
using UnityEngine;

namespace LegionKnight
{
    [CreateAssetMenu(fileName = "New Energy", menuName = "Legion Knight/Energy", order = 1)]
    public class EnergyConfig : CollectibleConfig
    {
        [SerializeField]
        private bool m_CanBreakMaxAmount = false;
        [SerializeField]
        private int m_MaxAmount;

        [Header("Daily Reset")]
        [Tooltip("Jam berapa energy di-reset ke max setiap hari. Contoh: 15 = jam 15:00")]
        [SerializeField, Range(0, 23)]
        private int m_DailyResetHour = 15;

        [Header("Regen")]
        [SerializeField]
        private bool m_CanRegen = false;
        [SerializeField]
        private int m_RegenEverySeconds = 1;
        [SerializeField]
        private int m_RegenAmount = 1;

        public int MaxAmount => m_MaxAmount;
        public bool CanBreakMaxAmount => m_CanBreakMaxAmount;
        public int DailyResetHour => m_DailyResetHour;
        public bool CanRegen => m_CanRegen;
        public int RegenEverEverySeconds => m_RegenEverySeconds;
        public int RegenAmount => m_RegenAmount;

        public void AddEnergy(int amount)
        {
            Player.Instance.EnergyController.Add(this, amount);
        }

        public void SetEnergy(int amount)
        {
            Player.Instance.EnergyController.Set(this, amount);
        }
    }
}