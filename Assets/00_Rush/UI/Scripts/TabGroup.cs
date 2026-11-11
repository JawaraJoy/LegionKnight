using System;
using UnityEngine;
using LegionKnight;

namespace Rush
{
    /// <summary>
    /// Manages a group of tabs. Attach to the parent GameObject that holds all tab buttons and views.
    /// Drag tabs into the array in the Inspector; order determines which is shown first.
    /// </summary>
    public class TabGroup : UIView
    {
        [SerializeField] private TabEntry[] m_Tabs;
        [SerializeField] private int m_DefaultTabIndex = 0;

        private int m_ActiveIndex = -1;

        protected override void ShowInternal()
        {
            base.ShowInternal();
            SelectTab(m_DefaultTabIndex);
        }

        private void SelectTab(int index)
        {
            if (index < 0 || index >= m_Tabs.Length) return;
            if (index == m_ActiveIndex) return;

            m_ActiveIndex = index;

            for (int i = 0; i < m_Tabs.Length; i++)
            {
                if (m_Tabs[i].IsEnabled)
                    m_Tabs[i].SetState(i == m_ActiveIndex);
            }
        }

        // Called from TabEntry buttons via UnityEvent / onClick
        public void OnTabClicked(int index) => SelectTab(index);
    }
}