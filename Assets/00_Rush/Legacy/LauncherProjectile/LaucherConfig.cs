using UnityEngine;


namespace LegionKnight
{
    [CreateAssetMenu(fileName = "Launcher", menuName = "Legion Knight/Combat/Launcher")]
    public class LauncherConfig : ScriptableObject
    {
        [Tooltip("Physics simulation mode used by all projectiles")]
        public SimulationMode Simulation;
        [Tooltip("Ordered list of projectiles fired by this launcher")]
        public ProjectileShotConfig[] Projectiles;
        [Tooltip("Delay between spawning each projectile")]
        public float FireInterval;
        [Tooltip("If true, launcher will not fire without a target")]
        public bool RequireTarget;
    }

    /// <summary>Determines whether projectile uses 2D or 3D physics.</summary>
    public enum SimulationMode { Mode2D, Mode3D }
    /// <summary>Defines how projectile reacts on hit.</summary>
    public enum HitBehavior { Destroy, Pierce, Explode, Stick }
    /// <summary>Defines projectile movement pattern.</summary>
    public enum MotionType { Straight, Spiral, Curve }
}