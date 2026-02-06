using UnityEngine;

namespace LegionKnight
{
    [System.Serializable]
    public class GachaCurrencyCost
    {
        [SerializeField] private CurrencyDefinition m_Definition;
        [SerializeField] private int m_Amount;

        public CurrencyDefinition Definition => m_Definition;
        public int Amount => m_Amount;

        public GachaCurrencyCost(CurrencyDefinition definition, int amount)
        {
            m_Definition = definition;
            m_Amount = amount;
        }
    }
}
