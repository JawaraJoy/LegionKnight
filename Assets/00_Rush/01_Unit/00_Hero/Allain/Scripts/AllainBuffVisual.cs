using UnityEngine;

namespace Rush
{
    public class AllainBuffVisual : MonoBehaviour
    {
        [SerializeField] private StatModifier m_Modifier;
        [SerializeField] private GameObject[] m_Marks;
        [SerializeField] private ParticleSystem m_Aura;
        [System.Serializable]
        private class OrbitField
        {
            [SerializeField] private Transform[] m_Orbs = new Transform[0];
            [SerializeField] private Vector2 m_Radius = new Vector2(1.4f, .65f);
            [SerializeField] private Vector3 m_Center = new Vector3(0f, .2f, 0f);
            [SerializeField] private float m_DegreesPerSecond = 120f;
            public Transform[] Orbs => m_Orbs;
            public Vector2 Radius => m_Radius;
            public Vector3 Center => m_Center;
            public float DegreesPerSecond => m_DegreesPerSecond;
        }
        [SerializeField] private OrbitField m_Orbit = new OrbitField();
        private float m_OrbitAngle;
        private void OnEnable()
        {
            m_OrbitAngle = 0f;
            UpdateOrbitInternal();
            if (m_Aura == null) return;
            m_Aura.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            m_Aura.Play(true);
        }
        private void OnDisable()
        {
            if (m_Aura != null) m_Aura.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
        private void LateUpdate()
        {
            m_OrbitAngle = Mathf.Repeat(m_OrbitAngle + m_Orbit.DegreesPerSecond * Time.deltaTime, 360f);
            UpdateOrbitInternal();
            int count = m_Modifier != null && m_Modifier.IsActive ? m_Modifier.StackCount : 0;
            for (int i = 0; i < m_Marks.Length; i++)
                if (m_Marks[i] != null && m_Marks[i].activeSelf != (i < count))
                    m_Marks[i].SetActive(i < count);
        }
        private void UpdateOrbitInternal()
        {
            for (int i = 0; i < m_Orbit.Orbs.Length; i++)
            {
                if (m_Orbit.Orbs[i] == null) continue;
                float angle = (m_OrbitAngle + 360f * i / m_Orbit.Orbs.Length) * Mathf.Deg2Rad;
                m_Orbit.Orbs[i].localPosition = m_Orbit.Center + new Vector3(
                    Mathf.Cos(angle) * m_Orbit.Radius.x, Mathf.Sin(angle) * m_Orbit.Radius.y, 0f);
            }
        }
    }
}
