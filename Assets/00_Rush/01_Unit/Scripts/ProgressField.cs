using UnityEngine;
using UnityEngine.Events;

namespace Rush
{
    [System.Serializable]
    public partial class ProgressField : IProgressable, IReseter
    {
        [SerializeField]
        private int m_Level = 1;
        [SerializeField]
        protected int m_MaxLevel = 10;
        protected int LevelInternal => Mathf.Clamp(m_Level, 1, m_MaxLevel);
        public int Level => LevelInternal;
        public int MaxLevel => m_MaxLevel;
        [SerializeField]
        private UnityEvent<int> m_OnLevelChanged;
        public int GetLevelScaleByOther(ProgressField other)
        {
            return Mathf.Max(1, LevelInternal + other.LevelInternal);
        }
        public void AddLevel(int amount)
        {
            m_Level = Mathf.Clamp(m_Level + amount, 1, m_MaxLevel);
            m_OnLevelChanged?.Invoke(LevelInternal);
        }
        private void SetLevelInternal(int level)
        {
            m_Level = Mathf.Clamp(level, 1, m_MaxLevel);
            m_OnLevelChanged?.Invoke(LevelInternal);
        }   
        public void SetLevel(int level)
        {
           
            SetLevelInternal(level);
        }
        public void SetMaxLevel(int maxLevel)
        {
            m_MaxLevel = maxLevel;
            m_Level = Mathf.Clamp(m_Level, 1, m_MaxLevel);
            m_OnLevelChanged?.Invoke(LevelInternal);
        }

        public void ResetProgression()
        {
            SetLevelInternal(1);
        }

        public ProgressField(int level, int maxLevel)
        {
            m_Level = level;
            m_MaxLevel = maxLevel;
        }
    }
}
