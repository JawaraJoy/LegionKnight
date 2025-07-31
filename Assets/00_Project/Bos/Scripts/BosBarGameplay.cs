using UnityEngine;

namespace LegionKnight
{
    public partial class BosBarGameplay : UIView
    {
        [SerializeField]
        private BosHealthBarView m_BosHealthBar;
        [SerializeField]
        private BosCastingBarView m_CastingBar;

        public void SetCastingName(string castingName)
        {
            m_CastingBar.SetCastingName(castingName);
            
        }
        public void SetBosName(BosDefinition defi)
        {
            m_BosHealthBar.SetBosName(defi);
        }
        public void SetHealth(float rate)
        {
            m_BosHealthBar.SetHealth(rate);
        }
        public void SetCastingTime(float castingTime)
        {
            m_CastingBar.SetCastingTime(castingTime);
        }

        public void HideCastingBar()
        {
            m_CastingBar.Hide();
        }
        public void ShowCastingBar()
        {
            m_CastingBar.Show();
        }
        public void ShowHealthBar()
        {
            m_BosHealthBar.Show();
        }
        
        public void HideHealthBar()
        {
            m_BosHealthBar.Hide();
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
        public void SetBosName(BosDefinition defi)
        {
            var bosBar = GetBosBar();
            if (bosBar != null)
            {
                bosBar.SetBosName(defi);
            }
        }
        public void SetHealth(float rate)
        {
            var bosBar = GetBosBar();
            if (bosBar != null)
            {
                bosBar.SetHealth(rate);
            }
        }
        public void ShowHealthBar()
        {
            var bosBar = GetBosBar();
            if (bosBar != null)
            {
                bosBar.ShowHealthBar();
            }
        }

        public void HideHealthBar()
        {
            var bosBar = GetBosBar();
            if (bosBar != null)
            {
                bosBar.HideHealthBar();
            }
        }
    }
}
