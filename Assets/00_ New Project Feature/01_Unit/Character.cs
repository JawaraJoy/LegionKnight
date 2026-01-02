using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.Events;

namespace Rush
{
    public partial class Character : MonoBehaviour
    {
        [SerializeField]
        private CharacterConfig m_Config;
        public CharacterConfig Config => m_Config;

        [SerializeField]
        private UnityEvent<CharacterContext> m_OnInit;

        public void Init(CharacterConfig config)
        {
            m_Config = config;
            var context = new CharacterContext(m_Config, gameObject);
            m_OnInit?.Invoke(context);
        }
    }
}
