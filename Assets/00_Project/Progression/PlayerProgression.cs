using UnityEngine;
using UnityEngine.Events;

namespace LegionKnight
{
    public partial class PlayerProgression : Progression
    {
        
    }

    public partial class Player
    {
        [SerializeField]
        private PlayerProgression m_PlayerProgression;

        public void InitPlayerProgression()
        {
            m_PlayerProgression.Init();
        }
        public void AddOnCurrentExpRateChange(UnityAction<float> action)
        {
            m_PlayerProgression.AddOnCurrentExpRateChange(action);
        }
        public void RemoveOnCurrentExpRateChange(UnityAction<float> action)
        {
            m_PlayerProgression.RemoveOnCurrentExpRateChange(action);
        }
        public void AddPlayerExperience(int exp)
        {
            m_PlayerProgression.AddExperience(exp);
        }
        public void AddExperienceSlowly(int exp, float growSpeed = 50f)
        {
            m_PlayerProgression.AddExperienceSlowly(exp, growSpeed);
        }
        public float GetPlayerLevelProgressionRate()
        {
            return m_PlayerProgression.GetLevelProgressionRate();
        }
        public void AddOnLevelUp(UnityAction<int> action)
        {
            m_PlayerProgression.AddOnLevelUp(action);
        }
        public void RemoveOnLevelUp(UnityAction<int> action)
        {
            m_PlayerProgression.RemoveOnLevelUp(action);
        }
        public int GetPlayerLevel()
        {
            return m_PlayerProgression.GetCurrentLevel();
        }
        public void ShowLevelUpPanel()
        {
            m_PlayerProgression.ShowLevelUpPanel();
        }
    }
    public partial class PlayerAgent
    {
        public void ShowLevelUpPanel()
        {
            Player.Instance.ShowLevelUpPanel();
        }
    }
}
