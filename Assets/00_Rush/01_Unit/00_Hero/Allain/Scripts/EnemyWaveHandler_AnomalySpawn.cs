using UnityEngine;

namespace Rush
{
    public partial class EnemyWaveHandler
    {
        // Uses the same wave shape and origin as normal enemy spawning, never a live enemy transform.
        public bool TryGetRandomSpawnPose(out Vector3 position, out Quaternion rotation)
        {
            position = default;
            rotation = Quaternion.identity;
            if (m_EnemyWavePost == null || m_EnemyWavePost.PostToSpawn == null) return false;
            Transform origin = m_EnemyWavePost.PostToSpawn;
            position = origin.position;
            rotation = origin.rotation;
            var waves = m_CurrentEnemyWave?.MinionWaveFields;
            if (waves == null || waves.Length == 0) return false;
            var wave = waves[Mathf.Clamp(m_CurrenWaveIndex, 0, waves.Length - 1)];
            int count = Mathf.Max(1, GetAllMinions(wave).Length);
            int index = Random.Range(0, count);
            if (wave.SpawnShapeConfig != null)
                wave.SpawnShapeConfig.GetSpawnTransform(origin, index, count, out position, out rotation);
            return true;
        }
    }
}
