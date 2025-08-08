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
        [SerializeField]
        private UnityEvent m_OnAnyEnemies;
        [SerializeField]
        private UnityEvent m_OnEnemyGone;

        [SerializeField]
        private Vector2 m_OffsiteRandom;

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
            unit.SpawnMinion(m_SpawningSpot, m_OffsiteRandom);
            //AssetReferenceGameObject asset = defi.ModelPrefab;
            //AsyncOperationHandle<GameObject> handle = asset.InstantiateAsync(m_SpawningSpot, false);
            //StartCoroutine(SpawningMinion(defi, handle));
        }
        private IEnumerator SpawningMinion(MinionDefinition defi, AsyncOperationHandle<GameObject> handle)
        {
            yield return handle;
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                GameObject spawned = handle.Result;
                if (spawned.TryGetComponent(out Minion minion))
                {
                    minion.Init(defi);
                    m_SpawningSpot.DetachChildren();

                    SetPositionRandomRadius(spawned);
                }
            }
        }
        private void SetPositionRandomRadius(GameObject minion)
        {
            float startX = m_SpawningSpot.position.x;
            float startY = m_SpawningSpot.position.y;
            float AddX = m_OffsiteRandom.x + startX;
            float AddY = m_OffsiteRandom.y + startY;

            float randomX = Random.Range(startX, AddX);
            float randomY = Random.Range(startY, AddY);
            Vector2 randomRadiues = new(randomX, randomY);

            minion.transform.position = randomRadiues;
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
            CheckEnemy();
        }
    }
}
