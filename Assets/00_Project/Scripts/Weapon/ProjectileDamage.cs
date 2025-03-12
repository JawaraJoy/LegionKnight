using Rush;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace LegionKnight
{
    public partial class ProjectileDamage : Damageable
    {
        [SerializeField]
        private string m_TargetTag = "Bos Enemy";
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
            
            FindTarget();
        }
        private void FixedUpdate()
        {
            FollowTarget();
        }

        public void FollowTarget()
        {
            if (m_Target == null)
            {
                FindTargetInternal();
            }

            // Calculate direction to the target
            Vector3 direction = (m_Target.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(direction);

            // Smoothly rotate towards the target
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, m_RotateSpeed * Time.deltaTime);

            // Move forward
            m_Rb.linearVelocity = transform.forward * m_Speed;
        }

        public void AddForce(Vector2 force)
        {
            m_Rb.AddForce(force, ForceMode2D.Impulse);
        }
        public void FindTarget()
        {
            FindTargetInternal();   
        }
        private void FindTargetInternal()
        {
            m_Speed = Random.Range(m_MinTravelSpeed, m_MaxTravelSpeed);
            GameObject[] enemies = GameObject.FindGameObjectsWithTag(m_TargetTag);
            float shortestDistance = Mathf.Infinity;
            GameObject nearestEnemy = null;

            foreach (GameObject enemy in enemies)
            {
                float distance = Vector2.Distance(transform.position, enemy.transform.position);
                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    nearestEnemy = enemy;
                }
            }

            if (nearestEnemy != null)
            {
                m_Target = nearestEnemy.transform;
            }
        }
    }
}
