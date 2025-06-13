using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace LegionKnight
{
    public class CharacterDefinitionSetter : MonoBehaviour
    {
        [SerializeField]
        private CharacterDefinition m_CharacterDefinition;

        [SerializeField]
        private UnityEvent<CharacterDefinition> m_OnCharacterDefinitionSet;
        [SerializeField]
        private UnityEvent<CharacterDefinition> m_OnSetCharacterButtonClicked;

        [SerializeField]
        private Button m_SetCharacterButton;


        private void Start()
        {
            m_SetCharacterButton.onClick.AddListener(SetCharacterDefinitionFromButton);
        }
        public void SetCharacterDefinition(CharacterDefinition characterDefinition)
        {
            m_CharacterDefinition = characterDefinition;
            m_OnCharacterDefinitionSet?.Invoke(m_CharacterDefinition);

        }

        public void SetCharacterDefinitionFromButton()
        {
            if (m_CharacterDefinition != null)
            {
                m_OnSetCharacterButtonClicked.Invoke(m_CharacterDefinition);
            }
            else
            {
                Debug.LogWarning("Character Definition is not set.");
            }
        }
    }
}
