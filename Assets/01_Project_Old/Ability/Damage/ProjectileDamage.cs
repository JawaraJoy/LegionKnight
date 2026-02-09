using Rush;
using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public partial class ProjectileDamage : Damageable, IAbility, ISelfAbility
    {
        [SerializeField]
        private AbilityDefinition m_AbilityDefinition; // Reference to the ability definition
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

        [SerializeField]
        private UnityEvent<AbilityDefinition, int> m_OnAbilityInitialized = new();

        private int m_FindTargetCount;
        private const int m_FindTargetCounMax = 3;

        public void Initialize(AbilityDefinition defi, int level)
        {
            // Initialize the ability with the provided definition
            // This can include setting up specific properties or configurations based on the definition
            if (defi != null)
            {
                m_Damage = defi.GetFinalAttack(level); // Assuming AbilityDefinition has a GetFinalAttack method
                m_Health = defi.GetFinalHealth(level); // Assuming AbilityDefinition has a GetFinalHealth method
                m_OnAbilityInitialized?.Invoke(defi, level); // Invoke the event with the ability definition and level
            }
        }

        public void InitializeForPlayer()
        {
            if (m_AbilityDefinition == null) return;
            CharacterDefinition characterDefinition = Player.Instance.UsedCharacter; // Get the character definition from the player instance
            CharacterUnit unit = Player.Instance.GetCharacterUnit(characterDefinition);
            int level = unit.Level;
            m_Damage = m_AbilityDefinition.GetFinalAttack(level); // Assuming AbilityDefinition has a GetFinalAttack method
            m_Health = m_AbilityDefinition.GetFinalHealth(level);
        }

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
            //m_Rb.linearVelocity = GetSpeed() * m_VelocityWitoutTarget;
            Vector2 localUp = transform.up;
            m_Rb.linearVelocity = localUp * GetSpeed();
            //Quaternion lookRotation = Quaternion.LookRotation(Vector3.forward, m_VelocityWitoutTarget);
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
            // Direction to target (2D)
            Vector2 direction = (m_Target.position - transform.position);

            // Angle on Z axis (XY plane)
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // If your projectile faces UP, subtract 90f
            angle -= 90f;

            // Z-only rotation
            Quaternion lookRotation = Quaternion.Euler(0f, 0f, angle);

            // Smooth rotate
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                lookRotation,
                m_RotateSpeed * Time.deltaTime
            );

            // Move forward (2D)
            m_Rb.linearVelocity = transform.up * GetSpeed();
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

        public void SetStat(Stat stat)
        {
            if (stat == null) return;
            m_Damage = stat.Attack;
            m_Health = stat.Health;
            m_Defend = stat.Defense;
        }
    }
}
