using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace LegionKnight
{
    public partial class SkillView : MonoBehaviour
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
        public string SkillName => m_SkillName;

        private SkillDefinition m_SkillDefinition;

        public void Init(SkillDefinition skillDefinition)
        {
            m_SkillDefinition = skillDefinition;
            m_SkillName = m_SkillDefinition.SkillName;
            m_Icon.sprite = m_SkillDefinition.Icon;
            m_SkilNameText.text = m_SkillName;
        }
        public void SetFill(float fill)
        {
            m_Fill.fillAmount = fill;
        }

        public void Active()
        {
            m_OnActive?.Invoke();
        }
    }
}
