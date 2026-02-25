using Rush;
using UnityEngine;

namespace LegionKnight
{
    [System.Serializable]
    public class GachaCurrencyCost
    {
        [SerializeField] private ItemConfig m_ItemConfig;
        [SerializeField] private int m_Amount;

        public ItemConfig ItemConfig => m_ItemConfig;
        public int Amount => m_Amount;

        public GachaCurrencyCost(ItemConfig definition, int amount)
        {
            m_ItemConfig = definition;
            m_Amount = amount;
        }
    }
}
