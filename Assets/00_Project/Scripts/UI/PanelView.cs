using System.Collections.Generic;
using UnityEngine;

namespace LegionKnight
{
    public static partial class PanelId
    {

    }
    public partial class PanelView : UIView
    {
        [SerializeField]
        private bool m_IsBusyPanel;
        public bool IsBusyPanel => m_IsBusyPanel;

        [SerializeField]
        private List<UIView> m_Bindings = new();

        protected T GetBinding<T>() where T : UIView
        {
            T match = (T)m_Bindings.Find(x => x.GetType() == typeof(T)) ?? null;
            return match;
        }
        protected T GetBinding<T>(string uniqueId) where T : UIView
        {
            T match = (T)m_Bindings.Find(x => x.UniqueId == uniqueId) ?? null;
            return match;
        }
        protected UIView GetBinding(string uniqueId)
        {
            UIView match = m_Bindings.Find(x => x.UniqueId == uniqueId);
            if (match == null)
            {
                match = null;
            }
            return match;
        }
        private bool HasBinding(string uniqueId)
        {
            return m_Bindings.Contains(GetBinding(uniqueId));
        }
        private bool HasBinding<T>() where T : UIView
        {
            return m_Bindings.Contains(GetBinding<T>());
        }
        private bool HasBinding<T>(string uniqueId) where T : UIView
        {
            return m_Bindings.Contains(GetBinding<T>(uniqueId));
        }
        public virtual void ShowBinding(string uniqueId)
        {
            ShowBindingInternal(uniqueId);
        }
        public virtual void HideBinding(string uniqueId)
        {
            HideBindingInternal(uniqueId);
        }
        protected virtual void ShowBindingInternal(string uniqueId)
        {
            if (HasBinding(uniqueId))
            {
                GetBinding(uniqueId).Show();
            }
        }
        protected virtual void HideBindingInternal(string uniqueId)
        {
            if (HasBinding(uniqueId))
            {
                GetBinding(uniqueId).Hide();
            }
        }
    }
}
