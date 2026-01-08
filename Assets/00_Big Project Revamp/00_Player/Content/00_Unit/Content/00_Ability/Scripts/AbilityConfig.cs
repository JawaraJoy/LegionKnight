using System.Collections.Generic;
using UnityEngine;

namespace Rush
{
    public abstract partial class AbilityConfig : Configuration
    {
        [SerializeField]
        private Sprite m_Icon;
        [SerializeField]
        private LayerMask m_TargetFilter = ~0;
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
        [SerializeField]
        private AbilityDeliver m_Deliver;
        public AbilityDeliver Deliver => m_Deliver;

        [SerializeField]
        private EffectCalculatorField m_EffectCalculator;

        public Sprite Icon => m_Icon;
        public TargetPriority TargetPriority => m_TargetPriority;
        public bool CanTargetDeathUnit => m_CanTargetDeathUnit;
        public LayerMask TargetFilter => m_TargetFilter;
        public float InitialDelay => m_InitialDelay;
        public float Range => m_Range;
        public int MaxTargetCount => m_MaxTargetCount;
        public EffectCalculatorField EffectCalculator => m_EffectCalculator;
    }
    
    public static partial class AbilityUltility
    {
        private static readonly Collider[] m_ColliderBuffer = new Collider[128];
        public static List<Targetable> GetTargetables(AbilityContext context)
        {
            AbilityConfig config = context.AbilityDeliver.Config;
            Vector3 deliverPosition = context.AbilityDeliver.transform.position;
            int count = Physics.OverlapSphereNonAlloc(deliverPosition, config.Range, m_ColliderBuffer, config.TargetFilter);

            TargetPriority targetPriority = context.AbilityDeliver.Config.TargetPriority;


            int maxTargetCount = config.MaxTargetCount;
            List<Targetable> targetables = new();
            for (int i = 0; i < count; i++)
            {
                if (m_ColliderBuffer[i].TryGetComponent(out Targetable target))
                {
                    if (i + 1 < maxTargetCount)
                    {
                        if (target.IsAlive)
                        {
                            targetables.Add(target);
                        }
                    }
                }
            }
            return targetables;
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
