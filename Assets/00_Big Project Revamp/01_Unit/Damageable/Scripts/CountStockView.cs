using LegionKnight;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Rush
{
    public class CountStockView : UIView
    {
        [SerializeField]
        private int m_PrewarmCount = 5;
        [SerializeField]
        private CounterPieceView m_CountViewPrefab;
        [SerializeField]
        private Transform m_CountViewParent;
        [SerializeField]
        private float m_HideDelay = 0.5f;
        [SerializeField]
        private UnityEvent m_OnStartHiding;
        private List<CounterPieceView> m_Spawneds = new List<CounterPieceView>();
        public void HideDelay()
        {
            if (IsShowInternal)
            {
                RushGameManager.Instance.StartCoroutine(Hiding());
            }   
        }
        private IEnumerator Hiding()
        {
            m_OnStartHiding.Invoke();
            yield return new WaitForSeconds(m_HideDelay);
            HideInternal();
        }

        

        private void Start()
        {
            // prewarm here to avoid GC when showing the count stock view
            for (int i = 0; i < m_PrewarmCount; i++)
            {
                CounterPieceView view = Instantiate(m_CountViewPrefab, m_CountViewParent);
                m_Spawneds.Add(view);
                view.Hide();
            }
        }

        public virtual void SetCount(int count)
        {
            // Ensure enough pooled objects
            if (m_Spawneds.Count < count)
            {
                int spawnCount = count - m_Spawneds.Count;
                for (int i = 0; i < spawnCount; i++)
                {
                    CounterPieceView view = Instantiate(m_CountViewPrefab, m_CountViewParent);
                    m_Spawneds.Add(view);
                    view.Hide();
                }
            }

            int currentActive = GetActiveCount();

            if (currentActive > count)
            {
                // 🔥 Hide from the latest active (reverse order)
                for (int i = m_Spawneds.Count - 1; i >= 0 && currentActive > count; i--)
                {
                    if (m_Spawneds[i].IsShow)
                    {
                        m_Spawneds[i].HideDelay();
                        currentActive--;
                    }
                }
            }
            else if (currentActive < count)
            {
                // Show from earliest hidden
                for (int i = 0; i < m_Spawneds.Count && currentActive < count; i++)
                {
                    if (!m_Spawneds[i].IsShow)
                    {
                        m_Spawneds[i].Show();
                        currentActive++;
                    }
                }
            }

            if (count > 0)
                ShowInternal();
            else
                HideDelay();
        }
        private int GetActiveCount()
        {
            int count = 0;
            for (int i = 0; i < m_Spawneds.Count; i++)
            {
                if (m_Spawneds[i].IsShow)
                    count++;
            }
            return count;
        }
    }
}
