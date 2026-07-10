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
        private void InitByCategory(UnitConfig unitConfig)
        {
            m_UnitConfig = unitConfig;
            SkillConfig[] skillConfigs = unitConfig.GetSkillsByCategory(m_SkillCategoryConfig);
            if (skillConfigs.Length > 0)
            {
                SkillConfig skillConfig = skillConfigs[0];
                if (skillConfig == null)
                {
                    Debug.LogError($"SkillConfig is null for unit {unitConfig.name} and category {m_SkillCategoryConfig.name}");
                    return;
                }
                m_Icon.sprite = skillConfig.CollectibleField.Icon;
                m_Button.onClick.RemoveAllListeners();
                m_Button.onClick.AddListener(() => DetailView(skillConfig));
            }
            else
            {
                HideInternal();
            }
            
        }
        private void InitInternal(SkillConfig skill)
        {
            m_Icon.sprite = skill.CollectibleField.Icon;
            m_Button.onClick.RemoveAllListeners();
            m_Button.onClick.AddListener(() => DetailView(skill));
        }
        public void Init(UnitConfig unitConfig)
        {
            InitByCategory(unitConfig);
        }
        public void Init(SkillConfig skill)
        {
            InitInternal(skill);
        }
        private void DetailView(SkillConfig skillConfig)
        {
            SkillDescription.ShowDetail(skillConfig);
            m_OnDetailView.Invoke(skillConfig);
        }
    }
}
