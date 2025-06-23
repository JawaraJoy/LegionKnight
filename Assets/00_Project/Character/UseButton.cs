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

        private CharacterUnit m_CharacterUnit;
        public void Init(CharacterDefinition defi)
        {
            CharacterUnit unit = Player.Instance.GetCharacterUnit(defi);
            m_CharacterUnit = unit;
            bool isCharacterUsed = unit.IsUsed;
            m_UseButton.interactable = !isCharacterUsed;

            m_UseText.text = isCharacterUsed ? "Used" : "Use";
        }
        public void Init()
        {
            if (m_CharacterUnit == null) return;
            bool isCharacterUsed = m_CharacterUnit.IsUsed;
            m_UseButton.interactable = !isCharacterUsed;
            m_UseText.text = isCharacterUsed ? "Used" : "Use";
        }
    }
}
