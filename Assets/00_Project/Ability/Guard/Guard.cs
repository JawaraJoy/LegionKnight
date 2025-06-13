using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace LegionKnight
{
    public class Guard : MonoBehaviour
    {
        [SerializeField]
        private LayerMask m_ContactLayerMask;
        private bool m_IsActive = false;
        [SerializeField]
        private float m_Duration = 5f;

        [SerializeField]
        private SpriteRenderer m_SpriteModel;

        [SerializeField]
        private AssetReferenceGameObject m_ImpactEffect;

        [SerializeField]
        private UnityEvent m_OnDeactiveGuard;
        [SerializeField]
        private UnityEvent<int> m_OnContact;
        private void OnTriggerEnter2D(Collider2D collision)
        {
            OnContact(collision.gameObject);
        }

        private void Start()
        {
            ActiveGuard();
        }

        private void OnContact(GameObject contactObject)
        {
            if ((m_ContactLayerMask & (1 << contactObject.layer)) == 0)
            {
                return; // Ignore objects not in the contact layer mask
            }
            if (contactObject.TryGetComponent(out Damageable damageable))
            {
                // Handle damageable object contact
                damageable.TakeDamage(100); // Example: apply 1 damage
                m_OnContact?.Invoke(damageable.Damage);
                SpawningImpact(contactObject);
            }
        }

        private Vector3 GetImpactPosition(GameObject contactObject)
        {
            // Calculate the impact position based on the contact object
            return contactObject.transform.position;
        }


        private void ActiveGuard()
        {
            m_IsActive = true;
            StartCoroutine(GuardCoroutine());
        }
        private IEnumerator GuardCoroutine()
        {
            yield return new WaitForSeconds(m_Duration);
            DeactiveGuardInternal();
        }
        private void DeactiveGuardInternal()
        {
            m_OnDeactiveGuard?.Invoke();
            m_IsActive = false;
        }
        public void DeactiveGuard()
        {
            DeactiveGuardInternal();
        }
        private void SpawningImpact(GameObject other)
        {
            AsyncOperationHandle<GameObject> handle = m_ImpactEffect.InstantiateAsync(GetImpactPosition(other), Quaternion.identity);
            StartCoroutine(SpawningImpact(handle));
        }
        private IEnumerator SpawningImpact(AsyncOperationHandle<GameObject> handle)
        {
            yield return handle;
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                GameObject impactEffect = handle.Result;
                impactEffect.SetActive(true);
                yield return new WaitForSeconds(1f); // Adjust duration as needed
                var release =  Addressables.ReleaseInstance(impactEffect);
                if (release)
                {
                    Debug.Log("Impact effect released successfully.");
                }
                else
                {
                    Debug.LogError("Failed to release impact effect.");
                }
            }
            else
            {
                Debug.LogError("Failed to load impact effect.");
            }
        }
    }
}
