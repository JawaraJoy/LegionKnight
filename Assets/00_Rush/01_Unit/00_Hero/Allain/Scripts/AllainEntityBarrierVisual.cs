using UnityEngine;

namespace Rush
{
    public class AllainEntityBarrierVisual : MonoBehaviour
    {
        [SerializeField] private Damageable m_Damageable;
        [SerializeField] private GameObject[] m_Rings;
        private void LateUpdate()
        {
            int count = m_Damageable != null && m_Damageable.Health > 0 ? m_Damageable.Barrier : 0;
            for (int i = 0; i < m_Rings.Length; i++)
                if (m_Rings[i] != null && m_Rings[i].activeSelf != (i < count))
                    m_Rings[i].SetActive(i < count);
        }
    }
}
