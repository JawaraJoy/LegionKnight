using LegionKnight;
using System.Collections;
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
            RushGameManager.Instance.StartCoroutine(RefreshInitInternal());

        }
        private void InitInternal()
        {

        }
        private IEnumerator RefreshInitInternal()
        {
            m_OnInit?.Invoke(this);
            m_Progression.SetLevel(m_Config.Progression.Level);
            m_Progression.SetMaxLevel(m_Config.Progression.MaxLevel);
            /*if (m_Config is HeroUnitConfig heroConfig) 
            {
                int savedLevel = Player.Instance.HeroDeck.GetHeroUnit(heroConfig).Level;
                m_Progression.SetLevel(savedLevel);
            }*/

            foreach (MonoBehaviour reseter in m_Binds)
            {
                if (reseter is IReseter resetter)
                {
                    resetter.ResetProgression();
                    yield return new WaitForEndOfFrame();
                }
            }

            yield return new WaitForEndOfFrame();

            foreach (MonoBehaviour bind in m_Binds)
            {
                if (bind is IUnitExtension extention)
                {
                    extention.Init(this);
                    yield return new WaitForEndOfFrame();
                }
            }
        }
        public void RefreshInit()
        {
            StartCoroutine(RefreshInitInternal());
        }


    }
}
