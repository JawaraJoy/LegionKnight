using UnityEngine;

namespace Rush
{
    public abstract class StatusEffectConfig : Configuration
    {
        [Header("Visual")]
        [SerializeField] private Sprite m_Icon;
        [SerializeField] private ModifierType m_ModifierType = ModifierType.Buff;
        [SerializeField] private StatusEffector m_EffectorPrefab;

        [Header("Reapply")]
        [SerializeField] private StatusReapplyBehavior m_ReapplyBehavior = StatusReapplyBehavior.Stack;

        [Header("Stack")]
        [SerializeField] private int m_InitialStack = 1;
        [SerializeField] private int m_MaxStackCount = 1;
        [SerializeField] private int m_StackPerApply = 1;

        [Header("Main Duration")]
        [SerializeField] private bool m_UseMainDuration = true;
        [SerializeField] private float m_MainDuration = 5f;
        [SerializeField] private bool m_ResetMainDurationOnReapply = true;

        [Header("Stack Decay")]
        [SerializeField] private bool m_UseStackDecay = false;
        [SerializeField] private float m_StackDecayInterval = 2f;
        [SerializeField] private int m_StackDecayAmountPerInterval = 1;
        [SerializeField] private bool m_ResetMainDurationOnStackDecay = true;
        [SerializeField] private bool m_ResetStackDecayTimerOnReapply = true;

        [Header("Remove Rule")]
        [SerializeField] private StatusRemoveRule m_RemoveRule = StatusRemoveRule.OnStackZero;

        [Header("On Effect End - Activate Skill On Infected")]
        [SerializeField] protected SkillConfig[] m_InfectorSkillsToActivateOnDoneEffect;

        public Sprite Icon => m_Icon;
        public ModifierType ModifierType => m_ModifierType;
        public StatusEffector EffectorPrefab => m_EffectorPrefab;

        public StatusReapplyBehavior ReapplyBehavior => m_ReapplyBehavior;

        public int MaxStackCount => Mathf.Max(1, m_MaxStackCount);
        public int StackPerApply => Mathf.Max(1, m_StackPerApply);
        public int InitialStack => Mathf.Clamp(m_InitialStack, 1, MaxStackCount);

        public bool UseMainDuration => m_UseMainDuration;
        public float MainDuration => Mathf.Max(0f, m_MainDuration);
        public bool ResetMainDurationOnReapply => m_ResetMainDurationOnReapply;

        public bool UseStackDecay => m_UseStackDecay;
        public float StackDecayInterval => Mathf.Max(0.01f, m_StackDecayInterval);
        public int StackDecayAmountPerInterval => Mathf.Max(1, m_StackDecayAmountPerInterval);
        public bool ResetMainDurationOnStackDecay => m_ResetMainDurationOnStackDecay;
        public bool ResetStackDecayTimerOnReapply => m_ResetStackDecayTimerOnReapply;

        public StatusRemoveRule RemoveRule => m_RemoveRule;

        /// <summary>
        /// Dipanggil sekali saat effect mulai aktif.
        /// Cocok untuk setup VFX, baseline stat, subscribe event, dsb.
        /// </summary>
        public abstract void OnEffectStarted(StatusEffectContext context);

        /// <summary>
        /// Dipanggil setiap ada stack yang bertambah, termasuk stack pertama.
        /// Cocok untuk apply buff/debuff per stack.
        /// </summary>
        public abstract void OnStackAdded(StatusEffectContext context);

        /// <summary>
        /// Dipanggil setiap ada stack yang berkurang.
        /// Cocok untuk rollback buff/debuff per stack.
        /// </summary>
        public abstract void OnStackRemoved(StatusEffectContext context);

        /// <summary>
        /// Dipanggil sekali saat effect selesai total.
        /// Cocok untuk cleanup VFX, unsubscribe event, dsb.
        /// </summary>
        public abstract void OnEffectEnded(StatusEffectContext context);
    }

    public enum StatusReapplyBehavior
    {
        Stack,
        Refresh,
        Override
    }

    public enum StatusRemoveRule
    {
        None = 0,
        OnMainDurationEnd = 1,
        OnStackZero = 2,
        OnStackReachMax = 3,
        OnStackExceedMax = 4
    }
}