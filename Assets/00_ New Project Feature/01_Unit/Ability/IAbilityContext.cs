using MoreMountains.Tools;
using System.Collections.Generic;
using UnityEngine;

namespace Rush
{
    public partial interface IAbilityContext
    {
        AbilityConfig Config { get; }
        Character Owner { get; }
        List<GameObject> Targets { get; }
    }
    [System.Serializable]
    public partial class AbilityContext : ProgressField, IAbilityContext
    {
        [SerializeField, MMReadOnly]
        private AbilityConfig m_Config;
        [SerializeField, MMReadOnly]
        private Character m_Owner;
        [SerializeField, MMReadOnly]
        private List<GameObject> m_Targets = new();
        public AbilityConfig Config => m_Config;
        public Character Owner => m_Owner;
        public List<GameObject> Targets => m_Targets;
        public AbilityContext(AbilityConfig config, Character owner)
        {
            m_Config = config;
            m_Owner = owner;
        }
        public void Retarget(List<GameObject> newTargets)
        {
            m_Targets = newTargets;
        }
    }
}
