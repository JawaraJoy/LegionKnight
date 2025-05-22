using UnityEngine;

namespace LegionKnight
{
    [CreateAssetMenu(fileName = "New SlideShow", menuName = "Legion Knight/SlideShow", order = 1)]
    public partial class SlideShowDefinition : ScriptableObject
    {
        [SerializeField]
        private string m_SlideShowName;
        [SerializeField]
        private Sprite[] m_Slides;

        public string SlideShowName => m_SlideShowName;
        public Sprite[] Slides => m_Slides;


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
}
