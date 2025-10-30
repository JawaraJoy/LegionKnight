using TMPro;
using UnityEngine;

namespace LegionKnight
{
    public class AbilityEffectDescription : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI m_DescriptionText;

        public void SetDescription(string description)
        {
            if (string.IsNullOrEmpty(description))
            {
                m_DescriptionText.text = "No Description";
            }
            else
            {
                m_DescriptionText.text = description;
            }
        }
    }
}
