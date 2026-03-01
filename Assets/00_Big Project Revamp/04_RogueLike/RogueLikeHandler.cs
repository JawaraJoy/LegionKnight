using LegionKnight;
using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.Events;

namespace Rush
{
    public class RogueLikeHandler : MonoBehaviour
    {
        [SerializeField]
        private RogueLikeConfig m_Config;
        [SerializeField, MMReadOnly]
        private int m_CurrentLevel = 1;
        [SerializeField, MMReadOnly]
        private int m_CurrentExperience;

        [SerializeField]
        private UnityEvent<int, int> m_OnExperienceAdded;
        [SerializeField]
        private UnityEvent<int> m_OnLevelUp; // Event that triggers when the player levels up, passing the new level as an argument
        [SerializeField]
        private UnityEvent<CardConfig> m_OnCardSelected;
        public RogueLikeConfig Config => m_Config;
        public int CurrentExperience => m_CurrentExperience;
        public int CurrentLevel => m_CurrentLevel;
        public UnityEvent<int, int> OnExperienceAdded => m_OnExperienceAdded;
        public UnityEvent<int> OnLevelUp => m_OnLevelUp;
        public UnityEvent<CardConfig> OnCardSelected => m_OnCardSelected;

        private RogueLikeCardPanel m_CardPanel;
        private RogueLikeCardPanel CardPanel
        {
            get
            {
                if (m_CardPanel == null)
                {
                    m_CardPanel = CanvasManager.Instance.GetPanel<RogueLikeCardPanel>();
                }
                return m_CardPanel;
            }
        }
        public void ResetProgress()
        {
            SetLevel(1);
            SetExperience(0);
            OnExperienceAddedInvoke(m_CurrentExperience);
        }
        public void AddExperience(int amount)
        {
            m_CurrentExperience += amount;
            CheckLevelUp();
        }
        private void SetExperience(int amount)
        {
            m_CurrentExperience = amount;
            CheckLevelUp();
        }
        private void CheckLevelUp()
        {
            int nextLevelExp = m_Config.LevelFormula.GetCurrentMaxExperience(m_CurrentLevel + 1);
            if (m_CurrentExperience >= nextLevelExp)
            {
                int excessExp = m_CurrentExperience - nextLevelExp;
                OnLevelUpInvoke();
                m_CurrentExperience = excessExp; // Carry over excess experience to the next level
            }
            OnExperienceAddedInvoke(m_CurrentExperience);
        }
        private void OnLevelUpInvoke()
        {
            AddLevel(1);
            m_OnLevelUp.Invoke(m_CurrentLevel);
            // Implement level-up logic here (e.g., increase stats, unlock skills, etc.)
            CardPanel.ShowCards(m_Config.GetDifferenceCardRandom());

        }
        private void OnExperienceAddedInvoke(int amount)
        {
            m_OnExperienceAdded.Invoke(amount, m_Config.LevelFormula.GetCurrentMaxExperience(m_CurrentLevel));
        }

        private void SetLevel(int level)
        {
            m_CurrentLevel = level;
        }
        private void AddLevel(int amount)
        {
            m_CurrentLevel += amount;
        }
    }
}
