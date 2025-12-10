using System.Collections;
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
        [SerializeField]
        private UnityEvent<int> m_OnAmountChanged = new();
        protected int m_AmountValue;
        protected object m_Definition;
        public object Definition => m_Definition;
        public int Amount => m_AmountValue;
        public void Init(object defi)
        {
            InitInternal(defi);
        }
        protected virtual void InitInternal(object defi)
        {
            m_Definition = defi;
            OnDefinitionSetInvoke(defi);
        }

        public void AddAmountWithCountDown(int addCount)
        {
            StartCoroutine(AddCountDown(addCount));
        }

        private IEnumerator AddCountDown(int addCount)
        {
            int start = m_AmountValue;
            int target = m_AmountValue + addCount;

            for (int i = start; i < target; i++)
            {
                m_AmountValue = i + 1;

                m_OnAmountChanged?.Invoke(m_AmountValue);

                if (m_Amount != null)
                {
                    m_Amount.text = m_AmountValue.ToString();
                }
                Debug.Log($"Counting up loot amount: {m_AmountValue}");
                yield return new WaitForSeconds(0.05f);
            }
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
                m_Amount.text = m_AmountValue.ToString();
                Debug.Log($"Set loot amount: {m_AmountValue}");
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
