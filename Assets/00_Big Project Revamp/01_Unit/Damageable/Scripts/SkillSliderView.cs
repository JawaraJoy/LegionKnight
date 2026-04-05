using LegionKnight;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Rush
{
    public class SkillSliderView : UIView
    {
        [SerializeField]
        private TextMeshProUGUI m_SkillNameText;
        [SerializeField]
        private Image m_SkillIcon;
        [SerializeField]
        private Image m_FillRate;

        public void SetSkill(Skill skill)
        {
            if (skill == null)
            {
                Debug.LogError("Skill is null.");
                return;
            }
            m_SkillNameText.text = skill.SkillConfig.BaseInfo.Name;
            m_SkillIcon.sprite = skill.SkillConfig.CollectibleField.Icon;
            int currentCharge = Mathf.RoundToInt(skill.RemainingCharge);
            int maxCharge = Mathf.RoundToInt(skill.SkillConfig.Activation.Charge);
            SetFillInternal(currentCharge, maxCharge);
        }
        public void SetFill(int current, int max)
        {
            SetFillInternal(current, max);
        }
        private void SetFillInternal(int current, int max)
        {
            m_FillRate.fillAmount = (float)current / max;
        }
    }
}
