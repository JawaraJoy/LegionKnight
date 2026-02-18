using MoreMountains.Tools;
using System.Collections.Generic;
using UnityEngine;

namespace Rush
{
    public class SkillController : MonoBehaviour, IUnitExtension, IHasSkills
    {
        [SerializeField]
        private Transform m_SkillSpawnPost;
        [SerializeField, MMReadOnly]
        private ModuleContext m_ModuleContext;
        [SerializeField, MMReadOnly]
        private List<Skill> m_Skills = new();
        public IReadOnlyList<Skill> Skills => m_Skills;

        public IModuleContext ModuleContext => m_ModuleContext;

        public void Init(Unit unitOwner)
        {
            m_ModuleContext = new ModuleContext(unitOwner, gameObject);
            AddNewSkillsInternal(m_ModuleContext.Unit.Config.Skills);
        }

        private Skill GetSkillActivatorInternal(string id)
        {
            return m_Skills.Find(x => x.SkillContext.Skill.SkillConfig.BaseInfo.Id == id);
        }
        public Skill GetSkillActivator(SkillConfig config)
        {
            return GetSkillActivatorInternal(config.BaseInfo.Id);
        }
        private IReadOnlyList<Skill> GetSkillsByCategoryInternal(SkillCategoryConfig category)
        {
            List<Skill> findCategories = new List<Skill>();
            foreach (Skill skill in m_Skills) 
            { 
                if (skill.SkillConfig.Category == category)
                {
                    findCategories.Add(skill);
                }
            }
            return findCategories;
        }
        private IReadOnlyList<Skill> GetSkillsByMultiCategoryInternal(SkillCategoryConfig[] skillCategories)
        {
            List<Skill> findCategories = new List<Skill>();
            foreach (Skill skill in m_Skills)
            {
                foreach (SkillCategoryConfig skillCategory in skillCategories)
                {
                    findCategories.AddRange(GetSkillsByCategoryInternal(skillCategory));
                }
            }
            return findCategories;
        }
        public IReadOnlyList<Skill> GetSkillsByMultiCategory(SkillCategoryConfig[] skillCategories)
        {
            return GetSkillsByMultiCategoryInternal(skillCategories);
        }
        public void SetSkillCategoryLevel(SkillCategoryConfig category, int level)
        {
            IReadOnlyList<Skill> skills = GetSkillsByCategoryInternal(category);
            foreach(Skill skill in skills)
            {
                skill.Progression.SetLevel(level);
            }
        }
        
        public void AddSkillCategoryLevel(SkillCategoryConfig category, int level)
        {
            IReadOnlyList<Skill> skills = GetSkillsByCategoryInternal(category);
            foreach (Skill skill in skills)
            {
                skill.Progression.AddLevel(level);
            }
        }
        private bool HasSkillActivatorInternal(string id, out Skill skill)
        {
            bool hasSkill = GetSkillActivatorInternal(id) != null;
            if (hasSkill) 
            { 
                skill = GetSkillActivatorInternal(id);
            }
            else
            {
                skill = null;
            }
            return hasSkill;
        }
        public bool HasSkill(SkillConfig config, out Skill skill)
        {
            bool hasSkill = GetSkillActivatorInternal(config.BaseInfo.Id) != null;
            if (hasSkill)
            {
                skill = GetSkillActivatorInternal(config.BaseInfo.Id);
            }
            else
            {
                skill = null;
            }
            return hasSkill;
        }
        private void AddNewSkillsInternal(SkillConfig[] configs)
        {
            foreach (SkillConfig config in configs)
            {
                AddNewSkillInternal(config);
            }
        }
        public void AddNewSkills(SkillConfig[] configs)
        {
            AddNewSkillsInternal(configs);
        }
        public void AddNewSkill(SkillConfig config)
        {
            AddNewSkillInternal(config);
        }
        private void AddNewSkillInternal(SkillConfig config)
        {
            if (!m_ModuleContext.Initialized) return;

            if (HasSkillActivatorInternal(config.BaseInfo.Id, out Skill skill))
            {
                skill.Init(config, m_ModuleContext);
            }
            else
            {
                Skill spawned = Instantiate(config.ActivatorPrefab, m_SkillSpawnPost, false);
                spawned.Init(config, m_ModuleContext);
                RegisterSkillInternal(spawned);
            }
        }
        private void RegisterSkillInternal(Skill skill)
        {
            if (GetSkillActivatorInternal(skill.SkillConfig.BaseInfo.Id) == null)
            {
                m_Skills.Add(skill);
            }
        }
        private void UnregisterSkillInternal(Skill skill)
        {
            if (GetSkillActivatorInternal(skill.SkillConfig.BaseInfo.Id) != null)
            {
                m_Skills.Remove(skill);
            }
        }
    }
}
