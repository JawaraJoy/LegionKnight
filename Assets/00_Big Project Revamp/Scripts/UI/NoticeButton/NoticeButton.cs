using UnityEngine;
using UnityEngine.UI;
using LegionKnight;

namespace Rush
{
    public abstract class NoticeButton : UIView
    {
        [SerializeField]
        private Button m_CheckButton;
        private void Awake()
        {
            m_CheckButton.onClick.AddListener(NoticeCheckInternal);
        }
        protected abstract bool HasNewContent();

        public void NoticeCheck()
        {
            NoticeCheckInternal();
        }
        private void NoticeCheckInternal()
        {
            if (HasNewContent())
            {
                ShowInternal();
            }
            else
            {
                HideInternal();
            }
        }
    }
}
