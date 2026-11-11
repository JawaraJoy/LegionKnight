using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using LegionKnight;

namespace Rush
{
    public class QuestCatalogView : UIView, IUpdater
    {
        [SerializeField] private QuestTaskItemPool m_TaskItemPool;
        [SerializeField] private ScrollRect m_Scroll;

        [SerializeField] private GameObject m_CountdownGroup;
        [SerializeField] private TextMeshProUGUI m_CountdownText;
        [SerializeField] private TextMeshProUGUI m_ResetCycleLabel; // e.g. "Resets Daily" / "Resets Weekly"

        private readonly List<QuestTaskItemUI> m_ActiveItems = new();
        private QuestCatalogConfig m_Catalog;

        // ── IUpdater ──────────────────────────────────────────────────────────

        public bool IsActive => IsShown && m_Catalog != null;

        public void Tick()
        {
            var state = RushPlayer.Instance.QuestManager.GetCatalogState(m_Catalog);
            RefreshCountdownInternal(state);
        }

        // ── Lifecycle ─────────────────────────────────────────────────────────

        protected override void ShowInternal()
        {
            base.ShowInternal();
            UpdateBank.Instance.RegisterUpdateTick(gameObject, this);
            if (m_Catalog == null) return;
            RefreshCountdownInternal(RushPlayer.Instance.QuestManager.GetCatalogState(m_Catalog));
            RefreshAllTasksInternal();
        }

        protected override void HideInternal()
        {
            UpdateBank.Instance.UnregisterUpdateTick(gameObject);
            base.HideInternal();
        }

        // ── Public ────────────────────────────────────────────────────────────

        public void Populate(QuestCatalogConfig catalog)
        {
            m_Catalog = catalog;
            RepopulateInternal();
            RefreshResetLabelInternal();
            if (m_Catalog != null)
                RefreshCountdownInternal(RushPlayer.Instance.QuestManager.GetCatalogState(m_Catalog));
            if (m_Scroll != null && m_Scroll.content != null)
            {
                m_Scroll.StopMovement();
                m_Scroll.content.anchoredPosition = Vector2.zero;
            }
        }

        public void RefreshTask(QuestTaskConfig task)
        {
            foreach (var item in m_ActiveItems)
            {
                if (item.Task != task) continue;
                item.Refresh(RushPlayer.Instance.QuestManager.GetTaskState(task));
            }
        }

        public void RefreshAllTasks() => RefreshAllTasksInternal();

        private void RefreshAllTasksInternal()
        {
            foreach (var item in m_ActiveItems)
                item.Refresh(RushPlayer.Instance.QuestManager.GetTaskState(item.Task));
        }

        // ── Private ───────────────────────────────────────────────────────────

        private void RepopulateInternal()
        {
            if (m_TaskItemPool == null || m_Catalog == null) return;
            m_TaskItemPool.ReturnAll();
            m_ActiveItems.Clear();

            var states = RushPlayer.Instance.QuestManager.GetTaskStates(m_Catalog);
            foreach (var state in states)
            {
                var item = m_TaskItemPool.Rent();
                item.transform.SetAsLastSibling();
                item.Setup(state);
                m_ActiveItems.Add(item);
            }
        }

        private void RefreshResetLabelInternal()
        {
            if (m_ResetCycleLabel == null || m_Catalog == null) return;
            m_ResetCycleLabel.text = m_Catalog.ResetCycle switch
            {
                QuestResetCycle.Daily => "Resets Daily",
                QuestResetCycle.Weekly => $"Resets {m_Catalog.WeeklyResetDay}",
                _ => string.Empty
            };
        }

        private void RefreshCountdownInternal(QuestCatalogState state)
        {
            if (m_CountdownGroup != null)
                m_CountdownGroup.SetActive(state.SecondsUntilReset > 0);

            if (m_CountdownText != null && state.SecondsUntilReset > 0)
                m_CountdownText.text = FormatCountdownInternal(state.SecondsUntilReset);
        }

        private static string FormatCountdownInternal(double totalSeconds)
        {
            var span = System.TimeSpan.FromSeconds(totalSeconds);
            if (span.Days > 0) return $"{span.Days}d {span.Hours:D2}:{span.Minutes:D2}:{span.Seconds:D2}";
            return span.Hours > 0
                ? $"{span.Hours:D2}:{span.Minutes:D2}:{span.Seconds:D2}"
                : $"{span.Minutes:D2}:{span.Seconds:D2}";
        }
    }
}