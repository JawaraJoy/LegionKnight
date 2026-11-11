using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rush
{
    public class AnomalyPulse : AbilityDeliver
    {
        private bool m_Activated;
        protected override void ActivateInternal()
        {
            if (m_Activated) return;
            m_Activated = true;
            base.ActivateInternal();
            StartCoroutine(PulseInternal());
        }

        private IEnumerator PulseInternal()
        {
            // Finish the originating hit/death before applying secondary damage.
            yield return null;
            if (m_AbilityContext.SkillContext.ModuleContext.Unit == null)
            {
                Destroy(gameObject);
                yield break;
            }
            var config = (AnomalyPulseConfig)m_AbilityConfig;
            Vector3 center = m_DeliverTransform.position;
            var targets = TargetsInternal(center, Mathf.Max(config.Range, config.Pull.Radius));
            var pulls = new List<PullState>();
            foreach (var target in targets)
            {
                if (!target.IsAlive) continue;
                Vector3 start = target.TargetTransform.position;
                float distance = Vector3.Distance(center, start);
                if (distance <= config.Pull.Radius && config.Pull.Distance > 0f &&
                    !(target.ModuleContext.Unit.Config is BossUnitConfig))
                    pulls.Add(new PullState(target, target.ModuleContext.Unit.transform,
                        Vector3.MoveTowards(start, center, config.Pull.Distance) - start));
                if (distance <= config.Range && target.ModuleContext.Unit.HasBind(out Damageable damageable))
                    damageable.TakeDamage(m_AbilityContext);
            }
            float elapsed = 0f;
            float previous = 0f;
            do
            {
                elapsed += Time.deltaTime;
                float progress = config.Pull.Duration > 0f ? Mathf.Clamp01(elapsed / config.Pull.Duration) : 1f;
                foreach (var pull in pulls)
                    if (pull.Root != null && pull.Root.gameObject.activeInHierarchy && pull.Target.IsAlive)
                        pull.Root.position += pull.Displacement * (progress - previous);
                previous = progress;
                yield return null;
            } while (elapsed < config.Pull.Duration);
            Destroy(gameObject);
        }

        private List<ITargetable> TargetsInternal(Vector3 center, float radius)
        {
            var targets = new List<ITargetable>();
            var units = new HashSet<Unit>();
            if (RushGameManager.Instance.GameConfig.PhysicsMode == PhysicsMode.Physics2D)
            {
                foreach (var hit in Physics2D.OverlapCircleAll(center, radius, m_AbilityConfig.TargetFilter))
                    AddTargetInternal(hit.gameObject, targets, units);
            }
            else
            {
                foreach (var hit in Physics.OverlapSphere(center, radius, m_AbilityConfig.TargetFilter))
                    AddTargetInternal(hit.gameObject, targets, units);
            }
            return targets;
        }

        private void AddTargetInternal(GameObject candidate, List<ITargetable> targets, HashSet<Unit> units)
        {
            if (!candidate.TryGetComponent(out ITargetable target) || !target.IsAlive ||
                !candidate.activeInHierarchy || !AbilityUltility.IsTargetAllowedByTargetObject(this, target)) return;
            if (units.Add(target.ModuleContext.Unit)) targets.Add(target);
        }

        private sealed class PullState
        {
            public readonly ITargetable Target;
            public readonly Transform Root;
            public readonly Vector3 Displacement;
            public PullState(ITargetable target, Transform root, Vector3 displacement)
            { Target = target; Root = root; Displacement = displacement; }
        }
    }
}
