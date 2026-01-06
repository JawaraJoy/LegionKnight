using MoreMountains.Tools;
using UnityEngine;

namespace Rush
{
    [CreateAssetMenu(fileName = "Unit", menuName = "Rush/Unit/Unit", order = 0)]
    public partial class UnitConfig : Configuration
    {
        [SerializeField]
        private StatsProgressField m_MainStats;
        public StatsProgressField MainStats => m_MainStats;
    }

    [System.Serializable]
    public partial class UnitContext
    {
        [SerializeField, MMReadOnly]
        private UnitConfig m_CharacterConfig;
        [SerializeField, MMReadOnly]
        private Unit m_CharacterObject;
        public UnitConfig CharacterConfig => m_CharacterConfig;
        public Unit CharacterObject => m_CharacterObject;
        public UnitContext(UnitConfig characterConfig, Unit characterObject)
        {
            m_CharacterConfig = characterConfig;
            m_CharacterObject = characterObject;
        }
    }
}
