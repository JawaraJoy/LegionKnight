using UnityEngine;

namespace Rush
{
    // The particle player owns the lifetime of this pooled world-space effect.
    public sealed class LiuraImpactVfx : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer m_Flash;
        [SerializeField] private SpriteRenderer m_Ring;
        [SerializeField] private ProjectileConfig m_Explosion;
        [SerializeField] private float m_Duration = 0.22f;
        [SerializeField] private float m_FlashWidth = 1f;
        private const float c_FlashPeak = 0.2f;
        private const float c_RingPeak = 0.62f;
        [SerializeField] private SpriteRenderer[] m_Sparks;
        [SerializeField] private SpriteRenderer m_EchoRing;
        [SerializeField] private float m_SparkRadius;
        [SerializeField] private float m_SparkLength;
        private float m_Elapsed;

        public void Restart()
        {
            RestartInternal();
        }

        private void RestartInternal()
        {
            m_Elapsed = 0f;
            transform.rotation = Quaternion.identity;
            SampleInternal(0f);
        }

        private void LateUpdate()
        {
            m_Elapsed += Time.deltaTime;
            SampleInternal(Mathf.Clamp01(m_Elapsed / m_Duration));
        }

        private void SampleInternal(float t)
        {
            float flash = t < c_FlashPeak ? Mathf.Lerp(0.18f, 1f, t / c_FlashPeak)
                : 1f - Mathf.InverseLerp(c_FlashPeak, 1f, t);
            m_Flash.transform.localScale = Vector3.one * (m_FlashWidth / m_Flash.sprite.bounds.size.x * flash);
            SetAlpha(m_Flash, flash);
            SampleSparksInternal(t);
            if (m_Ring == null) return;
            float expansion = Mathf.SmoothStep(0.1f, 1f, Mathf.Clamp01(t / c_RingPeak));
            float diameter = 2f * m_Explosion.ExplodeSetup.ExplosionRadius * expansion;
            Vector3 size = m_Ring.sprite.bounds.size;
            m_Ring.transform.localScale = new Vector3(diameter / size.x, diameter / size.y, 1f);
            SetAlpha(m_Ring, 1f - Mathf.InverseLerp(c_RingPeak, 1f, t));
        }


        private void SampleSparksInternal(float t)
        {
            float travel = 1f - (1f - t) * (1f - t);
            float visible = Mathf.Sin(Mathf.PI * Mathf.Pow(t, 0.65f));
            for (int i = 0; i < m_Sparks.Length; i++)
            {
                SpriteRenderer spark = m_Sparks[i];
                float angle = (float)i / m_Sparks.Length * 2f * Mathf.PI + 0.15f * travel;
                float stagger = (i & 1) == 0 ? 1f : 0.76f;
                Vector3 direction = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f);
                float distance = m_SparkRadius * stagger * travel;
                spark.transform.localPosition = direction * distance;
                spark.transform.localRotation = Quaternion.Euler(0f, 0f, angle * Mathf.Rad2Deg);
                float length = m_SparkLength * visible * stagger;
                spark.transform.localScale = new Vector3(length / spark.sprite.bounds.size.x,
                    length * 0.34f / spark.sprite.bounds.size.y, 1f);
                SetAlpha(spark, visible);
            }
            if (m_EchoRing == null) return;
            float echo = Mathf.InverseLerp(0.13f, 0.88f, t);
            float diameter = m_Explosion.ExplodeSetup.ExplosionRadius * 2f * Mathf.SmoothStep(0.12f, 0.90f, echo);
            Vector3 size = m_EchoRing.sprite.bounds.size;
            m_EchoRing.transform.localScale = new Vector3(diameter / size.x, diameter / size.y, 1f);
            m_EchoRing.transform.localRotation = Quaternion.Euler(0f, 0f, 28f - 56f * echo);
            SetAlpha(m_EchoRing, Mathf.Sin(echo * Mathf.PI) * 0.85f);
        }

        private void SetAlpha(SpriteRenderer renderer, float alpha)
        {
            Color color = renderer.color;
            color.a = alpha;
            renderer.color = color;
        }
    }
}
