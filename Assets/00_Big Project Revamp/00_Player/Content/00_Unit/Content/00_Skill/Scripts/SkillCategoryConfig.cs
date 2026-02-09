using UnityEngine;

namespace Rush
{
    [CreateAssetMenu(fileName = "Skill Category", menuName = "Rush/Combat/SkillCategory", order = 2)]
    public class SkillCategoryConfig : Configuration
    {
        [SerializeField]
        private SkillType m_SkillType;
        public SkillType SkillType => m_SkillType;
    }
    public enum SkillType
    {
        Active = 0,
        Passive = 1,
        Other = 2,
    }
}
