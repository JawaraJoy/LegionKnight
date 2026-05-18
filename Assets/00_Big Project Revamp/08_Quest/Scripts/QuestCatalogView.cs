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

        [Header("Reset Countdown")]
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
        }

        protected override void HideInternal()
        {
            UpdateBank.Instance.UnregisterUpdateTick(gameObject);
            //m_TaskItemPool?.ReturnAll();
            //m_ActiveItems.Clear();
            base.HideInternal();
        }

        // ── Public ────────────────────────────────────────────────────────────

        public void Populate(QuestCatalogConfig catalog)
        {
            m_Catalog = catalog;
            RepopulateInternal();
            RefreshResetLabelInternal();
        }

        public void RefreshTask(QuestTaskConfig task)
        {
            if (m_Catalog?.Tasks == null) return;
            for (int i = 0; i < m_Catalog.Tasks.Length; i++)
            {
                if (m_Catalog.Tasks[i] != task) continue;
                if (i >= m_ActiveItems.Count) break;
                var state = RushPlayer.Instance.QuestManager.GetTaskState(task);
                m_ActiveItems[i].Refresh(state);
                break;
            }
        }

        public void RefreshAllTasks()
        {
            if (m_Catalog?.Tasks == null) return;
            for (int i = 0; i < m_Catalog.Tasks.Length && i < m_ActiveItems.Count; i++)
            {
                var state = RushPlayer.Instance.QuestManager.GetTaskState(m_Catalog.Tasks[i]);
                m_ActiveItems[i].Refresh(state);
            }
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
                QuestResetCycle.Weekly => $"Resets Weekly ({m_Catalog.WeeklyResetDay})",
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
            return span.Hours > 0
                ? $"{span.Hours:D2}:{span.Minutes:D2}:{span.Seconds:D2}"
                : $"{span.Minutes:D2}:{span.Seconds:D2}";
        }
    }
}