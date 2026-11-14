using UnityEngine;

namespace Rush
{
    public sealed class BattleCastingSfxFeedback : MonoBehaviour
    {
        [SerializeField] private BattleSfxCue m_InterruptCue;

        public void PlayInterrupted(Skill skill) => PlayInterruptedInternal(skill);

        private void PlayInterruptedInternal(Skill skill)
        {
            if (skill == null || skill.CurrentInterruptCount <= 0 ||
                skill.CurrentInterruptCount < skill.MaxInterruptCount || m_InterruptCue == null)
                return;

            Unit owner = skill.SkillContext?.ModuleContext?.Unit;
            if (owner == null || !owner.HasBind(out Damageable damageable) || !damageable.IsAlive)
                return;

            m_InterruptCue.Play();
        }
    }
}
