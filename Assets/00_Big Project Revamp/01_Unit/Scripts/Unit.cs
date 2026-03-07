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
        private UnitConfig m_Config;

        [SerializeField] // change tp progression monobehaviour later
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
            m_Progression = m_Config.Progression;
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
