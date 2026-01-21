using UnityEngine;

namespace LegionKnight
{
    [CreateAssetMenu(fileName = "New SlideShow", menuName = "Legion Knight/SlideShow", order = 1)]
    public partial class SlideShowDefinition : ScriptableObject
    {
        [SerializeField]
        private string m_SlideShowName;
        [SerializeField]
        private SlideShows[] m_Slides;
        public string SlideShowName => m_SlideShowName;
        public SlideShows[] Slides => m_Slides;
        public void StartSlideShow()
        {
            GameManager.Instance.StartSlideShow(this);
        }
        public void NextSlideShow()
        {
            GameManager.Instance.ShowNextSlide();
        }
        public void PreviousSlideShow()
        {
            GameManager.Instance.ShowPreviousSlide();
        }
        public void SetSlideShow()
        {
            GameManager.Instance.SetSlideShow(this);
        }
    }

    [System.Serializable]
    public class SlideShows
    {
        [SerializeField]
        private Sprite m_Illustration;
        [SerializeField, TextArea]
        private string m_Dialogue;
        public Sprite Illustration => m_Illustration;
        public string Dialogue => m_Dialogue;
    }
}
