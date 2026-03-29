using Rush;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LegionKnight
{
    public class UseButton : MonoBehaviour
    {
        [SerializeField]
        private Button m_UseButton;

        [SerializeField]
        private TextMeshProUGUI m_UseText;

        private HeroUnit m_HeroUnit;
        public void Init(HeroUnitConfig heroConfig)
        {
            HeroUnit unit = Player.Instance.HeroesCollection.GetHeroUnit(heroConfig);
            m_HeroUnit = unit;
            bool isCharacterUsed = unit.IsUsed;
            m_UseButton.interactable = !isCharacterUsed;

            m_UseText.text = isCharacterUsed ? "Used" : "Use";
        }
        public void Init()
        {
            if (m_HeroUnit == null) return;
            bool isCharacterUsed = m_HeroUnit.IsUsed;
            m_UseButton.interactable = !isCharacterUsed;
            m_UseText.text = isCharacterUsed ? "Used" : "Use";
        }
    }
}
