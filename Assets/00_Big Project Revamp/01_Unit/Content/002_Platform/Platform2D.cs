using MoreMountains.Tools;
using UnityEngine;

namespace Rush
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class Platform2D : MonoBehaviour, IUpdater
    {
        [SerializeField]
        private Transform m_TouchDownSpot;
        [SerializeField]
        private TouchDownCheckField m_TouchDownCheck;
        [SerializeField, MMReadOnly]
        private PlatformConfig m_Config;
        [SerializeField, MMReadOnly]
        private PlatformContext m_Context;
        public PlatformConfig Config => m_Config;
        public PlatformContext Context => m_Context;
        public Transform TouchDownSpot => m_TouchDownSpot;
        public TouchDownCheckField TouchDown => m_TouchDownCheck;

        public bool IsActive => gameObject.activeInHierarchy;

        public void Init(PlatformConfig config, Unit ownerUnit)
        {
            m_Config = config;
            m_Context = new PlatformContext(this, ownerUnit);
        }

        public void Tick()
        {
            float speed = m_Config.Speed * RushGameManager.Instance.PlatformManager.GlobalSpeedRate;
            PlatformAbilityTriggerDirection direction = PlatformUtility.GetPlatformDirection(transform.position, m_TouchDownSpot.position);
        }
    }
}
