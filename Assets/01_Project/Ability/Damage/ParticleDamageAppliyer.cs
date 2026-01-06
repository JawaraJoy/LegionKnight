using System.Collections.Generic;
using UnityEngine;

namespace LegionKnight
{
    [RequireComponent(typeof(ParticleSystem))]
    public class ParticleDamageAppliyer : MonoBehaviour
    {
        [Header("Damage")]
        [SerializeField] private AbilityDefinition m_Ability;
        [SerializeField] private LayerMask m_LayerMask;
        private readonly Dictionary<GameObject, float> m_HitCooldown = new();
        [SerializeField] private float m_HitInterval = 0.2f;

        private ParticleSystem m_ParticleSystem;
        private readonly List<ParticleCollisionEvent> m_CollisionEvents = new();

        private GameObject m_Sender;
        private int m_Level;

        private void Awake()
        {
            m_ParticleSystem = GetComponent<ParticleSystem>();
        }
        public void Init(GameObject sender)
        {
            m_Sender = sender;
        }
        protected virtual int GetFinalDamage()
        {
            if (m_Ability == null && m_Sender == null) return 0;
            if (m_Sender == null) return m_Ability.Attack;
            if (m_Sender.TryGetComponent(out IProgressable progressable))
            {
                m_Level = progressable.GetLevel();
            }
            if (m_Ability == null) return 0;
            return m_Ability.GetFinalAttack(m_Level);
        }

        private void OnParticleCollision(GameObject other)
        {
            if (((1 << other.layer) & m_LayerMask.value) == 0)
                return;

            float time = Time.time;
            if (m_HitCooldown.TryGetValue(other, out float lastHit) &&
                time - lastHit < m_HitInterval)
                return;

            m_HitCooldown[other] = time;

            if (other.TryGetComponent(out Damageable damageable))
            {
                damageable.TakeDamage(GetFinalDamage());
            }
        }
    }
}
