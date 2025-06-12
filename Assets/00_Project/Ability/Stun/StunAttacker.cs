using UnityEngine;

namespace LegionKnight
{
    public class StunAttacker : MonoBehaviour
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
    }
}
