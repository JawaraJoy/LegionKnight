using MoreMountains.Tools;
using UnityEngine;

namespace Rush
{
    public class UnitExtention_StatModifier{   }

    public partial class Unit
    {
        [SerializeField, MMReadOnly]
        private StatModifier m_StatModifier;
        public StatModifier StatModifier => m_StatModifier;
    }
}
