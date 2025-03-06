using Rush;
using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public partial class Damageable : Contact2D
    {
        private int m_Damage;
        private int m_Health;
        private int m_CurrentHealth;
        [SerializeField]
        private UnityEvent m_OnDeath = new();
        protected override void OnContactedBehaviourInvoke(IContactable other)
        {
            base.OnContactedBehaviourInvoke(other);

            if (other.GetSelf().TryGetComponent(out DamagePlatform platform))
            {
                //Player.Instance.AddCurrencyAmount(platform.GetNormalTouchDown().CurrencyDefinition, platform.GetNormalTouchDown().Amount);
                platform.SetCanContact(false);
            }
        }
        public int Damage => m_Damage;
        public int Health => m_Health;
        public int CurrentHealth => m_CurrentHealth;
        private void ClampHealth()
        {
            m_CurrentHealth = Mathf.Clamp(m_CurrentHealth, 0, m_Health);
        }
        public void Init(int damage, int health)
        {
            m_Damage = damage;
            m_Health = health;
            m_CurrentHealth = m_Health;
        }

        protected void TakeDamageInternal(int damage)
        {
            m_CurrentHealth -= damage;
            DeathHandler();
            ClampHealth();
        }
        protected void HealInternal(int heal)
        {
            m_CurrentHealth += heal;
            ClampHealth();
        }

        private void DeathHandler()
        {
            if (m_CurrentHealth < 1)
            {
                m_OnDeath?.Invoke();
            }
        }

    }
}
