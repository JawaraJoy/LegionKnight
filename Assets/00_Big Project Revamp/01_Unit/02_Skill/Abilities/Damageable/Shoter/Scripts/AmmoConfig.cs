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

        [SerializeField, Header("Flight Noise")]
        private float m_SwayAmplitude = 0f;
        [SerializeField]
        private float m_SwayFrequency = 5f;

        [SerializeField, Header("Arc")]
        private float m_ArcHeight = 0f;

        [SerializeField, Header("Targeting")]
        [Tooltip("If true, ammo instantly rotates to face its target when Shot() is called")]
        private bool m_LookAtTargetOnShot = false;
        [SerializeField]
        private float m_HomingDelay = 0f;
        [SerializeField]
        private float m_InitialWanderAngle = 0f;

        [SerializeField, Header("Movement")]
        protected LocalAxis m_ForwardAxis = LocalAxis.Y;
        [SerializeField]
        private LayerMask m_TargetLayer;
        [SerializeField]
        protected TargetingMode m_TargetingDistributeMode = TargetingMode.None;
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

        public Ammo AmmoPrefab => m_AmmoPrefab;
        public bool LookAtTargetOnShot => m_LookAtTargetOnShot;
        public LocalAxis ForwardAxis => m_ForwardAxis;
        public TargetingMode TargetingDistributeMode => m_TargetingDistributeMode;
        public LayerMask TargetLayer => m_TargetLayer;
        public float HomingTurnSpeed => m_HomingTurnSpeed;
        public float Speed => m_Speed;
        public float Lifetime => m_Lifetime;
        public float MaxDistance => m_MaxDistance;
        public float SwayAmplitude => m_SwayAmplitude;
        public float SwayFrequency => m_SwayFrequency;
        public float ArcHeight => m_ArcHeight;
        public float HomingDelay => m_HomingDelay;
        public float InitialWanderAngle => m_InitialWanderAngle;
    }
}