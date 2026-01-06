using MoreMountains.Tools;
using System.Collections.Generic;
using UnityEngine;

namespace Rush
{
    public partial interface IAbilityContext
    {
        AbilityConfig Config { get; }
        Unit Owner { get; }
    }
    [System.Serializable]
    public partial class AbilityContext : ProgressField, IAbilityContext
    {
        [SerializeField]
        private bool m_IsActive = false;
        [SerializeField, MMReadOnly]
        private AbilityConfig m_Config;

        [SerializeField, MMReadOnly]
        private Unit m_Owner;
        [SerializeField]
        private List<AbilityActivator> m_ActivatorInstancePool = new();
        public AbilityConfig Config => m_Config;
        public Unit Owner => m_Owner;
        public bool IsActive => m_IsActive;

        public AbilityContext(AbilityConfig config, Unit owner)
        {
            m_Config = config;
            m_Owner = owner;
        }
        private void AddActivatorInstanceToPool(AbilityActivator instance)
        {
            if (!m_ActivatorInstancePool.Contains(instance))
            {
                m_ActivatorInstancePool.Add(instance);
            }
        }
        private void RemoveActivatorInstanceFromPool(AbilityActivator instance)
        {
            if (m_ActivatorInstancePool.Contains(instance))
            {
                m_ActivatorInstancePool.Remove(instance);
            }
        }
        public void ActivateAllActivators()
        {
            if (!m_IsActive) return;
            foreach (AbilityActivator activator in m_ActivatorInstancePool)
            {
                activator.Activate();
            }
        }
    }
}
