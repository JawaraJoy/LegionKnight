using UnityEngine;

namespace Rush
{
    public abstract partial class AbilityConfig : Configuration, IHasIcon
    {
        [SerializeField]
        private Sprite m_Icon;
        [Header("Targeting Setup")]
        [SerializeField]
        private LayerMask m_TargetFilter = ~0;
        [SerializeField]
        private TargetObject m_TargetObject = TargetObject.Enemy;
        [SerializeField]
        private TargetPriority m_TargetPriority = TargetPriority.Nearest;
        [SerializeField]
        private bool m_CanTargetDeathUnit = false;
        [Header("Advanced Priority Filter")]
        [SerializeField]
        private bool m_UseForwardCone = false;

        [SerializeField, Range(0f, 180f)]
        private float m_ConeAngle = 60f;

        [SerializeField]
        private bool m_RequireLineOfSight = false;
        [SerializeField]
        private float m_Range = 5f;
        [SerializeField]
        [Tooltip("If true, use all targets found in range instead of MaxTargetCount")]
        private bool m_UseAllTargetsInRange = false;
        public bool UseAllTargetsInRange => m_UseAllTargetsInRange;
        [SerializeField]
        private int m_MaxTargetCount = 1;
        [Space(10)]
        [SerializeField]
        private float m_InitialDelay = 0f;
        [SerializeField]
        private AbilityDeliver m_DeliverPrefab;
        [SerializeField]
        private AbilityPowerField m_Power;

        [Header("Status Effect")]
        [SerializeField]
        private StatusEffectConfig[] m_StatusEffectOnSelf;
        [SerializeField]
        private StatusEffectConfig[] m_StatusEffectOnDelivered;
        public StatusEffectConfig[] StatusEffectOnDelivered => m_StatusEffectOnDelivered;
        public StatusEffectConfig[] StatusEffectOnSelf => m_StatusEffectOnSelf;
        public Sprite Icon => m_Icon;
        public AbilityDeliver DeliverPrefab => m_DeliverPrefab;
        public TargetPriority TargetPriority => m_TargetPriority;
        public TargetObject TargetObject => m_TargetObject;
        public bool CanTargetDeathUnit => m_CanTargetDeathUnit;
        public LayerMask TargetFilter => m_TargetFilter;
        public float InitialDelay => m_InitialDelay;
        public float Range => m_Range;
        public int MaxTargetCount => m_MaxTargetCount;
        public AbilityPowerField Power => m_Power;
        public bool UseForwardCone => m_UseForwardCone;
        public float ConeAngle => m_ConeAngle;
        public bool RequireLineOfSight => m_RequireLineOfSight;
    }
}
