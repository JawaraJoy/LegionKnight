using System;
using UnityEngine;

namespace Rush
{
    public sealed class LiuraProjectileVfx : MonoBehaviour
    {
        [Serializable]
        private struct WandPoint
        {
            [SerializeField] private string m_SlotName;
            [SerializeField] private Vector2 m_LocalPoint;
            public string SlotName => m_SlotName;
            public Vector2 LocalPoint => m_LocalPoint;
        }

        [SerializeField] private VFXPool m_Muzzle;
        [SerializeField] private LineRenderer m_Trail;
        [SerializeField] private LiuraCometTrail m_Comet;
        [SerializeField] private WandPoint[] m_WandPoints;
        [SerializeField] private AbilityConfig[] m_MissileStages;
        [SerializeField] private float m_FirstStageAlpha = 0.45f;
        [SerializeField] private float m_StageWidthStep = 0.22f;
        [SerializeField] private float m_BaseTrailWidth = 1.08f;
        private const float c_StageAlphaStep = 0.04f;

        // Bound to Ammo.OnShot, after the pooled projectile has been positioned.
        public void OnShot(AbilityContext context)
        {
            PlayShotInternal(context);
        }

        private void PlayShotInternal(AbilityContext context)
        {
            m_Comet.Begin(m_Comet.transform.position);
            int stage = Array.IndexOf(m_MissileStages, context.AbilityDeliver.AbilityConfig);
            if (stage >= 0)
            {
                m_Trail.widthMultiplier = m_BaseTrailWidth + stage * m_StageWidthStep;
                Color core = m_Trail.startColor;
                core.a = Mathf.Clamp01(m_FirstStageAlpha + stage * c_StageAlphaStep);
                m_Trail.startColor = core;
            }
            Unit owner = context.SkillContext.ModuleContext.Unit;
            m_Muzzle.transform.position = WandPosition(owner);
            m_Muzzle.Play();
        }

        private Vector3 WandPosition(Unit owner)
        {
            if (owner == null) return transform.position;
            if (!owner.HasBind(out AvatarSpine avatar))
                avatar = owner.GetComponentInChildren<AvatarSpine>(true);
            if (avatar != null && avatar.SkeletonAnimation != null)
            {
                var animation = avatar.SkeletonAnimation;
                foreach (var point in m_WandPoints)
                {
                    var slot = animation.Skeleton?.FindSlot(point.SlotName);
                    if (slot?.AppliedPose.Attachment == null) continue;
                    slot.Bone.AppliedPose.LocalToWorld(point.LocalPoint.x, point.LocalPoint.y, out float x, out float y);
                    return animation.transform.TransformPoint(new Vector3(x, y));
                }
            }
            return transform.position;
        }
    }
}
