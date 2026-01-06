using UnityEngine;

namespace Rush
{
    public partial interface IProgressable
    {
        int Level { get; }
        int MaxLevel { get; }
        void SetLevel(int level);
        void AddLevel(int amount);
    }
    [System.Serializable]
    public partial class ProgressField : IProgressable
    {
        [SerializeField]
        private int m_Level;
        [SerializeField]
        protected int m_MaxLevel;
        protected int LevelInternal => Mathf.Clamp(m_Level, 1, m_MaxLevel);
        public int Level => LevelInternal;
        public int MaxLevel => m_MaxLevel;
        public int GetLevelScaleByOther(ProgressField other)
        {
            return Mathf.Max(1, LevelInternal + other.LevelInternal);
        }
        public void AddLevel(int amount)
        {
            m_Level = Mathf.Clamp(m_Level + amount, 0, m_MaxLevel);
        }
        public void SetLevel(int level)
        {
            m_Level = Mathf.Clamp(level, 0, m_MaxLevel);
        }
    }
}
