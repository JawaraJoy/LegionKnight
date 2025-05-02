using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public partial class Player : Singleton<Player>
    {
        [SerializeField]
        private NameSupplyDefinition m_NameSupplyDefinition;
        [SerializeField]
        private string m_PlayerName;
        [SerializeField]
        private CharacterDefinition m_CharacterDefinition;

        public CharacterDefinition CharacterDefinition => m_CharacterDefinition;
        [SerializeField]
        private UnityEvent m_OnStart = new();
        [SerializeField]
        private UnityEvent<CharacterDefinition> m_OnSetCharacterDefinition = new();
        public void Init()
        {
            
            OnStartInvoke();
        }
        public void SetCharacterDefinition(CharacterDefinition definition)
        {
            m_CharacterDefinition = definition;
            OnSetCharacterDefinitionInvoke(definition);
        }
        private void OnStartInvoke()
        {
            m_OnStart?.Invoke();

            m_PlayerName = UnityService.Instance.PlayerName;

            GameManager.Instance.SetPlayerNameView(m_PlayerName);
        }
        private void OnSetCharacterDefinitionInvoke(CharacterDefinition definition)
        {
            m_OnSetCharacterDefinition?.Invoke(definition);

        }
        public int Attack => m_CharacterDefinition.Attack;
    }
}
