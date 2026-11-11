using UnityEngine;

namespace LegionKnight
{
    /// <summary>
    /// 2D physics-based projectile implementation.
    /// </summary>
    public class Projectile2D : ProjectileBase
    {
        protected override void UpdateMovement()
        {
            float speed = m_Config.Speed *
                (m_Config.SpeedCurve != null ? m_Config.SpeedCurve.Evaluate(m_Timer) : 1f);

            Vector3 move = m_Direction * speed * Time.deltaTime;

            if (m_Config.Motion == MotionType.Curve && m_Config.Curve != null)
            {
                move += transform.up * m_Config.Curve.Evaluate(m_Timer);
            }

            transform.position += move;
        }

        protected override void Explode()
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(
                transform.position,
                m_Config.ExplosionRadius
            );

            foreach (var hit in hits)
            {
                // Apply damage or effects here
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            HandleHit(other.gameObject);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            OnCollideExit?.Invoke(other.gameObject);
        }
    }
}
