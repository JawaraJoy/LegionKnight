
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Rush;

namespace LegionKnight
{
    public partial class SkillView : UIView
    {
        [SerializeField]
        private string m_SkillName;
        [SerializeField]
        private Image m_Fill;
        [SerializeField]
        private Image m_Icon;
        [SerializeField]
        private TextMeshProUGUI m_SkilNameText;
        [SerializeField]
        private TextMeshProUGUI m_ChargeText;
        [SerializeField]
        private UnityEvent m_OnActive;
        [SerializeField]
        private UnityEvent<int> m_OnChargeAmount;
        public string SkillName => m_SkillName;

        private Skill m_Skill;
        public void Init(Skill skill)
        {
            m_Skill = skill;
            m_SkillName = m_Skill.SkillConfig.BaseInfo.Name;
            m_Icon.sprite = m_Skill.SkillConfig.CollectibleField.Icon;
            m_SkilNameText.text = m_SkillName;
        }
        public void SetFill(float fill)
        {
            m_Fill.fillAmount = fill;
        }
        public void SetRemainingAmount(int amount)
        {
            string fillText = $"{amount}/{m_Skill.SkillConfig.Activation.Charge}";
            m_Fill.fillAmount = (float)amount / m_Skill.SkillConfig.Activation.Charge;
            m_ChargeText.text = fillText;
        }
        public void ChargeAmount(int amount)
        {
            m_OnChargeAmount?.Invoke(amount);
            Debug.Log($"[Charge Mana] {amount} for skill {m_SkillName}");
        }

        public void Active()
        {
            m_OnActive?.Invoke();
        }
    }
}
