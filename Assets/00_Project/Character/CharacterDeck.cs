using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public partial class CharacterDeck : MonoBehaviour
    {
        [SerializeField]
        private CharacterDefinition m_DefaultCharacter;
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
        [SerializeField]
        private UnityEvent<CharacterDefinition> m_OnCharacterLevelUp = new();
        [SerializeField]
        private UnityEvent<CharacterDefinition> m_OnCharacterStarUp = new();

        public UnityEvent<CharacterDefinition> OnCharacterLevelUp => m_OnCharacterLevelUp;
        public UnityEvent<CharacterDefinition> OnCharacterStarUp => m_OnCharacterStarUp;
        public CharacterDefinition DefaultCharacter => m_DefaultCharacter;
        public List<CharacterUnit> CharacterUnits => m_CharacterUnits;
        public CharacterDefinition UsedCharacter => m_UsedCharacter;
        public StandbyPlatformDefinition GetHeroStandbyPlatform()
        {
            return GetCharacterUnitInternal(m_UsedCharacter).UniquePlatform;
        }
        private CharacterUnit GetCharacterUnitInternal(CharacterDefinition definition)
        {
            CharacterUnit match = m_CharacterUnits.Find(x => x.Definition == definition);
            return match;
        }
        private CharacterUnit GetCharacterUnitInternal(string id)
        {
            CharacterUnit match = m_CharacterUnits.Find(x => x.Definition.Id == id);
            return match;
        }
        public CharacterUnit GetCharacterUnit(CharacterDefinition definition)
        {
            return GetCharacterUnitInternal(definition);
        }

        public void Init()
        {
            if (UnityService.Instance.HasData("usedcharacter"))
            {
                m_UsedCharacter = GetCharacterUnitInternal(UnityService.Instance.GetData<string>("usedcharacter")).Definition;
            }
            else
            {
                m_UsedCharacter = m_DefaultCharacter;
                UnityService.Instance.SaveData("usedcharacter", m_UsedCharacter.Id);
            }
            OnInitializedInvoke();
        }
        private void OnInitializedInvoke()
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
            GetCharacterUnitInternal(m_UsedCharacter).SetIsUsed(true);
            foreach (CharacterUnit unit in m_CharacterUnits)
            {
                if (unit.Definition != m_UsedCharacter)
                {
                    unit.SetIsUsed(false);
                }
            }
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
        private void OnCharacterLevelUpInvoke(CharacterDefinition defi)
        {
            m_OnCharacterLevelUp?.Invoke(defi);
        }
        public void AddExp(CharacterDefinition defi, int exp)
        {
            GetCharacterUnitInternal(defi).AddExp(exp);
        }
        public void LevelUp()
        {
            GetCharacterUnitInternal(m_UsedCharacter).LevelUp();
        }
        public int GetLevel(CharacterDefinition defi) => GetCharacterUnitInternal(defi).Level;
        public int GetExp(CharacterDefinition defi) => GetCharacterUnitInternal(defi).Exp;
        public int GetStar(CharacterDefinition defi) => GetCharacterUnitInternal(defi).Star;
        public int GetCurrentMaxExp(CharacterDefinition defi) => GetCharacterUnitInternal(defi).CurrentMaxExp;
        public CurrencyDefinition GetShardCurrency(CharacterDefinition defi)
        {
            return GetCharacterUnitInternal(defi).ShardDefinition;
        }
    }
}
