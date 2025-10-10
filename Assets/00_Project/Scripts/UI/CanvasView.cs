using System.Collections.Generic;
using System.Linq;
using TMPro;
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
        private TextMeshProUGUI m_GameVersionText;
        [SerializeField]
        private UnityEvent<bool> m_OnIsBusyUpdate = new();
        [SerializeField]
        private UnityEvent<PanelView> m_OnPanelShow = new();
        [SerializeField]
        private UnityEvent<PanelView> m_OnPanelHide = new();   

        private void Start()
        {
            // set aplication bundle version code

            m_GameVersionText.text = $"Ver.{Application.version}";
        }

        protected T GetPanelInternal<T>() where T : PanelView
        {
            T match = (T)m_Panels.Find(x => x.GetType() == typeof(T)) ?? null;
            return match;
        }
        protected T GetPanelInternal<T>(string uniqueId) where T : PanelView
        {
            T match = (T)m_Panels.Find(x => x.UniqueId == uniqueId) ?? null;
            return match;
        }
        protected PanelView GetPanelInternal(string uniqueId)
        {
            PanelView match = m_Panels.Find(x => x.UniqueId == uniqueId);
            if (match == null)
            {
                match = null;
            }
            return match;
        }
        public PanelView GetPanel(string uniqueId)
        {
            return GetPanelInternal(uniqueId);
        }
        public T GetPanel<T>() where T : PanelView
        {
            return GetPanelInternal<T>();
        }
        private bool HasPanel(string uniqueId)
        {
            return m_Panels.Contains(GetPanelInternal(uniqueId));
        }
        private bool HasPanel<T>() where T : PanelView
        {
            return m_Panels.Contains(GetPanelInternal<T>());
        }
        private bool HasPanel<T>(string uniqueId) where T : PanelView
        {
            return m_Panels.Contains(GetPanelInternal<T>(uniqueId));
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
                GetPanelInternal(uniqueId).Show();
                HandleIsBusy();
                m_OnPanelShow.Invoke(GetPanelInternal(uniqueId));
            }
        }
        protected virtual void HidePanelInternal(string uniqueId)
        {
            if (HasPanel(uniqueId))
            {
                GetPanelInternal(uniqueId).Hide();
                HandleIsBusy();
                m_OnPanelHide.Invoke(GetPanelInternal(uniqueId));
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
