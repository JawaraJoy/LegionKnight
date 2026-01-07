using MoreMountains.Tools;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Rush
{
    public class Targetable : MonoBehaviour
    {
        [SerializeField, MMReadOnly]
        private bool m_IsAlive = true;
        [SerializeField, MMReadOnly]
        private bool m_IsTargeted = false;
        [SerializeField, MMReadOnly]
        private List<Attacker> m_Attackers = new();
        public bool IsTargeted => m_IsTargeted;
        public bool IsAlive => m_IsAlive;
        public void SetTargeted(bool targeted)
        {
            m_IsTargeted = targeted;
        }
        public void SetAlive(bool alive)
        {
            m_IsAlive = alive;
        }
        public void AddAttacker(Attacker attacker)
        {
            if (!m_Attackers.Contains(attacker))
            {
                m_Attackers.Add(attacker);
            }
        }
        public void RemoveAttacker(Attacker attacker)
        {
            if (m_Attackers.Contains(attacker))
            {
                m_Attackers.Remove(attacker);
            }
        }
    }
}
