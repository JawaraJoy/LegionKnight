using UnityEngine;
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
        public string SkillName => m_SkillName;
        public void SetFill(float fill)
        {
            m_Fill.fillAmount = fill;
        }
    }
}
