
using UnityEngine;

namespace Rush
{
    public abstract class StatusEffectConfig : Configuration
    {
        [SerializeField]
        private Sprite m_Icon;
        [SerializeField]
        private ModifierType m_ModifierType = ModifierType.Buff;
        [SerializeField]
        private StatusEffector m_EffectorPrefab;
        [SerializeField]
        protected float m_Duration = 1.0f;
        [SerializeField]
        private int m_MaxStackCount = 1;
        [SerializeField]
        private int m_UpdatePerStackCount = 1;
        [SerializeField]
        private bool m_UseStackDuration = true;
        [SerializeField]
        private float m_StackDurationUpdate = 1f;
        [SerializeField]
        private bool m_ResetDurationOnStackUpdate = true;
        [SerializeField]
        private HowStackUpdate m_HowStackUpdate = HowStackUpdate.Addictive;
        [SerializeField]
        private HowStatRemoved m_HowToRemove = HowStatRemoved.RemoveOnDurationEnd;
        [Header("OnStackEmpty Activation OnInfected")]
        [SerializeField]
        protected SkillConfig[] m_InfectedSkillsToActivateOnStackEmpty;
        public SkillConfig[] InfectedSkillsToActivateOnStackEmpty => m_InfectedSkillsToActivateOnStackEmpty;
        public ModifierType ModifierType => m_ModifierType;
        public bool UseStackDuration => m_UseStackDuration;
        public float Duration => m_Duration;
        public float StackDurationUpdate => m_StackDurationUpdate;
        public int MaxStackCount => m_MaxStackCount;
        public int GetStartingStack()
        {
            int startingStack = 0;
            switch (m_HowStackUpdate)
            {
                case HowStackUpdate.Addictive:
                    startingStack = 1;
                    break;
                case HowStackUpdate.Subtractive:
                    startingStack = m_MaxStackCount;
                    break;
            }
            return startingStack;
        }
        public int UpdatePerStackCount => m_UpdatePerStackCount;
        public bool ResetDurationOnStackUpdate => m_ResetDurationOnStackUpdate;
        public HowStackUpdate HowStackUpdate => m_HowStackUpdate;
        public HowStatRemoved HowToRemove => m_HowToRemove;
        public Sprite Icon => m_Icon;
        public StatusEffector EffectorPrefab => m_EffectorPrefab;
        public abstract void ApplyEffect(StatusEffectContext context);
        public abstract void OnStackAdded(StatusEffectContext context);
        public abstract void OnStackRemoved(StatusEffectContext context);
        public abstract void DoneEffect(StatusEffectContext context);
    }

    public enum StatusCategory
    {
        None,
        DamageOverTime,
        CrowdControl,
        BuffAttack,
        BuffDefense,
        Shield,
        Utility
    }
    public enum StatusIdentityPolicy
    {
        SameConfig,
        SameConfigAndSource,
        SameGroup
    }
}
