using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public class StunTarget : MonoBehaviour
    {
        private float m_StunDuration = 2.0f; // Duration of the stun effect

        private bool m_IsStunned = false;
        [SerializeField]
        private ParticleSystem m_StunEffect; // Optional: Particle effect to show when stunned

        [SerializeField]
        private UnityEvent m_OnStunStart = new ();
        [SerializeField]
        private UnityEvent m_OnStunEnd = new ();
        public void Stun(float duration)
        {
            if (!m_IsStunned)
            {
                m_IsStunned = true;
                m_StunDuration = duration; // Update the stun duration if needed
                // Add logic to visually indicate the stun effect, e.g., change color or play animation
                // Spawn Particle effects or play sound effects here

                m_StunEffect.Play(); // Play the stun effect if assigned
                Debug.Log("Target is stunned!");
                // Start a coroutine to handle the stun duration
                StartCoroutine(StunCoroutine());
            }
        }
        private IEnumerator StunCoroutine()
        {
            m_OnStunStart?.Invoke();
            yield return new WaitForSeconds(m_StunDuration);
            m_IsStunned = false;
            m_OnStunEnd?.Invoke();
            m_StunEffect.Stop(); // Stop the stun effect if assigned
            // Add logic to revert the visual indication of the stun effect
            Debug.Log("Target is no longer stunned!");
        }
    }
}
