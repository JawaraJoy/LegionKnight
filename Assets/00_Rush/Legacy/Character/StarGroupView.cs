using Rush;
using UnityEngine;

namespace LegionKnight
{
    public class StarGroupView : UIView
    {
        [SerializeField]
        private StarView[] m_StarViews;

        public void Init(HeroUnitConfig heroConfig)
        {
            HeroUnit heroUnit = Player.Instance.HeroesCollection.GetHeroUnit(heroConfig);
            if (m_StarViews == null || heroUnit == null) return;
            for (int slot = 0; slot < m_StarViews.Length; slot++)
            {
                var star = m_StarViews[slot];
                if (star == null) continue;
                if (slot >= HeroEvolution.StarSlots)
                {
                    star.Hide();
                    continue;
                }
                star.Show();
                star.SetTier(HeroEvolution.StarTier(heroUnit.Star, slot));
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

