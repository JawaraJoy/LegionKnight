using UnityEngine;

namespace Rush
{
    public sealed class LiuraChargeVfx : MonoBehaviour
    {
        [SerializeField] private Charger m_Charger;
        [SerializeField] private SpriteRenderer m_Energy;
        [SerializeField] private SpriteRenderer m_Glint;
        [SerializeField] private bool m_FromPlatform;
        [SerializeField] private float m_Duration = 0.55f;
        [SerializeField] private float m_Width = 1.5f;
        [SerializeField] private float m_SourceOffsetY;
        private const float c_Arrival = 0.74f;
        private const float c_GlintWidth = 1.55f;
        private const float c_ArcHeight = 1.85f;
        private const float c_BodyHeight = 0.42f;
        private Vector3 m_Origin;
        [SerializeField] private LiuraCometTrail[] m_Trails;
        [SerializeField] private SpriteRenderer[] m_OrbitLights;
        private Unit m_Owner;
        private Renderer m_Body;
        private float m_Elapsed;
        private bool m_Playing;

        // Captured before the charger yields for any configured initial delay.
        public void OnChargeStart(AbilityContext context)
        {
            var deliver = context.AbilityDeliver as AbilityDeliver;
            m_Origin = deliver?.ActivationOrigin ?? transform.position;
            m_Origin.y += m_SourceOffsetY;
        }

        // The native charge amount event fires only for a living recipient.
        public void OnCharge()
        {
            PlayInternal();
        }

        private void PlayInternal()
        {
            AbilityContext context = m_Charger.AbilityContext;
            m_Owner = context.SkillContext.ModuleContext.Unit;
            m_Body = null;
            if (!m_Owner.HasBind(out AvatarSpine avatar))
                avatar = m_Owner.GetComponentInChildren<AvatarSpine>(true);
            if (avatar != null && avatar.SkeletonAnimation != null)
                avatar.SkeletonAnimation.TryGetComponent(out m_Body);
            m_Energy.flipX = !m_FromPlatform && ChestPosition.x < m_Origin.x;
            m_Elapsed = 0f;
            m_Playing = true;
            SampleInternal(0f);
            foreach (LiuraCometTrail trail in m_Trails) trail.Begin(trail.transform.position);
        }

        private Vector3 ChestPosition
        {
            get
            {
                if (m_Body == null || m_Body.bounds.size.y <= Mathf.Epsilon)
                    return m_Owner.transform.position + Vector3.up;
                Bounds bounds = m_Body.bounds;
                return new Vector3(m_Owner.transform.position.x,
                    Mathf.Lerp(bounds.min.y, bounds.max.y, c_BodyHeight), bounds.center.z);
            }
        }

        private void LateUpdate()
        {
            if (!m_Playing) return;
            if (m_Owner == null || !m_Owner.gameObject.activeInHierarchy)
            {
                HideInternal();
                return;
            }
            m_Elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(m_Elapsed / m_Duration);
            SampleInternal(t);
            if (t >= 1f) HideInternal();
        }

        private void SampleInternal(float t)
        {
            float travel = Mathf.Clamp01(t / c_Arrival);
            float eased = Mathf.SmoothStep(0f, 1f, travel);
            Vector3 chest = ChestPosition;
            Vector3 path = Vector3.Lerp(m_Origin, chest, eased);
            Vector3 energyScale = Vector3.one * (m_Width / m_Energy.sprite.bounds.size.x);
            if (m_FromPlatform)
            {
                // Grow the ribbon from the jewel, then draw its lower tip up into the hero.
                Vector3 tail = Vector3.Lerp(m_Origin, chest, Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(0.52f, 1f, travel)));
                energyScale.y = Mathf.Max(0.01f, Vector3.Distance(tail, path)) / m_Energy.sprite.bounds.size.y;
                energyScale.x *= Mathf.Sin(Mathf.PI * travel);
                m_Energy.transform.rotation = Quaternion.FromToRotation(Vector3.up, (path - tail).sqrMagnitude > 0.0001f ? path - tail : Vector3.up);
                path = (tail + path) * 0.5f;
            }
            else
            {
                path.y += Mathf.Sin(Mathf.PI * travel) * c_ArcHeight;
                energyScale *= Mathf.Lerp(1f, 0.45f, eased);
            }
            m_Energy.enabled = travel < 1f;
            m_Energy.transform.position = path;
            m_Energy.transform.localScale = energyScale;
            SampleStreamsInternal(t, travel, path, chest);
            bool arrival = t >= c_Arrival;
            float glint = arrival ? Mathf.Sin(Mathf.PI * Mathf.InverseLerp(c_Arrival, 1f, t))
                : Mathf.Clamp01(1f - travel * 4f);
            m_Glint.transform.position = arrival ? chest : m_Origin;
            m_Glint.enabled = glint > 0f;
            m_Glint.transform.localScale = Vector3.one * (glint * c_GlintWidth / m_Glint.sprite.bounds.size.x);
        }


        private void ResetTrailsInternal()
        {
            foreach (LiuraCometTrail trail in m_Trails) trail.Clear();
        }

        private void SampleStreamsInternal(float t, float travel, Vector3 path, Vector3 chest)
        {
            if (m_FromPlatform)
            {
                Vector3 center = Vector3.Lerp(m_Origin, chest, travel);
                float radius = Mathf.Sin(travel * Mathf.PI) * m_Width * 0.58f;
                for (int i = 0; i < m_OrbitLights.Length; i++)
                {
                    float phase = travel * 2.5f * Mathf.PI + i * Mathf.PI;
                    SpriteRenderer light = m_OrbitLights[i];
                    light.transform.position = center + new Vector3(Mathf.Cos(phase), 0.30f * Mathf.Sin(phase), 0f) * radius;
                    light.enabled = travel < 1f;
                    light.sortingOrder = Mathf.Sin(phase) > 0f ? 13 : 8;
                    float width = Mathf.Lerp(0.36f, 0.57f, Mathf.Sin(travel * Mathf.PI));
                    light.transform.localScale = Vector3.one * (width / light.sprite.bounds.size.x);
                }
            }
            else
            {
                m_Trails[0].transform.position = path;
            }
            if (t >= c_Arrival)
                foreach (LiuraCometTrail trail in m_Trails) trail.EndEmission();
        }

        private void OnDisable() => HideInternal();

        private void HideInternal()
        {
            m_Playing = false;
            ResetTrailsInternal();
            foreach (SpriteRenderer light in m_OrbitLights) light.enabled = false;
            m_Energy.enabled = false;
            m_Glint.enabled = false;
        }
    }
}
