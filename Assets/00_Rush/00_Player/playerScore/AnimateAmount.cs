using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.Events;

namespace Rush
{
    public class AnimateAmount : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI m_ItemAmountText;
        [SerializeField]
        private int m_AnimateSteps = 6;
        [SerializeField]
        private float m_AnimateDuration = 0.5f;
        private Coroutine m_AmountRoutine;

        private int m_AnimateDurationRoutine;

        private UnityEvent m_OnAmountCountChanged;
        private UnityEvent<int> m_OnAmountChanged;

        private int m_CurrentDisplayedAmount;
        public void SetAmountAnimated(int amount)
        {
            SetAmountAnimatedInternal(amount);
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
    }
}
