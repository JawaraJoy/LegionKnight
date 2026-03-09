using MoreMountains.Tools;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Rush
{
    public class EnemyWaveHandler : MonoBehaviour
    {
        [SerializeField]
        private WaveState m_WaveState = WaveState.Rest;
        [SerializeField]
        private EnemyWaveConfig m_CurrentEnemyWave;

        [SerializeField]
        private UnityEvent<int, int> m_OnCurrentThresholdRateChanged;
        [SerializeField]
        private UnityEvent<Sprite> m_OnWaveIconChanged;
        [SerializeField] 
        private UnityEvent<EnemyWaveConfig> m_OnWaveSetCleared;
        [SerializeField]
        private UnityEvent<Unit> m_OnBossSpawn;
        [SerializeField]
        private UnityEvent<Unit> m_OnBossDespawn;
        public UnityEvent<EnemyWaveConfig> OnWaveSetCleared => m_OnWaveSetCleared;
        private int m_CurrenWaveIndex = -1;
        private int m_CurrentThreshold = 0;
        private EnemyWaveSpawnPost m_EnemyWavePost;

        // Optional: track actives (useful for cleanup between waves)
        private readonly HashSet<Unit> m_ActiveEnemies = new();
        // ✅ Pool per config (prevents wrong-type reuse)
        private readonly Dictionary<EnemyUnitConfig, Queue<Unit>> m_PoolsByConfig = new();

        private Unit m_BossUnitExisten;
        private Sprite m_CurrentWaveIcon;
        private int m_CurrentMaxThreshold;

        [SerializeField, MMReadOnly]
        private int m_WaveLevel = 1;
        public Sprite CurrentWaveIcon => m_CurrentWaveIcon;
        public UnityEvent<int, int> OnCurrentThresholdRateChanged => m_OnCurrentThresholdRateChanged;
        public UnityEvent<Sprite> OnWaveIconChanged => m_OnWaveIconChanged;
        public EnemyWaveSpawnPost EnemyWavePost => m_EnemyWavePost;
        [SerializeField]
        private UnityEvent<int> m_OnWaveLevelChanged;
        private void AddWaveLevelInternal(int amount)
        {
            m_WaveLevel += amount;
            m_OnWaveLevelChanged?.Invoke(m_WaveLevel);
            foreach (Unit spawner in GetActiveEnemies())
            {
                spawner.Progression.SetLevel(m_WaveLevel);
            }
        }
        private void SetCurrentWaveIcon(Sprite waveIcon)
        {
            m_CurrentWaveIcon = waveIcon;
            m_OnWaveIconChanged?.Invoke(waveIcon);
        }
        public HashSet<Unit> GetActiveEnemies()
        {
            return m_ActiveEnemies;
        }
        private void LoopBackToStart()
        {
            // reset progress for next cycle
            SetCurrentThresholdInternal(0);
            SetCurrentWaveIndexInternal(0);
            SetWaveStateInternal(WaveState.Rest);
        }
        private MinionWaveField GetMinionWaveByIndexInternal(int waveIndex)
        {
            var waves = m_CurrentEnemyWave.MinionWaveFields;

            if (waves == null || waveIndex < 0 || waveIndex >= waves.Length)
                return null;

            return waves[waveIndex];
        }
        private bool HasMinionWaveByIndexInternal(int waveIndex, out MinionWaveField minionWaveField)
        {
            minionWaveField = GetMinionWaveByIndexInternal(waveIndex);
            return minionWaveField != null;
        }
        public void SetEnemyWavePost(EnemyWaveSpawnPost wave)
        {
            m_EnemyWavePost = wave;
        }
        public void SetNewWave(EnemyWaveConfig waveConfig)
        {
            m_CurrentEnemyWave = waveConfig;
            
            PrewarmUnitCurrentWave(waveConfig);
        }
        private void PrewarmUnitCurrentWave(EnemyWaveConfig waveConfig)
        {
            SetCurrentThresholdInternal(0);
            SetCurrentWaveIndexInternal(0);
            SetWaveStateInternal(WaveState.Rest);

            var enemies = GetAllEnemyWaves(waveConfig);

            for (int i = 0; i < enemies.Length; i++)
            {
                EnemyUnitConfig cfg = enemies[i];
                if (cfg == null) continue;

                // Create inactive unit and store in correct pool
                Unit unit = CreateUnit(cfg);
                GetOrCreatePool(cfg).Enqueue(unit);
            }
        }
        private EnemyUnitConfig[] GetAllEnemyWaves(EnemyWaveConfig waveConfig)
        {
            BossUnitConfig bossConfig = waveConfig.BossWaveField.BossConfig;
            List<MinionUnitConfig> minionConfigs = new List<MinionUnitConfig>();
            foreach (MinionWaveField minionWave in waveConfig.MinionWaveFields)
            {
                minionConfigs.AddRange(minionWave.MinionConfigs);
            }
            List<EnemyUnitConfig> enemyConfigs = new List<EnemyUnitConfig>{bossConfig};
            enemyConfigs.AddRange(minionConfigs);
            return enemyConfigs.ToArray();
        }
        private EnemyUnitConfig[] GetAllMinions(MinionWaveField waveField)
        {
            return waveField.MinionConfigs;
        }
        private Queue<Unit> GetOrCreatePool(EnemyUnitConfig cfg)
        {
            if (!m_PoolsByConfig.TryGetValue(cfg, out var pool))
            {
                pool = new Queue<Unit>();
                m_PoolsByConfig.Add(cfg, pool);
            }
            return pool;
        }

        private Unit GetUnitFromPool(EnemyUnitConfig enemyConfig)
        {
            if (enemyConfig == null)
            {
                Debug.LogError("[EnemyWaveHandler] enemyConfig is null.");
                return null;
            }

            if (m_EnemyWavePost == null || m_EnemyWavePost.PostToSpawn == null)
            {
                Debug.LogError("[EnemyWaveHandler] EnemyWaveSpawnPost not ready. Did you call SetEnemyWavePost?");
                return null;
            }

            var pool = GetOrCreatePool(enemyConfig);
            Unit unit = pool.Count > 0 ? pool.Dequeue() : CreateUnit(enemyConfig);

            // Always re-parent & re-activate
            unit.transform.SetParent(m_EnemyWavePost.PostToSpawn, false);
            unit.gameObject.SetActive(true);

            // Track active
            m_ActiveEnemies.Add(unit);
            unit.Init(enemyConfig);
            
            return unit;
        }
        private Unit CreateUnit(EnemyUnitConfig enemyConfig)
        {
            Unit unit = Instantiate(enemyConfig.UnitPrefab, m_EnemyWavePost.PostToSpawn, false);
            unit.Init(enemyConfig);
            if (unit.HasBind(out Damageable damageable))
            {
                damageable.OnDeath.AddListener((context) => DespawnUnitInternal(unit));
            }

            unit.gameObject.SetActive(false);
            return unit;
        }
        private void DespawnUnitInternal(Unit unit)
        {
            if (unit == null) return;

            // Determine which pool it belongs to
            if (unit.Config is not EnemyUnitConfig enemyCfg)
            {
                Debug.LogWarning($"[EnemyWaveHandler] DespawnUnit called but unit.Config is not EnemyUnitConfig: {unit.name}");
                unit.gameObject.SetActive(false);
                m_ActiveEnemies.Remove(unit);
                return;
            }

            unit.gameObject.SetActive(false);
            unit.transform.SetParent(m_EnemyWavePost.PostToSpawn, false);

            GetOrCreatePool(enemyCfg).Enqueue(unit);
            m_ActiveEnemies.Remove(unit);
            DespawnBoss(unit);
        }

        private void SpawnBoss(Unit bossUnit)
        {
            if (bossUnit == null) return;
            bossUnit.transform.position = m_EnemyWavePost.PostToSpawn.position;
            if (bossUnit.Config is BossUnitConfig)
            {
                m_BossUnitExisten = bossUnit;
                m_OnBossSpawn?.Invoke(bossUnit);
                AddWaveLevelInternal(1);
            }
        }
        private void DespawnBoss(Unit bossUnit)
        {
            if (bossUnit == null) return;
            if (bossUnit.Config is BossUnitConfig)
            {
                m_BossUnitExisten = null;

                m_OnWaveSetCleared?.Invoke(m_CurrentEnemyWave);
                m_OnBossDespawn?.Invoke(bossUnit);
                
            }
        }
        /// <summary>
        /// Call this when an enemy dies / should despawn.
        /// You can call from Health script, AI script, etc.
        /// </summary>
        public void DespawnUnit(Unit unit)
        {
            DespawnUnitInternal(unit);
        }
        private void SetWaveStateInternal(WaveState state)
        {
            m_WaveState = state;

            switch (m_WaveState)
            {
                case WaveState.Rest:
                    SetCurrentWaveIcon(m_CurrentEnemyWave.Icon);
                    break;
                case WaveState.Minion:
                    int waveLength = m_CurrentEnemyWave.MinionWaveFields.Length;
                    if (m_CurrenWaveIndex >= 0 && m_CurrenWaveIndex < waveLength)
                    {
                        SetCurrentWaveIcon(m_CurrentEnemyWave.MinionWaveFields[m_CurrenWaveIndex].Icon);
                    }  
                    break;
                case WaveState.Boss:
                    SetCurrentWaveIcon(m_CurrentEnemyWave.BossWaveField.Icon);
                    break;
            }
        }
        private void SetCurrentWaveIndexInternal(int waveIndex)
        {
            m_CurrenWaveIndex = waveIndex;
        }
        private void SetCurrentThresholdInternal(int amount)
        {
            m_CurrentThreshold = amount;
        }
        private void AddCurrentWaveIndexInternal(int amount)
        {
            m_CurrenWaveIndex += amount;
        }
        private void AddCurrentThresholdInternal(int amount)
        {
            if (m_BossUnitExisten != null)
            {
                return;
            }
            m_CurrentThreshold += amount;
            UpdateState();
            m_OnCurrentThresholdRateChanged?.Invoke(m_CurrentThreshold, m_CurrentMaxThreshold);
        }
        public void AddCurrentThreshold(int amount)
        {
            AddCurrentThresholdInternal(amount);
        }

        private void UpdateState()
        {
            switch (m_WaveState)
            {
                case WaveState.Rest:
                    m_CurrentMaxThreshold = m_CurrentEnemyWave.RestThreshold;
                    RestState(m_CurrentMaxThreshold);
                    break;
                case WaveState.Minion:

                    int waveLength = m_CurrentEnemyWave.MinionWaveFields.Length;

                    if (m_CurrenWaveIndex >= waveLength)
                    {
                        SetWaveStateInternal(WaveState.Boss);
                        UpdateState();
                        return;
                    }

                    m_CurrentMaxThreshold = m_CurrentEnemyWave.MinionWaveFields[m_CurrenWaveIndex].ThresholdToSpawn;
                    MinionState(m_CurrentMaxThreshold);
                    break;
                case WaveState.Boss:
                    m_CurrentMaxThreshold = m_CurrentEnemyWave.BossWaveField.ThresholdToSpawn;
                    BossState(m_CurrentMaxThreshold);
                    break;
            }
        }
        private void RestState(int currentMaxThreshold)
        { 
            SetCurrentWaveIcon(m_CurrentEnemyWave.Icon);
            if (m_CurrentThreshold >= currentMaxThreshold)
            {
                int overFlow = m_CurrentThreshold - currentMaxThreshold;
                SetCurrentWaveIndexInternal(0);
                SetWaveStateInternal(WaveState.Minion);
                m_CurrentThreshold = overFlow;
                UpdateState();
            }
        }
        private void MinionState(int currentMaxThreshold)
        {
            int waveLenght = m_CurrentEnemyWave.MinionWaveFields.Length;
            if (m_CurrentThreshold >= currentMaxThreshold)
            {
                int overFlow = m_CurrentThreshold - currentMaxThreshold;
                if (m_CurrenWaveIndex < waveLenght)
                {
                    MinionWaveField minionWave = m_CurrentEnemyWave.MinionWaveFields[m_CurrenWaveIndex];
                    EnemyUnitConfig[] enemyUnitConfigs = GetAllMinions(minionWave);

                    var shape = minionWave.SpawnShapeConfig;
                    int total = enemyUnitConfigs.Length;

                    for (int i = 0; i < total; i++)
                    {
                        var cfg = enemyUnitConfigs[i];
                        Unit u = GetUnitFromPool(cfg);
                        u.Init(cfg);

                        if (shape != null)
                        {
                            shape.GetSpawnTransform(m_EnemyWavePost.PostToSpawn, i, total, out var pos, out var rot);
                            u.transform.SetPositionAndRotation(pos, rot);
                        }
                    }

                    AddCurrentWaveIndex(1);
                }
                else
                {
                    if (m_CurrenWaveIndex >= waveLenght)
                    {
                        SetWaveStateInternal(WaveState.Boss);
                        return;
                    }
                }
                m_CurrentThreshold = overFlow;
            }
        }
        private void BossState(int currentMaxThreshold)
        {
            if (m_BossUnitExisten != null)
                return;

            if (m_CurrentThreshold >= currentMaxThreshold)
            {
                int overFlow = m_CurrentThreshold - currentMaxThreshold;

                BossUnitConfig bossUnitConfig = m_CurrentEnemyWave.BossWaveField.BossConfig;
                Unit bossUnit = GetUnitFromPool(bossUnitConfig);

                bossUnit.Init(bossUnitConfig);
                SpawnBoss(bossUnit);

                m_CurrentThreshold = overFlow;
            }
        }
        public void SetCurrentWaveIndex(int waveIndex)
        {
            SetCurrentWaveIndexInternal(waveIndex);
        }
        public void AddCurrentWaveIndex(int  amount)
        {
            AddCurrentWaveIndexInternal(amount);
        }
    }
}
