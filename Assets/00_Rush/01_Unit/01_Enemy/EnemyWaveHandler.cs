using LegionKnight;
using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Rush
{
    public partial class EnemyWaveHandler : MonoBehaviour, IReseter
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
        [SerializeField]
        private UnityEvent<Unit> m_OnEnemyDespawn;
        [SerializeField]
        private UnityEvent<int> m_OnCurrentScoreChanged;
        public UnityEvent<EnemyWaveConfig> OnWaveSetCleared => m_OnWaveSetCleared;
        private int m_CurrenWaveIndex = -1;
        private int m_CurrentThreshold = 0;
        private int m_CurrentScore = 0;
        private EnemyWaveSpawnPost m_EnemyWavePost;

        // Optional: track actives (useful for cleanup between waves)
        private readonly HashSet<Unit> m_ActiveEnemies = new();
        // ✅ Pool per config (prevents wrong-type reuse)
        private readonly Dictionary<EnemyUnitConfig, Queue<Unit>> m_PoolsByConfig = new();

        private readonly HashSet<Unit> m_ReservedEnemies = new();
        private readonly Dictionary<Unit, EnemyUnitConfig> m_UnitConfigs = new();
        private readonly Dictionary<Unit, Coroutine> m_PendingSpawns = new();
        private Transform m_PoolRoot;
        private int m_SpawnGeneration;
        private bool m_IsResetting;
        private bool m_IncludeBoss = true;
        private int m_WaveVersion;
        private const int c_ClassicDifficultyPerCycle = 1;
        public bool IncludesBoss => m_IncludeBoss;
        [SerializeField]
        private int m_RecentSpawnCount = 8;
        private readonly List<Vector3> m_RecentMinionSpawnPositions = new();
        private Unit m_BossUnitExisten;
        private Sprite m_CurrentWaveIcon;
        private int m_CurrentMaxThreshold;

        public Unit BossUnitExisten => m_BossUnitExisten;

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
            OnWaveLevelChangedInvoke(m_WaveLevel);
            foreach (Unit spawner in GetActiveEnemies())
            {
                spawner.SetCombatLevel(m_WaveLevel);
            }
        }
        private void SetCurrentWaveIcon(Sprite waveIcon)
        {
            if (m_CurrentWaveIcon == waveIcon) return;
            m_CurrentWaveIcon = waveIcon;
            OnWaveIconChangedInvoke(waveIcon);
        }
        public HashSet<Unit> GetActiveEnemies()
        {
            return m_ActiveEnemies;
        }
        private void AddScoreInternal(int amount)
        {
            m_CurrentScore += amount;
            OnScoreChangedInvoke(m_CurrentScore);
        }
        private void SetScoreInternal(int amount)
        {
            m_CurrentScore = amount;
            OnScoreChangedInvoke(m_CurrentScore);
        }
        private void OnScoreChangedInvoke(int newScore)
        {
            m_OnCurrentScoreChanged?.Invoke(newScore);
            // check highest player score
            // if new score is higher, update the score in player
        }

