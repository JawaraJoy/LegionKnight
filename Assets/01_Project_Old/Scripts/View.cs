using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public interface IView
    {
        void Show();
        void Hide();
    }
    // Base class for all views. Mainly used to have a unique id for each view, so that we can easily find them in the hierarchy and manage them.
    public partial class View : MonoBehaviour, IView
    {

        [SerializeField]
        protected GameObject m_Content;
        [SerializeField]
        private UnityEvent m_OnShow = new();
        [SerializeField]
        private UnityEvent m_OnHide = new();
        protected bool IsShowInternal => m_Content.activeSelf;
        public GameObject Content => m_Content;
        public bool IsShown => IsShowInternal;
        public UnityEvent OnShow => m_OnShow;
        public UnityEvent OnHide => m_OnHide;
        public virtual void Show()
        {
            ShowInternal();
        }
        public virtual void Hide()
        {
            HideInternal();
        }

        public virtual void SetContentActive(bool set)
        {
            if (set)
            {
                ShowInternal();
            }
            else
            {
                HideInternal();
            }
        }

        [ContextMenu("Show")]
        protected virtual void ShowInternal()
        {
            if(!m_Content) return;

            if (MasterPanelUtility.IsShow) return;
            m_Content.SetActive(true);
            OnShowInvoke();
        }
        [ContextMenu("Hide")]
        protected virtual void HideInternal()
        {
            m_Content.SetActive(false);
            OnHideInvoke();
        }

        protected virtual void OnShowInvoke()
        {
            Debug.Log(gameObject.name);
            m_OnShow?.Invoke();
        }
        protected virtual void OnHideInvoke()
        {
            m_OnHide?.Invoke();
        }
    }
}
