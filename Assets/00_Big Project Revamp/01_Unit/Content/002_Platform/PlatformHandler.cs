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
        private List<PlatformConfig> m_PreparedPlatformConfig = new();
        [SerializeField]
        private List<PlatformConfig> m_WaitingListPlatformConfig = new();
        [SerializeField, MMReadOnly]
        private List<Platform2D> m_ActivePlatforms = new();
        [SerializeField, MMReadOnly]
        private Queue<Platform2D> m_PlatformPool = new();
        public PlatformHandlerConfig Config => m_Config;
        [SerializeField, MMReadOnly]
        private float m_GlobalSpeedRate = 1f;
        [SerializeField, MMReadOnly]
        private float m_GlobalPerfectTouchRange = 0.3f;
        [SerializeField, MMReadOnly]
        private Vector2 m_LastContactPoint = Vector2.zero;
        public float GlobalSpeedRate => m_GlobalSpeedRate;
        public float GlobalPerfectTouchRange => m_GlobalPerfectTouchRange;
        public void Prepare(PlatformHandlerConfig config)
        {
            ClearPreparedPlatformConfigsInternal();
            m_Config = config;
            // 
            m_LastContactPoint = m_PlatformSpawnSpot.position;
            SetGlobalSpeedRate(config.GlobalSpeedRate);
            SetGlobalPerfectTouchRange(config.GlobalPerfectTouchRange);
        }
        public void SetLastContactPoint(Vector2 point)
        {
            m_LastContactPoint = point;
        }
        private void SetGlobalSpeedRate(float value)
        {
            m_GlobalSpeedRate = value;
        }
        private void SetGlobalPerfectTouchRange(float value)
        {
            m_GlobalPerfectTouchRange = value;
        }

        private PlatformConfig GetPreparedPlatformConfig(string id)
        {
            return m_PreparedPlatformConfig.Find(config => config.BaseInfo.Id == id);
        }
        private bool HasPreparedPlatformConfig(string id, out PlatformConfig config)
        {
            config = GetPreparedPlatformConfig(id);
            return config != null;
        }
        private bool HasPreparedPlatformConfig(PlatformConfig config)
        {
            return GetPreparedPlatformConfig(config.BaseInfo.Id) != null;
        }
        private void AddPreparedPlatformConfigInternal(PlatformConfig config)
        {
            if (!HasPreparedPlatformConfig(config))
            {
                m_PreparedPlatformConfig.Add(config);
            }
        }
        private void RemovePreparedPlatformConfigInternal(PlatformConfig config)
        {
            if (HasPreparedPlatformConfig(config))
            {
                m_PreparedPlatformConfig.Remove(config);
            }
        }
        private void ClearPreparedPlatformConfigsInternal()
        {
            m_PreparedPlatformConfig.Clear();
        }
        public void AddPreparedPlatformConfig(PlatformConfig config)
        {
            AddPreparedPlatformConfigInternal(config);
        }
        public void RemovePreparedPlatformConfig(PlatformConfig config)
        {
            RemovePreparedPlatformConfigInternal(config);
        }
        public void ClearPreparedPlatformConfigs()
        {
            ClearPreparedPlatformConfigsInternal();
        }
    }
}
