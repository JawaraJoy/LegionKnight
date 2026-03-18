using UnityEngine;

namespace Rush
{
    public abstract class StatusEffectConfig : Configuration
    {
        [SerializeField] private Sprite m_Icon;
        [SerializeField] private ModifierType m_ModifierType = ModifierType.Buff;
        [SerializeField] private StatusEffector m_EffectorPrefab;
        [SerializeField] protected float m_Duration = 1f;

        [Header("Reapply")]
        [SerializeField] private StatusReapplyBehavior m_ReapplyBehavior = StatusReapplyBehavior.Stack;

        [Header("Stack")]
        [SerializeField] private int m_InitialStack = 1;
        [SerializeField] private int m_MaxStackCount = 1;
        [SerializeField] private int m_StackPerApply = 1;

        [Header("Duration")]
        [SerializeField] private bool m_ResetDurationOnReapply = true;
        [SerializeField] private bool m_UseBonusDurationPerReapply = false;
        [SerializeField] private float m_BonusDurationPerReapply = 0f;

        [Header("Remove")]
        [SerializeField] private HowStatRemoved m_HowToRemove = HowStatRemoved.RemoveOnDurationEnd;

        [Header("OnStackEmpty Activation OnInfected")]
        [SerializeField] protected SkillConfig[] m_InfectorSkillsToActivateOnDoneEffect;

        public Sprite Icon => m_Icon;
        public ModifierType ModifierType => m_ModifierType;
        public StatusEffector EffectorPrefab => m_EffectorPrefab;
        public float Duration => m_Duration;
        public StatusReapplyBehavior ReapplyBehavior => m_ReapplyBehavior;
        public int MaxStackCount => Mathf.Max(1, m_MaxStackCount);
        public int StackPerApply => Mathf.Max(1, m_StackPerApply);
        public int InitialStack => Mathf.Clamp(m_InitialStack, 1, MaxStackCount);
        public bool ResetDurationOnReapply => m_ResetDurationOnReapply;
        public bool UseBonusDurationPerReapply => m_UseBonusDurationPerReapply;
        public float BonusDurationPerReapply => m_BonusDurationPerReapply;
        public HowStatRemoved HowToRemove => m_HowToRemove;
        public abstract void ApplyEffect(StatusEffectContext context);
        public abstract void OnStackAdded(StatusEffectContext context);
        public abstract void OnStackRemoved(StatusEffectContext context);
        public abstract void DoneEffect(StatusEffectContext context);
    }

    public enum StatusReapplyBehavior
    {
        Stack,
        Refresh,
        Override
    }
}