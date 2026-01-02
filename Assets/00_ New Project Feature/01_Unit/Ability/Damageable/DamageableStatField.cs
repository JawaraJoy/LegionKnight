using Rush;
using UnityEngine;

namespace LegionKnight
{
    [System.Serializable]
    public class DamageableStatField
    {
        [SerializeField]
        private float m_Health = 100f;
        [SerializeField]
        private float m_Defense = 0f;
        [SerializeField]
        private int m_Shield = 0;
        [SerializeField]
        private int m_Barrier = 0;

        public void Init(AbilityContext context)
        {
            m_Health = Mathf.Max(0f, context.GetFinalStat().Health);
            m_Defense = Mathf.Max(0f, context.GetFinalStat().Defense);
        }
    }

}
