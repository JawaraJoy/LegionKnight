using UnityEngine;

namespace LegionKnight
{
    [System.Serializable]
    public class CastingStat
    {
        [SerializeField]
        private float m_CastingTime = 1f;
        [SerializeField]
        private float m_CastingGrowth = 0.1f;

        private float m_CurrentCastingTime;

        public float CastingTime => m_CastingTime;
        public float CastingGrowth => m_CastingGrowth;

        private float CastingTimeFlowInternal(int level)
        {
            m_CurrentCastingTime += Time.deltaTime;
            m_CurrentCastingTime = Mathf.Clamp(m_CurrentCastingTime, 0f, GetCastingTime(level));
            return m_CurrentCastingTime;
        }

        public void StartCasting()
        {
            m_CurrentCastingTime = 0f;
        }

        public void ResetCastingTime()
        {
            m_CurrentCastingTime = 0f;
        }

        private float GetCastingTime(int level)
        {
            return m_CastingTime + (m_CastingGrowth * level);
        }

        public float CastingTimeFlow(int level)
        {
            return CastingTimeFlowInternal(level);
        }
    }
}
