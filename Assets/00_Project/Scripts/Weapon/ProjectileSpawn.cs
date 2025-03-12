using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace LegionKnight
{
    [System.Serializable]
    public partial class ProjectileSpawn
    {
        [SerializeField]
        private int m_DamageOveride;
        [SerializeField]
        private Vector2 m_StartingForce;
        [SerializeField]
        private AssetReferenceGameObject m_ProjectileAsset;
        private AsyncOperationHandle<GameObject> m_Handle;

        [SerializeField]
        private Transform m_SpawnPost;

        private ProjectileDamage m_SpawnedProjectileDamage;

        [SerializeField]
        private UnityEvent<ProjectileDamage> m_OnWeaponSpawned = new();

        public void LoadProjectile()
        {
            m_Handle = m_ProjectileAsset.InstantiateAsync(m_SpawnPost.position, Quaternion.identity);
            m_Handle.Completed += SpawnProjectile;
        }

        private void OnWeaponSpawnedInvoke()
        {
            m_OnWeaponSpawned?.Invoke(m_SpawnedProjectileDamage);
        }

        private void SpawnProjectile(AsyncOperationHandle<GameObject> handle)
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                GameObject result = handle.Result;
                if (result.TryGetComponent(out ProjectileDamage projectile))
                {
                    
                    m_SpawnedProjectileDamage = projectile;

                    if (m_DamageOveride > 0)
                    {
                        m_SpawnedProjectileDamage.Init(m_DamageOveride, 1);
                    }
                    else
                    {
                        m_SpawnedProjectileDamage.Init(Player.Instance.Attack, 1);
                    }
                    m_SpawnedProjectileDamage.AddForce(m_StartingForce);
                    m_SpawnedProjectileDamage.FindTarget();
                    OnWeaponSpawnedInvoke();
                }
            }
        }
    }
}
