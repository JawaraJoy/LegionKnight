using UnityEngine;

namespace LegionKnight
{
    public class StunAttacker : MonoBehaviour, IAbility
    {
        [SerializeField]
        private float m_StunDuration = 2.0f; // Duration of the stun effect
        private void OnTriggerEnter2D(Collider2D collision)
        {
            // Check if the collided object has a StunTarget component
            StunTarget stunTarget = collision.GetComponent<StunTarget>();
            if (stunTarget != null)
            {
                // Call the Stun method on the StunTarget component
                stunTarget.Stun(m_StunDuration);
                Debug.Log("Attacker stunned the target!");
            }
            else
            {
                Debug.Log("No StunTarget component found on the collided object.");
            }
        }
        public void Initialize(AbilityDefinition defi, int level)
        {
            // Initialize the ability with the provided definition
            // This can include setting up specific properties or configurations based on the definition
            if (defi != null)
            {
                m_StunDuration = defi.GetFinalStunDuration(level); // Assuming AbilityDefinition has a StunDuration property
            }
        }
    }
}
