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
        protected float m_Duration = 1.0f;
        public Sprite Icon => m_Icon;
        public StatusEffector EffectorPrefab => m_EffectorPrefab;
        public float Duration => m_Duration;
        public abstract void ApplyEffect(Unit unitTarget);
        public abstract void DoneEffect(Unit unitTarget);
    }
}
