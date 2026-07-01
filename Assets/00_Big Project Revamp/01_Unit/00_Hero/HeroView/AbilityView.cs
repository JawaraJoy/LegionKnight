using LegionKnight;
using MoreMountains.Tools;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Rush
{
    public class AbilityView : MonoBehaviour
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
        private SkillDescriptionPanel m_HeroSkillDescription;

        public SkillDescriptionPanel HeroSkillDescription
        {
            get
            {
                if (m_HeroSkillDescription == null)
                {
                    m_HeroSkillDescription = CanvasManager.Instance.GetPanel<SkillDescriptionPanel>();  
                }
                return m_HeroSkillDescription;
            }
        }
        private void InitInternal(UnitConfig unitConfig)
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
            
        }
        public void Init(UnitConfig unitConfig)
        {
            InitInternal(unitConfig);
        }

        private void DetailView(SkillConfig skillConfig)
        {
            HeroSkillDescription.ShowDetail(skillConfig);
            m_OnDetailView.Invoke(skillConfig);
        }
    }
}
