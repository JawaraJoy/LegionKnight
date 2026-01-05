using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.Events;

namespace Rush
{
    public partial class Unit : MonoBehaviour
    {
        [SerializeField]
        private UnitConfig m_Config;
        public UnitConfig Config => m_Config;

        [SerializeField]
        private UnityEvent<CharacterContext> m_OnInit;

        public void Init(UnitConfig config)
        {
            m_Config = config;
            var context = new CharacterContext(m_Config, this);
            m_OnInit?.Invoke(context);
        }
    }
}
