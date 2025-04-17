using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace LegionKnight
{
    public partial class ProtectActivation : MonoBehaviour
    {
        [SerializeField]
        private AssetReferenceGameObject m_NormalProtectAsset;
        [SerializeField]    
        private List<Damageable> m_Damageable = new();
        [SerializeField]
        private UnityEvent m_OnProtectGone = new();

        [SerializeField]
        private ProtectActivationAgent m_ProtectActivationAgent;

        private AsyncOperationHandle<GameObject> m_Handle;

        public void SetProtectActivationAgent(ProtectActivationAgent protectActivationAgent)
        {
            SetProtectActivationAgentInternal(protectActivationAgent);
        }
        public void SetProtectActivationAgentInternal(ProtectActivationAgent protectActivationAgent)
        {
            if (m_ProtectActivationAgent != null)
            {
                if (m_ProtectActivationAgent.TryGetComponent(out DestroySelf destroySelf))
                {
                    destroySelf.DestroyMe();
                }
            }
            m_ProtectActivationAgent = protectActivationAgent;
        }
        public void LoadProtectAsset()
        {
            StartCoroutine(LoadingProtectAsset());
        }
        private IEnumerator LoadingProtectAsset()
        {
            m_Handle = m_NormalProtectAsset.InstantiateAsync(transform, false);
            yield return m_Handle;
            if (m_Handle.Status == AsyncOperationStatus.Succeeded)
            {
                GameObject result = m_Handle.Result;
                result.transform.SetParent(transform);
            }
        }
        public void OnProtectGoneInvoke()
        {
            m_OnProtectGone?.Invoke();
            if (m_ProtectActivationAgent != null)
            {
                if (m_ProtectActivationAgent.TryGetComponent(out DestroySelf destroySelf))
                {
                    destroySelf.DestroyMe();
                }
            }
        }
        public void AddShield(int add)
        {
            foreach (var shield in m_Damageable)
            {
                shield.AddShield(add);
            }
        }
        public void AddBarrier(int add)
        {
            foreach (var shield in m_Damageable)
            {
                shield.AddBarrier(add);
            }
        }
    }
}
