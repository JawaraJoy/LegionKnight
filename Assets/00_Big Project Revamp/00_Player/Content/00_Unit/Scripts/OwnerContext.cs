using MoreMountains.Tools;
using UnityEngine;

namespace Rush
{
    [System.Serializable]
    public partial class OwnerContext
    {
        [SerializeField, MMReadOnly]
        private UnitConfig m_UnitConfig;
        [SerializeField, MMReadOnly]
        private Unit m_UnitObject;
        public UnitConfig UnitConfig => m_UnitConfig;
        public Unit UnitObject => m_UnitObject;
        public OwnerContext(UnitConfig unitConfig, Unit unitObject)
        {
            m_UnitConfig = unitConfig;
            m_UnitObject = unitObject;
        }
    }
}
