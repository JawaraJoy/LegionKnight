using System;
using UnityEngine;

namespace Rush
{
    public abstract class SpawnShapeConfig : Configuration
    {
        /// <summary>
        /// Returns spawn position and rotation for each projectile index.
        /// totalCount is provided from ShotAbilityConfig.FireCount.
        /// </summary>
        public abstract void GetSpawnTransform(Transform origin, int index, int totalCount, out Vector3 position, out Quaternion rotation);
    }
}
