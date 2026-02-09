using UnityEngine;
using System.Collections.Generic;
using MoreMountains.Tools;

namespace LegionKnight
{
    [RequireComponent(typeof(ParticleSystem))]
    public class SkillProjectileParticleController : MonoBehaviour
    {
        [Header("Config")]
        [SerializeField] private SkillParticleAttractorConfig m_Config;
        private ParticleSystem.EmissionModule m_Emission;
        private int m_OverrideEmissionCount = -1;

        [Header("Targets")]
        [SerializeField] private List<Transform> m_Targets = new();

        private ParticleSystem m_PS;
        private ParticleSystem.Particle[] m_Particles;
        private List<Vector4> m_CustomData;

        [SerializeField, MMReadOnly]
        private SkillVFXState m_State;
        private float m_StateTimer;

        void Awake()
        {
            m_PS = GetComponent<ParticleSystem>();
            m_Emission = m_PS.emission;

            m_Particles = new ParticleSystem.Particle[m_PS.main.maxParticles];
            m_CustomData = new List<Vector4>(m_PS.main.maxParticles);

            var custom = m_PS.customData;
            custom.enabled = true;
            custom.SetMode(
                ParticleSystemCustomData.Custom1,
                ParticleSystemCustomDataMode.Vector
            );
        }

        void OnEnable()
        {
            SetState(SkillVFXState.Charge);
        }

        void LateUpdate()
        {
            UpdateState();

            if (m_State != SkillVFXState.Attract)
                return;

            int count = m_PS.GetParticles(m_Particles);
            m_PS.GetCustomParticleData(m_CustomData, ParticleSystemCustomData.Custom1);

            for (int i = 0; i < count; i++)
            {
                int targetIndex = (int)m_CustomData[i].x;
                if (targetIndex < 0 || targetIndex >= m_Targets.Count)
                    continue;

                Transform target = m_Targets[targetIndex];
                if (!target) continue;

                Vector3 pos = m_Particles[i].position;
                Vector3 toTarget = target.position - pos;

                float distance = toTarget.magnitude;

                if (distance <= m_Config.arriveDistance)
                {
                    m_Particles[i].remainingLifetime = 0f;
                    continue;
                }

                float normalizedDistance =
                    Mathf.Clamp01(distance / m_Config.distanceNormalization);

                float speedMultiplier =
                    m_Config.accelerationCurve.Evaluate(normalizedDistance);

                Vector3 direction = toTarget.normalized;

                // 🌪 Spiral (locked per particle)
                float spiralSeed = m_CustomData[i].y;
                Vector3 perpendicular =
                    Vector3.Cross(direction, Vector3.up).normalized;

                float spiral =
                    Mathf.Sin(Time.time * m_Config.spiralFrequency + spiralSeed) *
                    m_Config.spiralStrength * normalizedDistance;

                direction += perpendicular * spiral;
                direction.Normalize();

                pos += direction *
                       (m_Config.maxSpeed * speedMultiplier * Time.deltaTime);

                m_Particles[i].position = pos;
            }

            m_PS.SetParticles(m_Particles, count);
        }

        // 🔐 TARGET LOCKING AT SPAWN
        void OnParticleSystemStopped()
        {
            m_CustomData.Clear();
        }

        void OnParticleTrigger()
        {
            // Optional hook if needed
        }

        private void ApplyEmissionCount(int count)
        {
            count = Mathf.Max(1, count);

            if (m_Emission.burstCount > 0)
            {
                ParticleSystem.Burst burst = m_Emission.GetBurst(0);
                burst.count = count;
                m_Emission.SetBurst(0, burst);
            }
        }

        public void SetEmissionCount(int count)
        {
            m_OverrideEmissionCount = Mathf.Max(1, count);
        }

        private void LockTargetsForNewParticles()
        {
            int count = m_PS.particleCount;
            m_PS.GetCustomParticleData(m_CustomData, ParticleSystemCustomData.Custom1);

            while (m_CustomData.Count < count)
            {
                int targetIndex = Random.Range(0, m_Targets.Count);
                float spiralSeed = Random.Range(0f, 1000f);

                m_CustomData.Add(new Vector4(
                    targetIndex,
                    spiralSeed,
                    0f,
                    0f
                ));
            }

            m_PS.SetCustomParticleData(m_CustomData, ParticleSystemCustomData.Custom1);
        }

        // 🔄 SKILL STATE MACHINE
        void UpdateState()
        {
            m_StateTimer += Time.deltaTime;

            switch (m_State)
            {
                case SkillVFXState.Charge:
                    if (m_StateTimer >= m_Config.chargeDuration)
                        SetState(SkillVFXState.Release);
                    break;

                case SkillVFXState.Release:
                    if (m_StateTimer >= m_Config.releaseDuration)
                        SetState(SkillVFXState.Attract);
                    break;

                case SkillVFXState.Attract:
                    if (m_StateTimer >= m_Config.attractDuration)
                        SetState(SkillVFXState.End);
                    break;
            }
        }

        private void SetState(SkillVFXState newState)
        {
            m_State = newState;
            m_StateTimer = 0f;

            if (newState == SkillVFXState.Release)
            {
                m_PS.Play();
                LockTargetsForNewParticles();
            }

            if (newState == SkillVFXState.End)
            {
                m_PS.Stop();
            }
        }

        // 🔌 API
        public void SetTargets(List<Transform> targets)
        {
            m_Targets = targets;
        }

        public void PlayParticle(List<Transform> targets)
        {
            if (targets == null || targets.Count == 0)
                return;

            m_Targets = targets;

            m_StateTimer = 0f;
            m_State = SkillVFXState.None;

            m_PS.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            m_CustomData.Clear();

            int emissionCount =
                m_OverrideEmissionCount > 0
                    ? m_OverrideEmissionCount
                    : m_Config.emissionCount;

            ApplyEmissionCount(emissionCount);

            SetState(SkillVFXState.Charge);
        }
    }

    public enum SkillVFXState
    {
        None,
        Charge,
        Release,
        Attract,
        End
    }
}
