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
        private Platform2D m_CurrentTouchedPlatform; // Platform terakhir yang player touchdown
        [SerializeField, MMReadOnly]
        private float m_ActiveBoostDuration;
        [SerializeField, MMReadOnly]
        private float m_ActiveBoostElapsed; // Platform terakhir yang player touchdown
        [SerializeField, MMReadOnly]
        private Platform2D m_CurrentLastDisplayedPlatform;
        [SerializeField, MMReadOnly]
        private Queue<Platform2D> m_StackedPlatforms = new();

        [SerializeField]
        private TouchDownCheckField m_TouchDownCheckField;

        [Header("Boost Events")]
        [SerializeField]
        private UnityEvent<float, int> m_OnBoostStart = new(); // (duration, perfectComboCount)
        [SerializeField]
        private UnityEvent m_OnBoostTick = new();
        [SerializeField]
        private UnityEvent m_OnBoostEnd = new();
        [SerializeField]
        private UnityEvent<float, float> m_OnBoostDurationTick = new(); // (remainingDuration, totalDuration)
        [SerializeField]
        private UnityEvent m_OnPrepare = new();
        [SerializeField]
        private UnityEvent<int> m_OnPerfectCountChanged = new(); // current combo count
        [SerializeField]
        private UnityEvent<int, int> m_OnCurrentBoostStockChanged = new(); // (currentStock, maxStock)
        [SerializeField]
        private UnityEvent<int> m_OnBoostEnabled = new(); // parameter: overflow (comboCount - threshold)
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
        private bool m_IsSpawningNextPlatform = false; // Flag to prevent multiple spawn requests
        [SerializeField, MMReadOnly]
        private bool m_IsBoostActive = false; // Flag: ada platform yang sedang boosting, tahan spawn berikutnya
        [SerializeField, MMReadOnly]
        private int m_GlobalPerfectCount = 0; // Akumulasi perfect landing lintas semua platform
        [SerializeField, MMReadOnly]
        private int m_CurrentBoostStock = 0; // Runtime stock, tidak disimpan di ScriptableObject

        private readonly Dictionary<string, Queue<Platform2D>> m_Pools = new Dictionary<string, Queue<Platform2D>>();

        public TouchDownCheckField TouchDownCheckField => m_TouchDownCheckField;
        public PlatformHandlerConfig Config => m_Config;

        // aku mau tiap platform yang diactivekan dihitung
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

        /// <summary>
        /// Dipanggil oleh PlatformBooster untuk memberi tahu handler
        /// bahwa ada / tidak ada platform yang sedang boost.
        /// Selama boost aktif, spawn platform berikutnya ditahan.
        /// </summary>
        public void SetCurrentTouchedPlatform(Platform2D platform)
        {
            SetCurrentTouchedPlatformInternal(platform);
        }

        private void SetCurrentTouchedPlatformInternal(Platform2D platform)
        {
            m_CurrentTouchedPlatform = platform;
        }

        public void SetIsBoostActive(bool value)
        {
            m_IsBoostActive = value;
            if (!value)
                ResetGlobalPerfectCount();
        }
        public float MinGlobalSpeedRate => m_MinGlobalSpeedRate;
        public float MaxGlobalSpeedRate => m_MaxGlobalSpeedRate;
        public float GlobalPerfectTouchRange => m_GlobalPerfectTouchRange;
        public int TotalPlayedPlatforms => m_CurrentStackedPlatformsCount;

        public void Prepare(PlatformHandlerConfig config)
        {
            if (config == null) return;
            if (config == m_Config) return;
            m_CurrentStackedPlatformsCount = 0;
            ClearPreparedPlatformConfigsInternal();
            ClearWaitingListPlatformConfigInternal();
            m_Config = config;

            // platform asli dari sini saya anggap punya player
            Unit unitPlayer = RushPlayer.Instance.Unit;
            if (unitPlayer.HasBind(out PlatformController controller))
            {
                AddPreparedPlatformsConfigInternal(config.InitialPlatformConfigs, controller);
                if (unitPlayer.Config is IHasPlatform owner)
                {
                    AddPreparedPlatformsConfigInternal(owner.UniquePlatforms, controller);
                }
            }
            m_LastContactPoint = RushPlayer.Instance.PlatformSpawnPost.position;
            SetMaxGlobalSpeedRateInternal(config.MaxGlobalSpeedRate);
            SetMinGlobalSpeedRateInternal(config.MinGlobalSpeedRate);
            SetGlobalPerfectTouchRange(config.GlobalPerfectTouchRange);

            InputToWaitingListByRandom();

            // Isi current boost stock sesuai max saat prepare
            if (config.BoostField != null)
                SetBoostStockInternal(config.BoostField.MaxBoostStock);

            // Subscribe global perfect landing event
            // m_TouchDownCheckField adalah field handler level yang di-invoke dari TouchDownCheck
            // setiap kali player landing di platform manapun
            m_TouchDownCheckField.RegisterPerfectLandingCallback(OnGlobalPerfectLanding);
            m_TouchDownCheckField.RegisterNormalLandingCallback(OnGlobalNormalLanding);

            m_OnPrepare?.Invoke();
        }
        private void AddTotalPlayedPlatform(int add)
        {
            m_CurrentStackedPlatformsCount += add;
        }
        public void SetLastContactPoint(Vector2 point)
        {
            m_LastContactPoint = point;
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
        private void SetGlobalPerfectTouchRange(float value)
        {
            m_GlobalPerfectTouchRange = value;
        }
        private void AddGlobalSpeedRateInternal(float value)
        {
            m_MinGlobalSpeedRate += value;
            m_MaxGlobalSpeedRate += value;
            ClampSpeedRate();
        }
        public void AddGlobalSpeedRate(float value)
        {
            AddGlobalSpeedRateInternal(value);
        }

        private void ClampSpeedRate()
        {
            m_MinGlobalSpeedRate = Mathf.Min(m_MinGlobalSpeedRate, m_Config.SpeedRateLimit);
            m_MaxGlobalSpeedRate = Mathf.Min(m_MaxGlobalSpeedRate, m_Config.SpeedRateLimit);
        }

        private PlatformConfig GetPreparedPlatformConfig(string id)
        {
            return m_PreparedPlatformConfigs.Find(config => config.BaseInfo.Id == id);
        }
        private PlatformConfig GetWaitingListPlatformConfigInternal(string id)
        {
            return m_WaitingListPlatformConfigs.Find(Config => Config.BaseInfo.Id == id);
        }
        private bool HasPreparedPlatformConfig(string id, out PlatformConfig config)
        {
            config = GetPreparedPlatformConfig(id);
            return config != null;
        }
        private bool HasWaitingListPlatformConfig(string id, out PlatformConfig config)
        {
            config = GetWaitingListPlatformConfigInternal(id);
            return config != null;
        }
        private bool HasPreparedPlatformConfig(PlatformConfig config)
        {
            return GetPreparedPlatformConfig(config.BaseInfo.Id) != null;
        }
        private bool HasWaitingListPlatformConfig(PlatformConfig config)
        {
            return GetWaitingListPlatformConfigInternal(config.BaseInfo.Id) != null;
        }
        public void AddPreparedPlatformConfigs(PlatformConfig[] configs, PlatformController controller)
        {
            AddPreparedPlatformsConfigInternal(configs, controller);
        }
        private void AddPreparedPlatformsConfigInternal(PlatformConfig[] configs, PlatformController controller)
        {
            int length = configs.Length;
            if (length == 0) return;
            for (int i = 0; i < configs.Length; i++)
            {
                AddPreparedPlatformConfigInternal(configs[i], controller);
            }
        }

        private void AddPreparedPlatformConfigInternal(PlatformConfig config, PlatformController controller)
        {
            if (!HasPreparedPlatformConfig(config))
            {
                m_PreparedPlatformConfigs.Add(config);
                PreWarm(config, controller);
            }
            else
            {
                string id = config.BaseInfo.Id;

                if (!m_Pools.ContainsKey(id))
                {
                    m_Pools.Add(id, new Queue<Platform2D>());
                }

                Platform2D platform = CreateNewPlatform(config, controller);
                m_Pools[id].Enqueue(platform);
                // tambah jumlah prewarmnya 1 saja
            }
        }
        private void RemovePreparedPlatformConfigInternal(PlatformConfig config)
        {
            if (HasPreparedPlatformConfig(config))
            {
                m_PreparedPlatformConfigs.Remove(config);
            }
        }
        private void AddWaitingListPlatformConfigsInternal(PlatformConfig[] configs)
        {
            for (int i = 0; i < configs.Length; i++)
            {
                AddWaitingListPlatformConfigInternal(configs[i]);
            }
        }
        private void AddWaitingListPlatformConfigInternal(PlatformConfig config)
        {
            if (!HasWaitingListPlatformConfig(config))
            {
                m_WaitingListPlatformConfigs.Add(config);
            }
        }
        private void RemoveWaitingListPlatformConfigsInternal(PlatformConfig config)
        {
            if (HasWaitingListPlatformConfig(config))
            {
                m_WaitingListPlatformConfigs.Remove(config);
                if (m_PreparedPlatformConfigs.Count > 0)
                {
                    InputToWaitingListByRandom();
                }
            }
        }
        private void ClearPreparedPlatformConfigsInternal()
        {
            m_PreparedPlatformConfigs.Clear();
        }
        private void ClearWaitingListPlatformConfigInternal()
        {
            m_WaitingListPlatformConfigs.Clear();
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

        public void Play()
        {
            m_IsPaused = false;
            if (m_Config == null) return;

            //InputToWaitingListByRandom();
            SpawnNextPlatformFromWaitingListInternal(m_Config.InitialSpawnDelay);
        }
        public void Pause()
        {
            m_IsPaused = true;
        }
        public void Resume()
        {
            m_IsPaused = false;
        }
        private bool HasAvailableInstance(PlatformConfig config)
        {
            if (!m_Pools.ContainsKey(config.BaseInfo.Id))
                return false;

            return m_Pools[config.BaseInfo.Id].Count > 0;
        }
        private PlatformConfig GetWeightedRandomAvailablePlatformConfig(List<PlatformConfig> configs)
        {
            List<PlatformConfig> availableConfigs = new();

            foreach (var config in configs)
            {
                if (HasAvailableInstance(config))
                {
                    availableConfigs.Add(config);
                }
            }

            if (availableConfigs.Count == 0)
                return null;

            int totalWeight = 0;
            foreach (var config in availableConfigs)
            {
                totalWeight += Mathf.Max(1, config.PrewarmCount);
            }

            int randomValue = Random.Range(0, totalWeight);
            int cumulative = 0;

            foreach (var config in availableConfigs)
            {
                cumulative += Mathf.Max(1, config.PrewarmCount);

                if (randomValue < cumulative)
                    return config;
            }

            return availableConfigs[0];
        }
        private void InputToWaitingListByRandom()
        {
            if (IsWaitingListFull(out _))
                return;

            foreach (var config in m_PreparedPlatformConfigs)
            {
                if (!HasWaitingListPlatformConfig(config))
                {
                    m_WaitingListPlatformConfigs.Add(config);
                }
            }
        }
        private bool IsWaitingListFull(out int possibleSlotCount)
        {
            int maxSlot = m_Config.MaxStackedPlatforms;
            bool isFull = m_WaitingListPlatformConfigs.Count >= maxSlot;

            possibleSlotCount = 0;
            if (!isFull)
            {
                possibleSlotCount = maxSlot - m_WaitingListPlatformConfigs.Count;
            }
            return isFull;
        }
        public void SpawnNextPlatformFromWaitingList(float delay)
        {
            SpawnNextPlatformFromWaitingListInternal(delay);
        }
        private void SpawnNextPlatformFromWaitingListInternal(float delay)
        {
            // Tahan spawn jika ada platform yang sedang boosting
            if (m_IsBoostActive)
                return;

            if (m_WaitingListPlatformConfigs.Count == 0 || m_IsSpawningNextPlatform)
                return;

            m_IsSpawningNextPlatform = true; // Set flag to true to prevent multiple spawns

            StartCoroutine(SpawningNextPlatformFromWaitingList(delay));
        }

        private IEnumerator SpawningNextPlatformFromWaitingList(float delay)
        {
            yield return new WaitForSeconds(delay);

            PlatformConfig nextPlatform = PlatformUtility.GetRandomPlatformConfig(m_WaitingListPlatformConfigs, HasAvailableInstance);

            if (nextPlatform == null)
            {
                Debug.LogWarning("No available platform in pool!");
                m_IsSpawningNextPlatform = false;
                yield break;
            }
            Platform2D platform = GetFromPool(nextPlatform);
            platform.StartMove(PlatformUtility.GetStartingSpawnHorizontalPosition(m_Config.SpawnHorizontalDistanceFromPost, m_LastContactPoint));
            RemoveWaitingListPlatformConfigsInternal(nextPlatform);

            AddGlobalSpeedRateInternal(m_Config.SpeedRateGrowthDificulityEachStack);

            // After the spawn is completed, reset the flag to allow the next spawn
            m_IsSpawningNextPlatform = false;
        }

        private void PreWarm(PlatformConfig config, PlatformController controller)
        {
            string id = config.BaseInfo.Id;

            if (!m_Pools.ContainsKey(id))
            {
                m_Pools.Add(id, new Queue<Platform2D>());
            }

            for (int i = 0; i < config.PrewarmCount; i++)
            {
                Platform2D platform = CreateNewPlatform(config, controller);
                m_Pools[id].Enqueue(platform);
            }
        }


        private Platform2D CreateNewPlatform(PlatformConfig config, PlatformController controller)
        {
            Debug.Log($"Controller null? {controller == null}");
            Debug.Log($"ModuleContext null? {controller.ModuleContext == null}");

            Platform2D newPlatform = Instantiate(config.PlatformPrefab, transform);
            newPlatform.IniPlatform(config);
            if (controller != null)
            {
                newPlatform.Init(config.AttackSkill, controller.ModuleContext);
            }

            newPlatform.gameObject.SetActive(false);
            return newPlatform;
        }

        private Platform2D GetFromPool(PlatformConfig config)
        {
            if (!m_Pools.ContainsKey(config.BaseInfo.Id))
            {
                m_Pools.Add(config.BaseInfo.Id, new Queue<Platform2D>());
            }

            Queue<Platform2D> pool = m_Pools[config.BaseInfo.Id];

            Platform2D platform;

            if (pool.Count == 0)
            {
                return null;
            }

            platform = pool.Dequeue();

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
            platform.OnBoostEnd.AddListener(() => m_OnBoostEnd?.Invoke());

            return platform;
        }

        private void ReturnToPoolInternal(Platform2D platform)
        {
            string id = platform.PlatformConfig.BaseInfo.Id;

            if (!m_Pools.ContainsKey(id))
            {
                m_Pools.Add(id, new Queue<Platform2D>());
            }

            // Bersihkan boost check listener agar tidak menumpuk saat platform di-reuse
            platform.TouchDownCheck.ClearBoostCheck();

            platform.gameObject.SetActive(false);

            m_Pools[id].Enqueue(platform);
        }
        public void ReturnToPool(Platform2D platform)
        {
            ReturnToPoolInternal(platform);
        }

        private void HandlePlatformReached(Platform2D platform)
        {
            AddTotalPlayedPlatform(1);

            m_LastContactPoint = platform.TouchDownSpot.position;

            m_StackedPlatforms.Enqueue(platform);

            // platform paling atas (baru masuk)
            m_CurrentNewDisplayedPlatform = platform;

            LimitStackSize();

            SpawnNextPlatformFromWaitingListInternal(m_Config.NextSpawnDelay);
            Debug.Log($"Reached: {platform.PlatformConfig.BaseInfo.Name} | Count before: {m_StackedPlatforms.Count}");
        }

        /// <summary>
        /// Dipanggil dari TouchDownCheckField (global handler level) setiap kali ada perfect landing.
        /// Mengakumulasi count lintas semua platform dan trigger boost jika threshold tercapai.
        /// </summary>
        private void OnGlobalPerfectLanding(ISkillContext context)
        {
            if (m_Config.BoostField == null) return;
            if (m_IsBoostActive) return;

            m_GlobalPerfectCount++;
            m_OnPerfectCountChanged?.Invoke(m_GlobalPerfectCount);
            Debug.Log($"[Boost] GlobalPerfectCount: {m_GlobalPerfectCount}");

            // Jika threshold tercapai, isi stock dan invoke OnBoostEnabled dengan overflow
            if (m_GlobalPerfectCount >= m_Config.BoostField.BoostThreshold)
            {
                int overflow = m_GlobalPerfectCount - m_Config.BoostField.BoostThreshold;
                m_OnBoostEnabled?.Invoke(overflow);
                Debug.Log($"[Boost] Threshold tercapai, overflow: {overflow}");
            }
        }

        /// <summary>
        /// Aktifkan boost secara manual dari luar.
        /// Menggunakan 1 stock dan menghitung durasi dari GlobalPerfectCount saat ini.
        /// </summary>
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
        }

        public void ActivateBoost()
        {
            ActivateBoostInternal();
        }

        /// <summary>
        /// Reset perfect count saat ada normal landing — boost harus dicapai dengan perfect BERUNTUN.
        /// </summary>
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
        private void LimitStackSize()
        {
            int maxStack = m_Config.MaxStackedPlatforms;

            if (m_StackedPlatforms.Count > maxStack)
            {
                Platform2D bottomPlatform = m_StackedPlatforms.Dequeue();

                ReturnToPoolInternal(bottomPlatform);
            }
            UpdateDisplayedPlatformReferences();
        }
        private void UpdateDisplayedPlatformReferences()
        {
            if (m_StackedPlatforms.Count == 0)
            {
                m_CurrentLastDisplayedPlatform = null;
                return;
            }

            m_CurrentLastDisplayedPlatform = m_StackedPlatforms.Peek();
        }

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

        public void AddBoostStock(int amount)
        {
            AddBoostStockInternal(amount);
        }

        public void ConsumeBoostStock(int amount)
        {
            ConsumeBoostStockInternal(amount);
        }

        public void ResetProgression()
        {
            //throw new System.NotImplementedException();
        }
    }
}