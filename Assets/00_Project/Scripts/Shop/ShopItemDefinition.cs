using UnityEngine;

namespace LegionKnight
{
    public partial class ShopItemDefinition : ScriptableObject
    {
        [SerializeField]
        private CurrencyDefinition m_CurrencyDefinition;
        [SerializeField]
        private int m_Price;
        [SerializeField]
        private Object m_ItemToSell;
        [SerializeField]
        private int m_Amount;
        [SerializeField]
        private Sprite m_Icon;
        [SerializeField]
        private int m_ShopingPointReward;
    }
}
