
using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.Events;

namespace Rush
{
    public class RogueLikeHandler : MonoBehaviour, IReseter
    {
        [SerializeField]
        private RogueLikeConfig m_Config;
        [SerializeField, MMReadOnly]
        private int m_ForPlayerCurrentLevel = 1;
        [SerializeField, MMReadOnly]
        private int m_ForPlayerCurrentExperience;

        [SerializeField]
        private UnityEvent<int, int> m_OnForPlayerExperienceAdded;
        [SerializeField]
        private UnityEvent<int> m_OnForPlayerLevelUp;
        [SerializeField, MMReadOnly]
        private int m_ForBossCurrentLevel = 1;

        [SerializeField, MMReadOnly]
        private int m_ForBossCurrentExperience;

        [SerializeField]
        private UnityEvent<int, int> m_OnForBossExperienceAdded;

        [SerializeField]
        private UnityEvent<int> m_OnForBossLevelUp;
        [SerializeField]
        private UnityEvent<CardConfig> m_OnCardCollected;
        public RogueLikeConfig Config => m_Config;
        public int ForPlayerCurrentExperience => m_ForPlayerCurrentExperience;
        public int ForPlayerCurrentLevel => m_ForPlayerCurrentLevel;
        public UnityEvent<int, int> OnForPlayerExperienceAdded => m_OnForPlayerExperienceAdded;
        public UnityEvent<int> OnForPlayerLevelUp => m_OnForPlayerLevelUp;
        public int ForBossCurrentLevel => m_ForBossCurrentLevel;
        public int ForBossCurrentExperience => m_ForBossCurrentExperience;

        public UnityEvent<int, int> OnForBossExperienceAdded => m_OnForBossExperienceAdded;
        public UnityEvent<int> OnForBossLevelUp => m_OnForBossLevelUp;
        public UnityEvent<CardConfig> OnCardCollected => m_OnCardCollected;
        public void ResetProgression()
        {
            SetForPlayerLevel(1);
            SetForPlayerExperience(0);
            SetForBossLevel(1);
            OnForPlayerExperienceAddedInvoke(m_ForPlayerCurrentExperience);
        }
        public void AddForPlayerExperience(int amount)
        {
            m_ForPlayerCurrentExperience += amount;
            CheckForPlayerLevelUp();
        }
        private void SetForPlayerExperience(int amount)
        {
            m_ForPlayerCurrentExperience = amount;
            CheckForPlayerLevelUp();
        }
        private void CheckForPlayerLevelUp()
        {
            int nextLevelExp = m_Config.ForPlayerLevelFormula.GetCurrentMaxExperience(m_ForPlayerCurrentLevel + 1);
            if (m_ForPlayerCurrentExperience >= nextLevelExp)
            {
                int excessExp = m_ForPlayerCurrentExperience - nextLevelExp;
                OnLevelUpInvoke();
                m_ForPlayerCurrentExperience = excessExp; // Carry over excess experience to the next level
            }
            OnForPlayerExperienceAddedInvoke(m_ForPlayerCurrentExperience);
        }
        private void OnLevelUpInvoke()
        {
            AddForPlayerLevel(1);
            m_OnForPlayerLevelUp.Invoke(m_ForPlayerCurrentLevel);
            // Implement level-up logic here (e.g., increase stats, unlock skills, etc.)
        }
        private void OnForPlayerExperienceAddedInvoke(int amount)
        {
            m_OnForPlayerExperienceAdded.Invoke(amount, m_Config.ForPlayerLevelFormula.GetCurrentMaxExperience(m_ForPlayerCurrentLevel+1));
        }

        private void SetForPlayerLevel(int level)
        {
            m_ForPlayerCurrentLevel = level;
        }
        private void AddForPlayerLevel(int amount)
        {
            m_ForPlayerCurrentLevel += amount;
        }

        private void OnBossLevelUpInvoke()
        {
            AddForBossLevel(1);
            m_OnForBossLevelUp.Invoke(m_ForBossCurrentLevel);
        }

        private void SetForBossLevel(int level)
        {
            m_ForBossCurrentLevel = level;
        }

        private void AddForBossLevel(int amount)
        {
            m_ForBossCurrentLevel += amount;
        }
    }
}
