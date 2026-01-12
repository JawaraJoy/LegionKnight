using MoreMountains.Tools;
using System.Collections.Generic;
using UnityEngine;

namespace Rush
{
    public class Skill : MonoBehaviour
    {
        [SerializeField]
        private Transform m_SkillSpawnPost;
        [SerializeField, MMReadOnly]
        private ModuleContext m_ModuleContext;
        [SerializeField, MMReadOnly]
        private List<SkillActivator> m_SkillActivators = new();

        public void Init(Unit unitOwner)
        {
            m_ModuleContext = new ModuleContext(unitOwner, gameObject);
            PrepareActivators();
        }

        private SkillActivator GetSkillActivatorInternal(string id)
        {
            return m_SkillActivators.Find(x => x.Context.Activator.SkillConfig.BaseInfo.Id == id);
        }
        private List<SkillActivator> GetSkillsByCategoryInternal(SkillCategory category)
        {
            List<SkillActivator> findCategories = new List<SkillActivator>();
            foreach (SkillActivator activator in m_SkillActivators) 
            { 
                if (activator.SkillConfig.Category == category)
                {
                    findCategories.Add(activator);
                }
            }
            return findCategories;
        }
        public void SetSkillCategoryLevel(SkillCategory category, int level)
        {
            List<SkillActivator> activators = GetSkillsByCategoryInternal(category);
            foreach(SkillActivator activator in activators)
            {
                activator.Progression.SetLevel(level);
            }
        }
        public void AddSkillCategoryLevel(SkillCategory category, int level)
        {
            List<SkillActivator> activators = GetSkillsByCategoryInternal(category);
            foreach (SkillActivator activator in activators)
            {
                activator.Progression.AddLevel(level);
            }
        }
        private bool HasSkillActivatorInternal(string id, out SkillActivator activator)
        {
            bool hasSkillActivator = GetSkillActivatorInternal(id) != null;
            if (hasSkillActivator) 
            { 
                activator = GetSkillActivatorInternal(id);
            }
            else
            {
                activator = null;
            }
            return hasSkillActivator;
        }
        private void PrepareActivators()
        {
            if (!m_ModuleContext.Initialized) return;
            SkillActivatorConfig[] skills = m_ModuleContext.UnitOwner.Config.Skills;
            
            for (int i = 0; i < skills.Length; i++)
            {
                var skillConfig = skills[i];
                PreparActivator(skillConfig);
            }
        }
        private void PreparActivator(SkillActivatorConfig config)
        {
            if (HasSkillActivatorInternal(config.BaseInfo.Id, out SkillActivator activator))
            {
                activator.Init(config, m_ModuleContext);
            }
            else
            {
                SkillActivator spawned = Instantiate(config.ActivatorPrefab, m_SkillSpawnPost, false);
                spawned.Init(config, m_ModuleContext);
                RegisterSkillInternal(spawned);
            }
        }
        private void RegisterSkillInternal(SkillActivator activator)
        {
            if (GetSkillActivatorInternal(activator.SkillConfig.BaseInfo.Id) == null)
            {
                m_SkillActivators.Add(activator);
            }
        }
        private void UnregisterSkillInternal(SkillActivator activator)
        {
            if (GetSkillActivatorInternal(activator.SkillConfig.BaseInfo.Id) != null)
            {
                m_SkillActivators.Remove(activator);
            }
        }
    }
}
