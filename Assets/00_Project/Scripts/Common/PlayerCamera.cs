using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public partial class PlayerCamera : Singleton<PlayerCamera>
    {
        private bool m_StayFollow;
        [SerializeField]
        private UnityEvent<bool> m_OnSetStayFollow = new();
        
        public void SetStayFollow(bool set)
        {
            m_StayFollow = set;
            OnSetStayFollowInvoke();
        }
        private void OnSetStayFollowInvoke()
        {
            m_OnSetStayFollow?.Invoke(m_StayFollow);
        }
    }
}
