using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public partial class SlideShowManager : SlideShowHandler
    {
        
    }

    public partial class GameManager
    {
        [SerializeField]
        private SlideShowManager m_SlideShowManager;
        public SlideShowHandler SlideShowHandler => m_SlideShowManager;
        public SlideShowDefinition CurrentSlideShow => m_SlideShowManager.CurrentSlideShow;

        public void StartSlideShow(SlideShowDefinition defi)
        {
            m_SlideShowManager.StartSlideShow(defi);
        }
        public void StartSlideShow()
        {
            m_SlideShowManager.StartSlideShow();
        }
        public void ShowNextSlide()
        {
            m_SlideShowManager.ShowNextSlide();
        }
        public void ShowPreviousSlide()
        {
            m_SlideShowManager.ShowPreviousSlide();
        }
        public void SetSlideShow(SlideShowDefinition defi)
        {
            m_SlideShowManager.SetSlideShow(defi);
        }
        public void AddOnSlideChanged(UnityAction<SlideShowHandler> action)
        {
            m_SlideShowManager.AddOnSlideChanged(action);

        }
        public void RemoveOnSlideChanged(UnityAction<SlideShowHandler> action)
        {
            m_SlideShowManager.RemoveOnSlideChanged(action);
        }
    }
}
