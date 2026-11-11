using System;
using System.Collections.Generic;
using LegionKnight;
using UnityEngine;
using UnityEngine.UI;

namespace Rush
{
    public abstract class RewardPopupPanel : PanelView
    {
        [Serializable]
        private sealed class LayoutField
        {
            [SerializeField] private RectTransform m_Dialog;
            [SerializeField] private RectTransform m_Body;
            [SerializeField] private RewardPopupItem m_Single;
            [SerializeField] private RewardPopupItem m_Template;
            [SerializeField] private ScrollRect m_Scroll;
            [SerializeField] private GridLayoutGroup m_Grid;
            [SerializeField] private GameObject m_Empty;

            public RectTransform Dialog => m_Dialog;
            public RectTransform Body => m_Body;
            public RewardPopupItem Single => m_Single;
            public RewardPopupItem Template => m_Template;
            public ScrollRect Scroll => m_Scroll;
            public GridLayoutGroup Grid => m_Grid;
            public GameObject Empty => m_Empty;
        }

        private const int c_MaxVisibleRows = 2;
        [SerializeField] private LayoutField m_Layout = new();
        private readonly List<LootField> m_Rewards = new();
        private readonly List<RewardPopupItem> m_GridItems = new();

        public void ShowLoot(LootField[] loots)
        {
            ShowLootInternal(loots);
        }

        private void ShowLootInternal(LootField[] loots)
        {
            m_Rewards.Clear();
            if (loots != null)
            {
                foreach (LootField loot in loots)
                {
                    if (loot?.ItemLoot != null) m_Rewards.Add(loot);
                }
            }

            if (m_Layout.Dialog == null)
            {
                ShowInternal();
                // Older canvas prefabs use an addressable LootMonitor.
                var monitor = GetBindingInternal<LootMonitor>();
                if (monitor != null)
                {
                    monitor.ClearAllLootViews();
                    monitor.AddLootsView(new List<LootField>(m_Rewards));
                }
                return;
            }

            if (!IsShowInternal) ShowInternal();

            int count = m_Rewards.Count;
            bool single = count == 1;
            int gridCount = count > 1 ? count : 0;
            m_Layout.Empty.SetActive(count == 0);
            m_Layout.Single.gameObject.SetActive(single);
            m_Layout.Scroll.gameObject.SetActive(gridCount != 0);
            if (single) m_Layout.Single.SetReward(m_Rewards[0]);
            BindGridInternal(gridCount);

            GridLayoutGroup grid = m_Layout.Grid;
            int rows = Mathf.CeilToInt((float)gridCount / grid.constraintCount);
            int visibleRows = Mathf.Clamp(rows, 1, c_MaxVisibleRows);
            float bodyHeight = single
                ? ((RectTransform)m_Layout.Single.transform).rect.height
                : grid.cellSize.y * visibleRows + grid.spacing.y * (visibleRows - 1);
            float verticalMargins = m_Layout.Body.offsetMin.y - m_Layout.Body.offsetMax.y;
            m_Layout.Dialog.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, bodyHeight + verticalMargins);

            ScrollRect scroll = m_Layout.Scroll;
            scroll.StopMovement();
            scroll.vertical = rows > c_MaxVisibleRows;
            if (gridCount > 0)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(m_Layout.Dialog);
                scroll.content.anchoredPosition = Vector2.zero;
                if (scroll.vertical)
                {
                    scroll.verticalScrollbar.SetValueWithoutNotify(1f);
                    scroll.verticalNormalizedPosition = 1f;
                }
            }
        }

        private void BindGridInternal(int count)
        {
            Transform container = m_Layout.Grid.transform;
            if (m_GridItems.Count == 0)
            {
                // The template is outside the grid. A Unity reload can leave untracked clones inside it.
                foreach (Transform oldCard in container)
                {
                    oldCard.gameObject.SetActive(false);
                    Destroy(oldCard.gameObject);
                }
            }

            for (int index = 0; index < count; index++)
            {
                if (index == m_GridItems.Count)
                    m_GridItems.Add(Instantiate(m_Layout.Template, container, false));

                RewardPopupItem cell = m_GridItems[index];
                cell.SetReward(m_Rewards[index]);
                cell.gameObject.SetActive(true);
            }

            for (int index = count; index < m_GridItems.Count; index++)
                m_GridItems[index].gameObject.SetActive(false);
        }
    }
}
