using UnityEngine;

namespace LegionKnight
{
    public class PlayerDamageable : MonoBehaviour
    {
        [SerializeField]
        private Damageable m_Damageable;

        public void TakeDamage(int damage)
        {
            if (m_Damageable != null)
            {
                m_Damageable.TakeDamage(damage);
            }
        }
        public void Heal(int amount)
        {
            if (m_Damageable != null)
            {
                m_Damageable.AddCurrentHealth(amount);
            }
        }
    }

    public partial class Player
    {
        [SerializeField]
        private PlayerDamageable m_PlayerDamageable;

        public void TakeDamage(int damage)
        {
            m_PlayerDamageable.TakeDamage(damage);
        }
        public void Heal(int amount)
        {
            m_PlayerDamageable.Heal(amount);
        }
    }
}
