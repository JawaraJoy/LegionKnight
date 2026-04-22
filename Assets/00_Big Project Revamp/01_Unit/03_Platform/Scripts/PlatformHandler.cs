using LegionKnight;
using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Rush
{
    public class PlatformHandler : MonoBehaviour, IReseter
    {
        [SerializeField, MMReadOnly]
        private PlatformHandlerConfig m_Config;
        [SerializeField]
        private bool m_IsPaused = false;
        [SerializeField, MMReadOnly]
        private List<PlatformConfig> m_PreparedPlatformConfigs = new();
        [SerializeField]
        private List<PlatformConfig> m_WaitingListPlatformConfigs = new();
        [SerializeField, MMReadOnly]
        private Platform2D m_CurrentNewDisplayedPlatform;
        [SerializeField, MMReadOnly]
        private Platform2D m_CurrentTouchedPlatform;
        [SerializeField, MMReadOnly]
        private float m_ActiveBoostDuration;
        [SerializeField, MMReadOnly]
        private float m_ActiveBoostElapsed;
        [SerializeField, MMReadOnly]
        private Platform2D m_CurrentLastDisplayedPlatform;
        [SerializeField, MMReadOnly]
        private Queue<Platform2D> m_StackedPlatforms = new();

        [SerializeField]
        private TouchDownCheckField m_TouchDownCheckField;

        [Header("Boost Events")]
        [SerializeField]
        private UnityEvent<float, int> m_OnBoostStart = new();
        [SerializeField]
        private UnityEvent m_OnBoostTick = new();
        [SerializeField]
        private UnityEvent m_OnBoostEnd = new();
        [SerializeField]
        private UnityEvent<float, float> m_OnBoostDurationTick = new();
        [SerializeField]
        private UnityEvent m_OnPrepare = new();
        [SerializeField]
        private UnityEvent<int> m_OnPerfectCountChanged = new();
        [SerializeField]
        private UnityEvent<int, int> m_OnCurrentBoostStockChanged = new();
        [SerializeField]
        private UnityEvent<int> m_OnBoostEnabled = new();
        [SerializeField]
        private UnityEvent m_OnBoostDisabled = new();

        public UnityEvent<float, int> OnBoostStart => m_OnBoostStart;
        public UnityEvent OnBoostTick => m_OnBoostTick;
        public UnityEvent OnBoostEnd => m_OnBoostEnd;
        public UnityEvent<float, float> OnBoostDurationTick => m_OnBoostDurationTick;
        public UnityEvent OnPrepare => m_OnPrepare;
        public UnityEvent<int> OnPerfectCountChanged => m_OnPerfectCountChanged;
        public UnityEvent<int, int> OnCurrentBoostStockChanged => m_OnCurrentBoostStockChanged;
        public UnityEvent<int> OnBoostEnabled => m_OnBoostEnabled;
        public UnityEvent OnBoostDisabled => m_OnBoostDisabled;

        [SerializeField, MMReadOnly]
        private bool m_IsSpawningNextPlatform = false;
        [SerializeField, MMReadOnly]
        private bool m_IsBoostActive = false;
        [SerializeField, MMReadOnly]
        private int m_GlobalPerfectCount = 0;
        [SerializeField, MMReadOnly]
        private int m_CurrentBoostStock = 0;

        // Pool instance per platform ID
        private readonly Dictionary<string, Queue<Platform2D>> m_Pools = new();

        // *** Spawn weight terpisah dari pool — tidak perlu Instantiate saat collect kartu ***
        private readonly Dictionary<string, int> m_SpawnWeights = new();

        public TouchDownCheckField TouchDownCheckField => m_TouchDownCheckField;
        public PlatformHandlerConfig Config => m_Config;

        [SerializeField, MMReadOnly]
        private int m_CurrentStackedPlatformsCount;
        [SerializeField]
        private float m_MinGlobalSpeedRate = 1.0f;
        [SerializeField, MMReadOnly]
        private float m_MaxGlobalSpeedRate = 1f;
        [SerializeField, MMReadOnly]
        private float m_GlobalPerfectTouchRange = 0.3f;
        [SerializeField, MMReadOnly]
        private Vector2 m_LastContactPoint = Vector2.zero;

        public bool IsPaused => m_IsPaused;
        public bool IsBoostActive => m_IsBoostActive;
        public Platform2D CurrentTouchedPlatform => m_CurrentTouchedPlatform;
        public int GlobalPerfectCount => m_GlobalPerfectCount;
        public int CurrentBoostStock => m_CurrentBoostStock;
        public Vector2 LastContactPoint => m_LastContactPoint;
        public float MinGlobalSpeedRate => m_MinGlobalSpeedRate;
        public float MaxGlobalSpeedRate => m_MaxGlobalSpeedRate;
        public float GlobalPerfectTouchRange => m_GlobalPerfectTouchRange;
        public int TotalPlayedPlatforms => m_CurrentStackedPlatformsCount;

        // -------------------------------------------------------------------------
        // Public setters
        // -------------------------------------------------------------------------

        public void SetCurrentTouchedPlatform(Platform2D platform)
        {
            m_CurrentTouchedPlatform = platform;
        }

        public void SetIsBoostActive(bool value)
        {
            m_IsBoostActive = value;
            if (!value)
                ResetGlobalPerfectCount();
        }

        public void SetLastContactPoint(Vector2 point)
        {
            m_LastContactPoint = point;
        }

        // -------------------------------------------------------------------------
        // Prepare & lifecycle
        // -------------------------------------------------------------------------

        public void Prepare(PlatformHandlerConfig config)
        {
            if (config == null) return;
            if (config == m_Config) return;

            m_CurrentStackedPlatformsCount = 0;
            ClearPreparedPlatformConfigsInternal();
            ClearWaitingListPlatformConfigInternal();
            m_Config = config;

            Unit unitPlayer = RushPlayer.Instance.Unit;
            if (unitPlayer.HasBind(out PlatformController controller))
            {
                AddPreparedPlatformsConfigInternal(config.InitialPlatformConfigs, controller);
                if (unitPlayer.Config is IHasPlatform owner)
                    AddPreparedPlatformsConfigInternal(owner.UniquePlatforms, controller);
            }

            m_LastContactPoint = RushPlayer.Instance.PlatformSpawnPost.position;
            SetMaxGlobalSpeedRateInternal(config.MaxGlobalSpeedRate);
            SetMinGlobalSpeedRateInternal(config.MinGlobalSpeedRate);
            SetGlobalPerfectTouchRange(config.GlobalPerfectTouchRange);

            InputToWaitingListByRandom();

            if (config.BoostField != null)
                SetBoostStockInternal(config.BoostField.MaxBoostStock);

            m_TouchDownCheckField.RegisterPerfectLandingCallback(OnGlobalPerfectLanding);
            m_TouchDownCheckField.RegisterNormalLandingCallback(OnGlobalNormalLanding);

            m_OnPrepare?.Invoke();
        }

        public void Play()
        {
            m_IsPaused = false;
            if (m_Config == null) return;
            SpawnNextPlatformFromWaitingListInternal(m_Config.InitialSpawnDelay);
            m_LastContactPoint = RushPlayer.Instance.PlatformSpawnPost.position;
        }

        public void Pause()
        {
            m_IsPaused = true;
        }

        public void Resume()
        {
            RushGameManager.Instance.StartCoroutine(Resuming(0.4f));
        }
        private IEnumerator Resuming(float delay)
        {
            yield return new WaitForSeconds(delay);
            m_IsPaused = false;
        }
        // -------------------------------------------------------------------------
        // Prepared platform config management
        // -------------------------------------------------------------------------

        public void AddPreparedPlatformConfigs(PlatformConfig[] configs, PlatformController controller)
        {
            AddPreparedPlatformsConfigInternal(configs, controller);
        }

        public void AddPreparedPlatformConfig(PlatformConfig config, PlatformController controller)
        {
            AddPreparedPlatformConfigInternal(config, controller);
        }

        public void RemovePreparedPlatformConfig(PlatformConfig config)
        {
            RemovePreparedPlatformConfigInternal(config);
        }

        public void ClearPreparedPlatformConfigs()
        {
            ClearPreparedPlatformConfigsInternal();
        }

        private void AddPreparedPlatformsConfigInternal(PlatformConfig[] configs, PlatformController controller)
        {
            if (configs.Length == 0) return;
            foreach (PlatformConfig config in configs)
                AddPreparedPlatformConfigInternal(config, controller);
        }

        private void AddPreparedPlatformConfigInternal(PlatformConfig config, PlatformController controller)
        {
            string id = config.BaseInfo.Id;

            if (!HasPreparedPlatformConfig(config))
            {
                // Platform baru: daftarkan, buat pool, set weight awal dari PrewarmCount
                m_PreparedPlatformConfigs.Add(config);
                m_SpawnWeights[id] = config.PrewarmCount;
                PreWarm(config, controller);
            }
            else
            {
                // Platform sudah ada (collect kartu): cukup naikkan weight, tidak perlu Instantiate
                m_SpawnWeights[id] = m_SpawnWeights.TryGetValue(id, out int current) ? current + 1 : 1;
            }
        }

        private void RemovePreparedPlatformConfigInternal(PlatformConfig config)
        {
            if (!HasPreparedPlatformConfig(config)) return;
            m_PreparedPlatformConfigs.Remove(config);
            m_SpawnWeights.Remove(config.BaseInfo.Id);
        }

        private void ClearPreparedPlatformConfigsInternal()
        {
            m_PreparedPlatformConfigs.Clear();
            m_SpawnWeights.Clear();
        }

        // -------------------------------------------------------------------------
        // Waiting list management
        // -------------------------------------------------------------------------

        private void AddWaitingListPlatformConfigInternal(PlatformConfig config)
        {
            if (!HasWaitingListPlatformConfig(config))
                m_WaitingListPlatformConfigs.Add(config);
        }

        private void RemoveWaitingListPlatformConfigsInternal(PlatformConfig config)
        {
            if (!HasWaitingListPlatformConfig(config)) return;
            m_WaitingListPlatformConfigs.Remove(config);
            if (m_PreparedPlatformConfigs.Count > 0)
                InputToWaitingListByRandom();
        }

        private void ClearWaitingListPlatformConfigInternal()
        {
            m_WaitingListPlatformConfigs.Clear();
        }

        private void InputToWaitingListByRandom()
        {
            if (IsWaitingListFull(out _)) return;
            foreach (PlatformConfig config in m_PreparedPlatformConfigs)
            {
                if (!HasWaitingListPlatformConfig(config))
                    m_WaitingListPlatformConfigs.Add(config);
            }
        }

        private bool IsWaitingListFull(out int possibleSlotCount)
        {
            int maxSlot = m_Config.MaxStackedPlatforms;
            bool isFull = m_WaitingListPlatformConfigs.Count >= maxSlot;
            possibleSlotCount = isFull ? 0 : maxSlot - m_WaitingListPlatformConfigs.Count;
            return isFull;
        }

        // -------------------------------------------------------------------------
        // Weighted random spawn
        // -------------------------------------------------------------------------

        /// <summary>
        /// Memilih platform secara weighted random berdasarkan m_SpawnWeights.
        /// Weight naik setiap kali player collect kartu platform tersebut,
        /// tanpa perlu membuat instance baru — lebih efisien dari pool-as-weight.
        /// </summary>
        private PlatformConfig GetWeightedRandomAvailablePlatformConfig(List<PlatformConfig> configs)
        {
            List<PlatformConfig> available = new();
            foreach (PlatformConfig config in configs)
            {
                if (HasAvailableInstance(config))
                    available.Add(config);
            }

            if (available.Count == 0) return null;

            int totalWeight = 0;
            foreach (PlatformConfig config in available)
                totalWeight += GetWeight(config);

            int roll = Random.Range(0, totalWeight);
            int cumulative = 0;

            foreach (PlatformConfig config in available)
            {
                cumulative += GetWeight(config);
                if (roll < cumulative)
                    return config;
            }

            return available[0];
        }

        private int GetWeight(PlatformConfig config)
        {
            return m_SpawnWeights.TryGetValue(config.BaseInfo.Id, out int w) ? Mathf.Max(1, w) : 1;
        }

        // -------------------------------------------------------------------------
        // Spawn coroutine
        // -------------------------------------------------------------------------

        public void SpawnNextPlatformFromWaitingList(float delay)
        {
            SpawnNextPlatformFromWaitingListInternal(delay);
        }

        private void SpawnNextPlatformFromWaitingListInternal(float delay)
        {
            if (m_IsBoostActive) return;
            if (m_WaitingListPlatformConfigs.Count == 0 || m_IsSpawningNextPlatform) return;

            m_IsSpawningNextPlatform = true;
            StartCoroutine(SpawningNextPlatformFromWaitingList(delay));
        }

        private IEnumerator SpawningNextPlatformFromWaitingList(float delay)
        {
            yield return new WaitForSeconds(delay);

            PlatformConfig nextConfig = GetWeightedRandomAvailablePlatformConfig(m_WaitingListPlatformConfigs);
            if (nextConfig == null)
            {
                Debug.LogWarning("No available platform in pool!");
                m_IsSpawningNextPlatform = false;
                yield break;
            }

            Platform2D platform = GetFromPool(nextConfig);
            platform.StartMove(PlatformUtility.GetStartingSpawnHorizontalPosition(
                m_Config.SpawnHorizontalDistanceFromPost, m_LastContactPoint));
            RemoveWaitingListPlatformConfigsInternal(nextConfig);
            AddGlobalSpeedRateInternal(m_Config.SpeedRateGrowthDificulityEachStack);

            m_IsSpawningNextPlatform = false;
        }

        // -------------------------------------------------------------------------
        // Pool management
        // -------------------------------------------------------------------------

        private void PreWarm(PlatformConfig config, PlatformController controller)
        {
            string id = config.BaseInfo.Id;
            if (!m_Pools.ContainsKey(id))
                m_Pools.Add(id, new Queue<Platform2D>());

            for (int i = 0; i < config.PrewarmCount; i++)
                m_Pools[id].Enqueue(CreateNewPlatform(config, controller));
        }

        private Platform2D CreateNewPlatform(PlatformConfig config, PlatformController controller)
        {
            Platform2D newPlatform = Instantiate(config.PlatformPrefab, transform);
            newPlatform.IniPlatform(config);
            if (controller != null)
                newPlatform.Init(config.AttackSkill, controller.ModuleContext);

            newPlatform.gameObject.SetActive(false);
            return newPlatform;
        }

        private bool HasAvailableInstance(PlatformConfig config)
        {
            return m_Pools.TryGetValue(config.BaseInfo.Id, out Queue<Platform2D> pool) && pool.Count > 0;
        }

        private Platform2D GetFromPool(PlatformConfig config)
        {
            string id = config.BaseInfo.Id;
            if (!m_Pools.ContainsKey(id))
                m_Pools.Add(id, new Queue<Platform2D>());

            Queue<Platform2D> pool = m_Pools[id];
            if (pool.Count == 0) return null;

            Platform2D platform = pool.Dequeue();
            platform.gameObject.SetActive(true);
            m_CurrentNewDisplayedPlatform = platform;

            platform.OnReachDestination.RemoveAllListeners();
            platform.OnReachDestination.AddListener(() => HandlePlatformReached(platform));

            platform.OnBoostStart.RemoveAllListeners();
            platform.OnBoostStart.AddListener((duration, combo) => m_OnBoostStart?.Invoke(duration, combo));

            platform.OnBoostTick.RemoveAllListeners();
            platform.OnBoostTick.AddListener(() =>
            {
                m_ActiveBoostElapsed += 1f;
                float remaining = Mathf.Max(0f, m_ActiveBoostDuration - m_ActiveBoostElapsed);
                m_OnBoostTick?.Invoke();
                m_OnBoostDurationTick?.Invoke(remaining, m_ActiveBoostDuration);
            });

            platform.OnBoostEnd.RemoveAllListeners();
            platform.OnBoostEnd.AddListener(() => OnBoostEndInvoke());

            return platform;
        }
        private void OnBoostEndInvoke()
        {
            m_OnBoostEnd?.Invoke();
            RushPlayer.Instance.Jump.SetCanJump(true);
        }
        private void ReturnToPoolInternal(Platform2D platform)
        {
            string id = platform.PlatformConfig.BaseInfo.Id;
            if (!m_Pools.ContainsKey(id))
                m_Pools.Add(id, new Queue<Platform2D>());

            platform.TouchDownCheck.ClearBoostCheck();
            platform.gameObject.SetActive(false);
            m_Pools[id].Enqueue(platform);
        }

        public void ReturnToPool(Platform2D platform)
        {
            ReturnToPoolInternal(platform);
        }

        // -------------------------------------------------------------------------
        // Platform reached handler
        // -------------------------------------------------------------------------

        private void HandlePlatformReached(Platform2D platform)
        {
            AddTotalPlayedPlatform(1);
            m_LastContactPoint = platform.TouchDownSpot.position;
            m_StackedPlatforms.Enqueue(platform);
            m_CurrentNewDisplayedPlatform = platform;
            LimitStackSize();
            SpawnNextPlatformFromWaitingListInternal(m_Config.NextSpawnDelay);
            Debug.Log($"Reached: {platform.PlatformConfig.BaseInfo.Name} | Stack: {m_StackedPlatforms.Count}");
        }

        private void LimitStackSize()
        {
            if (m_StackedPlatforms.Count > m_Config.MaxStackedPlatforms)
                ReturnToPoolInternal(m_StackedPlatforms.Dequeue());

            m_CurrentLastDisplayedPlatform = m_StackedPlatforms.Count > 0
                ? m_StackedPlatforms.Peek()
                : null;
        }

        private void AddTotalPlayedPlatform(int add)
        {
            m_CurrentStackedPlatformsCount += add;
        }

        // -------------------------------------------------------------------------
        // Speed rate
        // -------------------------------------------------------------------------

        public void AddGlobalSpeedRate(float value) => AddGlobalSpeedRateInternal(value);

        private void AddGlobalSpeedRateInternal(float value)
        {
            m_MinGlobalSpeedRate += value;
            m_MaxGlobalSpeedRate += value;
            ClampSpeedRate();
        }

        private void SetMaxGlobalSpeedRateInternal(float value)
        {
            m_MaxGlobalSpeedRate = value;
            ClampSpeedRate();
        }

        private void SetMinGlobalSpeedRateInternal(float value)
        {
            m_MinGlobalSpeedRate = value;
            ClampSpeedRate();
        }

        private void ClampSpeedRate()
        {
            m_MinGlobalSpeedRate = Mathf.Min(m_MinGlobalSpeedRate, m_Config.SpeedRateLimit);
            m_MaxGlobalSpeedRate = Mathf.Min(m_MaxGlobalSpeedRate, m_Config.SpeedRateLimit);
        }

        private void SetGlobalPerfectTouchRange(float value)
        {
            m_GlobalPerfectTouchRange = value;
        }

        // -------------------------------------------------------------------------
        // Boost system
        // -------------------------------------------------------------------------

        public void ActivateBoost() => ActivateBoostInternal();

        private void ActivateBoostInternal()
        {
            PlatformBoostField boostField = m_Config.BoostField;
            if (boostField == null) return;
            if (m_IsBoostActive) return;
            if (m_CurrentBoostStock <= 0)
            {
                Debug.Log("[PlatformHandler] Boost stock habis.");
                return;
            }
            if (m_GlobalPerfectCount < boostField.BoostThreshold)
            {
                Debug.Log($"[PlatformHandler] Combo belum cukup. {m_GlobalPerfectCount}/{boostField.BoostThreshold}");
                return;
            }
            if (m_CurrentTouchedPlatform == null) return;

            int comboCount = m_GlobalPerfectCount > 0 ? m_GlobalPerfectCount : boostField.BoostThreshold;
            float duration = boostField.CalculateBoostDuration(comboCount);
            ConsumeBoostStockInternal(1);
            m_ActiveBoostDuration = duration;
            m_ActiveBoostElapsed = 0f;
            m_OnBoostStart?.Invoke(duration, comboCount);
            m_CurrentTouchedPlatform.Boost(boostField, duration, comboCount);

            RushPlayer.Instance.Jump.SetCanJump(false);
        }

        private void OnGlobalPerfectLanding(ISkillContext context)
        {
            if (m_Config.BoostField == null) return;
            if (m_IsBoostActive) return;

            m_GlobalPerfectCount++;
            m_OnPerfectCountChanged?.Invoke(m_GlobalPerfectCount);
            Debug.Log($"[Boost] GlobalPerfectCount: {m_GlobalPerfectCount}");

            if (m_GlobalPerfectCount >= m_Config.BoostField.BoostThreshold)
            {
                int overflow = m_GlobalPerfectCount - m_Config.BoostField.BoostThreshold;
                m_OnBoostEnabled?.Invoke(overflow);
                Debug.Log($"[Boost] Threshold tercapai, overflow: {overflow}");
            }
        }

        private void OnGlobalNormalLanding(ISkillContext context)
        {
            bool wasEnabled = m_Config.BoostField != null
                && m_GlobalPerfectCount >= m_Config.BoostField.BoostThreshold;
            ResetGlobalPerfectCount();
            if (wasEnabled)
                m_OnBoostDisabled?.Invoke();
            Debug.Log("[Boost] Normal landing - perfect count reset to 0");
        }

        private void ResetGlobalPerfectCount()
        {
            m_GlobalPerfectCount = 0;
            m_OnPerfectCountChanged?.Invoke(m_GlobalPerfectCount);
        }

        public void AddBoostStock(int amount) => AddBoostStockInternal(amount);
        public void ConsumeBoostStock(int amount) => ConsumeBoostStockInternal(amount);

        private void SetBoostStockInternal(int value)
        {
            int max = m_Config.BoostField?.MaxBoostStock ?? 0;
            m_CurrentBoostStock = Mathf.Clamp(value, 0, max);
            m_OnCurrentBoostStockChanged?.Invoke(m_CurrentBoostStock, max);
        }

        private void AddBoostStockInternal(int amount)
        {
            int max = m_Config.BoostField?.MaxBoostStock ?? 0;
            m_CurrentBoostStock = Mathf.Clamp(m_CurrentBoostStock + amount, 0, max);
            m_OnCurrentBoostStockChanged?.Invoke(m_CurrentBoostStock, max);
        }

        private void ConsumeBoostStockInternal(int amount)
        {
            int max = m_Config.BoostField?.MaxBoostStock ?? 0;
            m_CurrentBoostStock = Mathf.Clamp(m_CurrentBoostStock - amount, 0, max);
            m_OnCurrentBoostStockChanged?.Invoke(m_CurrentBoostStock, max);
        }

        // -------------------------------------------------------------------------
        // Helpers
        // -------------------------------------------------------------------------

        private PlatformConfig GetPreparedPlatformConfig(string id)
        {
            return m_PreparedPlatformConfigs.Find(config => config.BaseInfo.Id == id);
        }

        private PlatformConfig GetWaitingListPlatformConfigInternal(string id)
        {
            return m_WaitingListPlatformConfigs.Find(config => config.BaseInfo.Id == id);
        }

        private bool HasPreparedPlatformConfig(PlatformConfig config)
        {
            return GetPreparedPlatformConfig(config.BaseInfo.Id) != null;
        }

        private bool HasWaitingListPlatformConfig(PlatformConfig config)
        {
            return GetWaitingListPlatformConfigInternal(config.BaseInfo.Id) != null;
        }
        // === TAMBAHKAN FUNCTION INI DI DALAM CLASS (bebas taruh di mana, rekomendasi di bawah Pool) ===
        private void ReturnAllSpawnedPlatforms(bool includeSceneCheck = false)
        {
            // Hentikan spawning biar tidak race condition
            StopAllCoroutines();
            m_IsSpawningNextPlatform = false;

            // Return semua dari stack
            while (m_StackedPlatforms.Count > 0)
            {
                Platform2D platform = m_StackedPlatforms.Dequeue();
                ReturnToPoolInternal(platform);
            }

            // Return current new displayed
            if (m_CurrentNewDisplayedPlatform != null)
            {
                ReturnToPoolInternal(m_CurrentNewDisplayedPlatform);
                m_CurrentNewDisplayedPlatform = null;
            }

            // OPTIONAL: scan semua child (safety kalau ada yang nyangkut)
            if (includeSceneCheck)
            {
                foreach (Transform child in transform)
                {
                    Platform2D platform = child.GetComponent<Platform2D>();
                    if (platform != null && platform.gameObject.activeSelf)
                    {
                        ReturnToPoolInternal(platform);
                    }
                }
            }

            // Reset reference
            m_CurrentLastDisplayedPlatform = null;
            m_CurrentTouchedPlatform = null;

            Debug.Log("[PlatformHandler] All spawned platforms returned.");
        }
        public void ResetProgression()
        {
            if (m_Config == null) return;

            
            StopAllCoroutines();

            // ---------------------------------------------------------------------
            // Reset flags
            // ---------------------------------------------------------------------
            m_IsPaused = false;
            m_IsBoostActive = false;
            m_IsSpawningNextPlatform = false;

            // ---------------------------------------------------------------------
            // Reset counters
            // ---------------------------------------------------------------------
            m_GlobalPerfectCount = 0;
            m_CurrentStackedPlatformsCount = 0;
            m_ActiveBoostElapsed = 0f;
            m_ActiveBoostDuration = 0f;

            m_OnPerfectCountChanged?.Invoke(m_GlobalPerfectCount);

            // ---------------------------------------------------------------------
            // Reset boost stock
            // ---------------------------------------------------------------------
            if (m_Config.BoostField != null)
            {
                SetBoostStockInternal(m_Config.BoostField.MaxBoostStock);
            }

            // ---------------------------------------------------------------------
            // Reset speed
            // ---------------------------------------------------------------------
            SetMinGlobalSpeedRateInternal(m_Config.MinGlobalSpeedRate);
            SetMaxGlobalSpeedRateInternal(m_Config.MaxGlobalSpeedRate);

            ReturnAllSpawnedPlatforms(true);

            if (m_CurrentTouchedPlatform != null)
            {
                m_CurrentTouchedPlatform = null;
            }

            m_CurrentLastDisplayedPlatform = null;

            // ---------------------------------------------------------------------
            // Reset waiting list
            // ---------------------------------------------------------------------
            ClearWaitingListPlatformConfigInternal();
            InputToWaitingListByRandom();

            // ---------------------------------------------------------------------
            // Reset last contact point (spawn origin)
            // ---------------------------------------------------------------------
            

            // ---------------------------------------------------------------------
            // Optional: clear touch check state
            // ---------------------------------------------------------------------
            if (m_TouchDownCheckField != null)
            {
                m_TouchDownCheckField.ResetProgression();
            }
            Debug.Log("[PlatformHandler] Progression Reset Complete");
        }
    }
}