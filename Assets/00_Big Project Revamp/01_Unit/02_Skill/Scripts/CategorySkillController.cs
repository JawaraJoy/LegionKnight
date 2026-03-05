using MoreMountains.Tools;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

        // it called on other class unityevent
        public void AddSkill(Skill skill)
        {
            if (skill.SkillConfig.Category == m_SkillCategoryConfig)
            {
                m_Skills.Add(skill);
            }
           
        }
        // it called on other class unityevent
        public void RemoveSkill(Skill skill)
        {
            if (m_Skills.Contains(skill))
            {
                m_Skills.Remove(skill);
            }
            
        }
        public void AddCharge(int amount)
        {
            foreach (Skill skill in m_Skills)
            {
                skill.AddCharge(amount);
            }
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
        }
        public void ClearSkills()
        {
            m_Skills.Clear();
        }
        public void SetQueueIndex(int index)
        {
            m_QueueIndex = Mathf.Clamp(index, 0, m_Skills.Count - 1);
        }
    }
}
