using Rush;
using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    [System.Serializable]
    public partial class Currency 
    {
        [SerializeField]
        private ItemConfig m_ItemConfig;
        [SerializeField]
        private int m_Amount;
        [SerializeField]
        private UnityEvent<int> m_OnCurrencyAmountChanged = new();
        [SerializeField]
        private UnityEvent<Currency> m_OnCurrencyChanged = new();
        [SerializeField]
        private UnityEvent<int> m_OnCurrencyAmountGet = new();
        public ItemConfig ItemConfig => m_ItemConfig;
        public int Amount => m_Amount;

        public void Init()
        {
            OnCurrencyAmountChangedInvoke(m_Amount);
        }
        
        public void SetAmount(int set)
        {
            m_Amount = set;
            OnCurrencyAmountChangedInvoke(m_Amount);
            
        }
        public void AddAmount(int add)
        {
            m_Amount += add;
            OnCurrencyAmountChangedInvoke(m_Amount);
            OncurrencyAmountGetInvoke(add);
        }
        public void RemoveAmount(int remove)
        {
            m_Amount -= remove;
            OnCurrencyAmountChangedInvoke(m_Amount);
        }
        public Currency(ItemConfig itemConfig, int amount)
        {
            m_ItemConfig = itemConfig;
            m_Amount = amount;
        }
        private void OnCurrencyAmountChangedInvoke(int amount)
        {
            m_OnCurrencyAmountChanged?.Invoke(amount);
            m_OnCurrencyChanged?.Invoke(this);
            
        }

        private void OncurrencyAmountGetInvoke(int amount)
        {
            m_OnCurrencyAmountGet?.Invoke(amount);
        }
    }
}
