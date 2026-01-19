using System;
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
        private UnityEvent<int> m_OnCharacterLevelUpAmount = new();
        [SerializeField]
        private UnityEvent<CharacterDefinition> m_OnCharacterStarUp = new();
        [SerializeField]
        private UnityEvent<CharacterDefinition> m_OnCharacterOwned = new();
        [SerializeField]
        private UnityEvent<int> m_OnCharacterOwnedAmount = new();

        public UnityEvent<CharacterDefinition> OnCharacterLevelUp => m_OnCharacterLevelUp;
        public UnityEvent<CharacterDefinition> OnCharacterStarUp => m_OnCharacterStarUp;
        public UnityEvent<CharacterDefinition> OnCharacterOwned => m_OnCharacterOwned;
        public UnityEvent<int> OnCharacterOwnedAmount => m_OnCharacterOwnedAmount;
        public UnityEvent<int> OnCharacterLevelUpAmount => m_OnCharacterLevelUpAmount;
        public UnityEvent<CharacterDefinition> OnInitialized => m_OnInitialized;
        public UnityEvent<CharacterDefinition> OnCharacterUsed => m_OnCharacterUsed;
        public UnityEvent<CharacterDefinition> OnSelectedCharacter => m_OnSelectedCharacter;

        public CharacterDefinition DefaultCharacter => m_DefaultCharacter;
        public List<CharacterUnit> CharacterUnits => m_CharacterUnits;
        public CharacterDefinition UsedCharacter => m_UsedCharacter;
        public CharacterDefinition SelectedCharacter => m_SelectedCharacter;

        private void Start()
        {
            foreach (CharacterUnit unit in m_CharacterUnits)
            {
                //unit.Init();
                unit.Definition.InitAsOwner();
            }
        }
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
                m_SelectedCharacter = m_UsedCharacter;
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
                //unit.Definition.InitAsOwner();
            }
        }
        public void SetOwned(CharacterDefinition defi, bool set)
        {
            GetCharacterUnitInternal(defi).SetOwned(set);
            if (set)
            {
                OnCharacterOwnedInvoke(defi);
            }
        }

        private void OnCharacterOwnedInvoke(CharacterDefinition defi)
        {
            m_OnCharacterOwned?.Invoke(defi);
            int ownedAmount = 0;
            foreach (CharacterUnit unit in m_CharacterUnits)
            {
                if (unit.Owned)
                {
                    ownedAmount++;
                }
            }
            m_OnCharacterOwnedAmount?.Invoke(ownedAmount);
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
            UnityService.Instance.SaveData("usedcharacter", m_UsedCharacter.Id);
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
            CanvasManager.Instance.SetCharacterSelected(m_SelectedCharacter);
        }
        public void AddExp(CharacterDefinition defi, int exp)
        {
            GetCharacterUnitInternal(defi).AddExp(exp);
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
