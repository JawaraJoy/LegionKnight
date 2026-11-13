using System.Collections.Generic;
using System.Linq;
using LegionKnight;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Rush
{
    public class SkillDescriptionPanel : PanelView
    {
        [SerializeField] private Image m_SkillIcon;
        [SerializeField] private TextMeshProUGUI m_SkillNameText;
        [SerializeField] private TextMeshProUGUI m_SkillDescriptionText;
        [SerializeField] private HeroSkillDetailsView m_Details;

        private SkillConfig m_Config;
        private UnitConfig m_Owner;
        private Skill m_Runtime;
        private SkillController m_Controller;
        private readonly List<ProgressField> m_Observed = new();

        public void ShowDetail(SkillConfig config) => ShowPreview(config, null);

        public void ShowPreview(SkillConfig config, UnitConfig owner)
        {
            if (config == null) return;
            ClearRuntime();
            m_Config = config;
            m_Owner = owner;
            ShowInternal();
            Display(true);
        }

        public void ShowRuntimeDetail(Skill skill)
        {
            if (skill == null || skill.SkillConfig == null) return;
            ClearRuntime();
            m_Config = skill.SkillConfig;
            m_Runtime = skill;
            var unit = skill.SkillContext?.ModuleContext?.Unit;
            m_Owner = unit != null ? unit.Config : null;
            if (unit != null) unit.HasBind(out m_Controller);
            ShowInternal();
            if (IsShown)
            {
                Observe(skill);
                if (m_Controller != null)
                {
                    foreach (var learned in m_Controller.Skills) Observe(learned);
                    m_Controller.OnSkillAdded?.AddListener(OnRosterChanged);
                    m_Controller.OnSkillRemoved?.AddListener(OnRosterChanged);
                }
            }
            Display(true);
        }

        private void Observe(Skill skill)
        {
            var progress = skill != null ? skill.Progression : null;
            if (progress == null || m_Observed.Contains(progress)) return;
            m_Observed.Add(progress);
            progress.OnLevelChanged.AddListener(OnLevelChanged);
        }

        private void OnRosterChanged(Skill skill)
        {
            if (m_Controller != null && !m_Controller.Skills.Contains(m_Runtime)) { Hide(); return; }
            StopObservingLevels();
            Observe(m_Runtime);
            foreach (var learned in m_Controller.Skills) Observe(learned);
            Display(false);
        }

        private void OnLevelChanged(int level) => Display(false);

        private void Display(bool resetScroll)
        {
            if (m_Config == null) return;
            int level = m_Runtime != null ? m_Runtime.Progression.Level : m_Config.LevelSet?.Level ?? 1;
            if (m_Details != null)
                m_Details.Display(m_Config, m_Owner, level, GameplayDescription.ForSkillPanel(m_Config, level, m_Controller), resetScroll);
            else
            {
                m_SkillIcon.sprite = m_Config.CollectibleField.Icon;
                m_SkillNameText.text = m_Config.BaseInfo.Name;
                m_SkillDescriptionText.text = GameplayDescription.Skill(m_Config, level, m_Controller);
            }
        }

        protected override void HideInternal()
        {
            ClearRuntime();
            base.HideInternal();
        }

        private void OnDisable() => ClearRuntime();

        private void StopObservingLevels()
        {
            foreach (var progress in m_Observed) progress.OnLevelChanged.RemoveListener(OnLevelChanged);
            m_Observed.Clear();
        }

        private void ClearRuntime()
        {
            StopObservingLevels();
            if (m_Controller != null)
            {
                m_Controller.OnSkillAdded?.RemoveListener(OnRosterChanged);
                m_Controller.OnSkillRemoved?.RemoveListener(OnRosterChanged);
            }
            m_Controller = null;
            m_Runtime = null;
        }
    }
}
