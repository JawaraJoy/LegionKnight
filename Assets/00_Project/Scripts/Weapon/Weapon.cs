using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace LegionKnight
{
    public enum SpawnMode
    {
        RandomSingle = 0,
        Burst = 1,
        Spray = 2,
    }
    [System.Serializable]
    public partial class ProjectileSpawn
    {
        [SerializeField]
        private AssetReferenceGameObject m_ProjectileAsset;
        private AsyncOperationHandle<GameObject> m_Handle;

        [SerializeField]
        private Transform m_SpawnPost;

        private ProjectileDamage m_SpawnedProjectileDamage;

        public void LoadProjectile()
        {
            m_Handle = m_ProjectileAsset.InstantiateAsync();
            m_Handle.Completed += SpawnProjectile;
        }

        private void SpawnProjectile(AsyncOperationHandle<GameObject> handle)
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                GameObject result = handle.Result;
                if (result.TryGetComponent(out ProjectileDamage damage))
                {
                    m_SpawnedProjectileDamage = damage;
                    m_SpawnedProjectileDamage.SetTarget()
                }
            }
        }
    }
    public partial class Weapon : MonoBehaviour
    {
        

        
    }
}
