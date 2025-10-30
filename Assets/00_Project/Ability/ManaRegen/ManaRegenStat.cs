using UnityEngine;

namespace LegionKnight
{
    [System.Serializable]
    public class ManaRegenStat
    {
        [SerializeField]
        private int m_ManaRegenFlat;
        [SerializeField]
        private int m_RegenDuration;
        public int ManaRegenFlat => m_ManaRegenFlat;
        public int RegenDuration => m_RegenDuration;
    }

    public partial class AbilityDefinition
    {
        [SerializeField]
        private ManaRegenStat m_ManaRegenStat;
        public ManaRegenStat ManaRegenStat => m_ManaRegenStat;
    }
}
