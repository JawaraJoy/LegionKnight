using UnityEngine;

namespace Rush
{
    [System.Serializable]
    public class CardSkillField
    {
        [SerializeField]
        private CardPurpose m_CardPurpose;
        [SerializeField]
        private SkillConfig m_SkillConfig;
        public CardPurpose CardPurpose => m_CardPurpose;
        public SkillConfig SkillConfig => m_SkillConfig;
    }
    public enum CardPurpose
    {
        Activation,
        SkillUp,
    }
}
