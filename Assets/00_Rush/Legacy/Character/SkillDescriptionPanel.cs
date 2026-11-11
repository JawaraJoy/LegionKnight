using UnityEngine;
using LegionKnight;
using TMPro;
using UnityEngine.UI;

namespace Rush
{
    public class SkillDescriptionPanel : PanelView
    {
        [SerializeField]
        private Image m_SkillIcon;
        [SerializeField]
        private TextMeshProUGUI m_SkillNameText;
        [SerializeField]
        private TextMeshProUGUI m_SkillDescriptionText;

        private void ShowDetailInternal(SkillConfig skillConfig)
        {
            if (skillConfig == null) return;
            ShowDetailInternal(skillConfig, skillConfig.LevelSet.Level, null);
        }
        private void ShowDetailInternal(SkillConfig skillConfig, int level, SkillController controller)
        {
            if (skillConfig == null) return;
            m_SkillIcon.sprite = skillConfig.CollectibleField.Icon;
            m_SkillNameText.text = skillConfig.BaseInfo.Name;
            m_SkillDescriptionText.text = GameplayDescription.Skill(skillConfig, level, controller);
            ShowInternal();
        }
        public void ShowDetail(SkillConfig skillConfig)
        {
            ShowDetailInternal(skillConfig);
        }
        public void ShowRuntimeDetail(Skill skill)
        {
            if (skill == null) return;
            SkillController controller = null;
            if (skill.SkillContext != null && skill.SkillContext.ModuleContext != null && skill.SkillContext.ModuleContext.Unit != null)
                skill.SkillContext.ModuleContext.Unit.HasBind(out controller);
            ShowDetailInternal(skill.SkillConfig, skill.Progression.Level, controller);
        }
    }
}
