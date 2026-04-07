using LegionKnight;
using MoreMountains.Tools;
using UnityEngine;

namespace Rush
{
    public class PlayerHighestScore : MonoBehaviour
    {
        [SerializeField, MMReadOnly]
        private int m_HighestScore = 0;
        private  const string HighestScoreKey = "HighestScore";
        private void InitInternal()
        {
            bool hasKey = UnityService.Instance.HasData(HighestScoreKey);
            if (hasKey)
            {
                m_HighestScore = UnityService.Instance.GetData<int>(HighestScoreKey);
            }
        }
        private void SetScoreInternal(int score)
        {
            if (score < m_HighestScore)
            {
                m_HighestScore = score;
            }
        }
    }
    public partial class RushPlayer
    {
        [SerializeField]
        private PlayerHighestScore m_PlayerScore;
        public PlayerHighestScore PlayerScore => m_PlayerScore;
    }
}
