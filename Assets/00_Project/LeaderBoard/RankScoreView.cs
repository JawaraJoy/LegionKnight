using TMPro;
using Unity.Services.Leaderboards.Models;
using UnityEngine;

namespace LegionKnight
{
    public partial class RankScoreView : UIView
    {
        [SerializeField]
        private TextMeshProUGUI m_RankText;
        [SerializeField]
        private TextMeshProUGUI m_NameText;
        [SerializeField]
        private TextMeshProUGUI m_ScoreText;

        public void SetRankScore(LeaderboardEntry entry, int rank)
        {
            if (entry == null)
            {
                m_RankText.text = "N/A";
                m_NameText.text = "N/A";
                m_ScoreText.text = "N/A";
            }
            else
            {
                m_RankText.text = $"#{rank}";
                m_NameText.text = entry.PlayerName;
                m_ScoreText.text = entry.Score.ToString();
            }

        }
        public void Clear()
        {
            m_RankText.text = string.Empty;
            m_NameText.text = string.Empty;
            m_ScoreText.text = string.Empty;
        }
    }
}
