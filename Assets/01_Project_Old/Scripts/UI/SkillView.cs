
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Rush;

namespace LegionKnight
{
    public partial class SkillView : PanelView
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
        private UnityEvent m_OnActive;
        [SerializeField]
        private UnityEvent<int> m_OnChargeAmount;
        public string SkillName => m_SkillName;

        private SkillConfig m_SkillConfig;
        public void Init(SkillConfig skillConfig)
        {
            m_SkillConfig = skillConfig;
            m_SkillName = m_SkillConfig.BaseInfo.Name;
            m_Icon.sprite = m_SkillConfig.CollectibleField.Icon;
            m_SkilNameText.text = m_SkillName;
        }
        public void SetFill(float fill)
        {
            m_Fill.fillAmount = fill;
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
