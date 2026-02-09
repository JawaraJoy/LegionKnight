using LegionKnight;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Rush
{
    public class RushCharacterSpine : MonoBehaviour
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
        [SerializeField]
        private UnityEvent<SpineAnimDefinition> m_OnAnimationDone;
        [SerializeField]
        private SpineEvent[] m_SpineEvents;
        public SpineObject CurrentSpineObject => m_CurrentSpineObject;
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
            string key = "";
            if (spineObject.Defi is CharacterDefinition characterDef)
            {
                key = characterDef.Id;
            }
            if (spineObject.Defi is BosDefinition bosDef)
            {
                key = bosDef.Id;
            }
            if (spineObject.Defi is MinionDefinition minionDef)
            {
                key = minionDef.Label;
            }
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
                string id = GetId(spineObject.Defi);
                HideSpineObjectInternal(id);
            }
        }

        public void ChangeSpine(ScriptableObject defi)
        {
            HideAllSpineInternal();
            AssetReferenceGameObject existingSpineObject = GetAsset(defi);
            string id = GetId(defi);
            if (existingSpineObject == null)
            {
                Debug.LogError("CharacterDefinition does not have a valid CharacterPrefab.");
                m_OnNoSpineObjectFound?.Invoke();
                return;
            }
            if (HasSpineObject(id))
            {
                ShowSpineObjectInternal(id);
                m_CurrentSpineObject = GetSpineObject(id);
            }
            else
            {
                StartCoroutine(SpawningCharSpineObject(defi));
            }
            
        }
        private AssetReferenceGameObject GetAsset(ScriptableObject defi)
        {
            if (defi is CharacterDefinition characterDef)
            {
                return characterDef.CharacterPrefab;
            }
            if (defi is BosDefinition bosDef)
            {
                return bosDef.BosPrefab;
            }
            Debug.LogError("ScriptableObject does not have a valid Prefab.");
            return null;
        }
        private string GetId(ScriptableObject defi)
        {
            if (defi is CharacterDefinition characterDef)
            {
                return characterDef.Id;
            }
            if (defi is BosDefinition bosDef)
            {
                return bosDef.Id;
            }
            Debug.LogError("ScriptableObject does not have a valid Id.");
            return string.Empty;
        }
        private IEnumerator SpawningCharSpineObject(ScriptableObject defi)
        {
            AssetReferenceGameObject existingSpineObject = GetAsset(defi);
            string id = GetId(defi);
            if (existingSpineObject == null)
            {
                Debug.LogError("CharacterDefinition does not have a valid Prefab.");
                m_OnNoSpineObjectFound?.Invoke();
                yield break;
            }
            m_Handle = existingSpineObject.InstantiateAsync(m_SpineParent, false);
            yield return m_Handle;
            if (m_Handle.Status == AsyncOperationStatus.Succeeded)
            {
                GameObject spawnedObject = m_Handle.Result;
                if (spawnedObject.TryGetComponent(out SpineObject spineObject))
                {
                    spineObject.InitCharSpine(defi);
                    yield return new WaitUntil(() => spineObject.Initialized);
                    AddSpineObjectInternal(spineObject);
                    ShowSpineObjectInternal(id);
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
            m_CurrentSpineObject.PlayJump();
        }
        public void PlayIdle()
        {
            m_CurrentSpineObject?.PlayIdle();
        }
        public void PlayAttack()
        {
            m_CurrentSpineObject.PlayAttack();
        }
        public void PlayDeath()
        {
            m_CurrentSpineObject.PlayDeath();
        }
        public void FlipX(bool left)
        {
            m_CurrentSpineObject.FlipX(left);
        }
        
        public void PlayAnimationOnce(string key)
        {
            if (m_CurrentSpineObject != null)
            {
                m_CurrentSpineObject.PlayAnimationOnce(key);
            }
            else
            {
                Debug.LogWarning("No SpineObject is currently set.");
            }
        }
        private SpineEvent GetSpineEvent(SpineAnimDefinition defi)
        {
            foreach (var spineEvent in m_SpineEvents)
            {
                if (spineEvent.Definition == defi)
                {
                    return spineEvent;
                }
            }
            return null;
        }
        public void SetAnim(SpineAnimDefinition anim)
        {
            if (m_CurrentSpineObject == null)
            {
                Debug.LogWarning("No SpineObject is currently set.");
                return;
            }
            m_CurrentSpineObject.SetAnim(anim);
            anim.Play(m_CurrentSpineObject.SkeletonAnimation, () => OnAnimationDone(anim));
            var spineEvent = GetSpineEvent(anim);
            spineEvent?.OnStart.Invoke();
        }
        private void OnAnimationDone(SpineAnimDefinition anim)
        {
            Debug.Log($"Animation done for {anim.AnimName}");
            var spineEvent = GetSpineEvent(anim);
            spineEvent?.OnEnd.Invoke();
        }
    }
}
