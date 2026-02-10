using UnityEngine;

namespace Rush
{
    public enum LocalAxis
    {
        X,
        Y,
        Z
    }
    public class AmmoConfig : Configuration
    {
        [SerializeField]
        private Ammo m_AmmoPrefab;
        public Ammo AmmoPrefab => m_AmmoPrefab;
        [SerializeField]
        protected LocalAxis m_ForwardAxis = LocalAxis.Y;
        [SerializeField]
        private LayerMask m_TargetLayer;
        [SerializeField]
        protected TargetingDistributeMode m_TargetingDistributeMode = TargetingDistributeMode.None;
        
        [SerializeField]
        [Tooltip("Movement speed in units per second")]
        protected float m_Speed = 10f;
        [SerializeField]
        [Tooltip("How long the Ammo stays alive in seconds (0 = infinite)")]
        protected float m_Lifetime = 0f;
        [SerializeField]
        [Tooltip("Maximum distance the Ammo can travel (0 = infinite)")]
        private float m_MaxDistance = 10f;
        [SerializeField]
        protected float m_HomingTurnSpeed = 90f;
        public LocalAxis ForwardAxis => m_ForwardAxis;
        public TargetingDistributeMode TargetingDistributeMode => m_TargetingDistributeMode;
        public LayerMask TargetLayer => m_TargetLayer;
        public float HomingTurnSpeed => m_HomingTurnSpeed;

        public float Speed => m_Speed;
        public float Lifetime => m_Lifetime;
        public float MaxDistance => m_MaxDistance;
        
    }
}
