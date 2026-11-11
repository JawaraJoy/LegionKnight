using UnityEngine;

namespace Rush
{
    public sealed class MinionPresentation : MonoBehaviour, IUnitExtension
    {
        [System.Serializable]
        private class FeedbackField
        {
            [SerializeField] private AllainFeedbackEffect m_Arrival;
            [SerializeField] private AllainFeedbackEffect m_Defeat;

            public AllainFeedbackEffect Arrival => m_Arrival;
            public AllainFeedbackEffect Defeat => m_Defeat;
        }

        [SerializeField] private FeedbackField m_Feedback;
        [SerializeField] private CapsuleCollider2D m_Hitbox;
        private Unit m_Unit;
        private Damageable m_Damageable;
        private SkillController m_Skills;
        private PingPongMover2D m_Mover;
        private bool m_ArrivalPending;
        private bool m_Dying;
        private bool m_HasOriginalShape;
        private Vector2 m_OriginalSize;
        private Vector2 m_OriginalOffset;
        public IModuleContext ModuleContext { get; private set; }

        public void Init(Unit unit)
        {
            m_Unit = unit;
            ModuleContext = new ModuleContext(unit, gameObject);
            unit.HasBind(out m_Damageable);
            unit.HasBind(out m_Skills);
            TryGetComponent(out m_Mover);
            m_Dying = false;
            if (m_Mover != null) m_Mover.enabled = true;

            if (!m_HasOriginalShape)
            {
                m_OriginalSize = m_Hitbox.size;
                m_OriginalOffset = m_Hitbox.offset;
                m_HasOriginalShape = true;
            }
            Vector2 scale = unit.Config.UnitScale;
            m_Hitbox.size = Vector2.Scale(m_OriginalSize, scale);
            m_Hitbox.offset = Vector2.Scale(m_OriginalOffset, scale);
            m_Hitbox.enabled = true;
            if (m_Skills != null)
                foreach (Skill skill in m_Skills.Skills)
                {
                    skill.OnTryActivate.RemoveListener(PlayAttackInternal);
                    skill.OnTryActivate.AddListener(PlayAttackInternal);
                }
            m_ArrivalPending = true;
            PlayArrivalIfReadyInternal();
        }

        private void OnEnable() => PlayArrivalIfReadyInternal();

        private void PlayArrivalIfReadyInternal()
        {
            if (!m_ArrivalPending || !isActiveAndEnabled) return;
            m_ArrivalPending = false;
            PlayEffectInternal(m_Feedback.Arrival);
        }

        private void PlayAttackInternal(Skill skill)
        {
            if (!m_Dying && m_Damageable.IsAlive) m_Unit.Config.PlayAttack(m_Unit);
        }

        public void PlayDefeat() => PlayDefeatInternal();

        private void PlayDefeatInternal()
        {
            if (m_Dying || m_Unit == null) return;
            m_Dying = true;
            m_Hitbox.enabled = false;
            if (m_Mover != null) m_Mover.enabled = false;
            PlayEffectInternal(m_Feedback.Defeat);
        }

        private void PlayEffectInternal(AllainFeedbackEffect prefab)
        {
            if (prefab == null) return;
            Vector3 center = m_Hitbox.transform.TransformPoint(m_Hitbox.offset);
            var effect = Instantiate(prefab, center, Quaternion.identity);
            effect.transform.localScale = Vector3.Scale(effect.transform.localScale, m_Unit.Config.UnitScale);
            effect.Play();
        }

        private void OnDisable()
        {
            if (m_Skills == null) return;
            foreach (Skill skill in m_Skills.Skills)
                skill.OnTryActivate.RemoveListener(PlayAttackInternal);
        }
    }
}
