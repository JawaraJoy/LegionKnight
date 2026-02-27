using UnityEngine;

namespace Rush
{
    [System.Serializable]
    public class ActivationTriggerField
    {
        [Tooltip("Ketika Mana Atau Charge kondisinya terpenuhi")]
        [SerializeField]
        private bool m_AutoActiveOnReady = false;
        [SerializeField]
        private ForceActiveState m_ForceActiveState = ForceActiveState.OnChargeFull;
        [SerializeField]
        private float m_Charge = 10f;
        [SerializeField]
        private float m_Cooldown = 10f;

        public bool AutoActiveOnReady => m_AutoActiveOnReady;
        public ForceActiveState ForceActiveState => m_ForceActiveState;
        public float Charge => m_Charge;
        public float Cooldown => m_Cooldown;
    }

    public enum ForceActiveState
    {
        None = 0,
        OnCooldownDone,
        OnChargeFull,
        OnHit,
        OnGetHit,
        OnStatModified,
        OnDeclareAttack,
        OnDamageDealed,
        OnDamageTaken,
        OnHealing,
        OnHealed,
        OnNormalTouch,
        OnPerfectTouch,
    }
}
