using System;
using UnityEngine;

namespace Rush
{
    public abstract class SpawnShapeConfig : ScriptableObject
    {
        /// <summary>
        /// Returns spawn position and rotation for each projectile index.
        /// totalCount is provided from ShotAbilityConfig.FireCount.
        /// </summary>
        public abstract void GetSpawnTransform(Transform origin, int index, int totalCount, out Vector3 position, out Quaternion rotation);
    }
    public enum FireMode
    {
        Instant,     // semua keluar sekaligus
        Burst,       // keluar per kelompok
        Interval,     // satu-satu cepat (Gutling)
        Loop,        // arah muter 0→N→0
        PingPong,    // arah bolak-balik
        Random       // arah random tiap shot
    }
}
