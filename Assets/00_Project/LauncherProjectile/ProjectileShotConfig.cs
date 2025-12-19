using UnityEngine;

namespace LegionKnight
{
    [System.Serializable]
    public class ProjectileShotConfig
    {
        [Header("Prefab")]
        [Tooltip("Projectile prefab to spawn for this shot")]
        public ProjectileBase ProjectilePrefab;

        [Header("Direction")]
        [Tooltip("Local firing direction relative to launcher")]
        public Vector3 LocalDirection = Vector3.right;

        [Tooltip("If true, direction is rotated by launcher rotation")]
        public bool UseLauncherRotation = true;

        [Header("Movement")]
        [Tooltip("Movement type: straight, spiral, or curve")]
        public MotionType Motion;

        [Tooltip("Base movement speed")]
        public float Speed = 15f;

        [Tooltip("Optional speed multiplier over lifetime")]
        public AnimationCurve SpeedCurve;

        [Tooltip("Optional curve offset over lifetime")]
        public AnimationCurve Curve;

        [Header("Spiral")]
        [Tooltip("Radius of spiral offset")]
        public float SpiralRadius;

        [Tooltip("Angular speed of spiral motion")]
        public float SpiralSpeed;

        [Header("Homing")]
        [Tooltip("Enable homing behavior")]
        public bool Homing;

        [Tooltip("How fast projectile turns toward target")]
        public float TurnSpeed = 8f;

        [Header("Hit Behavior")]
        [Tooltip("What happens when projectile hits something")]
        public HitBehavior HitBehavior;

        [Tooltip("How many targets projectile can pierce")]
        public int PierceCount = 1;

        [Tooltip("Explosion radius if using Explode behavior")]
        public float ExplosionRadius;

        [Header("Lifetime")]
        [Tooltip("If true, projectile auto-despawns after lifetime")]
        public bool UseLifetime = true;

        [Tooltip("Lifetime duration in seconds")]
        public float Lifetime = 5f;
    }
}
