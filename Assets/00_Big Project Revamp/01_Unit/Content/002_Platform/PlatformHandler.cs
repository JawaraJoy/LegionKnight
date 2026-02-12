using MoreMountains.Tools;
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
        private Platform2D m_CurrentNewDisplayedPlatformConfig;
        [SerializeField, MMReadOnly]
        private Platform2D m_CurrentLastDisplayedPlatform;

        [SerializeField, MMReadOnly]
        private List<Platform2D> m_ActivePlatforms = new();
        [SerializeField, MMReadOnly]
        private Queue<Platform2D> m_StackedPlatforms = new();

        private readonly Dictionary<string, Queue<Platform2D>> m_Pools = new Dictionary<string, Queue<Platform2D>>();

        public PlatformHandlerConfig Config => m_Config;

        // aku mau tiap platform yang diactivekan dihitung
        [SerializeField, MMReadOnly]
        private int m_TotalPlayedPlatforms;
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
        public void Prepare(PlatformHandlerConfig config)
        {
            m_TotalPlayedPlatforms = 0;
            ClearPreparedPlatformConfigsInternal();
            ClearWaitingListPlatformConfigInternal();
            m_Config = config;
            // 
            AddPreparedPlatformsConfigInternal(config.InitialPlatformConfigs, gameObject);
            m_LastContactPoint = m_PlatformSpawnSpot.position;
            SetGlobalSpeedRate(config.MaxGlobalSpeedRate);
            SetGlobalPerfectTouchRange(config.GlobalPerfectTouchRange);

            InputToWaitingListByRandom();
        }
        private void AddTotalPlayedPlatform(int add)
        {
            m_TotalPlayedPlatforms += add;
        }
        public void SetLastContactPoint(Vector2 point)
        {
            m_LastContactPoint = point;
        }
        private void SetGlobalSpeedRate(float value)
        {
            m_MaxGlobalSpeedRate = value;
        }
        private void SetGlobalPerfectTouchRange(float value)
        {
            m_GlobalPerfectTouchRange = value;
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
                AddPreparedPlatformConfigInternal((PlatformConfig)configs[i], ownerObject);
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
                AddWaitingListPlatformConfigInternal((PlatformConfig)configs[i]);
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
            PerformNextPlatformFromWaitingList();
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

        private void PerformNextPlatformFromWaitingList()
        {
            if (m_WaitingListPlatformConfigs.Count == 0)
                return;

            PlatformConfig nextPlatform = m_WaitingListPlatformConfigs[0];
            Platform2D platform = GetFromPool(nextPlatform);
            platform.StartMove(PlatformUtility.GetStartingSpawnHorizontalPosition(m_Config.SpawnHorizontalDistanceFromPost, m_LastContactPoint));
            RemoveWaitingListPlatformConfigsInternal(nextPlatform);
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
                Platform2D platform = CreateNewPlatform(config);
                platform.Init(config, ownerObject);
                platform.gameObject.SetActive(false);
                m_Pools[id].Enqueue(platform);
            }
        }


        private Platform2D CreateNewPlatform(PlatformConfig config)
        {
            Platform2D newPlatform = Instantiate(config.PlatformPrefab, transform);
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

            if (pool.Count > 0)
            {
                platform = pool.Dequeue();
            }
            else
            {
                platform = CreateNewPlatform(config);
            }

            platform.gameObject.SetActive(true);

            // Set CurrentNewPlayedPlatform
            // Set CurrentLastPlayedPlatform from the bottom of stacked platform

            platform.OnReachDestination.RemoveAllListeners();
            platform.OnReachDestination.AddListener(() => HandlePlatformReached(platform));

            return platform;
        }

        private void ReturnToPool(Platform2D platform)
        {
            string id = platform.Config.BaseInfo.Id;

            if (!m_Pools.ContainsKey(id))
            {
                m_Pools.Add(id, new Queue<Platform2D>());
            }

            platform.gameObject.SetActive(false);

            m_ActivePlatforms.Remove(platform);

            m_Pools[id].Enqueue(platform);
        }

        private void HandlePlatformReached(Platform2D platform)
        {
            AddTotalPlayedPlatform(1);

            m_LastContactPoint = platform.Pivot.position;

            m_StackedPlatforms.Enqueue(platform);

            LimitStackSize();

            PerformNextPlatformFromWaitingList();
        }
        private void LimitStackSize()
        {
            int maxStack = m_Config.MaxStackedPlatforms;

            if (m_StackedPlatforms.Count > maxStack)
            {
                Platform2D bottomPlatform = m_StackedPlatforms.Dequeue();

                ReturnToPool(bottomPlatform);
            }
        }

    }
}
