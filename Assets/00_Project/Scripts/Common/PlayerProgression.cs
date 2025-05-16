using UnityEngine;

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
        public void AddPlayerExperience(int exp)
        {
            m_PlayerProgression.AddExperience(exp);
        }
        public float GetPlayerLevelProgressionRate()
        {
            return m_PlayerProgression.GetLevelProgressionRate();
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
