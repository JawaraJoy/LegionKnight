using LegionKnight;
using UnityEngine;

namespace Rush
{
    public class RushPlayerJumpAgent : MonoBehaviour
    {
        private PlayerJump m_Jump;
        private PlayerJump JumpInternal
        {
            get
            {
                if (m_Jump == null)
                {
                    m_Jump = RushPlayer.Instance.Jump;
                }
                return m_Jump;
            }
        }
        public void JumpPress()
        {
            if (JumpInternal == null) return;
            JumpInternal.JumpPress();
        }
        public void JumpUnPress()
        {
            if (JumpInternal == null) return;
            JumpInternal.JumpUnPress();
        }

        public void SetGameObjectActive(bool active)
        {
            if (JumpInternal != null)
            {
                JumpInternal.gameObject.SetActive(active);
            }
            else
            {
                Debug.LogError($"Player doesnt has [{m_Jump.GetType()}] in Binds");
            }

        }
    }
}
