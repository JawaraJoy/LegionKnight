using UnityEngine;

namespace Rush
{
    public class AllainFeedbackEmitter : MonoBehaviour
    {
        [SerializeField] private AllainFeedbackEffect m_Effect;
        [SerializeField] private bool m_PlayOnEnable;
        [SerializeField] private bool m_AtOwner;

        private void OnEnable()
        {
            foreach (var trail in GetComponentsInChildren<TrailRenderer>()) trail.Clear();
            if (m_PlayOnEnable) PlayInternal(transform.position);
        }
        public void Play() => PlayInternal(transform.position);
        public void PlayAtTarget(GameObject target) => PlayInternal(target != null ? target.transform.position : transform.position);
        public void PlayFromAbility(AbilityContext context)
        {
            Unit owner = context.SkillContext.ModuleContext.Unit;
            PlayInternal(m_AtOwner && owner != null ? owner.transform.position : transform.position);
        }
        private void PlayInternal(Vector3 position)
        {
            if (m_Effect == null) return;
            var effect = Instantiate(m_Effect, position, Quaternion.identity);
            effect.Play();
        }
    }
}
