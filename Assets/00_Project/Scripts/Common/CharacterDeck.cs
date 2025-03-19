using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public partial class CharacterDeck : MonoBehaviour
    {
        [SerializeField]
        private CharacterDefinition m_UsedCharacter;
        [SerializeField]
        private CharacterDefinition m_SelectedCharacter;
        [SerializeField]
        private List<CharacterUnit> m_CharacterUnits = new();
        [SerializeField]
        private UnityEvent<CharacterDefinition> m_OnInitialized = new();
        [SerializeField]
        private UnityEvent<CharacterDefinition> m_OnCharacterUsed = new();
        [SerializeField]
        private UnityEvent<CharacterDefinition> m_OnSelectedCharacter = new();
        private CharacterUnit GetCharacterUnit(CharacterDefinition definition)
        {
            CharacterUnit match = m_CharacterUnits.Find(x => x.Definition == definition);
            return match;
        }
        public void Init()
        {
            OnInitialized();
        }
        private void OnInitialized()
        {
            m_OnInitialized?.Invoke(m_UsedCharacter);
        }
        public void SetOwned(CharacterDefinition defi, bool set)
        {
            GetCharacterUnit(defi).SetOwned(set);
        }
        public void UsedCharacter()
        {
            m_UsedCharacter = m_SelectedCharacter;
            OnCharacterUsedInvoke();
        }
        public void SetSelectedCharacter(CharacterDefinition defi)
        {
            m_SelectedCharacter = defi;
            OnSelectedCharacterInvoke();
        }
        private void OnCharacterUsedInvoke()
        {
            m_OnCharacterUsed?.Invoke(m_UsedCharacter);
        }
        private void OnSelectedCharacterInvoke()
        {
            m_OnSelectedCharacter?.Invoke(m_SelectedCharacter);
            GameManager.Instance.SetCharacterSelected(m_SelectedCharacter);
        }
    }
}
