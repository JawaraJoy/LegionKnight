using UnityEngine;

namespace Rush
{
    public interface IProgressable
    {
        int CurrentLevel { get; }
        int MaxLevel { get; }
        void SetCurrentLevel(int level);
        void AddCurrentLevel(int amount);
    }
    public class ProgressField : IProgressable
    {
        [SerializeField]
        protected int m_CurrentLevel;
        [SerializeField]
        protected int m_MaxLevel;
        public int CurrentLevel => m_CurrentLevel;
        public int MaxLevel => m_MaxLevel;

        public void AddCurrentLevel(int amount)
        {
            m_CurrentLevel = Mathf.Clamp(m_CurrentLevel + amount, 0, m_MaxLevel);
        }
        public void SetCurrentLevel(int level)
        {
            m_CurrentLevel = Mathf.Clamp(level, 0, m_MaxLevel);
        }
    }
}
