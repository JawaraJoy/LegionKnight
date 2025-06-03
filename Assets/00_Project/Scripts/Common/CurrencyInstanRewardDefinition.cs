using UnityEngine;

namespace LegionKnight
{
    [CreateAssetMenu(fileName = "New CurrencyInstanReward", menuName = "Legion Knight/CurrencyInstanReward", order = 1)]
    public partial class CurrencyInstanRewardDefinition : ScriptableObject
    {
        [SerializeField]
        private CurrencyDefinition m_CurrencyDefinition;
        [SerializeField]
        private int m_Amount;

        public CurrencyDefinition CurrencyDefinition => m_CurrencyDefinition;
        public int Amount => m_Amount;

        public void AddToPlayerCurrency()
        {
            Player.Instance.AddCurrencyAmount(m_CurrencyDefinition, m_Amount);
        }
    }
}
