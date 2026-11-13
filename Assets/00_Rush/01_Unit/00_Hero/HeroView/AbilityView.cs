using LegionKnight;
using MoreMountains.Tools;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Rush
{
    public class AbilityView : UIView
    {
        [SerializeField]
        private SkillCategoryConfig m_SkillCategoryConfig;
        [SerializeField, MMReadOnly]
        private UnitConfig m_UnitConfig;
        [SerializeField]
        private Image m_Icon;

        [SerializeField]
        private Button m_Button;

        [SerializeField]
        private UnityEvent<SkillConfig> m_OnDetailView;

        [SerializeField, MMReadOnly]
        private SkillDescriptionPanel m_SkillDescription;

        private SkillConfig m_SelectedSkill;

        public SkillDescriptionPanel SkillDescription
        {
            get
            {
                if (m_SkillDescription == null)
                {
                    m_SkillDescription = CanvasManager.Instance.GetPanel<SkillDescriptionPanel>();  
                }
                return m_SkillDescription;
            }
        }
        private Skill m_RuntimeSkill;

        public void Init(UnitConfig unitConfig)
        {
            SkillConfig[] skills = unitConfig.GetSkillsByCategory(m_SkillCategoryConfig);
            if (skills.Length == 0 || skills[0] == null)
            {
                m_SelectedSkill = null;
                m_RuntimeSkill = null;
                HideInternal();
                return;
            }
            Bind(skills[0], unitConfig, null);
            if (!IsShown) ShowInternal();
        }

        public void Init(SkillConfig skill) => Bind(skill, null, null);

        public void InitRuntime(Skill skill, UnitConfig owner) => Bind(skill.SkillConfig, owner, skill);

        public void InitPreview(SkillConfig skill, UnitConfig owner) => Bind(skill, owner, null);

        private void Bind(SkillConfig config, UnitConfig owner, Skill runtime)
        {
            m_UnitConfig = owner;
            m_SelectedSkill = config;
            m_RuntimeSkill = runtime;
            m_Icon.sprite = config.CollectibleField.Icon;
            m_Button.onClick.RemoveListener(OnButtonClickedInternal);
            m_Button.onClick.AddListener(OnButtonClickedInternal);
        }

        private void OnButtonClickedInternal()
        {
            if (m_SelectedSkill == null) return;
            if (m_RuntimeSkill != null) SkillDescription.ShowRuntimeDetail(m_RuntimeSkill);
            else SkillDescription.ShowPreview(m_SelectedSkill, m_UnitConfig);
            m_OnDetailView?.Invoke(m_SelectedSkill);
        }

        private void OnDestroy()
        {
            if (m_Button != null) m_Button.onClick.RemoveListener(OnButtonClickedInternal);
        }
    }
}
