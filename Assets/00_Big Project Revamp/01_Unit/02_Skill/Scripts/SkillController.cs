using MoreMountains.Tools;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
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

        [Header("Casting Global Events")]
        [SerializeField]
        private UnityEvent<int> m_OnCharge;
        [SerializeField] 
        private UnityEvent<Skill> m_OnAnyCastingStart;
        [SerializeField] 
        private UnityEvent<float> m_OnAnyCastingUpdate;
        [SerializeField]
        private UnityEvent<int, int> m_OnCastingInterruptUpdate;
        [SerializeField] 
        private UnityEvent<Skill> m_OnAnyCastingSuccess;
        [SerializeField] 
        private UnityEvent<Skill> m_OnAnyCastingFail;
        public IReadOnlyList<Skill> Skills => m_Skills;

        private Skill m_CurrentCastingSkill;

        public IModuleContext ModuleContext => m_ModuleContext;
        [SerializeField]
        private UnityEvent<IModuleContext> m_OnInit;
        [SerializeField]
        private UnityEvent<Skill> m_OnSkillAdded;
        [SerializeField]
        private UnityEvent<Skill> m_OnSkillRemoved;
        [SerializeField]
        private UnityEvent m_OnResetProgress;
        public UnityEvent<int> OnCharge => m_OnCharge;

        private readonly Dictionary<Skill, UnityAction> m_StartListeners = new();
        private readonly Dictionary<Skill, UnityAction<float>> m_UpdateListeners = new();
        private readonly Dictionary<Skill, UnityAction<int, int>> m_InterruptListeners = new();
        private readonly Dictionary<Skill, UnityAction> m_SuccessListeners = new();
        private readonly Dictionary<Skill, UnityAction> m_FailListeners = new();

        public void Init(Unit unitOwner)
        {
            m_ModuleContext = new ModuleContext(unitOwner, gameObject);
            AddNewSkillsInternal(m_ModuleContext.Unit.Config.Skills);
            m_OnInit?.Invoke(m_ModuleContext);
        }
        private AbilityDeliver GetAbilityDeliverInternal(AbilityConfig abilityConfig)
        {
            foreach (Skill skill in m_Skills)
            {
                if (skill.HasAbility(abilityConfig.BaseInfo.Id, out AbilityDeliver abilityDeliver))
                {
                    return abilityDeliver;
                }
            }
            return null;
        }
        public bool HasAbility(AbilityConfig abilityConfig, out AbilityDeliver abilityDeliver)
        {
            abilityDeliver = GetAbilityDeliverInternal(abilityConfig);
            return abilityDeliver != null;
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
                skill.Progression.AddLevel(1);
                //skill.Init(config, m_ModuleContext);
            }
            else
            {
                RegisterSkillInternal(config);
            }
        }
        
        public void AddNewSkillWithoutAddLevel(SkillConfig config)
        {
            AddNewSkillWithoutAddLevelInternal(config);
        }
        public void AddNewSkillsWithoutAddLevel(SkillConfig[] configs)
        {
            foreach (SkillConfig config in configs)
            {
                AddNewSkillWithoutAddLevelInternal(config);
            }
        }
        private void AddNewSkillWithoutAddLevelInternal(SkillConfig config)
        {
            if (!m_ModuleContext.Initialized) return;
            if (HasSkillActivatorInternal(config.BaseInfo.Id, out Skill skill))
            {
                //skill.Init(config, m_ModuleContext);
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

                RegisterCastingEvents(newSkill); // 🔥 TAMBAHAN

                m_OnSkillAdded?.Invoke(newSkill);
            }
        }
        private void RegisterCastingEvents(Skill skill)
        {
            void startAction()
            {
                m_OnAnyCastingStart?.Invoke(skill);
                m_CurrentCastingSkill = skill;
                Debug.Log($"Start Casting: {skill.SkillConfig.name}");
            }

            void updateAction(float progress)
            {
                m_OnAnyCastingUpdate?.Invoke(progress);
            }
            void interuptAction(int current, int max)
            {
                m_OnCastingInterruptUpdate?.Invoke(current, max);
            }

            void successAction()
            {
                m_OnAnyCastingSuccess?.Invoke(skill);
                m_CurrentCastingSkill = null;
            }

            void failAction()
            {
                m_OnAnyCastingFail?.Invoke(skill);
                m_CurrentCastingSkill = null;
            }

            skill.OnCastingStartEvent.AddListener(startAction);
            skill.OnCastingUpdateEvent.AddListener(updateAction);
            skill.OnCastingInterruptEvent.AddListener(interuptAction);
            skill.OnCastingSuccessEvent.AddListener(successAction);
            skill.OnCastingFailEvent.AddListener(failAction);

            m_StartListeners[skill] = startAction;
            m_UpdateListeners[skill] = updateAction;
            m_InterruptListeners[skill] = interuptAction;
            m_SuccessListeners[skill] = successAction;
            m_FailListeners[skill] = failAction;
        }
        private void UnregisterSkillInternal(Skill skill)
        {
            if (GetSkillActivatorInternal(skill.SkillConfig.BaseInfo.Id) != null)
            {
                skill.ResetProgression();
                m_Skills.Remove(skill);
                m_RemovedSkills.Add(skill);
                m_OnSkillRemoved?.Invoke(skill);
                UnregisterCastingEvents(skill);
            }
        }
        private void UnregisterCastingEvents(Skill skill)
        {
            if (m_StartListeners.TryGetValue(skill, out var start))
            {
                skill.OnCastingStartEvent.RemoveListener(start);
                m_StartListeners.Remove(skill);
            }

            if (m_UpdateListeners.TryGetValue(skill, out var update))
            {
                skill.OnCastingUpdateEvent.RemoveListener(update);
                m_UpdateListeners.Remove(skill);
            }
            if (m_InterruptListeners.TryGetValue(skill, out var interupt))
            {
                skill.OnCastingInterruptEvent.RemoveListener(interupt);
                m_InterruptListeners.Remove(skill);
            }

            if (m_SuccessListeners.TryGetValue(skill, out var success))
            {
                skill.OnCastingSuccessEvent.RemoveListener(success);
                m_SuccessListeners.Remove(skill);
            }

            if (m_FailListeners.TryGetValue(skill, out var fail))
            {
                skill.OnCastingFailEvent.RemoveListener(fail);
                m_FailListeners.Remove(skill);
            }
        }

        private void TakeInteruptDamageInternal(int damage)
        {
            if (m_CurrentCastingSkill != null)
            {
                m_CurrentCastingSkill.TakeInteruptDamage(damage);
            }
        }
        public void TakeInteruptDamage(int damage)
        {
            TakeInteruptDamageInternal(damage);
        }
        public void AddCharges(SkillConfig[] skillConfigs, int chargeAmount)
        {
            AddChargesInternal(skillConfigs, chargeAmount);
        }
        private void AddChargesInternal(SkillConfig[] skillConfigs, int chargeAmount)
        {
            foreach (SkillConfig config in skillConfigs)
            {
                AddChargeInternal(config, chargeAmount);
            }
            m_OnCharge?.Invoke(chargeAmount);
        }
        private void AddChargeInternal(SkillConfig skillConfig, int chargeAmount)
        {
            if (HasSkillActivatorInternal(skillConfig.BaseInfo.Id, out Skill skill))
            {
                skill.AddCharge(chargeAmount);
            }
        }
        public void ForceActives(SkillConfig[] skillConfigs)
        {
            foreach (SkillConfig config in skillConfigs)
            {
                ForceActiveInternal(config);
            }
        }
        public void ForceActivesOverrideTargets(SkillConfig[] skillConfigs, List<ITargetable> targetables)
        {
            foreach(SkillConfig config in skillConfigs)
            {
                ForceActivateOverrideTargetInternal(config, targetables);
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
        public void ForceActiveOverrideTarget(SkillConfig skillConfig, List<ITargetable> targetables)
        {
            ForceActivateOverrideTargetInternal(skillConfig, targetables);
        }
        private void ForceActivateOverrideTargetInternal(SkillConfig skillConfig, List<ITargetable> targets)
        {
            if (HasSkillActivatorInternal(skillConfig.BaseInfo.Id, out Skill skill))
            {
                skill.ForceActivateAllOverrideTargets(targets);
            }
            else
            {
                AddNewSkillInternal(skillConfig);
                if (HasSkillActivatorInternal(skillConfig.BaseInfo.Id, out Skill skillAdded))
                {
                    skillAdded.ForceActivateAllOverrideTargets(targets);
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
            if (m_Skills.Count == 0) return;
            if (m_ModuleContext != null)
            {
                List<Skill> skillsToReset = new List<Skill>(m_Skills);
                foreach (Skill skill in skillsToReset)
                {
                    skill.ResetProgression();
                    UnregisterSkillInternal(skill);
                }
                m_OnResetProgress?.Invoke();
            }
        }

        public CategorySkillController GetCategoryController(SkillCategoryConfig categoryConfig)
        {
            return m_CategorySkillControllers.FirstOrDefault(x => x.SkillCategoryConfig.BaseInfo.Id == categoryConfig.BaseInfo.Id);
        }
    }
}
