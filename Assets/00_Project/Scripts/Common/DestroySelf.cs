using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AddressableAssets;

namespace LegionKnight
{
    public partial class DestroySelf : MonoBehaviour
    {
        [SerializeField]
        private bool m_ActiveSelfDestructOnStart = false;
        [SerializeField]
        private float m_SelfDestructDelay;
        [SerializeField]
        private float m_Delay = 1f;
        [SerializeField]
        private UnityEvent m_OnStartDestroy = new();
        [SerializeField]
        private UnityEvent m_OnDestroyDone = new();
        private IEnumerator Start()
        {
            if (!m_ActiveSelfDestructOnStart) yield break;
            yield return new WaitForSeconds(m_SelfDestructDelay);
            //Destroy(gameObject);
            Addressables.ReleaseInstance(gameObject);
        }
        public void DestroyMe()
        {
            StartCoroutine(DestroyingMe());
        }
        private IEnumerator DestroyingMe()
        {
            m_OnStartDestroy?.Invoke();
            yield return new WaitForSeconds(m_Delay);
            m_OnDestroyDone?.Invoke();
            Addressables.ReleaseInstance(gameObject);
            //Destroy(gameObject);
        }
        public void SetActiveSelfDestructOnStart(bool set)
        {
            m_ActiveSelfDestructOnStart = set;
        }
    }
}
