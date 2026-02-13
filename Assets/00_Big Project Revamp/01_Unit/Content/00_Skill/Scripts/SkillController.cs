using MoreMountains.Tools;
using System.Collections.Generic;
using UnityEngine;

namespace Rush
{
    public class SkillController : MonoBehaviour, IUnitExtension
    {
        [SerializeField]
        private Transform m_SkillSpawnPost;
        [SerializeField, MMReadOnly]
        private ModuleContext m_ModuleContext;
        [SerializeField, MMReadOnly]
        private List<Skill> m_SkillActivators = new();
        public IReadOnlyList<Skill> SkillActivators => m_SkillActivators;
        public void Init(Unit unitOwner)
        {
            m_ModuleContext = new ModuleContext(unitOwner, gameObject);
            PrepareSkillsInternal(m_ModuleContext.UnitOwner.Config.Skills);
        }

        private Skill GetSkillActivatorInternal(string id)
        {
            return m_SkillActivators.Find(x => x.Context.Activator.SkillConfig.BaseInfo.Id == id);
        }
        public Skill GetSkillActivator(SkillConfig config)
        {
            return GetSkillActivatorInternal(config.BaseInfo.Id);
        }
        private IReadOnlyList<Skill> GetSkillsByCategoryInternal(SkillCategoryConfig category)
        {
            List<Skill> findCategories = new List<Skill>();
            foreach (Skill activator in m_SkillActivators) 
            { 
                if (activator.SkillConfig.Category == category)
                {
                    findCategories.Add(activator);
                }
            }
            return findCategories;
        }
        private IReadOnlyList<Skill> GetSkillsByMultiCategoryInternal(SkillCategoryConfig[] skillCategories)
        {
            List<Skill> findCategories = new List<Skill>();
            foreach (Skill activator in m_SkillActivators)
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
            IReadOnlyList<Skill> activators = GetSkillsByCategoryInternal(category);
            foreach(Skill activator in activators)
            {
                activator.Progression.SetLevel(level);
            }
        }
        
        public void AddSkillCategoryLevel(SkillCategoryConfig category, int level)
        {
            IReadOnlyList<Skill> activators = GetSkillsByCategoryInternal(category);
            foreach (Skill activator in activators)
            {
                activator.Progression.AddLevel(level);
            }
        }
        private bool HasSkillActivatorInternal(string id, out Skill skill)
        {
            bool hasSkillActivator = GetSkillActivatorInternal(id) != null;
            if (hasSkillActivator) 
            { 
                skill = GetSkillActivatorInternal(id);
            }
            else
            {
                skill = null;
            }
            return hasSkillActivator;
        }
        public bool HasSkillActivator(SkillConfig config, out Skill skill)
        {
            bool hasSkillActivator = GetSkillActivatorInternal(config.BaseInfo.Id) != null;
            if (hasSkillActivator)
            {
                skill = GetSkillActivatorInternal(config.BaseInfo.Id);
            }
            else
            {
                skill = null;
            }
            return hasSkillActivator;
        }
        private void PrepareSkillsInternal(SkillConfig[] configs)
        {
            if (!m_ModuleContext.Initialized) return;
            
            for (int i = 0; i < configs.Length; i++)
            {
                var skillConfig = configs[i];
                AddNewSkillInternal(skillConfig);
            }
        }
        public void AddNewSkills(SkillConfig[] configs)
        {
            foreach (SkillConfig config in configs)
            {
                AddNewSkillInternal(config);
            }
        }
        public void AddNewSkill(SkillConfig config)
        {
            AddNewSkillInternal(config);
        }
        private void AddNewSkillInternal(SkillConfig config)
        {
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
                m_SkillActivators.Add(skill);
            }
        }
        private void UnregisterSkillInternal(Skill skill)
        {
            if (GetSkillActivatorInternal(skill.SkillConfig.BaseInfo.Id) != null)
            {
                m_SkillActivators.Remove(skill);
            }
        }
    }
}
