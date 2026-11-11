using Rush;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace LegionKnight
{
    public class LootItemView : UIView
    {
        [SerializeField]
        private LootField m_LootField;
        [SerializeField]
        private Image m_Icon;
        [SerializeField]
        private Image m_Frame;
        [SerializeField]
        private TextMeshProUGUI m_ItemNameText;
        [SerializeField]
        private TextMeshProUGUI m_ItemAmountText;

        [Header("Animation")]
        [SerializeField, Min(1)]
        private int m_AnimateSteps = 6;
        [SerializeField, Min(0.01f)]
        private float m_AnimateDuration = 0.15f;

        [SerializeField]
        private UnityEvent<int> m_OnAmountChanged = new();
        [SerializeField]
        private UnityEvent m_OnAmountCountChanged = new();

        public LootField LootField => m_LootField;

        private Coroutine m_AmountRoutine;
        private int m_CurrentDisplayedAmount;

        public void Init(LootField lootField)
        {
            InitInternal(lootField);
        }

        protected virtual void InitInternal(LootField lootField)
        {
            if (lootField == null || lootField.ItemLoot == null)
            {
                return;
            }

            m_LootField = lootField;

            CollectibleConfig itemLoot = lootField.ItemLoot;
            int amount = lootField.Amount;

            SetNameInternal(itemLoot.BaseInfo.Name);
            SetIconInternal(itemLoot);
            SetFrameInternal(itemLoot);
            SetAmountImmediateInternal(amount);
        }

        public void Bind(LootField lootField)
        {
            BindInternal(lootField);
        }

        protected virtual void BindInternal(LootField lootField)
        {
            if (lootField == null || lootField.ItemLoot == null)
            {
                return;
            }

            m_LootField = lootField;

            SetNameInternal(lootField.ItemLoot.BaseInfo.Name);
            SetIconInternal(lootField.ItemLoot);
            SetFrameInternal(lootField.ItemLoot);
        }

        public void SetAmount(int amount)
        {
            SetAmountInternal(amount);
        }

        protected virtual void SetAmountInternal(int amount)
        {
            SetAmountImmediateInternal(amount);
        }

        public void SetAmountImmediate(int amount)
        {
            SetAmountImmediateInternal(amount);
        }

        protected virtual void SetAmountImmediateInternal(int amount)
        {
            if (m_AmountRoutine != null)
            {
                RushGameManager.Instance.StopCoroutine(m_AmountRoutine);
                m_AmountRoutine = null;
            }

            m_CurrentDisplayedAmount = amount;

            if (m_ItemAmountText != null)
            {
                m_ItemAmountText.text = amount.ToString();
            }

            m_OnAmountChanged?.Invoke(amount);
        }

        public void SetAmountAnimated(int amount)
        {
            SetAmountAnimatedInternal(amount);
        }

        protected virtual void SetAmountAnimatedInternal(int amount)
        {
            if (!gameObject.activeInHierarchy)
            {
                SetAmountImmediateInternal(amount);
                return;
            }

            if (m_AmountRoutine != null)
            {
                RushGameManager.Instance.StopCoroutine(m_AmountRoutine);
            }

            m_AmountRoutine = RushGameManager.Instance.StartCoroutine(AnimatingAmount(amount));
        }

        private IEnumerator AnimatingAmount(int targetAmount)
        {
            int startAmount = m_CurrentDisplayedAmount;
            int steps = Mathf.Max(1, m_AnimateSteps);
            float duration = Mathf.Max(0.01f, m_AnimateDuration);
            float waitPerStep = duration / steps;

            if (startAmount == targetAmount)
            {
                SetAmountImmediateInternal(targetAmount);
                m_AmountRoutine = null;
                yield break;
            }

            for (int step = 1; step <= steps; step++)
            {
                int amount = Mathf.RoundToInt(Mathf.Lerp(startAmount, targetAmount, (float)step / steps));
                m_CurrentDisplayedAmount = amount;

                if (m_ItemAmountText != null)
                {
                    m_ItemAmountText.text = amount.ToString();
                }

                m_OnAmountChanged?.Invoke(amount);

                if (step % 2 == 0)
                {
                    m_OnAmountCountChanged?.Invoke();
                }

                if (step < steps)
                {
                    yield return new WaitForSeconds(waitPerStep);
                }
            }

            SetAmountImmediateInternal(targetAmount);
            m_AmountRoutine = null;
        }

        public void AddAmountWithCountDown(int addCount)
        {
            AddAmountWithCountDownInternal(addCount);
        }

        protected virtual void AddAmountWithCountDownInternal(int addCount)
        {
            int target = m_CurrentDisplayedAmount + addCount;
            SetAmountAnimatedInternal(target);
        }

        private void SetNameInternal(string itemName)
        {
            if (m_ItemNameText != null)
            {
                m_ItemNameText.text = itemName;
            }
        }

        private void SetIconInternal(CollectibleConfig collectibleConfig)
        {
            if (m_Icon == null || collectibleConfig == null)
            {
                return;
            }

            m_Icon.sprite = collectibleConfig.CollectibleField.Icon;
        }

        private void SetFrameInternal(CollectibleConfig collectibleConfig)
        {
            if (m_Frame == null || collectibleConfig == null)
            {
                return;
            }

            m_Frame.color = collectibleConfig.CollectibleField.RarityConfig.Color;
        }
    }
}