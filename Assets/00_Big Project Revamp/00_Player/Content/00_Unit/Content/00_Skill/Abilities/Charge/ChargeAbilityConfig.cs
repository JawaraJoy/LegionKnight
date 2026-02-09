using UnityEngine;

namespace Rush
{
    public partial class ChargeAbilityConfig : AbilityConfig
    {
        [SerializeField]
        private int m_ChargeTickCount = 0;
        [SerializeField]
        private float m_ChargeTickInterval = 0;
        [SerializeField]
        private SkillCategoryConfig m_SkillCategoryToCharge;
        public int ChargeTickCount => m_ChargeTickCount;
        public float ChargeTickInterval => m_ChargeTickInterval;
        public SkillCategoryConfig SkillCategoryToCharge => m_SkillCategoryToCharge;
    }
}
