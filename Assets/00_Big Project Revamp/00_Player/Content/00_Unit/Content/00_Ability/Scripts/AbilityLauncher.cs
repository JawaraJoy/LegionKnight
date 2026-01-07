using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.Events;

namespace Rush
{
    public partial class AbilityLauncher : MonoBehaviour
    {
        [SerializeField, MMReadOnly]
        private SkillConfig m_Config;
        [SerializeField]
        private UnityEvent<SkillConfig> m_OnInit;
        [SerializeField]
        private UnityEvent<SkillConfig> m_OnLaunch;

        public void Init(SkillConfig config)
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
