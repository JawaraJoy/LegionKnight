using LegionKnight;
using UnityEngine;
using UnityEngine.Events;

namespace Rush
{
    public partial class Unit : Bindable
    {
        [SerializeField]
        private bool m_IsPlayer;
        [SerializeField]
        protected UnitConfig m_Config;

        [SerializeField]
        private ProgressField m_Progression;
        public bool IsPlayer => m_IsPlayer;
        // add Stats Modifier MonoBehaviour later
        public UnitConfig Config => m_Config;
        public ProgressField Progression => m_Progression;
        [SerializeField]
        private UnityEvent<Unit> m_OnInit;
        public void Init(UnitConfig config)
        {
            m_Config = config;
            RefreshInitInternal();

        }
        private void RefreshInitInternal()
        {
            m_OnInit?.Invoke(this);
            m_Progression.SetLevel(m_Config.Progression.Level);
            m_Progression.SetMaxLevel(m_Config.Progression.MaxLevel);
            /*if (m_Config is HeroUnitConfig heroConfig) 
            {
                int savedLevel = Player.Instance.HeroDeck.GetHeroUnit(heroConfig).Level;
                m_Progression.SetLevel(savedLevel);
            }*/

            foreach (MonoBehaviour bind in m_Binds)
            {
                if (bind is IUnitExtension extention)
                {
                    extention.Init(this);
                }
            }
        }
        public void RefreshInit()
        {
            //RefreshInitInternal();
        }
    }
}
