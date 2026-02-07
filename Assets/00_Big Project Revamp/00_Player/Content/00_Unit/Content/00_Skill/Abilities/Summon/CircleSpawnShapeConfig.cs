using UnityEngine;

namespace Rush
{
    [CreateAssetMenu(fileName = "CircleSummonShape", menuName = "Rush/Combat/Shape/Circle", order = 0)]
    public class CircleSpawnShapeConfig : SpawnShapeConfig
    {
        public override void GetSpawnTransform(Transform origin, int index, int totalCount, out Vector3 position, out Quaternion rotation)
        {
            throw new System.NotImplementedException();
        }
    }
}
