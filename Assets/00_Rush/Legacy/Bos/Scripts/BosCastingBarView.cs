using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LegionKnight
{
    public class BosCastingBarView : UIView
    {
        [SerializeField]
        private TextMeshProUGUI m_CastingTimeText;
        [SerializeField]
        private Slider m_CastBar;

        public void SetCastingName(string castingName)
        {
            m_CastingTimeText.text = castingName;
            ShowInternal();
        }

        public void SetCastingTime(float castingTime)
        {
            m_CastBar.value = castingTime;
        }
    }
}
