using LegionKnight;
using MoreMountains.Tools;
using UnityEngine;

namespace Rush
{
    public partial class PlatformUnit : Unit
    {
        [SerializeField, MMReadOnly]
        private Unit m_UnitOwner;
        public Unit UnitOwner => m_UnitOwner;
        public void SetUnitOwner(Unit owner)
        {
            m_UnitOwner = owner;
        }
    }

    public partial class Platform
    {
        [SerializeField, MMReadOnly]
        private Unit m_UnitOwner;
        public Unit UnitOwner => m_UnitOwner;
        public void SetUnitOwner(Unit owner)
        {
            m_UnitOwner = owner;
        }
    }
}
