using System.Collections;
using UnityEngine;

namespace Rush
{
    public class AttackerExtention_Direct
    {
        
    }
    public partial class Attacker
    {
        /// <summary>
        /// Perform direct attack to target and immediately return to pool.
        /// </summary>
        public void DirectAttack(Targetable target, float delay)
        {
            m_OnAttackStart?.Invoke(m_AbilityContext);
            StartCoroutine(DirectAttacking(target, delay));
        }
        private IEnumerator DirectAttacking(Targetable target, float delay)
        {
            yield return new WaitForSeconds(delay);
            if (target.HasBind(out Damageable damageable))
            {
                damageable.TakeDamage(this);
                DirectDamageAbilityConfig config;
                if (m_AbilityContext.AbilityDeliver.Config is DirectDamageAbilityConfig directConfig)
                {
                    config = directConfig;
                    ExplodeDirectAttack(target, config);
                }
            }
            m_OnAttackDone?.Invoke(m_AbilityContext);
        }

        public void ExplodeDirectAttack(Targetable target, DirectDamageAbilityConfig config)
        {
            PhysicsMode mode = RushGameManager.Instance.GameConfig.PhysicsMode;
            if (mode == PhysicsMode.Physics2D)
            {
                Collider2D[] hits = Physics2D.OverlapCircleAll(
                    target.transform.position,
                    config.ExplodeSetup.ExplosionRadius,
                    config.TargetFilter
                );

                for (int i = 0; i < hits.Length; i++)
                {
                    if (!hits[i].TryGetComponent(out Targetable t)) continue;
                    if (!config.CanTargetDeathUnit && !t.IsAlive) continue;

                    if (t.HasBind(out Damageable dmg))
                        dmg.TakeDamage(this);
                }
            }
            else
            {
                Collider[] hits = Physics.OverlapSphere(
                    target.transform.position,
                    config.ExplodeSetup.ExplosionRadius,
                    config.TargetFilter
                );

                for (int i = 0; i < hits.Length; i++)
                {
                    if (!hits[i].TryGetComponent(out Targetable t)) continue;
                    if (!config.CanTargetDeathUnit && !t.IsAlive) continue;

                    if (t.HasBind(out Damageable dmg))
                        dmg.TakeDamage(this);
                }
            }
        }
    }
}
