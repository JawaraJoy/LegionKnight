using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rush
{
    public class PlatformHandler : MonoBehaviour
    {
        [SerializeField]
        private Transform m_PlatformSpawnSpot;
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
            m_CurrentStackedPlatformsCount = 0;
            ClearPreparedPlatformConfigsInternal();
            ClearWaitingListPlatformConfigInternal();
            m_Config = config;
            // 
            AddPreparedPlatformsConfigInternal(config.InitialPlatformConfigs, gameObject);
            m_LastContactPoint = m_PlatformSpawnSpot.position;
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
        private void AddPreparedPlatformsConfigInternal(PlatformConfig[] configs, GameObject ownerObject)
        {
            for (int i = 0; i < configs.Length; i++)
            {
                AddPreparedPlatformConfigInternal(configs[i], ownerObject);
            }
        }
        
        private void AddPreparedPlatformConfigInternal(PlatformConfig config, GameObject ownerObject)
        {
            if (!HasPreparedPlatformConfig(config))
            {
                m_PreparedPlatformConfigs.Add(config);
                PreWarm(config, ownerObject);
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
        public void AddPreparedPlatformConfig(PlatformConfig config, GameObject ownerObject)
        {
            AddPreparedPlatformConfigInternal(config, ownerObject);
            
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
            SpawnNextPlatformFromWaitingList(m_Config.InitialSpawnDelay);
            //InputToWaitingListByRandom();
        }
        public void Pause()
        {
            m_IsPaused = true;
        }

        private void InputToWaitingListByRandom()
        {
            if (!IsWaitingListFull(out int possibleSlotCount))
            {
                List<PlatformConfig> waitingList = new(PlatformUtility.GetPlatformConfigWaitingListFromPreparationRandomly(m_PreparedPlatformConfigs.ToArray(), possibleSlotCount));
                AddWaitingListPlatformConfigsInternal(waitingList.ToArray());
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

        private void SpawnNextPlatformFromWaitingList(float delay)
        {
            if (m_WaitingListPlatformConfigs.Count == 0)
                return;

            StartCoroutine(SpawningNextPlatformFromWaitingList(delay));
        }
        private IEnumerator SpawningNextPlatformFromWaitingList(float delay)
        {
            yield return new WaitForSeconds(delay);
            PlatformConfig nextPlatform = m_WaitingListPlatformConfigs[0];
            Platform2D platform = GetFromPool(nextPlatform);
            platform.StartMove(PlatformUtility.GetStartingSpawnHorizontalPosition(m_Config.SpawnHorizontalDistanceFromPost, m_LastContactPoint));
            RemoveWaitingListPlatformConfigsInternal(nextPlatform);

            AddGlobalSpeedRateInternal(m_Config.SpeedRateGrowthDificulityEachStack);
        }

        private void PreWarm(PlatformConfig config, GameObject ownerObject)
        {
            string id = config.BaseInfo.Id;

            if (!m_Pools.ContainsKey(id))
            {
                m_Pools.Add(id, new Queue<Platform2D>());
            }

            for (int i = 0; i < config.PrewarmCount; i++)
            {
                Platform2D platform = CreateNewPlatform(config, ownerObject);
                m_Pools[id].Enqueue(platform);
            }
        }


        private Platform2D CreateNewPlatform(PlatformConfig config, GameObject ownerObject)
        {
            Platform2D newPlatform = Instantiate(config.PlatformPrefab, transform);
            newPlatform.Init(config, ownerObject);
            newPlatform.gameObject.SetActive(false);
            return newPlatform;
        }

        private Platform2D GetFromPool(PlatformConfig config, GameObject ownerObject = null)
        {
            if (!m_Pools.ContainsKey(config.BaseInfo.Id))
            {
                m_Pools.Add(config.BaseInfo.Id, new Queue<Platform2D>());
            }

            Queue<Platform2D> pool = m_Pools[config.BaseInfo.Id];

            Platform2D platform;

            if (pool.Count > 0)
            {
                platform = pool.Dequeue();
            }
            else
            {
                
                if (ownerObject != null)
                {
                    platform = CreateNewPlatform(config, ownerObject);
                }
                else
                {
                    platform = CreateNewPlatform(config, gameObject);
                }
            }

            platform.gameObject.SetActive(true);
            m_CurrentNewDisplayedPlatform = platform;

            platform.OnReachDestination.RemoveAllListeners();
            platform.OnReachDestination.AddListener(() => HandlePlatformReached(platform));
            
            return platform;
        }

        private void ReturnToPoolInternal(Platform2D platform)
        {
            string id = platform.Config.BaseInfo.Id;

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

            m_LastContactPoint = platform.Pivot.position;

            m_StackedPlatforms.Enqueue(platform);

            // platform paling atas (baru masuk)
            m_CurrentNewDisplayedPlatform = platform;

            LimitStackSize();

            SpawnNextPlatformFromWaitingList(m_Config.NextSpawnDelay);
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
