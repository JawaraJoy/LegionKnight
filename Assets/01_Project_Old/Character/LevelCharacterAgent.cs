using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public class LevelCharacterAgent : MonoBehaviour
    {
        private int m_CurrentLevel = 0;
        [SerializeField]
        private UnityEvent<int> m_OnLevelChanged = new();
        public void SetCurrentLevel(int level)
        {
            if (level < 0)
            {
                Debug.LogError("Level cannot be negative.");
                return;
            }
            m_CurrentLevel = level;
            m_OnLevelChanged.Invoke(m_CurrentLevel);
            Debug.Log($"Current level set to: {m_CurrentLevel}");
        }
    }
}
