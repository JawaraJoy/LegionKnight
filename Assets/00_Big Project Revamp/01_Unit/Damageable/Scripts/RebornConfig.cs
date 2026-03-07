using UnityEngine;

namespace Rush
{
    [CreateAssetMenu(fileName = "Reborn", menuName = "Rush/Combat/Reborn")]
    public class RebornConfig : ScriptableObject
    {
        [SerializeField]
        private int m_StartingReborn = 0;
        [SerializeField]
        private float m_RebornDelay = 0f;
        [SerializeField]
        private float m_HealthRate = 1f;
        [SerializeField]
        private int m_ShieldAmount;
        [SerializeField]
        private int m_BarrierAmount;
        [SerializeField]
        private float m_ShieldRateBaseOnMaxHealth = 0f;
        [SerializeField]
        private float m_InvincibleTime = 0f;
        public int StartingReborn => m_StartingReborn;
        public float RebornDelay => m_RebornDelay;
        public void ApplyReborn(Damageable damageable)
        {
            damageable.RefreshDamageableStat(m_HealthRate, true);

            damageable.AddShield(m_ShieldAmount, true);
            damageable.AddShieldBasedOnMaxHealth(m_ShieldRateBaseOnMaxHealth, true);
            damageable.AddBarrier(m_BarrierAmount, true);

            damageable.InvisibleForWhile(m_InvincibleTime);
        }
    }
}
