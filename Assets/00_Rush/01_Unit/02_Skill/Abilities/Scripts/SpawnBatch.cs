using System.Collections.Generic;
using UnityEngine;

namespace Rush
{
    public class SpawnBatchTracker : MonoBehaviour
    {
        private readonly List<Vector3> m_Positions = new();
        public IReadOnlyList<Vector3> Positions => m_Positions;

        public void ClearBatch()
        {
            m_Positions.Clear();
        }

        public void RegisterPosition(Vector3 position)
        {
            m_Positions.Add(position);
        }
    }
}   