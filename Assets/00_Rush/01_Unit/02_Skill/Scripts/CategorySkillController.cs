using MoreMountains.Tools;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;

namespace Rush
{
    public class CategorySkillController : MonoBehaviour
    {
        [SerializeField]
        private SkillController m_SkillController;
        [SerializeField]
        private SkillCategoryConfig m_SkillCategoryConfig;
        [SerializeField]
        private ActivationMode m_ActivationMode;
        public SkillCategoryConfig SkillCategoryConfig => m_SkillCategoryConfig;

        private int m_QueueIndex;

        [SerializeField, MMReadOnly]
        private List<Skill> m_Skills = new();
        [SerializeField]
        private UnityEvent<int> m_OnQueueChanged;
        [SerializeField]
        private UnityEvent<int, int> m_OnChargeSkill;
        [SerializeField]
        private UnityEvent<Skill> m_OnSkillAdded;
        [SerializeField]
        private UnityEvent<Skill> m_OnCurrentSkillChanged;
        [SerializeField]
        private UnityEvent<Skill> m_OnSkillRemoved;
        [SerializeField]
        private UnityEvent<Skill> m_OnForceActive;
        private Skill GetCurrentSkill()
        {
            if (m_Skills.Count == 0)
                return null;

            m_QueueIndex %= m_Skills.Count;

            return m_Skills[m_QueueIndex];
        }
        private void Init()
        {
            m_QueueIndex = 0;
            OnCurrentSkillChangedInvoke(GetCurrentSkill());
        }

        // it called on other class unityevent
        public void AddSkill(Skill skill)
        {
            AddSkillInternal(skill);
        }
        private void AddSkillInternal(Skill skill)
        {
            if (skill == null || skill.SkillConfig.Category != m_SkillCategoryConfig || m_Skills.Contains(skill))
                return;
            skill.OnTryActivate.RemoveListener(OnSkillActivatedInvoke);
            skill.OnChargeUpdate.RemoveListener(OnChargeSkillInvoke);
            m_Skills.Add(skill);
            skill.OnTryActivate.AddListener(OnSkillActivatedInvoke);
            skill.OnChargeUpdate.AddListener(OnChargeSkillInvoke);
            OnSkillAddedInvoke(skill);
            if (m_Skills.Count == 1)
                Init();
        }
        private void UnregisterSkillListenersInternal(Skill skill)
        {
            if (skill == null)
                return;
            skill.OnTryActivate.RemoveListener(OnSkillActivatedInvoke);
            skill.OnChargeUpdate.RemoveListener(OnChargeSkillInvoke);
        }
        private void AddAbilityToSkills(AbilityConfig abilityConfig)
        {
            foreach (Skill skill in m_Skills)
            {
                skill.AddAbility(abilityConfig);
            }
        }
        public void AddStatusEffectOnSelf<T>(StatusEffectConfig statusEffectConfig) where T : IAbilityDeliver
        {
            AddStatusEffectOnSelfInternal<T>(statusEffectConfig);
        }
        public void AddStatusEffectOnTarget<T>(StatusEffectConfig statusEffectConfig) where T : IAbilityDeliver
        {
            AddStatusEffectOnTargetInternal<T>(statusEffectConfig);
        }
        private void AddStatusEffectOnSelfInternal<T>(StatusEffectConfig statusEffectConfig) where T : IAbilityDeliver
        {
            foreach (IAbilityDeliver deliver in GetAbilityDeliversInternal<T>())
            {
                deliver.AddCustomStatusEffectOnSelf(statusEffectConfig);
            }
        }
        private void AddStatusEffectOnTargetInternal<T>(StatusEffectConfig statusEffectConfig) where T : IAbilityDeliver
        {
            foreach (IAbilityDeliver deliver in GetAbilityDeliversInternal<T>())
            {
                deliver.AddCustomStatusEffectOnDelivered(statusEffectConfig);
            }
        }

