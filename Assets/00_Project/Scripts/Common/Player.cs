using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public partial class Player : Singleton<Player>
    {
        [SerializeField]
        private CharacterDefinition m_CharacterDefinition;

        public CharacterDefinition CharacterDefinition => m_CharacterDefinition;
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
        }
        public int Attack => m_CharacterDefinition.Attack;
    }
}
