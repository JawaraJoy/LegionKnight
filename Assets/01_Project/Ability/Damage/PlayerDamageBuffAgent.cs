using UnityEngine;

namespace LegionKnight
{
    public class PlayerDamageBuffAgent : MonoBehaviour
    {
        [SerializeField]
        private float m_Duration = 5f;
        private PlayerDamageBuff m_PlayerDamageBuff;

        private PlayerDamageBuff GetPlayerDamageBuff()
        {
            if (m_PlayerDamageBuff == null)
            {
                m_PlayerDamageBuff = Player.Instance.GetPlayerDamageBuff();
            }
            return m_PlayerDamageBuff;
        }

        public void AddAttackRateTemp(float attackRate)
        {
            GetPlayerDamageBuff().AddAttackRateTemp(attackRate, m_Duration);
        }
    }
}
