using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public class StarView : UIView
    {
        [SerializeField]
        private GameObject m_StarView;
        [SerializeField]
        private UnityEvent m_OnShowStar = new UnityEvent();
        [SerializeField]
        private UnityEvent m_OnHideStar = new UnityEvent();
        public void ShowStar()
        {
            if (m_StarView != null)
            {
                m_StarView.SetActive(true);
                m_OnShowStar?.Invoke();
            }
        }
        public void HideStar()
        {
            if (m_StarView != null)
            {
                m_StarView.SetActive(false);
                m_OnHideStar?.Invoke();
            }
        }
    }
}
