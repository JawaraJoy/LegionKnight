using UnityEngine;

namespace Rush
{
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(100)]
    [RequireComponent(typeof(LineRenderer))]
    public sealed class LiuraCometTrail : MonoBehaviour
    {
        [SerializeField, Min(0.01f)] private float m_Lifetime = 0.32f;
        [SerializeField] private LineRenderer m_Line;
        // A distance-independent sample rate keeps short turns and straight shots equally smooth.
        private const float c_SampleInterval = 1f / 90f;
        private const int c_Capacity = 64;
        private readonly Vector3[] m_Positions = new Vector3[c_Capacity];
        private readonly float[] m_Times = new float[c_Capacity];
        private int m_Newest;
        private int m_Count;
        private float m_PreviousTime;
        private Vector3 m_PreviousPosition;
        private bool m_Recording;
        private bool m_Running;

        public void Begin(Vector3 origin)
        {
            BeginInternal(origin, Time.time);
        }

        private void BeginInternal(Vector3 origin, float now)
        {
            ClearInternal();
            m_Recording = true;
            m_Running = true;
            m_PreviousPosition = origin;
            m_PreviousTime = now;
            AppendInternal(origin, now);
        }

        public void EndEmission() => m_Recording = false;

        public void Clear() => ClearInternal();

        private void OnDisable() => ClearInternal();

        private void ClearInternal()
        {
            m_Running = false;
            m_Recording = false;
            m_Count = 0;
            m_Newest = 0;
            m_Line.positionCount = 0;
        }

        private void LateUpdate()
        {
            SampleInternal(transform.position, Time.time);
        }

        private void AppendInternal(Vector3 point, float time)
        {
            m_Newest = (m_Newest + 1) % c_Capacity;
            m_Positions[m_Newest] = point;
            m_Times[m_Newest] = time;
            m_Count = Mathf.Min(m_Count + 1, c_Capacity);
        }

        private void SampleInternal(Vector3 point, float now)
        {
            if (!m_Running) return;
            float cutoff = now - m_Lifetime;
            if (m_Recording)
            {
                float next = Mathf.Max(m_Times[m_Newest] + c_SampleInterval, cutoff);
                for (; next <= now; next += c_SampleInterval)
                {
                    float fraction = Mathf.InverseLerp(m_PreviousTime, now, next);
                    AppendInternal(Vector3.Lerp(m_PreviousPosition, point, fraction), next);
                    // Bound work even where absolute clock precision exceeds the sampling interval.
                    if (next + c_SampleInterval <= next) break;
                }
                m_PreviousTime = now;
                m_PreviousPosition = point;
            }
            while (m_Count > 0 && m_Times[(m_Newest - m_Count + 1 + c_Capacity) % c_Capacity] < cutoff)
                m_Count--;
            if (m_Count == 0)
            {
                ClearInternal();
                return;
            }

            // Head first: the width and color curves taper toward the oldest position.
            int head = m_Recording && (point - m_Positions[m_Newest]).sqrMagnitude > 0.000001f ? 1 : 0;
            m_Line.positionCount = m_Count + head;
            if (head != 0) m_Line.SetPosition(0, point);
            for (int i = 0; i < m_Count; i++)
                m_Line.SetPosition(i + head, m_Positions[(m_Newest - i + c_Capacity) % c_Capacity]);
        }
    }
}
