using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace LegionKnight
{
    public partial class ParticleSpawner : MonoBehaviour
    {
        [SerializeField]
        private AssetReferenceGameObject m_Particle;

        private AsyncOperationHandle<GameObject> m_Handle;

        public void SpawnParticle()
        {
            m_Handle = m_Particle.InstantiateAsync(transform.position, Quaternion.identity);
            m_Handle.Completed += OnSpawnPartcle;
        }

        private void OnSpawnPartcle(AsyncOperationHandle<GameObject> handle)
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                GameObject result = handle.Result;
                if (result.TryGetComponent(out ParticleSystem particle))
                {
                    particle.Play();
                }
            }
            //transform.DetachChildren();
        }
    }
}
