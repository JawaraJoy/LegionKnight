using UnityEngine;

namespace LegionKnight
{
    /// <summary>
    /// 3D physics-based projectile implementation.
    /// </summary>
    public class Projectile3D : ProjectileBase
    {
        protected override void UpdateMovement()
        {
            float speed = m_Config.Speed *
                (m_Config.SpeedCurve != null ? m_Config.SpeedCurve.Evaluate(m_Timer) : 1f);

            Vector3 move = m_Direction * speed * Time.deltaTime;

            if (m_Config.Motion == MotionType.Spiral)
            {
                float angle = m_Timer * m_Config.SpiralSpeed;
                Vector3 offset = transform.up * Mathf.Sin(angle) * m_Config.SpiralRadius;
                move += offset * Time.deltaTime;
            }
            else if (m_Config.Motion == MotionType.Curve && m_Config.Curve != null)
            {
                move += transform.up * m_Config.Curve.Evaluate(m_Timer);
            }

            transform.position += move;
        }

        protected override void Explode()
        {
            Collider[] hits = Physics.OverlapSphere(
                transform.position,
                m_Config.ExplosionRadius
            );

            foreach (var hit in hits)
            {
                // Apply damage or effects here
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            HandleHit(other.gameObject);
        }
        private void OnTriggerExit(Collider other)
        {
            OnCollideExit?.Invoke(other.gameObject);
        }
    }
}

