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
        private static List<Targetable> GetTargetables(AbilityContext context)
        {

            AbilityDeliver deliver = context.AbilityDeliver;


            return GetPhysicsModeInternal() switch
            {
                PhysicsMode.Physics2D => GetTargetables2DInternal(deliver),
                PhysicsMode.Physics3D => GetTargetables3DInternal(deliver),
                _ => new List<Targetable>()
            };
        }
        private static Damageable GetDamageable(Targetable targetable)
        {
            if (targetable.HasBind(out Damageable damageable))
            {
                return damageable;
            }
            else
            {
                return null;
            }
        }
        private static List<Targetable> GetTargetables2DInternal(AbilityDeliver deliver)
        {
            AbilityConfig deliverConfig = deliver.Config;
            Vector3 deliverPost = deliver.transform.position;

            List<Targetable> result = new();

            ContactFilter2D filter = new()
            {
                useLayerMask = true,
                layerMask = deliverConfig.TargetFilter,
                useTriggers = true
            };

            int count = Physics2D.OverlapCircle(deliverPost, deliverConfig.Range, filter, m_ColliderBuffer2D);

            for (int i = 0; i < count; i++)
            {
                if (m_ColliderBuffer2D[i].TryGetComponent(out Targetable target))
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

        private static List<Targetable> GetTargetables3DInternal(AbilityDeliver deliver)
        {
            AbilityConfig deliverConfig = deliver.Config;
            Vector3 deliverPost = deliver.transform.position;

            List<Targetable> result = new();
            int count = Physics.OverlapSphereNonAlloc(deliverPost, deliverConfig.Range, m_ColliderBuffer3D, deliverConfig.TargetFilter);

            for (int i = 0; i < count; i++)
            {
                if (m_ColliderBuffer3D[i].TryGetComponent(out Targetable target))
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
        public static bool IsTargetAllowedByTargetObject(AbilityDeliver deliver, Targetable targetable)
        {
            return IsTargetAllowedByTargetObjectInternal(deliver, targetable);
        }
        private static bool IsTargetAllowedByTargetObjectInternal(AbilityDeliver deliver, Targetable targetable)
        {
            if (deliver == null || targetable == null)
                return false;

            AbilityConfig config = deliver.Config;
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
        private static Unit GetOwnerUnit(AbilityDeliver deliver)
        {
            return deliver.AbilityContext.SkillContext.ModuleContext.UnitOwner;
        }

        private static Unit GetTargetUnit(Targetable targetable)
        {
            if (targetable == null)
                return null;

            if (targetable.HasBind(out Unit unit))
                return unit;

            return null;
        }
        public static List<Targetable> ApplyTargetPriority(AbilityContext context)
        {
            AbilityDeliver deliver = context.AbilityDeliver;
            List<Targetable> candidates = GetTargetables(context);
            if (candidates == null || candidates.Count == 0)
                return candidates;

            ApplyContextFilters(deliver, candidates);

            AbilityConfig config = deliver.Config;
            TargetPriority priority = config.TargetPriority;
            int maxCount = Mathf.Max(1, config.MaxTargetCount);

            Vector3 origin = deliver.transform.position;

            switch (priority)
            {
                case TargetPriority.Nearest:
                    candidates.Sort((a, b) =>
                        (a.transform.position - origin).sqrMagnitude
                        .CompareTo((b.transform.position - origin).sqrMagnitude));
                    break;

                case TargetPriority.Farthest:
                    candidates.Sort((a, b) =>
                        (b.transform.position - origin).sqrMagnitude
                        .CompareTo((a.transform.position - origin).sqrMagnitude));
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
        private static int GetHealth(Targetable target)
        {
            Damageable dmg = GetDamageable(target);
            return dmg != null ? dmg.Health : int.MaxValue;
        }

        private static float GetHealthRate(Targetable target)
        {
            Damageable dmg = GetDamageable(target);
            return dmg != null ? dmg.CurrentHealthRate : float.MaxValue;
        }
        private static void Shuffle<T>(List<T> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                int j = Random.Range(i, list.Count);
                (list[i], list[j]) = (list[j], list[i]);
            }
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
            float finalScaleAmount = finalEffect.InitialAmount + finalEffect.InitialAmount * finalEffect.MultiplierAmount;

            ScalingStat scalingStat = config.EffectCalculator.ScaleBy;
            switch (scalingStat)
            {
                case ScalingStat.Attack:
                    finalScaleAmount = ownerStats.Attack * finalEffect.MultiplierAmount + finalEffect.InitialAmount;
                    break;
                case ScalingStat.Health:
                    finalScaleAmount = ownerStats.Health * finalEffect.MultiplierAmount + finalEffect.InitialAmount;
                    break;
                case ScalingStat.Defense:
                    finalScaleAmount = ownerStats.Defense * finalEffect.MultiplierAmount + finalEffect.InitialAmount;
                    break;

            }

            return finalScaleAmount;
        }
        private static void ApplyContextFilters(AbilityDeliver deliver, List<Targetable> candidates)
        {
            AbilityConfig config = deliver.Config;

            if (config.UseForwardCone)
                FilterByCone(deliver.transform, config.ConeAngle, candidates);

            if (config.RequireLineOfSight)
                FilterByLineOfSight(deliver.transform.position, candidates);
        }
        private static void FilterByCone(Transform origin, float coneAngle, List<Targetable> targets)
        {
            Vector3 forward = origin.forward;
            float cosLimit = Mathf.Cos(coneAngle * 0.5f * Mathf.Deg2Rad);

            for (int i = targets.Count - 1; i >= 0; i--)
            {
                Vector3 dir = (targets[i].transform.position - origin.position).normalized;
                float dot = Vector3.Dot(forward, dir);

                if (dot < cosLimit)
                    targets.RemoveAt(i);
            }
        }
        private static void FilterByLineOfSight(Vector3 origin, List<Targetable> targets)
        {
            PhysicsMode mode = GetPhysicsModeInternal();

            for (int i = targets.Count - 1; i >= 0; i--)
            {
                Vector3 targetPos = targets[i].transform.position;
                bool blocked;

                if (mode == PhysicsMode.Physics2D)
                {
                    RaycastHit2D hit = Physics2D.Linecast(origin, targetPos);
                    blocked = hit.collider != null &&
                              !hit.collider.TryGetComponent<Targetable>(out _);
                }
                else
                {
                    if (Physics.Linecast(origin, targetPos, out RaycastHit hit))
                        blocked = !hit.collider.TryGetComponent<Targetable>(out _);
                    else
                        blocked = false;
                }

                if (blocked)
                    targets.RemoveAt(i);
            }
        }
    }
}
