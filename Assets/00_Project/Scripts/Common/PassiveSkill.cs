using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    [System.Serializable]
    public partial class SkillActivation
    {
        [SerializeField]
        private string m_SkillName;
        [SerializeField]
        private int m_ManaThreshold;
        private int m_Mana;
        [SerializeField]
        private bool m_HasSkillView = false;
        [SerializeField]
        private UnityEvent m_OnActive = new();
        public void Init()
        {
            //spawn to ui;
        }
        public SkillActivation(SkillDefinition definition)
        {
            m_SkillName = definition.SkillName;
            m_ManaThreshold = definition.Manathreshold;
            m_HasSkillView = definition.HasSkillView;
            m_OnActive.AddListener(definition.SpawnSkills);
        }

        private float m_FillRate;
        public void ForceActivated()
        {
            m_OnActive?.Invoke();
        }
        public void AddMana(int add)
        {
            m_Mana += add;
            OnActiveInvoke();
        }
        public void SetMana(int set)
        {
            m_Mana = set;
            OnActiveInvoke();
        }

        private void OnActiveInvoke()
        {
            if (m_Mana >= m_ManaThreshold)
            {
                m_OnActive?.Invoke();
                m_Mana = 0;
            }
            m_FillRate = (float)m_Mana / (float)m_ManaThreshold;
            if (!m_HasSkillView) return;
            GameManager.Instance.SetSkillViewFill(m_SkillName, m_FillRate);
        }
    }

    public partial class PassiveSkill : MonoBehaviour
    {
        [SerializeField]
        private List<SkillActivation> m_SkillActivations = new();
        public void Init(List<SkillDefinition> definitions)
        {
            foreach(SkillDefinition skill in definitions)
            {
                AddSkillInternal(skill);
            }
        }
        public void ForceActivated(int indexSkill)
        {
            m_SkillActivations[indexSkill].ForceActivated();
        }

        public void AddOneMana(int indexSkill)
        {
            m_SkillActivations[indexSkill].AddMana(1);
        }
        public void ResetMana(int indexSkill)
        {
            m_SkillActivations[indexSkill].SetMana(0);
        }
        public void AddSkill(SkillDefinition definition)
        {
            AddSkillInternal(definition);
        }
        public void AddSkillInternal(SkillDefinition definition)
        {
            SkillActivation newSkill = new SkillActivation(definition);
            if (m_SkillActivations.Contains(newSkill)) return;
            m_SkillActivations.Add(newSkill);
            newSkill.Init();
        }
    }
}
