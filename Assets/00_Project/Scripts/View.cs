using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public interface IView
    {
        void Show();
        void Hide();
    }
    public partial class View : MonoBehaviour, IView
    {

        [SerializeField]
        protected GameObject m_Content;
        [SerializeField]
        private UnityEvent m_OnShow = new();
        [SerializeField]
        private UnityEvent m_OnHide = new();
        protected bool m_IsShow => m_Content.activeSelf;
        public GameObject Content => m_Content;
        public bool IsShow => m_IsShow;
        public virtual void Show()
        {
            ShowInternal();
        }
        public virtual void Hide()
        {
            HideInternal();
        }

        protected virtual void ShowInternal()
        {
            if (MasterPanelUtility.IsShow) return;
            m_Content.SetActive(true);
            OnShowInvoke();
        }
        protected virtual void HideInternal()
        {
            m_Content.SetActive(false);
            OnHideInvoke();
        }

        protected virtual void OnShowInvoke()
        {
            m_OnShow?.Invoke();
        }
        protected virtual void OnHideInvoke()
        {
            m_OnHide?.Invoke();
        }
    }
}
