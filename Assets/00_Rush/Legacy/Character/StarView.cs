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
        private UnityEngine.UI.Image m_Fill;
        private Rush.EvolutionStarGradient m_Gradient;

        public void SetTier(int tier)
        {
            if (m_StarView == null) return;
            if (tier == 0)
            {
                HideStar();
                return;
            }
            ShowStar();
            if (m_Fill == null) m_Fill = m_StarView.GetComponentInChildren<UnityEngine.UI.Image>(true);
            if (m_Fill == null) return;
            if (tier == 4 && m_Gradient == null)
                m_Gradient = m_Fill.gameObject.AddComponent<Rush.EvolutionStarGradient>();
            if (m_Gradient != null) m_Gradient.enabled = tier == 4;
            m_Fill.color = Rush.EvolutionStarGradient.FillColor(tier);
        }

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

