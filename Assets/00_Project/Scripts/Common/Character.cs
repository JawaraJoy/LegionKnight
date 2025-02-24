using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    
    public partial class Character : MonoBehaviour
    {
        [SerializeField]
        private CharacterDefinition m_CharacterDefinition;
        [SerializeField]
        private CharacterSprite m_CharacterModel;
        
        public CharacterDefinition CharacterDefinition => m_CharacterDefinition;

        private void Start()
        {
            if (m_CharacterDefinition == null) return;
            OnSetCharacterDefinitionInvoke(m_CharacterDefinition);
        }
        [SerializeField]
        private UnityEvent<CharacterDefinition> m_OnSetCharacterDefinition = new();
        public void SetCharacterDefinition(CharacterDefinition definition)
        {
            m_CharacterDefinition = definition;
            OnSetCharacterDefinitionInvoke(definition);
        }

        private void OnSetCharacterDefinitionInvoke(CharacterDefinition definition)
        {
            m_OnSetCharacterDefinition?.Invoke(definition);
            m_CharacterModel.SetSprite(m_CharacterDefinition.Icon);
        }
    }
}
