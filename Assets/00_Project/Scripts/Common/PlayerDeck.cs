using System.Collections.Generic;
using UnityEngine;

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
