using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public partial class ProgressReach : MonoBehaviour
    {
        [SerializeField]
        private int m_LevelTarget = 1;

        [SerializeField]
        private UnityEvent<bool> m_OnLevelReached = new();
        [SerializeField]
        private UnityEvent m_OnLevelReachedTrue = new();
        [SerializeField]
        private UnityEvent m_OnLevelReachedFalse = new();

        private void Awake()
        {
            Player.Instance.AddOnStart(Init);
            Player.Instance.AddOnLevelUp(OnLevelUp); // Fix: Use a method with the correct signature
        }

        private void OnDestroy()
        {
            Player.Instance.RemoveOnStart(Init);
        }

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

        private void OnLevelUp(int level) // Fix: Add a method matching UnityAction<int> signature
        {
            Init();
        }
    }
}
