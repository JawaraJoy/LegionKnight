using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public partial class PlayerDeck : CharacterDeck
    {
        
    }
    public partial class Player
    {
        [SerializeField]
        private PlayerDeck m_PlayerDeck;
        public CharacterDefinition DefaultCharacter => m_PlayerDeck.DefaultCharacter;
        public int GetHeroLevel(CharacterDefinition defi) => m_PlayerDeck.GetLevel(defi);
        public int GetHeroExp(CharacterDefinition defi) => m_PlayerDeck.GetExp(defi);
        public int GetHeroStar(CharacterDefinition defi) => m_PlayerDeck.GetStar(defi);
        public int GetHeroCurrentMaxExp(CharacterDefinition defi) => m_PlayerDeck.GetCurrentMaxExp(defi);

        public UnityEvent<CharacterDefinition> OnHeroLevelUp => m_PlayerDeck.OnCharacterLevelUp;
        public UnityEvent<CharacterDefinition> OnHeroStarUp => m_PlayerDeck.OnCharacterStarUp;
        public CurrencyDefinition GetHeroShardCurrency(CharacterDefinition defi)
        {
            return m_PlayerDeck.GetShardCurrency(defi);
        }
        public void AddHeroExp(CharacterDefinition defi, int exp)
        {
            m_PlayerDeck.AddExp(defi, exp);
        }
        public void InitPlayerDeck()
        {
            m_PlayerDeck.Init();
        }

        public void SetOwned(CharacterDefinition defi, bool set)
        {
            m_PlayerDeck.SetOwned(defi, set);
        }
        public void SetSelectedCharacter(CharacterDefinition defi)
        {
            m_PlayerDeck.SetSelectedCharacter(defi);
        }
        public void SetUsedCharacter()
        {
            m_PlayerDeck.SetUsedCharacter();
        }
        public CharacterUnit GetCharacterUnit(CharacterDefinition definition)
        {
            return m_PlayerDeck.GetCharacterUnit(definition);
        }
        
        public void AddUniqueHeroPlatform()
        {
            GameManager.Instance.AddStandbyPlatform(new List<StandbyPlatformDefinition> { GetHeroStandbyPlatformInternal() });
        }
        private StandbyPlatformDefinition GetHeroStandbyPlatformInternal()
        {
            return m_PlayerDeck.GetHeroStandbyPlatform();
        }
        public StandbyPlatformDefinition GetHeroStandbyPlatform()
        {
            return m_PlayerDeck.GetHeroStandbyPlatform();
        }
        public List<CharacterUnit> CharacterUnits => m_PlayerDeck.CharacterUnits;
        public CharacterDefinition UsedCharacter => m_PlayerDeck.UsedCharacter;
    }
    public partial class PlayerAgent
    {
        public void InitPlayerDeck()
        {
            Player.Instance.InitPlayerDeck();
        }

        public void SetOwned(CharacterDefinition defi, bool set)
        {
            Player.Instance.SetOwned(defi, set);
        }
        public void SetSelectedCharacter(CharacterDefinition defi)
        {
            Player.Instance.SetSelectedCharacter(defi);
        }
        public void SetUsedCharacter()
        {
            Player.Instance.SetUsedCharacter();
        }
    }
}
