using System.Collections.Generic;
using UnityEngine;
using static Rush.Projectile;

namespace Rush
{
    public abstract partial class AbilityConfig : Configuration
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
        [SerializeField]
        private float m_InitialDelay = 0f;
        [SerializeField]
        private float m_Range = 5f;
        [SerializeField]
        private int m_MaxTargetCount = 1;

        [Space(10)]
        [SerializeField]
        private AbilityDeliver m_DeliverPrefab;
        public AbilityDeliver DeliverPrefab => m_DeliverPrefab;

        [SerializeField]
        private EffectCalculatorField m_EffectCalculator;

        public Sprite Icon => m_Icon;
        public TargetPriority TargetPriority => m_TargetPriority;
        public TargetObject TargetObject => m_TargetObject;
        public bool CanTargetDeathUnit => m_CanTargetDeathUnit;
        public LayerMask TargetFilter => m_TargetFilter;
        public float InitialDelay => m_InitialDelay;
        public float Range => m_Range;
        public int MaxTargetCount => m_MaxTargetCount;
        public EffectCalculatorField EffectCalculator => m_EffectCalculator;
    }
    
    public static partial class AbilityUltility
    {
        private static readonly Collider[] m_ColliderBuffer3D = new Collider[32];
        private static readonly Collider2D[] m_ColliderBuffer2D = new Collider2D[32];

        public static List<Targetable> GetTargetables(AbilityContext context, PhysicsMode physicsMode)
        {
            unit
            AbilityConfig config = context.AbilityDeliver.Config;
            Vector3 pos = context.AbilityDeliver.transform.position;

            return physicsMode switch
            {
                PhysicsMode.Physics2D => GetTargetables2DInternal(pos, config),
                PhysicsMode.Physics3D => GetTargetables3DInternal(pos, config),
                _ => new List<Targetable>()
            };
        }
        private static List<Targetable> GetTargetables2DInternal(Vector3 pos, AbilityConfig config)
        {
            List<Targetable> result = new();

            int count = Physics.OverlapSphereNonAlloc(
                pos,
                config.Range,
                m_ColliderBuffer3D,
                config.TargetFilter
            );

            for (int i = 0; i < count; i++)
            {
                if (m_ColliderBuffer3D[i].TryGetComponent(out Targetable target))
                {
                    if (target.IsAlive || config.CanTargetDeathUnit)
                    {
                        result.Add(target);
                    }
                }
            }

            return result;
        }
        private static bool IsTargetAllowedByFilter(AbilityConfig config, Targetable targetable)
        {
            if (config == null || targetable == null)
                return false;

            int targetLayer = targetable.gameObject.layer;
            return (config.TargetFilter.value & (1 << targetLayer)) != 0;
        }
        private static List<Targetable> GetTargetables3DInternal(Vector3 pos, AbilityConfig config)
        {
            List<Targetable> result = new();
            int count3D = Physics.OverlapSphereNonAlloc(pos, config.Range, m_ColliderBuffer3D, config.TargetFilter);

            for (int i = 0; i < count3D && result.Count < config.MaxTargetCount; i++)
            {
                if (m_ColliderBuffer3D[i].TryGetComponent(out Targetable target))
                {
                    if (target.IsAlive || config.CanTargetDeathUnit)
                    {
                        result.Add(target);
                    }
                }
            }
            return result;
        }
        public static float GetFinalEffectAmount(AbilityContext context)
        {
            AbilityConfig config = context.AbilityDeliver.Config;
            
            Unit ownerObject = context.SkillContext.ModuleContext.UnitOwner;
            int ownerLevel = ownerObject.Progression.Level;
            StatField ownerStats = ownerObject.Config.MainStats.GetFinalStat(ownerLevel);

            int skillLevel = context.SkillContext.Activator.Progression.Level;
            EffectField baseEffe = config.EffectCalculator.BaseAmount;
            EffectField scaleEffe = config.EffectCalculator.ScaleByLevel;
            EffectField finalEffect = EffectField.GetFinalEffect(baseEffe, scaleEffe, skillLevel);
            float finalScaleAmount = finalEffect.InitialDamage + finalEffect.InitialDamage * finalEffect.DamageMultiplier;

            ScalingStat scalingStat = config.EffectCalculator.ScaleBy;
            switch (scalingStat)
            {
                case ScalingStat.Attack:
                    finalScaleAmount = ownerStats.Attack * finalEffect.DamageMultiplier + finalEffect.InitialDamage;
                    break;
                case ScalingStat.Health:
                    finalScaleAmount = ownerStats.Health * finalEffect.DamageMultiplier + finalEffect.InitialDamage;
                    break;
                case ScalingStat.Defense:
                    finalScaleAmount = ownerStats.Defense * finalEffect.DamageMultiplier + finalEffect.InitialDamage;
                    break;

            }
            
            return finalScaleAmount;
        }
    }
    
}
