using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace LegionKnight
{
    public class CharacterSpine : MonoBehaviour
    {
        private readonly Dictionary<string, SpineObject> m_SpineObjects = new();

        private AsyncOperationHandle<GameObject> m_Handle;

        private SpineObject m_CurrentSpineObject;

        [SerializeField]
        private Transform m_SpineParent;

        [SerializeField]
        private UnityEvent<SpineObject> m_OnSpineObjectAdded;
        [SerializeField]
        private UnityEvent<SpineObject> m_OnSpineObjectShow;
        [SerializeField]
        private UnityEvent<SpineObject> m_OnSpineObjectHide;

        [SerializeField]
        private UnityEvent m_OnNoSpineObjectFound;
        private SpineObject GetSpineObject(string key)
        {
            if (m_SpineObjects.TryGetValue(key, out var spineObject))
            {
                return spineObject;
            }
            Debug.LogWarning($"SpineObject with key {key} not found.");
            return null;
        }
        private void AddSpineObjectInternal(SpineObject spineObject)
        {
            string key = spineObject.Defi.Id;
            if (!m_SpineObjects.ContainsKey(key))
            {
                m_SpineObjects.Add(key, spineObject);
                m_OnSpineObjectAdded?.Invoke(spineObject);
            }
            else
            {
                Debug.LogWarning($"SpineObject with key {key} already exists.");
            }
        }

        private bool HasSpineObject(string key)
        {
            return m_SpineObjects.ContainsKey(key);
        }

        private void ShowSpineObjectInternal(string key)
        {
            if (HasSpineObject(key))
            {
                GetSpineObject(key).Show();
                m_OnSpineObjectShow?.Invoke(GetSpineObject(key));
            }
            else
            {
                Debug.LogWarning($"SpineObject with key {key} not found.");
            }
        }

        private void HideSpineObjectInternal(string key)
        {
            if (HasSpineObject(key))
            {
                GetSpineObject(key).Hide();
                m_OnSpineObjectHide?.Invoke(GetSpineObject(key));
            }
            else
            {
                Debug.LogWarning($"SpineObject with key {key} not found.");
            }
        }

        private void HideAllSpineInternal()
        {
            foreach (var spineObject in m_SpineObjects.Values)
            {
                HideSpineObjectInternal(spineObject.Defi.Id);
            }
        }

        public void ChangeSpine(CharacterDefinition defi)
        {
            HideAllSpineInternal();
            AssetReferenceGameObject existingSpineObject = defi.CharacterPrefab;
            if (existingSpineObject == null)
            {
                Debug.LogError("CharacterDefinition does not have a valid CharacterPrefab.");
                m_OnNoSpineObjectFound?.Invoke();
                return;
            }
            if (HasSpineObject(defi.Id))
            {
                ShowSpineObjectInternal(defi.Id);
                m_CurrentSpineObject = GetSpineObject(defi.Id);
            }
            else
            {
                StartCoroutine(SpawningSpineObject(defi));
            }
        }

        private IEnumerator SpawningSpineObject(CharacterDefinition defi)
        {
            m_Handle = defi.CharacterPrefab.InstantiateAsync(m_SpineParent, false);
            yield return m_Handle;
            if (m_Handle.Status == AsyncOperationStatus.Succeeded)
            {
                GameObject spawnedObject = m_Handle.Result;
                if (spawnedObject.TryGetComponent(out SpineObject spineObject))
                {
                    spineObject.InitSpine(defi);
                    yield return new WaitUntil(() => spineObject.Initialized);
                    AddSpineObjectInternal(spineObject);
                    ShowSpineObjectInternal(spineObject.Defi.Id);
                    m_CurrentSpineObject = spineObject;
                }
                else
                {
                    Debug.LogError("Spawned object does not have a SpineObject component.");
                }
            }
            else
            {
                Debug.LogError("Failed to instantiate SpineObject.");
            }

            
        }
        public void PlayJump()
        {
            m_CurrentSpineObject?.PlayJump();
        }
        public void PlayIdle()
        {
            m_CurrentSpineObject?.PlayIdle();
        }
        public void PlayAttack()
        {
            m_CurrentSpineObject?.PlayAttack();
        }
        public void PlayDeath()
        {
            m_CurrentSpineObject?.PlayDeath();
        }
    }
}
