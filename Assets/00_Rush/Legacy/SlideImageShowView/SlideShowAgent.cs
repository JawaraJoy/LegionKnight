using UnityEngine;

namespace LegionKnight
{
    public partial class SlideShowAgent : MonoBehaviour
    {
        public void StartSlideShow(SlideShowDefinition defi)
        {
            GameManager.Instance.StartSlideShow(defi);
        }
        public void StartSlideShow()
        {
            GameManager.Instance.StartSlideShow();
        }
        public void ShowNextSlide()
        {
            GameManager.Instance.ShowNextSlide();
        }
        public void ShowPreviousSlide()
        {
            GameManager.Instance.ShowPreviousSlide();
        }
        public void SetSlideShow(SlideShowDefinition defi)
        {
            GameManager.Instance.SetSlideShow(defi);
        }
    }
}
