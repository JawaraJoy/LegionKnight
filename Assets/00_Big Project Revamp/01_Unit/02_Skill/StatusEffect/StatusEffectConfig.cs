using UnityEngine;

namespace Rush
{
    public abstract class StatusEffectConfig : Configuration
    {
        [SerializeField]
        private Sprite m_Icon;
        [SerializeField]
        private StatusEffector m_EffectorPrefab;
        [SerializeField]
        private bool m_ResetDurationWhenStacked = true;
        [SerializeField]
        private int m_MaxStack = 1;
        [SerializeField]
        protected float m_Duration = 1.0f;
        public Sprite Icon => m_Icon;
        public StatusEffector EffectorPrefab => m_EffectorPrefab;
        public int MaxStack => m_MaxStack;
        public float Duration => m_Duration;
        public bool ResetDurationWhenStacked => m_ResetDurationWhenStacked;
        public abstract void ApplyEffect(Unit unitTarget);
        public abstract void OnStackRemoved(Unit unitTarget);
        public abstract void DoneEffect(Unit unitTarget);
    }
}