private void OnWaveLevelChangedInvoke(int waveLevel)
        {
            m_OnWaveLevelChanged?.Invoke(waveLevel);
        }

        private void OnWaveIconChangedInvoke(Sprite waveIcon)
        {
            m_OnWaveIconChanged?.Invoke(waveIcon);
        }

        private void OnWaveSetClearedInvoke(EnemyWaveConfig waveConfig)
        {
            m_OnWaveSetCleared?.Invoke(waveConfig);
        }

        private void OnBossSpawnInvoke(Unit bossUnit)
        {
            m_OnBossSpawn?.Invoke(bossUnit);
        }

        private void OnBossDespawnInvoke(Unit bossUnit)
        {
            m_OnBossDespawn?.Invoke(bossUnit);
        }

        private void OnEnemyDespawnInvoke(Unit enemyUnit)
        {
            m_OnEnemyDespawn?.Invoke(enemyUnit);
        }

        private void OnCurrentThresholdRateChangedInvoke(int currentThreshold, int maximumThreshold)
        {
            m_OnCurrentThresholdRateChanged?.Invoke(currentThreshold, maximumThreshold);
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
            SetNewWaveInternal(waveConfig, true);
        }

        public void SetNewWave(EnemyWaveConfig waveConfig, bool includeBoss)
        {
            SetNewWaveInternal(waveConfig, includeBoss);
        }

        private void SetNewWaveInternal(EnemyWaveConfig waveConfig, bool includeBoss)
        {
            if (waveConfig == null) return;
            if (m_IncludeBoss != includeBoss) ResetProgressionInternal();
            m_IncludeBoss = includeBoss;
            ++m_WaveVersion;
            m_CurrentEnemyWave = waveConfig;
            PrewarmUnitCurrentWave(waveConfig);
            m_CurrentMaxThreshold = waveConfig.RestThreshold;
            OnCurrentThresholdRateChangedInvoke(m_CurrentThreshold, m_CurrentMaxThreshold);
        }

        private void PrewarmUnitCurrentWave(EnemyWaveConfig waveConfig)
        {
            SetCurrentThresholdInternal(0);
            SetCurrentWaveIndexInternal(0);
            SetWaveStateInternal(WaveState.Rest);
            EnsurePoolRootInternal();
            var required = new Dictionary<EnemyUnitConfig, int>();
            foreach (EnemyUnitConfig config in GetAllEnemyWaves(waveConfig))
            {
                if (config == null) continue;
                required[config] = required.TryGetValue(config, out int count) ? count + 1 : 1;
            }
            foreach (var entry in required)
            {
                int allocated = 0;
                foreach (var owned in m_UnitConfigs)
                    if (owned.Key != null && owned.Value == entry.Key) allocated++;
                Queue<Unit> pool = EnsurePoolInternal(entry.Key);
                while (allocated < entry.Value)
                {
                    Unit unit = CreateUnit(entry.Key);
                    if (unit == null) break;
                    pool.Enqueue(unit);
                    allocated++;
                }
            }
        }
        private EnemyUnitConfig[] GetAllEnemyWaves(EnemyWaveConfig waveConfig)
        {
            var configs = new List<EnemyUnitConfig>();
            if (m_IncludeBoss && waveConfig.BossWaveField?.BossConfig != null)
                configs.Add(waveConfig.BossWaveField.BossConfig);
            if (waveConfig.MinionWaveFields != null)
                foreach (MinionWaveField wave in waveConfig.MinionWaveFields)
                    if (wave?.MinionConfigs != null) configs.AddRange(wave.MinionConfigs);
            return configs.ToArray();
        }
        private EnemyUnitConfig[] GetAllMinions(MinionWaveField waveField)
        {
            return waveField.MinionConfigs;
        }
        private Queue<Unit> EnsurePoolInternal(EnemyUnitConfig cfg)
        {
            if (!m_PoolsByConfig.TryGetValue(cfg, out var pool))
            {
                pool = new Queue<Unit>();
                m_PoolsByConfig.Add(cfg, pool);
            }
            return pool;
        }

        private void AcquireUnitInternal(EnemyUnitConfig enemyConfig, out Unit unit)
        {
            unit = null;
            if (enemyConfig == null || enemyConfig.UnitPrefab == null || m_EnemyWavePost == null
                || m_EnemyWavePost.PostToSpawn == null || m_IsResetting) return;
            Queue<Unit> pool = EnsurePoolInternal(enemyConfig);
            while (pool.Count > 0 && unit == null) unit = pool.Dequeue();
            if (unit == null) unit = CreateUnit(enemyConfig);
            if (unit == null) return;
            unit.gameObject.SetActive(false);
            unit.transform.SetParent(m_EnemyWavePost.PostToSpawn, false);
            m_ReservedEnemies.Add(unit);
        }
        private Unit CreateUnit(EnemyUnitConfig enemyConfig)
        {
            if (enemyConfig == null || enemyConfig.UnitPrefab == null) return null;
            EnsurePoolRootInternal();
            // An inactive parent prevents Awake/OnEnable and combat before preparation.
            Unit unit = Instantiate(enemyConfig.UnitPrefab, m_PoolRoot, false);
            unit.gameObject.SetActive(false);
            m_UnitConfigs.Add(unit, enemyConfig);
            return unit;
        }
        private void EnsurePoolRootInternal()
        {
            if (m_PoolRoot != null) return;
            var root = new GameObject("Enemy pool");
            root.SetActive(false);
            root.transform.SetParent(transform, false);
            m_PoolRoot = root.transform;
        }

        private void SpawnUnitInternal(EnemyUnitConfig config, Vector3 position, Quaternion rotation, bool boss)
        {
            if (boss && !m_IncludeBoss) return;
            AcquireUnitInternal(config, out Unit unit);
            if (unit == null) return;
            unit.transform.SetPositionAndRotation(position, rotation);
            if (boss) m_BossUnitExisten = unit;
            m_PendingSpawns.Add(unit, null);
            m_PendingSpawns[unit] = StartCoroutine(PreparingSpawnInternal(unit, config, boss, m_SpawnGeneration));
        }

        private IEnumerator PreparingSpawnInternal(Unit unit, EnemyUnitConfig config, bool boss, int generation)
        {
            yield return null;
            yield return unit.InitializeForSpawn(config, Mathf.Max(m_WaveLevel, config.Progression.Level));
            if (generation != m_SpawnGeneration || !m_ReservedEnemies.Contains(unit)) yield break;
            m_PendingSpawns.Remove(unit);
            if (unit.TryGetComponent(out PingPongMover2D mover)) mover.PrepareForSpawn();
            m_ActiveEnemies.Add(unit);
            unit.gameObject.SetActive(true);
            if (boss) SpawnBoss(unit);
        }

        private void DespawnUnitInternal(Unit unit)
        {
            ReturnUnitToPoolInternal(unit, true);
        }

        private void ReturnUnitToPoolInternal(Unit unit, bool defeated)
        {
            if (unit == null || !m_ReservedEnemies.Remove(unit)) return;
            bool wasActive = m_ActiveEnemies.Remove(unit);
            if (m_PendingSpawns.TryGetValue(unit, out Coroutine preparation))
            {
                m_PendingSpawns.Remove(unit);
                if (preparation != null) StopCoroutine(preparation);
            }
            EnemyUnitConfig config = m_UnitConfigs[unit];
            unit.gameObject.SetActive(false);
            unit.transform.SetParent(m_PoolRoot, false);
            Queue<Unit> pool = EnsurePoolInternal(config);
            if (!pool.Contains(unit)) pool.Enqueue(unit);
            bool grantRewards = defeated && wasActive && !m_IsResetting;
            if (grantRewards)
            {
                GrantDefeatRewardsInternal(config);
                OnEnemyDespawnInvoke(unit);
            }
            DespawnBossInternal(unit, grantRewards);
        }

        protected virtual void GrantDefeatRewardsInternal(EnemyUnitConfig config)
        {
            RushGameManager.Instance.RogueLikeManager.AddForPlayerExperience(config.ExpReward);
            RushPlayer.Instance.PlayerScore.AddScore(config.ScoreReward);
        }

        protected virtual void SetBossDetailVisibleInternal(bool visible)
        {
            GameplayPanel gameplayPanel = CanvasManager.Instance.GetPanel<GameplayPanel>();
            if (gameplayPanel != null) gameplayPanel.ShowBossDetailContent(visible);
        }

        private void SpawnBoss(Unit bossUnit)
        {
            if (bossUnit == null || m_BossUnitExisten != bossUnit) return;
            if (bossUnit.TryGetComponent(out BossBattlePresentation presentation)) presentation.Enter(bossUnit, this);
            SetBossDetailVisibleInternal(true);
            OnBossSpawnInvoke(bossUnit);
            AddWaveLevelInternal(1);
        }
        private void DespawnBossInternal(Unit bossUnit, bool defeated)
        {
            if (bossUnit == null || m_BossUnitExisten != bossUnit) return;
            EnemyWaveConfig clearedWave = m_CurrentEnemyWave;
            if (defeated) SetWaveStateInternal(WaveState.Cleared);
            m_BossUnitExisten = null;
            SetBossDetailVisibleInternal(false);
            if (!defeated) return;
            OnBossDespawnInvoke(bossUnit);
            OnWaveSetClearedInvoke(clearedWave);
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
                    var wave = m_CurrentEnemyWave.BossWaveField;
                    SetCurrentWaveIcon(wave.BossConfig?.CollectibleField.Icon ?? wave.Icon);
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
            if (m_IsResetting || m_CurrentEnemyWave == null || m_BossUnitExisten != null || m_WaveState == WaveState.Cleared) return;
            m_CurrentThreshold += amount;
            UpdateState();
            OnCurrentThresholdRateChangedInvoke(m_CurrentThreshold, m_CurrentMaxThreshold);
        }
        public void AddChargeToBossSummons(int amount)
        {
            Unit boss = m_BossUnitExisten;
            if (boss != null && m_ActiveEnemies.Contains(boss) && boss.HasBind(out SummonController summons))
                summons.AddChargeToSummon(amount);
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
                        if (!m_IncludeBoss)
                        {
                            CompleteMinionCycleInternal();
                            return;
                        }
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
            if (m_CurrentThreshold < currentMaxThreshold) return;
            var wave = m_CurrentEnemyWave.MinionWaveFields[m_CurrenWaveIndex];
            var configs = GetAllMinions(wave);
            Transform origin = m_EnemyWavePost != null ? m_EnemyWavePost.PostToSpawn : null;
            if (origin == null || configs == null) return;
            var occupied = new List<Vector3>(m_RecentMinionSpawnPositions);
            foreach (Unit unit in m_ReservedEnemies)
                if (unit != null) occupied.Add(unit.transform.position);
            for (int i = 0; i < configs.Length; i++)
            {
                if (configs[i] == null) continue;
                Vector3 position = origin.position;
                Quaternion rotation = origin.rotation;
                if (wave.SpawnShapeConfig is CircleSpawnShapeConfig circle)
                    circle.GetDistributedSpawnTransform(origin, i, configs.Length, occupied, out position, out rotation);
                else if (wave.SpawnShapeConfig != null)
                    wave.SpawnShapeConfig.GetSpawnTransform(origin, i, configs.Length, out position, out rotation);
                SpawnUnitInternal(configs[i], position, rotation, false);
                occupied.Add(position);
                m_RecentMinionSpawnPositions.Add(position);
                while (m_RecentMinionSpawnPositions.Count > Mathf.Max(0, m_RecentSpawnCount))
                    m_RecentMinionSpawnPositions.RemoveAt(0);
            }
            m_CurrentThreshold -= currentMaxThreshold;
            AddCurrentWaveIndexInternal(1);
            if (!m_IncludeBoss && m_CurrenWaveIndex >= m_CurrentEnemyWave.MinionWaveFields.Length)
                CompleteMinionCycleInternal();
        }

        private void CompleteMinionCycleInternal()
        {
            if (m_IncludeBoss || m_IsResetting || m_WaveState == WaveState.Cleared) return;
            EnemyWaveConfig completed = m_CurrentEnemyWave;
            int overflow = m_CurrentThreshold;
            int version = m_WaveVersion;
            SetWaveStateInternal(WaveState.Cleared);
            SetCurrentThresholdInternal(0);
            AddWaveLevelInternal(c_ClassicDifficultyPerCycle);
            OnWaveSetClearedInvoke(completed);
            // A stage listener may synchronously start the next cycle using the same config.
            if (!m_IncludeBoss && m_WaveVersion != version && m_WaveState == WaveState.Rest)
                m_CurrentThreshold += overflow;
        }

        private void BossState(int currentMaxThreshold)
        {
            if (!m_IncludeBoss) return;
            if (m_BossUnitExisten != null || m_CurrentThreshold < currentMaxThreshold) return;
            Transform origin = m_EnemyWavePost != null ? m_EnemyWavePost.PostToSpawn : null;
            if (origin == null) return;
            m_CurrentThreshold -= currentMaxThreshold;
            SpawnUnitInternal(m_CurrentEnemyWave.BossWaveField.BossConfig, origin.position, origin.rotation, true);
        }
        public void SetCurrentWaveIndex(int waveIndex)
        {
            SetCurrentWaveIndexInternal(waveIndex);
        }
        public void AddCurrentWaveIndex(int  amount)
        {
            AddCurrentWaveIndexInternal(amount);
        }

        public void ResetProgression()
        {
            ResetProgressionInternal();
        }
        private void ResetProgressionInternal()
        {
            if (m_IsResetting) return;
            m_IsResetting = true;
            try
            {
                ++m_SpawnGeneration;
                var reserved = new List<Unit>(m_ReservedEnemies);
                foreach (Unit unit in reserved) ReturnUnitToPoolInternal(unit, false);
                StopAllCoroutines();
                m_PendingSpawns.Clear();
                m_ReservedEnemies.Clear();
                m_RecentMinionSpawnPositions.Clear();
                m_ActiveEnemies.Clear();
                m_BossUnitExisten = null;
                m_EnemyWavePost?.ResetProgression();
                SetCurrentThresholdInternal(0);
                SetCurrentWaveIndexInternal(0);
                SetScoreInternal(0);
                m_WaveLevel = 1;
                OnWaveLevelChangedInvoke(m_WaveLevel);
                if (m_CurrentEnemyWave == null) return;
                SetWaveStateInternal(WaveState.Rest);
                m_CurrentMaxThreshold = m_CurrentEnemyWave.RestThreshold;
                OnCurrentThresholdRateChangedInvoke(0, m_CurrentMaxThreshold);
            }
            finally
            {
                m_IsResetting = false;
            }
        }

    }
}
