using LegionKnight;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Rush
{
    [System.Serializable]
    public partial struct ForceEffect
    {
        public string ForceName;
        public Vector2 ForceDirection;
    }
    public class RigidBody2DForce : MonoBehaviour
    {
        [SerializeField] private ForceEffect[] m_ForceEffects;
        [SerializeField] private Rigidbody2D m_Rb;
        [SerializeField] private float m_StopForceDelay;
        [SerializeField] private UnityEvent m_OnForced;

        private Coroutine m_StopCoroutine;
        private RigidbodyConstraints2D m_OriginalConstraints;

        private void Awake()
        {
            m_OriginalConstraints = m_Rb.constraints;
        }

        private void OnForcedInvoke()
        {
            m_Rb.constraints &= ~RigidbodyConstraints2D.FreezePositionX;
            m_Rb.freezeRotation = true;

            StopForceInternal();
            m_OnForced?.Invoke();
        }

        private ForceEffect GetForceEffect(string forceName)
        {
            foreach (var f in m_ForceEffects)
            {
                if (f.ForceName == forceName)
                    return f;
            }

            Debug.LogWarning($"ForceEffect '{forceName}' not found");
            return default;
        }

        public void ApplyForce(string forceName)
        {
            Vector2 force = GetForceEffect(forceName).ForceDirection;

            m_Rb.linearVelocity = Vector2.zero; // or velocity for older Unity
            m_Rb.AddForce(force, ForceMode2D.Impulse);

            OnForcedInvoke();
        }

        private void StopForceInternal()
        {
            if (m_StopCoroutine != null)
                StopCoroutine(m_StopCoroutine);

            m_StopCoroutine = StartCoroutine(StopForcing());
        }

        private IEnumerator StopForcing()
        {
            yield return new WaitForSeconds(m_StopForceDelay);

            m_Rb.linearVelocity = Vector2.zero; // or velocity
            m_Rb.constraints = m_OriginalConstraints;
        }
    }
}
