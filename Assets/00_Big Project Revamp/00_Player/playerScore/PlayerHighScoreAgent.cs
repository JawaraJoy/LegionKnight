using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

namespace Rush
{
    public class PlayerHighScoreAgent : MonoBehaviour
    {
        // used for multiply score by combo perfect
        [SerializeField]
        private int m_ExpMultiplyByPerfect = 1;
        public void ApplyHighScoreToPlayer()
        {
            RushPlayer.Instance.PlayerScore.ApplyHighScoreToPlayer();
        }
        public void AddScore(int score)
        {
            RushPlayer.Instance.PlayerScore.AddScore(score);
        }
        public void AddScoreByMultiply(int perfectCombo)
        {
            int totalAmount = (perfectCombo + 1) * m_ExpMultiplyByPerfect;
            RushPlayer.Instance.PlayerScore.AddScore(totalAmount);
        }
    }
}
