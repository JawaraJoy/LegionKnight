using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public partial class SlideShowHandler : MonoBehaviour
    {
        [SerializeField]
        private UnityEvent<SlideShowHandler> m_OnSlideShowChanged = new();

        private SlideShowDefinition m_CurrentSlideShow;

        public SlideShowDefinition CurrentSlideShow => m_CurrentSlideShow;

        private int m_CurrentSlideIndex = 0;
        private int TotalSlidesInternal => m_CurrentSlideShow.Slides.Length;
        public int CurrentSlideIndex => m_CurrentSlideIndex;
        public int TotalSlides => TotalSlidesInternal;

        public void AddOnSlideChanged(UnityAction<SlideShowHandler> action)
        {
            if (action == null)
            {
                Debug.LogError("Action is null.");
                return;
            }
            m_OnSlideShowChanged.AddListener(action);
        }
        public void RemoveOnSlideChanged(UnityAction<SlideShowHandler> action)
        {
            if (action == null)
            {
                Debug.LogError("Action is null.");
                return;
            }
            m_OnSlideShowChanged.RemoveListener(action);
        }

        public void ShowNextSlide()
        {
            if (m_CurrentSlideShow == null)
            {
                Debug.LogError("No SlideShow set.");
                return;
            }
            // Check if there are slides to show
            m_CurrentSlideIndex = (m_CurrentSlideIndex + 1) % TotalSlidesInternal;
            ShowCurrentSlide();
        }
        public void ShowPreviousSlide()
        {
            if (m_CurrentSlideShow == null)
            {
                Debug.LogError("No SlideShow set.");
                return;
            }
            // Check if there are slides to show
            m_CurrentSlideIndex = (m_CurrentSlideIndex - 1 + TotalSlidesInternal) % TotalSlidesInternal;
            ShowCurrentSlide();
        }

        private void ShowCurrentSlide()
        {
            if (m_CurrentSlideShow == null)
            {
                Debug.LogError("No SlideShow set.");
                return;
            }
            OnSlideShowChangeInvoke();
        }
        public void StartSlideShow(SlideShowDefinition defi)
        {
            m_CurrentSlideShow = defi;
            m_CurrentSlideIndex = 0;
            ShowCurrentSlide();
        }
        public void StartSlideShow()
        {
            m_CurrentSlideIndex = 0;
            ShowCurrentSlide();
        }
        public void SetSlideShow(SlideShowDefinition defi)
        {
            m_CurrentSlideShow = defi;
            m_CurrentSlideIndex = 0;
        }

        private void OnSlideShowChangeInvoke()
        {
            m_OnSlideShowChanged?.Invoke(this);
        }
    }
}
