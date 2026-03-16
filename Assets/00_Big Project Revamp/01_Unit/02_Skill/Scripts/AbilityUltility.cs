using LegionKnight;
using System.Collections.Generic;
using UnityEngine;

namespace Rush
{
    public static partial class AbilityUltility
    {
        private static readonly Collider[] m_ColliderBuffer3D = new Collider[32];
        private static readonly Collider2D[] m_ColliderBuffer2D = new Collider2D[32];

        private static GameConfig m_GameConfig;
        private static PhysicsMode GetPhysicsModeInternal()
        {
            if (m_GameConfig == null)
            {
                GameConfig gameConfig = RushGameManager.Instance.GameConfig;
                m_GameConfig = gameConfig;
            }
            return m_GameConfig.PhysicsMode;
        }
        private static List<ITargetable> GetTargetables(AbilityContext context)
        {

            IAbilityDeliver deliver = context.AbilityDeliver;


            return GetPhysicsModeInternal() switch
            {
                PhysicsMode.Physics2D => GetTargetables2DInternal(deliver),
                PhysicsMode.Physics3D => GetTargetables3DInternal(deliver),
                _ => new List<ITargetable>()
            };
        }
        private static IDamageable GetDamageable(ITargetable targetable)
        {
            if (targetable.ModuleContext.Unit.HasBind(out IDamageable damageable))
            {
                return damageable;
            }
            else
            {
                return null;
            }
        }
        private static List<ITargetable> GetTargetables2DInternal(IAbilityDeliver deliver)
        {
            AbilityConfig deliverConfig = deliver.AbilityConfig;
            Vector3 deliverPost = deliver.DeliverTransform.position;

            List<ITargetable> result = new();

            ContactFilter2D filter = new()
            {
                useLayerMask = true,
                layerMask = deliverConfig.TargetFilter,
                useTriggers = true
            };

            int count = Physics2D.OverlapCircle(deliverPost, deliverConfig.Range, filter, m_ColliderBuffer2D);

            for (int i = 0; i < count; i++)
            {
                if (m_ColliderBuffer2D[i].TryGetComponent(out ITargetable target))
                {
                    if (!(target.IsAlive || deliverConfig.CanTargetDeathUnit))
                        continue;

                    if (!IsTargetAllowedByTargetObjectInternal(deliver, target))
                        continue;

                    result.Add(target);
                }
            }
            return result;
        }

        private static List<ITargetable> GetTargetables3DInternal(IAbilityDeliver deliver)
        {
            AbilityConfig deliverConfig = deliver.AbilityConfig;
            Vector3 deliverPost = deliver.DeliverTransform.position;

            List<ITargetable> result = new();
            int count = Physics.OverlapSphereNonAlloc(deliverPost, deliverConfig.Range, m_ColliderBuffer3D, deliverConfig.TargetFilter);

            for (int i = 0; i < count; i++)
            {
                if (m_ColliderBuffer3D[i].TryGetComponent(out ITargetable target))
                {
                    if (!(target.IsAlive || deliverConfig.CanTargetDeathUnit))
                        continue;

                    if (!IsTargetAllowedByTargetObjectInternal(deliver, target))
                        continue;

                    result.Add(target);
                }
            }
            return result;
        }
        public static bool IsTargetAllowedByTargetObject(IAbilityDeliver deliver, ITargetable targetable)
        {
            return IsTargetAllowedByTargetObjectInternal(deliver, targetable);
        }
        private static bool IsTargetAllowedByTargetObjectInternal(IAbilityDeliver deliver, ITargetable targetable)
        {
            if (deliver == null || targetable == null)
                return false;

            AbilityConfig config = deliver.AbilityConfig;
            if (config == null)
                return false;

            if (config.TargetObject == TargetObject.All)
                return true;

            Unit owner = GetOwnerUnit(deliver);
            Unit targetUnit = GetTargetUnit(targetable);

            if (owner == null || targetUnit == null)
                return false;

            // Self
            if (config.TargetObject == TargetObject.Self)
                return owner == targetUnit;

            FactionConfig ownerFaction = owner.Config.Faction;
            FactionConfig targetFaction = targetUnit.Config.Faction;

            bool isSameFaction = ownerFaction == targetFaction;

            switch (config.TargetObject)
            {
                case TargetObject.Enemy:
                    return !isSameFaction;

                case TargetObject.Ally:
                    return isSameFaction && owner != targetUnit;

                case TargetObject.Player:
                    return targetUnit.IsPlayer;

                default:
                    return false;
            }
        }
        private static Unit GetOwnerUnit(IAbilityDeliver deliver)
        {
            return deliver.AbilityContext.SkillContext.ModuleContext.Unit;
        }

