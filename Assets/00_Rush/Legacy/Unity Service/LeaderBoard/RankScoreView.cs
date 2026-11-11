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

        private int m_Rank;

        protected override void ShowInternal()
        {
            gameObject.SetActive(true);
            base.ShowInternal();
        }

        protected override void HideInternal()
        {
            base.HideInternal();
            gameObject.SetActive(false);
        }

        public void SetRankScore(LeaderboardEntry entry, int rank)
        {
            if (entry == null)
            {
                m_RankText.text = "-";
                m_NameText.text = "You (unranked)";
                m_ScoreText.text = "-";
            }
            else
            {
                m_Rank = rank;
                m_RankText.text = $"#{m_Rank}";
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
