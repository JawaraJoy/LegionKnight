using System.Collections.Generic;
using UnityEngine;
using LegionKnight;

namespace Rush
{
    public class AchievementCatalogView : UIView
    {
        [SerializeField] private AchievementTaskItemPool m_TaskItemPool;

        private readonly List<AchievementTaskItemUI> m_ActiveItems = new();
        private AchievementCatalogConfig m_Catalog;

        public void Populate(AchievementCatalogConfig catalog)
        {
            m_Catalog = catalog;
            RepopulateInternal();
        }

        public void RefreshTask(AchievementTaskConfig task)
        {
            if (m_Catalog?.Tasks == null) return;
            for (int i = 0; i < m_Catalog.Tasks.Length; i++)
            {
                if (m_Catalog.Tasks[i] != task) continue;
                if (i >= m_ActiveItems.Count) break;
                var state = RushPlayer.Instance.AchievementManager.GetTaskState(task);
                m_ActiveItems[i].Refresh(state);
                break;
            }
        }

        protected override void HideInternal()
        {
            //m_TaskItemPool?.ReturnAll();
            m_ActiveItems.Clear();
            base.HideInternal();
        }

        private void RepopulateInternal()
        {
            if (m_TaskItemPool == null || m_Catalog == null) return;
            m_TaskItemPool.ReturnAll();
            m_ActiveItems.Clear();

            var states = RushPlayer.Instance.AchievementManager.GetTaskStates(m_Catalog);
            foreach (var state in states)
            {
                var item = m_TaskItemPool.Rent();
                item.Setup(state);
                m_ActiveItems.Add(item);
            }
        }
    }
}