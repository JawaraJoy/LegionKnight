using Rush;
using UnityEngine;

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

        [SerializeField]
        private bool m_WithoutTarget = false;
        [SerializeField]
        private Vector2 m_VelocityWitoutTarget;

        private int m_FindTargetCount;
        private const int m_FindTargetCounMax = 3;

        public void SetTarget(Transform target)
        {
            m_Target = target;
            
            FindTarget();
        }
        public void SetWithoutTarget(bool withoutTarget)
        {
            m_WithoutTarget = withoutTarget;
        }
        private void Update()
        {
            if (m_WithoutTarget)
            {
                Move();
            }
            else
            {
                FollowTarget();
            }    
        }
        private void Move()
        {
            if (m_VelocityWitoutTarget == Vector2.zero) return;
            m_Rb.linearVelocity = GetSpeed() * m_VelocityWitoutTarget;
            Quaternion lookRotation = Quaternion.LookRotation(Vector3.forward, m_VelocityWitoutTarget);
        }
        private void FollowTarget()
        {
            if (m_Target == null)
            {
                m_FindTargetCount++;
                if (m_FindTargetCount < m_FindTargetCounMax)
                {
                    FindTargetInternal();
                }
                else
                {
                    Destroy(gameObject);
                }
            }
            if (m_Target == null) return;
            // Calculate direction to the target
            Vector3 direction = (m_Target.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(direction);

            // Smoothly rotate towards the target
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, m_RotateSpeed * Time.deltaTime);

            // Move forward
            m_Rb.linearVelocity = transform.forward * GetSpeed();
        }

        public void AddForce(Vector2 force)
        {
            m_Rb.AddForce(force, ForceMode2D.Impulse);
        }
        private float GetSpeed()
        {
            m_Speed = Random.Range(m_MinTravelSpeed, m_MaxTravelSpeed);
            return m_Speed;
        }
        public void FindTarget()
        {
            FindTargetInternal();   
        }
        private void FindTargetInternal()
        {
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
