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
        [SerializeField]
        private ForceEffect[] m_ForceEffects;

        [SerializeField]
        private Rigidbody2D m_Rb;
        [SerializeField]
        private float m_StopForceDelay;
        [SerializeField]
        private UnityEvent m_OnForced;
        private void OnForcedInvoke()
        {
            m_Rb.constraints &= ~RigidbodyConstraints2D.FreezePositionX; // Unfreeze X position to allow movement
            m_Rb.freezeRotation = true; // Prevent rotation during death effect
            //Player.Instance.Death();
            StopForceInternal();
            OnForcedInvoke();
        }
        public void OnDeathInvoke()
        {
            OnForcedInvoke();
        }
        private ForceEffect GetForceEffect(string forceName)
        {
            ForceEffect match = new ();
            foreach (var f in m_ForceEffects)
            {
                if (f.ForceName == forceName)
                {
                    match = f;
                }
            }
            return match;
        }
        public void ApplyForce(string forceName)
        {
            Vector2 force = GetForceEffect(forceName).ForceDirection;
            m_Rb.AddForce(force, ForceMode2D.Impulse);
        }
        private void StopForceInternal()
        {
            StartCoroutine(StopForcing());
        }
        private IEnumerator StopForcing()
        {
            yield return new WaitForSeconds(m_StopForceDelay);
            GameManager.Instance.ApplyPotOfLife();
            m_Rb.AddForce(Vector2.zero, ForceMode2D.Impulse);
        }
    }
}
