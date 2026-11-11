using UnityEngine;

namespace Rush
{
    public class AllainPulseAreaVisual : MonoBehaviour
    {
        [System.Serializable]
        private class AreaField
        {
            [SerializeField] private LineRenderer[] m_Lines = new LineRenderer[0];
            [SerializeField] private float m_HoldDuration = .25f;
            [SerializeField] private float m_Lifetime = .85f;
            public LineRenderer[] Lines => m_Lines;
            public float HoldDuration => m_HoldDuration;
            public float Lifetime => m_Lifetime;
        }
        [SerializeField] private AreaField m_Area = new AreaField();
        private Color[] m_StartColors;
        private Color[] m_EndColors;
        private float m_Elapsed;

        private void Awake()
        {
            m_StartColors = new Color[m_Area.Lines.Length];
            m_EndColors = new Color[m_Area.Lines.Length];
            for (int i = 0; i < m_Area.Lines.Length; i++)
            {
                if (m_Area.Lines[i] == null) continue;
                m_StartColors[i] = m_Area.Lines[i].startColor;
                m_EndColors[i] = m_Area.Lines[i].endColor;
            }
        }
        private void OnEnable()
        {
            m_Elapsed = 0f;
            UpdateLinesInternal();
        }
        private void Update()
        {
            m_Elapsed += Time.deltaTime;
            UpdateLinesInternal();
        }
        private void UpdateLinesInternal()
        {
            float fadeDuration = m_Area.Lifetime - m_Area.HoldDuration;
            float alpha = m_Elapsed <= m_Area.HoldDuration ? 1f :
                fadeDuration > 0f ? Mathf.Clamp01((m_Area.Lifetime - m_Elapsed) / fadeDuration) : 0f;
            for (int i = 0; i < m_Area.Lines.Length; i++)
            {
                var line = m_Area.Lines[i];
                if (line == null) continue;
                Color start = m_StartColors[i], end = m_EndColors[i];
                start.a *= alpha; end.a *= alpha;
                line.startColor = start; line.endColor = end;
                line.enabled = alpha > 0f;
            }
        }
    }
}
