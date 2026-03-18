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
            m_OnCurrentSkillChanged.Invoke(GetCurrentSkill());
        }

        // it called on other class unityevent
        public void AddSkill(Skill skill)
        {
            if (skill.SkillConfig.Category == m_SkillCategoryConfig)
            {
                m_Skills.Add(skill);
                skill.OnActivate.AddListener(OnSkillActivatedInvoke);
                skill.OnChargeUpdate.AddListener(OnChargeSkillInvoke);
                m_OnSkillAdded.Invoke(skill);
            }
            if (m_Skills.Count == 1)
            {
                Init();
            }
        }
        // it called on other class unityevent
        public void RemoveSkill(Skill skill)
        {
            if (m_Skills.Contains(skill))
            {
                skill.OnActivate.RemoveListener(OnSkillActivatedInvoke);

                m_Skills.Remove(skill);

                if (m_Skills.Count == 0)
                    m_QueueIndex = 0;
                else
                    m_QueueIndex %= m_Skills.Count;

                m_OnSkillRemoved.Invoke(skill);
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
            m_OnCurrentSkillChanged.Invoke(GetCurrentSkill());
            int currentCharge = Mathf.RoundToInt(GetCurrentSkill().RemainingCharge);
            int maxCharge = Mathf.RoundToInt(GetCurrentSkill().SkillConfig.Activation.Charge);
            m_OnChargeSkill.Invoke(currentCharge, maxCharge);
            m_OnForceActive.Invoke(skill);
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
            m_OnQueueChanged?.Invoke(index);
        }
        public void ClearSkills()
        {
            foreach (var skill in m_Skills)
            {
                skill.OnActivate.RemoveListener(OnSkillActivatedInvoke);
            }

            m_Skills.Clear();
            m_QueueIndex = 0;
            m_OnQueueChanged?.Invoke(m_QueueIndex);
        }
        public void SetQueueIndex(int index)
        {
            m_QueueIndex = Mathf.Clamp(index, 0, m_Skills.Count - 1);
            m_OnQueueChanged?.Invoke(m_QueueIndex);
            m_OnCurrentSkillChanged?.Invoke(GetCurrentSkill());
        }
    }
}
