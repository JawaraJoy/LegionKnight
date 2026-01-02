using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.Events;

namespace Rush
{
    public partial class AbilityLauncher : MonoBehaviour
    {
        [SerializeField, MMReadOnly]
        private AbilityConfig m_Config;
        [SerializeField]
        private UnityEvent<AbilityConfig> m_OnInit;
        [SerializeField]
        private UnityEvent<AbilityConfig> m_OnLaunch;

        public void Init(AbilityConfig config)
        {
            m_Config = config;
            m_OnInit?.Invoke(m_Config);
        }
        public void Launch()
        {
            m_OnLaunch?.Invoke(m_Config);
        }
    }
}
