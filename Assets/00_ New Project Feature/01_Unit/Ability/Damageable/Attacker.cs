using LegionKnight;
using UnityEngine;

namespace Rush
{
    public class Attacker : MonoBehaviour, IAbility
    {
        [SerializeField]
        private int m_Attack = 10;
        [SerializeField]
        private ScaningType m_ScaningType = ScaningType.Nearest;
        public int Attack => m_Attack;

        private AbilityContext m_AbilityContext;

        public void Init(AbilityContext context)
        {
            m_AbilityContext = context;
            StatField stats = m_AbilityContext.GetFinalStat();
            m_Attack = Mathf.RoundToInt(stats.Attack);
        }


    }
}
