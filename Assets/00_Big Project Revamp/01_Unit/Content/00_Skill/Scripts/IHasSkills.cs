using System.Collections.Generic;
using UnityEngine;

namespace Rush
{
    public interface IHasSkills
    {
        IReadOnlyList<Skill> Skills {  get; }
        Skill GetSkillActivator(SkillConfig config);
        IReadOnlyList<Skill> GetSkillsByMultiCategory(SkillCategoryConfig[] skillCategories);
        void SetSkillCategoryLevel(SkillCategoryConfig category, int level);
        void AddSkillCategoryLevel(SkillCategoryConfig category, int level);
        bool HasSkillActivator(SkillConfig config, out Skill skill);
        void AddNewSkills(SkillConfig[] configs);
        void AddNewSkill(SkillConfig config);
    }
}
