using LegionKnight;
using UnityEngine;

namespace Rush
{
    public class PlayerTouchDownAgent : MonoBehaviour
    {
        private TouchDown m_TouchDown;
        private PlayerJump m_PlayerJump;
        private PlayerJump PlayerJumpInternal
        {
            get
            {
                if (m_PlayerJump == null)
                {
                    m_PlayerJump = RushPlayer.Instance.Jump;
                }
                return m_PlayerJump;
            }
        }

        private TouchDown TouchDownInternal
        {
            get
            {
                if (m_TouchDown == null)
                {
                    if (PlayerJumpInternal.HasBind(out TouchDown touchdown))
                    {
                        m_TouchDown = touchdown;
                    }
                }
                return m_TouchDown;
            }
        }

        public void SetGameObjectActive(bool active)
        {
            if (TouchDownInternal != null)
            {
                TouchDownInternal.gameObject.SetActive(active);
            }
            else
            {
                Debug.LogError($"Player doesnt has [{m_TouchDown.GetType()}] in Binds");
            }
            
        }
    }
}
