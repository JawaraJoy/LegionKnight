using System.Collections.Generic;
using UnityEngine;

namespace Rush
{
    [System.Serializable]
    public partial class AbilitySetUpField
    {
        [SerializeField]
        private AbilityPurpose m_Purpose = AbilityPurpose.Damaging;
        [SerializeField]
        private ScaningMethod m_ScaningMethod = ScaningMethod.ScanOnce;
        [SerializeField]
        private TargetPriority m_Priority = TargetPriority.Nearest;
        [SerializeField]
        private LayerMask m_LayerMask = ~0;
        [SerializeField]
        private float m_InitialDelay = 0f;
        [SerializeField]
        private float m_ScanInterval = 1f;
        [SerializeField]
        private float m_Range = 5f;
        [SerializeField]
        private int m_MaxTargetCount = 1;
        [SerializeField]
        private AbilityActivator m_Activator;
        public AbilityActivator Activator => m_Activator;

        [SerializeField]
        private EffectCalculatorField m_EffectCalculator;

        public AbilityPurpose Purpose => m_Purpose;
        public ScaningMethod ScaningMethod => m_ScaningMethod;
        public TargetPriority Priority => m_Priority;
        public LayerMask LayerMask => m_LayerMask;
        public float InitialDelay => m_InitialDelay;
        public float ScanInterval => m_ScanInterval;
        public float Range => m_Range;
        public int MaxTargetCount => m_MaxTargetCount;
        public EffectCalculatorField EffectCalculator => m_EffectCalculator;
    }
    
    public static partial class AbilityUltility
    {
        private static readonly Collider[] m_ColliderBuffer = new Collider[128];

        public static List<Targetable> GetTargetables(AbilityContext context, AbilityPurpose purpose)
        {
            AbilitySetUpField abilitySetUp = context.Config.GetAbilitySetUp(purpose);
            Vector3 ownerPosition = context.Owner.transform.position;
            int count = Physics.OverlapSphereNonAlloc(ownerPosition, abilitySetUp.Range, m_ColliderBuffer, abilitySetUp.LayerMask);

            List<Targetable> targetables = new();
            for (int i = 0; i < count; i++)
            {
                if (m_ColliderBuffer[i].TryGetComponent(out Targetable target))
                {
                    if (target.IsAlive)
                    {
                        targetables.Add(target);
                    }
                }
            }
            return targetables;
        }
        public static float GetFinalEffectAmount(AbilityContext context, AbilityPurpose purpose)
        {
            AbilitySetUpField abilitySetUp = context.Config.GetAbilitySetUp(purpose);
            ScalingStat scalingStat = abilitySetUp.EffectCalculator.ScaleBy;
            StatField ownerStats = context.Owner.Config.MainStats.GetFinalStat(context.Owner.Progression.Level);

            EffectField baseEffe = abilitySetUp.EffectCalculator.BaseAmount;
            EffectField scaleEffe = abilitySetUp.EffectCalculator.ScaleByLevel;
            EffectField finalEffect = EffectField.GetFinalEffect(baseEffe, scaleEffe, context.GetLevelScaleByOther(context.Owner.Progression));
            float finalScaleAmount = finalEffect.InitialDamage + finalEffect.InitialDamage * finalEffect.DamageMultiplier;
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
