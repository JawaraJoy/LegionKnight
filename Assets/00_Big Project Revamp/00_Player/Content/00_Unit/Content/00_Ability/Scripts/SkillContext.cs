using MoreMountains.Tools;
using System.Collections.Generic;
using UnityEngine;

namespace Rush
{
    [System.Serializable]
    public partial class SkillContext : ProgressField
    {
        [SerializeField, MMReadOnly]
        private SkillConfig m_Config;

        [SerializeField, MMReadOnly]
        private Unit m_Owner;
        [SerializeField]
        private SkillActivator m_ActivatorSpawned;
        public SkillConfig Config => m_Config;
        public Unit Owner => m_Owner;

        public SkillContext(SkillConfig config, Unit owner)
        {
            m_Config = config;
            m_Owner = owner;
        }
    }
}
