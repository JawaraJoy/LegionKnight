using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rush
{
    public class PlatformHandler : MonoBehaviour
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
        private Platform2D m_CurrentLastDisplayedPlatform;
        [SerializeField, MMReadOnly]
        private Queue<Platform2D> m_StackedPlatforms = new();

        [SerializeField]
        private TouchDownCheckField m_TouchDownCheckField;
        [SerializeField, MMReadOnly]
        private bool m_IsSpawningNextPlatform = false; // Flag to prevent multiple spawn requests

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
        public Vector2 LastContactPoint => m_LastContactPoint;
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
            }
            m_LastContactPoint = RushPlayer.Instance.PlatformSpawnPost.position;
            SetMaxGlobalSpeedRateInternal(config.MaxGlobalSpeedRate);
            SetMinGlobalSpeedRateInternal(config.MinGlobalSpeedRate);
            SetGlobalPerfectTouchRange(config.GlobalPerfectTouchRange);

            InputToWaitingListByRandom();
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
            
            return platform;
        }

        private void ReturnToPoolInternal(Platform2D platform)
        {
            string id = platform.PlatformConfig.BaseInfo.Id;

            if (!m_Pools.ContainsKey(id))
            {
                m_Pools.Add(id, new Queue<Platform2D>());
            }

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
    }
}
