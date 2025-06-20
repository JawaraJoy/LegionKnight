using UnityEngine;

namespace LegionKnight
{
    public class StarGroupView : UIView
    {
        [SerializeField]
        private StarView[] m_StarViews;

        public void Init(int openedStar, int maxStar)
        {
            if (m_StarViews == null || m_StarViews.Length == 0)
            {
                Debug.LogWarning("Star views are not initialized.");
                return;
            }
            for (int i = 0; i < maxStar; i++)
            {
                if (i < m_StarViews.Length)
                {
                    m_StarViews[i].Show();
                }
                else
                {
                    Debug.LogWarning($"Star view at index {i} does not exist.");
                }
            }
            for (int i = 0; i < openedStar; i++)
            {
                if (i < m_StarViews.Length)
                {
                    m_StarViews[i].ShowStar();
                }
                else
                {
                    Debug.LogWarning($"Star view at index {i} does not exist.");
                }
            }
            
            if (openedStar >= maxStar)
            {
                Debug.Log("All stars are opened.");
            }
            else
            {
                Debug.Log($"Opened stars: {openedStar}, Max stars: {maxStar}");
            }
        }
        public void ShowStar(int index)
        {
            if (index < 0 || index >= m_StarViews.Length)
            {
                Debug.LogWarning($"Index {index} is out of bounds for star views.");
                return;
            }
            m_StarViews[index].ShowStar();
        }
        public void HideStar(int index)
        {
            if (index < 0 || index >= m_StarViews.Length)
            {
                Debug.LogWarning($"Index {index} is out of bounds for star views.");
                return;
            }
            m_StarViews[index].HideStar();
        }
    }
}
