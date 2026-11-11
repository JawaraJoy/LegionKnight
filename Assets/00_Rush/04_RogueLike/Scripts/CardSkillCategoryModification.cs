using UnityEngine;

namespace Rush
{
    [System.Serializable]
    public class CardSkillCategoryModification
    {
        [SerializeField]
        private SkillCategoryConfig m_SkillCategoryConfig;
        [SerializeField]
        private StatusEffectConfig[] m_OnSelf;
        [SerializeField]
        private StatusEffectConfig[] m_OnTarget;

        public SkillCategoryConfig SkillCategoryConfig => m_SkillCategoryConfig;
        public StatusEffectConfig[] OnSelf => m_OnSelf;
        public StatusEffectConfig[] OnTarget => m_OnTarget;

        public void ApplyModification(SkillController skillController)
        {
            if (skillController.HasCategoryController(m_SkillCategoryConfig, out CategorySkillController categorySkill))
            {
                foreach (var statusEffect in m_OnSelf)
                {
                    categorySkill.AddStatusEffectOnSelf<DamageAbilityDeliver>(statusEffect);
                }
                foreach (var statusEffect in m_OnTarget)
                {
                    categorySkill.AddStatusEffectOnTarget<DamageAbilityDeliver>(statusEffect);
                }
            }
        }
    }
}
