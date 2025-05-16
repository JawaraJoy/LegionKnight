using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public partial class CharacterJump : MonoBehaviour
    {
        [SerializeField]
        private bool m_CanJump;
        [SerializeField]
        private bool m_UseHoldJump = false;
        [SerializeField]
        private bool m_CanFly = false;
        [SerializeField]
        private float m_FlyForce = 10f; // Adjust as needed

        private bool m_IsFlying = false;
        [SerializeField]
        private float m_FlyDuration = 2f; // Maximum duration the character can fly (in seconds)
        private float m_CurrentFlyTime = 0f;
        [SerializeField]
        private float m_PlatformSpeedUpOnFly = 1f; // Speed up factor when on a platform

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


        [SerializeField]
        private UnityEvent<float> m_OnJumpForceChange = new();
        [SerializeField]
        private UnityEvent<float> m_OnFallMultiplierChange = new();
        [SerializeField]
        private UnityEvent<float> m_OnMaxJumpDistanceChange = new();

        [SerializeField]
        private UnityEvent m_OnStartFly = new();
        [SerializeField]
        private UnityEvent m_OnStopFly = new();

        private float m_LinearVelocityY;

        private Vector2 m_GravityMod;
        private Vector2 m_StartingJumpPost;

        private bool m_IsJumping;
        private float m_JumpPressDuration;

        private float m_SaveFallSpeed;
        private void Start()
        {
            m_GravityMod = new Vector2(0f, -Physics.gravity.y);
        }

        private void Update()
        {
            CheckGrounded();
            Fly();

        }
        private void FixedUpdate()
        {
            
        }

        public void SetJumpForce(float set)
        {
            m_JumpForce = set;
            m_OnJumpForceChange?.Invoke(m_JumpForce);
        }
        public void StartFly()
        {
            StartCoroutine(StartFlying());
        }

        private IEnumerator StartFlying()
        {
            m_SaveFallSpeed = m_FallMultiplier;
            m_FallMultiplier = 0f; // Disable fall speed while flying
            m_CanJump = false; // Disable jumping while flying
            m_IsFlying = true;
            m_CanFly = true;
            m_CurrentFlyTime = 0f; // Reset fly timer
            m_Rb.gravityScale = 0f; // Disable gravity while flying
            m_Rb.bodyType = RigidbodyType2D.Kinematic; // Set to kinematic to prevent physics interactions
            yield return new WaitForSeconds(0.5f); // Wait for the fly duration
            GameManager.Instance.SetSpeedPlatformRate(m_PlatformSpeedUpOnFly);
            OnStartFlyInvoke();
        }

        private void Fly()
        {
            if (m_IsFlying && m_CanFly)
            {
                m_CurrentFlyTime += Time.deltaTime;
                if (m_CurrentFlyTime >= m_FlyDuration)
                {
                    StopFly();
                }
                else
                {
                    m_Rb.linearVelocity = new Vector2(m_Rb.linearVelocityX, m_FlyForce);
                    //m_Rb.AddRelativeForceY(m_FlyForce * Time.deltaTime, ForceMode2D.Force);
                }
            }
        }

        private void StopFly()
        {
            m_FallMultiplier = m_SaveFallSpeed; // Restore fall speed
            m_CanJump = true; // Re-enable jumping after flying
            m_IsFlying = false;
            m_CanFly = false;
            m_Rb.gravityScale = 1f; // Restore gravity
            m_Rb.linearVelocity = Vector2.zero; // Optionally stop movement
            m_CurrentFlyTime = 0f; // Reset fly timer
            m_Rb.bodyType = RigidbodyType2D.Dynamic; // Set back to dynamic for normal physics interactions
            GameManager.Instance.SetLevelOver(true);
            StartCoroutine(StopFlying()); // Start coroutine to stop flying
        }

        private IEnumerator StopFlying()
        {
            yield return new WaitForSeconds(0.25f); // Wait for the fly duration
            GameManager.Instance.SetSpeedPlatformRate(1f);
            GameManager.Instance.SetLevelOver(false);
            GameManager.Instance.SpawnPlatform();
            OnStopFlyInvoke();
        }

        private void OnStartFlyInvoke()
        {
            m_OnStartFly?.Invoke();
        }
        private void OnStopFlyInvoke()
        {
            m_OnStopFly?.Invoke();
        }

        public void SetFallSpeed(float set)
        {
            m_FallMultiplier = set;
            m_OnFallMultiplierChange?.Invoke(m_FallMultiplier);
        }
        public void SetMaxJumpDistance(float set)
        {
            m_MaxJumpDistance = set;
            m_OnMaxJumpDistanceChange?.Invoke(m_MaxJumpDistance);
        }
        public void SetCanJump(bool set)
        {
            m_CanJump = set;
        }
        public void SetUseHoldJump(bool set)
        {
            m_UseHoldJump = set;
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

            if (m_UseHoldJump && m_Rb.linearVelocityY > 0)
            {
                m_Rb.linearVelocity = new Vector2(m_Rb.linearVelocityX, m_Rb.linearVelocityY * 0.6f);
            }
        }

        private void OnLinearVelocityChangedInvoke(float val)
        {
            m_OnLinearVelocityYChanged?.Invoke(val);

            if (m_UseHoldJump && m_Rb.linearVelocityY > 0f && m_IsJumping)
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
