using UnityEngine;

namespace LegionKnight
{
    [CreateAssetMenu(fileName = "SkillVFXConfig", menuName = "Legion Knight/SkillVFXConfig", order = 1)]
    public class SkillParticleAttractorConfig : ScriptableObject
    {
        [SerializeField]
        private string m_Id = "SkillParticleAttractorConfig";
        public string Id => m_Id;
        [Header("Emission")]
        [Tooltip("Default projectile count for this skill")]
        public int emissionCount = 30;
        [Header("Timing")]
        public float chargeDuration = 0.3f;
        public float releaseDuration = 0.2f;
        public float attractDuration = 1.5f;

        [Header("Movement")]
        public float maxSpeed = 18f;
        public float arriveDistance = 0.12f;
        public float distanceNormalization = 6f;

        [Header("Acceleration Curve")]
        public AnimationCurve accelerationCurve =
            AnimationCurve.EaseInOut(0, 1, 1, 0);

        [Header("Spiral")]
        public float spiralStrength = 3f;
        public float spiralFrequency = 6f;
    }
}
