using UnityEngine;

namespace LegionKnight
{

    [CreateAssetMenu(fileName = "New AchieveDefinition", menuName = "Legion Knight/Achievement", order = 1)]
    public class AchieveDefinition : ScriptableObject
    {
        [SerializeField]
        private string m_Id;
        [SerializeField]
        private string m_Label;
        [SerializeField]
        private string m_UnclockedMessage;

        [SerializeField]
        private int m_AchieveScore;

        public string Id => m_Id;
        public string Label => m_Label;
        public string UnclockedMessage => m_UnclockedMessage;
        public int AchieveScore => m_AchieveScore;

        public void SetScore(int score)
        {
            Player.Instance.SetAchievementScore(m_Id, score);
        }
        public void AddScore(int score)
        {
            Player.Instance.AddAchievementScore(m_Id, score);
        }
    }
}
