using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.Events;

namespace Rush
{
    public class AbilityActivator : MonoBehaviour
    {
        [SerializeField]
        private AbilityPurpose m_Purpose = AbilityPurpose.Damaging;
        [SerializeField, MMReadOnly]
        private AbilityContext m_Context;
        [SerializeField]
        private UnityEvent<AbilityContext> m_OnInit;
        [SerializeField]
        private UnityEvent<AbilityContext> m_OnActivate;
        public void Init(AbilityContext context)
        {
            m_Context = context;
            m_OnInit?.Invoke(m_Context);
            Debug.Log("Ability Initialized: " + m_Context.Config.BaseInfo.Name);
        }
        public void Activate()
        {
            Debug.Log("Ability Activated: " + m_Context.Config.BaseInfo.Name);
            m_OnActivate?.Invoke(m_Context);
        }
    }
}
