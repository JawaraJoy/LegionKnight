using Rush;
using UnityEngine;

namespace LegionKnight
{
    public partial class ProjectileDamage : Damageable
    {
        private Transform m_Target;  // Target to follow
        [SerializeField]
        private float m_RotateSpeed = 5f; // Rotation speed
        [SerializeField]
        private float m_MinTravelSpeed = 5f;
        [SerializeField]
        private float m_MaxTravelSpeed = 10f;

        private float m_Speed;
        [SerializeField]
        private Rigidbody2D m_Rb;

        public void SetTarget(Transform target)
        {
            m_Target = target;
            m_Speed = Random.Range(m_MinTravelSpeed, m_MaxTravelSpeed);
        }
        private void FixedUpdate()
        {
            FollowTarget();
        }

        public void FollowTarget()
        {
            if (m_Target == null) return;

            // Calculate direction to the target
            Vector3 direction = (m_Target.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(direction);

            // Smoothly rotate towards the target
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, m_RotateSpeed * Time.deltaTime);

            // Move forward
            m_Rb.linearVelocity = transform.forward * m_Speed;
        }
    }
}
