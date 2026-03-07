using MoreMountains.Tools;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Rush
{
    public class SkillController : MonoBehaviour, IUnitExtension, IHasSkills, IReseter
    {
        [SerializeField]
        private Transform m_SkillSpawnPost;
        [SerializeField, MMReadOnly]
        private ModuleContext m_ModuleContext;
        [SerializeField, MMReadOnly]
        private List<Skill> m_Skills = new();

        [SerializeField, MMReadOnly]
        private List<Skill> m_RemovedSkills = new();

        [SerializeField]
        private CategorySkillController[] m_CategorySkillControllers;
        public IReadOnlyList<Skill> Skills => m_Skills;

        public IModuleContext ModuleContext => m_ModuleContext;
        [SerializeField]
        private UnityEvent<IModuleContext> m_OnInit;
        [SerializeField]
        private UnityEvent<Skill> m_OnSkillAdded;
        [SerializeField]
        private UnityEvent<Skill> m_OnSkillRemoved;
        [SerializeField]
        private UnityEvent m_OnResetProgress;

        public void Init(Unit unitOwner)
        {
            m_ModuleContext = new ModuleContext(unitOwner, gameObject);
            AddNewSkillsInternal(m_ModuleContext.Unit.Config.Skills);
            m_OnInit?.Invoke(m_ModuleContext);
        }

        private Skill GetSkillActivatorInternal(string id)
        {
            return m_Skills.Find(x => x.SkillContext.Skill.SkillConfig.BaseInfo.Id == id);
        }
        private Skill GetSkillByIndex(int index)
        {
            if (index < 0 || index >= m_Skills.Count) return null;
            return m_Skills[index];
        }
        public Skill GetSkillActivator(SkillConfig config)
        {
            return GetSkillActivatorInternal(config.BaseInfo.Id);
        }
        public IReadOnlyList<Skill> GetSkillsByCategory(SkillCategoryConfig category)
        {
            return GetSkillsByCategoryInternal(category);
        }
        private CategorySkillController GetCategoryControllerInternal(SkillCategoryConfig category)
        {
            return m_CategorySkillControllers.FirstOrDefault(x => x.SkillCategoryConfig.BaseInfo.Id == category.BaseInfo.Id);
        }
        private bool HasCategoryControllerInternal(SkillCategoryConfig category, out CategorySkillController categorySkill)
        {
            categorySkill = GetCategoryControllerInternal(category);
            return GetCategoryControllerInternal(category) != null;
        }
        public bool HasCategoryController(SkillCategoryConfig category, out CategorySkillController categorySkill)
        {
            return HasCategoryControllerInternal(category, out categorySkill);
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
            HashSet<Skill> result = new HashSet<Skill>();

            foreach (SkillCategoryConfig category in skillCategories)
            {
                var skills = GetSkillsByCategoryInternal(category);
                foreach (var skill in skills)
                {
                    result.Add(skill);
                }
            }

            return result.ToList();
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
                //skill.Init(config, m_ModuleContext);
                skill.Progression.AddLevel(1);
            }
            else
            {
                RegisterSkillInternal(config);
            }
        }
        private void RegisterSkillInternal(SkillConfig skillConfig)
        {
            Skill newSkill = null;
            if (m_RemovedSkills.Any(x => x.SkillConfig.BaseInfo.Id == skillConfig.BaseInfo.Id))
            {
                newSkill = m_RemovedSkills.Find(x => x.SkillConfig.BaseInfo.Id == skillConfig.BaseInfo.Id);
            }
            else
            {
                newSkill = Instantiate(skillConfig.ActivatorPrefab, m_SkillSpawnPost, false);
                newSkill.Init(skillConfig, m_ModuleContext);
            }
            if (newSkill != null)
            {
                m_Skills.Add(newSkill);
                m_RemovedSkills.Remove(newSkill);
                m_OnSkillAdded?.Invoke(newSkill);
            }
        }
        private void UnregisterSkillInternal(Skill skill)
        {
            if (GetSkillActivatorInternal(skill.SkillConfig.BaseInfo.Id) != null)
            {
                m_Skills.Remove(skill);
                m_RemovedSkills.Add(skill);
                m_OnSkillRemoved?.Invoke(skill);
            }
        }

        public void ForceActives(SkillConfig[] skillConfigs)
        {
            foreach (SkillConfig config in skillConfigs)
            {
                ForceActiveInternal(config);
            }
        }
        public void ForceActiveByIndex(int index)
        {
            Skill skill = GetSkillByIndex(index);
            if (skill != null)
            {
                ForceActiveInternal(skill.SkillConfig);
            }
        }
        public void ForceActive(SkillConfig skillConfig)
        {
            ForceActiveInternal(skillConfig);
        }
        private void ForceActiveInternal(SkillConfig skillConfig)
        {
            if (HasSkillActivatorInternal(skillConfig.BaseInfo.Id, out Skill skill))
            {
                skill.ForceActivateAll();
            }
            else
            {
                AddNewSkillInternal(skillConfig);
                if (HasSkillActivatorInternal(skillConfig.BaseInfo.Id, out Skill skillAdded))
                {
                    skillAdded.ForceActivateAll();
                }
            }
        }
        public void ForceActiveByCategory(SkillCategoryConfig categoryConfig)
        {
            Skill[] skills = GetSkillsByCategoryInternal(categoryConfig).ToArray();
            if (skills.Length > 0)
            {
                foreach (Skill skill in skills)
                {
                    skill.ForceActivateAll();
                }
            }
        }
        public void ResetProgression()
        {
            foreach (Skill skill in m_Skills)
            {
                skill.Progression.SetLevel(1);
            }
            m_OnResetProgress?.Invoke();
        }

        public CategorySkillController GetCategoryController(SkillCategoryConfig categoryConfig)
        {
            return m_CategorySkillControllers.FirstOrDefault(x => x.SkillCategoryConfig.BaseInfo.Id == categoryConfig.BaseInfo.Id);
        }
    }
}
