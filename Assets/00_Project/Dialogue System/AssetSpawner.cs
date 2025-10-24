using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace LegionKnight
{
    [Serializable]
    public class AssetReferenceComponent<T> : AssetReferenceT<GameObject> where T : Component
    {
        public AssetReferenceComponent(string guid) : base(guid) { }

        /// <summary>
        /// Coroutine-style instantiate that yields until completed,
        /// then returns the component of type T via callback.
        /// </summary>
        public IEnumerator InstantiateComponentCoroutine(Transform parent, Action<T, AsyncOperationHandle<GameObject>> onDone, bool instantiateInWorldSpace = false)
        {
            var handle = base.InstantiateAsync(parent, instantiateInWorldSpace);
            yield return handle;

            if (handle.Status == AsyncOperationStatus.Succeeded && handle.Result != null)
            {
                T component = handle.Result.GetComponent<T>();
                onDone?.Invoke(component, handle);
            }
            else
            {
                onDone?.Invoke(null, handle);
            }
        }

        /// <summary>
        /// Releases an instantiated object safely.
        /// </summary>
        public void AReleaseInstance(GameObject instance)
        {
            if (instance != null)
            {
                Addressables.ReleaseInstance(instance);
            }
        }

        /// <summary>
        /// Releases a handle safely if valid.
        /// </summary>
        public void ReleaseHandle(AsyncOperationHandle handle)
        {
            if (handle.IsValid())
            {
                Addressables.Release(handle);
            }
        }
    }
}
