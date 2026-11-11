using UnityEngine;

namespace Rush
{
    // Allain's wave stays perpendicular to the projectile's local forward axis.
    // The shared Ammo implementation mixes a world vector into a local translation.
    public class AllainHomingProjectile : Projectile
    {
        private float m_SwayElapsed;
        public override void PrepareForSpawn(Vector3 position, Quaternion rotation)
        {
            base.PrepareForSpawn(position, rotation);
            m_SwayElapsed = 0f;
        }
        protected override float Move()
        {
            m_SwayElapsed += Time.deltaTime;
            Vector3 forward = m_Config.ForwardAxis == LocalAxis.X ? Vector3.right :
                m_Config.ForwardAxis == LocalAxis.Z ? Vector3.forward : Vector3.up;
            Vector3 lateral = m_Config.ForwardAxis == LocalAxis.X ? Vector3.up : Vector3.right;
            float distance = m_Config.Speed * Time.deltaTime;
            float sway = Mathf.Sin(m_SwayElapsed * m_Config.SwayFrequency) * m_Config.SwayAmplitude;
            transform.Translate(forward * distance + lateral * (sway * Time.deltaTime), Space.Self);
            return distance;
        }
    }
}
