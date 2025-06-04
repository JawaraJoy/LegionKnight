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

        private void OnEnable()
        {
            Player.Instance.AddOnStart(Init);
            Player.Instance.AddOnLevelUp(OnLevelUp);
        }
        private void OnDisable()
        {
            Player.Instance.RemoveOnStart(Init);
            Player.Instance.RemoveOnLevelUp(OnLevelUp); // Fix: Use a method with the correct signature
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
