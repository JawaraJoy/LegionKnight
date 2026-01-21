using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace LegionKnight
{
    public class EnemyController : MonoBehaviour
    {

        [SerializeField]
        private List<IEnemy> m_EnemyList = new();

        [SerializeField]
        private Transform m_SpawningSpot;
        [SerializeField]
        private MinionUnit[] m_MinionUnits;

        [SerializeField]
        private bool m_IsEnemyExist = false;
        [SerializeField]
        private bool m_CanSpawnEnemy = false;
        public bool CanSpawnEnemy => m_CanSpawnEnemy;
        [SerializeField]
        private UnityEvent m_OnAnyEnemies;
        [SerializeField]
        private UnityEvent m_OnEnemyGone;
        [SerializeField]
        private UnityEvent<IEnemy> m_OnEnemyDeath;

        [SerializeField]
        private float m_Radius = 7f;


        public void SetSpawningSpot(Transform spot)
        {
            m_SpawningSpot = spot;
        }
        public void SetCanSpawnEnemy(bool can)
        {
            m_CanSpawnEnemy = can;
        }
        private MinionUnit GetMinionUnit(MinionDefinition def)
        {
            foreach (var unit in m_MinionUnits)
            {
                if (unit.Definition == def) return unit;
            }
            return null;
        }
        public void ResetAllToCantSpawn()
        {
            foreach (var unit in m_MinionUnits)
            {
                unit.SetCanSpawn(false);
            }
        }
        public void SetCanSpawnUnit(MinionDefinition defi, bool set)
        {
            MinionUnit unit = GetMinionUnit(defi);
            if (unit == null) return;
            unit.SetCanSpawn(set);
        }
        public void SpawnMinion(MinionDefinition defi)
        {
            if (!m_CanSpawnEnemy) return;
            MinionUnit unit = GetMinionUnit(defi);
            if (unit == null) return;
            unit.SpawnMinion(m_SpawningSpot, GetRandomRadiusSpawn());
            //AssetReferenceGameObject asset = defi.ModelPrefab;
            //AsyncOperationHandle<GameObject> handle = asset.InstantiateAsync(m_SpawningSpot, false);
            //StartCoroutine(SpawningMinion(defi, handle));
        }

        public void CustomSpawnMinions(MinionDefinition defi, int amount, Transform selectedSpot, float radius)
        {
            MinionUnit unit = GetMinionUnit(defi);
            if (unit == null) return;
            for (int i = 0; i < amount; i++)
            {
                unit.CustomSpawnMinion(selectedSpot, GetRandomRadiesSpawn(selectedSpot, radius));
            }
            Debug.Log($"Custom Spawned {amount} {defi.name} at {selectedSpot.name} with radius {radius}");
        }

        private Vector2 GetRandomRadiusSpawn()
        {
            Vector2 randomOffset = Random.insideUnitCircle * m_Radius;
            Vector3 spawnTransformPosition = m_SpawningSpot.position;
            Vector3 spawnPosition = new Vector3(spawnTransformPosition.x + randomOffset.x, spawnTransformPosition.y + randomOffset.y, 0);
            return new Vector2 (spawnPosition.x, spawnPosition.y);
        }
        private Vector2 GetRandomRadiesSpawn(Transform selectedSpot, float radius)
        {
            Vector2 randomOffset = Random.insideUnitCircle * m_Radius;
            Vector3 spawnTransformPosition = selectedSpot.position;
            Vector3 spawnPosition = new Vector3(spawnTransformPosition.x + randomOffset.x, spawnTransformPosition.y + randomOffset.y, 0);
            return new Vector2(spawnPosition.x, spawnPosition.y);
        }
        private void CheckEnemy()
        {
            // is enemy exist if there is 1 enemy on the list

            m_IsEnemyExist = m_EnemyList.Count > 0;
            if (m_IsEnemyExist)
            {
                m_OnAnyEnemies?.Invoke();
            }
            else
            {
                m_OnEnemyGone?.Invoke();
            }
            Debug.Log($"Enemy {m_EnemyList.Count}");
        }
        public void AddEnemy(IEnemy enemy)
        {
            m_EnemyList.Add(enemy);
            CheckEnemy();
        }
        public void RemoveEnemy(IEnemy enemy)
        {
            m_EnemyList.Remove(enemy);
            m_OnEnemyDeath?.Invoke(enemy);
            CheckEnemy();
        }
    }
}
