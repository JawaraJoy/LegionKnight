using UnityEngine;

namespace LegionKnight
{
    public partial class BosBarGameplay : UIView
    {
        [SerializeField]
        private BosCastingBarView m_CastingBarView;

        public void SetCastingName(string castingName)
        {
            m_CastingBarView.SetCastingName(castingName);
            
        }
        public void SetCastingTime(float castingTime)
        {
            m_CastingBarView.SetCastingTime(castingTime);
        }

        public void HideCastingBar()
        {
            m_CastingBarView.Hide();
        }

    }

    public partial class GameplayPanel
    {
        private BosBarGameplay GetBosBar()
        {
            return GetBinding<BosBarGameplay>();
        }

        public void SetCastingName(string castingName)
        {
            var bosBar = GetBosBar();
            if (bosBar != null)
            {
                bosBar.SetCastingName(castingName);
            }
        }
        public void SetCastingTime(float castingTime)
        {
            var bosBar = GetBosBar();
            if (bosBar != null)
            {
                bosBar.SetCastingTime(castingTime);
            }
        }

        public void HideCastingBar()
        {
            var bosBar = GetBosBar();
            if (bosBar != null)
            {
                bosBar.HideCastingBar();
            }
        }
    }
}
