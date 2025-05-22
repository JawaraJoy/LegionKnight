using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public partial class ProgressReach : MonoBehaviour
    {
        [SerializeField]
        private int m_LevelTarget = 1;

        [SerializeField]
        private UnityEvent<bool> m_OnLevelReached = new ();
        [SerializeField]
        private UnityEvent m_OnLevelReachedTrue = new();
        [SerializeField]
        private UnityEvent m_OnLevelReachedFalse = new();

        private bool IsLevelReached()
        {
            return Player.Instance.GetPlayerLevel() >= m_LevelTarget;
        }
        public void Init()
        {
            m_OnLevelReached?.Invoke(IsLevelReached());

            if (IsLevelReached())
            {
                m_OnLevelReachedTrue?.Invoke();
            }
            else
            {
                m_OnLevelReachedFalse?.Invoke();
            }
        }
    }
}
