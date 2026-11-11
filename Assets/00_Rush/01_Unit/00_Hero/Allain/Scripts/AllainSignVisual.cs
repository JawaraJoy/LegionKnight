using UnityEngine;

namespace Rush
{
    public class AllainSignVisual : MonoBehaviour
    {
        [SerializeField] private StatusEffector m_Effector;
        [SerializeField] private GameObject[] m_Marks;
        [SerializeField] private ParticleSystem m_Aura;
        [SerializeField] private GameObject m_TechDisplay;
        [SerializeField] private GameObject m_StackDisplay;
        [SerializeField] private float m_TechRotationSpeed = 40f;
        private void LateUpdate()
        {
            int count = m_Effector != null && m_Effector.IsActive ? m_Effector.CurrentStack : 0;
            if (m_StackDisplay != null) m_StackDisplay.SetActive(count > 0);
            if (m_TechDisplay != null)
            {
                m_TechDisplay.SetActive(count > 0);
                if (count > 0) m_TechDisplay.transform.Rotate(Vector3.forward, m_TechRotationSpeed * Time.deltaTime, Space.Self);
            }
            if (m_Aura != null)
            {
                if (count > 0 && !m_Aura.isPlaying) m_Aura.Play(true);
                else if (count == 0 && m_Aura.isPlaying)
                    m_Aura.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }
            for (int i = 0; i < m_Marks.Length; i++)
                if (m_Marks[i] != null && m_Marks[i].activeSelf != (i < count))
                    m_Marks[i].SetActive(i < count);
        }
        private void OnDisable()
        {
            if (m_TechDisplay != null) m_TechDisplay.SetActive(false);
            if (m_StackDisplay != null) m_StackDisplay.SetActive(false);
            if (m_Aura != null) m_Aura.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            foreach (var mark in m_Marks) if (mark != null) mark.SetActive(false);
        }
    }
}