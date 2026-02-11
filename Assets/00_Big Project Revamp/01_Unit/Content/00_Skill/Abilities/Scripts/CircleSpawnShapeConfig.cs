using UnityEngine;

namespace Rush
{
    [CreateAssetMenu(fileName = "CircleSummonShape", menuName = "Rush/Combat/Shape/Circle", order = 0)]
    public class CircleSpawnShapeConfig : SpawnShapeConfig
    {
        [SerializeField] 
        private float m_Radius = 1.5f;
        [SerializeField] 
        private bool m_FaceOutward = true;
        [SerializeField] 
        private float m_AngleOffsetDeg = 0f;

        public override void GetSpawnTransform(Transform origin, int index, int totalCount,
            out Vector3 position, out Quaternion rotation)
        {
            // totalCount = jumlah unit yang mengitari origin (target) untuk "batch" ini
            int count = Mathf.Max(1, totalCount);

            float angle = (360f / count) * index + m_AngleOffsetDeg;
            float rad = angle * Mathf.Deg2Rad;

            Vector3 offset = new Vector3(Mathf.Cos(rad), 0f, Mathf.Sin(rad)) * m_Radius;
            position = origin.position + offset;

            if (m_FaceOutward)
            {
                Vector3 dir = (position - origin.position);
                dir.y = 0f;
                rotation = dir.sqrMagnitude > 0.0001f ? Quaternion.LookRotation(dir.normalized, Vector3.up) : origin.rotation;
            }
            else
            {
                rotation = origin.rotation;
            }
        }
    }
}
