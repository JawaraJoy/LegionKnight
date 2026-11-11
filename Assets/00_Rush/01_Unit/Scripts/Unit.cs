using System.Collections;
using UnityEngine;
using UnityEngine.Events;
namespace Rush
{
    public partial class Unit : Bindable
    {
        [SerializeField] private bool m_IsPlayer;
        [SerializeField] protected UnitConfig m_Config;
        [SerializeField] private ProgressField m_Progression;
        [SerializeField] private UnityEvent<Unit> m_OnInit;

        private StatField m_ProgressionBonus = StatField.Zero;
        internal int SpawnGeneration { get; private set; }
        public bool IsPlayer => m_IsPlayer;
        public UnitConfig Config => m_Config;
        public ProgressField Progression => m_Progression;
        public StatField BaseStats => m_Config.MainStats.GetFinalStat(m_Progression.Level) + m_ProgressionBonus;

        public void SetCombatLevel(int level) => SetCombatLevelInternal(level);
        private void SetCombatLevelInternal(int level)
        {
            float healthRate = 0f;
            bool hasHealth = HasBind(out Damageable damageable);
            if (hasHealth) healthRate = damageable.HealthRate;
            m_Progression.SetLevel(level);
            if (hasHealth)
            {
                damageable.RefreshDamageableStat(healthRate, true);
                if (healthRate > 0f && damageable.Health == 0 && damageable.MaxHealth > 0)
                    damageable.SetHealth(1);
            }
        }

        public void Init(UnitConfig config) => InitInternal(config);
        private void InitInternal(UnitConfig config)
        {
            m_Config = config;
            RushGameManager.Instance.StartCoroutine(RefreshInitInternal());
        }

        protected virtual void InitializeProgressionInternal()
        {
            SetProgressionInternal(m_Config.Progression.Level, StatField.Zero);
        }

        protected void SetProgressionInternal(int level, StatField bonus)
        {
            m_Progression.SetMaxLevel(m_Config.Progression.MaxLevel);
            m_Progression.SetLevel(level);
            m_ProgressionBonus = bonus ?? StatField.Zero;
        }

        private IEnumerator RefreshInitInternal()
        {
            InitializeProgressionInternal();
            OnInitInvoke();
            foreach (MonoBehaviour bind in m_Binds)
            {
                if (bind is IReseter resetter)
                {
                    resetter.ResetProgression();
                    yield return null;
                }
            }
            yield return null;
            foreach (MonoBehaviour bind in m_Binds)
            {
                if (bind is IUnitExtension extension)
                {
                    extension.Init(this);
                    yield return null;
                }
            }
        }

        // The spawner owns this routine, so pool return/reset can cancel all preparation.
        public IEnumerator InitializeForSpawn(UnitConfig config, int level)
        {
            return InitializeForSpawnInternal(config, level);
        }

        private IEnumerator InitializeForSpawnInternal(UnitConfig config, int level)
        {
            SpawnGeneration++;
            m_Config = config;
            InitializeProgressionInternal();
            m_Progression.SetLevel(level);
            OnInitInvoke();
            foreach (MonoBehaviour bind in m_Binds)
            {
                if (bind is IReseter resetter)
                {
                    resetter.ResetProgression();
                    yield return null;
                }
            }
            foreach (MonoBehaviour bind in m_Binds)
            {
                if (bind is Damageable damageable)
                    yield return damageable.InitializeForSpawn(this);
                else if (bind is IUnitExtension extension)
                    extension.Init(this);
                yield return null;
            }
        }

        public void RefreshInit() => StartCoroutine(RefreshInitInternal());
        private void OnInitInvoke() => m_OnInit?.Invoke(this);
    }
}