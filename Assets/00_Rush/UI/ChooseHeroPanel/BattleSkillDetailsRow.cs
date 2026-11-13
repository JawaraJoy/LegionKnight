using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Rush
{
    public sealed class BattleSkillDetailsRow : MonoBehaviour
    {
        [Serializable]
        private sealed class Slot
        {
            [SerializeField] private SkillCategoryConfig m_Category;
            [SerializeField] private Button m_Button;
            [SerializeField] private Image m_Icon, m_Rim;
            private Skill m_Skill;
            private SkillDescriptionPanel m_Panel;

            public void Bind(SkillController controller, SkillDescriptionPanel panel)
            {
                m_Panel = panel;
                m_Skill = null;
                if (controller != null)
                {
                    foreach (var skill in controller.Skills)
                    {
                        if (skill == null || skill.SkillConfig?.Category != m_Category) continue;
                        m_Skill = skill;
                        break;
                    }
                }
                var art = m_Skill != null ? m_Skill.SkillConfig.CollectibleField : null;
                m_Button.interactable = m_Skill != null;
                m_Icon.sprite = art?.Icon;
                m_Icon.enabled = m_Icon.sprite != null;
                m_Rim.color = art?.RarityConfig != null ? art.RarityConfig.Color : Color.gray;
                m_Button.onClick.RemoveListener(Open);
                if (m_Skill != null) m_Button.onClick.AddListener(Open);
            }

            private void Open()
            {
                if (m_Skill != null) m_Panel.ShowRuntimeDetail(m_Skill);
            }
        }

        [SerializeField] private SkillDescriptionPanel m_Panel;
        [SerializeField] private Slot[] m_Slots;

        [SerializeField] private TMP_Text m_HeroName;
        [SerializeField] private Image m_Portrait;
        private SkillController m_Controller;

        private void OnEnable()
        {
            var hero = RushPlayer.Instance?.Unit;
            var config = hero?.Config;
            if (m_HeroName != null) m_HeroName.text = config != null ? config.BaseInfo.Name : string.Empty;
            if (m_Portrait != null)
            {
                m_Portrait.sprite = config?.CollectibleField?.Icon;
                m_Portrait.enabled = m_Portrait.sprite != null;
            }
            if (hero != null && hero.HasBind(out m_Controller))
            {
                m_Controller.OnSkillAdded?.AddListener(OnSkillsChanged);
                m_Controller.OnSkillRemoved?.AddListener(OnSkillsChanged);
            }
            RefreshSkills();
        }

        private void OnSkillsChanged(Skill skill) => RefreshSkills();

        private void RefreshSkills()
        {
            foreach (var slot in m_Slots) slot.Bind(m_Controller, m_Panel);
        }

        private void OnDisable()
        {
            if (m_Controller != null)
            {
                m_Controller.OnSkillAdded?.RemoveListener(OnSkillsChanged);
                m_Controller.OnSkillRemoved?.RemoveListener(OnSkillsChanged);
                m_Controller = null;
            }
            if (m_Panel != null && m_Panel.IsShown) m_Panel.Hide();
        }
    }
}
