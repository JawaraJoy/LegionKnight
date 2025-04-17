using System.Collections.Generic;
using UnityEngine;

namespace LegionKnight
{
    public partial class Shield : MonoBehaviour
    {
        [SerializeField]
        private bool m_Active = true;
        [SerializeField]
        private Damageable m_Damageable;
        public void DestroyContact(GameObject other)
        {
            if (m_Active)
            {
                Destroy(other);
            }
        }
        public void SetActive(bool set)
        {
            m_Active = set;
        }
        public void AddHealth(int health)
        {
            if (m_Damageable != null)
            {
                m_Damageable.AddHealth(health);
            }
        }
        public void AddShield(int shield)
        {
            if (m_Damageable != null)
            {
                m_Damageable.AddShield(shield);
            }
        }
        public void AddBarrier(int barrier)
        {
            if (m_Damageable != null)
            {
                m_Damageable.AddBarrier(barrier);
            }
        }
    }

}
