using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public partial class SkillSpawner : MonoBehaviour
    {
        [SerializeField]
        private bool m_RandomSpawn = false;
        [SerializeField]
        private List<ProjectileSpawn> m_Skills = new();
        [SerializeField]
        private bool m_SpawnOnStart = true;

        [SerializeField]
        private bool m_IsSpawnInterval = false; // If true, will spawn at intervals
        [SerializeField] private float m_SpawnInterval = 2f; // X: seconds between spawns
        [SerializeField] private int m_MaxSpawnCount = 2;    // Y: max number of spawns
        private int m_CurrentSpawnCount = 0;
        private float m_SpawnTimer = 0f;
        private bool m_IsSpawning = false;

        [SerializeField]
        private UnityEvent m_OnSpawnIntervalStop = new();

        private void Start()
        {
            if (m_SpawnOnStart)
            {
                if (m_IsSpawnInterval)
                {
                    StartSpawningInternal();
                }
                else
                {
                    SpawnProjectileInternal();
                }
            }
        }
        private void Update()
        {
            SpawnInterval();
        }
        private void SpawnInterval()
        {
            if (!m_IsSpawning || m_CurrentSpawnCount >= m_MaxSpawnCount)
                return;

            m_SpawnTimer += Time.deltaTime;
            if (m_SpawnTimer >= m_SpawnInterval)
            {
                m_SpawnTimer = 0f;
                SpawnProjectileInternal();
                m_CurrentSpawnCount++;
                if (m_CurrentSpawnCount >= m_MaxSpawnCount)
                {
                    StopSpawningInternal();
                }
            }
        }
        private void StartSpawningInternal()
        {
            m_CurrentSpawnCount = 0;
            m_SpawnTimer = 0f;
            m_IsSpawning = true;
        }

        private void StopSpawningInternal()
        {
            m_IsSpawning = false;
            m_OnSpawnIntervalStop?.Invoke();
        }
        public void SpawnProjectile()
        {
            SpawnProjectileInternal();
        }
        private void SpawnProjectileInternal()
        {
            
            if (m_RandomSpawn)
            {
                SpawnRandomProjectile();
            }
            else
            {
                foreach (ProjectileSpawn skill in m_Skills)
                {
                    skill.LoadProjectile();
                }
            }
        }
        private void SpawnRandomProjectile()
        {
            if (m_Skills.Count == 0) return;
            int randomIndex = GetRandomIndex();
            m_Skills[randomIndex].LoadProjectile();
        }

        private int GetRandomIndex()
        {
            if (m_Skills.Count == 0)
            {
                Debug.LogWarning("No skills available to spawn.");
                return -1; // or handle this case as needed
            }
            return Random.Range(0, m_Skills.Count);
        }

        public void SpawnRandomInterval(int count)
        {
            m_IsSpawnInterval = true;
            m_MaxSpawnCount = count;
            StartSpawningInternal();
        }
    }
}
