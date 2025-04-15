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
        public List<CharacterUnit> CharacterUnits => m_CharacterUnits;
        public CharacterDefinition UsedCharacter => m_UsedCharacter;
        private CharacterUnit GetCharacterUnitInternal(CharacterDefinition definition)
        {
            CharacterUnit match = m_CharacterUnits.Find(x => x.Definition == definition);
            return match;
        }
        public CharacterUnit GetCharacterUnit(CharacterDefinition definition)
        {
            return GetCharacterUnitInternal(definition);
        }

        public void Init()
        {
            OnInitialized();
        }
        private void OnInitialized()
        {
            m_OnInitialized?.Invoke(m_UsedCharacter);
            foreach(CharacterUnit unit in m_CharacterUnits)
            {
                unit.Init();
            }
        }
        public void SetOwned(CharacterDefinition defi, bool set)
        {
            GetCharacterUnitInternal(defi).SetOwned(set);
        }
        public void SetUsedCharacter()
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
