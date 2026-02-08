using System;
using UnityEngine;

namespace Rush
{
    [CreateAssetMenu(fileName = "Fan Shape", menuName = "Rush/Combat/Shape/Fan")]
    public class FanSpawnShapeConfig : SpawnShapeConfig
    {
        [Tooltip("Total angle of the spread in degrees.")]
        [SerializeField]
        private float m_SpreadAngle = 45f;

        public override void GetSpawnTransform(
            Transform origin,
            int index,
            int totalCount,
            out Vector3 position,
            out Quaternion rotation
        )
        {
            position = origin.position;

            if (totalCount <= 1)
            {
                rotation = origin.rotation;
                return;
            }

            float step = m_SpreadAngle / (totalCount - 1);
            float start = -m_SpreadAngle * 0.5f;

            float angle = start + step * index;

            rotation = origin.rotation * Quaternion.Euler(0f, 0f, angle);
        }
    }
}
