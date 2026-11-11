using System;
using UnityEngine;

namespace Rush
{
    [CreateAssetMenu(fileName = "Shot Gun Shape", menuName = "Rush/Combat/Shape/Shot Gun")]
    public class ShotgunSpawnShape : SpawnShapeConfig
    {
        [Header("Shotgun Shape")]

        [Tooltip("Random angle deviation in degrees.")]
        [SerializeField]
        private float m_RandomAngle = 15f;

        [Tooltip("Random spawn offset radius.")]
        [SerializeField]
        private float m_PositionSpread = 0.2f;

        public override void GetSpawnTransform(Transform origin, int index, int totalCount, out Vector3 position, out Quaternion rotation)
        {
            float angle = UnityEngine.Random.Range(-m_RandomAngle, m_RandomAngle);
            rotation = origin.rotation * Quaternion.Euler(0f, 0f, angle);

            Vector2 offset = UnityEngine.Random.insideUnitCircle * m_PositionSpread;
            position = origin.position + (Vector3)offset;
        }
    }
}
