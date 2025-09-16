using UnityEngine;

namespace LegionKnight
{
    public class Patrol : MonoBehaviour
    {
        public float patrolSpeed = 2f;
        public float leftPatrolPointX = -5f;
        public float rightPatrolPointX = 5f;

        private Rigidbody2D rb;
        private int moveDirection = 1; // 1 for right, -1 for left

        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            if (rb == null)
            {
                Debug.LogError("Rigidbody2D not found on this GameObject!");
                enabled = false; // Disable script if Rigidbody2D is missing
            }
        }

        void FixedUpdate()
        {
            // Move the Rigidbody2D
            rb.linearVelocity = new Vector2(patrolSpeed * moveDirection, rb.linearVelocityY);

            // Check for boundaries and reverse direction
            if (transform.position.x >= rightPatrolPointX && moveDirection == 1)
            {
                moveDirection = -1; // Change to move left
                                    // Optional: Flip sprite here if needed
            }
            else if (transform.position.x <= leftPatrolPointX && moveDirection == -1)
            {
                moveDirection = 1; // Change to move right
                                   // Optional: Flip sprite here if needed
            }
        }

        // Optional: Visualize patrol points in the editor
        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(new Vector3(leftPatrolPointX, transform.position.y, transform.position.z), 0.2f);
            Gizmos.DrawWireSphere(new Vector3(rightPatrolPointX, transform.position.y, transform.position.z), 0.2f);
            Gizmos.DrawLine(new Vector3(leftPatrolPointX, transform.position.y, transform.position.z), new Vector3(rightPatrolPointX, transform.position.y, transform.position.z));
        }
    }
}
