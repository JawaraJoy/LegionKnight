using UnityEngine;
using LegionKnight;
using TMPro;
using UnityEngine.UI;

namespace Rush
{
    public class HeroSkillDescription : UIView
    {
        [SerializeField]
        private Image m_SkillIcon;
        [SerializeField]
        private TextMeshProUGUI m_SkillNameText;
        [SerializeField]
        private TextMeshProUGUI m_SkillDescriptionText;

        private void ShowDetailInternal(SkillConfig skillConfig)
        {
            m_SkillIcon.sprite = skillConfig.CollectibleField.Icon;
            m_SkillNameText.text = skillConfig.BaseInfo.Name;
            m_SkillDescriptionText.text = skillConfig.BaseInfo.Description;
            ShowInternal();
        }
        public void ShowDetail(SkillConfig skillConfig)
        {
            ShowDetailInternal(skillConfig);
        }
    }
}
