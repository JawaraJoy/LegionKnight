using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LegionKnight
{
    public partial class SlideShowView : UIView
    {
        [SerializeField]
        private Image m_SlideImage;

        [SerializeField]
        private TextMeshProUGUI m_SlidePageText;
        [SerializeField]
        private Button m_CloseButton;

        private void Start()
        {
            GameManager.Instance.AddOnSlideChanged(ShowSlide);
        }
        private void OnDestroy()
        {
            GameManager.Instance.RemoveOnSlideChanged(ShowSlide);
        }

        private void OnEnable()
        {
            ShowSlide(GameManager.Instance.SlideShowHandler);
        }

        private void ShowSlide(SlideShowHandler slide)
        {
            if (!m_IsShow)
            {
                ShowInternal();
            }
            if (slide == null)
            {
                Debug.LogError("SlideShowHandler is null.");
                return;
            }
            if (slide.CurrentSlideShow == null)
            {
                Debug.LogError("CurrentSlideShow is null.");
                return;
            }
            m_SlideImage.sprite = slide.CurrentSlideShow.Slides[slide.CurrentSlideIndex];
            m_SlidePageText.text = $"{slide.CurrentSlideIndex + 1}/{slide.TotalSlides}";

            // Show the close button if already on the last slide
            m_CloseButton.gameObject.SetActive(slide.CurrentSlideIndex == slide.TotalSlides - 1);
            
        }

        public void ShowNext()
        {
            GameManager.Instance.ShowNextSlide();
        }
        public void ShowPrevious()
        {
            GameManager.Instance.ShowPreviousSlide();
        }
    }
}
