using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace LegionKnight
{
    public partial class ItemView : UIView
    {
        [SerializeField]
        protected Image m_Icon;
        [SerializeField]
        protected TextMeshProUGUI m_Amount;
        [SerializeField]
        private UnityEvent<object> m_OnDefinitionSet = new();

        protected int m_AmountValue;

        protected object m_Definition;
        public object Definition => m_Definition;
        public void Init(object defi)
        {
            InitInternal(defi);
        }
        protected virtual void InitInternal(object defi)
        {
            m_Definition = defi;
            OnDefinitionSetInvoke(defi);
        }
        protected virtual void OnDefinitionSetInvoke(object defi)
        {
            m_OnDefinitionSet?.Invoke(defi);
        }
        protected void SetAmountInternal(int amount)
        {
            m_AmountValue = amount;
            if (m_Amount != null)
            {
                m_Amount.text = amount.ToString();
                Debug.Log($"Set loot amount: {amount}");
            }
        }
        public void SetAmount(int amount)
        {
            SetAmountInternal(amount);
        }
        public void AddAmount(int amount)
        {
            SetAmountInternal(m_AmountValue + amount);
        }
    }
}
