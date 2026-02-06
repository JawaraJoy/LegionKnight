using UnityEngine;

namespace LegionKnight
{
    [System.Serializable]
    public class GachaReward
    {
        [SerializeField] private ScriptableObject m_Definition;
        [SerializeField] private int m_Amount = 1;
        [SerializeField] private float m_Weight = 1f;

        public ScriptableObject Definition => m_Definition;
        public int Amount => m_Amount;
        public float Weight => m_Weight;

        public void Apply()
        {
            if (m_Definition is CurrencyDefinition currency)
            {
                Player.Instance.AddCurrencyAmount(currency, m_Amount);
                return;
            }

            if (m_Definition is CharacterDefinition character)
            {
                if (Player.Instance.GetCharacterUnit(character).Owned)
                {
                    GameManager.Instance.AddStarConvertCount(character.DuplicateStarReward);
                }
                else
                {
                    Player.Instance.SetOwned(character, true);
                }
            }
        }
    }

    public partial class CharacterDefinition : ScriptableObject
    {
        [SerializeField] private int m_DuplicateStarReward = 3;

        public int DuplicateStarReward => m_DuplicateStarReward;
    }

}
