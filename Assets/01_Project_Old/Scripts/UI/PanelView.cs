using System.Collections.Generic;
using UnityEngine;

namespace LegionKnight
{
    public partial class PanelView : UIView
    {

        [SerializeField]
        protected List<UIView> m_Bindings = new();

        protected T GetBindingInternal<T>() where T : UIView
        {
            T match = (T)m_Bindings.Find(x => x.GetType() == typeof(T)) ?? null;
            return match;
        }
        public T GetBinding<T>() where T : UIView
        {
            return GetBindingInternal<T>();
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
        private bool HasBindingInternal(string uniqueId)
        {
            return m_Bindings.Contains(GetBinding(uniqueId));
        }
        public bool HasBinding<T>(out T binded) where T : UIView
        {
            binded = GetBinding<T>();
            return binded != null;
        }
        private bool HasBindingInternal<T>() where T : UIView
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
            if (HasBindingInternal(uniqueId))
            {
                GetBinding(uniqueId).Show();
            }
        }
        protected virtual void HideBindingInternal(string uniqueId)
        {
            if (HasBindingInternal(uniqueId))
            {
                GetBinding(uniqueId).Hide();
            }
        }
    }
}
