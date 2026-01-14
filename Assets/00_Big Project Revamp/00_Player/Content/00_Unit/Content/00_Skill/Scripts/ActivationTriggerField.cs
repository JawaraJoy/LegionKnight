using UnityEngine;

namespace Rush
{
    [System.Serializable]
    public class ActivationTriggerField
    {
        [SerializeField]
        private bool m_AutoActiveOnReady = false;
        [SerializeField]
        private SkillTriggerState m_ReadyState = SkillTriggerState.OnChargeFull;
        [SerializeField]
        private float m_Charge = 10f;
        [SerializeField]
        private float m_Cooldown = 10f;

        public bool AutoActiveOnReady => m_AutoActiveOnReady;
        public SkillTriggerState TriggerState => m_ReadyState;
        public float Charge => m_Charge;
        public float Cooldown => m_Cooldown;
    }

    public enum SkillTriggerState
    {
        OnCooldownDone = 0,
        OnChargeFull = 1,
        OnHit = 2,
        OnDeclareAttack = 3,
        OnDamageDealed = 4,
    }
}
