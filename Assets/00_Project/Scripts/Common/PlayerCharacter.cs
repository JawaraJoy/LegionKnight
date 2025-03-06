using UnityEngine;

namespace LegionKnight
{
    public partial class PlayerCharacter : Character
    {
        
    }

    public partial class Player
    {
        [SerializeField]
        private PlayerCharacter m_Character;

        public void SetCharacterDefinition(CharacterDefinition definition)
        {
            m_Character.SetCharacterDefinition(definition);
        }

        public int Attack => m_Character.CharacterDefinition.Attack;
    }
}
