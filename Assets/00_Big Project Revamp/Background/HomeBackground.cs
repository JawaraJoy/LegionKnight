using LegionKnight;
using UnityEngine;

namespace Rush
{
    public class HomeBackground : Singleton<HomeBackground>, IView
    {
        [SerializeField]
        private GameObject m_Content;
        [SerializeField]
        private SpriteRenderer m_Background;

        public void Show()
        {
            m_Content.SetActive(true);
        }

        public void Hide()
        {
            m_Content.SetActive(false);
        }
    }
}