        private IAbilityDeliver[] GetAbilityDeliversInternal<T>() where T : IAbilityDeliver
        {
            List<IAbilityDeliver> delivers = new List<IAbilityDeliver>();
            foreach (Skill skill in m_Skills)
            {
                delivers.AddRange(skill.Delivers);
            }
            delivers.RemoveAll(deliver => !(deliver is T));
            return delivers.ToArray();

        }
        // it called on other class unityevent
        public void RemoveSkill(Skill skill)
        {
            if (m_Skills.Contains(skill))
            {
                UnregisterSkillListenersInternal(skill);

                m_Skills.Remove(skill);

                if (m_Skills.Count == 0)
                    m_QueueIndex = 0;
                else
                    m_QueueIndex %= m_Skills.Count;

                OnSkillRemovedInvoke(skill);
            }
        }
        public void AddCharge(int amount)
        {
            if (m_Skills.Count == 0)
                return;

            switch (m_ActivationMode)
            {
                case ActivationMode.All:
                    foreach (Skill skill in m_Skills)
                        skill.AddCharge(amount);
                    break;

                case ActivationMode.Queue:
                    m_Skills[m_QueueIndex].AddCharge(amount);
                    break;
            }
            int currentCharge = Mathf.RoundToInt(GetCurrentSkill().RemainingCharge);
            int maxCharge = Mathf.RoundToInt(GetCurrentSkill().SkillConfig.Activation.Charge);
            OnChargeSkillInvoke(currentCharge, maxCharge);
        }

        private void OnChargeSkillInvoke(int currentCharge, int maxCharge)
        {
            m_OnChargeSkill.Invoke(currentCharge, maxCharge);
        }
        public void AddLevel(int amount)
        {
            foreach (Skill skill in m_Skills)
            {
                skill.Progression.AddLevel(amount);
            }
        }
        public void ForceActives()
        {
            switch(m_ActivationMode)
            {
                case ActivationMode.All:
                    ForceActiveAll();
                    break;
                case ActivationMode.Queue:
                    ForceActiveQueue();
                    break;
            }
        }
        private void OnSkillActivatedInvoke(Skill skill)
        {
            if (m_ActivationMode != ActivationMode.Queue)
                return;

            if (m_Skills.Count == 0)
                return;

            if (m_Skills[m_QueueIndex] == skill)
            {
                m_QueueIndex = (m_QueueIndex + 1) % m_Skills.Count;
            }
            OnCurrentSkillChangedInvoke(GetCurrentSkill());
            int currentCharge = Mathf.RoundToInt(GetCurrentSkill().RemainingCharge);
            int maxCharge = Mathf.RoundToInt(GetCurrentSkill().SkillConfig.Activation.Charge);
            OnChargeSkillInvoke(currentCharge, maxCharge);
            OnForceActiveInvoke(skill);
        }
        private void ForceActiveAll()
        {
            foreach (Skill skill in m_Skills)
            {
                skill.ForceActivateAll();
            }
        }
        private void ForceActiveQueue()
        {
            if (m_Skills.Count == 0) return;
            m_Skills[m_QueueIndex].ForceActivateAll();
            m_QueueIndex = (m_QueueIndex + 1) % m_Skills.Count;
        }
        public void ForceActiveByIndex(int index)
        {
            if (m_Skills.Count == 0) return;

            index = Mathf.Clamp(index, 0, m_Skills.Count - 1);

            m_Skills[index].ForceActivateAll();
            OnQueueChangedInvoke(index);
        }
        public void ClearSkills()
        {
            foreach (var skill in m_Skills)
            {
                UnregisterSkillListenersInternal(skill);
            }

            m_Skills.Clear();
            m_QueueIndex = 0;
            OnQueueChangedInvoke(m_QueueIndex);
        }
        public void SetQueueIndex(int index)
        {
            m_QueueIndex = Mathf.Clamp(index, 0, m_Skills.Count - 1);
            OnQueueChangedInvoke(m_QueueIndex);
            OnCurrentSkillChangedInvoke(GetCurrentSkill());
        }
    

private void OnQueueChangedInvoke(int queueIndex)
        {
            m_OnQueueChanged?.Invoke(queueIndex);
        }

        private void OnSkillAddedInvoke(Skill skill)
        {
            m_OnSkillAdded?.Invoke(skill);
        }

        private void OnCurrentSkillChangedInvoke(Skill skill)
        {
            m_OnCurrentSkillChanged?.Invoke(skill);
        }

        private void OnSkillRemovedInvoke(Skill skill)
        {
            m_OnSkillRemoved?.Invoke(skill);
        }

        private void OnForceActiveInvoke(Skill skill)
        {
            m_OnForceActive?.Invoke(skill);
        }
}
}
