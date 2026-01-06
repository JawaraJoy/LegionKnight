using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.Events;

namespace Rush
{
    public partial class Unit : MonoBehaviour
    {
        [SerializeField]
        private UnitConfig m_Config;
        [SerializeField]
        private ProgressField m_Progresstion;
        public UnitConfig Config => m_Config;
        public ProgressField Progression => m_Progresstion;

        [SerializeField]
        private UnityEvent<UnitContext> m_OnInit;

        public void Init(UnitConfig config)
        {
            m_Config = config;
            var context = new UnitContext(m_Config, this);
            m_OnInit?.Invoke(context);
        }
    }
}
