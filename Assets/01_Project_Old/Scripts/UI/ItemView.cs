using Rush;
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
        private UnityEvent<CollectibleConfig> m_OnCollectibleConfigSet = new();
        [SerializeField]
        private UnityEvent<int> m_OnAmountChanged = new();
        [SerializeField]
        private UnityEvent m_OnAmountCountChanged = new();
        protected int m_AmountValue;
        protected CollectibleConfig m_CollectibleConfig;
        public CollectibleConfig CollectibleConfig => m_CollectibleConfig;
        public int Amount => m_AmountValue;
        public void Init(CollectibleConfig collectibleConfig)
        {
            InitInternal(collectibleConfig);
        }
        protected virtual void InitInternal(CollectibleConfig collectibleConfig)
        {
            m_CollectibleConfig = collectibleConfig;
            OnDefinitionSetInvoke(collectibleConfig);
        }

        public void AddAmountWithCountDown(int addCount)
        {
            StartCoroutine(AddCountDown(addCount));
        }
        int m_AmountTriggerCount = 0;
        private IEnumerator AddCountDown(int addCount)
        {
            int start = m_AmountValue;
            int target = m_AmountValue + addCount;
            for (int i = start; i < target; i++)
            {
                m_AmountValue = i + 1;
                m_OnAmountChanged?.Invoke(m_AmountValue);

                m_AmountTriggerCount++;
                if (m_AmountTriggerCount >= 5)
                {
                    m_OnAmountCountChanged?.Invoke();
                    m_AmountTriggerCount = 0;
                }
                if (m_Amount != null)
                {
                    m_Amount.text = m_AmountValue.ToString();
                }
                Debug.Log($"Counting up loot amount: {m_AmountValue}");
                yield return new WaitForSeconds(0.05f);
            }
        }
        protected virtual void OnDefinitionSetInvoke(CollectibleConfig collectibleConfig)
        {
            m_OnCollectibleConfigSet?.Invoke(collectibleConfig);
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
