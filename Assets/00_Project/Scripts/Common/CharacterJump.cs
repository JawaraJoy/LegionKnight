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
        [SerializeField, Range(0.001f, 1f)]
        private float m_JumpPressMaxDuration = 1f;
        [SerializeField]
        private float m_JumpMultiplier = 1f;
        [SerializeField]
        private float m_FallMultiplier = 1f;
        [SerializeField]
        private float m_MaxJumpDistance = 5f;

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

        private Vector2 m_GravityMod;
        private Vector2 m_StartingJumpPost;

        private bool m_IsJumping;
        private float m_JumpPressDuration;
        private void Start()
        {
            m_GravityMod = new Vector2(0f, -Physics.gravity.y);
        }

        private void Update()
        {
            CheckGrounded();
        }

        public void SetCanJump(bool set)
        {
            m_CanJump = set;
        }

        void CheckGrounded()
        {
            m_IsGrounded = Physics2D.OverlapCircle(m_GroundCheck.position, m_GroundCheckRadius, m_GroundLayer);
            m_LinearVelocityY = m_Rb.linearVelocityY;
            OnLinearVelocityChangedInvoke(m_LinearVelocityY);
            ClampJumpDistance();
        }

        private void ClampJumpDistance()
        {
            //Vector2 currentPost = new Vector2(m_Rb.position.x, m_Rb.position.y);
            float jumpDistance = Vector2.Distance(m_StartingJumpPost, m_Rb.position);
            if (jumpDistance > m_MaxJumpDistance)
            {
                m_Rb.linearVelocity = Vector2.zero;
                m_StartingJumpPost = m_Rb.position;
            }
        }

        public void JumpPress()
        {
            Debug.Log("Jump");
            if (!m_IsGrounded || !m_CanJump) return;
            m_Rb.linearVelocity = new Vector2(m_Rb.linearVelocity.x, m_JumpForce);
            //m_Rb.AddForceY(m_JumpForce, ForceMode2D.Force);
            OnStartJumpInvoke();
        }
        public void JumpUnPress()
        {
            m_IsJumping = false;

            if (m_Rb.linearVelocityY > 0)
            {
                m_Rb.linearVelocity = new Vector2(m_Rb.linearVelocityX, m_Rb.linearVelocityY * 0.6f);
            }
        }

        private void OnLinearVelocityChangedInvoke(float val)
        {
            m_OnLinearVelocityYChanged?.Invoke(val);

            if (m_Rb.linearVelocityY > 0f && m_IsJumping)
            {
                m_JumpPressDuration += Time.deltaTime;
                if (m_JumpPressDuration > m_JumpPressMaxDuration)
                {
                    m_IsJumping = false;
                }

                float t = m_JumpPressDuration / m_JumpPressMaxDuration;
                float currentJumpM = -m_JumpMultiplier;
                if (t > 0.5f)
                {
                    currentJumpM = -m_JumpMultiplier * (1 - t);
                }
                m_Rb.linearVelocity += m_GravityMod * currentJumpM * Time.deltaTime;
            }
            if (m_Rb.linearVelocityY < 0f)
            {
                m_Rb.linearVelocity -= m_GravityMod * m_FallMultiplier * Time.deltaTime;
            }
        }
        private void OnStartJumpInvoke()
        {
            m_OnStartJump?.Invoke();
            m_IsJumping = true;
            m_JumpPressDuration = 0f;
            m_StartingJumpPost = m_Rb.position;
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
