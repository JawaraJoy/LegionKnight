using UnityEngine;

namespace Rush
{
    public enum BossEnergyKind { Cast, Launch, Impact, Slash, Shield, Barrier, Summon, Devotion, Projectile }

    [RequireComponent(typeof(MeshRenderer))]
    public sealed class BossEnergyVfx : MonoBehaviour
    {
        private static readonly int s_Color = Shader.PropertyToID("_Tint");
        private static readonly int s_Phase = Shader.PropertyToID("_Phase");
        private static readonly int s_Mode = Shader.PropertyToID("_Mode");
        private const float c_VisualDepth = -0.15f;
        private MeshRenderer m_Renderer;
        private MaterialPropertyBlock m_Properties;
        private Transform m_Follow;
        private Vector3 m_Offset;
        private Vector3 m_Position;
        private float m_Duration;
        private float m_Elapsed;
        private bool m_Attached;

        public void Play(BossEnergyKind kind, Color color, Vector3 position, float size, float duration, Transform follow = null)
            => PlayInternal(kind, color, position, size, duration, follow);

        private void PlayInternal(BossEnergyKind kind, Color color, Vector3 position, float size, float duration, Transform follow)
        {
            if (m_Renderer == null) m_Renderer = GetComponent<MeshRenderer>();
            m_Properties ??= new MaterialPropertyBlock();
            m_Follow = follow;
            m_Offset = follow != null ? position - follow.position : Vector3.zero;
            m_Position = position;
            m_Duration = duration;
            m_Elapsed = 0f;
            m_Attached = kind == BossEnergyKind.Projectile;
            transform.position = position + Vector3.forward * c_VisualDepth;
            transform.localScale = m_Attached ? new Vector3(size, size * 2f, 1f) : Vector3.one * size;
            m_Properties.SetColor(s_Color, color);
            m_Properties.SetFloat(s_Mode, (float)kind);
            m_Properties.SetFloat(s_Phase, 0f);
            m_Renderer.SetPropertyBlock(m_Properties);
            gameObject.SetActive(true);
        }

        public void Stop() => gameObject.SetActive(false);

        private void OnEnable() { m_Elapsed = 0f; }

        private void Update()
        {
            m_Elapsed += Time.deltaTime;
            if (m_Duration > 0f && m_Elapsed >= m_Duration)
            {
                gameObject.SetActive(false);
                return;
            }
            if (!m_Attached)
                transform.position = (m_Follow != null ? m_Follow.position + m_Offset : m_Position) + Vector3.forward * c_VisualDepth;
            float phase = m_Duration > 0f ? m_Elapsed / m_Duration : Mathf.Repeat(m_Elapsed, 1f);
            m_Properties.SetFloat(s_Phase, phase);
            m_Renderer.SetPropertyBlock(m_Properties);
        }

        private void OnDisable() { m_Follow = null; }
    }
}
