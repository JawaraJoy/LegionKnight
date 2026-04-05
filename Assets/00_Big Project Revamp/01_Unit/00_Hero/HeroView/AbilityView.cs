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
        private TextMeshProUGUI m_NameText;

        [SerializeField]
        private Button m_Button;

        [SerializeField]
        private UnityEvent<SkillConfig> m_OnDetailView;

        [SerializeField]
        private HeroSkillDescription m_HeroSkillDescription;
        private void InitInternal(UnitConfig unitConfig)
        {
            m_UnitConfig = unitConfig;
            SkillConfig[] skillConfigs = unitConfig.GetSkillsByCategory(m_SkillCategoryConfig);
            if (skillConfigs.Length >= 0)
            {
                SkillConfig skillConfig = skillConfigs[0];
                m_Icon.sprite = skillConfig.CollectibleField.Icon;
                m_NameText.text = skillConfig.BaseInfo.Name;
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
            m_HeroSkillDescription.ShowDetail(skillConfig);
            m_OnDetailView.Invoke(skillConfig);
        }
    }
}
