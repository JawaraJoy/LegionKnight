using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public class Casting : MonoBehaviour
    {
        [SerializeField]
        private string m_CastingName = "Default Casting";
        [SerializeField]
        private CastingStat m_CastingStat;
        private bool m_CanCast = false;

        [SerializeField]
        private UnityEvent<float> m_OnCastingTimeFlow;

        [SerializeField]
        private UnityEvent<string> m_OnCastStart;
        [SerializeField]
        private UnityEvent m_OnCastEnd;
        [SerializeField]
        private UnityEvent m_OnCastFailed;

        [SerializeField]
        protected int m_Level = 1; // This should be set based on your game logic

        public virtual void InitLevel(int level)
        {
            m_CanCast = false;
            m_Level = level;
        }
        public void ResetCastingTime()
        {
            m_CastingStat.ResetCastingTime();
            m_CanCast = false;
        }
        protected float CastingTimeFlowInternal()
        {
            return m_CastingStat.CastingTimeFlow(m_Level);
        }
        public virtual void StartCasting()
        {
            m_CastingStat.StartCasting();
            m_CanCast = true;
            m_OnCastStart.Invoke(m_CastingName);
        }

        public void StopCasting()
        {
            m_CastingStat.ResetCastingTime();
            m_CanCast = false;
            m_OnCastFailed.Invoke();
        }

        private void Update()
        {
            HandleCast();
        }

        private void HandleCast()
        {
            if (m_CanCast)
            {
                float castingTime = CastingTimeFlowInternal();
                float castingTimePercentage = castingTime / m_CastingStat.CastingTime;
                m_OnCastingTimeFlow.Invoke(castingTimePercentage);
                if (castingTime >= m_CastingStat.CastingTime)
                {
                    // Trigger the casting action here
                    m_CanCast = false;
                    m_OnCastEnd.Invoke();
                    // Reset or perform any post-cast actions
                }
            }
        }
    }
}
