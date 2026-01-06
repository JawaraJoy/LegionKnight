using UnityEngine;

namespace LegionKnight
{
    public class PlayerDotInstant : MonoBehaviour
    {
        [SerializeField]
        private int m_Damage = 1;
        [SerializeField]
        private float m_Duration = 1f;

        public void Initialize(int damage, float duration)
        {
            m_Damage = damage;
            m_Duration = duration;
        }
        public void ApplyDamageOverTime()
        {
            if (m_Damage <= 0 || m_Duration <= 0)
            {
                Debug.LogWarning("Invalid damage or duration values.");
                return;
            }
            // Assuming there's a method to apply damage over time to the player
            Player.Instance.ApplyPlayerDamageOverTime(m_Damage, m_Duration);
        }

    }
}
