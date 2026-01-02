using MoreMountains.Tools;
using UnityEngine;

namespace Rush
{
    [CreateAssetMenu(fileName = "Character", menuName = "Rush/Character/Character", order = 0)]
    public partial class CharacterConfig : Configuration
    {
        [SerializeField]
        private StatsProgressField m_MainStats;
        public StatsProgressField MainStats => m_MainStats;
    }

    [System.Serializable]
    public partial class CharacterContext
    {
        [SerializeField, MMReadOnly]
        private CharacterConfig m_CharacterConfig;
        [SerializeField, MMReadOnly]
        private GameObject m_CharacterObject;
        public CharacterConfig CharacterConfig => m_CharacterConfig;
        public GameObject CharacterObject => m_CharacterObject;
        public CharacterContext(CharacterConfig characterConfig, GameObject characterObject)
        {
            m_CharacterConfig = characterConfig;
            m_CharacterObject = characterObject;
        }
    }
}
