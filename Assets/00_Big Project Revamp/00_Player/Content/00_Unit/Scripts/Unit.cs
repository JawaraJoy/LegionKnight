using UnityEngine;
using UnityEngine.Events;

namespace Rush
{
    public partial class Unit : Bindable
    {
        [SerializeField]
        private UnitConfig m_Config;

        [SerializeField] // change tp progression monobehaviour later
        private ProgressField m_Progression;
        // add Stats Modifier MonoBehaviour later
        public UnitConfig Config => m_Config;
        public ProgressField Progression => m_Progression;
        [SerializeField]
        private UnityEvent<Unit> m_OnInit;
        public void Init(UnitConfig config)
        {
            m_Config = config;
            m_OnInit?.Invoke(this);
        }
        
    }
}
