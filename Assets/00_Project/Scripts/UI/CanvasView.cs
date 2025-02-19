using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public partial class CanvasView : UIView
    {
        [SerializeField]
        private List<PanelView> m_Panels = new();

        private bool m_IsBusy;

        [SerializeField]
        private UnityEvent<bool> m_OnIsBusyUpdate = new();

        private T GetPanel<T>() where T : PanelView
        {
            T match = (T)m_Panels.Find(x => x.GetType() == typeof(T)) ?? null;
            return match;
        }
        private T GetPanel<T>(string uniqueId) where T : PanelView
        {
            T match = (T)m_Panels.Find(x => x.UniqueId == uniqueId) ?? null;
            return match;
        }
        private PanelView GetPanel(string uniqueId)
        {
            PanelView match = m_Panels.Find(x => x.UniqueId == uniqueId);
            if (match == null)
            {
                match = null;
            }
            return match;
        }
        private bool HasPanel(string uniqueId)
        {
            return m_Panels.Contains(GetPanel(uniqueId));
        }
        private bool HasPanel<T>() where T : PanelView
        {
            return m_Panels.Contains(GetPanel<T>());
        }
        private bool HasPanel<T>(string uniqueId) where T : PanelView
        {
            return m_Panels.Contains(GetPanel<T>(uniqueId));
        }
        public virtual void ShowPanel(string uniqueId)
        {
            ShowPanelInternal(uniqueId);
        }
        public virtual void HidePanel(string uniqueId)
        {
            HidePanelInternal(uniqueId);
        }
        protected virtual void ShowPanelInternal(string uniqueId)
        {
            if (HasPanel(uniqueId))
            {
                GetPanel(uniqueId).Show();
                HandleIsBusy();
            }
        }
        protected virtual void HidePanelInternal(string uniqueId)
        {
            if (HasPanel(uniqueId))
            {
                GetPanel(uniqueId).Hide();
                HandleIsBusy();
            }
        }

        private void HandleIsBusy()
        {
            m_IsBusy = m_Panels.Any(x => x.IsBusyPanel && m_IsBusy);
            OnIsBusyUpdateInvoke(m_IsBusy);
        }
        private void OnIsBusyUpdateInvoke(bool busy)
        {
            m_OnIsBusyUpdate?.Invoke(busy);
        }
    }
}
