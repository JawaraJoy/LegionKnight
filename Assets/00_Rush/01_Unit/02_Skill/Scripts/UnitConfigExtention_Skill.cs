using System;
using UnityEngine;

namespace Rush
{
    public abstract partial class UnitConfig
    {
        [SerializeField]
        private SkillConfig[] m_Skills;
        public SkillConfig[] Skills => m_Skills;

        private SkillConfig[] GetSkillByCategoryInternal(SkillCategoryConfig category)
        {
            var skills = Array.FindAll(m_Skills, skill => skill.Category.BaseInfo.Id == category.BaseInfo.Id);
            return skills;
        }
        public SkillConfig[] GetSkillsByCategory(SkillCategoryConfig category)
        {
            return GetSkillByCategoryInternal(category);
        }
    }
}
