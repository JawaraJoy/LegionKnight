using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public partial class CharacterJump : MonoBehaviour
    {
        [SerializeField]
        private bool m_CanJump;
        [SerializeField]
        private float m_JumpForce = 10f;           // Force applied for the jump
        [SerializeField]
        private Transform m_GroundCheck;          // Empty GameObject to check if the player is on the ground
        [SerializeField]
        private float m_GroundCheckRadius = 0.2f; // Radius to detect ground
        [SerializeField]
        private LayerMask m_GroundLayer;          // Layer representing the ground
        [SerializeField]
        private UnityEvent<float> m_OnLinearVelocityYChanged = new();
        [SerializeField]
        private UnityEvent m_OnStartJump = new();

        [SerializeField]
        private Rigidbody2D m_Rb;
        private bool m_IsGrounded;

        private float m_LinearVelocityY;

        private void Update()
        {
            CheckGrounded();
        }

        void CheckGrounded()
        {
            m_IsGrounded = Physics2D.OverlapCircle(m_GroundCheck.position, m_GroundCheckRadius, m_GroundLayer);
            m_LinearVelocityY = m_Rb.linearVelocityY;
            OnLinearVelocityChangedInvoke(m_LinearVelocityY);
        }

        public void Jump()
        {
            Debug.Log("Jump");
            if (!m_IsGrounded || !m_CanJump) return;
            m_Rb.linearVelocity = new Vector2(m_Rb.linearVelocity.x, m_JumpForce);
            //m_Rb.AddForceY(m_JumpForce, ForceMode2D.Force);
            OnStartJumpInvoke();
        }

        private void OnLinearVelocityChangedInvoke(float val)
        {
            m_OnLinearVelocityYChanged?.Invoke(val);
        }
        private void OnStartJumpInvoke()
        {
            m_OnStartJump?.Invoke();
        }

        void OnDrawGizmosSelected()
        {
            if (m_GroundCheck != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(m_GroundCheck.position, m_GroundCheckRadius);
            }
        }
    }
}