        private static Unit GetTargetUnit(ITargetable targetable)
        {
            if (targetable == null)
                return null;

            return targetable.ModuleContext.Unit;
        }
        public static List<ITargetable> ApplyTargetPriority(AbilityContext context)
        {
            IAbilityDeliver deliver = context.AbilityDeliver;
            List<ITargetable> candidates = GetTargetables(context);
            if (candidates == null || candidates.Count == 0)
                return candidates;

            ApplyContextFilters(deliver, candidates);

            AbilityConfig config = deliver.AbilityConfig;
            TargetPriority priority = config.TargetPriority;
            int maxCount = config.UseAllTargetsInRange 
                ? Mathf.Min(candidates.Count, config.MaxTargetCount)  // ambil semua tapi max tetap berlaku
                : Mathf.Max(1, config.MaxTargetCount);

            Vector3 origin = deliver.DeliverTransform.position;

            switch (priority)
            {
                case TargetPriority.Nearest:
                    candidates.Sort((a, b) =>
                        (a.TargetTransform.position - origin).sqrMagnitude
                        .CompareTo((b.TargetTransform.position - origin).sqrMagnitude));
                    break;

                case TargetPriority.Farthest:
                    candidates.Sort((a, b) =>
                        (b.TargetTransform.position - origin).sqrMagnitude
                        .CompareTo((a.TargetTransform.position - origin).sqrMagnitude));
                    break;

                case TargetPriority.Random:
                    Shuffle(candidates);
                    break;

                case TargetPriority.LowestHealth:
                    candidates.Sort((a, b) =>
                        GetHealth(a).CompareTo(GetHealth(b)));
                    break;

                case TargetPriority.HighestHealth:
                    candidates.Sort((a, b) =>
                        GetHealth(b).CompareTo(GetHealth(a)));
                    break;

                case TargetPriority.LowestHealthRate:
                    candidates.Sort((a, b) =>
                        GetHealthRate(a).CompareTo(GetHealthRate(b)));
                    break;

                case TargetPriority.HighestHealthRate:
                    candidates.Sort((a, b) =>
                        GetHealthRate(b).CompareTo(GetHealthRate(a)));
                    break;
            }

            if (candidates.Count > maxCount)
                candidates.RemoveRange(maxCount, candidates.Count - maxCount);

            return candidates;
        }
        private static int GetHealth(ITargetable target)
        {
            IDamageable dmg = GetDamageable(target);
            return dmg != null ? dmg.Health : int.MaxValue;
        }

        private static float GetHealthRate(ITargetable target)
        {
            IDamageable dmg = GetDamageable(target);
            return dmg != null ? dmg.HealthRate : float.MaxValue;
        }
        private static void Shuffle<T>(List<T> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                int j = Random.Range(i, list.Count);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }
        public static AttackerField GetAttacker(IAbilityContext context)
        {
            float attackPower = GetFinalPowerAmountInternal(context);
            int roundAttackPower = Mathf.CeilToInt(attackPower);
            float damageBaseTargetMaxHp = 0;
            DamageType damageType = DamageType.CompareWithDefense;
            AbilityConfig abilityConfig = context.AbilityDeliver.AbilityConfig;
            if (abilityConfig is DamageAbilityConfig damageAbilityConfig)
            {
                damageBaseTargetMaxHp = damageAbilityConfig.DamageBasedTargetMaxHP;
                damageType = damageAbilityConfig.DamageType;
            }
            AttackerField attackerField = new AttackerField(roundAttackPower, damageBaseTargetMaxHp, damageType);
            return attackerField;
        }

        private static float GetFinalPowerAmountInternal(IAbilityContext context)
        {
            AbilityConfig config = context.AbilityDeliver.AbilityConfig;

            Unit ownerObject = context.SkillContext.ModuleContext.Unit;
            int ownerLevel = ownerObject.Progression.Level;
            StatField ownerStats = ownerObject.Config.MainStats.GetFinalStat(ownerLevel);
            if (ownerObject.HasBind(out StatController statController))
            {
                StatField controllerFinalStat = statController.GetFinalStat(ownerStats);
                ownerStats = controllerFinalStat;
            }

            int skillLevel = context.SkillContext.Skill.Progression.Level;
            PowerField basePower = config.Power.BaseAmount;
            PowerField scalePower = config.Power.ScaleByLevel;
            PowerField finaPower = PowerField.GetFinalPower(basePower, scalePower, skillLevel);
            float finalScaleAmount = finaPower.InitialAmount + finaPower.InitialAmount * finaPower.MultiplierAmount;

            ScalingWithStat scalingStat = config.Power.ScaleBy;
            switch (scalingStat)
            {
                case ScalingWithStat.Attack:
                    finalScaleAmount = ownerStats.Attack * finaPower.MultiplierAmount + finaPower.InitialAmount;
                    break;
                case ScalingWithStat.Health:
                    finalScaleAmount = ownerStats.Health * finaPower.MultiplierAmount + finaPower.InitialAmount;
                    break;
                case ScalingWithStat.Defense:
                    finalScaleAmount = ownerStats.Defense * finaPower.MultiplierAmount + finaPower.InitialAmount;
                    break;

            }

            return finalScaleAmount;
        }
        public static float GetFinalPowerAmount(IAbilityContext context)
        {
            return GetFinalPowerAmountInternal(context);
        }
        private static void ApplyContextFilters(IAbilityDeliver deliver, List<ITargetable> candidates)
        {
            AbilityConfig config = deliver.AbilityConfig;

            if (config.UseForwardCone)
                FilterByCone(deliver.DeliverTransform, config.ConeAngle, candidates);

            if (config.RequireLineOfSight)
                FilterByLineOfSight(deliver.DeliverTransform.position, candidates);
        }
        private static void FilterByCone(Transform origin, float coneAngle, List<ITargetable> targets)
        {
            Vector3 forward = origin.forward;
            float cosLimit = Mathf.Cos(coneAngle * 0.5f * Mathf.Deg2Rad);

            for (int i = targets.Count - 1; i >= 0; i--)
            {
                Vector3 dir = (targets[i].TargetTransform.position - origin.position).normalized;
                float dot = Vector3.Dot(forward, dir);

                if (dot < cosLimit)
                    targets.RemoveAt(i);
            }
        }
        private static void FilterByLineOfSight(Vector3 origin, List<ITargetable> targets)
        {
            PhysicsMode mode = GetPhysicsModeInternal();

            for (int i = targets.Count - 1; i >= 0; i--)
            {
                Vector3 targetPos = targets[i].TargetTransform.position;
                bool blocked;

                if (mode == PhysicsMode.Physics2D)
                {
                    RaycastHit2D hit = Physics2D.Linecast(origin, targetPos);
                    blocked = hit.collider != null &&
                              !hit.collider.TryGetComponent<ITargetable>(out _);
                }
                else
                {
                    if (Physics.Linecast(origin, targetPos, out RaycastHit hit))
                        blocked = !hit.collider.TryGetComponent<ITargetable>(out _);
                    else
                        blocked = false;
                }

                if (blocked)
                    targets.RemoveAt(i);
            }
        }
        public static void OnSkillEventActivates(IHasSkills skillOwner, ForceActiveState filterState)
        {
            List<Skill> activators = new(skillOwner.Skills);
            foreach (var activator in activators)
            {
                ForceActiveState state = activator.SkillConfig.Activation.ForceActiveState;
                if (state == filterState)
                {
                    activator.ForceActivateAll();
                }
            }

        }
        public static void OnAbilityDeliveredInvoke(IAbilityContext abilityOwner, Unit unitReceiver)
        {
            Unit unitDeliver = abilityOwner.SkillContext.ModuleContext.Unit;
            
            if (unitDeliver == null)
            {
                Debug.LogError($"{nameof(OnAbilityDeliveredInvoke)} cant found Unit component");
                return;
            }
            abilityOwner.SkillContext.Skill.OnAbilityDelivered?.Invoke(unitDeliver);
            ApplyStatusEffect(abilityOwner, unitReceiver);
        }
       
        private static void ApplyStatusEffect(IAbilityContext senderContext, Unit unitTarget)
        {
            AbilityConfig abilityConfig = senderContext.AbilityDeliver.AbilityConfig;
            StatusEffectConfig[] statusEffects = abilityConfig.StatusEffectOnDelivered;
            
            
            if (unitTarget.HasBind(out StatusEffectController controller))
            {
                if (statusEffects.Length >= 0)
                {
                    foreach (var effect in statusEffects)
                    {
                        controller.ApplyEffector(effect, senderContext, unitTarget);
                    }
                }
            }
            if (senderContext.SkillContext.ModuleContext.Unit.HasBind(out StatusEffectController selfController))
            {
                StatusEffectConfig[] selfEffects = abilityConfig.StatusEffectOnSelf;
                if (selfEffects.Length >= 0)
                {
                    foreach (var effect in selfEffects)
                    {
                        selfController.ApplyEffector(effect, senderContext, senderContext.SkillContext.ModuleContext.Unit);
                    }
                }
            }
        }
        public static void LookAtFirstTarget2D(Transform subject, ITargetable targetable)
        {
            if (targetable == null)
                return;

            Vector2 direction = targetable.TargetTransform.position - subject.position;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;

            subject.rotation = Quaternion.Euler(0f, 0f, angle);
        }
    }
}
